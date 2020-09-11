using System;
using NitroxModel.System.Windows.Debug;

namespace NitroxModel.System.Debug
{
    public abstract class Breakpoint : IDisposable
    {
        protected readonly ProcessEx process;
        private bool isApplied;

        public bool IsDisposed { get; private set; }
        public IntPtr Address { get; private set; }

        protected Breakpoint(ProcessEx process, IntPtr address)
        {
            this.process = process;
            Address = address;
        }

        protected abstract void ApplyInternal();
        protected abstract void DiposeInternal();

        public static bool operator ==(Breakpoint left, Breakpoint right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Breakpoint left, Breakpoint right)
        {
            return !Equals(left, right);
        }

        public void Apply()
        {
            if (isApplied)
            {
                throw new Exception($"Breakpoint on 0x{Address.ToString("X")} is already applied.");
            }
            isApplied = true;

            ApplyInternal();
        }

        public bool Remove(Context context)
        {
            if (!isApplied)
            {
                return false;
            }

            if (!RemoveInternal(context))
            {
                return false;
            }
            isApplied = false;
            return true;
        }

        public void Dispose()
        {
            if (process == null)
            {
                throw new Exception("Unable to cleanup breakpoint because process is dead or breakpoint was never applied.");
            }
            if (IsDisposed)
            {
                throw new ObjectDisposedException("Breakpoint is already disposed.");
            }
            IsDisposed = true;

            DiposeInternal();
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != GetType())
            {
                return false;
            }
            return Equals((Breakpoint)obj);
        }

        public override int GetHashCode()
        {
            return Address.GetHashCode();
        }

        public override string ToString()
        {
            return $"{nameof(Address)}: 0x{Address.ToString("X")}";
        }

        protected virtual bool RemoveInternal(Context context)
        {
            return false;
        }

        protected bool Equals(Breakpoint other)
        {
            return Address.Equals(other.Address);
        }

        public sealed class Context
        {
            public Breakpoint Breakpoint { get; }
            public ProcessDebugger.DebugEventOptions DebuggerOptions { get; private set; }
            public int ThreadId { get; }

            public ThreadContext64 ThreadContext;

            public Context(Breakpoint breakpoint, int threadId, ref ThreadContext64 threadContext, ProcessDebugger.DebugEventOptions options)
            {
                Breakpoint = breakpoint;
                ThreadId = threadId;
                ThreadContext = threadContext;
                DebuggerOptions = options;
            }

            public Context(Breakpoint breakpoint, uint threadId, ref ThreadContext64 threadContext, ProcessDebugger.DebugEventOptions options) : this(breakpoint, (int)threadId, ref threadContext, options)
            {
            }
        }
    }
}
