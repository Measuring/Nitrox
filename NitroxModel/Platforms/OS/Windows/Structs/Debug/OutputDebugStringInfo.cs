using System.Runtime.InteropServices;

namespace NitroxModel.Platforms.OS.Windows.Structs.Debug
{
    [StructLayout(LayoutKind.Sequential)]
    public struct OutputDebugStringInfo
    {
        [MarshalAs(UnmanagedType.LPStr)]
        public string lpDebugStringData;

        public ushort fUnicode;
        public ushort nDebugStringLength;
    }
}
