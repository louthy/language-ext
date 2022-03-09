using System;
using LanguageExt.ClassInstances;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    /// <summary>
    /// Existing entry updated 
    /// </summary>
    /// <param name="OldValue">Value as it was before the change</param>
    /// <param name="Value">Value</param>
    /// <typeparam name="A">Old value type</typeparam>
    /// <typeparam name="B">New value type</typeparam>
    public sealed class EntryMapped<A, B> : 
        Change<B>,
        IEquatable<EntryMapped<A, B>>
    {
        /// <summary>
        /// Original value mapped-from
        /// </summary>
        public readonly A From;
        
        /// <summary>
        /// Value mapped-to
        /// </summary>
        public readonly B To;

        internal EntryMapped(A @from, B to)
        {
            From = @from;
            To = to;
        }

        public override bool Equals(Change<B> obj) =>
            obj is EntryMapped<A, B> rhs && Equals(rhs);

        public bool Equals(EntryMapped<A, B> rhs) =>
           !ReferenceEquals(rhs, null) &&
            default(EqDefault<A>).Equals(From, rhs.From) &&
            default(EqDefault<B>).Equals(To, rhs.To);

        public override int GetHashCode() =>
            FNV32.Next(
                From?.GetHashCode() ?? FNV32.OffsetBasis,
                To?.GetHashCode() ?? FNV32.OffsetBasis);

        public override string ToString() => $"{From} -> {To}";

        public void Deconstruct(out A oldValue, out B value)
        {
            oldValue = From;
            value = To;
        }
    }
}
