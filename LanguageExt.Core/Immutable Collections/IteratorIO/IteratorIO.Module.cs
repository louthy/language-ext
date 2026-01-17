using System;

namespace LanguageExt;

public partial class IteratorIO
{
    /// <summary>
    /// Lift a pure iterator into an `IteratorIO`
    /// </summary>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns>IteratorIO</returns>
    public static IteratorIO<A> lift<A>(Iterator<A> items) =>
        new IteratorIO<A>.Lift(items);

    /// <summary>
    /// Empty sequence
    /// </summary>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns>IteratorIO</returns>
    public static IteratorIO<A> empty<A>() =>
        IteratorIO<A>.Nil.Default;

    /// <summary>
    /// Construct a singleton sequence 
    /// </summary>
    /// <param name="head">Head item</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns>IteratorIO</returns>
    public static IteratorIO<A> singleton<A>(A head) =>
        new IteratorIO<A>.Singleton(head);
    
    /// <summary>
    /// Create an IteratorIO from a continuation function.  This function
    /// is called repeatedly until it yields `Nil`
    /// </summary>
    /// <param name="IteratorIO"></param>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns></returns>
    public static IteratorIO<A> cons<A>(A head, Func<IteratorIO<A>> tail) =>
        new IteratorIO<A>.Cons(head, tail);

    /// <summary>
    /// Construct a sequence from a head item and a tail sequence
    /// </summary>
    /// <param name="head">Head item</param>
    /// <param name="tail">Tail sequences</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns>IteratorIO</returns>
    public static IteratorIO<A> cons<A>(A head, IteratorIO<A> tail) =>
        new IteratorIO<A>.ConsStrict(head, tail);

    /// <summary>
    /// Create an IteratorIO from a continuation function.  This function
    /// is called repeatedly until it yields `Nil`
    /// </summary>
    /// <param name="IteratorIO"></param>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns></returns>
    public static IteratorIO<A> lazy<A>(Func<(A Head, IteratorIO<A> Tail)> next) =>
        new IteratorIO<A>.Cont(next);

    /// <summary>
    /// Create an IteratorIO from a continuation function.  This function
    /// is called repeatedly until it yields `Nil`
    /// </summary>
    /// <param name="IteratorIO"></param>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns></returns>
    public static IteratorIO<A> lazy<A>(Func<IteratorIO<A>> next) =>
        new IteratorIO<A>.Lazy(next);
}
