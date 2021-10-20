using System;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances
{
    public struct FTry<A, B> : 
        Functor<Try<A>, Try<B>, A, B>,
        BiFunctor<Try<A>, Try<B>, A, Unit, B>
    {
        public static readonly FTry<A, B> Inst = default(FTry<A, B>);

        [Pure]
        public Try<B> BiMap(Try<A> ma, Func<A, B> fa, Func<Unit, B> fb) =>
            FOptional<MTry<A>, MTry<B>, Try<A>, Try<B>, A, B>.Inst.BiMap(ma, fa, fb);

        [Pure]
        public Try<B> Map(Try<A> ma, Func<A, B> f) =>
            FOptional<MTry<A>, MTry<B>, Try<A>, Try<B>, A, B>.Inst.Map(ma, f);
    }
}
