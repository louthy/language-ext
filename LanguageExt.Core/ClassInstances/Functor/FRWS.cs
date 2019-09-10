using System;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances
{
    public struct FRWS<MonoidW, R, W, S, A, B> :
        Functor<RWS<MonoidW, R, W, S, A>, RWS<MonoidW, R, W, S, B>, A, B>
        where MonoidW : struct, Monoid<W>
    {
        public static readonly FRWS<MonoidW, R, W, S, A, B> Inst = default;

        [Pure]
        public RWS<MonoidW, R, W, S, B> Map(RWS<MonoidW, R, W, S, A> ma, Func<A, B> f) =>
            MRWS<MonoidW, R, W, S, A>.Inst.Bind<MRWS<MonoidW, R, W, S, B>, RWS<MonoidW, R, W, S, B>, B>(ma, a =>
                MRWS<MonoidW, R, W, S, B>.Inst.Return(_ => f(a)));
    }
}
