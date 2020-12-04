using System;
using System.Runtime.InteropServices;

namespace NitroxModel.System.Windows.Debug
{
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct CreateProcessDebugInfo64
    {
        [FieldOffset(0)]
        public IntPtr hFile;
        [FieldOffset(0x8)]
        public IntPtr hProcess;
        [FieldOffset(0x10)]
        public IntPtr hThread;
        [FieldOffset(0x18)]
        public IntPtr lpBaseOfImage;
        [FieldOffset(0x20)]
        public uint dwDebugInfoFileOffset;
        [FieldOffset(0x24)]
        public uint nDebugInfoSize;
        [FieldOffset(0x28)]
        public IntPtr lpThreadLocalBase;
        [FieldOffset(0x30)]
        public DebugEvent64.PThreadStartRoutine lpStartAddress;
        [FieldOffset(0x38)]
        public IntPtr lpImageName;
        [FieldOffset(0x3A)]
        public ushort fUnicode;
    }
}
