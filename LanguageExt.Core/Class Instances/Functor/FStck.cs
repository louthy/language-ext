using System;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances
{
    public struct FStck<A, B> : 
        Functor<Stck<A>, Stck<B>, A, B>
    {
        public static readonly FStck<A, B> Inst = default(FStck<A, B>);

        [Pure]
        public Stck<B> Map(Stck<A> ma, Func<A, B> f) =>
            new Stck<B>(ma.Map(f));
    }
}
