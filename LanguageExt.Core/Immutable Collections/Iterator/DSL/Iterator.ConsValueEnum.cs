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
    internal sealed class ConsValueEnum : Cons
    {
        Exception? exception;
        IEnumerator<A>? enumerator;
        int tailAcquired;
        Iterator<A>? tailValue;
        long count = -1;

        public override string ToString() => 
            "Iterator";

        internal ConsValueEnum(A head, IEnumerator<A> enumerator)
        {
            Head = head;
            this.enumerator = enumerator;
        }

        public new void Deconstruct(out A head, out Iterator<A> tail)
        {
            head = Head;
            tail = Tail;
        }

        /// <summary>
        /// Head element
        /// </summary>
        public override A Head { get; }

        /// <summary>
        /// Tail of the sequence
        /// </summary>
        public override Iterator<A> Tail
        {
            get
            {
                if(tailAcquired == 2) return tailValue!;
                if(tailAcquired == 3) exception!.Rethrow();

                SpinWait sw = default;
                while (tailAcquired < 2)
                {
                    if (Interlocked.CompareExchange(ref tailAcquired, 1, 0) == 0)
                    {
                        try
                        {
                            if (enumerator!.MoveNext())
                            {
                                tailValue = new ConsValueEnum(enumerator.Current, enumerator);
                            }
                            else
                            {
                                enumerator?.Dispose();
                                enumerator = null;
                                tailValue = Nil.Default;
                            }

                            tailAcquired = 2;
                        }
                        catch (Exception e)
                        {
                            exception = e;
                            tailAcquired = 3;
                            throw;
                        }
                    }
                    else
                    {
                        sw.SpinOnce();
                    }
                }

                if(tailAcquired == 3) exception!.Rethrow();
                return tailValue!;
            }
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
            if (Interlocked.CompareExchange(ref tailAcquired, 1, 0) == 0)
            {
                tailValue = Nil.Default;
                tailAcquired = 2;
                return new ConsValueEnum(Head, enumerator!);
            }
            else
            {
                return this;
            }
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
            enumerator?.Dispose();
            enumerator = null;
        }
    }
}
