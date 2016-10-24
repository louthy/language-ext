using System;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System.Collections.Generic;

namespace LanguageExt.ClassInstances
{
    public struct FLst<A, B> : 
        Functor<Lst<A>, Lst<B>, A, B>
    {
        public static readonly FLst<A, B> Inst = default(FLst<A, B>);

        public Lst<B> Map(Lst<A> ma, Func<A, B> f) =>
            ma.Map(f);
    }
}
