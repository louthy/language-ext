using System;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt.Traits;

public interface Foldable<out T> where T : Foldable<T>
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Abstract members
    //

    /// <summary>
    /// Right-associative fold of a structure, lazy in the accumulator.
    ///
    /// In the case of lists, 'Fold', when applied to a binary operator, a
    /// starting value (typically the right-identity of the operator), and a
    /// list, reduces the list using the binary operator, from right to left.
    /// </summary>
    public static abstract S Fold<A, S>(Func<A, Func<S, S>> f, S initialState, K<T, A> ta);

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
    public static abstract S FoldBack<A, S>(Func<S, Func<A, S>> f, S initialState, K<T, A> ta);
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Default implementations
    //

    /// <summary>
    /// Right-associative fold of a structure, lazy in the accumulator.
    ///
    /// In the case of lists, 'Fold', when applied to a binary operator, a
    /// starting value (typically the right-identity of the operator), and a
    /// list, reduces the list using the binary operator, from right to left.
    /// </summary>
    public static virtual S Fold<A, S>(Func<A, S, S> f, S initialState, K<T, A> ta) =>
       T.Fold(curry(f), initialState, ta);

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
    public static virtual S FoldBack<A, S>(Func<S, A, S> f, S initialState, K<T, A> ta) =>
        T.FoldBack(curry(f), initialState, ta);

    /// <summary>
    /// Given a structure with elements whose type is a `Monoid`, combine them
    /// via the monoid's `Append` operator.  This fold is right-associative and
    /// lazy in the accumulator.  When you need a strict left-associative fold,
    /// use 'foldMap'' instead, with 'id' as the map.
    /// </summary>
    public static virtual A Fold<A>(K<T, A> tm) 
        where A : Monoid<A> =>
        T.FoldMap(identity, tm) ;

    /// <summary>
    /// Map each element of the structure into a monoid, and combine the
    /// results with `Append`.  This fold is right-associative and lazy in the
    /// accumulator.  For strict left-associative folds consider `FoldMapBack`
    /// instead.
    /// </summary>
    public static virtual B FoldMap<A, B>(Func<A, B> f, K<T, A> ta)
        where B : Monoid<B> =>
        T.Fold((x, a) => f(x).Append(a), B.Empty, ta);

    /// <summary>
    /// A left-associative variant of 'FoldMap' that is strict in the
    /// accumulator.  Use this method for strict reduction when partial
    /// results are merged via `Append`.
    /// </summary>
    public static virtual B FoldMapBack<A, B>(Func<A, B> f, K<T, A> ta)
        where B : Monoid<B> =>
        T.FoldBack((x, a) => x.Append(f(a)), B.Empty, ta);

    /// <summary>
    /// List of elements of a structure, from left to right
    /// </summary>
    public static virtual Seq<A> ToSeq<A>(K<T, A> ta) =>
        T.Fold((a, s) => s.Add(a), Seq<A>(), ta);

    /// <summary>
    /// List of elements of a structure, from left to right
    /// </summary>
    public static virtual bool IsEmpty<A>(K<T, A> ta) =>
        T.Fold((_, _) => false, true, ta);

    /// <summary>
    /// Returns the size/length of a finite structure as an `int`.  The
    /// default implementation just counts elements starting with the leftmost.
    /// 
    /// Instances for structures that can compute the element count faster
    /// than via element-by-element counting, should provide a specialised
    /// implementation.
    /// </summary>
    public static virtual int Count<A>(K<T, A> ta) =>
        T.FoldBack((c, _) => c + 1, 0, ta);

    /// <summary>
    /// Does an element that fits the predicate occur in the structure?
    /// </summary>
    public static virtual bool Exists<A>(Func<A, bool> predicate, K<T, A> ta) =>
        T.FoldBack((s, c) => s || predicate(c), false, ta);

    /// <summary>
    /// Does the predicate hold for all elements in the structure?
    /// </summary>
    public static virtual bool ForAll<A>(Func<A, bool> predicate, K<T, A> ta) =>
        T.FoldBack((s, c) => s && predicate(c), true, ta);

    /// <summary>
    /// Does the element exist in the structure?
    /// </summary>
    public static virtual bool Contains<EqA, A>(A value, K<T, A> ta) where EqA : Eq<A> =>
        T.Exists(x => EqA.Equals(value, x), ta);

    /// <summary>
    /// Computes the sum of the numbers of a structure.
    /// </summary>
    public static virtual A Sum<NumA, A>(K<T, A> ta) where NumA : Num<A> =>
        T.Fold(NumA.Plus, NumA.FromInteger(0), ta);

    /// <summary>
    /// Computes the sum of the numbers of a structure.
    /// </summary>
    public static virtual A Product<NumA, A>(K<T, A> ta) where NumA : Num<A> =>
        T.Fold(NumA.Product, NumA.FromInteger(1), ta);
}
