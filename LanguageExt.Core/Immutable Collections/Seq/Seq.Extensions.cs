#pragma warning disable CS0693 // Type parameter has the same name as the type parameter from outer type

using LanguageExt.ClassInstances;
using LanguageExt.Traits;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.CompilerServices;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static partial class SeqExtensions
{
    public static Seq<A> As<A>(this K<Seq, A> xs) =>
        (Seq<A>)xs;
    
    /// <summary>
    /// Monadic join
    /// </summary>
    [Pure]
    public static Seq<A> Flatten<A>(this Seq<Seq<A>> ma) =>
        ma.Bind(identity);

    /// <summary>
    /// Applies the given function 'selector' to each element of the sequence. Returns the sequence 
    /// comprised of the results for each element where the function returns Some(f(x)).
    /// </summary>
    /// <typeparam name="A">sequence item type</typeparam>
    /// <param name="list">sequence</param>
    /// <param name="selector">Selector function</param>
    /// <returns>Mapped and filtered sequence</returns>
    [Pure]
    public static Seq<B> Choose<A, B>(this Seq<A> list, Func<A, Option<B>> selector) =>
        Seq.choose(list, selector);

    /// <summary>
    /// Applies the given function 'selector' to each element of the sequence. Returns the 
    /// sequence comprised of the results for each element where the function returns Some(f(x)).
    /// An index value is passed through to the selector function also.
    /// </summary>
    /// <typeparam name="A">sequence item type</typeparam>
    /// <param name="list">sequence</param>
    /// <param name="selector">Selector function</param>
    /// <returns>Mapped and filtered sequence</returns>
    [Pure]
    public static Seq<B> Choose<A, B>(this Seq<A> list, Func<int, A, Option<B>> selector) =>
        Seq.choose(list, selector);

    /// <summary>
    /// Reverses the sequence (Reverse in LINQ)
    /// </summary>
    /// <typeparam name="T">sequence item type</typeparam>
    /// <param name="list">sequence to reverse</param>
    /// <returns>Reversed sequence</returns>
    [Pure]
    public static Seq<T> Rev<T>(this Seq<T> list) =>
        Seq.rev(list);

    /// <summary>
    /// Concatenate two sequences (Concat in LINQ)
    /// </summary>
    /// <typeparam name="T">sequence item type</typeparam>
    /// <param name="lhs">First sequence</param>
    /// <param name="rhs">Second sequence</param>
    /// <returns>Concatenated sequence</returns>
    [Pure]
    public static Seq<T> Append<T>(this Seq<T> lhs, Seq<T> rhs) =>
        Seq.append(lhs, rhs);

    /// <summary>
    /// Concatenate a sequence and a sequence of sequences
    /// </summary>
    /// <typeparam name="T">List item type</typeparam>
    /// <param name="lhs">First list</param>
    /// <param name="rhs">Second list</param>
    /// <returns>Concatenated list</returns>
    [Pure]
    public static Seq<T> Append<T>(this Seq<T> x, Seq<Seq<T>> xs) =>
        Seq.append(x, xs);

    /// <summary>
    /// Applies a function to each element of the sequence, threading an accumulator argument 
    /// through the computation. This function takes the state argument, and applies the function 
    /// to it and the first element of the sequence. Then, it passes this result into the function 
    /// along with the second element, and so on. Finally, it returns the list of intermediate 
    /// results and the final result.
    /// </summary>
    /// <typeparam name="S">State type</typeparam>
    /// <typeparam name="T">sequence item type</typeparam>
    /// <param name="list">sequence to fold</param>
    /// <param name="state">Initial state</param>
    /// <param name="folder">Folding function</param>
    /// <returns>Aggregate state</returns>
    [Pure]
    public static Seq<S> Scan<S, T>(this Seq<T> list, S state, Func<S, T, S> folder) =>
        Seq.scan(list, state, folder);

    /// <summary>
    /// Applies a function to each element of the sequence (from last element to first), 
    /// threading an accumulator argument through the computation. This function takes the state 
    /// argument, and applies the function to it and the first element of the sequence. Then, it 
    /// passes this result into the function along with the second element, and so on. Finally, 
    /// it returns the list of intermediate results and the final result.
    /// </summary>
    /// <typeparam name="S">State type</typeparam>
    /// <typeparam name="T">Enumerable item type</typeparam>
    /// <param name="list">Enumerable to fold</param>
    /// <param name="state">Initial state</param>
    /// <param name="folder">Folding function</param>
    /// <returns>Aggregate state</returns>
    [Pure]
    public static Seq<S> ScanBack<S, T>(this Seq<T> list, S state, Func<S, T, S> folder) =>
        Seq.scanBack(list, state, folder);

    /// <summary>
    /// Joins two sequences together either into a single sequence using the join 
    /// function provided
    /// </summary>
    /// <param name="list">First sequence to join</param>
    /// <param name="other">Second sequence to join</param>
    /// <param name="zipper">Join function</param>
    /// <returns>Joined sequence</returns>
    [Pure]
    public static Seq<V> Zip<T, U, V>(this Seq<T> list, Seq<U> other, Func<T, U, V> zipper) =>
        toSeq(Enumerable.Zip(list, other, zipper));

    /// <summary>
    /// Joins two sequences together either into an sequence of tuples
    /// </summary>
    /// <param name="list">First sequence to join</param>
    /// <param name="other">Second sequence to join</param>
    /// <param name="zipper">Join function</param>
    /// <returns>Joined sequence of tuples</returns>
    [Pure]
    public static Seq<(T First, U Second)> Zip<T, U>(this Seq<T> list, Seq<U> other) =>
        toSeq(Enumerable.Zip(list, other, (t, u) => (t, u)));

    /// <summary>
    /// Return a new sequence with all duplicate values removed
    /// </summary>
    /// <typeparam name="T">sequence item type</typeparam>
    /// <param name="list">sequence</param>
    /// <returns>A new sequence with all duplicate values removed</returns>
    [Pure]
    public static Seq<T> Distinct<T>(this Seq<T> list) =>
        toSeq(Enumerable.Distinct(list));

    /// <summary>
    /// Return a new sequence with all duplicate values removed
    /// </summary>
    /// <typeparam name="T">sequence item type</typeparam>
    /// <param name="list">sequence</param>
    /// <returns>A new sequence with all duplicate values removed</returns>
    [Pure]
    public static Seq<T> Distinct<EQ, T>(this Seq<T> list) where EQ : Eq<T> =>
        toSeq(Enumerable.Distinct(list, new EqCompare<T>(static (x, y) => EQ.Equals(x, y), static x => EQ.GetHashCode(x))));

    /// <summary>
    /// Return a new sequence with all duplicate values removed
    /// </summary>
    /// <typeparam name="T">sequence item type</typeparam>
    /// <param name="list">sequence</param>
    /// <returns>A new sequence with all duplicate values removed</returns>
    [Pure]
    public static Seq<T> Distinct<T, K>(this Seq<T> list, Func<T, K> keySelector, Option<Func<K, K, bool>> compare = default) =>
        toSeq(Enumerable.Distinct(list, 
            new EqCompare<T>(
                (a, b) => compare.IfNone(EqDefault<K>.Equals)(keySelector(a), keySelector(b)), 
                a => compare.Match(Some: _  => 0, None: () => EqDefault<K>.GetHashCode(keySelector(a))))));

    /// <summary>
    /// The tails function returns all final segments of the argument, longest first. For example:
    /// 
    ///     tails(['a','b','c']) == [['a','b','c'], ['b','c'], ['c'],[]]
    /// </summary>
    /// <typeparam name="A">Seq item type</typeparam>
    /// <param name="self">Seq</param>
    /// <returns>Seq of Seq of A</returns>
    [Pure]
    public static Seq<Seq<A>> Tails<A>(this Seq<A> self) =>
        Seq.tails(self);

    /// <summary>
    /// Span, applied to a predicate 'pred' and a list, returns a tuple where first element is 
    /// longest prefix (possibly empty) of elements that satisfy 'pred' and second element is the 
    /// remainder of the list:
    /// </summary>
    /// <example>
    /// Seq.span(List(1,2,3,4,1,2,3,4), x => x 〈 3) == (List(1,2),List(3,4,1,2,3,4))
    /// </example>
    /// <example>
    /// Seq.span(List(1,2,3), x => x 〈 9) == (List(1,2,3),List())
    /// </example>
    /// <example>
    /// Seq.span(List(1,2,3), x => x 〈 0) == (List(),List(1,2,3))
    /// </example>
    /// <typeparam name="T">List element type</typeparam>
    /// <param name="self">List</param>
    /// <param name="pred">Predicate</param>
    /// <returns>Split list</returns>
    [Pure]
    public static (Seq<T>, Seq<T>) Span<T>(this Seq<T> self, Func<T, bool> pred) =>
        Seq.span(self, pred);
 
    /// <summary>
    /// Convert to a queryable 
    /// </summary>
    /// <returns></returns>
    [Pure]
    public static IQueryable<A> AsQueryable<A>(this Seq<A> source) =>
        // NOTE TO FUTURE ME: Don't delete this thinking it's not needed!
        source.Value.AsQueryable();
}
