using System;
using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// Represents an error value.  
/// </summary>
/// <remarks>
/// On its own this doesn't do much, but  allows other monads to convert
/// from it and provide binding extensions that mean it can be lifted into
/// other monads without specifying lots of extra generic arguments.
/// </remarks>
/// <param name="Value">Bound value</param>
/// <typeparam name="E">Bound value type</typeparam>
public readonly record struct Fail<E>(E Value)
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Standard operators
    //

    public Fail<F> MapFail<F>(Func<E, F> f) =>
        new(f(Value));

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Conversion
    //

    public Either<E, A> ToEither<A>() =>
        Either.Left<E, A>(Value);

    public override string ToString() =>
        $"Fail({Value})";

    public Either<E, C> SelectMany<B, C>(Func<Unit, Pure<B>> bind, Func<Unit, B, C> project) =>
        this;

    public Option<C> SelectMany<B, C>(Func<Unit, Option<B>> bind, Func<Unit, B, C> project) =>
        default;
}

public static class FailExtensions
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Conversion
    //
    
    public static Fin<A> ToFin<A>(this Fail<Error> fail) =>
        Fin.Fail<A>(fail.Value);
    
    public static Validation<F, A> ToValidation<F, A>(this Fail<F> fail) 
        where F : Monoid<F> =>
        Validation.Fail<F, A>(fail.Value);
    
    public static Eff<RT, A> ToEff<RT, A>(this Fail<Error> fail) =>
        Eff<RT, A>.Fail(fail.Value);
    
    public static Eff<A> ToEff<A>(this Fail<Error> fail) =>
        Eff<A>.Fail(fail.Value);
}
