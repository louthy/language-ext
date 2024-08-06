namespace LanguageExt.Traits;

/// <summary>
/// Delegate that takes a monad and returns an IO monad, if `MonadIO<M>.WithRunInIO` is implemented
/// </summary>
/// <typeparam name="M">Monad trait</typeparam>
/// <typeparam name="A">Bound value type</typeparam>
public delegate IO<A> UnliftIO<out M, A>(K<M, A> ma)
    where M : Monad<M>;
    
