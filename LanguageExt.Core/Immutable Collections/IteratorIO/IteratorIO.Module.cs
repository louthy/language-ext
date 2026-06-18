using System;

namespace LanguageExt;

public partial class IteratorIO
{
    /// <summary>
    /// Empty sequence
    /// </summary>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns>An iterator</returns>
    public static IteratorIO<A> empty<A>() =>
        IteratorIO<A>.Nil.Default;

    /// <summary>
    /// Construct a singleton sequence 
    /// </summary>
    /// <param name="head">Head item</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns>An iterator</returns>
    public static IteratorIO<A> singleton<A>(A head) =>
        new IteratorIO<A>.Singleton(head);
    
    /// <summary>
    /// Create an IteratorIO from a continuation function.  This function
    /// is called repeatedly until it yields `Nil`
    /// </summary>
    /// <param name="IteratorIO"></param>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns>An iterator</returns>
    public static IteratorIO<A> cons<A>(A head, Func<IteratorIO<A>> tail) =>
        new IteratorIO<A>.Cons(head, tail);
    
    /// <summary>
    /// Create an IteratorIO from a continuation function.  This function
    /// is called repeatedly until it yields `Nil`
    /// </summary>
    /// <param name="IteratorIO"></param>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns>An iterator</returns>
    public static IteratorIO<A> cons<A>(A head, IO<IteratorIO<A>> tail) =>
        new IteratorIO<A>.ConsIO(head, tail);

    /// <summary>
    /// Construct a sequence from a head item and a tail sequence
    /// </summary>
    /// <param name="head">Head item</param>
    /// <param name="tail">Tail sequences</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns>An iterator</returns>
    public static IteratorIO<A> cons<A>(A head, IteratorIO<A> tail) =>
        new IteratorIO<A>.ConsStrict(head, tail);

    /// <summary>
    /// Create an IteratorIO from a continuation function.  This function
    /// is called repeatedly until it yields `Nil`
    /// </summary>
    /// <param name="IteratorIO"></param>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns>An iterator</returns>
    public static IteratorIO<A> lazy<A>(Func<(A Head, IteratorIO<A> Tail)> next) =>
        new IteratorIO<A>.Cont(next);

    /// <summary>
    /// Create an IteratorIO from a continuation function.  
    /// </summary>
    /// <param name="IteratorIO"></param>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns>An iterator</returns>
    public static IteratorIO<A> lazy<A>(Func<IteratorIO<A>> next) =>
        new IteratorIO<A>.Lazy(next);

    /// <summary>
    /// Lift a pure iterator into an `IteratorIO`
    /// </summary>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns>An iterator</returns>
    public static IteratorIO<A> lift<A>(Iterator<A> iterator) =>
        new IteratorIO<A>.Lift(iterator);
    
    /// <summary>
    /// Create an IteratorIO from an iterator wrapped in an IO operation  
    /// </summary>
    /// <param name="IteratorIO"></param>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns>An iterator</returns>
    public static IteratorIO<A> liftIO<A>(IO<IteratorIO<A>> next) =>
        new LiftIO<A>(next);
    
    /// <summary>
    /// Create an IteratorIO from an IO operation.  
    /// </summary>
    /// <param name="IteratorIO"></param>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns>An iterator</returns>
    public static IteratorIO<A> liftIO<A>(IO<A> next) =>
        new LiftIO2<A>(next);
    
    /// <summary>
    /// Yield a value forever
    /// </summary>
    /// <param name="value">Value to yield</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns>An iterator</returns>
    public static IteratorIO<A> forever<A>(A value) =>
        new IterForever<A>(value);
    
    /// <summary>
    /// Yields an effect forever, if the IO operation fails, the yielding stops
    /// </summary>
    /// <param name="value">Value to yield</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns>An iterator</returns>
    public static IteratorIO<A> forever<A>(IO<A> operation) =>
        new IterForeverIO<A>(operation);
}
