using System;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class Prelude
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
    public static K<F, Unit> forBackM<T, F, A, B>(K<T, A> ta, Func<A, K<F, B>> f)
        where F : Applicative<F>
        where T : FoldableBack<T> =>
        ta.ForBackM(f);

    /// <summary>
    /// Same behaviour as `FoldBack` but allows early exit of the operation once
    /// the predicate function becomes `false` for the state/value pair 
    /// </summary>
    public static S foldBackWhile<T, A, S>(
        Func<S, A, S> f, 
        Func<(S State, A Value), bool> predicate, 
        S initialState, 
        K<T, A> ta) 
        where T : FoldableBack<T> =>
        ta.FoldBackWhile(f, predicate, initialState);

    /// <summary>
    /// Same behaviour as `FoldBack` but the fold operation returns a monadic type and allows
    /// early exit of the operation once the predicate function becomes `false` for the
    /// state/value pair 
    /// </summary>
    public static K<M, S> foldBackWhileM<T, A, M, S>(
        Func<S, A, K<M, S>> f, 
        Func<(S State, A Value), bool> predicate, 
        S initialState, 
        K<T, A> ta)
        where T : FoldableBack<T> 
        where M : Monad<M> =>
        ta.FoldBackWhileM(f, predicate, initialState);

    /// <summary>
    /// Same behaviour as `FoldBack` but allows early exit of the operation once
    /// the predicate function becomes `false` for the state/value pair
    /// </summary>
    public static S foldBackUntil<T, A, S>(
        Func<S, A, S> f, 
        Func<(S State, A Value), bool> predicate, 
        S initialState, 
        K<T, A> ta) 
        where T : FoldableBack<T> =>
        ta.FoldBackUntil(f, predicate, initialState);

    /// <summary>
    /// Same behaviour as `FoldBack` but the fold operation returns a monadic type and allows
    /// early exit of the operation once the predicate function becomes `false` for the
    /// state/value pair 
    /// </summary>
    public static K<M, S> foldBackUntilM<T, A, M, S>(
        Func<S, A, K<M, S>> f, 
        Func<(S State, A Value), bool> predicate, 
        S initialState, 
        K<T, A> ta)
        where T : FoldableBack<T> 
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
    public static S foldBack<T, A, S>(Func<S, A, S> f, S initialState, K<T, A> ta) 
        where T : FoldableBack<T> =>
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
    public static K<M, S> foldBackM<T, A, M, S>(
        Func<S, A, K<M, S>> f, 
        S initialState, 
        K<T, A> ta)
        where T : FoldableBack<T>
        where M : Monad<M> =>
        ta.FoldBackM(f, initialState);

    /// <summary>
    /// Does an element that fits the predicate occur in the structure?
    /// </summary>
    public static bool existsBack<T, A>(Func<A, bool> predicate, K<T, A> ta) 
        where T : FoldableBack<T> =>
        ta.ExistsBack(predicate);

    /// <summary>
    /// Does the predicate hold for all elements in the structure?
    /// </summary>
    public static bool forAllBack<T, A>(Func<A, bool> predicate, K<T, A> ta) 
        where T : FoldableBack<T> =>
        ta.ForAllBack(predicate);

    /// <summary>
    /// Partition a foldable into two sequences based on a predicate
    /// </summary>
    /// <param name="f">Predicate function</param>
    /// <param name="ta">Foldable structure</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Partitioned structure</returns>
    public static (Arr<A> True, Arr<A> False) partitionBack<T, A>(Func<A, bool> f, K<T, A> ta)
        where T : FoldableBack<T> =>
        ta.PartitionBack(f);
}
