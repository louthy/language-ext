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
    IEnumerable<A>,
    IEquatable<Iterator<A>>,
    IDisposable,
    K<Iterator, A>
{
    /// <summary>
    /// Empty iterator
    /// </summary>
    public static readonly Iterator<A> Empty = new Nil();
    
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
    /// Return the number of items in the sequence.
    /// </summary>
    /// <remarks>
    /// Requires all items to be evaluated, this will happen only once however.
    /// </remarks>
    [Pure]
    public abstract long Count  { get; }

    /// <summary>
    /// Clone the iterator so that we can consume it without having the head item referenced.
    /// This will stop any GC pressure when processing large or infinite sequences.
    /// </summary>
    [Pure]
    public abstract Iterator<A> Clone();

    /// <summary>
    /// When iterating a sequence, it is possible (before evaluation of the `Tail`) to Terminate the current
    /// iterator and to take a new iterator that continues on from the current location.  The reasons for doing
    /// this are to break the linked-list chain so that there isn't a big linked-list of objects in memory that
    /// can't be garbage collected. 
    /// </summary>
    /// <remarks>
    /// Any other iterator references that came before this one will terminate at this point.  Splitting the
    /// previous and subsequent iterators here. 
    /// </remarks>
    /// <returns>New iterator that starts from the current iterator position.</returns>
    public abstract Iterator<A> Split();

    /// <summary>
    /// Create an `IEnumerable` from an `Iterator`
    /// </summary>
    [Pure]
    public IEnumerable<A> AsEnumerable()
    {
        for (var ma = Clone(); !ma.IsEmpty; ma = ma.Tail)
        {
            yield return  ma.Head;
        }
    }

    /// <summary>
    /// Create an `Iterable` from an `Iterator`
    /// </summary>
    [Pure]
    public Iterable<A> AsIterable() =>
        Iterable.createRange(AsEnumerable());

    /// <summary>
    /// Functor map
    /// </summary>
    [Pure]
    public Iterator<B> Select<B>(Func<A, B> f) =>
        Map(f);

    /// <summary>
    /// Functor map
    /// </summary>
    [Pure]
    public Iterator<B> Map<B>(Func<A, B> f)
    {
        return Go(this, f).GetIterator();
        static IEnumerable<B> Go(Iterator<A> ma, Func<A, B> f)
        {
            for (var a = ma.Clone(); !a.IsEmpty; a = a.Tail)
            {
                yield return f(a.Head);
            }
        }
    }

    /// <summary>
    /// Monad bind
    /// </summary>
    [Pure]
    public Iterator<B> Bind<B>(Func<A, Iterator<B>> f)
    {
        return Go(this, f).GetIterator();
        static IEnumerable<B> Go(Iterator<A> ma, Func<A, Iterator<B>> f)
        {
            for (var a = ma.Clone(); !a.IsEmpty; a = a.Tail)
            {
                for (var b = f(a.Head); !b.IsEmpty; b = b.Tail)
                {
                    yield return b.Head;
                }
            }
        }
    }

    /// <summary>
    /// Monad bind
    /// </summary>
    [Pure]
    public Iterator<C> SelectMany<B, C>(Func<A, Iterator<B>> bind, Func<A, B, C> project)
    {
        return Go(this, bind, project).GetIterator();
        static IEnumerable<C> Go(Iterator<A> ma, Func<A, Iterator<B>> bind, Func<A, B, C> project)
        {
            for (var a = ma.Clone(); !a.IsEmpty; a = a.Tail)
            {
                for (var b = bind(a.Head); !b.IsEmpty; b = b.Tail)
                {
                    yield return project(a.Head, b.Head);
                }
            }
        }
    }

    /// <summary>
    /// Applicative apply
    /// </summary>
    [Pure]
    public Iterator<B> Apply<B>(Iterator<Func<A, B>> ff, Iterator<A> fa)
    {
        return Go(ff, fa).GetIterator();
        static IEnumerable<B> Go(Iterator<Func<A, B>> ff, Iterator<A> fa)
        {
            for (var f = ff.Clone(); !f.IsEmpty; f = f.Tail)
            {
                for (var a = fa.Clone(); !a.IsEmpty; a = a.Tail)
                {
                    yield return f.Head(a.Head);
                }
            }
        }
    }
    
    /// <summary>
    /// Concatenate two iterators
    /// </summary>
    [Pure]
    public Iterator<A> Concat(Iterator<A> other)
    {
        return Go(this, other).GetIterator();
        static IEnumerable<A> Go(Iterator<A> ma, Iterator<A> mb)
        {
            for (var a = ma.Clone(); !a.IsEmpty; a = a.Tail)
            {
                yield return ma.Head;
            }
            for (var b = mb.Clone(); !b.IsEmpty; b = b.Tail)
            {
                yield return mb.Head;
            }
        }
    }

    /// <summary>
    /// Fold the sequence while there are more items remaining
    /// </summary>
    [Pure]
    public S Fold<S>(
        S state,
        Func<A, Func<S, S>> f)
    {
        for(var xs = Clone(); !xs.IsEmpty; xs = xs.Tail)
        {
            state = f(xs.Head)(state);
        }
        return state;
    }

    /// <summary>
    /// Fold the sequence while there are more items remaining
    /// </summary>
    [Pure]
    public S Fold<S>(        
        S state,
        Func<S, A, S> f)
    {
        for(var xs = Clone(); !xs.IsEmpty; xs = xs.Tail)
        {
            state = f(state, xs.Head);
        }
        return state;
    }

    /// <summary>
    /// Fold the sequence in reverse while there are more items remaining
    /// </summary>
    [Pure]
    public S FoldBack<S>(
        S state,
        Func<A, Func<S, S>> f)
    {
        foreach(var x in Clone().AsEnumerable().Reverse())
        {
            state = f(x)(state);
        }
        return state;
    }

    /// <summary>
    /// Fold the sequence in reverse while there are more items remaining
    /// </summary>
    [Pure]
    public S FoldBack<S>(        
        S state,
        Func<S, A, S> f)
    {
        foreach(var x in Clone().AsEnumerable().Reverse())
        {
            state = f(state, x);
        }
        return state;
    }

    /// <summary>
    /// Fold the sequence while the predicate returns `true` and there are more items remaining
    /// </summary>
    [Pure]
    public S FoldWhile<S>(
        S state,
        Func<A, Func<S, S>> f,
        Func<(S State, A Value), bool> predicate)
    {
        for(var xs = Clone(); !xs.IsEmpty; xs = xs.Tail)
        {
            if (!predicate((state, xs.Head))) return state;
            state = f(xs.Head)(state);
        }
        return state;
    }

    /// <summary>
    /// Fold the sequence while the predicate returns `true` and there are more items remaining
    /// </summary>
    [Pure]
    public S FoldWhile<S>(
        S state,
        Func<S, A, S> f,
        Func<(S State, A Value), bool> predicate)
    {
        for(var xs = Clone(); !xs.IsEmpty; xs = xs.Tail)
        {
            if (!predicate((state, xs.Head))) return state;
            state = f(state, xs.Head);
        }
        return state;
    }
    
    /// <summary>
    /// Fold the sequence in reverse while the predicate returns `true` and there are more items remaining
    /// </summary>
    [Pure]
    public S FoldBackWhile<S>(
        S state,
        Func<S, Func<A, S>> f, 
        Func<(S State, A Value), bool> predicate) 
    {
        foreach(var x in Clone().AsEnumerable().Reverse())
        {
            if (!predicate((state, x))) return state;
            state = f(state)(x);
        }
        return state;
    }
    
    /// <summary>
    /// Fold the sequence in reverse while the predicate returns `true` and there are more items remaining
    /// </summary>
    [Pure]
    public S FoldBackWhile<S>(
        S state,
        Func<S, A, S> f, 
        Func<(S State, A Value), bool> predicate)
    {
        foreach(var x in Clone().AsEnumerable().Reverse())
        {
            if (!predicate((state, x))) return state;
            state = f(state, x);
        }
        return state;
    }
    
    /// <summary>
    /// Fold the sequence until the predicate returns `true` or the sequence ends
    /// </summary>
    [Pure]
    public S FoldUntil<S>(
        S state,
        Func<A, Func<S, S>> f,
        Func<(S State, A Value), bool> predicate)
    {
        for(var xs = Clone(); !xs.IsEmpty; xs = xs.Tail)
        {
            state = f(xs.Head)(state);
            if (predicate((state, xs.Head))) return state;
        }
        return state;
    }

    /// <summary>
    /// Fold the sequence until the predicate returns `true` or the sequence ends
    /// </summary>
    [Pure]
    public S FoldUntil<S>(
        S state,
        Func<S, A, S> f,
        Func<(S State, A Value), bool> predicate)
    {
        for(var xs = Clone(); !xs.IsEmpty; xs = xs.Tail)
        {
            state = f(state, xs.Head);
            if (predicate((state, xs.Head))) return state;
        }
        return state;
    }
    
    /// <summary>
    /// Fold the sequence in reverse until the predicate returns `true` or the sequence ends
    /// </summary>
    [Pure]
    public S FoldBackUntil<S>(
        S state,
        Func<S, Func<A, S>> f, 
        Func<(S State, A Value), bool> predicate) 
    {
        foreach(var x in Clone().AsEnumerable().Reverse())
        {
            state = f(state)(x);
            if (predicate((state, x))) return state;
        }
        return state;
    }
    
    /// <summary>
    /// Fold the sequence in reverse until the predicate returns `true` or the sequence ends
    /// </summary>
    [Pure]
    public S FoldBackUntil<S>(
        S state,
        Func<S, A, S> f, 
        Func<(S State, A Value), bool> predicate)
    {
        foreach(var x in Clone().AsEnumerable().Reverse())
        {
            state = f(state, x);
            if (predicate((state, x))) return state;
        }
        return state;
    }

    /// <summary>
    /// Interleave two iterator sequences together
    /// </summary>
    /// <remarks>
    /// Whilst there are items in both sequences, each is yielded after the other.  Once one sequence
    /// runs out of items, the remaining items of the other sequence is yielded alone.
    /// </remarks>
    [Pure]
    public Iterator<A> Merge(Iterator<A> other)
    {
        return Go(this, other).GetIterator();        
        static IEnumerable<A> Go(Iterator<A> ma, Iterator<A> mb)
        {
            var a = ma.Clone();
            var b = mb.Clone();

            while (!a.IsEmpty && !b.IsEmpty)
            {
                yield return a.Head;
                yield return b.Head;
                a = a.Tail;
                b = b.Tail;
            }

            if (a.IsEmpty)
            {
                while (!b.IsEmpty)
                {
                    yield return b.Head;
                    b = b.Tail;
                }
            }
            else
            {
                while (!a.IsEmpty)
                {
                    yield return a.Head;
                    a = a.Tail;
                }
            }
        }
    }
    
    /// <summary>
    /// Zips the items of two sequences together
    /// </summary>
    /// <remarks>
    /// The output sequence will be as long as the shortest input sequence.
    /// </remarks>
    [Pure]
    public Iterator<(A First , A Second)> Zip(Iterator<A> other)
    {
        return Go(this, other).GetIterator();        
        static IEnumerable<(A First , A Second)> Go(Iterator<A> ma, Iterator<A> mb)
        {
            var a = ma.Clone();
            var b = mb.Clone();

            while (!a.IsEmpty && !b.IsEmpty)
            {
                yield return (a.Head, b.Head);
                a = a.Tail;
                b = b.Tail;
            }
        }
    }

    /// <summary>
    /// Combine two sequences
    /// </summary>
    public static Iterator<A> operator +(Iterator<A> ma, Iterator<A> mb) =>
        ma.Concat(mb);

    /// <summary>
    /// Dispose
    /// </summary>
    public abstract void Dispose();

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
        public override Iterator<A> Split() =>
            this;

        /// <summary>
        /// Return the number of items in the sequence.
        /// </summary>
        /// <remarks>
        /// Requires all items to be evaluated, this will happen only once however.
        /// </remarks>
        [Pure]
        public override long Count =>
            0;

        public override void Dispose()
        {
        }
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
    
    internal sealed class ConsValue : Cons
    {
        long count;
        A head;
        Iterator<A> tail;

        public ConsValue(A head, Iterator<A> tail)
        {
            this.head = head;
            this.tail = tail;
            count = -1;
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
        public override Iterator<A> Split()
        {
            throw new InvalidOperationException("Can't split an Iterator when the the Tail has already been consumed");
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
        
        public override void Dispose() =>
            Tail.Dispose();
    }

    internal sealed class ConsValueEnum : Cons
    {
        Exception? exception;
        IEnumerator<A>? enumerator;
        int tailAcquired;
        Iterator<A>? tailValue;
        long count = -1;

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
                throw new InvalidOperationException("Can't split an Iterator when the the Tail has already been consumed");
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

    internal sealed class ConsFirst : Cons
    {
        IEnumerable<A> enumerable;
        int firstAcquired;
        Iterator<A>? firstValue;

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
        var iterA = Clone();
        var iterB = rhs.Clone();
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
        var iter = Clone();
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

    /// <summary>
    /// Get enumerator
    /// </summary>
    /// <returns></returns>
    [Pure]
    public IEnumerator<A> GetEnumerator() => 
        AsEnumerable().GetEnumerator();

    [Pure]
    IEnumerator IEnumerable.GetEnumerator() => 
        GetEnumerator();

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
