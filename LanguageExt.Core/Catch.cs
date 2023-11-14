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

    public readonly struct CatchValue<E, A>
    {
        public readonly Func<E, bool> Match;
        public readonly Func<E, A> Value;

        public CatchValue(Func<E, bool> match, Func<E, A> value) =>
            (Match, Value) = (match, value);
    }

    public readonly struct CatchError
    {
        public readonly Func<Error, bool> Match;
        public readonly Func<Error, Error> Value;

        public CatchError(Func<Error, bool> match, Func<Error, Error> value) =>
            (Match, Value) = (match, value);
    }

    public readonly struct CatchError<E>
    {
        public readonly Func<E, bool> Match;
        public readonly Func<E, E> Value;

        public CatchError(Func<E, bool> match, Func<E, E> value) =>
            (Match, Value) = (match, value);
    }
}
