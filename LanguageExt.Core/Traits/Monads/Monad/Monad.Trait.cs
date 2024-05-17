using System;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.Traits;

/// <summary>
/// Monad trait
/// </summary>
/// <typeparam name="M">Self referring trait</typeparam>
public interface Monad<M> : Applicative<M> 
    where M : Monad<M>
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Abstract members
    //
    
    public static abstract K<M, B> Bind<A, B>(K<M, A> ma, Func<A, K<M, B>> f);

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Default implementations
    //
    
    public static virtual K<M, A> Flatten<A>(K<M, K<M, A>> mma) =>
        M.Bind(mma, identity);
    
    public new static virtual K<M, B> Map<A, B>(Func<A, B> f, K<M, A> ma) =>
        M.Bind(ma, x => M.Pure(f(x)));

    /// <summary>
    /// Lifts the IO monad into a monad transformer stack.  
    /// </summary>
    /// <remarks>
    /// If this method isn't overloaded in the inner monad or any monad in the
    /// stack on the way to the inner monad then it will throw an exception.
    ///
    /// This isn't ideal - however it appears to be the only way to achieve this
    /// kind of functionality in C# without resorting to magic. 
    /// </remarks>
    /// <param name="ma">IO computation to lift</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>The outer monad with the IO monad lifted into it</returns>
    /// <exception cref="ExceptionalException">If this method isn't overloaded in
    /// the inner monad or any monad in the stack on the way to the inner monad
    /// then it will throw an exception.</exception>
    public static virtual K<M, A> LiftIO<A>(IO<A> ma) =>
        throw new ExceptionalException(Errors.IONotInTransformerStack);

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
    public static virtual K<M, Func<K<M, A>, IO<A>>> UnliftIO<A>() =>
        M.WithRunInIO((Func<K<M, A>, IO<A>> run) => IO.Pure(run));
    
    /// <summary>
    /// Unlifts the IO monad from the monad transformer stack.  
    /// </summary>
    /// <remarks>
    /// If this method isn't overloaded in the inner monad or any monad in the
    /// stack on the way to the inner monad then it will throw an exception.
    ///
    /// This isn't ideal - however it appears to be the only way to achieve this
    /// kind of functionality in C# without resorting to magic. 
    /// </remarks>
    /// <param name="mma">IO computation to lift</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>The outer monad with the IO monad lifted into it</returns>
    /// <exception cref="ExceptionalException">If this method isn't overloaded in
    /// the inner monad or any monad in the stack on the way to the inner monad
    /// then it will throw an exception.</exception>
    public static virtual K<M, B> WithRunInIO<A, B>(Func<Func<K<M, A>, IO<A>>, IO<B>> inner) =>
        throw new ExceptionalException(Errors.UnliftIONotSupported);
}
