using LanguageExt.Common;
using LanguageExt.Effects.Traits;
using LanguageExt.TypeClasses;

namespace LanguageExt;

/// <summary>
/// Represents an error value.  
/// </summary>
/// <remarks>
/// On its own this doesn't do much, but it allows other monads to convert
/// from it and provide binding extensions that mean this will work with
/// any monad.
///
/// It simplifies certain scenarios where additional generic arguments are
/// needed.  This only requires rhe generic argument of the value which
/// means the C# inference system can work.
/// </remarks>
/// <param name="Value">Bound value</param>
/// <typeparam name="E">Bound value type</typeparam>
public readonly record struct Fail<E>(E Value)
{
    public Either<E, A> ToEither<A>() =>
        Either<E, A>.Left(Value);
    
    public Validation<E, A> ToValidation<A>() =>
        Validation<E, A>.Fail(Prelude.Seq1(Value));
    
    public IO<RT, E, A> ToIO<RT, A>()
        where RT : struct, HasIO<RT, E> =>
        IO<RT, E, A>.Fail(Value);
}

public static class FailExtensions
{
    public static Fin<A> ToFin<A>(this Fail<Error> fail) =>
        Fin<A>.Fail(fail.Value);
    
    public static Validation<MonadFail, FAIL, A> ToValidation<MonadFail, FAIL, A>(this Fail<FAIL> fail) 
        where MonadFail : struct, Monoid<FAIL>, Eq<FAIL> =>
        Validation<MonadFail, FAIL, A>.Fail(fail.Value);
    
    public static Eff<RT, A> ToEff<RT, A>(this Fail<Error> fail)
        where RT : struct, HasIO<RT, Error> =>
        Eff<RT, A>.Fail(fail.Value);
    
    public static Eff<A> ToEff<A>(this Fail<Error> fail) =>
        Eff<A>.Fail(fail.Value);
}
