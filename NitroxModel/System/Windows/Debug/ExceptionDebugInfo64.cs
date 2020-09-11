using System.Runtime.InteropServices;

namespace NitroxModel.System.Windows.Debug
{
    [StructLayout(LayoutKind.Sequential)]
    public struct ExceptionDebugInfo64
    {
        public DebugExceptionRecord64 ExceptionRecord;
        public uint dwFirstChance;
    }
}
