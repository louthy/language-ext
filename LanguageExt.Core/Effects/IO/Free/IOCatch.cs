using System;
using LanguageExt.Traits;
using LanguageExt.Common;
using System.Threading.Tasks;

namespace LanguageExt;

abstract record IOCatch<A> : IO<A>
{
    public abstract ValueTask<IO<A>> Invoke(EnvIO envIO);
}

record IOCatch<X, A>(
    K<IO, X> Operation,
    Func<Error, bool> Predicate,
    Func<Error, K<IO, X>> Failure,
    Func<X, IO<A>> Next) : IOCatch<A>
{
    public override IO<B> Map<B>(Func<A, B> f) =>
        new IOCatch<X, B>(Operation, Predicate, Failure, x => Next(x).Map(f));

    public override IO<B> Bind<B>(Func<A, K<IO, B>> f) =>
        new IOCatch<X, B>(Operation, Predicate, Failure, x => Next(x).Bind(f));

    public override IO<B> ApplyBack<B>(K<IO, Func<A, B>> f) =>
        new IOCatch<X, B>(Operation, Predicate, Failure, x => Next(x).ApplyBack(f));

    public override async ValueTask<IO<A>> Invoke(EnvIO envIO)
    {
        try
        {
            var x = await Operation.As().RunAsync(envIO);
            return Next(x);
        }
        catch(Exception e)
        {
            var err = Error.New(e);
            if (Predicate(err))
            {
                var x = await Failure(e).As().RunAsync(envIO);
                return Next(x);
            }
            throw;
        }
    }
}
