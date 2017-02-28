using System;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances
{
    public struct FNullable<A, B> : 
        Functor<A?, B?, A, B>,
        BiFunctor<A?, B?, Unit, A, B>
        where A : struct
        where B : struct
    {
        public static readonly FNullable<A, B> Inst = default(FNullable<A, B>);

        [Pure]
        public B? BiMap(A? ma, Func<Unit, B> fa, Func<A, B> fb) =>
            FOptional<MNullable<A>, MNullable<B>, A?, B?, A, B>.Inst.BiMap(ma, fa, fb);

        [Pure]
        public B? Map(A? ma, Func<A, B> f) =>
            FOptional<MNullable<A>, MNullable<B>, A?, B?, A, B>.Inst.Map(ma, f);
    }
}
