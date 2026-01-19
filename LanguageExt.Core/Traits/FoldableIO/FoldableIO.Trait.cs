using System;
using LanguageExt.ClassInstances;
using static LanguageExt.Prelude;

namespace LanguageExt.Traits;

/// <summary>
/// <para>
/// FoldableIO structures are those that can support repeated binary applications.  You will see
/// two 'flavours' of methods in the `FoldableIO` trait: forward and backward folds, which represent
/// different approaches to associativity when applying the binary function: 
/// </para>
/// <para>
/// `Fold(Func〈S, A, S〉, S)` is equal to: `((((S * A1) * A2) * A3) * A4) * ... An)`
/// </para>
/// <para>
/// `FoldBack(Func〈S, A, S〉, S)` is equal to: `(A1 * (A2 * (A3 * (A4 * ... (An * S))))`
/// </para>
/// <para>
/// > Where the `*` operator represents the binary function passed to `Fold`.
/// </para>
/// <para>
/// This repeated application over a structure (often a collection, but not exclusively) is known as a
/// *fold*; and is a fundamental operation in functional programming.
/// </para>
/// <para>
/// It should be noted that backward folds could come with additional overhead or problems depending on
/// the underlying implementations.  A lazy sequence like `Iterable` would need to be completely evaluated
/// before it could perform the first binary operation of a backward fold. Also, if the `Iterable` is
/// infinite, then the backward fold can never be completed.   
/// </para>
/// <para>
/// Whereas, a type like `Set`, which is presorted, or a type like `Arr`, or `Lst`, which support
/// random-access, can easily and efficiently perform backward folds; because it's cheap to access the
/// last value in the FoldableIO structure and work backwards.
/// </para>
/// </summary>
/// <typeparam name="T"></typeparam>
public interface FoldableIO<T> : IterableKIO<T>
    where T : FoldableIO<T>
{
    /// <summary>
    /// Same behaviour as `Fold` but allows early exit of the operation once
    /// the predicate function becomes `false` for the state/value pair 
    /// </summary>
    static virtual IO<S> FoldWhileIO<A, S>(
        Func<S, A, S> f,
        Func<(S State, A Value), bool> predicate,
        S initialState,
        K<T, A> ta) 
    {
        return +Monad.recur((initialState, T.ForwardIteratorIO(ta)), go);

        K<IO, Next<(S State, IteratorIO<A> Iter), S>> go((S State, IteratorIO<A> Iter) input) =>
            input.Iter.NextIO() *
            (n => n is (Exist<A> (var head), var tail) && predicate((input.State, head))
                      ? Next.Loop<(S State, IteratorIO<A> Iter), S>((f(input.State, head), tail))
                      : Next.Done<(S State, IteratorIO<A> Iter), S>(input.State));
    }

    /// <summary>
    /// Fold until the `Option` returns `None`
    /// </summary>
    /// <param name="f">Fold function</param>
    /// <param name="initialState">Initial state for the fold</param>
    /// <param name="ta">FoldableIO structure</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <typeparam name="S">State type</typeparam>
    /// <returns>Aggregated value</returns>
    static virtual IO<S> FoldMaybeIO<A, S>(
        Func<S, A, Option<S>> f,
        S initialState,
        K<T, A> ta)
    {
        return +Monad.recur((initialState, T.ForwardIteratorIO(ta)), go);

        K<IO, Next<(S State, IteratorIO<A> Iter), S>> go((S State, IteratorIO<A> Iter) input) =>
            input.Iter.NextIO() *
            (n => n is (Exist<A> (var head), var tail)
                      ? f(input.State, head) switch
                        {
                            { IsSome: true, Case: S nstate } =>
                                Next.Loop<(S State, IteratorIO<A> Iter), S>((nstate, tail)),

                            _ => Next.Done<(S State, IteratorIO<A> Iter), S>(input.State)
                        }
                      : Next.Done<(S State, IteratorIO<A> Iter), S>(input.State));
    }

    /// <summary>
    /// Same behaviour as `Fold` but the fold operation returns a monadic type and allows
    /// early exit of the operation once the predicate function becomes `false` for the
    /// state/value pair 
    /// </summary>
    static virtual MS FoldWhileM<MS, M, A, S>(
        Func<S, A, MS> f,
        Func<(S State, A Value), bool> predicate,
        S initialState,
        K<T, A> ta)
        where M : MonadIO<M>
        where MS : K<M, S>
    {
        return (MS)Monad.recur((initialState, T.ForwardIteratorIO(ta)), go);

        K<M, Next<(S State, IteratorIO<A> Iter), S>> go((S State, IteratorIO<A> Iter) input) =>
            input.Iter.NextIO() >>
            (n => n is (Exist<A> (var head), var tail) && predicate((input.State, head))
                      ? f(input.State, head) * (ns => Next.Loop<(S State, IteratorIO<A> Iter), S>((ns, tail)))
                      : M.Pure(Next.Done<(S State, IteratorIO<A> Iter), S>(input.State)));
    }

    /// <summary>
    /// Same behaviour as `Fold` but allows early exit of the operation once
    /// the predicate function becomes `false` for the state/value pair
    /// </summary>
    static virtual IO<S> FoldUntilIO<A, S>(
        Func<S, A, S> f,
        Func<(S State, A Value), bool> predicate,
        in S initialState,
        K<T, A> ta) 
    {
        return +Monad.recur((initialState, T.ForwardIteratorIO(ta)), go);

        K<IO, Next<(S State, IteratorIO<A> Iter), S>> go((S State, IteratorIO<A> Iter) input) =>
            input.Iter.NextIO() * (n => n is (Exist<A> (var head), var tail)
                                            ? f(input.State, head) switch
                                              {
                                                    var ns when predicate((ns, head)) => 
                                                        Next.Done<(S State, IteratorIO<A> Iter), S>(input.State),
                                                    
                                                    var ns => 
                                                        Next.Loop<(S State, IteratorIO<A> Iter), S>((ns, tail))  
                                                        
                                              } 
                                            : Next.Done<(S State, IteratorIO<A> Iter), S>(input.State));
    }
    
    /// <summary>
    /// Same behaviour as `Fold` but the fold operation returns a monadic type and allows
    /// early exit of the operation once the predicate function becomes `false` for the
    /// state/value pair 
    /// </summary>
    static virtual MS FoldUntilM<MS, M, A, S>(
        Func<S, A, MS> f, 
        Func<(S State, A Value), bool> predicate, 
        in S initialState, 
        K<T, A> ta) 
        where M : MonadIO<M>
        where MS : K<M, S>
    {
        return (MS)Monad.recur((initialState, T.ForwardIteratorIO(ta)), go);

        K<M, Next<(S State, IteratorIO<A> Iter), S>> go((S State, IteratorIO<A> Iter) input) =>
            input.Iter.NextIO() >>
            (n => n is (Exist<A> (var head), var tail)
                      ? f(input.State, head).Map(ns => predicate((ns, head))
                                                           ? Next.Done<(S State, IteratorIO<A> Iter), S>(ns)
                                                           : Next.Loop<(S State, IteratorIO<A> Iter), S>((ns, tail)))
                      : M.Pure(Next.Done<(S State, IteratorIO<A> Iter), S>(input.State)));
    }

    /// <summary>
    /// Right-associative fold of a structure, lazy in the accumulator.
    ///
    /// In the case of lists, 'Fold', when applied to a binary operator, a
    /// starting value (typically the right-identity of the operator), and a
    /// list, reduces the list using the binary operator, from right to lefTA.
    /// </summary>
    static virtual IO<S> FoldIO<A, S>(Func<S, A, S> f, in S initialState, K<T, A> ta)
    {
        return +Monad.recur((initialState, T.ForwardIteratorIO(ta)), go);

        K<IO, Next<(S State, IteratorIO<A> Iter), S>> go((S State, IteratorIO<A> Iter) input) =>
            input.Iter.NextIO() *
            (n => n is (Exist<A> (var head), var tail)
                      ? Next.Loop<(S State, IteratorIO<A> Iter), S>((f(input.State, head), tail))
                      : Next.Done<(S State, IteratorIO<A> Iter), S>(input.State));
    }  

    /// <summary>
    /// Right-associative fold of a structure, lazy in the accumulator.
    ///
    /// In the case of lists, 'Fold', when applied to a binary operator, a
    /// starting value (typically the right-identity of the operator), and a
    /// list, reduces the list using the binary operator, from right to lefTA.
    /// </summary>
    static virtual MS FoldM<MS, M, A, S>(
        Func<S, A, MS> f,
        in S initialState,
        K<T, A> ta)
        where M : MonadIO<M>
        where MS : K<M, S>
    {
        return (MS)Monad.recur((initialState, T.ForwardIteratorIO(ta)), go);

        K<M, Next<(S State, IteratorIO<A> Iter), S>> go((S State, IteratorIO<A> Iter) input) =>
            input.Iter.NextIO() >>
            (n => n is (Exist<A> (var head), var tail)
                      ? f(input.State, head) * (ns => Next.Loop<(S State, IteratorIO<A> Iter), S>((ns, tail)))
                      : M.Pure(Next.Done<(S State, IteratorIO<A> Iter), S>(input.State)));
    }

    /// <summary>
    /// List of elements of a structure
    /// </summary>
    /// <remarks>
    /// The sequence is lazy
    /// </remarks>
    static virtual IO<Seq<A>> ToSeqIO<A>(K<T, A> ta) =>
        T.FoldIO((xs, x) => xs.Add(x), Seq<A>(), ta);

    /// <summary>
    /// List of elements of a structure
    /// </summary>
    static virtual IO<Lst<A>> ToLstIO<A>(K<T, A> ta) =>
        T.FoldIO((xs, x) => xs.Add(x), Lst<A>(), ta);

    /// <summary>
    /// List of elements of a structure, from left to right
    /// </summary>
    static virtual IO<bool> IsEmptyIO<A>(K<T, A> ta) =>
        T.ForwardIteratorIO(ta).NextIO() * (n => n is not (Exist<A>, _));

    /// <summary>
    /// Returns the size/length of a finite structure as an `int`.  The
    /// default implementation just counts elements starting with the leftmost.
    /// 
    /// Instances for structures that can compute the element count faster
    /// than via element-by-element counting, should provide a specialised
    /// implementation.
    /// </summary>
    static virtual IO<long> CountIO<A>(K<T, A> ta) =>
        T.FoldIO((c, _) => c + 1L, 0L, ta);

    /// <summary>
    /// Does an element that fits the predicate occur in the structure?
    /// </summary>
    static virtual IO<bool> ExistsIO<A>(Func<A, bool> predicate, K<T, A> ta) 
    {
        return +Monad.recur(T.ForwardIteratorIO(ta), go);

        K<IO, Next<IteratorIO<A>, bool>> go(IteratorIO<A> iter) =>
            iter.NextIO() *
            (n => n is (Exist<A> (var head), var tail)
                      ? predicate(head)
                            ? Next.Done<IteratorIO<A>, bool>(true)
                            : Next.Loop<IteratorIO<A>, bool>(tail)
                      : Next.Done<IteratorIO<A>, bool>(false));
    }  

    /// <summary>
    /// Does the predicate hold for all elements in the structure?
    /// </summary>
    static virtual IO<bool> ForAllIO<A>(Func<A, bool> predicate, K<T, A> ta)
    {
        return +Monad.recur(T.ForwardIteratorIO(ta), go);

        K<IO, Next<IteratorIO<A>, bool>> go(IteratorIO<A> iter) =>
            iter.NextIO() *
            (n => n is (Exist<A> (var head), var tail)
                      ? predicate(head)
                            ? Next.Loop<IteratorIO<A>, bool>(tail)
                            : Next.Done<IteratorIO<A>, bool>(false)
                      : Next.Done<IteratorIO<A>, bool>(true));
    }

    /// <summary>
    /// Does the element exist in the structure?
    /// </summary>
    static virtual IO<bool> ContainsIO<EqA, A>(A value, K<T, A> ta)
        where EqA : Eq<A> =>
        T.ExistsIO(x => EqA.Equals(value, x), ta);

    /// <summary>
    /// Does the element exist in the structure?
    /// </summary>
    static virtual IO<bool> ContainsIO<A>(A value, K<T, A> ta) =>
        T.ContainsIO<EqDefault<A>, A>(value, ta);

    /// <summary>
    /// Find the first element that match the predicate
    /// </summary>
    static virtual IO<Option<A>> FindIO<A>(Func<A, bool> predicate, K<T, A> ta)
    {
        return +Monad.recur(T.ForwardIteratorIO(ta), go);

        K<IO, Next<IteratorIO<A>, Option<A>>> go(IteratorIO<A> iter) =>
            iter.NextIO() *
            (n => n is (Exist<A> (var head), var tail)
                      ? predicate(head)
                            ? Next.Done<IteratorIO<A>, Option<A>>(head)
                            : Next.Loop<IteratorIO<A>, Option<A>>(tail)
                      : Next.Done<IteratorIO<A>, Option<A>>(default));
    }

    /// <summary>
    /// Get the head item in the FoldableIO or `None`
    /// </summary>
    static virtual IO<Option<A>> HeadIO<A>(K<T, A> ta) =>
        T.ForwardIteratorIO(ta).NextIO() * (n => n is (Exist<A> (var head), _)
                                                     ? Some(head)
                                                     : None);

    /// <summary>
    /// Get the head item in the `FoldableIO` or `Alternative.Empty`
    /// </summary>
    static virtual K<M, A> HeadM<M, A>(K<T, A> ta)
        where M : MonadIO<M>, Alternative<M> =>
        M.LiftIO(T.ForwardIteratorIO(ta).NextIO() * (n => n is (Exist<A> (var head), _)
                                                              ? M.Pure(head)
                                                              : M.Empty<A>()))
         .Flatten();

    /// <summary>
    /// Map each element of a structure to a monadic action, evaluate these
    /// actions from left to right, and ignore the results. 
    /// </summary>
    static virtual K<M, Unit> IterM<MB, M, A, B>(Func<A, MB> f, K<T, A> ta)
        where M : MonadIO<M>
        where MB : K<M, B>
    {
        return Monad.recur(T.ForwardIteratorIO(ta), go);

        K<M, Next<IteratorIO<A>, Unit>> go(IteratorIO<A> input) =>
            input.NextIO() >> (n => n is (Exist<A> (var head), var tail)
                                       ? f(head) * (_ => Next.Loop<IteratorIO<A>, Unit>(tail))
                                       : M.Pure(Next.Done<IteratorIO<A>, Unit>(default)));
    }
    
    /// <summary>
    /// Map each element of a structure to an action, evaluate these
    /// actions from left to right, and ignore the results.  For a version that
    /// doesn't ignore the results see `Traversable.traverse`.
    /// </summary>
    static virtual IO<Unit> IterIO<A>(Action<A> f, K<T, A> ta)
    {
        return +Monad.recur(T.ForwardIteratorIO(ta), go);

        K<IO, Next<IteratorIO<A>, Unit>> go(IteratorIO<A> input) =>
            input.NextIO() >> (n => n is (Exist<A> (var head), var tail)
                                        ? IO.lift(() => f(head)) * (_ => Next.Loop<IteratorIO<A>, Unit>(tail))
                                        : IO.pure(Next.Done<IteratorIO<A>, Unit>(default)));
    }
    
    /// <summary>
    /// Map each element of a structure to an action, evaluate these
    /// actions from left to right, and ignore the results.  For a version that
    /// doesn't ignore the results see `Traversable.traverse`.
    /// </summary>
    static virtual IO<Unit> IterIO<A>(Action<long, A> f, K<T, A> ta)
    {
        return +Monad.recur((0L, T.ForwardIteratorIO(ta)), go);

        K<IO, Next<(long, IteratorIO<A>), Unit>> go((long Ix, IteratorIO<A> Iter) input) =>
            input.Iter.NextIO() >>
            (n => n is (Exist<A> (var head), var tail)
                      ? IO.lift(() => f(input.Ix, head)) * (_ => Next.Loop<(long, IteratorIO<A>), Unit>((input.Ix + 1L, tail)))
                      : IO.pure(Next.Done<(long, IteratorIO<A>), Unit>(default)));
    }
    
        
    /// <summary>
    /// Inject a value in between each item in the enumerable 
    /// </summary>
    /// <param name="sep">Item to inject</param>
    /// <param name="ta">Foldable structure</param>
    /// <returns>An iterable with the values injected</returns>
    static virtual IteratorIO<A> IntersperseIO<A>(A sep, K<T, A> ta)
    {
        return IteratorIO.liftIO(T.ForwardIteratorIO(ta).NextIO() *
                                 (n => n is (Exist<A> (var head), var tail)
                                           ? IteratorIO.cons(head, prependToAll(tail))
                                           : IteratorIO.empty<A>()));

        IO<IteratorIO<A>> prependToAll(IteratorIO<A> iter) =>
            iter.NextIO() * (n => n is (Exist<A> (var h), var t)
                                      ? IteratorIO.cons(sep, IteratorIO.cons(h, prependToAll(t)))
                                      : IteratorIO.empty<A>());
    }
}
