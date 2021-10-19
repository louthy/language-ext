using System;
using LanguageExt.Common;

namespace LanguageExt
{
    public readonly struct CatchValue<A>
    {
        public readonly Func<Error, bool> Match;
        public readonly Func<Error, A> Value;

        public CatchValue(Func<Error, bool> match, Func<Error, A> value) =>
            (Match, Value) = (match, value);
    }

    public readonly struct CatchError
    {
        public readonly Func<Error, bool> Match;
        public readonly Func<Error, Error> Value;

        public CatchError(Func<Error, bool> match, Func<Error, Error> value) =>
            (Match, Value) = (match, value);
    }
}
