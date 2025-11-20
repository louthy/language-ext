using System;
using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt;

public partial class FinT
{
    /// <summary>
    /// Lift a pure value into the monad-transformer
    /// </summary>
    /// <param name="value">Value to lift</param>
    /// <returns>`FinT`</returns>
    public static FinT<M, A> Succ<M, A>(A value)  
        where M : Monad<M> =>  
        lift(M.Pure(value));

    /// <summary>
    /// Lift a fail value into the monad-transformer
    /// </summary>
    /// <param name="value">Value to lift</param>
    /// <returns>`FinT`</returns>
    public static FinT<M, A> Fail<M, A>(Error value)  
        where M : Monad<M> =>  
        lift<M, A>(Fin.Fail<A>(value));

    /// <summary>
    /// Lifts a given `Fin` value into the transformer 
    /// </summary>
    /// <param name="ma">`Fin` value</param>
    /// <returns>`FinT`</returns>
    public static FinT<M, A> lift<M, A>(Fin<A> ma)  
        where M : Monad<M> =>  
        new(M.Pure(ma));

    /// <summary>
    /// Lifts a given pure value into the transformer
    /// </summary>
    /// <param name="ma">Value to lift</param>
    /// <returns>`FinT`</returns>
    public static FinT<M, A> lift<M, A>(Pure<A> value)  
        where M : Monad<M> =>
        lift<M, A>(Fin.Succ(value.Value));

    /// <summary>
    /// Lifts a given failure value into the transformer
    /// </summary>
    /// <param name="fail">Value to lift</param>
    /// <returns>`FinT`</returns>
    public static FinT<M, A> lift<M, A>(Fail<Error> value)  
        where M : Monad<M> =>  
        lift<M, A>(Fin.Fail<A>(value.Value));

    /// <summary>
    /// Lifts a given monad into the transformer
    /// </summary>
    /// <param name="monad">Monad to lift</param>
    /// <returns>`FinT`</returns>
    public static FinT<M, A> lift<M, A>(K<M, A> monad) 
        where M : Monad<M> =>  
        new(M.Map(Fin.Succ, monad));

    /// <summary>
    /// Lifts a given monad into the transformer
    /// </summary>
    /// <param name="ma">Monad to lift</param>
    /// <returns>`FinT`</returns>
    public static FinT<M, A> lift<M, A>(K<M, Fin<A>> ma) 
        where M : Monad<M> =>  
        new(ma);

    /// <summary>
    /// Lifts a given `IO` monad into the transformer
    /// </summary>
    /// <param name="ma">Monad to lift</param>
    /// <returns>`FinT`</returns>
    public static FinT<M, A> liftIO<M, A>(IO<A> ma)
        where M : MonadIO<M> =>  
        lift(M.LiftIO(ma));

    /// <summary>
    /// Lifts a given `IO` monad into the transformer
    /// </summary>
    /// <param name="ma">Monad to lift</param>
    /// <returns>`FinT`</returns>
    public static FinT<M, A> liftIO<M, A>(IO<Fin<A>> ma)
        where M : MonadIO<M> =>  
        lift(M.LiftIO(ma));    

    /// <summary>
    /// Lifts a given `IO` monad into the transformer
    /// </summary>
    /// <param name="ma">Monad to lift</param>
    /// <returns>`FinT`</returns>
    internal static FinT<M, A> liftIOMaybe<M, A>(IO<A> ma)
        where M : Monad<M> =>  
        lift(M.LiftIOMaybe(ma));

    /// <summary>
    /// Lifts a given `IO` monad into the transformer
    /// </summary>
    /// <param name="ma">Monad to lift</param>
    /// <returns>`FinT`</returns>
    internal static FinT<M, A> liftIOMaybe<M, A>(IO<Fin<A>> ma)
        where M : Monad<M> =>  
        lift(M.LiftIOMaybe(ma));    
}
