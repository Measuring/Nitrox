using System;
using System.Runtime.InteropServices;

namespace NitroxModel.System.Windows
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct ProcessBasicInformation
    {
        public NtStatus ExitStatus;
        public IntPtr PebBaseAddress;
        public UIntPtr AffinityMask;
        public int BasePriority;
        public UIntPtr UniqueProcessId;
        public UIntPtr InheritedFromUniqueProcessId;
    }
}
