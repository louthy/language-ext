using System;
using LanguageExt;
using LanguageExt.TypeClasses;
using static LanguageExt.TypeClass;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Hash of any type in the TryOption type-class
    /// </summary>
    public struct HashableTryOption<HashA, A> : Hashable<TryOption<A>>
        where HashA : struct, Hashable<A>
    {
        [Pure]
        public int GetHashCode(TryOption<A> x)
        {
            var res = x.Try();
            return res.IsFaulted || res.Value.IsNone ? 0 : default(HashA).GetHashCode(res.Value.Value);
        }
    }

    /// <summary>
    /// Hash of any type in the TryOption type-class
    /// </summary>
    public struct HashableTryOption<A> : Hashable<TryOption<A>>
    {
        [Pure]
        public int GetHashCode(TryOption<A> x) =>
            default(HashableTryOption<HashableDefault<A>, A>).GetHashCode(x);
    }
}
