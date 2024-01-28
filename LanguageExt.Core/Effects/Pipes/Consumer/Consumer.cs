using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using LanguageExt.Common;
using LanguageExt.Effects.Traits;
using static LanguageExt.Pipes.Proxy;
using static LanguageExt.Prelude;

namespace LanguageExt.Pipes;

/// <summary>
/// Consumers both can only be `awaiting` 
/// </summary>
/// <remarks>
///       Upstream | Downstream
///           +---------+
///           |         |
///     Unit <==       <== Unit
///           |         |
///      IN  ==>       ==> Void
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
    public static Consumer<RT, A, R> Pure<RT, A, R>(R value) where RT : HasIO<RT, Error> =>
        new Pure<RT, Unit, A, Unit, Void, R>(value).ToConsumer();
        
    /// <summary>
    /// Wait for a value from upstream (whilst in a consumer)
    /// </summary>
    /// <remarks>
    /// This is the simpler version (fewer generic arguments required) of `await` that works
    /// for consumers.  In pipes, use `Pipe.await`
    /// </remarks>
    [Pure, MethodImpl(mops)]
    public static Consumer<RT, A, A> awaiting<RT, A>() where RT : HasIO<RT, Error> =>
        request<RT, Unit, A, Unit, Void>(unit).ToConsumer();

    /// <summary>
    /// Lift the IO monad into the Consumer monad transformer (a specialism of the Proxy monad transformer)
    /// </summary>
    [Pure, MethodImpl(mops)]
    public static Consumer<RT, A, R> lift<RT, A, R>(Eff<R> ma) where RT : HasIO<RT, Error> =>
        lift<RT, Unit, A, Unit, Void, R>(ma).ToConsumer();

    /// <summary>
    /// Lift the IO monad into the Consumer monad transformer (a specialism of the Proxy monad transformer)
    /// </summary>
    [Pure, MethodImpl(mops)]
    public static Consumer<RT, A, R> lift<RT, A, R>(Eff<RT, R> ma) where RT : HasIO<RT, Error> =>
        lift<RT, Unit, A, Unit, Void, R>(ma).ToConsumer();

    /// <summary>
    /// Lift the IO monad into the Consumer monad transformer (a specialism of the Proxy monad transformer)
    /// </summary>
    [Pure, MethodImpl(mops)]
    public static Consumer<RT, A, Unit> lift<RT, A>(Eff<RT, Unit> ma) where RT : HasIO<RT, Error> =>
        lift<RT, Unit, A, Unit, Void, Unit>(ma).ToConsumer();

    /// <summary>
    /// Lift the IO monad into the Consumer monad transformer (a specialism of the Proxy monad transformer)
    /// </summary>
    [Pure, MethodImpl(mops)]
    public static Consumer<RT, A, R> lift<RT, A, R>(Transducer<RT, R> ma) where RT : HasIO<RT, Error> =>
        lift<RT, Unit, A, Unit, Void, R>(ma).ToConsumer();

    /// <summary>
    /// Lift the IO monad into the Consumer monad transformer (a specialism of the Proxy monad transformer)
    /// </summary>
    [Pure, MethodImpl(mops)]
    public static Consumer<RT, A, R> lift<RT, A, R>(Transducer<RT, Sum<Error, R>> ma) where RT : HasIO<RT, Error> =>
        lift<RT, Unit, A, Unit, Void, R>(ma).ToConsumer();

    /// <summary>
    /// Lift the IO monad into the Consumer monad transformer (a specialism of the Proxy monad transformer)
    /// </summary>
    [Pure, MethodImpl(mops)]
    public static Consumer<RT, A, R> lift<RT, A, R>(Transducer<Unit, R> ma) where RT : HasIO<RT, Error> =>
        lift<RT, Unit, A, Unit, Void, R>(ma).ToConsumer();

    /// <summary>
    /// Lift the IO monad into the Consumer monad transformer (a specialism of the Proxy monad transformer)
    /// </summary>
    [Pure, MethodImpl(mops)]
    public static Consumer<RT, A, R> lift<RT, A, R>(Transducer<Unit, Sum<Error, R>> ma) where RT : HasIO<RT, Error> =>
        lift<RT, Unit, A, Unit, Void, R>(ma).ToConsumer();
        
    /// <summary>
    /// Consume all values using a monadic function
    /// </summary>
    [Pure, MethodImpl(mops)]
    public static Consumer<RT, A, R> mapM<RT, A, R>(Func<A, Eff<RT, Unit>> f) where RT : HasIO<RT, Error> =>
        cat<RT, A, R>().ForEach(a => lift<RT, A>(f(a)));
        
    /// <summary>
    /// Consume all values using a monadic function
    /// </summary>
    [Pure, MethodImpl(mops)]
    public static Consumer<RT, A, Unit> mapM<RT, A>(Func<A, Eff<RT, Unit>> f) where RT : HasIO<RT, Error> =>
        cat<RT, A, Unit>().ForEach(a => lift<RT, A>(f(a)));
        
    /// <summary>
    /// Consume all values using a monadic function
    /// </summary>
    [Pure, MethodImpl(mops)]
    public static Consumer<RT, A, R> mapM<RT, A, R>(Func<A, Eff<Unit>> f) where RT : HasIO<RT, Error> =>
        cat<RT, A, R>().ForEach(a => lift<RT, A, Unit>(f(a)));

    /// <summary>
    /// Consume all values using a monadic function
    /// </summary>
    [Pure, MethodImpl(mops)]
    public static Consumer<RT, A, Unit> mapM<RT, A>(Func<A, Eff<Unit>> f) where RT : HasIO<RT, Error> =>
        cat<RT, A, Unit>().ForEach(a => lift<RT, A, Unit>(f(a)));
    
    /// <summary>
    /// Consume all values using a monadic function
    /// </summary>
    [Pure, MethodImpl(mops)]
    public static Consumer<RT, A, R> mapM<RT, A, R>(Func<A, Transducer<RT, Unit>> f) where RT : HasIO<RT, Error> =>
        cat<RT, A, R>().ForEach(a => lift<RT, A>(f(a)));
    
    /// <summary>
    /// Consume all values using a monadic function
    /// </summary>
    [Pure, MethodImpl(mops)]
    public static Consumer<RT, A, R> mapM<RT, A, R>(Func<A, Transducer<RT, Sum<Error, Unit>>> f) where RT : HasIO<RT, Error> =>
        cat<RT, A, R>().ForEach(a => lift<RT, A>(f(a)));
    
    /// <summary>
    /// Consume all values using a monadic function
    /// </summary>
    [Pure, MethodImpl(mops)]
    public static Consumer<RT, A, R> mapM<RT, A, R>(Func<A, Transducer<Unit, Unit>> f) where RT : HasIO<RT, Error> =>
        cat<RT, A, R>().ForEach(a => lift<RT, A>(f(a)));
    
    /// <summary>
    /// Consume all values using a monadic function
    /// </summary>
    [Pure, MethodImpl(mops)]
    public static Consumer<RT, A, R> mapM<RT, A, R>(Func<A, Transducer<Unit, Sum<Error, Unit>>> f) where RT : HasIO<RT, Error> =>
        cat<RT, A, R>().ForEach(a => lift<RT, A>(f(a)));
    
}
