using System;
using System.Numerics;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static partial class FoldableExtensions
{
    /// <summary>
    /// Same behaviour as `Fold` but allows early exit of the operation once
    /// the predicate function becomes `false` for the state/value pair 
    /// </remarks>
    public static S FoldWhile<T, A, S>(
        this K<T, A> ta,
        Func<A, Func<S, S>> f, 
        Func<(S State, A Value), bool> predicate, 
        S initialState)
        where T : Foldable<T> =>
        T.FoldWhile(f, predicate, initialState, ta);
    
    /// <summary>
    /// Same behaviour as `Fold` but allows early exit of the operation once
    /// the predicate function becomes `false` for the state/value pair 
    /// </remarks>
    public static S FoldWhile<T, A, S>(
        this K<T, A> ta,
        Func<S, A, S> f, 
        Func<(S State, A Value), bool> predicate, 
        S initialState)
        where T : Foldable<T> =>
        T.FoldWhile(a => s => f(s, a), predicate, initialState, ta);

    /// <summary>
    /// Same behaviour as `FoldBack` but allows early exit of the operation once
    /// the predicate function becomes `false` for the state/value pair 
    /// </remarks>
    public static S FoldBackWhile<T, A, S>(
        this K<T, A> ta,
        Func<S, Func<A, S>> f, 
        Func<(S State, A Value), bool> predicate, 
        S initialState) 
        where T : Foldable<T> =>
        T.FoldBackWhile(f, predicate, initialState, ta);

    /// <summary>
    /// Same behaviour as `FoldBack` but allows early exit of the operation once
    /// the predicate function becomes `false` for the state/value pair 
    /// </remarks>
    public static S FoldBackWhile<T, A, S>(
        this K<T, A> ta,
        Func<S, A, S> f, 
        Func<(S State, A Value), bool> predicate, 
        S initialState) 
        where T : Foldable<T> =>
        T.FoldBackWhile(curry(f), predicate, initialState, ta);

    /// <summary>
    /// Same behaviour as `Fold` but the fold operation returns a monadic type and allows
    /// early exit of the operation once the predicate function becomes `false` for the
    /// state/value pair 
    /// </remarks>
    public static K<M, S> FoldWhileM<T, A, M, S>(
        this K<T, A> ta,
        Func<A, Func<S, K<M, S>>> f, 
        Func<A, bool> predicate, 
        S initialState) 
        where T : Foldable<T>
        where M : Monad<M> =>
        T.FoldWhileM(f, predicate, initialState, ta);

    /// <summary>
    /// Same behaviour as `Fold` but the fold operation returns a monadic type and allows
    /// early exit of the operation once the predicate function becomes `false` for the
    /// state/value pair 
    /// </remarks>
    public static K<M, S> FoldWhileM<T, A, M, S>(
        this K<T, A> ta,
        Func<S, A, K<M, S>> f, 
        Func<A, bool> predicate, 
        S initialState) 
        where T : Foldable<T>
        where M : Monad<M> =>
        T.FoldWhileM<A, M, S>(a => s => f(s, a), predicate, initialState, ta);

    /// <summary>
    /// Same behaviour as `FoldBack` but the fold operation returns a monadic type and allows
    /// early exit of the operation once the predicate function becomes `false` for the
    /// state/value pair 
    /// </remarks>
    public static K<M, S> FoldBackWhileM<T, A, M, S>(
        this K<T, A> ta,
        Func<S, Func<A, K<M, S>>> f, 
        Func<A, bool> predicate, 
        S initialState)
        where T : Foldable<T> 
        where M : Monad<M> =>
        T.FoldBackWhileM(f, predicate, initialState, ta);

    /// <summary>
    /// Same behaviour as `FoldBack` but the fold operation returns a monadic type and allows
    /// early exit of the operation once the predicate function becomes `false` for the
    /// state/value pair 
    /// </remarks>
    public static K<M, S> FoldBackWhileM<T, A, M, S>(
        this K<T, A> ta,
        Func<S, A, K<M, S>> f, 
        Func<A, bool> predicate, 
        S initialState)
        where T : Foldable<T> 
        where M : Monad<M> =>
        T.FoldBackWhileM(curry(f), predicate, initialState, ta);

    /// <summary>
    /// Same behaviour as `Fold` but allows early exit of the operation once
    /// the predicate function becomes `false` for the state/value pair
    /// </remarks>
    public static S FoldUntil<T, A, S>(
        this K<T, A> ta,
        Func<A, Func<S, S>> f,
        Func<(S State, A Value), bool> predicate,
        S initialState) 
        where T : Foldable<T> =>
        T.FoldUntil(f, predicate, initialState, ta);

    /// <summary>
    /// Same behaviour as `Fold` but allows early exit of the operation once
    /// the predicate function becomes `false` for the state/value pair
    /// </remarks>
    public static S FoldUntil<T, A, S>(
        this K<T, A> ta,
        Func<S, A, S> f,
        Func<(S State, A Value), bool> predicate,
        S initialState) 
        where T : Foldable<T> =>
        T.FoldUntil(a => s => f(s, a), predicate, initialState, ta);

    /// <summary>
    /// Same behaviour as `Fold` but the fold operation returns a monadic type and allows
    /// early exit of the operation once the predicate function becomes `false` for the
    /// state/value pair 
    /// </remarks>
    public static K<M, S> FoldUntilM<T, A, M, S>(
        this K<T, A> ta,
        Func<A, Func<S, K<M, S>>> f, 
        Func<A, bool> predicate, 
        S initialState) 
        where M : Monad<M>
        where T : Foldable<T> => 
        T.FoldUntilM(f, predicate, initialState, ta);

    /// <summary>
    /// Same behaviour as `Fold` but the fold operation returns a monadic type and allows
    /// early exit of the operation once the predicate function becomes `false` for the
    /// state/value pair 
    /// </remarks>
    public static K<M, S> FoldUntilM<T, A, M, S>(
        this K<T, A> ta,
        Func<S, A, K<M, S>> f, 
        Func<A, bool> predicate, 
        S initialState) 
        where M : Monad<M>
        where T : Foldable<T> => 
        T.FoldUntilM<A, M, S>(a => s => f(s, a), predicate, initialState, ta);

    /// <summary>
    /// Same behaviour as `FoldBack` but allows early exit of the operation once
    /// the predicate function becomes `false` for the state/value pair
    /// </remarks>
    public static S FoldBackUntil<T, A, S>(
        this K<T, A> ta,
        Func<S, Func<A, S>> f, 
        Func<(S State, A Value), bool> predicate, 
        S initialState) 
        where T : Foldable<T> =>
        T.FoldBackUntil(f, predicate, initialState, ta);

    /// <summary>
    /// Same behaviour as `FoldBack` but allows early exit of the operation once
    /// the predicate function becomes `false` for the state/value pair
    /// </remarks>
    public static S FoldBackUntil<T, A, S>(
        this K<T, A> ta,
        Func<S, A, S> f, 
        Func<(S State, A Value), bool> predicate, 
        S initialState) 
        where T : Foldable<T> =>
        T.FoldBackUntil(curry(f), predicate, initialState, ta);

    /// <summary>
    /// Same behaviour as `FoldBack` but the fold operation returns a monadic type and allows
    /// early exit of the operation once the predicate function becomes `false` for the
    /// state/value pair 
    /// </remarks>
    public static K<M, S> FoldBackUntilM<T, A, M, S>(
        this K<T, A> ta,
        Func<S, Func<A, K<M, S>>> f, 
        Func<A, bool> predicate, 
        S initialState)
        where T : Foldable<T> 
        where M : Monad<M> =>
        T.FoldBackUntilM(f, predicate, initialState, ta);

    /// <summary>
    /// Same behaviour as `FoldBack` but the fold operation returns a monadic type and allows
    /// early exit of the operation once the predicate function becomes `false` for the
    /// state/value pair 
    /// </remarks>
    public static K<M, S> FoldBackUntilM<T, A, M, S>(
        this K<T, A> ta,
        Func<S, A, K<M, S>> f, 
        Func<A, bool> predicate, 
        S initialState)
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
    public static S Fold<T, A, S>(this K<T, A> ta, Func<A, Func<S, S>> f, S initialState) 
        where T : Foldable<T> =>
        T.Fold(f, initialState, ta);

    /// <summary>
    /// Right-associative fold of a structure, lazy in the accumulator.
    ///
    /// In the case of lists, 'Fold', when applied to a binary operator, a
    /// starting value (typically the right-identity of the operator), and a
    /// list, reduces the list using the binary operator, from right to left.
    /// </summary>
    public static S Fold<T, A, S>(this K<T, A> ta, Func<S, A, S> f, S initialState) 
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
        Func<A, Func<S, K<M, S>>> f, 
        S initialState) 
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
        Func<S, A, K<M, S>> f, 
        S initialState) 
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
    public static S FoldBack<T, A, S>(this K<T, A> ta, Func<S, Func<A, S>> f, S initialState) 
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
    public static S FoldBack<T, A, S>(this K<T, A> ta, Func<S, A, S> f, S initialState) 
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
        Func<S, Func<A, K<M, S>>> f, 
        S initialState)
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
        Func<S, A, K<M, S>> f, 
        S initialState)
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
        T.FoldMap(identity, tm) ;

    /// <summary>
    /// Given a structure with elements whose type is a `Monoid`, combine them
    /// via the monoid's `Append` operator.  This fold is right-associative and
    /// lazy in the accumulator.  When you need a strict left-associative fold,
    /// use 'foldMap'' instead, with 'id' as the map.
    /// </summary>
    public static A FoldWhile<T, A>(this K<T, A> tm, Func<A, bool> predicate) 
        where T : Foldable<T>
        where A : Monoid<A> =>
        T.FoldMapWhile(identity, predicate, tm) ;

    /// <summary>
    /// Given a structure with elements whose type is a `Monoid`, combine them
    /// via the monoid's `Append` operator.  This fold is right-associative and
    /// lazy in the accumulator.  When you need a strict left-associative fold,
    /// use 'foldMap'' instead, with 'id' as the map.
    /// </summary>
    public static A FoldUntil<T, A>(this K<T, A> tm, Func<A, bool> predicate) 
        where T : Foldable<T>
        where A : Monoid<A> =>
        T.FoldMapUntil(identity, predicate, tm) ;

    /// <summary>
    /// Map each element of the structure into a monoid, and combine the
    /// results with `Append`.  This fold is right-associative and lazy in the
    /// accumulator.  For strict left-associative folds consider `FoldMapBack`
    /// instead.
    /// </summary>
    public static B FoldMap<T, A, B>(this K<T, A> ta, Func<A, B> f)
        where T : Foldable<T>
        where B : Monoid<B> =>
        T.Fold(x => a => f(x).Combine(a), B.Empty, ta);

    /// <summary>
    /// Map each element of the structure into a monoid, and combine the
    /// results with `Append`.  This fold is right-associative and lazy in the
    /// accumulator.  For strict left-associative folds consider `FoldMapBack`
    /// instead.
    /// </summary>
    public static B FoldMapWhile<T, A, B>(this K<T, A> ta, Func<A, B> f, Func<A, bool> predicate)
        where T : Foldable<T>
        where B : Monoid<B> =>
        T.FoldWhile(x => a => f(x).Combine(a), s => predicate(s.Value), B.Empty, ta);

    /// <summary>
    /// Map each element of the structure into a monoid, and combine the
    /// results with `Append`.  This fold is right-associative and lazy in the
    /// accumulator.  For strict left-associative folds consider `FoldMapBack`
    /// instead.
    /// </summary>
    public static B FoldMapUntil<T, A, B>(this K<T, A> ta, Func<A, B> f, Func<A, bool> predicate)
        where T : Foldable<T>
        where B : Monoid<B> =>
        T.FoldUntil(x => a => f(x).Combine(a), s => predicate(s.Value), B.Empty, ta);

    /// <summary>
    /// A left-associative variant of 'FoldMap' that is strict in the
    /// accumulator.  Use this method for strict reduction when partial
    /// results are merged via `Append`.
    /// </summary>
    public static B FoldMapBack<T, A, B>(this K<T, A> ta, Func<A, B> f)
        where T : Foldable<T>
        where B : Monoid<B> =>
        T.FoldBack(x => a => x.Combine(f(a)), B.Empty, ta);

    /// <summary>
    /// A left-associative variant of 'FoldMap' that is strict in the
    /// accumulator.  Use this method for strict reduction when partial
    /// results are merged via `Append`.
    /// </summary>
    public static B FoldMapBackWhile<T, A, B>(this K<T, A> ta, Func<A, B> f, Func<A, bool> predicate)
        where T : Foldable<T>
        where B : Monoid<B> =>
        T.FoldBackWhile(x => a => x.Combine(f(a)), s => predicate(s.Value), B.Empty, ta);

    /// <summary>
    /// A left-associative variant of 'FoldMap' that is strict in the
    /// accumulator.  Use this method for strict reduction when partial
    /// results are merged via `Append`.
    /// </summary>
    public static B FoldMapBackUntil<T, A, B>(this K<T, A> ta, Func<A, B> f, Func<A, bool> predicate)
        where T : Foldable<T> 
        where B : Monoid<B> =>
        T.FoldBackUntil(x => a => x.Combine(f(a)), s => predicate(s.Value), B.Empty, ta);

    /// <summary>
    /// List of elements of a structure, from left to right
    /// </summary>
    public static Seq<A> ToSeq<T, A>(this K<T, A> ta) 
        where T : Foldable<T> =>
        T.Fold(a => s => s.Add(a), Seq<A>(), ta);

    /// <summary>
    /// List of elements of a structure, from left to right
    /// </summary>
    public static Lst<A> Freeze<T, A>(this K<T, A> ta) 
        where T : Foldable<T> =>
        T.Fold(a => s => s.Add(a), List.empty<A>(), ta);

    /// <summary>
    /// List of elements of a structure, from left to right
    /// </summary>
    public static EnumerableM<A> ToEnumerable<T, A>(this K<T, A> ta) 
        where T : Foldable<T> =>
        T.Fold(a => s =>
                    {
                        s.Add(a);
                        return s;
                    }, new System.Collections.Generic.List<A>(), ta).AsEnumerableM();

    /// <summary>
    /// List of elements of a structure, from left to right
    /// </summary>
    public static bool IsEmpty<T, A>(this K<T, A> ta) 
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
    public static int Count<T, A>(this K<T, A> ta) 
        where T : Foldable<T> =>
        T.FoldBack(c => _ => c + 1, 0, ta);

    /// <summary>
    /// Does an element that fits the predicate occur in the structure?
    /// </summary>
    public static bool Exists<T, A>(this K<T, A> ta, Func<A, bool> predicate) 
        where T : Foldable<T> =>
        T.FoldBackUntil(s => c => s || predicate(c), s => s.State, false, ta);

    /// <summary>
    /// Does the predicate hold for all elements in the structure?
    /// </summary>
    public static bool ForAll<T, A>(this K<T, A> ta, Func<A, bool> predicate) 
        where T : Foldable<T> =>
        T.FoldBackWhile(s => c => s && predicate(c), s => s.State, true, ta);

    /// <summary>
    /// Does the element exist in the structure?
    /// </summary>
    public static bool Contains<EqA, T, A>(this K<T, A> ta, A value) 
        where EqA : Eq<A> 
        where T : Foldable<T> =>
        T.Exists(x => EqA.Equals(value, x), ta);

    /// <summary>
    /// Computes the sum of the numbers of a structure.
    /// </summary>
    public static A Sum<T, A>(this K<T, A> ta) 
        where T : Foldable<T> 
        where A : IAdditionOperators<A, A, A>, IAdditiveIdentity<A, A> =>
        T.Fold(x => y => x + y, A.AdditiveIdentity, ta);

    /// <summary>
    /// Computes the product of the numbers of a structure.
    /// </summary>
    public static A Product<T, A>(this K<T, A> ta) 
        where T : Foldable<T> 
        where A : IMultiplyOperators<A, A, A>, IMultiplicativeIdentity<A, A> =>
        T.Fold(x => y => x * y, A.MultiplicativeIdentity, ta);

    /// <summary>
    /// Get the head item in the foldable
    /// </summary>
    /// <exception cref="InvalidOperationException">Throws if sequence is empty.  Consider using `HeadOrNone`</exception>
    public static A Head<T, A>(this K<T, A> ta) 
        where T : Foldable<T> =>
        T.HeadOrNone(ta).IfNone(() => throw new InvalidOperationException("Sequence empty"));

    /// <summary>
    /// Get the head item in the foldable or `None`
    /// </summary>
    public static Option<A> HeadOrNone<T, A>(this K<T, A> ta) 
        where T : Foldable<T> =>
        T.FoldWhile(x => _ => Some(x), s => s.State.IsNone, Option<A>.None, ta);

    /// <summary>
    /// Get the head item in the foldable
    /// </summary>
    /// <exception cref="InvalidOperationException">Throws if sequence is empty.  Consider using `HeadOrNone`</exception>
    public static A Last<T, A>(this K<T, A> ta) 
        where T : Foldable<T> =>
        T.LastOrNone(ta).IfNone(() => throw new InvalidOperationException("Sequence empty"));

    /// <summary>
    /// Get the head item in the foldable or `None`
    /// </summary>
    public static Option<A> LastOrNone<T, A>(this K<T, A> ta) 
        where T : Foldable<T> =>
        T.FoldBackWhile(_ => Some, s => s.State.IsNone, Option<A>.None, ta);

    /// <summary>
    /// Map each element of a structure to an 'Applicative' action, evaluate these
    /// actions from left to right, and ignore the results.  For a version that
    /// doesn't ignore the results see `Traversable.traverse`.
    /// </summary>
    public static K<F, Unit> Iter<T, A, F, B>(this K<T, A> ta, Func<A, K<F, B>> f)
        where T : Foldable<T>
        where F : Applicative<F> =>
        T.Iter(f, ta);
}
