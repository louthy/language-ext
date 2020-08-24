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
    public struct Aff<A>
    {
        internal const MethodImplOptions mops = MethodImplOptions.AggressiveInlining;

        internal ThunkAsync<A> thunk;

        /// <summary>
        /// Constructor
        /// </summary>
        [MethodImpl(AffOpt.mops)]
        internal Aff(ThunkAsync<A> thunk) =>
            this.thunk = thunk ?? throw new ArgumentNullException(nameof(thunk));

        /// <summary>
        /// Invoke the effect
        /// </summary>
        [Pure, MethodImpl(AffOpt.mops)]
        public ValueTask<Fin<A>> RunIO() =>
            thunk.Value();

        /// <summary>
        /// Launch the async process without awaiting the result
        /// </summary>
        /// <returns></returns>
        [MethodImpl(AffOpt.mops)]
        public Aff<Unit> FireAndForget()
        {
            var t = thunk;
            return Aff<Unit>(() => { 
                ignore(t.Value());
                return unit.AsValueTask();
            });
        }

        /// <summary>
        /// Custom awaiter so Aff can be used with async/await 
        /// </summary>
        public AffAwaiter<A> GetAwaiter() =>
            new AffAwaiter<A>(this);
        
        /// <summary>
        /// Lift an asynchronous effect into the Aff monad
        /// </summary>
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<A> Effect(Func<ValueTask<A>> f) =>
            new Aff<A>(ThunkAsync<A>.Lazy(f));

        /// <summary>
        /// Lift an asynchronous effect into the Aff monad
        /// </summary>
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<A> EffectMaybe(Func<ValueTask<Fin<A>>> f) =>
            new Aff<A>(ThunkAsync<A>.Lazy(f));
        
        /// <summary>
        /// Lift an asynchronous effect into the Aff monad
        /// </summary>
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Unit> Effect(Func<ValueTask> f) =>
            new Aff<Unit>(ThunkAsync<Unit>.Lazy(async () =>
            {
                await f().ConfigureAwait(false);
                return Fin<Unit>.Succ(default);
            }));

        /// <summary>
        /// Lift a value into the Aff monad 
        /// </summary>
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<A> Success(A value) =>
            new Aff<A>(ThunkAsync<A>.Success(value));

        /// <summary>
        /// Lift a failure into the Aff monad 
        /// </summary>
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<A> Fail(Error error) =>
            new Aff<A>(ThunkAsync<A>.Fail(error));

        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<A> operator |(Aff<A> ma, Aff<A> mb) => new Aff<A>(ThunkAsync<A>.Lazy(
            async () =>
            {
                var ra = await ma.RunIO().ConfigureAwait(false);
                return ra.IsSucc
                    ? ra
                    : await mb.RunIO().ConfigureAwait(false);
            }));

        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<A> operator |(Aff<A> ma, Eff<A> mb) => new Aff<A>(ThunkAsync<A>.Lazy(
            async () =>
            {
                var ra = await ma.RunIO().ConfigureAwait(false);
                return ra.IsSucc
                    ? ra
                    : mb.RunIO();
            }));

        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<A> operator |(Eff<A> ma, Aff<A> mb) => new Aff<A>(ThunkAsync<A>.Lazy(
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
        public static implicit operator Aff<A>(Eff<A> ma) =>
            EffectMaybe(() => ma.RunIO().AsValueTask());
    }
    
    public static partial class AffExtensions
    {
        //
        // Map
        //
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<B> Map<A, B>(this Aff<A> ma, Func<A, B> f) =>
            new Aff<B>(ma.thunk.Map(f));

        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<B> MapAsync<A, B>(this Aff<A> ma, Func<A, ValueTask<B>> f) =>
            new Aff<B>(ma.thunk.MapAsync(f));

        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<A> MapFail<A>(this Aff<A> ma, Func<Error, Error> f) =>
            ma.BiMap(identity, f);

        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<A> MapFailAsync<A>(this Aff<A> ma, Func<Error, ValueTask<Error>> f) =>
            ma.BiMapAsync(x => x.AsValueTask(), f);

        //
        // Bi-map
        //
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<B> BiMap<A, B>(this Aff<A> ma, Func<A, B> Succ, Func<Error, Error> Fail) =>
            new Aff<B>(ma.thunk.BiMap(Succ, Fail));

        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<B> BiMapAsync<A, B>(this Aff<A> ma, Func<A, ValueTask<B>> Succ, Func<Error, ValueTask<Error>> Fail) =>
            new Aff<B>(ma.thunk.BiMapAsync(Succ, Fail));

        //
        // Match
        //

        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<B> Match<A, B>(this Aff<A> ma, Func<A, B> Succ, Func<Error, B> Fail) =>
            Aff(async () => { 
                var r = await ma.RunIO().ConfigureAwait(false);
                return r.IsSucc
                    ? Succ(r.Value)
                    : Fail(r.Error);
            });
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, B> Match<Env, A, B>(this Aff<A> ma, Func<A, B> Succ, Aff<Env, B> Fail) where Env : struct, HasCancel<Env> =>
            AffMaybe<Env, B>(async env => {
                var r = await ma.RunIO().ConfigureAwait(false);
                return r.IsSucc
                    ? Succ(r.Value)
                    : await Fail.RunIO(env).ConfigureAwait(false);
            });

        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, B> Match<Env, A, B>(this Aff<A> ma, Aff<Env, B> Succ, Func<Error, B> Fail) where Env : struct, HasCancel<Env> =>
            AffMaybe<Env, B>(async env => {
                var r = await ma.RunIO().ConfigureAwait(false);
                return r.IsSucc
                    ? await Succ.RunIO(env).ConfigureAwait(false)
                    : Fail(r.Error);
            });

        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<B> Match<A, B>(this Aff<A> ma, Aff<B> Succ, Func<Error, B> Fail) =>
            AffMaybe<B>(async () =>
            {
                var r = await ma.RunIO().ConfigureAwait(false);
                return r.IsSucc
                           ? await Succ.RunIO().ConfigureAwait(false)
                           : Fail(r.Error);
            });

        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, B> Match<Env, A, B>(this Aff<A> ma, Aff<Env, B> Succ, Aff<Env, B> Fail) where Env : struct, HasCancel<Env> =>
            AffMaybe<Env, B>(async env =>
            {
                var r = await ma.RunIO().ConfigureAwait(false);
                return r.IsSucc
                    ? await Succ.RunIO(env).ConfigureAwait(false)
                    : await Fail.RunIO(env).ConfigureAwait(false);
            });

        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<B> Match<A, B>(this Aff<A> ma, Aff<B> Succ, Aff<B> Fail) =>
            AffMaybe<B>(async () =>
            {
                var r = await ma.RunIO().ConfigureAwait(false);
                return r.IsSucc
                    ? await Succ.RunIO().ConfigureAwait(false)
                    : await Fail.RunIO().ConfigureAwait(false);
            });

        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<B> Match<A, B>(this Aff<A> ma, B Succ, Func<Error, B> Fail) =>
            Aff<B>(async () =>
            {
                var r = await ma.RunIO().ConfigureAwait(false);
                return r.IsSucc
                    ? Succ
                    : Fail(r.Error);
            });

        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<B> Match<A, B>(this Aff<A> ma, Func<A, B> Succ, B Fail) =>
            Aff<B>(async () =>
            {
                var r = await ma.RunIO().ConfigureAwait(false);
                return r.IsSucc
                           ? Succ(r.Value)
                           : Fail;
            });

        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<B> Match<A, B>(this Aff<A> ma, B Succ, B Fail) =>
            Aff<B>(async () =>
            {
                var r = await ma.RunIO().ConfigureAwait(false);
                return r.IsSucc
                           ? Succ
                           : Fail;
            });
        
        //
        // IfNone
        //
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<A> IfFail<A>(this Aff<A> ma, Func<Error, A> f) =>
            AffMaybe<A>(
                async () =>
                {
                    var res = await ma.RunIO().ConfigureAwait(false);
                    return res.IsSucc
                               ? res
                               : Fin<A>.Succ(f(res.Error));
                });
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<A> IfFail<A>(this Aff<A> ma, A alternative) =>
            AffMaybe<A>(
                async () =>
                {
                    var res = await ma.RunIO().ConfigureAwait(false);
                    return res.IsSucc
                               ? res
                               : Fin<A>.Succ(alternative);
                });
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, A> IfFail<Env, A>(this Aff<A> ma, Aff<Env, A> alternative) where Env : struct, HasCancel<Env> =>
            AffMaybe<Env, A>(
                async env =>
                {
                    var res = await ma.RunIO().ConfigureAwait(false);
                    return res.IsSucc
                               ? res
                               : await alternative.RunIO(env).ConfigureAwait(false);
                });
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<A> IfFail<A>(this Aff<A> ma, Aff<A> alternative) =>
            AffMaybe<A>(
                async () =>
                {
                    var res = await ma.RunIO().ConfigureAwait(false);
                    return res.IsSucc
                               ? res
                               : await alternative.RunIO().ConfigureAwait(false);
                });
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, A> IfFail<Env, A>(this Aff<A> ma, Eff<Env, A> alternative) where Env : struct, HasCancel<Env> =>
            AffMaybe<Env, A>(
                async env =>
                {
                    var res = await ma.RunIO().ConfigureAwait(false);
                    return res.IsSucc
                               ? res
                               : alternative.RunIO(env);
                });
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<A> IfFail<A>(this Aff<A> ma, Eff<A> alternative) =>
            AffMaybe<A>(
                async () =>
                {
                    var res = await ma.RunIO().ConfigureAwait(false);
                    return res.IsSucc
                               ? res
                               : alternative.RunIO();
                });
        
        //
        // Iter / Do
        //
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Unit> Iter<A>(this Aff<A> ma, Func<A, Unit> f) =>
            Aff<Unit>(
                async () =>
                {
                    var res = await ma.RunIO().ConfigureAwait(false);
                    if (res.IsSucc)
                    {
                        f(res.Value);
                    }
                    return unit;
                });
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, Unit> Iter<Env, A>(this Aff<A> ma, Func<A, Aff<Env, Unit>> f) where Env : struct, HasCancel<Env> =>
            Aff<Env, Unit>(
                async env =>
                {
                    var res = await ma.RunIO().ConfigureAwait(false);
                    if (res.IsSucc)
                    {
                        ignore(await f(res.Value).RunIO(env).ConfigureAwait(false));
                    }
                    return unit;
                });
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Unit> Iter<A>(this Aff<A> ma, Func<A, Aff<Unit>> f) =>
            Aff<Unit>(
                async () =>
                {
                    var res = await ma.RunIO().ConfigureAwait(false);
                    if (res.IsSucc)
                    {
                        ignore(await f(res.Value).RunIO().ConfigureAwait(false));
                    }
                    return unit;
                });
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, Unit> Iter<Env, A>(this Aff<A> ma, Func<A, Eff<Env, Unit>> f) where Env : struct, HasCancel<Env> =>
            Aff<Env, Unit>(
                async env =>
                {
                    var res = await ma.RunIO().ConfigureAwait(false);
                    if (res.IsSucc)
                    {
                        ignore(f(res.Value).RunIO(env));
                    }
                    return unit;
                });
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Unit> Iter<A>(this Aff<A> ma, Func<A, Eff<Unit>> f) =>
            Aff<Unit>(
                async () =>
                {
                    var res = await ma.RunIO().ConfigureAwait(false);
                    if (res.IsSucc)
                    {
                        ignore(f(res.Value).RunIO());
                    }
                    return unit;
                });
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<A> Do<A>(this Aff<A> ma, Func<A, Unit> f) =>
            AffMaybe<A>(
                async () =>
                {
                    var res = await ma.RunIO().ConfigureAwait(false);
                    if (res.IsSucc)
                    {
                        f(res.Value);
                    }
                    return res;
                });
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, A> Do<Env, A>(this Aff<A> ma, Func<A, Aff<Env, Unit>> f) where Env : struct, HasCancel<Env> =>
            AffMaybe<Env, A>(
                async env =>
                {
                    var res = await ma.RunIO().ConfigureAwait(false);
                    if (res.IsSucc)
                    {
                        var ures = await f(res.Value).RunIO(env).ConfigureAwait(false);
                        return ures.IsFail
                                   ? Fin<A>.Fail(ures.Error)
                                   : res;
                    }
                    return res;
                });
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, A> Do<Env, A>(this Aff<A> ma, Func<A, Eff<Env, Unit>> f) where Env : struct, HasCancel<Env> =>
            AffMaybe<Env, A>(
                async env =>
                {
                    var res = await ma.RunIO().ConfigureAwait(false);
                    if (res.IsSucc)
                    {
                        var ures = f(res.Value).RunIO(env);
                        return ures.IsFail
                                   ? Fin<A>.Fail(ures.Error)
                                   : res;
                    }
                    return res;
                });
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<A> Do<A>(this Aff<A> ma, Func<A, Aff<Unit>> f) =>
            AffMaybe<A>(
                async () =>
                {
                    var res = await ma.RunIO().ConfigureAwait(false);
                    if (res.IsSucc)
                    {
                        var ures = await f(res.Value).RunIO().ConfigureAwait(false);
                        return ures.IsFail
                                   ? Fin<A>.Fail(ures.Error)
                                   : res;
                    }
                    return res;
                });
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<A> Do<A>(this Aff<A> ma, Func<A, Eff<Unit>> f) =>
            AffMaybe<A>(
                async () =>
                {
                    var res = await ma.RunIO().ConfigureAwait(false);
                    if (res.IsSucc)
                    {
                        var ures = f(res.Value).RunIO();
                        return ures.IsFail
                                   ? Fin<A>.Fail(ures.Error)
                                   : res;
                    }
                    return res;
                });
        
        //
        // Filter
        //

        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<A> Filter<A>(this Aff<A> ma, Func<A, bool> f) =>
            ma.Bind(x => f(x) ? SuccessEff<A>(x) : FailEff<A>(Error.New(Thunk.CancelledText)));        

        //
        // Bind
        //

        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, B> Bind<Env, A, B>(this Aff<A> ma, Func<A, Aff<Env, B>> f) where Env : struct, HasCancel<Env> =>
            new Aff<Env, B>(ma.thunk.Map(x => ThunkFromIO(f(x))).Flatten());

        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<B> Bind<A, B>(this Aff<A> ma, Func<A, Aff<B>> f) =>
            new Aff<B>(ma.thunk.Map(x => ThunkFromIO(f(x))).Flatten());

        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, B> Bind<Env, A, B>(this Aff<A> ma, Func<A, Eff<Env, B>> f) where Env : struct, HasCancel<Env> =>
            new Aff<Env, B>(ma.thunk.Map(x => ThunkFromIO(f(x))).Flatten());

        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<B> Bind<A, B>(this Aff<A> ma, Func<A, Eff<B>> f) =>
            new Aff<B>(ma.thunk.Map(x => ThunkFromIO(f(x))).Flatten());

        //
        // Bi-bind
        //

        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, B> BiBind<Env, A, B>(this Aff<A> ma, Func<A, Aff<Env, B>> Succ, Func<Error, Aff<Env, B>> Fail) where Env : struct, HasCancel<Env> =>
            ma.Match(Succ, Fail)
              .Flatten();

        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<B> BiBind<A, B>(this Aff<A> ma, Func<A, Aff<B>> Succ, Func<Error, Aff<B>> Fail) =>
            ma.Match(Succ, Fail)
              .Flatten();

        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, B> BiBind<Env, A, B>(this Aff<A> ma, Func<A, Eff<Env, B>> Succ, Func<Error, Eff<Env, B>> Fail) where Env : struct, HasCancel<Env> =>
            ma.Match(Succ, Fail)
              .Flatten();

        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<B> BiBind<A, B>(this Aff<A> ma, Func<A, Eff<B>> Succ, Func<Error, Eff<B>> Fail) =>
            ma.Match(Succ, Fail)
              .Flatten();

        //
        // Flatten
        //
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<A> Flatten<A>(this Aff<Aff<A>> ma) =>
            new Aff<A>(ma.thunk.Map(ThunkFromIO).Flatten());
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, A> Flatten<Env, A>(this Aff<Aff<Env, A>> ma) where Env : struct, HasCancel<Env> =>
            new Aff<Env, A>(ma.thunk.Map(ThunkFromIO).Flatten());
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<A> Flatten<A>(this Aff<Eff<A>> ma) =>
            new Aff<A>(ma.thunk.Map(ThunkFromIO).Flatten());
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, A> Flatten<Env, A>(this Aff<Eff<Env, A>> ma) where Env : struct, HasCancel<Env> =>
            new Aff<Env, A>(ma.thunk.Map(ThunkFromIO).Flatten());

        //
        // Select
        //
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<B> Select<A, B>(this Aff<A> ma, Func<A, B> f) =>
            Map(ma, f);

        //
        // SelectMany
        //
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, B> SelectMany<Env, A, B>(this Aff<A> ma, Func<A, Aff<Env, B>> f) where Env : struct, HasCancel<Env> =>
            Bind(ma, f);
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<B> SelectMany<A, B>(this Aff<A> ma, Func<A, Aff<B>> f) =>
            Bind(ma, f);
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, B> SelectMany<Env, A, B>(this Aff<A> ma, Func<A, Eff<Env, B>> f) where Env : struct, HasCancel<Env> =>
            Bind(ma, f);
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<B> SelectMany<A, B>(this Aff<A> ma, Func<A, Eff<B>> f) =>
            Bind(ma, f);
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, C> SelectMany<Env, A, B, C>(this Aff<A> ma, Func<A, Aff<Env, B>> bind, Func<A, B, C> project) where Env : struct, HasCancel<Env> =>
            Bind(ma, x => Map(bind(x), y => project(x, y)));
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<C> SelectMany<A, B, C>(this Aff<A> ma, Func<A, Aff<B>> bind, Func<A, B, C> project) =>
            Bind(ma, x => Map(bind(x), y => project(x, y)));
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, C> SelectMany<Env, A, B, C>(this Aff<A> ma, Func<A, Eff<Env, B>> bind, Func<A, B, C> project) where Env : struct, HasCancel<Env> =>
            Bind(ma, x => Map(bind(x), y => project(x, y)));
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<C> SelectMany<A, B, C>(this Aff<A> ma, Func<A, Eff<B>> bind, Func<A, B, C> project) =>
            Bind(ma, x => Map(bind(x), y => project(x, y)));
        
        //
        // Where
        //
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<A> Where<A>(this Aff<A> ma, Func<A, bool> f) =>
            Filter(ma, f);
        
        //
        // Zip
        //
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<(A, B)> Zip<A, B>(Aff<A> ma, Aff<B> mb) =>
            new Aff<(A, B)>(ThunkAsync<(A, B)>.Lazy(async () =>
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
        public static Aff<(A, B)> Zip<A, B>(Aff<A> ma, Eff<B> mb) =>
            new Aff<(A, B)>(ThunkAsync<(A, B)>.Lazy(async () =>
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
        public static Aff<(A, B)> Zip<A, B>(Eff<A> ma, Aff<B> mb) =>
            new Aff<(A, B)>(ThunkAsync<(A, B)>.Lazy(async () =>
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
        static ThunkAsync<A> ThunkFromIO<A>(Aff<A> ma) =>
            ma.thunk;
        
    }    
}
