using System;
using System.Runtime.InteropServices;

namespace NitroxModel.System.Windows
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct SecurityAttributes
    {
        public int length;
        public IntPtr lpSecurityDescriptor;
        public bool bInheritHandle;
    }
}
