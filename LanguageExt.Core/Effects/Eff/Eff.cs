using System;
using LanguageExt.Common;
using System.Threading.Tasks;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using LanguageExt.Effects.Traits;
using LanguageExt.Pipes;
using LanguageExt.Thunks;

namespace LanguageExt
{
    /// <summary>
    /// Synchronous IO monad
    /// </summary>
    public readonly struct Eff<RT, A>
        where RT : struct 
    {
        internal Thunk<RT, A> Thunk => thunk ?? Thunk<RT, A>.Fail(Errors.Bottom);
        readonly Thunk<RT, A> thunk;

        /// <summary>
        /// Constructor
        /// </summary>
        [MethodImpl(Opt.Default)]
        internal Eff(Thunk<RT, A> thunk) =>
            this.thunk = thunk ?? throw new ArgumentNullException(nameof(thunk));

        /// <summary>
        /// Invoke the effect
        /// </summary>
        [Pure, MethodImpl(Opt.Default)]
        public Fin<A> Run(RT env) =>
            Thunk.Value(env);

        /// <summary>
        /// Invoke the effect
        /// </summary>
        [Pure, MethodImpl(Opt.Default)]
        public Fin<A> ReRun(RT env) =>
            Thunk.ReValue(env);

        /// <summary>
        /// Clone the effect
        /// </summary>
        /// <remarks>
        /// If the effect had already run, then this state will be wiped in the clone, meaning it can be re-run
        /// </remarks>
        [Pure, MethodImpl(Opt.Default)]
        public Eff<RT, A> Clone() =>
            new Eff<RT, A>(Thunk.Clone());        

        /// <summary>
        /// Invoke the effect
        /// </summary>
        /// <remarks>
        /// Throws on error
        /// </remarks>
        [MethodImpl(Opt.Default)]
        public Unit RunUnit(RT env) =>
            Thunk.Value(env).Case switch
            {
                A _     => unit,
                Error e => e.Throw(),
                _       => throw new NotSupportedException()
            };

        /// <summary>
        /// Lift a synchronous effect into the IO monad
        /// </summary>
        [Pure, MethodImpl(Opt.Default)]
        public static Eff<RT, A> EffectMaybe(Func<RT, Fin<A>> f) =>
            new Eff<RT, A>(Thunk<RT, A>.Lazy(f));

        /// <summary>
        /// Lift a synchronous effect into the IO monad
        /// </summary>
        [Pure, MethodImpl(Opt.Default)]
        public static Eff<RT, A> Effect(Func<RT, A> f) =>
            new Eff<RT, A>(Thunk<RT, A>.Lazy(e => Fin<A>.Succ(f(e))));

        /// <summary>
        /// Lift a value into the IO monad 
        /// </summary>
        [Pure, MethodImpl(Opt.Default)]
        public static Eff<RT, A> Success(A value) =>
            new Eff<RT, A>(Thunk<RT, A>.Success(value));

        /// <summary>
        /// Lift a failure into the IO monad 
        /// </summary>
        [Pure, MethodImpl(Opt.Default)]
        public static Eff<RT, A> Fail(Error error) =>
            new Eff<RT, A>(Thunk<RT, A>.Fail(error));

        [Pure, MethodImpl(Opt.Default)]
        public static Eff<RT, A> operator |(Eff<RT, A> ma, Eff<RT, A> mb) =>
            new Eff<RT, A>(Thunk<RT, A>.Lazy(
                env =>
                {
                    var ra = ma.ReRun(env);
                    return ra.IsSucc
                        ? ra
                        : mb.ReRun(env);
                }));

        [Pure, MethodImpl(Opt.Default)]
        public static Eff<RT, A> operator |(Eff<RT, A> ma, Eff<A> mb) =>
            new Eff<RT, A>(Thunk<RT, A>.Lazy(
                e =>
                {
                    var ra = ma.ReRun(e);
                    return ra.IsSucc
                        ? ra
                        : mb.ReRun();
                }));

        [Pure, MethodImpl(Opt.Default)]
        public static Eff<RT, A> operator |(Eff<A> ma, Eff<RT, A> mb) =>
            new Eff<RT, A>(Thunk<RT, A>.Lazy(
                e =>
                {
                    var ra = ma.ReRun();
                    return ra.IsSucc
                        ? ra
                        : mb.ReRun(e);
                }));

        [Pure, MethodImpl(Opt.Default)]
        public static Eff<RT, A> operator |(Eff<RT, A> ma, EffCatch<RT, A> mb) =>
            new Eff<RT, A>(Thunk<RT, A>.Lazy(
                env =>
                {
                    var ra = ma.ReRun(env);
                    return ra.IsSucc
                        ? ra
                        : mb.Run(env, ra.Error);
                }));

        [Pure, MethodImpl(Opt.Default)]
        public static Eff<RT, A> operator |(Eff<RT, A> ma, EffCatch<A> mb) =>
            new Eff<RT, A>(Thunk<RT, A>.Lazy(
                env =>
                {
                    var ra = ma.ReRun(env);
                    return ra.IsSucc
                        ? ra
                        : mb.Run(ra.Error);
                }));
        
        [Pure, MethodImpl(Opt.Default)]
        public static Eff<RT, A> operator |(Eff<RT, A> ma, CatchValue<A> value) =>
            new Eff<RT, A>(Thunk<RT, A>.Lazy(
                                env =>
                                {
                                    var ra = ma.ReRun(env);
                                    return ra.IsSucc
                                               ? ra
                                               : value.Match(ra.Error)
                                                   ? FinSucc(value.Value(ra.Error))
                                                   : ra;
                                }));
        
        [Pure, MethodImpl(Opt.Default)]
        public static Eff<RT, A> operator |(Eff<RT, A> ma, CatchError value) =>
            new Eff<RT, A>(Thunk<RT, A>.Lazy(
                                env =>
                                {
                                    var ra = ma.ReRun(env);
                                    return ra.IsSucc
                                               ? ra
                                               : value.Match(ra.Error)
                                                   ? FinFail<A>(value.Value(ra.Error))
                                                   : ra;
                                }));

        /// <summary>
        /// Implicit conversion from pure Eff
        /// </summary>
        public static implicit operator Eff<RT, A>(Eff<A> ma) =>
            EffectMaybe(env => ma.ReRun());
    }

    public static partial class AffExtensions
    {
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, A> ToAff<RT, A>(this Eff<RT, A> ma) where RT : struct, HasCancel<RT> =>
            Aff<RT, A>.EffectMaybe(e => new ValueTask<Fin<A>>(ma.ReRun(e)));

        // Map and map-left
 
        [Pure, MethodImpl(Opt.Default)]
        public static Eff<RT, B> Map<RT, A, B>(this Eff<RT, A> ma, Func<A, B> f) where RT : struct =>
            new Eff<RT, B>(ma.Thunk.Map(f));

        [Pure, MethodImpl(Opt.Default)]
        public static Eff<RT, A> MapFail<RT, A>(this Eff<RT, A> ma, Func<Error, Error> f) where RT : struct =>
            ma.BiMap(identity, f);
        
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, B> MapAsync<RT, A, B>(this Eff<RT, A> ma, Func<A, ValueTask<B>> f) where RT : struct, HasCancel<RT> =>
            new Aff<RT, B>(ma.Thunk.MapAsync(f));
        
        //
        // Bi-map
        //

        [Pure, MethodImpl(Opt.Default)]
        public static Eff<RT, B> BiMap<RT, A, B>(this Eff<RT, A> ma, Func<A, B> Succ, Func<Error, Error> Fail) where RT : struct =>
            new Eff<RT, B>(ma.Thunk.BiMap(Succ, Fail));

        //
        // Match
        //
        
        [Pure, MethodImpl(Opt.Default)]
        public static Eff<RT, B> Match<RT, A, B>(this Eff<RT, A> ma, Func<A, B> Succ, Func<Error, B> Fail) where RT : struct, HasCancel<RT> =>
            Eff<RT, B>(env => { 
            
                var r = ma.ReRun(env);
                return r.IsSucc
                    ? Succ(r.Value)
                    : Fail(r.Error);
            });
        
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, B> MatchEff<RT, A, B>(this Eff<RT, A> ma, Func<A, B> Succ, Aff<RT, B> Fail) where RT : struct, HasCancel<RT> =>
            AffMaybe<RT, B>(async env =>
            {
                var r = ma.ReRun(env);
                return r.IsSucc
                    ? Succ(r.Value)
                    : await Fail.ReRun(env).ConfigureAwait(false);
            });
        
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, B> MatchAff<RT, A, B>(this Eff<RT, A> ma, Func<A, B> Succ, Func<Error, Aff<RT, B>> Fail) where RT : struct, HasCancel<RT> =>
            AffMaybe<RT, B>(async env =>
                             {
                                 var r = ma.ReRun(env);
                                 return r.IsSucc
                                            ? Succ(r.Value)
                                            : await Fail(r.Error).Run(env).ConfigureAwait(false);
                             });
        
        [Pure, MethodImpl(Opt.Default)]
        public static Eff<RT, B> MatchEff<RT, A, B>(this Eff<RT, A> ma, Func<A, B> Succ, Eff<RT, B> Fail) where RT : struct =>
            EffMaybe<RT, B>(env =>
            {
                var r = ma.ReRun(env);
                return r.IsSucc
                    ? Succ(r.Value)
                    : Fail.ReRun(env);
            });
        
        [Pure, MethodImpl(Opt.Default)]
        public static Eff<RT, B> MatchEff<RT, A, B>(this Eff<RT, A> ma, Func<A, B> Succ, Func<Error, Eff<RT, B>> Fail) where RT : struct =>
            EffMaybe<RT, B>(env =>
                             {
                                 var r = ma.ReRun(env);
                                 return r.IsSucc
                                            ? Succ(r.Value)
                                            : Fail(r.Error).Run(env);
                             });

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, B> MatchAff<RT, A, B>(this Eff<RT, A> ma, Aff<RT, B> Succ, Func<Error, B> Fail) where RT : struct, HasCancel<RT> =>
            AffMaybe<RT, B>(async env =>
            {
                var r = ma.ReRun(env);
                return r.IsSucc
                    ? await Succ.ReRun(env).ConfigureAwait(false)
                    : Fail(r.Error);
            });

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, B> MatchAff<RT, A, B>(this Eff<RT, A> ma, Func<A, Aff<RT, B>> Succ, Func<Error, B> Fail) where RT : struct, HasCancel<RT> =>
            AffMaybe<RT, B>(async env =>
                             {
                                 var r = ma.ReRun(env);
                                 return r.IsSucc
                                            ? await Succ(r.Value).Run(env).ConfigureAwait(false)
                                            : Fail(r.Error);
                             });

        [Pure, MethodImpl(Opt.Default)]
        public static Eff<RT, B> MatchEff<RT, A, B>(this Eff<RT, A> ma, Eff<RT, B> Succ, Func<Error, B> Fail) where RT : struct =>
            EffMaybe<RT, B>(env =>
            {
                var r = ma.ReRun(env);
                return r.IsSucc
                    ? Succ.ReRun(env)
                    : Fail(r.Error);
            });

        [Pure, MethodImpl(Opt.Default)]
        public static Eff<RT, B> MatchEff<RT, A, B>(this Eff<RT, A> ma, Func<A, Eff<RT, B>> Succ, Func<Error, B> Fail) where RT : struct =>
            EffMaybe<RT, B>(env =>
                             {
                                 var r = ma.ReRun(env);
                                 return r.IsSucc
                                            ? Succ(r.Value).Run(env)
                                            : Fail(r.Error);
                             });

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, B> MatchAff<RT, A, B>(this Eff<RT, A> ma, Aff<RT, B> Succ, Aff<RT, B> Fail) where RT : struct, HasCancel<RT> =>
            AffMaybe<RT, B>(async env =>
            {
                var r = ma.ReRun(env);
                return r.IsSucc
                    ? await Succ.ReRun(env).ConfigureAwait(false)
                    : await Fail.ReRun(env).ConfigureAwait(false);
            });

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, B> MatchAff<RT, A, B>(this Eff<RT, A> ma, Func<A, Aff<RT, B>> Succ, Func<Error, Aff<RT, B>> Fail) where RT : struct, HasCancel<RT> =>
            AffMaybe<RT, B>(async env =>
                             {
                                 var r = ma.ReRun(env);
                                 return r.IsSucc
                                            ? await Succ(r.Value).Run(env).ConfigureAwait(false)
                                            : await Fail(r.Error).Run(env).ConfigureAwait(false);
                             });

        [Pure, MethodImpl(Opt.Default)]
        public static Eff<RT, B> MatchEff<RT, A, B>(this Eff<RT, A> ma, Eff<RT, B> Succ, Eff<RT, B> Fail) where RT : struct =>
            EffMaybe<RT, B>(env =>
            {
                var r = ma.ReRun(env);
                return r.IsSucc
                    ? Succ.ReRun(env)
                    : Fail.ReRun(env);
            });

        [Pure, MethodImpl(Opt.Default)]
        public static Eff<RT, B> MatchEff<RT, A, B>(this Eff<RT, A> ma, Func<A, Eff<RT, B>> Succ, Func<Error, Eff<RT, B>> Fail) where RT : struct =>
            EffMaybe<RT, B>(env =>
                             {
                                 var r = ma.ReRun(env);
                                 return r.IsSucc
                                            ? Succ(r.Value).Run(env)
                                            : Fail(r.Error).Run(env);
                             });

        [Pure, MethodImpl(Opt.Default)]
        public static Eff<RT, B> Match<RT, A, B>(this Eff<RT, A> ma, B Succ, Func<Error, B> Fail) where RT : struct =>
            Eff<RT, B>(env =>
            {
                var r = ma.ReRun(env);
                return r.IsSucc
                    ? Succ
                    : Fail(r.Error);
            });

        [Pure, MethodImpl(Opt.Default)]
        public static Eff<RT, B> Match<RT, A, B>(this Eff<RT, A> ma, Func<A, B> Succ, B Fail) where RT : struct =>
            Eff<RT, B>(env =>
            {
                var r = ma.ReRun(env);
                return r.IsSucc
                           ? Succ(r.Value)
                           : Fail;
            });

        [Pure, MethodImpl(Opt.Default)]
        public static Eff<RT, B> Match<RT, A, B>(this Eff<RT, A> ma, B Succ, B Fail) where RT : struct =>
            Eff<RT, B>(env =>
            {
                var r = ma.ReRun(env);
                return r.IsSucc
                           ? Succ
                           : Fail;
            });
        
        //
        // IfNone
        //
        
        [Pure, MethodImpl(Opt.Default)]
        public static Eff<RT, A> IfFail<RT, A>(this Eff<RT, A> ma, Func<Error, A> f) where RT : struct =>
            EffMaybe<RT, A>(
                env =>
                {
                    var res = ma.ReRun(env);
                    return res.IsSucc
                               ? res
                               : Fin<A>.Succ(f(res.Error));
                });
        
        [Pure, MethodImpl(Opt.Default)]
        public static Eff<RT, A> IfFail<RT, A>(this Eff<RT, A> ma, A alternative) where RT : struct =>
            EffMaybe<RT, A>(
                env =>
                {
                    var res = ma.ReRun(env);
                    return res.IsSucc
                               ? res
                               : Fin<A>.Succ(alternative);
                });
        
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, A> IfFailAff<RT, A>(this Eff<RT, A> ma, Aff<RT, A> alternative) where RT : struct, HasCancel<RT> =>
            AffMaybe<RT, A>(
                async env =>
                {
                    var res = ma.ReRun(env);
                    return res.IsSucc
                               ? res
                               : await alternative.ReRun(env).ConfigureAwait(false);
                });
        
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, A> IfFailAff<RT, A>(this Eff<RT, A> ma, Func<Error, Aff<RT, A>> alternative) where RT : struct, HasCancel<RT> =>
            AffMaybe<RT, A>(
                async env =>
                {
                    var res = ma.ReRun(env);
                    return res.IsSucc
                               ? res
                               : await alternative(res.Error).Run(env).ConfigureAwait(false);
                });
        
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, A> IfFailAff<RT, A>(this Eff<RT, A> ma, Aff<A> alternative) where RT : struct, HasCancel<RT> =>
            AffMaybe<RT, A>(
                async env =>
                {
                    var res = ma.ReRun(env);
                    return res.IsSucc
                               ? res
                               : await alternative.ReRun().ConfigureAwait(false);
                });
        
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, A> IfFailAff<RT, A>(this Eff<RT, A> ma, Func<Error, Aff<A>> alternative) where RT : struct, HasCancel<RT> =>
            AffMaybe<RT, A>(
                async env =>
                {
                    var res = ma.ReRun(env);
                    return res.IsSucc
                               ? res
                               : await alternative(res.Error).Run().ConfigureAwait(false);
                });
        
        [Pure, MethodImpl(Opt.Default)]
        public static Eff<RT, A> IfFailEff<RT, A>(this Eff<RT, A> ma, Eff<RT, A> alternative) where RT : struct =>
            EffMaybe<RT, A>(
                env =>
                {
                    var res = ma.ReRun(env);
                    return res.IsSucc
                               ? res
                               : alternative.ReRun(env);
                });
        
        [Pure, MethodImpl(Opt.Default)]
        public static Eff<RT, A> IfFailEff<RT, A>(this Eff<RT, A> ma, Func<Error, Eff<RT, A>> alternative) where RT : struct =>
            EffMaybe<RT, A>(
                env =>
                {
                    var res = ma.ReRun(env);
                    return res.IsSucc
                               ? res
                               : alternative(res.Error).Run(env);
                });
        
        [Pure, MethodImpl(Opt.Default)]
        public static Eff<RT, A> IfFailEff<RT, A>(this Eff<RT, A> ma, Eff<A> alternative) where RT : struct =>
            EffMaybe<RT, A>(
                env =>
                {
                    var res = ma.ReRun(env);
                    return res.IsSucc
                               ? res
                               : alternative.ReRun();
                });
        
        [Pure, MethodImpl(Opt.Default)]
        public static Eff<RT, A> IfFailEff<RT, A>(this Eff<RT, A> ma, Func<Error, Eff<A>> alternative) where RT : struct =>
            EffMaybe<RT, A>(
                env =>
                {
                    var res = ma.ReRun(env);
                    return res.IsSucc
                               ? res
                               : alternative(res.Error).Run();
                });
        
        //
        // Iter / Do
        //
        
        [Pure, MethodImpl(Opt.Default)]
        public static Eff<RT, Unit> Iter<RT, A>(this Eff<RT, A> ma, Func<A, Unit> f) where RT : struct =>
            Eff<RT, Unit>(
                env =>
                {
                    var res = ma.ReRun(env);
                    if (res.IsSucc)
                    {
                        f(res.Value);
                    }
                    return unit;
                });
        
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, Unit> Iter<RT, A>(this Eff<RT, A> ma, Func<A, Aff<RT, Unit>> f) where RT : struct, HasCancel<RT> =>
            Aff<RT, Unit>(
                async env =>
                {
                    var res = ma.ReRun(env);
                    if (res.IsSucc)
                    {
                        ignore(await f(res.Value).Run(env).ConfigureAwait(false));
                    }
                    return unit;
                });
        
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, Unit> Iter<RT, A>(this Eff<RT, A> ma, Func<A, Aff<Unit>> f) where RT : struct, HasCancel<RT> =>
            Aff<RT, Unit>(
                async env =>
                {
                    var res = ma.ReRun(env);
                    if (res.IsSucc)
                    {
                        ignore(await f(res.Value).Run().ConfigureAwait(false));
                    }
                    return unit;
                });
        
        [Pure, MethodImpl(Opt.Default)]
        public static Eff<RT, Unit> Iter<RT, A>(this Eff<RT, A> ma, Func<A, Eff<RT, Unit>> f) where RT : struct =>
            Eff<RT, Unit>(
                env =>
                {
                    var res = ma.ReRun(env);
                    if (res.IsSucc)
                    {
                        ignore(f(res.Value).Run(env));
                    }
                    return unit;
                });
        
        [Pure, MethodImpl(Opt.Default)]
        public static Eff<RT, Unit> Iter<RT, A>(this Eff<RT, A> ma, Func<A, Eff<Unit>> f) where RT : struct =>
            Eff<RT, Unit>(
                env =>
                {
                    var res = ma.ReRun(env);
                    if (res.IsSucc)
                    {
                        ignore(f(res.Value).Run());
                    }
                    return unit;
                });
        
        [Pure, MethodImpl(Opt.Default)]
        public static Eff<RT, A> Do<RT, A>(this Eff<RT, A> ma, Func<A, Unit> f) where RT : struct =>
            EffMaybe<RT, A>(
                env =>
                {
                    var res = ma.ReRun(env);
                    if (res.IsSucc)
                    {
                        f(res.Value);
                    }
                    return res;
                });
        
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, A> Do<RT, A>(this Eff<RT, A> ma, Func<A, Aff<RT, Unit>> f) where RT : struct, HasCancel<RT> =>
            AffMaybe<RT, A>(
                async env =>
                {
                    var res = ma.ReRun(env);
                    if (res.IsSucc)
                    {
                        var ures = await f(res.Value).Run(env).ConfigureAwait(false);
                        return ures.IsFail
                                   ? Fin<A>.Fail(ures.Error)
                                   : res;
                    }
                    return res;
                });
        
        [Pure, MethodImpl(Opt.Default)]
        public static Eff<RT, A> Do<RT, A>(this Eff<RT, A> ma, Func<A, Eff<RT, Unit>> f) where RT : struct =>
            EffMaybe<RT, A>(
                env =>
                {
                    var res = ma.ReRun(env);
                    if (res.IsSucc)
                    {
                        var ures = f(res.Value).Run(env);
                        return ures.IsFail
                                   ? Fin<A>.Fail(ures.Error)
                                   : res;
                    }
                    return res;
                });
        
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, A> Do<RT, A>(this Eff<RT, A> ma, Func<A, Aff<Unit>> f) where RT : struct, HasCancel<RT> =>
            AffMaybe<RT, A>(
                async env =>
                {
                    var res = ma.ReRun(env);
                    if (res.IsSucc)
                    {
                        var ures = await f(res.Value).Run().ConfigureAwait(false);
                        return ures.IsFail
                                   ? Fin<A>.Fail(ures.Error)
                                   : res;
                    }
                    return res;
                });
        
        [Pure, MethodImpl(Opt.Default)]
        public static Eff<RT, A> Do<RT, A>(this Eff<RT, A> ma, Func<A, Eff<Unit>> f) where RT : struct =>
            EffMaybe<RT, A>(
                env =>
                {
                    var res = ma.ReRun(env);
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
        
        [Pure, MethodImpl(Opt.Default)]
        public static Eff<RT, A> Filter<RT, A>(this Eff<RT, A> ma, Func<A, bool> f) where RT : struct =>
            ma.Bind(x => f(x) ? SuccessEff<A>(x) : FailEff<A>(Errors.Cancelled));        

        //
        // Bind
        //

        [Pure, MethodImpl(Opt.Default)]
        public static Eff<RT, B> Bind<RT, A, B>(this Eff<RT, A> ma, Func<A, Eff<B>> f) where RT : struct =>
            new Eff<RT, B>(Thunk<RT, B>.Lazy(
                               env =>
                               {
                                   var fa = ma.ReRun(env);
                                   if (fa.IsFail) return FinFail<B>(fa.Error);
                                   var mb = f(fa.Value);
                                   return mb.Run();
                               }));

        [Pure, MethodImpl(Opt.Default)]
        public static Eff<RT, B> Bind<RT, A, B>(this Eff<RT, A> ma, Func<A, Eff<RT, B>> f) where RT : struct =>
            new Eff<RT, B>(Thunk<RT, B>.Lazy(
                               env =>
                               {
                                   var fa = ma.ReRun(env);
                                   if (fa.IsFail) return FinFail<B>(fa.Error);
                                   var mb = f(fa.Value);
                                   return mb.Run(env);
                               }));
        
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, B> Bind<RT, A, B>(this Eff<RT, A> ma, Func<A, Aff<B>> f) where RT : struct, HasCancel<RT> =>
            new Aff<RT, B>(ThunkAsync<RT, B>.Lazy(
                               async env =>
                               {
                                   var fa = ma.ReRun(env);
                                   if (fa.IsFail) return FinFail<B>(fa.Error);
                                   var mb = f(fa.Value);
                                   return await mb.Run().ConfigureAwait(false);
                               }));

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, B> Bind<RT, A, B>(this Eff<RT, A> ma, Func<A, Aff<RT, B>> f) where RT : struct, HasCancel<RT> =>
            new Aff<RT, B>(ThunkAsync<RT, B>.Lazy(
                               async env =>
                               {
                                   var fa = ma.ReRun(env);
                                   if (fa.IsFail) return FinFail<B>(fa.Error);
                                   var mb = f(fa.Value);
                                   return await mb.Run(env).ConfigureAwait(false);
                               }));        
        
        //
        // Flatten
        //
        
        [Pure, MethodImpl(Opt.Default)]
        public static Eff<RT, A> Flatten<RT, A>(this Eff<RT, Eff<RT, A>> ma) where RT : struct =>
            new Eff<RT, A>(ma.Thunk.Map(ThunkFromIO).Flatten());
        
        [Pure, MethodImpl(Opt.Default)]
        public static Eff<RT, A> Flatten<RT, A>(this Eff<RT, Eff<A>> ma) where RT : struct =>
            new Eff<RT, A>(ma.Thunk.Map(ThunkFromIO).Flatten());
        
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, A> Flatten<RT, A>(this Eff<RT, Aff<RT, A>> ma) where RT : struct, HasCancel<RT> =>
            new Aff<RT, A>(ma.Thunk.Map(ThunkFromIO).Flatten());
        
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, A> Flatten<RT, A>(this Eff<RT, Aff<A>> ma) where RT : struct, HasCancel<RT> =>
            new Aff<RT, A>(ma.Thunk.Map(ThunkFromIO).Flatten());

        //
        // Select
        //
        
        [Pure, MethodImpl(Opt.Default)]
        public static Eff<RT, B> Select<RT, A, B>(this Eff<RT, A> ma, Func<A, B> f) where RT : struct =>
            Map(ma, f);
        
        //
        // SelectMany
        //
        
        [Pure, MethodImpl(Opt.Default)]
        public static Eff<RT, B> SelectMany<RT, A, B>(this Eff<RT, A> ma, Func<A, Eff<RT, B>> f) where RT : struct =>
            Bind(ma, f);
        
        [Pure, MethodImpl(Opt.Default)]
        public static Eff<RT, B> SelectMany<RT, A, B>(this Eff<RT, A> ma, Func<A, Eff<B>> f) where RT : struct =>
            Bind(ma, f);
        
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, B> SelectMany<RT, A, B>(this Eff<RT, A> ma, Func<A, Aff<RT, B>> f) where RT : struct, HasCancel<RT> =>
            Bind(ma, f);
        
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, B> SelectMany<RT, A, B>(this Eff<RT, A> ma, Func<A, Aff<B>> f) where RT : struct, HasCancel<RT> =>
            Bind(ma, f);
        
        [Pure, MethodImpl(Opt.Default)]
        public static Eff<RT, C> SelectMany<RT, A, B, C>(this Eff<RT, A> ma, Func<A, Eff<RT, B>> bind, Func<A, B, C> project) where RT : struct =>
            Bind(ma, x => Map(bind(x), y => project(x, y)));
        
        [Pure, MethodImpl(Opt.Default)]
        public static Eff<RT, C> SelectMany<RT, A, B, C>(this Eff<RT, A> ma, Func<A, Eff<B>> bind, Func<A, B, C> project) where RT : struct =>
            Bind(ma, x => Map(bind(x), y => project(x, y)));
        
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, C> SelectMany<RT, A, B, C>(this Eff<RT, A> ma, Func<A, Aff<RT, B>> bind, Func<A, B, C> project) where RT : struct, HasCancel<RT> =>
            Bind(ma, x => Map(bind(x), y => project(x, y)));
        
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, C> SelectMany<RT, A, B, C>(this Eff<RT, A> ma, Func<A, Aff<B>> bind, Func<A, B, C> project) where RT : struct, HasCancel<RT> =>
            Bind(ma, x => Map(bind(x), y => project(x, y)));
        
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, C> SelectMany<RT, A, B, C>(this Eff<RT, A> ma, Func<A, Effect<RT, B>> bind, Func<A, B, C> project) where RT : struct, HasCancel<RT> =>
            Bind(ma, x => Map(bind(x).RunEffect(), y => project(x, y)));

        //
        // Where
        //
        
        [Pure, MethodImpl(Opt.Default)]
        public static Eff<RT, A> Where<RT, A>(this Eff<RT, A> ma, Func<A, bool> f) where RT : struct =>
            Filter(ma, f);

        //
        // Zip
        //
        
        [Pure, MethodImpl(Opt.Default)]
        public static Eff<RT, (A, B)> Zip<RT, A, B>(Eff<RT, A> ma, Eff<RT, B> mb) where RT : struct =>
            new Eff<RT, (A, B)>(Thunk<RT, (A, B)>.Lazy(e =>
            {
                var ta = ma.ReRun(e);
                if (ta.IsFail) return ta.Cast<(A, B)>();
                var tb = mb.ReRun(e);
                if (tb.IsFail) return tb.Cast<(A, B)>();
                return Fin<(A, B)>.Succ((ta.Value, tb.Value));
            }));
        
        [Pure, MethodImpl(Opt.Default)]
        public static Eff<RT, (A, B)> Zip<RT, A, B>(Eff<A> ma, Eff<RT, B> mb) where RT : struct =>
            new Eff<RT, (A, B)>(Thunk<RT, (A, B)>.Lazy(e =>
            {
                var ta = ma.ReRun();
                if (ta.IsFail) return ta.Cast<(A, B)>();
                var tb = mb.ReRun(e);
                if (tb.IsFail) return tb.Cast<(A, B)>();
                return Fin<(A, B)>.Succ((ta.Value, tb.Value));
            }));
        
        [Pure, MethodImpl(Opt.Default)]
        public static Eff<RT, (A, B)> Zip<RT, A, B>(Eff<RT, A> ma, Eff<B> mb) where RT : struct =>
            new Eff<RT, (A, B)>(Thunk<RT, (A, B)>.Lazy(e =>
            {
                var ta = ma.ReRun(e);
                if (ta.IsFail) return ta.Cast<(A, B)>();
                var tb = mb.ReRun();
                if (tb.IsFail) return tb.Cast<(A, B)>();
                return Fin<(A, B)>.Succ((ta.Value, tb.Value));
            }));           
       
        [Pure, MethodImpl(Opt.Default)]
        static Thunk<RT, A> ThunkFromIO<RT, A>(Eff<RT, A> ma) where RT : struct =>
            ma.Thunk;
    }    
}
