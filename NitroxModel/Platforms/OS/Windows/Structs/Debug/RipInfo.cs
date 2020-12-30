using System.Runtime.InteropServices;

namespace NitroxModel.Platforms.OS.Windows.Structs.Debug
{
    [StructLayout(LayoutKind.Sequential)]
    public struct RipInfo
    {
        public uint dwError;
        public uint dwType;
    }
}
