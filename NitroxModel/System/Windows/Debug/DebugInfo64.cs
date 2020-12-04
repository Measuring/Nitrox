using System.Runtime.InteropServices;

namespace NitroxModel.System.Windows.Debug
{
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct DebugInfo64
    {
        [FieldOffset(0)]
        public ExceptionDebugInfo64 Exception;

        // [FieldOffset(0)]
        // public CreateThreadDebugInfo CreateThread;
        //
        [FieldOffset(0)]
        public CreateProcessDebugInfo64 CreateProcessInfo;
        //
        // [FieldOffset(0)]
        // public ExitThreadDebugInfo ExitThread;
        //
        // [FieldOffset(0)]
        // public ExitProcessDebugInfo ExitProcess;
        
        [FieldOffset(0)]
        public LoadDllDebugInfo LoadDll;
        
        // [FieldOffset(0)]
        // public UnloadDllDebugInfo UnloadDll;
        //
        // [FieldOffset(0)]
        // public OutputDebugStringInfo DebugString;
        //
        // [FieldOffset(0)]
        // public RipInfo RipInfo;
    }
}
