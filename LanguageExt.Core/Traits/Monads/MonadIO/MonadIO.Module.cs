using System;
using LanguageExt.Common;

namespace LanguageExt.Traits;

/// <summary>
/// Monad module
/// </summary>
public static class MonadIO
{
    /// <summary>
    /// Embeds the `IO` monad into the `M<A>` monad.  NOTE: This will fail if the monad transformer
    /// stack doesn't have an `IO` monad as its innermost monad.
    /// </summary>
    public static K<M, A> liftIO<M, A>(IO<A> ma) 
        where M : Monad<M> =>
        M.LiftIO(ma);
    
    /// <summary>
    /// Embeds the `IO` monad into the `M<A>` monad.  NOTE: This will fail if the monad transformer
    /// stack doesn't have an `IO` monad as its innermost monad.
    /// </summary>
    public static K<M, A> liftIO<M, A>(K<IO, A> ma) 
        where M : Monad<M> =>
        M.LiftIO(ma);
    
    /// <summary>
    /// Unlifts the IO monad from the monad transformer stack.  
    /// </summary>
    /// <remarks>
    /// If the `WithRunInIO` method isn't overloaded in the inner monad or any
    /// monad in the stack on the way to the inner monad then it will throw an
    /// exception.
    ///
    /// This isn't ideal - however it appears to be the only way to achieve this
    /// kind of functionality in C# without resorting to magic. 
    /// </remarks>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>The outer monad with the IO monad lifted into it</returns>
    /// <exception cref="ExceptionalException">If the `WithRunInIO` method isn't
    /// overloaded in the inner monad or any monad in the stack on the way to the
    /// inner monad then it will throw an exception.</exception>
    public static K<M, UnliftIO<M, A>> unliftIO<M, A>()
        where M : Monad<M> =>
        M.UnliftIO<A>();

    /// <summary>
    /// Unlifts the IO monad from the monad transformer stack.  
    /// </summary>
    /// <remarks>
    /// If the `WithRunInIO` method isn't overloaded in the inner monad or any monad
    /// in the stack on the way to the inner monad then it will throw an exception.
    ///
    /// This isn't ideal - however it appears to be the only way to achieve this
    /// kind of functionality in C# without resorting to magic. 
    /// </remarks>
    /// <param name="mma">IO computation to lift</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>The outer monad with the IO monad lifted into it</returns>
    /// <exception cref="ExceptionalException">If the `WithRunInIO` method isn't
    /// overloaded in the inner monad or any monad in the stack on the way to the inner
    /// monad then it will throw an exception.</exception>
    public static K<M, B> withRunInIO<M, A, B>(
        Func<UnliftIO<M, A>, IO<B>> inner) 
        where M : Monad<M> =>
        M.WithRunInIO(inner);
    
    /// <summary>
    /// Convert an action in `M` to an action in `IO`.
    /// </summary>
    /// <param name="ma"></param>
    /// <typeparam name="M"></typeparam>
    /// <typeparam name="A"></typeparam>
    /// <returns></returns>
    public static K<M, IO<A>> toIO<M, A>(K<M, A> ma) 
        where M : Monad<M> =>
        withRunInIO<M, A, IO<A>>(run => IO.pure(run(ma)));
    
    
    /// <summary>
    /// Map the underlying IO monad
    /// </summary>
    public static K<M, B> mapIO<M, A, B>(Func<IO<A>, IO<B>> f, K<M, A> ma)
        where M : MonadIO<M>, Monad<M> =>
        M.MapIO(ma, f);

}
