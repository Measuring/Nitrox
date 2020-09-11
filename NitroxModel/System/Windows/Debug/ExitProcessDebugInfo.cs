using System;
using System.Runtime.InteropServices;

namespace NitroxModel.System.Windows.Debug
{
    [StructLayout(LayoutKind.Sequential)]
    public struct ExitProcessDebugInfo
    {
        public uint dwExitCode;
    }
}
