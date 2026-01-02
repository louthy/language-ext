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
        where T : Foldable<T>
        where F : Applicative<F>
    {
        /// <summary>
        /// Fold the structure: `ta` and pass each element that it yields to `f`, resulting in an `F` applicative-value.
        /// The fold operator is applicative `Action`, which causes each applicative-value to be sequenced.      
        /// </summary>
        /// <param name="ta">Foldable structure - TODO: Change the argument to T when this feature lands in C#: https://github.com/dotnet/csharplang/issues/9453</param>
        /// <returns></returns>
        public K<F, Unit> ForM(K<T, A> ta) =>
            ta.Fold((fs, x) => fs.BackAction(f(x)), pure<F, Unit>(unit));
    }

    /// <param name="ta">Foldable structure - TODO: Change the argument to T when this feature lands in C#: https://github.com/dotnet/csharplang/issues/9453</param>
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
            T.Fold((fs, x) => fs.BackAction(f(x)), pure<F, Unit>(unit), ta);
    }

    /// <param name="ta">Foldable structure - TODO: Change the argument to T when this feature lands in C#: https://github.com/dotnet/csharplang/issues/9453</param>
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
            T.FoldStepBack(ta, initialState);

        /// <summary>
        /// Fold until the `Option` returns `None`
        /// </summary>
        /// <param name="f">Fold function</param>
        /// <param name="initialState">Initial state for the fold</param>
        /// <typeparam name="S">State type</typeparam>
        /// <returns>Aggregated value</returns>
        public S FoldMaybe<S>(
            Func<S, A, Option<S>> f,
            S initialState) =>
            T.FoldMaybe(f, initialState, ta);

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
        /// Same behaviour as `Fold` but allows early exit of the operation once
        /// the predicate function becomes `false` for the state/value pair 
        /// </summary>
        public S FoldWhile<S>(
            Func<S, A, S> f,
            Func<(S State, A Value), bool> predicate,
            S initialState) =>
            T.FoldWhile(f, predicate, initialState, ta);

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
        /// Same behaviour as `Fold` but the fold operation returns a monadic type and allows
        /// early exit of the operation once the predicate function becomes `false` for the
        /// state/value pair 
        /// </summary>
        public K<M, S> FoldWhileM<M, S>(
            Func<S, A, K<M, S>> f,
            Func<(S State, A Value), bool> predicate,
            S initialState)
            where M : Monad<M> =>
            T.FoldWhileM<K<M, S>, M, A, S>(f, predicate, initialState, ta);

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
        /// Same behaviour as `Fold` but allows early exit of the operation once
        /// the predicate function becomes `false` for the state/value pair
        /// </summary>
        public S FoldUntil<S>(
            Func<S, A, S> f,
            Func<(S State, A Value), bool> predicate,
            S initialState) =>
            T.FoldUntil(f, predicate, initialState, ta);

        /// <summary>
        /// Same behaviour as `Fold` but the fold operation returns a monadic type and allows
        /// early exit of the operation once the predicate function becomes `false` for the
        /// state/value pair 
        /// </summary>
        public K<M, S> FoldUntilM<M, S>(
            Func<S, A, K<M, S>> f,
            Func<(S State, A Value), bool> predicate,
            S initialState)
            where M : Monad<M> =>
            T.FoldUntilM<K<M, S>, M, A, S>(f, predicate, initialState, ta);

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
        /// Right-associative fold of a structure, lazy in the accumulator.
        ///
        /// In the case of lists, 'Fold', when applied to a binary operator, a
        /// starting value (typically the right-identity of the operator), and a
        /// list, reduces the list using the binary operator, from right to left.
        /// </summary>
        public S Fold<S>(Func<S, A, S> f, S initialState) =>
            T.Fold(f, initialState, ta);

        /// <summary>
        /// Right-associative fold of a structure, lazy in the accumulator.
        ///
        /// In the case of lists, 'Fold', when applied to a binary operator, a
        /// starting value (typically the right-identity of the operator), and a
        /// list, reduces the list using the binary operator, from right to left.
        /// </summary>
        public K<M, S> FoldM<M, S>(Func<S, A, K<M, S>> f, S initialState)
            where M : Monad<M> =>
            T.FoldM<K<M, S>, M, A, S>(f, initialState, ta);

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
        /// Return the number of items in a foldable structure
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
        /// Iterate over the structure from left to right, applying the monadic action to each element.
        /// </summary>
        public K<F, Unit> IterM<F, B>(Func<A, K<F, B>> f)
            where F : Monad<F> =>
            T.IterM<K<F, B>, F, A, B>(f, ta);

        /// <summary>
        /// Iterate over the structure from left to right, applying the action to each element.
        /// </summary>
        public Unit Iter(Action<A> f) =>
            T.Iter(f, ta);

        /// <summary>
        /// Iterate over the structure from left to right, applying the action to each element.
        /// </summary>
        public Unit Iter(Action<int, A> f) =>
            T.Iter(f, ta);

        /// <summary>
        /// Find the minimum value in the structure
        /// </summary>
        public Option<A> Min() =>
            T.Min(ta);

        /// <summary>
        /// Find the maximum value in the structure
        /// </summary>
        public Option<A> Max() =>
            T.Max(ta);

        /// <summary>
        /// Find the minimum value in the structure
        /// </summary>
        public A Min(A initialMin) =>
            T.Min(initialMin, ta);

        /// <summary>
        /// Find the maximum value in the structure
        /// </summary>
        public A Max(A initialMax) =>
            T.Max(initialMax, ta);
        
        /// <summary>
        /// Find the element at the specified index or `None` if out of range
        /// </summary>
        public Option<A> At(Index index) =>
            T.At(index, ta);

        /// <summary>
        /// Partition a foldable into two sequences based on a predicate
        /// </summary>
        /// <param name="f">Predicate function</param>
        /// <param name="ta">Foldable structure</param>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>Partitioned structure</returns>
        public (Seq<A> True, Seq<A> False) Partition(Func<A, bool> f) =>
            T.Partition(f, ta);
    }
    
    /// <param name="ta">Foldable structure - TODO: Change the argument to T when this feature lands in C#: https://github.com/dotnet/csharplang/issues/9453</param>
    /// <typeparam name="A">Value type</typeparam>
    extension<EqA, T, A>(K<T, A> ta)
        where T : Foldable<T>
        where EqA : Eq<A> 
    {
        /// <summary>
        /// Does the element exist in the structure?
        /// </summary>
        public bool Contains(A value) =>
            T.Contains<EqA, A>(value, ta);
    }
    
    /// <param name="ta">Foldable structure - TODO: Change the argument to T when this feature lands in C#: https://github.com/dotnet/csharplang/issues/9453</param>
    /// <typeparam name="A">Value type</typeparam>
    extension<OrdA, T, A>(K<T, A> ta)
        where T : Foldable<T>
        where OrdA : Ord<A>
    {

        /// <summary>
        /// Find the minimum value in the structure
        /// </summary>
        public Option<A> Min() =>
            T.Min<OrdA, A>(ta);

        /// <summary>
        /// Find the maximum value in the structure
        /// </summary>
        public Option<A> Max() =>
            T.Max<OrdA, A>(ta);

        /// <summary>
        /// Find the minimum value in the structure
        /// </summary>
        public A Min(A initialMin) =>
            T.Min<OrdA, A>(initialMin, ta);

        /// <summary>
        /// Find the maximum value in the structure
        /// </summary>
        public A Max(A initialMax) =>
            T.Max<OrdA, A>(initialMax, ta);
    }
    
    /// <param name="ta">Foldable structure - TODO: Change the argument to T when this feature lands in C#: https://github.com/dotnet/csharplang/issues/9453</param>
    /// <typeparam name="T">Foldable type</typeparam>
    /// <typeparam name="A">Bound values</typeparam>
    extension<T, A>(K<T, A> ta)
        where T : Foldable<T>
        where A : Monoid<A>
    {
        /// <summary>
        /// Given a structure with elements whose type is a `Monoid`, combine them
        /// via the monoid's `Append` operator.  This fold is right-associative and
        /// lazy in the accumulator.  When you need a strict left-associative fold,
        /// use `FoldMap` instead, with `identity` as the map.
        /// </summary>
        public A Fold() =>
            T.Fold((s, x) => s + x, A.Empty, ta);

        /// <summary>
        /// Given a structure with elements whose type is a `Monoid`, combine them
        /// via the monoid's `Append` operator.  This fold is right-associative and
        /// lazy in the accumulator.  When you need a strict left-associative fold,
        /// use `FoldMap` instead, with `identity` as the map.
        /// </summary>
        public A FoldWhile(Func<(A State, A Value), bool> predicate) =>
            T.FoldWhile((s, x) => s + x, predicate, A.Empty, ta);

        /// <summary>
        /// Given a structure with elements whose type is a `Monoid`, combine them
        /// via the monoid's `Append` operator.  This fold is right-associative and
        /// lazy in the accumulator.  When you need a strict left-associative fold,
        /// use `FoldMap` instead, with `identity` as the map.
        /// </summary>
        public A FoldUntil(Func<(A State, A Value), bool> predicate) =>
            T.FoldUntil((s, x) => s + x, predicate, A.Empty, ta);
    }

    /// <param name="ta">Foldable structure - TODO: Change the argument to T when this feature lands in C#: https://github.com/dotnet/csharplang/issues/9453</param>
    /// <typeparam name="T">Foldable type</typeparam>
    /// <typeparam name="A">Bound values</typeparam>
    extension<T, A>(K<T, A> ta)
        where T : Foldable<T>
        where A : IAdditionOperators<A, A, A>, IAdditiveIdentity<A, A>
    {
        /// <summary>
        /// Find sum of all the values in the structure
        /// </summary>
        public A Sum() =>
            T.Fold((s, x) => s + x, A.AdditiveIdentity, ta);
    }

    /// <param name="ta">Foldable structure - TODO: Change the argument to T when this feature lands in C#: https://github.com/dotnet/csharplang/issues/9453</param>
    /// <typeparam name="T">Foldable type</typeparam>
    /// <typeparam name="A">Bound values</typeparam>
    extension<T, A>(K<T, A> ta)
        where T : Foldable<T>
        where A : IMultiplyOperators<A, A, A>, IMultiplicativeIdentity<A, A>
    {
        /// <summary>
        /// Find product of all the values in the structure
        /// </summary>
        public A Product() =>
            T.Fold((s, x) => s * x, A.MultiplicativeIdentity, ta);
    }
    
    /// <param name="ta">Foldable structure - TODO: Change the argument to T when this feature lands in C#: https://github.com/dotnet/csharplang/issues/9453</param>
    /// <typeparam name="T">Foldable type</typeparam>
    /// <typeparam name="A">Bound values</typeparam>
    extension<T, A>(K<T, A> ta)
        where T : Foldable<T>
        where A : INumber<A>
    {
        /// <summary>
        /// Find the average of all the values in the structure
        /// </summary>
        public A Average() =>
            T.Fold((s, x) => (s.Count + A.One, s.Total + x), (Count: A.Zero, Total: A.Zero), ta) switch
            {
                var (count, _) when count == A.Zero => A.Zero,
                var (count, total)                  => total / count
            };
    }
}
