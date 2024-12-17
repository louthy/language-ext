using System;
using LanguageExt.Traits;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using static LanguageExt.Pipes.Proxy;
using static LanguageExt.Prelude;

namespace LanguageExt.Pipes;

/// <summary>
/// Consumers both can only be `awaiting` 
/// </summary>
/// <remarks>
/// 
///       Upstream | Downstream
///           +---------+
///           |         |
///     Unit〈==      〈== Unit
///           |         |
///      IN  ==〉      ==〉Void
///           |    |    |
///           +----|----+
///                |
///                A
/// </remarks>
public static class Consumer
{
    /// <summary>
    /// Monad return / pure
    /// </summary>
    [Pure, MethodImpl(mops)]
    public static Consumer<A, M, R> Pure<A, M, R>(R value)
        where M : Monad<M> =>
        new Pure<Unit, A, Unit, Void, M, R>(value).ToConsumer();
        
    /// <summary>
    /// Wait for a value from upstream (whilst in a consumer)
    /// </summary>
    [Pure, MethodImpl(mops)]
    public static Consumer<A, M, A> awaiting<M, A>() 
        where M : Monad<M> =>
        request<Unit, A, Unit, Void, M>(unit).ToConsumer();

    /// <summary>
    /// Lift the IO monad into the Consumer monad transformer (a specialism of the Proxy monad transformer)
    /// </summary>
    [Pure, MethodImpl(mops)]
    public static Consumer<A, M, R> lift<A, M, R>(K<M, R> ma)
        where M : Monad<M>  =>
        lift<Unit, A, Unit, Void, M, R>(ma).ToConsumer();

    /// <summary>
    /// Lift the IO monad into the Consumer monad transformer (a specialism of the Proxy monad transformer)
    /// </summary>
    [Pure, MethodImpl(mops)]
    public static Consumer<A, M, R> liftIO<A, M, R>(IO<R> ma) 
        where M : Monad<M> =>
        liftIO<Unit, A, Unit, Void, M, R>(ma).ToConsumer();
        
    /// <summary>
    /// Consume all values using a monadic function
    /// </summary>
    [Pure, MethodImpl(mops)]
    public static Consumer<A, M, R> mapM<A, M, R>(Func<A, K<M, Unit>> f) 
        where M : Monad<M> =>
        cat<A, M, R>().ForEach(a => lift<A, M, Unit>(f(a)));
        
    /// <summary>
    /// Consume all values using a monadic function
    /// </summary>
    [Pure, MethodImpl(mops)]
    public static Consumer<A, M, R> mapM<A, M, R>(Func<A, IO<Unit>> f) 
        where M : Monad<M> =>
        cat<A, M, R>().ForEach(a => lift<A, M, Unit>(M.LiftIO(f(a))));
    
}
