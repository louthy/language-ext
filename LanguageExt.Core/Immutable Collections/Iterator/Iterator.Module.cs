using System;

namespace LanguageExt;

public partial class Iterator
{
    /// <summary>
    /// Empty sequence
    /// </summary>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns>An iterator</returns>
    public static Iterator<A> empty<A>() =>
        Iterator<A>.Nil.Default;

    /// <summary>
    /// Construct a singleton sequence 
    /// </summary>
    /// <param name="head">Head item</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns>An iterator</returns>
    public static Iterator<A> singleton<A>(A head) =>
        new Iterator<A>.Singleton(head);

    /// <summary>
    /// Create an iterator from a continuation function.  This function
    /// is called repeatedly until it yields `Nil`
    /// </summary>
    /// <param name="iterator"></param>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns>An iterator</returns>
    public static Iterator<A> cons<A>(A head, Func<Iterator<A>> tail) =>
        new Iterator<A>.Cons(head, tail);

    /// <summary>
    /// Construct a sequence from a head item and a tail sequence
    /// </summary>
    /// <param name="head">Head item</param>
    /// <param name="tail">Tail sequences</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns>An iterator</returns>
    public static Iterator<A> cons<A>(A head, Iterator<A> tail) =>
        new Iterator<A>.ConsStrict(head, tail);

    /// <summary>
    /// Create an iterator from a continuation function.  This function
    /// is called repeatedly until it yields `Nil`
    /// </summary>
    /// <param name="iterator"></param>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns>An iterator</returns>
    public static Iterator<A> lazy<A>(Func<(A Head, Iterator<A> Tail)> next) =>
        new Iterator<A>.Cont(next);

    /// <summary>
    /// Create an iterator from a continuation function.  This function
    /// is called repeatedly until it yields `Nil`
    /// </summary>
    /// <param name="iterator"></param>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns>An iterator</returns>
    public static Iterator<A> lazy<A>(Func<Iterator<A>> next) =>
        new Iterator<A>.Lazy(next);

    /// <summary>
    /// Yield a value forever
    /// </summary>
    /// <param name="value">Value to yield</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns>An iterator</returns>
    public static Iterator<A> forever<A>(A value) =>
        new IterForever<A>(value);
}
