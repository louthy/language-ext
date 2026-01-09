using System;
using System.Collections.Generic;
using LanguageExt.ClassInstances;
using static LanguageExt.Prelude;
using L = LanguageExt;

namespace LanguageExt.Traits;

/// <summary>
/// <para>
/// Foldable structures are those that can support repeated binary applications.  You will see
/// two 'flavours' of methods in the `Foldable` trait: forward and backward folds, which represent
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
/// last value in the foldable structure and work backwards.
/// </para>
/// </summary>
/// <typeparam name="T"></typeparam>
public interface FoldableBack<T> : IterableBackK<T>, Foldable<T>
    where T : FoldableBack<T>
{
    /// <summary>
    /// Runs a single step of the folding operation. The return value indicates whether the folding
    /// operation should continue, and if so, what the next step should be.
    /// </summary>
    /// <remarks>
    /// Mostly, consumers of `Foldable` shouldn't use `FoldStep` or `FoldStepBack` - these methods are the
    /// building blocks of every other method in the `Foldable` trait. It's more idiomatically functional
    /// to use the other methods that are built with `FoldStep` or `FoldStepBack` than to use them directly.
    ///
    /// Also, the return type `Fold〈A, S〉` is not guaranteed to be pure - it very likely won't be - and
    /// so should be used with care (usually in a tight folding loop) and definitely not shared.
    /// </remarks>
    /// <remarks>
    /// It is up to the consumer of this method to implement the actual state-aggregation (the folding)
    /// before passing it to the continuation function. 
    /// </remarks>
    /// <param name="ta">Foldable structure</param>
    /// <param name="initialState">Initial state value</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <typeparam name="S">State type</typeparam>
    /// <returns>A discriminated union that can be either `Done` or `Loop`.</returns>
    static virtual Fold<A, S> FoldStepBack<A, S>(K<T, A> ta, in S initialState) =>
        ta.BackwardIterator() is (Exist<A> head, var tail)
            ? L.Fold.Loop(initialState, head.Value, tail.FoldStep)
            : L.Fold.Done<A, S>(initialState);

    /// <summary>
    /// Same behaviour as `FoldBack` but allows early exit of the operation once
    /// the predicate function becomes `false` for the state/value pair 
    /// </summary>
    public static virtual S FoldBackWhile<A, S>(
        Func<S, A, S> f,
        Func<(S State, A Value), bool> predicate,
        in S initialState,
        K<T, A> ta)
    {
        var state = initialState;
        for (var i = T.BackwardIterator(ta); i is (Exist<A> (var value), var tail); i = tail)
        {
            if (predicate((state, value)))
            {
                state = f(state, value);
            }
            else
            {
                return state;
            }                    
        }
        return state;
    }

    /// <summary>
    /// Fold until the `Option` returns `None`
    /// </summary>
    /// <param name="f">Fold function</param>
    /// <param name="initialState">Initial state for the fold</param>
    /// <param name="ta">Foldable structure</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <typeparam name="S">State type</typeparam>
    /// <returns>Aggregated value</returns>
    static virtual S FoldBackMaybe<A, S>(
        Func<S, A, Option<S>> f,
        in S initialState,
        K<T, A> ta)
    {
        var state = initialState;
        for (var i = T.BackwardIterator(ta); i is (Exist<A> (var value), var tail); i = tail)
        {
            var option = f(state, value);
            if (option.IsSome)
            {
                state = (S)option;
            }
            else
            {
                return state;
            }                    
        }
        return state;
    }

    /// <summary>
    /// Same behaviour as `FoldBack` but the fold operation returns a monadic type and allows
    /// early exit of the operation once the predicate function becomes `false` for the
    /// state/value pair 
    /// </summary>
    static virtual MS FoldBackWhileM<MS, M, A, S>(
        Func<S, A, MS> f,
        Func<(S State, A Value), bool> predicate,
        in S initialState,
        K<T, A> ta)
        where M : Monad<M>
        where MS : K<M, S>
    {
        return (MS)Monad.recur((initialState, T.BackwardIterator(ta)), go);
        
        K<M, Next<(S State, Iterator<A> Iter), S>> go((S State, Iterator<A> Iter) input) =>
            input.Iter is (Exist<A> (var head), var tail) && predicate((input.State, head)) 
                ? f(input.State, head).Map(ns => Next.Loop<(S State, Iterator<A> Iter), S>((ns, tail)))
                : M.Pure(Next.Done<(S State, Iterator<A> Iter), S>(input.State));
    }

    /// <summary>
    /// Same behaviour as `FoldBack` but allows early exit of the operation once
    /// the predicate function becomes `false` for the state/value pair
    /// </summary>
    static virtual S FoldBackUntil<A, S>(
        Func<S, A, S> f, 
        Func<(S State, A Value), bool> predicate, 
        in S initialState, 
        K<T, A> ta)
    {
        var step = T.FoldStepBack(ta, initialState);
        while(true)
        {
            switch (step)
            {
                case Fold<A, S>.Done(var state):
                    return state;
                
                case Fold<A, S>.Loop(var state, var value, var next):
                    if (predicate((state, value)))
                    {
                        return state;
                    }
                    else
                    {
                        step = next(f(state, value));
                    }                    
                    break;

                default: 
                    throw new NotSupportedException();
            }
        }
    }

    /// <summary>
    /// Same behaviour as `FoldBack` but the fold operation returns a monadic type and allows
    /// early exit of the operation once the predicate function becomes `false` for the
    /// state/value pair 
    /// </summary>
    static virtual MS FoldBackUntilM<MS, M, A, S>(
        Func<S, A, MS> f, 
        Func<(S State, A Value), bool> predicate, 
        in S initialState, 
        K<T, A> ta)
        where M : Monad<M>
        where MS : K<M, S>
    {
        return (MS)Monad.recur((initialState, T.BackwardIterator(ta)), go);
        
        K<M, Next<(S State, Iterator<A> Iter), S>> go((S State, Iterator<A> Iter) input) =>
            input.Iter is (Exist<A> (var head), var tail)
                ? predicate((input.State, head))
                      ? M.Pure(Next.Done<(S State, Iterator<A> Iter), S>(input.State))
                      : f(input.State, head).Map(ns => Next.Loop<(S State, Iterator<A> Iter), S>((ns, tail)))
                : M.Pure(Next.Done<(S State, Iterator<A> Iter), S>(input.State));
    }

    /// <summary>
    /// Left-associative fold of a structure, lazy in the accumulator.  This
    /// is rarely what you want but can work well for structures with efficient
    /// right-to-left sequencing and an operator that is lazy in its left
    /// argumenTA.
    /// 
    /// In the case of lists, 'FoldLeft', when applied to a binary operator, a
    /// starting value (typically the left-identity of the operator), and a
    /// list, reduces the list using the binary operator, from left to right
    /// </summary>
    /// <remarks>
    /// Note that to produce the outermost application of the operator, the
    /// entire input list must be traversed.  Like all left-associative folds,
    /// `FoldBack` will diverge if given an infinite lisTA.
    /// </remarks>
    static virtual S FoldBack<A, S>(Func<S, A, S> f, in S initialState, K<T, A> ta)
    {
        var step = T.FoldStepBack(ta, initialState);
        while(true)
        {
            switch (step)
            {
                case Fold<A, S>.Done(var state):
                    return state;
                
                case Fold<A, S>.Loop(var state, var value, var next):
                    step = next(f(state, value));
                    break;

                default: 
                    throw new NotSupportedException();
            }
        }
    }

    /// <summary>
    /// Left-associative fold of a structure, lazy in the accumulator.  This
    /// is rarely what you want, but can work well for structures with efficient
    /// right-to-left sequencing and an operator that is lazy in its left
    /// argumenTA.
    /// 
    /// In the case of lists, 'FoldLeft', when applied to a binary operator, a
    /// starting value (typically the left-identity of the operator), and a
    /// list, reduces the list using the binary operator, from left to right
    /// </summary>
    /// <remarks>
    /// Note that to produce the outermost application of the operator the
    /// entire input list must be traversed.  Like all left-associative folds,
    /// `FoldBack` will diverge if given an infinite lisTA.
    /// </remarks>
    static virtual MS FoldBackM<MS, M, A, S>(
        Func<S, A, MS> f, 
        in S initialState, 
        K<T, A> ta)
        where M : Monad<M>
        where MS : K<M, S>
    {
        return (MS)Monad.recur((initialState, T.BackwardIterator(ta)), go);
        
        K<M, Next<(S State, Iterator<A> Iter), S>> go((S State, Iterator<A> Iter) input) =>
            input.Iter is (Exist<A> (var head), var tail)
                ? f(input.State, head).Map(ns => Next.Loop<(S State, Iterator<A> Iter), S>((ns, tail)))
                : M.Pure(Next.Done<(S State, Iterator<A> Iter), S>(input.State));
    }    
    
    /// <summary>
    /// List of elements of a structure, from left to right
    /// </summary>
    /// <remarks>
    /// The sequence is lazy
    /// </remarks>
    static virtual Seq<A> ToSeqBack<A>(K<T, A> ta)
    {
        return new Seq<A>(go(ta));

        static IEnumerable<A> go(K<T, A> ta)
        {
            var step = T.FoldStepBack(ta, unit);
            while (true)
            {
                switch (step)
                {
                    case Fold<A, Unit>.Done(_):
                        yield break;

                    case Fold<A, Unit>.Loop(_, var value, var next):
                        yield return value;
                        step = next(default);
                        break;

                    default:
                        throw new NotSupportedException();
                }
            }
        }
    }

    /// <summary>
    /// List of elements of a structure, from left to right
    /// </summary>
    static virtual Lst<A> ToLstBack<A>(K<T, A> ta)
    {
        return new Lst<A>(go(ta));

        static IEnumerable<A> go(K<T, A> ta)
        {
            var step = T.FoldStepBack(ta, unit);
            while (true)
            {
                switch (step)
                {
                    case Fold<A, Unit>.Done(_):
                        yield break;

                    case Fold<A, Unit>.Loop(_, var value, var next):
                        yield return value;
                        step = next(default);
                        break;

                    default:
                        throw new NotSupportedException();
                }
            }
        }
    }

    /// <summary>
    /// List of elements of a structure, from left to right
    /// </summary>
    static virtual Arr<A> ToArrBack<A>(K<T, A> ta)
    {
        return new Arr<A>(go(ta));
        IEnumerable<A> go(K<T, A> ta)
        {
            var step = T.FoldStepBack(ta, unit);
            while (true)
            {
                switch (step)
                {
                    case Fold<A, Unit>.Done(_):
                        yield break;

                    case Fold<A, Unit>.Loop(_, var value, var next):
                        yield return value;
                        step = next(default);
                        break;

                    default:
                        throw new NotSupportedException();
                }
            }
        }
    }

    /// <summary>
    /// List of elements of a structure, from left to right
    /// </summary>
    /// <remarks>
    /// The sequence is lazy
    /// </remarks>
    static virtual Iterable<A> ToIterableBack<A>(K<T, A> ta)
    {
        return go(ta).AsIterable();
        IEnumerable<A> go(K<T, A> ta)
        {
            var step = T.FoldStepBack(ta, unit);
            while (true)
            {
                switch (step)
                {
                    case Fold<A, Unit>.Done(_):
                        yield break;

                    case Fold<A, Unit>.Loop(_, var value, var next):
                        yield return value;
                        step = next(default);
                        break;

                    default:
                        throw new NotSupportedException();
                }
            }
        }
    }

    /// <summary>
    /// Does an element that fits the predicate occur in the structure?
    /// </summary>
    static virtual bool ExistsBack<A>(Func<A, bool> predicate, K<T, A> ta)
    {
        var step  = T.FoldStepBack(ta, unit);
        while (true)
        {
            switch (step)
            {
                case Fold<A, Unit>.Done(_):
                    return false;

                case Fold<A, Unit>.Loop(_, var value, var next):
                    if(predicate(value)) return true;
                    step = next(default);
                    break;

                default:
                    throw new NotSupportedException();
            }
        }
    }

    /// <summary>
    /// Does the predicate hold for all elements in the structure?
    /// </summary>
    static virtual bool ForAllBack<A>(Func<A, bool> predicate, K<T, A> ta)
    {
        var step  = T.FoldStepBack(ta, unit);
        while (true)
        {
            switch (step)
            {
                case Fold<A, Unit>.Done(_):
                    return true;

                case Fold<A, Unit>.Loop(_, var value, var next):
                    if(!predicate(value)) return false;
                    step = next(default);
                    break;

                default:
                    throw new NotSupportedException();
            }
        }
    }

    /// <summary>
    /// Does the element exist in the structure?
    /// </summary>
    static virtual bool ContainsBack<EqA, A>(A value, K<T, A> ta) 
        where EqA : Eq<A>
    {
        var step  = T.FoldStepBack(ta, unit);
        while (true)
        {
            switch (step)
            {
                case Fold<A, Unit>.Done(_):
                    return false;

                case Fold<A, Unit>.Loop(_, var x, var next):
                    if(EqA.Equals(value, x)) return true;
                    step = next(default);
                    break;

                default:
                    throw new NotSupportedException();
            }
        }
    }

    /// <summary>
    /// Does the element exist in the structure?
    /// </summary>
    static virtual bool ContainsBack<A>(A value, K<T, A> ta) =>
        T.ContainsBack<EqDefault<A>, A>(value, ta);
    
    /// <summary>
    /// Find the last element that match the predicate
    /// </summary>
    static virtual Option<A> FindBack<A>(Func<A, bool> predicate, K<T, A> ta)
    {
        var step  = T.FoldStepBack(ta, unit);
        while (true)
        {
            switch (step)
            {
                case Fold<A, Unit>.Done(_):
                    return default;

                case Fold<A, Unit>.Loop(_, var value, var next):
                    if(predicate(value)) return Some(value);
                    step = next(default);
                    break;

                default:
                    throw new NotSupportedException();
            }
        }
    }

    /// <summary>
    /// Find the elements that match the predicate
    /// </summary>
    /// <remarks>
    /// The sequence is lazy, but note, if the original foldable structure is lazy,
    /// then it will need to be consumed in its entirety before the values are yielded.
    /// </remarks>
    static virtual Iterable<A> FindAllBack<A>(Func<A, bool> predicate, K<T, A> ta)
    {
        return go(ta).AsIterable();
        IEnumerable<A> go(K<T, A> ta)
        {
            var step = T.FoldStepBack(ta, unit);
            while (true)
            {
                switch (step)
                {
                    case Fold<A, Unit>.Done(_):
                        yield break;

                    case Fold<A, Unit>.Loop(_, var value, var next):
                        if (predicate(value))
                        {
                            yield return value;
                        }
                        step = next(default);
                        break;

                    default:
                        throw new NotSupportedException();
                }
            }
        }
    }

    /// <summary>
    /// Get the last item in the foldable or `None`
    /// </summary>
    static virtual Option<A> Last<A>(K<T, A> ta)
    {
        var step = T.FoldStepBack(ta, unit);
        switch (step)
        {
            case Fold<A, Unit>.Done(_):
                return default;

            case Fold<A, Unit>.Loop(_, var value, _):
                return value;

            default:
                throw new NotSupportedException();
        }
    }
    
    /// <summary>
    /// Find the element at the specified index or `None` if out of range
    /// </summary>
    static virtual Option<A> At<A>(Index index, K<T, A> ta)
    {
        var step = index.IsFromEnd
                       ? T.FoldStepBack(ta, unit)
                       : T.FoldStep(ta, unit);

        var ix = 0;

        while (true)
        {
            switch (step)
            {
                case Fold<A, Unit>.Done(_):
                    return default;

                case Fold<A, Unit>.Loop(_, var value, _) when ix == index.Value:
                    return value;

                case Fold<A, Unit>.Loop(_, _, var next):
                    ix++;
                    step = next(default);
                    break;

                default:
                    throw new NotSupportedException();
            }
        }
    }

    /// <summary>
    /// Partition a foldable into two sequences based on a predicate
    /// </summary>
    /// <param name="f">Predicate function</param>
    /// <param name="ta">Foldable structure</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Partitioned structure</returns>
    static virtual (Seq<A> True, Seq<A> False) PartitionBack<A>(Func<A, bool> f, K<T, A> ta)
    {
        var step   = T.FoldStepBack(ta, unit);
        var @true  = Seq<A>();
        var @false = Seq<A>();
    
        while (true)
        {
            switch (step)
            {
                case Fold<A, Unit>.Done(_):
                    return (@true, @false);

                case Fold<A, Unit>.Loop(_, var value, var next):
                    if (f(value))
                    {
                        @true = @true.Add(value);
                    }
                    else
                    {
                        @false = @false.Add(value);
                    }
                    step = next(default);
                    break;

                default:
                    throw new NotSupportedException();
            }
        }
    }
}
