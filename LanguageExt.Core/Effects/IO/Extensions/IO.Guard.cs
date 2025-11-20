using System;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static class IOGuardExtensions
{
    extension(Guard<Error, Unit> guard)
    {
        /// <summary>
        /// Monadic binding support for `IO`
        /// </summary>
        public IO<B> Bind<B>(Func<Unit, IO<B>> f) =>
            guard.Flag 
                ? f(unit).As() 
                : Fail(guard.OnFalse());

        /// <summary>
        /// Monadic binding support for `IO`
        /// </summary>
        public IO<C> SelectMany<B, C>(Func<Unit, IO<B>> bind, 
                                      Func<Unit, B, C> project) =>
            guard.Flag
                ? bind(default).As().Map(b => project(default, b))
                : Fail(guard.OnFalse());

        /// <summary>
        /// Natural transformation to `IO`
        /// </summary>
        public IO<Unit> ToIO() =>
            IO.lift(() => guard.Flag
                              ? unit
                              : guard.OnFalse().Throw());
    }
}
