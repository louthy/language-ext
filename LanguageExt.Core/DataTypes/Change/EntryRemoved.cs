using System;
using LanguageExt.ClassInstances;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    /// <summary>
    /// Existing entry removed 
    /// </summary>
    /// <typeparam name="A">Value type</typeparam>
    public sealed class EntryRemoved<A> : 
        Change<A>, 
        IEquatable<EntryRemoved<A>>
    {
        /// <summary>
        /// Value that was removed
        /// </summary>
        public readonly A OldValue;

        internal EntryRemoved(A oldValue) =>
            OldValue = oldValue;

        public override bool Equals(Change<A> obj) =>
            obj is EntryRemoved<A> rhs && Equals(rhs);

        public override int GetHashCode() => 
            OldValue?.GetHashCode() ?? FNV32.OffsetBasis;
        
        public bool Equals(EntryRemoved<A> rhs) =>
           !ReferenceEquals(rhs, null) &&
            default(EqDefault<A>).Equals(OldValue, rhs.OldValue);

        public void Deconstruct(out A oldValue)
        {
            oldValue = OldValue;
        }
        
        public override string ToString() => $"-{OldValue}";
    }
}
