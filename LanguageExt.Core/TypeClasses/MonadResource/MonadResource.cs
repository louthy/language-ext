using LanguageExt.Attributes;
using System;

namespace LanguageExt.TypeClasses
{
    [Typeclass]
    public interface MonadResource : Typeclass
    {
        Resource<R> Use<R>(Func<R> acquire) where R : IDisposable;

        Resource<Unit> Release<R>(Resource<R> resource) where R : IDisposable;
    }
}
