using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Common;

namespace LanguageExt;

/// <summary>
/// Wrapper for `IEnumerator` that makes it work like an immutable sequence.
/// 
/// It is thread-safe and impossible for any item in the sequence to be enumerated more than once.
/// </summary>
/// <remarks>
/// `IEnumerator` from the .NET BCL has several problems: 
///
///    * It's very imperative
///    * It's not thread-safe, two enumerators can't be shared
/// 
/// The lack of support for sharing of enumerators means that it's problematic using it internally
/// in types like `StreamT`, or anything that needs to keep an `IEnumerator` alive for any period
/// of time.
///
/// NOTE: There is a per-item allocation to hold the state of the iterator.  These are discarded as
/// you enumerate the sequence.  However, technically it is possible to hold the initial `Iterator`
/// value and subsequently gain a cached sequence of every item encountered in the enumerator.
///
/// That may well be valuable for circumstances where re-evaluation would be expensive.  However,
/// for infinite-streams this would be extremely problematic.  So, make sure you discard any
/// previous `IteratorAsync` values as you walk the sequence. 
/// </remarks>
/// <typeparam name="A">Item value type</typeparam>
public abstract partial class IteratorAsync<A> 
{
    internal sealed class ConsValueEnum : Cons
    {
        Exception? exception;
        IAsyncEnumerator<A>? enumerator;
        int tailAcquired;
        IteratorAsync<A>? tailValue;

        internal ConsValueEnum(ValueTask<A> head, IAsyncEnumerator<A> enumerator)
        {
            Head = head;
            this.enumerator = enumerator;
        }

        public new void Deconstruct(out ValueTask<A> head, out ValueTask<IteratorAsync<A>> tail)
        {
            head = Head;
            tail = Tail;
        }

        /// <summary>
        /// Head element
        /// </summary>
        public override ValueTask<A> Head { get; }

        /// <summary>
        /// Tail of the sequence
        /// </summary>
        public override ValueTask<IteratorAsync<A>> Tail
        {
            get
            {
                if(tailAcquired == 2) return new(tailValue!);
                if(tailAcquired == 3) exception!.Rethrow();
                return TailAsync();
            }
        }
        
        /// <summary>
        /// Tail of the sequence
        /// </summary>
        async ValueTask<IteratorAsync<A>> TailAsync()
        {
            SpinWait sw = default;
            while (tailAcquired < 2)
            {
                if (Interlocked.CompareExchange(ref tailAcquired, 1, 0) == 0)
                {
                    try
                    {
                        if (await enumerator!.MoveNextAsync())
                        {
                            tailValue = new ConsValueEnum(new(enumerator.Current), enumerator);
                        }
                        else
                        {
                            var e = enumerator;
                            if(e != null) await e.DisposeAsync();
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
        public override ValueTask<long> Count =>
            Tail.Bind(t => t.Count.Map(c => c + 1));
        
        public override async ValueTask DisposeAsync()
        {
            var e = enumerator;
            if(e != null) await e.DisposeAsync();
            enumerator = null;
        }
    }
}
