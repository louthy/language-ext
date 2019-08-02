using LanguageExt;
using LanguageExt.ClassInstances;
using LanguageExt.TypeClasses;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using static LanguageExt.Prelude;
using static LanguageExt.TypeClass;

public static class SeqExtensions
{
    /// <summary>
    /// Monadic join
    /// </summary>
    [Pure]
    public static Seq<A> Flatten<A>(this Seq<Seq<A>> ma) =>
        ma.Bind(identity);

    /// <summary>
    /// Get the number of items in the sequence
    /// </summary>
    [Pure]
    [Obsolete("Please use Seq<A> directly.  ISeq<A> is less performant than Seq<A> and will go away eventually")]
    public static int Count<A>(this ISeq<A> seq) =>
        seq.Count;

    /// <summary>
    /// Get the number of items in the sequence
    /// </summary>
    [Pure]
    public static int Count<A>(this Seq<A> seq) =>
        seq.Count;

    /// <summary>
    /// Get the head item in the sequence
    /// </summary>
    [Pure]
    [Obsolete("Please use Seq<A> directly.  ISeq<A> is less performant than Seq<A> and will go away eventually")]
    public static A First<A>(this ISeq<A> seq) =>
        seq.Head;

    /// <summary>
    /// Get the head item in the sequence
    /// </summary>
    [Pure]
    public static A First<A>(this Seq<A> seq) =>
        seq.Head;

    /// <summary>
    /// Get the head item in the sequence
    /// </summary>
    [Pure]
    [Obsolete("Please use Seq<A> directly.  ISeq<A> is less performant than Seq<A> and will go away eventually")]
    public static A FirstOrDefault<A>(this ISeq<A> seq) =>
        seq.IsEmpty
            ? default(A)
            : seq.Head;

    /// <summary>
    /// Get the head item in the sequence
    /// </summary>
    [Pure]
    public static A FirstOrDefault<A>(this Seq<A> seq) =>
        seq.IsEmpty
            ? default(A)
            : seq.Head;

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
        LanguageExt.Seq.choose(list, selector);

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
        LanguageExt.Seq.choose(list, selector);

    /// <summary>
    /// Returns the sum total of all the items in the list (Sum in LINQ)
    /// </summary>
    /// <param name="list">List to sum</param>
    /// <returns>Sum total</returns>
    [Pure]
    public static A Sum<MonoidA, A>(this Seq<A> list) where MonoidA : struct, Monoid<A> =>
        mconcat<MonoidA, A>(list.AsEnumerable());

    /// <summary>
    /// Returns the sum total of all the items in the list (Sum in LINQ)
    /// </summary>
    /// <param name="list">List to sum</param>
    /// <returns>Sum total</returns>
    [Pure]
    public static int Sum(this Seq<int> list) =>
        LanguageExt.Seq.sum(list);

    /// <summary>
    /// Returns the sum total of all the items in the list (Sum in LINQ)
    /// </summary>
    /// <param name="list">List to sum</param>
    /// <returns>Sum total</returns>
    [Pure]
    public static float Sum(this Seq<float> list) =>
        LanguageExt.Seq.sum(list);

    /// <summary>
    /// Returns the sum total of all the items in the list (Sum in LINQ)
    /// </summary>
    /// <param name="list">List to sum</param>
    /// <returns>Sum total</returns>
    [Pure]
    public static double Sum(this Seq<double> list) =>
        LanguageExt.Seq.sum(list);

    /// <summary>
    /// Returns the sum total of all the items in the list (Sum in LINQ)
    /// </summary>
    /// <param name="list">List to sum</param>
    /// <returns>Sum total</returns>
    [Pure]
    public static decimal Sum(this Seq<decimal> list) =>
        LanguageExt.Seq.sum(list);

    /// <summary>
    /// Reverses the sequence (Reverse in LINQ)
    /// </summary>
    /// <typeparam name="T">sequence item type</typeparam>
    /// <param name="list">sequence to reverse</param>
    /// <returns>Reversed sequence</returns>
    [Pure]
    public static Seq<T> Rev<T>(this Seq<T> list) =>
        LanguageExt.Seq.rev(list);

    /// <summary>
    /// Concatenate two sequences (Concat in LINQ)
    /// </summary>
    /// <typeparam name="T">sequence item type</typeparam>
    /// <param name="lhs">First sequence</param>
    /// <param name="rhs">Second sequence</param>
    /// <returns>Concatenated sequence</returns>
    [Pure]
    public static Seq<T> Append<T>(this Seq<T> lhs, Seq<T> rhs) =>
        LanguageExt.Seq.append(lhs, rhs);

    /// <summary>
    /// Concatenate a sequence and a sequence of sequences
    /// </summary>
    /// <typeparam name="T">List item type</typeparam>
    /// <param name="lhs">First list</param>
    /// <param name="rhs">Second list</param>
    /// <returns>Concatenated list</returns>
    [Pure]
    public static Seq<T> Append<T>(this Seq<T> x, Seq<Seq<T>> xs) =>
        LanguageExt.Seq.append(x, xs);

    /// <summary>
    /// Applies a function 'folder' to each element of the sequence whilst the predicate function 
    /// returns True for the item being processed, threading an aggregate state through the 
    /// computation. The fold function takes the state argument, and applies the function 'folder' 
    /// to it and the first element of the sequence. Then, it feeds this result into the function 'folder' 
    /// along with the second element, and so on. It returns the final result.
    /// </summary>
    /// <typeparam name="S">State type</typeparam>
    /// <typeparam name="T">sequence item type</typeparam>
    /// <param name="list">sequence to fold</param>
    /// <param name="state">Initial state</param>
    /// <param name="folder">Fold function</param>
    /// <param name="preditem">Predicate function</param>
    /// <returns>Aggregate value</returns>
    [Pure]
    public static S FoldWhile<S, T>(this Seq<T> list, S state, Func<S, T, S> folder, Func<T, bool> preditem) =>
        LanguageExt.Seq.foldWhile(list, state, folder, preditem: preditem);

    /// <summary>
    /// Applies a function 'folder' to each element of the sequence, threading an accumulator 
    /// argument through the computation (and whilst the predicate function returns True when passed 
    /// the aggregate state). The fold function takes the state argument, and applies the function 
    /// 'folder' to it and the first element of the sequence. Then, it feeds this result into the 
    /// function 'folder' along with the second element, and so on. It returns the final result. 
    /// </summary>
    /// <typeparam name="S">State type</typeparam>
    /// <typeparam name="T">sequence item type</typeparam>
    /// <param name="list">sequence to fold</param>
    /// <param name="state">Initial state</param>
    /// <param name="folder">Fold function</param>
    /// <param name="predstate">Predicate function</param>
    /// <returns>Aggregate value</returns>
    [Pure]
    public static S FoldWhile<S, T>(this Seq<T> list, S state, Func<S, T, S> folder, Func<S, bool> predstate) =>
        LanguageExt.Seq.foldWhile(list, state, folder, predstate: predstate);

    /// <summary>
    /// Applies a function 'folder' to each element of the sequence (from last element to first)
    /// whilst the predicate function returns True for the item being processed, threading an 
    /// aggregate state through the computation. The fold function takes the state argument, and 
    /// applies the function 'folder' to it and the first element of the sequence. Then, it feeds this 
    /// result into the function 'folder' along with the second element, and so on. It returns the 
    /// final result.
    /// </summary>
    /// <typeparam name="S">State type</typeparam>
    /// <typeparam name="T">sequence item type</typeparam>
    /// <param name="list">sequence to fold</param>
    /// <param name="state">Initial state</param>
    /// <param name="folder">Fold function</param>
    /// <param name="preditem">Predicate function</param>
    /// <returns>Aggregate value</returns>
    [Pure]
    public static S FoldBackWhile<S, T>(this Seq<T> list, S state, Func<S, T, S> folder, Func<T, bool> preditem) =>
        LanguageExt.Seq.foldBackWhile(list, state, folder, preditem: preditem);

    /// <summary>
    /// Applies a function 'folder' to each element of the sequence (from last element to first), 
    /// threading an accumulator argument through the computation (and whilst the predicate function 
    /// returns True when passed the aggregate state). The fold function takes the state argument, 
    /// and applies the function 'folder' to it and the first element of the sequence. Then, it feeds 
    /// this result into the function 'folder' along with the second element, and so on. It returns 
    /// the final result.
    /// </summary>
    /// <typeparam name="S">State type</typeparam>
    /// <typeparam name="T">sequence item type</typeparam>
    /// <param name="list">sequence to fold</param>
    /// <param name="state">Initial state</param>
    /// <param name="folder">Fold function</param>
    /// <param name="predstate">Predicate function</param>
    /// <returns>Aggregate value</returns>
    [Pure]
    public static S FoldBackWhile<S, T>(this Seq<T> list, S state, Func<S, T, S> folder, Func<S, bool> predstate) =>
        LanguageExt.Seq.foldBackWhile(list, state, folder, predstate: predstate);

    /// <summary>
    /// Applies a function 'folder' to each element of the sequence whilst the predicate function 
    /// returns False for the item being processed, threading an aggregate state through the 
    /// computation. The fold function takes the state argument, and applies the function 'folder' 
    /// to it and the first element of the sequence. Then, it feeds this result into the function 'folder' 
    /// along with the second element, and so on. It returns the final result.
    /// </summary>
    /// <typeparam name="S">State type</typeparam>
    /// <typeparam name="T">sequence item type</typeparam>
    /// <param name="list">sequence to fold</param>
    /// <param name="state">Initial state</param>
    /// <param name="folder">Fold function</param>
    /// <param name="preditem">Predicate function</param>
    /// <returns>Aggregate value</returns>
    [Pure]
    public static S FoldUntil<S, T>(this Seq<T> list, S state, Func<S, T, S> folder, Func<T, bool> preditem) =>
        LanguageExt.Seq.foldUntil(list, state, folder, preditem: preditem);

    /// <summary>
    /// Applies a function 'folder' to each element of the sequence, threading an accumulator 
    /// argument through the computation (and whilst the predicate function returns False when passed 
    /// the aggregate state). The fold function takes the state argument, and applies the function 
    /// 'folder' to it and the first element of the sequence. Then, it feeds this result into the 
    /// function 'folder' along with the second element, and so on. It returns the final result. 
    /// </summary>
    /// <typeparam name="S">State type</typeparam>
    /// <typeparam name="T">Enumerable item type</typeparam>
    /// <param name="list">Enumerable to fold</param>
    /// <param name="state">Initial state</param>
    /// <param name="folder">Fold function</param>
    /// <param name="predstate">Predicate function</param>
    /// <returns>Aggregate value</returns>
    [Pure]
    public static S FoldUntil<S, T>(this Seq<T> list, S state, Func<S, T, S> folder, Func<S, bool> predstate) =>
        LanguageExt.Seq.foldUntil(list, state, folder, predstate: predstate);

    /// <summary>
    /// Applies a function 'folder' to each element of the sequence (from last element to first)
    /// whilst the predicate function returns False for the item being processed, threading an 
    /// aggregate state through the computation. The fold function takes the state argument, and 
    /// applies the function 'folder' to it and the first element of the sequence. Then, it feeds this 
    /// result into the function 'folder' along with the second element, and so on. It returns the 
    /// final result.
    /// </summary>
    /// <typeparam name="S">State type</typeparam>
    /// <typeparam name="T">sequence item type</typeparam>
    /// <param name="list">sequence to fold</param>
    /// <param name="state">Initial state</param>
    /// <param name="folder">Fold function</param>
    /// <param name="preditem">Predicate function</param>
    /// <returns>Aggregate value</returns>
    [Pure]
    public static S FoldBackUntil<S, T>(this Seq<T> list, S state, Func<S, T, S> folder, Func<T, bool> preditem) =>
        LanguageExt.Seq.foldBackUntil(list, state, folder, preditem: preditem);

    /// <summary>
    /// Applies a function 'folder' to each element of the sequence (from last element to first), 
    /// threading an accumulator argument through the computation (and whilst the predicate function 
    /// returns False when passed the aggregate state). The fold function takes the state argument, 
    /// and applies the function 'folder' to it and the first element of the sequence. Then, it feeds 
    /// this result into the function 'folder' along with the second element, and so on. It returns 
    /// the final result.
    /// </summary>
    /// <typeparam name="S">State type</typeparam>
    /// <typeparam name="T">sequence item type</typeparam>
    /// <param name="list">sequence to fold</param>
    /// <param name="state">Initial state</param>
    /// <param name="folder">Fold function</param>
    /// <param name="predstate">Predicate function</param>
    /// <returns>Aggregate value</returns>
    [Pure]
    public static S FoldBackUntil<S, T>(this Seq<T> list, S state, Func<S, T, S> folder, Func<S, bool> predstate) =>
        LanguageExt.Seq.foldBackUntil(list, state, folder, predstate: predstate);

    /// <summary>
    /// Applies a function to each element of the sequence, threading 
    /// an accumulator argument through the computation. This function first applies the function 
    /// to the first two elements of the sequence. Then, it passes this result into the function along 
    /// with the third element and so on. Finally, it returns the final result.
    /// </summary>
    /// <typeparam name="T">sequence item type</typeparam>
    /// <param name="list">sequence to reduce</param>
    /// <param name="reducer">Reduce function</param>
    /// <returns>Aggregate value</returns>
    [Pure]
    public static T Reduce<T>(this Seq<T> list, Func<T, T, T> reducer) =>
        LanguageExt.Seq.reduce(list, reducer);

    /// <summary>
    /// Applies a function to each element of the sequence (from last element to first), threading an accumulator argument 
    /// through the computation. This function first applies the function to the last two 
    /// elements of the sequence. Then, it passes this result into the function along with the previous
    /// element and so on. Finally, it returns the final result.
    /// </summary>
    /// <typeparam name="T">sequence item type</typeparam>
    /// <param name="list">sequence to reduce</param>
    /// <param name="reducer">Reduce function</param>
    /// <returns>Aggregate value</returns>
    [Pure]
    public static T ReduceBack<T>(this Seq<T> list, Func<T, T, T> reducer) =>
        LanguageExt.Seq.reduceBack(list, reducer);

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
        LanguageExt.Seq.scan(list, state, folder);

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
        LanguageExt.Seq.scanBack(list, state, folder);

    /// <summary>
    /// Returns Some(x) for the first item in the sequence that matches the predicate 
    /// provided, None otherwise.
    /// </summary>
    /// <typeparam name="T">sequence item type</typeparam>
    /// <param name="list">sequence to search</param>
    /// <param name="pred">Predicate</param>
    /// <returns>Some(x) for the first item in the sequence that matches the predicate 
    /// provided, None otherwise.</returns>
    [Pure]
    public static Option<T> Find<T>(this Seq<T> list, Func<T, bool> pred) =>
        LanguageExt.Seq.find(list, pred);

    /// <summary>
    /// Returns [x] for the first item in the sequence that matches the predicate 
    /// provided, [] otherwise.
    /// </summary>
    /// <typeparam name="T">sequence item type</typeparam>
    /// <param name="list">sequence to search</param>
    /// <param name="pred">Predicate</param>
    /// <returns>[x] for the first item in the sequence that matches the predicate 
    /// provided, [] otherwise.</returns>
    [Pure]
    public static Seq<T> FindSeq<T>(this Seq<T> list, Func<T, bool> pred) =>
        LanguageExt.Seq.findSeq(list, pred);

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
        Seq(Enumerable.Zip(list, other, zipper));

    /// <summary>
    /// Joins two sequences together either into an sequence of tuples
    /// </summary>
    /// <param name="list">First sequence to join</param>
    /// <param name="other">Second sequence to join</param>
    /// <param name="zipper">Join function</param>
    /// <returns>Joined sequence of tuples</returns>
    [Pure]
    public static Seq<(T Left, U Right)> Zip<T, U>(this Seq<T> list, Seq<U> other) =>
        Seq(Enumerable.Zip(list, other, (t, u) => (t, u)));

    /// <summary>
    /// Invokes an action for each item in the sequence in order
    /// </summary>
    /// <typeparam name="T">sequence item type</typeparam>
    /// <param name="list">sequence to iterate</param>
    /// <param name="action">Action to invoke with each item</param>
    /// <returns>Unit</returns>
    public static Unit Iter<T>(this Seq<T> list, Action<T> action) =>
        LanguageExt.Seq.iter(list, action);

    /// <summary>
    /// Invokes an action for each item in the sequence in order and supplies
    /// a running index value.
    /// </summary>
    /// <typeparam name="T">sequence item type</typeparam>
    /// <param name="list">sequence to iterate</param>
    /// <param name="action">Action to invoke with each item</param>
    /// <returns>Unit</returns>
    public static Unit Iter<T>(this Seq<T> list, Action<int, T> action) =>
        LanguageExt.Seq.iter(list, action);

    /// <summary>
    /// Return a new sequence with all duplicate values removed
    /// </summary>
    /// <typeparam name="T">sequence item type</typeparam>
    /// <param name="list">sequence</param>
    /// <returns>A new sequence with all duplicate values removed</returns>
    [Pure]
    public static Seq<T> Distinct<T>(this Seq<T> list) =>
        Seq(Enumerable.Distinct(list));

    /// <summary>
    /// Return a new sequence with all duplicate values removed
    /// </summary>
    /// <typeparam name="T">sequence item type</typeparam>
    /// <param name="list">sequence</param>
    /// <returns>A new sequence with all duplicate values removed</returns>
    [Pure]
    public static Seq<T> Distinct<EQ, T>(this Seq<T> list) where EQ : struct, Eq<T> =>
        Seq(Enumerable.Distinct(list, new EqCompare<T>((x, y) => default(EQ).Equals(x, y))));

    /// <summary>
    /// Return a new sequence with all duplicate values removed
    /// </summary>
    /// <typeparam name="T">sequence item type</typeparam>
    /// <param name="list">sequence</param>
    /// <returns>A new sequence with all duplicate values removed</returns>
    [Pure]
    public static Seq<T> Distinct<T, K>(this Seq<T> list, Func<T, K> keySelector, Option<Func<K, K, bool>> compare = default(Option<Func<K, K, bool>>)) =>
         Seq(Enumerable.Distinct(list, new EqCompare<T>((a, b) => compare.IfNone(EqualityComparer<K>.Default.Equals)(keySelector(a), keySelector(b)), a => keySelector(a)?.GetHashCode() ?? 0)));

    /// <summary>
    /// Apply a sequence of values to a sequence of functions
    /// </summary>
    /// <param name="fabc">sequence of functions</param>
    /// <param name="fa">sequence of argument values</param>
    /// <returns>Returns the result of applying the sequence argument values to the sequence functions</returns>
    [Pure]
    public static Seq<B> Apply<A, B>(this Seq<Func<A, B>> fabc, Seq<A> fa) =>
        ApplSeq<A, B>.Inst.Apply(fabc, fa);

    /// <summary>
    /// Apply a sequence of values to a sequence of functions
    /// </summary>
    /// <param name="fabc">sequence of functions</param>
    /// <param name="fa">sequence of argument values</param>
    /// <returns>Returns the result of applying the sequence argument values to the sequence functions</returns>
    [Pure]
    public static Seq<B> Apply<A, B>(this Func<A, B> fabc, Seq<A> fa) =>
        ApplSeq<A, B>.Inst.Apply(fabc.Cons(), fa);

    /// <summary>
    /// Apply a sequence of values to a sequence of functions of arity 2
    /// </summary>
    /// <param name="fabc">sequence of functions</param>
    /// <param name="fa">sequence argument values</param>
    /// <returns>Returns the result of applying the sequence of argument values to the 
    /// IEnumerable of functions: a sequence of functions of arity 1</returns>
    [Pure]
    public static Seq<Func<B, C>> Apply<A, B, C>(this Seq<Func<A, B, C>> fabc, Seq<A> fa) =>
        ApplSeq<A, B, C>.Inst.Apply(fabc.Map(curry), fa);

    /// <summary>
    /// Apply a sequence of values to a sequence of functions of arity 2
    /// </summary>
    /// <param name="fabc">sequence of functions</param>
    /// <param name="fa">sequence argument values</param>
    /// <returns>Returns the result of applying the sequence of argument values to the 
    /// sequence of functions: a sequence of functions of arity 1</returns>
    [Pure]
    public static Seq<Func<B, C>> Apply<A, B, C>(this Func<A, B, C> fabc, Seq<A> fa) =>
        ApplSeq<A, B, C>.Inst.Apply(curry(fabc).Cons(), fa);

    /// <summary>
    /// Apply sequence of values to a sequence of functions of arity 2
    /// </summary>
    /// <param name="fabc">sequence of functions</param>
    /// <param name="fa">sequence argument values</param>
    /// <param name="fb">sequence argument values</param>
    /// <returns>Returns the result of applying the sequence of arguments to the sequence of functions</returns>
    [Pure]
    public static Seq<C> Apply<A, B, C>(this Seq<Func<A, B, C>> fabc, Seq<A> fa, Seq<B> fb) =>
        ApplSeq<A, B, C>.Inst.Apply(fabc.Map(curry), fa, fb);

    /// <summary>
    /// Apply sequence of values to an sequence of functions of arity 2
    /// </summary>
    /// <param name="fabc">sequence of functions</param>
    /// <param name="fa">sequence argument values</param>
    /// <param name="fb">sequence argument values</param>
    /// <returns>Returns the result of applying the sequence of arguments to the sequence of functions</returns>
    [Pure]
    public static Seq<C> Apply<A, B, C>(this Func<A, B, C> fabc, Seq<A> fa, Seq<B> fb) =>
        ApplSeq<A, B, C>.Inst.Apply(curry(fabc).Cons(), fa, fb);

    /// <summary>
    /// Apply a sequence of values to a sequence of functions of arity 2
    /// </summary>
    /// <param name="fabc">sequence of functions</param>
    /// <param name="fa">sequence argument values</param>
    /// <returns>Returns the result of applying the sequence of argument values to the 
    /// sequence of functions: a sequence of functions of arity 1</returns>
    [Pure]
    public static Seq<Func<B, C>> Apply<A, B, C>(this Seq<Func<A, Func<B, C>>> fabc, Seq<A> fa) =>
        ApplSeq<A, B, C>.Inst.Apply(fabc, fa);

    /// <summary>
    /// Apply an sequence of values to an sequence of functions of arity 2
    /// </summary>
    /// <param name="fabc">sequence of functions</param>
    /// <param name="fa">sequence argument values</param>
    /// <returns>Returns the result of applying the sequence of argument values to the 
    /// sequence of functions: a sequence of functions of arity 1</returns>
    [Pure]
    public static Seq<Func<B, C>> Apply<A, B, C>(this Func<A, Func<B, C>> fabc, Seq<A> fa) =>
        ApplSeq<A, B, C>.Inst.Apply(fabc.Cons(), fa);

    /// <summary>
    /// Apply sequence of values to an sequence of functions of arity 2
    /// </summary>
    /// <param name="fabc">sequence of functions</param>
    /// <param name="fa">sequence argument values</param>
    /// <param name="fb">sequence argument values</param>
    /// <returns>Returns the result of applying the sequence of arguments to the sequence of functions</returns>
    [Pure]
    public static Seq<C> Apply<A, B, C>(this Seq<Func<A, Func<B, C>>> fabc, Seq<A> fa, Seq<B> fb) =>
        ApplSeq<A, B, C>.Inst.Apply(fabc, fa, fb);

    /// <summary>
    /// Apply sequence of values to a sequence of functions of arity 2
    /// </summary>
    /// <param name="fabc">sequence of functions</param>
    /// <param name="fa">sequence argument values</param>
    /// <param name="fb">sequence argument values</param>
    /// <returns>Returns the result of applying the sequence of arguments to the sequence of functions</returns>
    [Pure]
    public static Seq<C> Apply<A, B, C>(this Func<A, Func<B, C>> fabc, Seq<A> fa, Seq<B> fb) =>
        ApplSeq<A, B, C>.Inst.Apply(fabc.Cons(), fa, fb);

    /// <summary>
    /// Evaluate fa, then fb, ignoring the result of fa
    /// </summary>
    /// <param name="fa">Applicative to evaluate first</param>
    /// <param name="fb">Applicative to evaluate second and then return</param>
    /// <returns>Applicative of type FB derived from Applicative of B</returns>
    [Pure]
    public static Seq<B> Action<A, B>(this Seq<A> fa, Seq<B> fb) =>
        ApplSeq<A, B>.Inst.Action(fa, fb);

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
        LanguageExt.Seq.tails(self);

    /// <summary>
    /// The tailsr function returns all final segments of the argument, longest first. For example:
    /// 
    ///     tails(['a','b','c']) == [['a','b','c'], ['b','c'], ['c'],[]]
    /// </summary>
    /// <remarks>Differs from `tails` in implementation only.  The `tailsr` uses recursive processing
    /// whereas `tails` uses a while loop aggregation followed by a reverse.  For small sequences 
    /// `tailsr` is probably more efficient.
    /// of the `Se` </remarks>
    /// <typeparam name="A">Seq item type</typeparam>
    /// <param name="self">Seq</param>
    /// <returns>Seq of Seq of A</returns>
    [Pure]
    public static Seq<Seq<A>> Tailsr<A>(this Seq<A> self) =>
        LanguageExt.Seq.tailsr(self);

    /// <summary>
    /// Span, applied to a predicate 'pred' and a list, returns a tuple where first element is 
    /// longest prefix (possibly empty) of elements that satisfy 'pred' and second element is the 
    /// remainder of the list:
    /// </summary>
    /// <example>
    /// Seq.span(List(1,2,3,4,1,2,3,4), x => x &lt; 3) == (List(1,2),List(3,4,1,2,3,4))
    /// </example>
    /// <example>
    /// Seq.span(List(1,2,3), x => x &lt; 9) == (List(1,2,3),List())
    /// </example>
    /// <example>
    /// Seq.span(List(1,2,3), x => x &lt; 0) == (List(),List(1,2,3))
    /// </example>
    /// <typeparam name="T">List element type</typeparam>
    /// <param name="self">List</param>
    /// <param name="pred">Predicate</param>
    /// <returns>Split list</returns>
    [Pure]
    public static (Seq<T>, Seq<T>) Span<T>(this Seq<T> self, Func<T, bool> pred) =>
        LanguageExt.Seq.span(self, pred);

    /// <summary>
    /// Applies an accumulator function over a sequence. The specified seed value is used as the initial accumulator value.
    /// </summary>
    [Pure]
    public static TAccumulate Aggregate<TSource, TAccumulate>(this Seq<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func) =>
        Enumerable.Aggregate(source.Value, seed, func);

    /// <summary>
    /// Applies an accumulator function over a sequence. The specified seed value is used as the initial accumulator value, and the specified function is used to select the result value.
    /// </summary>
    [Pure]
    public static TResult Aggregate<TSource, TAccumulate, TResult>(this Seq<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func, Func<TAccumulate, TResult> resultSelector) =>
        Enumerable.Aggregate(source.Value, seed, func, resultSelector);

    /// <summary>
    /// Applies an accumulator function over a sequence.
    /// </summary>
    [Pure]
    public static TSource Aggregate<TSource>(this Seq<TSource> source, Func<TSource, TSource, TSource> func) =>
        Enumerable.Aggregate(source.Value, func);

    /// <summary>
    /// Determines whether all elements of a sequence satisfy a condition.
    /// </summary>
    [Pure]
    public static bool All<TSource>(this Seq<TSource> source, Func<TSource, bool> predicate) =>
        Enumerable.All(source.Value, predicate);

    /// <summary>
    /// Determines whether a sequence contains any elements.
    /// </summary>
    [Pure]
    public static bool Any<TSource>(this Seq<TSource> source) =>
        Enumerable.Any(source.Value);

    /// <summary>
    /// Determines whether any element of a sequence satisfies a condition.
    /// </summary>
    [Pure]
    public static bool Any<TSource>(this Seq<TSource> source, Func<TSource, bool> predicate) =>
        Enumerable.Any(source.Value, predicate);

    /// <summary>
    /// Returns the input typed as IEnumerable<T>.
    /// </summary>
    [Pure]
    public static IEnumerable<TSource> AsEnumerable<TSource>(this Seq<TSource> source) =>
        Enumerable.AsEnumerable(source.Value);

    /// <summary>
    /// Converts a generic IEnumerable<T> to a generic IQueryable<T>.
    /// </summary>
    [Pure]
    public static IQueryable<TElement> AsQueryable<TElement>(this Seq<TElement> source) =>
        Queryable.AsQueryable(source.Value.AsQueryable());

    /// <summary>
    /// Computes the average of a sequence of Decimal values.
    /// </summary>
    [Pure]
    public static decimal Average(this Seq<decimal> source) =>
        Enumerable.Average(source.Value);

    /// <summary>
    /// Computes the average of a sequence of Decimal values that are obtained by invoking a transform function on each element of the input sequence.
    /// </summary>
    [Pure]
    public static decimal Average<TSource>(this Seq<TSource> source, Func<TSource, decimal> selector) =>
        Enumerable.Average(source.Value, selector);

    /// <summary>
    /// Computes the average of a sequence of nullable Decimal values.
    /// </summary>
    [Pure]
    public static decimal? Average(this Seq<decimal?> source) =>
        Enumerable.Average(source.Value);

    /// <summary>
    /// Computes the average of a sequence of nullable Decimal values that are obtained by invoking a transform function on each element of the input sequence.
    /// </summary>
    [Pure]
    public static decimal? Average<TSource>(this Seq<TSource> source, Func<TSource, decimal?> selector) =>
        Enumerable.Average(source.Value, selector);

    /// <summary>
    /// Computes the average of a sequence of Double values.
    /// </summary>
    [Pure]
    public static double Average(this Seq<double> source) =>
        Enumerable.Average(source.Value);

    /// <summary>
    /// Computes the average of a sequence of Int32 values.
    /// </summary>
    [Pure]
    public static double Average(this Seq<int> source) =>
        Enumerable.Average(source.Value);

    /// <summary>
    /// Computes the average of a sequence of Int64 values.
    /// </summary>
    [Pure]
    public static double Average(this Seq<long> source) =>
        Enumerable.Average(source.Value);

    /// <summary>
    /// Computes the average of a sequence of Double values that are obtained by invoking a transform function on each element of the input sequence.
    /// </summary>
    [Pure]
    public static double Average<TSource>(this Seq<TSource> source, Func<TSource, double> selector) =>
        Enumerable.Average(source.Value, selector);

    /// <summary>
    /// Computes the average of a sequence of Int32 values that are obtained by invoking a transform function on each element of the input sequence.
    /// </summary>
    [Pure]
    public static double Average<TSource>(this Seq<TSource> source, Func<TSource, int> selector) =>
        Enumerable.Average(source.Value, selector);

    /// <summary>
    /// Computes the average of a sequence of Int64 values that are obtained by invoking a transform function on each element of the input sequence.
    /// </summary>
    [Pure]
    public static double Average<TSource>(this Seq<TSource> source, Func<TSource, long> selector) =>
        Enumerable.Average(source.Value, selector);

    /// <summary>
    /// Computes the average of a sequence of nullable Double values.
    /// </summary>
    [Pure]
    public static double? Average(this Seq<double?> source) =>
        Enumerable.Average(source.Value);

    /// <summary>
    /// Computes the average of a sequence of nullable Int32 values.
    /// </summary>
    [Pure]
    public static double? Average(this Seq<int?> source) =>
        Enumerable.Average(source.Value);

    /// <summary>
    /// Computes the average of a sequence of nullable Int64 values.
    /// </summary>
    [Pure]
    public static double? Average(this Seq<long?> source) =>
        Enumerable.Average(source.Value);

    /// <summary>
    /// Computes the average of a sequence of nullable Double values that are obtained by invoking a transform function on each element of the input sequence.
    /// </summary>
    [Pure]
    public static double? Average<TSource>(this Seq<TSource> source, Func<TSource, double?> selector) =>
        Enumerable.Average(source.Value, selector);

    /// <summary>
    /// Computes the average of a sequence of nullable Int32 values that are obtained by invoking a transform function on each element of the input sequence.
    /// </summary>
    [Pure]
    public static double? Average<TSource>(this Seq<TSource> source, Func<TSource, int?> selector) =>
        Enumerable.Average(source.Value, selector);

    /// <summary>
    /// Computes the average of a sequence of nullable Int64 values that are obtained by invoking a transform function on each element of the input sequence.
    /// </summary>
    [Pure]
    public static double? Average<TSource>(this Seq<TSource> source, Func<TSource, long?> selector) =>
        Enumerable.Average(source.Value, selector);

    /// <summary>
    /// Computes the average of a sequence of Single values.
    /// </summary>
    [Pure]
    public static float Average(this Seq<float> source) =>
        Enumerable.Average(source.Value);

    /// <summary>
    /// Computes the average of a sequence of Single values that are obtained by invoking a transform function on each element of the input sequence.
    /// </summary>
    [Pure]
    public static float Average<TSource>(this Seq<TSource> source, Func<TSource, float> selector) =>
        Enumerable.Average(source.Value, selector);

    /// <summary>
    /// Computes the average of a sequence of nullable Single values.
    /// </summary>
    [Pure]
    public static float? Average(this Seq<float?> source) =>
        Enumerable.Average(source.Value);

    /// <summary>
    /// Computes the average of a sequence of nullable Single values that are obtained by invoking a transform function on each element of the input sequence.
    /// </summary>
    [Pure]
    public static float? Average<TSource>(this Seq<TSource> source, Func<TSource, float?> selector) =>
        Enumerable.Average(source.Value, selector);

    /// <summary>
    /// Concatenates two sequences.
    /// </summary>
    [Pure]
    public static IEnumerable<TSource> Concat<TSource>(this Seq<TSource> first, IEnumerable<TSource> second) =>
        Enumerable.Concat(first.Value, second);

    /// <summary>
    /// Determines whether a sequence contains a specified element by using the default equality comparer.
    /// </summary>
    [Pure]
    public static bool Contains<TSource>(this Seq<TSource> source, TSource value) =>
        Enumerable.Contains(source.Value, value);

    /// <summary>
    /// Determines whether a sequence contains a specified element by using a specified IEqualityComparer<T>.
    /// </summary>
    [Pure]
    public static bool Contains<TSource>(this Seq<TSource> source, TSource value, IEqualityComparer<TSource> comparer) =>
        Enumerable.Contains(source.Value, value, comparer);

    /// <summary>
    /// Returns a number that represents how many elements in the specified sequence satisfy a condition.
    /// </summary>
    [Pure]
    public static int Count<TSource>(this Seq<TSource> source, Func<TSource, bool> predicate) =>
        Enumerable.Count(source.Value, predicate);

    /// <summary>
    /// Returns the elements of the specified sequence or the type parameter's default value in a singleton collection if the sequence is empty.
    /// </summary>
    [Pure]
    public static IEnumerable<TSource> DefaultIfEmpty<TSource>(this Seq<TSource> source) =>
        Enumerable.DefaultIfEmpty(source.Value);

    /// <summary>
    /// Returns the elements of the specified sequence or the specified value in a singleton collection if the sequence is empty.
    /// </summary>
    [Pure]
    public static IEnumerable<TSource> DefaultIfEmpty<TSource>(this Seq<TSource> source, TSource defaultValue) =>
        Enumerable.DefaultIfEmpty(source.Value, defaultValue);

    /// <summary>
    /// Returns distinct elements from a sequence by using a specified IEqualityComparer<T> to compare values.
    /// </summary>
    [Pure]
    public static IEnumerable<TSource> Distinct<TSource>(this Seq<TSource> source, IEqualityComparer<TSource> comparer) =>
        Enumerable.Distinct(source.Value, comparer);

    /// <summary>
    /// Returns the element at a specified index in a sequence.
    /// </summary>
    [Pure]
    public static TSource ElementAt<TSource>(this Seq<TSource> source, int index) =>
        Enumerable.ElementAt(source.Value, index);

    /// <summary>
    /// Returns the element at a specified index in a sequence or a default value if the index is out of range.
    /// </summary>
    [Pure]
    public static TSource ElementAtOrDefault<TSource>(this Seq<TSource> source, int index) =>
        Enumerable.ElementAtOrDefault(source.Value, index);

    /// <summary>
    /// Produces the set difference of two sequences by using the default equality comparer to compare values.
    /// </summary>
    [Pure]
    public static IEnumerable<TSource> Except<TSource>(this Seq<TSource> first, IEnumerable<TSource> second) =>
        Enumerable.Except(first.Value, second);

    /// <summary>
    /// Produces the set difference of two sequences by using the specified IEqualityComparer<T> to compare values.
    /// </summary>
    [Pure]
    public static IEnumerable<TSource> Except<TSource>(this Seq<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer) =>
        Enumerable.Except(first.Value, second, comparer);

    /// <summary>
    /// Returns the first element in a sequence that satisfies a specified condition.
    /// </summary>
    [Pure]
    public static TSource First<TSource>(this Seq<TSource> source, Func<TSource, bool> predicate) =>
        Enumerable.First(source.Value, predicate);

    /// <summary>
    /// Returns the first element of the sequence that satisfies a condition or a default value if no such element is found.
    /// </summary>
    [Pure]
    public static TSource FirstOrDefault<TSource>(this Seq<TSource> source, Func<TSource, bool> predicate) =>
        Enumerable.FirstOrDefault(source.Value, predicate);

    /// <summary>
    /// Groups the elements of a sequence according to a specified key selector function and projects the elements for each group by using a specified function.
    /// </summary>
    [Pure]
    public static IEnumerable<IGrouping<TKey, TElement>> GroupBy<TSource, TKey, TElement>(this Seq<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector) =>
        Enumerable.GroupBy(source.Value, keySelector, elementSelector);

    /// <summary>
    /// Groups the elements of a sequence according to a key selector function. The keys are compared by using a comparer and each group's elements are projected by using a specified function.
    /// </summary>
    [Pure]
    public static IEnumerable<IGrouping<TKey, TElement>> GroupBy<TSource, TKey, TElement>(this Seq<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer) =>
        Enumerable.GroupBy(source.Value, keySelector, elementSelector, comparer);

    /// <summary>
    /// Groups the elements of a sequence according to a specified key selector function.
    /// </summary>
    [Pure]
    public static IEnumerable<IGrouping<TKey, TSource>> GroupBy<TSource, TKey>(this Seq<TSource> source, Func<TSource, TKey> keySelector) =>
        Enumerable.GroupBy(source.Value, keySelector);

    /// <summary>
    /// Groups the elements of a sequence according to a specified key selector function and compares the keys by using a specified comparer.
    /// </summary>
    [Pure]
    public static IEnumerable<IGrouping<TKey, TSource>> GroupBy<TSource, TKey>(this Seq<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer) =>
        Enumerable.GroupBy(source.Value, keySelector, comparer);

    /// <summary>
    /// Groups the elements of a sequence according to a specified key selector function and creates a result value from each group and its key. The elements of each group are projected by using a specified function.
    /// </summary>
    [Pure]
    public static IEnumerable<TResult> GroupBy<TSource, TKey, TElement, TResult>(this Seq<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, Func<TKey, IEnumerable<TElement>, TResult> resultSelector) =>
        Enumerable.GroupBy(source.Value, keySelector, elementSelector, resultSelector);

    /// <summary>
    /// Groups the elements of a sequence according to a specified key selector function and creates a result value from each group and its key. Key values are compared by using a specified comparer, and the elements of each group are projected by using a specified function.
    /// </summary>
    [Pure]
    public static IEnumerable<TResult> GroupBy<TSource, TKey, TElement, TResult>(this Seq<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, Func<TKey, IEnumerable<TElement>, TResult> resultSelector, IEqualityComparer<TKey> comparer) =>
        Enumerable.GroupBy(source.Value, keySelector, elementSelector, resultSelector, comparer);

    /// <summary>
    /// Groups the elements of a sequence according to a specified key selector function and creates a result value from each group and its key.
    /// </summary>
    [Pure]
    public static IEnumerable<TResult> GroupBy<TSource, TKey, TResult>(this Seq<TSource> source, Func<TSource, TKey> keySelector, Func<TKey, IEnumerable<TSource>, TResult> resultSelector) =>
        Enumerable.GroupBy(source.Value, keySelector, resultSelector);

    /// <summary>
    /// Groups the elements of a sequence according to a specified key selector function and creates a result value from each group and its key. The keys are compared by using a specified comparer.
    /// </summary>
    [Pure]
    public static IEnumerable<TResult> GroupBy<TSource, TKey, TResult>(this Seq<TSource> source, Func<TSource, TKey> keySelector, Func<TKey, IEnumerable<TSource>, TResult> resultSelector, IEqualityComparer<TKey> comparer) =>
        Enumerable.GroupBy(source.Value, keySelector, resultSelector, comparer);

    /// <summary>
    /// Correlates the elements of two sequences based on equality of keys and groups the results. The default equality comparer is used to compare keys.
    /// </summary>
    [Pure]
    public static IEnumerable<TResult> GroupJoin<TOuter, TInner, TKey, TResult>(this Seq<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector) =>
        Enumerable.GroupJoin(outer.Value, inner, outerKeySelector, innerKeySelector, resultSelector);

    /// <summary>
    /// Correlates the elements of two sequences based on key equality and groups the results. A specified IEqualityComparer<T> is used to compare keys.
    /// </summary>
    [Pure]
    public static IEnumerable<TResult> GroupJoin<TOuter, TInner, TKey, TResult>(this Seq<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector, IEqualityComparer<TKey> comparer) =>
        Enumerable.GroupJoin(outer.Value, inner, outerKeySelector, innerKeySelector, resultSelector, comparer);

    /// <summary>
    /// Produces the set intersection of two sequences by using the default equality comparer to compare values.
    /// </summary>
    [Pure]
    public static IEnumerable<TSource> Intersect<TSource>(this Seq<TSource> first, IEnumerable<TSource> second) =>
        Enumerable.Intersect(first.Value, second);

    /// <summary>
    /// Produces the set intersection of two sequences by using the specified IEqualityComparer<T> to compare values.
    /// </summary>
    [Pure]
    public static IEnumerable<TSource> Intersect<TSource>(this Seq<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer) =>
        Enumerable.Intersect(first.Value, second, comparer);

    /// <summary>
    /// Correlates the elements of two sequences based on matching keys. The default equality comparer is used to compare keys.
    /// </summary>
    [Pure]
    public static IEnumerable<TResult> Join<TOuter, TInner, TKey, TResult>(this Seq<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector) =>
        Enumerable.Join(outer.Value, inner, outerKeySelector, innerKeySelector, resultSelector);

    /// <summary>
    /// Correlates the elements of two sequences based on matching keys. A specified IEqualityComparer<T> is used to compare keys.
    /// </summary>
    [Pure]
    public static IEnumerable<TResult> Join<TOuter, TInner, TKey, TResult>(this Seq<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector, IEqualityComparer<TKey> comparer) =>
        Enumerable.Join(outer.Value, inner, outerKeySelector, innerKeySelector, resultSelector, comparer);

    /// <summary>
    /// Returns the last element of a sequence.
    /// </summary>
    [Pure]
    public static TSource Last<TSource>(this Seq<TSource> source) =>
        Enumerable.Last(source.Value);

    /// <summary>
    /// Returns the last element of a sequence that satisfies a specified condition.
    /// </summary>
    [Pure]
    public static TSource Last<TSource>(this Seq<TSource> source, Func<TSource, bool> predicate) =>
        Enumerable.Last(source.Value, predicate);

    /// <summary>
    /// Returns the last element of a sequence, or a default value if the sequence contains no elements.
    /// </summary>
    [Pure]
    public static TSource LastOrDefault<TSource>(this Seq<TSource> source) =>
        Enumerable.LastOrDefault(source.Value);

    /// <summary>
    /// Returns the last element of a sequence that satisfies a condition or a default value if no such element is found.
    /// </summary>
    [Pure]
    public static TSource LastOrDefault<TSource>(this Seq<TSource> source, Func<TSource, bool> predicate) =>
        Enumerable.LastOrDefault(source.Value, predicate);

    /// <summary>
    /// Returns an Int64 that represents the total number of elements in a sequence.
    /// </summary>
    [Pure]
    public static long LongCount<TSource>(this Seq<TSource> source) =>
        Enumerable.LongCount(source.Value);

    /// <summary>
    /// Returns an Int64 that represents how many elements in a sequence satisfy a condition.
    /// </summary>
    [Pure]
    public static long LongCount<TSource>(this Seq<TSource> source, Func<TSource, bool> predicate) =>
        Enumerable.LongCount(source.Value, predicate);

    /// <summary>
    /// Returns the maximum value in a sequence of Decimal values.
    /// </summary>
    [Pure]
    public static decimal Max(this Seq<decimal> source) =>
        Enumerable.Max(source.Value);

    /// <summary>
    /// Invokes a transform function on each element of a sequence and returns the maximum Decimal value.
    /// </summary>
    [Pure]
    public static decimal Max<TSource>(this Seq<TSource> source, Func<TSource, decimal> selector) =>
        Enumerable.Max(source.Value, selector);

    /// <summary>
    /// Returns the maximum value in a sequence of nullable Decimal values.
    /// </summary>
    [Pure]
    public static decimal? Max(this Seq<decimal?> source) =>
        Enumerable.Max(source.Value);

    /// <summary>
    /// Invokes a transform function on each element of a sequence and returns the maximum nullable Decimal value.
    /// </summary>
    [Pure]
    public static decimal? Max<TSource>(this Seq<TSource> source, Func<TSource, decimal?> selector) =>
        Enumerable.Max(source.Value, selector);

    /// <summary>
    /// Returns the maximum value in a sequence of Double values.
    /// </summary>
    [Pure]
    public static double Max(this Seq<double> source) =>
        Enumerable.Max(source.Value);

    /// <summary>
    /// Invokes a transform function on each element of a sequence and returns the maximum Double value.
    /// </summary>
    [Pure]
    public static double Max<TSource>(this Seq<TSource> source, Func<TSource, double> selector) =>
        Enumerable.Max(source.Value, selector);

    /// <summary>
    /// Returns the maximum value in a sequence of nullable Double values.
    /// </summary>
    [Pure]
    public static double? Max(this Seq<double?> source) =>
        Enumerable.Max(source.Value);

    /// <summary>
    /// Invokes a transform function on each element of a sequence and returns the maximum nullable Double value.
    /// </summary>
    [Pure]
    public static double? Max<TSource>(this Seq<TSource> source, Func<TSource, double?> selector) =>
        Enumerable.Max(source.Value, selector);

    /// <summary>
    /// Returns the maximum value in a sequence of Single values.
    /// </summary>
    [Pure]
    public static float Max(this Seq<float> source) =>
        Enumerable.Max(source.Value);

    /// <summary>
    /// Invokes a transform function on each element of a sequence and returns the maximum Single value.
    /// </summary>
    [Pure]
    public static float Max<TSource>(this Seq<TSource> source, Func<TSource, float> selector) =>
        Enumerable.Max(source.Value, selector);

    /// <summary>
    /// Returns the maximum value in a sequence of nullable Single values.
    /// </summary>
    [Pure]
    public static float? Max(this Seq<float?> source) =>
        Enumerable.Max(source.Value);

    /// <summary>
    /// Invokes a transform function on each element of a sequence and returns the maximum nullable Single value.
    /// </summary>
    [Pure]
    public static float? Max<TSource>(this Seq<TSource> source, Func<TSource, float?> selector) =>
        Enumerable.Max(source.Value, selector);

    /// <summary>
    /// Returns the maximum value in a sequence of Int32 values.
    /// </summary>
    [Pure]
    public static int Max(this Seq<int> source) =>
        Enumerable.Max(source.Value);

    /// <summary>
    /// Invokes a transform function on each element of a sequence and returns the maximum Int32 value.
    /// </summary>
    [Pure]
    public static int Max<TSource>(this Seq<TSource> source, Func<TSource, int> selector) =>
        Enumerable.Max(source.Value, selector);

    /// <summary>
    /// Returns the maximum value in a sequence of nullable Int32 values.
    /// </summary>
    [Pure]
    public static int? Max(this Seq<int?> source) =>
        Enumerable.Max(source.Value);

    /// <summary>
    /// Invokes a transform function on each element of a sequence and returns the maximum nullable Int32 value.
    /// </summary>
    [Pure]
    public static int? Max<TSource>(this Seq<TSource> source, Func<TSource, int?> selector) =>
        Enumerable.Max(source.Value, selector);

    /// <summary>
    /// Returns the maximum value in a sequence of Int64 values.
    /// </summary>
    [Pure]
    public static long Max(this Seq<long> source) =>
        Enumerable.Max(source.Value);

    /// <summary>
    /// Invokes a transform function on each element of a sequence and returns the maximum Int64 value.
    /// </summary>
    [Pure]
    public static long Max<TSource>(this Seq<TSource> source, Func<TSource, long> selector) =>
        Enumerable.Max(source.Value, selector);

    /// <summary>
    /// Returns the maximum value in a sequence of nullable Int64 values.
    /// </summary>
    [Pure]
    public static long? Max(this Seq<long?> source) =>
        Enumerable.Max(source.Value);

    /// <summary>
    /// Invokes a transform function on each element of a sequence and returns the maximum nullable Int64 value.
    /// </summary>
    [Pure]
    public static long? Max<TSource>(this Seq<TSource> source, Func<TSource, long?> selector) =>
        Enumerable.Max(source.Value, selector);

    /// <summary>
    /// Invokes a transform function on each element of a generic sequence and returns the maximum resulting value.
    /// </summary>
    [Pure]
    public static TResult Max<TSource, TResult>(this Seq<TSource> source, Func<TSource, TResult> selector) =>
        Enumerable.Max(source.Value, selector);

    /// <summary>
    /// Returns the maximum value in a generic sequence.
    /// </summary>
    [Pure]
    public static TSource Max<TSource>(this Seq<TSource> source) =>
        Enumerable.Max(source.Value);

    /// <summary>
    /// Returns the minimum value in a sequence of Decimal values.
    /// </summary>
    [Pure]
    public static decimal Min(this Seq<decimal> source) =>
        Enumerable.Min(source.Value);

    /// <summary>
    /// Invokes a transform function on each element of a sequence and returns the minimum Decimal value.
    /// </summary>
    [Pure]
    public static decimal Min<TSource>(this Seq<TSource> source, Func<TSource, decimal> selector) =>
        Enumerable.Min(source.Value, selector);

    /// <summary>
    /// Returns the minimum value in a sequence of nullable Decimal values.
    /// </summary>
    [Pure]
    public static decimal? Min(this Seq<decimal?> source) =>
        Enumerable.Min(source.Value);

    /// <summary>
    /// Invokes a transform function on each element of a sequence and returns the minimum nullable Decimal value.
    /// </summary>
    [Pure]
    public static decimal? Min<TSource>(this Seq<TSource> source, Func<TSource, decimal?> selector) =>
        Enumerable.Min(source.Value, selector);

    /// <summary>
    /// Returns the minimum value in a sequence of Double values.
    /// </summary>
    [Pure]
    public static double Min(this Seq<double> source) =>
        Enumerable.Min(source.Value);

    /// <summary>
    /// Invokes a transform function on each element of a sequence and returns the minimum Double value.
    /// </summary>
    [Pure]
    public static double Min<TSource>(this Seq<TSource> source, Func<TSource, double> selector) =>
        Enumerable.Min(source.Value, selector);

    /// <summary>
    /// Returns the minimum value in a sequence of nullable Double values.
    /// </summary>
    [Pure]
    public static double? Min(this Seq<double?> source) =>
        Enumerable.Min(source.Value);

    /// <summary>
    /// Invokes a transform function on each element of a sequence and returns the minimum nullable Double value.
    /// </summary>
    [Pure]
    public static double? Min<TSource>(this Seq<TSource> source, Func<TSource, double?> selector) =>
        Enumerable.Min(source.Value, selector);

    /// <summary>
    /// Returns the minimum value in a sequence of Single values.
    /// </summary>
    [Pure]
    public static float Min(this Seq<float> source) =>
        Enumerable.Min(source.Value);

    /// <summary>
    /// Invokes a transform function on each element of a sequence and returns the minimum Single value.
    /// </summary>
    [Pure]
    public static float Min<TSource>(this Seq<TSource> source, Func<TSource, float> selector) =>
        Enumerable.Min(source.Value, selector);

    /// <summary>
    /// Returns the minimum value in a sequence of nullable Single values.
    /// </summary>
    [Pure]
    public static float? Min(this Seq<float?> source) =>
        Enumerable.Min(source.Value);

    /// <summary>
    /// Invokes a transform function on each element of a sequence and returns the minimum nullable Single value.
    /// </summary>
    [Pure]
    public static float? Min<TSource>(this Seq<TSource> source, Func<TSource, float?> selector) =>
        Enumerable.Min(source.Value, selector);

    /// <summary>
    /// Returns the minimum value in a sequence of Int32 values.
    /// </summary>
    [Pure]
    public static int Min(this Seq<int> source) =>
        Enumerable.Min(source.Value);

    /// <summary>
    /// Invokes a transform function on each element of a sequence and returns the minimum Int32 value.
    /// </summary>
    [Pure]
    public static int Min<TSource>(this Seq<TSource> source, Func<TSource, int> selector) =>
        Enumerable.Min(source.Value, selector);

    /// <summary>
    /// Returns the minimum value in a sequence of nullable Int32 values.
    /// </summary>
    [Pure]
    public static int? Min(this Seq<int?> source) =>
        Enumerable.Min(source.Value);

    /// <summary>
    /// Invokes a transform function on each element of a sequence and returns the minimum nullable Int32 value.
    /// </summary>
    [Pure]
    public static int? Min<TSource>(this Seq<TSource> source, Func<TSource, int?> selector) =>
        Enumerable.Min(source.Value, selector);

    /// <summary>
    /// Returns the minimum value in a sequence of Int64 values.
    /// </summary>
    [Pure]
    public static long Min(this Seq<long> source) =>
        Enumerable.Min(source.Value);

    /// <summary>
    /// Invokes a transform function on each element of a sequence and returns the minimum Int64 value.
    /// </summary>
    [Pure]
    public static long Min<TSource>(this Seq<TSource> source, Func<TSource, long> selector) =>
        Enumerable.Min(source.Value, selector);

    /// <summary>
    /// Returns the minimum value in a sequence of nullable Int64 values.
    /// </summary>
    [Pure]
    public static long? Min(this Seq<long?> source) =>
        Enumerable.Min(source.Value);

    /// <summary>
    /// Invokes a transform function on each element of a sequence and returns the minimum nullable Int64 value.
    /// </summary>
    [Pure]
    public static long? Min<TSource>(this Seq<TSource> source, Func<TSource, long?> selector) =>
        Enumerable.Min(source.Value, selector);

    /// <summary>
    /// Invokes a transform function on each element of a generic sequence and returns the minimum resulting value.
    /// </summary>
    [Pure]
    public static TResult Min<TSource, TResult>(this Seq<TSource> source, Func<TSource, TResult> selector) =>
        Enumerable.Min(source.Value, selector);

    /// <summary>
    /// Returns the minimum value in a generic sequence.
    /// </summary>
    [Pure]
    public static TSource Min<TSource>(this Seq<TSource> source) =>
        Enumerable.Min(source.Value);

    /// <summary>
    /// Sorts the elements of a sequence in ascending order according to a key.
    /// </summary>
    [Pure]
    public static IOrderedEnumerable<TSource> OrderBy<TSource, TKey>(this Seq<TSource> source, Func<TSource, TKey> keySelector) =>
        Enumerable.OrderBy(source.Value, keySelector);

    /// <summary>
    /// Sorts the elements of a sequence in ascending order by using a specified comparer.
    /// </summary>
    [Pure]
    public static IOrderedEnumerable<TSource> OrderBy<TSource, TKey>(this Seq<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey> comparer) =>
        Enumerable.OrderBy(source.Value, keySelector, comparer);

    /// <summary>
    /// Sorts the elements of a sequence in descending order according to a key.
    /// </summary>
    [Pure]
    public static IOrderedEnumerable<TSource> OrderByDescending<TSource, TKey>(this Seq<TSource> source, Func<TSource, TKey> keySelector) =>
        Enumerable.OrderByDescending(source.Value, keySelector);

    /// <summary>
    /// Sorts the elements of a sequence in descending order by using a specified comparer.
    /// </summary>
    [Pure]
    public static IOrderedEnumerable<TSource> OrderByDescending<TSource, TKey>(this Seq<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey> comparer) =>
        Enumerable.OrderByDescending(source.Value, keySelector, comparer);

    /// <summary>
    /// Inverts the order of the elements in a sequence.
    /// </summary>
    [Pure]
    public static IEnumerable<TSource> Reverse<TSource>(this Seq<TSource> source) =>
        Enumerable.Reverse(source.Value);

    /// <summary>
    /// Determines whether two sequences are equal by comparing the elements by using the default equality comparer for their type.
    /// </summary>
    [Pure]
    public static bool SequenceEqual<TSource>(this Seq<TSource> first, IEnumerable<TSource> second) =>
        Enumerable.SequenceEqual(first.Value, second);

    /// <summary>
    /// Determines whether two sequences are equal by comparing their elements by using a specified IEqualityComparer<T>.
    /// </summary>
    [Pure]
    public static bool SequenceEqual<TSource>(this Seq<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer) =>
        Enumerable.SequenceEqual(first.Value, second, comparer);

    /// <summary>
    /// Returns the only element of a sequence, and throws an exception if there is not exactly one element in the sequence.
    /// </summary>
    [Pure]
    public static TSource Single<TSource>(this Seq<TSource> source) =>
        Enumerable.Single(source.Value);

    /// <summary>
    /// Returns the only element of a sequence that satisfies a specified condition, and throws an exception if more than one such element exists.
    /// </summary>
    [Pure]
    public static TSource Single<TSource>(this Seq<TSource> source, Func<TSource, bool> predicate) =>
        Enumerable.Single(source.Value, predicate);

    /// <summary>
    /// Returns the only element of a sequence, or a default value if the sequence is empty; this method throws an exception if there is more than one element in the sequence.
    /// </summary>
    [Pure]
    public static TSource SingleOrDefault<TSource>(this Seq<TSource> source) =>
        Enumerable.SingleOrDefault(source.Value);

    /// <summary>
    /// Returns the only element of a sequence that satisfies a specified condition or a default value if no such element exists; this method throws an exception if more than one element satisfies the condition.
    /// </summary>
    [Pure]
    public static TSource SingleOrDefault<TSource>(this Seq<TSource> source, Func<TSource, bool> predicate) =>
        Enumerable.SingleOrDefault(source.Value, predicate);

    /// <summary>
    /// Bypasses a specified number of elements in a sequence and then returns the remaining elements.
    /// </summary>
    [Pure]
    public static IEnumerable<TSource> Skip<TSource>(this Seq<TSource> source, int count) =>
        Enumerable.Skip(source.Value, count);

    /// <summary>
    /// Bypasses elements in a sequence as long as a specified condition is true and then returns the remaining elements.
    /// </summary>
    [Pure]
    public static IEnumerable<TSource> SkipWhile<TSource>(this Seq<TSource> source, Func<TSource, bool> predicate) =>
        Enumerable.SkipWhile(source.Value, predicate);

    /// <summary>
    /// Bypasses elements in a sequence as long as a specified condition is true and then returns the remaining elements. The element's index is used in the logic of the predicate function.
    /// </summary>
    [Pure]
    public static IEnumerable<TSource> SkipWhile<TSource>(this Seq<TSource> source, Func<TSource, int, bool> predicate) =>
        Enumerable.SkipWhile(source.Value, predicate);

    /// <summary>
    /// Computes the sum of the sequence of Decimal values that are obtained by invoking a transform function on each element of the input sequence.
    /// </summary>
    [Pure]
    public static decimal Sum<TSource>(this Seq<TSource> source, Func<TSource, decimal> selector) =>
        Enumerable.Sum(source.Value, selector);

    /// <summary>
    /// Computes the sum of a sequence of nullable Decimal values.
    /// </summary>
    [Pure]
    public static decimal? Sum(this Seq<decimal?> source) =>
        Enumerable.Sum(source.Value);

    /// <summary>
    /// Computes the sum of the sequence of nullable Decimal values that are obtained by invoking a transform function on each element of the input sequence.
    /// </summary>
    [Pure]
    public static decimal? Sum<TSource>(this Seq<TSource> source, Func<TSource, decimal?> selector) =>
        Enumerable.Sum(source.Value, selector);

    /// <summary>
    /// Computes the sum of the sequence of Double values that are obtained by invoking a transform function on each element of the input sequence.
    /// </summary>
    [Pure]
    public static double Sum<TSource>(this Seq<TSource> source, Func<TSource, double> selector) =>
        Enumerable.Sum(source.Value, selector);

    /// <summary>
    /// Computes the sum of a sequence of nullable Double values.
    /// </summary>
    [Pure]
    public static double? Sum(this Seq<double?> source) =>
        Enumerable.Sum(source.Value);

    /// <summary>
    /// Computes the sum of the sequence of nullable Double values that are obtained by invoking a transform function on each element of the input sequence.
    /// </summary>
    [Pure]
    public static double? Sum<TSource>(this Seq<TSource> source, Func<TSource, double?> selector) =>
        Enumerable.Sum(source.Value, selector);

    /// <summary>
    /// Computes the sum of the sequence of Single values that are obtained by invoking a transform function on each element of the input sequence.
    /// </summary>
    [Pure]
    public static float Sum<TSource>(this Seq<TSource> source, Func<TSource, float> selector) =>
        Enumerable.Sum(source.Value, selector);

    /// <summary>
    /// Computes the sum of a sequence of nullable Single values.
    /// </summary>
    [Pure]
    public static float? Sum(this Seq<float?> source) =>
        Enumerable.Sum(source.Value);

    /// <summary>
    /// Computes the sum of the sequence of nullable Single values that are obtained by invoking a transform function on each element of the input sequence.
    /// </summary>
    [Pure]
    public static float? Sum<TSource>(this Seq<TSource> source, Func<TSource, float?> selector) =>
        Enumerable.Sum(source.Value, selector);

    /// <summary>
    /// Computes the sum of the sequence of Int32 values that are obtained by invoking a transform function on each element of the input sequence.
    /// </summary>
    [Pure]
    public static int Sum<TSource>(this Seq<TSource> source, Func<TSource, int> selector) =>
        Enumerable.Sum(source.Value, selector);

    /// <summary>
    /// Computes the sum of a sequence of nullable Int32 values.
    /// </summary>
    [Pure]
    public static int? Sum(this Seq<int?> source) =>
        Enumerable.Sum(source.Value);

    /// <summary>
    /// Computes the sum of the sequence of nullable Int32 values that are obtained by invoking a transform function on each element of the input sequence.
    /// </summary>
    [Pure]
    public static int? Sum<TSource>(this Seq<TSource> source, Func<TSource, int?> selector) =>
        Enumerable.Sum(source.Value, selector);

    /// <summary>
    /// Computes the sum of a sequence of Int64 values.
    /// </summary>
    [Pure]
    public static long Sum(this Seq<long> source) =>
        Enumerable.Sum(source.Value);

    /// <summary>
    /// Computes the sum of the sequence of Int64 values that are obtained by invoking a transform function on each element of the input sequence.
    /// </summary>
    [Pure]
    public static long Sum<TSource>(this Seq<TSource> source, Func<TSource, long> selector) =>
        Enumerable.Sum(source.Value, selector);

    /// <summary>
    /// Computes the sum of a sequence of nullable Int64 values.
    /// </summary>
    [Pure]
    public static long? Sum(this Seq<long?> source) =>
        Enumerable.Sum(source.Value);

    /// <summary>
    /// Computes the sum of the sequence of nullable Int64 values that are obtained by invoking a transform function on each element of the input sequence.
    /// </summary>
    [Pure]
    public static long? Sum<TSource>(this Seq<TSource> source, Func<TSource, long?> selector) =>
        Enumerable.Sum(source.Value, selector);

    /// <summary>
    /// Returns a specified number of contiguous elements from the start of a sequence.
    /// </summary>
    [Pure]
    public static IEnumerable<TSource> Take<TSource>(this Seq<TSource> source, int count) =>
        Enumerable.Take(source.Value, count);

    /// <summary>
    /// Returns elements from a sequence as long as a specified condition is true.
    /// </summary>
    [Pure]
    public static IEnumerable<TSource> TakeWhile<TSource>(this Seq<TSource> source, Func<TSource, bool> predicate) =>
        Enumerable.TakeWhile(source.Value, predicate);

    /// <summary>
    /// Returns elements from a sequence as long as a specified condition is true. The element's index is used in the logic of the predicate function.
    /// </summary>
    [Pure]
    public static IEnumerable<TSource> TakeWhile<TSource>(this Seq<TSource> source, Func<TSource, int, bool> predicate) =>
        Enumerable.TakeWhile(source.Value, predicate);

    /// <summary>
    /// Creates an array from a IEnumerable<T>.
    /// </summary>
    [Pure]
    public static TSource[] ToArray<TSource>(this Seq<TSource> source) =>
        Enumerable.ToArray(source.Value);

    /// <summary>
    /// Creates a Dictionary<TKey,TValue> from an IEnumerable<T> according to specified key selector and element selector functions.
    /// </summary>
    [Pure]
    public static Dictionary<TKey, TElement> ToDictionary<TSource, TKey, TElement>(this Seq<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector) =>
        Enumerable.ToDictionary(source.Value, keySelector, elementSelector);

    /// <summary>
    /// Creates a Dictionary<TKey,TValue> from an IEnumerable<T> according to a specified key selector function, a comparer, and an element selector function.
    /// </summary>
    [Pure]
    public static Dictionary<TKey, TElement> ToDictionary<TSource, TKey, TElement>(this Seq<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer) =>
        Enumerable.ToDictionary(source.Value, keySelector, elementSelector, comparer);

    /// <summary>
    /// Creates a Dictionary<TKey,TValue> from an IEnumerable<T> according to a specified key selector function.
    /// </summary>
    [Pure]
    public static Dictionary<TKey, TSource> ToDictionary<TSource, TKey>(this Seq<TSource> source, Func<TSource, TKey> keySelector) =>
        Enumerable.ToDictionary(source.Value, keySelector);

    /// <summary>
    /// Creates a Dictionary<TKey,TValue> from an IEnumerable<T> according to a specified key selector function and key comparer.
    /// </summary>
    [Pure]
    public static Dictionary<TKey, TSource> ToDictionary<TSource, TKey>(this Seq<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer) =>
        Enumerable.ToDictionary(source.Value, keySelector, comparer);

    /// <summary>
    /// Creates a List<T> from an IEnumerable<T>.
    /// </summary>
    [Pure]
    public static List<TSource> ToList<TSource>(this Seq<TSource> source) =>
        Enumerable.ToList(source.Value);

    /// <summary>
    /// Creates a Lookup<TKey,TElement> from an IEnumerable<T> according to specified key selector and element selector functions.
    /// </summary>
    [Pure]
    public static ILookup<TKey, TElement> ToLookup<TSource, TKey, TElement>(this Seq<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector) =>
        Enumerable.ToLookup(source.Value, keySelector, elementSelector);

    /// <summary>
    /// Creates a Lookup<TKey,TElement> from an IEnumerable<T> according to a specified key selector function, a comparer and an element selector function.
    /// </summary>
    [Pure]
    public static ILookup<TKey, TElement> ToLookup<TSource, TKey, TElement>(this Seq<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer) =>
        Enumerable.ToLookup(source.Value, keySelector, elementSelector, comparer);

    /// <summary>
    /// Creates a Lookup<TKey,TElement> from an IEnumerable<T> according to a specified key selector function.
    /// </summary>
    [Pure]
    public static ILookup<TKey, TSource> ToLookup<TSource, TKey>(this Seq<TSource> source, Func<TSource, TKey> keySelector) =>
        Enumerable.ToLookup(source.Value, keySelector);

    /// <summary>
    /// Creates a Lookup<TKey,TElement> from an IEnumerable<T> according to a specified key selector function and key comparer.
    /// </summary>
    [Pure]
    public static ILookup<TKey, TSource> ToLookup<TSource, TKey>(this Seq<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer) =>
        Enumerable.ToLookup(source.Value, keySelector, comparer);

    /// <summary>
    /// Produces the set union of two sequences by using the default equality comparer.
    /// </summary>
    [Pure]
    public static IEnumerable<TSource> Union<TSource>(this Seq<TSource> first, IEnumerable<TSource> second) =>
        Enumerable.Union(first.Value, second);

    /// <summary>
    /// Produces the set union of two sequences by using a specified IEqualityComparer<T>.
    /// </summary>
    [Pure]
    public static IEnumerable<TSource> Union<TSource>(this Seq<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer) =>
        Enumerable.Union(first.Value, second, comparer);

    /// <summary>
    /// Applies a specified function to the corresponding elements of two sequences, producing a sequence of the results.
    /// </summary>
    [Pure]
    public static IEnumerable<TResult> Zip<TFirst, TSecond, TResult>(this Seq<TFirst> first, IEnumerable<TSecond> second, Func<TFirst, TSecond, TResult> resultSelector) =>
        Enumerable.Zip(first.Value, second, resultSelector);
}
