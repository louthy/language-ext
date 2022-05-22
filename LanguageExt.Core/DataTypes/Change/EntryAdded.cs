using System;
using LanguageExt.ClassInstances;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    /// <summary>
    /// Entry added to a collection
    /// </summary>
    /// <typeparam name="A">Value type</typeparam>
    public sealed class EntryAdded<A> :
        Change<A>, 
        IEquatable<EntryAdded<A>>
    {
        /// <summary>
        /// Value that has been added
        /// </summary>
        public readonly A Value;

        internal EntryAdded(A value) =>
            Value = value;

        public override bool Equals(Change<A> obj) =>
            obj is EntryAdded<A> rhs && Equals(rhs);

        public bool Equals(EntryAdded<A> rhs) =>
           !ReferenceEquals(rhs, null) &&
            default(EqDefault<A>).Equals(Value, rhs.Value);

        public override int GetHashCode() =>
            Value?.GetHashCode() ?? FNV32.OffsetBasis;

        public void Deconstruct(out A value)
        {
            value = Value;
        }

        public override string ToString() => $"+{Value}";
    }
}
