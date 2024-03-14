using System;
using System.Numerics;
using static LanguageExt.Prelude;

namespace LanguageExt.Traits;

public interface Foldable<out T> where T : Foldable<T>
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Abstract members
    //

    /// <summary>
    /// Same behaviour as `Fold` but allows early exit of the operation once
    /// the predicate function becomes `false` for the state/value pair 
    /// </remarks>
    public static abstract S FoldWhile<A, S>(
        Func<A, Func<S, S>> f, 
        Func<(S State, A Value), bool> predicate, 
        S initialState, 
        K<T, A> ta);

    /// <summary>
    /// Same behaviour as `FoldBack` but allows early exit of the operation once
    /// the predicate function becomes `false` for the state/value pair 
    /// </remarks>
    public static abstract S FoldBackWhile<A, S>(
        Func<S, Func<A, S>> f, 
        Func<(S State, A Value), bool> predicate, 
        S initialState, 
        K<T, A> ta);
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Default implementations
    //

    /// <summary>
    /// Same behaviour as `Fold` but the fold operation returns a monadic type and allows
    /// early exit of the operation once the predicate function becomes `false` for the
    /// state/value pair 
    /// </remarks>
    public static virtual K<M, S> FoldWhileM<A, M, S>(
        Func<A, Func<S, K<M, S>>> f, 
        Func<A, bool> predicate, 
        S initialState, 
        K<T, A> ta) 
        where M : Monad<M>
    {
        return T.FoldWhile(acc, s => predicate(s.Value), Monad.pure<M, S>, ta)(initialState);

        Func<Func<S, K<M, S>>, Func<S, K<M, S>>> acc(A value) =>
            bind => state => Monad.bind(f(value)(state), bind);
    }

    /// <summary>
    /// Same behaviour as `FoldBack` but the fold operation returns a monadic type and allows
    /// early exit of the operation once the predicate function becomes `false` for the
    /// state/value pair 
    /// </remarks>
    public static virtual K<M, S> FoldBackWhileM<A, M, S>(
        Func<S, Func<A, K<M, S>>> f, 
        Func<A, bool> predicate, 
        S initialState, 
        K<T, A> ta)
        where M : Monad<M>
    {
        return T.FoldBackWhile(acc, s => predicate(s.Value), Monad.pure<M, S>, ta)(initialState);

        Func<A, Func<S, K<M, S>>> acc(Func<S, K<M, S>> bind) =>
            value => state => Monad.bind(f(state)(value), bind);
    }

    /// <summary>
    /// Same behaviour as `Fold` but allows early exit of the operation once
    /// the predicate function becomes `false` for the state/value pair
    /// </remarks>
    public static virtual S FoldUntil<A, S>(
        Func<A, Func<S, S>> f,
        Func<(S State, A Value), bool> predicate,
        S initialState,
        K<T, A> ta) =>
        T.FoldWhile(f, not(predicate), initialState, ta);

    /// <summary>
    /// Same behaviour as `Fold` but the fold operation returns a monadic type and allows
    /// early exit of the operation once the predicate function becomes `false` for the
    /// state/value pair 
    /// </remarks>
    public static virtual K<M, S> FoldUntilM<A, M, S>(
        Func<A, Func<S, K<M, S>>> f, 
        Func<A, bool> predicate, 
        S initialState, 
        K<T, A> ta) 
        where M : Monad<M>
    {
        return T.FoldUntil(acc, s => predicate(s.Value), Monad.pure<M, S>, ta)(initialState);

        Func<Func<S, K<M, S>>, Func<S, K<M, S>>> acc(A value) =>
            bind => state => Monad.bind(f(value)(state), bind);
    }

    /// <summary>
    /// Same behaviour as `FoldBack` but allows early exit of the operation once
    /// the predicate function becomes `false` for the state/value pair
    /// </remarks>
    public static virtual S FoldBackUntil<A, S>(
        Func<S, Func<A, S>> f, 
        Func<(S State, A Value), bool> predicate, 
        S initialState, 
        K<T, A> ta) =>
        T.FoldBackWhile(f, not(predicate), initialState, ta);

    /// <summary>
    /// Same behaviour as `FoldBack` but the fold operation returns a monadic type and allows
    /// early exit of the operation once the predicate function becomes `false` for the
    /// state/value pair 
    /// </remarks>
    public static virtual K<M, S> FoldBackUntilM<A, M, S>(
        Func<S, Func<A, K<M, S>>> f, 
        Func<A, bool> predicate, 
        S initialState, 
        K<T, A> ta)
        where M : Monad<M>
    {
        return T.FoldBackUntil(acc, s => predicate(s.Value), Monad.pure<M, S>, ta)(initialState);

        Func<A, Func<S, K<M, S>>> acc(Func<S, K<M, S>> bind) =>
            value => state => Monad.bind(f(state)(value), bind);
    }

    /// <summary>
    /// Right-associative fold of a structure, lazy in the accumulator.
    ///
    /// In the case of lists, 'Fold', when applied to a binary operator, a
    /// starting value (typically the right-identity of the operator), and a
    /// list, reduces the list using the binary operator, from right to left.
    /// </summary>
    public static virtual S Fold<A, S>(Func<A, Func<S, S>> f, S initialState, K<T, A> ta) =>
        T.FoldWhile(f, _ => true, initialState, ta);

    /// <summary>
    /// Right-associative fold of a structure, lazy in the accumulator.
    ///
    /// In the case of lists, 'Fold', when applied to a binary operator, a
    /// starting value (typically the right-identity of the operator), and a
    /// list, reduces the list using the binary operator, from right to left.
    /// </summary>
    public static virtual K<M, S> FoldM<A, M, S>(
        Func<A, Func<S, K<M, S>>> f, 
        S initialState, 
        K<T, A> ta) 
        where M : Monad<M>
    {
        return T.Fold(acc, Monad.pure<M, S>, ta)(initialState);

        Func<Func<S, K<M, S>>, Func<S, K<M, S>>> acc(A value) =>
            bind => state => Monad.bind(f(value)(state), bind);
    }
    
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
    public static virtual S FoldBack<A, S>(Func<S, Func<A, S>> f, S initialState, K<T, A> ta) =>
        T.FoldBackWhile(f, _ => true, initialState, ta);

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
    public static virtual K<M, S> FoldBackM<A, M, S>(
        Func<S, Func<A, K<M, S>>> f, 
        S initialState, 
        K<T, A> ta)
        where M : Monad<M>
    {
        return T.FoldBack(acc, Monad.pure<M, S>, ta)(initialState);

        Func<A, Func<S, K<M, S>>> acc(Func<S, K<M, S>> bind) =>
            value => state => Monad.bind(f(state)(value), bind);
    }
    
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
    /// Given a structure with elements whose type is a `Monoid`, combine them
    /// via the monoid's `Append` operator.  This fold is right-associative and
    /// lazy in the accumulator.  When you need a strict left-associative fold,
    /// use 'foldMap'' instead, with 'id' as the map.
    /// </summary>
    public static virtual A FoldWhile<A>(Func<A, bool> predicate, K<T, A> tm) 
        where A : Monoid<A> =>
        T.FoldMapWhile(identity, predicate, tm) ;

    /// <summary>
    /// Given a structure with elements whose type is a `Monoid`, combine them
    /// via the monoid's `Append` operator.  This fold is right-associative and
    /// lazy in the accumulator.  When you need a strict left-associative fold,
    /// use 'foldMap'' instead, with 'id' as the map.
    /// </summary>
    public static virtual A FoldUntil<A>(Func<A, bool> predicate, K<T, A> tm) 
        where A : Monoid<A> =>
        T.FoldMapUntil(identity, predicate, tm) ;

    /// <summary>
    /// Map each element of the structure into a monoid, and combine the
    /// results with `Append`.  This fold is right-associative and lazy in the
    /// accumulator.  For strict left-associative folds consider `FoldMapBack`
    /// instead.
    /// </summary>
    public static virtual B FoldMap<A, B>(Func<A, B> f, K<T, A> ta)
        where B : Monoid<B> =>
        T.Fold(x => a => f(x).Combine(a), B.Empty, ta);

    /// <summary>
    /// Map each element of the structure into a monoid, and combine the
    /// results with `Append`.  This fold is right-associative and lazy in the
    /// accumulator.  For strict left-associative folds consider `FoldMapBack`
    /// instead.
    /// </summary>
    public static virtual B FoldMapWhile<A, B>(Func<A, B> f, Func<A, bool> predicate, K<T, A> ta)
        where B : Monoid<B> =>
        T.FoldWhile(x => a => f(x).Combine(a), s => predicate(s.Value), B.Empty, ta);

    /// <summary>
    /// Map each element of the structure into a monoid, and combine the
    /// results with `Append`.  This fold is right-associative and lazy in the
    /// accumulator.  For strict left-associative folds consider `FoldMapBack`
    /// instead.
    /// </summary>
    public static virtual B FoldMapUntil<A, B>(Func<A, B> f, Func<A, bool> predicate, K<T, A> ta)
        where B : Monoid<B> =>
        T.FoldUntil(x => a => f(x).Combine(a), s => predicate(s.Value), B.Empty, ta);

    /// <summary>
    /// A left-associative variant of 'FoldMap' that is strict in the
    /// accumulator.  Use this method for strict reduction when partial
    /// results are merged via `Append`.
    /// </summary>
    public static virtual B FoldMapBack<A, B>(Func<A, B> f, K<T, A> ta)
        where B : Monoid<B> =>
        T.FoldBack(x => a => x.Combine(f(a)), B.Empty, ta);

    /// <summary>
    /// A left-associative variant of 'FoldMap' that is strict in the
    /// accumulator.  Use this method for strict reduction when partial
    /// results are merged via `Append`.
    /// </summary>
    public static virtual B FoldMapBackWhile<A, B>(Func<A, B> f, Func<A, bool> predicate, K<T, A> ta)
        where B : Monoid<B> =>
        T.FoldBackWhile(x => a => x.Combine(f(a)), s => predicate(s.Value), B.Empty, ta);

    /// <summary>
    /// A left-associative variant of 'FoldMap' that is strict in the
    /// accumulator.  Use this method for strict reduction when partial
    /// results are merged via `Append`.
    /// </summary>
    public static virtual B FoldMapBackUntil<A, B>(Func<A, B> f, Func<A, bool> predicate, K<T, A> ta)
        where B : Monoid<B> =>
        T.FoldBackUntil(x => a => x.Combine(f(a)), s => predicate(s.Value), B.Empty, ta);

    /// <summary>
    /// List of elements of a structure, from left to right
    /// </summary>
    public static virtual Seq<A> ToSeq<A>(K<T, A> ta) =>
        T.Fold(a => s => s.Add(a), Seq<A>(), ta);

    /// <summary>
    /// List of elements of a structure, from left to right
    /// </summary>
    public static virtual Lst<A> Freeze<A>(K<T, A> ta) =>
        T.Fold(a => s => s.Add(a), List.empty<A>(), ta);

    /// <summary>
    /// List of elements of a structure, from left to right
    /// </summary>
    public static virtual EnumerableM<A> ToEnumerable<A>(K<T, A> ta) =>
        T.Fold(a => s =>
               {
                   s.Add(a);
                   return s;
               }, new System.Collections.Generic.List<A>(), ta).AsEnumerableM();

    /// <summary>
    /// List of elements of a structure, from left to right
    /// </summary>
    public static virtual bool IsEmpty<A>(K<T, A> ta) =>
        T.FoldWhile(_ => _ => false, s => s.State, true, ta);

    /// <summary>
    /// Returns the size/length of a finite structure as an `int`.  The
    /// default implementation just counts elements starting with the leftmost.
    /// 
    /// Instances for structures that can compute the element count faster
    /// than via element-by-element counting, should provide a specialised
    /// implementation.
    /// </summary>
    public static virtual int Count<A>(K<T, A> ta) =>
        T.FoldBack(c => _ => c + 1, 0, ta);

    /// <summary>
    /// Does an element that fits the predicate occur in the structure?
    /// </summary>
    public static virtual bool Exists<A>(Func<A, bool> predicate, K<T, A> ta) =>
        T.FoldBackUntil(s => c => s || predicate(c), s => s.State, false, ta);

    /// <summary>
    /// Does the predicate hold for all elements in the structure?
    /// </summary>
    public static virtual bool ForAll<A>(Func<A, bool> predicate, K<T, A> ta) =>
        T.FoldBackWhile(s => c => s && predicate(c), s => s.State, true, ta);

    /// <summary>
    /// Does the element exist in the structure?
    /// </summary>
    public static virtual bool Contains<EqA, A>(A value, K<T, A> ta) where EqA : Eq<A> =>
        T.Exists(x => EqA.Equals(value, x), ta);

    /// <summary>
    /// Computes the sum of the numbers of a structure.
    /// </summary>
    public static virtual A Sum<A>(K<T, A> ta) 
        where A : IAdditionOperators<A, A, A>, IAdditiveIdentity<A, A> =>
        T.Fold(x => y => x + y, A.AdditiveIdentity, ta);

    /// <summary>
    /// Computes the product of the numbers of a structure.
    /// </summary>
    public static virtual A Product<A>(K<T, A> ta) 
        where A : IMultiplyOperators<A, A, A>, IMultiplicativeIdentity<A, A> =>
        T.Fold(x => y => x * y, A.MultiplicativeIdentity, ta);

    /// <summary>
    /// Get the head item in the foldable
    /// </summary>
    /// <exception cref="InvalidOperationException">Throws if sequence is empty.  Consider using `HeadOrNone`</exception>
    public static virtual A Head<A>(K<T, A> ta) =>
        T.HeadOrNone(ta).IfNone(() => throw new InvalidOperationException("Sequence empty"));

    /// <summary>
    /// Get the head item in the foldable or `None`
    /// </summary>
    public static virtual Option<A> HeadOrNone<A>(K<T, A> ta) =>
        T.FoldWhile(x => _ => Some(x), s => s.State.IsNone, Option<A>.None, ta);

    /// <summary>
    /// Get the head item in the foldable
    /// </summary>
    /// <exception cref="InvalidOperationException">Throws if sequence is empty.  Consider using `HeadOrNone`</exception>
    public static virtual A Last<A>(K<T, A> ta) =>
        T.LastOrNone(ta).IfNone(() => throw new InvalidOperationException("Sequence empty"));

    /// <summary>
    /// Get the head item in the foldable or `None`
    /// </summary>
    public static virtual Option<A> LastOrNone<A>(K<T, A> ta) =>
        T.FoldBackWhile(_ => Some, s => s.State.IsNone, Option<A>.None, ta);
    
    /// <summary>
    /// Map each element of a structure to an 'Applicative' action, evaluate these
    /// actions from left to right, and ignore the results.  For a version that
    /// doesn't ignore the results see `Traversable.traverse`.
    /// </summary>
    public static virtual K<F, Unit> Iter<A, F, B>(Func<A, K<F, B>> f, K<T, A> ta) 
        where F : Applicative<F>
    {
        return T.Fold(acc, F.Pure(unit), ta);
        Func<K<F, Unit>, K<F, Unit>> acc(A x) =>
            k => F.Map(_ => unit, F.Action(f(x), k));
    }
}
