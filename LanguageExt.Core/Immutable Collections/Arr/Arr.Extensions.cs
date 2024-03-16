using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using LanguageExt.ClassInstances;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static class ArrExtensions
{
    public static Arr<A> As<A>(this K<Arr, A> xs) =>
        (Arr<A>)xs;
    
    /// <summary>
    /// Monadic join
    /// </summary>
    [Pure]
    public static A[] Flatten<A>(this A[][] ma) =>
        ma.Bind(identity).ToArray();

    /// <summary>
    /// Monadic join
    /// </summary>
    [Pure]
    public static Arr<A> Flatten<A>(this Arr<Arr<A>> ma) =>
        ma.Bind(identity);

    /// <summary>
    /// Returns the element at a specified index in a sequence.
    /// </summary>
    [Pure]
    public static T ElementAt<T>(this Arr<T> source, int index) =>
        source.Value.ElementAt(index);

    /// <summary>
    /// Returns the element at a specified index in a sequence or a default value if the index is out of range.
    /// </summary>
    [Pure]
    public static T? ElementAtOrDefault<T>(this Arr<T> source, int index) =>
        source.Value.ElementAtOrDefault(index);

    /// <summary>
    /// Produces the set difference of two sequences by using the default equality comparer to compare values.
    /// </summary>
    [Pure]
    public static IEnumerable<T> Except<T>(this Arr<T> first, IEnumerable<T> second) =>
        first.Value.Except(second);

    /// <summary>
    /// Produces the set difference of two sequences by using the specified IEqualityComparer<T> to compare values.
    /// </summary>
    [Pure]
    public static IEnumerable<T> Except<T>(this Arr<T> first, IEnumerable<T> second, IEqualityComparer<T> comparer) =>
        first.Value.Except(second, comparer);

    /// <summary>
    /// Returns the first element of a sequence.
    /// </summary>
    [Pure]
    public static T First<T>(this Arr<T> source) =>
        source.Value.First();

    /// <summary>
    /// Returns the first element in a sequence that satisfies a specified condition.
    /// </summary>
    [Pure]
    public static T First<T>(this Arr<T> source, Func<T, bool> predicate) =>
        source.Value.First(predicate);

    /// <summary>
    /// Returns the first element of a sequence, or a default value if the sequence contains no elements.
    /// </summary>
    [Pure]
    public static T? FirstOrDefault<T>(this Arr<T> source) =>
        source.Value.FirstOrDefault();

    /// <summary>
    /// Returns the first element of the sequence that satisfies a condition or a default value if no such element is found.
    /// </summary>
    [Pure]
    public static T? FirstOrDefault<T>(this Arr<T> source, Func<T?, bool> predicate) =>
        source.Value.FirstOrDefault(predicate);

    /// <summary>
    /// Groups the elements of a sequence according to a specified key selector function and projects the elements for each group by using a specified function.
    /// </summary>
    [Pure]
    public static IEnumerable<IGrouping<TKey, TElement>> GroupBy<T, TKey, TElement>(this Arr<T> source, Func<T, TKey> keySelector, Func<T, TElement> elementSelector) =>
        source.Value.GroupBy(keySelector, elementSelector);

    /// <summary>
    /// Groups the elements of a sequence according to a key selector function. The keys are compared by using a comparer and each group's elements are projected by using a specified function.
    /// </summary>
    [Pure]
    public static IEnumerable<IGrouping<TKey, TElement>> GroupBy<T, TKey, TElement>(this Arr<T> source, Func<T, TKey> keySelector, Func<T, TElement> elementSelector, IEqualityComparer<TKey> comparer) =>
        source.Value.GroupBy(keySelector, elementSelector, comparer);

    /// <summary>
    /// Groups the elements of a sequence according to a specified key selector function.
    /// </summary>
    [Pure]
    public static IEnumerable<IGrouping<TKey, T>> GroupBy<T, TKey>(this Arr<T> source, Func<T, TKey> keySelector) =>
        source.Value.GroupBy(keySelector);

    /// <summary>
    /// Groups the elements of a sequence according to a specified key selector function and compares the keys by using a specified comparer.
    /// </summary>
    [Pure]
    public static IEnumerable<IGrouping<TKey, T>> GroupBy<T, TKey>(this Arr<T> source, Func<T, TKey> keySelector, IEqualityComparer<TKey> comparer) =>
        source.Value.GroupBy(keySelector, comparer);

    /// <summary>
    /// Groups the elements of a sequence according to a specified key selector function and creates a result value from each group and its key. The elements of each group are projected by using a specified function.
    /// </summary>
    [Pure]
    public static IEnumerable<R> GroupBy<T, TKey, TElement, R>(this Arr<T> source, Func<T, TKey> keySelector, Func<T, TElement> elementSelector, Func<TKey, IEnumerable<TElement>, R> resultSelector) =>
        source.Value.GroupBy(keySelector, elementSelector, resultSelector);

    /// <summary>
    /// Groups the elements of a sequence according to a specified key selector function and creates a result value from each group and its key. Key values are compared by using a specified comparer, and the elements of each group are projected by using a specified function.
    /// </summary>
    [Pure]
    public static IEnumerable<R> GroupBy<T, TKey, TElement, R>(this Arr<T> source, Func<T, TKey> keySelector, Func<T, TElement> elementSelector, Func<TKey, IEnumerable<TElement>, R> resultSelector, IEqualityComparer<TKey> comparer) =>
        source.Value.GroupBy(keySelector, elementSelector, resultSelector, comparer);

    /// <summary>
    /// Groups the elements of a sequence according to a specified key selector function and creates a result value from each group and its key.
    /// </summary>
    [Pure]
    public static IEnumerable<R> GroupBy<T, TKey, R>(this Arr<T> source, Func<T, TKey> keySelector, Func<TKey, IEnumerable<T>, R> resultSelector) =>
        source.Value.GroupBy(keySelector, resultSelector);

    /// <summary>
    /// Groups the elements of a sequence according to a specified key selector function and creates a result value from each group and its key. The keys are compared by using a specified comparer.
    /// </summary>
    [Pure]
    public static IEnumerable<R> GroupBy<T, TKey, R>(this Arr<T> source, Func<T, TKey> keySelector, Func<TKey, IEnumerable<T>, R> resultSelector, IEqualityComparer<TKey> comparer) =>
        source.Value.GroupBy(keySelector, resultSelector, comparer);

    /// <summary>
    /// Correlates the elements of two sequences based on equality of keys and groups the results. The default equality comparer is used to compare keys.
    /// </summary>
    [Pure]
    public static IEnumerable<R> GroupJoin<TOuter, TInner, TKey, R>(this Arr<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, R> resultSelector) =>
        outer.Value.GroupJoin(inner, outerKeySelector, innerKeySelector, resultSelector);

    /// <summary>
    /// Correlates the elements of two sequences based on key equality and groups the results. A specified IEqualityComparer<T> is used to compare keys.
    /// </summary>
    [Pure]
    public static IEnumerable<R> GroupJoin<TOuter, TInner, TKey, R>(this Arr<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, R> resultSelector, IEqualityComparer<TKey> comparer) =>
        outer.Value.GroupJoin(inner, outerKeySelector, innerKeySelector, resultSelector, comparer);

    /// <summary>
    /// Produces the set intersection of two sequences by using the default equality comparer to compare values.
    /// </summary>
    [Pure]
    public static IEnumerable<T> Intersect<T>(this Arr<T> first, IEnumerable<T> second) =>
        first.Value.Intersect(second);

    /// <summary>
    /// Produces the set intersection of two sequences by using the specified IEqualityComparer<T> to compare values.
    /// </summary>
    [Pure]
    public static IEnumerable<T> Intersect<T>(this Arr<T> first, IEnumerable<T> second, IEqualityComparer<T> comparer) =>
        first.Value.Intersect(second, comparer);

    /// <summary>
    /// Correlates the elements of two sequences based on matching keys. The default equality comparer is used to compare keys.
    /// </summary>
    [Pure]
    public static IEnumerable<R> Join<TOuter, TInner, TKey, R>(this Arr<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, R> resultSelector) =>
        outer.Value.Join(inner, outerKeySelector, innerKeySelector, resultSelector);

    /// <summary>
    /// Correlates the elements of two sequences based on matching keys. A specified IEqualityComparer<T> is used to compare keys.
    /// </summary>
    [Pure]
    public static IEnumerable<R> Join<TOuter, TInner, TKey, R>(this Arr<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, R> resultSelector, IEqualityComparer<TKey> comparer) =>
        outer.Value.Join(inner, outerKeySelector, innerKeySelector, resultSelector, comparer);

    /// <summary>
    /// Returns the last element of a sequence.
    /// </summary>
    [Pure]
    public static T Last<T>(this Arr<T> source) =>
        source.Value.Last();

    /// <summary>
    /// Returns the last element of a sequence that satisfies a specified condition.
    /// </summary>
    [Pure]
    public static T Last<T>(this Arr<T> source, Func<T, bool> predicate) =>
        source.Value.Last(predicate);

    /// <summary>
    /// Returns the last element of a sequence, or a default value if the sequence contains no elements.
    /// </summary>
    [Pure]
    public static T? LastOrDefault<T>(this Arr<T> source) =>
        source.Value.LastOrDefault();

    /// <summary>
    /// Returns the last element of a sequence that satisfies a condition or a default value if no such element is found.
    /// </summary>
    [Pure]
    public static T? LastOrDefault<T>(this Arr<T> source, Func<T?, bool> predicate) =>
        Enumerable.LastOrDefault(source.Value, predicate);

    /// <summary>
    /// Returns an Int64 that represents the total number of elements in a sequence.
    /// </summary>
    [Pure]
    public static long LongCount<T>(this Arr<T> source) =>
        source.Value.LongCount();

    /// <summary>
    /// Returns an Int64 that represents how many elements in a sequence satisfy a condition.
    /// </summary>
    [Pure]
    public static long LongCount<T>(this Arr<T> source, Func<T, bool> predicate) =>
        source.Value.LongCount(predicate);

    /// <summary>
    /// Returns the maximum value in a sequence of Decimal values.
    /// </summary>
    [Pure]
    public static decimal Max(this Arr<decimal> source) =>
        source.Value.Max();

    /// <summary>
    /// Invokes a transform function on each element of a sequence and returns the maximum Decimal value.
    /// </summary>
    [Pure]
    public static decimal Max<T>(this Arr<T> source, Func<T, decimal> selector) =>
        source.Value.Max(selector);

    /// <summary>
    /// Returns the maximum value in a sequence of nullable Decimal values.
    /// </summary>
    [Pure]
    public static decimal? Max(this Arr<decimal?> source) =>
        source.Value.Max();

    /// <summary>
    /// Invokes a transform function on each element of a sequence and returns the maximum nullable Decimal value.
    /// </summary>
    [Pure]
    public static decimal? Max<T>(this Arr<T> source, Func<T, decimal?> selector) =>
        source.Value.Max(selector);

    /// <summary>
    /// Returns the maximum value in a sequence of Double values.
    /// </summary>
    [Pure]
    public static double Max(this Arr<double> source) =>
        source.Value.Max();

    /// <summary>
    /// Invokes a transform function on each element of a sequence and returns the maximum Double value.
    /// </summary>
    [Pure]
    public static double Max<T>(this Arr<T> source, Func<T, double> selector) =>
        source.Value.Max(selector);

    /// <summary>
    /// Returns the maximum value in a sequence of nullable Double values.
    /// </summary>
    [Pure]
    public static double? Max(this Arr<double?> source) =>
        source.Value.Max();

    /// <summary>
    /// Invokes a transform function on each element of a sequence and returns the maximum nullable Double value.
    /// </summary>
    [Pure]
    public static double? Max<T>(this Arr<T> source, Func<T, double?> selector) =>
        source.Value.Max(selector);

    /// <summary>
    /// Returns the maximum value in a sequence of Single values.
    /// </summary>
    [Pure]
    public static float Max(this Arr<float> source) =>
        source.Value.Max();

    /// <summary>
    /// Invokes a transform function on each element of a sequence and returns the maximum Single value.
    /// </summary>
    [Pure]
    public static float Max<T>(this Arr<T> source, Func<T, float> selector) =>
        source.Value.Max(selector);

    /// <summary>
    /// Returns the maximum value in a sequence of nullable Single values.
    /// </summary>
    [Pure]
    public static float? Max(this Arr<float?> source) =>
        source.Value.Max();

    /// <summary>
    /// Invokes a transform function on each element of a sequence and returns the maximum nullable Single value.
    /// </summary>
    [Pure]
    public static float? Max<T>(this Arr<T> source, Func<T, float?> selector) =>
        source.Value.Max(selector);

    /// <summary>
    /// Returns the maximum value in a sequence of Int32 values.
    /// </summary>
    [Pure]
    public static int Max(this Arr<int> source) =>
        source.Value.Max();

    /// <summary>
    /// Invokes a transform function on each element of a sequence and returns the maximum Int32 value.
    /// </summary>
    [Pure]
    public static int Max<T>(this Arr<T> source, Func<T, int> selector) =>
        source.Value.Max(selector);

    /// <summary>
    /// Returns the maximum value in a sequence of nullable Int32 values.
    /// </summary>
    [Pure]
    public static int? Max(this Arr<int?> source) =>
        source.Value.Max();

    /// <summary>
    /// Invokes a transform function on each element of a sequence and returns the maximum nullable Int32 value.
    /// </summary>
    [Pure]
    public static int? Max<T>(this Arr<T> source, Func<T, int?> selector) =>
        source.Value.Max(selector);

    /// <summary>
    /// Returns the maximum value in a sequence of Int64 values.
    /// </summary>
    [Pure]
    public static long Max(this Arr<long> source) =>
        source.Value.Max();

    /// <summary>
    /// Invokes a transform function on each element of a sequence and returns the maximum Int64 value.
    /// </summary>
    [Pure]
    public static long Max<T>(this Arr<T> source, Func<T, long> selector) =>
        source.Value.Max(selector);

    /// <summary>
    /// Returns the maximum value in a sequence of nullable Int64 values.
    /// </summary>
    [Pure]
    public static long? Max(this Arr<long?> source) =>
        source.Value.Max();

    /// <summary>
    /// Invokes a transform function on each element of a sequence and returns the maximum nullable Int64 value.
    /// </summary>
    [Pure]
    public static long? Max<T>(this Arr<T> source, Func<T, long?> selector) =>
        source.Value.Max(selector);

    /// <summary>
    /// Invokes a transform function on each element of a generic sequence and returns the maximum resulting value.
    /// </summary>
    [Pure]
    public static R? Max<T, R>(this Arr<T> source, Func<T, R> selector) =>
        source.Value.Max(selector);

    /// <summary>
    /// Returns the maximum value in a generic sequence.
    /// </summary>
    [Pure]
    public static T? Max<T>(this Arr<T> source) =>
        source.Value.Max();

    /// <summary>
    /// Returns the minimum value in a sequence of Decimal values.
    /// </summary>
    [Pure]
    public static decimal Min(this Arr<decimal> source) =>
        source.Value.Min();

    /// <summary>
    /// Invokes a transform function on each element of a sequence and returns the minimum Decimal value.
    /// </summary>
    [Pure]
    public static decimal Min<T>(this Arr<T> source, Func<T, decimal> selector) =>
        source.Value.Min(selector);

    /// <summary>
    /// Returns the minimum value in a sequence of nullable Decimal values.
    /// </summary>
    [Pure]
    public static decimal? Min(this Arr<decimal?> source) =>
        source.Value.Min();

    /// <summary>
    /// Invokes a transform function on each element of a sequence and returns the minimum nullable Decimal value.
    /// </summary>
    [Pure]
    public static decimal? Min<T>(this Arr<T> source, Func<T, decimal?> selector) =>
        source.Value.Min(selector);

    /// <summary>
    /// Returns the minimum value in a sequence of Double values.
    /// </summary>
    [Pure]
    public static double Min(this Arr<double> source) =>
        source.Value.Min();

    /// <summary>
    /// Invokes a transform function on each element of a sequence and returns the minimum Double value.
    /// </summary>
    [Pure]
    public static double Min<T>(this Arr<T> source, Func<T, double> selector) =>
        source.Value.Min(selector);

    /// <summary>
    /// Returns the minimum value in a sequence of nullable Double values.
    /// </summary>
    [Pure]
    public static double? Min(this Arr<double?> source) =>
        source.Value.Min();

    /// <summary>
    /// Invokes a transform function on each element of a sequence and returns the minimum nullable Double value.
    /// </summary>
    [Pure]
    public static double? Min<T>(this Arr<T> source, Func<T, double?> selector) =>
        source.Value.Min(selector);

    /// <summary>
    /// Returns the minimum value in a sequence of Single values.
    /// </summary>
    [Pure]
    public static float Min(this Arr<float> source) =>
        source.Value.Min();

    /// <summary>
    /// Invokes a transform function on each element of a sequence and returns the minimum Single value.
    /// </summary>
    [Pure]
    public static float Min<T>(this Arr<T> source, Func<T, float> selector) =>
        source.Value.Min(selector);

    /// <summary>
    /// Returns the minimum value in a sequence of nullable Single values.
    /// </summary>
    [Pure]
    public static float? Min(this Arr<float?> source) =>
        source.Value.Min();

    /// <summary>
    /// Invokes a transform function on each element of a sequence and returns the minimum nullable Single value.
    /// </summary>
    [Pure]
    public static float? Min<T>(this Arr<T> source, Func<T, float?> selector) =>
        source.Value.Min(selector);

    /// <summary>
    /// Returns the minimum value in a sequence of Int32 values.
    /// </summary>
    [Pure]
    public static int Min(this Arr<int> source) =>
        source.Value.Min();

    /// <summary>
    /// Invokes a transform function on each element of a sequence and returns the minimum Int32 value.
    /// </summary>
    [Pure]
    public static int Min<T>(this Arr<T> source, Func<T, int> selector) =>
        source.Value.Min(selector);

    /// <summary>
    /// Returns the minimum value in a sequence of nullable Int32 values.
    /// </summary>
    [Pure]
    public static int? Min(this Arr<int?> source) =>
        source.Value.Min();

    /// <summary>
    /// Invokes a transform function on each element of a sequence and returns the minimum nullable Int32 value.
    /// </summary>
    [Pure]
    public static int? Min<T>(this Arr<T> source, Func<T, int?> selector) =>
        source.Value.Min(selector);

    /// <summary>
    /// Returns the minimum value in a sequence of Int64 values.
    /// </summary>
    [Pure]
    public static long Min(this Arr<long> source) =>
        source.Value.Min();

    /// <summary>
    /// Invokes a transform function on each element of a sequence and returns the minimum Int64 value.
    /// </summary>
    [Pure]
    public static long Min<T>(this Arr<T> source, Func<T, long> selector) =>
        source.Value.Min(selector);

    /// <summary>
    /// Returns the minimum value in a sequence of nullable Int64 values.
    /// </summary>
    [Pure]
    public static long? Min(this Arr<long?> source) =>
        source.Value.Min();

    /// <summary>
    /// Invokes a transform function on each element of a sequence and returns the minimum nullable Int64 value.
    /// </summary>
    [Pure]
    public static long? Min<T>(this Arr<T> source, Func<T, long?> selector) =>
        source.Value.Min(selector);

    /// <summary>
    /// Invokes a transform function on each element of a generic sequence and returns the minimum resulting value.
    /// </summary>
    [Pure]
    public static R? Min<T, R>(this Arr<T> source, Func<T, R> selector) =>
        source.Value.Min(selector);

    /// <summary>
    /// Returns the minimum value in a generic sequence.
    /// </summary>
    [Pure]
    public static T? Min<T>(this Arr<T> source) =>
        source.Value.Min();

    /// <summary>
    /// Sorts the elements of a sequence in ascending order according to a key.
    /// </summary>
    [Pure]
    public static IOrderedEnumerable<T> OrderBy<T, TKey>(this Arr<T> source, Func<T, TKey> keySelector) =>
        source.Value.OrderBy(keySelector);

    /// <summary>
    /// Sorts the elements of a sequence in ascending order by using a specified comparer.
    /// </summary>
    [Pure]
    public static IOrderedEnumerable<T> OrderBy<T, TKey>(this Arr<T> source, Func<T, TKey> keySelector, IComparer<TKey> comparer) =>
        source.Value.OrderBy(keySelector, comparer);

    /// <summary>
    /// Sorts the elements of a sequence in descending order according to a key.
    /// </summary>
    [Pure]
    public static IOrderedEnumerable<T> OrderByDescending<T, TKey>(this Arr<T> source, Func<T, TKey> keySelector) =>
        source.Value.OrderByDescending(keySelector);

    /// <summary>
    /// Sorts the elements of a sequence in descending order by using a specified comparer.
    /// </summary>
    [Pure]
    public static IOrderedEnumerable<T> OrderByDescending<T, TKey>(this Arr<T> source, Func<T, TKey> keySelector, IComparer<TKey> comparer) =>
        source.Value.OrderByDescending(keySelector, comparer);

    /// <summary>
    /// Inverts the order of the elements in a sequence.
    /// </summary>
    [Pure]
    public static IEnumerable<T> Reverse<T>(this Arr<T> source) =>
        source.Value.Reverse();

    /// <summary>
    /// Determines whether two sequences are equal by comparing the elements by using the default equality comparer for their type.
    /// </summary>
    [Pure]
    public static bool SequenceEqual<T>(this Arr<T> first, IEnumerable<T> second) =>
        EqEnumerable<T>.Equals(first.Value, second);

    /// <summary>
    /// Determines whether two sequences are equal by comparing their elements by using a specified IEqualityComparer<T>.
    /// </summary>
    [Pure]
    public static bool SequenceEqual<T>(this Arr<T> first, IEnumerable<T> second, IEqualityComparer<T> comparer) =>
        first.Value.SequenceEqual(second, comparer);

    /// <summary>
    /// Returns the only element of a sequence, and throws an exception if there is not exactly one element in the sequence.
    /// </summary>
    [Pure]
    public static T Single<T>(this Arr<T> source) =>
        source.Value.Single();

    /// <summary>
    /// Returns the only element of a sequence that satisfies a specified condition, and throws an exception if more than one such element exists.
    /// </summary>
    [Pure]
    public static T Single<T>(this Arr<T> source, Func<T, bool> predicate) =>
        source.Value.Single(predicate);

    /// <summary>
    /// Returns the only element of a sequence, or a default value if the sequence is empty; this method throws an exception if there is more than one element in the sequence.
    /// </summary>
    [Pure]
    public static T? SingleOrDefault<T>(this Arr<T> source) =>
        source.Value.SingleOrDefault();

    /// <summary>
    /// Returns the only element of a sequence that satisfies a specified condition or a default value if no such element exists; this method throws an exception if more than one element satisfies the condition.
    /// </summary>
    [Pure]
    public static T? SingleOrDefault<T>(this Arr<T> source, Func<T?, bool> predicate) =>
        source.Value.SingleOrDefault(predicate);

    /// <summary>
    /// Bypasses a specified number of elements in a sequence and then returns the remaining elements.
    /// </summary>
    [Pure]
    public static IEnumerable<T> Skip<T>(this Arr<T> source, int count) =>
        source.Value.Skip(count);

    /// <summary>
    /// Bypasses elements in a sequence as long as a specified condition is true and then returns the remaining elements.
    /// </summary>
    [Pure]
    public static IEnumerable<T> SkipWhile<T>(this Arr<T> source, Func<T, bool> predicate) =>
        source.Value.SkipWhile(predicate);

    /// <summary>
    /// Bypasses elements in a sequence as long as a specified condition is true and then returns the remaining elements. The element's index is used in the logic of the predicate function.
    /// </summary>
    [Pure]
    public static IEnumerable<T> SkipWhile<T>(this Arr<T> source, Func<T, int, bool> predicate) =>
        source.Value.SkipWhile(predicate);

    /// <summary>
    /// Computes the sum of a sequence of Decimal values.
    /// </summary>
    [Pure]
    public static decimal Sum(this Arr<decimal> source) =>
        source.Value.Sum();

    /// <summary>
    /// Computes the sum of the sequence of Decimal values that are obtained by invoking a transform function on each element of the input sequence.
    /// </summary>
    [Pure]
    public static decimal Sum<T>(this Arr<T> source, Func<T, decimal> selector) =>
        source.Value.Sum(selector);

    /// <summary>
    /// Computes the sum of a sequence of nullable Decimal values.
    /// </summary>
    [Pure]
    public static decimal? Sum(this Arr<decimal?> source) =>
        source.Value.Sum();

    /// <summary>
    /// Computes the sum of the sequence of nullable Decimal values that are obtained by invoking a transform function on each element of the input sequence.
    /// </summary>
    [Pure]
    public static decimal? Sum<T>(this Arr<T> source, Func<T, decimal?> selector) =>
        source.Value.Sum(selector);

    /// <summary>
    /// Computes the sum of a sequence of Double values.
    /// </summary>
    [Pure]
    public static double Sum(this Arr<double> source) =>
        source.Value.Sum();

    /// <summary>
    /// Computes the sum of the sequence of Double values that are obtained by invoking a transform function on each element of the input sequence.
    /// </summary>
    [Pure]
    public static double Sum<T>(this Arr<T> source, Func<T, double> selector) =>
        source.Value.Sum(selector);

    /// <summary>
    /// Computes the sum of a sequence of nullable Double values.
    /// </summary>
    [Pure]
    public static double? Sum(this Arr<double?> source) =>
        source.Value.Sum();

    /// <summary>
    /// Computes the sum of the sequence of nullable Double values that are obtained by invoking a transform function on each element of the input sequence.
    /// </summary>
    [Pure]
    public static double? Sum<T>(this Arr<T> source, Func<T, double?> selector) =>
        source.Value.Sum(selector);

    /// <summary>
    /// Computes the sum of a sequence of Single values.
    /// </summary>
    [Pure]
    public static float Sum(this Arr<float> source) =>
        source.Value.Sum();

    /// <summary>
    /// Computes the sum of the sequence of Single values that are obtained by invoking a transform function on each element of the input sequence.
    /// </summary>
    [Pure]
    public static float Sum<T>(this Arr<T> source, Func<T, float> selector) =>
        source.Value.Sum(selector);

    /// <summary>
    /// Computes the sum of a sequence of nullable Single values.
    /// </summary>
    [Pure]
    public static float? Sum(this Arr<float?> source) =>
        source.Value.Sum();

    /// <summary>
    /// Computes the sum of the sequence of nullable Single values that are obtained by invoking a transform function on each element of the input sequence.
    /// </summary>
    [Pure]
    public static float? Sum<T>(this Arr<T> source, Func<T, float?> selector) =>
        source.Value.Sum(selector);

    /// <summary>
    /// Computes the sum of a sequence of Int32 values.
    /// </summary>
    [Pure]
    public static int Sum(this Arr<int> source) =>
        source.Value.Sum();

    /// <summary>
    /// Computes the sum of the sequence of Int32 values that are obtained by invoking a transform function on each element of the input sequence.
    /// </summary>
    [Pure]
    public static int Sum<T>(this Arr<T> source, Func<T, int> selector) =>
        source.Value.Sum(selector);

    /// <summary>
    /// Computes the sum of a sequence of nullable Int32 values.
    /// </summary>
    [Pure]
    public static int? Sum(this Arr<int?> source) =>
        source.Value.Sum();

    /// <summary>
    /// Computes the sum of the sequence of nullable Int32 values that are obtained by invoking a transform function on each element of the input sequence.
    /// </summary>
    [Pure]
    public static int? Sum<T>(this Arr<T> source, Func<T, int?> selector) =>
        source.Value.Sum(selector);

    /// <summary>
    /// Computes the sum of a sequence of Int64 values.
    /// </summary>
    [Pure]
    public static long Sum(this Arr<long> source) =>
        source.Value.Sum();

    /// <summary>
    /// Computes the sum of the sequence of Int64 values that are obtained by invoking a transform function on each element of the input sequence.
    /// </summary>
    [Pure]
    public static long Sum<T>(this Arr<T> source, Func<T, long> selector) =>
        source.Value.Sum(selector);

    /// <summary>
    /// Computes the sum of a sequence of nullable Int64 values.
    /// </summary>
    [Pure]
    public static long? Sum(this Arr<long?> source) =>
        source.Value.Sum();

    /// <summary>
    /// Computes the sum of the sequence of nullable Int64 values that are obtained by invoking a transform function on each element of the input sequence.
    /// </summary>
    [Pure]
    public static long? Sum<T>(this Arr<T> source, Func<T, long?> selector) =>
        source.Value.Sum(selector);

    /// <summary>
    /// Returns a specified number of contiguous elements from the start of a sequence.
    /// </summary>
    [Pure]
    public static IEnumerable<T> Take<T>(this Arr<T> source, int count) =>
        source.Value.Take(count);

    /// <summary>
    /// Returns elements from a sequence as long as a specified condition is true.
    /// </summary>
    [Pure]
    public static IEnumerable<T> TakeWhile<T>(this Arr<T> source, Func<T, bool> predicate) =>
        source.Value.TakeWhile(predicate);

    /// <summary>
    /// Returns elements from a sequence as long as a specified condition is true. The element's index is used in the logic of the predicate function.
    /// </summary>
    [Pure]
    public static IEnumerable<T> TakeWhile<T>(this Arr<T> source, Func<T, int, bool> predicate) =>
        source.Value.TakeWhile(predicate);

    /// <summary>
    /// Creates an array from a IEnumerable<T>.
    /// </summary>
    [Pure]
    public static T[] ToArray<T>(this Arr<T> source) =>
        source.Value.ToArray();

    /// <summary>
    /// Creates a Dictionary<TKey,TValue> from an IEnumerable<T> according to specified key selector and element selector functions.
    /// </summary>
    [Pure]
    public static Dictionary<TKey, TElement> ToDictionary<T, TKey, TElement>(this Arr<T> source, Func<T, TKey> keySelector, Func<T, TElement> elementSelector) where TKey : notnull =>
        source.Value.ToDictionary(keySelector, elementSelector);

    /// <summary>
    /// Creates a Dictionary<TKey,TValue> from an IEnumerable<T> according to a specified key selector function, a comparer, and an element selector function.
    /// </summary>
    [Pure]
    public static Dictionary<TKey, TElement> ToDictionary<T, TKey, TElement>(this Arr<T> source, Func<T, TKey> keySelector, Func<T, TElement> elementSelector, IEqualityComparer<TKey> comparer) where TKey : notnull =>
        source.Value.ToDictionary(keySelector, elementSelector, comparer);

    /// <summary>
    /// Creates a Dictionary<TKey,TValue> from an IEnumerable<T> according to a specified key selector function.
    /// </summary>
    [Pure]
    public static Dictionary<TKey, T> ToDictionary<T, TKey>(this Arr<T> source, Func<T, TKey> keySelector) where TKey : notnull =>
        source.Value.ToDictionary(keySelector);

    /// <summary>
    /// Creates a Dictionary<TKey,TValue> from an IEnumerable<T> according to a specified key selector function and key comparer.
    /// </summary>
    [Pure]
    public static Dictionary<TKey, T> ToDictionary<T, TKey>(this Arr<T> source, Func<T, TKey> keySelector, IEqualityComparer<TKey> comparer) where TKey : notnull =>
        source.Value.ToDictionary(keySelector, comparer);

    /// <summary>
    /// Creates a List<T> from an IEnumerable<T>.
    /// </summary>
    [Pure]
    public static List<T> ToList<T>(this Arr<T> source) =>
        source.Value.ToList();

    /// <summary>
    /// Creates a Lookup<TKey,TElement> from an IEnumerable<T> according to specified key selector and element selector functions.
    /// </summary>
    [Pure]
    public static ILookup<TKey, TElement> ToLookup<T, TKey, TElement>(this Arr<T> source, Func<T, TKey> keySelector, Func<T, TElement> elementSelector) =>
        source.Value.ToLookup(keySelector, elementSelector);

    /// <summary>
    /// Creates a Lookup<TKey,TElement> from an IEnumerable<T> according to a specified key selector function, a comparer and an element selector function.
    /// </summary>
    [Pure]
    public static ILookup<TKey, TElement> ToLookup<T, TKey, TElement>(this Arr<T> source, Func<T, TKey> keySelector, Func<T, TElement> elementSelector, IEqualityComparer<TKey> comparer) =>
        source.Value.ToLookup(keySelector, elementSelector, comparer);

    /// <summary>
    /// Creates a Lookup<TKey,TElement> from an IEnumerable<T> according to a specified key selector function.
    /// </summary>
    [Pure]
    public static ILookup<TKey, T> ToLookup<T, TKey>(this Arr<T> source, Func<T, TKey> keySelector) =>
        source.Value.ToLookup(keySelector);

    /// <summary>
    /// Creates a Lookup<TKey,TElement> from an IEnumerable<T> according to a specified key selector function and key comparer.
    /// </summary>
    [Pure]
    public static ILookup<TKey, T> ToLookup<T, TKey>(this Arr<T> source, Func<T, TKey> keySelector, IEqualityComparer<TKey> comparer) =>
        source.Value.ToLookup(keySelector, comparer);

    /// <summary>
    /// Produces the set union of two sequences by using the default equality comparer.
    /// </summary>
    [Pure]
    public static IEnumerable<T> Union<T>(this Arr<T> first, IEnumerable<T> second) =>
        first.Value.Union(second);

    /// <summary>
    /// Produces the set union of two sequences by using a specified IEqualityComparer<T>.
    /// </summary>
    [Pure]
    public static IEnumerable<T> Union<T>(this Arr<T> first, IEnumerable<T> second, IEqualityComparer<T> comparer) =>
        first.Value.Union(second, comparer);

    /// <summary>
    /// Applies a specified function to the corresponding elements of two sequences, producing a sequence of the results.
    /// </summary>
    [Pure]
    public static IEnumerable<R> Zip<TFirst, TSecond, R>(this Arr<TFirst> first, IEnumerable<TSecond> second, Func<TFirst, TSecond, R> resultSelector) =>
        first.Value.Zip(second, resultSelector);

    [Pure]
    public static Arr<A> Filter<A>(this Arr<A> ma, Func<A, bool> f) =>
        Where(ma, f);

    [Pure]
    public static Arr<A> Where<A>(this Arr<A> ma, Func<A, bool> f)
    {
        var mb = new List<A>();
        foreach (var a in ma)
        {
            if (f(a))
            {
                mb.Add(a);
            }
        }
        return new Arr<A>(mb);
    }

    [Pure]
    public static Arr<B> Map<A, B>(this Arr<A> ma, Func<A, B> f) =>
        Select(ma, f);

    [Pure]
    public static Arr<B> Select<A, B>(this Arr<A> ma, Func<A, B> f)
    {
        var mb = new B[ma.Count];
        var index = 0;
        foreach (var a in ma)
        {
            mb[index] = f(a);
            index++;
        }
        return new Arr<B>(mb);
    }

    [Pure]
    public static Arr<B> Bind<A, B>(this Arr<A> ma, Func<A, Arr<B>> f) =>
        SelectMany(ma, f);

    [Pure]
    public static Arr<B> SelectMany<A, B>(this Arr<A> ma, Func<A, Arr<B>> f)
    {
        var mb = new List<B>();
        foreach (var a in ma)
        {
            foreach (var b in f(a))
            {
                mb.Add(b);
            }
        }
        return new Arr<B>(mb.ToArray());
    }

    [Pure]
    public static Arr<C> SelectMany<A, B, C>(this Arr<A> ma, Func<A, Arr<B>> bind, Func<A, B, C> project)
    {
        var mc = new List<C>();
        foreach (var a in ma)
        {
            foreach (var b in bind(a))
            {
                mc.Add(project(a, b));
            }
        }
        return new Arr<C>(mc.ToArray());
    }
    
    /// <summary>
    /// Apply a sequence of values to a sequence of functions
    /// </summary>
    /// <param name="fabc">sequence of functions</param>
    /// <param name="fa">sequence of argument values</param>
    /// <returns>Returns the result of applying the sequence argument values to the sequence functions</returns>
    [Pure]
    public static Arr<B> Apply<A, B>(this Arr<Func<A, B>> fabc, Arr<A> fa) =>
        fabc.Bind(fa.Map);

    /// <summary>
    /// Apply a sequence of values to a sequence of functions of arity 2
    /// </summary>
    /// <param name="fabc">sequence of functions</param>
    /// <param name="fa">sequence argument values</param>
    /// <returns>Returns the result of applying the sequence of argument values to the 
    /// IEnumerable of functions: a sequence of functions of arity 1</returns>
    [Pure]
    public static Arr<Func<B, C>> Apply<A, B, C>(this Arr<Func<A, B, C>> fabc, Arr<A> fa) =>
        fabc.Bind(f => fa.Map(curry(f)));

    /// <summary>
    /// Apply sequence of values to a sequence of functions of arity 2
    /// </summary>
    /// <param name="fabc">sequence of functions</param>
    /// <param name="fa">sequence argument values</param>
    /// <param name="fb">sequence argument values</param>
    /// <returns>Returns the result of applying the sequence of arguments to the sequence of functions</returns>
    [Pure]
    public static Arr<C> Apply<A, B, C>(this Arr<Func<A, B, C>> fabc, Arr<A> fa, Arr<B> fb) =>
        fabc.Bind(f => fa.Bind(a => fb.Map(b => f(a, b))));

    /// <summary>
    /// Apply a sequence of values to a sequence of functions of arity 2
    /// </summary>
    /// <param name="fabc">sequence of functions</param>
    /// <param name="fa">sequence argument values</param>
    /// <returns>Returns the result of applying the sequence of argument values to the 
    /// sequence of functions: a sequence of functions of arity 1</returns>
    [Pure]
    public static Arr<Func<B, C>> Apply<A, B, C>(this Arr<Func<A, Func<B, C>>> fabc, Arr<A> fa) =>
        fabc.Bind(fa.Map);

    /// <summary>
    /// Apply sequence of values to an sequence of functions of arity 2
    /// </summary>
    /// <param name="fabc">sequence of functions</param>
    /// <param name="fa">sequence argument values</param>
    /// <param name="fb">sequence argument values</param>
    /// <returns>Returns the result of applying the sequence of arguments to the sequence of functions</returns>
    [Pure]
    public static Arr<C> Apply<A, B, C>(this Arr<Func<A, Func<B, C>>> fabc, Arr<A> fa, Arr<B> fb) =>
        fabc.Bind(f => fa.Bind(a => fb.Map(f(a))));

    /// <summary>
    /// Evaluate fa, then fb, ignoring the result of fa
    /// </summary>
    /// <param name="fa">Applicative to evaluate first</param>
    /// <param name="fb">Applicative to evaluate second and then return</param>
    /// <returns>Applicative of type FB derived from Applicative of B</returns>
    [Pure]
    public static Arr<B> Action<A, B>(this Arr<A> fa, Arr<B> fb) =>
        fa.Bind(_ => fb);

}
