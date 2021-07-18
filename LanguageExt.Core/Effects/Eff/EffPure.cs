using System;
using LanguageExt.Common;
using System.Threading.Tasks;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using LanguageExt.Effects.Traits;
using LanguageExt.Thunks;

namespace LanguageExt
{
    /// <summary>
    /// Synchronous IO monad
    /// </summary>
    public readonly struct Eff<A>
    {
        internal Thunk<A> Thunk => thunk ?? Thunk<A>.Fail(Errors.Bottom);
        readonly Thunk<A> thunk;

        /// <summary>
        /// Constructor
        /// </summary>
        [MethodImpl(AffOpt.mops)]
        internal Eff(Thunk<A> thunk) =>
            this.thunk = thunk ?? throw new ArgumentNullException(nameof(thunk));

        /// <summary>
        /// Invoke the effect
        /// </summary>
        [Pure, MethodImpl(AffOpt.mops)]
        public Fin<A> Run() =>
            Thunk.Value();

        /// <summary>
        /// Invoke the effect
        /// </summary>
        [Pure, MethodImpl(AffOpt.mops)]
        public Fin<A> ReRun() =>
            Thunk.ReValue();

        /// <summary>
        /// Clone the effect
        /// </summary>
        /// <remarks>
        /// If the effect had already run, then this state will be wiped in the clone, meaning it can be re-run
        /// </remarks>
        [Pure, MethodImpl(AffOpt.mops)]
        public Eff<A> Clone() =>
            new Eff<A>(Thunk.Clone());        

        /// <summary>
        /// Invoke the effect
        /// </summary>
        [MethodImpl(AffOpt.mops)]
        public Unit RunUnit() =>
            ignore(Thunk.Value());

        /// <summary>
        /// Lift a synchronous effect into the IO monad
        /// </summary>
        [Pure, MethodImpl(AffOpt.mops)]
        public static Eff<A> EffectMaybe(Func<Fin<A>> f) =>
            new Eff<A>(Thunk<A>.Lazy(f));

        /// <summary>
        /// Lift a synchronous effect into the IO monad
        /// </summary>
        [Pure, MethodImpl(AffOpt.mops)]
        public static Eff<A> Effect(Func<A> f) =>
            new Eff<A>(Thunk<A>.Lazy(() => Fin<A>.Succ(f())));

        /// <summary>
        /// Lift a value into the IO monad 
        /// </summary>
        [Pure, MethodImpl(AffOpt.mops)]
        public static Eff<A> Success(A value) =>
            new Eff<A>(Thunk<A>.Success(value));

        /// <summary>
        /// Lift a failure into the IO monad 
        /// </summary>
        [Pure, MethodImpl(AffOpt.mops)]
        public static Eff<A> Fail(Error error) =>
            new Eff<A>(Thunk<A>.Fail(error));

        [Pure, MethodImpl(AffOpt.mops)]
        public static Eff<A> operator |(Eff<A> ma, Eff<A> mb) => new Eff<A>(Thunk<A>.Lazy(
            () =>
            {
                var ra = ma.ReRun();
                return ra.IsSucc
                    ? ra
                    : mb.ReRun();
            }));

        [Pure, MethodImpl(AffOpt.mops)]
        public static Eff<A> operator |(Eff<A> ma, EffCatch<A> mb) =>
            new Eff<A>(Thunk<A>.Lazy(
                () =>
                {
                    var ra = ma.ReRun();
                    return ra.IsSucc
                        ? ra
                        : mb.Run(ra.Error);
                }));

        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<A> operator |(Eff<A> ma, AffCatch<A> mb) =>
            new Aff<A>(ThunkAsync<A>.Lazy(
                async () =>
                {
                    var ra = ma.ReRun();
                    return ra.IsSucc
                        ? ra
                        : await mb.Run(ra.Error).ConfigureAwait(false);
                }));

        [Pure, MethodImpl(AffOpt.mops)]
        public static Eff<A> operator |(Eff<A> ma, CatchValue<A> value) =>
            new Eff<A>(Thunk<A>.Lazy(
                           () =>
                           {
                               var ra = ma.ReRun();
                               return ra.IsSucc
                                          ? ra
                                          : value.Match(ra.Error)
                                              ? FinSucc(value.Value(ra.Error))
                                              : ra;
                           }));
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Eff<A> operator |(Eff<A> ma, CatchError value) =>
            new Eff<A>(Thunk<A>.Lazy(
                           () =>
                           {
                               var ra = ma.ReRun();
                               return ra.IsSucc
                                          ? ra
                                          : value.Match(ra.Error)
                                              ? FinFail<A>(value.Value(ra.Error))
                                              : ra;
                           }));
        
        [Pure, MethodImpl(AffOpt.mops)]
        public Eff<Env, A> WithEnv<Env>() where Env : struct 
        {
            var self = this;
            return Eff<Env, A>.EffectMaybe(e => self.ReRun());
        }
        
        [Pure, MethodImpl(AffOpt.mops)]
        public Aff<A> ToAsync() 
        {
            var self = this;
            return Aff<A>.EffectMaybe(() => new ValueTask<Fin<A>>(self.ReRun()));
        }
        
        [Pure, MethodImpl(AffOpt.mops)]
        public Aff<Env, A> ToAsyncWithEnv<Env>() where Env : struct, HasCancel<Env>
        {
            var self = this;
            return Aff<Env, A>.EffectMaybe(e => new ValueTask<Fin<A>>(self.ReRun()));
        }

        [Pure, MethodImpl(AffOpt.mops)]
        public S Fold<S>(S state, Func<S, A, S> f)
        {
            var r = Run();
            return r.IsSucc
                ? f(state, r.Value)
                : state;
        }

        [Pure, MethodImpl(AffOpt.mops)]
        public bool Exists(Func<A, bool> f)
        {
            var r = Run();
            return r.IsSucc && f(r.Value);
        }

        [Pure, MethodImpl(AffOpt.mops)]
        public bool ForAll(Func<A, bool> f)
        {
            var r = Run();
            return r.IsFail || (r.IsSucc && f(r.Value));
        }
    }
    
    public static partial class AffExtensions
    {
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<A> ToAsync<A>(this Eff<A> ma) =>
            Aff<A>.EffectMaybe(() => new ValueTask<Fin<A>>(ma.ReRun()));

        [Pure, MethodImpl(AffOpt.mops)]
        public static Eff<B> Map<A, B>(this Eff<A> ma, Func<A, B> f) =>
            new Eff<B>(ma.Thunk.Map(f));
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, B> MapAsync<Env, A, B>(this Eff<A> ma, Func<A, ValueTask<B>> f) where Env : struct, HasCancel<Env> =>
            new Aff<Env, B>(ma.Thunk.MapAsync<Env, A, B>(f));
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<B> MapAsync<A, B>(this Eff<A> ma, Func<A, ValueTask<B>> f) =>
            new Aff<B>(ma.Thunk.MapAsync(f));

        [Pure, MethodImpl(AffOpt.mops)]
        public static Eff<B> BiMap<A, B>(this Eff<A> ma, Func<A, B> Succ, Func<Error, Error> Fail) =>
            new Eff<B>(ma.Thunk.BiMap(Succ, Fail));

        //
        // Match
        // 
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Eff<B> Match<A, B>(this Eff<A> ma, Func<A, B> Succ, Func<Error, B> Fail) =>
            Eff(() => { 
                var r = ma.ReRun();
                return r.IsSucc
                    ? Succ(r.Value)
                    : Fail(r.Error);
            });
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, B> MatchAff<Env, A, B>(this Eff<A> ma, Func<A, B> Succ, Aff<Env, B> Fail) where Env : struct, HasCancel<Env> =>
            AffMaybe<Env, B>(async env =>
            {
                var r = ma.ReRun();
                return r.IsSucc
                    ? Succ(r.Value)
                    : await Fail.ReRun(env).ConfigureAwait(false);
            });
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, B> MatchAff<Env, A, B>(this Eff<A> ma, Func<A, B> Succ, Func<Error, Aff<Env, B>> Fail) where Env : struct, HasCancel<Env> =>
            AffMaybe<Env, B>(async env =>
                             {
                                 var r = ma.ReRun();
                                 return r.IsSucc
                                            ? Succ(r.Value)
                                            : await Fail(r.Error).Run(env).ConfigureAwait(false);
                             });
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Eff<Env, B> MatchEff<Env, A, B>(this Eff<A> ma, Func<A, B> Succ, Eff<Env, B> Fail) where Env : struct =>
            EffMaybe<Env, B>(env =>
            {
                var r = ma.ReRun();
                return r.IsSucc
                    ? Succ(r.Value)
                    : Fail.ReRun(env);
            });
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Eff<Env, B> MatchEff<Env, A, B>(this Eff<A> ma, Func<A, B> Succ, Func<Error, Eff<Env, B>> Fail) where Env : struct =>
            EffMaybe<Env, B>(env =>
                             {
                                 var r = ma.ReRun();
                                 return r.IsSucc
                                            ? Succ(r.Value)
                                            : Fail(r.Error).Run(env);
                             });
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Eff<B> MatchEff<A, B>(this Eff<A> ma, Func<A, B> Succ, Eff<B> Fail) =>
            EffMaybe<B>(() =>
            {
                var r = ma.ReRun();
                return r.IsSucc
                    ? Succ(r.Value)
                    : Fail.ReRun();
            });
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Eff<B> MatchEff<A, B>(this Eff<A> ma, Func<A, B> Succ, Func<Error, Eff<B>> Fail) =>
            EffMaybe<B>(() =>
                        {
                            var r = ma.ReRun();
                            return r.IsSucc
                                       ? Succ(r.Value)
                                       : Fail(r.Error).Run();
                        });

        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, B> MatchAff<Env, A, B>(this Eff<A> ma, Aff<Env, B> Succ, Func<Error, B> Fail) where Env : struct, HasCancel<Env> =>
            AffMaybe<Env, B>(async env =>
            {
                var r = ma.ReRun();
                return r.IsSucc
                    ? await Succ.ReRun(env).ConfigureAwait(false)
                    : Fail(r.Error);
            });

        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, B> MatchAff<Env, A, B>(this Eff<A> ma, Func<A, Aff<Env, B>> Succ, Func<Error, B> Fail) where Env : struct, HasCancel<Env> =>
            AffMaybe<Env, B>(async env =>
                             {
                                 var r = ma.ReRun();
                                 return r.IsSucc
                                            ? await Succ(r.Value).Run(env).ConfigureAwait(false)
                                            : Fail(r.Error);
                             });

        [Pure, MethodImpl(AffOpt.mops)]
        public static Eff<Env, B> MatchEff<Env, A, B>(this Eff<A> ma, Eff<Env, B> Succ, Func<Error, B> Fail) where Env : struct =>
            EffMaybe<Env, B>(env =>
            {
                var r = ma.ReRun();
                return r.IsSucc
                    ? Succ.ReRun(env)
                    : Fail(r.Error);
            });

        [Pure, MethodImpl(AffOpt.mops)]
        public static Eff<Env, B> MatchEff<Env, A, B>(this Eff<A> ma, Func<A, Eff<Env, B>> Succ, Func<Error, B> Fail) where Env : struct =>
            EffMaybe<Env, B>(env =>
                             {
                                 var r = ma.ReRun();
                                 return r.IsSucc
                                            ? Succ(r.Value).Run(env)
                                            : Fail(r.Error);
                             });

        [Pure, MethodImpl(AffOpt.mops)]
        public static Eff<B> MatchEff<A, B>(this Eff<A> ma, Eff<B> Succ, Func<Error, B> Fail) =>
            EffMaybe<B>(() =>
            {
                var r = ma.ReRun();
                return r.IsSucc
                    ? Succ.ReRun()
                    : Fail(r.Error);
            });

        [Pure, MethodImpl(AffOpt.mops)]
        public static Eff<B> MatchEff<A, B>(this Eff<A> ma, Func<A, Eff<B>> Succ, Func<Error, B> Fail) =>
            EffMaybe<B>(() =>
                        {
                            var r = ma.ReRun();
                            return r.IsSucc
                                       ? Succ(r.Value).Run()
                                       : Fail(r.Error);
                        });

        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, B> MatchAff<Env, A, B>(this Eff<A> ma, Aff<Env, B> Succ, Aff<Env, B> Fail) where Env : struct, HasCancel<Env> =>
            AffMaybe<Env, B>(async env =>
            {
                var r = ma.ReRun();
                return r.IsSucc
                    ? await Succ.ReRun(env).ConfigureAwait(false)
                    : await Fail.ReRun(env).ConfigureAwait(false);
            });

        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, B> MatchAff<Env, A, B>(this Eff<A> ma, Func<A, Aff<Env, B>> Succ, Func<Error, Aff<Env, B>> Fail) where Env : struct, HasCancel<Env> =>
            AffMaybe<Env, B>(async env =>
                             {
                                 var r = ma.ReRun();
                                 return r.IsSucc
                                            ? await Succ(r.Value).Run(env).ConfigureAwait(false)
                                            : await Fail(r.Error).Run(env).ConfigureAwait(false);
                             });

        [Pure, MethodImpl(AffOpt.mops)]
        public static Eff<Env, B> MatchEff<Env, A, B>(this Eff<A> ma, Eff<Env, B> Succ, Eff<Env, B> Fail) where Env : struct =>
            EffMaybe<Env, B>(env =>
            {
                var r = ma.ReRun();
                return r.IsSucc
                    ? Succ.ReRun(env)
                    : Fail.ReRun(env);
            });

        [Pure, MethodImpl(AffOpt.mops)]
        public static Eff<Env, B> MatchEff<Env, A, B>(this Eff<A> ma, Func<A, Eff<Env, B>> Succ, Func<Error, Eff<Env, B>> Fail) where Env : struct =>
            EffMaybe<Env, B>(env =>
            {
                var r = ma.ReRun();
                return r.IsSucc
                    ? Succ(r.Value).Run(env)
                    : Fail(r.Error).Run(env);
            });

        [Pure, MethodImpl(AffOpt.mops)]
        public static Eff<B> MatchEff<A, B>(this Eff<A> ma, Eff<B> Succ, Eff<B> Fail) =>
            EffMaybe<B>(() =>
            {
                var r = ma.ReRun();
                return r.IsSucc
                    ? Succ.ReRun()
                    : Fail.ReRun();
            });

        [Pure, MethodImpl(AffOpt.mops)]
        public static Eff<B> MatchEff<A, B>(this Eff<A> ma, Func<A, Eff<B>> Succ, Func<Error, Eff<B>> Fail) =>
            EffMaybe<B>(() =>
                        {
                            var r = ma.ReRun();
                            return r.IsSucc
                                       ? Succ(r.Value).Run()
                                       : Fail(r.Error).Run();
                        });

        [Pure, MethodImpl(AffOpt.mops)]
        public static Eff<B> Match<A, B>(this Eff<A> ma, B Succ, Func<Error, B> Fail) =>
            Eff<B>(() =>
            {
                var r = ma.ReRun();
                return r.IsSucc
                    ? Succ
                    : Fail(r.Error);
            });

        [Pure, MethodImpl(AffOpt.mops)]
        public static Eff<B> Match<Env, A, B>(this Eff<A> ma, Func<A, B> Succ, B Fail) =>
            Eff<B>(() =>
            {
                var r = ma.ReRun();
                return r.IsSucc
                           ? Succ(r.Value)
                           : Fail;
            });

        [Pure, MethodImpl(AffOpt.mops)]
        public static Eff<Env, B> Match<Env, A, B>(this Eff<A> ma, B Succ, B Fail) where Env : struct =>
            Eff<Env, B>(env =>
            {
                var r = ma.ReRun();
                return r.IsSucc
                           ? Succ
                           : Fail;
            });

        [Pure, MethodImpl(AffOpt.mops)]
        public static Eff<B> Match<A, B>(this Eff<A> ma, B Succ, B Fail) =>
            Eff<B>(() =>
            {
                var r = ma.ReRun();
                return r.IsSucc
                           ? Succ
                           : Fail;
            });
        
        //
        // IfNone
        //
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Eff<A> IfFail<A>(this Eff<A> ma, Func<Error, A> f) =>
            EffMaybe<A>(
                () =>
                {
                    var res = ma.ReRun();
                    return res.IsSucc
                               ? res
                               : Fin<A>.Succ(f(res.Error));
                });
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Eff<A> IfFail<Env, A>(this Eff<A> ma, A alternative) =>
            EffMaybe<A>(
                () =>
                {
                    var res = ma.ReRun();
                    return res.IsSucc
                               ? res
                               : Fin<A>.Succ(alternative);
                });
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, A> IfFailAff<Env, A>(this Eff<A> ma, Aff<Env, A> alternative) where Env : struct, HasCancel<Env> =>
            AffMaybe<Env, A>(
                async env =>
                {
                    var res = ma.ReRun();
                    return res.IsSucc
                               ? res
                               : await alternative.ReRun(env).ConfigureAwait(false);
                });
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, A> IfFailAff<Env, A>(this Eff<A> ma, Func<Error, Aff<Env, A>> alternative) where Env : struct, HasCancel<Env> =>
            AffMaybe<Env, A>(
                async env =>
                {
                    var res = ma.ReRun();
                    return res.IsSucc
                               ? res
                               : await alternative(res.Error).Run(env).ConfigureAwait(false);
                });
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<A> IfFailAff<A>(this Eff<A> ma, Aff<A> alternative) =>
            AffMaybe<A>(
                async () =>
                {
                    var res = ma.ReRun();
                    return res.IsSucc
                               ? res
                               : await alternative.ReRun().ConfigureAwait(false);
                });
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<A> IfFailAff<A>(this Eff<A> ma, Func<Error, Aff<A>> alternative) =>
            AffMaybe<A>(
                async () =>
                {
                    var res = ma.ReRun();
                    return res.IsSucc
                               ? res
                               : await alternative(res.Error).Run().ConfigureAwait(false);
                });
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Eff<Env, A> IfFailEff<Env, A>(this Eff<A> ma, Eff<Env, A> alternative) where Env : struct =>
            EffMaybe<Env, A>(
                env =>
                {
                    var res = ma.ReRun();
                    return res.IsSucc
                               ? res
                               : alternative.ReRun(env);
                });
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Eff<Env, A> IfFailEff<Env, A>(this Eff<A> ma, Func<Error, Eff<Env, A>> alternative) where Env : struct =>
            EffMaybe<Env, A>(
                env =>
                {
                    var res = ma.ReRun();
                    return res.IsSucc
                               ? res
                               : alternative(res.Error).Run(env);
                });
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Eff<A> IfFailEff<A>(this Eff<A> ma, Eff<A> alternative) =>
            EffMaybe<A>(
                () =>
                {
                    var res = ma.ReRun();
                    return res.IsSucc
                               ? res
                               : alternative.ReRun();
                });
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Eff<A> IfFailEff<A>(this Eff<A> ma, Func<Error, Eff<A>> alternative) =>
            EffMaybe<A>(
                () =>
                {
                    var res = ma.ReRun();
                    return res.IsSucc
                               ? res
                               : alternative(res.Error).Run();
                });
        
        //
        // Iter / Do
        //
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Eff<Unit> Iter<A>(this Eff<A> ma, Func<A, Unit> f) =>
            Eff<Unit>(
                () =>
                {
                    var res = ma.ReRun();
                    if (res.IsSucc)
                    {
                        f(res.Value);
                    }
                    return unit;
                });
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, Unit> Iter<Env, A>(this Eff<A> ma, Func<A, Aff<Env, Unit>> f) where Env : struct, HasCancel<Env> =>
            Aff<Env, Unit>(
                async env =>
                {
                    var res = ma.ReRun();
                    if (res.IsSucc)
                    {
                        ignore(await f(res.Value).Run(env).ConfigureAwait(false));
                    }
                    return unit;
                });
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Unit> Iter<A>(this Eff<A> ma, Func<A, Aff<Unit>> f) =>
            Aff<Unit>(
                async () =>
                {
                    var res = ma.ReRun();
                    if (res.IsSucc)
                    {
                        ignore(await f(res.Value).Run().ConfigureAwait(false));
                    }
                    return unit;
                });
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Eff<Env, Unit> Iter<Env, A>(this Eff<A> ma, Func<A, Eff<Env, Unit>> f) where Env : struct =>
            Eff<Env, Unit>(
                env =>
                {
                    var res = ma.ReRun();
                    if (res.IsSucc)
                    {
                        ignore(f(res.Value).Run(env));
                    }
                    return unit;
                });
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Eff<Unit> Iter<A>(this Eff<A> ma, Func<A, Eff<Unit>> f) =>
            Eff<Unit>(
                () =>
                {
                    var res = ma.ReRun();
                    if (res.IsSucc)
                    {
                        ignore(f(res.Value).Run());
                    }
                    return unit;
                });
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Eff<A> Do<A>(this Eff<A> ma, Func<A, Unit> f) =>
            EffMaybe<A>(
                () =>
                {
                    var res = ma.ReRun();
                    if (res.IsSucc)
                    {
                        f(res.Value);
                    }
                    return res;
                });
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, A> Do<Env, A>(this Eff<A> ma, Func<A, Aff<Env, Unit>> f) where Env : struct, HasCancel<Env> =>
            AffMaybe<Env, A>(
                async env =>
                {
                    var res = ma.ReRun();
                    if (res.IsSucc)
                    {
                        var ures = await f(res.Value).Run(env).ConfigureAwait(false);
                        return ures.IsFail
                                   ? Fin<A>.Fail(ures.Error)
                                   : res;
                    }
                    return res;
                });
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Eff<Env, A> Do<Env, A>(this Eff<A> ma, Func<A, Eff<Env, Unit>> f) where Env : struct =>
            EffMaybe<Env, A>(
                env =>
                {
                    var res = ma.ReRun();
                    if (res.IsSucc)
                    {
                        var ures = f(res.Value).Run(env);
                        return ures.IsFail
                                   ? Fin<A>.Fail(ures.Error)
                                   : res;
                    }
                    return res;
                });
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<A> Do<A>(this Eff<A> ma, Func<A, Aff<Unit>> f) =>
            AffMaybe<A>(
                async () =>
                {
                    var res = ma.ReRun();
                    if (res.IsSucc)
                    {
                        var ures = await f(res.Value).Run().ConfigureAwait(false);
                        return ures.IsFail
                                   ? Fin<A>.Fail(ures.Error)
                                   : res;
                    }
                    return res;
                });
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Eff<A> Do<A>(this Eff<A> ma, Func<A, Eff<Unit>> f) =>
            EffMaybe<A>(
                () =>
                {
                    var res = ma.ReRun();
                    if (res.IsSucc)
                    {
                        var ures = f(res.Value).Run();
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
        public static Eff<A> Filter<A>(this Eff<A> ma, Func<A, bool> f) =>
            ma.Bind(x => f(x) ? SuccessEff<A>(x) : FailEff<A>(Errors.Cancelled));        
        
        //
        // Bind
        //

        [Pure, MethodImpl(AffOpt.mops)]
        public static Eff<B> Bind<A, B>(this Eff<A> ma, Func<A, Eff<B>> f) =>
            new Eff<B>(ma.Thunk.Map(x => ThunkFromIO(f(x))).Flatten());

        [Pure, MethodImpl(AffOpt.mops)]
        public static Eff<Env, B> Bind<Env, A, B>(this Eff<A> ma, Func<A, Eff<Env, B>> f) where Env : struct =>
            new Eff<Env, B>(ma.Thunk.Map(x => ThunkFromIO(f(x))).Flatten());

        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<B> Bind<A, B>(this Eff<A> ma, Func<A, Aff<B>> f) =>
            new Aff<B>(ma.Thunk.Map(x => ThunkFromIO(f(x))).Flatten());

        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, B> Bind<Env, A, B>(this Eff<A> ma, Func<A, Aff<Env, B>> f) where Env : struct, HasCancel<Env> =>
            new Aff<Env, B>(ma.Thunk.Map(x => ThunkFromIO(f(x))).Flatten());
        
        //
        // Bi-bind
        //
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Eff<B> BiBind<A, B>(this Eff<A> ma, Func<A, Eff<B>> Succ, Func<Error, Eff<B>> Fail) =>
            ma.Match(Succ, Fail)
              .Flatten();

        [Pure, MethodImpl(AffOpt.mops)]
        public static Eff<Env, B> BiBind<Env, A, B>(this Eff<A> ma, Func<A, Eff<Env, B>> Succ, Func<Error, Eff<Env, B>> Fail) where Env : struct =>
            ma.Match(Succ, Fail)
              .Flatten();

        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<B> BiBind<A, B>(this Eff<A> ma, Func<A, Aff<B>> Succ, Func<Error, Aff<B>> Fail) =>
            ma.Match(Succ, Fail)
              .Flatten();

        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, B> BiBind<Env, A, B>(this Eff<A> ma, Func<A, Aff<Env, B>> Succ, Func<Error, Aff<Env, B>> Fail) where Env : struct, HasCancel<Env> =>
            ma.Match(Succ, Fail)
              .Flatten();

        //
        // Flatten
        //
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Eff<A> Flatten<A>(this Eff<Eff<A>> ma) =>
            new Eff<A>(ma.Thunk.Map(ThunkFromIO).Flatten());
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Eff<Env, A> Flatten<Env, A>(this Eff<Eff<Env, A>> ma) where Env : struct =>
            new Eff<Env, A>(ma.Thunk.Map(ThunkFromIO).Flatten());
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<A> Flatten<A>(this Eff<Aff<A>> ma) =>
            new Aff<A>(ma.Thunk.Map(ThunkFromIO).Flatten());
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, A> Flatten<Env, A>(this Eff<Aff<Env, A>> ma) where Env : struct, HasCancel<Env> =>
            new Aff<Env, A>(ma.Thunk.Map(ThunkFromIO).Flatten());

        //
        // Select
        //
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Eff<B> Select<A, B>(this Eff<A> ma, Func<A, B> f) =>
            Map(ma, f);

        //
        // SelectMany
        //
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Eff<B> SelectMany<A, B>(this Eff<A> ma, Func<A, Eff<B>> f) =>
            Bind(ma, f);
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Eff<Env, B> SelectMany<Env, A, B>(this Eff<A> ma, Func<A, Eff<Env, B>> f)  where Env : struct =>
            Bind(ma, f);
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<B> SelectMany<A, B>(this Eff<A> ma, Func<A, Aff<B>> f) =>
            Bind(ma, f);
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, B> SelectMany<Env, A, B>(this Eff<A> ma, Func<A, Aff<Env, B>> f) where Env : struct, HasCancel<Env> =>
            Bind(ma, f);
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Eff<C> SelectMany<A, B, C>(this Eff<A> ma, Func<A, Eff<B>> bind, Func<A, B, C> project) =>
            Bind(ma, x => Map(bind(x), y => project(x, y)));
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Eff<Env, C> SelectMany<Env, A, B, C>(this Eff<A> ma, Func<A, Eff<Env, B>> bind, Func<A, B, C> project) where Env : struct, HasCancel<Env> =>
            Bind(ma, x => Map(bind(x), y => project(x, y)));
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<C> SelectMany<A, B, C>(this Eff<A> ma, Func<A, Aff<B>> bind, Func<A, B, C> project) =>
            Bind(ma, x => Map(bind(x), y => project(x, y)));
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<Env, C> SelectMany<Env, A, B, C>(this Eff<A> ma, Func<A, Aff<Env, B>> bind, Func<A, B, C> project) where Env : struct, HasCancel<Env> =>
            Bind(ma, x => Map(bind(x), y => project(x, y)));

        //
        // Where
        //
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Eff<A> Where<A>(this Eff<A> ma, Func<A, bool> f) =>
            Filter(ma, f);

        //
        // Zip
        //
        
        [Pure, MethodImpl(AffOpt.mops)]
        public static Eff<(A, B)> Zip<A, B>(Eff<A> ma, Eff<B> mb) =>
            new Eff<(A, B)>(Thunk<(A, B)>.Lazy(() =>
            {
                var ta = ma.ReRun();
                if (ta.IsFail) return ta.Cast<(A, B)>();
                var tb = mb.ReRun();
                if (tb.IsFail) return tb.Cast<(A, B)>();
                return Fin<(A, B)>.Succ((ta.Value, tb.Value));
            }));
        
        [Pure, MethodImpl(AffOpt.mops)]
        static Thunk<A> ThunkFromIO<A>(Eff<A> ma) =>
            ma.Thunk;
    }
}
