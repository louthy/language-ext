using System;
using LanguageExt.Traits;

namespace LanguageExt;

public partial class OptionT
{
    /// <summary>
    /// Lift a pure value into the monad-transformer
    /// </summary>
    /// <param name="value">Value to lift</param>
    /// <returns>`OptionT`</returns>
    public static OptionT<M, A> Some<M, A>(A value)  
        where M : Monad<M> =>
        lift(M.Pure(value));

    /// <summary>
    /// Lift a `None` value into the monad-transformer   
    /// </summary>
    /// <returns>`OptionT`</returns>
    public static OptionT<M, A> None<M, A>()  
        where M : Monad<M> =>
        OptionT<M, A>.None;
    
    /// <summary>
    /// Lifts a given monad into the transformer
    /// </summary>
    /// <param name="monad">Monad to lift</param>
    /// <returns>`OptionT`</returns>
    public static OptionT<M, A> lift<M, A>(Pure<A> monad) 
        where M : Monad<M> =>
        Some<M, A>(monad.Value);

    /// <summary>
    /// Lifts a given monad into the transformer
    /// </summary>
    /// <param name="monad">Monad to lift</param>
    /// <returns>`OptionT`</returns>
    public static OptionT<M, A> lift<M, A>(Option<A> monad) 
        where M : Monad<M> =>
        new(M.Pure(monad));

    /// <summary>
    /// Lifts a given monad into the transformer
    /// </summary>
    /// <param name="monad">Monad to lift</param>
    /// <returns>`OptionT`</returns>
    public static OptionT<M, A> lift<M, A>(Fail<Unit> monad) 
        where M : Monad<M> =>
        lift<M, A>(Option<A>.None);

    /// <summary>
    /// Lifts a given monad into the transformer
    /// </summary>
    /// <param name="monad">Monad to lift</param>
    /// <returns>`OptionT`</returns>
    public static OptionT<M, A> lift<M, A>(K<M, A> monad) 
        where M : Monad<M> =>
        new(M.Map(Option.Some, monad));

    /// <summary>
    /// Lifts a given monad into the transformer
    /// </summary>
    /// <param name="monad">Monad to lift</param>
    /// <returns>`OptionT`</returns>
    public static OptionT<M, A> lift<M, A>(K<M, Option<A>> monad) 
        where M : Monad<M> =>
        new(monad);

    /// <summary>
    /// Lifts a given monad into the transformer
    /// </summary>
    /// <param name="monad">Monad to lift</param>
    /// <returns>`OptionT`</returns>
    public static OptionT<M, A> liftIO<M, A>(IO<A> monad) 
        where M : MonadIO<M> =>
        lift(M.LiftIO(monad));

    /// <summary>
    /// Lifts a given monad into the transformer
    /// </summary>
    /// <param name="monad">Monad to lift</param>
    /// <returns>`OptionT`</returns>
    internal static OptionT<M, A> liftIOMaybe<M, A>(IO<A> monad) 
        where M : Monad<M> =>
        lift(M.LiftIOMaybe(monad));

    /// <summary>
    /// Lifts a given monad into the transformer
    /// </summary>
    /// <param name="monad">Monad to lift</param>
    /// <returns>`OptionT`</returns>
    public static OptionT<M, A> liftIO<M, A>(IO<Option<A>> monad) 
        where M : MonadIO<M> =>
        lift(M.LiftIO(monad));
    
    /// <summary>
    /// Lifts a given monad into the transformer
    /// </summary>
    /// <param name="monad">Monad to lift</param>
    /// <returns>`OptionT`</returns>
    internal static OptionT<M, A> liftIOMaybe<M, A>(IO<Option<A>> monad) 
        where M : Monad<M> =>
        lift(M.LiftIOMaybe(monad));
}
