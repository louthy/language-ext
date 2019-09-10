using System;
using System.Diagnostics.Contracts;
using LanguageExt.TypeClasses;

namespace LanguageExt.ClassInstances
{
    public struct ApplRWS<MonoidW, R, W, S, A, B, C> :
        Applicative<RWS<MonoidW, R, W, S, Func<A, Func<B, C>>>, RWS<MonoidW, R, W, S, Func<B, C>>, RWS<MonoidW, R, W, S, A>, RWS<MonoidW, R, W, S, B>, RWS<MonoidW, R, W, S, C>, A, B, C>
        where MonoidW : struct, Monoid<W>
    {
        public static readonly ApplRWS<MonoidW, R, W, S, A, B, C> Inst = default;

        [Pure]
        public RWS<MonoidW, R, W, S, Func<B, C>> Apply(RWS<MonoidW, R, W, S, Func<A, Func<B, C>>> fabc, RWS<MonoidW, R, W, S, A> fa) =>
            ApplRWS<MonoidW, R, W, S, A, Func<B, C>>.Inst.Apply(fabc, fa);

        [Pure]
        public RWS<MonoidW, R, W, S, C> Apply(RWS<MonoidW, R, W, S, Func<A, Func<B, C>>> fabc, RWS<MonoidW, R, W, S, A> fa, RWS<MonoidW, R, W, S, B> fb) =>
            ApplRWS<MonoidW, R, W, S, B, C>.Inst.Apply(Apply(fabc, fa), fb);

        [Pure]
        public RWS<MonoidW, R, W, S, A> Pure(A x) =>
            MRWS<MonoidW, R, W, S, A>.Inst.Return(_ => x);
    }

    public struct ApplRWS<MonoidW, R, W, S, A, B> :
        Functor<RWS<MonoidW, R, W, S, A>, RWS<MonoidW, R, W, S, B>, A, B>,
        Applicative<RWS<MonoidW, R, W, S, Func<A, B>>, RWS<MonoidW, R, W, S, A>, RWS<MonoidW, R, W, S, B>, A, B>
        where MonoidW : struct, Monoid<W>
    {
        public static readonly ApplRWS<MonoidW, R, W, S, A, B> Inst = default;

        [Pure]
        public RWS<MonoidW, R, W, S, B> Action(RWS<MonoidW, R, W, S, A> fa, RWS<MonoidW, R, W, S, B> fb) =>
            MRWS<MonoidW, R, W, S, A>.Inst.Bind<MRWS<MonoidW, R, W, S, B>, RWS<MonoidW, R, W, S, B>, B>(fa, _ => fb);

        [Pure]
        public RWS<MonoidW, R, W, S, B> Apply(RWS<MonoidW, R, W, S, Func<A, B>> fab, RWS<MonoidW, R, W, S, A> fa) =>
            MRWS<MonoidW, R, W, S, Func<A, B>>.Inst.Bind<MRWS<MonoidW, R, W, S, B>, RWS<MonoidW, R, W, S, B>, B>(fab, f =>
                MRWS<MonoidW, R, W, S, A>.Inst.Bind<MRWS<MonoidW, R, W, S, B>, RWS<MonoidW, R, W, S, B>, B>(fa, a =>
                    MRWS<MonoidW, R, W, S, B>.Inst.Return(_ => f(a))));
        [Pure]
        public RWS<MonoidW, R, W, S, B> Map(RWS<MonoidW, R, W, S, A> ma, Func<A, B> f) =>
            FRWS<MonoidW, R, W, S, A, B>.Inst.Map(ma, f);

        [Pure]
        public RWS<MonoidW, R, W, S, A> Pure(A x) =>
            MRWS<MonoidW, R, W, S, A>.Inst.Return(_ => x);
    }

}
