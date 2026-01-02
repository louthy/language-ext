using System;
using System.Numerics;
using System.Threading.Tasks;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

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
    /// Partition a foldable into two sequences based on a predicate
    /// </summary>
    /// <param name="f">Predicate function</param>
    /// <param name="ta">Foldable structure</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Partitioned structure</returns>
    public static (Seq<A> True, Seq<A> False) partition<T, A>(Func<A, bool> f, K<T, A> ta)
        where T : Foldable<T> =>
        ta.Partition(f);
}
