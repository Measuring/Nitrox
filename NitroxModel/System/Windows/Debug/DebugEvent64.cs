using System;
using System.Runtime.InteropServices;

namespace NitroxModel.System.Windows.Debug
{
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct DebugEvent64
    {
        public delegate uint PThreadStartRoutine(IntPtr lpThreadParameter);

        [FieldOffset(0)]
        public DebugEventType dwDebugEventCode;

        [FieldOffset(0x4)]
        public int dwProcessId;

        [FieldOffset(0x8)]
        public uint dwThreadId;

        [FieldOffset(0xC)]
        public DebugInfo64 DebugInfo;
    }
}
