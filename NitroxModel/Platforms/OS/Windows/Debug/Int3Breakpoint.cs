using System;

namespace NitroxModel.Platforms.OS.Windows.Debug
{
    /// <summary>
    ///     Uses the interrupt instruction to notify the debugger of a breakpoint.
    /// </summary>
    /// <remarks>
    ///     This works by modifying the memory at the given address and writing the instruction. Then waiting for a debugger to
    ///     respond to the event. After which the debugger should immediately restore the memory by calling
    ///     <see cref="RemoveInternal" /> to reset the instruction pointer register (on x86-x64 this is RIP) back to the
    ///     breakpoint address (now with correct memory) and continue like normal.
    ///     When the breakpoint has been handled it should <see cref="ApplyInternal" /> again to "reset" and allow the
    ///     breakpoint to hit again.
    ///     This will continue until either the process is killed or the breakpoint is removed by disposing.
    /// </remarks>
    public sealed class Int3Breakpoint : Breakpoint
    {
        /// <summary>
        ///     Single byte instruction representing "int 3" in assembly for x86-64 process architectures.
        /// </summary>
        private static readonly byte[] interruptInstruction = { 0xCC };

        private byte[] bytesToRestore = Array.Empty<byte>();

        public Int3Breakpoint(ProcessEx process, IntPtr address) : base(process, address)
        {
        }

        protected override void ApplyInternal()
        {
            bytesToRestore = process.ReadMemory(Address, interruptInstruction.Length);
            process.WriteMemory(Address, interruptInstruction, true);
        }

        protected override bool RemoveInternal(Context context)
        {
            // Restore memory location and instruction register (RIP) at breakpoint address.
            process.WriteMemory(Address, bytesToRestore, true);
            context.ThreadContext.Rip = UIntPtr.Subtract(context.ThreadContext.Rip, interruptInstruction.Length);
            process.SetThreadContext(context.ThreadId, context.ThreadContext);

            return true;
        }

        protected override void DiposeInternal()
        {
            process.WriteMemory(Address, bytesToRestore, true);
        }
    }
}
