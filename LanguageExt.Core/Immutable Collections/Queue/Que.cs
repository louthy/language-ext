#nullable enable

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using LanguageExt.ClassInstances;

namespace LanguageExt;

/// <summary>
/// Immutable queue collection
/// </summary>
/// <typeparam name="A">Item value type</typeparam>
[Serializable]
public readonly struct Que<A> : 
    IEnumerable<A>, 
    IEquatable<Que<A>>
{
    public static readonly Que<A> Empty = new (QueInternal<A>.Empty);

    readonly QueInternal<A> value;
    internal QueInternal<A> Value => value ?? QueInternal<A>.Empty;

    internal Que(QueInternal<A> value) =>
        this.value = value;

    /// <summary>
    /// Construct from a enumerable of items
    /// </summary>
    /// <param name="items">Items to construct the queue from</param>
    public Que(IEnumerable<A> items) =>
        value = new QueInternal<A>(items);

    /// <summary>
    /// Impure iteration of the bound value in the structure
    /// </summary>
    /// <returns>
    /// Returns the original unmodified structure
    /// </returns>
    public Que<A> Do(Action<A> f)
    {
        this.Iter(f);
        return this;
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
            : toSeq(Value).Case;

    /// <summary>
    /// Is the queue empty
    /// </summary>
    [Pure]
    public bool IsEmpty
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => value?.IsEmpty ?? true;
    }

    /// <summary>
    /// Number of items in the queue
    /// </summary>
    [Pure]
    public int Count
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => value?.Count ?? 0;
    }

    /// <summary>
    /// Alias of Count
    /// </summary>
    [Pure]
    public int Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => value?.Count ?? 0;
    }

    /// <summary>
    /// Returns an empty queue 
    /// </summary>
    /// <returns>Empty queue</returns>
    [Pure]
    public Que<A> Clear() =>
        Empty;

    /// <summary>
    /// Look at the item at the front of the queue
    /// </summary>
    /// <exception cref="InvalidOperationException">Throws if the queue is empty</exception>
    /// <returns>The item at the front of the queue, or throw an exception if none exists</returns>
    [Pure]
    public A Peek() =>
        Value.Peek();

    /// <summary>
    /// Removes the item from the front of the queue
    /// </summary>
    /// <returns>A new `Que` with the first item removed</returns>
    [Pure]
    public Que<A> Dequeue() =>
        new (Value.Dequeue());

    /// <summary>
    /// Removes the item from the front of the queue
    /// </summary>
    /// <exception cref="InvalidOperationException">Throws if the queue is empty</exception>
    /// <returns>A tuple containing the new `Que` with the first item removed and the first item</returns>
    [Pure]
    public (Que<A> Queue, A Value) DequeueUnsafe() =>
        (Dequeue(), Peek());

    /// <summary>
    /// Removes the item from the front of the queue
    /// </summary>
    /// <returns>A tuple containing the new `Que` with the first item removed and optionally the first item</returns>
    [Pure]
    public (Que<A> Queue, Option<A> Value) TryDequeue() =>
        Value.TryDequeue().MapFirst(qi => new Que<A>(qi));

    /// <summary>
    /// Look at the item at the front of the queue, if it exists.  
    /// </summary>
    /// <returns>The item at the front of the queue, if it exists.  `None` otherwise.</returns>
    [Pure]
    public Option<A> TryPeek() =>
        Value.TryPeek();

    /// <summary>
    /// Add an item to the end of the queue
    /// </summary>
    /// <param name="value">Value to add to the queue</param>
    /// <returns>A new queue with the item added</returns>
    [Pure]
    public Que<A> Enqueue(A value) =>
        new (Value.Enqueue(value));

    /// <summary>
    /// Convert to a `Seq`
    /// </summary>
    /// <returns>`Seq`</returns>
    [Pure]
    public Seq<A> ToSeq() =>
        Value.ToSeq();

    /// <summary>
    /// Convert to an `IEnumerable`
    /// </summary>
    /// <returns>`IEnumerable`</returns>
    [Pure]
    public IEnumerable<A> AsEnumerable() =>
        Value.AsEnumerable();

    /// <summary>
    /// Get an enumerator of the collection
    /// </summary>
    /// <returns>`IEnumerator`</returns>
    [Pure]
    public IEnumerator<A> GetEnumerator() =>
        AsEnumerable().GetEnumerator();

    /// <summary>
    /// Get an enumerator of the collection
    /// </summary>
    /// <returns>`IEnumerator`</returns>
    [Pure]
    IEnumerator IEnumerable.GetEnumerator() =>
        AsEnumerable().GetEnumerator();

    /// <summary>
    /// Append two queues together
    /// </summary>
    /// <param name="lhs">First part of the queue</param>
    /// <param name="rhs">Second part of the queue</param>
    /// <returns>Concatenated queue</returns>
    [Pure]
    public static Que<A> operator +(Que<A> lhs, Que<A> rhs) =>
        lhs.Append(rhs);

    /// <summary>
    /// Append two queues together
    /// </summary>
    /// <param name="rhs">Second part of the queue</param>
    /// <returns>Concatenated queue</returns>
    [Pure]
    public Que<A> Append(Que<A> rhs) =>
        new (Value.Append(rhs));

    /// <summary>
    /// Subtract one queue from another
    /// </summary>
    /// <param name="lhs">Starting queue</param>
    /// <param name="rhs">Items to remove from the queue</param>
    /// <returns>lhs - rhs</returns>
    [Pure]
    public static Que<A> operator -(Que<A> lhs, Que<A> rhs) =>
        lhs.Subtract(rhs);

    /// <summary>
    /// Subtract one queue from another
    /// </summary>
    /// <param name="rhs">Items to remove from the queue</param>
    /// <returns>lhs - rhs</returns>
    [Pure]
    public Que<A> Subtract(Que<A> rhs) =>
        new (Enumerable.Except(Value.AsEnumerable(), rhs.Value.AsEnumerable()));

    /// <summary>
    /// Queue equality
    /// </summary>
    /// <param name="lhs">First queue</param>
    /// <param name="rhs">Second queue</param>
    /// <returns>`true` if the queues are equal</returns>
    [Pure]
    public static bool operator ==(Que<A> lhs, Que<A> rhs) =>
        lhs.Equals(rhs);

    /// <summary>
    /// Queue inequality
    /// </summary>
    /// <param name="lhs">First queue</param>
    /// <param name="rhs">Second queue</param>
    /// <returns>`true` if the queues are not-equal</returns>
    [Pure]
    public static bool operator !=(Que<A> lhs, Que<A> rhs) =>
        !(lhs == rhs);

    /// <summary>
    /// Hash code of the items in the queue
    /// </summary>
    /// <returns>Hash code</returns>
    [Pure]
    public override int GetHashCode() =>
        Value.GetHashCode();

    /// <summary>
    /// Queue equality
    /// </summary>
    /// <param name="obj">Value that may be a queue</param>
    /// <returns>`true` if the queues are equal</returns>
    [Pure]
    public override bool Equals(object obj) =>
        obj is Que<A> q && Equals(q);

    /// <summary>
    /// Queue equality
    /// </summary>
    /// <param name="other">Second queue</param>
    /// <returns>`true` if the queues are equal</returns>
    [Pure]
    public bool Equals(Que<A> other) =>
        GetHashCode() == other.GetHashCode() &&
        default(EqEnumerable<A>).Equals(this.Value, other.Value);

    /// <summary>
    /// Implicit conversion from an untyped empty list
    /// </summary>
    public static implicit operator Que<A>(SeqEmpty _) =>
        Empty;
}
