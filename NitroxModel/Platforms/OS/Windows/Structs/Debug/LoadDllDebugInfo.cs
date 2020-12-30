using System;
using System.Runtime.InteropServices;

namespace NitroxModel.Platforms.OS.Windows.Structs.Debug
{
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct LoadDllDebugInfo
    {
        [FieldOffset(0)]
        public IntPtr hFile;
        [FieldOffset(0x8)]
        public IntPtr lpBaseOfDll;
        [FieldOffset(0x10)]
        public uint dwDebugInfoFileOffset;
        [FieldOffset(0x14)]
        public uint nDebugInfoSize;
        [FieldOffset(0x18)]
        public IntPtr lpImageName;
        [FieldOffset(0x20)]
        public ushort fUnicode;
    }
}
