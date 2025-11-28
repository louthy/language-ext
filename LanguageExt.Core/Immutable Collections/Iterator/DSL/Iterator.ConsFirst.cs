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
    internal sealed class ConsFirst : Cons
    {
        IEnumerable<A> enumerable;
        int firstAcquired;
        Iterator<A>? firstValue;

        public override string ToString() => 
            "Iterator";

        internal ConsFirst(IEnumerable<A> enumerable) =>
            this.enumerable = enumerable;

        public override A Head =>
            First.Head;

        public override Iterator<A> Tail =>
            First.Tail;

        public new void Deconstruct(out A head, out Iterator<A> tail)
        {
            head = Head;
            tail = Tail;
        }

        Iterator<A> First
        {
            get
            {
                if (firstAcquired == 2) return firstValue!;
                
                SpinWait sw = default;
                while (firstAcquired < 2)
                {
                    if (Interlocked.CompareExchange(ref firstAcquired, 1, 0) == 0)
                    {
                        try
                        {
                            var enumerator = enumerable.GetEnumerator();
                            if (enumerator.MoveNext())
                            {
                                firstValue = new ConsValueEnum(enumerator.Current, enumerator);
                            }
                            else
                            {
                                enumerator.Dispose();
                                firstValue = Nil.Default;
                            }

                            firstAcquired = 2;
                        }
                        catch (Exception)
                        {
                            firstAcquired = 0;
                            throw;
                        }
                    }
                    else
                    {
                        sw.SpinOnce();
                    }
                }
                return firstValue!;
            }
        }

        /// <summary>
        /// Return true if there are no elements in the sequence.
        /// </summary>
        public override bool IsEmpty =>
            First.IsEmpty;

        /// <summary>
        /// Clone the iterator so that we can consume it without having the head item referenced.
        /// This will stop any GC pressure.
        /// </summary>
        public override Iterator<A> Clone() =>
            new ConsFirst(enumerable);

        /// <summary>
        /// When iterating a sequence, it is possible (before evaluation of the `Tail`) to Terminate the current
        /// iterator and to take a new iterator that continues on from the current location.  The reasons for doing
        /// this are to break the linked-list chain so that there isn't a big linked-list of objects in memory that
        /// can't be garbage collected. 
        /// </summary>
        /// <returns>New iterator that starts from the current iterator position</returns>
        public override Iterator<A> Split() =>
            Clone();

        /// <summary>
        /// Return the number of items in the sequence.
        /// </summary>
        /// <remarks>
        /// Requires all items to be evaluated, this will happen only once however.
        /// </remarks>
        [Pure]
        public override long Count =>
            First.Count;

        public override void Dispose()
        {
            if (Interlocked.CompareExchange(ref firstAcquired, 1, 2) == 2)
            {
                firstValue?.Dispose();
                firstValue = null;
                firstAcquired = 0;
            }
        }
    }
}
