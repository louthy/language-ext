using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using LanguageExt.ClassInstances;
using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt;

public abstract partial class Iterator<A> 
{
    internal sealed class ConsValueLazy : Cons
    {
        long count;
        A head;
        Exception? error;
        Iterator<A>? tail;
        Func<Iterator<A>>? tailF;
        int tailAcquired;

        public override string ToString() => 
            "Iterator";

        public ConsValueLazy(A head, Func<Iterator<A>> tailF)
        {
            this.head = head;
            this.tailF = tailF;
            tail = null;
            count = -1;
        }
        
        public new void Deconstruct(out A h, out Iterator<A> t)
        {
            h = Head;
            t = Tail;
        }

        /// <summary>
        /// Head element
        /// </summary>
        public override A Head => head;

        /// <summary>
        /// Tail of the sequence
        /// </summary>
        public override Iterator<A> Tail => TailLazy();

        Iterator<A> TailLazy()
        {
            if (tailAcquired == 2) return tail!;
            if (tailAcquired == 3) error!.Rethrow();

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

            return tail!;
        }

        /// <summary>
        /// Return true if there are no elements in the sequence.
        /// </summary>
        public override bool IsEmpty =>
            false;

        /// <summary>
        /// Clone the iterator so that we can consume it without having the head item referenced.
        /// This will stop any GC pressure.
        /// </summary>
        public override Iterator<A> Clone() =>
            this;

        /// <summary>
        /// When iterating a sequence, it is possible (before evaluation of the `Tail`) to Terminate the current
        /// iterator and to take a new iterator that continues on from the current location.  The reasons for doing
        /// this are to break the linked-list chain so that there isn't a big linked-list of objects in memory that
        /// can't be garbage collected. 
        /// </summary>
        /// <returns>New iterator that starts from the current iterator position</returns>
        public override Iterator<A> Split()
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
        public override long Count
        {
            get
            {
                if (count == -1)
                {
                    count = 1 + Tail.Count;
                }
                return count;
            }
        }

        public override void Dispose()
        {
            if (tailAcquired == 2)
            {
                Tail.Dispose();
            }
        }
    }
}
