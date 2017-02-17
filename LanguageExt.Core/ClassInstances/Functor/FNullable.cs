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
            ma.HasValue
                ? fb == null
                    ? (B?)null
                    : fb(ma.Value)
                : fa == null
                    ? (B?)null
                    : fa(unit);

        [Pure]
        public B? Map(A? ma, Func<A, B> f) =>
            ma.HasValue && f != null
                ? f(ma.Value)
                : (B?)null;
    }
}
