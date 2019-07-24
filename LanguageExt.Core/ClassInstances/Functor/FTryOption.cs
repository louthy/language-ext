using System;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances
{
    public struct FTryOption<A, B> : 
        Functor<TryOption<A>, TryOption<B>, A, B>,
        BiFunctor<TryOption<A>, TryOption<B>, A, Unit, B>
    {
        public static readonly FTryOption<A, B> Inst = default(FTryOption<A, B>);

        [Pure]
        public TryOption<B> BiMap(TryOption<A> ma, Func<A, B> fa, Func<Unit, B> fb) =>
            FOptional<MTryOption<A>, MTryOption<B>, TryOption<A>, TryOption<B>, A, B>.Inst.BiMap(ma, fa, fb);

        [Pure]
        public TryOption<B> Map(TryOption<A> ma, Func<A, B> f) =>
            FOptional<MTryOption<A>, MTryOption<B>, TryOption<A>, TryOption<B>, A, B>.Inst.Map(ma, f);
    }
}
