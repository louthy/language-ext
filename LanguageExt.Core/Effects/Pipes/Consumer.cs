using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using LanguageExt.Effects.Traits;
using static LanguageExt.Pipes.Proxy;
using static LanguageExt.Prelude;

namespace LanguageExt.Pipes
{
    public static class Consumer
    {
        /// <summary>
        /// Monad return / pure
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<RT, A, R> Pure<RT, A, R>(R value) where RT : struct, HasCancel<RT> =>
            new Pure<RT, Unit, A, Unit, Void, R>(value).ToConsumer();
        
        /// <summary>
        /// Wait for a value from upstream (whilst in a consumer)
        /// </summary>
        /// <remarks>
        /// This is the simpler version (fewer generic arguments required) of `await` that works
        /// for consumers.  In pipes, use `Pipe.await`
        /// </remarks>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<RT, A, A> awaiting<RT, A>() where RT : struct, HasCancel<RT> =>
            request<RT, Unit, A, Unit, Void>(unit).ToConsumer();

        
        /// <summary>
        /// Lift the IO monad into the Consumer monad transformer (a specialism of the Proxy monad transformer)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<RT, A, R> lift<RT, A, R>(Aff<R> ma) where RT : struct, HasCancel<RT> =>
            lift<RT, Unit, A, Unit, Void, R>(ma).ToConsumer();

        /// <summary>
        /// Lift the IO monad into the Consumer monad transformer (a specialism of the Proxy monad transformer)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<RT, A, R> lift<RT, A, R>(Eff<R> ma) where RT : struct, HasCancel<RT> =>
            lift<RT, Unit, A, Unit, Void, R>(ma).ToConsumer();

        /// <summary>
        /// Lift the IO monad into the Consumer monad transformer (a specialism of the Proxy monad transformer)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<RT, A, R> lift<RT, A, R>(Aff<RT, R> ma) where RT : struct, HasCancel<RT> =>
            lift<RT, Unit, A, Unit, Void, R>(ma).ToConsumer();

        /// <summary>
        /// Lift the IO monad into the Consumer monad transformer (a specialism of the Proxy monad transformer)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<RT, A, R> lift<RT, A, R>(Eff<RT, R> ma) where RT : struct, HasCancel<RT> =>
            lift<RT, Unit, A, Unit, Void, R>(ma).ToConsumer();


        /// <summary>
        /// Lift the IO monad into the Consumer monad transformer (a specialism of the Proxy monad transformer)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<RT, A, Unit> lift<RT, A>(Aff<RT, Unit> ma) where RT : struct, HasCancel<RT> =>
            lift<RT, Unit, A, Unit, Void, Unit>(ma).ToConsumer();

        /// <summary>
        /// Lift the IO monad into the Consumer monad transformer (a specialism of the Proxy monad transformer)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<RT, A, Unit> lift<RT, A>(Eff<RT, Unit> ma) where RT : struct, HasCancel<RT> =>
            lift<RT, Unit, A, Unit, Void, Unit>(ma).ToConsumer();

        
        
        /// <summary>
        /// Lift am IO monad into the `Proxy` monad transformer
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<RT, IN, R> use<RT, IN, R>(Aff<R> ma) 
            where RT : struct, HasCancel<RT>
            where R : IDisposable =>
            use<RT, Unit, IN, Unit, Void, R>(ma).ToConsumer();         

        /// <summary>
        /// Lift am IO monad into the `Proxy` monad transformer
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<RT, IN, R> use<RT, IN, R>(Eff<R> ma) 
            where RT : struct, HasCancel<RT>
            where R : IDisposable =>
            use<RT, Unit, IN, Unit, Void, R>(ma).ToConsumer();         

        /// <summary>
        /// Lift am IO monad into the `Proxy` monad transformer
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<RT, IN, R> use<RT, IN, R>(Aff<RT, R> ma) 
            where RT : struct, HasCancel<RT> 
            where R : IDisposable =>
            use<RT, Unit, IN, Unit, Void, R>(ma).ToConsumer();         

        /// <summary>
        /// Lift am IO monad into the `Proxy` monad transformer
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<RT, IN, R> use<RT, IN, R>(Eff<RT, R> ma) 
            where RT : struct, HasCancel<RT> 
            where R : IDisposable =>
            use<RT, Unit, IN, Unit, Void, R>(ma).ToConsumer();         
        
        
        /// <summary>
        /// Lift am IO monad into the `Proxy` monad transformer
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<RT, IN, R> use<RT, IN, R>(Aff<R> ma, Func<R, Unit> dispose) 
            where RT : struct, HasCancel<RT> =>
            use<RT, Unit, IN, Unit, Void, R>(ma, dispose).ToConsumer();         

        /// <summary>
        /// Lift am IO monad into the `Proxy` monad transformer
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<RT, IN, R> use<RT, IN, R>(Eff<R> ma, Func<R, Unit> dispose) 
            where RT : struct, HasCancel<RT> =>
            use<RT, Unit, IN, Unit, Void, R>(ma, dispose).ToConsumer();         

        /// <summary>
        /// Lift am IO monad into the `Proxy` monad transformer
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<RT, IN, R> use<RT, IN, R>(Aff<RT, R> ma, Func<R, Unit> dispose) 
            where RT : struct, HasCancel<RT>  =>
            use<RT, Unit, IN, Unit, Void, R>(ma, dispose).ToConsumer();         

        /// <summary>
        /// Lift am IO monad into the `Proxy` monad transformer
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<RT, IN, R> use<RT, IN, R>(Eff<RT, R> ma, Func<R, Unit> dispose) 
            where RT : struct, HasCancel<RT>  =>
            use<RT, Unit, IN, Unit, Void, R>(ma, dispose).ToConsumer();         

        /// <summary>
        /// Release a previously used resource
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<RT, IN, Unit> release<RT, IN, R>(R dispose) 
            where RT : struct, HasCancel<RT> =>
            Proxy.release<RT, Unit, IN, Unit, Void, R>(dispose).ToConsumer();
        
        /// <summary>
        /// Consume all values using a monadic function
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<RT, A, R> mapM<RT, A, R>(Func<A, Aff<RT, Unit>> f) where RT : struct, HasCancel<RT> =>
            Proxy.cat<RT, A, R>()
                 .For<RT, A, A, R>(a => lift<RT, A>(f(a)));

        /// <summary>
        /// Consume all values using a monadic function
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<RT, A, Unit> mapM<RT, A>(Func<A, Aff<RT, Unit>> f) where RT : struct, HasCancel<RT> =>
            Proxy.cat<RT, A, Unit>()
                 .For<RT, A, A, Unit>(a => lift<RT, A>(f(a)));

        /// <summary>
        /// Consume all values using a monadic function
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<RT, A, R> mapM<RT, A, R>(Func<A, Eff<RT, Unit>> f) where RT : struct, HasCancel<RT> =>
            Proxy.cat<RT, A, R>()
                 .For<RT, A, A, R>(a => lift<RT, A>(f(a)));
        
        /// <summary>
        /// Consume all values using a monadic function
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<RT, A, Unit> mapM<RT, A>(Func<A, Eff<RT, Unit>> f) where RT : struct, HasCancel<RT> =>
            Proxy.cat<RT, A, Unit>()
                 .For<RT, A, A, Unit>(a => lift<RT, A>(f(a)));
        
        

        /// <summary>
        /// Consume all values using a monadic function
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<RT, A, R> mapM<RT, A, R>(Func<A, Aff<Unit>> f) where RT : struct, HasCancel<RT> =>
            Proxy.cat<RT, A, R>()
                 .For<RT, A, A, R>(a => lift<RT, A, Unit>(f(a)));

        /// <summary>
        /// Consume all values using a monadic function
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<RT, A, Unit> mapM<RT, A>(Func<A, Aff<Unit>> f) where RT : struct, HasCancel<RT> =>
            Proxy.cat<RT, A, Unit>()
                 .For<RT, A, A, Unit>(a => lift<RT, A, Unit>(f(a)));

        /// <summary>
        /// Consume all values using a monadic function
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<RT, A, R> mapM<RT, A, R>(Func<A, Eff<Unit>> f) where RT : struct, HasCancel<RT> =>
            Proxy.cat<RT, A, R>()
                 .For<RT, A, A, R>(a => lift<RT, A, Unit>(f(a)));

        /// <summary>
        /// Consume all values using a monadic function
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<RT, A, Unit> mapM<RT, A>(Func<A, Eff<Unit>> f) where RT : struct, HasCancel<RT> =>
            Proxy.cat<RT, A, Unit>()
                 .For<RT, A, A, Unit>(a => lift<RT, A, Unit>(f(a)));        
        
        /// <summary>
        /// Creates a consumer that returns the result of running either the left or right effect
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<RT, IN, Either<A, B>> sequence<RT, IN, A, B>(Either<Effect<RT, A>, Effect<RT, B>> ms) where RT : struct, HasCancel<RT> =>
            Consumer.lift<RT, IN, Either<A, B>>(
                ms.Match(
                    Left: l => l.RunEffect().Map(Left<A, B>),
                    Right: r => r.RunEffect().Map(Right<A, B>)));

        /// <summary>
        /// Creates a consumer that returns the result of the effects
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<RT, IN, (A, B)> sequence<RT, IN, A, B>((Effect<RT, A>, Effect<RT, B>) ms) where RT : struct, HasCancel<RT> =>
            Consumer.lift<RT, IN, (A, B)>((ms.Item1.RunEffect(), ms.Item2.RunEffect()).Sequence());
        
        /// <summary>
        /// Creates a consumer that returns the result of the effects
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<RT, IN, (A, B, C)> sequence<RT, IN, A, B, C>((Effect<RT, A>, Effect<RT, B>, Effect<RT, C>) ms) where RT : struct, HasCancel<RT> =>
            Consumer.lift<RT, IN, (A, B, C)>((ms.Item1.RunEffect(), ms.Item2.RunEffect(), ms.Item3.RunEffect()).Sequence());

        /// <summary>
        /// Creates a consumer that returns the result of the effects
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<RT, IN, (A, B, C, D)> sequence<RT, IN, A, B, C, D>((Effect<RT, A>, Effect<RT, B>, Effect<RT, C>, Effect<RT, D>) ms) where RT : struct, HasCancel<RT> =>
            Consumer.lift<RT, IN, (A, B, C, D)>((ms.Item1.RunEffect(), ms.Item2.RunEffect(), ms.Item3.RunEffect(), ms.Item4.RunEffect()).Sequence());
    }
}
