using System;
using LanguageExt;
using LanguageExt.TypeClasses;
using static LanguageExt.TypeClass;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Hash of any values bound by the Try monad
    /// </summary>
    public struct HashableTry<HashA, A> : Hashable<Try<A>>
        where HashA : struct, Hashable<A>
    {
        [Pure]
        public int GetHashCode(Try<A> x)
        {
            var res = x.Try();
            return res.IsFaulted 
                ? 0 
                : default(HashA).GetHashCode(res.Value);
        }
    }

    /// <summary>
    /// Hash of any values bound by the Try monad
    /// </summary>
    public struct HashableTry<A> : Hashable<Try<A>>
    {
        [Pure]
        public int GetHashCode(Try<A> x) =>
            default(HashableTry<HashableDefault<A>, A>).GetHashCode(x);
    }
}
