using System;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances
{
    public struct ApplResource<A> :
        Applicative<Resource<Func<A, A>>, Resource<A>, Resource<A>, A, A>,
        Applicative<Resource<Func<A, Func<A, A>>>, Resource<Func<A, A>>, Resource<A>, Resource<A>, Resource<A>, A, A, A>
    {
        [Pure]
        public Resource<A> Action(Resource<A> fa, Resource<A> fb) =>
            ApplResource<A, A>.Inst.Action(fa, fb);

        public Resource<A> Apply(Resource<Func<A, A>> fab, Resource<A> fa) =>
            ApplResource<A, A>.Inst.Apply(fab, fa);

        [Pure]
        public Resource<Func<A, A>> Apply(Resource<Func<A, Func<A, A>>> fab, Resource<A> fa) =>
            ApplResource<A, Func<A, A>>.Inst.Apply(fab, fa);

        [Pure]
        public Resource<A> Apply(Resource<Func<A, Func<A, A>>> fab, Resource<A> fa, Resource<A> fb) =>
            Apply(Apply(fab, fa), fb);

        [Pure]
        public Resource<A> Pure(A x) =>
            ApplResource<A, A>.Inst.Pure(x);
    }

    public struct ApplResource<A, B> :

        Functor<Resource<A>, Resource<B>, A, B>,
        Applicative<Resource<Func<A, B>>, Resource<A>, Resource<B>, A, B>
    {
        public static readonly ApplResource<A, B> Inst = default;

        [Pure]
        public Resource<B> Action(Resource<A> fa, Resource<B> fb) =>
            MResource<A>.Inst.Bind<MResource<B>, Resource<B>, B>(fa, _ => fb);

        [Pure]
        public Resource<B> Apply(Resource<Func<A, B>> fab, Resource<A> fa) =>
            MResource<Func<A, B>>.Inst.Bind<MResource<B>, Resource<B>, B>(fab, f =>
                MResource<A>.Inst.Bind<MResource<B>, Resource<B>, B>(fa, a =>
                    MResource<B>.Inst.Return(_ => f(a))));

        [Pure]
        public Resource<B> Map(Resource<A> ma, Func<A, B> f) =>
            FResource<A, B>.Inst.Map(ma, f);

        [Pure]
        public Resource<A> Pure(A x) =>
            MResource<A>.Inst.Return(_ => x);
    }

    public struct ApplResource<A, B, C> :
        Applicative<Resource<Func<A, Func<B, C>>>, Resource<Func<B, C>>, Resource<A>, Resource<B>, Resource<C>, A, B, C>
    {
        public static readonly ApplResource<A, B, C> Inst = default;

        [Pure]
        public Resource<Func<B, C>> Apply(Resource<Func<A, Func<B, C>>> fabc, Resource<A> fa) =>
            ApplResource<A, Func<B, C>>.Inst.Apply(fabc, fa);

        [Pure]
        public Resource<C> Apply(Resource<Func<A, Func<B, C>>> fabc, Resource<A> fa, Resource<B> fb) =>
            ApplResource<B,C>.Inst.Apply(Apply(fabc, fa), fb);

        [Pure]
        public Resource<A> Pure(A x) =>
            MResource<A>.Inst.Return(_ => x);
    }
}
