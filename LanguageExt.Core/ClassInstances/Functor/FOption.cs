using System;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances
{
    public struct FOption<A, B> : 
        Functor<Option<A>, Option<B>, A, B>,
        BiFunctor<Option<A>, Option<B>, Unit, A, B>
    {
        public static readonly FOption<A, B> Inst = default(FOption<A, B>);

        [Pure]
        public Option<B> BiMap(Option<A> ma, Func<Unit, B> fa, Func<A, B> fb) =>
            FOptional<MOption<A>, MOption<B>, Option<A>, Option<B>, A, B>.Inst.BiMap(ma, fa, fb);

        [Pure]
        public Option<B> Map(Option<A> ma, Func<A, B> f) =>
            MOption<A>.Inst.Bind<MOption<B>, Option<B>, B>(ma, a => MOption<B>.Inst.Return(f(a)));
    }
}
