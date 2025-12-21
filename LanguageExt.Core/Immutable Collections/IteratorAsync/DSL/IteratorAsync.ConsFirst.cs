using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Threading;
using System.Threading.Tasks;

namespace LanguageExt;

public abstract partial class IteratorAsync<A> 
{
    internal sealed class ConsFirst : Cons
    {
        IAsyncEnumerable<A> enumerable;
        int firstAcquired;
        IteratorAsync<A>? firstValue;

        internal ConsFirst(IAsyncEnumerable<A> enumerable) =>
            this.enumerable = enumerable;

        public override ValueTask<A> Head =>
            First().Bind(f => f.Head);

        public override ValueTask<IteratorAsync<A>> Tail =>
            First().Bind(f => f.Tail);

        public new void Deconstruct(out ValueTask<A> head, out ValueTask<IteratorAsync<A>> tail)
        {
            head = Head;
            tail = Tail;
        }

        async ValueTask<IteratorAsync<A>> First()
        {
            if (firstAcquired == 2) return firstValue!;
            
            SpinWait sw = default;
            while (firstAcquired < 2)
            {
                if (Interlocked.CompareExchange(ref firstAcquired, 1, 0) == 0)
                {
                    try
                    {
                        var enumerator = enumerable.GetAsyncEnumerator();
                        if (await enumerator.MoveNextAsync())
                        {
                            firstValue = new ConsValueEnum(new(enumerator.Current), enumerator);
                        }
                        else
                        {
                            await enumerator.DisposeAsync();
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

        /// <summary>
        /// Return true if there are no elements in the sequence.
        /// </summary>
        public override ValueTask<bool> IsEmpty =>
            First().Bind(f => f.IsEmpty);

        /// <summary>
        /// Clone the iterator so that we can consume it without having the head item referenced.
        /// This will stop any GC pressure.
        /// </summary>
        public override IteratorAsync<A> Clone() =>
            new ConsFirst(enumerable);

        /// <summary>
        /// When iterating a sequence, it is possible (before evaluation of the `Tail`) to Terminate the current
        /// iterator and to take a new iterator that continues on from the current location.  The reasons for doing
        /// this are to break the linked-list chain so that there isn't a big linked-list of objects in memory that
        /// can't be garbage collected. 
        /// </summary>
        /// <returns>New iterator that starts from the current iterator position</returns>
        public override IteratorAsync<A> Split() =>
            Clone();

        /// <summary>
        /// Return the number of items in the sequence.
        /// </summary>
        /// <remarks>
        /// Requires all items to be evaluated, this will happen only once however.
        /// </remarks>
        [Pure]
        public override ValueTask<long> Count =>
            First().Bind(f => f.Count);

        public override async ValueTask DisposeAsync()
        {
            if (Interlocked.CompareExchange(ref firstAcquired, 1, 2) == 2)
            {
                var fv = firstValue;
                if(fv != null) await fv.DisposeAsync();
                firstValue = null;
                firstAcquired = 0;
            }
        }
    }
}
