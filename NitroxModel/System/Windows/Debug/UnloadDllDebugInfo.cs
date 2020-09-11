using System;
using System.Runtime.InteropServices;

namespace NitroxModel.System.Windows.Debug
{
    [StructLayout(LayoutKind.Sequential)]
    public struct UnloadDllDebugInfo
    {
        public IntPtr lpBaseOfDll;
    }
}
