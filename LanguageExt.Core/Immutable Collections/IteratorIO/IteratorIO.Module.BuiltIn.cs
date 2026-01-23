#pragma warning disable CS0693 // Type parameter has the same name as the type parameter from outer type
using System;
using System.Collections.Generic;
using LanguageExt.ClassInstances;
using LanguageExt.Traits;

namespace LanguageExt;

public partial class IteratorIO
{
    /// <summary>
    /// Create an IteratorIO from an `IEnumerable` collection
    /// </summary>
    /// <param name="items">Collection to iterate</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns>IteratorIO of the collection</returns>
    public static IteratorIO<A> forward<A>(IEnumerable<A> items) =>
        new IteratorIO<A>.Enumerable(items);

    /// <summary>
    /// Create an IteratorIO from an `IAsyncEnumerable` collection
    /// </summary>
    /// <param name="items">Collection to iterate</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns>IteratorIO of the collection</returns>
    public static IteratorIO<A> forward<A>(IAsyncEnumerable<A> items) =>
        new IteratorIO<A>.AsyncEnumerable(items);

    /// <summary>
    /// Create an IteratorIO from an `IObservable` collection
    /// </summary>
    /// <param name="items">Collection to iterate</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns>IteratorIO of the collection</returns>
    public static IteratorIO<A> forward<A>(IObservable<A> items) =>
        liftIO(IO.lift(e => forward(items.ToAsyncEnumerable(e.Token))));

    /// <summary>
    /// Create an IteratorIO from a `ReadOnlySpan` collection
    /// </summary>
    /// <param name="items">Collection to iterate</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns>IteratorIO of the collection</returns>
    public static IteratorIO<A> forward<A>(ReadOnlySpan<A> items) =>
        forward(Arr.create(items));

    /// <summary>
    /// Create an IteratorIO from a `ReadOnlySpan` collection
    /// </summary>
    /// <param name="items">Collection to iterate</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns>IteratorIO of the collection</returns>
    public static IteratorIO<A> backward<A>(ReadOnlySpan<A> items) =>
        backward(Arr.create(items));

    /// <summary>
    /// Create an IteratorIO from an `Arr` collection
    /// </summary>
    /// <param name="items">Collection to iterate</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns>IteratorIO of the collection</returns>
    public static IteratorIO<A> forward<A>(Arr<A> items) =>
        new IteratorIO<A>.IterArr(items, 0, items.Count);

    /// <summary>
    /// Create an IteratorIO from an `Arr` collection
    /// </summary>
    /// <param name="items">Collection to iterate</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns>IteratorIO of the collection</returns>
    public static IteratorIO<A> backward<A>(Arr<A> items) =>
        new IteratorIO<A>.IterArrBkwd(items, items.Count - 1L, items.Count);

    /// <summary>
    /// Create an IteratorIO from an `HashMap` collection
    /// </summary>
    /// <param name="items">Collection to iterate</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns>IteratorIO of the collection</returns>
    public static IteratorIO<V> unordered<K, V>(HashMap<K, V> items) =>
        new IterHashMapValue<EqDefault<K>, K, V>(
            TrieMap.IteratorState<EqDefault<K>, K, V>.Setup(items.Value.Root));

    /// <summary>
    /// Create an IteratorIO from an `HashMap` collection
    /// </summary>
    /// <param name="items">Collection to iterate</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns>IteratorIO of the collection</returns>
    public static IteratorIO<V> unordered<EqK, K, V>(HashMap<EqK, K, V> items) 
        where EqK : Eq<K> =>
        new IterHashMapValue<EqK, K, V>(
            TrieMap.IteratorState<EqK, K, V>.Setup(items.Value.Root));

    /// <summary>
    /// Create an iterator from an `HashSet` collection
    /// </summary>
    /// <param name="items">Collection to iterate</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns>Iterator of the collection</returns>
    public static IteratorIO<A> unordered<A>(HashSet<A> items) =>
        new IterHashSet<EqDefault<A>, A>(
            TrieSet.IteratorState<EqDefault<A>, A>.Setup(items.Value.Root));

    /// <summary>
    /// Create an iterator from an `HashSet` collection
    /// </summary>
    /// <param name="items">Collection to iterate</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns>Iterator of the collection</returns>
    public static IteratorIO<A> unordered<EqA, A>(HashSet<EqA, A> items) 
        where EqA : Eq<A> =>
        new IterHashSet<EqA, A>(
            TrieSet.IteratorState<EqA, A>.Setup(items.Value.Root));

    /// <summary>
    /// Create an IteratorIO from a `Set` collection
    /// </summary>
    /// <param name="items">Collection to iterate</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns>IteratorIO of the collection</returns>
    public static IteratorIO<A> forward<A>(Lst<A> items) =>
        new IteratorIO<A>.IterLstFwd(new Lst.IteratorState<A>(items.Value.Root));

    /// <summary>
    /// Create an IteratorIO from a `Set` collection
    /// </summary>
    /// <param name="items">Collection to iterate</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns>IteratorIO of the collection</returns>
    public static IteratorIO<A> backward<A>(Lst<A> items) =>
        new IteratorIO<A>.IterLstBkwd(new Lst.IteratorState<A>(items.Value.Root));

    /// <summary>
    /// Create an IteratorIO from a `Set` collection
    /// </summary>
    /// <param name="items">Collection to iterate</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns>IteratorIO of the collection</returns>
    public static IteratorIO<(K Key, V Value)> forward<K, V>(Map<K, V> items) =>
        new IterMapFwd<K, V>(new Map.IteratorState<K, V>(items.Value.Root));

    /// <summary>
    /// Create an IteratorIO from a `Set` collection
    /// </summary>
    /// <param name="items">Collection to iterate</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns>IteratorIO of the collection</returns>
    public static IteratorIO<(K Key, V Value)> backward<K, V>(Map<K, V> items) =>
        new IterMapBkwd<K, V>(new Map.IteratorState<K, V>(items.Value.Root));

    /// <summary>
    /// Create an IteratorIO from a `Seq` collection
    /// </summary>
    /// <param name="items">Collection to iterate</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns>IteratorIO of the collection</returns>
    public static IteratorIO<A> forward<A>(Seq<A> items) =>
        new IteratorIO<A>.IterSeq(items);

    /// <summary>
    /// Create an IteratorIO from a `Set` collection
    /// </summary>
    /// <param name="items">Collection to iterate</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns>IteratorIO of the collection</returns>
    public static IteratorIO<A> forward<A>(Set<A> items) =>
        new IteratorIO<A>.IterSetFwd(new Set.IteratorState<A>(items.Value.Root));

    /// <summary>
    /// Create an IteratorIO from a `Set` collection
    /// </summary>
    /// <param name="items">Collection to iterate</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns>IteratorIO of the collection</returns>
    public static IteratorIO<A> backward<A>(Set<A> items) =>
        new IteratorIO<A>.IterSetBkwd(new Set.IteratorState<A>(items.Value.Root));
}
