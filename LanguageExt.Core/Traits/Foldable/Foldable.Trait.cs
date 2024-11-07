using System;
using System.Linq;
using System.Numerics;
using LanguageExt.ClassInstances;
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
    /// </summary>
    public static abstract S FoldWhile<A, S>(
        Func<A, Func<S, S>> f, 
        Func<(S State, A Value), bool> predicate, 
        S initialState, 
        K<T, A> ta);

    /// <summary>
    /// Same behaviour as `FoldBack` but allows early exit of the operation once
    /// the predicate function becomes `false` for the state/value pair 
    /// </summary>
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
    /// Fold until the `Option` returns `None`
    /// </summary>
    /// <param name="f">Fold function</param>
    /// <param name="initialState">Initial state for the fold</param>
    /// <param name="ta">Foldable structure</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <typeparam name="S">State type</typeparam>
    /// <returns>Aggregated value</returns>
    public static virtual S FoldMaybe<A, S>(
        Func<S, Func<A, Option<S>>> f,
        S initialState,
        K<T, A> ta) =>
        T.FoldWhile<A, (bool IsSome, S Value)>(
            a => s => f(s.Value)(a) switch
                      {
                          { IsSome: true, Case: S value } => (true, value),
                          _                               => (false, s.Value)
                      },
            s => s.State.IsSome,
            (true, initialState), 
            ta).Value;

    /// <summary>
    /// Fold until the `Option` returns `None`
    /// </summary>
    /// <param name="f">Fold function</param>
    /// <param name="initialState">Initial state for the fold</param>
    /// <param name="ta">Foldable structure</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <typeparam name="S">State type</typeparam>
    /// <returns>Aggregated value</returns>
    public static virtual S FoldBackMaybe<A, S>(
        Func<A, Func<S, Option<S>>> f,
        S initialState,
        K<T, A> ta) =>
        T.FoldBackWhile<A, (bool IsSome, S Value)>(
            s => a => f(a)(s.Value) switch
                      {
                          { IsSome: true, Case: S value } => (true, value),
                          _                               => (false, s.Value)
                      },
            s => s.State.IsSome,
            (true, initialState),
            ta).Value;
    
    /// <summary>
    /// Same behaviour as `Fold` but the fold operation returns a monadic type and allows
    /// early exit of the operation once the predicate function becomes `false` for the
    /// state/value pair 
    /// </summary>
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
    /// </summary>
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
    /// </summary>
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
    /// </summary>
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
    /// </summary>
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
    public static virtual A FoldWhile<A>(Func<(A State, A Value), bool> predicate, K<T, A> tm) 
        where A : Monoid<A> =>
        T.FoldMapWhile(identity, predicate, tm) ;

    /// <summary>
    /// Given a structure with elements whose type is a `Monoid`, combine them
    /// via the monoid's `Append` operator.  This fold is right-associative and
    /// lazy in the accumulator.  When you need a strict left-associative fold,
    /// use 'foldMap'' instead, with 'id' as the map.
    /// </summary>
    public static virtual A FoldUntil<A>(Func<(A State, A Value), bool> predicate, K<T, A> tm) 
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
    public static virtual B FoldMapWhile<A, B>(Func<A, B> f, Func<(B State, A Value), bool> predicate, K<T, A> ta)
        where B : Monoid<B> =>
        T.FoldWhile(x => a => f(x).Combine(a), predicate, B.Empty, ta);

    /// <summary>
    /// Map each element of the structure into a monoid, and combine the
    /// results with `Append`.  This fold is right-associative and lazy in the
    /// accumulator.  For strict left-associative folds consider `FoldMapBack`
    /// instead.
    /// </summary>
    public static virtual B FoldMapUntil<A, B>(Func<A, B> f, Func<(B State, A Value), bool> predicate, K<T, A> ta)
        where B : Monoid<B> =>
        T.FoldUntil(x => a => f(x).Combine(a), predicate, B.Empty, ta);

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
    public static virtual B FoldMapBackWhile<A, B>(Func<A, B> f, Func<(B State, A Value), bool> predicate, K<T, A> ta)
        where B : Monoid<B> =>
        T.FoldBackWhile(x => a => x.Combine(f(a)), predicate, B.Empty, ta);

    /// <summary>
    /// A left-associative variant of 'FoldMap' that is strict in the
    /// accumulator.  Use this method for strict reduction when partial
    /// results are merged via `Append`.
    /// </summary>
    public static virtual B FoldMapBackUntil<A, B>(Func<A, B> f, Func<(B State, A Value), bool> predicate, K<T, A> ta)
        where B : Monoid<B> =>
        T.FoldBackUntil(x => a => x.Combine(f(a)), predicate, B.Empty, ta);

    /// <summary>
    /// List of elements of a structure, from left to right
    /// </summary>
    public static virtual Seq<A> ToSeq<A>(K<T, A> ta) =>
        T.Fold(a => s => s.Add(a), Seq<A>(), ta);

    /// <summary>
    /// List of elements of a structure, from left to right
    /// </summary>
    public static virtual Lst<A> ToLst<A>(K<T, A> ta) =>
        T.Fold(a => s => s.Add(a), List.empty<A>(), ta);

    /// <summary>
    /// List of elements of a structure, from left to right
    /// </summary>
    public static virtual Arr<A> ToArr<A>(K<T, A> ta) =>
        new (T.ToIterable(ta).ToArray());

    /// <summary>
    /// List of elements of a structure, from left to right
    /// </summary>
    public static virtual Iterable<A> ToIterable<A>(K<T, A> ta) =>
        T.Fold(a => s =>
               {
                   s.Add(a);
                   return s;
               }, new System.Collections.Generic.List<A>(), ta).AsIterable();

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
        T.FoldUntil(c => s => s || predicate(c), s => s.State, false, ta);

    /// <summary>
    /// Does the predicate hold for all elements in the structure?
    /// </summary>
    public static virtual bool ForAll<A>(Func<A, bool> predicate, K<T, A> ta) =>
        T.FoldWhile(c => s => s && predicate(c), s => s.State, true, ta);

    /// <summary>
    /// Does the element exist in the structure?
    /// </summary>
    public static virtual bool Contains<EqA, A>(A value, K<T, A> ta) where EqA : Eq<A> =>
        T.Exists(x => EqA.Equals(value, x), ta);

    /// <summary>
    /// Does the element exist in the structure?
    /// </summary>
    public static virtual bool Contains<A>(A value, K<T, A> ta) =>
        T.Exists(x => EqDefault<A>.Equals(value, x), ta);

    /// <summary>
    /// Find the first element that match the predicate
    /// </summary>
    public static virtual Option<A> Find<A>(Func<A, bool> predicate, K<T, A> ta) =>
        T.FoldWhile(a => s => predicate(a) ? Some(a) : s, s => s.State.IsNone, Option<A>.None, ta);

    /// <summary>
    /// Find the last element that match the predicate
    /// </summary>
    public static virtual Option<A> FindBack<A>(Func<A, bool> predicate, K<T, A> ta) =>
        T.FoldBackWhile(s => a => predicate(a) ? Some(a) : s, s => s.State.IsNone, Option<A>.None, ta);

    /// <summary>
    /// Find the the elements that match the predicate
    /// </summary>
    public static virtual Seq<A> FindAll<A>(Func<A, bool> predicate, K<T, A> ta) =>
        T.Fold(a => s => predicate(a) ? s.Add(a) : s, Seq<A>.Empty, ta);

    /// <summary>
    /// Find the the elements that match the predicate
    /// </summary>
    public static virtual Seq<A> FindAllBack<A>(Func<A, bool> predicate, K<T, A> ta) =>
        T.FoldBack(s => a => predicate(a) ? s.Add(a) : s, Seq<A>.Empty, ta);

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
    /// Get the head item in the foldable or `None`
    /// </summary>
    public static virtual Option<A> Head<A>(K<T, A> ta) =>
        T.FoldWhile(x => _ => Some(x), s => s.State.IsNone, Option<A>.None, ta);

    /// <summary>
    /// Get the head item in the foldable or `None`
    /// </summary>
    public static virtual Option<A> Last<A>(K<T, A> ta) =>
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
    
    /// <summary>
    /// Map each element of a structure to an action, evaluate these
    /// actions from left to right, and ignore the results.  For a version that
    /// doesn't ignore the results see `Traversable.traverse`.
    /// </summary>
    public static virtual Unit Iter<A>(Action<A> f, K<T, A> ta) =>
        T.Fold(a => _ => { f(a); return unit; }, unit, ta);
    
    /// <summary>
    /// Map each element of a structure to an action, evaluate these
    /// actions from left to right, and ignore the results.  For a version that
    /// doesn't ignore the results see `Traversable.traverse`.
    /// </summary>
    public static virtual Unit Iter<A>(Action<int, A> f, K<T, A> ta) =>
        ignore(T.Fold(a => ix => { f(ix, a); return ix + 1; }, 0, ta));

    /// <summary>
    /// Find the minimum value in the structure
    /// </summary>
    public static virtual Option<A> Min<OrdA, A>(K<T, A> ta)
        where OrdA : Ord<A> =>
        T.Fold(x => s => s switch
                         {
                             { IsNone: true }                                             => Some(x),
                             { IsSome: true, Value: null }                                => s,
                             { IsSome: true, Value: var s1 } when OrdA.Compare(x, s1) < 0 => Some(x),
                             _                                                            => s
                         },
               Option<A>.None,
               ta);

    /// <summary>
    /// Find the minimum value in the structure
    /// </summary>
    public static virtual Option<A> Min<A>(K<T, A> ta)
        where A : IComparable<A> =>
        T.Fold(x => s => s switch
                         {
                             { IsNone: true }                                         => Some(x),
                             { IsSome: true, Value: var s1 } when x.CompareTo(s1) < 0 => Some(x),
                             _                                                        => s
                         },
               Option<A>.None,
               ta);

    /// <summary>
    /// Find the maximum value in the structure
    /// </summary>
    public static virtual Option<A> Max<OrdA, A>(K<T, A> ta)
        where OrdA : Ord<A> =>
        T.Fold(x => s => s switch
                         {
                             { IsNone: true }                                             => Some(x),
                             { IsSome: true, Value: null }                                => Some(x),
                             { IsSome: true, Value: var s1 } when OrdA.Compare(x, s1) > 0 => Some(x),
                             _                                                            => s
                         },
               Option<A>.None,
               ta);

    /// <summary>
    /// Find the maximum value in the structure
    /// </summary>
    public static virtual Option<A> Max<A>(K<T, A> ta)
        where A : IComparable<A> =>
        T.Fold(x => s => s switch
                         {
                             { IsNone: true }                                         => Some(x),
                             { IsSome: true, Value: var s1 } when x.CompareTo(s1) > 0 => Some(x),
                             _                                                        => s
                         },
               Option<A>.None,
               ta);

    /// <summary>
    /// Find the minimum value in the structure
    /// </summary>
    public static virtual A Min<OrdA, A>(K<T, A> ta, A initialMin)
        where OrdA : Ord<A> =>
        T.Fold(x => s => OrdA.Compare(x, s) < 0 ? x : s, initialMin, ta);

    /// <summary>
    /// Find the minimum value in the structure
    /// </summary>
    public static virtual A Min<A>(K<T, A> ta, A initialMin)
        where A : IComparable<A> =>
        T.Fold(x => s => x.CompareTo(s) < 0 ? x : s, initialMin, ta);

    /// <summary>
    /// Find the maximum value in the structure
    /// </summary>
    public static virtual A Max<OrdA, A>(K<T, A> ta, A initialMax)
        where OrdA : Ord<A> =>
        T.Fold(x => s => OrdA.Compare(x, s) > 0 ? x : s, initialMax, ta);

    /// <summary>
    /// Find the maximum value in the structure
    /// </summary>
    public static virtual A Max<A>(K<T, A> ta, A initialMax)
        where A : IComparable<A> =>
        T.Fold(x => s => x.CompareTo(s) > 0 ? x : s, initialMax, ta);

    /// <summary>
    /// Find the average of all the values in the structure
    /// </summary>
    public static virtual A Average<A>(K<T, A> ta)
        where A : INumber<A>
    {
        var (n, t) = T.Fold(x => s => (s.Count + A.One, s.Total + x), (Count: A.AdditiveIdentity, Total: A.AdditiveIdentity), ta);
        return t / n;
    }

    /// <summary>
    /// Find the average of all the values in the structure
    /// </summary>
    public static virtual B Average<A, B>(Func<A, B> f, K<T, A> ta)
        where B : INumber<B>
    {
        var (n, t) = T.Fold(x => s => (s.Count + B.One, s.Total + f(x)), (Count: B.AdditiveIdentity, Total: B.AdditiveIdentity), ta);
        return t / n;
    }

    /// <summary>
    /// Find the element at the specified index or `None` if out of range
    /// </summary>
    public static virtual Option<A> At<A>(K<T, A> ta, Index index) =>
        index.IsFromEnd
            ? T.FoldBackWhile(s => a => s.Index == index.Value
                                            ? (s.Index + 1, Some(a))
                                            : (s.Index + 1, Option<A>.None),
                              s => s.State.Result.IsNone,
                              (Index: 0, Result: Option<A>.None),
                              ta).Result
            : T.FoldWhile(a => s => s.Index == index.Value
                                        ? (s.Index + 1, Some(a))
                                        : (s.Index + 1, Option<A>.None),
                          s => s.State.Result.IsNone,
                          (Index: 0, Result: Option<A>.None),
                          ta).Result;

    /// <summary>
    /// Partition a foldable into two sequences based on a predicate
    /// </summary>
    /// <param name="f">Predicate function</param>
    /// <param name="ta">Foldable structure</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Partitioned structure</returns>
    public static virtual (Seq<A> True, Seq<A> False) Partition<A>(Func<A, bool> f, K<T, A> ta) =>
        T.Fold(x => s => f(x) ? (s.True.Add(x), s.False)
                              : (s.True, s.False.Add(x)),
               (True: Seq<A>(), False: Seq<A>()),
               ta);
}
