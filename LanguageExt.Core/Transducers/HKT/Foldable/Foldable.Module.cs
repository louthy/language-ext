using System;
using LanguageExt.ClassInstances;
using LanguageExt.TypeClasses;

namespace LanguageExt.HKT;

public static class Foldable
{
    /// <summary>
    /// Right-associative fold of a structure, lazy in the accumulator.
    ///
    /// In the case of lists, 'Fold', when applied to a binary operator, a
    /// starting value (typically the right-identity of the operator), and a
    /// list, reduces the list using the binary operator, from right to left.
    /// </summary>
    public static S fold<T, A, S>(Func<A, S, S> f, S initialState, Foldable<T, A> ta)
        where T : Foldable<T> =>
        T.Fold(f, initialState, ta);
    
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
    public static S foldBack<T, A, S>(Func<S, A, S> f, S initialState, Foldable<T, A> ta)
        where T : Foldable<T> =>
        T.FoldBack(f, initialState, ta);

    /// <summary>
    /// Given a structure with elements whose type is a `Monoid`, combine them
    /// via the monoid's `Append` operator.  This fold is right-associative and
    /// lazy in the accumulator.  When you need a strict left-associative fold,
    /// use 'foldMap'' instead, with 'id' as the map.
    /// </summary>
    public static A fold<T, M, A>(Foldable<T, A> tm)
        where M : Monoid<A>
        where T : Foldable<T> =>
        T.Fold<M, A>(tm);

    /// <summary>
    /// Map each element of the structure into a monoid, and combine the
    /// results with `Append`.  This fold is right-associative and lazy in the
    /// accumulator.  For strict left-associative folds consider `FoldMapBack`
    /// instead.
    /// </summary>
    public static B foldMap<T, M, A, B>(Func<A, B> f, Foldable<T, A> ta)
        where M : Monoid<B>
        where T : Foldable<T> =>
        T.FoldMap<M, A, B>(f, ta);

    /// <summary>
    /// A left-associative variant of 'FoldMap' that is strict in the
    /// accumulator.  Use this method for strict reduction when partial
    /// results are merged via `Append`.
    /// </summary>
    public static B foldMapBack<T, M, A, B>(Func<A, B> f, Foldable<T, A> ta)
        where M : Monoid<B>
        where T : Foldable<T> =>
        T.FoldBack((x, a) => M.Append(x, f(a)), M.Empty, ta);

    /// <summary>
    /// List of elements of a structure, from left to right
    /// </summary>
    public static Seq<A> toSeq<T, A>(Foldable<T, A> ta)
        where T : Foldable<T> =>
        T.ToSeq(ta);

    /// <summary>
    /// List of elements of a structure, from left to right
    /// </summary>
    public static bool isEmpty<T, A>(Foldable<T, A> ta)
        where T : Foldable<T> =>
        T.IsEmpty(ta);

    /// <summary>
    /// Returns the size/length of a finite structure as an `int`.  The
    /// default implementation just counts elements starting with the leftmost.
    /// 
    /// Instances for structures that can compute the element count faster
    /// than via element-by-element counting, should provide a specialised
    /// implementation.
    /// </summary>
    public static int count<T, A>(Foldable<T, A> ta)
        where T : Foldable<T> =>
        T.FoldBack((c, _) => c + 1, 0, ta);

    /// <summary>
    /// Does an element that fits the predicate occur in the structure?
    /// </summary>
    public static bool exists<T, A>(Func<A, bool> predicate, Foldable<T, A> ta)
        where T : Foldable<T> =>
        T.FoldBack((s, c) => s || predicate(c), false, ta);

    /// <summary>
    /// Does the predicate hold for all elements in the structure?
    /// </summary>
    public static bool forAll<T, A>(Func<A, bool> predicate, Foldable<T, A> ta)
        where T : Foldable<T> =>
        T.FoldBack((s, c) => s && predicate(c), true, ta);

    /// <summary>
    /// Does the element exist in the structure?
    /// </summary>
    public static bool contains<T, EqA, A>(A value, Foldable<T, A> ta)
        where T : Foldable<T>
        where EqA : Eq<A> =>
        T.Contains<EqA, A>(value, ta);

    /// <summary>
    /// Does the element exist in the structure?
    /// </summary>
    public static bool contains<T, A>(A value, Foldable<T, A> ta)
        where T : Foldable<T> =>
        T.Contains<EqDefault<A>, A>(value, ta);

    /// <summary>
    /// Computes the sum of the numbers of a structure.
    /// </summary>
    public static A sum<T, NumA, A>(Foldable<T, A> ta) 
        where NumA : Num<A>
        where T : Foldable<T> =>
        T.Sum<NumA, A>(ta);

    /// <summary>
    /// Computes the sum of the numbers of a structure.
    /// </summary>
    public static int sum<T>(Foldable<T, int> ta) 
        where T : Foldable<T> =>
        T.Sum<TInt, int>(ta);

    /// <summary>
    /// Computes the sum of the numbers of a structure.
    /// </summary>
    public static long sum<T>(Foldable<T, long> ta) 
        where T : Foldable<T> =>
        T.Sum<TLong, long>(ta);

    /// <summary>
    /// Computes the sum of the numbers of a structure.
    /// </summary>
    public static float sum<T>(Foldable<T, float> ta) 
        where T : Foldable<T> =>
        T.Sum<TFloat, float>(ta);

    /// <summary>
    /// Computes the sum of the numbers of a structure.
    /// </summary>
    public static double sum<T>(Foldable<T, double> ta) 
        where T : Foldable<T> =>
        T.Sum<TDouble, double>(ta);
}
