#nullable enable

using System;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using LanguageExt.DataTypes.Serialisation;

namespace LanguageExt.ClassInstances
{
    public readonly struct ApplEitherAsync<L, A, B> :
        BiFunctorAsync<EitherAsync<L, A>, EitherAsync<L, B>, L, A, L, B>,
        ApplicativeAsync<EitherAsync<L, Func<A, B>>, EitherAsync<L, A>, EitherAsync<L, B>, A, B>
    {
        public static ApplEitherAsync<L, A, B> Inst = default;

        [Pure]
        public EitherAsync<L, B> Action(EitherAsync<L, A> fa, EitherAsync<L, B> fb)
        {
            return new EitherAsync<L, B>(Go());
            async Task<EitherData<L, B>> Go()
            {
                var (ta, tb) = await WaitAsync.All(fa.Data, fb.Data);
                return (ta.State, tb.State) switch
                {
                    (EitherStatus.IsRight, EitherStatus.IsRight) => tb,
                    (EitherStatus.IsLeft, _) => EitherData.Left<L, B>(ta.Left),
                    (_, EitherStatus.IsLeft) => tb,
                    _ => EitherData<L, B>.Bottom
                };
            }
        }

        [Pure]
        public EitherAsync<L, B> Apply(EitherAsync<L, Func<A, B>> fab, EitherAsync<L, A> fa)
        {
            return new EitherAsync<L, B>(Go());
            async Task<EitherData<L, B>> Go()
            {
                var (ta, tb) = await WaitAsync.All(fab.Data, fa.Data);
                return (ta.State, tb.State) switch
                {
                    (EitherStatus.IsRight, EitherStatus.IsRight) => EitherData.Right<L, B>(ta.Right(tb.Right)),
                    (EitherStatus.IsLeft, _) => EitherData.Left<L, B>(ta.Left),
                    (_, EitherStatus.IsLeft) => EitherData.Left<L, B>(tb.Left),
                    _ => EitherData<L, B>.Bottom
                };
            }
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public EitherAsync<L, A> PureAsync(Task<A> x) =>
            EitherAsync<L, A>.RightAsync(x);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public EitherAsync<L, B> BiMapAsync(EitherAsync<L, A> ma, Func<L, ValueTask<L>> fa, Func<A, ValueTask<B>> fb) =>
            default(FEitherAsync<L, A, B>).BiMapAsync(ma, fa, fb);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public EitherAsync<L, B> Map(EitherAsync<L, A> ma, Func<A, B> f) => 
            default(FEitherAsync<L, A, B>).Map(ma, f);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public EitherAsync<L, B> MapAsync(EitherAsync<L, A> ma, Func<A, ValueTask<B>> f) =>
            default(FEitherAsync<L, A, B>).MapAsync(ma, f);
    }
}
