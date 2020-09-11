using System;
using System.Runtime.InteropServices;

namespace NitroxModel.System.Windows.Debug
{
    [StructLayout(LayoutKind.Sequential)]
    public struct DebugExceptionRecord64
    {
        public DebugExceptionCode ExceptionCode;
        public uint ExceptionFlags;
        public IntPtr ExceptionRecord;
        public IntPtr ExceptionAddress;
        public uint NumberParameters;
        public uint UnusedAlignment;
        
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 15, ArraySubType = UnmanagedType.U4)]
        public ulong[] ExceptionInformation;
    }
}
