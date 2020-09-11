using System.Runtime.InteropServices;

namespace NitroxModel.System.Windows.Debug
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
