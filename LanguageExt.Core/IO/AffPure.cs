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
    /// Asynchronous effect monad
    /// </summary>
    [AsyncMethodBuilder(typeof(AffMethodBuilder<>))]
    public struct AffPure<A>
    {
        internal const MethodImplOptions mops = MethodImplOptions.AggressiveInlining;

        internal ThunkAsync<A> thunk;

        /// <summary>
        /// Constructor
        /// </summary>
        [MethodImpl(AffOpt.mops)]
        internal AffPure(ThunkAsync<A> thunk) =>
            this.thunk = thunk ?? throw new ArgumentNullException(nameof(thunk));

        /// <summary>
        /// Invoke the effect
        /// </summary>
        [Pure, MethodImpl(AffOpt.mops)]
        public ValueTask<Fin<A>> RunIO() =>
            thunk.Value();

        /// <summary>
        /// Custom awaiter so Aff can be used with async/await 
        /// </summary>
        public AffAwaiter<A> GetAwaiter() =>
            new AffAwaiter<A>(this);
        
        /// <summary>
        /// Lift an asynchronous effect into the Aff monad
        /// </summary>
        [Pure, MethodImpl(AffOpt.mops)]
        public static AffPure<A> Effect(Func<ValueTask<A>> f) =>
            new AffPure<A>(ThunkAsync<A>.Lazy(f));

        /// <summary>
        /// Lift an asynchronous effect into the Aff monad
        /// </summary>
        [Pure, MethodImpl(AffOpt.mops)]
        public static AffPure<A> EffectMaybe(Func<ValueTask<Fin<A>>> f) =>
            new AffPure<A>(ThunkAsync<A>.Lazy(f));
        
        /// <summary>
        /// Lift an asynchronous effect into the Aff monad
        /// </summary>
        [Pure, MethodImpl(AffOpt.mops)]
        public static AffPure<Unit> Effect(Func<ValueTask> f) =>
            new AffPure<Unit>(ThunkAsync<Unit>.Lazy(async () =>
            {
                await f().ConfigureAwait(false);
                return Fin<Unit>.Succ(default);
            }));

        /// <summary>
        /// Lift a value into the Aff monad 
        /// </summary>
        [Pure, MethodImpl(AffOpt.mops)]
        public static AffPure<A> Success(A value) =>
            new AffPure<A>(ThunkAsync<A>.Success(value));

        /// <summary>
        /// Lift a failure into the Aff monad 
        /// </summary>
        [Pure, MethodImpl(AffOpt.mops)]
        public static AffPure<A> Fail(Error error) =>
            new AffPure<A>(ThunkAsync<A>.Fail(error));

        [Pure, MethodImpl(AffOpt.mops)]
        public static AffPure<A> operator |(AffPure<A> ma, AffPure<A> mb) => new AffPure<A>(ThunkAsync<A>.Lazy(
            async () =>
            {
                var ra = await ma.RunIO().ConfigureAwait(false);
                return ra.IsSucc
                    ? ra
                    : await mb.RunIO().ConfigureAwait(false);
            }));

        [Pure, MethodImpl(AffOpt.mops)]
        public static AffPure<A> operator |(AffPure<A> ma, EffPure<A> mb) => new AffPure<A>(ThunkAsync<A>.Lazy(
            async () =>
            {
                var ra = await ma.RunIO().ConfigureAwait(false);
                return ra.IsSucc
                    ? ra
                    : mb.RunIO();
            }));

        [Pure, MethodImpl(AffOpt.mops)]
        public static AffPure<A> operator |(EffPure<A> ma, AffPure<A> mb) => new AffPure<A>(ThunkAsync<A>.Lazy(
            async () =>
            {
                var ra = ma.RunIO();
                return ra.IsSucc
                    ? ra
                    : await mb.RunIO().ConfigureAwait(false);
            }));

        [Pure, MethodImpl(AffOpt.mops)]
        public Aff<Env, A> WithEnv<Env>() where Env : struct, HasCancel<Env>
        {
            var self = this;
            return Aff<Env, A>.EffectMaybe(e => self.RunIO());
        }

        [Pure, MethodImpl(AffOpt.mops)]
        public async Task<S> Fold<S>(S state, Func<S, A, S> f)
        {
            var r = await RunIO().ConfigureAwait(false);
            return r.IsSucc
                ? f(state, r.Value)
                : state;
        }

        [Pure, MethodImpl(AffOpt.mops)]
        public async Task<bool> Exists(Func<A, bool> f)
        {
            var r = await RunIO().ConfigureAwait(false);
            return r.IsSucc && f(r.Value);
        }

        [Pure, MethodImpl(AffOpt.mops)]
        public async Task<bool> ForAll(Func<A, bool> f)
        {
            var r = await RunIO().ConfigureAwait(false);
            return r.IsFail || (r.IsSucc && f(r.Value));
        }

        /// <summary>
        /// Clear the memoised value
        /// </summary>
        [MethodImpl(AffOpt.mops)]
        public Unit Clear()
        {
            thunk = thunk.Clone();
            return default;
        }
        /// <summary>
        /// Implicit conversion from pure Eff
        /// </summary>
        public static implicit operator AffPure<A>(EffPure<A> ma) =>
            EffectMaybe(() => ma.RunIO().AsValueTask());
    }
    
    public static partial class AffExtensions
    {
        [Pure, MethodImpl(AffOpt.mops)]
        public static AffPure<B> Map<A, B>(this AffPure<A> ma, Func<A, B> f) =>
            new AffPure<B>(ma.thunk.Map(f));

        [Pure, MethodImpl(AffOpt.mops)]
        public static AffPure<B> MapAsync<A, B>(this AffPure<A> ma, Func<A, ValueTask<B>> f) =>
            new AffPure<B>(ma.thunk.MapAsync(f));

        [Pure, MethodImpl(AffOpt.mops)]
        public static AffPure<A> MapFail<A>(this AffPure<A> ma, Func<Error, Error> f) =>
            ma.BiMap(identity, f);

        [Pure, MethodImpl(AffOpt.mops)]
        public static AffPure<A> MapFailAsync<A>(this AffPure<A> ma, Func<Error, ValueTask<Error>> f) =>
            ma.BiMapAsync(x => x.AsValueTask(), f);

        [Pure, MethodImpl(AffOpt.mops)]
        public static AffPure<B> BiMap<A, B>(this AffPure<A> ma, Func<A, B> Succ, Func<Error, Error> Fail) =>
            new AffPure<B>(ma.thunk.BiMap(Succ, Fail));

        [Pure, MethodImpl(AffOpt.mops)]
        public static AffPure<B> BiMapAsync<A, B>(this AffPure<A> ma, Func<A, ValueTask<B>> Succ, Func<Error, ValueTask<Error>> Fail) =>
            new AffPure<B>(ma.thunk.BiMapAsync(Succ, Fail));

        [Pure, MethodImpl(AffOpt.mops)]
        public static AffPure<B> Match<A, B>(this AffPure<A> ma, Func<A, B> Succ, Func<Error, B> Fail) =>
            Aff(async () => { 
            
                var r = await ma.RunIO().ConfigureAwait(false);
                return r.IsSucc
                    ? Succ(r.Value)
                    : Fail(r.Error);
            });

        [Pure, MethodImpl(AffOpt.mops)]
        public static AffPure<A> Filter<A>(this AffPure<A> ma, Func<A, bool> f) =>
            ma.Bind(x => f(x) ? SuccessEff<A>(x) : FailEff<A>(Error.New(Thunk.CancelledText)));        


        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, B> Bind<Env, A, B>(this AffPure<A> ma, Func<A, Aff<Env, B>> f) where Env : struct, HasCancel<Env> =>
            new Aff<Env, B>(ma.thunk.Map(x => ThunkFromIO(f(x))).Flatten());

        [Pure, MethodImpl(AffOpt.mops)]
        public static AffPure<B> Bind<A, B>(this AffPure<A> ma, Func<A, AffPure<B>> f) =>
            new AffPure<B>(ma.thunk.Map(x => ThunkFromIO(f(x))).Flatten());

        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, B> Bind<Env, A, B>(this AffPure<A> ma, Func<A, Eff<Env, B>> f) where Env : struct, HasCancel<Env> =>
            new Aff<Env, B>(ma.thunk.Map(x => ThunkFromIO(f(x))).Flatten());

        [Pure, MethodImpl(AffOpt.mops)]
        public static AffPure<B> Bind<A, B>(this AffPure<A> ma, Func<A, EffPure<B>> f) =>
            new AffPure<B>(ma.thunk.Map(x => ThunkFromIO(f(x))).Flatten());

        

        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, B> BiBind<Env, A, B>(this AffPure<A> ma, Func<A, Aff<Env, B>> Succ, Func<Error, Aff<Env, B>> Fail) where Env : struct, HasCancel<Env> =>
            ma.Match(Succ, Fail)
              .Flatten();

        [Pure, MethodImpl(AffOpt.mops)]
        public static AffPure<B> BiBind<A, B>(this AffPure<A> ma, Func<A, AffPure<B>> Succ, Func<Error, AffPure<B>> Fail) =>
            ma.Match(Succ, Fail)
              .Flatten();

        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, B> BiBind<Env, A, B>(this AffPure<A> ma, Func<A, Eff<Env, B>> Succ, Func<Error, Eff<Env, B>> Fail) where Env : struct, HasCancel<Env> =>
            ma.Match(Succ, Fail)
              .Flatten();

        [Pure, MethodImpl(AffOpt.mops)]
        public static AffPure<B> BiBind<A, B>(this AffPure<A> ma, Func<A, EffPure<B>> Succ, Func<Error, EffPure<B>> Fail) =>
            ma.Match(Succ, Fail)
              .Flatten();
        
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static AffPure<A> Flatten<A>(this AffPure<AffPure<A>> ma) =>
            new AffPure<A>(ma.thunk.Map(ThunkFromIO).Flatten());
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, A> Flatten<Env, A>(this AffPure<Aff<Env, A>> ma) where Env : struct, HasCancel<Env> =>
            new Aff<Env, A>(ma.thunk.Map(ThunkFromIO).Flatten());
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static AffPure<A> Flatten<A>(this AffPure<EffPure<A>> ma) =>
            new AffPure<A>(ma.thunk.Map(ThunkFromIO).Flatten());
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, A> Flatten<Env, A>(this AffPure<Eff<Env, A>> ma) where Env : struct, HasCancel<Env> =>
            new Aff<Env, A>(ma.thunk.Map(ThunkFromIO).Flatten());

        
        [Pure, MethodImpl(AffOpt.mops)]
        public static AffPure<B> Select<A, B>(this AffPure<A> ma, Func<A, B> f) =>
            Map(ma, f);
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, B> SelectMany<Env, A, B>(this AffPure<A> ma, Func<A, Aff<Env, B>> f) where Env : struct, HasCancel<Env> =>
            Bind(ma, f);
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static AffPure<B> SelectMany<A, B>(this AffPure<A> ma, Func<A, AffPure<B>> f) =>
            Bind(ma, f);
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, B> SelectMany<Env, A, B>(this AffPure<A> ma, Func<A, Eff<Env, B>> f) where Env : struct, HasCancel<Env> =>
            Bind(ma, f);
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static AffPure<B> SelectMany<A, B>(this AffPure<A> ma, Func<A, EffPure<B>> f) =>
            Bind(ma, f);

        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, C> SelectMany<Env, A, B, C>(this AffPure<A> ma, Func<A, Aff<Env, B>> bind, Func<A, B, C> project) where Env : struct, HasCancel<Env> =>
            Bind(ma, x => Map(bind(x), y => project(x, y)));
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static AffPure<C> SelectMany<A, B, C>(this AffPure<A> ma, Func<A, AffPure<B>> bind, Func<A, B, C> project) =>
            Bind(ma, x => Map(bind(x), y => project(x, y)));
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, C> SelectMany<Env, A, B, C>(this AffPure<A> ma, Func<A, Eff<Env, B>> bind, Func<A, B, C> project) where Env : struct, HasCancel<Env> =>
            Bind(ma, x => Map(bind(x), y => project(x, y)));
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static AffPure<C> SelectMany<A, B, C>(this AffPure<A> ma, Func<A, EffPure<B>> bind, Func<A, B, C> project) =>
            Bind(ma, x => Map(bind(x), y => project(x, y)));
        
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static AffPure<A> Where<A>(this AffPure<A> ma, Func<A, bool> f) =>
            Filter(ma, f);
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static AffPure<(A, B)> Zip<A, B>(AffPure<A> ma, AffPure<B> mb) =>
            new AffPure<(A, B)>(ThunkAsync<(A, B)>.Lazy(async () =>
            {
                var ta = ma.RunIO().AsTask();
                var tb = mb.RunIO().AsTask();
                await System.Threading.Tasks.Task.WhenAll(ta, tb).ConfigureAwait(false);
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
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static AffPure<(A, B)> Zip<A, B>(AffPure<A> ma, EffPure<B> mb) =>
            new AffPure<(A, B)>(ThunkAsync<(A, B)>.Lazy(async () =>
            {
                var ta = ma.RunIO().AsTask();
                var ra = await ta.ConfigureAwait(false);
                if (!ta.CompletedSuccessfully())
                {
                    return Fin<(A, B)>.Fail(ra.Error);
                }

                var rb = mb.RunIO();
                if(rb.IsFail) return Fin<(A, B)>.Fail(rb.Error);

                return Fin<(A, B)>.Succ((ra.Value, rb.Value));
            }));  
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static AffPure<(A, B)> Zip<A, B>(EffPure<A> ma, AffPure<B> mb) =>
            new AffPure<(A, B)>(ThunkAsync<(A, B)>.Lazy(async () =>
            {
                var ra = ma.RunIO();
                if(ra.IsFail) return Fin<(A, B)>.Fail(ra.Error);
                
                var tb = mb.RunIO().AsTask();
                var rb = await tb.ConfigureAwait(false);
                if (!tb.CompletedSuccessfully())
                {
                    return Fin<(A, B)>.Fail(rb.Error);
                }

                return Fin<(A, B)>.Succ((ra.Value, rb.Value));
            }));         
        
        [Pure, MethodImpl(AffOpt.mops)]
        static ThunkAsync<A> ThunkFromIO<A>(AffPure<A> ma) =>
            ma.thunk;
        
    }    
}
