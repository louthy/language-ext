using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
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
public abstract partial class IteratorAsync<A> : 
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
            if (token.IsCancellationRequested) throw new OperationCanceledException();
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
