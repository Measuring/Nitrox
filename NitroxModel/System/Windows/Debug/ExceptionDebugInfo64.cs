using System.Runtime.InteropServices;

namespace NitroxModel.System.Windows.Debug
{
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct ExceptionDebugInfo64
    {
        [FieldOffset(0)]
        public DebugExceptionRecord64 ExceptionRecord;
        [FieldOffset(0x8)]
        public uint dwFirstChance;
    }
}
