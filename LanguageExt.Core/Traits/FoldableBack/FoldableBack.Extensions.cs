using System;
using System.Collections.Generic;
using LanguageExt.ClassInstances;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static partial class FoldableBackExtensions
{
    /// <param name="ta">Foldable structure</param>
    /// <typeparam name="A">Value type</typeparam>
    extension<T, A>(K<T, A> ta)
        where T : Foldable<T>, FoldableBack<T>
    {
        /// <summary>
        /// Find the last index of an element in the structure that matches the element provided
        /// </summary>
        /// <param name="index">Initial index to start the search</param>
        /// <param name="count">Maximum number of elements to test before giving up</param>
        /// <param name="item">Element to search for</param>
        /// <returns>`Some(index)` if the predicate returns `true`, otherwise `None`</returns>
        public Option<LongIndex> IndexOf(
            LongIndex index, 
            long count, 
            Func<A, bool> predicate) =>
            index.IsFromEnd
                ? T.IndexOfBack(index.Value - 1, count, predicate, ta) * (ix => LongIndex.FromEnd(ix + 1))
                : T.IndexOf(index.Value, count, predicate, ta)         * LongIndex.FromStart;

        /// <summary>
        /// Find the last index of an element in the structure that matches the element provided
        /// </summary>
        /// <param name="index">Initial index to start the search</param>
        /// <param name="item">Element to search for</param>
        /// <returns>`Some(index)` if the predicate returns `true`, otherwise `None`</returns>
        public Option<LongIndex> IndexOf(
            LongIndex index, 
            Func<A, bool> predicate) =>
            index.IsFromEnd
                ? T.IndexOfBack(index.Value - 1, None, predicate, ta) * (ix => LongIndex.FromEnd(ix + 1))
                : T.IndexOf(index.Value, None, predicate, ta)         * LongIndex.FromStart;

        /// <summary>
        /// Find the first element that matches the predicate
        /// </summary>
        public Option<A> Find(LongIndex index, long count, Func<A, bool> predicate) =>
            index.IsFromEnd
                ? T.FindBack(index.Value - 1, count, predicate, ta)
                : T.Find(index.Value, count, predicate, ta);
    
        /// <summary>
        /// Find the first element that matches the predicate
        /// </summary>
        public Option<A> Find(LongIndex index, Func<A, bool> predicate) =>
            index.IsFromEnd
                ? T.FindBack(index.Value - 1, None, predicate, ta)
                : T.Find(index.Value, None, predicate, ta);
    }
    
    /// <param name="ta">Foldable structure</param>
    /// <typeparam name="A">Value type</typeparam>
    extension<EqA, T, A>(K<T, A> ta)
        where T : Foldable<T>, FoldableBack<T>
        where EqA : Eq<A>
    {
        /// <summary>
        /// Find the last index of an element in the structure that matches the element provided
        /// </summary>
        /// <param name="startIndex">Initial index to start the search</param>
        /// <param name="count">Maximum number of elements to test before giving up</param>
        /// <param name="item">Element to search for</param>
        /// <returns>`Some(index)` if the predicate returns `true`, otherwise `None`</returns>
        public Option<LongIndex> IndexOf(
            LongIndex startIndex,
            long count,
            A item) =>
            startIndex.IsFromEnd
                 ? T.IndexOfBack(startIndex.Value - 1, count, x => EqA.Equals(item, x), ta) * (ix => LongIndex.FromEnd(ix + 1))
                 : T.IndexOf(startIndex.Value, count, x => EqA.Equals(item, x), ta) * LongIndex.FromStart;

        /// <summary>
        /// Find the last index of an element in the structure that matches the element provided
        /// </summary>
        /// <param name="startIndex">Initial index to start the search</param>
        /// <param name="item">Element to search for</param>
        /// <returns>`Some(index)` if the predicate returns `true`, otherwise `None`</returns>
        public Option<LongIndex> IndexOf(
            LongIndex startIndex,
            A item) =>
            startIndex.IsFromEnd
                ? T.IndexOfBack(startIndex.Value - 1, None, x => EqA.Equals(item, x), ta) * (ix => LongIndex.FromEnd(ix + 1))
                : T.IndexOf(startIndex.Value, None, x => EqA.Equals(item, x), ta) * LongIndex.FromStart;
    }
    
    /// <param name="f">Mapping operation</param>
    /// <typeparam name="T">Foldable</typeparam>
    /// <typeparam name="F">Applicative</typeparam>
    /// <typeparam name="A">Input bound value</typeparam>
    /// <typeparam name="B">Mapping bound value</typeparam>
    extension<T, F, A, B>(Func<A, K<F, B>> f)
        where T : FoldableBack<T>
        where F : Applicative<F>
    {
        /// <summary>
        /// Fold the structure: `ta` and pass each element that it yields to `f`, resulting in an `F` applicative-value.
        /// The fold operator is applicative `Action`, which causes each applicative-value to be sequenced.      
        /// </summary>
        /// <param name="ta">Foldable structure</param>
        /// <returns></returns>
        public K<F, Unit> ForBackM(K<T, A> ta) =>
            ta.FoldBack((fs, x) => fs.BackAction(f(x)), pure<F, Unit>(unit));
    }

    /// <param name="ta">Foldable structure</param>
    /// <typeparam name="T">Foldable</typeparam>
    /// <typeparam name="F">Applicative</typeparam>
    /// <typeparam name="A">Input bound value</typeparam>
    /// <typeparam name="B">Mapping bound value</typeparam>
    extension<T, F, A, B>(K<T, A> ta)
        where T : FoldableBack<T>
        where F : Applicative<F>
    {
        /// <summary>
        /// Fold the structure: `ta` and pass each element that it yields to `f`, resulting in an `F` applicative-value.
        /// The fold operator is applicative `Action`, which causes each applicative-value to be sequenced.      
        /// </summary>
        /// <param name="f">Mapping operation</param>
        /// <returns></returns>
        public K<F, Unit> ForBackM(Func<A, K<F, B>> f) =>
            T.FoldBack((fs, x) => fs.BackAction(f(x)), pure<F, Unit>(unit), ta);
    }

    /// <param name="ta">Foldable structure</param>
    /// <typeparam name="A">Value type</typeparam>
    extension<T, A>(K<T, A> ta)
        where T : FoldableBack<T>
    {
        /// <summary>
        /// Fold until the `Option` returns `None`
        /// </summary>
        /// <param name="f">Fold function</param>
        /// <param name="initialState">Initial state for the fold</param>
        /// <typeparam name="S">State type</typeparam>
        /// <returns>Aggregated value</returns>
        public S FoldBackMaybe<S>(
            Func<S, A, Option<S>> f,
            S initialState) =>
            T.FoldBackMaybe(f, initialState, ta);

        /// <summary>
        /// Same behaviour as `FoldBack` but allows early exit of the operation once
        /// the predicate function becomes `false` for the state/value pair 
        /// </summary>
        public S FoldBackWhile<S>(
            Func<S, A, S> f,
            Func<(S State, A Value), bool> predicate,
            S initialState) =>
            T.FoldBackWhile(f, predicate, initialState, ta);

        /// <summary>
        /// Same behaviour as `FoldBack` but the fold operation returns a monadic type and allows
        /// early exit of the operation once the predicate function becomes `false` for the
        /// state/value pair 
        /// </summary>
        public K<M, S> FoldBackWhileM<M, S>(
            Func<S, A, K<M, S>> f,
            Func<(S State, A Value), bool> predicate,
            S initialState)
            where M : Monad<M> =>
            T.FoldBackWhileM<K<M, S>, M, A, S>(f, predicate, initialState, ta);

        /// <summary>
        /// Same behaviour as `FoldBack` but allows early exit of the operation once
        /// the predicate function becomes `false` for the state/value pair
        /// </summary>
        public S FoldBackUntil<S>(
            Func<S, A, S> f,
            Func<(S State, A Value), bool> predicate,
            S initialState) =>
            T.FoldBackUntil(f, predicate, initialState, ta);

        /// <summary>
        /// Same behaviour as `FoldBack` but the fold operation returns a monadic type and allows
        /// early exit of the operation once the predicate function becomes `false` for the
        /// state/value pair 
        /// </summary>
        public K<M, S> FoldBackUntilM<M, S>(
            Func<S, A, K<M, S>> f,
            Func<(S State, A Value), bool> predicate,
            S initialState)
            where M : Monad<M> =>
            T.FoldBackUntilM<K<M, S>, M, A, S>(f, predicate, initialState, ta);

        /// <summary>
        /// Left-associative fold of a structure, lazy in the accumulator.  This
        /// is rarely what you want, but can work well for structures with efficient
        /// right-to-left sequencing and an operator that is lazy in its left
        /// argument.
        /// 
        /// In the case of lists, 'FoldLeft', when applied to a binary operator, a
        /// starting value (typically the left-identity of the operator), and a
        /// list, reduces the list using the binary operator, from left to right
        /// </summary>
        /// <remarks>
        /// Note that to produce the outermost application of the operator the
        /// entire input list must be traversed.  Like all left-associative folds,
        /// `FoldBack` will diverge if given an infinite list.
        /// </remarks>
        public S FoldBack<S>(Func<S, A, S> f, S initialState) =>
            T.FoldBack(f, initialState, ta);

        /// <summary>
        /// Left-associative fold of a structure, lazy in the accumulator.  This
        /// is rarely what you want, but can work well for structures with efficient
        /// right-to-left sequencing and an operator that is lazy in its left
        /// argument.
        /// 
        /// In the case of lists, 'FoldLeft', when applied to a binary operator, a
        /// starting value (typically the left-identity of the operator), and a
        /// list, reduces the list using the binary operator, from left to right
        /// </summary>
        /// <remarks>
        /// Note that to produce the outermost application of the operator the
        /// entire input list must be traversed.  Like all left-associative folds,
        /// `FoldBack` will diverge if given an infinite list.
        /// </remarks>
        public K<M, S> FoldBackM<M, S>(
            Func<S, A, K<M, S>> f,
            S initialState)
            where M : Monad<M> =>
            T.FoldBackM<K<M, S>, M, A, S>(f, initialState, ta);

        /// <summary>
        /// List of elements of a structure, from left to right
        /// </summary>
        public Seq<A> ToSeqBack() =>
            T.ToSeqBack(ta);

        /// <summary>
        /// List of elements of a structure, from left to right
        /// </summary>
        public Lst<A> ToLstBack() =>
            T.ToLstBack(ta);

        /// <summary>
        /// List of elements of a structure, from left to right
        /// </summary>
        public Arr<A> ToArrBack() =>
            T.ToArrBack(ta);

        /// <summary>
        /// List of elements of a structure, from left to right
        /// </summary>
        public Iterable<A> ToIterableBack() =>
            T.ToIterableBack(ta);

        /// <summary>
        /// Does an element that fits the predicate occur in the structure?
        /// </summary>
        public bool ExistsBack(Func<A, bool> predicate) =>
            T.ExistsBack(predicate, ta);

        /// <summary>
        /// Does the predicate hold for all elements in the structure?
        /// </summary>
        public bool ForAllBack(Func<A, bool> predicate) =>
            T.ForAllBack(predicate, ta);

        /// <summary>
        /// Does the element exist in the structure?
        /// </summary>
        public bool ContainsBack(A value) =>
            T.ContainsBack(value, ta);

        /// <summary>
        /// Find the first element that matches the predicate
        /// </summary>
        public Option<A> FindBack(long endIndex, long count, Func<A, bool> predicate) =>
            T.FindBack(endIndex, count, predicate, ta);
    
        /// <summary>
        /// Find the first element that matches the predicate
        /// </summary>
        public Option<A> FindBack(long endIndex, Func<A, bool> predicate) =>
            T.FindBack(endIndex, None, predicate, ta);
    
        /// <summary>
        /// Find the first element that matches the predicate
        /// </summary>
        public Option<A> FindBack(Func<A, bool> predicate) =>
            T.FindBack(None, None, predicate, ta);

        /// <summary>
        /// Get the head item in the foldable or `None`
        /// </summary>
        public Option<A> Last =>
            T.Last(ta);

        /// <summary>
        /// Find the last index of an element in the structure that matches the predicate
        /// </summary>
        /// <param name="endIndex">Initial index to start the search (from the end of the foldable structure)</param>
        /// <param name="count">Maximum number of elements to test before giving up</param>
        /// <param name="ta">Foldable structure</param>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>`Some(index)` if the predicate returns `true`, otherwise `None`</returns>
        public Option<long> IndexOfBack(long endIndex, long count, Func<A, bool> predicate) =>
            T.IndexOfBack(endIndex, count, predicate, ta);    

        /// <summary>
        /// Find the last index of an element in the structure that matches the predicate
        /// </summary>
        /// <param name="endIndex">Initial index to start the search (from the end of the foldable structure)</param>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>`Some(index)` if the predicate returns `true`, otherwise `None`</returns>
        public Option<long> IndexOfBack(long endIndex, Func<A, bool> predicate) =>
            T.IndexOfBack(endIndex, None, predicate, ta);    

        /// <summary>
        /// Find the last index of an element in the structure that matches the predicate
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>`Some(index)` if the predicate returns `true`, otherwise `None`</returns>
        public Option<long> IndexOfBack(Func<A, bool> predicate) =>
            T.IndexOfBack(None, None, predicate, ta);    

        /// <summary>
        /// Find the last index of an element in the structure that matches the element provided
        /// </summary>
        /// <param name="endIndex">Initial index to start the search (from the end of the foldable structure)</param>
        /// <param name="count">Maximum number of elements to test before giving up</param>
        /// <param name="item">Element to search for</param>
        /// <param name="eq">Equality comparer</param>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>`Some(index)` if the predicate returns `true`, otherwise `None`</returns>
        public Option<long> IndexOfBack(long endIndex, long count, A item, IEqualityComparer<A> eq) =>
            T.IndexOfBack(endIndex, count, x => eq.Equals(item, x), ta);    

        /// <summary>
        /// Find the last index of an element in the structure that matches the element provided
        /// </summary>
        /// <param name="endIndex">Initial index to start the search (from the end of the foldable structure)</param>
        /// <param name="item">Element to search for</param>
        /// <param name="eq">Equality comparer</param>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>`Some(index)` if the predicate returns `true`, otherwise `None`</returns>
        public Option<long> IndexOfBack(long endIndex, A item, IEqualityComparer<A> eq) =>
            T.IndexOfBack(endIndex, None, x => eq.Equals(item, x), ta);    

        /// <summary>
        /// Find the last index of an element in the structure that matches the element provided
        /// </summary>
        /// <param name="item">Element to search for</param>
        /// <param name="eq">Equality comparer</param>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>`Some(index)` if the predicate returns `true`, otherwise `None`</returns>
        public Option<long> IndexOfBack(A item, IEqualityComparer<A> eq) => 
            T.IndexOfBack(None, None, x => eq.Equals(item, x), ta);    

        /// <summary>
        /// Find the last index of an element in the structure that matches the element provided
        /// </summary>
        /// <param name="endIndex">Initial index to start the search (from the end of the foldable structure)</param>
        /// <param name="count">Maximum number of elements to test before giving up</param>
        /// <param name="item">Element to search for</param>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>`Some(index)` if the predicate returns `true`, otherwise `None`</returns>
        public Option<long> IndexOfBack(long endIndex, long count, A item) =>
            ta.IndexOfBack<EqDefault<A>, T, A>(endIndex, count, item);    

        /// <summary>
        /// Find the last index of an element in the structure that matches the element provided
        /// </summary>
        /// <param name="endIndex">Initial index to start the search (from the end of the foldable structure)</param>
        /// <param name="item">Element to search for</param>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>`Some(index)` if the predicate returns `true`, otherwise `None`</returns>
        public Option<long> IndexOfBack(long endIndex, A item) =>
            ta.IndexOfBack<EqDefault<A>, T, A>(endIndex, item);    

        /// <summary>
        /// Find the last index of an element in the structure that matches the element provided
        /// </summary>
        /// <param name="item">Element to search for</param>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>`Some(index)` if the predicate returns `true`, otherwise `None`</returns>
        public Option<long> IndexOfBack(A item) => 
            ta.IndexOfBack<EqDefault<A>, T, A>(item);    
        
        /// <summary>
        /// Partition a foldable into two sequences based on a predicate
        /// </summary>
        /// <param name="f">Predicate function</param>
        /// <param name="ta">Foldable structure</param>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>Partitioned structure</returns>
        public (Arr<A> True, Arr<A> False) PartitionBack(Func<A, bool> f) =>
            T.PartitionBack(f, ta);
    }
    
    /// <param name="ta">Foldable structure</param>
    /// <typeparam name="A">Value type</typeparam>
    extension<EqA, T, A>(K<T, A> ta)
        where T : FoldableBack<T>
        where EqA : Eq<A> 
    {
        /// <summary>
        /// Does the element exist in the structure?
        /// </summary>
        public bool ContainsBack(A value) =>
            T.ContainsBack<EqA, A>(value, ta);
        
        /// <summary>
        /// Find the last index of an element in the structure that matches the element provided
        /// </summary>
        /// <param name="endIndex">Initial index to start the search</param>
        /// <param name="count">Maximum number of elements to test before giving up</param>
        /// <param name="item">Element to search for</param>
        /// <returns>`Some(index)` if the predicate returns `true`, otherwise `None`</returns>
        public Option<long> IndexOfBack(long endIndex, long count, A item) =>
            T.IndexOfBack(endIndex, count, x => EqA.Equals(item, x), ta);    

        /// <summary>
        /// Find the last index of an element in the structure that matches the element provided
        /// </summary>
        /// <param name="endIndex">Initial index to start the search</param>
        /// <param name="item">Element to search for</param>
        /// <returns>`Some(index)` if the predicate returns `true`, otherwise `None`</returns>
        public Option<long> IndexOfBack(long endIndex, A item) => 
            T.IndexOfBack(endIndex, None, x => EqA.Equals(item, x), ta);    

        /// <summary>
        /// Find the last index of an element in the structure that matches the element provided
        /// </summary>
        /// <param name="item">Element to search for</param>
        /// <returns>`Some(index)` if the predicate returns `true`, otherwise `None`</returns>
        public Option<long> IndexOfBack(A item) =>
            T.IndexOfBack(None, None, x => EqA.Equals(item, x), ta);    
    }
    
    /// <param name="ta">Foldable structure</param>
    /// <typeparam name="T">Foldable type</typeparam>
    /// <typeparam name="A">Bound values</typeparam>
    extension<T, A>(K<T, A> ta)
        where T : FoldableBack<T>
        where A : Monoid<A>
    {
        /// <summary>
        /// Given a structure with elements whose type is a `Monoid`, combine them
        /// via the monoid's `Append` operator.  This fold is right-associative and
        /// lazy in the accumulator.  When you need a strict left-associative fold,
        /// use `FoldMap` instead, with `identity` as the map.
        /// </summary>
        public A FoldBack() =>
            T.FoldBack((s, x) => s + x, A.Empty, ta);

        /// <summary>
        /// Given a structure with elements whose type is a `Monoid`, combine them
        /// via the monoid's `Append` operator.  This fold is right-associative and
        /// lazy in the accumulator.  When you need a strict left-associative fold,
        /// use `FoldMap` instead, with `identity` as the map.
        /// </summary>
        public A FoldWhileBack(Func<(A State, A Value), bool> predicate) =>
            T.FoldBackWhile((s, x) => s + x, predicate, A.Empty, ta);

        /// <summary>
        /// Given a structure with elements whose type is a `Monoid`, combine them
        /// via the monoid's `Append` operator.  This fold is right-associative and
        /// lazy in the accumulator.  When you need a strict left-associative fold,
        /// use `FoldMap` instead, with `identity` as the map.
        /// </summary>
        public A FoldBackUntil(Func<(A State, A Value), bool> predicate) =>
            T.FoldBackUntil((s, x) => s + x, predicate, A.Empty, ta);
    }

}
