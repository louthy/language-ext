using System;
using LanguageExt.Common;

namespace LanguageExt;

public readonly record struct CatchValue<A>(Func<Error, bool> Match, Func<Error, A> Value)
{
    public CatchValue<Error, A> As() =>
        new (Match, Value);
}

public readonly record struct CatchValue<E, A>(Func<E, bool> Match, Func<E, A> Value);

public readonly record struct CatchError(Func<Error, bool> Match, Func<Error, Error> Value)
{
    public CatchError<Error> As() =>
        new (Match, Value);
}

public readonly record struct CatchError<E>(Func<E, bool> Match, Func<E, E> Value);

public readonly record struct CatchIO<A>(Func<Error, bool> Match, Func<Error, IO<A>> Value)
{
    public CatchIO<Error, A> As() =>
        new (Match, Value);
}

public readonly record struct CatchIO<E, A>(Func<E, bool> Match, Func<E, IO<A>> Value);
