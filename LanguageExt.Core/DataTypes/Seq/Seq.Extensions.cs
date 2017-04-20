using LanguageExt;
using LanguageExt.ClassInstances;
using LanguageExt.TypeClasses;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using static LanguageExt.Prelude;

public static class SeqExtensions
{
    /// <summary>
    /// Get the number of items in the sequence
    /// </summary>
    public static int Count<A>(this ISeq<A> seq) =>
        seq.Count;

    /// <summary>
    /// Get the head item in the sequence
    /// </summary>
    public static A Head<A>(this ISeq<A> seq) =>
        seq.Head;

    /// <summary>
    /// Get the head item in the sequence
    /// </summary>
    public static Seq<A> Tail<A>(this ISeq<A> seq) =>
        seq.Tail;

    /// <summary>
    /// Get the head item in the sequence
    /// </summary>
    public static Option<A> HeadOrNone<A>(this ISeq<A> seq) =>
        seq.IsEmpty
            ? None
            : Some(seq.Head);

    /// <summary>
    /// Get the head item in the sequence
    /// </summary>
    public static A First<A>(this ISeq<A> seq) =>
        seq.Head;

    /// <summary>
    /// Get the head item in the sequence
    /// </summary>
    public static A FirstOrDefault<A>(this ISeq<A> seq) =>
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
    /// <param name="pred">Predicate function</param>
    /// <returns>Aggregate value</returns>
    [Pure]
    public static S FoldWhile<S, T>(this Seq<T> list, S state, Func<S, T, S> folder, Func<T, bool> pred) =>
        LanguageExt.Seq.foldWhile(list, state, folder, pred);

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
    /// <param name="pred">Predicate function</param>
    /// <returns>Aggregate value</returns>
    [Pure]
    public static S FoldWhile<S, T>(this Seq<T> list, S state, Func<S, T, S> folder, Func<S, bool> pred) =>
        LanguageExt.Seq.foldWhile(list, state, folder, pred);

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
    /// <param name="pred">Predicate function</param>
    /// <returns>Aggregate value</returns>
    [Pure]
    public static S FoldBackWhile<S, T>(this Seq<T> list, S state, Func<S, T, S> folder, Func<T, bool> pred) =>
        LanguageExt.Seq.foldBackWhile(list, state, folder, pred);

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
    /// <param name="pred">Predicate function</param>
    /// <returns>Aggregate value</returns>
    [Pure]
    public static S FoldBackWhile<S, T>(this Seq<T> list, S state, Func<S, T, S> folder, Func<S, bool> pred) =>
        LanguageExt.Seq.foldBackWhile(list, state, folder, pred);

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
    /// <param name="pred">Predicate function</param>
    /// <returns>Aggregate value</returns>
    [Pure]
    public static S FoldUntil<S, T>(this Seq<T> list, S state, Func<S, T, S> folder, Func<T, bool> pred) =>
        LanguageExt.Seq.foldUntil(list, state, folder, pred);

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
    /// <param name="pred">Predicate function</param>
    /// <returns>Aggregate value</returns>
    [Pure]
    public static S FoldUntil<S, T>(this Seq<T> list, S state, Func<S, T, S> folder, Func<S, bool> pred) =>
        LanguageExt.Seq.foldUntil(list, state, folder, pred);

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
    /// <param name="pred">Predicate function</param>
    /// <returns>Aggregate value</returns>
    [Pure]
    public static S FoldBackUntil<S, T>(this Seq<T> list, S state, Func<S, T, S> folder, Func<T, bool> pred) =>
        LanguageExt.Seq.foldBackUntil(list, state, folder, pred);

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
    /// <param name="pred">Predicate function</param>
    /// <returns>Aggregate value</returns>
    [Pure]
    public static S FoldBackUntil<S, T>(this Seq<T> list, S state, Func<S, T, S> folder, Func<S, bool> pred) =>
        LanguageExt.Seq.foldBackUntil(list, state, folder, pred);

    /// <summary>
    /// Applies a function to each element of the sequence (from last element to first), threading 
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
        LanguageExt.Seq.reduceBack(list, reducer);

    /// <summary>
    /// Applies a function to each element of the sequence, threading an accumulator argument 
    /// through the computation. This function first applies the function to the first two 
    /// elements of the sequence. Then, it passes this result into the function along with the third 
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
}
