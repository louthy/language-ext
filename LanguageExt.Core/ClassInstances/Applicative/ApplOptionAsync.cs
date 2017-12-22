using System;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances
{
    public struct ApplOptionAsync<A, B> :
        FunctorAsync<OptionAsync<A>, OptionAsync<B>, A, B>,
        BiFunctorAsync<OptionAsync<A>, OptionAsync<B>, A, Unit, B>,
        Applicative<OptionAsync<Func<A, B>>, OptionAsync<A>, OptionAsync<B>, A, B>
    {
        public static readonly ApplOptionAsync<A, B> Inst = default(ApplOptionAsync<A, B>);

        [Pure]
        public OptionAsync<B> BiMapAsync(OptionAsync<A> ma, Func<A, B> fa, Func<Unit, B> fb) =>
            default(FOptionAsync<A, B>).BiMapAsync(ma, fa, fb);

        [Pure]
        public OptionAsync<B> BiMapAsync(OptionAsync<A> ma, Func<A, B> fa, Func<Unit, Task<B>> fb) =>
            default(FOptionAsync<A, B>).BiMapAsync(ma, fa, fb);

        [Pure]
        public OptionAsync<B> BiMapAsync(OptionAsync<A> ma, Func<A, Task<B>> fa, Func<Unit, Task<B>> fb) =>
            default(FOptionAsync<A, B>).BiMapAsync(ma, fa, fb);

        [Pure]
        public OptionAsync<B> BiMapAsync(OptionAsync<A> ma, Func<A, Task<B>> fa, Func<Unit, B> fb) =>
            default(FOptionAsync<A, B>).BiMapAsync(ma, fa, fb);

        [Pure]
        public OptionAsync<B> MapAsync(OptionAsync<A> ma, Func<A, B> f) =>
            default(FOptionAsync<A, B>).MapAsync(ma, f);

        [Pure]
        public OptionAsync<B> MapAsync(OptionAsync<A> ma, Func<A, Task<B>> f) =>
            default(FOptionAsync<A, B>).MapAsync(ma, f);

        [Pure]
        public OptionAsync<B> Apply(OptionAsync<Func<A, B>> fab, OptionAsync<A> fa) =>
            from f in fab
            from a in fa
            select f(a);

        [Pure]
        public OptionAsync<A> Pure(A x) =>
            MOptionAsync<A>.Inst.ReturnAsync(x.AsTask());

        [Pure]
        public OptionAsync<B> Action(OptionAsync<A> fa, OptionAsync<B> fb) =>
            from a in fa
            from b in fb
            select b;
    }

    public struct ApplOptionAsync<A, B, C> :
        Applicative<OptionAsync<Func<A, Func<B, C>>>, OptionAsync<Func<B, C>>, OptionAsync<A>, OptionAsync<B>, OptionAsync<C>, A, B, C>
    {
        public static readonly ApplOptionAsync<A, B, C> Inst = default(ApplOptionAsync<A, B, C>);

        [Pure]
        public OptionAsync<Func<B, C>> Apply(OptionAsync<Func<A, Func<B, C>>> fab, OptionAsync<A> fa) =>
            from f in fab
            from a in fa
            select f(a);

        [Pure]
        public OptionAsync<C> Apply(OptionAsync<Func<A, Func<B, C>>> fab, OptionAsync<A> fa, OptionAsync<B> fb) =>
            from f in fab
            from a in fa
            from b in fb
            select f(a)(b);

        [Pure]
        public OptionAsync<A> Pure(A x) =>
            MOptionAsync<A>.Inst.ReturnAsync(x.AsTask());
    }
}
