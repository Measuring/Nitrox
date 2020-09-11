using System.Runtime.InteropServices;

namespace NitroxModel.System.Windows.Debug
{
    public struct DebugInfo64
    {
        public ExceptionDebugInfo64 Exception;

        public CreateThreadDebugInfo CreateThread;

        public CreateProcessDebugInfo CreateProcessInfo;

        public ExitThreadDebugInfo ExitThread;

        public ExitProcessDebugInfo ExitProcess;

        public LoadDllDebugInfo LoadDll;

        public UnloadDllDebugInfo UnloadDll;

        public OutputDebugStringInfo DebugString;

        public RipInfo RipInfo;
    }
}
