using System;
using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt;

public class TryT
{
    /// <summary>
    /// Lift a pure success value into the monad-transformer
    /// </summary>
    /// <param name="value">Value to lift</param>
    /// <returns>`TryT`</returns>
    public static TryT<M, A> Succ<M, A>(A value) 
        where M : Monad<M> => 
        lift(M.Pure(value));

    /// <summary>
    /// Lift a fail value into the monad-transformer
    /// </summary>
    /// <param name="value">Value to lift</param>
    /// <returns>`TryT`</returns>
    public static TryT<M, A> Fail<M, A>(Error value)  
        where M : Monad<M> => 
        lift<M, A>(Fin.Fail<A>(value));
    
    /// <summary>
    /// Lifts a given monad into the transformer
    /// </summary>
    /// <param name="ma">Monad to lift</param>
    /// <returns>`TryT`</returns>
    public static TryT<M, A> lift<M, A>(K<M, A> ma) 
        where M : Monad<M> => 
        new(M.Map(Try.Succ, ma));

    /// <summary>
    /// Lifts a given lazy-value into the transformer
    /// </summary>
    /// <param name="f">Lazy value to lift</param>
    /// <returns>`TryT`</returns>
    public static TryT<M, A> lift<M, A>(Func<Fin<A>> f)  
        where M : Monad<M> => 
        new(M.Pure(Try.lift(f)));

    /// <summary>
    /// Lifts a given value into the transformer
    /// </summary>
    /// <param name="ma">Value to lift</param>
    /// <returns>`TryT`</returns>
    public static TryT<M, A> lift<M, A>(Fin<A> ma)  
        where M : Monad<M> => 
        new(M.Pure(Try.lift(ma)));

    /// <summary>
    /// Lifts a given value into the transformer
    /// </summary>
    /// <param name="ma">Value to lift</param>
    /// <returns>`TryT`</returns>
    public static TryT<M, A> lift<M, A>(Pure<A> ma)  
        where M : Monad<M> => 
        Succ<M, A>(ma.Value);

    /// <summary>
    /// Lifts a given value into the transformer
    /// </summary>
    /// <param name="ma">Value to lift</param>
    /// <returns>`TryT`</returns>
    public static TryT<M, A> lift<M, A>(Fail<Error> ma)  
        where M : Monad<M> => 
        lift<M, A>(Fin.Fail<A>(ma.Value));

    /// <summary>
    /// Lifts a given `IO` monad into the `TryT` transformer
    /// </summary>
    /// <param name="ma">IO monad to lift</param>
    /// <returns>`TryT`</returns>
    public static TryT<M, A> liftIO<M, A>(IO<A> ma)   
        where M : MonadIO<M> => 
        new(M.LiftIO(ma.Try().Run()).Map(Try.lift));

    /// <summary>
    /// Lifts a given `IO` monad into the `TryT` transformer
    /// </summary>
    /// <param name="ma">IO monad to lift</param>
    /// <returns>`TryT`</returns>
    internal static TryT<M, A> liftIOMaybe<M, A>(IO<A> ma)   
        where M : Monad<M> => 
        new(M.LiftIOMaybe(ma.Try().Run()).Map(Try.lift));
}
