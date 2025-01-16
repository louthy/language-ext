using System;
using System.Collections.Generic;

namespace LanguageExt;

public partial class IteratorAsync
{
    /// <summary>
    /// Create an iterator from an `IAsyncEnumerable`
    /// </summary>
    /// <param name="enumerable"></param>
    /// <typeparam name="A"></typeparam>
    /// <returns></returns>
    public static IteratorAsync<A> from<A>(IAsyncEnumerable<A> enumerable) =>
        new IteratorAsync<A>.ConsFirst(enumerable);

    /// <summary>
    /// Construct a singleton sequence 
    /// </summary>
    /// <param name="head">Head item</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>IteratorAsync</returns>
    public static IteratorAsync<A> singleton<A>(A head) =>
        new IteratorAsync<A>.ConsValue(head, IteratorAsync<A>.Nil.Default);

    /// <summary>
    /// Construct a sequence from a head item and a tail sequence
    /// </summary>
    /// <param name="head">Head item</param>
    /// <param name="tail">Tail sequences</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>IteratorAsync</returns>
    public static IteratorAsync<A> Cons<A>(A head, IteratorAsync<A> tail) =>
        new IteratorAsync<A>.ConsValue(head, tail);

    /// <summary>
    /// Construct a sequence from a head item and a tail sequence
    /// </summary>
    /// <param name="head">Head item</param>
    /// <param name="tail">Tail sequences</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>IteratorAsync</returns>
    public static IteratorAsync<A> Cons<A>(A head, Func<IteratorAsync<A>> tail) =>
        new IteratorAsync<A>.ConsValueLazy(new(head), tail);

    /// <summary>
    /// Empty sequence
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>IteratorAsync</returns>
    public static IteratorAsync<A> Nil<A>() =>
        IteratorAsync<A>.Nil.Default;
}
