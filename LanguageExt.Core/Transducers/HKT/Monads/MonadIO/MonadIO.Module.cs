namespace LanguageExt.HKT;

public class MonadIO
{
    /// <summary>
    /// Embeds the `IO` monad into the `M<A>` monad.  NOTE: This will fail if the monad transformer
    /// stack doesn't have an `IO` monad as its inner-most monad.
    /// </summary>
    public static K<M, A> liftIO<M, A>(IO<A> ma) 
        where M : MonadIO<M> =>
        M.LiftIO(ma);

    /// <summary>
    /// This throws an error: `Errors.IONotInTransformerStack`.  It's used as a default implementation
    /// of `liftIO` for non-IO, non-transformer monads to indicate that the `liftIO` operation can't
    /// succeed because we've hit the inner most monad and the monad isn't the IO monad.
    /// </summary>
    /// <remarks>
    /// This is entirely due to the lack of true ad-hoc polymorphism in C#.  I
    /// </remarks>
    public static K<M, A> liftNoIO<M, A>(IO<A> ma)
        where M : MonadIO<M> =>
        M.LiftNoIO(ma);
}
