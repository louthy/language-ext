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
    internal sealed class ConsValue : Cons
    {
        A head;
        Iterator<A> tail;

        public override string ToString() => 
            "Iterator";

        public ConsValue(A head, Iterator<A> tail)
        {
            this.head = head;
            this.tail = tail;
            Count = -1;
        }
        
        public new void Deconstruct(out A head, out Iterator<A> tail)
        {
            head = Head;
            tail = Tail;
        }

        /// <summary>
        /// Head element
        /// </summary>
        public override A Head => head;

        /// <summary>
        /// Tail of the sequence
        /// </summary>
        public override Iterator<A> Tail => tail;

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
            new ConsValue(Head, Tail.Clone());

        /// <summary>
        /// When iterating a sequence, it is possible (before evaluation of the `Tail`) to Terminate the current
        /// iterator and to take a new iterator that continues on from the current location.  The reasons for doing
        /// this are to break the linked-list chain so that there isn't a big linked-list of objects in memory that
        /// can't be garbage collected. 
        /// </summary>
        /// <returns>New iterator that starts from the current iterator position</returns>
        public override Iterator<A> Split() =>
            this;

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
                if (field == -1)
                {
                    field = 1 + Tail.Count;
                }
                return field;
            }
        }
        
        public override void Dispose() =>
            Tail.Dispose();
    }
}
