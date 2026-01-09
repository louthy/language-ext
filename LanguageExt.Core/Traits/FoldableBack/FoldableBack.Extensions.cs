using System;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static partial class FoldableBackExtensions
{
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
        /// Find the last element that match the predicate
        /// </summary>
        public Option<A> FindBack(Func<A, bool> predicate) =>
            T.FindBack(predicate, ta);

        /// <summary>
        /// Find the elements that match the predicate
        /// </summary>
        public Iterable<A> FindAllBack(Func<A, bool> predicate) =>
            T.FindAllBack(predicate, ta);

        /// <summary>
        /// Get the head item in the foldable or `None`
        /// </summary>
        public Option<A> Last =>
            T.Last(ta);
        
        /// <summary>
        /// Find the element at the specified index or `None` if out of range
        /// </summary>
        public Option<A> AtBack(int index) =>
            T.AtBack(index, ta);

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

    extension<T, A, FS>(K<T, A> ta)
        where T : FoldableBack<T, FS>?
        where FS : allows ref struct
    {
        /// <summary>
        /// Low-level interface for folding using stack-based primitives.
        /// </summary>
        public void StepBackSetup(ref FS refState) =>
            T.FoldStepBackSetup(ta, ref refState);

        /// <summary>
        /// Low-level interface for folding using stack-based primitives.
        /// </summary>
        public bool StepBack(ref FS refState, out A value) =>
            T.FoldStepBack(ta, ref refState, out value);
    }
}
