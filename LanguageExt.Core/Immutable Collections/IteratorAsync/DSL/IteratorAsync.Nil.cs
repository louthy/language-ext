using System;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;

namespace LanguageExt;

public abstract partial class IteratorAsync<A>
{
    /// <summary>
    /// Nil iterator case
    ///
    /// The end of the sequence.
    /// </summary>
    public sealed class Nil : IteratorAsync<A>
    {
        public static readonly IteratorAsync<A> Default = new Nil();

        /// <summary>
        /// Head element
        /// </summary>
        public override ValueTask<A> Head =>
            throw new InvalidOperationException("Nil iterator has no head");

        /// <summary>
        /// Tail of the sequence
        /// </summary>
        public override ValueTask<IteratorAsync<A>> Tail =>
            new(this);

        /// <summary>
        /// Return true if there are no elements in the sequence.
        /// </summary>
        public override ValueTask<bool> IsEmpty =>
            new(true);

        /// <summary>
        /// Clone the iterator so that we can consume it without having the head item referenced.
        /// This will stop any GC pressure.
        /// </summary>
        public override IteratorAsync<A> Clone() =>
            this;

        /// <summary>
        /// When iterating a sequence, it is possible (before evaluation of the `Tail`) to Terminate the current
        /// iterator and to take a new iterator that continues on from the current location.  The reasons for doing
        /// this are to break the linked-list chain so that there isn't a big linked-list of objects in memory that
        /// can't be garbage collected. 
        /// </summary>
        /// <returns>New iterator that starts from the current iterator position</returns>
        public override IteratorAsync<A> Split() =>
            this;

        /// <summary>
        /// Return the number of items in the sequence.
        /// </summary>
        /// <remarks>
        /// Requires all items to be evaluated, this will happen only once however.
        /// </remarks>
        [Pure]
        public override ValueTask<long> Count =>
            new(0);

        public override ValueTask DisposeAsync() =>
            ValueTask.CompletedTask;
    }
}
