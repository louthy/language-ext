#pragma warning disable CS0693 // Type parameter has the same name as the type parameter from outer type
using System;
using System.Collections.Generic;
using LanguageExt.ClassInstances;
using LanguageExt.Traits;

namespace LanguageExt;

public partial class Iterator
{
    /// <summary>
    /// Create an iterator from an `IEnumerable` collection
    /// </summary>
    /// <param name="items">Collection to iterate</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns>Iterator of the collection</returns>
    public static Iterator<A> forward<A>(IEnumerable<A> items) =>
        new Iterator<A>.Enumerable(items);

    /// <summary>
    /// Create an iterator from a `ReadOnlySpan` collection
    /// </summary>
    /// <param name="items">Collection to iterate</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns>Iterator of the collection</returns>
    public static Iterator<A> forward<A>(params ReadOnlySpan<A> items) =>
        forward(Arr.create(items));

    /// <summary>
    /// Create an iterator from a `ReadOnlySpan` collection
    /// </summary>
    /// <param name="items">Collection to iterate</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns>Iterator of the collection</returns>
    public static Iterator<A> backward<A>(ReadOnlySpan<A> items) =>
        backward(Arr.create(items));

    /// <summary>
    /// Create an iterator from an `Arr` collection
    /// </summary>
    /// <param name="items">Collection to iterate</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns>Iterator of the collection</returns>
    public static Iterator<A> forward<A>(Arr<A> items) =>
        items.ForwardIterator();

    /// <summary>
    /// Create an iterator from an `Arr` collection
    /// </summary>
    /// <param name="items">Collection to iterate</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns>Iterator of the collection</returns>
    public static Iterator<A> backward<A>(Arr<A> items) =>
        items.BackwardIterator();

    /// <summary>
    /// Create an iterator from an `HashMap` collection
    /// </summary>
    /// <param name="items">Collection to iterate</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns>Iterator of the collection</returns>
    public static Iterator<V> unordered<K, V>(HashMap<K, V> items) =>
        new IterHashMapValue<EqDefault<K>, K, V>(
            TrieMap.IteratorState<EqDefault<K>, K, V>.Setup(items.Value.Root));

    /// <summary>
    /// Create an iterator from an `HashMap` collection
    /// </summary>
    /// <param name="items">Collection to iterate</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns>Iterator of the collection</returns>
    public static Iterator<V> unordered<EqK, K, V>(HashMap<EqK, K, V> items) 
        where EqK : Eq<K> =>
        new IterHashMapValue<EqK, K, V>(
            TrieMap.IteratorState<EqK, K, V>.Setup(items.Value.Root));

    /// <summary>
    /// Create an iterator from an `HashSet` collection
    /// </summary>
    /// <param name="items">Collection to iterate</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns>Iterator of the collection</returns>
    public static Iterator<A> unordered<A>(HashSet<A> items) =>
        new IterHashSet<EqDefault<A>, A>(
            TrieSet.IteratorState<EqDefault<A>, A>.Setup(items.Value.Root));

    /// <summary>
    /// Create an iterator from an `HashSet` collection
    /// </summary>
    /// <param name="items">Collection to iterate</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns>Iterator of the collection</returns>
    public static Iterator<A> unordered<EqA, A>(HashSet<EqA, A> items) 
        where EqA : Eq<A> =>
        new IterHashSet<EqA, A>(
            TrieSet.IteratorState<EqA, A>.Setup(items.Value.Root));
    
    /// <summary>
    /// Create an iterator from a `Set` collection
    /// </summary>
    /// <param name="items">Collection to iterate</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns>Iterator of the collection</returns>
    public static Iterator<A> forward<A>(Lst<A> items) =>
        items.ForwardIterator();

    /// <summary>
    /// Create an iterator from a `Set` collection
    /// </summary>
    /// <param name="items">Collection to iterate</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns>Iterator of the collection</returns>
    public static Iterator<A> backward<A>(Lst<A> items) =>
        items.BackwardIterator();

    /// <summary>
    /// Create an iterator from a `Set` collection
    /// </summary>
    /// <param name="items">Collection to iterate</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns>Iterator of the collection</returns>
    public static Iterator<(K Key, V Value)> forward<K, V>(Map<K, V> items) =>
        new IterMapFwd<K, V>(new Map.IteratorState<K, V>(items.Value.Root));

    /// <summary>
    /// Create an iterator from a `Set` collection
    /// </summary>
    /// <param name="items">Collection to iterate</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns>Iterator of the collection</returns>
    public static Iterator<(K Key, V Value)> backward<K, V>(Map<K, V> items) =>
        new IterMapBkwd<K, V>(new Map.IteratorState<K, V>(items.Value.Root));

    /// <summary>
    /// Create an iterator from a `Seq` collection
    /// </summary>
    /// <param name="items">Collection to iterate</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns>Iterator of the collection</returns>
    public static Iterator<A> forward<A>(Seq<A> items) =>
        items.ForwardIterator();

    /// <summary>
    /// Create an iterator from a `Set` collection
    /// </summary>
    /// <param name="items">Collection to iterate</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns>Iterator of the collection</returns>
    public static Iterator<A> forward<A>(Set<A> items) =>
        items.ForwardIterator();

    /// <summary>
    /// Create an iterator from a `Set` collection
    /// </summary>
    /// <param name="items">Collection to iterate</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns>Iterator of the collection</returns>
    public static Iterator<A> backward<A>(Set<A> items) =>
        items.BackwardIterator();
}
