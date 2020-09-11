using System;
using System.Runtime.InteropServices;

namespace NitroxModel.System.Windows.Debug
{
    [StructLayout(LayoutKind.Sequential)]
    public struct CreateProcessDebugInfo
    {
        public IntPtr hFile;
        public IntPtr hProcess;
        public IntPtr hThread;
        public IntPtr lpBaseOfImage;
        public uint dwDebugInfoFileOffset;
        public uint nDebugInfoSize;
        public IntPtr lpThreadLocalBase;
        public DebugEvent64.PThreadStartRoutine lpStartAddress;
        public IntPtr lpImageName;
        public ushort fUnicode;
    }
}
