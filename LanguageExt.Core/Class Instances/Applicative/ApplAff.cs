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
    public readonly struct ApplAff<RT, A, B> : 
        BiFunctorAsync<Aff<RT, A>, Aff<RT, B>, Error, A, Error, B>,
        ApplicativeAsync<Aff<RT, Func<A, B>>, Aff<RT, A>, Aff<RT, B>, A, B>
        where RT : struct, HasCancel<RT>
    {
        public static readonly ApplAff<RT, A, B> Inst = default;

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Aff<RT, B> BiMapAsync(Aff<RT, A> ma, Func<Error, ValueTask<Error>> fa, Func<A, ValueTask<B>> fb) =>
            default(FAff<RT, A, B>).BiMapAsync(ma, fa, fb);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Aff<RT, B> Map(Aff<RT, A> ma, Func<A, B> f) =>
            default(FAff<RT, A, B>).Map(ma, f);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Aff<RT, B> MapAsync(Aff<RT, A> ma, Func<A, ValueTask<B>> f) =>
            default(FAff<RT, A, B>).MapAsync(ma, f);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Aff<RT, A> PureAsync(Task<A> x) =>
            Aff(async () => await x.ConfigureAwait(false)).Memo();

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Aff<RT, B> Apply(Aff<RT, Func<A, B>> fab, Aff<RT, A> fa) =>
            AffMaybe<RT, B>(async env =>
            {
                var (f, a) = await WaitAsync.All(fab.Run(env), fa.Run(env)).ConfigureAwait(false);

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
        public Aff<RT, B> Action(Aff<RT, A> fa, Aff<RT, B> fb) => 
            AffMaybe<RT, B>(async env =>
            {
                var (a, b) = await WaitAsync.All(fa.Run(env), fb.Run(env)).ConfigureAwait(false);

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
    
    public readonly struct ApplAff<A, B> : 
        BiFunctorAsync<Aff<A>, Aff<B>, Error, A, Error, B>,
        ApplicativeAsync<Aff<Func<A, B>>, Aff<A>, Aff<B>, A, B>
    {
        public static readonly ApplAff<A, B> Inst = default;

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Aff<B> BiMapAsync(Aff<A> ma, Func<Error, ValueTask<Error>> fa, Func<A, ValueTask<B>> fb) =>
            default(FAff<A, B>).BiMapAsync(ma, fa, fb);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Aff<B> Map(Aff<A> ma, Func<A, B> f) =>
            default(FAff<A, B>).Map(ma, f);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Aff<B> MapAsync(Aff<A> ma, Func<A, ValueTask<B>> f) =>
            default(FAff<A, B>).MapAsync(ma, f);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Aff<A> PureAsync(Task<A> x) =>
            Aff(async () => await x.ConfigureAwait(false)).Memo();

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Aff<B> Apply(Aff<Func<A, B>> fab, Aff<A> fa) =>
            AffMaybe<B>(async () =>
            {
                var (f, a) = await WaitAsync.All(fab.Run(), fa.Run()).ConfigureAwait(false);

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
        public Aff<B> Action(Aff<A> fa, Aff<B> fb) => 
            AffMaybe<B>(async () =>
            {
                var (a, b) = await WaitAsync.All(fa.Run(), fb.Run()).ConfigureAwait(false);

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
