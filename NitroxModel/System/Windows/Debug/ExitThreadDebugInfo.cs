using System;
using System.Runtime.InteropServices;

namespace NitroxModel.System.Windows.Debug
{
    [StructLayout(LayoutKind.Sequential)]
    public struct ExitThreadDebugInfo
    {
        public uint dwExitCode;
    }
}
