/*
using System;

namespace LanguageExt.Traits;

public static class FoldableBackIO
{
    /// <summary>
    /// Fold the structure: `ta` and pass each element that it yields to `f`, resulting in an `F` applicative-value.
    /// The fold operator is applicative `Action`, which causes each applicative-value to be sequenced.      
    /// </summary>
    /// <param name="ta">Foldable structure</param>
    /// <param name="f">Mapping operation</param>
    /// <typeparam name="T">Foldable</typeparam>
    /// <typeparam name="F">Applicative</typeparam>
    /// <typeparam name="A">Input bound value</typeparam>
    /// <typeparam name="B">Mapping bound value</typeparam>
    /// <returns></returns>
    public static K<F, Unit> forM<T, F, A, B>(K<T, A> ta, Func<A, K<F, B>> f)
        where F : Applicative<F>
        where T : FoldableBackIO<T> =>
        ta.ForBackM(f);

    /// <summary>
    /// Same behaviour as `FoldBack` but allows early exit of the operation once
    /// the predicate function becomes `false` for the state/value pair 
    /// </summary>
    public static S foldWhile<T, A, S>(
        Func<S, A, S> f, 
        Func<(S State, A Value), bool> predicate, 
        S initialState, 
        K<T, A> ta) 
        where T : FoldableBackIO<T> =>
        ta.FoldBackWhile(f, predicate, initialState);

    /// <summary>
    /// Same behaviour as `FoldBack` but the fold operation returns a monadic type and allows
    /// early exit of the operation once the predicate function becomes `false` for the
    /// state/value pair 
    /// </summary>
    public static K<M, S> foldWhileM<T, A, M, S>(
        Func<S, A, K<M, S>> f, 
        Func<(S State, A Value), bool> predicate, 
        S initialState, 
        K<T, A> ta)
        where T : FoldableBackIO<T> 
        where M : Monad<M> =>
        ta.FoldBackWhileM(f, predicate, initialState);

    /// <summary>
    /// Same behaviour as `FoldBack` but allows early exit of the operation once
    /// the predicate function becomes `false` for the state/value pair
    /// </summary>
    public static S foldUntil<T, A, S>(
        Func<S, A, S> f, 
        Func<(S State, A Value), bool> predicate, 
        S initialState, 
        K<T, A> ta) 
        where T : FoldableBackIO<T> =>
        ta.FoldBackUntil(f, predicate, initialState);

    /// <summary>
    /// Same behaviour as `FoldBack` but the fold operation returns a monadic type and allows
    /// early exit of the operation once the predicate function becomes `false` for the
    /// state/value pair 
    /// </summary>
    public static K<M, S> foldUntilM<T, A, M, S>(
        Func<S, A, K<M, S>> f, 
        Func<(S State, A Value), bool> predicate, 
        S initialState, 
        K<T, A> ta)
        where T : FoldableBackIO<T> 
        where M : Monad<M> =>
        ta.FoldBackUntilM(f, predicate, initialState);
    
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
    /// `FoldBack' will diverge if given an infinite list.
    /// </remarks>
    public static S fold<T, A, S>(Func<S, A, S> f, S initialState, K<T, A> ta) 
        where T : FoldableBackIO<T> =>
        ta.FoldBack(f, initialState);

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
    /// `FoldBack' will diverge if given an infinite list.
    /// </remarks>
    public static K<M, S> foldM<T, A, M, S>(
        Func<S, A, K<M, S>> f, 
        S initialState, 
        K<T, A> ta)
        where T : FoldableBackIO<T>
        where M : Monad<M> =>
        ta.FoldBackM(f, initialState);

    /// <summary>
    /// List of elements of a structure, from left to right
    /// </summary>
    public static Seq<A> toSeq<T, A>(K<T, A> ta) 
        where T : FoldableBackIO<T> =>
        ta.ToSeqBack();

    /// <summary>
    /// List of elements of a structure, from left to right
    /// </summary>
    public static Lst<A> toLst<T, A>(K<T, A> ta) 
        where T : FoldableBackIO<T> =>
        ta.ToLstBack();

    /// <summary>
    /// List of elements of a structure, from left to right
    /// </summary>
    public static Arr<A> toArr<T, A>(K<T, A> ta)
        where T : FoldableBackIO<T> =>
        ta.ToArrBack();

    /// <summary>
    /// List of elements of a structure, from left to right
    /// </summary>
    public static Iterable<A> toIterable<T, A>(K<T, A> ta) 
        where T : FoldableBackIO<T> =>
        ta.ToIterableBack();

    /// <summary>
    /// Does an element that fits the predicate occur in the structure?
    /// </summary>
    public static bool exists<T, A>(Func<A, bool> predicate, K<T, A> ta) 
        where T : FoldableBackIO<T> =>
        ta.ExistsBack(predicate);

    /// <summary>
    /// Does the predicate hold for all elements in the structure?
    /// </summary>
    public static bool forAll<T, A>(Func<A, bool> predicate, K<T, A> ta) 
        where T : FoldableBackIO<T> =>
        ta.ForAllBack(predicate);

    /// <summary>
    /// Does the element exist in the structure?
    /// </summary>
    public static bool contains<EqA, T, A>(A value, K<T, A> ta) 
        where EqA : Eq<A> 
        where T : FoldableBackIO<T> =>
        T.ContainsBack<EqA, A>(value, ta);

    /// <summary>
    /// Does the element exist in the structure?
    /// </summary>
    public static bool contains<T, A>(A value, K<T, A> ta)
        where T : FoldableBackIO<T> =>
        ta.ContainsBack(value);

    /// <summary>
    /// Find the last element that match the predicate
    /// </summary>
    public static Option<A> find<T, A>(Func<A, bool> predicate, K<T, A> ta) 
        where T : FoldableBackIO<T> =>
        ta.FindBack(predicate);

    /// <summary>
    /// Find the elements that match the predicate
    /// </summary>
    public static Iterator<A> findAll<T, A>(Func<A, bool> predicate, K<T, A> ta) 
        where T : FoldableBackIO<T> =>
        ta.FindAllBack(predicate);

    /// <summary>
    /// Get the head item in the foldable or `None`
    /// </summary>
    public static Option<A> last<T, A>(K<T, A> ta) 
        where T : FoldableBackIO<T> =>
        ta.Last;

    /// <summary>
    /// Find the element at the specified index or `None` if out of range
    /// </summary>
    public static Option<A> at<T, A>(K<T, A> ta, int index)
        where T : FoldableBackIO<T> =>
        ta.AtBack(index);

    /// <summary>
    /// Partition a foldable into two sequences based on a predicate
    /// </summary>
    /// <param name="f">Predicate function</param>
    /// <param name="ta">Foldable structure</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Partitioned structure</returns>
    public static (Arr<A> True, Arr<A> False) partition<T, A>(Func<A, bool> f, K<T, A> ta)
        where T : FoldableBackIO<T> =>
        ta.PartitionBack(f);
}
*/
