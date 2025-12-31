using System;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class Deriving
{
    public interface Foldable<Supertype, Subtype> :
        Foldable<Supertype>,
        Traits.Natural<Supertype, Subtype>,
        Traits.CoNatural<Supertype, Subtype>
        where Supertype : Foldable<Supertype>, Foldable<Supertype, Subtype>
        where Subtype : Foldable<Subtype>
    {
        /// <summary>
        /// Same behaviour as `Fold` but allows early exit of the operation once
        /// the predicate function becomes `false` for the state/value pair 
        /// </summary>
        static S Foldable<Supertype>.FoldWhile<A, S>(
            Func<A, Func<S, S>> f,
            Func<(S State, A Value), bool> predicate,
            S initialState,
            K<Supertype, A> ta) =>
            Subtype.FoldWhile(f, predicate, initialState, Supertype.Transform(ta));

        /// <summary>
        /// Same behaviour as `FoldBack` but allows early exit of the operation once
        /// the predicate function becomes `false` for the state/value pair 
        /// </summary>
        static S Foldable<Supertype>.FoldBackWhile<A, S>(
            Func<S, Func<A, S>> f,
            Func<(S State, A Value), bool> predicate,
            S initialState,
            K<Supertype, A> ta) =>
            Subtype.FoldBackWhile(f, predicate, initialState, Supertype.Transform(ta));

        /// <summary>
        /// Fold until the `Option` returns `None`
        /// </summary>
        /// <param name="f">Fold function</param>
        /// <param name="initialState">Initial state for the fold</param>
        /// <param name="ta">Foldable structure</param>
        /// <typeparam name="A">Value type</typeparam>
        /// <typeparam name="S">State type</typeparam>
        /// <returns>Aggregated value</returns>
        static S Foldable<Supertype>.FoldMaybe<A, S>(
            Func<S, Func<A, Option<S>>> f,
            S initialState,
            K<Supertype, A> ta) =>
            Subtype.FoldMaybe(f, initialState, Supertype.Transform(ta));

        /// <summary>
        /// Fold until the `Option` returns `None`
        /// </summary>
        /// <param name="f">Fold function</param>
        /// <param name="initialState">Initial state for the fold</param>
        /// <param name="ta">Foldable structure</param>
        /// <typeparam name="A">Value type</typeparam>
        /// <typeparam name="S">State type</typeparam>
        /// <returns>Aggregated value</returns>
        static S Foldable<Supertype>.FoldBackMaybe<A, S>(
            Func<A, Func<S, Option<S>>> f,
            S initialState,
            K<Supertype, A> ta) =>
            Subtype.FoldBackMaybe(f, initialState, Supertype.Transform(ta));

        /// <summary>
        /// Same behaviour as `Fold` but the fold operation returns a monadic type and allows
        /// early exit of the operation once the predicate function becomes `false` for the
        /// state/value pair 
        /// </summary>
        static K<M, S> Foldable<Supertype>.FoldWhileM<A, M, S>(
            Func<A, Func<S, K<M, S>>> f,
            Func<(S State, A Value), bool> predicate,
            S initialState,
            K<Supertype, A> ta) =>
            Subtype.FoldWhileM(f, predicate, initialState, Supertype.Transform(ta));

        /// <summary>
        /// Same behaviour as `FoldBack` but the fold operation returns a monadic type and allows
        /// early exit of the operation once the predicate function becomes `false` for the
        /// state/value pair 
        /// </summary>
        static K<M, S> Foldable<Supertype>.FoldBackWhileM<A, M, S>(
            Func<S, Func<A, K<M, S>>> f,
            Func<(S State, A Value), bool> predicate,
            S initialState,
            K<Supertype, A> ta) =>
            Subtype.FoldBackWhileM(f, predicate, initialState, Supertype.Transform(ta));

        /// <summary>
        /// Same behaviour as `Fold` but allows early exit of the operation once
        /// the predicate function becomes `false` for the state/value pair
        /// </summary>
        static S Foldable<Supertype>.FoldUntil<A, S>(
            Func<A, Func<S, S>> f,
            Func<(S State, A Value), bool> predicate,
            S initialState,
            K<Supertype, A> ta) =>
            Subtype.FoldUntil(f, predicate, initialState, Supertype.Transform(ta));

        /// <summary>
        /// Same behaviour as `Fold` but the fold operation returns a monadic type and allows
        /// early exit of the operation once the predicate function becomes `false` for the
        /// state/value pair 
        /// </summary>
        static K<M, S> Foldable<Supertype>.FoldUntilM<A, M, S>(
            Func<A, Func<S, K<M, S>>> f,
            Func<(S State, A Value), bool> predicate,
            S initialState,
            K<Supertype, A> ta) =>
            Subtype.FoldUntilM(f, predicate, initialState, Supertype.Transform(ta));

        /// <summary>
        /// Same behaviour as `FoldBack` but allows early exit of the operation once
        /// the predicate function becomes `false` for the state/value pair
        /// </summary>
        static S Foldable<Supertype>.FoldBackUntil<A, S>(
            Func<S, Func<A, S>> f,
            Func<(S State, A Value), bool> predicate,
            S initialState,
            K<Supertype, A> ta) =>
            Subtype.FoldBackUntil(f, predicate, initialState, Supertype.Transform(ta));

        /// <summary>
        /// Same behaviour as `FoldBack` but the fold operation returns a monadic type and allows
        /// early exit of the operation once the predicate function becomes `false` for the
        /// state/value pair 
        /// </summary>
        static K<M, S> Foldable<Supertype>.FoldBackUntilM<A, M, S>(
            Func<S, Func<A, K<M, S>>> f,
            Func<(S State, A Value), bool> predicate,
            S initialState,
            K<Supertype, A> ta) =>
            Subtype.FoldBackUntilM(f, predicate, initialState, Supertype.Transform(ta));

        /// <summary>
        /// Right-associative fold of a structure, lazy in the accumulator.
        ///
        /// In the case of lists, 'Fold', when applied to a binary operator, a
        /// starting value (typically the right-identity of the operator), and a
        /// list, reduces the list using the binary operator, from right to left.
        /// </summary>
        static S Foldable<Supertype>.Fold<A, S>(Func<A, Func<S, S>> f, S initialState, K<Supertype, A> ta) =>
            Subtype.Fold(f, initialState, Supertype.Transform(ta));

        /// <summary>
        /// Right-associative fold of a structure, lazy in the accumulator.
        ///
        /// In the case of lists, 'Fold', when applied to a binary operator, a
        /// starting value (typically the right-identity of the operator), and a
        /// list, reduces the list using the binary operator, from right to left.
        /// </summary>
        static K<M, S> Foldable<Supertype>.FoldM<A, M, S>(
            Func<A, Func<S, K<M, S>>> f,
            S initialState,
            K<Supertype, A> ta) =>
            Subtype.FoldM(f, initialState, Supertype.Transform(ta));

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
        static S Foldable<Supertype>.FoldBack<A, S>(Func<S, Func<A, S>> f, S initialState, K<Supertype, A> ta) =>
            Subtype.FoldBack(f, initialState, Supertype.Transform(ta));

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
        static K<M, S> Foldable<Supertype>.FoldBackM<A, M, S>(
            Func<S, Func<A, K<M, S>>> f,
            S initialState,
            K<Supertype, A> ta) =>
            Subtype.FoldBackM(f, initialState, Supertype.Transform(ta));

        /// <summary>
        /// Given a structure with elements whose type is a `Monoid`, combine them
        /// via the monoid's `Append` operator.  This fold is right-associative and
        /// lazy in the accumulator.  When you need a strict left-associative fold,
        /// use 'foldMap'' instead, with 'id' as the map.
        /// </summary>
        static A Foldable<Supertype>.Fold<A>(K<Supertype, A> tm) =>
            Subtype.Fold(Supertype.Transform(tm));

        /// <summary>
        /// Given a structure with elements whose type is a `Monoid`, combine them
        /// via the monoid's `Append` operator.  This fold is right-associative and
        /// lazy in the accumulator.  When you need a strict left-associative fold,
        /// use 'foldMap'' instead, with 'id' as the map.
        /// </summary>
        static A Foldable<Supertype>.FoldWhile<A>(Func<(A State, A Value), bool> predicate, K<Supertype, A> tm) =>
            Subtype.FoldWhile(predicate, Supertype.Transform(tm));

        /// <summary>
        /// Given a structure with elements whose type is a `Monoid`, combine them
        /// via the monoid's `Append` operator.  This fold is right-associative and
        /// lazy in the accumulator.  When you need a strict left-associative fold,
        /// use 'foldMap'' instead, with 'id' as the map.
        /// </summary>
        static A Foldable<Supertype>.FoldUntil<A>(Func<(A State, A Value), bool> predicate, K<Supertype, A> tm) =>
            Subtype.FoldUntil(predicate, Supertype.Transform(tm));

        /// <summary>
        /// Map each element of the structure into a monoid, and combine the
        /// results with `Append`.  This fold is right-associative and lazy in the
        /// accumulator.  For strict left-associative folds consider `FoldMapBack`
        /// instead.
        /// </summary>
        static B Foldable<Supertype>.FoldMap<A, B>(Func<A, B> f, K<Supertype, A> ta) =>
            Subtype.FoldMap(f, Supertype.Transform(ta));

        /// <summary>
        /// Map each element of the structure into a monoid, and combine the
        /// results with `Append`.  This fold is right-associative and lazy in the
        /// accumulator.  For strict left-associative folds consider `FoldMapBack`
        /// instead.
        /// </summary>
        static B Foldable<Supertype>.FoldMapWhile<A, B>(
            Func<A, B> f, Func<(B State, A Value), bool> predicate,
            K<Supertype, A> ta) =>
            Subtype.FoldMapWhile(f, predicate, Supertype.Transform(ta));

        /// <summary>
        /// Map each element of the structure into a monoid, and combine the
        /// results with `Append`.  This fold is right-associative and lazy in the
        /// accumulator.  For strict left-associative folds consider `FoldMapBack`
        /// instead.
        /// </summary>
        static B Foldable<Supertype>.FoldMapUntil<A, B>(
            Func<A, B> f, Func<(B State, A Value), bool> predicate,
            K<Supertype, A> ta) =>
            Subtype.FoldMapUntil(f, predicate, Supertype.Transform(ta));

        /// <summary>
        /// A left-associative variant of 'FoldMap' that is strict in the
        /// accumulator.  Use this method for strict reduction when partial
        /// results are merged via `Append`.
        /// </summary>
        static B Foldable<Supertype>.FoldMapBack<A, B>(Func<A, B> f, K<Supertype, A> ta) =>
            Subtype.FoldMapBack(f, Supertype.Transform(ta));

        /// <summary>
        /// A left-associative variant of 'FoldMap' that is strict in the
        /// accumulator.  Use this method for strict reduction when partial
        /// results are merged via `Append`.
        /// </summary>
        static B Foldable<Supertype>.FoldMapWhileBack<A, B>(
            Func<A, B> f, Func<(B State, A Value), bool> predicate,
            K<Supertype, A> ta) =>
            Subtype.FoldMapWhileBack(f, predicate, Supertype.Transform(ta));

        /// <summary>
        /// A left-associative variant of 'FoldMap' that is strict in the
        /// accumulator.  Use this method for strict reduction when partial
        /// results are merged via `Append`.
        /// </summary>
        static B Foldable<Supertype>.FoldMapUntilBack<A, B>(Func<A, B> f, Func<(B State, A Value), bool> predicate,
                                                            K<Supertype, A> ta) =>
            Subtype.FoldMapUntilBack(f, predicate, Supertype.Transform(ta));

        /// <summary>
        /// List of elements of a structure, from left to right
        /// </summary>
        static Seq<A> Foldable<Supertype>.ToSeq<A>(K<Supertype, A> ta) =>
            Subtype.ToSeq(Supertype.Transform(ta));

        /// <summary>
        /// List of elements of a structure, from left to right
        /// </summary>
        static Lst<A> Foldable<Supertype>.ToLst<A>(K<Supertype, A> ta) =>
            Subtype.ToLst(Supertype.Transform(ta));

        /// <summary>
        /// List of elements of a structure, from left to right
        /// </summary>
        static Arr<A> Foldable<Supertype>.ToArr<A>(K<Supertype, A> ta) =>
            Subtype.ToArr(Supertype.Transform(ta));

        /// <summary>
        /// List of elements of a structure, from left to right
        /// </summary>
        static Iterable<A> Foldable<Supertype>.ToIterable<A>(K<Supertype, A> ta) =>
            Subtype.ToIterable(Supertype.Transform(ta));

        /// <summary>
        /// List of elements of a structure, from left to right
        /// </summary>
        static bool Foldable<Supertype>.IsEmpty<A>(K<Supertype, A> ta) =>
            Subtype.IsEmpty(Supertype.Transform(ta));

        /// <summary>
        /// Returns the size/length of a finite structure as an `int`.  The
        /// default implementation just counts elements starting with the leftmost.
        /// 
        /// Instances for structures that can compute the element count faster
        /// than via element-by-element counting, should provide a specialised
        /// implementation.
        /// </summary>
        static int Foldable<Supertype>.Count<A>(K<Supertype, A> ta) =>
            Subtype.Count(Supertype.Transform(ta));

        /// <summary>
        /// Does an element that fits the predicate occur in the structure?
        /// </summary>
        static bool Foldable<Supertype>.Exists<A>(Func<A, bool> predicate, K<Supertype, A> ta) =>
            Subtype.Exists(predicate, Supertype.Transform(ta));

        /// <summary>
        /// Does the predicate hold for all elements in the structure?
        /// </summary>
        static bool Foldable<Supertype>.ForAll<A>(Func<A, bool> predicate, K<Supertype, A> ta) =>
            Subtype.ForAll(predicate, Supertype.Transform(ta));

        /// <summary>
        /// Does the element exist in the structure?
        /// </summary>
        static bool Foldable<Supertype>.Contains<EqA, A>(A value, K<Supertype, A> ta) =>
            Subtype.Contains(value, Supertype.Transform(ta));

        /// <summary>
        /// Does the element exist in the structure?
        /// </summary>
        static bool Foldable<Supertype>.Contains<A>(A value, K<Supertype, A> ta) =>
            Subtype.Contains(value, Supertype.Transform(ta));

        /// <summary>
        /// Find the first element that match the predicate
        /// </summary>
        static Option<A> Foldable<Supertype>.Find<A>(Func<A, bool> predicate, K<Supertype, A> ta) =>
            Subtype.Find(predicate, Supertype.Transform(ta));

        /// <summary>
        /// Find the last element that match the predicate
        /// </summary>
        static Option<A> Foldable<Supertype>.FindBack<A>(Func<A, bool> predicate, K<Supertype, A> ta) =>
            Subtype.FindBack(predicate, Supertype.Transform(ta));

        /// <summary>
        /// Find the elements that match the predicate
        /// </summary>
        static Iterable<A> Foldable<Supertype>.FindAll<A>(Func<A, bool> predicate, K<Supertype, A> ta) =>
            Subtype.FindAll(predicate, Supertype.Transform(ta));

        /// <summary>
        /// Find the elements that match the predicate
        /// </summary>
        static Iterable<A> Foldable<Supertype>.FindAllBack<A>(Func<A, bool> predicate, K<Supertype, A> ta) =>
            Subtype.FindAllBack(predicate, Supertype.Transform(ta));

        /// <summary>
        /// Computes the sum of the numbers of a structure.
        /// </summary>
        static A Foldable<Supertype>.Sum<A>(K<Supertype, A> ta) =>
            Subtype.Sum(Supertype.Transform(ta));

        /// <summary>
        /// Computes the product of the numbers of a structure.
        /// </summary>
        static A Foldable<Supertype>.Product<A>(K<Supertype, A> ta) =>
            Subtype.Product(Supertype.Transform(ta));

        /// <summary>
        /// Get the head item in the foldable or `None`
        /// </summary>
        static Option<A> Foldable<Supertype>.Head<A>(K<Supertype, A> ta) =>
            Subtype.Head(Supertype.Transform(ta));

        /// <summary>
        /// Get the head item in the foldable or `None`
        /// </summary>
        static Option<A> Foldable<Supertype>.Last<A>(K<Supertype, A> ta) =>
            Subtype.Last(Supertype.Transform(ta));

        /// <summary>
        /// Map each element of a structure to an 'Applicative' action, evaluate these
        /// actions from left to right, and ignore the results.  For a version that
        /// doesn't ignore the results see `Traversable.traverse`.
        /// </summary>
        static K<F, Unit> Foldable<Supertype>.Iter<F, A, B>(Func<A, K<F, B>> f, K<Supertype, A> ta) =>
            Subtype.Iter(f, Supertype.Transform(ta));

        /// <summary>
        /// Map each element of a structure to an action, evaluate these
        /// actions from left to right, and ignore the results.  For a version that
        /// doesn't ignore the results see `Traversable.traverse`.
        /// </summary>
        static Unit Foldable<Supertype>.Iter<A>(Action<A> f, K<Supertype, A> ta) =>
            Subtype.Iter(f, Supertype.Transform(ta));

        /// <summary>
        /// Map each element of a structure to an action, evaluate these
        /// actions from left to right, and ignore the results.  For a version that
        /// doesn't ignore the results see `Traversable.traverse`.
        /// </summary>
        static Unit Foldable<Supertype>.Iter<A>(Action<int, A> f, K<Supertype, A> ta) =>
            Subtype.Iter(f, Supertype.Transform(ta));

        /// <summary>
        /// Find the minimum value in the structure
        /// </summary>
        static Option<A> Foldable<Supertype>.Min<OrdA, A>(K<Supertype, A> ta) =>
            Subtype.Min<OrdA, A>(Supertype.Transform(ta));

        /// <summary>
        /// Find the minimum value in the structure
        /// </summary>
        static Option<A> Foldable<Supertype>.Min<A>(K<Supertype, A> ta) =>
            Subtype.Min(Supertype.Transform(ta));

        /// <summary>
        /// Find the maximum value in the structure
        /// </summary>
        static Option<A> Foldable<Supertype>.Max<OrdA, A>(K<Supertype, A> ta) =>
            Subtype.Max<OrdA, A>(Supertype.Transform(ta));

        /// <summary>
        /// Find the maximum value in the structure
        /// </summary>
        static Option<A> Foldable<Supertype>.Max<A>(K<Supertype, A> ta) =>
            Subtype.Max(Supertype.Transform(ta));

        /// <summary>
        /// Find the minimum value in the structure
        /// </summary>
        static A Foldable<Supertype>.Min<OrdA, A>(K<Supertype, A> ta, A initialMin) =>
            Subtype.Min<OrdA, A>(Supertype.Transform(ta), initialMin);

        /// <summary>
        /// Find the minimum value in the structure
        /// </summary>
        static A Foldable<Supertype>.Min<A>(K<Supertype, A> ta, A initialMin) =>
            Subtype.Min(Supertype.Transform(ta), initialMin);

        /// <summary>
        /// Find the maximum value in the structure
        /// </summary>
        static A Foldable<Supertype>.Max<OrdA, A>(K<Supertype, A> ta, A initialMax) =>
            Subtype.Max<OrdA, A>(Supertype.Transform(ta), initialMax);

        /// <summary>
        /// Find the maximum value in the structure
        /// </summary>
        static A Foldable<Supertype>.Max<A>(K<Supertype, A> ta, A initialMax) =>
            Subtype.Max(Supertype.Transform(ta), initialMax);

        /// <summary>
        /// Find the average of all the values in the structure
        /// </summary>
        static A Foldable<Supertype>.Average<A>(K<Supertype, A> ta) =>
            Subtype.Average(Supertype.Transform(ta));

        /// <summary>
        /// Find the average of all the values in the structure
        /// </summary>
        static B Foldable<Supertype>.Average<A, B>(Func<A, B> f, K<Supertype, A> ta) =>
            Subtype.Average(f, Supertype.Transform(ta));

        /// <summary>
        /// Find the element at the specified index or `None` if out of range
        /// </summary>
        static Option<A> Foldable<Supertype>.At<A>(K<Supertype, A> ta, Index index) =>
            Subtype.At(Supertype.Transform(ta), index);

        /// <summary>
        /// Partition a foldable into two sequences based on a predicate
        /// </summary>
        /// <param name="f">Predicate function</param>
        /// <param name="ta">Foldable structure</param>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>Partitioned structure</returns>
        static (Seq<A> True, Seq<A> False) Foldable<Supertype>.Partition<A>(Func<A, bool> f, K<Supertype, A> ta) =>
            Subtype.Partition(f, Supertype.Transform(ta));
    }
}
