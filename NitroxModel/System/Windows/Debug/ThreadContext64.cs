using System;
using System.Runtime.InteropServices;

namespace NitroxModel.System.Windows.Debug
{
    /// <summary>
    ///     See <a href="https://www.pinvoke.net/default.aspx/kernel32/GetThreadContext.html">GetThreadContext</a>
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 16)]
    public struct ThreadContext64
    {
        public ulong P1Home;
        public ulong P2Home;
        public ulong P3Home;
        public ulong P4Home;
        public ulong P5Home;
        public ulong P6Home;

        public CONTEXT_FLAGS ContextFlags;
        public uint MxCsr;

        public ushort SegCs;
        public ushort SegDs;
        public ushort SegEs;
        public ushort SegFs;
        public ushort SegGs;
        public ushort SegSs;
        public uint EFlags;

        public ulong Dr0;
        public ulong Dr1;
        public ulong Dr2;
        public ulong Dr3;
        public ulong Dr6;
        public ulong Dr7;

        public UIntPtr Rax;
        public UIntPtr Rcx;
        public UIntPtr Rdx;
        public UIntPtr Rbx;
        public UIntPtr Rsp;
        public UIntPtr Rbp;
        public UIntPtr Rsi;
        public UIntPtr Rdi;
        public UIntPtr R8;
        public UIntPtr R9;
        public UIntPtr R10;
        public UIntPtr R11;
        public UIntPtr R12;
        public UIntPtr R13;
        public UIntPtr R14;
        public UIntPtr R15;
        public UIntPtr Rip;

        public XSAVE_FORMAT64 DUMMYUNIONNAME;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 26)]
        public M128A[] VectorRegister;

        public ulong VectorControl;

        public ulong DebugControl;
        public ulong LastBranchToRip;
        public ulong LastBranchFromRip;
        public ulong LastExceptionToRip;
        public ulong LastExceptionFromRip;

        public override string ToString()
        {
            return $"{nameof(Rax)}: 0x{Rax:X}, {nameof(Rbx)}: 0x{Rbx:X}, {nameof(Rcx)}: 0x{Rcx:X}, {nameof(Rdx)}: 0x{Rdx:X}, {nameof(Rsp)}: 0x{Rsp:X}, {nameof(Rbp)}: 0x{Rbp:X}, {nameof(Rsi)}: 0x{Rsi:X}, {nameof(Rdi)}: 0x{Rdi:X}, {nameof(R8)}: 0x{R8:X}, {nameof(R9)}: 0x{R9:X}, {nameof(R10)}: 0x{R10:X}, {nameof(R11)}: 0x{R11:X}, {nameof(R12)}: 0x{R12:X}, {nameof(R13)}: 0x{R13:X}, {nameof(R14)}: 0x{R14:X}, {nameof(R15)}: 0x{R15:X}, {nameof(Rip)}: 0x{Rip:X}";
        }
    }

    public enum CONTEXT_FLAGS : uint
    {
        CONTEXT_i386 = 0x10000,
        CONTEXT_i486 = 0x10000, //  same as i386
        CONTEXT_CONTROL = CONTEXT_i386 | 0x01, // SS:SP, CS:IP, FLAGS, BP
        CONTEXT_INTEGER = CONTEXT_i386 | 0x02, // AX, BX, CX, DX, SI, DI
        CONTEXT_SEGMENTS = CONTEXT_i386 | 0x04, // DS, ES, FS, GS
        CONTEXT_FLOATING_POINT = CONTEXT_i386 | 0x08, // 387 state
        CONTEXT_DEBUG_REGISTERS = CONTEXT_i386 | 0x10, // DB 0-3,6,7
        CONTEXT_EXTENDED_REGISTERS = CONTEXT_i386 | 0x20, // cpu specific extensions
        CONTEXT_FULL = CONTEXT_CONTROL | CONTEXT_INTEGER | CONTEXT_SEGMENTS,
        CONTEXT_ALL = CONTEXT_CONTROL | CONTEXT_INTEGER | CONTEXT_SEGMENTS | CONTEXT_FLOATING_POINT | CONTEXT_DEBUG_REGISTERS | CONTEXT_EXTENDED_REGISTERS
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct M128A
    {
        public ulong High;
        public long Low;

        public override string ToString()
        {
            return string.Format("High:{0}, Low:{1}", High, Low);
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 16)]
    public struct XSAVE_FORMAT64
    {
        public ushort ControlWord;
        public ushort StatusWord;
        public byte TagWord;
        public byte Reserved1;
        public ushort ErrorOpcode;
        public uint ErrorOffset;
        public ushort ErrorSelector;
        public ushort Reserved2;
        public uint DataOffset;
        public ushort DataSelector;
        public ushort Reserved3;
        public uint MxCsr;
        public uint MxCsr_Mask;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public M128A[] FloatRegisters;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public M128A[] XmmRegisters;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 96)]
        public byte[] Reserved4;
    }
}
