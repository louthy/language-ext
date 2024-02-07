using System;
using LanguageExt.Common;
using LanguageExt.Effects.Traits;
using LanguageExt.HKT;
using LanguageExt.TypeClasses;

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
public readonly record struct Fail<E>(E Value) : KStar<Any, E>
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
        Either<E, A>.Left(Value);
    
    public Validation<E, A> ToValidation<A>() =>
        Validation<E, A>.Fail([Value]);
    
    public IO<RT, E, A> ToIO<RT, A>()
        where RT : HasIO<RT, E> =>
        IO<RT, E, A>.Fail(Value);
    
    public IO<E, A> ToIO<A>() =>
        IO<E, A>.Fail(Value);

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Transducer
    //

    public Transducer<Unit, E> Morphism { get; } =
        Transducer.pure(Value);

    public Reducer<Unit, S> Transform<S>(Reducer<E, S> reduce) =>
        Morphism.Transform(reduce);

    public override string ToString() =>
        $"Fail({Value})";
}

public static class FailExtensions
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Conversion
    //
    
    public static Fin<A> ToFin<A>(this Fail<Error> fail) =>
        Fin<A>.Fail(fail.Value);
    
    public static Validation<MonadFail, FAIL, A> ToValidation<MonadFail, FAIL, A>(this Fail<FAIL> fail) 
        where MonadFail : Monoid<FAIL>, Eq<FAIL> =>
        Validation<MonadFail, FAIL, A>.Fail(fail.Value);
    
    public static Eff<RT, A> ToEff<RT, A>(this Fail<Error> fail)
        where RT : HasIO<RT, Error> =>
        Eff<RT, A>.Fail(fail.Value);
    
    public static Eff<A> ToEff<A>(this Fail<Error> fail) =>
        Eff<A>.Fail(fail.Value);
}
