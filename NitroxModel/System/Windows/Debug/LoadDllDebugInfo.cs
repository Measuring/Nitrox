using System;
using System.Runtime.InteropServices;

namespace NitroxModel.System.Windows.Debug
{
    [StructLayout(LayoutKind.Sequential)]
    public struct LoadDllDebugInfo
    {
        public IntPtr hFile;
        public IntPtr lpBaseOfDll;
        public uint dwDebugInfoFileOffset;
        public uint nDebugInfoSize;
        public IntPtr lpImageName;
        public ushort fUnicode;
    }
}
