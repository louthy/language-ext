using System;
using LanguageExt.Pipes;
using static LanguageExt.Prelude;
using LanguageExt.Effects.Traits;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using static LanguageExt.Pipes.Proxy;
using System.Runtime.CompilerServices;

namespace LanguageExt.Pipes
{
    /// <summary>
    /// Pipes both can both be `await` and can `yield`
    /// </summary>
    /// <remarks>
    ///       Upstream | Downstream
    ///           +---------+
    ///           |         |
    ///     Unit <==       <== Unit
    ///           |         |
    ///      IN  ==>       ==> OUT
    ///           |    |    |
    ///           +----|----+
    ///                |
    ///                A
    /// </remarks>
    public static class Pipe
    {
        /// <summary>
        /// Monad return / pure
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Pipe<RT, A, B, R> Pure<RT, A, B, R>(R value) where RT : struct, HasCancel<RT> =>
            new Pure<RT, Unit, A, Unit, B, R>(value).ToPipe();
        
        /// <summary>
        /// Wait for a value from upstream (whilst in a pipe)
        /// </summary>
        /// <remarks>
        /// This is the version of `await` that works for pipes.  In consumers, use `Consumer.await`
        /// </remarks>
        [Pure, MethodImpl(Proxy.mops)]
        public static Pipe<RT, A, Y, A> awaiting<RT, A, Y>() where RT : struct, HasCancel<RT> =>
            request<RT, Unit, A, Unit, Y>(unit).ToPipe();
        
        /// <summary>
        /// Send a value downstream (whilst in a pipe)
        /// </summary>
        /// <remarks>
        /// This is the version of `yield` that works for pipes.  In producers, use `Producer.yield`
        /// </remarks>
        [Pure, MethodImpl(Proxy.mops)]
        public static Pipe<RT, X, A, Unit> yield<RT, X, A>(A value) where RT : struct, HasCancel<RT> =>
            respond<RT, Unit, X, Unit, A>(value).ToPipe();
        
        [Pure, MethodImpl(Proxy.mops)]
        internal static Pipe<RT, IN, OUT, X> enumerate<RT, IN, OUT, X>(EnumerateData<X> xs)
            where RT : struct, HasCancel<RT> =>
            new Enumerate<RT, Unit, IN, Unit, OUT, X, X>(xs, Pipe.Pure<RT, IN, OUT, X>).ToPipe();
        
        [Pure, MethodImpl(Proxy.mops)]
        public static Pipe<RT, IN, X, X> enumerate<RT, IN, X>(IEnumerable<X> xs)
            where RT : struct, HasCancel<RT> =>
            new Enumerate<RT, Unit, IN, Unit, X, X, X>(xs, Pipe.Pure<RT, IN, X, X>).ToPipe();

        [Pure, MethodImpl(Proxy.mops)]
        public static Pipe<RT, IN, OUT, X> enumerate<RT, IN, OUT, X>(IEnumerable<X> xs)
            where RT : struct, HasCancel<RT> =>
            new Enumerate<RT, Unit, IN, Unit, OUT, X, X>(xs, Pipe.Pure<RT, IN, OUT, X>).ToPipe();
        
        [Pure, MethodImpl(Proxy.mops)]
        public static Pipe<RT, IN, X, X> enumerate<RT, IN, X>(IAsyncEnumerable<X> xs)
            where RT : struct, HasCancel<RT> =>
            new Enumerate<RT, Unit, IN, Unit, X, X, X>(xs, Pipe.Pure<RT, IN, X, X>).ToPipe();

        [Pure, MethodImpl(Proxy.mops)]
        public static Pipe<RT, IN, OUT, X> enumerate<RT, IN, OUT, X>(IAsyncEnumerable<X> xs)
            where RT : struct, HasCancel<RT> =>
            new Enumerate<RT, Unit, IN, Unit, OUT, X, X>(xs, Pipe.Pure<RT, IN, OUT, X>).ToPipe();

        [Pure, MethodImpl(Proxy.mops)]
        public static Pipe<RT, IN, X, X> observe<RT, IN, X>(IObservable<X> xs)
            where RT : struct, HasCancel<RT> =>
            new Enumerate<RT, Unit, IN, Unit, X, X, X>(xs, Pipe.Pure<RT, IN, X, X>).ToPipe();

        [Pure, MethodImpl(Proxy.mops)]
        public static Pipe<RT, IN, OUT, X> observe<RT, IN, OUT, X>(IObservable<X> xs)
            where RT : struct, HasCancel<RT> =>
            new Enumerate<RT, Unit, IN, Unit, OUT, X, X>(xs, Pipe.Pure<RT, IN, OUT, X>).ToPipe();

        /// <summary>
        /// Only forwards values that satisfy the predicate.
        /// </summary>
        public static Pipe<RT, A, A, Unit> filter<RT, A>(Func<A, bool> f)  where RT : struct, HasCancel<RT> =>
            Proxy.cat<RT, A, Unit>().For(a => f(a)
                    ? Pipe.yield<RT, A, A>(a)
                    : Pipe.Pure<RT, A, A, Unit>(default))
                .ToPipe();

        /// <summary>
        /// Map the output of the pipe (not the bound value as is usual with Map)
        /// </summary>
        public static Pipe<RT, A, B, R> map<RT, A, B, R>(Func<A, B> f) where RT : struct, HasCancel<RT> =>
            Proxy.cat<RT, A, R>().For(a => Pipe.yield<RT, A, B>(f(a))).ToPipe();

        /// <summary>
        /// Map the output of the pipe (not the bound value as is usual with Map)
        /// </summary>
        public static Pipe<RT, A, B, Unit> map<RT, A, B>(Func<A, B> f) where RT : struct, HasCancel<RT> =>
            Proxy.cat<RT, A, Unit>().For(a => Pipe.yield<RT, A, B>(f(a))).ToPipe();

        /// <summary>
        /// Map the output of the pipe (not the bound value as is usual with Map)
        /// </summary>
        public static Pipe<A, B, Unit> map<A, B>(Func<A, B> f) =>
            new Pipe<A, B, Unit>.Await(x => new Pipe<A, B, Unit>.Yield(f(x), PureProxy.PipePure<A, B, Unit>));

        /// <summary>
        /// Apply a monadic function to all values flowing downstream (not the bound value as is usual with Map)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Pipe<RT, A, B, R> mapM<RT, A, B, R>(Func<A, Aff<RT, B>> f) where RT : struct, HasCancel<RT> =>
            Proxy.cat<RT, A, R>()
                 .ForEach<RT, A, A, B, R>(a => Pipe.lift<RT, A, B, B>(f(a))
                                                .Bind(Pipe.yield<RT, A, B>)
                 .ToPipe());

        /// <summary>
        /// Apply a monadic function to all values flowing downstream (not the bound value as is usual with Map)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Pipe<RT, A, A, Unit> mapM<RT, A>(Func<A, Aff<RT, A>> f) where RT : struct, HasCancel<RT> =>
            Proxy.cat<RT, A, Unit>()
                .ForEach<RT, A, A, A, Unit>(a => Pipe.lift<RT, A, A, A>(f(a))
                    .Bind(Pipe.yield<RT, A, A>)
                    .ToPipe());

        /// <summary>
        /// Apply a monadic function to all values flowing downstream (not the bound value as is usual with Map)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Pipe<RT, A, B, R> mapM<RT, A, B, R>(Func<A, Aff<B>> f) where RT : struct, HasCancel<RT> =>
            Proxy.cat<RT, A, R>()
                 .ForEach<RT, A, A, B, R>(a => Pipe.lift<RT, A, B, B>(f(a))
                                                .Bind(Pipe.yield<RT, A, B>)
                 .ToPipe());
        
        /// <summary>
        /// Apply a monadic function to all values flowing downstream (not the bound value as is usual with Map)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Pipe<RT, A, B, R> mapM<RT, A, B, R>(Func<A, Eff<RT, B>> f) where RT : struct, HasCancel<RT> =>
            Proxy.cat<RT, A, R>()
                .ForEach<RT, A, A, B, R>(a => Pipe.lift<RT, A, B, B>(f(a))
                    .Bind(Pipe.yield<RT, A, B>)
                    .ToPipe());
        
        /// <summary>
        /// Apply a monadic function to all values flowing downstream (not the bound value as is usual with Map)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Pipe<RT, A, A, Unit> mapM<RT, A>(Func<A, Eff<RT, A>> f) where RT : struct, HasCancel<RT> =>
            Proxy.cat<RT, A, Unit>()
                .ForEach<RT, A, A, A, Unit>(a => Pipe.lift<RT, A, A, A>(f(a))
                    .Bind(Pipe.yield<RT, A, A>)
                    .ToPipe());

        /// <summary>
        /// Apply a monadic function to all values flowing downstream (not the bound value as is usual with Map)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Pipe<RT, A, B, R> mapM<RT, A, B, R>(Func<A, Eff<B>> f) where RT : struct, HasCancel<RT> =>
            Proxy.cat<RT, A, R>()
                .ForEach<RT, A, A, B, R>(a => Pipe.lift<RT, A, B, B>(f(a))
                    .Bind(Pipe.yield<RT, A, B>)
                    .ToPipe());

        /// <summary>
        /// Lift the IO monad into the Pipe monad transformer (a specialism of the Proxy monad transformer)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Pipe<RT, A, B, R> lift<RT, A, B, R>(Aff<R> ma) where RT : struct, HasCancel<RT> =>
            lift<RT, Unit, A, Unit, B, R>(ma).ToPipe(); 
 
        /// <summary>
        /// Lift the IO monad into the Pipe monad transformer (a specialism of the Proxy monad transformer)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Pipe<RT, A, B, R> lift<RT, A, B, R>(Eff<R> ma) where RT : struct, HasCancel<RT> =>
            lift<RT, Unit, A, Unit, B, R>(ma).ToPipe(); 
 
        /// <summary>
        /// Lift the IO monad into the Pipe monad transformer (a specialism of the Proxy monad transformer)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Pipe<RT, A, B, R> lift<RT, A, B, R>(Aff<RT, R> ma) where RT : struct, HasCancel<RT> =>
            lift<RT, Unit, A, Unit, B, R>(ma).ToPipe(); 
 
        /// <summary>
        /// Lift the IO monad into the Pipe monad transformer (a specialism of the Proxy monad transformer)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Pipe<RT, A, B, R> lift<RT, A, B, R>(Eff<RT, R> ma) where RT : struct, HasCancel<RT> =>
            lift<RT, Unit, A, Unit, B, R>(ma).ToPipe(); 
        
        /// <summary>
        /// Lift am IO monad into the `Proxy` monad transformer
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Pipe<RT, IN, OUT, R> use<RT, IN, OUT, R>(Aff<R> ma) 
            where RT : struct, HasCancel<RT>
            where R : IDisposable =>
            use<RT, Unit, IN, Unit, OUT, R>(ma).ToPipe();

        /// <summary>
        /// Lift am IO monad into the `Proxy` monad transformer
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Pipe<RT, IN, OUT, R> use<RT, IN, OUT, R>(Eff<R> ma) 
            where RT : struct, HasCancel<RT>
            where R : IDisposable =>
            use<RT, Unit, IN, Unit, OUT, R>(ma).ToPipe();

        /// <summary>
        /// Lift am IO monad into the `Proxy` monad transformer
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Pipe<RT, IN, OUT, R> use<RT, IN, OUT, R>(Aff<RT, R> ma) 
            where RT : struct, HasCancel<RT> 
            where R : IDisposable =>
            use<RT, Unit, IN, Unit, OUT, R>(ma).ToPipe();

        /// <summary>
        /// Lift am IO monad into the `Proxy` monad transformer
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Pipe<RT, IN, OUT, R> use<RT, IN, OUT, R>(Eff<RT, R> ma) 
            where RT : struct, HasCancel<RT> 
            where R : IDisposable =>
            use<RT, Unit, IN, Unit, OUT, R>(ma).ToPipe();
        
        
        /// <summary>
        /// Lift am IO monad into the `Proxy` monad transformer
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Pipe<RT, IN, OUT, R> use<RT, IN, OUT, R>(Aff<R> ma, Func<R, Unit> dispose) 
            where RT : struct, HasCancel<RT> =>
            use<RT, Unit, IN, Unit, OUT, R>(ma, dispose).ToPipe();

        /// <summary>
        /// Lift am IO monad into the `Proxy` monad transformer
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Pipe<RT, IN, OUT, R> use<RT, IN, OUT, R>(Eff<R> ma, Func<R, Unit> dispose) 
            where RT : struct, HasCancel<RT> =>
            use<RT, Unit, IN, Unit, OUT, R>(ma, dispose).ToPipe();

        /// <summary>
        /// Lift am IO monad into the `Proxy` monad transformer
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Pipe<RT, IN, OUT, R> use<RT, IN, OUT, R>(Aff<RT, R> ma, Func<R, Unit> dispose) 
            where RT : struct, HasCancel<RT> =>
            use<RT, Unit, IN, Unit, OUT, R>(ma, dispose).ToPipe();

        /// <summary>
        /// Lift am IO monad into the `Proxy` monad transformer
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Pipe<RT, IN, OUT, R> use<RT, IN, OUT, R>(Eff<RT, R> ma, Func<R, Unit> dispose) 
            where RT : struct, HasCancel<RT> =>
            use<RT, Unit, IN, Unit, OUT, R>(ma, dispose).ToPipe();      
        
        /// <summary>
        /// Release a previously used resource
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Pipe<RT, IN, OUT, Unit> release<RT, IN, OUT, R>(R dispose) 
            where RT : struct, HasCancel<RT> =>
            Proxy.release<RT, Unit, IN, Unit, OUT, R>(dispose).ToPipe();

        /// <summary>
        /// Folds values coming down-stream, when the predicate returns false the folded value is yielded 
        /// </summary>
        /// <param name="Initial">Initial state</param>
        /// <param name="Fold">Fold operation</param>
        /// <param name="WhileState">Predicate</param>
        /// <returns>A pipe that folds</returns>
        [Pure, MethodImpl(Proxy.mops)]
        public static Pipe<RT, IN, OUT, Unit> foldWhile<RT, IN, OUT>(OUT Initial, Func<OUT, IN, OUT> Fold, Func<OUT, bool> WhileState) 
            where RT : struct, HasCancel<RT> =>
            foldUntil<RT, IN, OUT>(Initial, Fold, x => !WhileState(x));
 
        /// <summary>
        /// Folds values coming down-stream, when the predicate returns true the folded value is yielded 
        /// </summary>
        /// <param name="Initial">Initial state</param>
        /// <param name="Fold">Fold operation</param>
        /// <param name="UntilState">Predicate</param>
        /// <returns>A pipe that folds</returns>
        public static Pipe<RT, IN, OUT, Unit> foldUntil<RT, IN, OUT>(OUT Initial, Func<OUT, IN, OUT> Fold, Func<OUT, bool> UntilState)
            where RT : struct, HasCancel<RT>
        {
            var state = Initial;
            return Pipe.awaiting<RT, IN, OUT>()
                       .Bind(x =>
                             {
                                 state = Fold(state, x);
                                 if (UntilState(state))
                                 {
                                     var nstate = state;
                                     state = Initial;
                                     return Pipe.yield<RT, IN, OUT>(nstate);
                                 }
                                 else
                                 {
                                     return Pipe.Pure<RT, IN, OUT, Unit>(unit);
                                 }
                             })
                       .ToPipe();
        }

        /// <summary>
        /// Folds values coming down-stream, when the predicate returns false the folded value is yielded 
        /// </summary>
        /// <param name="Initial">Initial state</param>
        /// <param name="Fold">Fold operation</param>
        /// <param name="WhileValue">Predicate</param>
        /// <returns>A pipe that folds</returns>
        [Pure, MethodImpl(Proxy.mops)]
        public static Pipe<RT, IN, OUT, Unit> foldWhile<RT, IN, OUT>(OUT Initial, Func<OUT, IN, OUT> Fold, Func<IN, bool> WhileValue) 
            where RT : struct, HasCancel<RT> =>
            foldUntil<RT, IN, OUT>(Initial, Fold, x => !WhileValue(x));
 
        /// <summary>
        /// Folds values coming down-stream, when the predicate returns true the folded value is yielded 
        /// </summary>
        /// <param name="Initial">Initial state</param>
        /// <param name="Fold">Fold operation</param>
        /// <param name="UntilValue">Predicate</param>
        /// <returns>A pipe that folds</returns>
        public static Pipe<RT, IN, OUT, Unit> foldUntil<RT, IN, OUT>(OUT Initial, Func<OUT, IN, OUT> Fold, Func<IN, bool> UntilValue)
            where RT : struct, HasCancel<RT>
        {
            var state = Initial;
            return Pipe.awaiting<RT, IN, OUT>()
                       .Bind(x =>
                             {
                                 if (UntilValue(x))
                                 {
                                     var nstate = state;
                                     state = Initial;
                                     return Pipe.yield<RT, IN, OUT>(nstate);
                                 }
                                 else
                                 {
                                     state = Fold(state, x);
                                     return Pipe.Pure<RT, IN, OUT, Unit>(unit);
                                 }
                             })
                       .ToPipe();
        }


        /// <summary>
        /// Strict left scan
        /// </summary>
        public static Pipe<RT, IN, OUT, Unit> scan<RT, IN, OUT, S>(Func<S, IN, S> Step, S Begin, Func<S, OUT> Done)
            where RT : struct, HasCancel<RT>
        {
            return go(Begin);

            Pipe<RT, IN, OUT, Unit> go(S x) =>
                from _ in Pipe.yield<RT, IN, OUT>(Done(x))
                from a in Pipe.awaiting<RT, IN, OUT>()
                let x1 = Step(x, a)
                from r in go(x1)
                select r;
        }
    }
}
