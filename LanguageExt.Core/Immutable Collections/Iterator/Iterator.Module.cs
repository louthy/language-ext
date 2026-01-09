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
    public static Iterator<A> from<A>(IEnumerable<A> items) =>
        new Iterator<A>.Enumerable(items);

    /// <summary>
    /// Create an iterator from an `Arr` collection
    /// </summary>
    /// <param name="items">Collection to iterate</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns>Iterator of the collection</returns>
    public static Iterator<A> from<A>(Arr<A> items) =>
        new Iterator<A>.IterArr(items, 0, items.Count);

    /// <summary>
    /// Create an iterator from a `Seq` collection
    /// </summary>
    /// <param name="items">Collection to iterate</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns>Iterator of the collection</returns>
    public static Iterator<A> from<A>(Seq<A> items) =>
        new Iterator<A>.IterSeq(items);
    
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
