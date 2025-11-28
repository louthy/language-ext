#pragma warning disable CS0693 // Type parameter has the same name as the type parameter from outer type

using System;
using System.Linq;
using System.Collections.Generic;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using System.Text;
using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class LstExtensions
{
    public static Lst<A> As<A>(this K<Lst, A> xs) =>
        (Lst<A>)xs;

    /// <summary>
    /// Accesses the head of an enumerable and yields the remainder without multiple-enumerations
    /// </summary>
    /// <exception cref="ExpectedException">Throws if sequence is empty</exception>
    public static (A Head, IEnumerable<A> Tail) HeadAndTail<A>(this IEnumerable<A> ma) =>
        ma.HeadAndTailSafe()
          .IfNone(() => throw Exceptions.SequenceEmpty);
    
    /// <summary>
    /// Accesses the head of an enumerable and yields the remainder without multiple-enumerations
    /// </summary>
    public static Option<(A Head, IEnumerable<A> Tail)> HeadAndTailSafe<A>(this IEnumerable<A> ma)
    {
        var iter = ma.GetEnumerator();
        A head;
        if (iter.MoveNext())
        {
            head = iter.Current;
        }
        else
        {
            iter.Dispose();
            return None;
        }
        return Some((head, tail(iter)));

        static IEnumerable<A> tail(IEnumerator<A> rest)
        {
            try
            {
                while (rest.MoveNext())
                {
                    yield return rest.Current;
                }
            }
            finally
            {
                rest.Dispose();
            }
        }
    }
    
    /// <summary>
    /// Monadic join
    /// </summary>
    [Pure]
    public static Lst<A> Flatten<A>(this Lst<Lst<A>> ma) =>
        ma.Bind(identity);

    /// <summary>
    /// Monadic join
    /// </summary>
    [Pure]
    public static IEnumerable<A> Flatten<A>(this IEnumerable<IEnumerable<A>> ma) =>
        ma.Bind(identity);

    /// <summary>
    /// Match empty list, or multi-item list
    /// </summary>
    /// <typeparam name="B">Return value type</typeparam>
    /// <param name="Empty">Match for an empty list</param>
    /// <param name="More">Match for a non-empty</param>
    /// <returns>Result of match function invoked</returns>
    public static B Match<A, B>(this IEnumerable<A> list,
        Func<B> Empty,
        Func<Seq<A>, B> More) =>
        toSeq(list).Match(Empty, More);

    /// <summary>
    /// List pattern matching
    /// </summary>
    [Pure]
    public static B Match<A, B>(this IEnumerable<A> list,
        Func<B> Empty,
        Func<A, Seq<A>, B> More) =>
        toSeq(list).Match(Empty, More);

    /// <summary>
    /// List pattern matching
    /// </summary>
    [Pure]
    public static R Match<T, R>(this IEnumerable<T> list,
        Func<R> Empty,
        Func<T, R> One,
        Func<T, Seq<T>, R> More ) =>
        toSeq(list).Match(Empty, One, More);

    /// <summary>
    /// Get all items in the list except the last one
    /// </summary>
    /// <remarks>
    /// Must evaluate the last item to know it's the last, but won't return it
    /// </remarks>
    /// <param name="list">List</param>
    /// <returns>The initial items (all but the last)</returns>
    [Pure]
    public static IEnumerable<A> Init<A>(this IEnumerable<A> list) =>
        List.init(list);

    /// <summary>
    /// Get the tail of the list (skips the head item)
    /// </summary>
    /// <param name="list">List</param>
    /// <returns>Enumerable of T</returns>
    [Pure]
    public static Iterable<T> Tail<T>(this IEnumerable<T> list) =>
        List.tail(list);

    /// <summary>
    /// Inject a value in between each item in the enumerable 
    /// </summary>
    /// <param name="ma">Enumerable to inject values into</param>
    /// <param name="value">Item to inject</param>
    /// <typeparam name="A">Bound type</typeparam>
    /// <returns>An enumerable with the values injected</returns>
    [Pure]
    public static IEnumerable<A> Intersperse<A>(this IEnumerable<A> ma, A value)
    {
        var isFirst = true;
        foreach(var item in ma)
        {
            if (!isFirst)
            {
                yield return value;
            }

            yield return item;
            isFirst = false;
        }
    }

    /// <summary>
    /// Concatenate all strings into one
    /// </summary>
    [Pure]
    public static string Concat(this IEnumerable<string> xs)
    {
        var sb = new StringBuilder();
        foreach (var x in xs)
        {
            sb.Append(x);
        }

        return sb.ToString();
    }

    /// <summary>
    /// Reverses the list (Reverse in LINQ)
    /// </summary>
    /// <typeparam name="A">List item type</typeparam>
    /// <param name="list">Listto reverse</param>
    /// <returns>Reversed list</returns>
    [Pure]
    public static Lst<A> Rev<A>(this Lst<A> list) =>
        List.rev(list);

    /// <summary>
    /// Applies a function to each element of the collection (from last element to first), threading 
    /// an accumulator argument through the computation. This function first applies the function 
    /// to the first two elements of the list. Then, it passes this result into the function along 
    /// with the third element and so on. Finally, it returns the final result.
    /// </summary>
    /// <typeparam name="T">Enumerable item type</typeparam>
    /// <param name="list">Enumerable to reduce</param>
    /// <param name="reducer">Reduce function</param>
    /// <returns>Aggregate value</returns>
    [Pure]
    public static T Reduce<T>(this IEnumerable<T> list, Func<T, T, T> reducer) =>
        List.reduce(list, reducer);

    /// <summary>
    /// Applies a function to each element of the collection, threading an accumulator argument 
    /// through the computation. This function first applies the function to the first two 
    /// elements of the list. Then, it passes this result into the function along with the third 
    /// element and so on. Finally, it returns the final result.
    /// </summary>
    /// <typeparam name="T">Enumerable item type</typeparam>
    /// <param name="list">Enumerable to reduce</param>
    /// <param name="reducer">Reduce function</param>
    /// <returns>Aggregate value</returns>
    [Pure]
    public static T ReduceBack<T>(this IEnumerable<T> list, Func<T, T, T> reducer) =>
        List.reduceBack(list, reducer);

    /// <summary>
    /// Applies a function to each element of the collection, threading an accumulator argument 
    /// through the computation. This function takes the state argument, and applies the function 
    /// to it and the first element of the list. Then, it passes this result into the function 
    /// along with the second element, and so on. Finally, it returns the list of intermediate 
    /// results and the final result.
    /// </summary>
    /// <typeparam name="S">State type</typeparam>
    /// <typeparam name="T">Enumerable item type</typeparam>
    /// <param name="list">Enumerable to fold</param>
    /// <param name="state">Initial state</param>
    /// <param name="folder">Folding function</param>
    /// <returns>Aggregate state</returns>
    [Pure]
    public static IEnumerable<S> Scan<S, T>(this IEnumerable<T> list, S state, Func<S, T, S> folder) =>
        List.scan(list, state, folder);

    /// <summary>
    /// Applies a function to each element of the collection (from last element to first), 
    /// threading an accumulator argument through the computation. This function takes the state 
    /// argument, and applies the function to it and the first element of the list. Then, it 
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
    public static IEnumerable<S> ScanBack<S, T>(this IEnumerable<T> list, S state, Func<S, T, S> folder) =>
        List.scanBack(list, state, folder);

    /// <summary>
    /// Iterate each item in the enumerable in order (consume items)
    /// </summary>
    /// <typeparam name="T">Enumerable item type</typeparam>
    /// <param name="list">Enumerable to consume</param>
    /// <returns>Unit</returns>
    public static Unit Consume<T>(this IEnumerable<T> list) =>
        List.consume(list);

    /// <summary>
    /// Return a new enumerable with all duplicate values removed
    /// </summary>
    /// <typeparam name="T">Enumerable item type</typeparam>
    /// <param name="list">Enumerable</param>
    /// <returns>A new enumerable with all duplicate values removed</returns>
    [Pure]
    public static IEnumerable<T> Distinct<EQ, T>(this IEnumerable<T> list) where EQ : Eq<T> =>
        List.distinct<EQ, T>(list);

    /// <summary>
    /// The tails function returns all final segments of the argument, longest first. For example,
    ///  i.e. tails(['a','b','c']) == [['a','b','c'], ['b','c'], ['c'],[]]
    /// </summary>
    /// <typeparam name="T">List item type</typeparam>
    /// <param name="self">List</param>
    /// <returns>Enumerable of Enumerables of T</returns>
    [Pure]
    public static IEnumerable<IEnumerable<T>> Tails<T>(this IEnumerable<T> self) =>
        List.tails(self);

    /// <summary>
    /// Span, applied to a predicate 'pred' and a list, returns a tuple where first element is 
    /// longest prefix (possibly empty) of elements that satisfy 'pred' and second element is the 
    /// remainder of the list:
    /// </summary>
    /// <example>
    /// List.span(List(1,2,3,4,1,2,3,4), x => x 〈 3) == (List(1,2),List(3,4,1,2,3,4))
    /// </example>
    /// <example>
    /// List.span(List(1,2,3), x => x 〈 9) == (List(1,2,3),List())
    /// </example>
    /// <example>
    /// List.span(List(1,2,3), x => x 〈 0) == (List(),List(1,2,3))
    /// </example>
    /// <typeparam name="T">List element type</typeparam>
    /// <param name="self">List</param>
    /// <param name="pred">Predicate</param>
    /// <returns>Split list</returns>
    [Pure]
    public static (IEnumerable<T>, IEnumerable<T>) Span<T>(this IEnumerable<T> self, Func<T, bool> pred) =>
        List.span(self, pred);

    /// <summary>
    /// Monadic bind function for IEnumerable
    /// </summary>
    [Pure]
    public static IEnumerable<R> Bind<T, R>(this IEnumerable<T> self, Func<T, IEnumerable<R>> binder) =>
        EnumerableOptimal.BindFast(self, binder);

    /// <summary>
    /// LINQ Select implementation for Lst
    /// </summary>
    [Pure]
    public static Lst<B> Select<A, B>(this Lst<A> self, Func<A, B> map) =>
        new (self.AsIterable().Select(map));

    /// <summary>
    /// Monadic bind function for Lst that returns an IEnumerable
    /// </summary>
    [Pure]
    public static IEnumerable<B> BindEnumerable<A, B>(this Lst<A> self, Func<A, Lst<B>> binder) =>
        EnumerableOptimal.BindFast(self, binder);

    /// <summary>
    /// Monadic bind function
    /// </summary>
    [Pure]
    public static Lst<B> Bind<A, B>(this Lst<A> self, Func<A, Lst<B>> binder) =>
        new (self.BindEnumerable(binder));

    /// <summary>
    /// Monadic bind function
    /// </summary>
    [Pure]
    public static Lst<B> Bind<A, B>(this Lst<A> self, Func<A, K<Lst, B>> binder) =>
        Bind(self, x => binder(x).As());

    /// <summary>
    /// Returns the number of items in the Lst T
    /// </summary>
    /// <typeparam name="A">Item type</typeparam>
    /// <param name="list">List to count</param>
    /// <returns>The number of items in the list</returns>
    [Pure]
    public static int Count<A>(this Lst<A> self) =>
        self.Count;

    /// <summary>
    /// LINQ bind implementation for Lst
    /// </summary>
    [Pure]
    public static Lst<C> SelectMany<A, B, C>(this Lst<A> self, Func<A, Lst<B>> bind, Func<A, B, C> project) =>
        self.Bind(t => bind(t).Map(u => project(t, u)));

    /// <summary>
    /// Take all but the last item in an enumerable
    /// </summary>
    /// <typeparam name="T">Bound value type</typeparam>
    public static IEnumerable<T> SkipLast<T>(this IEnumerable<T> self)
    {
        using var iter = self.GetEnumerator();
        bool remaining;
        var first = true;
        T? item = default;

        do
        {
            remaining = iter.MoveNext();
            if (remaining)
            {
                if (!first) yield return item!;
                item = iter.Current;
                first = false;
            }
        } while (remaining);
    }

    /// <summary>
    /// Take all but the last n items in an enumerable
    /// </summary>
    /// <typeparam name="T">Bound value type</typeparam>
    public static IEnumerable<T> SkipLast<T>(this IEnumerable<T> self, int n)
    {
        using var iter = self.GetEnumerator();
        bool remaining ;
        var cache = new Queue<T>(n + 1);

        do
        {
            remaining = iter.MoveNext();
            if (remaining)
            {
                cache.Enqueue(iter.Current);
                if (cache.Count > n) yield return cache.Dequeue();
            }
        } while (remaining);
    }

    /// <summary>
    /// Convert the enumerable to an Option.  
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="self">This</param>
    /// <returns>If enumerable is empty then return None, else Some(head)</returns>
    public static Option<A> ToOption<A>(this IEnumerable<A> self) =>
        self.Match(
            ()     => Option<A>.None,
            (x, _) => Option.Some(x));
    
    /// <summary>
    /// Convert to a queryable 
    /// </summary>
    [Pure]
    public static IQueryable<A> AsQueryable<A>(this Lst<A> source) =>
        // NOTE TO FUTURE ME: Don't delete this thinking it's not needed!
        source.Value.AsQueryable();
}
