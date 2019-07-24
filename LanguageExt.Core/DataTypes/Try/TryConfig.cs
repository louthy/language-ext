using System;

namespace LanguageExt
{
    public static class TryConfig
    {
        public static Action<Exception> ErrorLogger = ex => { };
    }
}
