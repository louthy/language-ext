using System;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances
{
    public struct FQue<A, B> : 
        Functor<Que<A>, Que<B>, A, B>
    {
        public static readonly FQue<A, B> Inst = default(FQue<A, B>);

        [Pure]
        public Que<B> Map(Que<A> ma, Func<A, B> f) =>
            new Que<B>(ma.Map(f));
    }
}
