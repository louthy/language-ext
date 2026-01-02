using System;
using System.Collections.Generic;
using LanguageExt.ClassInstances;
using static LanguageExt.Prelude;

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
public interface Foldable<out T> where T : Foldable<T>
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
    static abstract Fold<A, S> FoldStep<A, S>(K<T, A> ta, in S initialState);
    
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
    static abstract Fold<A, S> FoldStepBack<A, S>(K<T, A> ta, in S initialState);    
    
    /// <summary>
    /// Same behaviour as `Fold` but allows early exit of the operation once
    /// the predicate function becomes `false` for the state/value pair 
    /// </summary>
    static virtual S FoldWhile<A, S>(
        Func<S, A, S> f,
        Func<(S State, A Value), bool> predicate,
        in S initialState,
        K<T, A> ta)
    {
        var step = T.FoldStep(ta, initialState);
        while(true)
        {
            switch (step)
            {
                case Fold<A, S>.Done(var state):
                    return state;
                
                case Fold<A, S>.Loop(var state, var value, var next):
                    if (predicate((state, value)))
                    {
                        step = next(f(state, value));
                    }
                    else
                    {
                        return state;
                    }                    
                    break;

                default: 
                    throw new NotSupportedException();
            }
        }
    }    

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
                        step = next(f(state, value));
                    }
                    else
                    {
                        return state;
                    }                    
                    break;

                default: 
                    throw new NotSupportedException();
            }
        }
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
    static virtual S FoldMaybe<A, S>(
        Func<S, A, Option<S>> f,
        in S initialState,
        K<T, A> ta)
    {
        var step = T.FoldStep(ta, initialState);
        while(true)
        {
            switch (step)
            {
                case Fold<A, S>.Done(var state):
                    return state;
                
                case Fold<A, S>.Loop(var state, var value, var next):
                    switch(f(state, value))
                    {
                        case { IsSome: true, Case: S state1}:
                            step = next(state1);
                            break;
                        
                        default:
                            return state;
                    }
                    break;

                default: 
                    throw new NotSupportedException();
            }
        }
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
        var step = T.FoldStepBack(ta, initialState);
        while(true)
        {
            switch (step)
            {
                case Fold<A, S>.Done(var state):
                    return state;
                
                case Fold<A, S>.Loop(var state, var value, var next):
                    switch(f(state, value))
                    {
                        case { IsSome: true, Case: S state1}:
                            step = next(state1);
                            break;
                        
                        default:
                            return state;
                    }
                    break;

                default: 
                    throw new NotSupportedException();
            }
        }
    }
    
    /// <summary>
    /// Same behaviour as `Fold` but the fold operation returns a monadic type and allows
    /// early exit of the operation once the predicate function becomes `false` for the
    /// state/value pair 
    /// </summary>
    static virtual MS FoldWhileM<MS, M, A, S>(
        Func<S, A, MS> f, 
        Func<(S State, A Value), bool> predicate, 
        in S initialState, 
        K<T, A> ta)
        where M : Monad<M>
        where MS : K<M, S>
    {
        var step = T.FoldStep(ta, initialState);
        return (MS)Monad.recur(step, go); 

        K<M, Next<Fold<A, S>, S>> go(Fold<A, S> nextStep)
        {
            switch (nextStep)
            {
                case Fold<A, S>.Done(var state):
                    return M.Pure(Next.Done<Fold<A, S>, S>(state));

                case Fold<A, S>.Loop(var state, var value, var next):
                    if (predicate((state, value)))
                    {
                        return f(state, value).Map(s => Next.Loop<Fold<A, S>, S>(next(s)));
                    }
                    else
                    {
                        return M.Pure(Next.Done<Fold<A, S>, S>(state));
                    }

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
    static virtual MS FoldBackWhileM<MS, M, A, S>(
        Func<S, A, MS> f, 
        Func<(S State, A Value), bool> predicate, 
        in S initialState, 
        K<T, A> ta)
        where M : Monad<M>
        where MS : K<M, S>
    {
        var step = T.FoldStepBack(ta, initialState);
        return (MS)Monad.recur(step, go); 

        K<M, Next<Fold<A, S>, S>> go(Fold<A, S> nextStep)
        {
            switch (nextStep)
            {
                case Fold<A, S>.Done(var state):
                    return M.Pure(Next.Done<Fold<A, S>, S>(state));

                case Fold<A, S>.Loop(var state, var value, var next):
                    if (predicate((state, value)))
                    {
                        return f(state, value).Map(s => Next.Loop<Fold<A, S>, S>(next(s)));
                    }
                    else
                    {
                        return M.Pure(Next.Done<Fold<A, S>, S>(state));
                    }

                default: 
                    throw new NotSupportedException();
            }
        }
    }

    /// <summary>
    /// Same behaviour as `Fold` but allows early exit of the operation once
    /// the predicate function becomes `false` for the state/value pair
    /// </summary>
    static virtual S FoldUntil<A, S>(
        Func<S, A, S> f,
        Func<(S State, A Value), bool> predicate,
        in S initialState,
        K<T, A> ta)
    {
        var step = T.FoldStep(ta, initialState);
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
    /// Same behaviour as `Fold` but the fold operation returns a monadic type and allows
    /// early exit of the operation once the predicate function becomes `false` for the
    /// state/value pair 
    /// </summary>
    static virtual MS FoldUntilM<MS, M, A, S>(
        Func<S, A, MS> f, 
        Func<(S State, A Value), bool> predicate, 
        in S initialState, 
        K<T, A> ta) 
        where M : Monad<M>
        where MS : K<M, S>
    {
        var step = T.FoldStep(ta, initialState);
        return (MS)Monad.recur(step, go); 

        K<M, Next<Fold<A, S>, S>> go(Fold<A, S> nextStep)
        {
            switch (nextStep)
            {
                case Fold<A, S>.Done(var state):
                    return M.Pure(Next.Done<Fold<A, S>, S>(state));

                case Fold<A, S>.Loop(var state, var value, var next):
                    if (predicate((state, value)))
                    {
                        return M.Pure(Next.Done<Fold<A, S>, S>(state));
                    }
                    else
                    {
                        return f(state, value).Map(s => Next.Loop<Fold<A, S>, S>(next(s)));
                    }

                default: 
                    throw new NotSupportedException();
            }
        }
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
        var step = T.FoldStepBack(ta, initialState);
        return (MS)Monad.recur(step, go); 

        K<M, Next<Fold<A, S>, S>> go(Fold<A, S> nextStep)
        {
            switch (nextStep)
            {
                case Fold<A, S>.Done(var state):
                    return M.Pure(Next.Done<Fold<A, S>, S>(state));

                case Fold<A, S>.Loop(var state, var value, var next):
                    if (predicate((state, value)))
                    {
                        return M.Pure(Next.Done<Fold<A, S>, S>(state));
                    }
                    else
                    {
                        return f(state, value).Map(s => Next.Loop<Fold<A, S>, S>(next(s)));
                    }

                default: 
                    throw new NotSupportedException();
            }
        }
    }

    /// <summary>
    /// Right-associative fold of a structure, lazy in the accumulator.
    ///
    /// In the case of lists, 'Fold', when applied to a binary operator, a
    /// starting value (typically the right-identity of the operator), and a
    /// list, reduces the list using the binary operator, from right to lefTA.
    /// </summary>
    static virtual S Fold<A, S>(Func<S, A, S> f, in S initialState, K<T, A> ta)
    {
        var step = T.FoldStep(ta, initialState);
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
        where M : Monad<M>
        where MS : K<M, S>
    {
        var step = T.FoldStep(ta, initialState);
        return (MS)Monad.recur(step, go);

        K<M, Next<Fold<A, S>, S>> go(Fold<A, S> nextStep)
        {
            switch (nextStep)
            {
                case Fold<A, S>.Done(var state):
                    return M.Pure(Next.Done<Fold<A, S>, S>(state));

                case Fold<A, S>.Loop(var state, var value, var next):
                    return f(state, value).Map(s => Next.Loop<Fold<A, S>, S>(next(s)));

                default:
                    throw new NotSupportedException();
            }
        }
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
        var step = T.FoldStepBack(ta, initialState);
        return (MS)Monad.recur(step, go); 

        K<M, Next<Fold<A, S>, S>> go(Fold<A, S> nextStep)
        {
            switch (nextStep)
            {
                case Fold<A, S>.Done(var state):
                    return M.Pure(Next.Done<Fold<A, S>, S>(state));

                case Fold<A, S>.Loop(var state, var value, var next):
                    return f(state, value).Map(s => Next.Loop<Fold<A, S>, S>(next(s)));

                default: 
                    throw new NotSupportedException();
            }
        }
    }
    
    /// <summary>
    /// List of elements of a structure, from left to right
    /// </summary>
    /// <remarks>
    /// The sequence is lazy
    /// </remarks>
    static virtual Seq<A> ToSeq<A>(K<T, A> ta)
    {
        return new Seq<A>(go(ta));

        static IEnumerable<A> go(K<T, A> ta)
        {
            var step = T.FoldStep(ta, unit);
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
    static virtual Lst<A> ToLst<A>(K<T, A> ta)
    {
        return new Lst<A>(go(ta));

        static IEnumerable<A> go(K<T, A> ta)
        {
            var step = T.FoldStep(ta, unit);
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
    static virtual Arr<A> ToArr<A>(K<T, A> ta)
    {
        return new Arr<A>(go(ta));

        IEnumerable<A> go(K<T, A> ta)
        {
            var step = T.FoldStep(ta, unit);
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
    static virtual Iterable<A> ToIterable<A>(K<T, A> ta)
    {
        return go(ta).AsIterable();

        IEnumerable<A> go(K<T, A> ta)
        {
            var step = T.FoldStep(ta, unit);
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
    static virtual bool IsEmpty<A>(K<T, A> ta)
    {
        var step = T.FoldStep(ta, unit);
        switch (step)
        {
            case Fold<A, Unit>.Loop(_, _, _):
                return false;

            default:
                return true;
        }
    }


    /// <summary>
    /// Returns the size/length of a finite structure as an `int`.  The
    /// default implementation just counts elements starting with the leftmosTA.
    /// 
    /// Instances for structures that can compute the element count faster
    /// than via element-by-element counting, should provide a specialised
    /// implementation.
    /// </summary>
    static virtual int Count<A>(K<T, A> ta)
    {
        var step  = T.FoldStep(ta, unit);
        var count = 0;
        while (true)
        {
            switch (step)
            {
                case Fold<A, Unit>.Done(_):
                    return count;

                case Fold<A, Unit>.Loop(_, _, var next):
                    count++;
                    step = next(default);
                    break;

                default:
                    throw new NotSupportedException();
            }
        }
    }

    /// <summary>
    /// Does an element that fits the predicate occur in the structure?
    /// </summary>
    static virtual bool Exists<A>(Func<A, bool> predicate, K<T, A> ta)
    {
        var step  = T.FoldStep(ta, unit);
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
    static virtual bool ForAll<A>(Func<A, bool> predicate, K<T, A> ta)
    {
        var step  = T.FoldStep(ta, unit);
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
    static virtual bool Contains<EqA, A>(A value, K<T, A> ta) 
        where EqA : Eq<A>
    {
        var step  = T.FoldStep(ta, unit);
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
    static virtual bool Contains<A>(A value, K<T, A> ta) =>
        T.Contains<EqDefault<A>, A>(value, ta);
    
    /// <summary>
    /// Find the first element that match the predicate
    /// </summary>
    static virtual Option<A> Find<A>(Func<A, bool> predicate, K<T, A> ta)
    {
        var step = T.FoldStep(ta, unit);
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
    /// The sequence is lazy
    /// </remarks>
    static virtual Iterable<A> FindAll<A>(Func<A, bool> predicate, K<T, A> ta)
    {
        return go(ta).AsIterable();
        IEnumerable<A> go(K<T, A> ta)
        {
            var step = T.FoldStep(ta, unit);
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
    /// Get the head item in the foldable or `None`
    /// </summary>
    static virtual Option<A> Head<A>(K<T, A> ta)
    {
        var step = T.FoldStep(ta, unit);
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
    /// Map each element of a structure to a monadic action, evaluate these
    /// actions from left to right, and ignore the results. 
    /// </summary>
    static virtual K<M, Unit> IterM<MB, M, A, B>(Func<A, MB> f, K<T, A> ta)
        where M : Monad<M>
        where MB : K<M, B>
    {
        var step = T.FoldStep(ta, unit);
        return Monad.recur(step, go); 

        K<M, Next<Fold<A, Unit>, Unit>> go(Fold<A, Unit> nextStep)
        {
            switch (nextStep)
            {
                case Fold<A, Unit>.Done(_):
                    return M.Pure(Next.Done<Fold<A, Unit>, Unit>(default));

                case Fold<A, Unit>.Loop(_, var value, var next):
                    return f(value).Map(_ => Next.Loop<Fold<A, Unit>, Unit>(next(default)));

                default: 
                    throw new NotSupportedException();
            }
        }
    }
    
    /// <summary>
    /// Map each element of a structure to an action, evaluate these
    /// actions from left to right, and ignore the results.  For a version that
    /// doesn't ignore the results see `Traversable.traverse`.
    /// </summary>
    static virtual Unit Iter<A>(Action<A> f, K<T, A> ta)
    {
        var step = T.FoldStep(ta, unit);
        
        while (true)
        {
            switch (step)
            {
                case Fold<A, Unit>.Done(_):
                    return default;

                case Fold<A, Unit>.Loop(_, var value, var next):
                    f(value);
                    step = next(default);
                    break;

                default:
                    throw new NotSupportedException();
            }
        }
    }

    
    /// <summary>
    /// Map each element of a structure to an action, evaluate these
    /// actions from left to right, and ignore the results.  For a version that
    /// doesn't ignore the results see `Traversable.traverse`.
    /// </summary>
    static virtual Unit Iter<A>(Action<int, A> f, K<T, A> ta)
    {
        var step = T.FoldStep(ta, unit);
        var ix   = 0;
        
        while (true)
        {
            switch (step)
            {
                case Fold<A, Unit>.Done(_):
                    return default;

                case Fold<A, Unit>.Loop(_, var value, var next):
                    f(ix, value);
                    step = next(default);
                    break;

                default:
                    throw new NotSupportedException();
            }
        }
    }

    /// <summary>
    /// Find the minimum value in the structure
    /// </summary>
    static virtual Option<A> Min<OrdA, A>(K<T, A> ta)
        where OrdA : Ord<A>
    {
        var step = T.FoldStep(ta, unit);
        A   current;
        
        switch (step)
        {
            case Fold<A, Unit>.Done(_):
                return default;

            case Fold<A, Unit>.Loop(_, var value, var next):
                current = value;
                step = next(default);
                break;

            default:
                throw new NotSupportedException();
        }

        while (true)
        {
            switch (step)
            {
                case Fold<A, Unit>.Done(_):
                    return current;

                case Fold<A, Unit>.Loop(_, var value, var next):
                    if (OrdA.Compare(value, current) < 0)
                    {
                        current = value;
                    }
                    step = next(default);
                    break;

                default:
                    throw new NotSupportedException();
            }
        }
    }

    /// <summary>
    /// Find the minimum value in the structure
    /// </summary>
    static virtual Option<A> Min<A>(K<T, A> ta) =>
        T.Min<OrdDefault<A>, A>(ta);
    
    /// <summary>
    /// Find the maximum value in the structure
    /// </summary>
    static virtual Option<A> Max<OrdA, A>(K<T, A> ta)
        where OrdA : Ord<A> 
    {
        var step = T.FoldStep(ta, unit);
        A   current;
        
        switch (step)
        {
            case Fold<A, Unit>.Done(_):
                return default;

            case Fold<A, Unit>.Loop(_, var value, var next):
                current = value;
                step = next(default);
                break;

            default:
                throw new NotSupportedException();
        }

        while (true)
        {
            switch (step)
            {
                case Fold<A, Unit>.Done(_):
                    return current;

                case Fold<A, Unit>.Loop(_, var value, var next):
                    if (OrdA.Compare(value, current) > 0)
                    {
                        current = value;
                    }
                    step = next(default);
                    break;

                default:
                    throw new NotSupportedException();
            }
        }
    }

    /// <summary>
    /// Find the maximum value in the structure
    /// </summary>
    static virtual Option<A> Max<A>(K<T, A> ta) =>
        T.Max<OrdDefault<A>, A>(ta);
    
    /// <summary>
    /// Find the minimum value in the structure
    /// </summary>
    static virtual A Min<OrdA, A>(A initialMin, K<T, A> ta) 
        where OrdA : Ord<A> => 
        T.Min<OrdA, A>(ta).IfNone(initialMin);

    /// <summary>
    /// Find the minimum value in the structure
    /// </summary>
    static virtual A Min<A>(A initialMin, K<T, A> ta) => 
        T.Min<OrdDefault<A>, A>(ta).IfNone(initialMin);

    /// <summary>
    /// Find the maximum value in the structure
    /// </summary>
    static virtual A Max<OrdA, A>(A initialMax, K<T, A> ta) 
        where OrdA : Ord<A> => 
        T.Min<OrdA, A>(ta).IfNone(initialMax);

    /// <summary>
    /// Find the maximum value in the structure
    /// </summary>
    static virtual A Max<A>(A initialMax, K<T, A> ta) =>
        T.Max<OrdDefault<A>, A>(ta).IfNone(initialMax);

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
    static virtual (Seq<A> True, Seq<A> False) Partition<A>(Func<A, bool> f, K<T, A> ta)
    {
        var step   = T.FoldStep(ta, unit);
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
