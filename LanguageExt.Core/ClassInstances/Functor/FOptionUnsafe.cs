using System;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances
{
    public struct FOptionUnsafe<A, B> : 
        Functor<OptionUnsafe<A>, OptionUnsafe<B>, A, B>,
        BiFunctor<OptionUnsafe<A>, OptionUnsafe<B>, Unit, A, B>
    {
        public static readonly FOptionUnsafe<A, B> Inst = default(FOptionUnsafe<A, B>);

        [Pure]
        public OptionUnsafe<B> BiMap(OptionUnsafe<A> ma, Func<Unit, B> fa, Func<A, B> fb) =>
            ma.IsNone
                ? fa == null
                    ? OptionUnsafe<B>.None
                    : fa(unit)
                : fb == null
                    ? OptionUnsafe<B>.None
                    : fb(ma.Value);

        [Pure]
        public OptionUnsafe<B> Map(OptionUnsafe<A> ma, Func<A, B> f) =>
            ma.IsSome
                ? OptionUnsafe<B>.Some(f(ma.Value))
                : OptionUnsafe<B>.None;
    }
}
