using System;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class Deriving
{
    public interface FoldableBack<Supertype, Subtype> :
        FoldableBack<Supertype>,
        Traits.Natural<Supertype, Subtype>,
        Traits.CoNatural<Supertype, Subtype>
        where Supertype : FoldableBack<Supertype>, FoldableBack<Supertype, Subtype>
        where Subtype : FoldableBack<Subtype>
    {
        /// <summary>
        /// Same behaviour as `FoldBack` but allows early exit of the operation once
        /// the predicate function becomes `false` for the state/value pair 
        /// </summary>
        static S FoldableBack<Supertype>.FoldBackWhile<A, S>(
            Func<S, A, S> f,
            Func<(S State, A Value), bool> predicate,
            in S initialState,
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
        static S FoldableBack<Supertype>.FoldBackMaybe<A, S>(
            Func<S, A, Option<S>> f,
            in S initialState,
            K<Supertype, A> ta) =>
            Subtype.FoldBackMaybe(f, initialState, Supertype.Transform(ta));

        /// <summary>
        /// Same behaviour as `FoldBack` but the fold operation returns a monadic type and allows
        /// early exit of the operation once the predicate function becomes `false` for the
        /// state/value pair 
        /// </summary>
        static MS FoldableBack<Supertype>.FoldBackWhileM<MS, M, A, S>(
            Func<S, A, MS> f,
            Func<(S State, A Value), bool> predicate,
            in S initialState,
            K<Supertype, A> ta) =>
            Subtype.FoldBackWhileM<MS, M, A, S>(f, predicate, initialState, Supertype.Transform(ta));

        /// <summary>
        /// Same behaviour as `FoldBack` but allows early exit of the operation once
        /// the predicate function becomes `false` for the state/value pair
        /// </summary>
        static S FoldableBack<Supertype>.FoldBackUntil<A, S>(
            Func<S, A, S> f,
            Func<(S State, A Value), bool> predicate,
            in S initialState,
            K<Supertype, A> ta) =>
            Subtype.FoldBackUntil(f, predicate, initialState, Supertype.Transform(ta));

        /// <summary>
        /// Same behaviour as `FoldBack` but the fold operation returns a monadic type and allows
        /// early exit of the operation once the predicate function becomes `false` for the
        /// state/value pair 
        /// </summary>
        static MS FoldableBack<Supertype>.FoldBackUntilM<MS, M, A, S>(
            Func<S, A, MS> f,
            Func<(S State, A Value), bool> predicate,
            in S initialState,
            K<Supertype, A> ta) =>
            Subtype.FoldBackUntilM<MS, M, A, S>(f, predicate, initialState, Supertype.Transform(ta));

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
        static S FoldableBack<Supertype>.FoldBack<A, S>(Func<S, A, S> f, in S initialState, K<Supertype, A> ta) =>
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
        static MS FoldableBack<Supertype>.FoldBackM<MS, M, A, S>(
            Func<S, A, MS> f,
            in S initialState,
            K<Supertype, A> ta) =>
            Subtype.FoldBackM<MS, M, A, S>(f, initialState, Supertype.Transform(ta));

        /// <summary>
        /// List of elements of a structure, from left to right
        /// </summary>
        static Seq<A> FoldableBack<Supertype>.ToSeqBack<A>(K<Supertype, A> ta) =>
            Subtype.ToSeqBack(Supertype.Transform(ta));

        /// <summary>
        /// List of elements of a structure, from left to right
        /// </summary>
        static Lst<A> FoldableBack<Supertype>.ToLstBack<A>(K<Supertype, A> ta) =>
            Subtype.ToLstBack(Supertype.Transform(ta));

        /// <summary>
        /// List of elements of a structure, from left to right
        /// </summary>
        static Arr<A> FoldableBack<Supertype>.ToArrBack<A>(K<Supertype, A> ta) =>
            Subtype.ToArrBack(Supertype.Transform(ta));

        /// <summary>
        /// List of elements of a structure, from left to right
        /// </summary>
        static Iterable<A> FoldableBack<Supertype>.ToIterableBack<A>(K<Supertype, A> ta) =>
            Subtype.ToIterableBack(Supertype.Transform(ta));

        /// <summary>
        /// Does an element that fits the predicate occur in the structure?
        /// </summary>
        static bool FoldableBack<Supertype>.ExistsBack<A>(Func<A, bool> predicate, K<Supertype, A> ta) =>
            Subtype.ExistsBack(predicate, Supertype.Transform(ta));

        /// <summary>
        /// Does the predicate hold for all elements in the structure?
        /// </summary>
        static bool FoldableBack<Supertype>.ForAllBack<A>(Func<A, bool> predicate, K<Supertype, A> ta) =>
            Subtype.ForAllBack(predicate, Supertype.Transform(ta));

        /// <summary>
        /// Does the element exist in the structure?
        /// </summary>
        static bool FoldableBack<Supertype>.ContainsBack<EqA, A>(A value, K<Supertype, A> ta) =>
            Subtype.ContainsBack(value, Supertype.Transform(ta));

        /// <summary>
        /// Does the element exist in the structure?
        /// </summary>
        static bool FoldableBack<Supertype>.ContainsBack<A>(A value, K<Supertype, A> ta) =>
            Subtype.ContainsBack(value, Supertype.Transform(ta));

        /// <summary>
        /// Find the last element that match the predicate
        /// </summary>
        static Option<A> FoldableBack<Supertype>.FindBack<A>(Func<A, bool> predicate, K<Supertype, A> ta) =>
            Subtype.FindBack(predicate, Supertype.Transform(ta));

        /// <summary>
        /// Find the elements that match the predicate
        /// </summary>
        static Iterator<A> FoldableBack<Supertype>.FindAllBack<A>(Func<A, bool> predicate, K<Supertype, A> ta) =>
            Subtype.FindAllBack(predicate, Supertype.Transform(ta));

        /// <summary>
        /// Get the head item in the foldable or `None`
        /// </summary>
        static Option<A> FoldableBack<Supertype>.Last<A>(K<Supertype, A> ta) =>
            Subtype.Last(Supertype.Transform(ta));

        /// <summary>
        /// Find the element at the specified index or `None` if out of range
        /// </summary>
        static Option<A> FoldableBack<Supertype>.AtBack<A>(long index, K<Supertype, A> ta) =>
            Subtype.AtBack(index, Supertype.Transform(ta));

        /// <summary>
        /// Partition a foldable into two sequences based on a predicate
        /// </summary>
        /// <param name="f">Predicate function</param>
        /// <param name="ta">Foldable structure</param>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>Partitioned structure</returns>
        static (Arr<A> True, Arr<A> False) FoldableBack<Supertype>.PartitionBack<A>(Func<A, bool> f, K<Supertype, A> ta) =>
            Subtype.PartitionBack(f, Supertype.Transform(ta));
        
        static Iterator<A> IterableBackK<Supertype>.BackwardIterator<A>(K<Supertype, A> fa) => 
            Subtype.BackwardIterator(Supertype.Transform(fa));
    }
}
