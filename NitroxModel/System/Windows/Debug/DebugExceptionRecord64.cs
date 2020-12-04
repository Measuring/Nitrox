using System;
using System.Runtime.InteropServices;

namespace NitroxModel.System.Windows.Debug
{
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct DebugExceptionRecord64
    {
        [FieldOffset(0)]
        public DebugExceptionCode ExceptionCode;
        [FieldOffset(0x8)]
        public uint ExceptionFlags;
        [FieldOffset(0xC)]
        public IntPtr ExceptionRecord;
        [FieldOffset(0x14)]
        public IntPtr ExceptionAddress;
        [FieldOffset(0x1C)]
        public uint NumberParameters;
        [FieldOffset(0x20)]
        public uint UnusedAlignment;
        
        // [FieldOffset(0x24)]
        // [MarshalAs(UnmanagedType.ByValArray, SizeConst = 15, ArraySubType = UnmanagedType.U4)]
        // public ulong[] ExceptionInformation;
    }
}
