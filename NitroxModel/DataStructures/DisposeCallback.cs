using System;

namespace NitroxModel.DataStructures
{
    public class DisposeCallback : IDisposable
    {
        private readonly Action disposer;

        public DisposeCallback()
        {
        }

        public DisposeCallback(Action disposer)
        {
            this.disposer = disposer;
        }

        public void Dispose()
        {
            disposer?.Invoke();
        }
    }
}
