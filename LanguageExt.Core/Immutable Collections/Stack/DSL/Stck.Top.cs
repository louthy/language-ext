using System.Diagnostics.Contracts;

namespace LanguageExt;

public abstract partial record Stck<A>
{
    /// <summary>
    /// Value on top of the stack that has a reference to the rest of the stack
    /// </summary>
    /// <param name="Value">Value on the top of the stack</param>
    /// <param name="Rest">The rest of the stack</param>
    public sealed record Top(A Value, Stck<A> Rest) 
        : Stck<A>
    {
        /// <summary>
        /// Rest of the stack underneath the top item 
        /// </summary>
        public Stck<A> Rest { get; internal set; } = Rest;

        /// <summary>
        /// Reference version for use in pattern-matching
        /// </summary>
        [Pure]
        public override object? Case => Value;

        /// <summary>
        /// Is the stack empty?
        /// </summary>
        [Pure]
        public override bool IsEmpty =>
            false;

        /// <summary>
        /// Number of items in the stack
        /// </summary>
        public override long Count { get; } = 
            Rest.Count + 1;

        /// <summary>
        /// Number of items in the stack truncated to an `int`
        /// </summary>
        public override int Length => 
            (int)Count;

        /// <summary>
        /// Return the item on the top of the stack without affecting the stack itself.
        /// </summary>
        /// <returns>Top item value or None if the stack is empty.</returns>
        [Pure]
        public override Option<A> Peek() =>
            Value;

        /// <summary>
        /// Pop an item off the top of the stack. 
        /// </summary>
        /// <remarks>
        /// If there's nothing on the stack, this does nothing.  Use pattern-matching, `IsEmpty`, or `Peek` to know
        /// whether `Pop` will have an effect.
        /// </remarks>
        /// <returns>Stack with the top item popped</returns>
        [Pure]
        public override Stck<A> Pop() =>
            Rest;

        [Pure]
        public override string ToString()=>
            CollectionFormat.ToShortArrayString(this);
    }
}
