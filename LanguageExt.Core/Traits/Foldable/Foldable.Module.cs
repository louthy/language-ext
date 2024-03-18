using System;
using System.Numerics;
using static LanguageExt.Prelude;

namespace LanguageExt.Traits;

public static class Foldable
{
    /// <summary>
    /// Same behaviour as `Fold` but allows early exit of the operation once
    /// the predicate function becomes `false` for the state/value pair 
    /// </remarks>
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
    /// </remarks>
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
    /// </remarks>
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
    /// </remarks>
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
    /// </remarks>
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
    /// </remarks>
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
    /// </remarks>
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
    /// </remarks>
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
    /// </remarks>
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
    /// </remarks>
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
    /// </remarks>
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
    /// </remarks>
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
    /// </remarks>
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
    /// </remarks>
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
    /// </remarks>
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
    /// </remarks>
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
        T.Fold(x => a => f(x).Combine(a), B.Empty, ta);

    /// <summary>
    /// Map each element of the structure into a monoid, and combine the
    /// results with `Append`.  This fold is right-associative and lazy in the
    /// accumulator.  For strict left-associative folds consider `FoldMapBack`
    /// instead.
    /// </summary>
    public static B foldMapWhile<T, A, B>(Func<A, B> f, Func<(B State, A Value), bool> predicate, K<T, A> ta)
        where T : Foldable<T>
        where B : Monoid<B> =>
        T.FoldWhile(x => a => f(x).Combine(a), predicate, B.Empty, ta);

    /// <summary>
    /// Map each element of the structure into a monoid, and combine the
    /// results with `Append`.  This fold is right-associative and lazy in the
    /// accumulator.  For strict left-associative folds consider `FoldMapBack`
    /// instead.
    /// </summary>
    public static B foldMapUntil<T, A, B>(Func<A, B> f, Func<(B State, A Value), bool> predicate, K<T, A> ta)
        where T : Foldable<T>
        where B : Monoid<B> =>
        T.FoldUntil(x => a => f(x).Combine(a), predicate, B.Empty, ta);

    /// <summary>
    /// A left-associative variant of 'FoldMap' that is strict in the
    /// accumulator.  Use this method for strict reduction when partial
    /// results are merged via `Append`.
    /// </summary>
    public static B foldMapBack<T, A, B>(Func<A, B> f, K<T, A> ta)
        where T : Foldable<T>
        where B : Monoid<B> =>
        T.FoldBack(x => a => x.Combine(f(a)), B.Empty, ta);

    /// <summary>
    /// A left-associative variant of 'FoldMap' that is strict in the
    /// accumulator.  Use this method for strict reduction when partial
    /// results are merged via `Append`.
    /// </summary>
    public static B foldMapBackWhile<T, A, B>(Func<A, B> f, Func<(B State, A Value), bool> predicate, K<T, A> ta)
        where T : Foldable<T>
        where B : Monoid<B> =>
        T.FoldBackWhile(x => a => x.Combine(f(a)), predicate, B.Empty, ta);

    /// <summary>
    /// A left-associative variant of 'FoldMap' that is strict in the
    /// accumulator.  Use this method for strict reduction when partial
    /// results are merged via `Append`.
    /// </summary>
    public static B foldMapBackUntil<T, A, B>(Func<A, B> f, Func<(B State, A Value), bool> predicate, K<T, A> ta)
        where T : Foldable<T> 
        where B : Monoid<B> =>
        T.FoldBackUntil(x => a => x.Combine(f(a)), predicate, B.Empty, ta);

    /// <summary>
    /// List of elements of a structure, from left to right
    /// </summary>
    public static Seq<A> toSeq<T, A>(K<T, A> ta) 
        where T : Foldable<T> =>
        T.Fold(a => s => s.Add(a), Seq<A>(), ta);

    /// <summary>
    /// List of elements of a structure, from left to right
    /// </summary>
    public static Lst<A> toLst<T, A>(K<T, A> ta) 
        where T : Foldable<T> =>
        T.Fold(a => s => s.Add(a), List.empty<A>(), ta);

    /// <summary>
    /// List of elements of a structure, from left to right
    /// </summary>
    public static Arr<A> toArr<T, A>(K<T, A> ta)
        where T : Foldable<T> =>
        T.ToArr(ta);

    /// <summary>
    /// List of elements of a structure, from left to right
    /// </summary>
    public static EnumerableM<A> toEnumerable<T, A>(K<T, A> ta) 
        where T : Foldable<T> =>
        T.Fold(a => s =>
                    {
                        s.Add(a);
                        return s;
                    }, new System.Collections.Generic.List<A>(), ta).AsEnumerableM();

    /// <summary>
    /// List of elements of a structure, from left to right
    /// </summary>
    public static bool isEmpty<T, A>(K<T, A> ta) 
        where T : Foldable<T> =>
        T.FoldWhile(_ => _ => false, s => s.State, true, ta);

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
        T.FoldBack(c => _ => c + 1, 0, ta);

    /// <summary>
    /// Does an element that fits the predicate occur in the structure?
    /// </summary>
    public static bool exists<T, A>(Func<A, bool> predicate, K<T, A> ta) 
        where T : Foldable<T> =>
        T.FoldBackUntil(s => c => s || predicate(c), s => s.State, false, ta);

    /// <summary>
    /// Does the predicate hold for all elements in the structure?
    /// </summary>
    public static bool forAll<T, A>(Func<A, bool> predicate, K<T, A> ta) 
        where T : Foldable<T> =>
        T.FoldBackWhile(s => c => s && predicate(c), s => s.State, true, ta);

    /// <summary>
    /// Does the element exist in the structure?
    /// </summary>
    public static bool contains<EqA, T, A>(A value, K<T, A> ta) 
        where EqA : Eq<A> 
        where T : Foldable<T> =>
        T.Exists(x => EqA.Equals(value, x), ta);

    /// <summary>
    /// Does the element exist in the structure?
    /// </summary>
    public static bool contains<T, A>(A value, K<T, A> ta)
        where T : Foldable<T> =>
        T.Contains(value, ta);

    /// <summary>
    /// Find the first element that match the predicate
    /// </summary>
    public static Option<A> find<T, A>(Func<A, bool> predicate, K<T, A> ta)
        where T : Foldable<T> =>
        T.Find(predicate, ta);

    /// <summary>
    /// Find the last element that match the predicate
    /// </summary>
    public static Option<A> findBack<T, A>(Func<A, bool> predicate, K<T, A> ta) 
        where T : Foldable<T> =>
        T.FindBack(predicate, ta);

    /// <summary>
    /// Find the the elements that match the predicate
    /// </summary>
    public static Seq<A> findAll<T, A>(Func<A, bool> predicate, K<T, A> ta) 
        where T : Foldable<T> =>
        T.FindAll(predicate, ta);

    /// <summary>
    /// Find the the elements that match the predicate
    /// </summary>
    public static Seq<A> findAllBack<T, A>(Func<A, bool> predicate, K<T, A> ta) 
        where T : Foldable<T> =>
        T.FindAllBack(predicate, ta);

    /// <summary>
    /// Computes the sum of the numbers of a structure.
    /// </summary>
    public static A sum<T, A>(K<T, A> ta) 
        where T : Foldable<T> 
        where A : IAdditionOperators<A, A, A>, IAdditiveIdentity<A, A> =>
        T.Sum(ta);

    /// <summary>
    /// Computes the product of the numbers of a structure.
    /// </summary>
    public static A product<T, A>(K<T, A> ta) 
        where T : Foldable<T> 
        where A : IMultiplyOperators<A, A, A>, IMultiplicativeIdentity<A, A> =>
        T.Product(ta);

    /// <summary>
    /// Get the head item in the foldable or `None`
    /// </summary>
    public static Option<A> head<T, A>(K<T, A> ta) 
        where T : Foldable<T> =>
        T.FoldWhile(x => _ => Some(x), s => s.State.IsNone, Option<A>.None, ta);

    /// <summary>
    /// Get the head item in the foldable or `None`
    /// </summary>
    public static Option<A> last<T, A>(K<T, A> ta) 
        where T : Foldable<T> =>
        T.FoldBackWhile(_ => Some, s => s.State.IsNone, Option<A>.None, ta);

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
        ignore(T.Fold(a => ix => { f(ix, a); return ix + 1; }, 0, ta));
    
    /// <summary>
    /// Map each element of a structure to an action, evaluate these
    /// actions from left to right, and ignore the results.  For a version that
    /// doesn't ignore the results see `Traversable.traverse`.
    /// </summary>
    public static Unit iter<T, A>(Action<A> f, K<T, A> ta)
        where T : Foldable<T> =>
        T.Iter(f, ta);
    
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
        T.Min(ta);

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
        where T : Foldable<T>
        where A : IComparable<A> =>
        T.Max(ta);
    
    /// <summary>
    /// Find the minimum value in the structure
    /// </summary>
    public static A min<OrdA, T, A>(K<T, A> ta, A initialMin)
        where T : Foldable<T>
        where OrdA : Ord<A> =>
        T.Min<OrdA, A>(ta, initialMin);

    /// <summary>
    /// Find the minimum value in the structure
    /// </summary>
    public static A min<T, A>(K<T, A> ta, A initialMin)
        where T : Foldable<T>
        where A : IComparable<A> =>
        T.Min(ta, initialMin);

    /// <summary>
    /// Find the maximum value in the structure
    /// </summary>
    public static A max<OrdA, T, A>(K<T, A> ta, A initialMax)
        where T : Foldable<T>
        where OrdA : Ord<A> =>
        T.Max<OrdA, A>(ta, initialMax);

    /// <summary>
    /// Find the maximum value in the structure
    /// </summary>
    public static A max<T, A>(K<T, A> ta, A initialMax)
        where T : Foldable<T>
        where A : IComparable<A> =>
        T.Max(ta, initialMax);

    /// <summary>
    /// Find the average of all the values in the structure
    /// </summary>
    public static A average<T, A>(K<T, A> ta)
        where T : Foldable<T>
        where A : INumber<A> =>
        T.Average(ta);

    /// <summary>
    /// Find the average of all the values in the structure
    /// </summary>
    public static B average<T, A, B>(Func<A, B> f, K<T, A> ta)
        where T : Foldable<T>
        where B : INumber<B> =>
        T.Average(f, ta);

    /// <summary>
    /// Find the element at the specified index or `None` if out of range
    /// </summary>
    public static Option<A> at<T, A>(K<T, A> ta, Index index)
        where T : Foldable<T> =>
        T.At(ta, index);
}
