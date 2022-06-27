#nullable enable

using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using static LanguageExt.Prelude;
using LanguageExt.ClassInstances;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace LanguageExt;

/// <summary>
/// Event thrown when an item is dequeued from an `AtomQue`
/// </summary>
/// <typeparam name="A">Item that was dequeued</typeparam>
public delegate void AtomDequeuedEvent<in A>(A value);

/// <summary>
/// Event thrown when an item is enqueued to an `AtomQue`
/// </summary>
/// <typeparam name="A">Item that was enqueued</typeparam>
public delegate void AtomEnqueuedEvent<in A>(A value);

/// <summary>
/// Atoms provide a way to manage shared, synchronous, independent state without 
/// locks.  `AtomQue` wraps the language-ext `Que`, and makes sure all operations are atomic and thread-safe
/// without resorting to locking.
/// </summary>
/// <remarks>
/// See the [concurrency section](https://github.com/louthy/language-ext/wiki/Concurrency) of the wiki for more info.
/// </remarks>
/// <typeparam name="A">Item value type</typeparam>
public class AtomQue<A> : 
    IEquatable<AtomQue<A>>, 
    IEquatable<Que<A>>,
    IEnumerable<A>
{
    QueInternal<A> items;
    public event AtomDequeuedEvent<A>? Dequeued;
    public event AtomEnqueuedEvent<A>? Enqueued;

    internal AtomQue() =>
        items = QueInternal<A>.Empty;

    internal AtomQue(IEnumerable<A> items) =>
        this.items = new QueInternal<A>(items);

    internal AtomQue(Que<A> items) =>
        this.items = new QueInternal<A>(items);

    /// <summary>
    /// Impure iteration of the bound value in the structure
    /// </summary>
    /// <returns>
    /// Returns the original unmodified structure
    /// </returns>
    public Unit Do(Action<A> f)
    {
        this.Iter(f);
        return default;
    }

    /// <summary>
    /// Reference version for use in pattern-matching
    /// </summary>
    /// <remarks>
    ///
    ///     Empty collection     = null
    ///     Singleton collection = A
    ///     More                 = (A, Seq<A>)   -- head and tail
    ///
    ///     var res = list.Case switch
    ///     {
    ///       
    ///        (var x, var xs) => ...,
    ///        A value         => ...,
    ///        _               => ...
    ///     }
    /// 
    /// </remarks>
    [Pure]
    public object? Case =>
        IsEmpty
            ? null
            : Prelude.toSeq(items).Case;

    /// <summary>
    /// Is the queue empty
    /// </summary>
    [Pure]
    public bool IsEmpty
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => items.IsEmpty;
    }

    /// <summary>
    /// Number of items in the queue
    /// </summary>
    [Pure]
    public int Count
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => items.Count;
    }

    /// <summary>
    /// Alias of Count
    /// </summary>
    [Pure]
    public int Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => items.Count;
    }

    /// <summary>
    /// Clears the queue atomically
    /// </summary>
    public Unit Clear()
    {
        SpinWait sw = default;
        while (true)
        {
            var oitems = items;
            if (oitems.IsEmpty) return default;
            var nitems = new QueInternal<A>();
            if (ReferenceEquals(Interlocked.CompareExchange(ref items, nitems, oitems), oitems))
            {
                if (Dequeued != null)
                {
                    foreach (var x in oitems)
                    {
                        Dequeued?.Invoke(x);
                    }
                }
                return default;
            }
            else
            {
                sw.SpinOnce();
            }
        }
    }

    /// <summary>
    /// Look at the item at the front of the queue, if it exists.  
    /// </summary>
    /// <returns>The item at the front of the queue, if it exists.  `None` otherwise.</returns>
    [Pure]
    public Option<A> Peek()
    {
        var xs = items;
        return xs.IsEmpty
            ? None
            : xs.Peek();
    }

    /// <summary>
    /// Removes the item from the front of the queue atomically
    /// </summary>
    /// <returns>The item that was at the front of the queue (if it existed, `None` otherwise)</returns>
    public Option<A> Dequeue()
    {
        SpinWait sw = default;
        while (true)
        {
            var oitems = items;
            if (oitems.IsEmpty) return default;
            var top = oitems.Peek();
            var nitems = oitems.Dequeue();
            if (ReferenceEquals(Interlocked.CompareExchange(ref items, nitems, oitems), oitems))
            {
                Dequeued?.Invoke(top);
                return top;
            }
            else
            {
                sw.SpinOnce();
            }
        }
    }
    
    /// <summary>
    /// Removes the item from the front of the queue atomically
    /// </summary>
    /// <exception cref="InvalidOperationException">If the queue is empty</exception>
    /// <returns>The item that was at the front of the queue, or an `InvalidOperationException` exception</returns>
    public A DequeueUnsafe()
    {
        SpinWait sw = default;
        while (true)
        {
            var oitems = items;
            if (oitems.IsEmpty) throw new InvalidOperationException("Queue is empty");
            var top = oitems.Peek();
            var nitems = oitems.Dequeue();
            if (ReferenceEquals(Interlocked.CompareExchange(ref items, nitems, oitems), oitems))
            {
                Dequeued?.Invoke(top);
                return top;
            }
            else
            {
                sw.SpinOnce();
            }
        }
    }

    /// <summary>
    /// Add an item to the end of the queue atomically
    /// </summary>
    /// <param name="value">Value to add to the queue</param>
    public Unit Enqueue(A value)
    {
        SpinWait sw = default;
        while (true)
        {
            var oitems = items;
            var nitems = oitems.Enqueue(value);
            if (ReferenceEquals(Interlocked.CompareExchange(ref items, nitems, oitems), oitems))
            {
                Enqueued?.Invoke(value);
                return default;
            }
            else
            {
                sw.SpinOnce();
            }
        }
    }
    
    [Pure]
    public Seq<A> ToSeq() =>
        items.ToSeq();

    [Pure]
    public IEnumerable<A> AsEnumerable() =>
        items.AsEnumerable();

    [Pure]
    public IEnumerator<A> GetEnumerator() =>
        AsEnumerable().GetEnumerator();

    [Pure]
    IEnumerator IEnumerable.GetEnumerator() =>
        AsEnumerable().GetEnumerator();

    [Pure]
    public Unit Append(Que<A> rhs)
    {
        SpinWait sw = default;
        while (true)
        {
            var oitems = items;
            var nitems = oitems.Append(rhs.Value);
            if (ReferenceEquals(Interlocked.CompareExchange(ref items, nitems, oitems), oitems))
            {
                if (Enqueued != null)
                {
                    foreach (var item in rhs.Value)
                    {
                        Enqueued?.Invoke(item);
                    }
                }
                return default;
            }
            else
            {
                sw.SpinOnce();
            }
        }
    }

    [Pure]
    public Unit Append(AtomQue<A> rhs)
    {
        var ritems = rhs.items;
        SpinWait sw = default;
        while (true)
        {
            var oitems = items;
            var nitems = oitems.Append(ritems);
            if (ReferenceEquals(Interlocked.CompareExchange(ref items, nitems, oitems), oitems))
            {
                if (Enqueued != null)
                {
                    foreach (var item in ritems)
                    {
                        Enqueued?.Invoke(item);
                    }
                }
                return default;
            }
            else
            {
                sw.SpinOnce();
            }
        }
    }

    [Pure]
    public static bool operator ==(AtomQue<A> lhs, AtomQue<A> rhs) =>
        lhs.Equals(rhs);

    [Pure]
    public static bool operator ==(AtomQue<A> lhs, Que<A> rhs) =>
        lhs.Equals(rhs);

    [Pure]
    public static bool operator ==(Que<A> lhs, AtomQue<A> rhs) =>
        rhs.Equals(lhs);

    [Pure]
    public static bool operator !=(AtomQue<A> lhs, AtomQue<A> rhs) =>
        !(lhs == rhs);

    [Pure]
    public static bool operator !=(Que<A> lhs, AtomQue<A> rhs) =>
        !(lhs == rhs);

    [Pure]
    public static bool operator !=(AtomQue<A> lhs, Que<A> rhs) =>
        !(lhs == rhs);

    [Pure]
    public override int GetHashCode() =>
        items.GetHashCode();

    [Pure]
    public override bool Equals(object obj) =>
        obj switch
        {
            AtomQue<A> q => this == q,
            Que<A> q     => this == q,
            _            => false
        };

    [Pure]
    public bool Equals(AtomQue<A>? other) =>
        other is not null &&
        GetHashCode() == other.GetHashCode() &&
        default(EqEnumerable<A>).Equals(items, other.items);

    [Pure]
    public bool Equals(Que<A> other) =>
        GetHashCode() == other.GetHashCode() &&
        default(EqEnumerable<A>).Equals(items, other.Value);
}
