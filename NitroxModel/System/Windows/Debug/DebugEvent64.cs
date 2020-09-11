using System;
using System.Runtime.InteropServices;

namespace NitroxModel.System.Windows.Debug
{
    [StructLayout(LayoutKind.Sequential)]
    public struct DebugEvent64
    {
        public delegate uint PThreadStartRoutine(IntPtr lpThreadParameter);

        public DebugEventType dwDebugEventCode;

        public int dwProcessId;

        public uint dwThreadId;

        public DebugInfo64 DebugInfo;
    }
}
