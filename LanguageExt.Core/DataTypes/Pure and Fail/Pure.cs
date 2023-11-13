using LanguageExt.Common;
using LanguageExt.Effects.Traits;
using LanguageExt.TypeClasses;

namespace LanguageExt;

/// <summary>
/// Represents a pure value.  Usually understood to be the 'success' value.
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
/// <typeparam name="A">Bound value type</typeparam>
public readonly record struct Pure<A>(A Value)
{
    public Option<A> ToOption() =>
        Option<A>.Some(Value);
    
    public Either<L, A> ToEither<L>() =>
        Either<L, A>.Right(Value);
    
    public Fin<A> ToFin() =>
        Fin<A>.Succ(Value);
    
    public Validation<FAIL, A> ToValidation<FAIL>() =>
        Validation<FAIL, A>.Success(Value);
    
    public Validation<MonadFail, FAIL, A> ToValidation<MonadFail, FAIL>() 
        where MonadFail : struct, Monoid<FAIL>, Eq<FAIL> =>
        Validation<MonadFail, FAIL, A>.Success(Value);
    
    public IO<RT, E, A> ToIO<RT, E>()
        where RT : struct, HasIO<RT, E> =>
        IO<RT, E, A>.Pure(Value);
    
    public Eff<RT, A> ToEff<RT>()
        where RT : struct, HasIO<RT, Error> =>
        Eff<RT, A>.Success(Value);
    
    public Eff<A> ToEff() =>
        Eff<A>.Success(Value);
}
