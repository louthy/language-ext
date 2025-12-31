using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static partial class FoldableExtensions
{
    /// <summary>
    /// Same behaviour as `Fold` but allows early exit of the operation once
    /// the predicate function becomes `false` for the state/value pair 
    /// </summary>
    public static S FoldWhileT<T, U, A, S>(
        this K<T, K<U, A>> tua,
        S initialState,
        Func<A, Func<S, S>> f,
        Func<(S State, A Value), bool> predicate)
        where T : Foldable<T> 
        where U : Foldable<U> =>
        Foldable.fold(ua => s1 => Foldable.foldWhile(f, predicate, s1, ua), initialState, tua);

    /// <summary>
    /// Same behaviour as `Fold` but allows early exit of the operation once
    /// the predicate function becomes `false` for the state/value pair 
    /// </summary>
    public static S FoldWhileT<T, U, A, S>(
        this K<T, K<U, A>> tua,
        S initialState,
        Func<S, A, S> f, 
        Func<(S State, A Value), bool> predicate)
        where T : Foldable<T> 
        where U : Foldable<U> =>
        Foldable.fold(ua => s1 => Foldable.foldWhile(f, predicate, s1, ua), initialState, tua);

    /// <summary>
    /// Same behaviour as `FoldBack` but allows early exit of the operation once
    /// the predicate function becomes `false` for the state/value pair 
    /// </summary>
    public static S FoldBackWhileT<T, U, A, S>(
        this K<T, K<U, A>> tua,
        S initialState,
        Func<S, Func<A, S>> f, 
        Func<(S State, A Value), bool> predicate) 
        where T : Foldable<T> 
        where U : Foldable<U> =>
        Foldable.foldBack(s1 => ua => Foldable.foldBackWhile(f, predicate, s1, ua), initialState, tua);
    
    /// <summary>
    /// Same behaviour as `FoldBack` but allows early exit of the operation once
    /// the predicate function becomes `false` for the state/value pair 
    /// </summary>
    public static S FoldBackWhileT<T, U, A, S>(
        this K<T, K<U, A>> tua,
        S initialState,
        Func<S, A, S> f, 
        Func<(S State, A Value), bool> predicate) 
        where T : Foldable<T> 
        where U : Foldable<U> =>
        Foldable.foldBack(s1 => ua => Foldable.foldBackWhile(f, predicate, s1, ua), initialState, tua);

    /// <summary>
    /// Same behaviour as `Fold` but allows early exit of the operation once
    /// the predicate function becomes `false` for the state/value pair
    /// </summary>
    public static S FoldUntilT<T, U, A, S>(
        this K<T, K<U, A>> tua,
        S initialState,
        Func<A, Func<S, S>> f,
        Func<(S State, A Value), bool> predicate) 
        where T : Foldable<T> 
        where U : Foldable<U> =>
        Foldable.fold(ua => s1 => Foldable.foldUntil(f, predicate, s1, ua), initialState, tua);

    /// <summary>
    /// Same behaviour as `Fold` but allows early exit of the operation once
    /// the predicate function becomes `false` for the state/value pair
    /// </summary>
    public static S FoldUntilT<T, U, A, S>(
        this K<T, K<U, A>> tua,
        S initialState,
        Func<S, A, S> f,
        Func<(S State, A Value), bool> predicate) 
        where T : Foldable<T> 
        where U : Foldable<U> =>
        Foldable.fold(ua => s1 => Foldable.foldUntil(f, predicate, s1, ua), initialState, tua);
    
    /// <summary>
    /// Same behaviour as `FoldBack` but allows early exit of the operation once
    /// the predicate function becomes `false` for the state/value pair
    /// </summary>
    public static S FoldBackUntilT<T, U, A, S>(
        this K<T, K<U, A>> tua,
        S initialState,
        Func<S, Func<A, S>> f, 
        Func<(S State, A Value), bool> predicate) 
        where T : Foldable<T> 
        where U : Foldable<U> =>
        Foldable.foldBack(s1 => ua => Foldable.foldBackUntil(f, predicate, s1, ua), initialState, tua);

    /// <summary>
    /// Same behaviour as `FoldBack` but allows early exit of the operation once
    /// the predicate function becomes `false` for the state/value pair
    /// </summary>
    public static S FoldBackUntilT<T, U, A, S>(
        this K<T, K<U, A>> tua,
        S initialState,
        Func<S, A, S> f, 
        Func<(S State, A Value), bool> predicate) 
        where T : Foldable<T> 
        where U : Foldable<U> =>
        Foldable.foldBack(s1 => ua => Foldable.foldBackUntil(f, predicate, s1, ua), initialState, tua);

    /// <summary>
    /// Right-associative fold of a structure, lazy in the accumulator.
    ///
    /// In the case of lists, 'Fold', when applied to a binary operator, a
    /// starting value (typically the right-identity of the operator), and a
    /// list, reduces the list using the binary operator, from right to left.
    /// </summary>
    public static S FoldT<T, U, A, S>(
        this K<T, K<U, A>> tua,
        S initialState,
        Func<A, Func<S, S>> f) 
        where T : Foldable<T> 
        where U : Foldable<U> =>
        Foldable.fold(ua => s1 => Foldable.fold(f, s1, ua), initialState, tua);

    /// <summary>
    /// Right-associative fold of a structure, lazy in the accumulator.
    ///
    /// In the case of lists, 'Fold', when applied to a binary operator, a
    /// starting value (typically the right-identity of the operator), and a
    /// list, reduces the list using the binary operator, from right to left.
    /// </summary>
    public static S FoldT<T, U, A, S>(
        this K<T, K<U, A>> tua,
        S initialState,
        Func<S, A, S> f) 
        where T : Foldable<T> 
        where U : Foldable<U> =>
        Foldable.fold(ua => s1 => Foldable.fold(f, s1, ua), initialState, tua);
    
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
    public static S FoldBackT<T, U, A, S>(
        this K<T, K<U, A>> tua,
        S initialState,
        Func<S, Func<A, S>> f) 
        where T : Foldable<T> 
        where U : Foldable<U> =>
        Foldable.foldBack(s1 => ua => Foldable.foldBack(f, s1, ua), initialState, tua);
    
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
    public static S FoldBackT<T, U, A, S>(
        this K<T, K<U, A>> tua,
        S initialState,
        Func<S, A, S> f) 
        where T : Foldable<T> 
        where U : Foldable<U> =>
        Foldable.foldBack(s1 => ua => Foldable.foldBack(f, s1, ua), initialState, tua);
    
    /// <summary>
    /// Given a structure with elements whose type is a `Monoid`, combine them
    /// via the monoid's `Append` operator.  This fold is right-associative and
    /// lazy in the accumulator.  When you need a strict left-associative fold,
    /// use 'foldMap'' instead, with 'id' as the map.
    /// </summary>
    public static A FoldT<T, U, A>(this K<T, K<U, A>> tua) 
        where T : Foldable<T> 
        where U : Foldable<U>
        where A : Monoid<A> =>
        Foldable.fold(ua => s1 => Foldable.fold(a => s => s + a, s1, ua), A.Empty, tua);

    /// <summary>
    /// Given a structure with elements whose type is a `Monoid`, combine them
    /// via the monoid's `Append` operator.  This fold is right-associative and
    /// lazy in the accumulator.  When you need a strict left-associative fold,
    /// use 'foldMap'' instead, with 'id' as the map.
    /// </summary>
    public static A FoldWhileT<T, U, A>(this K<T, K<U, A>> tua, Func<(A State, A Value), bool> predicate) 
        where T : Foldable<T> 
        where U : Foldable<U>
        where A : Monoid<A> =>
        Foldable.fold(ua => s1 => Foldable.foldWhile(a => s => s + a, predicate, s1, ua), A.Empty, tua);

    /// <summary>
    /// Given a structure with elements whose type is a `Monoid`, combine them
    /// via the monoid's `Append` operator.  This fold is right-associative and
    /// lazy in the accumulator.  When you need a strict left-associative fold,
    /// use 'foldMap'' instead, with 'id' as the map.
    /// </summary>
    public static A FoldUntilT<T, U, A>(this K<T, K<U, A>> tua, Func<(A State, A Value), bool> predicate) 
        where T : Foldable<T> 
        where U : Foldable<U>
        where A : Monoid<A> =>
        Foldable.fold(ua => s1 => Foldable.foldUntil(a => s => s + a, predicate, s1, ua), A.Empty, tua);

    /// <summary>
    /// Map each element of the structure into a monoid, and combine the
    /// results with `Append`.  This fold is right-associative and lazy in the
    /// accumulator.  For strict left-associative folds consider `FoldMapBack`
    /// instead.
    /// </summary>
    public static B FoldMapT<T, U, A, B>(this K<T, K<U, A>> tua, Func<A, B> f)
        where T : Foldable<T>
        where U : Foldable<U>
        where B : Monoid<B> =>
        Foldable.fold(ua => s1 => Foldable.fold(a => s => s + f(a), s1, ua), B.Empty, tua);  

    /// <summary>
    /// Map each element of the structure into a monoid, and combine the
    /// results with `Append`.  This fold is right-associative and lazy in the
    /// accumulator.  For strict left-associative folds consider `FoldMapBack`
    /// instead.
    /// </summary>
    public static B FoldMapWhileT<T, U, A, B>(this K<T, K<U, A>> tua, Func<A, B> f, Func<(B State, A Value), bool> predicate)
        where T : Foldable<T>
        where U : Foldable<U>
        where B : Monoid<B> =>
        Foldable.fold(ua => s1 => Foldable.foldWhile(a => s => s + f(a), predicate, s1, ua), B.Empty, tua);  

    /// <summary>
    /// Map each element of the structure into a monoid, and combine the
    /// results with `Append`.  This fold is right-associative and lazy in the
    /// accumulator.  For strict left-associative folds consider `FoldMapBack`
    /// instead.
    /// </summary>
    public static B FoldMapUntilT<T, U, A, B>(this K<T, K<U, A>> tua, Func<A, B> f, Func<(B State, A Value), bool> predicate)
        where T : Foldable<T>
        where U : Foldable<U>
        where B : Monoid<B> =>
        Foldable.fold(ua => s1 => Foldable.foldUntil(a => s => s + f(a), predicate, s1, ua), B.Empty, tua);  

    /// <summary>
    /// A left-associative variant of 'FoldMap' that is strict in the
    /// accumulator.  Use this method for strict reduction when partial
    /// results are merged via `Append`.
    /// </summary>
    public static B FoldMapBackT<T, U, A, B>(this K<T, K<U, A>> tua, Func<A, B> f)
        where T : Foldable<T>
        where U : Foldable<U>
        where B : Monoid<B> =>
        Foldable.foldBack(s1 => ua => Foldable.foldBack(s => a => s + f(a), s1, ua), B.Empty, tua);  

    /// <summary>
    /// A left-associative variant of 'FoldMap' that is strict in the
    /// accumulator.  Use this method for strict reduction when partial
    /// results are merged via `Append`.
    /// </summary>
    public static B FoldMapBackWhileT<T, U, A, B>(this K<T, K<U, A>> tua, Func<A, B> f, Func<(B State, A Value), bool> predicate)
        where T : Foldable<T>
        where U : Foldable<U>
        where B : Monoid<B> =>
        Foldable.foldBack(s1 => ua => Foldable.foldBackWhile(s => a => s + f(a), predicate, s1, ua), B.Empty, tua);  

    /// <summary>
    /// A left-associative variant of 'FoldMap' that is strict in the
    /// accumulator.  Use this method for strict reduction when partial
    /// results are merged via `Append`.
    /// </summary>
    public static B FoldMapBackUntilT<T, U, A, B>(this K<T, K<U, A>> tua, Func<A, B> f, Func<(B State, A Value), bool> predicate)
        where T : Foldable<T> 
        where U : Foldable<U>
        where B : Monoid<B> =>
        Foldable.foldBack(s1 => ua => Foldable.foldBackUntil(s => a => s + f(a), predicate, s1, ua), B.Empty, tua);

    /// <summary>
    /// List of elements of a structure, from left to right
    /// </summary>
    public static Seq<A> ToSeqT<T, U, A>(this K<T, K<U, A>> tua)
        where T : Foldable<T>
        where U : Foldable<U> =>
        Foldable.fold(ua => s1 => Foldable.fold(a => s => s.Add(a), s1, ua), Seq<A>.Empty, tua);

    /// <summary>
    /// List of elements of a structure, from left to right
    /// </summary>
    public static Lst<A> ToLstT<T, U, A>(this K<T, K<U, A>> tua)
        where T : Foldable<T>
        where U : Foldable<U> =>
        Foldable.fold(ua => s1 => Foldable.fold(a => s => s.Add(a), s1, ua), Lst<A>.Empty, tua);

    /// <summary>
    /// List of elements of a structure, from left to right
    /// </summary>
    public static Arr<A> ToArrT<T, U, A>(this K<T, K<U, A>> tua)
        where T : Foldable<T>
        where U : Foldable<U>
    {
        return new(Go().ToArray());
        IEnumerable<A> Go()
        {
            foreach(var ua in tua.ToIterable())
            {
                foreach (var a in ua.ToIterable())
                {
                    yield return a;
                }
            }
        }
    }

    /// <summary>
    /// List of elements of a structure, from left to right
    /// </summary>
    public static Iterable<A> ToEnumerableT<T, U, A>(this K<T, K<U, A>> tua)
        where T : Foldable<T>
        where U : Foldable<U> =>
        Foldable.fold(
            ua => s1 => Foldable.fold(
                      a => s =>
                           {
                               s.Add(a);
                               return s;
                           },
                      s1,
                      ua),
            new List<A>(),
            tua).AsIterable();

    /// <summary>
    /// List of elements of a structure, from left to right
    /// </summary>
    public static bool IsEmptyT<T, U, A>(this K<T, K<U, A>> tua)
        where T : Foldable<T>
        where U : Foldable<U> =>
        Foldable.fold(ua => s => s && ua.IsEmpty, true, tua);
    
    /// <summary>
    /// Returns the size/length of a finite structure as an `int`.  The
    /// default implementation just counts elements starting with the leftmost.
    /// 
    /// Instances for structures that can compute the element count faster
    /// than via element-by-element counting, should provide a specialised
    /// implementation.
    /// </summary>
    public static int CountT<T, U, A>(this K<T, K<U, A>> tua) 
        where T : Foldable<T>
        where U : Foldable<U> =>
        Foldable.fold(ua => s => s + ua.Count, 0, tua);

    /// <summary>
    /// Does an element that fits the predicate occur in the structure?
    /// </summary>
    public static bool ExistsT<T, U, A>(this K<T, K<U, A>> tua, Func<A, bool> predicate) 
        where T : Foldable<T>
        where U : Foldable<U> =>
        Foldable.fold(ua => s => s || ua.Exists(predicate), false, tua);

    /// <summary>
    /// Does the predicate hold for all elements in the structure?
    /// </summary>
    public static bool ForAllT<T, U, A>(this K<T, K<U, A>> tua, Func<A, bool> predicate) 
        where T : Foldable<T>
        where U : Foldable<U> =>
        Foldable.fold(ua => s => s && ua.ForAll(predicate), true, tua);
    
    /// <summary>
    /// Does the element exist in the structure?
    /// </summary>
    public static bool ContainsT<EqA, T, U, A>(this K<T, K<U, A>> tua, A value) 
        where EqA : Eq<A> 
        where T : Foldable<T>
        where U : Foldable<U> =>
        Foldable.exists(ua => Foldable.contains<EqA, U, A>(value, ua), tua);
    
    /// <summary>
    /// Does the element exist in the structure?
    /// </summary>
    public static bool ContainsT<T, U, A>(this K<T, K<U, A>> tua, A value) 
        where T : Foldable<T>
        where U : Foldable<U> =>
        Foldable.exists(ua => Foldable.contains(value, ua), tua);
    
    /// <summary>
    /// Find the first element that match the predicate
    /// </summary>
    public static Option<A> FindT<T, U, A>(this K<T, K<U, A>> tua, Func<A, bool> predicate)
        where T : Foldable<T>
        where U : Foldable<U> =>
        Foldable.foldWhile(ua => s1 => s1 || Foldable.find(predicate, ua), s => s.State.IsNone, Option<A>.None, tua);

    /// <summary>
    /// Find the last element that match the predicate
    /// </summary>
    public static Option<A> FindBackT<T, U, A>(this K<T, K<U, A>> tua, Func<A, bool> predicate) 
        where T : Foldable<T>
        where U : Foldable<U> =>
        Foldable.foldBackWhile(s1 => ua => s1 || Foldable.find(predicate, ua), s => s.State.IsNone, Option<A>.None, tua);

    /// <summary>
    /// Find the the elements that match the predicate
    /// </summary>
    public static Iterable<A> FindAllT<T, U, A>(this K<T, K<U, A>> tua, Func<A, bool> predicate)
        where T : Foldable<T>
        where U : Foldable<U> =>
        Foldable.fold(ua => s1 => s1 + Foldable.findAll(predicate, ua), Iterable<A>.Empty, tua);

    /// <summary>
    /// Find the the elements that match the predicate
    /// </summary>
    public static Iterable<A> FindAllBackT<T, U, A>(this K<T, K<U, A>> tua, Func<A, bool> predicate) 
        where T : Foldable<T>
        where U : Foldable<U> =>
        Foldable.foldBack(s1 => ua => s1 + Foldable.findAllBack(predicate, ua), Iterable<A>.Empty, tua);
    
    /// <summary>
    /// Computes the sum of the numbers of a structure.
    /// </summary>
    public static A SumT<T, U, A>(this K<T, K<U, A>> tua) 
        where T : Foldable<T>
        where U : Foldable<U>
        where A : IAdditionOperators<A, A, A>, IAdditiveIdentity<A, A> =>
        Foldable.fold(ua => s => s + Foldable.sum(ua), A.AdditiveIdentity, tua);

    /// <summary>
    /// Computes the product of the numbers of a structure.
    /// </summary>
    public static A ProductT<T, U, A>(this K<T, K<U, A>> tua) 
        where T : Foldable<T>
        where U : Foldable<U>
        where A : IMultiplyOperators<A, A, A>, IMultiplicativeIdentity<A, A> =>
        Foldable.fold(ua => s => s * Foldable.product(ua), A.MultiplicativeIdentity, tua);

    /// <summary>
    /// Get the head item in the foldable or `None`
    /// </summary>
    public static Option<A> HeadT<T, U, A>(this K<T, K<U, A>> tua) 
        where T : Foldable<T>
        where U : Foldable<U> =>
        Foldable.foldWhile(
                     ua => s => s || Foldable.head(ua), 
                     s => s.State.IsNone, 
                     Option<A>.None, 
                     tua);

    /// <summary>
    /// Get the head item in the foldable or `None`
    /// </summary>
    public static Option<A> LastT<T, U, A>(this K<T, K<U, A>> tua) 
        where T : Foldable<T>
        where U : Foldable<U> =>
        Foldable.foldBackWhile(
                     s => ua => s || Foldable.last(ua), 
                     s => s.State.IsNone, 
                     Option<A>.None, 
                     tua);

    /// <summary>
    /// Map each element of a structure to an 'Applicative' action, evaluate these
    /// actions from left to right, and ignore the results.  For a version that
    /// doesn't ignore the results see `Traversable.traverse`.
    /// </summary>
    public static K<F, Unit> IterT<T, U, A, F, B>(this K<T, K<U, A>> tua, Func<A, K<F, B>> f)
        where T : Foldable<T>
        where U : Foldable<U>
        where F : Monad<F> =>
        Foldable.iter(ua => Foldable.iter(f, ua), tua);
    
    /// <summary>
    /// Map each element of a structure to an action, evaluate these
    /// actions from left to right, and ignore the results.  For a version that
    /// doesn't ignore the results see `Traversable.traverse`.
    /// </summary>
    public static Unit IterT<T, U, A>(this K<T, K<U, A>> tua, Action<int, A> f) 
        where T : Foldable<T> 
        where U : Foldable<U> =>
        ignore(Foldable.fold(ua => ix1 => Foldable.fold(a => ix2 => { f(ix2, a); return ix2 + 1; }, ix1, ua), 0, tua));
    
    /// <summary>
    /// Map each element of a structure to an action, evaluate these
    /// actions from left to right, and ignore the results.  For a version that
    /// doesn't ignore the results see `Traversable.traverse`.
    /// </summary>
    public static Unit IterT<T, U, A>(this K<T, K<U, A>> tua, Action<A> f)
        where T : Foldable<T>
        where U : Foldable<U> =>
        Foldable.fold(ua => _ => Foldable.iter(f, ua), unit, tua);
    
    /// <summary>
    /// Find the minimum value in the structure
    /// </summary>
    public static Option<A> MinT<OrdA, T, U, A>(this K<T, K<U, A>> tua)
        where T : Foldable<T>
        where U : Foldable<U>
        where OrdA : Ord<A> =>
        Foldable.fold(
            ua => s => s switch
                       {
                           { IsSome: true, Value: A value } => Foldable.min<OrdA, U, A>(ua, value),
                           _                                => Foldable.min<OrdA, U, A>(ua)
                       },
            Option<A>.None,
            tua);

    /// <summary>
    /// Find the minimum value in the structure
    /// </summary>
    public static Option<A> MinT<T, U, A>(this K<T, K<U, A>> tua)
        where T : Foldable<T>
        where U : Foldable<U>
        where A : IComparable<A> =>
        Foldable.fold(
            ua => s => s switch
                       {
                           { IsSome: true, Value: A value } => Foldable.min(ua, value),
                           _                                => Foldable.min(ua)
                       },
            Option<A>.None,
            tua);

    /// <summary>
    /// Find the maximum value in the structure
    /// </summary>
    public static Option<A> MaxT<OrdA, T, U, A>(this K<T, K<U, A>> tua)
        where T : Foldable<T>
        where U : Foldable<U>
        where OrdA : Ord<A> =>
        Foldable.fold(
            ua => s => s switch
                       {
                           { IsSome: true, Value: A value } => Foldable.max<OrdA, U, A>(ua, value),
                           _                                => Foldable.max<OrdA, U, A>(ua)
                       },
            Option<A>.None,
            tua);
    
    /// <summary>
    /// Find the maximum value in the structure
    /// </summary>
    public static Option<A> MaxT<T, U, A>(this K<T, K<U, A>> tua)
        where T : Foldable<T>
        where U : Foldable<U>
        where A : IComparable<A> =>
        Foldable.fold(
            ua => s => s switch
                       {
                           { IsSome: true, Value: A value } => Foldable.max(ua, value),
                           _                                => Foldable.max(ua)
                       },
            Option<A>.None,
            tua);
        
    /// <summary>
    /// Find the minimum value in the structure
    /// </summary>
    public static A MinT<OrdA, T, U, A>(this K<T, K<U, A>> tua, A initialMin)
        where T : Foldable<T>
        where U : Foldable<U>
        where OrdA : Ord<A> =>
        Foldable.fold(ua => s => Foldable.min<OrdA, U, A>(ua, s), initialMin, tua);

    /// <summary>
    /// Find the minimum value in the structure
    /// </summary>
    public static A MinT<T, U, A>(this K<T, K<U, A>> tua, A initialMin)
        where T : Foldable<T>
        where U : Foldable<U>
        where A : IComparable<A> =>
        Foldable.fold(ua => s => Foldable.min(ua, s), initialMin, tua);

    /// <summary>
    /// Find the maximum value in the structure
    /// </summary>
    public static A MaxT<OrdA, T, U, A>(this K<T, K<U, A>> tua, A initialMax)
        where T : Foldable<T>
        where U : Foldable<U>
        where OrdA : Ord<A> =>
        Foldable.fold(ua => s => Foldable.max<OrdA, U, A>(ua, s), initialMax, tua);
    
    /// <summary>
    /// Find the maximum value in the structure
    /// </summary>
    public static A MaxT<T, U, A>(this K<T, K<U, A>> tua, A initialMax)
        where T : Foldable<T>
        where U : Foldable<U>
        where A : IComparable<A> =>
        Foldable.fold(ua => s => Foldable.max(ua, s), initialMax, tua);

    /// <summary>
    /// Find the average of all the values in the structure
    /// </summary>
    public static A AverageT<T, A>(this K<T, A> ta)
        where T : Foldable<T>
        where A : INumber<A> =>
        T.Average(ta);

    /// <summary>
    /// Find the average of all the values in the structure
    /// </summary>
    public static B AverageT<T, A, B>(this K<T, A> ta, Func<A, B> f)
        where T : Foldable<T>
        where B : INumber<B> =>
        T.Average(f, ta);

    /// <summary>
    /// Find the element at the specified index or `None` if out of range
    /// </summary>
    public static Option<A> AtT<T, A>(this K<T, A> ta, Index index)
        where T : Foldable<T> =>
        T.At(ta, index);    
}
