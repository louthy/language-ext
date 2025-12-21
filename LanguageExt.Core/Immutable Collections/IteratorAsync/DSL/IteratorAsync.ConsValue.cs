using System.Diagnostics.Contracts;
using System.Threading.Tasks;

namespace LanguageExt;

public abstract partial class IteratorAsync<A> 
{
    internal sealed class ConsValue : Cons
    {
        readonly A head;
        readonly IteratorAsync<A> tail;

        public ConsValue(A head, IteratorAsync<A> tail)
        {
            this.head = head;
            this.tail = tail;
        }
        
        public new void Deconstruct(out ValueTask<A> h, out ValueTask<IteratorAsync<A>> t)
        {
            h = Head;
            t = Tail;
        }

        /// <summary>
        /// Head element
        /// </summary>
        public override ValueTask<A> Head => new(head);

        /// <summary>
        /// Tail of the sequence
        /// </summary>
        public override ValueTask<IteratorAsync<A>> Tail => new(tail);

        /// <summary>
        /// Return true if there are no elements in the sequence.
        /// </summary>
        public override ValueTask<bool> IsEmpty =>
            new(false);

        /// <summary>
        /// Clone the iterator so that we can consume it without having the head item referenced.
        /// This will stop any GC pressure.
        /// </summary>
        public override IteratorAsync<A> Clone() =>
            new ConsValue(head, tail.Clone());

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
            Tail.Bind(t => t.Count.Map(c => c + 1));
        
        public override async ValueTask DisposeAsync() =>
            await (await Tail).DisposeAsync();
    }
}
