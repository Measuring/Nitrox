namespace NitroxModel.Helper
{
    public static class EventHandlers
    {
        public delegate void EventHandler<in TSender>(TSender sender);
        public delegate void EventHandler<in TSender, in TArg>(TSender sender, TArg eventArgs);
    }
}
