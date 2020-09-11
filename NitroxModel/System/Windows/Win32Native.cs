using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using Microsoft.Win32.SafeHandles;
using NitroxModel.System.Windows.Debug;

namespace NitroxModel.System.Windows
{
    internal static class Win32Native
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [SuppressUnmanagedCodeSecurity]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll", EntryPoint = "WaitForDebugEvent")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool WaitForDebugEvent(ref DebugEvent64 lpDebugEvent, uint dwMilliseconds);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool DebugActiveProcess(int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool ContinueDebugEvent(int dwProcessId, uint dwThreadId, ContinueStatus dwContinueStatus);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool DebugActiveProcessStop(int dwProcessId);

        /// <summary>
        ///     If true, kills the debugged process when debugger detaches.
        /// </summary>
        /// <param name="killOnExit"></param>
        [DllImport("kernel32.dll")]
        public static extern void DebugSetProcessKillOnExit(bool killOnExit);

        [DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWow64Process(
            [In] SafeHandle hProcess,
            [Out] [MarshalAs(UnmanagedType.Bool)] out bool wow64Process
        );

        public static string QueryFullProcessImageName(SafeHandle process, uint flags = 0)
        {
            try
            {
                StringBuilder fileNameBuilder = new StringBuilder(1024);
                int size = fileNameBuilder.Capacity;
                return QueryFullProcessImageName(process, flags, fileNameBuilder, ref size) ? fileNameBuilder.ToString() : null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static ThreadContext64 GetThreadContext(SafeHandle thread)
        {
            ThreadContext64 context = default;
            context.ContextFlags = CONTEXT_FLAGS.CONTEXT_ALL;
            GetThreadContext(thread, ref context);
            return context;
        }

        [DllImport("kernel32.dll")]
        public static extern bool TerminateProcess(SafeHandle hProcess, int exitCode);

        [DllImport("kernel32.dll")]
        public static extern uint SuspendThread(SafeHandle hThread);

        [DllImport("kernel32.dll")]
        public static extern uint ResumeThread(SafeHandle hThread);

        [DllImport("ntdll.dll", PreserveSig = false)]
        public static extern void NtSuspendProcess(SafeHandle processHandle);

        [DllImport("ntdll.dll", PreserveSig = false, SetLastError = true)]
        public static extern void NtResumeProcess(SafeHandle processHandle);

        [DllImport("kernel32.dll")]
        public static extern bool CreateProcess(
            string lpApplicationName,
            string lpCommandLine,
            IntPtr lpProcessAttributes,
            IntPtr lpThreadAttributes,
            bool bInheritHandles,
            ProcessCreationFlags dwCreationFlags,
            IntPtr lpEnvironment,
            string lpCurrentDirectory,
            ref StartupInfo lpStartupInfo,
            out ProcessInfo lpProcessInformation
        );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern SafeProcessHandle OpenProcess(
            ProcessAccessFlags processAccess,
            bool bInheritHandle,
            int processId
        );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool ReadProcessMemory(
            SafeHandle hProcess,
            IntPtr lpBaseAddress,
            [Out] byte[] lpBuffer,
            int dwSize,
            out IntPtr lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool WriteProcessMemory(SafeHandle hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int nSize, out int lpNumberOfBytesWritten);

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtQueryInformationProcess(SafeHandle processHandle, int processInformationClass, ref ProcessBasicInformation processInformation, int processInformationLength, IntPtr returnLength);

        [DllImport("kernel32.dll")]
        public static extern IntPtr CreateRemoteThread(SafeHandle hProcess, IntPtr lpThreadAttributes, uint dwStackSize, IntPtr lpStartAddress,
                                                       IntPtr lpParameter, uint dwCreationFlags, out IntPtr lpThreadId);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetThreadContext(SafeHandle threadHandle, [In] ThreadContext64 context);

        [DllImport("kernel32.dll")]
        internal static extern bool FlushInstructionCache(SafeHandle hProcess, IntPtr lpBaseAddress, uint dwSize);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern SafeAccessTokenHandle OpenThread(ThreadAccess dwDesiredAccess, bool bInheritHandle, uint dwThreadId);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool GetThreadContext(SafeHandle threadHandle, [In] [Out] ref ThreadContext64 context);

        [DllImport("kernel32.dll")]
        private static extern bool QueryFullProcessImageName([In] SafeHandle hProcess, [In] uint dwFlags, [Out] StringBuilder lpExeName, [In] [Out] ref int lpdwSize);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool QueryFullProcessImageName([In] SafeHandle hProcess, [In] int dwFlags, [Out] StringBuilder lpExeName, ref int lpdwSize);
    }
}
