using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
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
/// previous `IteratorAsync` values as you walk the sequence. 
/// </remarks>
/// <typeparam name="A">Item value type</typeparam>
public abstract class IteratorAsync<A> : 
    IAsyncEnumerable<A>,
    IAsyncDisposable,
    K<IteratorAsync, A>
{
    /// <summary>
    /// Empty iterator
    /// </summary>
    public static readonly IteratorAsync<A> Empty = new Nil();
    
    /// <summary>
    /// Head element
    /// </summary>
    [Pure]
    public abstract ValueTask<A> Head { get; }

    /// <summary>
    /// Tail of the sequence
    /// </summary>
    [Pure]
    public abstract ValueTask<IteratorAsync<A>> Tail { get; }
    
    /// <summary>
    /// Return true if there are no elements in the sequence.
    /// </summary>
    [Pure]
    public abstract ValueTask<bool> IsEmpty  { get; }
    
    /// <summary>
    /// Return the number of items in the sequence.
    /// </summary>
    /// <remarks>
    /// Requires all items to be evaluated, this will happen only once however.
    /// </remarks>
    [Pure]
    public abstract ValueTask<long> Count  { get; }

    /// <summary>
    /// Clone the iterator so that we can consume it without having the head item referenced.
    /// This will stop any GC pressure when processing large or infinite sequences.
    /// </summary>
    [Pure]
    public abstract IteratorAsync<A> Clone();

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
    public abstract IteratorAsync<A> Split();

    /// <summary>
    /// Create an `IEnumerable` from an `Iterator`
    /// </summary>
    [Pure]
    public async IAsyncEnumerable<A> AsEnumerable([EnumeratorCancellation] CancellationToken token)
    {
        for (var ma = Clone(); !await ma.IsEmpty; ma = await ma.Tail)
        {
            if (token.IsCancellationRequested) throw new TaskCanceledException();
            yield return await ma.Head;
        }
    }

    /// <summary>
    /// Functor map
    /// </summary>
    [Pure]
    public IteratorAsync<B> Select<B>(Func<A, B> f) =>
        Map(f);

    /// <summary>
    /// Functor map
    /// </summary>
    [Pure]
    public IteratorAsync<B> Map<B>(Func<A, B> f)
    {
        return Go(this, f).GetIteratorAsync();
        static async IAsyncEnumerable<B> Go(IteratorAsync<A> ma, Func<A, B> f)
        {
            for (var a = ma.Clone(); !await a.IsEmpty; a = await a.Tail)
            {
                yield return f(await a.Head);
            }
        }
    }

    /// <summary>
    /// Monad bind
    /// </summary>
    [Pure]
    public IteratorAsync<B> Bind<B>(Func<A, IteratorAsync<B>> f)
    {
        return Go(this, f).GetIteratorAsync();
        static async IAsyncEnumerable<B> Go(IteratorAsync<A> ma, Func<A, IteratorAsync<B>> f)
        {
            for (var a = ma.Clone(); !await a.IsEmpty; a = await a.Tail)
            {
                for (var b = f(await a.Head); !await b.IsEmpty; b = await b.Tail)
                {
                    yield return await b.Head;
                }
            }
        }
    }

    /// <summary>
    /// Monad bind
    /// </summary>
    [Pure]
    public IteratorAsync<C> SelectMany<B, C>(Func<A, IteratorAsync<B>> bind, Func<A, B, C> project)
    {
        return Go(this, bind, project).GetIteratorAsync();
        static async IAsyncEnumerable<C> Go(IteratorAsync<A> ma, Func<A, IteratorAsync<B>> bind, Func<A, B, C> project)
        {
            for (var a = ma.Clone(); !await a.IsEmpty; a = await a.Tail)
            {
                for (var b = bind(await a.Head); !await b.IsEmpty; b = await b.Tail)
                {
                    yield return project(await a.Head, await b.Head);
                }
            }
        }
    }

    /// <summary>
    /// Applicative apply
    /// </summary>
    [Pure]
    public IteratorAsync<B> Apply<B>(IteratorAsync<Func<A, B>> ff, IteratorAsync<A> fa)
    {
        return Go(ff, fa).GetIteratorAsync();
        static async IAsyncEnumerable<B> Go(IteratorAsync<Func<A, B>> ff, IteratorAsync<A> fa)
        {
            for (var f = ff.Clone(); !await f.IsEmpty; f = await f.Tail)
            {
                for (var a = fa.Clone(); !await a.IsEmpty; a = await a.Tail)
                {
                    yield return (await f.Head)(await a.Head);
                }
            }
        }
    }
    
    /// <summary>
    /// Concatenate two iterators
    /// </summary>
    [Pure]
    public IteratorAsync<A> Concat(IteratorAsync<A> other)
    {
        return Go(this, other).GetIteratorAsync();
        static async IAsyncEnumerable<A> Go(IteratorAsync<A> ma, IteratorAsync<A> mb)
        {
            for (var a = ma.Clone(); !await a.IsEmpty; a = await a.Tail)
            {
                yield return await ma.Head;
            }
            for (var b = mb.Clone(); !await b.IsEmpty; b = await b.Tail)
            {
                yield return await mb.Head;
            }
        }
    }

    /// <summary>
    /// Fold the sequence while there are more items remaining
    /// </summary>
    [Pure]
    public async ValueTask<S> Fold<S>(
        S state,
        Func<A, Func<S, S>> f)
    {
        for(var xs = Clone(); !await xs.IsEmpty; xs = await xs.Tail)
        {
            state = f(await xs.Head)(state);
        }
        return state;
    }

    /// <summary>
    /// Fold the sequence while there are more items remaining
    /// </summary>
    [Pure]
    public async ValueTask<S> Fold<S>(        
        S state,
        Func<S, A, S> f)
    {
        for(var xs = Clone(); !await xs.IsEmpty; xs = await xs.Tail)
        {
            state = f(state, await xs.Head);
        }
        return state;
    }

    /// <summary>
    /// Fold the sequence while the predicate returns `true` and there are more items remaining
    /// </summary>
    [Pure]
    public async ValueTask<S> FoldWhile<S>(
        S state,
        Func<A, Func<S, S>> f,
        Func<(S State, A Value), bool> predicate)
    {
        for(var xs = Clone(); !await xs.IsEmpty; xs = await xs.Tail)
        {
            if (!predicate((state, await xs.Head))) return state;
            state = f(await xs.Head)(state);
        }
        return state;
    }

    /// <summary>
    /// Fold the sequence while the predicate returns `true` and there are more items remaining
    /// </summary>
    [Pure]
    public async ValueTask<S> FoldWhile<S>(
        S state,
        Func<S, A, S> f,
        Func<(S State, A Value), bool> predicate)
    {
        for(var xs = Clone(); !await xs.IsEmpty; xs = await xs.Tail)
        {
            if (!predicate((state, await xs.Head))) return state;
            state = f(state, await xs.Head);
        }
        return state;
    }
    
    /// <summary>
    /// Fold the sequence until the predicate returns `true` or the sequence ends
    /// </summary>
    [Pure]
    public async ValueTask<S> FoldUntil<S>(
        S state,
        Func<A, Func<S, S>> f,
        Func<(S State, A Value), bool> predicate)
    {
        for(var xs = Clone(); !await xs.IsEmpty; xs = await xs.Tail)
        {
            state = f(await xs.Head)(state);
            if (predicate((state, await xs.Head))) return state;
        }
        return state;
    }

    /// <summary>
    /// Fold the sequence until the predicate returns `true` or the sequence ends
    /// </summary>
    [Pure]
    public async ValueTask<S> FoldUntil<S>(
        S state,
        Func<S, A, S> f,
        Func<(S State, A Value), bool> predicate)
    {
        for(var xs = Clone(); !await xs.IsEmpty; xs = await xs.Tail)
        {
            state = f(state, await xs.Head);
            if (predicate((state, await xs.Head))) return state;
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
    public IteratorAsync<A> Merge(IteratorAsync<A> other)
    {
        return Go(this, other).GetIteratorAsync();        
        static async IAsyncEnumerable<A> Go(IteratorAsync<A> ma, IteratorAsync<A> mb)
        {
            var a = ma.Clone();
            var b = mb.Clone();

            // TODO: Replace this with a WaitAny tasks for each side, replacing the 
            //       task as each one yields.
            
            while (!await a.IsEmpty && !await b.IsEmpty)
            {
                yield return await a.Head;
                yield return await b.Head;
                a = await a.Tail;
                b = await b.Tail;
            }

            if (await a.IsEmpty)
            {
                while (!await b.IsEmpty)
                {
                    yield return await b.Head;
                    b = await b.Tail;
                }
            }
            else
            {
                while (!await a.IsEmpty)
                {
                    yield return await a.Head;
                    a = await a.Tail;
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
    public IteratorAsync<(A First , A Second)> Zip(IteratorAsync<A> other)
    {
        return Go(this, other).GetIteratorAsync();        
        static async IAsyncEnumerable<(A First , A Second)> Go(IteratorAsync<A> ma, IteratorAsync<A> mb)
        {
            var a = ma.Clone();
            var b = mb.Clone();

            while (!await a.IsEmpty && !await b.IsEmpty)
            {
                yield return (await a.Head, await b.Head);
                a = await a.Tail;
                b = await b.Tail;
            }
        }
    }

    /// <summary>
    /// Combine two sequences
    /// </summary>
    public static IteratorAsync<A> operator +(IteratorAsync<A> ma, IteratorAsync<A> mb) =>
        ma.Concat(mb);

    /// <summary>
    /// Dispose
    /// </summary>
    public abstract ValueTask DisposeAsync();

    /// <summary>
    /// Nil iterator case
    ///
    /// The end of the sequence.
    /// </summary>
    public sealed class Nil : IteratorAsync<A>
    {
        public static readonly IteratorAsync<A> Default = new Nil();

        /// <summary>
        /// Head element
        /// </summary>
        public override ValueTask<A> Head =>
            throw new InvalidOperationException("Nil iterator has no head");

        /// <summary>
        /// Tail of the sequence
        /// </summary>
        public override ValueTask<IteratorAsync<A>> Tail =>
            new (this);

        /// <summary>
        /// Return true if there are no elements in the sequence.
        /// </summary>
        public override ValueTask<bool> IsEmpty =>
            new(true);

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
            new(0);

        public override ValueTask DisposeAsync() =>
            ValueTask.CompletedTask;
    }

    /// <summary>
    /// Cons iterator case.
    ///
    /// Contains a head value and a tail that represents the rest of the sequence.
    /// </summary>
    public abstract class Cons : IteratorAsync<A>
    {
        public void Deconstruct(out ValueTask<A> head, out ValueTask<IteratorAsync<A>> tail)
        {
            head = Head;
            tail = Tail;
        }
    }
    
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
        public override IteratorAsync<A> Split()
        {
            throw new InvalidOperationException("Can't split an IteratorAsync when the the Tail has already been consumed");
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
        
        public override async ValueTask DisposeAsync() =>
            await (await Tail).DisposeAsync();
    }
    
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
                       : throw new InvalidOperationException("Can't split an Iterator when the the Tail has already been consumed");
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
                throw new InvalidOperationException("Can't split an IteratorAsync when the the Tail has already been consumed");
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

    /// <summary>
    /// Get enumerator
    /// </summary>
    /// <returns></returns>
    [Pure]
    public IAsyncEnumerator<A> GetAsyncEnumerator(CancellationToken token) => 
        AsEnumerable(token).GetAsyncEnumerator(token);

    [Pure]
    public override string ToString() =>
        "async iterator";
}
