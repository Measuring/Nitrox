using System.Runtime.InteropServices;

namespace NitroxModel.Platforms.OS.Windows.Structs.Debug
{
    [StructLayout(LayoutKind.Sequential)]
    public struct ExitProcessDebugInfo
    {
        public uint dwExitCode;
    }
}
