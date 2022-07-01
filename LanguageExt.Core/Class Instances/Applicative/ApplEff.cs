#nullable enable

using System;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using LanguageExt.Common;
using LanguageExt.Effects.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt.ClassInstances
{
    public readonly struct ApplEff<RT, A, B> : 
        BiFunctor<Eff<RT, A>, Eff<RT, B>, Error, A, Error, B>,
        Applicative<Eff<RT, Func<A, B>>, Eff<RT, A>, Eff<RT, B>, A, B>
        where RT : struct
    {
        public static readonly ApplEff<RT, A, B> Inst = default;

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Eff<RT, B> BiMap(Eff<RT, A> ma, Func<Error, Error> fa, Func<A, B> fb) =>
            default(FEff<RT, A, B>).BiMap(ma, fa, fb);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Eff<RT, B> Map(Eff<RT, A> ma, Func<A, B> f) =>
            default(FEff<RT, A, B>).Map(ma, f);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Eff<RT, A> Pure(A x) =>
            SuccessEff(x);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Eff<RT, B> Apply(Eff<RT, Func<A, B>> fab, Eff<RT, A> fa) =>
            EffMaybe<RT, B>(env =>
            {
                var (f, a) = (fab.Run(env), fa.Run(env));

                if (f.IsFail || a.IsFail)
                {
                    var errs = Errors.None;
                    if (f.IsFail) errs += f.Error;
                    if (a.IsFail) errs += a.Error;
                    return errs;
                }
                else
                {
                    return f.Value(a.Value);
                }
            });

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Eff<RT, B> Action(Eff<RT, A> fa, Eff<RT, B> fb) => 
            EffMaybe<RT, B>(env =>
            {
                var (a, b) = (fa.Run(env), fb.Run(env));

                if (a.IsFail || b.IsFail)
                {
                    var errs = Errors.None;
                    if (a.IsFail) errs += b.Error;
                    if (a.IsFail) errs += a.Error;
                    return errs;
                }
                else
                {
                    return b.Value;
                }
            });
    }
    
    public readonly struct ApplEff<A, B> : 
        BiFunctor<Eff<A>, Eff<B>, Error, A, Error, B>,
        Applicative<Eff<Func<A, B>>, Eff<A>, Eff<B>, A, B>
    {
        public static readonly ApplEff<A, B> Inst = default;

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Eff<B> BiMap(Eff<A> ma, Func<Error, Error> fa, Func<A,B> fb) =>
            default(FEff<A, B>).BiMap(ma, fa, fb);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Eff<B> Map(Eff<A> ma, Func<A, B> f) =>
            default(FEff<A, B>).Map(ma, f);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Eff<A> Pure(A x) =>
            SuccessEff(x);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Eff<B> Apply(Eff<Func<A, B>> fab, Eff<A> fa) =>
            EffMaybe<B>(() =>
            {
                var (f, a) = (fab.Run(), fa.Run());

                if (f.IsFail || a.IsFail)
                {
                    var errs = Errors.None;
                    if (f.IsFail) errs += f.Error;
                    if (a.IsFail) errs += a.Error;
                    return errs;
                }
                else
                {
                    return f.Value(a.Value);
                }
            });

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Eff<B> Action(Eff<A> fa, Eff<B> fb) => 
            EffMaybe<B>(() =>
            {
                var (a, b) = (fa.Run(), fb.Run());

                if (a.IsFail || b.IsFail)
                {
                    var errs = Errors.None;
                    if (a.IsFail) errs += b.Error;
                    if (a.IsFail) errs += a.Error;
                    return errs;
                }
                else
                {
                    return b.Value;
                }
            });
    }
}
