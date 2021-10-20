using System;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances
{
    public struct ApplEitherAsync<L, A, B> :
        FunctorAsync<EitherAsync<L, A>, EitherAsync<L, B>, A, B>,
        BiFunctorAsync<EitherAsync<L, A>, EitherAsync<L, B>, L, A, B>,
        ApplicativeAsync<EitherAsync<L, Func<A, B>>, EitherAsync<L, A>, EitherAsync<L, B>, A, B>
    {
        public static ApplEitherAsync<L, A, B> Inst = default(ApplEitherAsync<L, A, B>);

        [Pure]
        public EitherAsync<L, B> Action(EitherAsync<L, A> fa, EitherAsync<L, B> fb) =>
            from a in fa
            from b in fb
            select b;

        [Pure]
        public EitherAsync<L, B> Apply(EitherAsync<L, Func<A, B>> fab, EitherAsync<L, A> fa) =>
            from f in fab
            from a in fa
            select f(a);

        [Pure]
        public EitherAsync<L, B> BiMapAsync(EitherAsync<L, A> ma, Func<L, B> fa, Func<A, B> fb) =>
            default(FEitherAsync<L, A, B>).BiMapAsync(ma, fa, fb);

        [Pure]
        public EitherAsync<L, B> BiMapAsync(EitherAsync<L, A> ma, Func<L, Task<B>> fa, Func<A, B> fb) =>
            default(FEitherAsync<L, A, B>).BiMapAsync(ma, fa, fb);

        [Pure]
        public EitherAsync<L, B> BiMapAsync(EitherAsync<L, A> ma, Func<L, B> fa, Func<A, Task<B>> fb) =>
            default(FEitherAsync<L, A, B>).BiMapAsync(ma, fa, fb);

        [Pure]
        public EitherAsync<L, B> BiMapAsync(EitherAsync<L, A> ma, Func<L, Task<B>> fa, Func<A, Task<B>> fb) =>
            default(FEitherAsync<L, A, B>).BiMapAsync(ma, fa, fb);

        [Pure]
        public EitherAsync<L, B> Map(EitherAsync<L, A> ma, Func<A, B> f) =>
            ma.Map(f);

        [Pure]
        public EitherAsync<L, B> MapAsync(EitherAsync<L, A> ma, Func<A, Task<B>> f) =>
            ma.MapAsync(f);

        [Pure]
        public EitherAsync<L, A> PureAsync(Task<A> x) =>
            EitherAsync<L, A>.RightAsync(x);
    }

    public struct ApplEitherAsync<L, A, B, C> :
        ApplicativeAsync<EitherAsync<L, Func<A, Func<B, C>>>, EitherAsync<L, Func<B, C>>, EitherAsync<L, A>, EitherAsync<L, B>, EitherAsync<L, C>, A, B, C>
    {
        public static ApplEitherAsync<L, A, B, C> Inst = default(ApplEitherAsync<L, A, B, C>);

        public EitherAsync<L, Func<B, C>> Apply(EitherAsync<L, Func<A, Func<B, C>>> fabc, EitherAsync<L, A> fa) =>
            from f in fabc
            from a in fa
            select f(a);

        public EitherAsync<L, C> Apply(EitherAsync<L, Func<A, Func<B, C>>> fabc, EitherAsync<L, A> fa, EitherAsync<L, B> fb) =>
            from f in fabc
            from a in fa
            from b in fb
            select f(a)(b);

        public EitherAsync<L, A> PureAsync(Task<A> x) =>
            EitherAsync<L, A>.RightAsync(x);
    }
}
