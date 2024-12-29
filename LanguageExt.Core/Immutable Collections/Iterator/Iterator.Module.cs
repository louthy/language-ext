using System.Collections.Generic;

namespace LanguageExt;

public partial class Iterator
{
    /// <summary>
    /// Create an iterator from an `IEnumerable`
    /// </summary>
    /// <param name="enumerable"></param>
    /// <typeparam name="A"></typeparam>
    /// <returns></returns>
    public static Iterator<A> from<A>(IEnumerable<A> enumerable) =>
        new Iterator<A>.ConsFirst(enumerable);

    /// <summary>
    /// Construct a singleton sequence 
    /// </summary>
    /// <param name="head">Head item</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Iterator</returns>
    public static Iterator<A> singleton<A>(A head) =>
        new Iterator<A>.ConsValue(head, Iterator<A>.Nil.Default);

    /// <summary>
    /// Construct a sequence from a head item and a tail sequence
    /// </summary>
    /// <param name="head">Head item</param>
    /// <param name="tail">Tail sequences</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Iterator</returns>
    public static Iterator<A> Cons<A>(A head, Iterator<A> tail) =>
        new Iterator<A>.ConsValue(head, tail);

    /// <summary>
    /// Empty sequence
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Iterator</returns>
    public static Iterator<A> Nil<A>() =>
        Iterator<A>.Nil.Default;
}
