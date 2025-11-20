using System;
using System.Numerics;
using System.Threading.Tasks;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static partial class Prelude
{
    /// <summary>
    /// Same behaviour as `Fold` but allows early exit of the operation once
    /// the predicate function becomes `false` for the state/value pair 
    /// </summary>
    public static S foldWhile<T, A, S>(
        Func<A, Func<S, S>> f, 
        Func<(S State, A Value), bool> predicate, 
        S initialState,
        K<T, A> ta)
        where T : Foldable<T> =>
        T.FoldWhile(f, predicate, initialState, ta);
    
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
        T.FoldWhile(a => s => f(s, a), predicate, initialState, ta);

    /// <summary>
    /// Same behaviour as `FoldBack` but allows early exit of the operation once
    /// the predicate function becomes `false` for the state/value pair 
    /// </summary>
    public static S foldBackWhile<T, A, S>(
        Func<S, Func<A, S>> f, 
        Func<(S State, A Value), bool> predicate, 
        S initialState, 
        K<T, A> ta) 
        where T : Foldable<T> =>
        T.FoldBackWhile(f, predicate, initialState, ta);

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
        T.FoldBackWhile(curry(f), predicate, initialState, ta);

    /// <summary>
    /// Same behaviour as `Fold` but the fold operation returns a monadic type and allows
    /// early exit of the operation once the predicate function becomes `false` for the
    /// state/value pair 
    /// </summary>
    public static K<M, S> foldWhileM<T, A, M, S>(
        Func<A, Func<S, K<M, S>>> f, 
        Func<A, bool> predicate, 
        S initialState, 
        K<T, A> ta) 
        where T : Foldable<T>
        where M : Monad<M> =>
        T.FoldWhileM(f, predicate, initialState, ta);

    /// <summary>
    /// Same behaviour as `Fold` but the fold operation returns a monadic type and allows
    /// early exit of the operation once the predicate function becomes `false` for the
    /// state/value pair 
    /// </summary>
    public static K<M, S> foldWhileM<T, A, M, S>(
        Func<S, A, K<M, S>> f, 
        Func<A, bool> predicate, 
        S initialState, 
        K<T, A> ta) 
        where T : Foldable<T>
        where M : Monad<M> =>
        T.FoldWhileM<A, M, S>(a => s => f(s, a), predicate, initialState, ta);

    /// <summary>
    /// Same behaviour as `FoldBack` but the fold operation returns a monadic type and allows
    /// early exit of the operation once the predicate function becomes `false` for the
    /// state/value pair 
    /// </summary>
    public static K<M, S> foldBackWhileM<T, A, M, S>(
        Func<S, Func<A, K<M, S>>> f, 
        Func<A, bool> predicate, 
        S initialState, 
        K<T, A> ta)
        where T : Foldable<T> 
        where M : Monad<M> =>
        T.FoldBackWhileM(f, predicate, initialState, ta);

    /// <summary>
    /// Same behaviour as `FoldBack` but the fold operation returns a monadic type and allows
    /// early exit of the operation once the predicate function becomes `false` for the
    /// state/value pair 
    /// </summary>
    public static K<M, S> foldBackWhileM<T, A, M, S>(
        Func<S, A, K<M, S>> f, 
        Func<A, bool> predicate, 
        S initialState, 
        K<T, A> ta)
        where T : Foldable<T> 
        where M : Monad<M> =>
        T.FoldBackWhileM(curry(f), predicate, initialState, ta);

    /// <summary>
    /// Same behaviour as `Fold` but allows early exit of the operation once
    /// the predicate function becomes `false` for the state/value pair
    /// </summary>
    public static S foldUntil<T, A, S>(
        Func<A, Func<S, S>> f,
        Func<(S State, A Value), bool> predicate,
        S initialState, 
        K<T, A> ta) 
        where T : Foldable<T> =>
        T.FoldUntil(f, predicate, initialState, ta);

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
        T.FoldUntil(a => s => f(s, a), predicate, initialState, ta);

    /// <summary>
    /// Same behaviour as `Fold` but the fold operation returns a monadic type and allows
    /// early exit of the operation once the predicate function becomes `false` for the
    /// state/value pair 
    /// </summary>
    public static K<M, S> foldUntilM<T, A, M, S>(
        Func<A, Func<S, K<M, S>>> f, 
        Func<A, bool> predicate, 
        S initialState, 
        K<T, A> ta) 
        where M : Monad<M>
        where T : Foldable<T> => 
        T.FoldUntilM(f, predicate, initialState, ta);

    /// <summary>
    /// Same behaviour as `Fold` but the fold operation returns a monadic type and allows
    /// early exit of the operation once the predicate function becomes `false` for the
    /// state/value pair 
    /// </summary>
    public static K<M, S> foldUntilM<T, A, M, S>(
        Func<S, A, K<M, S>> f, 
        Func<A, bool> predicate, 
        S initialState, 
        K<T, A> ta) 
        where M : Monad<M>
        where T : Foldable<T> => 
        T.FoldUntilM<A, M, S>(a => s => f(s, a), predicate, initialState, ta);

    /// <summary>
    /// Same behaviour as `FoldBack` but allows early exit of the operation once
    /// the predicate function becomes `false` for the state/value pair
    /// </summary>
    public static S foldBackUntil<T, A, S>(
        Func<S, Func<A, S>> f, 
        Func<(S State, A Value), bool> predicate, 
        S initialState, 
        K<T, A> ta) 
        where T : Foldable<T> =>
        T.FoldBackUntil(f, predicate, initialState, ta);

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
        T.FoldBackUntil(curry(f), predicate, initialState, ta);

    /// <summary>
    /// Same behaviour as `FoldBack` but the fold operation returns a monadic type and allows
    /// early exit of the operation once the predicate function becomes `false` for the
    /// state/value pair 
    /// </summary>
    public static K<M, S> foldBackUntilM<T, A, M, S>(
        Func<S, Func<A, K<M, S>>> f, 
        Func<A, bool> predicate, 
        S initialState, 
        K<T, A> ta)
        where T : Foldable<T> 
        where M : Monad<M> =>
        T.FoldBackUntilM(f, predicate, initialState, ta);

    /// <summary>
    /// Same behaviour as `FoldBack` but the fold operation returns a monadic type and allows
    /// early exit of the operation once the predicate function becomes `false` for the
    /// state/value pair 
    /// </summary>
    public static K<M, S> foldBackUntilM<T, A, M, S>(
        Func<S, A, K<M, S>> f, 
        Func<A, bool> predicate, 
        S initialState, 
        K<T, A> ta)
        where T : Foldable<T> 
        where M : Monad<M> =>
        T.FoldBackUntilM(curry(f), predicate, initialState, ta);

    /// <summary>
    /// Right-associative fold of a structure, lazy in the accumulator.
    ///
    /// In the case of lists, 'Fold', when applied to a binary operator, a
    /// starting value (typically the right-identity of the operator), and a
    /// list, reduces the list using the binary operator, from right to left.
    /// </summary>
    public static S fold<T, A, S>(Func<A, Func<S, S>> f, S initialState, K<T, A> ta) 
        where T : Foldable<T> =>
        T.Fold(f, initialState, ta);

    /// <summary>
    /// Right-associative fold of a structure, lazy in the accumulator.
    ///
    /// In the case of lists, 'Fold', when applied to a binary operator, a
    /// starting value (typically the right-identity of the operator), and a
    /// list, reduces the list using the binary operator, from right to left.
    /// </summary>
    public static S fold<T, A, S>(Func<S, A, S> f, S initialState, K<T, A> ta) 
        where T : Foldable<T> =>
        T.Fold(a => s => f(s, a), initialState, ta);

    /// <summary>
    /// Right-associative fold of a structure, lazy in the accumulator.
    ///
    /// In the case of lists, 'Fold', when applied to a binary operator, a
    /// starting value (typically the right-identity of the operator), and a
    /// list, reduces the list using the binary operator, from right to left.
    /// </summary>
    public static K<M, S> foldM<T, A, M, S>(
        Func<A, Func<S, K<M, S>>> f, 
        S initialState, 
        K<T, A> ta) 
        where T : Foldable<T>
        where M : Monad<M> =>
        T.FoldM(f, initialState, ta);

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
        T.FoldM<A, M, S>(a => s => f(s, a), initialState, ta);
    
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
    public static S foldBack<T, A, S>(Func<S, Func<A, S>> f, S initialState, K<T, A> ta) 
        where T : Foldable<T> =>
        T.FoldBack(f, initialState, ta);
    
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
        T.FoldBack(curry(f), initialState, ta);

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
        Func<S, Func<A, K<M, S>>> f, 
        S initialState, 
        K<T, A> ta)
        where T : Foldable<T>
        where M : Monad<M> =>
        T.FoldBackM(f, initialState, ta);

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
        T.FoldBackM(curry(f), initialState, ta);
    
    /// <summary>
    /// Given a structure with elements whose type is a `Monoid`, combine them
    /// via the monoid's `Append` operator.  This fold is right-associative and
    /// lazy in the accumulator.  When you need a strict left-associative fold,
    /// use 'foldMap'' instead, with 'id' as the map.
    /// </summary>
    public static A fold<T, A>(K<T, A> tm) 
        where T : Foldable<T>
        where A : Monoid<A> =>
        T.FoldMap(identity, tm) ;

    /// <summary>
    /// Given a structure with elements whose type is a `Monoid`, combine them
    /// via the monoid's `Append` operator.  This fold is right-associative and
    /// lazy in the accumulator.  When you need a strict left-associative fold,
    /// use 'foldMap'' instead, with 'id' as the map.
    /// </summary>
    public static A foldWhile<T, A>(Func<(A State, A Value), bool> predicate, K<T, A> tm) 
        where T : Foldable<T>
        where A : Monoid<A> =>
        T.FoldMapWhile(identity, predicate, tm) ;

    /// <summary>
    /// Given a structure with elements whose type is a `Monoid`, combine them
    /// via the monoid's `Append` operator.  This fold is right-associative and
    /// lazy in the accumulator.  When you need a strict left-associative fold,
    /// use 'foldMap'' instead, with 'id' as the map.
    /// </summary>
    public static A foldUntil<T, A>(Func<(A State, A Value), bool> predicate, K<T, A> tm) 
        where T : Foldable<T>
        where A : Monoid<A> =>
        T.FoldMapUntil(identity, predicate, tm) ;

    /// <summary>
    /// Map each element of the structure into a monoid, and combine the
    /// results with `Append`.  This fold is right-associative and lazy in the
    /// accumulator.  For strict left-associative folds consider `FoldMapBack`
    /// instead.
    /// </summary>
    public static B foldMap<T, A, B>(Func<A, B> f, K<T, A> ta)
        where T : Foldable<T>
        where B : Monoid<B> =>
        T.FoldMap(f, ta);

    /// <summary>
    /// Map each element of the structure into a monoid, and combine the
    /// results with `Append`.  This fold is right-associative and lazy in the
    /// accumulator.  For strict left-associative folds consider `FoldMapBack`
    /// instead.
    /// </summary>
    public static B foldMapWhile<T, A, B>(Func<A, B> f, Func<(B State, A Value), bool> predicate, K<T, A> ta)
        where T : Foldable<T>
        where B : Monoid<B> =>
        T.FoldMapWhile(f, predicate, ta);

    /// <summary>
    /// Map each element of the structure into a monoid, and combine the
    /// results with `Append`.  This fold is right-associative and lazy in the
    /// accumulator.  For strict left-associative folds consider `FoldMapBack`
    /// instead.
    /// </summary>
    public static B foldMapUntil<T, A, B>(Func<A, B> f, Func<(B State, A Value), bool> predicate, K<T, A> ta)
        where T : Foldable<T>
        where B : Monoid<B> =>
        T.FoldMapUntil(f, predicate, ta);

    /// <summary>
    /// A left-associative variant of 'FoldMap' that is strict in the
    /// accumulator.  Use this method for strict reduction when partial
    /// results are merged via `Append`.
    /// </summary>
    public static B foldMapBack<T, A, B>(Func<A, B> f, K<T, A> ta)
        where T : Foldable<T>
        where B : Monoid<B> =>
        T.FoldMapBack(f, ta);

    /// <summary>
    /// A left-associative variant of 'FoldMap' that is strict in the
    /// accumulator.  Use this method for strict reduction when partial
    /// results are merged via `Append`.
    /// </summary>
    public static B foldMapBackWhile<T, A, B>(Func<A, B> f, Func<(B State, A Value), bool> predicate, K<T, A> ta)
        where T : Foldable<T>
        where B : Monoid<B> =>
        T.FoldMapBackWhile(f, predicate, ta);

    /// <summary>
    /// A left-associative variant of 'FoldMap' that is strict in the
    /// accumulator.  Use this method for strict reduction when partial
    /// results are merged via `Append`.
    /// </summary>
    public static B foldMapBackUntil<T, A, B>(Func<A, B> f, Func<(B State, A Value), bool> predicate, K<T, A> ta)
        where T : Foldable<T> 
        where B : Monoid<B> =>
        T.FoldMapBackUntil(f, predicate, ta);

    /// <summary>
    /// Map each element of a structure to an 'Applicative' action, evaluate these
    /// actions from left to right, and ignore the results.  For a version that
    /// doesn't ignore the results see `Traversable.traverse`.
    /// </summary>
    public static K<F, Unit> iter<T, A, F, B>(Func<A, K<F, B>> f, K<T, A> ta)
        where T : Foldable<T>
        where F : Applicative<F> =>
        T.Iter(f, ta);
    
    /// <summary>
    /// Map each element of a structure to an action, evaluate these
    /// actions from left to right, and ignore the results.  For a version that
    /// doesn't ignore the results see `Traversable.traverse`.
    /// </summary>
    public static Unit iter<T, A>(Action<int, A> f, K<T, A> ta) 
        where T : Foldable<T> =>
        T.Iter(f, ta);
    
    /// <summary>
    /// Map each element of a structure to an action, evaluate these
    /// actions from left to right, and ignore the results.  For a version that
    /// doesn't ignore the results see `Traversable.traverse`.
    /// </summary>
    public static Unit iter<T, A>(Action<A> f, K<T, A> ta)
        where T : Foldable<T> =>
        T.Iter(f, ta);

    /// <summary>
    /// Partition a foldable into two sequences based on a predicate
    /// </summary>
    /// <param name="f">Predicate function</param>
    /// <param name="ta">Foldable structure</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Partitioned structure</returns>
    public static (Seq<A> True, Seq<A> False) partition<T, A>(Func<A, bool> f, K<T, A> ta)
        where T : Foldable<T> =>
        T.Partition(f, ta);
}
