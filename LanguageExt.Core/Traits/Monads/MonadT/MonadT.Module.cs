namespace LanguageExt.Traits;

public static partial class MonadT
{
    /// <summary>
    /// Lift a monad into a transformer
    /// </summary>
    /// <param name="ma">Monad to lift</param>
    /// <typeparam name="MTran">Transformer</typeparam>
    /// <typeparam name="M">Monad</typeparam>
    /// <typeparam name="A">Bound value</typeparam>
    /// <returns>Monad transformer with the monad lifted into it</returns>
    public static K<MTran, A> lift<MTran, M, A>(K<M, A> ma)
        where M : Monad<M>
        where MTran : MonadT<MTran, M> =>
        MTran.Lift(ma);
}
