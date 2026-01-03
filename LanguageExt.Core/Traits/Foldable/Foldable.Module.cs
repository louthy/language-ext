using System;
using System.Numerics;

namespace LanguageExt.Traits;

public static class Foldable
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
        where T : Foldable<T> =>
        ta.ForM(f);
    
    /// <summary>
    /// Same behaviour as `Fold` but allows early exit of the operation once
    /// the predicate function becomes `false` for the state/value pair 
    /// </summary>
    public static S foldWhile<T, A, S>(
        Func<S, A, S> f, 
        Func<(S State, A Value), bool> predicate, 
        S initialState,
        K<T, A> ta)
        where T : Foldable<T> =>
        ta.FoldWhile(f, predicate, initialState);

    /// <summary>
    /// Same behaviour as `FoldBack` but allows early exit of the operation once
    /// the predicate function becomes `false` for the state/value pair 
    /// </summary>
    public static S foldBackWhile<T, A, S>(
        Func<S, A, S> f, 
        Func<(S State, A Value), bool> predicate, 
        S initialState, 
        K<T, A> ta) 
        where T : Foldable<T> =>
        ta.FoldBackWhile(f, predicate, initialState);

    /// <summary>
    /// Same behaviour as `Fold` but the fold operation returns a monadic type and allows
    /// early exit of the operation once the predicate function becomes `false` for the
    /// state/value pair 
    /// </summary>
    public static K<M, S> foldWhileM<T, A, M, S>(
        Func<S, A, K<M, S>> f, 
        Func<(S State, A Value), bool> predicate, 
        S initialState, 
        K<T, A> ta) 
        where T : Foldable<T>
        where M : Monad<M> =>
        ta.FoldWhileM(f, predicate, initialState);

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
        where T : Foldable<T> 
        where M : Monad<M> =>
        ta.FoldBackWhileM(f, predicate, initialState);

    /// <summary>
    /// Same behaviour as `Fold` but allows early exit of the operation once
    /// the predicate function becomes `false` for the state/value pair
    /// </summary>
    public static S foldUntil<T, A, S>(
        Func<S, A, S> f,
        Func<(S State, A Value), bool> predicate,
        S initialState, 
        K<T, A> ta) 
        where T : Foldable<T> =>
        ta.FoldUntil(f, predicate, initialState);

    /// <summary>
    /// Same behaviour as `Fold` but the fold operation returns a monadic type and allows
    /// early exit of the operation once the predicate function becomes `false` for the
    /// state/value pair 
    /// </summary>
    public static K<M, S> foldUntilM<T, A, M, S>(
        Func<S, A, K<M, S>> f, 
        Func<(S State, A Value), bool> predicate, 
        S initialState, 
        K<T, A> ta) 
        where M : Monad<M>
        where T : Foldable<T> => 
        ta.FoldUntilM(f, predicate, initialState);

    /// <summary>
    /// Same behaviour as `FoldBack` but allows early exit of the operation once
    /// the predicate function becomes `false` for the state/value pair
    /// </summary>
    public static S foldBackUntil<T, A, S>(
        Func<S, A, S> f, 
        Func<(S State, A Value), bool> predicate, 
        S initialState, 
        K<T, A> ta) 
        where T : Foldable<T> =>
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
        where T : Foldable<T> 
        where M : Monad<M> =>
        ta.FoldBackUntilM(f, predicate, initialState);

    /// <summary>
    /// Right-associative fold of a structure, lazy in the accumulator.
    ///
    /// In the case of lists, 'Fold', when applied to a binary operator, a
    /// starting value (typically the right-identity of the operator), and a
    /// list, reduces the list using the binary operator, from right to left.
    /// </summary>
    public static S fold<T, A, S>(Func<S, A, S> f, S initialState, K<T, A> ta) 
        where T : Foldable<T> =>
        ta.Fold(f, initialState);

    /// <summary>
    /// Right-associative fold of a structure, lazy in the accumulator.
    ///
    /// In the case of lists, 'Fold', when applied to a binary operator, a
    /// starting value (typically the right-identity of the operator), and a
    /// list, reduces the list using the binary operator, from right to left.
    /// </summary>
    public static K<M, S> foldM<T, A, M, S>(
        Func<S, A, K<M, S>> f, 
        S initialState, 
        K<T, A> ta) 
        where T : Foldable<T>
        where M : Monad<M> =>
        ta.FoldM(f, initialState);
    
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
        where T : Foldable<T> =>
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
        where T : Foldable<T>
        where M : Monad<M> =>
        ta.FoldBackM(f, initialState);

    /// <summary>
    /// Given a structure with elements whose type is a `Monoid`, combine them
    /// via the monoid's `Append` operator.  This fold is right-associative and
    /// lazy in the accumulator.  When you need a strict left-associative fold,
    /// use 'foldMap'' instead, with 'id' as the map.
    /// </summary>
    public static A fold<T, A>(K<T, A> ta)
        where T : Foldable<T>
        where A : Monoid<A> =>
        ta.Fold();

    /// <summary>
    /// Given a structure with elements whose type is a `Monoid`, combine them
    /// via the monoid's `Append` operator.  This fold is right-associative and
    /// lazy in the accumulator.  When you need a strict left-associative fold,
    /// use 'foldMap'' instead, with 'id' as the map.
    /// </summary>
    public static A foldWhile<T, A>(Func<(A State, A Value), bool> predicate, K<T, A> ta) 
        where T : Foldable<T>
        where A : Monoid<A> =>
        ta.FoldWhile(predicate) ;

    /// <summary>
    /// Given a structure with elements whose type is a `Monoid`, combine them
    /// via the monoid's `Append` operator.  This fold is right-associative and
    /// lazy in the accumulator.  When you need a strict left-associative fold,
    /// use 'foldMap'' instead, with 'id' as the map.
    /// </summary>
    public static A foldUntil<T, A>(Func<(A State, A Value), bool> predicate, K<T, A> ta) 
        where T : Foldable<T>
        where A : Monoid<A> =>
        ta.FoldUntil(predicate) ;

    /// <summary>
    /// List of elements of a structure, from left to right
    /// </summary>
    public static Seq<A> toSeq<T, A>(K<T, A> ta) 
        where T : Foldable<T> =>
        ta.ToSeq();

    /// <summary>
    /// List of elements of a structure, from left to right
    /// </summary>
    public static Lst<A> toLst<T, A>(K<T, A> ta) 
        where T : Foldable<T> =>
        ta.ToLst();

    /// <summary>
    /// List of elements of a structure, from left to right
    /// </summary>
    public static Arr<A> toArr<T, A>(K<T, A> ta)
        where T : Foldable<T> =>
        ta.ToArr();

    /// <summary>
    /// List of elements of a structure, from left to right
    /// </summary>
    public static Iterable<A> toIterable<T, A>(K<T, A> ta) 
        where T : Foldable<T> =>
        ta.ToIterable();

    /// <summary>
    /// List of elements of a structure, from left to right
    /// </summary>
    public static bool isEmpty<T, A>(K<T, A> ta)
        where T : Foldable<T> =>
        ta.IsEmpty;

    /// <summary>
    /// Returns the size/length of a finite structure as an `int`.  The
    /// default implementation just counts elements starting with the leftmost.
    /// 
    /// Instances for structures that can compute the element count faster
    /// than via element-by-element counting, should provide a specialised
    /// implementation.
    /// </summary>
    public static int count<T, A>(K<T, A> ta) 
        where T : Foldable<T> =>
        ta.Count;

    /// <summary>
    /// Does an element that fits the predicate occur in the structure?
    /// </summary>
    public static bool exists<T, A>(Func<A, bool> predicate, K<T, A> ta) 
        where T : Foldable<T> =>
        ta.Exists(predicate);

    /// <summary>
    /// Does the predicate hold for all elements in the structure?
    /// </summary>
    public static bool forAll<T, A>(Func<A, bool> predicate, K<T, A> ta) 
        where T : Foldable<T> =>
        ta.ForAll(predicate);

    /// <summary>
    /// Does the element exist in the structure?
    /// </summary>
    public static bool contains<EqA, T, A>(A value, K<T, A> ta) 
        where EqA : Eq<A> 
        where T : Foldable<T> =>
        T.Contains<EqA, A>(value, ta);

    /// <summary>
    /// Does the element exist in the structure?
    /// </summary>
    public static bool contains<T, A>(A value, K<T, A> ta)
        where T : Foldable<T> =>
        ta.Contains(value);

    /// <summary>
    /// Find the first element that match the predicate
    /// </summary>
    public static Option<A> find<T, A>(Func<A, bool> predicate, K<T, A> ta)
        where T : Foldable<T> =>
        ta.Find(predicate);

    /// <summary>
    /// Find the last element that match the predicate
    /// </summary>
    public static Option<A> findBack<T, A>(Func<A, bool> predicate, K<T, A> ta) 
        where T : Foldable<T> =>
        ta.FindBack(predicate);

    /// <summary>
    /// Find the elements that match the predicate
    /// </summary>
    public static Iterable<A> findAll<T, A>(Func<A, bool> predicate, K<T, A> ta) 
        where T : Foldable<T> =>
        ta.FindAll(predicate);

    /// <summary>
    /// Find the elements that match the predicate
    /// </summary>
    public static Iterable<A> findAllBack<T, A>(Func<A, bool> predicate, K<T, A> ta) 
        where T : Foldable<T> =>
        ta.FindAllBack(predicate);

    /// <summary>
    /// Computes the sum of the numbers of a structure.
    /// </summary>
    public static A sum<T, A>(K<T, A> ta) 
        where T : Foldable<T> 
        where A : IAdditionOperators<A, A, A>, IAdditiveIdentity<A, A> =>
        ta.Sum();

    /// <summary>
    /// Computes the product of the numbers of a structure.
    /// </summary>
    public static A product<T, A>(K<T, A> ta) 
        where T : Foldable<T> 
        where A : IMultiplyOperators<A, A, A>, IMultiplicativeIdentity<A, A> =>
        ta.Product();

    /// <summary>
    /// Get the head item in the foldable or `None`
    /// </summary>
    public static Option<A> head<T, A>(K<T, A> ta) 
        where T : Foldable<T> =>
        ta.Head;

    /// <summary>
    /// Get the head item in the foldable or `None`
    /// </summary>
    public static Option<A> last<T, A>(K<T, A> ta) 
        where T : Foldable<T> =>
        ta.Last;

    /// <summary>
    /// Map each element of a structure to an 'Applicative' action, evaluate these
    /// actions from left to right, and ignore the results.  For a version that
    /// doesn't ignore the results see `Traversable.traverse`.
    /// </summary>
    public static K<F, Unit> iterM<T, A, F, B>(Func<A, K<F, B>> f, K<T, A> ta)
        where T : Foldable<T>
        where F : Monad<F> =>
        ta.IterM(f);
    
    /// <summary>
    /// Map each element of a structure to an action, evaluate these
    /// actions from left to right, and ignore the results.  For a version that
    /// doesn't ignore the results see `Traversable.traverse`.
    /// </summary>
    public static Unit iter<T, A>(Action<int, A> f, K<T, A> ta) 
        where T : Foldable<T> =>
        ta.Iter(f);
    
    /// <summary>
    /// Map each element of a structure to an action, evaluate these
    /// actions from left to right, and ignore the results.  For a version that
    /// doesn't ignore the results see `Traversable.traverse`.
    /// </summary>
    public static Unit iter<T, A>(Action<A> f, K<T, A> ta)
        where T : Foldable<T> =>
        ta.Iter(f);
    
    /// <summary>
    /// Find the minimum value in the structure
    /// </summary>
    public static Option<A> min<OrdA, T, A>(K<T, A> ta)
        where T : Foldable<T>
        where OrdA : Ord<A> =>
        T.Min<OrdA, A>(ta);

    /// <summary>
    /// Find the minimum value in the structure
    /// </summary>
    public static Option<A> min<T, A>(K<T, A> ta)
        where T : Foldable<T>
        where A : IComparable<A> =>
        ta.Min();

    /// <summary>
    /// Find the maximum value in the structure
    /// </summary>
    public static Option<A> max<OrdA, T, A>(K<T, A> ta)
        where T : Foldable<T>
        where OrdA : Ord<A> =>
        T.Max<OrdA, A>(ta);

    /// <summary>
    /// Find the maximum value in the structure
    /// </summary>
    public static Option<A> max<T, A>(K<T, A> ta)
        where T : Foldable<T> =>
        ta.Max();
    
    /// <summary>
    /// Find the minimum value in the structure
    /// </summary>
    public static A min<OrdA, T, A>(K<T, A> ta, A initialMin)
        where T : Foldable<T>
        where OrdA : Ord<A> =>
        T.Min<OrdA, A>(initialMin, ta);

    /// <summary>
    /// Find the minimum value in the structure
    /// </summary>
    public static A min<T, A>(K<T, A> ta, A initialMin)
        where T : Foldable<T> =>
        ta.Min(initialMin);

    /// <summary>
    /// Find the maximum value in the structure
    /// </summary>
    public static A max<OrdA, T, A>(K<T, A> ta, A initialMax)
        where T : Foldable<T>
        where OrdA : Ord<A> =>
        T.Max<OrdA, A>(initialMax, ta);

    /// <summary>
    /// Find the maximum value in the structure
    /// </summary>
    public static A max<T, A>(K<T, A> ta, A initialMax)
        where T : Foldable<T> =>
        ta.Max(initialMax);

    /// <summary>
    /// Find the average of all the values in the structure
    /// </summary>
    public static A average<T, A>(K<T, A> ta)
        where T : Foldable<T>
        where A : INumber<A> =>
        ta.Average();

    /// <summary>
    /// Find the element at the specified index or `None` if out of range
    /// </summary>
    public static Option<A> at<T, A>(K<T, A> ta, Index index)
        where T : Foldable<T> =>
        ta.At(index);

    /// <summary>
    /// Partition a foldable into two sequences based on a predicate
    /// </summary>
    /// <param name="f">Predicate function</param>
    /// <param name="ta">Foldable structure</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Partitioned structure</returns>
    public static (Seq<A> True, Seq<A> False) partition<T, A>(Func<A, bool> f, K<T, A> ta)
        where T : Foldable<T> =>
        ta.Partition(f);

    /// <summary>
    /// Low-level interface for folding using stack-based primitives.
    /// </summary>
    public static void stepSetup<T, FS, A>(K<T, A> ta, ref FS refState)
        where T : Foldable<T, FS> 
        where FS : allows ref struct =>
        T.FoldStepSetup(ta, ref refState);

    /// <summary>
    /// Low-level interface for folding using stack-based primitives.
    /// </summary>
    public static bool step<T, FS, A>(K<T, A> ta, ref FS refState, out A value)
        where T : Foldable<T, FS> 
        where FS : allows ref struct =>
        T.FoldStep(ta, ref refState, out value);

    /// <summary>
    /// Low-level interface for folding using stack-based primitives.
    /// </summary>
    public static void stepBackSetup<T, FS, A>(K<T, A> ta, ref FS refState)
        where T : Foldable<T, FS> 
        where FS : allows ref struct =>
        T.FoldStepBackSetup(ta, ref refState);

    /// <summary>
    /// Low-level interface for folding using stack-based primitives.
    /// </summary>
    public static bool stepBack<T, FS, A>(K<T, A> ta, ref FS refState, out A value)
        where T : Foldable<T, FS> 
        where FS : allows ref struct =>
        T.FoldStepBack(ta, ref refState, out value);
}
