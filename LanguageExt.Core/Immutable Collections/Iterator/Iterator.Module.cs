#pragma warning disable CS0693 // Type parameter has the same name as the type parameter from outer type
using System;
using System.Collections.Generic;

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
    
    /// <summary>
    /// Create an iterator from a continuation function.  This function
    /// is called repeatedly until it yields `Nil`
    /// </summary>
    /// <param name="iterator"></param>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns></returns>
    public static Iterator<A> Cons<A>(A head, Func<Iterator<A>> tail) =>
        new Iterator<A>.Cons(head, tail);

    /// <summary>
    /// Construct a sequence from a head item and a tail sequence
    /// </summary>
    /// <param name="head">Head item</param>
    /// <param name="tail">Tail sequences</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns>Iterator</returns>
    public static Iterator<A> ConsStrict<A>(A head, Iterator<A> tail) =>
        new Iterator<A>.ConsStrict(head, tail);

    /// <summary>
    /// Create an iterator from a continuation function.  This function
    /// is called repeatedly until it yields `Nil`
    /// </summary>
    /// <param name="iterator"></param>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns></returns>
    public static Iterator<A> Lazy<A>(Func<(A Head, Iterator<A> Tail)> next) =>
        new Iterator<A>.Cont(next);

    /// <summary>
    /// Create an iterator from a continuation function.  This function
    /// is called repeatedly until it yields `Nil`
    /// </summary>
    /// <param name="iterator"></param>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns></returns>
    public static Iterator<A> Lazy<A>(Func<Iterator<A>> next) =>
        new Iterator<A>.Lazy(next);

    /// <summary>
    /// Construct a singleton sequence 
    /// </summary>
    /// <param name="head">Head item</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns>Iterator</returns>
    public static Iterator<A> singleton<A>(A head) =>
        new Iterator<A>.Singleton(head);

    /// <summary>
    /// Empty sequence
    /// </summary>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns>Iterator</returns>
    public static Iterator<A> Nil<A>() =>
        Iterator<A>.Nil.Default;
}
