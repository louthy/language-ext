using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using LanguageExt.Interfaces;
using LanguageExt.Pipes;
using static LanguageExt.Pipes.Proxy;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    public static class Pipe
    {
        /// <summary>
        /// Monad return / pure
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Pipe<Env, A, B, R> Pure<Env, A, B, R>(R value) where Env : struct, HasCancel<Env> =>
            new Pure<Env, Unit, A, Unit, B, R>(value).ToPipe();
        
        /// <summary>
        /// Wait for a value from upstream (whilst in a pipe)
        /// </summary>
        /// <remarks>
        /// This is the version of `await` that works for pipes.  In consumers, use `await`
        /// </remarks>
        [Pure, MethodImpl(Proxy.mops)]
        public static Pipe<Env, A, Y, A> awaiting<Env, A, Y>() where Env : struct, HasCancel<Env> =>
            request<Env, Unit, A, Unit, Y>(unit).ToPipe();
        
        /// <summary>
        /// Send a value downstream (whilst in a pipe)
        /// </summary>
        /// <remarks>
        /// This is the version of `yield` that works for pipes.  In producers, use `yield`
        /// </remarks>
        [Pure, MethodImpl(Proxy.mops)]
        public static Pipe<Env, X, A, Unit> yield<Env, X, A>(A value) where Env : struct, HasCancel<Env> =>
            respond<Env, Unit, X, Unit, A>(value).ToPipe();
        
        /// <summary>
        /// Resource management 
        /// </summary>
        /// <param name="Acq">Acquires the resource</param>
        /// <param name="Rel">Releases the resource</param>
        /// <param name="Use">Uses the resource</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="B">Value to produce</typeparam>
        /// <typeparam name="H">Type of resource to acquire</typeparam>
        /// <typeparam name="R">Return value of the Use operation</typeparam>
        /// <returns>Producer</returns>
        [Pure, MethodImpl(Proxy.mops)]
        public static Pipe<Env, A, B, R> use<Env, A, B, H, R>(
            IO<H> Acq,
            Func<H, Unit> Rel,
            Func<H, Pipe<Env, A, B, R>> Use) where Env : struct, HasCancel<Env> =>
                PipesInternal.Use(Acq, Use, Rel);

        /// <summary>
        /// Resource management 
        /// </summary>
        /// <param name="Acq">Acquires the resource</param>
        /// <param name="Use">Uses the resource</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="B">Value to produce</typeparam>
        /// <typeparam name="H">Type of resource to acquire</typeparam>
        /// <typeparam name="R">Return value of the Use operation</typeparam>
        /// <returns>Producer</returns>
        [Pure, MethodImpl(Proxy.mops)]
        public static Pipe<Env, A, B, R> use<Env, A, B, H, R>(
            IO<H> Acq,
            Func<H, Pipe<Env, A, B, R>> Use) 
            where Env : struct, HasCancel<Env>
            where H : IDisposable =>
                PipesInternal.Use(Acq, Use, PipesInternal.Dispose);

        /// <summary>
        /// Resource management 
        /// </summary>
        /// <param name="Acq">Acquires the resource</param>
        /// <param name="Use">Uses the resource</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="B">Value to produce</typeparam>
        /// <typeparam name="H">Type of resource to acquire</typeparam>
        /// <typeparam name="R">Return value of the Use operation</typeparam>
        /// <returns>Producer</returns>
        [Pure, MethodImpl(Proxy.mops)]
        public static Pipe<Env, A, B, R> use<Env, A, B, H, R>(
            SIO<H> Acq,
            Func<H, Pipe<Env, A, B, R>> Use) 
            where Env : struct, HasCancel<Env>
            where H : IDisposable =>
                PipesInternal.Use(Acq.ToAsync(), Use, PipesInternal.Dispose);
        
        /// <summary>
        /// Resource management 
        /// </summary>
        /// <param name="Acq">Acquires the resource</param>
        /// <param name="Use">Uses the resource</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="B">Value to produce</typeparam>
        /// <typeparam name="H">Type of resource to acquire</typeparam>
        /// <typeparam name="R">Return value of the Use operation</typeparam>
        /// <returns>Producer</returns>
        [Pure, MethodImpl(Proxy.mops)]
        public static Pipe<Env, A, B, R> use<Env, A, B, H, R>(
            IO<Env, H> Acq,
            Func<H, Pipe<Env, A, B, R>> Use) 
            where Env : struct, HasCancel<Env>
            where H : IDisposable =>
                PipesInternal.Use(Acq, Use, PipesInternal.Dispose);

        /// <summary>
        /// Resource management 
        /// </summary>
        /// <param name="Acq">Acquires the resource</param>
        /// <param name="Use">Uses the resource</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="B">Value to produce</typeparam>
        /// <typeparam name="H">Type of resource to acquire</typeparam>
        /// <typeparam name="R">Return value of the Use operation</typeparam>
        /// <returns>Producer</returns>
        [Pure, MethodImpl(Proxy.mops)]
        public static Pipe<Env, A, B, R> use<Env, A, B, H, R>(
            SIO<Env, H> Acq,
            Func<H, Pipe<Env, A, B, R>> Use) 
            where Env : struct, HasCancel<Env>
            where H : IDisposable =>
                PipesInternal.Use(Acq.ToAsync(), Use, PipesInternal.Dispose);

        /// <summary>
        /// Only forwards values that satisfy the predicate.
        /// </summary>
        public static Pipe<Env, A, A, Unit> filter<Env, A>(Func<A, bool> f)  where Env : struct, HasCancel<Env> =>
            Proxy.cat<Env, A, Unit>().For(a => f(a)
                    ? Pipe.yield<Env, A, A>(a)
                    : Pipe.Pure<Env, A, A, Unit>(default))
                .ToPipe();

        /// <summary>
        /// Map the output of the pipe (not the bound value as is usual with Map)
        /// </summary>
        public static Pipe<Env, A, B, R> map<Env, A, B, R>(Func<A, B> f) where Env : struct, HasCancel<Env> =>
            Proxy.cat<Env, A, R>().For(a => Pipe.yield<Env, A, B>(f(a))).ToPipe();

        /// <summary>
        /// Map the output of the pipe (not the bound value as is usual with Map)
        /// </summary>
        public static Pipe<Env, A, A, R> map<Env, A, R>(Func<A, A> f) where Env : struct, HasCancel<Env> =>
            Proxy.cat<Env, A, R>().For(a => Pipe.yield<Env, A, A>(f(a))).ToPipe();

        /// <summary>
        /// Apply a monadic function to all values flowing downstream (not the bound value as is usual with Map)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Pipe<Env, A, B, R> mapM<Env, A, B, R>(Func<A, IO<Env, B>> f) where Env : struct, HasCancel<Env> =>
            Proxy.cat<Env, A, R>()
                 .For<Env, A, A, B, R>(a => Pipe.liftIO<Env, A, B, B>(f(a))
                                                .Bind(Pipe.yield<Env, A, B>)
                 .ToPipe());

        /// <summary>
        /// Apply a monadic function to all values flowing downstream (not the bound value as is usual with Map)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Pipe<Env, A, A, Unit> mapM<Env, A>(Func<A, IO<Env, A>> f) where Env : struct, HasCancel<Env> =>
            Proxy.cat<Env, A, Unit>()
                .For<Env, A, A, A, Unit>(a => Pipe.liftIO<Env, A, A, A>(f(a))
                    .Bind(Pipe.yield<Env, A, A>)
                    .ToPipe());

        /// <summary>
        /// Apply a monadic function to all values flowing downstream (not the bound value as is usual with Map)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Pipe<Env, A, B, R> mapM<Env, A, B, R>(Func<A, IO<B>> f) where Env : struct, HasCancel<Env> =>
            Proxy.cat<Env, A, R>()
                 .For<Env, A, A, B, R>(a => Pipe.liftIO<Env, A, B, B>(f(a))
                                                .Bind(Pipe.yield<Env, A, B>)
                 .ToPipe());
        
        /// <summary>
        /// Apply a monadic function to all values flowing downstream (not the bound value as is usual with Map)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Pipe<Env, A, B, R> mapM<Env, A, B, R>(Func<A, SIO<Env, B>> f) where Env : struct, HasCancel<Env> =>
            Proxy.cat<Env, A, R>()
                .For<Env, A, A, B, R>(a => Pipe.liftIO<Env, A, B, B>(f(a))
                    .Bind(Pipe.yield<Env, A, B>)
                    .ToPipe());
        
        /// <summary>
        /// Apply a monadic function to all values flowing downstream (not the bound value as is usual with Map)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Pipe<Env, A, A, Unit> mapM<Env, A>(Func<A, SIO<Env, A>> f) where Env : struct, HasCancel<Env> =>
            Proxy.cat<Env, A, Unit>()
                .For<Env, A, A, A, Unit>(a => Pipe.liftIO<Env, A, A, A>(f(a))
                    .Bind(Pipe.yield<Env, A, A>)
                    .ToPipe());

        /// <summary>
        /// Apply a monadic function to all values flowing downstream (not the bound value as is usual with Map)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Pipe<Env, A, B, R> mapM<Env, A, B, R>(Func<A, SIO<B>> f) where Env : struct, HasCancel<Env> =>
            Proxy.cat<Env, A, R>()
                .For<Env, A, A, B, R>(a => Pipe.liftIO<Env, A, B, B>(f(a))
                    .Bind(Pipe.yield<Env, A, B>)
                    .ToPipe());

        /// <summary>
        /// Lift the IO monad into the Pipe monad transformer (a specialism of the Proxy monad transformer)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Pipe<Env, A, B, R> liftIO<Env, A, B, R>(IO<R> ma) where Env : struct, HasCancel<Env> =>
            liftIO<Env, Unit, A, Unit, B, R>(ma).ToPipe(); 
 
        /// <summary>
        /// Lift the IO monad into the Pipe monad transformer (a specialism of the Proxy monad transformer)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Pipe<Env, A, B, R> liftIO<Env, A, B, R>(SIO<R> ma) where Env : struct, HasCancel<Env> =>
            liftIO<Env, Unit, A, Unit, B, R>(ma).ToPipe(); 
 
        /// <summary>
        /// Lift the IO monad into the Pipe monad transformer (a specialism of the Proxy monad transformer)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Pipe<Env, A, B, R> liftIO<Env, A, B, R>(IO<Env, R> ma) where Env : struct, HasCancel<Env> =>
            liftIO<Env, Unit, A, Unit, B, R>(ma).ToPipe(); 
 
        /// <summary>
        /// Lift the IO monad into the Pipe monad transformer (a specialism of the Proxy monad transformer)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Pipe<Env, A, B, R> liftIO<Env, A, B, R>(SIO<Env, R> ma) where Env : struct, HasCancel<Env> =>
            liftIO<Env, Unit, A, Unit, B, R>(ma).ToPipe(); 
    }
}
