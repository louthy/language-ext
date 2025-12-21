using System;
using System.Diagnostics.Contracts;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Common;

namespace LanguageExt;

public abstract partial class IteratorAsync<A> 
{
    internal sealed class ConsValueLazy : Cons
    {
        ValueTask<A> head;
        Exception? error;
        IteratorAsync<A>? tail;
        Func<IteratorAsync<A>>? tailF;
        int tailAcquired;

        public ConsValueLazy(ValueTask<A> head, Func<IteratorAsync<A>> tailF)
        {
            this.head = head;
            this.tailF = tailF;
            tail = null;
        }
        
        public new void Deconstruct(out ValueTask<A> h, out ValueTask<IteratorAsync<A>> t)
        {
            h = Head;
            t = Tail;
        }

        /// <summary>
        /// Head element
        /// </summary>
        public override ValueTask<A> Head => head;

        /// <summary>
        /// Tail of the sequence
        /// </summary>
        public override ValueTask<IteratorAsync<A>> Tail
        {
            get
            {
                if (tailAcquired == 2) return new(tail!);
                if (tailAcquired == 3) error!.Rethrow();
                return TailLazy();
            }
        }

        ValueTask<IteratorAsync<A>> TailLazy()
        {
            SpinWait sw = default;
            while (tailAcquired < 2)
            {
                if (Interlocked.CompareExchange(ref tailAcquired, 1, 0) == 0)
                {
                    try
                    {
                        tail = tailF!();
                        tailF = null;
                        tailAcquired = 2;
                    }
                    catch(Exception e)
                    {
                        error = e;
                        tailF = null;
                        tailAcquired = 3;
                        throw;
                    }
                }
                else
                {
                    sw.SpinOnce();
                }
            }

            return new(tail!);
        }

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
            this;

        /// <summary>
        /// When iterating a sequence, it is possible (before evaluation of the `Tail`) to Terminate the current
        /// iterator and to take a new iterator that continues on from the current location.  The reasons for doing
        /// this are to break the linked-list chain so that there isn't a big linked-list of objects in memory that
        /// can't be garbage collected. 
        /// </summary>
        /// <returns>New iterator that starts from the current iterator position</returns>
        public override IteratorAsync<A> Split()
        {
            var h = head;
            var t = tailF;
            return tailAcquired == 0
                       ? new ConsValueLazy(h, t!)
                       : this;
        }

        /// <summary>
        /// Return the number of items in the sequence.
        /// </summary>
        /// <remarks>
        /// Requires all items to be evaluated, this will happen only once however.
        /// </remarks>
        [Pure]
        public override ValueTask<long> Count =>
            Tail.Bind(static t => t.Count.Map(static c => c + 1));

        public override async ValueTask DisposeAsync()
        {
            if (tailAcquired == 2)
            {
                await (await Tail).DisposeAsync();
            }
        }
    }    
}
