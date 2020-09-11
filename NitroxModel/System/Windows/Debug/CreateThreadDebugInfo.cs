using System;

namespace NitroxModel.System.Windows.Debug
{
    public struct CreateThreadDebugInfo
    {
        public IntPtr hThread;
        public IntPtr lpThreadLocalBase;
        public DebugEvent64.PThreadStartRoutine lpStartAddress;
    }
}
