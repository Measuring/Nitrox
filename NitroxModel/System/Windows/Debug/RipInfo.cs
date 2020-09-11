using System.Runtime.InteropServices;

namespace NitroxModel.System.Windows.Debug
{
    [StructLayout(LayoutKind.Sequential)]
    public struct RipInfo
    {
        public uint dwError;
        public uint dwType;
    }
}
