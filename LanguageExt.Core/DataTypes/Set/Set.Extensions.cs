using System;
using System.Linq;
using System.Collections.Generic;
using LanguageExt;
using System.Diagnostics.Contracts;

public static class SetExtensions
{
    /// <summary>
    /// Applies an accumulator function over a sequence. The specified seed value is used as the initial accumulator value.
    /// </summary>
    [Pure]
    public static TAccumulate Aggregate<TSource, TAccumulate>(this Set<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func) =>
        Enumerable.Aggregate(source.AsEnumerable(), seed, func);

    /// <summary>
    /// Applies an accumulator function over a sequence. The specified seed value is used as the initial accumulator value, and the specified function is used to select the result value.
    /// </summary>
    [Pure]
    public static TResult Aggregate<TSource, TAccumulate, TResult>(this Set<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func, Func<TAccumulate, TResult> resultSelector) =>
        Enumerable.Aggregate(source.AsEnumerable(), seed, func, resultSelector);

    /// <summary>
    /// Applies an accumulator function over a sequence.
    /// </summary>
    [Pure]
    public static TSource Aggregate<TSource>(this Set<TSource> source, Func<TSource, TSource, TSource> func) =>
        Enumerable.Aggregate(source.AsEnumerable(), func);

    /// <summary>
    /// Determines whether all elements of a sequence satisfy a condition.
    /// </summary>
    [Pure]
    public static bool All<TSource>(this Set<TSource> source, Func<TSource, bool> predicate) =>
        Enumerable.All(source.AsEnumerable(), predicate);

    /// <summary>
    /// Determines whether a sequence contains any elements.
    /// </summary>
    [Pure]
    public static bool Any<TSource>(this Set<TSource> source) =>
        Enumerable.Any(source.AsEnumerable());

    /// <summary>
    /// Determines whether any element of a sequence satisfies a condition.
    /// </summary>
    [Pure]
    public static bool Any<TSource>(this Set<TSource> source, Func<TSource, bool> predicate) =>
        Enumerable.Any(source.AsEnumerable(), predicate);

    /// <summary>
    /// Returns the input typed as IEnumerable<T>.
    /// </summary>
    [Pure]
    public static IEnumerable<TSource> AsEnumerable<TSource>(this Set<TSource> source) =>
        Enumerable.AsEnumerable(source.AsEnumerable());

    /// <summary>
    /// Converts a generic IEnumerable<T> to a generic IQueryable<T>.
    /// </summary>
    [Pure]
    public static IQueryable<TElement> AsQueryable<TElement>(this Set<TElement> source) =>
        Queryable.AsQueryable(source.AsEnumerable().AsQueryable());

    /// <summary>
    /// Computes the average of a sequence of Decimal values.
    /// </summary>
    [Pure]
    public static decimal Average(this Set<decimal> source) =>
        Enumerable.Average(source.AsEnumerable());

    /// <summary>
    /// Computes the average of a sequence of Decimal values that are obtained by invoking a transform function on each element of the input sequence.
    /// </summary>
    [Pure]
    public static decimal Average<TSource>(this Set<TSource> source, Func<TSource, decimal> selector) =>
        Enumerable.Average(source.AsEnumerable(), selector);

    /// <summary>
    /// Computes the average of a sequence of nullable Decimal values.
    /// </summary>
    [Pure]
    public static decimal? Average(this Set<decimal?> source) =>
        Enumerable.Average(source.AsEnumerable());

    /// <summary>
    /// Computes the average of a sequence of nullable Decimal values that are obtained by invoking a transform function on each element of the input sequence.
    /// </summary>
    [Pure]
    public static decimal? Average<TSource>(this Set<TSource> source, Func<TSource, decimal?> selector) =>
        Enumerable.Average(source.AsEnumerable(), selector);

    /// <summary>
    /// Computes the average of a sequence of Double values.
    /// </summary>
    [Pure]
    public static double Average(this Set<double> source) =>
        Enumerable.Average(source.AsEnumerable());

    /// <summary>
    /// Computes the average of a sequence of Int32 values.
    /// </summary>
    [Pure]
    public static double Average(this Set<int> source) =>
        Enumerable.Average(source.AsEnumerable());

    /// <summary>
    /// Computes the average of a sequence of Int64 values.
    /// </summary>
    [Pure]
    public static double Average(this Set<long> source) =>
        Enumerable.Average(source.AsEnumerable());

    /// <summary>
    /// Computes the average of a sequence of Double values that are obtained by invoking a transform function on each element of the input sequence.
    /// </summary>
    [Pure]
    public static double Average<TSource>(this Set<TSource> source, Func<TSource, double> selector) =>
        Enumerable.Average(source.AsEnumerable(), selector);

    /// <summary>
    /// Computes the average of a sequence of Int32 values that are obtained by invoking a transform function on each element of the input sequence.
    /// </summary>
    [Pure]
    public static double Average<TSource>(this Set<TSource> source, Func<TSource, int> selector) =>
        Enumerable.Average(source.AsEnumerable(), selector);

    /// <summary>
    /// Computes the average of a sequence of Int64 values that are obtained by invoking a transform function on each element of the input sequence.
    /// </summary>
    [Pure]
    public static double Average<TSource>(this Set<TSource> source, Func<TSource, long> selector) =>
        Enumerable.Average(source.AsEnumerable(), selector);

    /// <summary>
    /// Computes the average of a sequence of nullable Double values.
    /// </summary>
    [Pure]
    public static double? Average(this Set<double?> source) =>
        Enumerable.Average(source.AsEnumerable());

    /// <summary>
    /// Computes the average of a sequence of nullable Int32 values.
    /// </summary>
    [Pure]
    public static double? Average(this Set<int?> source) =>
        Enumerable.Average(source.AsEnumerable());

    /// <summary>
    /// Computes the average of a sequence of nullable Int64 values.
    /// </summary>
    [Pure]
    public static double? Average(this Set<long?> source) =>
        Enumerable.Average(source.AsEnumerable());

    /// <summary>
    /// Computes the average of a sequence of nullable Double values that are obtained by invoking a transform function on each element of the input sequence.
    /// </summary>
    [Pure]
    public static double? Average<TSource>(this Set<TSource> source, Func<TSource, double?> selector) =>
        Enumerable.Average(source.AsEnumerable(), selector);

    /// <summary>
    /// Computes the average of a sequence of nullable Int32 values that are obtained by invoking a transform function on each element of the input sequence.
    /// </summary>
    [Pure]
    public static double? Average<TSource>(this Set<TSource> source, Func<TSource, int?> selector) =>
        Enumerable.Average(source.AsEnumerable(), selector);

    /// <summary>
    /// Computes the average of a sequence of nullable Int64 values that are obtained by invoking a transform function on each element of the input sequence.
    /// </summary>
    [Pure]
    public static double? Average<TSource>(this Set<TSource> source, Func<TSource, long?> selector) =>
        Enumerable.Average(source.AsEnumerable(), selector);

    /// <summary>
    /// Computes the average of a sequence of Single values.
    /// </summary>
    [Pure]
    public static float Average(this Set<float> source) =>
        Enumerable.Average(source.AsEnumerable());

    /// <summary>
    /// Computes the average of a sequence of Single values that are obtained by invoking a transform function on each element of the input sequence.
    /// </summary>
    [Pure]
    public static float Average<TSource>(this Set<TSource> source, Func<TSource, float> selector) =>
        Enumerable.Average(source.AsEnumerable(), selector);

    /// <summary>
    /// Computes the average of a sequence of nullable Single values.
    /// </summary>
    [Pure]
    public static float? Average(this Set<float?> source) =>
        Enumerable.Average(source.AsEnumerable());

    /// <summary>
    /// Computes the average of a sequence of nullable Single values that are obtained by invoking a transform function on each element of the input sequence.
    /// </summary>
    [Pure]
    public static float? Average<TSource>(this Set<TSource> source, Func<TSource, float?> selector) =>
        Enumerable.Average(source.AsEnumerable(), selector);

    /// <summary>
    /// Concatenates two sequences.
    /// </summary>
    [Pure]
    public static IEnumerable<TSource> Concat<TSource>(this Set<TSource> first, IEnumerable<TSource> second) =>
        Enumerable.Concat(first.AsEnumerable(), second);

    /// <summary>
    /// Determines whether a sequence contains a specified element by using the default equality comparer.
    /// </summary>
    [Pure]
    public static bool Contains<TSource>(this Set<TSource> source, TSource value) =>
        Enumerable.Contains(source.AsEnumerable(), value);

    /// <summary>
    /// Determines whether a sequence contains a specified element by using a specified IEqualityComparer<T>.
    /// </summary>
    [Pure]
    public static bool Contains<TSource>(this Set<TSource> source, TSource value, IEqualityComparer<TSource> comparer) =>
        Enumerable.Contains(source.AsEnumerable(), value, comparer);

    /// <summary>
    /// Returns the number of elements in a sequence.
    /// </summary>
    [Pure]
    public static int Count<TSource>(this Set<TSource> source) =>
        Enumerable.Count(source.AsEnumerable());

    /// <summary>
    /// Returns a number that represents how many elements in the specified sequence satisfy a condition.
    /// </summary>
    [Pure]
    public static int Count<TSource>(this Set<TSource> source, Func<TSource, bool> predicate) =>
        Enumerable.Count(source.AsEnumerable(), predicate);

    /// <summary>
    /// Returns the elements of the specified sequence or the type parameter's default value in a singleton collection if the sequence is empty.
    /// </summary>
    [Pure]
    public static IEnumerable<TSource> DefaultIfEmpty<TSource>(this Set<TSource> source) =>
        Enumerable.DefaultIfEmpty(source.AsEnumerable());

    /// <summary>
    /// Returns the elements of the specified sequence or the specified value in a singleton collection if the sequence is empty.
    /// </summary>
    [Pure]
    public static IEnumerable<TSource> DefaultIfEmpty<TSource>(this Set<TSource> source, TSource defaultValue) =>
        Enumerable.DefaultIfEmpty(source.AsEnumerable(), defaultValue);

    /// <summary>
    /// Returns distinct elements from a sequence by using the default equality comparer to compare values.
    /// </summary>
    [Pure]
    public static IEnumerable<TSource> Distinct<TSource>(this Set<TSource> source) =>
        Enumerable.Distinct(source.AsEnumerable());

    /// <summary>
    /// Returns distinct elements from a sequence by using a specified IEqualityComparer<T> to compare values.
    /// </summary>
    [Pure]
    public static IEnumerable<TSource> Distinct<TSource>(this Set<TSource> source, IEqualityComparer<TSource> comparer) =>
        Enumerable.Distinct(source.AsEnumerable(), comparer);

    /// <summary>
    /// Returns the element at a specified index in a sequence.
    /// </summary>
    [Pure]
    public static TSource ElementAt<TSource>(this Set<TSource> source, int index) =>
        Enumerable.ElementAt(source.AsEnumerable(), index);

    /// <summary>
    /// Returns the element at a specified index in a sequence or a default value if the index is out of range.
    /// </summary>
    [Pure]
    public static TSource ElementAtOrDefault<TSource>(this Set<TSource> source, int index) =>
        Enumerable.ElementAtOrDefault(source.AsEnumerable(), index);

    /// <summary>
    /// Produces the set difference of two sequences by using the default equality comparer to compare values.
    /// </summary>
    [Pure]
    public static IEnumerable<TSource> Except<TSource>(this Set<TSource> first, IEnumerable<TSource> second) =>
        Enumerable.Except(first.AsEnumerable(), second);

    /// <summary>
    /// Produces the set difference of two sequences by using the specified IEqualityComparer<T> to compare values.
    /// </summary>
    [Pure]
    public static IEnumerable<TSource> Except<TSource>(this Set<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer) =>
        Enumerable.Except(first.AsEnumerable(), second, comparer);

    /// <summary>
    /// Returns the first element of a sequence.
    /// </summary>
    [Pure]
    public static TSource First<TSource>(this Set<TSource> source) =>
        Enumerable.First(source.AsEnumerable());

    /// <summary>
    /// Returns the first element in a sequence that satisfies a specified condition.
    /// </summary>
    [Pure]
    public static TSource First<TSource>(this Set<TSource> source, Func<TSource, bool> predicate) =>
        Enumerable.First(source.AsEnumerable(), predicate);

    /// <summary>
    /// Returns the first element of a sequence, or a default value if the sequence contains no elements.
    /// </summary>
    [Pure]
    public static TSource FirstOrDefault<TSource>(this Set<TSource> source) =>
        Enumerable.FirstOrDefault(source.AsEnumerable());

    /// <summary>
    /// Returns the first element of the sequence that satisfies a condition or a default value if no such element is found.
    /// </summary>
    [Pure]
    public static TSource FirstOrDefault<TSource>(this Set<TSource> source, Func<TSource, bool> predicate) =>
        Enumerable.FirstOrDefault(source.AsEnumerable(), predicate);

    /// <summary>
    /// Groups the elements of a sequence according to a specified key selector function and projects the elements for each group by using a specified function.
    /// </summary>
    [Pure]
    public static IEnumerable<IGrouping<TKey, TElement>> GroupBy<TSource, TKey, TElement>(this Set<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector) =>
        Enumerable.GroupBy(source.AsEnumerable(), keySelector, elementSelector);

    /// <summary>
    /// Groups the elements of a sequence according to a key selector function. The keys are compared by using a comparer and each group's elements are projected by using a specified function.
    /// </summary>
    [Pure]
    public static IEnumerable<IGrouping<TKey, TElement>> GroupBy<TSource, TKey, TElement>(this Set<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer) =>
        Enumerable.GroupBy(source.AsEnumerable(), keySelector, elementSelector, comparer);

    /// <summary>
    /// Groups the elements of a sequence according to a specified key selector function.
    /// </summary>
    [Pure]
    public static IEnumerable<IGrouping<TKey, TSource>> GroupBy<TSource, TKey>(this Set<TSource> source, Func<TSource, TKey> keySelector) =>
        Enumerable.GroupBy(source.AsEnumerable(), keySelector);

    /// <summary>
    /// Groups the elements of a sequence according to a specified key selector function and compares the keys by using a specified comparer.
    /// </summary>
    [Pure]
    public static IEnumerable<IGrouping<TKey, TSource>> GroupBy<TSource, TKey>(this Set<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer) =>
        Enumerable.GroupBy(source.AsEnumerable(), keySelector, comparer);

    /// <summary>
    /// Groups the elements of a sequence according to a specified key selector function and creates a result value from each group and its key. The elements of each group are projected by using a specified function.
    /// </summary>
    [Pure]
    public static IEnumerable<TResult> GroupBy<TSource, TKey, TElement, TResult>(this Set<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, Func<TKey, IEnumerable<TElement>, TResult> resultSelector) =>
        Enumerable.GroupBy(source.AsEnumerable(), keySelector, elementSelector, resultSelector);

    /// <summary>
    /// Groups the elements of a sequence according to a specified key selector function and creates a result value from each group and its key. Key values are compared by using a specified comparer, and the elements of each group are projected by using a specified function.
    /// </summary>
    [Pure]
    public static IEnumerable<TResult> GroupBy<TSource, TKey, TElement, TResult>(this Set<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, Func<TKey, IEnumerable<TElement>, TResult> resultSelector, IEqualityComparer<TKey> comparer) =>
        Enumerable.GroupBy(source.AsEnumerable(), keySelector, elementSelector, resultSelector, comparer);

    /// <summary>
    /// Groups the elements of a sequence according to a specified key selector function and creates a result value from each group and its key.
    /// </summary>
    [Pure]
    public static IEnumerable<TResult> GroupBy<TSource, TKey, TResult>(this Set<TSource> source, Func<TSource, TKey> keySelector, Func<TKey, IEnumerable<TSource>, TResult> resultSelector) =>
        Enumerable.GroupBy(source.AsEnumerable(), keySelector, resultSelector);

    /// <summary>
    /// Groups the elements of a sequence according to a specified key selector function and creates a result value from each group and its key. The keys are compared by using a specified comparer.
    /// </summary>
    [Pure]
    public static IEnumerable<TResult> GroupBy<TSource, TKey, TResult>(this Set<TSource> source, Func<TSource, TKey> keySelector, Func<TKey, IEnumerable<TSource>, TResult> resultSelector, IEqualityComparer<TKey> comparer) =>
        Enumerable.GroupBy(source.AsEnumerable(), keySelector, resultSelector, comparer);

    /// <summary>
    /// Correlates the elements of two sequences based on equality of keys and groups the results. The default equality comparer is used to compare keys.
    /// </summary>
    [Pure]
    public static IEnumerable<TResult> GroupJoin<TOuter, TInner, TKey, TResult>(this Set<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector) =>
        Enumerable.GroupJoin(outer.AsEnumerable(), inner, outerKeySelector, innerKeySelector, resultSelector);

    /// <summary>
    /// Correlates the elements of two sequences based on key equality and groups the results. A specified IEqualityComparer<T> is used to compare keys.
    /// </summary>
    [Pure]
    public static IEnumerable<TResult> GroupJoin<TOuter, TInner, TKey, TResult>(this Set<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector, IEqualityComparer<TKey> comparer) =>
        Enumerable.GroupJoin(outer.AsEnumerable(), inner, outerKeySelector, innerKeySelector, resultSelector, comparer);

    /// <summary>
    /// Produces the set intersection of two sequences by using the default equality comparer to compare values.
    /// </summary>
    [Pure]
    public static IEnumerable<TSource> Intersect<TSource>(this Set<TSource> first, IEnumerable<TSource> second) =>
        Enumerable.Intersect(first.AsEnumerable(), second);

    /// <summary>
    /// Produces the set intersection of two sequences by using the specified IEqualityComparer<T> to compare values.
    /// </summary>
    [Pure]
    public static IEnumerable<TSource> Intersect<TSource>(this Set<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer) =>
        Enumerable.Intersect(first.AsEnumerable(), second, comparer);

    /// <summary>
    /// Correlates the elements of two sequences based on matching keys. The default equality comparer is used to compare keys.
    /// </summary>
    [Pure]
    public static IEnumerable<TResult> Join<TOuter, TInner, TKey, TResult>(this Set<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector) =>
        Enumerable.Join(outer.AsEnumerable(), inner, outerKeySelector, innerKeySelector, resultSelector);

    /// <summary>
    /// Correlates the elements of two sequences based on matching keys. A specified IEqualityComparer<T> is used to compare keys.
    /// </summary>
    [Pure]
    public static IEnumerable<TResult> Join<TOuter, TInner, TKey, TResult>(this Set<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector, IEqualityComparer<TKey> comparer) =>
        Enumerable.Join(outer.AsEnumerable(), inner, outerKeySelector, innerKeySelector, resultSelector, comparer);

    /// <summary>
    /// Returns the last element of a sequence.
    /// </summary>
    [Pure]
    public static TSource Last<TSource>(this Set<TSource> source) =>
        Enumerable.Last(source.AsEnumerable());

    /// <summary>
    /// Returns the last element of a sequence that satisfies a specified condition.
    /// </summary>
    [Pure]
    public static TSource Last<TSource>(this Set<TSource> source, Func<TSource, bool> predicate) =>
        Enumerable.Last(source.AsEnumerable(), predicate);

    /// <summary>
    /// Returns the last element of a sequence, or a default value if the sequence contains no elements.
    /// </summary>
    [Pure]
    public static TSource LastOrDefault<TSource>(this Set<TSource> source) =>
        Enumerable.LastOrDefault(source.AsEnumerable());

    /// <summary>
    /// Returns the last element of a sequence that satisfies a condition or a default value if no such element is found.
    /// </summary>
    [Pure]
    public static TSource LastOrDefault<TSource>(this Set<TSource> source, Func<TSource, bool> predicate) =>
        Enumerable.LastOrDefault(source.AsEnumerable(), predicate);

    /// <summary>
    /// Returns an Int64 that represents the total number of elements in a sequence.
    /// </summary>
    [Pure]
    public static long LongCount<TSource>(this Set<TSource> source) =>
        Enumerable.LongCount(source.AsEnumerable());

    /// <summary>
    /// Returns an Int64 that represents how many elements in a sequence satisfy a condition.
    /// </summary>
    [Pure]
    public static long LongCount<TSource>(this Set<TSource> source, Func<TSource, bool> predicate) =>
        Enumerable.LongCount(source.AsEnumerable(), predicate);

    /// <summary>
    /// Returns the maximum value in a sequence of Decimal values.
    /// </summary>
    [Pure]
    public static decimal Max(this Set<decimal> source) =>
        Enumerable.Max(source.AsEnumerable());

    /// <summary>
    /// Invokes a transform function on each element of a sequence and returns the maximum Decimal value.
    /// </summary>
    [Pure]
    public static decimal Max<TSource>(this Set<TSource> source, Func<TSource, decimal> selector) =>
        Enumerable.Max(source.AsEnumerable(), selector);

    /// <summary>
    /// Returns the maximum value in a sequence of nullable Decimal values.
    /// </summary>
    [Pure]
    public static decimal? Max(this Set<decimal?> source) =>
        Enumerable.Max(source.AsEnumerable());

    /// <summary>
    /// Invokes a transform function on each element of a sequence and returns the maximum nullable Decimal value.
    /// </summary>
    [Pure]
    public static decimal? Max<TSource>(this Set<TSource> source, Func<TSource, decimal?> selector) =>
        Enumerable.Max(source.AsEnumerable(), selector);

    /// <summary>
    /// Returns the maximum value in a sequence of Double values.
    /// </summary>
    [Pure]
    public static double Max(this Set<double> source) =>
        Enumerable.Max(source.AsEnumerable());

    /// <summary>
    /// Invokes a transform function on each element of a sequence and returns the maximum Double value.
    /// </summary>
    [Pure]
    public static double Max<TSource>(this Set<TSource> source, Func<TSource, double> selector) =>
        Enumerable.Max(source.AsEnumerable(), selector);

    /// <summary>
    /// Returns the maximum value in a sequence of nullable Double values.
    /// </summary>
    [Pure]
    public static double? Max(this Set<double?> source) =>
        Enumerable.Max(source.AsEnumerable());

    /// <summary>
    /// Invokes a transform function on each element of a sequence and returns the maximum nullable Double value.
    /// </summary>
    [Pure]
    public static double? Max<TSource>(this Set<TSource> source, Func<TSource, double?> selector) =>
        Enumerable.Max(source.AsEnumerable(), selector);

    /// <summary>
    /// Returns the maximum value in a sequence of Single values.
    /// </summary>
    [Pure]
    public static float Max(this Set<float> source) =>
        Enumerable.Max(source.AsEnumerable());

    /// <summary>
    /// Invokes a transform function on each element of a sequence and returns the maximum Single value.
    /// </summary>
    [Pure]
    public static float Max<TSource>(this Set<TSource> source, Func<TSource, float> selector) =>
        Enumerable.Max(source.AsEnumerable(), selector);

    /// <summary>
    /// Returns the maximum value in a sequence of nullable Single values.
    /// </summary>
    [Pure]
    public static float? Max(this Set<float?> source) =>
        Enumerable.Max(source.AsEnumerable());

    /// <summary>
    /// Invokes a transform function on each element of a sequence and returns the maximum nullable Single value.
    /// </summary>
    [Pure]
    public static float? Max<TSource>(this Set<TSource> source, Func<TSource, float?> selector) =>
        Enumerable.Max(source.AsEnumerable(), selector);

    /// <summary>
    /// Returns the maximum value in a sequence of Int32 values.
    /// </summary>
    [Pure]
    public static int Max(this Set<int> source) =>
        Enumerable.Max(source.AsEnumerable());

    /// <summary>
    /// Invokes a transform function on each element of a sequence and returns the maximum Int32 value.
    /// </summary>
    [Pure]
    public static int Max<TSource>(this Set<TSource> source, Func<TSource, int> selector) =>
        Enumerable.Max(source.AsEnumerable(), selector);

    /// <summary>
    /// Returns the maximum value in a sequence of nullable Int32 values.
    /// </summary>
    [Pure]
    public static int? Max(this Set<int?> source) =>
        Enumerable.Max(source.AsEnumerable());

    /// <summary>
    /// Invokes a transform function on each element of a sequence and returns the maximum nullable Int32 value.
    /// </summary>
    [Pure]
    public static int? Max<TSource>(this Set<TSource> source, Func<TSource, int?> selector) =>
        Enumerable.Max(source.AsEnumerable(), selector);

    /// <summary>
    /// Returns the maximum value in a sequence of Int64 values.
    /// </summary>
    [Pure]
    public static long Max(this Set<long> source) =>
        Enumerable.Max(source.AsEnumerable());

    /// <summary>
    /// Invokes a transform function on each element of a sequence and returns the maximum Int64 value.
    /// </summary>
    [Pure]
    public static long Max<TSource>(this Set<TSource> source, Func<TSource, long> selector) =>
        Enumerable.Max(source.AsEnumerable(), selector);

    /// <summary>
    /// Returns the maximum value in a sequence of nullable Int64 values.
    /// </summary>
    [Pure]
    public static long? Max(this Set<long?> source) =>
        Enumerable.Max(source.AsEnumerable());

    /// <summary>
    /// Invokes a transform function on each element of a sequence and returns the maximum nullable Int64 value.
    /// </summary>
    [Pure]
    public static long? Max<TSource>(this Set<TSource> source, Func<TSource, long?> selector) =>
        Enumerable.Max(source.AsEnumerable(), selector);

    /// <summary>
    /// Invokes a transform function on each element of a generic sequence and returns the maximum resulting value.
    /// </summary>
    [Pure]
    public static TResult Max<TSource, TResult>(this Set<TSource> source, Func<TSource, TResult> selector) =>
        Enumerable.Max(source.AsEnumerable(), selector);

    /// <summary>
    /// Returns the maximum value in a generic sequence.
    /// </summary>
    [Pure]
    public static TSource Max<TSource>(this Set<TSource> source) =>
        Enumerable.Max(source.AsEnumerable());

    /// <summary>
    /// Returns the minimum value in a sequence of Decimal values.
    /// </summary>
    [Pure]
    public static decimal Min(this Set<decimal> source) =>
        Enumerable.Min(source.AsEnumerable());

    /// <summary>
    /// Invokes a transform function on each element of a sequence and returns the minimum Decimal value.
    /// </summary>
    [Pure]
    public static decimal Min<TSource>(this Set<TSource> source, Func<TSource, decimal> selector) =>
        Enumerable.Min(source.AsEnumerable(), selector);

    /// <summary>
    /// Returns the minimum value in a sequence of nullable Decimal values.
    /// </summary>
    [Pure]
    public static decimal? Min(this Set<decimal?> source) =>
        Enumerable.Min(source.AsEnumerable());

    /// <summary>
    /// Invokes a transform function on each element of a sequence and returns the minimum nullable Decimal value.
    /// </summary>
    [Pure]
    public static decimal? Min<TSource>(this Set<TSource> source, Func<TSource, decimal?> selector) =>
        Enumerable.Min(source.AsEnumerable(), selector);

    /// <summary>
    /// Returns the minimum value in a sequence of Double values.
    /// </summary>
    [Pure]
    public static double Min(this Set<double> source) =>
        Enumerable.Min(source.AsEnumerable());

    /// <summary>
    /// Invokes a transform function on each element of a sequence and returns the minimum Double value.
    /// </summary>
    [Pure]
    public static double Min<TSource>(this Set<TSource> source, Func<TSource, double> selector) =>
        Enumerable.Min(source.AsEnumerable(), selector);

    /// <summary>
    /// Returns the minimum value in a sequence of nullable Double values.
    /// </summary>
    [Pure]
    public static double? Min(this Set<double?> source) =>
        Enumerable.Min(source.AsEnumerable());

    /// <summary>
    /// Invokes a transform function on each element of a sequence and returns the minimum nullable Double value.
    /// </summary>
    [Pure]
    public static double? Min<TSource>(this Set<TSource> source, Func<TSource, double?> selector) =>
        Enumerable.Min(source.AsEnumerable(), selector);

    /// <summary>
    /// Returns the minimum value in a sequence of Single values.
    /// </summary>
    [Pure]
    public static float Min(this Set<float> source) =>
        Enumerable.Min(source.AsEnumerable());

    /// <summary>
    /// Invokes a transform function on each element of a sequence and returns the minimum Single value.
    /// </summary>
    [Pure]
    public static float Min<TSource>(this Set<TSource> source, Func<TSource, float> selector) =>
        Enumerable.Min(source.AsEnumerable(), selector);

    /// <summary>
    /// Returns the minimum value in a sequence of nullable Single values.
    /// </summary>
    [Pure]
    public static float? Min(this Set<float?> source) =>
        Enumerable.Min(source.AsEnumerable());

    /// <summary>
    /// Invokes a transform function on each element of a sequence and returns the minimum nullable Single value.
    /// </summary>
    [Pure]
    public static float? Min<TSource>(this Set<TSource> source, Func<TSource, float?> selector) =>
        Enumerable.Min(source.AsEnumerable(), selector);

    /// <summary>
    /// Returns the minimum value in a sequence of Int32 values.
    /// </summary>
    [Pure]
    public static int Min(this Set<int> source) =>
        Enumerable.Min(source.AsEnumerable());

    /// <summary>
    /// Invokes a transform function on each element of a sequence and returns the minimum Int32 value.
    /// </summary>
    [Pure]
    public static int Min<TSource>(this Set<TSource> source, Func<TSource, int> selector) =>
        Enumerable.Min(source.AsEnumerable(), selector);

    /// <summary>
    /// Returns the minimum value in a sequence of nullable Int32 values.
    /// </summary>
    [Pure]
    public static int? Min(this Set<int?> source) =>
        Enumerable.Min(source.AsEnumerable());

    /// <summary>
    /// Invokes a transform function on each element of a sequence and returns the minimum nullable Int32 value.
    /// </summary>
    [Pure]
    public static int? Min<TSource>(this Set<TSource> source, Func<TSource, int?> selector) =>
        Enumerable.Min(source.AsEnumerable(), selector);

    /// <summary>
    /// Returns the minimum value in a sequence of Int64 values.
    /// </summary>
    [Pure]
    public static long Min(this Set<long> source) =>
        Enumerable.Min(source.AsEnumerable());

    /// <summary>
    /// Invokes a transform function on each element of a sequence and returns the minimum Int64 value.
    /// </summary>
    [Pure]
    public static long Min<TSource>(this Set<TSource> source, Func<TSource, long> selector) =>
        Enumerable.Min(source.AsEnumerable(), selector);

    /// <summary>
    /// Returns the minimum value in a sequence of nullable Int64 values.
    /// </summary>
    [Pure]
    public static long? Min(this Set<long?> source) =>
        Enumerable.Min(source.AsEnumerable());

    /// <summary>
    /// Invokes a transform function on each element of a sequence and returns the minimum nullable Int64 value.
    /// </summary>
    [Pure]
    public static long? Min<TSource>(this Set<TSource> source, Func<TSource, long?> selector) =>
        Enumerable.Min(source.AsEnumerable(), selector);

    /// <summary>
    /// Invokes a transform function on each element of a generic sequence and returns the minimum resulting value.
    /// </summary>
    [Pure]
    public static TResult Min<TSource, TResult>(this Set<TSource> source, Func<TSource, TResult> selector) =>
        Enumerable.Min(source.AsEnumerable(), selector);

    /// <summary>
    /// Returns the minimum value in a generic sequence.
    /// </summary>
    [Pure]
    public static TSource Min<TSource>(this Set<TSource> source) =>
        Enumerable.Min(source.AsEnumerable());

    /// <summary>
    /// Sorts the elements of a sequence in ascending order according to a key.
    /// </summary>
    [Pure]
    public static IOrderedEnumerable<TSource> OrderBy<TSource, TKey>(this Set<TSource> source, Func<TSource, TKey> keySelector) =>
        Enumerable.OrderBy(source.AsEnumerable(), keySelector);

    /// <summary>
    /// Sorts the elements of a sequence in ascending order by using a specified comparer.
    /// </summary>
    [Pure]
    public static IOrderedEnumerable<TSource> OrderBy<TSource, TKey>(this Set<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey> comparer) =>
        Enumerable.OrderBy(source.AsEnumerable(), keySelector, comparer);

    /// <summary>
    /// Sorts the elements of a sequence in descending order according to a key.
    /// </summary>
    [Pure]
    public static IOrderedEnumerable<TSource> OrderByDescending<TSource, TKey>(this Set<TSource> source, Func<TSource, TKey> keySelector) =>
        Enumerable.OrderByDescending(source.AsEnumerable(), keySelector);

    /// <summary>
    /// Sorts the elements of a sequence in descending order by using a specified comparer.
    /// </summary>
    [Pure]
    public static IOrderedEnumerable<TSource> OrderByDescending<TSource, TKey>(this Set<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey> comparer) =>
        Enumerable.OrderByDescending(source.AsEnumerable(), keySelector, comparer);

    /// <summary>
    /// Inverts the order of the elements in a sequence.
    /// </summary>
    [Pure]
    public static IEnumerable<TSource> Reverse<TSource>(this Set<TSource> source) =>
        Enumerable.Reverse(source.AsEnumerable());

    /// <summary>
    /// Determines whether two sequences are equal by comparing the elements by using the default equality comparer for their type.
    /// </summary>
    [Pure]
    public static bool SequenceEqual<TSource>(this Set<TSource> first, IEnumerable<TSource> second) =>
        Enumerable.SequenceEqual(first.AsEnumerable(), second);

    /// <summary>
    /// Determines whether two sequences are equal by comparing their elements by using a specified IEqualityComparer<T>.
    /// </summary>
    [Pure]
    public static bool SequenceEqual<TSource>(this Set<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer) =>
        Enumerable.SequenceEqual(first.AsEnumerable(), second, comparer);

    /// <summary>
    /// Returns the only element of a sequence, and throws an exception if there is not exactly one element in the sequence.
    /// </summary>
    [Pure]
    public static TSource Single<TSource>(this Set<TSource> source) =>
        Enumerable.Single(source.AsEnumerable());

    /// <summary>
    /// Returns the only element of a sequence that satisfies a specified condition, and throws an exception if more than one such element exists.
    /// </summary>
    [Pure]
    public static TSource Single<TSource>(this Set<TSource> source, Func<TSource, bool> predicate) =>
        Enumerable.Single(source.AsEnumerable(), predicate);

    /// <summary>
    /// Returns the only element of a sequence, or a default value if the sequence is empty; this method throws an exception if there is more than one element in the sequence.
    /// </summary>
    [Pure]
    public static TSource SingleOrDefault<TSource>(this Set<TSource> source) =>
        Enumerable.SingleOrDefault(source.AsEnumerable());

    /// <summary>
    /// Returns the only element of a sequence that satisfies a specified condition or a default value if no such element exists; this method throws an exception if more than one element satisfies the condition.
    /// </summary>
    [Pure]
    public static TSource SingleOrDefault<TSource>(this Set<TSource> source, Func<TSource, bool> predicate) =>
        Enumerable.SingleOrDefault(source.AsEnumerable(), predicate);

    /// <summary>
    /// Bypasses a specified number of elements in a sequence and then returns the remaining elements.
    /// </summary>
    [Pure]
    public static IEnumerable<TSource> Skip<TSource>(this Set<TSource> source, int count) =>
        Enumerable.Skip(source.AsEnumerable(), count);

    /// <summary>
    /// Bypasses elements in a sequence as long as a specified condition is true and then returns the remaining elements.
    /// </summary>
    [Pure]
    public static IEnumerable<TSource> SkipWhile<TSource>(this Set<TSource> source, Func<TSource, bool> predicate) =>
        Enumerable.SkipWhile(source.AsEnumerable(), predicate);

    /// <summary>
    /// Bypasses elements in a sequence as long as a specified condition is true and then returns the remaining elements. The element's index is used in the logic of the predicate function.
    /// </summary>
    [Pure]
    public static IEnumerable<TSource> SkipWhile<TSource>(this Set<TSource> source, Func<TSource, int, bool> predicate) =>
        Enumerable.SkipWhile(source.AsEnumerable(), predicate);

    /// <summary>
    /// Computes the sum of a sequence of Decimal values.
    /// </summary>
    [Pure]
    public static decimal Sum(this Set<decimal> source) =>
        Enumerable.Sum(source.AsEnumerable());

    /// <summary>
    /// Computes the sum of the sequence of Decimal values that are obtained by invoking a transform function on each element of the input sequence.
    /// </summary>
    [Pure]
    public static decimal Sum<TSource>(this Set<TSource> source, Func<TSource, decimal> selector) =>
        Enumerable.Sum(source.AsEnumerable(), selector);

    /// <summary>
    /// Computes the sum of a sequence of nullable Decimal values.
    /// </summary>
    [Pure]
    public static decimal? Sum(this Set<decimal?> source) =>
        Enumerable.Sum(source.AsEnumerable());

    /// <summary>
    /// Computes the sum of the sequence of nullable Decimal values that are obtained by invoking a transform function on each element of the input sequence.
    /// </summary>
    [Pure]
    public static decimal? Sum<TSource>(this Set<TSource> source, Func<TSource, decimal?> selector) =>
        Enumerable.Sum(source.AsEnumerable(), selector);

    /// <summary>
    /// Computes the sum of a sequence of Double values.
    /// </summary>
    [Pure]
    public static double Sum(this Set<double> source) =>
        Enumerable.Sum(source.AsEnumerable());

    /// <summary>
    /// Computes the sum of the sequence of Double values that are obtained by invoking a transform function on each element of the input sequence.
    /// </summary>
    [Pure]
    public static double Sum<TSource>(this Set<TSource> source, Func<TSource, double> selector) =>
        Enumerable.Sum(source.AsEnumerable(), selector);

    /// <summary>
    /// Computes the sum of a sequence of nullable Double values.
    /// </summary>
    [Pure]
    public static double? Sum(this Set<double?> source) =>
        Enumerable.Sum(source.AsEnumerable());

    /// <summary>
    /// Computes the sum of the sequence of nullable Double values that are obtained by invoking a transform function on each element of the input sequence.
    /// </summary>
    [Pure]
    public static double? Sum<TSource>(this Set<TSource> source, Func<TSource, double?> selector) =>
        Enumerable.Sum(source.AsEnumerable(), selector);

    /// <summary>
    /// Computes the sum of a sequence of Single values.
    /// </summary>
    [Pure]
    public static float Sum(this Set<float> source) =>
        Enumerable.Sum(source.AsEnumerable());

    /// <summary>
    /// Computes the sum of the sequence of Single values that are obtained by invoking a transform function on each element of the input sequence.
    /// </summary>
    [Pure]
    public static float Sum<TSource>(this Set<TSource> source, Func<TSource, float> selector) =>
        Enumerable.Sum(source.AsEnumerable(), selector);

    /// <summary>
    /// Computes the sum of a sequence of nullable Single values.
    /// </summary>
    [Pure]
    public static float? Sum(this Set<float?> source) =>
        Enumerable.Sum(source.AsEnumerable());

    /// <summary>
    /// Computes the sum of the sequence of nullable Single values that are obtained by invoking a transform function on each element of the input sequence.
    /// </summary>
    [Pure]
    public static float? Sum<TSource>(this Set<TSource> source, Func<TSource, float?> selector) =>
        Enumerable.Sum(source.AsEnumerable(), selector);

    /// <summary>
    /// Computes the sum of a sequence of Int32 values.
    /// </summary>
    [Pure]
    public static int Sum(this Set<int> source) =>
        Enumerable.Sum(source.AsEnumerable());

    /// <summary>
    /// Computes the sum of the sequence of Int32 values that are obtained by invoking a transform function on each element of the input sequence.
    /// </summary>
    [Pure]
    public static int Sum<TSource>(this Set<TSource> source, Func<TSource, int> selector) =>
        Enumerable.Sum(source.AsEnumerable(), selector);

    /// <summary>
    /// Computes the sum of a sequence of nullable Int32 values.
    /// </summary>
    [Pure]
    public static int? Sum(this Set<int?> source) =>
        Enumerable.Sum(source.AsEnumerable());

    /// <summary>
    /// Computes the sum of the sequence of nullable Int32 values that are obtained by invoking a transform function on each element of the input sequence.
    /// </summary>
    [Pure]
    public static int? Sum<TSource>(this Set<TSource> source, Func<TSource, int?> selector) =>
        Enumerable.Sum(source.AsEnumerable(), selector);

    /// <summary>
    /// Computes the sum of a sequence of Int64 values.
    /// </summary>
    [Pure]
    public static long Sum(this Set<long> source) =>
        Enumerable.Sum(source.AsEnumerable());

    /// <summary>
    /// Computes the sum of the sequence of Int64 values that are obtained by invoking a transform function on each element of the input sequence.
    /// </summary>
    [Pure]
    public static long Sum<TSource>(this Set<TSource> source, Func<TSource, long> selector) =>
        Enumerable.Sum(source.AsEnumerable(), selector);

    /// <summary>
    /// Computes the sum of a sequence of nullable Int64 values.
    /// </summary>
    [Pure]
    public static long? Sum(this Set<long?> source) =>
        Enumerable.Sum(source.AsEnumerable());

    /// <summary>
    /// Computes the sum of the sequence of nullable Int64 values that are obtained by invoking a transform function on each element of the input sequence.
    /// </summary>
    [Pure]
    public static long? Sum<TSource>(this Set<TSource> source, Func<TSource, long?> selector) =>
        Enumerable.Sum(source.AsEnumerable(), selector);

    /// <summary>
    /// Returns a specified number of contiguous elements from the start of a sequence.
    /// </summary>
    [Pure]
    public static IEnumerable<TSource> Take<TSource>(this Set<TSource> source, int count) =>
        Enumerable.Take(source.AsEnumerable(), count);

    /// <summary>
    /// Returns elements from a sequence as long as a specified condition is true.
    /// </summary>
    [Pure]
    public static IEnumerable<TSource> TakeWhile<TSource>(this Set<TSource> source, Func<TSource, bool> predicate) =>
        Enumerable.TakeWhile(source.AsEnumerable(), predicate);

    /// <summary>
    /// Returns elements from a sequence as long as a specified condition is true. The element's index is used in the logic of the predicate function.
    /// </summary>
    [Pure]
    public static IEnumerable<TSource> TakeWhile<TSource>(this Set<TSource> source, Func<TSource, int, bool> predicate) =>
        Enumerable.TakeWhile(source.AsEnumerable(), predicate);

    /// <summary>
    /// Creates an array from a IEnumerable<T>.
    /// </summary>
    [Pure]
    public static TSource[] ToArray<TSource>(this Set<TSource> source) =>
        Enumerable.ToArray(source.AsEnumerable());

    /// <summary>
    /// Creates a Dictionary<TKey,TValue> from an IEnumerable<T> according to specified key selector and element selector functions.
    /// </summary>
    [Pure]
    public static Dictionary<TKey, TElement> ToDictionary<TSource, TKey, TElement>(this Set<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector) =>
        Enumerable.ToDictionary(source.AsEnumerable(), keySelector, elementSelector);

    /// <summary>
    /// Creates a Dictionary<TKey,TValue> from an IEnumerable<T> according to a specified key selector function, a comparer, and an element selector function.
    /// </summary>
    [Pure]
    public static Dictionary<TKey, TElement> ToDictionary<TSource, TKey, TElement>(this Set<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer) =>
        Enumerable.ToDictionary(source.AsEnumerable(), keySelector, elementSelector, comparer);

    /// <summary>
    /// Creates a Dictionary<TKey,TValue> from an IEnumerable<T> according to a specified key selector function.
    /// </summary>
    [Pure]
    public static Dictionary<TKey, TSource> ToDictionary<TSource, TKey>(this Set<TSource> source, Func<TSource, TKey> keySelector) =>
        Enumerable.ToDictionary(source.AsEnumerable(), keySelector);

    /// <summary>
    /// Creates a Dictionary<TKey,TValue> from an IEnumerable<T> according to a specified key selector function and key comparer.
    /// </summary>
    [Pure]
    public static Dictionary<TKey, TSource> ToDictionary<TSource, TKey>(this Set<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer) =>
        Enumerable.ToDictionary(source.AsEnumerable(), keySelector, comparer);

    /// <summary>
    /// Creates a List<T> from an IEnumerable<T>.
    /// </summary>
    [Pure]
    public static List<TSource> ToList<TSource>(this Set<TSource> source) =>
        Enumerable.ToList(source.AsEnumerable());

    /// <summary>
    /// Creates a Lookup<TKey,TElement> from an IEnumerable<T> according to specified key selector and element selector functions.
    /// </summary>
    [Pure]
    public static ILookup<TKey, TElement> ToLookup<TSource, TKey, TElement>(this Set<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector) =>
        Enumerable.ToLookup(source.AsEnumerable(), keySelector, elementSelector);

    /// <summary>
    /// Creates a Lookup<TKey,TElement> from an IEnumerable<T> according to a specified key selector function, a comparer and an element selector function.
    /// </summary>
    [Pure]
    public static ILookup<TKey, TElement> ToLookup<TSource, TKey, TElement>(this Set<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer) =>
        Enumerable.ToLookup(source.AsEnumerable(), keySelector, elementSelector, comparer);

    /// <summary>
    /// Creates a Lookup<TKey,TElement> from an IEnumerable<T> according to a specified key selector function.
    /// </summary>
    [Pure]
    public static ILookup<TKey, TSource> ToLookup<TSource, TKey>(this Set<TSource> source, Func<TSource, TKey> keySelector) =>
        Enumerable.ToLookup(source.AsEnumerable(), keySelector);

    /// <summary>
    /// Creates a Lookup<TKey,TElement> from an IEnumerable<T> according to a specified key selector function and key comparer.
    /// </summary>
    [Pure]
    public static ILookup<TKey, TSource> ToLookup<TSource, TKey>(this Set<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer) =>
        Enumerable.ToLookup(source.AsEnumerable(), keySelector, comparer);

    /// <summary>
    /// Produces the set union of two sequences by using the default equality comparer.
    /// </summary>
    [Pure]
    public static IEnumerable<TSource> Union<TSource>(this Set<TSource> first, IEnumerable<TSource> second) =>
        Enumerable.Union(first.AsEnumerable(), second);

    /// <summary>
    /// Produces the set union of two sequences by using a specified IEqualityComparer<T>.
    /// </summary>
    [Pure]
    public static IEnumerable<TSource> Union<TSource>(this Set<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer) =>
        Enumerable.Union(first.AsEnumerable(), second, comparer);

    /// <summary>
    /// Applies a specified function to the corresponding elements of two sequences, producing a sequence of the results.
    /// </summary>
    [Pure]
    public static IEnumerable<TResult> Zip<TFirst, TSecond, TResult>(this Set<TFirst> first, IEnumerable<TSecond> second, Func<TFirst, TSecond, TResult> resultSelector) =>
        Enumerable.Zip(first.AsEnumerable(), second, resultSelector);
}
