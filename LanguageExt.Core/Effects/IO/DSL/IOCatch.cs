using System;
using LanguageExt.Traits;
using LanguageExt.Common;

namespace LanguageExt;

abstract record IOCatch<A> : IO<A>
{
    public abstract IO<A> MakeOperation();
    public abstract Func<Exception, IO<A>> MakeHandler();
}

record IOCatch<X, A>(
    K<IO, X> Operation,
    Func<Error, bool> Predicate,
    Func<Error, K<IO, X>> Failure,
    Func<X, K<IO, A>> Next) : IOCatch<A>
{
    public override IO<B> Map<B>(Func<A, B> f) =>
        new IOCatch<X, B>(Operation, Predicate, Failure, x => Next(x).Map(f));

    public override IO<B> Bind<B>(Func<A, K<IO, B>> f) =>
        new IOCatch<X, B>(Operation, Predicate, Failure, x => Next(x).Bind(f));

    public override Func<Exception, IO<A>> MakeHandler() =>
        e =>
        {
            var err = Error.New(e);
            return Predicate(err)
                ? Failure(e).Bind(Next).As()
                : IO.fail<A>(err);
        };

    public override IO<A> MakeOperation() =>
        Operation.Bind(x => new IOCatchPop<X>(IO.pure(x))).Bind(Next).As();
}
