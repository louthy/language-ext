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
    public struct IO<A>
    {
        internal const MethodImplOptions mops = MethodImplOptions.AggressiveInlining;

        internal readonly ThunkAsync<A> thunk;

        /// <summary>
        /// Constructor
        /// </summary>
        [MethodImpl(IO.mops)]
        internal IO(ThunkAsync<A> thunk) =>
            this.thunk = thunk ?? throw new ArgumentNullException(nameof(thunk));

        /// <summary>
        /// Invoke the effect
        /// </summary>
        [Pure, MethodImpl(IO.mops)]
        public ValueTask<Fin<A>> RunIO() =>
            thunk.Value();

        /// <summary>
        /// Lift an asynchronous effect into the IO monad
        /// </summary>
        [Pure, MethodImpl(IO.mops)]
        public static IO<A> Effect(Func<ValueTask<A>> f) =>
            new IO<A>(ThunkAsync<A>.Lazy(f));

        /// <summary>
        /// Lift an asynchronous effect into the IO monad
        /// </summary>
        [Pure, MethodImpl(IO.mops)]
        public static IO<A> EffectMaybe(Func<ValueTask<Fin<A>>> f) =>
            new IO<A>(ThunkAsync<A>.Lazy(f));
        
        /// <summary>
        /// Lift an asynchronous effect into the IO monad
        /// </summary>
        [Pure, MethodImpl(IO.mops)]
        public static IO<Unit> Effect(Func<ValueTask> f) =>
            new IO<Unit>(ThunkAsync<Unit>.Lazy(async () =>
            {
                await f();
                return Fin<Unit>.Succ(default);
            }));

        /// <summary>
        /// Lift a value into the IO monad 
        /// </summary>
        [Pure, MethodImpl(IO.mops)]
        public static IO<A> Success(A value) =>
            new IO<A>(ThunkAsync<A>.Success(value));

        /// <summary>
        /// Lift a failure into the IO monad 
        /// </summary>
        [Pure, MethodImpl(IO.mops)]
        public static IO<A> Fail(Error error) =>
            new IO<A>(ThunkAsync<A>.Fail(error));

        [Pure, MethodImpl(IO.mops)]
        public static IO<A> operator |(IO<A> ma, IO<A> mb) => new IO<A>(ThunkAsync<A>.Lazy(
            async () =>
            {
                var ra = await ma.RunIO();
                return ra.IsSucc
                    ? ra
                    : await mb.RunIO();
            }));

        [Pure, MethodImpl(IO.mops)]
        public static IO<A> operator |(IO<A> ma, SIO<A> mb) => new IO<A>(ThunkAsync<A>.Lazy(
            async () =>
            {
                var ra = await ma.RunIO();
                return ra.IsSucc
                    ? ra
                    : mb.RunIO();
            }));

        [Pure, MethodImpl(IO.mops)]
        public static IO<A> operator |(SIO<A> ma, IO<A> mb) => new IO<A>(ThunkAsync<A>.Lazy(
            async () =>
            {
                var ra = ma.RunIO();
                return ra.IsSucc
                    ? ra
                    : await mb.RunIO();
            }));

        [Pure, MethodImpl(IO.mops)]
        public IO<Env, A> WithEnv<Env>() where Env : Cancellable
        {
            var self = this;
            return IO<Env, A>.EffectMaybe(e => self.RunIO());
        }

        [Pure, MethodImpl(IO.mops)]
        public async Task<S> Fold<S>(S state, Func<S, A, S> f)
        {
            var r = await RunIO();
            return r.IsSucc
                ? f(state, r.Value)
                : state;
        }

        [Pure, MethodImpl(IO.mops)]
        public async Task<bool> Exists(Func<A, bool> f)
        {
            var r = await RunIO();
            return r.IsSucc && f(r.Value);
        }

        [Pure, MethodImpl(IO.mops)]
        public async Task<bool> ForAll(Func<A, bool> f)
        {
            var r = await RunIO();
            return r.IsFail || (r.IsSucc && f(r.Value));
        }

        /// <summary>
        /// Clear the memoised value
        /// </summary>
        [MethodImpl(IO.mops)]
        public IO<A> Clear()
        {
            thunk.Flush();
            return this;
        }
    }
    
    public static partial class IOExtensions
    {
        [Pure, MethodImpl(IO.mops)]
        public static IO<B> Map<A, B>(this IO<A> ma, Func<A, B> f) =>
            new IO<B>(ma.thunk.Map(f));

        [Pure, MethodImpl(IO.mops)]
        public static IO<B> MapAsync<Env, A, B>(this IO<A> ma, Func<A, ValueTask<B>> f) =>
            new IO<B>(ma.thunk.MapAsync(f));

        
        [Pure, MethodImpl(IO.mops)]
        public static IO<A> Filter<A>(this IO<A> ma, Func<A, bool> f) =>
            ma.Bind(x => f(x) ? SIO.SuccessIO<A>(x) : SIO.FailIO<A>(Error.New(Thunk.CancelledText)));        


        [Pure, MethodImpl(IO.mops)]
        public static IO<Env, B> Bind<Env, A, B>(this IO<A> ma, Func<A, IO<Env, B>> f) where Env : Cancellable =>
            new IO<Env, B>(ma.thunk.Map(x => ThunkFromIO(f(x))).Flatten());

        [Pure, MethodImpl(IO.mops)]
        public static IO<B> Bind<A, B>(this IO<A> ma, Func<A, IO<B>> f) =>
            new IO<B>(ma.thunk.Map(x => ThunkFromIO(f(x))).Flatten());

        [Pure, MethodImpl(IO.mops)]
        public static IO<Env, B> Bind<Env, A, B>(this IO<A> ma, Func<A, SIO<Env, B>> f) where Env : Cancellable =>
            new IO<Env, B>(ma.thunk.Map(x => ThunkFromIO(f(x))).Flatten());

        [Pure, MethodImpl(IO.mops)]
        public static IO<B> Bind<A, B>(this IO<A> ma, Func<A, SIO<B>> f) =>
            new IO<B>(ma.thunk.Map(x => ThunkFromIO(f(x))).Flatten());

        
        [Pure, MethodImpl(IO.mops)]
        public static IO<A> Flatten<A>(this IO<IO<A>> ma) =>
            new IO<A>(ma.thunk.Map(ThunkFromIO).Flatten());
        
        [Pure, MethodImpl(IO.mops)]
        public static IO<Env, A> Flatten<Env, A>(this IO<IO<Env, A>> ma) where Env : Cancellable =>
            new IO<Env, A>(ma.thunk.Map(ThunkFromIO).Flatten());
        
        [Pure, MethodImpl(IO.mops)]
        public static IO<A> Flatten<A>(this IO<SIO<A>> ma) =>
            new IO<A>(ma.thunk.Map(ThunkFromIO).Flatten());
        
        [Pure, MethodImpl(IO.mops)]
        public static IO<Env, A> Flatten<Env, A>(this IO<SIO<Env, A>> ma) where Env : Cancellable =>
            new IO<Env, A>(ma.thunk.Map(ThunkFromIO).Flatten());

        
        [Pure, MethodImpl(IO.mops)]
        public static IO<B> Select<A, B>(this IO<A> ma, Func<A, B> f) =>
            Map(ma, f);
        
        [Pure, MethodImpl(IO.mops)]
        public static IO<Env, B> SelectMany<Env, A, B>(this IO<A> ma, Func<A, IO<Env, B>> f) where Env : Cancellable =>
            Bind(ma, f);
        
        [Pure, MethodImpl(IO.mops)]
        public static IO<B> SelectMany<A, B>(this IO<A> ma, Func<A, IO<B>> f) =>
            Bind(ma, f);
        
        [Pure, MethodImpl(IO.mops)]
        public static IO<Env, B> SelectMany<Env, A, B>(this IO<A> ma, Func<A, SIO<Env, B>> f) where Env : Cancellable =>
            Bind(ma, f);
        
        [Pure, MethodImpl(IO.mops)]
        public static IO<B> SelectMany<A, B>(this IO<A> ma, Func<A, SIO<B>> f) =>
            Bind(ma, f);

        
        [Pure, MethodImpl(IO.mops)]
        public static IO<Env, C> SelectMany<Env, A, B, C>(this IO<A> ma, Func<A, IO<Env, B>> bind, Func<A, B, C> project) where Env : Cancellable =>
            Bind(ma, x => Map(bind(x), y => project(x, y)));
        
        [Pure, MethodImpl(IO.mops)]
        public static IO<C> SelectMany<A, B, C>(this IO<A> ma, Func<A, IO<B>> bind, Func<A, B, C> project) =>
            Bind(ma, x => Map(bind(x), y => project(x, y)));
        
        [Pure, MethodImpl(IO.mops)]
        public static IO<Env, C> SelectMany<Env, A, B, C>(this IO<A> ma, Func<A, SIO<Env, B>> bind, Func<A, B, C> project) where Env : Cancellable =>
            Bind(ma, x => Map(bind(x), y => project(x, y)));
        
        [Pure, MethodImpl(IO.mops)]
        public static IO<C> SelectMany<A, B, C>(this IO<A> ma, Func<A, SIO<B>> bind, Func<A, B, C> project) =>
            Bind(ma, x => Map(bind(x), y => project(x, y)));
        
        
        [Pure, MethodImpl(IO.mops)]
        public static IO<A> Where<A>(this IO<A> ma, Func<A, bool> f) =>
            Filter(ma, f);
        
        [Pure, MethodImpl(IO.mops)]
        public static IO<(A, B)> Zip<A, B>(IO<A> ma, IO<B> mb) =>
            new IO<(A, B)>(ThunkAsync<(A, B)>.Lazy(async () =>
            {
                var ta = ma.RunIO().AsTask();
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
        public static IO<(A, B)> Zip<A, B>(IO<A> ma, SIO<B> mb) =>
            new IO<(A, B)>(ThunkAsync<(A, B)>.Lazy(async () =>
            {
                var ta = ma.RunIO().AsTask();
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
        public static IO<(A, B)> Zip<A, B>(SIO<A> ma, IO<B> mb) =>
            new IO<(A, B)>(ThunkAsync<(A, B)>.Lazy(async () =>
            {
                var ra = ma.RunIO();
                if(ra.IsFail) return Fin<(A, B)>.Fail(ra.Error);
                
                var tb = mb.RunIO().AsTask();
                var rb = await tb;
                if (!tb.CompletedSuccessfully())
                {
                    return Fin<(A, B)>.Fail(rb.Error);
                }

                return Fin<(A, B)>.Succ((ra.Value, rb.Value));
            }));         
        
        [Pure, MethodImpl(IO.mops)]
        static ThunkAsync<A> ThunkFromIO<A>(IO<A> ma) =>
            ma.thunk;
        
    }    
}
