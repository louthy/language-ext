using System;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using LanguageExt.Common;

namespace LanguageExt.ClassInstances
{
    public struct FFin<A, B> : 
        Functor<Fin<A>, Fin<B>, A, B>,
        BiFunctor<Fin<A>, Fin<B>, A, Error, B>,
        BiFunctor<Fin<A>, Fin<B>, A, Error, B, Error>
    {
        public static readonly FFin<A, B> Inst = default(FFin<A, B>);

        [Pure]
        public Fin<B> BiMap(Fin<A> ma, Func<A, B> fa, Func<Error, B> fb) =>
            ma.BiMap(fa, fb);

        [Pure]
        public Fin<B> BiMap(Fin<A> ma, Func<A, B> fa, Func<Error, Error> fb) =>
            ma.BiMap(fa, fb);

        [Pure]
        public Fin<B> Map(Fin<A> ma, Func<A, B> f) =>
            ma.Map(f);
    }
}
