using System;

namespace LanguageExt
{
    [Flags]
    public enum InboxDirective
    {
        Default = 0,
        PushToFrontOfQueue = 1,
        Pause = 2
    }
}
