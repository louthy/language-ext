using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Threading;
using LanguageExt.ClassInstances;

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
/// previous `Iterator` values as you walk the sequence. 
/// </remarks>
/// <typeparam name="A">Item value type</typeparam>
public abstract class Iterator<A> : 
    IEquatable<Iterator<A>>
{
    /// <summary>
    /// Head element
    /// </summary>
    [Pure]
    public abstract A Head { get; }

    /// <summary>
    /// Tail of the sequence
    /// </summary>
    [Pure]
    public abstract Iterator<A> Tail { get; }
    
    /// <summary>
    /// Return true if there are no elements in the sequence.
    /// </summary>
    [Pure]
    public abstract bool IsEmpty  { get; }
    
    /// <summary>
    /// Nil iterator case
    ///
    /// The end of the sequence.
    /// </summary>
    public sealed class Nil : Iterator<A>
    {
        public static readonly Iterator<A> Default = new Nil();
        
        /// <summary>
        /// Head element
        /// </summary>
        public override A Head =>
            throw new InvalidOperationException("Nil iterator has no head");

        /// <summary>
        /// Tail of the sequence
        /// </summary>
        public override Iterator<A> Tail =>
            this;

        /// <summary>
        /// Return true if there are no elements in the sequence.
        /// </summary>
        public override bool IsEmpty =>
            true;
    }

    /// <summary>
    /// Cons iterator case.
    ///
    /// Contains a head value and a tail that represents the rest of the sequence.
    /// </summary>
    public abstract class Cons : Iterator<A>
    {
        public void Deconstruct(out A head, out Iterator<A> tail)
        {
            head = Head;
            tail = Tail;
        }
    }
    
    sealed class ConsValue : Cons
    {
        IEnumerator<A>? enumerator;
        int tailAcquired;
        Iterator<A>? tailValue;

        internal ConsValue(A head, IEnumerator<A> enumerator)
        {
            Head = head;
            this.enumerator = enumerator;
        }

        public void Deconstruct(out A head, out Iterator<A> tail)
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
                if (tailAcquired == 2) return tailValue!;

                SpinWait sw = default;
                while (tailAcquired < 2)
                {
                    if (Interlocked.CompareExchange(ref tailAcquired, 1, 0) == 0)
                    {
                        if (enumerator!.MoveNext())
                        {
                            tailValue = new ConsValue(enumerator.Current, enumerator);
                        }
                        else
                        {
                            enumerator.Dispose();
                            tailValue = Nil.Default;
                        }

                        enumerator = null;
                        tailAcquired = 2;
                    }
                    else
                    {
                        sw.SpinOnce();
                    }
                }

                return tailValue!;
            }
        }

        /// <summary>
        /// Return true if there are no elements in the sequence.
        /// </summary>
        public override bool IsEmpty =>
            false;
    }
    
    internal sealed class ConsFirst : Cons
    {
        IEnumerator<A>? enumerator;
        int firstAcquired;
        Iterator<A>? firstValue;

        internal ConsFirst(IEnumerator<A> enumerator) =>
            this.enumerator = enumerator;

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
                        if (enumerator!.MoveNext())
                        {
                            firstValue = new ConsValue(enumerator.Current, enumerator);
                        }
                        else
                        {
                            enumerator.Dispose();
                            firstValue = Nil.Default;
                        }
                        enumerator = null;
                        firstAcquired = 2;
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
    }

    /// <summary>
    /// Equality test
    /// </summary>
    /// <param name="obj">Other iterator to compare against</param>
    /// <returns>True if equal</returns>
    [Pure]
    public override bool Equals(object? obj) =>
        obj is Iterator<A> other && Equals(other);

    /// <summary>
    /// Equality test
    /// </summary>
    /// <param name="other">Other iterator to compare against</param>
    /// <returns>True if equal</returns>
    [Pure]
    public bool Equals(Iterator<A>? rhs)
    {
        if (rhs is null) return false;
        var iterA = this;
        var iterB = this;
        while (true)
        {
            if(iterA.IsEmpty && iterB.IsEmpty) return true; 
            if(iterA.IsEmpty || iterB.IsEmpty) return false;
            if(!EqDefault<A>.Equals(iterA.Head, iterB.Head)) return false;
            iterA = iterA.Tail;
            iterB = iterB.Tail;
        }
    }

    [Pure]
    public override int GetHashCode()
    {
        if(IsEmpty) return 0;
        var iter = this;
        var hash = OffsetBasis;
        while(!iter.IsEmpty)
        {
            var itemHash = iter.Head?.GetHashCode() ?? 0;
            unchecked
            {
                hash = (hash ^ itemHash) * Prime;
            }
            iter = iter.Tail;
        }
        return hash;
    }

    [Pure]
    public override string ToString() =>
        CollectionFormat.ToShortArrayString(this.AsEnumerable());
    /// <summary>
    /// Format the collection as `a, b, c, ...`
    /// </summary>
    [Pure]
    public string ToFullString(string separator = ", ") =>
        CollectionFormat.ToFullString(this.AsEnumerable(), separator);

    /// <summary>
    /// Format the collection as `[a, b, c, ...]`
    /// </summary>
    [Pure]
    public string ToFullArrayString(string separator = ", ") =>
        CollectionFormat.ToFullArrayString(this.AsEnumerable(), separator);

    const int OffsetBasis = -2128831035;
    const int Prime = 16777619;
}
