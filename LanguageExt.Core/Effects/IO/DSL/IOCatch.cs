using System;
using LanguageExt.Traits;
using LanguageExt.Common;
using System.Threading.Tasks;
using LanguageExt.DSL;

namespace LanguageExt.DSL;

abstract record IOCatch<A> : IODsl<A>
{
    public abstract ValueTask<A> Invoke(EnvIO envIO);
}

record IOCatch<X, A>(
    K<IO, X> Operation,
    Func<Error, bool> Predicate,
    Func<Error, K<IO, X>> Failure,
    Func<X, A> Next) : IOCatch<A>
{
    public override IODsl<B> Map<B>(Func<A, B> f) =>
        new IOCatch<X, B>(Operation, Predicate, Failure, x => f(Next(x)));

    public override async ValueTask<A> Invoke(EnvIO envIO)
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
