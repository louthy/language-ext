using System.Diagnostics.Contracts;

namespace LanguageExt;

public abstract partial record Stck<A>
{
    /// <summary>
    /// Terminating/empty stack
    /// </summary>
    public sealed record Nil : Stck<A>
    {
        /// <summary>
        /// Reference version for use in pattern-matching
        /// </summary>
        [Pure]
        public override object? Case => null;

        /// <summary>
        /// Is the stack empty?
        /// </summary>
        [Pure]
        public override bool IsEmpty =>
            true;

        /// <summary>
        /// Number of items in the stack
        /// </summary>
        public override long Count => 
            0;

        /// <summary>
        /// Number of items in the stack truncated to an `int`
        /// </summary>
        public override int Length => 
            0;
        
        /// <summary>
        /// Return the item on the top of the stack without affecting the stack itself.
        /// </summary>
        /// <returns>Top item value or None if the stack is empty.</returns>
        [Pure]
        public override Option<A> Peek() =>
            default;

        /// <summary>
        /// Pop an item off the top of the stack. 
        /// </summary>
        /// <remarks>
        /// If there's nothing on the stack this does nothing.  Use pattern-matching, `IsEmpty`, or `Peek` to know
        /// whether `Pop` will have an effect.
        /// </remarks>
        /// <returns>Stack with the top item popped</returns>
        [Pure]
        public override Stck<A> Pop() =>
            this;

        [Pure]
        public override string ToString()=>
            "[]";
    }
}
