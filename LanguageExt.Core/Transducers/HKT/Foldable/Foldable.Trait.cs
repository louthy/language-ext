using System;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;

namespace LanguageExt.HKT;

public interface Foldable<T> where T : Foldable<T>
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
    public static abstract S Fold<A, S>(Func<A, S, S> f, S initialState, Foldable<T, A> ta);

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
    public static abstract B FoldBack<A, B>(Func<B, A, B> f, B initialState, Foldable<T, A> ta);
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Default implementations
    //

    /// <summary>
    /// Given a structure with elements whose type is a `Monoid`, combine them
    /// via the monoid's `Append` operator.  This fold is right-associative and
    /// lazy in the accumulator.  When you need a strict left-associative fold,
    /// use 'foldMap'' instead, with 'id' as the map.
    /// </summary>
    public static virtual A Fold<M, A>(Foldable<T, A> tm) 
        where M : Monoid<A> =>
        T.FoldMap<M, A, A>(identity, tm) ;

    /// <summary>
    /// Map each element of the structure into a monoid, and combine the
    /// results with `Append`.  This fold is right-associative and lazy in the
    /// accumulator.  For strict left-associative folds consider `FoldMapBack`
    /// instead.
    /// </summary>
    public static virtual B FoldMap<M, A, B>(Func<A, B> f, Foldable<T, A> ta)
        where M : Monoid<B> =>
        T.Fold((x, a) => M.Append(f(x), a), M.Empty, ta);

    /// <summary>
    /// A left-associative variant of 'FoldMap' that is strict in the
    /// accumulator.  Use this method for strict reduction when partial
    /// results are merged via `Append`.
    /// </summary>
    public static virtual B FoldMapBack<M, A, B>(Func<A, B> f, Foldable<T, A> ta)
        where M : Monoid<B> =>
        T.FoldBack((x, a) => M.Append(x, f(a)), M.Empty, ta);

    /// <summary>
    /// List of elements of a structure, from left to right
    /// </summary>
    public static virtual Seq<A> ToSeq<A>(Foldable<T, A> ta) =>
        T.Fold((a, s) => s.Add(a), Seq<A>(), ta);

    /// <summary>
    /// List of elements of a structure, from left to right
    /// </summary>
    public static virtual bool IsEmpty<A>(Foldable<T, A> ta) =>
        T.Fold((_, _) => false, true, ta);

    /// <summary>
    /// Returns the size/length of a finite structure as an `int`.  The
    /// default implementation just counts elements starting with the leftmost.
    /// 
    /// Instances for structures that can compute the element count faster
    /// than via element-by-element counting, should provide a specialised
    /// implementation.
    /// </summary>
    public static virtual int Count<A>(Foldable<T, A> ta) =>
        T.FoldBack((c, _) => c + 1, 0, ta);

    /// <summary>
    /// Does an element that fits the predicate occur in the structure?
    /// </summary>
    public static virtual bool Exists<A>(Func<A, bool> predicate, Foldable<T, A> ta) =>
        T.FoldBack((s, c) => s || predicate(c), false, ta);

    /// <summary>
    /// Does the predicate hold for all elements in the structure?
    /// </summary>
    public static virtual bool ForAll<A>(Func<A, bool> predicate, Foldable<T, A> ta) =>
        T.FoldBack((s, c) => s && predicate(c), true, ta);

    /// <summary>
    /// Does the element exist in the structure?
    /// </summary>
    public static virtual bool Contains<EqA, A>(A value, Foldable<T, A> ta) where EqA : Eq<A> =>
        T.Exists(x => EqA.Equals(value, x), ta);

    /// <summary>
    /// Computes the sum of the numbers of a structure.
    /// </summary>
    public static virtual A Sum<NumA, A>(Foldable<T, A> ta) where NumA : Num<A> =>
        T.Fold(NumA.Plus, NumA.Empty, ta);

    /// <summary>
    /// Computes the sum of the numbers of a structure.
    /// </summary>
    public static virtual A Product<NumA, A>(Foldable<T, A> ta) where NumA : Num<A> =>
        T.Fold(NumA.Product, NumA.FromInteger(1), ta);
}
