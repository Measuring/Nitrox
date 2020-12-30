using System;

namespace NitroxModel.Platforms.OS.Windows.Structs.Debug
{
    public struct CreateThreadDebugInfo
    {
        public IntPtr hThread;
        public IntPtr lpThreadLocalBase;
        public DebugEvent64.PThreadStartRoutine lpStartAddress;
    }
}
