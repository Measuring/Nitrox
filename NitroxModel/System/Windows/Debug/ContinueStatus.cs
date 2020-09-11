namespace NitroxModel.System.Windows.Debug
{
    public enum ContinueStatus : uint
    {
        DBG_CONTINUE = 0x00010002,
        DBG_EXCEPTION_NOT_HANDLED = 0x80010001,
        DBG_REPLY_LATER = 0x40010001
    }
}
