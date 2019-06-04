using System;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances
{
    public struct ApplWriter<MonoidW, W, A, B, C> :
        Applicative<Writer<MonoidW, W, Func<A, Func<B, C>>>, Writer<MonoidW, W, Func<B, C>>, Writer<MonoidW, W,A>, Writer<MonoidW, W, B>, Writer<MonoidW, W,C>, A, B, C>
        where MonoidW : struct, Monoid<W>
    {
        public static readonly ApplWriter<MonoidW, W, A, B, C> Inst = default;

        [Pure]
        public Writer<MonoidW, W, Func<B, C>> Apply(Writer<MonoidW, W, Func<A, Func<B, C>>> fabc, Writer<MonoidW, W, A> fa) =>
            ApplWriter<MonoidW, W, A, Func<B, C>>.Inst.Apply(fabc, fa);

        [Pure]
        public Writer<MonoidW, W, C> Apply(Writer<MonoidW, W, Func<A, Func<B, C>>> fabc, Writer<MonoidW, W, A> fa, Writer<MonoidW, W, B> fb) =>
            ApplWriter<MonoidW, W, B, C>.Inst.Apply(Apply(fabc, fa), fb);

        [Pure]
        public Writer<MonoidW, W, A> Pure(A x) =>
            MWriter<MonoidW, W, A>.Inst.Return(_ => x);
    }

    public struct ApplWriter<MonoidW, W, A, B> :
        Functor<Writer<MonoidW, W, A>, Writer<MonoidW, W, B>, A, B>,
        Applicative<Writer<MonoidW, W, Func<A, B>>, Writer<MonoidW, W, A>, Writer<MonoidW, W, B>, A, B>
        where MonoidW : struct, Monoid<W>
    {
        public static readonly ApplWriter<MonoidW, W, A, B> Inst = default;

        [Pure]
        public Writer<MonoidW, W, B> Action(Writer<MonoidW, W, A> fa, Writer<MonoidW, W, B> fb) =>
            MWriter<MonoidW, W, A>.Inst.Bind<MWriter<MonoidW, W, B>, Writer<MonoidW, W, B>, B>(fa, _ => fb);

        [Pure]
        public Writer<MonoidW, W, B> Apply(Writer<MonoidW, W, Func<A, B>> fab, Writer<MonoidW, W, A> fa) =>
            MWriter<MonoidW, W, Func<A, B>>.Inst.Bind<MWriter<MonoidW, W, B>, Writer<MonoidW, W, B>, B>(fab, f =>
                MWriter<MonoidW, W, A>.Inst.Bind<MWriter<MonoidW, W, B>, Writer<MonoidW, W, B>, B>(fa, a =>
                    MWriter<MonoidW, W, B>.Inst.Return(_ => f(a))));
        [Pure]
        public Writer<MonoidW, W, B> Map(Writer<MonoidW, W, A> ma, Func<A, B> f) =>
            FWriter<MonoidW, W, A, B>.Inst.Map(ma, f);

        [Pure]
        public Writer<MonoidW, W, A> Pure(A x) =>
            MWriter<MonoidW, W, A>.Inst.Return(_ => x);
    }

}
