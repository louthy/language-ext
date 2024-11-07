using System;
using System.Numerics;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static partial class FoldableExtensions
{
    /// <summary>
    /// Fold until the `Option` returns `None`
    /// </summary>
    /// <param name="f">Fold function</param>
    /// <param name="initialState">Initial state for the fold</param>
    /// <param name="ta">Foldable structure</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <typeparam name="S">State type</typeparam>
    /// <returns>Aggregated value</returns>
    public static S FoldMaybe<T, A, S>(
        this K<T, A> ta,
        S initialState,
        Func<S, Func<A, Option<S>>> f) 
        where T : Foldable<T> =>
        T.FoldMaybe(f, initialState, ta);

    /// <summary>
    /// Fold until the `Option` returns `None`
    /// </summary>
    /// <param name="f">Fold function</param>
    /// <param name="initialState">Initial state for the fold</param>
    /// <param name="ta">Foldable structure</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <typeparam name="S">State type</typeparam>
    /// <returns>Aggregated value</returns>
    public static S FoldMaybe<T, A, S>(
        this K<T, A> ta,
        S initialState,
        Func<S, A, Option<S>> f) 
        where T : Foldable<T> =>
        T.FoldMaybe(s => a => f(s, a), initialState, ta);

    /// <summary>
    /// Fold until the `Option` returns `None`
    /// </summary>
    /// <param name="f">Fold function</param>
    /// <param name="initialState">Initial state for the fold</param>
    /// <param name="ta">Foldable structure</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <typeparam name="S">State type</typeparam>
    /// <returns>Aggregated value</returns>
    public static S FoldBackMaybe<T, A, S>(
        this K<T, A> ta,
        S initialState,
        Func<A, Func<S, Option<S>>> f)
        where T : Foldable<T> =>
        T.FoldBackMaybe(f, initialState, ta);

    /// <summary>
    /// Fold until the `Option` returns `None`
    /// </summary>
    /// <param name="f">Fold function</param>
    /// <param name="initialState">Initial state for the fold</param>
    /// <param name="ta">Foldable structure</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <typeparam name="S">State type</typeparam>
    /// <returns>Aggregated value</returns>
    public static S FoldBackMaybe<T, A, S>(
        this K<T, A> ta,
        S initialState,
        Func<S, A, Option<S>> f)
        where T : Foldable<T> =>
        T.FoldBackMaybe(a => s => f(s, a), initialState, ta);
    
    /// <summary>
    /// Same behaviour as `Fold` but allows early exit of the operation once
    /// the predicate function becomes `false` for the state/value pair 
    /// </summary>
    public static S FoldWhile<T, A, S>(
        this K<T, A> ta,
        S initialState,
        Func<A, Func<S, S>> f, 
        Func<(S State, A Value), bool> predicate)
        where T : Foldable<T> =>
        T.FoldWhile(f, predicate, initialState, ta);
    
    /// <summary>
    /// Same behaviour as `Fold` but allows early exit of the operation once
    /// the predicate function becomes `false` for the state/value pair 
    /// </summary>
    public static S FoldWhile<T, A, S>(
        this K<T, A> ta,
        S initialState,
        Func<S, A, S> f, 
        Func<(S State, A Value), bool> predicate)
        where T : Foldable<T> =>
        T.FoldWhile(a => s => f(s, a), predicate, initialState, ta);

    /// <summary>
    /// Same behaviour as `FoldBack` but allows early exit of the operation once
    /// the predicate function becomes `false` for the state/value pair 
    /// </summary>
    public static S FoldBackWhile<T, A, S>(
        this K<T, A> ta,
        S initialState,
        Func<S, Func<A, S>> f, 
        Func<(S State, A Value), bool> predicate) 
        where T : Foldable<T> =>
        T.FoldBackWhile(f, predicate, initialState, ta);

    /// <summary>
    /// Same behaviour as `FoldBack` but allows early exit of the operation once
    /// the predicate function becomes `false` for the state/value pair 
    /// </summary>
    public static S FoldBackWhile<T, A, S>(
        this K<T, A> ta,
        S initialState,
        Func<S, A, S> f, 
        Func<(S State, A Value), bool> predicate) 
        where T : Foldable<T> =>
        T.FoldBackWhile(curry(f), predicate, initialState, ta);

    /// <summary>
    /// Same behaviour as `Fold` but the fold operation returns a monadic type and allows
    /// early exit of the operation once the predicate function becomes `false` for the
    /// state/value pair 
    /// </summary>
    public static K<M, S> FoldWhileM<T, A, M, S>(
        this K<T, A> ta,
        S initialState,
        Func<A, Func<S, K<M, S>>> f, 
        Func<A, bool> predicate) 
        where T : Foldable<T>
        where M : Monad<M> =>
        T.FoldWhileM(f, predicate, initialState, ta);

    /// <summary>
    /// Same behaviour as `Fold` but the fold operation returns a monadic type and allows
    /// early exit of the operation once the predicate function becomes `false` for the
    /// state/value pair 
    /// </summary>
    public static K<M, S> FoldWhileM<T, A, M, S>(
        this K<T, A> ta,
        S initialState,
        Func<S, A, K<M, S>> f, 
        Func<A, bool> predicate) 
        where T : Foldable<T>
        where M : Monad<M> =>
        T.FoldWhileM<A, M, S>(a => s => f(s, a), predicate, initialState, ta);

    /// <summary>
    /// Same behaviour as `FoldBack` but the fold operation returns a monadic type and allows
    /// early exit of the operation once the predicate function becomes `false` for the
    /// state/value pair 
    /// </summary>
    public static K<M, S> FoldBackWhileM<T, A, M, S>(
        this K<T, A> ta,
        S initialState,
        Func<S, Func<A, K<M, S>>> f, 
        Func<A, bool> predicate)
        where T : Foldable<T> 
        where M : Monad<M> =>
        T.FoldBackWhileM(f, predicate, initialState, ta);

    /// <summary>
    /// Same behaviour as `FoldBack` but the fold operation returns a monadic type and allows
    /// early exit of the operation once the predicate function becomes `false` for the
    /// state/value pair 
    /// </summary>
    public static K<M, S> FoldBackWhileM<T, A, M, S>(
        this K<T, A> ta,
        S initialState,
        Func<S, A, K<M, S>> f, 
        Func<A, bool> predicate)
        where T : Foldable<T> 
        where M : Monad<M> =>
        T.FoldBackWhileM(curry(f), predicate, initialState, ta);

    /// <summary>
    /// Same behaviour as `Fold` but allows early exit of the operation once
    /// the predicate function becomes `false` for the state/value pair
    /// </summary>
    public static S FoldUntil<T, A, S>(
        this K<T, A> ta,
        S initialState,
        Func<A, Func<S, S>> f,
        Func<(S State, A Value), bool> predicate) 
        where T : Foldable<T> =>
        T.FoldUntil(f, predicate, initialState, ta);

    /// <summary>
    /// Same behaviour as `Fold` but allows early exit of the operation once
    /// the predicate function becomes `false` for the state/value pair
    /// </summary>
    public static S FoldUntil<T, A, S>(
        this K<T, A> ta,
        S initialState,
        Func<S, A, S> f,
        Func<(S State, A Value), bool> predicate) 
        where T : Foldable<T> =>
        T.FoldUntil(a => s => f(s, a), predicate, initialState, ta);

    /// <summary>
    /// Same behaviour as `Fold` but the fold operation returns a monadic type and allows
    /// early exit of the operation once the predicate function becomes `false` for the
    /// state/value pair 
    /// </summary>
    public static K<M, S> FoldUntilM<T, A, M, S>(
        this K<T, A> ta,
        S initialState,
        Func<A, Func<S, K<M, S>>> f, 
        Func<A, bool> predicate) 
        where M : Monad<M>
        where T : Foldable<T> => 
        T.FoldUntilM(f, predicate, initialState, ta);

    /// <summary>
    /// Same behaviour as `Fold` but the fold operation returns a monadic type and allows
    /// early exit of the operation once the predicate function becomes `false` for the
    /// state/value pair 
    /// </summary>
    public static K<M, S> FoldUntilM<T, A, M, S>(
        this K<T, A> ta,
        S initialState,
        Func<S, A, K<M, S>> f, 
        Func<A, bool> predicate) 
        where M : Monad<M>
        where T : Foldable<T> => 
        T.FoldUntilM<A, M, S>(a => s => f(s, a), predicate, initialState, ta);

    /// <summary>
    /// Same behaviour as `FoldBack` but allows early exit of the operation once
    /// the predicate function becomes `false` for the state/value pair
    /// </summary>
    public static S FoldBackUntil<T, A, S>(
        this K<T, A> ta,
        S initialState,
        Func<S, Func<A, S>> f, 
        Func<(S State, A Value), bool> predicate) 
        where T : Foldable<T> =>
        T.FoldBackUntil(f, predicate, initialState, ta);

    /// <summary>
    /// Same behaviour as `FoldBack` but allows early exit of the operation once
    /// the predicate function becomes `false` for the state/value pair
    /// </summary>
    public static S FoldBackUntil<T, A, S>(
        this K<T, A> ta,
        S initialState,
        Func<S, A, S> f, 
        Func<(S State, A Value), bool> predicate) 
        where T : Foldable<T> =>
        T.FoldBackUntil(curry(f), predicate, initialState, ta);

    /// <summary>
    /// Same behaviour as `FoldBack` but the fold operation returns a monadic type and allows
    /// early exit of the operation once the predicate function becomes `false` for the
    /// state/value pair 
    /// </summary>
    public static K<M, S> FoldBackUntilM<T, A, M, S>(
        this K<T, A> ta,
        S initialState,
        Func<S, Func<A, K<M, S>>> f, 
        Func<A, bool> predicate)
        where T : Foldable<T> 
        where M : Monad<M> =>
        T.FoldBackUntilM(f, predicate, initialState, ta);

    /// <summary>
    /// Same behaviour as `FoldBack` but the fold operation returns a monadic type and allows
    /// early exit of the operation once the predicate function becomes `false` for the
    /// state/value pair 
    /// </summary>
    public static K<M, S> FoldBackUntilM<T, A, M, S>(
        this K<T, A> ta,
        S initialState,
        Func<S, A, K<M, S>> f, 
        Func<A, bool> predicate)
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
    public static S Fold<T, A, S>(
        this K<T, A> ta,
        S initialState,
        Func<A, Func<S, S>> f) 
        where T : Foldable<T> =>
        T.Fold(f, initialState, ta);

    /// <summary>
    /// Right-associative fold of a structure, lazy in the accumulator.
    ///
    /// In the case of lists, 'Fold', when applied to a binary operator, a
    /// starting value (typically the right-identity of the operator), and a
    /// list, reduces the list using the binary operator, from right to left.
    /// </summary>
    public static S Fold<T, A, S>(this K<T, A> ta, S initialState, Func<S, A, S> f) 
        where T : Foldable<T> =>
        T.Fold(a => s => f(s, a), initialState, ta);

    /// <summary>
    /// Right-associative fold of a structure, lazy in the accumulator.
    ///
    /// In the case of lists, 'Fold', when applied to a binary operator, a
    /// starting value (typically the right-identity of the operator), and a
    /// list, reduces the list using the binary operator, from right to left.
    /// </summary>
    public static K<M, S> FoldM<T, A, M, S>(
        this K<T, A> ta, 
        S initialState,
        Func<A, Func<S, K<M, S>>> f) 
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
    public static K<M, S> FoldM<T, A, M, S>(
        this K<T, A> ta, 
        S initialState,
        Func<S, A, K<M, S>> f) 
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
    public static S FoldBack<T, A, S>(this K<T, A> ta, S initialState, Func<S, Func<A, S>> f) 
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
    public static S FoldBack<T, A, S>(this K<T, A> ta, S initialState, Func<S, A, S> f) 
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
    public static K<M, S> FoldBackM<T, A, M, S>(
        this K<T, A> ta, 
        S initialState,
        Func<S, Func<A, K<M, S>>> f)
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
    public static K<M, S> FoldBackM<T, A, M, S>(
        this K<T, A> ta, 
        S initialState,
        Func<S, A, K<M, S>> f)
        where T : Foldable<T>
        where M : Monad<M> =>
        T.FoldBackM(curry(f), initialState, ta);
    
    /// <summary>
    /// Given a structure with elements whose type is a `Monoid`, combine them
    /// via the monoid's `Append` operator.  This fold is right-associative and
    /// lazy in the accumulator.  When you need a strict left-associative fold,
    /// use 'foldMap'' instead, with 'id' as the map.
    /// </summary>
    public static A Fold<T, A>(this K<T, A> tm) 
        where T : Foldable<T>
        where A : Monoid<A> =>
        T.Fold(tm);

    /// <summary>
    /// Given a structure with elements whose type is a `Monoid`, combine them
    /// via the monoid's `Append` operator.  This fold is right-associative and
    /// lazy in the accumulator.  When you need a strict left-associative fold,
    /// use 'foldMap'' instead, with 'id' as the map.
    /// </summary>
    public static A FoldWhile<T, A>(this K<T, A> tm, Func<(A State, A Value), bool> predicate) 
        where T : Foldable<T>
        where A : Monoid<A> =>
        T.FoldWhile(predicate, tm);

    /// <summary>
    /// Given a structure with elements whose type is a `Monoid`, combine them
    /// via the monoid's `Append` operator.  This fold is right-associative and
    /// lazy in the accumulator.  When you need a strict left-associative fold,
    /// use 'foldMap'' instead, with 'id' as the map.
    /// </summary>
    public static A FoldUntil<T, A>(this K<T, A> tm, Func<(A State, A Value), bool> predicate) 
        where T : Foldable<T>
        where A : Monoid<A> =>
        T.FoldUntil(predicate, tm);

    /// <summary>
    /// Map each element of the structure into a monoid, and combine the
    /// results with `Append`.  This fold is right-associative and lazy in the
    /// accumulator.  For strict left-associative folds consider `FoldMapBack`
    /// instead.
    /// </summary>
    public static B FoldMap<T, A, B>(this K<T, A> ta, Func<A, B> f)
        where T : Foldable<T>
        where B : Monoid<B> =>
        T.FoldMap(f, ta);

    /// <summary>
    /// Map each element of the structure into a monoid, and combine the
    /// results with `Append`.  This fold is right-associative and lazy in the
    /// accumulator.  For strict left-associative folds consider `FoldMapBack`
    /// instead.
    /// </summary>
    public static B FoldMapWhile<T, A, B>(this K<T, A> ta, Func<A, B> f, Func<(B State, A Value), bool> predicate)
        where T : Foldable<T>
        where B : Monoid<B> =>
        T.FoldMapWhile(f, predicate, ta);

    /// <summary>
    /// Map each element of the structure into a monoid, and combine the
    /// results with `Append`.  This fold is right-associative and lazy in the
    /// accumulator.  For strict left-associative folds consider `FoldMapBack`
    /// instead.
    /// </summary>
    public static B FoldMapUntil<T, A, B>(this K<T, A> ta, Func<A, B> f, Func<(B State, A Value), bool> predicate)
        where T : Foldable<T>
        where B : Monoid<B> =>
        T.FoldMapUntil(f, predicate, ta);

    /// <summary>
    /// A left-associative variant of 'FoldMap' that is strict in the
    /// accumulator.  Use this method for strict reduction when partial
    /// results are merged via `Append`.
    /// </summary>
    public static B FoldMapBack<T, A, B>(this K<T, A> ta, Func<A, B> f)
        where T : Foldable<T>
        where B : Monoid<B> =>
        T.FoldMapBack(f, ta);

    /// <summary>
    /// A left-associative variant of 'FoldMap' that is strict in the
    /// accumulator.  Use this method for strict reduction when partial
    /// results are merged via `Append`.
    /// </summary>
    public static B FoldMapBackWhile<T, A, B>(this K<T, A> ta, Func<A, B> f, Func<(B State, A Value), bool> predicate)
        where T : Foldable<T>
        where B : Monoid<B> =>
        T.FoldMapBackWhile(f, predicate, ta);

    /// <summary>
    /// A left-associative variant of 'FoldMap' that is strict in the
    /// accumulator.  Use this method for strict reduction when partial
    /// results are merged via `Append`.
    /// </summary>
    public static B FoldMapBackUntil<T, A, B>(this K<T, A> ta, Func<A, B> f, Func<(B State, A Value), bool> predicate)
        where T : Foldable<T> 
        where B : Monoid<B> =>
        T.FoldMapBackUntil(f, predicate, ta);

    /// <summary>
    /// List of elements of a structure, from left to right
    /// </summary>
    public static Seq<A> ToSeq<T, A>(this K<T, A> ta) 
        where T : Foldable<T> =>
        T.ToSeq(ta);

    /// <summary>
    /// List of elements of a structure, from left to right
    /// </summary>
    public static Lst<A> ToLst<T, A>(this K<T, A> ta) 
        where T : Foldable<T> =>
        T.ToLst(ta);

    /// <summary>
    /// List of elements of a structure, from left to right
    /// </summary>
    public static Arr<A> ToArr<T, A>(this K<T, A> ta)
        where T : Foldable<T> =>
        T.ToArr(ta);

    /// <summary>
    /// List of elements of a structure, from left to right
    /// </summary>
    public static Iterable<A> ToIterable<T, A>(this K<T, A> ta)
        where T : Foldable<T> =>
        T.ToIterable(ta);

    /// <summary>
    /// List of elements of a structure, from left to right
    /// </summary>
    public static bool IsEmpty<T, A>(this K<T, A> ta) 
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
    public static int Count<T, A>(this K<T, A> ta) 
        where T : Foldable<T> =>
        T.Count(ta);

    /// <summary>
    /// Does an element that fits the predicate occur in the structure?
    /// </summary>
    public static bool Exists<T, A>(this K<T, A> ta, Func<A, bool> predicate) 
        where T : Foldable<T> =>
        T.Exists(predicate, ta);

    /// <summary>
    /// Does the predicate hold for all elements in the structure?
    /// </summary>
    public static bool ForAll<T, A>(this K<T, A> ta, Func<A, bool> predicate)
        where T : Foldable<T> =>
        T.ForAll(predicate, ta);

    /// <summary>
    /// Does the element exist in the structure?
    /// </summary>
    public static bool Contains<EqA, T, A>(this K<T, A> ta, A value) 
        where EqA : Eq<A> 
        where T : Foldable<T> =>
        T.Contains<EqA, A>(value, ta);

    /// <summary>
    /// Does the element exist in the structure?
    /// </summary>
    public static bool Contains<T, A>(this K<T, A> ta, A value)
        where T : Foldable<T> =>
         T.Contains(value, ta);

    /// <summary>
    /// Find the first element that match the predicate
    /// </summary>
    public static Option<A> Find<T, A>(this K<T, A> ta, Func<A, bool> predicate)
        where T : Foldable<T> =>
        T.Find(predicate, ta);

    /// <summary>
    /// Find the last element that match the predicate
    /// </summary>
    public static Option<A> FindBack<T, A>(this K<T, A> ta, Func<A, bool> predicate) 
        where T : Foldable<T> =>
        T.FindBack(predicate, ta);

    /// <summary>
    /// Find the elements that match the predicate
    /// </summary>
    public static Seq<A> FindAll<T, A>(this K<T, A> ta, Func<A, bool> predicate) 
        where T : Foldable<T> =>
        T.FindAll(predicate, ta);

    /// <summary>
    /// Find the elements that match the predicate
    /// </summary>
    public static Seq<A> FindAllBack<T, A>(this K<T, A> ta, Func<A, bool> predicate) 
        where T : Foldable<T> =>
        T.FindAllBack(predicate, ta);    
    
    /// <summary>
    /// Computes the sum of the numbers of a structure.
    /// </summary>
    public static A Sum<T, A>(this K<T, A> ta) 
        where T : Foldable<T> 
        where A : IAdditionOperators<A, A, A>, IAdditiveIdentity<A, A> =>
        T.Sum(ta);

    /// <summary>
    /// Computes the product of the numbers of a structure.
    /// </summary>
    public static A Product<T, A>(this K<T, A> ta) 
        where T : Foldable<T> 
        where A : IMultiplyOperators<A, A, A>, IMultiplicativeIdentity<A, A> =>
        T.Product(ta);

    /// <summary>
    /// Get the head item in the foldable or `None`
    /// </summary>
    public static Option<A> Head<T, A>(this K<T, A> ta) 
        where T : Foldable<T> =>
        T.Head(ta);

    /// <summary>
    /// Get the head item in the foldable or `None`
    /// </summary>
    public static Option<A> Last<T, A>(this K<T, A> ta) 
        where T : Foldable<T> =>
        T.Last(ta);

    /// <summary>
    /// Map each element of a structure to an 'Applicative' action, evaluate these
    /// actions from left to right, and ignore the results.  For a version that
    /// doesn't ignore the results see `Traversable.traverse`.
    /// </summary>
    public static K<F, Unit> Iter<T, A, F, B>(this K<T, A> ta, Func<A, K<F, B>> f)
        where T : Foldable<T>
        where F : Applicative<F> =>
        T.Iter(f, ta);

    /// <summary>
    /// Map each element of a structure to an action, evaluate these
    /// actions from left to right, and ignore the results.  For a version that
    /// doesn't ignore the results see `Traversable.traverse`.
    /// </summary>
    public static Unit Iter<T, A>(this K<T, A> ta, Action<A> f)
        where T : Foldable<T> =>
        T.Iter(f, ta);
    
    /// <summary>
    /// Map each element of a structure to an action, evaluate these
    /// actions from left to right, and ignore the results.  For a version that
    /// doesn't ignore the results see `Traversable.traverse`.
    /// </summary>
    public static Unit Iter<T, A>(this K<T, A> ta, Action<int, A> f) 
        where T : Foldable<T> =>
        T.Iter(f, ta);
        
    /// <summary>
    /// Find the minimum value in the structure
    /// </summary>
    public static Option<A> Min<OrdA, T, A>(this K<T, A> ta)
        where T : Foldable<T>
        where OrdA : Ord<A> =>
        T.Min<OrdA, A>(ta);

    /// <summary>
    /// Find the minimum value in the structure
    /// </summary>
    public static Option<A> Min<T, A>(this K<T, A> ta)
        where T : Foldable<T>
        where A : IComparable<A> =>
        T.Min(ta);

    /// <summary>
    /// Find the maximum value in the structure
    /// </summary>
    public static Option<A> Max<OrdA, T, A>(this K<T, A> ta)
        where T : Foldable<T>
        where OrdA : Ord<A> =>
        T.Max<OrdA, A>(ta);

    /// <summary>
    /// Find the maximum value in the structure
    /// </summary>
    public static Option<A> Max<T, A>(this K<T, A> ta)
        where T : Foldable<T>
        where A : IComparable<A> =>
        T.Max(ta);
    
    /// <summary>
    /// Find the minimum value in the structure
    /// </summary>
    public static A Min<OrdA, T, A>(this K<T, A> ta, A initialMin)
        where T : Foldable<T>
        where OrdA : Ord<A> =>
        T.Min<OrdA, A>(ta, initialMin);

    /// <summary>
    /// Find the minimum value in the structure
    /// </summary>
    public static A Min<T, A>(this K<T, A> ta, A initialMin)
        where T : Foldable<T>
        where A : IComparable<A> =>
        T.Min(ta, initialMin);

    /// <summary>
    /// Find the maximum value in the structure
    /// </summary>
    public static A Max<OrdA, T, A>(this K<T, A> ta, A initialMax)
        where T : Foldable<T>
        where OrdA : Ord<A> =>
        T.Max<OrdA, A>(ta, initialMax);

    /// <summary>
    /// Find the maximum value in the structure
    /// </summary>
    public static A Max<T, A>(this K<T, A> ta, A initialMax)
        where T : Foldable<T>
        where A : IComparable<A> =>
        T.Max(ta, initialMax);    

    /// <summary>
    /// Find the average of all the values in the structure
    /// </summary>
    public static A Average<T, A>(this K<T, A> ta)
        where T : Foldable<T>
        where A : INumber<A> =>
        T.Average(ta);

    /// <summary>
    /// Find the average of all the values in the structure
    /// </summary>
    public static B Average<T, A, B>(this K<T, A> ta, Func<A, B> f)
        where T : Foldable<T>
        where B : INumber<B> =>
        T.Average(f, ta);

    /// <summary>
    /// Find the element at the specified index or `None` if out of range
    /// </summary>
    public static Option<A> At<T, A>(this K<T, A> ta, Index index)
        where T : Foldable<T> =>
        T.At(ta, index);

    /// <summary>
    /// Partition a foldable into two sequences based on a predicate
    /// </summary>
    /// <param name="f">Predicate function</param>
    /// <param name="ta">Foldable structure</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Partitioned structure</returns>
    public static (Seq<A> True, Seq<A> False) Partition<T, A>(this K<T, A> ta, Func<A, bool> f)
        where T : Foldable<T> =>
        T.Partition(f, ta);
}
