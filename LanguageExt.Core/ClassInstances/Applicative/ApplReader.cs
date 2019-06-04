using System;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances
{
    public struct ApplReader<Env, A, B, C> :
        Applicative<Reader<Env, Func<A, Func<B, C>>>, Reader<Env, Func<B, C>>, Reader<Env,A>, Reader<Env, B>, Reader<Env,C>, A, B, C>
    {
        public static readonly ApplReader<Env, A, B, C> Inst = default;

        [Pure]
        public Reader<Env, Func<B, C>> Apply(Reader<Env, Func<A, Func<B, C>>> fabc, Reader<Env, A> fa) =>
            ApplReader<Env, A, Func<B, C>>.Inst.Apply(fabc, fa);

        [Pure]
        public Reader<Env, C> Apply(Reader<Env, Func<A, Func<B, C>>> fabc, Reader<Env, A> fa, Reader<Env, B> fb) =>
            ApplReader<Env, B, C>.Inst.Apply(Apply(fabc, fa), fb);

        [Pure]
        public Reader<Env, A> Pure(A x) =>
            MReader<Env, A>.Inst.Return(_ => x);
    }

    public struct ApplReader<Env, A, B> :
        Functor<Reader<Env, A>, Reader<Env, B>, A, B>,
        Applicative<Reader<Env, Func<A, B>>, Reader<Env, A>, Reader<Env, B>, A, B>
    {
        public static readonly ApplReader<Env, A, B> Inst = default;

        [Pure]
        public Reader<Env, B> Action(Reader<Env, A> fa, Reader<Env, B> fb) =>
            MReader<Env, A>.Inst.Bind<MReader<Env, B>, Reader<Env, B>, B>(fa, _ => fb);

        [Pure]
        public Reader<Env, B> Apply(Reader<Env, Func<A, B>> fab, Reader<Env, A> fa) =>
            MReader<Env, Func<A, B>>.Inst.Bind<MReader<Env, B>, Reader<Env, B>, B>(fab, f =>
                MReader<Env, A>.Inst.Bind<MReader<Env, B>, Reader<Env, B>, B>(fa, a =>
                    MReader<Env, B>.Inst.Return(_ => f(a))));
        [Pure]
        public Reader<Env, B> Map(Reader<Env, A> ma, Func<A, B> f) =>
            FReader<Env, A, B>.Inst.Map(ma, f);

        [Pure]
        public Reader<Env, A> Pure(A x) =>
            MReader<Env, A>.Inst.Return(_ => x);
    }

}
