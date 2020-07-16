using System;
using LanguageExt.Common;
using System.Threading.Tasks;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading;
using LanguageExt.Interfaces;
using LanguageExt.Thunks;

namespace LanguageExt
{
    /// <summary>
    /// Asynchronous IO monad
    /// </summary>
    public struct IO<Env, A> where Env : Cancellable
    {
        internal readonly ThunkAsync<Env, A> thunk;

        /// <summary>
        /// Constructor
        /// </summary>
        [MethodImpl(IO.mops)]
        internal IO(ThunkAsync<Env, A> thunk) =>
            this.thunk = thunk ?? throw new ArgumentNullException(nameof(thunk));

        /// <summary>
        /// Invoke the effect
        /// </summary>
        [Pure, MethodImpl(IO.mops)]
        public ValueTask<Fin<A>> RunIO(in Env env) =>
            thunk.Value(env ?? throw new ArgumentNullException(nameof(env)));

        /// <summary>
        /// Invoke the effect
        /// </summary>
        [MethodImpl(IO.mops)]
        public async ValueTask RunUnitIO(Env env) =>
            await thunk.Value(env ?? throw new ArgumentNullException(nameof(env)));

        /// <summary>
        /// Lift an asynchronous effect into the IO monad
        /// </summary>
        [Pure, MethodImpl(IO.mops)]
        public static IO<Env, A> Effect(Func<Env, ValueTask<A>> f) =>
            new IO<Env, A>(ThunkAsync<Env, A>.Lazy(f));

        /// <summary>
        /// Lift an asynchronous effect into the IO monad
        /// </summary>
        [Pure, MethodImpl(IO.mops)]
        public static IO<Env, A> EffectMaybe(Func<Env, ValueTask<Fin<A>>> f) =>
            new IO<Env, A>(ThunkAsync<Env, A>.Lazy(f));
        
        /// <summary>
        /// Lift an asynchronous effect into the IO monad
        /// </summary>
        [Pure, MethodImpl(IO.mops)]
        public static IO<Env, Unit> Effect(Func<Env, ValueTask> f) =>
            new IO<Env, Unit>(ThunkAsync<Env, Unit>.Lazy(async e =>
            {
                await f(e);
                return Fin<Unit>.Succ(default);
            }));

        /// <summary>
        /// Lift a value into the IO monad 
        /// </summary>
        [Pure, MethodImpl(IO.mops)]
        public static IO<Env, A> Success(A value) =>
            new IO<Env, A>(ThunkAsync<Env, A>.Success(value));

        /// <summary>
        /// Lift a failure into the IO monad 
        /// </summary>
        [Pure, MethodImpl(IO.mops)]
        public static IO<Env, A> Fail(Error error) =>
            new IO<Env, A>(ThunkAsync<Env, A>.Fail(error));

        [Pure, MethodImpl(IO.mops)]
        public static IO<Env, A> operator |(IO<Env, A> ma, IO<Env, A> mb) => new IO<Env, A>(ThunkAsync<Env, A>.Lazy(
            async env =>
            {
                var ra = await ma.RunIO(env);
                return ra.IsSucc
                    ? ra
                    : await mb.RunIO(env);
            }));

        [Pure, MethodImpl(IO.mops)]
        public static IO<Env, A> operator |(IO<Env, A> ma, IO<A> mb) => new IO<Env, A>(ThunkAsync<Env, A>.Lazy(
            async env =>
            {
                var ra = await ma.RunIO(env);
                return ra.IsSucc
                    ? ra
                    : await mb.RunIO();
            }));

        [Pure, MethodImpl(IO.mops)]
        public static IO<Env, A> operator |(IO<A> ma, IO<Env, A> mb) => new IO<Env, A>(ThunkAsync<Env, A>.Lazy(
            async env =>
            {
                var ra = await ma.RunIO();
                return ra.IsSucc
                    ? ra
                    : await mb.RunIO(env);
            }));
        
        [Pure, MethodImpl(IO.mops)]
        public static IO<Env, A> operator |(IO<Env, A> ma, SIO<Env, A> mb) => new IO<Env, A>(ThunkAsync<Env, A>.Lazy(
            async env =>
            {
                var ra = await ma.RunIO(env);
                return ra.IsSucc
                    ? ra
                    : mb.RunIO(env);
            }));
        
        [Pure, MethodImpl(IO.mops)]
        public static IO<Env, A> operator |(SIO<Env, A> ma, IO<Env, A> mb) => new IO<Env, A>(ThunkAsync<Env, A>.Lazy(
            async env =>
            {
                var ra = ma.RunIO(env);
                return ra.IsSucc
                    ? ra
                    : await mb.RunIO(env);
            }));

        /// <summary>
        /// Clear the memoised value
        /// </summary>
        [MethodImpl(IO.mops)]
        public IO<Env, A> Clear()
        {
            thunk.Flush();
            return this;
        }
    }

    public static partial class IOExtensions
    {        
        [Pure, MethodImpl(IO.mops)]
        public static IO<Env, B> Map<Env, A, B>(this IO<Env, A> ma, Func<A, B> f) where Env : Cancellable =>
            new IO<Env, B>(ma.thunk.Map(f));

        [Pure, MethodImpl(IO.mops)]
        public static IO<Env, B> MapAsync<Env, A, B>(this IO<Env, A> ma, Func<A, ValueTask<B>> f) where Env : Cancellable =>
            new IO<Env, B>(ma.thunk.MapAsync(f));

        
        [Pure, MethodImpl(IO.mops)]
        public static IO<Env, A> Filter<Env, A>(this IO<Env, A> ma, Func<A, bool> f) where Env : Cancellable =>
            ma.Bind(x => f(x) ? SIO.SuccessIO<A>(x) : SIO.FailIO<A>(Error.New(Thunk.CancelledText)));        


        [Pure, MethodImpl(IO.mops)]
        public static IO<Env, B> Bind<Env, A, B>(this IO<Env, A> ma, Func<A, IO<Env, B>> f)  where Env : Cancellable=>
            new IO<Env, B>(ma.thunk.Map(x => ThunkFromIO(f(x))).Flatten());

        [Pure, MethodImpl(IO.mops)]
        public static IO<Env, B> Bind<Env, A, B>(this IO<Env, A> ma, Func<A, IO<B>> f) where Env : Cancellable =>
            new IO<Env, B>(ma.thunk.Map(x => ThunkFromIO(f(x))).Flatten());

        [Pure, MethodImpl(IO.mops)]
        public static IO<Env, B> Bind<Env, A, B>(this IO<Env, A> ma, Func<A, SIO<Env, B>> f) where Env : Cancellable =>
            new IO<Env, B>(ma.thunk.Map(x => ThunkFromIO(f(x))).Flatten());

        [Pure, MethodImpl(IO.mops)]
        public static IO<Env, B> Bind<Env, A, B>(this IO<Env, A> ma, Func<A, SIO<B>> f)  where Env : Cancellable=>
            new IO<Env, B>(ma.thunk.Map(x => ThunkFromIO(f(x))).Flatten());

        
        [Pure, MethodImpl(IO.mops)]
        public static IO<Env, A> Flatten<Env, A>(this IO<Env, IO<Env, A>> ma) where Env : Cancellable =>
            new IO<Env, A>(ma.thunk.Map(ThunkFromIO).Flatten());
        
        [Pure, MethodImpl(IO.mops)]
        public static IO<Env, A> Flatten<Env, A>(this IO<Env, IO<A>> ma)  where Env : Cancellable =>
            new IO<Env, A>(ma.thunk.Map(ThunkFromIO).Flatten());
        
        [Pure, MethodImpl(IO.mops)]
        public static IO<Env, A> Flatten<Env, A>(this IO<Env, SIO<Env, A>> ma) where Env : Cancellable =>
            new IO<Env, A>(ma.thunk.Map(ThunkFromIO).Flatten());
        
        [Pure, MethodImpl(IO.mops)]
        public static IO<Env, A> Flatten<Env, A>(this IO<Env, SIO<A>> ma) where Env : Cancellable =>
            new IO<Env, A>(ma.thunk.Map(ThunkFromIO).Flatten());

        
        [Pure, MethodImpl(IO.mops)]
        public static IO<Env, B> Select<Env, A, B>(this IO<Env, A> ma, Func<A, B> f) where Env : Cancellable =>
            Map(ma, f);
        
        [Pure, MethodImpl(IO.mops)]
        public static IO<Env, B> SelectMany<Env, A, B>(this IO<Env, A> ma, Func<A, IO<Env, B>> f) where Env : Cancellable =>
            Bind(ma, f);
        
        [Pure, MethodImpl(IO.mops)]
        public static IO<Env, B> SelectMany<Env, A, B>(this IO<Env, A> ma, Func<A, IO<B>> f) where Env : Cancellable =>
            Bind(ma, f);
        
        [Pure, MethodImpl(IO.mops)]
        public static IO<Env, B> SelectMany<Env, A, B>(this IO<Env, A> ma, Func<A, SIO<Env, B>> f) where Env : Cancellable =>
            Bind(ma, f);
        
        [Pure, MethodImpl(IO.mops)]
        public static IO<Env, B> SelectMany<Env, A, B>(this IO<Env, A> ma, Func<A, SIO<B>> f) where Env : Cancellable =>
            Bind(ma, f);

        
        [Pure, MethodImpl(IO.mops)]
        public static IO<Env, C> SelectMany<Env, A, B, C>(this IO<Env, A> ma, Func<A, IO<Env, B>> bind, Func<A, B, C> project) where Env : Cancellable =>
            Bind(ma, x => Map(bind(x), y => project(x, y)));
        
        [Pure, MethodImpl(IO.mops)]
        public static IO<Env, C> SelectMany<Env, A, B, C>(this IO<Env, A> ma, Func<A, IO<B>> bind, Func<A, B, C> project) where Env : Cancellable =>
            Bind(ma, x => Map(bind(x), y => project(x, y)));
        
        [Pure, MethodImpl(IO.mops)]
        public static IO<Env, C> SelectMany<Env, A, B, C>(this IO<Env, A> ma, Func<A, SIO<Env, B>> bind, Func<A, B, C> project) where Env : Cancellable=>
            Bind(ma, x => Map(bind(x), y => project(x, y)));
        
        [Pure, MethodImpl(IO.mops)]
        public static IO<Env, C> SelectMany<Env, A, B, C>(this IO<Env, A> ma, Func<A, SIO<B>> bind, Func<A, B, C> project) where Env : Cancellable =>
            Bind(ma, x => Map(bind(x), y => project(x, y)));
 
        
        [Pure, MethodImpl(IO.mops)]
        public static IO<Env, A> Where<Env, A>(this IO<Env, A> ma, Func<A, bool> f) where Env : Cancellable =>
            Filter(ma, f);
        
        
        [Pure, MethodImpl(IO.mops)]
        public static IO<Env, (A, B)> Zip<Env, A, B>(this IO<Env, A> ma, IO<Env, B> mb) where Env : Cancellable =>
            new IO<Env, (A, B)>(ThunkAsync<Env, (A, B)>.Lazy(async e =>
            {
                var ta = ma.RunIO(e).AsTask();
                var tb = mb.RunIO(e).AsTask();
                await System.Threading.Tasks.Task.WhenAll(ta, tb);
                if (ta.CompletedSuccessfully() && tb.CompletedSuccessfully())
                {
                    return ta.Result.IsSucc && tb.Result.IsSucc
                        ? Fin<(A, B)>.Succ((ta.Result.Value, tb.Result.Value))
                        : ta.Result.IsFail
                            ? Fin<(A, B)>.Fail(ta.Result.Error)
                            : Fin<(A, B)>.Fail(tb.Result.Error);
                }
                else
                {
                    return (ta.IsCanceled || tb.IsCanceled)
                        ? Fin<(A, B)>.Fail(Error.New(Thunk.CancelledText))
                        : ta.IsFaulted
                            ? Fin<(A, B)>.Fail(Error.New(ta.Exception))
                            : Fin<(A, B)>.Fail(Error.New(tb.Exception));
                }
            }));
        
        [Pure, MethodImpl(IO.mops)]
        public static IO<Env, (A, B)> Zip<Env, A, B>(this IO<Env, A> ma, IO<B> mb) where Env : Cancellable =>
            new IO<Env, (A, B)>(ThunkAsync<Env, (A, B)>.Lazy(async e =>
            {
                var ta = ma.RunIO(e).AsTask();
                var tb = mb.RunIO().AsTask();
                await System.Threading.Tasks.Task.WhenAll(ta, tb);
                if (ta.CompletedSuccessfully() && tb.CompletedSuccessfully())
                {
                    return ta.Result.IsSucc && tb.Result.IsSucc
                        ? Fin<(A, B)>.Succ((ta.Result.Value, tb.Result.Value))
                        : ta.Result.IsFail
                            ? Fin<(A, B)>.Fail(ta.Result.Error)
                            : Fin<(A, B)>.Fail(tb.Result.Error);
                }
                else
                {
                    return (ta.IsCanceled || tb.IsCanceled)
                        ? Fin<(A, B)>.Fail(Error.New(Thunk.CancelledText))
                        : ta.IsFaulted
                            ? Fin<(A, B)>.Fail(Error.New(ta.Exception))
                            : Fin<(A, B)>.Fail(Error.New(tb.Exception));
                }
            }));        
        
        
        [Pure, MethodImpl(IO.mops)]
        public static IO<Env, (A, B)> Zip<Env, A, B>(this IO<A> ma, IO<Env, B> mb) where Env : Cancellable =>
            new IO<Env, (A, B)>(ThunkAsync<Env, (A, B)>.Lazy(async e =>
            {
                var ta = ma.RunIO().AsTask();
                var tb = mb.RunIO(e).AsTask();
                await System.Threading.Tasks.Task.WhenAll(ta, tb);
                if (ta.CompletedSuccessfully() && tb.CompletedSuccessfully())
                {
                    return ta.Result.IsSucc && tb.Result.IsSucc
                        ? Fin<(A, B)>.Succ((ta.Result.Value, tb.Result.Value))
                        : ta.Result.IsFail
                            ? Fin<(A, B)>.Fail(ta.Result.Error)
                            : Fin<(A, B)>.Fail(tb.Result.Error);
                }
                else
                {
                    return (ta.IsCanceled || tb.IsCanceled)
                        ? Fin<(A, B)>.Fail(Error.New(Thunk.CancelledText))
                        : ta.IsFaulted
                            ? Fin<(A, B)>.Fail(Error.New(ta.Exception))
                            : Fin<(A, B)>.Fail(Error.New(tb.Exception));
                }
            }));         
        
        [Pure, MethodImpl(IO.mops)]
        public static IO<Env, (A, B)> Zip<Env, A, B>(this IO<Env, A> ma, SIO<B> mb) where Env : Cancellable =>
            new IO<Env, (A, B)>(ThunkAsync<Env, (A, B)>.Lazy(async e =>
            {
                var ta = ma.RunIO(e).AsTask();
                var ra = await ta;
                if (!ta.CompletedSuccessfully())
                {
                    return Fin<(A, B)>.Fail(ra.Error);
                }

                var rb = mb.RunIO();
                if(rb.IsFail) return Fin<(A, B)>.Fail(rb.Error);

                return Fin<(A, B)>.Succ((ra.Value, rb.Value));
            }));  
        
        [Pure, MethodImpl(IO.mops)]
        public static IO<Env, (A, B)> Zip<Env, A, B>(this SIO<A> ma, IO<Env, B> mb) where Env : Cancellable =>
            new IO<Env, (A, B)>(ThunkAsync<Env, (A, B)>.Lazy(async e =>
            {
                var ra = ma.RunIO();
                if(ra.IsFail) return Fin<(A, B)>.Fail(ra.Error);
                
                var tb = mb.RunIO(e).AsTask();
                var rb = await tb;
                if (!tb.CompletedSuccessfully())
                {
                    return Fin<(A, B)>.Fail(rb.Error);
                }

                return Fin<(A, B)>.Succ((ra.Value, rb.Value));
            }));     
        
        
        [Pure, MethodImpl(IO.mops)]
        public static IO<Env, (A, B)> Zip<Env, A, B>(this IO<Env, A> ma, SIO<Env, B> mb) where Env : Cancellable =>
            new IO<Env, (A, B)>(ThunkAsync<Env, (A, B)>.Lazy(async e =>
            {
                var ta = ma.RunIO(e).AsTask();
                var ra = await ta;
                if (!ta.CompletedSuccessfully())
                {
                    return Fin<(A, B)>.Fail(ra.Error);
                }

                var rb = mb.RunIO(e);
                if(rb.IsFail) return Fin<(A, B)>.Fail(rb.Error);

                return Fin<(A, B)>.Succ((ra.Value, rb.Value));
            }));  
        
        [Pure, MethodImpl(IO.mops)]
        public static IO<Env, (A, B)> Zip<Env, A, B>(this SIO<Env, A> ma, IO<Env, B> mb) where Env : Cancellable =>
            new IO<Env, (A, B)>(ThunkAsync<Env, (A, B)>.Lazy(async e =>
            {
                var ra = ma.RunIO(e);
                if(ra.IsFail) return Fin<(A, B)>.Fail(ra.Error);
                
                var tb = mb.RunIO(e).AsTask();
                var rb = await tb;
                if (!tb.CompletedSuccessfully())
                {
                    return Fin<(A, B)>.Fail(rb.Error);
                }

                return Fin<(A, B)>.Succ((ra.Value, rb.Value));
            }));         
       
        [Pure, MethodImpl(IO.mops)]
        static ThunkAsync<Env, A> ThunkFromIO<Env, A>(IO<Env, A> ma) where Env : Cancellable =>
            ma.thunk;
    }       
}
