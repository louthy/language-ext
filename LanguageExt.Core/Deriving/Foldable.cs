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

        static K<Sub, A> transform<Super, Sub, A>(K<Super, A> ta) 
            where Super : Traits.Natural<Super, Sub> =>
            Super.Transform(ta);
        
        /// <summary>
        /// Same behaviour as `Fold` but allows early exit of the operation once
        /// the predicate function becomes `false` for the state/value pair 
        /// </summary>
        static S Foldable<Supertype>.FoldWhile<A, S>(
            Func<S, A, S> f,
            Func<(S State, A Value), bool> predicate,
            in S initialState,
            K<Supertype, A> ta) =>
            Subtype.FoldWhile(f, predicate, initialState, transform<Supertype, Subtype, A>(ta));

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
            Func<S, A, Option<S>> f,
            in S initialState,
            K<Supertype, A> ta) =>
            Subtype.FoldMaybe(f, initialState, transform<Supertype, Subtype, A>(ta));

        /// <summary>
        /// Same behaviour as `Fold` but the fold operation returns a monadic type and allows
        /// early exit of the operation once the predicate function becomes `false` for the
        /// state/value pair 
        /// </summary>
        static MS Foldable<Supertype>.FoldWhileM<MS, M, A, S>(
            Func<S, A, MS> f,
            Func<(S State, A Value), bool> predicate,
            in S initialState,
            K<Supertype, A> ta) =>
            Subtype.FoldWhileM<MS, M, A, S>(f, predicate, initialState, transform<Supertype, Subtype, A>(ta));

        /// <summary>
        /// Same behaviour as `Fold` but allows early exit of the operation once
        /// the predicate function becomes `false` for the state/value pair
        /// </summary>
        static S Foldable<Supertype>.FoldUntil<A, S>(
            Func<S, A, S> f,
            Func<(S State, A Value), bool> predicate,
            in S initialState,
            K<Supertype, A> ta) =>
            Subtype.FoldUntil(f, predicate, initialState, transform<Supertype, Subtype, A>(ta));

        /// <summary>
        /// Same behaviour as `Fold` but the fold operation returns a monadic type and allows
        /// early exit of the operation once the predicate function becomes `false` for the
        /// state/value pair 
        /// </summary>
        static MS Foldable<Supertype>.FoldUntilM<MS, M, A, S>(
            Func<S, A, MS> f,
            Func<(S State, A Value), bool> predicate,
            in S initialState,
            K<Supertype, A> ta) =>
            Subtype.FoldUntilM<MS, M, A, S>(f, predicate, initialState, transform<Supertype, Subtype, A>(ta));

        /// <summary>
        /// Right-associative fold of a structure, lazy in the accumulator.
        ///
        /// In the case of lists, 'Fold', when applied to a binary operator, a
        /// starting value (typically the right-identity of the operator), and a
        /// list, reduces the list using the binary operator, from right to left.
        /// </summary>
        static S Foldable<Supertype>.Fold<A, S>(Func<S, A, S> f, in S initialState, K<Supertype, A> ta) =>
            Subtype.Fold(f, initialState, transform<Supertype, Subtype, A>(ta));

        /// <summary>
        /// Right-associative fold of a structure, lazy in the accumulator.
        ///
        /// In the case of lists, 'Fold', when applied to a binary operator, a
        /// starting value (typically the right-identity of the operator), and a
        /// list, reduces the list using the binary operator, from right to left.
        /// </summary>
        static MS Foldable<Supertype>.FoldM<MS, M, A, S>(
            Func<S, A, MS> f,
            in S initialState,
            K<Supertype, A> ta) =>
            Subtype.FoldM<MS, M, A, S>(f, initialState, transform<Supertype, Subtype, A>(ta));

        /// <summary>
        /// List of elements of a structure, from left to right
        /// </summary>
        static Seq<A> Foldable<Supertype>.ToSeq<A>(K<Supertype, A> ta) =>
            Subtype.ToSeq(transform<Supertype, Subtype, A>(ta));

        /// <summary>
        /// List of elements of a structure, from left to right
        /// </summary>
        static Lst<A> Foldable<Supertype>.ToLst<A>(K<Supertype, A> ta) =>
            Subtype.ToLst(transform<Supertype, Subtype, A>(ta));

        /// <summary>
        /// List of elements of a structure, from left to right
        /// </summary>
        static Arr<A> Foldable<Supertype>.ToArr<A>(K<Supertype, A> ta) =>
            Subtype.ToArr(transform<Supertype, Subtype, A>(ta));

        /// <summary>
        /// List of elements of a structure, from left to right
        /// </summary>
        static Iterable<A> Foldable<Supertype>.ToIterable<A>(K<Supertype, A> ta) =>
            Subtype.ToIterable(transform<Supertype, Subtype, A>(ta));

        /// <summary>
        /// List of elements of a structure, from left to right
        /// </summary>
        static bool Foldable<Supertype>.IsEmpty<A>(K<Supertype, A> ta) =>
            Subtype.IsEmpty(transform<Supertype, Subtype, A>(ta));

        /// <summary>
        /// Does an element that fits the predicate occur in the structure?
        /// </summary>
        static bool Foldable<Supertype>.Exists<A>(Func<A, bool> predicate, K<Supertype, A> ta) =>
            Subtype.Exists(predicate, transform<Supertype, Subtype, A>(ta));

        /// <summary>
        /// Does the predicate hold for all elements in the structure?
        /// </summary>
        static bool Foldable<Supertype>.ForAll<A>(Func<A, bool> predicate, K<Supertype, A> ta) =>
            Subtype.ForAll(predicate, transform<Supertype, Subtype, A>(ta));

        /// <summary>
        /// Does the element exist in the structure?
        /// </summary>
        static bool Foldable<Supertype>.Contains<EqA, A>(A value, K<Supertype, A> ta) =>
            Subtype.Contains(value, transform<Supertype, Subtype, A>(ta));

        /// <summary>
        /// Does the element exist in the structure?
        /// </summary>
        static bool Foldable<Supertype>.Contains<A>(A value, K<Supertype, A> ta) =>
            Subtype.Contains(value, transform<Supertype, Subtype, A>(ta));

        /// <summary>
        /// Find the first element that match the predicate
        /// </summary>
        static Option<A> Foldable<Supertype>.Find<A>(
            Option<long> startIndex,
            Option<long> count,
            Func<A, bool> predicate,
            K<Supertype, A> ta) =>
            Subtype.Find(startIndex, count, predicate, transform<Supertype, Subtype, A>(ta));

        /// <summary>
        /// Get the head item in the foldable or `None`
        /// </summary>
        static Option<A> Foldable<Supertype>.Head<A>(K<Supertype, A> ta) =>
            Subtype.Head(transform<Supertype, Subtype, A>(ta));

        /// <summary>
        /// Map each element of a structure to an 'Applicative' action, evaluate these
        /// actions from left to right, and ignore the results.  For a version that
        /// doesn't ignore the results see `Traversable.traverse`.
        /// </summary>
        static K<M, Unit> Foldable<Supertype>.IterM<MB, M, A, B>(Func<A, MB> f, K<Supertype, A> ta) =>
            Subtype.IterM<MB, M, A, B>(f, transform<Supertype, Subtype, A>(ta));

        /// <summary>
        /// Map each element of a structure to an action, evaluate these
        /// actions from left to right, and ignore the results.  For a version that
        /// doesn't ignore the results see `Traversable.traverse`.
        /// </summary>
        static Unit Foldable<Supertype>.Iter<A>(Action<A> f, K<Supertype, A> ta) =>
            Subtype.Iter(f, transform<Supertype, Subtype, A>(ta));

        /// <summary>
        /// Map each element of a structure to an action, evaluate these
        /// actions from left to right, and ignore the results.  For a version that
        /// doesn't ignore the results see `Traversable.traverse`.
        /// </summary>
        static Unit Foldable<Supertype>.Iter<A>(Action<long, A> f, long initialIndex, K<Supertype, A> ta) =>
            Subtype.Iter(f, initialIndex, transform<Supertype, Subtype, A>(ta));

        /// <summary>
        /// Find the minimum value in the structure
        /// </summary>
        static Option<A> Foldable<Supertype>.Min<OrdA, A>(K<Supertype, A> ta) =>
            Subtype.Min<OrdA, A>(transform<Supertype, Subtype, A>(ta));

        /// <summary>
        /// Find the minimum value in the structure
        /// </summary>
        static Option<A> Foldable<Supertype>.Min<A>(K<Supertype, A> ta) =>
            Subtype.Min(transform<Supertype, Subtype, A>(ta));

        /// <summary>
        /// Find the maximum value in the structure
        /// </summary>
        static Option<A> Foldable<Supertype>.Max<OrdA, A>(K<Supertype, A> ta) =>
            Subtype.Max<OrdA, A>(transform<Supertype, Subtype, A>(ta));

        /// <summary>
        /// Find the maximum value in the structure
        /// </summary>
        static Option<A> Foldable<Supertype>.Max<A>(K<Supertype, A> ta) =>
            Subtype.Max(transform<Supertype, Subtype, A>(ta));

        /// <summary>
        /// Find the minimum value in the structure
        /// </summary>
        static A Foldable<Supertype>.Min<OrdA, A>(A initialMin, K<Supertype, A> ta) =>
            Subtype.Min<OrdA, A>(initialMin, transform<Supertype, Subtype, A>(ta));

        /// <summary>
        /// Find the minimum value in the structure
        /// </summary>
        static A Foldable<Supertype>.Min<A>(A initialMin, K<Supertype, A> ta) =>
            Subtype.Min(initialMin, transform<Supertype, Subtype, A>(ta));

        /// <summary>
        /// Find the maximum value in the structure
        /// </summary>
        static A Foldable<Supertype>.Max<OrdA, A>(A initialMax, K<Supertype, A> ta) =>
            Subtype.Max<OrdA, A>(initialMax, transform<Supertype, Subtype, A>(ta));

        static K<M, A> Foldable<Supertype>.HeadM<M, A>(K<Supertype, A> ta) => 
            Subtype.HeadM<M, A>(Supertype.Transform(ta));

        static Iterator<A> Foldable<Supertype>.Intersperse<A>(A sep, K<Supertype, A> ta) => 
            Subtype.Intersperse(sep, transform<Supertype, Subtype, A>(ta));

        /// <summary>
        /// Find the maximum value in the structure
        /// </summary>
        static A Foldable<Supertype>.Max<A>(A initialMax, K<Supertype, A> ta) =>
            Subtype.Max(initialMax, transform<Supertype, Subtype, A>(ta));
                
        /// <summary>
        /// Find the first index of an element in the structure that matches the predicate
        /// </summary>
        /// <param name="startIndex">Initial index to start the search</param>
        /// <param name="count">Maximum number of elements to test before giving up</param>
        /// <param name="ta">Foldable structure</param>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>`Some(index)` if the predicate returns `true`, otherwise `None`</returns>
        static Option<long> Foldable<Supertype>.IndexOf<A>(
            Option<long> startIndex, 
            Option<long> count, 
            Func<A, bool> predicate, 
            K<Supertype, A> ta) => 
            Subtype.IndexOf(startIndex, count, predicate, Supertype.Transform(ta));

        /// <summary>
        /// Partition foldable into two sequences based on a predicate
        /// </summary>
        /// <param name="f">Predicate function</param>
        /// <param name="ta">Foldable structure</param>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>Partitioned structure</returns>
        static (Arr<A> True, Arr<A> False) Foldable<Supertype>.Partition<A>(Func<A, bool> f, K<Supertype, A> ta) =>
            Subtype.Partition(f, transform<Supertype, Subtype, A>(ta));

        static Iterator<A> IterableK<Supertype>.ForwardIterator<A>(K<Supertype, A> fa) => 
            Subtype.ForwardIterator(transform<Supertype, Subtype, A>(fa));
    }
}
