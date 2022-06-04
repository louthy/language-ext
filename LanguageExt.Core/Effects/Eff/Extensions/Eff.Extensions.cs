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
    public static partial class AffExtensions
    {
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, A> ToAff<RT, A>(this Eff<RT, A> ma) where RT : struct, HasCancel<RT> =>
            Aff<RT, A>.EffectMaybe(e => new ValueTask<Fin<A>>(ma.Run(e)));

        // Map and map-left
 
        [Pure, MethodImpl(Opt.Default)]
        public static Eff<RT, B> Map<RT, A, B>(this Eff<RT, A> ma, Func<A, B> f) where RT : struct =>
            new (rt => ma.Run(rt).Map(f));

        [Pure, MethodImpl(Opt.Default)]
        public static Eff<RT, A> MapFail<RT, A>(this Eff<RT, A> ma, Func<Error, Error> f) where RT : struct =>
            ma.BiMap(identity, f);
        
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, B> MapAsync<RT, A, B>(this Eff<RT, A> ma, Func<A, ValueTask<B>> f) where RT : struct, HasCancel<RT> =>
            new(async rt => ma.Run(rt).Case switch
            {
                A x     => FinSucc(await f(x).ConfigureAwait(false)),
                Error e => FinFail<B>(e),
                _       => throw new BottomException()
            });
        
        //
        // Bi-map
        //

        [Pure, MethodImpl(Opt.Default)]
        public static Eff<RT, B> BiMap<RT, A, B>(this Eff<RT, A> ma, Func<A, B> Succ, Func<Error, Error> Fail) where RT : struct =>
            new(rt => ma.Run(rt).Case switch
            {
                A x     => FinSucc(Succ(x)),
                Error e => FinFail<B>(Fail(e)),
                _       => throw new BottomException()
            });

        //
        // Match
        //
        
        [Pure, MethodImpl(Opt.Default)]
        public static Eff<RT, B> Match<RT, A, B>(this Eff<RT, A> ma, Func<A, B> Succ, Func<Error, B> Fail) where RT : struct, HasCancel<RT> =>
            Eff<RT, B>(env => { 
            
                var r = ma.Run(env);
                return r.IsSucc
                    ? Succ(r.Value)
                    : Fail(r.Error);
            });
        
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, B> MatchEff<RT, A, B>(this Eff<RT, A> ma, Func<A, B> Succ, Aff<RT, B> Fail) where RT : struct, HasCancel<RT> =>
            AffMaybe<RT, B>(async env =>
            {
                var r = ma.Run(env);
                return r.IsSucc
                    ? Succ(r.Value)
                    : await Fail.Run(env).ConfigureAwait(false);
            });
        
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, B> MatchAff<RT, A, B>(this Eff<RT, A> ma, Func<A, B> Succ, Func<Error, Aff<RT, B>> Fail) where RT : struct, HasCancel<RT> =>
            AffMaybe<RT, B>(async env =>
                             {
                                 var r = ma.Run(env);
                                 return r.IsSucc
                                            ? Succ(r.Value)
                                            : await Fail(r.Error).Run(env).ConfigureAwait(false);
                             });
        
        [Pure, MethodImpl(Opt.Default)]
        public static Eff<RT, B> MatchEff<RT, A, B>(this Eff<RT, A> ma, Func<A, B> Succ, Eff<RT, B> Fail) where RT : struct =>
            EffMaybe<RT, B>(env =>
            {
                var r = ma.Run(env);
                return r.IsSucc
                    ? Succ(r.Value)
                    : Fail.Run(env);
            });
        
        [Pure, MethodImpl(Opt.Default)]
        public static Eff<RT, B> MatchEff<RT, A, B>(this Eff<RT, A> ma, Func<A, B> Succ, Func<Error, Eff<RT, B>> Fail) where RT : struct =>
            EffMaybe<RT, B>(env =>
                             {
                                 var r = ma.Run(env);
                                 return r.IsSucc
                                            ? Succ(r.Value)
                                            : Fail(r.Error).Run(env);
                             });

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, B> MatchAff<RT, A, B>(this Eff<RT, A> ma, Aff<RT, B> Succ, Func<Error, B> Fail) where RT : struct, HasCancel<RT> =>
            AffMaybe<RT, B>(async env =>
            {
                var r = ma.Run(env);
                return r.IsSucc
                    ? await Succ.Run(env).ConfigureAwait(false)
                    : Fail(r.Error);
            });

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, B> MatchAff<RT, A, B>(this Eff<RT, A> ma, Func<A, Aff<RT, B>> Succ, Func<Error, B> Fail) where RT : struct, HasCancel<RT> =>
            AffMaybe<RT, B>(async env =>
                             {
                                 var r = ma.Run(env);
                                 return r.IsSucc
                                            ? await Succ(r.Value).Run(env).ConfigureAwait(false)
                                            : Fail(r.Error);
                             });

        [Pure, MethodImpl(Opt.Default)]
        public static Eff<RT, B> MatchEff<RT, A, B>(this Eff<RT, A> ma, Eff<RT, B> Succ, Func<Error, B> Fail) where RT : struct =>
            EffMaybe<RT, B>(env =>
            {
                var r = ma.Run(env);
                return r.IsSucc
                    ? Succ.Run(env)
                    : Fail(r.Error);
            });

        [Pure, MethodImpl(Opt.Default)]
        public static Eff<RT, B> MatchEff<RT, A, B>(this Eff<RT, A> ma, Func<A, Eff<RT, B>> Succ, Func<Error, B> Fail) where RT : struct =>
            EffMaybe<RT, B>(env =>
                             {
                                 var r = ma.Run(env);
                                 return r.IsSucc
                                            ? Succ(r.Value).Run(env)
                                            : Fail(r.Error);
                             });

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, B> MatchAff<RT, A, B>(this Eff<RT, A> ma, Aff<RT, B> Succ, Aff<RT, B> Fail) where RT : struct, HasCancel<RT> =>
            AffMaybe<RT, B>(async env =>
            {
                var r = ma.Run(env);
                return r.IsSucc
                    ? await Succ.Run(env).ConfigureAwait(false)
                    : await Fail.Run(env).ConfigureAwait(false);
            });

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, B> MatchAff<RT, A, B>(this Eff<RT, A> ma, Func<A, Aff<RT, B>> Succ, Func<Error, Aff<RT, B>> Fail) where RT : struct, HasCancel<RT> =>
            AffMaybe<RT, B>(async env =>
                             {
                                 var r = ma.Run(env);
                                 return r.IsSucc
                                            ? await Succ(r.Value).Run(env).ConfigureAwait(false)
                                            : await Fail(r.Error).Run(env).ConfigureAwait(false);
                             });

        [Pure, MethodImpl(Opt.Default)]
        public static Eff<RT, B> MatchEff<RT, A, B>(this Eff<RT, A> ma, Eff<RT, B> Succ, Eff<RT, B> Fail) where RT : struct =>
            EffMaybe<RT, B>(env =>
            {
                var r = ma.Run(env);
                return r.IsSucc
                    ? Succ.Run(env)
                    : Fail.Run(env);
            });

        [Pure, MethodImpl(Opt.Default)]
        public static Eff<RT, B> MatchEff<RT, A, B>(this Eff<RT, A> ma, Func<A, Eff<RT, B>> Succ, Func<Error, Eff<RT, B>> Fail) where RT : struct =>
            EffMaybe<RT, B>(env =>
                             {
                                 var r = ma.Run(env);
                                 return r.IsSucc
                                            ? Succ(r.Value).Run(env)
                                            : Fail(r.Error).Run(env);
                             });

        [Pure, MethodImpl(Opt.Default)]
        public static Eff<RT, B> Match<RT, A, B>(this Eff<RT, A> ma, B Succ, Func<Error, B> Fail) where RT : struct =>
            Eff<RT, B>(env =>
            {
                var r = ma.Run(env);
                return r.IsSucc
                    ? Succ
                    : Fail(r.Error);
            });

        [Pure, MethodImpl(Opt.Default)]
        public static Eff<RT, B> Match<RT, A, B>(this Eff<RT, A> ma, Func<A, B> Succ, B Fail) where RT : struct =>
            Eff<RT, B>(env =>
            {
                var r = ma.Run(env);
                return r.IsSucc
                           ? Succ(r.Value)
                           : Fail;
            });

        [Pure, MethodImpl(Opt.Default)]
        public static Eff<RT, B> Match<RT, A, B>(this Eff<RT, A> ma, B Succ, B Fail) where RT : struct =>
            Eff<RT, B>(env =>
            {
                var r = ma.Run(env);
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
                    var res = ma.Run(env);
                    return res.IsSucc
                               ? res
                               : Fin<A>.Succ(f(res.Error));
                });
        
        [Pure, MethodImpl(Opt.Default)]
        public static Eff<RT, A> IfFail<RT, A>(this Eff<RT, A> ma, A alternative) where RT : struct =>
            EffMaybe<RT, A>(
                env =>
                {
                    var res = ma.Run(env);
                    return res.IsSucc
                               ? res
                               : Fin<A>.Succ(alternative);
                });
        
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, A> IfFailAff<RT, A>(this Eff<RT, A> ma, Aff<RT, A> alternative) where RT : struct, HasCancel<RT> =>
            AffMaybe<RT, A>(
                async env =>
                {
                    var res = ma.Run(env);
                    return res.IsSucc
                               ? res
                               : await alternative.Run(env).ConfigureAwait(false);
                });
        
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, A> IfFailAff<RT, A>(this Eff<RT, A> ma, Func<Error, Aff<RT, A>> alternative) where RT : struct, HasCancel<RT> =>
            AffMaybe<RT, A>(
                async env =>
                {
                    var res = ma.Run(env);
                    return res.IsSucc
                               ? res
                               : await alternative(res.Error).Run(env).ConfigureAwait(false);
                });
        
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, A> IfFailAff<RT, A>(this Eff<RT, A> ma, Aff<A> alternative) where RT : struct, HasCancel<RT> =>
            AffMaybe<RT, A>(
                async env =>
                {
                    var res = ma.Run(env);
                    return res.IsSucc
                               ? res
                               : await alternative.Run().ConfigureAwait(false);
                });
        
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, A> IfFailAff<RT, A>(this Eff<RT, A> ma, Func<Error, Aff<A>> alternative) where RT : struct, HasCancel<RT> =>
            AffMaybe<RT, A>(
                async env =>
                {
                    var res = ma.Run(env);
                    return res.IsSucc
                               ? res
                               : await alternative(res.Error).Run().ConfigureAwait(false);
                });
        
        [Pure, MethodImpl(Opt.Default)]
        public static Eff<RT, A> IfFailEff<RT, A>(this Eff<RT, A> ma, Eff<RT, A> alternative) where RT : struct =>
            EffMaybe<RT, A>(
                env =>
                {
                    var res = ma.Run(env);
                    return res.IsSucc
                               ? res
                               : alternative.Run(env);
                });
        
        [Pure, MethodImpl(Opt.Default)]
        public static Eff<RT, A> IfFailEff<RT, A>(this Eff<RT, A> ma, Func<Error, Eff<RT, A>> alternative) where RT : struct =>
            EffMaybe<RT, A>(
                env =>
                {
                    var res = ma.Run(env);
                    return res.IsSucc
                               ? res
                               : alternative(res.Error).Run(env);
                });
        
        [Pure, MethodImpl(Opt.Default)]
        public static Eff<RT, A> IfFailEff<RT, A>(this Eff<RT, A> ma, Eff<A> alternative) where RT : struct =>
            EffMaybe<RT, A>(
                env =>
                {
                    var res = ma.Run(env);
                    return res.IsSucc
                               ? res
                               : alternative.Run();
                });
        
        [Pure, MethodImpl(Opt.Default)]
        public static Eff<RT, A> IfFailEff<RT, A>(this Eff<RT, A> ma, Func<Error, Eff<A>> alternative) where RT : struct =>
            EffMaybe<RT, A>(
                env =>
                {
                    var res = ma.Run(env);
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
                    var res = ma.Run(env);
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
                    var res = ma.Run(env);
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
                    var res = ma.Run(env);
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
                    var res = ma.Run(env);
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
                    var res = ma.Run(env);
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
                    var res = ma.Run(env);
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
                    var res = ma.Run(env);
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
                    var res = ma.Run(env);
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
                    var res = ma.Run(env);
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
                    var res = ma.Run(env);
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
            new (env =>
            {
                var fa = ma.Run(env);
                if (fa.IsFail) return FinFail<B>(fa.Error);
                var mb = f(fa.Value);
                return mb.Run();
            });

        [Pure, MethodImpl(Opt.Default)]
        public static Eff<RT, B> Bind<RT, A, B>(this Eff<RT, A> ma, Func<A, Eff<RT, B>> f) where RT : struct =>
            new(env =>
            {
                var fa = ma.Run(env);
                if (fa.IsFail) return FinFail<B>(fa.Error);
                var mb = f(fa.Value);
                return mb.Run(env);
            });

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, B> Bind<RT, A, B>(this Eff<RT, A> ma, Func<A, Aff<B>> f)
            where RT : struct, HasCancel<RT> =>
            new (async env =>
            {
                var fa = ma.Run(env);
                if (fa.IsFail) return FinFail<B>(fa.Error);
                var mb = f(fa.Value);
                return await mb.Run().ConfigureAwait(false);
            });

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, B> Bind<RT, A, B>(this Eff<RT, A> ma, Func<A, Aff<RT, B>> f)
            where RT : struct, HasCancel<RT> =>
            new(async env =>
            {
                var fa = ma.Run(env);
                if (fa.IsFail) return FinFail<B>(fa.Error);
                var mb = f(fa.Value);
                return await mb.Run(env).ConfigureAwait(false);
            });        
        
        //
        // Flatten
        //

        [Pure, MethodImpl(Opt.Default)]
        public static Eff<RT, A> Flatten<RT, A>(this Eff<RT, Eff<RT, A>> mma) where RT : struct =>
            new(rt =>
            {
                var ma = mma.Run(rt);
                if (ma.IsFail) return ma.Error;
                return ma.Value.Run(rt);
            });
        
        [Pure, MethodImpl(Opt.Default)]
        public static Eff<RT, A> Flatten<RT, A>(this Eff<RT, Eff<A>> mma) where RT : struct =>
            new (rt =>
            {
                var ma = mma.Run(rt);
                if (ma.IsFail) return ma.Error;
                return ma.Value.Run();
            });
        
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, A> Flatten<RT, A>(this Eff<RT, Aff<RT, A>> mma) where RT : struct, HasCancel<RT> =>
            new (async rt =>
            {
                var ma = mma.Run(rt);
                if (ma.IsFail) return ma.Error;
                return await ma.Value.Run(rt).ConfigureAwait(false);
            });
        
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, A> Flatten<RT, A>(this Eff<RT, Aff<A>> mma) where RT : struct, HasCancel<RT> =>
            new (async rt =>
            {
                var ma = mma.Run(rt);
                if (ma.IsFail) return ma.Error;
                return await ma.Value.Run().ConfigureAwait(false);
            });

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
            new(e =>
            {
                var ta = ma.Run(e);
                if (ta.IsFail) return ta.Cast<(A, B)>();
                var tb = mb.Run(e);
                if (tb.IsFail) return tb.Cast<(A, B)>();
                return Fin<(A, B)>.Succ((ta.Value, tb.Value));
            });

        [Pure, MethodImpl(Opt.Default)]
        public static Eff<RT, (A, B)> Zip<RT, A, B>(Eff<A> ma, Eff<RT, B> mb) where RT : struct =>
            new(e =>
            {
                var ta = ma.Run();
                if (ta.IsFail) return ta.Cast<(A, B)>();
                var tb = mb.Run(e);
                if (tb.IsFail) return tb.Cast<(A, B)>();
                return Fin<(A, B)>.Succ((ta.Value, tb.Value));
            });

        [Pure, MethodImpl(Opt.Default)]
        public static Eff<RT, (A, B)> Zip<RT, A, B>(Eff<RT, A> ma, Eff<B> mb) where RT : struct =>
            new(e =>
            {
                var ta = ma.Run(e);
                if (ta.IsFail) return ta.Cast<(A, B)>();
                var tb = mb.Run();
                if (tb.IsFail) return tb.Cast<(A, B)>();
                return Fin<(A, B)>.Succ((ta.Value, tb.Value));
            });           
    }    

    
    public static partial class AffExtensions
    {
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<A> ToAsync<A>(this Eff<A> ma) =>
            Aff<A>.EffectMaybe(() => new ValueTask<Fin<A>>(ma.Run()));

        [Pure, MethodImpl(Opt.Default)]
        public static Eff<B> Map<A, B>(this Eff<A> ma, Func<A, B> f) =>
            new (() => ma.Run().Map(f));
        
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<B> MapAsync<A, B>(this Eff<A> ma, Func<A, ValueTask<B>> f) =>
            new(async () => ma.Run().Case switch
            {
                A x     => FinSucc(await f(x).ConfigureAwait(false)),
                Error e => FinFail<B>(e),
                _       => throw new BottomException()
            });

        [Pure, MethodImpl(Opt.Default)]
        public static Eff<A> MapFail<A>(this Eff<A> ma, Func<Error, Error> f) =>
            ma.BiMap(identity, f);

        [Pure, MethodImpl(Opt.Default)]
        public static Eff<B> BiMap<A, B>(this Eff<A> ma, Func<A, B> Succ, Func<Error, Error> Fail) =>
            new(() => ma.Run().Case switch
            {
                A x     => FinSucc(Succ(x)),
                Error e => FinFail<B>(Fail(e)),
                _       => throw new BottomException()
            });

        //
        // Match
        // 
        
        [Pure, MethodImpl(Opt.Default)]
        public static Eff<B> Match<A, B>(this Eff<A> ma, Func<A, B> Succ, Func<Error, B> Fail) =>
            Eff(() => { 
                var r = ma.Run();
                return r.IsSucc
                    ? Succ(r.Value)
                    : Fail(r.Error);
            });
        
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, B> MatchAff<RT, A, B>(this Eff<A> ma, Func<A, B> Succ, Aff<RT, B> Fail) where RT : struct, HasCancel<RT> =>
            AffMaybe<RT, B>(async env =>
            {
                var r = ma.Run();
                return r.IsSucc
                    ? Succ(r.Value)
                    : await Fail.Run(env).ConfigureAwait(false);
            });
        
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, B> MatchAff<RT, A, B>(this Eff<A> ma, Func<A, B> Succ, Func<Error, Aff<RT, B>> Fail) where RT : struct, HasCancel<RT> =>
            AffMaybe<RT, B>(async env =>
                             {
                                 var r = ma.Run();
                                 return r.IsSucc
                                            ? Succ(r.Value)
                                            : await Fail(r.Error).Run(env).ConfigureAwait(false);
                             });
        
        [Pure, MethodImpl(Opt.Default)]
        public static Eff<RT, B> MatchEff<RT, A, B>(this Eff<A> ma, Func<A, B> Succ, Eff<RT, B> Fail) where RT : struct =>
            EffMaybe<RT, B>(env =>
            {
                var r = ma.Run();
                return r.IsSucc
                    ? Succ(r.Value)
                    : Fail.Run(env);
            });
        
        [Pure, MethodImpl(Opt.Default)]
        public static Eff<RT, B> MatchEff<RT, A, B>(this Eff<A> ma, Func<A, B> Succ, Func<Error, Eff<RT, B>> Fail) where RT : struct =>
            EffMaybe<RT, B>(env =>
                             {
                                 var r = ma.Run();
                                 return r.IsSucc
                                            ? Succ(r.Value)
                                            : Fail(r.Error).Run(env);
                             });
        
        [Pure, MethodImpl(Opt.Default)]
        public static Eff<B> MatchEff<A, B>(this Eff<A> ma, Func<A, B> Succ, Eff<B> Fail) =>
            EffMaybe<B>(() =>
            {
                var r = ma.Run();
                return r.IsSucc
                    ? Succ(r.Value)
                    : Fail.Run();
            });
        
        [Pure, MethodImpl(Opt.Default)]
        public static Eff<B> MatchEff<A, B>(this Eff<A> ma, Func<A, B> Succ, Func<Error, Eff<B>> Fail) =>
            EffMaybe<B>(() =>
                        {
                            var r = ma.Run();
                            return r.IsSucc
                                       ? Succ(r.Value)
                                       : Fail(r.Error).Run();
                        });

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, B> MatchAff<RT, A, B>(this Eff<A> ma, Aff<RT, B> Succ, Func<Error, B> Fail) where RT : struct, HasCancel<RT> =>
            AffMaybe<RT, B>(async env =>
            {
                var r = ma.Run();
                return r.IsSucc
                    ? await Succ.Run(env).ConfigureAwait(false)
                    : Fail(r.Error);
            });

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, B> MatchAff<RT, A, B>(this Eff<A> ma, Func<A, Aff<RT, B>> Succ, Func<Error, B> Fail) where RT : struct, HasCancel<RT> =>
            AffMaybe<RT, B>(async env =>
                             {
                                 var r = ma.Run();
                                 return r.IsSucc
                                            ? await Succ(r.Value).Run(env).ConfigureAwait(false)
                                            : Fail(r.Error);
                             });

        [Pure, MethodImpl(Opt.Default)]
        public static Eff<RT, B> MatchEff<RT, A, B>(this Eff<A> ma, Eff<RT, B> Succ, Func<Error, B> Fail) where RT : struct =>
            EffMaybe<RT, B>(env =>
            {
                var r = ma.Run();
                return r.IsSucc
                    ? Succ.Run(env)
                    : Fail(r.Error);
            });

        [Pure, MethodImpl(Opt.Default)]
        public static Eff<RT, B> MatchEff<RT, A, B>(this Eff<A> ma, Func<A, Eff<RT, B>> Succ, Func<Error, B> Fail) where RT : struct =>
            EffMaybe<RT, B>(env =>
                             {
                                 var r = ma.Run();
                                 return r.IsSucc
                                            ? Succ(r.Value).Run(env)
                                            : Fail(r.Error);
                             });

        [Pure, MethodImpl(Opt.Default)]
        public static Eff<B> MatchEff<A, B>(this Eff<A> ma, Eff<B> Succ, Func<Error, B> Fail) =>
            EffMaybe<B>(() =>
            {
                var r = ma.Run();
                return r.IsSucc
                    ? Succ.Run()
                    : Fail(r.Error);
            });

        [Pure, MethodImpl(Opt.Default)]
        public static Eff<B> MatchEff<A, B>(this Eff<A> ma, Func<A, Eff<B>> Succ, Func<Error, B> Fail) =>
            EffMaybe<B>(() =>
                        {
                            var r = ma.Run();
                            return r.IsSucc
                                       ? Succ(r.Value).Run()
                                       : Fail(r.Error);
                        });

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, B> MatchAff<RT, A, B>(this Eff<A> ma, Aff<RT, B> Succ, Aff<RT, B> Fail) where RT : struct, HasCancel<RT> =>
            AffMaybe<RT, B>(async env =>
            {
                var r = ma.Run();
                return r.IsSucc
                    ? await Succ.Run(env).ConfigureAwait(false)
                    : await Fail.Run(env).ConfigureAwait(false);
            });

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, B> MatchAff<RT, A, B>(this Eff<A> ma, Func<A, Aff<RT, B>> Succ, Func<Error, Aff<RT, B>> Fail) where RT : struct, HasCancel<RT> =>
            AffMaybe<RT, B>(async env =>
                             {
                                 var r = ma.Run();
                                 return r.IsSucc
                                            ? await Succ(r.Value).Run(env).ConfigureAwait(false)
                                            : await Fail(r.Error).Run(env).ConfigureAwait(false);
                             });

        [Pure, MethodImpl(Opt.Default)]
        public static Eff<RT, B> MatchEff<RT, A, B>(this Eff<A> ma, Eff<RT, B> Succ, Eff<RT, B> Fail) where RT : struct =>
            EffMaybe<RT, B>(env =>
            {
                var r = ma.Run();
                return r.IsSucc
                    ? Succ.Run(env)
                    : Fail.Run(env);
            });

        [Pure, MethodImpl(Opt.Default)]
        public static Eff<RT, B> MatchEff<RT, A, B>(this Eff<A> ma, Func<A, Eff<RT, B>> Succ, Func<Error, Eff<RT, B>> Fail) where RT : struct =>
            EffMaybe<RT, B>(env =>
            {
                var r = ma.Run();
                return r.IsSucc
                    ? Succ(r.Value).Run(env)
                    : Fail(r.Error).Run(env);
            });

        [Pure, MethodImpl(Opt.Default)]
        public static Eff<B> MatchEff<A, B>(this Eff<A> ma, Eff<B> Succ, Eff<B> Fail) =>
            EffMaybe<B>(() =>
            {
                var r = ma.Run();
                return r.IsSucc
                    ? Succ.Run()
                    : Fail.Run();
            });

        [Pure, MethodImpl(Opt.Default)]
        public static Eff<B> MatchEff<A, B>(this Eff<A> ma, Func<A, Eff<B>> Succ, Func<Error, Eff<B>> Fail) =>
            EffMaybe<B>(() =>
                        {
                            var r = ma.Run();
                            return r.IsSucc
                                       ? Succ(r.Value).Run()
                                       : Fail(r.Error).Run();
                        });

        [Pure, MethodImpl(Opt.Default)]
        public static Eff<B> Match<A, B>(this Eff<A> ma, B Succ, Func<Error, B> Fail) =>
            Eff<B>(() =>
            {
                var r = ma.Run();
                return r.IsSucc
                    ? Succ
                    : Fail(r.Error);
            });

        [Pure, MethodImpl(Opt.Default)]
        public static Eff<B> Match<RT, A, B>(this Eff<A> ma, Func<A, B> Succ, B Fail) =>
            Eff<B>(() =>
            {
                var r = ma.Run();
                return r.IsSucc
                           ? Succ(r.Value)
                           : Fail;
            });

        [Pure, MethodImpl(Opt.Default)]
        public static Eff<RT, B> Match<RT, A, B>(this Eff<A> ma, B Succ, B Fail) where RT : struct =>
            Eff<RT, B>(env =>
            {
                var r = ma.Run();
                return r.IsSucc
                           ? Succ
                           : Fail;
            });

        [Pure, MethodImpl(Opt.Default)]
        public static Eff<B> Match<A, B>(this Eff<A> ma, B Succ, B Fail) =>
            Eff<B>(() =>
            {
                var r = ma.Run();
                return r.IsSucc
                           ? Succ
                           : Fail;
            });
        
        //
        // IfNone
        //
        
        [Pure, MethodImpl(Opt.Default)]
        public static Eff<A> IfFail<A>(this Eff<A> ma, Func<Error, A> f) =>
            EffMaybe<A>(
                () =>
                {
                    var res = ma.Run();
                    return res.IsSucc
                               ? res
                               : Fin<A>.Succ(f(res.Error));
                });
        
        [Pure, MethodImpl(Opt.Default)]
        public static Eff<A> IfFail<RT, A>(this Eff<A> ma, A alternative) =>
            EffMaybe<A>(
                () =>
                {
                    var res = ma.Run();
                    return res.IsSucc
                               ? res
                               : Fin<A>.Succ(alternative);
                });
        
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, A> IfFailAff<RT, A>(this Eff<A> ma, Aff<RT, A> alternative) where RT : struct, HasCancel<RT> =>
            AffMaybe<RT, A>(
                async env =>
                {
                    var res = ma.Run();
                    return res.IsSucc
                               ? res
                               : await alternative.Run(env).ConfigureAwait(false);
                });
        
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, A> IfFailAff<RT, A>(this Eff<A> ma, Func<Error, Aff<RT, A>> alternative) where RT : struct, HasCancel<RT> =>
            AffMaybe<RT, A>(
                async env =>
                {
                    var res = ma.Run();
                    return res.IsSucc
                               ? res
                               : await alternative(res.Error).Run(env).ConfigureAwait(false);
                });
        
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<A> IfFailAff<A>(this Eff<A> ma, Aff<A> alternative) =>
            AffMaybe<A>(
                async () =>
                {
                    var res = ma.Run();
                    return res.IsSucc
                               ? res
                               : await alternative.Run().ConfigureAwait(false);
                });
        
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<A> IfFailAff<A>(this Eff<A> ma, Func<Error, Aff<A>> alternative) =>
            AffMaybe<A>(
                async () =>
                {
                    var res = ma.Run();
                    return res.IsSucc
                               ? res
                               : await alternative(res.Error).Run().ConfigureAwait(false);
                });
        
        [Pure, MethodImpl(Opt.Default)]
        public static Eff<RT, A> IfFailEff<RT, A>(this Eff<A> ma, Eff<RT, A> alternative) where RT : struct =>
            EffMaybe<RT, A>(
                env =>
                {
                    var res = ma.Run();
                    return res.IsSucc
                               ? res
                               : alternative.Run(env);
                });
        
        [Pure, MethodImpl(Opt.Default)]
        public static Eff<RT, A> IfFailEff<RT, A>(this Eff<A> ma, Func<Error, Eff<RT, A>> alternative) where RT : struct =>
            EffMaybe<RT, A>(
                env =>
                {
                    var res = ma.Run();
                    return res.IsSucc
                               ? res
                               : alternative(res.Error).Run(env);
                });
        
        [Pure, MethodImpl(Opt.Default)]
        public static Eff<A> IfFailEff<A>(this Eff<A> ma, Eff<A> alternative) =>
            EffMaybe<A>(
                () =>
                {
                    var res = ma.Run();
                    return res.IsSucc
                               ? res
                               : alternative.Run();
                });
        
        [Pure, MethodImpl(Opt.Default)]
        public static Eff<A> IfFailEff<A>(this Eff<A> ma, Func<Error, Eff<A>> alternative) =>
            EffMaybe<A>(
                () =>
                {
                    var res = ma.Run();
                    return res.IsSucc
                               ? res
                               : alternative(res.Error).Run();
                });
        
        //
        // Iter / Do
        //
        
        [Pure, MethodImpl(Opt.Default)]
        public static Eff<Unit> Iter<A>(this Eff<A> ma, Func<A, Unit> f) =>
            Eff<Unit>(
                () =>
                {
                    var res = ma.Run();
                    if (res.IsSucc)
                    {
                        f(res.Value);
                    }
                    return unit;
                });
        
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, Unit> Iter<RT, A>(this Eff<A> ma, Func<A, Aff<RT, Unit>> f) where RT : struct, HasCancel<RT> =>
            Aff<RT, Unit>(
                async env =>
                {
                    var res = ma.Run();
                    if (res.IsSucc)
                    {
                        ignore(await f(res.Value).Run(env).ConfigureAwait(false));
                    }
                    return unit;
                });
        
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<Unit> Iter<A>(this Eff<A> ma, Func<A, Aff<Unit>> f) =>
            Aff<Unit>(
                async () =>
                {
                    var res = ma.Run();
                    if (res.IsSucc)
                    {
                        ignore(await f(res.Value).Run().ConfigureAwait(false));
                    }
                    return unit;
                });
        
        [Pure, MethodImpl(Opt.Default)]
        public static Eff<RT, Unit> Iter<RT, A>(this Eff<A> ma, Func<A, Eff<RT, Unit>> f) where RT : struct =>
            Eff<RT, Unit>(
                env =>
                {
                    var res = ma.Run();
                    if (res.IsSucc)
                    {
                        ignore(f(res.Value).Run(env));
                    }
                    return unit;
                });
        
        [Pure, MethodImpl(Opt.Default)]
        public static Eff<Unit> Iter<A>(this Eff<A> ma, Func<A, Eff<Unit>> f) =>
            Eff<Unit>(
                () =>
                {
                    var res = ma.Run();
                    if (res.IsSucc)
                    {
                        ignore(f(res.Value).Run());
                    }
                    return unit;
                });
        
        [Pure, MethodImpl(Opt.Default)]
        public static Eff<A> Do<A>(this Eff<A> ma, Func<A, Unit> f) =>
            EffMaybe<A>(
                () =>
                {
                    var res = ma.Run();
                    if (res.IsSucc)
                    {
                        f(res.Value);
                    }
                    return res;
                });
        
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, A> Do<RT, A>(this Eff<A> ma, Func<A, Aff<RT, Unit>> f) where RT : struct, HasCancel<RT> =>
            AffMaybe<RT, A>(
                async env =>
                {
                    var res = ma.Run();
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
        public static Eff<RT, A> Do<RT, A>(this Eff<A> ma, Func<A, Eff<RT, Unit>> f) where RT : struct =>
            EffMaybe<RT, A>(
                env =>
                {
                    var res = ma.Run();
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
        public static Aff<A> Do<A>(this Eff<A> ma, Func<A, Aff<Unit>> f) =>
            AffMaybe<A>(
                async () =>
                {
                    var res = ma.Run();
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
        public static Eff<A> Do<A>(this Eff<A> ma, Func<A, Eff<Unit>> f) =>
            EffMaybe<A>(
                () =>
                {
                    var res = ma.Run();
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
        public static Eff<A> Filter<A>(this Eff<A> ma, Func<A, bool> f) =>
            ma.Bind(x => f(x) ? SuccessEff<A>(x) : FailEff<A>(Errors.Cancelled));        
        
        //
        // Bind
        //

        [Pure, MethodImpl(Opt.Default)]
        public static Eff<B> Bind<A, B>(this Eff<A> ma, Func<A, Eff<B>> f) =>
            new(() =>
            {
                var fa = ma.Run();
                if (fa.IsFail) return FinFail<B>(fa.Error);
                var mb = f(fa.Value);
                return mb.Run();
            });

        [Pure, MethodImpl(Opt.Default)]
        public static Eff<RT, B> Bind<RT, A, B>(this Eff<A> ma, Func<A, Eff<RT, B>> f) where RT : struct =>
            new (env =>
            {
                var fa = ma.Run();
                if (fa.IsFail) return FinFail<B>(fa.Error);
                var mb = f(fa.Value);
                return mb.Run(env);
            });

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<B> Bind<A, B>(this Eff<A> ma, Func<A, Aff<B>> f) =>
            new(async () =>
            {
                var fa = ma.Run();
                if (fa.IsFail) return FinFail<B>(fa.Error);
                var mb = f(fa.Value);
                return await mb.Run().ConfigureAwait(false);
            });

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, B> Bind<RT, A, B>(this Eff<A> ma, Func<A, Aff<RT, B>> f)
            where RT : struct, HasCancel<RT> =>
            new(async env =>
            {
                var fa = ma.Run();
                if (fa.IsFail) return FinFail<B>(fa.Error);
                var mb = f(fa.Value);
                return await mb.Run(env).ConfigureAwait(false);
            });
        
        //
        // Bi-bind
        //
        
        [Pure, MethodImpl(Opt.Default)]
        public static Eff<B> BiBind<A, B>(this Eff<A> ma, Func<A, Eff<B>> Succ, Func<Error, Eff<B>> Fail) =>
            ma.Match(Succ, Fail)
              .Flatten();

        [Pure, MethodImpl(Opt.Default)]
        public static Eff<RT, B> BiBind<RT, A, B>(this Eff<A> ma, Func<A, Eff<RT, B>> Succ, Func<Error, Eff<RT, B>> Fail) where RT : struct =>
            ma.Match(Succ, Fail)
              .Flatten();

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<B> BiBind<A, B>(this Eff<A> ma, Func<A, Aff<B>> Succ, Func<Error, Aff<B>> Fail) =>
            ma.Match(Succ, Fail)
              .Flatten();

        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, B> BiBind<RT, A, B>(this Eff<A> ma, Func<A, Aff<RT, B>> Succ, Func<Error, Aff<RT, B>> Fail) where RT : struct, HasCancel<RT> =>
            ma.Match(Succ, Fail)
              .Flatten();

        //
        // Flatten
        //
        
        [Pure, MethodImpl(Opt.Default)]
        public static Eff<A> Flatten<A>(this Eff<Eff<A>> mma) =>
            new (() =>
            {
                var ma = mma.Run();
                if (ma.IsFail) return ma.Error;
                return ma.Value.Run();
            });

        [Pure, MethodImpl(Opt.Default)]
        public static Eff<RT, A> Flatten<RT, A>(this Eff<Eff<RT, A>> mma) where RT : struct =>
            new (rt =>
            {
                var ma = mma.Run();
                if (ma.IsFail) return ma.Error;
                return ma.Value.Run(rt);
            });
        
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<A> Flatten<A>(this Eff<Aff<A>> mma) =>
            new (async () =>
            {
                var ma = mma.Run();
                if (ma.IsFail) return ma.Error;
                return await ma.Value.Run().ConfigureAwait(false);
            });
        
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, A> Flatten<RT, A>(this Eff<Aff<RT, A>> mma) where RT : struct, HasCancel<RT> =>
            new (async rt =>
            {
                var ma = mma.Run();
                if (ma.IsFail) return ma.Error;
                return await ma.Value.Run(rt).ConfigureAwait(false);
            });

        //
        // Select
        //
        
        [Pure, MethodImpl(Opt.Default)]
        public static Eff<B> Select<A, B>(this Eff<A> ma, Func<A, B> f) =>
            Map(ma, f);

        //
        // SelectMany
        //
        
        [Pure, MethodImpl(Opt.Default)]
        public static Eff<B> SelectMany<A, B>(this Eff<A> ma, Func<A, Eff<B>> f) =>
            Bind(ma, f);
        
        [Pure, MethodImpl(Opt.Default)]
        public static Eff<RT, B> SelectMany<RT, A, B>(this Eff<A> ma, Func<A, Eff<RT, B>> f)  where RT : struct =>
            Bind(ma, f);
        
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<B> SelectMany<A, B>(this Eff<A> ma, Func<A, Aff<B>> f) =>
            Bind(ma, f);
        
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, B> SelectMany<RT, A, B>(this Eff<A> ma, Func<A, Aff<RT, B>> f) where RT : struct, HasCancel<RT> =>
            Bind(ma, f);
        
        [Pure, MethodImpl(Opt.Default)]
        public static Eff<C> SelectMany<A, B, C>(this Eff<A> ma, Func<A, Eff<B>> bind, Func<A, B, C> project) =>
            Bind(ma, x => Map(bind(x), y => project(x, y)));
        
        [Pure, MethodImpl(Opt.Default)]
        public static Eff<RT, C> SelectMany<RT, A, B, C>(this Eff<A> ma, Func<A, Eff<RT, B>> bind, Func<A, B, C> project) where RT : struct, HasCancel<RT> =>
            Bind(ma, x => Map(bind(x), y => project(x, y)));
        
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<C> SelectMany<A, B, C>(this Eff<A> ma, Func<A, Aff<B>> bind, Func<A, B, C> project) =>
            Bind(ma, x => Map(bind(x), y => project(x, y)));
        
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, C> SelectMany<RT, A, B, C>(this Eff<A> ma, Func<A, Aff<RT, B>> bind, Func<A, B, C> project) where RT : struct, HasCancel<RT> =>
            Bind(ma, x => Map(bind(x), y => project(x, y)));
        
        [Pure, MethodImpl(Opt.Default)]
        public static Aff<RT, C> SelectMany<RT, A, B, C>(this Eff<A> ma, Func<A, Effect<RT, B>> bind, Func<A, B, C> project) where RT : struct, HasCancel<RT> =>
            Bind(ma, x => Map(bind(x).RunEffect(), y => project(x, y)));

        //
        // Where
        //
        
        [Pure, MethodImpl(Opt.Default)]
        public static Eff<A> Where<A>(this Eff<A> ma, Func<A, bool> f) =>
            Filter(ma, f);

        //
        // Zip
        //

        [Pure, MethodImpl(Opt.Default)]
        public static Eff<(A, B)> Zip<A, B>(Eff<A> ma, Eff<B> mb) =>
            new(() =>
            {
                var ta = ma.Run();
                if (ta.IsFail) return ta.Cast<(A, B)>();
                var tb = mb.Run();
                if (tb.IsFail) return tb.Cast<(A, B)>();
                return Fin<(A, B)>.Succ((ta.Value, tb.Value));
            });
    }
}
