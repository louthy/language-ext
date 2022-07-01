#nullable enable

using System;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using LanguageExt.Common;

namespace LanguageExt.ClassInstances
{
    public readonly struct FTry<A, B> : 
        Functor<Try<A>, Try<B>, A, B>,
        BiFunctor<Try<A>, Try<B>, Error, A, Error, B>
    {
        public static readonly FTry<A, B> Inst = default;
        
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Try<B> Map(Try<A> ma, Func<A, B> f) => 
            ma.Map(f);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Try<B> BiMap(Try<A> ma, Func<Error, Error> fa, Func<A, B> fb) => 
            ma.BiMap(fb, fa);
    }
}
