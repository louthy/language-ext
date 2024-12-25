using System;
using LanguageExt.Traits;
using LanguageExt.Common;

namespace LanguageExt.DSL;

/// <summary>
/// Base-type of the IOCatch〈X, A〉 DSL-type used by the `IO` monad.
///
/// Use this to extend the core `IO` exception catching functionality.   
/// </summary>
/// <typeparam name="A">Bound value type</typeparam>
public abstract record IOCatch<A> : IO<A>
{
    /// <summary>
    /// Provide the `IO` computation to run inside the `try` block 
    /// </summary>
    /// <returns>`IO` computation to run inside the `try` block</returns>
    public abstract IO<A> MakeOperation();
    
    /// <summary>
    /// Provide the handler that will accept an `Exception` if one is thrown. 
    /// </summary>
    /// <returns>Function to handle exceptions that returns an `IO`</returns>
    public abstract Func<Exception, IO<A>> MakeHandler();
    
    public override string ToString() => 
        "IO catch";
}

record IOCatch<X, A>(
    K<IO, X> Operation,
    Func<Error, bool> Predicate,
    Func<Error, K<IO, X>> Failure,
    K<IO, Unit>? Final,
    Func<X, K<IO, A>> Next) : IOCatch<A>
{
    public override IO<B> Map<B>(Func<A, B> f) =>
        new IOCatch<X, B>(Operation, Predicate, Failure, Final, x => Next(x).Map(f));

    public override IO<B> Bind<B>(Func<A, K<IO, B>> f) =>
        new IOCatch<X, B>(Operation, Predicate, Failure, Final, x => Next(x).Bind(f));

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
    
    public override string ToString() => 
        "IO catch";
}
