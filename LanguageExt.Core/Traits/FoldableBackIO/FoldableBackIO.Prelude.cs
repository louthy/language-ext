/*
using System;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class Prelude
{
    /// <summary>
    /// Same behaviour as `FoldBack` but allows early exit of the operation once
    /// the predicate function becomes `false` for the state/value pair 
    /// </summary>
    public static S foldBackWhileIO<T, A, S>(
        Func<S, A, S> f, 
        Func<(S State, A Value), bool> predicate, 
        S initialState, 
        K<T, A> ta) 
        where T : FoldableBackIO<T> =>
        ta.FoldBackWhile(f, predicate, initialState);

    /// <summary>
    /// Same behaviour as `FoldBack` but allows early exit of the operation once
    /// the predicate function becomes `false` for the state/value pair
    /// </summary>
    public static S foldBackUntilIO<T, A, S>(
        Func<S, A, S> f, 
        Func<(S State, A Value), bool> predicate, 
        S initialState, 
        K<T, A> ta) 
        where T : FoldableBackIO<T> =>
        ta.FoldBackUntil(f, predicate, initialState);
    
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
    public static S foldBackIO<T, A, S>(Func<S, A, S> f, S initialState, K<T, A> ta) 
        where T : FoldableBackIO<T> =>
        ta.FoldBack(f, initialState);
}
*/
