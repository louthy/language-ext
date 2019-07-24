using System;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances
{
    public struct FOptionUnsafe<A, B> : 
        Functor<OptionUnsafe<A>, OptionUnsafe<B>, A, B>,
        BiFunctor<OptionUnsafe<A>, OptionUnsafe<B>, A, Unit, B>
    {
        public static readonly FOptionUnsafe<A, B> Inst = default(FOptionUnsafe<A, B>);

        [Pure]
        public OptionUnsafe<B> BiMap(OptionUnsafe<A> ma, Func<A, B> fa, Func<Unit, B> fb) =>
            FOptionalUnsafe<
                MOptionUnsafe<A>, MOptionUnsafe<B>, 
                OptionUnsafe<A>, OptionUnsafe<B>, 
                A, B>
            .Inst.BiMap(ma, fa, fb);

        [Pure]
        public OptionUnsafe<B> Map(OptionUnsafe<A> ma, Func<A, B> f) =>
            MOptionUnsafe<A>.Inst.Bind<MOptionUnsafe<B>, OptionUnsafe<B>, B>(ma, a => MOptionUnsafe<B>.Inst.Return(f(a)));
    }
}
