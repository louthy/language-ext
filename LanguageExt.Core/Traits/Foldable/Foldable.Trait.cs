using System;
using System.Numerics;

namespace LanguageExt.Traits;

/// <summary>
/// <para>
/// Foldable structures are those that can support repeated binary applications.  You will see
/// two 'flavours' of methods in the `Foldable` trait: forward and backward folds, which represent
/// different approaches to associativity when applying the binary function: 
/// </para>
/// <para>
/// `Fold(Func〈S, A, S〉, S)` is equal to: `((((S * A1) * A2) * A3) * A4) * ... An)`
/// </para>
/// <para>
/// `FoldBack(Func〈S, A, S〉, S)` is equal to: `(A1 * (A2 * (A3 * (A4 * ... (An * S))))`
/// </para>
/// <para>
/// > Where the `*` operator represents the binary function passed to `Fold`.
/// </para>
/// <para>
/// This repeated application over a structure (often a collection, but not exclusively) is known as a
/// *fold*; and is a fundamental operation in functional programming.
/// </para>
/// <para>
/// It should be noted that backward folds could come with additional overhead or problems depending on
/// the underlying implementations.  A lazy sequence like `Iterable` would need to be completely evaluated
/// before it could perform the first binary operation of a backward fold. Also, if the `Iterable` is
/// infinite, then the backward fold can never be completed.   
/// </para>
/// <para>
/// Whereas, a type like `Set`, which is presorted, or a type like `Arr`, or `Lst`, which support
/// random-access, can easily and efficiently perform backward folds; because it's cheap to access the
/// last value in the foldable structure and work backwards.
/// </para>
/// </summary>
/// <typeparam name="T"></typeparam>
public interface Foldable<out T> where T : Foldable<T>
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Abstract members
    //

    /// <summary>
    /// Runs a single step of the folding operation. The return value indicates whether the folding
    /// operation should continue, and if so, what the next step should be.
    /// </summary>
    /// <remarks>
    /// Mostly, consumers of `Foldable` shouldn't use `FoldStep` or `FoldStepBack` - these methods are the
    /// building blocks of every other method in the `Foldable` trait. It's more idiomatically functional
    /// to use the other methods that are built with `FoldStep` or `FoldStepBack` than to use them directly.
    ///
    /// Also, the return type `Fold〈A, S〉` is not guaranteed to be pure - it very likely won't be - and
    /// so should be used with care (usually in a tight folding loop) and definitely not shared.
    /// </remarks>
    /// <remarks>
    /// It is up to the consumer of this method to implement the actual state-aggregation (the folding)
    /// before passing it to the continuation function. 
    /// </remarks>
    /// <param name="ta">Foldable structure</param>
    /// <param name="initialState">Initial state value</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <typeparam name="S">State type</typeparam>
    /// <returns>A discriminated union that can be either `Done` or `Loop`.</returns>
    public static abstract Fold<A, S> FoldStep<TA, A, S>(in TA ta, in S initialState)
        where TA : K<T, A>;
    
    /// <summary>
    /// Runs a single step of the folding operation. The return value indicates whether the folding
    /// operation should continue, and if so, what the next step should be.
    /// </summary>
    /// <remarks>
    /// Mostly, consumers of `Foldable` shouldn't use `FoldStep` or `FoldStepBack` - these methods are the
    /// building blocks of every other method in the `Foldable` trait. It's more idiomatically functional
    /// to use the other methods that are built with `FoldStep` or `FoldStepBack` than to use them directly.
    ///
    /// Also, the return type `Fold〈A, S〉` is not guaranteed to be pure - it very likely won't be - and
    /// so should be used with care (usually in a tight folding loop) and definitely not shared.
    /// </remarks>
    /// <remarks>
    /// It is up to the consumer of this method to implement the actual state-aggregation (the folding)
    /// before passing it to the continuation function. 
    /// </remarks>
    /// <param name="ta">Foldable structure</param>
    /// <param name="initialState">Initial state value</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <typeparam name="S">State type</typeparam>
    /// <returns>A discriminated union that can be either `Done` or `Loop`.</returns>
    public static abstract Fold<A, S> FoldStepBack<TA, A, S>(in TA ta, in S initialState)
        where TA : K<T, A>;

    /// <summary>
    /// Same behaviour as `Fold` but allows early exit of the operation once
    /// the predicate function becomes `false` for the state/value pair 
    /// </summary>
    public static abstract S FoldWhile<TA, A, S>(
        Func<S, A, S> f,
        Func<(S State, A Value), bool> predicate,
        in S initialState,
        in TA ta) 
        where TA : K<T, A>;

    /// <summary>
    /// Same behaviour as `FoldBack` but allows early exit of the operation once
    /// the predicate function becomes `false` for the state/value pair 
    /// </summary>
    public static abstract S FoldBackWhile<TA, A, S>(
        Func<S, A, S> f,
        Func<(S State, A Value), bool> predicate,
        in S initialState,
        in TA ta)
        where TA : K<T, A>;

    /// <summary>
    /// Fold until the `Option` returns `None`
    /// </summary>
    /// <param name="f">Fold function</param>
    /// <param name="initialState">Initial state for the fold</param>
    /// <param name="ta">Foldable structure</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <typeparam name="S">State type</typeparam>
    /// <returns>Aggregated value</returns>
    public static abstract S FoldMaybe<TA, A, S>(
        Func<S, A, Option<S>> f,
        in S initialState,
        in TA ta)
        where TA : K<T, A>;

    /// <summary>
    /// Fold until the `Option` returns `None`
    /// </summary>
    /// <param name="f">Fold function</param>
    /// <param name="initialState">Initial state for the fold</param>
    /// <param name="ta">Foldable structure</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <typeparam name="S">State type</typeparam>
    /// <returns>Aggregated value</returns>
    public static abstract S FoldBackMaybe<TA, A, S>(
        Func<S, A, Option<S>> f,
        in S initialState,
        in TA ta)
        where TA : K<T, A>;

    /// <summary>
    /// Same behaviour as `Fold` but the fold operation returns a monadic type and allows
    /// early exit of the operation once the predicate function becomes `false` for the
    /// state/value pair 
    /// </summary>
    public static abstract MS FoldWhileM<TA, MS, A, M, S>(
        Func<S, A, MS> f,
        Func<(S State, A Value), bool> predicate,
        in S initialState,
        in TA ta)
        where TA : K<T, A>
        where MS : K<M, S>
        where M : Monad<M>;

    /// <summary>
    /// Same behaviour as `FoldBack` but the fold operation returns a monadic type and allows
    /// early exit of the operation once the predicate function becomes `false` for the
    /// state/value pair 
    /// </summary>
    public static abstract MS FoldBackWhileM<TA, MS, A, M, S>(
        Func<S, A, MS> f,
        Func<(S State, A Value), bool> predicate,
        in S initialState,
        in TA ta)
        where MS : K<M, S>
        where TA : K<T, A>
        where M : Monad<M>;

    /// <summary>
    /// Same behaviour as `Fold` but allows early exit of the operation once
    /// the predicate function becomes `false` for the state/value pair
    /// </summary>
    public static abstract S FoldUntil<TA, A, S>(
        Func<S, A, S> f,
        Func<(S State, A Value), bool> predicate,
        in S initialState,
        in TA ta)
        where TA : K<T, A>;

    /// <summary>
    /// Same behaviour as `Fold` but the fold operation returns a monadic type and allows
    /// early exit of the operation once the predicate function becomes `false` for the
    /// state/value pair 
    /// </summary>
    public static abstract MS FoldUntilM<TA, MS, A, M, S>(
        Func<S, A, MS> f,
        Func<(S State, A Value), bool> predicate,
        in S initialState,
        in TA ta)
        where MS : K<M, S> 
        where TA : K<T, A>
        where M : Monad<M>;

    /// <summary>
    /// Same behaviour as `FoldBack` but allows early exit of the operation once
    /// the predicate function becomes `false` for the state/value pair
    /// </summary>
    public static abstract S FoldBackUntil<TA, A, S>(
        Func<S, A, S> f,
        Func<(S State, A Value), bool> predicate,
        in S initialState,
        in TA ta) 
        where TA : K<T, A>;

    /// <summary>
    /// Same behaviour as `FoldBack` but the fold operation returns a monadic type and allows
    /// early exit of the operation once the predicate function becomes `false` for the
    /// state/value pair 
    /// </summary>
    public static abstract MS FoldBackUntilM<TA, MS, A, M, S>(
        Func<S, A, MS> f,
        Func<(S State, A Value), bool> predicate,
        in S initialState,
        in TA ta)
        where M : Monad<M>
        where MS : K<M, S>
        where TA : K<T, A>;

    /// <summary>
    /// Right-associative fold of a structure, lazy in the accumulator.
    ///
    /// In the case of lists, 'Fold', when applied to a binary operator, a
    /// starting value (typically the right-identity of the operator), and a
    /// list, reduces the list using the binary operator, from right to left.
    /// </summary>
    public static abstract S Fold<TA, A, S>(Func<S, A, S> f, in S initialState, in TA ta)
        where TA : K<T, A>;

    /// <summary>
    /// Right-associative fold of a structure, lazy in the accumulator.
    ///
    /// In the case of lists, 'Fold', when applied to a binary operator, a
    /// starting value (typically the right-identity of the operator), and a
    /// list, reduces the list using the binary operator, from right to left.
    /// </summary>
    public static abstract MS FoldM<TA, MS, A, M, S>(
        Func<S, A, MS> f,
        in S initialState,
        in TA ta)
        where TA : K<T, A>
        where MS : K<M, S>
        where M : Monad<M>;

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
    public static abstract S FoldBack<TA, A, S>(Func<S, A, S> f, in S initialState, in TA ta)
        where TA : K<T, A>;

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
    public static abstract MS FoldBackM<TA, MS, A, M, S>(
        Func<S, A, MS> f,
        in S initialState,
        in TA ta)
        where M : Monad<M>
        where MS : K<M, S>
        where TA : K<T, A>;

    /// <summary>
    /// Given a structure with elements whose type is a `Monoid`, combine them
    /// via the monoid's `Combine` operator
    /// </summary>
    public static abstract A Fold<TA, A>(in TA ta)
        where A : Monoid<A>
        where TA : K<T, A>;

    /// <summary>
    /// Given a structure with elements whose type is a `Monoid`, combine them
    /// via the monoid's `Combine` operator.
    /// </summary>
    public static abstract A FoldWhile<TA, A>(Func<(A State, A Value), bool> predicate, in TA ta)
        where A : Monoid<A>
        where TA : K<T, A>;

    /// <summary>
    /// Given a structure with elements whose type is a `Monoid`, combine them
    /// via the monoid's `Combine` operator.
    /// </summary>
    public static abstract A FoldUntil<TA, A>(Func<(A State, A Value), bool> predicate, in TA ta)
        where A : Monoid<A>
        where TA : K<T, A>;

    /// <summary>
    /// Map each element of the structure into a monoid, and combine the
    /// results with `Monoid.Combine`.  
    /// </summary>
    public static abstract B FoldMap<TA, A, B>(Func<A, B> f, in TA ta)
        where B : Monoid<B>
        where TA : K<T, A>;

    /// <summary>
    /// Map each element of the structure into a monoid, and combine the
    /// results with `Combine`.  
    /// </summary>
    public static abstract B FoldMapWhile<TA, A, B>(Func<A, B> f, Func<(B State, A Value), bool> predicate, in TA ta)
        where B : Monoid<B>
        where TA : K<T, A>;

    /// <summary>
    /// Map each element of the structure into a monoid, and combine the
    /// results with `Append`.  This fold is right-associative and lazy in the
    /// accumulator.  For strict left-associative folds consider `FoldMapBack`
    /// instead.
    /// </summary>
    public static abstract B FoldMapUntil<TA, A, B>(Func<A, B> f, Func<(B State, A Value), bool> predicate, in TA ta)
        where B : Monoid<B>
        where TA : K<T, A>;

    /// <summary>
    /// A left-associative variant of 'FoldMap' that is strict in the
    /// accumulator.  Use this method for strict reduction when partial
    /// results are merged via `Append`.
    /// </summary>
    public static abstract B FoldMapBack<TA, A, B>(Func<A, B> f, in TA ta)
        where B : Monoid<B>
        where TA : K<T, A>;

    /// <summary>
    /// A left-associative variant of 'FoldMap' that is strict in the
    /// accumulator.  Use this method for strict reduction when partial
    /// results are merged via `Append`.
    /// </summary>
    public static abstract B FoldMapWhileBack<TA, A, B>(Func<A, B> f, Func<(B State, A Value), bool> predicate, in TA ta)
        where B : Monoid<B>
        where TA : K<T, A>;

    /// <summary>
    /// A left-associative variant of 'FoldMap' that is strict in the
    /// accumulator.  Use this method for strict reduction when partial
    /// results are merged via `Append`.
    /// </summary>
    public static abstract B FoldMapUntilBack<TA, A, B>(Func<A, B> f, Func<(B State, A Value), bool> predicate, in TA ta)
        where B : Monoid<B>
        where TA : K<T, A>;

    /// <summary>
    /// List of elements of a structure, from left to right
    /// </summary>
    public static abstract Seq<A> ToSeq<TA, A>(in TA ta)
        where TA : K<T, A>;

    /// <summary>
    /// List of elements of a structure, from left to right
    /// </summary>
    public static abstract Lst<A> ToLst<TA, A>(in TA ta)
        where TA : K<T, A>;

    /// <summary>
    /// List of elements of a structure, from left to right
    /// </summary>
    public static abstract Arr<A> ToArr<TA, A>(in TA ta)
        where TA : K<T, A>;

    /// <summary>
    /// List of elements of a structure, from left to right
    /// </summary>
    public static abstract Iterable<A> ToIterable<TA, A>(in TA ta)
        where TA : K<T, A>;

    /// <summary>
    /// List of elements of a structure, from left to right
    /// </summary>
    public static abstract bool IsEmpty<TA, A>(in TA ta)
        where TA : K<T, A>;
    
    /// <summary>
    /// Returns the size/length of a finite structure as an `int`.  The
    /// default implementation just counts elements starting with the leftmost.
    /// 
    /// Instances for structures that can compute the element count faster
    /// than via element-by-element counting, should provide a specialised
    /// implementation.
    /// </summary>
    public static abstract int Count<TA, A>(in TA ta)
        where TA : K<T, A>;

    /// <summary>
    /// Does an element that fits the predicate occur in the structure?
    /// </summary>
    public static abstract bool Exists<TA, A>(Func<A, bool> predicate, in TA ta)
        where TA : K<T, A>;

    /// <summary>
    /// Does the predicate hold for all elements in the structure?
    /// </summary>
    public static abstract bool ForAll<TA, A>(Func<A, bool> predicate, in TA ta)
        where TA : K<T, A>;

    /// <summary>
    /// Does the element exist in the structure?
    /// </summary>
    public static abstract bool Contains<TA, EqA, A>(A value, in TA ta) 
        where EqA : Eq<A>
        where TA : K<T, A>;
    
    /// <summary>
    /// Does the element exist in the structure?
    /// </summary>
    public static abstract bool Contains<TA, A>(A value, in TA ta)
        where TA : K<T, A>;

    /// <summary>
    /// Find the first element that match the predicate
    /// </summary>
    public static abstract Option<A> Find<TA, A>(Func<A, bool> predicate, in TA ta)
        where TA : K<T, A>;

    /// <summary>
    /// Find the last element that match the predicate
    /// </summary>
    public static abstract Option<A> FindBack<TA, A>(Func<A, bool> predicate, in TA ta)
        where TA : K<T, A>;

    /// <summary>
    /// Find the elements that match the predicate
    /// </summary>
    public static abstract Iterable<A> FindAll<TA, A>(Func<A, bool> predicate, in TA ta)
        where TA : K<T, A>;

    /// <summary>
    /// Find the elements that match the predicate
    /// </summary>
    public static abstract Iterable<A> FindAllBack<TA, A>(Func<A, bool> predicate, in TA ta)
        where TA : K<T, A>;

    /// <summary>
    /// Computes the sum of the numbers of a structure.
    /// </summary>
    public static abstract A Sum<TA, A>(in TA ta)
        where A : IAdditionOperators<A, A, A>, IAdditiveIdentity<A, A>
        where TA : K<T, A>;

    /// <summary>
    /// Computes the product of the numbers of a structure.
    /// </summary>
    public static abstract A Product<TA, A>(in TA ta)
        where A : IMultiplyOperators<A, A, A>, IMultiplicativeIdentity<A, A>
        where TA : K<T, A>;

    /// <summary>
    /// Get the head item in the foldable or `None`
    /// </summary>
    public static abstract Option<A> Head<TA, A>(in TA ta)
        where TA : K<T, A>;

    /// <summary>
    /// Get the last item in the foldable or `None`
    /// </summary>
    public static abstract Option<A> Last<TA, A>(in TA ta)
        where TA : K<T, A>;

    /// <summary>
    /// Map each element of a structure to a monadic action, evaluate these
    /// actions from left to right, and ignore the results. 
    /// </summary>
    public static abstract MU IterM<TA, MB, MU, M, A, B>(Func<A, MB> f, in TA ta)
        where M : Monad<M>
        where MB : K<M, B>
        where MU : K<M, Unit>
        where TA : K<T, A>;

    /// <summary>
    /// Map each element of a structure to an action, evaluate these
    /// actions from left to right, and ignore the results.  For a version that
    /// doesn't ignore the results see `Traversable.traverse`.
    /// </summary>
    public static abstract Unit Iter<TA, A>(Action<A> f, in TA ta)
        where TA : K<T, A>;
    
    /// <summary>
    /// Map each element of a structure to an action, evaluate these
    /// actions from left to right, and ignore the results.  For a version that
    /// doesn't ignore the results see `Traversable.traverse`.
    /// </summary>
    public static abstract Unit Iter<TA, A>(Action<int, A> f, in TA ta)
        where TA : K<T, A>;

    /// <summary>
    /// Find the minimum value in the structure
    /// </summary>
    public static abstract Option<A> Min<TA, OrdA, A>(in TA ta)
        where OrdA : Ord<A>
        where TA : K<T, A>;

    /// <summary>
    /// Find the minimum value in the structure
    /// </summary>
    public static abstract Option<A> Min<TA, A>(in TA ta)
        where A : IComparable<A>
        where TA : K<T, A>;

    /// <summary>
    /// Find the maximum value in the structure
    /// </summary>
    public static abstract Option<A> Max<TA, OrdA, A>(in TA ta)
        where OrdA : Ord<A>
        where TA : K<T, A>;

    /// <summary>
    /// Find the maximum value in the structure
    /// </summary>
    public static abstract Option<A> Max<TA, A>(in TA ta)
        where A : IComparable<A>
        where TA : K<T, A>;

    /// <summary>
    /// Find the minimum value in the structure
    /// </summary>
    public static abstract A Min<TA, OrdA, A>(A initialMin, in TA ta)
        where OrdA : Ord<A>
        where TA : K<T, A>;

    /// <summary>
    /// Find the minimum value in the structure
    /// </summary>
    public static abstract A Min<TA, A>(A initialMin, in TA ta)
        where A : IComparable<A>
        where TA : K<T, A>;

    /// <summary>
    /// Find the maximum value in the structure
    /// </summary>
    public static abstract A Max<TA, OrdA, A>(A initialMax, in TA ta)
        where OrdA : Ord<A>
        where TA : K<T, A>;

    /// <summary>
    /// Find the maximum value in the structure
    /// </summary>
    public static abstract A Max<TA, A>(A initialMax, in TA ta)
        where A : IComparable<A>
        where TA : K<T, A>;

    /// <summary>
    /// Find the average of all the values in the structure
    /// </summary>
    public static abstract A Average<TA, A>(in TA ta)
        where A : INumber<A>
        where TA : K<T, A>;

    /// <summary>
    /// Find the element at the specified index or `None` if out of range
    /// </summary>
    public static abstract Option<A> At<TA, A>(Index index, in TA ta)
        where TA : K<T, A>;

    /// <summary>
    /// Partition a foldable into two sequences based on a predicate
    /// </summary>
    /// <param name="f">Predicate function</param>
    /// <param name="ta">Foldable structure</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Partitioned structure</returns>
    public static abstract (Seq<A> True, Seq<A> False) Partition<TA, A>(Func<A, bool> f, in TA ta)
        where TA : K<T, A>;
}
