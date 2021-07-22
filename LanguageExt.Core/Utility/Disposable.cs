using System;

namespace LanguageExt
{
    internal static class Disposable<A>
    {
        public static bool IsDisposable = typeof(IDisposable).IsAssignableFrom(typeof(A));
    }
}
