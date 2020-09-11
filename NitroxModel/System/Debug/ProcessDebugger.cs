using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NitroxModel.Helper;
using NitroxModel.Logger;
using NitroxModel.System.Windows;
using NitroxModel.System.Windows.Debug;

namespace NitroxModel.System.Debug
{
    /// <summary>
    ///     API for debugging other processes.
    /// </summary>
    /// <remarks>
    ///     <a href="https://docs.microsoft.com/en-us/windows/win32/debug/writing-the-debugger-s-main-loop">Writing the Debugger's Main Loop</a>
    ///     <a href="https://www.codeproject.com/Articles/132742/Writing-Windows-Debugger-Part-2">
    ///         Writing Windows Debugger - Part 2
    ///     </a>
    /// </remarks>
    public sealed class ProcessDebugger : IDisposable
    {
        private readonly Dictionary<IntPtr, Breakpoint> activeBreakpoints = new Dictionary<IntPtr, Breakpoint>();
        private readonly CancellationTokenSource cancellation;
        private readonly bool disposeProcess;
        private Thread debuggerThread;
        private bool disposed;
        private ProcessEx process;

        public ProcessEx Process
        {
            get => process;
            private set => Interlocked.Exchange(ref process, value);
        }

        private ProcessDebugger(Func<ProcessEx> processCreater, bool disposeProcess)
        {
            this.disposeProcess = disposeProcess;
            cancellation = new CancellationTokenSource();
            debuggerThread = new Thread(() =>
            {
                // Debugger loop. Running in separate thread because Windows' debugger API uses the current thread.
                try
                {
                    Process = processCreater();
                    if (Process == null)
                    {
                        throw new Exception("Unable to start debugging because the process was not started or has crashed.");
                    }

                    Win32Native.DebugSetProcessKillOnExit(false);

                    bool firstThread = true;
                    List<Breakpoint> breakpointsToApply = new List<Breakpoint>();
                    while (true)
                    {
                        if (cancellation.IsCancellationRequested)
                        {
                            break;
                        }
                        
                        foreach (Breakpoint breakpoint in breakpointsToApply)
                        {
                            breakpoint.Apply();
                        }
                        breakpointsToApply.Clear();
                        
                        DebugEvent64 evt = default;
                        ContinueStatus continueStatus = ContinueStatus.DBG_CONTINUE;
                        if (!Win32Native.WaitForDebugEvent(ref evt, 0))
                        {
                            Thread.Sleep(10);
                            continue;
                        }

                        switch (evt.dwDebugEventCode)
                        {
                            case DebugEventType.CREATE_THREAD_DEBUG_EVENT:
                                if (firstThread)
                                {
                                    firstThread = false;
                                    DebugEventOptions options = new DebugEventOptions();
                                    OnEntrypointBreak(options);
                                    if (options.StopDebuggingRequested)
                                    {
                                        Dispose();
                                    }
                                }
                                break;
                            case DebugEventType.EXCEPTION_DEBUG_EVENT:
                                IntPtr exceptionAddress = evt.DebugInfo.Exception.ExceptionRecord.ExceptionAddress;
                                switch (evt.DebugInfo.Exception.ExceptionRecord.ExceptionCode)
                                {
                                    case DebugExceptionCode.EXCEPTION_BREAKPOINT:
                                        if (activeBreakpoints.TryGetValue(exceptionAddress, out Breakpoint breakpoint))
                                        {
                                            // TODO: Implement next-stepping instead of "breakpointsToApply".
                                            ThreadContext64 ctx = process.GetThreadContext((int)evt.dwThreadId);
                                            DebugEventOptions options = new DebugEventOptions();
                                            Breakpoint.Context bpCtx = new Breakpoint.Context(breakpoint, evt.dwThreadId, ref ctx, options);
                                            
                                            breakpoint.Remove(bpCtx);
                                            OnBreakpointHit(bpCtx);
                                            breakpointsToApply.Add(breakpoint);

                                            if (options.StopDebuggingRequested)
                                            {
                                                Dispose();
                                            }
                                        }
                                        break;
                                    default:
                                        Log.Debug($"Native exception {evt.DebugInfo.Exception.ExceptionRecord.ExceptionCode.ToString()} occurred at: 0x{exceptionAddress.ToString("X")}");
                                        break;
                                }
                                break;
                        }
                        Win32Native.ContinueDebugEvent(evt.dwProcessId, evt.dwThreadId, continueStatus);
                    }
                    Win32Native.DebugActiveProcessStop(Process.Id);
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                    cancellation.Cancel();
                    Process = null;
                    throw;
                }
                finally
                {
                    Dispose();
                }
            });
            debuggerThread.IsBackground = true;
        }

        public static ProcessDebugger Create(Func<ProcessEx> processCreater, bool disposeProcess = true)
        {
            return new ProcessDebugger(processCreater, disposeProcess);
        }

        public async Task StartAsync()
        {
            if (disposed)
            {
                throw new ObjectDisposedException(nameof(ProcessDebugger));
            }
            if (debuggerThread == null || debuggerThread.IsAlive)
            {
                return;
            }

            debuggerThread.Start();
            await Task.Run(() =>
            {
                while (!cancellation.IsCancellationRequested && Process == null)
                {
                    Task.Delay(100).Wait();
                }
            });
        }

        public event EventHandlers.EventHandler<ProcessDebugger, DebugEventOptions> EntrypointBreak;
        public event EventHandlers.EventHandler<ProcessDebugger, Breakpoint.Context> BreakpointHit;

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }
            disposed = true;

            try
            {
                if (disposeProcess)
                {
                    Process.Dispose();
                }
                debuggerThread = null;
                cancellation.Cancel();
            }
            catch
            {
                // ignored
            }
        }

        public async Task WaitForDebuggerExit()
        {
            if (cancellation.IsCancellationRequested)
            {
                return;
            }

            await Task.Run(() =>
            {
                while (!cancellation.IsCancellationRequested)
                {
                    Task.Delay(10).Wait();
                }
            });
        }

        public void AddBreakpoint(IntPtr address)
        {
            if (process == null)
            {
                throw new Exception("Cannot add breakpoint if process hasn't started yet.");
            }
            if (activeBreakpoints.ContainsKey(address))
            {
                throw new Exception($"Another breakpoint already exists on address 0x{address.ToString("X")}");
            }

            // TODO: Validate address is in range of process memory and on a valid location (address is in code section).
            // TODO: Allow adding of breakpoints before process has started. Applying them as soon as address is valid.
            Int3Breakpoint breakpoint = new Int3Breakpoint(process, address);
            activeBreakpoints.Add(breakpoint.Address, breakpoint);
            breakpoint.Apply();
        }

        public bool RemoveBreakpoint(IntPtr address)
        {
            if (process == null)
            {
                return false;
            }
            if (!activeBreakpoints.TryGetValue(address, out Breakpoint breakpoint))
            {
                return false;
            }

            try
            {
                breakpoint.Dispose();
                activeBreakpoints.Remove(address);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void OnEntrypointBreak(DebugEventOptions options)
        {
            EntrypointBreak?.Invoke(this, options);
        }

        private void OnBreakpointHit(Breakpoint.Context context)
        {
            BreakpointHit?.Invoke(this, context);
        }

        public class DebugEventOptions
        {
            public bool StopDebuggingRequested { get; private set; }

            public void StopDebugging()
            {
                StopDebuggingRequested = true;
            }
        }
    }
}
