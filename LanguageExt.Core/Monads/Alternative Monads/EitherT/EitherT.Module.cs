using LanguageExt.Traits;

namespace LanguageExt;

public class EitherT
{
    /// <summary>
    /// Lift a pure value into the monad-transformer
    /// </summary>
    /// <param name="value">Value to lift</param>
    /// <returns>`EitherT`</returns>
    public static EitherT<L, M, A> Right<L, M, A>(A value)  
        where M : Monad<M> =>
        lift<L, M, A>(M.Pure(value));

    /// <summary>
    /// Lift a fail value into the monad-transformer
    /// </summary>
    /// <param name="value">Value to lift</param>
    /// <returns>`EitherT`</returns>
    public static EitherT<L, M, A> Left<L, M, A>(L value)  
        where M : Monad<M> =>
        lift<L, M, A>(Either.Left<L, A>(value));

    /// <summary>
    /// Lifts a given `Either` value into the transformer
    /// </summary>
    /// <param name="value">Value to lift</param>
    /// <returns>`EitherT`</returns>
    public static EitherT<L, M, A> lift<L, M, A>(Either<L, A> value)  
        where M : Monad<M> =>
        new(M.Pure(value));

    /// <summary>
    /// Lifts a given monad into the transformer
    /// </summary>
    /// <param name="ma">Monad to lift</param>
    /// <returns>`EitherT`</returns>
    public static EitherT<L, M, A> lift<L, M, A>(K<M, A> ma)  
        where M : Monad<M> =>
        new(M.Map(Either.Right<L, A>, ma));

    /// <summary>
    /// Lifts a given monad into the transformer
    /// </summary>
    /// <param name="ma">Monad to lift</param>
    /// <returns>`EitherT`</returns>
    public static EitherT<L, M, A> lift<L, M, A>(K<M, Either<L, A>> ma) 
        where M : Monad<M> =>
        new(ma);

    /// <summary>
    /// Lifts a given value into the transformer
    /// </summary>
    /// <param name="pure">Value to lift</param>
    /// <returns>`EitherT`</returns>
    public static EitherT<L, M, A> lift<L, M, A>(Pure<A> ma)  
        where M : Monad<M> =>
        Right<L, M, A>(ma.Value);

    /// <summary>
    /// Lifts a given value into the transformer
    /// </summary>
    /// <param name="fail">Value to lift</param>
    /// <returns>`EitherT`</returns>
    public static EitherT<L, M, A> lift<L, M, A>(Fail<L> ma)  
        where M : Monad<M> =>
        lift<L, M, A>(Either.Left<L, A>(ma.Value));

    /// <summary>
    /// Lifts a given monad into the transformer
    /// </summary>
    /// <param name="ma">Monad to lift</param>
    /// <returns>`EitherT`</returns>
    public static EitherT<L, M, A> liftIO<L, M, A>(K<IO, A> ma)  
        where M : MonadIO<M> =>
        lift<L, M, A>(M.LiftIO(ma));

    /// <summary>
    /// Lifts a given monad into the transformer
    /// </summary>
    /// <param name="ma">Monad to lift</param>
    /// <returns>`EitherT`</returns>
    public static EitherT<L, M, A> liftIO<L, M, A>(IO<Either<L, A>> ma)  
        where M : MonadIO<M> =>
        lift(M.LiftIO(ma));

    /// <summary>
    /// Lifts a given monad into the transformer
    /// </summary>
    /// <param name="ma">Monad to lift</param>
    /// <returns>`EitherT`</returns>
    internal static EitherT<L, M, A> liftIOMaybe<L, M, A>(K<IO, A> ma)  
        where M : Monad<M> =>
        lift<L, M, A>(M.LiftIOMaybe(ma));

    /// <summary>
    /// Lifts a given monad into the transformer
    /// </summary>
    /// <param name="ma">Monad to lift</param>
    /// <returns>`EitherT`</returns>
    internal static EitherT<L, M, A> liftIOMaybe<L, M, A>(IO<Either<L, A>> ma)  
        where M : Monad<M> =>
        lift(M.LiftIOMaybe(ma));    
}
