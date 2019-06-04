using System;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances
{
    public struct FWriter<MonoidW, W, A, B> :
        Functor<Writer<MonoidW, W, A>, Writer<MonoidW, W, B>, A, B>
        where MonoidW : struct, Monoid<W>
    {
        public static readonly FWriter<MonoidW, W, A, B> Inst = default;

        [Pure]
        public Writer<MonoidW, W, B> Map(Writer<MonoidW, W, A> ma, Func<A, B> f) =>
            MWriter<MonoidW, W, A>.Inst.Bind<MWriter<MonoidW, W, B>, Writer<MonoidW, W, B>, B>(ma, a =>
                MWriter<MonoidW, W, B>.Inst.Return(_ => f(a)));
    }
}
