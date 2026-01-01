using System;
using System.Numerics;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static partial class FoldableExtensions
{
    /// <param name="f">Mapping operation</param>
    /// <typeparam name="T">Foldable</typeparam>
    /// <typeparam name="F">Applicative</typeparam>
    /// <typeparam name="A">Input bound value</typeparam>
    /// <typeparam name="B">Mapping bound value</typeparam>
    extension<T, F, A, B>(Func<A, K<F, B>> f) 
        where T : Foldable<T> where F : Applicative<F>
    {
        /// <summary>
        /// Fold the structure: `ta` and pass each element that it yields to `f`, resulting in an `F` applicative-value.
        /// The fold operator is applicative `Action`, which causes each applicative-value to be sequenced.      
        /// </summary>
        /// <param name="ta">Foldable structure</param>
        /// <returns></returns>
        public K<F, Unit> ForM(K<T, A> ta) =>
            ta.Fold(pure<F, Unit>(unit), x => f(x).Action);
    }

    /// <param name="ta">Foldable structure</param>
    /// <typeparam name="T">Foldable</typeparam>
    /// <typeparam name="F">Applicative</typeparam>
    /// <typeparam name="A">Input bound value</typeparam>
    /// <typeparam name="B">Mapping bound value</typeparam>
    extension<T, F, A, B>(K<T, A> ta) 
        where T : Foldable<T> 
        where F : Applicative<F>
    {
        /// <summary>
        /// Fold the structure: `ta` and pass each element that it yields to `f`, resulting in an `F` applicative-value.
        /// The fold operator is applicative `Action`, which causes each applicative-value to be sequenced.      
        /// </summary>
        /// <param name="f">Mapping operation</param>
        /// <returns></returns>
        public K<F, Unit> ForM(Func<A, K<F, B>> f) =>
            ta.Fold(pure<F, Unit>(unit), x => f(x).Action);
    }

    /// <param name="ta">Foldable structure</param>
    /// <typeparam name="A">Value type</typeparam>
    extension<T, A>(K<T, A> ta) 
        where T : Foldable<T>
    {
        /// <summary>
        /// Runs a single step of the folding operation. The return value indicates whether the folding
        /// operation should continue, and if so, what the next step should be.
        /// </summary>
        /// <remarks>
        /// It is up to the consumer of this method to implement the actual state-aggregation (the folding)
        /// before passing it to the continuation function.  
        /// </remarks>
        /// <param name="ta">Foldable structure</param>
        /// <param name="initialState">Initial state value</param>
        /// <typeparam name="A">Value type</typeparam>
        /// <typeparam name="S">State type</typeparam>
        /// <returns>A discriminated union that can be either `Done` or `Loop`.</returns>
        public Fold<A, S> FoldStep<S>(S initialState) =>
            T.FoldStep(ta, initialState);
        
        /// <summary>
        /// Runs a single step of the folding operation. The return value indicates whether the folding
        /// operation should continue, and if so, what the next step should be.
        /// </summary>
        /// <remarks>
        /// It is up to the consumer of this method to implement the actual state-aggregation (the folding)
        /// before passing it to the continuation function.  
        /// </remarks>
        /// <param name="ta">Foldable structure</param>
        /// <param name="initialState">Initial state value</param>
        /// <typeparam name="A">Value type</typeparam>
        /// <typeparam name="S">State type</typeparam>
        /// <returns>A discriminated union that can be either `Done` or `Loop`.</returns>
        public Fold<A, S> FoldStepBack<S>(S initialState) => 
            T.FoldStep(ta, initialState);
        
        /// <summary>
        /// Fold until the `Option` returns `None`
        /// </summary>
        /// <param name="f">Fold function</param>
        /// <param name="initialState">Initial state for the fold</param>
        /// <typeparam name="S">State type</typeparam>
        /// <returns>Aggregated value</returns>
        public S FoldMaybe<S>(S initialState, Func<S, Func<A, Option<S>>> f) =>
            T.FoldMaybe(f, initialState, ta);

        /// <summary>
        /// Fold until the `Option` returns `None`
        /// </summary>
        /// <param name="f">Fold function</param>
        /// <param name="initialState">Initial state for the fold</param>
        /// <typeparam name="S">State type</typeparam>
        /// <returns>Aggregated value</returns>
        public S FoldMaybe<S>(
            S initialState,
            Func<S, A, Option<S>> f) =>
            T.FoldMaybe(s => a => f(s, a), initialState, ta);

        /// <summary>
        /// Fold until the `Option` returns `None`
        /// </summary>
        /// <param name="f">Fold function</param>
        /// <param name="initialState">Initial state for the fold</param>
        /// <typeparam name="S">State type</typeparam>
        /// <returns>Aggregated value</returns>
        public S FoldBackMaybe<S>(
            S initialState,
            Func<A, Func<S, Option<S>>> f) =>
            T.FoldBackMaybe(f, initialState, ta);

        /// <summary>
        /// Fold until the `Option` returns `None`
        /// </summary>
        /// <param name="f">Fold function</param>
        /// <param name="initialState">Initial state for the fold</param>
        /// <typeparam name="S">State type</typeparam>
        /// <returns>Aggregated value</returns>
        public S FoldBackMaybe<S>(
            S initialState,
            Func<S, A, Option<S>> f) =>
            T.FoldBackMaybe(a => s => f(s, a), initialState, ta);

        /// <summary>
        /// Same behaviour as `Fold` but allows early exit of the operation once
        /// the predicate function becomes `false` for the state/value pair 
        /// </summary>
        public S FoldWhile<S>(
            S initialState,
            Func<A, Func<S, S>> f, 
            Func<(S State, A Value), bool> predicate) =>
            T.FoldWhile(f, predicate, initialState, ta);

        /// <summary>
        /// Same behaviour as `Fold` but allows early exit of the operation once
        /// the predicate function becomes `false` for the state/value pair 
        /// </summary>
        public S FoldWhile<S>(
            S initialState,
            Func<S, A, S> f, 
            Func<(S State, A Value), bool> predicate) =>
            T.FoldWhile(a => s => f(s, a), predicate, initialState, ta);

        /// <summary>
        /// Same behaviour as `FoldBack` but allows early exit of the operation once
        /// the predicate function becomes `false` for the state/value pair 
        /// </summary>
        public S FoldBackWhile<S>(
            S initialState,
            Func<S, Func<A, S>> f, 
            Func<(S State, A Value), bool> predicate) =>
            T.FoldBackWhile(f, predicate, initialState, ta);

        /// <summary>
        /// Same behaviour as `FoldBack` but allows early exit of the operation once
        /// the predicate function becomes `false` for the state/value pair 
        /// </summary>
        public S FoldBackWhile<S>(
            S initialState,
            Func<S, A, S> f, 
            Func<(S State, A Value), bool> predicate) =>
            T.FoldBackWhile(curry(f), predicate, initialState, ta);

        /// <summary>
        /// Same behaviour as `Fold` but the fold operation returns a monadic type and allows
        /// early exit of the operation once the predicate function becomes `false` for the
        /// state/value pair 
        /// </summary>
        public K<M, S> FoldWhileM<M, S>(
            S initialState,
            Func<A, Func<S, K<M, S>>> f, 
            Func<(S State, A Value), bool> predicate) where M : Monad<M> =>
            T.FoldWhileM(f, predicate, initialState, ta);

        /// <summary>
        /// Same behaviour as `Fold` but the fold operation returns a monadic type and allows
        /// early exit of the operation once the predicate function becomes `false` for the
        /// state/value pair 
        /// </summary>
        public K<M, S> FoldWhileM<M, S>(
            S initialState,
            Func<S, A, K<M, S>> f, 
            Func<(S State, A Value), bool> predicate) where M : Monad<M> =>
            T.FoldWhileM<A, M, S>(a => s => f(s, a), predicate, initialState, ta);

        /// <summary>
        /// Same behaviour as `FoldBack` but the fold operation returns a monadic type and allows
        /// early exit of the operation once the predicate function becomes `false` for the
        /// state/value pair 
        /// </summary>
        public K<M, S> FoldBackWhileM<M, S>(
            S initialState,
            Func<S, Func<A, K<M, S>>> f, 
            Func<(S State, A Value), bool> predicate) where M : Monad<M> =>
            T.FoldBackWhileM(f, predicate, initialState, ta);

        /// <summary>
        /// Same behaviour as `FoldBack` but the fold operation returns a monadic type and allows
        /// early exit of the operation once the predicate function becomes `false` for the
        /// state/value pair 
        /// </summary>
        public K<M, S> FoldBackWhileM<M, S>(
            S initialState,
            Func<S, A, K<M, S>> f, 
            Func<(S State, A Value), bool> predicate) where M : Monad<M> =>
            T.FoldBackWhileM(curry(f), predicate, initialState, ta);

        /// <summary>
        /// Same behaviour as `Fold` but allows early exit of the operation once
        /// the predicate function becomes `false` for the state/value pair
        /// </summary>
        public S FoldUntil<S>(
            S initialState,
            Func<A, Func<S, S>> f,
            Func<(S State, A Value), bool> predicate) =>
            T.FoldUntil(f, predicate, initialState, ta);

        /// <summary>
        /// Same behaviour as `Fold` but allows early exit of the operation once
        /// the predicate function becomes `false` for the state/value pair
        /// </summary>
        public S FoldUntil<S>(
            S initialState,
            Func<S, A, S> f,
            Func<(S State, A Value), bool> predicate) =>
            T.FoldUntil(a => s => f(s, a), predicate, initialState, ta);

        /// <summary>
        /// Same behaviour as `Fold` but the fold operation returns a monadic type and allows
        /// early exit of the operation once the predicate function becomes `false` for the
        /// state/value pair 
        /// </summary>
        public K<M, S> FoldUntilM<M, S>(
            S initialState,
            Func<A, Func<S, K<M, S>>> f, 
            Func<(S State, A Value), bool> predicate) 
            where M : Monad<M> => 
            T.FoldUntilM(f, predicate, initialState, ta);

        /// <summary>
        /// Same behaviour as `Fold` but the fold operation returns a monadic type and allows
        /// early exit of the operation once the predicate function becomes `false` for the
        /// state/value pair 
        /// </summary>
        public K<M, S> FoldUntilM<M, S>(
            S initialState,
            Func<S, A, K<M, S>> f, 
            Func<(S State, A Value), bool> predicate) 
            where M : Monad<M> => 
            T.FoldUntilM<A, M, S>(a => s => f(s, a), predicate, initialState, ta);

        /// <summary>
        /// Same behaviour as `FoldBack` but allows early exit of the operation once
        /// the predicate function becomes `false` for the state/value pair
        /// </summary>
        public S FoldBackUntil<S>(
            S initialState,
            Func<S, Func<A, S>> f, 
            Func<(S State, A Value), bool> predicate) =>
            T.FoldBackUntil(f, predicate, initialState, ta);

        /// <summary>
        /// Same behaviour as `FoldBack` but allows early exit of the operation once
        /// the predicate function becomes `false` for the state/value pair
        /// </summary>
        public S FoldBackUntil<S>(
            S initialState,
            Func<S, A, S> f, 
            Func<(S State, A Value), bool> predicate) =>
            T.FoldBackUntil(curry(f), predicate, initialState, ta);

        /// <summary>
        /// Same behaviour as `FoldBack` but the fold operation returns a monadic type and allows
        /// early exit of the operation once the predicate function becomes `false` for the
        /// state/value pair 
        /// </summary>
        public K<M, S> FoldBackUntilM<M, S>(
            S initialState,
            Func<S, Func<A, K<M, S>>> f, 
            Func<(S State, A Value), bool> predicate) 
            where M : Monad<M> =>
            T.FoldBackUntilM(f, predicate, initialState, ta);

        /// <summary>
        /// Same behaviour as `FoldBack` but the fold operation returns a monadic type and allows
        /// early exit of the operation once the predicate function becomes `false` for the
        /// state/value pair 
        /// </summary>
        public K<M, S> FoldBackUntilM<M, S>(
            S initialState,
            Func<S, A, K<M, S>> f, 
            Func<(S State, A Value), bool> predicate) 
            where M : Monad<M> =>
            T.FoldBackUntilM(curry(f), predicate, initialState, ta);

        /// <summary>
        /// Right-associative fold of a structure, lazy in the accumulator.
        ///
        /// In the case of lists, 'Fold', when applied to a binary operator, a
        /// starting value (typically the right-identity of the operator), and a
        /// list, reduces the list using the binary operator, from right to left.
        /// </summary>
        public S Fold<S>(S initialState, Func<A, Func<S, S>> f) =>
            T.Fold(f, initialState, ta);

        /// <summary>
        /// Right-associative fold of a structure, lazy in the accumulator.
        ///
        /// In the case of lists, 'Fold', when applied to a binary operator, a
        /// starting value (typically the right-identity of the operator), and a
        /// list, reduces the list using the binary operator, from right to left.
        /// </summary>
        public S Fold<S>(S initialState, Func<S, A, S> f) =>
            T.Fold(a => s => f(s, a), initialState, ta);

        /// <summary>
        /// Right-associative fold of a structure, lazy in the accumulator.
        ///
        /// In the case of lists, 'Fold', when applied to a binary operator, a
        /// starting value (typically the right-identity of the operator), and a
        /// list, reduces the list using the binary operator, from right to left.
        /// </summary>
        public K<M, S> FoldM<M, S>(S initialState, Func<A, Func<S, K<M, S>>> f) 
            where M : Monad<M> =>
            T.FoldM(f, initialState, ta);

        /// <summary>
        /// Right-associative fold of a structure, lazy in the accumulator.
        ///
        /// In the case of lists, 'Fold', when applied to a binary operator, a
        /// starting value (typically the right-identity of the operator), and a
        /// list, reduces the list using the binary operator, from right to left.
        /// </summary>
        public K<M, S> FoldM<M, S>(S initialState, Func<S, A, K<M, S>> f) 
            where M : Monad<M> =>
            T.FoldM<A, M, S>(a => s => f(s, a), initialState, ta);

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
        public S FoldBack<S>(S initialState, Func<S, Func<A, S>> f) =>
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
        public S FoldBack<S>(S initialState, Func<S, A, S> f) =>
            T.FoldBack(curry(f), initialState, ta);

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
            S initialState,
            Func<S, Func<A, K<M, S>>> f) 
            where M : Monad<M> =>
            T.FoldBackM(f, initialState, ta);

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
            S initialState,
            Func<S, A, K<M, S>> f) 
            where M : Monad<M> =>
            T.FoldBackM(curry(f), initialState, ta);
    }

    extension<T, A>(K<T, A> tm) where T : Foldable<T> where A : Monoid<A>
    {
        /// <summary>
        /// Given a structure with elements whose type is a `Monoid`, combine them
        /// via the monoid's `Append` operator.  This fold is right-associative and
        /// lazy in the accumulator.  When you need a strict left-associative fold,
        /// use `FoldMap` instead, with `identity` as the map.
        /// </summary>
        public A Fold() =>
            T.Fold(tm);

        /// <summary>
        /// Given a structure with elements whose type is a `Monoid`, combine them
        /// via the monoid's `Append` operator.  This fold is right-associative and
        /// lazy in the accumulator.  When you need a strict left-associative fold,
        /// use `FoldMap` instead, with `identity` as the map.
        /// </summary>
        public A FoldWhile(Func<(A State, A Value), bool> predicate) =>
            T.FoldWhile(predicate, tm);

        /// <summary>
        /// Given a structure with elements whose type is a `Monoid`, combine them
        /// via the monoid's `Append` operator.  This fold is right-associative and
        /// lazy in the accumulator.  When you need a strict left-associative fold,
        /// use `FoldMap` instead, with `identity` as the map.
        /// </summary>
        public A FoldUntil(Func<(A State, A Value), bool> predicate) =>
            T.FoldUntil(predicate, tm);
    }

    extension<T, A>(K<T, A> ta) where T : Foldable<T>
    {
        /// <summary>
        /// Map each element of the structure into a monoid, and combine the
        /// results with `Append`.  This fold is right-associative and lazy in the
        /// accumulator.  For strict left-associative folds consider `FoldMapBack`
        /// instead.
        /// </summary>
        public B FoldMap<B>(Func<A, B> f) where B : Monoid<B> =>
            T.FoldMap(f, ta);

        /// <summary>
        /// Map each element of the structure into a monoid, and combine the
        /// results with `Append`.  This fold is right-associative and lazy in the
        /// accumulator.  For strict left-associative folds consider `FoldMapBack`
        /// instead.
        /// </summary>
        public B FoldMapWhile<B>(Func<A, B> f, Func<(B State, A Value), bool> predicate) where B : Monoid<B> =>
            T.FoldMapWhile(f, predicate, ta);

        /// <summary>
        /// Map each element of the structure into a monoid, and combine the
        /// results with `Append`.  This fold is right-associative and lazy in the
        /// accumulator.  For strict left-associative folds consider `FoldMapBack`
        /// instead.
        /// </summary>
        public B FoldMapUntil<B>(Func<A, B> f, Func<(B State, A Value), bool> predicate) where B : Monoid<B> =>
            T.FoldMapUntil(f, predicate, ta);

        /// <summary>
        /// A left-associative variant of 'FoldMap' that is strict in the
        /// accumulator.  Use this method for strict reduction when partial
        /// results are merged via `Append`.
        /// </summary>
        public B FoldMapBack<B>(Func<A, B> f) where B : Monoid<B> =>
            T.FoldMapBack(f, ta);

        /// <summary>
        /// A left-associative variant of 'FoldMap' that is strict in the
        /// accumulator.  Use this method for strict reduction when partial
        /// results are merged via `Append`.
        /// </summary>
        public B FoldMapBackWhile<B>(Func<A, B> f, Func<(B State, A Value), bool> predicate) where B : Monoid<B> =>
            T.FoldMapWhileBack(f, predicate, ta);

        /// <summary>
        /// A left-associative variant of 'FoldMap' that is strict in the
        /// accumulator.  Use this method for strict reduction when partial
        /// results are merged via `Append`.
        /// </summary>
        public B FoldMapBackUntil<B>(Func<A, B> f, Func<(B State, A Value), bool> predicate) where B : Monoid<B> =>
            T.FoldMapUntilBack(f, predicate, ta);

        /// <summary>
        /// List of elements of a structure, from left to right
        /// </summary>
        public Seq<A> ToSeq() =>
            T.ToSeq(ta);

        /// <summary>
        /// List of elements of a structure, from left to right
        /// </summary>
        public Lst<A> ToLst() =>
            T.ToLst(ta);

        /// <summary>
        /// List of elements of a structure, from left to right
        /// </summary>
        public Arr<A> ToArr() =>
            T.ToArr(ta);

        /// <summary>
        /// List of elements of a structure, from left to right
        /// </summary>
        public Iterable<A> ToIterable() =>
            T.ToIterable(ta);

        /// <summary>
        /// List of elements of a structure, from left to right
        /// </summary>
        public bool IsEmpty =>
            T.IsEmpty(ta);

        /// <summary>
        /// Returns the size/length of a finite structure as an `int`.  The
        /// default implementation just counts elements starting with the leftmost.
        /// 
        /// Instances for structures that can compute the element count faster
        /// than via element-by-element counting, should provide a specialised
        /// implementation.
        /// </summary>
        public int Count =>
            T.Count(ta);

        /// <summary>
        /// Does an element that fits the predicate occur in the structure?
        /// </summary>
        public bool Exists(Func<A, bool> predicate) =>
            T.Exists(predicate, ta);

        /// <summary>
        /// Does the predicate hold for all elements in the structure?
        /// </summary>
        public bool ForAll(Func<A, bool> predicate) =>
            T.ForAll(predicate, ta);
    }

    extension<EqA, T, A>(K<T, A> ta) where EqA : Eq<A> where T : Foldable<T>
    {
        /// <summary>
        /// Does the element exist in the structure?
        /// </summary>
        public bool Contains(A value) =>
            T.Contains<EqA, A>(value, ta);
    }

    extension<T, A>(K<T, A> ta) where T : Foldable<T>
    {
        /// <summary>
        /// Does the element exist in the structure?
        /// </summary>
        public bool Contains(A value) =>
            T.Contains(value, ta);

        /// <summary>
        /// Find the first element that match the predicate
        /// </summary>
        public Option<A> Find(Func<A, bool> predicate) =>
            T.Find(predicate, ta);

        /// <summary>
        /// Find the last element that match the predicate
        /// </summary>
        public Option<A> FindBack(Func<A, bool> predicate) =>
            T.FindBack(predicate, ta);

        /// <summary>
        /// Find the elements that match the predicate
        /// </summary>
        public Iterable<A> FindAll(Func<A, bool> predicate) =>
            T.FindAll(predicate, ta);

        /// <summary>
        /// Find the elements that match the predicate
        /// </summary>
        public Iterable<A> FindAllBack(Func<A, bool> predicate) =>
            T.FindAllBack(predicate, ta);
    }

    extension<T, A>(K<T, A> ta) 
        where T : Foldable<T> 
        where A : IAdditionOperators<A, A, A>, IAdditiveIdentity<A, A>
    {
        /// <summary>
        /// Computes the sum of the numbers of a structure.
        /// </summary>
        public A Sum() =>
            T.Sum(ta);
    }

    extension<T, A>(K<T, A> ta) 
        where T : Foldable<T> 
        where A : IMultiplyOperators<A, A, A>, IMultiplicativeIdentity<A, A>
    {
        /// <summary>
        /// Computes the product of the numbers of a structure.
        /// </summary>
        public A Product() =>
            T.Product(ta);
    }

    extension<T, A>(K<T, A> ta) 
        where T : Foldable<T>
    {
        /// <summary>
        /// Get the head item in the foldable or `None`
        /// </summary>
        public Option<A> Head =>
            T.Head(ta);

        /// <summary>
        /// Get the head item in the foldable or `None`
        /// </summary>
        public Option<A> Last =>
            T.Last(ta);

        /// <summary>
        /// Map each element of a structure to an 'Applicative' action, evaluate these
        /// actions from left to right, and ignore the results.  For a version that
        /// doesn't ignore the results see `Traversable.traverse`.
        /// </summary>
        public K<F, Unit> Iter<F, B>(Func<A, K<F, B>> f) 
            where F : Monad<F> =>
            T.Iter(f, ta);

        /// <summary>
        /// Map each element of a structure to an action, evaluate these
        /// actions from left to right, and ignore the results.  For a version that
        /// doesn't ignore the results see `Traversable.traverse`.
        /// </summary>
        public Unit Iter(Action<A> f) =>
            T.Iter(f, ta);

        /// <summary>
        /// Map each element of a structure to an action, evaluate these
        /// actions from left to right, and ignore the results.  For a version that
        /// doesn't ignore the results see `Traversable.traverse`.
        /// </summary>
        public Unit Iter(Action<int, A> f) =>
            T.Iter(f, ta);
    }

    /// <summary>
    /// Find the minimum value in the structure
    /// </summary>
    public static Option<A> Min<OrdA, T, A>(this K<T, A> ta)
        where T : Foldable<T>
        where OrdA : Ord<A> =>
        T.Min<OrdA, A>(ta);

    /// <summary>
    /// Find the minimum value in the structure
    /// </summary>
    public static Option<A> Min<T, A>(this K<T, A> ta)
        where T : Foldable<T>
        where A : IComparable<A> =>
        T.Min(ta);

    /// <summary>
    /// Find the maximum value in the structure
    /// </summary>
    public static Option<A> Max<OrdA, T, A>(this K<T, A> ta)
        where T : Foldable<T>
        where OrdA : Ord<A> =>
        T.Max<OrdA, A>(ta);

    /// <summary>
    /// Find the maximum value in the structure
    /// </summary>
    public static Option<A> Max<T, A>(this K<T, A> ta)
        where T : Foldable<T>
        where A : IComparable<A> =>
        T.Max(ta);
    
    /// <summary>
    /// Find the minimum value in the structure
    /// </summary>
    public static A Min<OrdA, T, A>(this K<T, A> ta, A initialMin)
        where T : Foldable<T>
        where OrdA : Ord<A> =>
        T.Min<OrdA, A>(ta, initialMin);

    /// <summary>
    /// Find the minimum value in the structure
    /// </summary>
    public static A Min<T, A>(this K<T, A> ta, A initialMin)
        where T : Foldable<T>
        where A : IComparable<A> =>
        T.Min(ta, initialMin);

    /// <summary>
    /// Find the maximum value in the structure
    /// </summary>
    public static A Max<OrdA, T, A>(this K<T, A> ta, A initialMax)
        where T : Foldable<T>
        where OrdA : Ord<A> =>
        T.Max<OrdA, A>(ta, initialMax);

    /// <summary>
    /// Find the maximum value in the structure
    /// </summary>
    public static A Max<T, A>(this K<T, A> ta, A initialMax)
        where T : Foldable<T>
        where A : IComparable<A> =>
        T.Max(ta, initialMax);    

    /// <summary>
    /// Find the average of all the values in the structure
    /// </summary>
    public static A Average<T, A>(this K<T, A> ta)
        where T : Foldable<T>
        where A : INumber<A> =>
        T.Average(ta);

    /// <summary>
    /// Find the average of all the values in the structure
    /// </summary>
    public static B Average<T, A, B>(this K<T, A> ta, Func<A, B> f)
        where T : Foldable<T>
        where B : INumber<B> =>
        T.Average(f, ta);

    /// <summary>
    /// Find the element at the specified index or `None` if out of range
    /// </summary>
    public static Option<A> At<T, A>(this K<T, A> ta, Index index)
        where T : Foldable<T> =>
        T.At(ta, index);

    /// <summary>
    /// Partition a foldable into two sequences based on a predicate
    /// </summary>
    /// <param name="f">Predicate function</param>
    /// <param name="ta">Foldable structure</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Partitioned structure</returns>
    public static (Seq<A> True, Seq<A> False) Partition<T, A>(this K<T, A> ta, Func<A, bool> f)
        where T : Foldable<T> =>
        T.Partition(f, ta);
}
