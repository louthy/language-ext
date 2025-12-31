using System;
using System.Collections.Generic;
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
    /// Runs a single step of the folding operation. The return value indicates whether the folding
    /// operation should continue, and if so, what the next step should be.
    /// </summary>
    /// <remarks>
    /// It is up to the consumer of
    /// this method to implement the actual state-aggregation (the folding) before passing it to the
    /// continuation function.  
    /// </remarks>
    /// <remarks>
    /// This differs from `FoldStepBack` in that it is a right-associative fold, whereas `FoldStepBack` is a
    /// left-associative fold.  If that's confusing, think that this works on a list in reverse.
    /// </remarks>
    /// <param name="ta">Foldable structure</param>
    /// <param name="initialState">Initial state value</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <typeparam name="S">State type</typeparam>
    /// <returns>A discriminated union that can be either `Done` or `Loop`.</returns>
    public static abstract Fold<A, S> FoldStep<A, S>(K<T, A> ta, S initialState);
    
    /// <summary>
    /// Runs a single step of the folding operation. The return value indicates whether the folding
    /// operation should continue, and if so, what the next step should be.
    /// </summary>
    /// <remarks>
    /// It is up to the consumer of
    /// this method to implement the actual state-aggregation (the folding) before passing it to the
    /// continuation function.  
    /// </remarks>
    /// <remarks>
    /// This differs from `FoldStep` in that it is a left-associative fold, whereas `FoldStepBack` is a
    /// right-associative fold.  If that's confusing, think that this works on a list in reverse.
    /// </remarks>
    /// <param name="ta">Foldable structure</param>
    /// <param name="initialState">Initial state value</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <typeparam name="S">State type</typeparam>
    /// <returns>A discriminated union that can be either `Done` or `Loop`.</returns>
    public static abstract Fold<A, S> FoldStepBack<A, S>(K<T, A> ta, S initialState);
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Default implementations
    //

    /// <summary>
    /// Same behaviour as `Fold` but allows early exit of the operation once
    /// the predicate function becomes `false` for the state/value pair 
    /// </summary>
    public static virtual S FoldWhile<A, S>(
        Func<A, Func<S, S>> f,
        Func<(S State, A Value), bool> predicate,
        S initialState,
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
                        step = next(f(value)(state));
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
        Func<S, Func<A, S>> f,
        Func<(S State, A Value), bool> predicate,
        S initialState,
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
                        step = next(f(state)(value));
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
    public static virtual S FoldMaybe<A, S>(
        Func<S, Func<A, Option<S>>> f,
        S initialState,
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
                    switch(f(state)(value))
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
    public static virtual S FoldBackMaybe<A, S>(
        Func<A, Func<S, Option<S>>> f,
        S initialState,
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
                    switch(f(value)(state))
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
    public static virtual K<M, S> FoldWhileM<A, M, S>(
        Func<A, Func<S, K<M, S>>> f, 
        Func<(S State, A Value), bool> predicate, 
        S initialState, 
        K<T, A> ta) 
        where M : Monad<M>
    {
        var step = T.FoldStep(ta, initialState);
        return Monad.recur(step, go); 

        K<M, Next<Fold<A, S>, S>> go(Fold<A, S> step)
        {
            switch (step)
            {
                case Fold<A, S>.Done(var state):
                    return M.Pure(Next.Done<Fold<A, S>, S>(state));

                case Fold<A, S>.Loop(var state, var value, var next):
                    if (predicate((state, value)))
                    {
                        return f(value)(state).Map(s => Next.Loop<Fold<A, S>, S>(next(s)));
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
    public static virtual K<M, S> FoldBackWhileM<A, M, S>(
        Func<S, Func<A, K<M, S>>> f, 
        Func<(S State, A Value), bool> predicate, 
        S initialState, 
        K<T, A> ta)
        where M : Monad<M>
    {
        var step = T.FoldStepBack(ta, initialState);
        return Monad.recur(step, go); 

        K<M, Next<Fold<A, S>, S>> go(Fold<A, S> step)
        {
            switch (step)
            {
                case Fold<A, S>.Done(var state):
                    return M.Pure(Next.Done<Fold<A, S>, S>(state));

                case Fold<A, S>.Loop(var state, var value, var next):
                    if (predicate((state, value)))
                    {
                        return f(state)(value).Map(s => Next.Loop<Fold<A, S>, S>(next(s)));
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
    public static virtual S FoldUntil<A, S>(
        Func<A, Func<S, S>> f,
        Func<(S State, A Value), bool> predicate,
        S initialState,
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
                        step = next(f(value)(state));
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
    public static virtual K<M, S> FoldUntilM<A, M, S>(
        Func<A, Func<S, K<M, S>>> f, 
        Func<(S State, A Value), bool> predicate, 
        S initialState, 
        K<T, A> ta) 
        where M : Monad<M>
    {
        var step = T.FoldStep(ta, initialState);
        return Monad.recur(step, go); 

        K<M, Next<Fold<A, S>, S>> go(Fold<A, S> step)
        {
            switch (step)
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
                        return f(value)(state).Map(s => Next.Loop<Fold<A, S>, S>(next(s)));
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
    public static virtual S FoldBackUntil<A, S>(
        Func<S, Func<A, S>> f, 
        Func<(S State, A Value), bool> predicate, 
        S initialState, 
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
                        step = next(f(state)(value));
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
    public static virtual K<M, S> FoldBackUntilM<A, M, S>(
        Func<S, Func<A, K<M, S>>> f, 
        Func<(S State, A Value), bool> predicate, 
        S initialState, 
        K<T, A> ta)
        where M : Monad<M>
    {
        var step = T.FoldStepBack(ta, initialState);
        return Monad.recur(step, go); 

        K<M, Next<Fold<A, S>, S>> go(Fold<A, S> step)
        {
            switch (step)
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
                        return f(state)(value).Map(s => Next.Loop<Fold<A, S>, S>(next(s)));
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
    /// list, reduces the list using the binary operator, from right to left.
    /// </summary>
    public static virtual S Fold<A, S>(Func<A, Func<S, S>> f, S initialState, K<T, A> ta)
    {
        var step = T.FoldStep(ta, initialState);
        while(true)
        {
            switch (step)
            {
                case Fold<A, S>.Done(var state):
                    return state;
                
                case Fold<A, S>.Loop(var state, var value, var next):
                    step = next(f(value)(state));
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
    /// list, reduces the list using the binary operator, from right to left.
    /// </summary>
    public static virtual K<M, S> FoldM<A, M, S>(
        Func<A, Func<S, K<M, S>>> f, 
        S initialState, 
        K<T, A> ta) 
        where M : Monad<M>
    {
        var step = T.FoldStep(ta, initialState);
        return Monad.recur(step, go); 

        K<M, Next<Fold<A, S>, S>> go(Fold<A, S> step)
        {
            switch (step)
            {
                case Fold<A, S>.Done(var state):
                    return M.Pure(Next.Done<Fold<A, S>, S>(state));

                case Fold<A, S>.Loop(var state, var value, var next):
                    return f(value)(state).Map(s => Next.Loop<Fold<A, S>, S>(next(s)));

                default: 
                    throw new NotSupportedException();
            }
        }
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
    /// `FoldBack` will diverge if given an infinite list.
    /// </remarks>
    public static virtual S FoldBack<A, S>(Func<S, Func<A, S>> f, S initialState, K<T, A> ta)
    {
        var step = T.FoldStepBack(ta, initialState);
        while(true)
        {
            switch (step)
            {
                case Fold<A, S>.Done(var state):
                    return state;
                
                case Fold<A, S>.Loop(var state, var value, var next):
                    step = next(f(state)(value));
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
    /// argument.
    /// 
    /// In the case of lists, 'FoldLeft', when applied to a binary operator, a
    /// starting value (typically the left-identity of the operator), and a
    /// list, reduces the list using the binary operator, from left to right
    /// </summary>
    /// <remarks>
    /// Note that to produce the outermost application of the operator the
    /// entire input list must be traversed.  Like all left-associative folds,
    /// `FoldBack` will diverge if given an infinite list.
    /// </remarks>
    public static virtual K<M, S> FoldBackM<A, M, S>(
        Func<S, Func<A, K<M, S>>> f, 
        S initialState, 
        K<T, A> ta)
        where M : Monad<M>
    {
        var step = T.FoldStepBack(ta, initialState);
        return Monad.recur(step, go); 

        K<M, Next<Fold<A, S>, S>> go(Fold<A, S> step)
        {
            switch (step)
            {
                case Fold<A, S>.Done(var state):
                    return M.Pure(Next.Done<Fold<A, S>, S>(state));

                case Fold<A, S>.Loop(var state, var value, var next):
                    return f(state)(value).Map(s => Next.Loop<Fold<A, S>, S>(next(s)));

                default: 
                    throw new NotSupportedException();
            }
        }
    }
    
    /// <summary>
    /// Given a structure with elements whose type is a `Monoid`, combine them
    /// via the monoid's `Combine` operator
    /// </summary>
    public static virtual A Fold<A>(K<T, A> ta) 
        where A : Monoid<A>
    {
        var step = T.FoldStep(ta, A.Empty);
        while(true)
        {
            switch (step)
            {
                case Fold<A, A>.Done(var state):
                    return state;
                
                case Fold<A, A>.Loop(var state, var value, var next):
                    step = next(state + value);
                    break;

                default: 
                    throw new NotSupportedException();
            }
        }
    }

    /// <summary>
    /// Given a structure with elements whose type is a `Monoid`, combine them
    /// via the monoid's `Combine` operator.
    /// </summary>
    public static virtual A FoldWhile<A>(Func<(A State, A Value), bool> predicate, K<T, A> ta) 
        where A : Monoid<A>
    {
        var step = T.FoldStep(ta, A.Empty);
        while(true)
        {
            switch (step)
            {
                case Fold<A, A>.Done(var state):
                    return state;
                
                case Fold<A, A>.Loop(var state, var value, var next):
                    if (predicate((state, value)))
                    {
                        step = next(state + value);
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
    /// Given a structure with elements whose type is a `Monoid`, combine them
    /// via the monoid's `Combine` operator.
    /// </summary>
    public static virtual A FoldUntil<A>(Func<(A State, A Value), bool> predicate, K<T, A> ta) 
        where A : Monoid<A> 
    {
        var step = T.FoldStep(ta, A.Empty);
        while(true)
        {
            switch (step)
            {
                case Fold<A, A>.Done(var state):
                    return state;
                
                case Fold<A, A>.Loop(var state, var value, var next):
                    if (predicate((state, value)))
                    {
                        return state;
                    }
                    else
                    {
                        step = next(state + value);
                    }
                    break;

                default: 
                    throw new NotSupportedException();
            }
        }
    }

    /// <summary>
    /// Map each element of the structure into a monoid, and combine the
    /// results with `Monoid.Combine`.  
    /// </summary>
    public static virtual B FoldMap<A, B>(Func<A, B> f, K<T, A> ta)
        where B : Monoid<B> 
    {
        var step = T.FoldStep(ta, B.Empty);
        while(true)
        {
            switch (step)
            {
                case Fold<A, B>.Done(var state):
                    return state;
                
                case Fold<A, B>.Loop(var state, var value, var next):
                    step = next(state + f(value));
                    break;

                default: 
                    throw new NotSupportedException();
            }
        }
    }

    /// <summary>
    /// Map each element of the structure into a monoid, and combine the
    /// results with `Combine`.  
    /// </summary>
    public static virtual B FoldMapWhile<A, B>(Func<A, B> f, Func<(B State, A Value), bool> predicate, K<T, A> ta)
        where B : Monoid<B> 
    {
        var step = T.FoldStep(ta, B.Empty);
        while(true)
        {
            switch (step)
            {
                case Fold<A, B>.Done(var state):
                    return state;
                
                case Fold<A, B>.Loop(var state, var value, var next):
                    if (predicate((state, value)))
                    {
                        step = next(state + f(value));
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
    /// Map each element of the structure into a monoid, and combine the
    /// results with `Append`.  This fold is right-associative and lazy in the
    /// accumulator.  For strict left-associative folds consider `FoldMapBack`
    /// instead.
    /// </summary>
    public static virtual B FoldMapUntil<A, B>(Func<A, B> f, Func<(B State, A Value), bool> predicate, K<T, A> ta)
        where B : Monoid<B>
    {
        var step = T.FoldStep(ta, B.Empty);
        while(true)
        {
            switch (step)
            {
                case Fold<A, B>.Done(var state):
                    return state;
                
                case Fold<A, B>.Loop(var state, var value, var next):
                    if (predicate((state, value)))
                    {
                        return state;
                    }
                    else
                    {
                        step = next(state + f(value));
                    }
                    break;

                default: 
                    throw new NotSupportedException();
            }
        }
    }

    /// <summary>
    /// A left-associative variant of 'FoldMap' that is strict in the
    /// accumulator.  Use this method for strict reduction when partial
    /// results are merged via `Append`.
    /// </summary>
    public static virtual B FoldMapBack<A, B>(Func<A, B> f, K<T, A> ta)
        where B : Monoid<B> 
    {
        var step = T.FoldStepBack(ta, B.Empty);
        while(true)
        {
            switch (step)
            {
                case Fold<A, B>.Done(var state):
                    return state;
                
                case Fold<A, B>.Loop(var state, var value, var next):
                    step = next(state + f(value));
                    break;

                default: 
                    throw new NotSupportedException();
            }
        }
    }

    /// <summary>
    /// A left-associative variant of 'FoldMap' that is strict in the
    /// accumulator.  Use this method for strict reduction when partial
    /// results are merged via `Append`.
    /// </summary>
    public static virtual B FoldMapWhileBack<A, B>(Func<A, B> f, Func<(B State, A Value), bool> predicate, K<T, A> ta)
        where B : Monoid<B> 
    {
        var step = T.FoldStepBack(ta, B.Empty);
        while(true)
        {
            switch (step)
            {
                case Fold<A, B>.Done(var state):
                    return state;
                
                case Fold<A, B>.Loop(var state, var value, var next):
                    if (predicate((state, value)))
                    {
                        step = next(state + f(value));
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
    /// A left-associative variant of 'FoldMap' that is strict in the
    /// accumulator.  Use this method for strict reduction when partial
    /// results are merged via `Append`.
    /// </summary>
    public static virtual B FoldMapUntilBack<A, B>(Func<A, B> f, Func<(B State, A Value), bool> predicate, K<T, A> ta)
        where B : Monoid<B>
    {
        var step = T.FoldStepBack(ta, B.Empty);
        while(true)
        {
            switch (step)
            {
                case Fold<A, B>.Done(var state):
                    return state;
                
                case Fold<A, B>.Loop(var state, var value, var next):
                    if (predicate((state, value)))
                    {
                        return state;
                    }
                    else
                    {
                        step = next(state + f(value));
                    }
                    break;

                default: 
                    throw new NotSupportedException();
            }
        }
    }
    
    /// <summary>
    /// List of elements of a structure, from left to right
    /// </summary>
    public static virtual Seq<A> ToSeq<A>(K<T, A> ta)
    {
        return new Seq<A>(go());

        IEnumerable<A> go()
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
    public static virtual Lst<A> ToLst<A>(K<T, A> ta)
    {
        return new Lst<A>(go());

        IEnumerable<A> go()
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
    public static virtual Arr<A> ToArr<A>(K<T, A> ta)
    {
        return new Arr<A>(go());

        IEnumerable<A> go()
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
    public static virtual Iterable<A> ToIterable<A>(K<T, A> ta)
    {
        return go().AsIterable();

        IEnumerable<A> go()
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
    public static virtual bool IsEmpty<A>(K<T, A> ta)
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
    /// default implementation just counts elements starting with the leftmost.
    /// 
    /// Instances for structures that can compute the element count faster
    /// than via element-by-element counting, should provide a specialised
    /// implementation.
    /// </summary>
    public static virtual int Count<A>(K<T, A> ta)
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
    public static virtual bool Exists<A>(Func<A, bool> predicate, K<T, A> ta)
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
    public static virtual bool ForAll<A>(Func<A, bool> predicate, K<T, A> ta)
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
    public static virtual bool Contains<EqA, A>(A value, K<T, A> ta) where EqA : Eq<A>
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
    public static virtual bool Contains<A>(A value, K<T, A> ta) 
    {
        var step  = T.FoldStep(ta, unit);
        while (true)
        {
            switch (step)
            {
                case Fold<A, Unit>.Done(_):
                    return false;

                case Fold<A, Unit>.Loop(_, var x, var next):
                    if(EqDefault<A>.Equals(value, x)) return true;
                    step = next(default);
                    break;

                default:
                    throw new NotSupportedException();
            }
        }
    }

    /// <summary>
    /// Find the first element that match the predicate
    /// </summary>
    public static virtual Option<A> Find<A>(Func<A, bool> predicate, K<T, A> ta)
    {
        var step  = T.FoldStep(ta, unit);
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
    public static virtual Option<A> FindBack<A>(Func<A, bool> predicate, K<T, A> ta)
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
    public static virtual Iterable<A> FindAll<A>(Func<A, bool> predicate, K<T, A> ta)
    {
        return go().AsIterable();
        IEnumerable<A> go()
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
    public static virtual Iterable<A> FindAllBack<A>(Func<A, bool> predicate, K<T, A> ta)
    {
        return go().AsIterable();
        IEnumerable<A> go()
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
    /// Computes the sum of the numbers of a structure.
    /// </summary>
    public static virtual A Sum<A>(K<T, A> ta) 
        where A : IAdditionOperators<A, A, A>, IAdditiveIdentity<A, A>
    {
        var step = T.FoldStep(ta, A.AdditiveIdentity);
        
        while (true)
        {
            switch (step)
            {
                case Fold<A, A>.Done(var state):
                    return state;

                case Fold<A, A>.Loop(var state, var value, var next):
                   step = next(state + value);
                    break;

                default:
                    throw new NotSupportedException();
            }
        }
    }
    
    /// <summary>
    /// Computes the product of the numbers of a structure.
    /// </summary>
    public static virtual A Product<A>(K<T, A> ta) 
        where A : IMultiplyOperators<A, A, A>, IMultiplicativeIdentity<A, A>
    {
        var step = T.FoldStep(ta, A.MultiplicativeIdentity);
        
        while (true)
        {
            switch (step)
            {
                case Fold<A, A>.Done(var state):
                    return state;

                case Fold<A, A>.Loop(var state, var value, var next):
                    step = next(state * value);
                    break;

                default:
                    throw new NotSupportedException();
            }
        }
    }

    /// <summary>
    /// Get the head item in the foldable or `None`
    /// </summary>
    public static virtual Option<A> Head<A>(K<T, A> ta)
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
    public static virtual Option<A> Last<A>(K<T, A> ta)
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
    public static virtual K<M, Unit> Iter<M, A, B>(Func<A, K<M, B>> f, K<T, A> ta) 
        where M : Monad<M>
    {
        var step = T.FoldStep(ta, unit);
        return Monad.recur(step, go); 

        K<M, Next<Fold<A, Unit>, Unit>> go(Fold<A, Unit> step)
        {
            switch (step)
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
    public static virtual Unit Iter<A>(Action<A> f, K<T, A> ta)
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
    public static virtual Unit Iter<A>(Action<int, A> f, K<T, A> ta)
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
    public static virtual Option<A> Min<OrdA, A>(K<T, A> ta)
        where OrdA : Ord<A>
    {
        var step = T.FoldStep(ta, unit);
        A current;
        
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
    public static virtual Option<A> Min<A>(K<T, A> ta)
        where A : IComparable<A> 
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
                    if (value.CompareTo(current) < 0)
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
    public static virtual Option<A> Max<OrdA, A>(K<T, A> ta)
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
    public static virtual Option<A> Max<A>(K<T, A> ta)
        where A : IComparable<A> 
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
                    if (value.CompareTo(current) > 0)
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
    public static virtual A Min<OrdA, A>(K<T, A> ta, A initialMin)
        where OrdA : Ord<A>
    {
        var step    = T.FoldStep(ta, unit);
        var current = initialMin;
        
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
    public static virtual A Min<A>(K<T, A> ta, A initialMin)
        where A : IComparable<A> 
    {
        var step    = T.FoldStep(ta, unit);
        var current = initialMin;

        while (true)
        {
            switch (step)
            {
                case Fold<A, Unit>.Done(_):
                    return current;

                case Fold<A, Unit>.Loop(_, var value, var next):
                    if (value.CompareTo(current) < 0)
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
    public static virtual A Max<OrdA, A>(K<T, A> ta, A initialMax)
        where OrdA : Ord<A> 
    {
        var step    = T.FoldStep(ta, unit);
        var current = initialMax;
        
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
    public static virtual A Max<A>(K<T, A> ta, A initialMax)
        where A : IComparable<A> 
    {
        var step    = T.FoldStep(ta, unit);
        var current = initialMax;

        while (true)
        {
            switch (step)
            {
                case Fold<A, Unit>.Done(_):
                    return current;

                case Fold<A, Unit>.Loop(_, var value, var next):
                    if (value.CompareTo(current) > 0)
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
    /// Find the average of all the values in the structure
    /// </summary>
    public static virtual A Average<A>(K<T, A> ta)
        where A : INumber<A>
    {
        var step  = T.FoldStep(ta, unit);
        var taken = A.Zero;
        var total = A.Zero;
        
        while (true)
        {
            switch (step)
            {
                case Fold<A, Unit>.Done(_):
                    return taken == A.Zero
                                ? A.Zero
                                : total / taken;

                case Fold<A, Unit>.Loop(_, var value, var next):
                    taken += A.One;
                    total += value;
                    step = next(default);
                    break;

                default:
                    throw new NotSupportedException();
            }
        }
    }

    /// <summary>
    /// Find the average of all the values in the structure
    /// </summary>
    public static virtual B Average<A, B>(Func<A, B> f, K<T, A> ta)
        where B : INumber<B>
    {
        var step  = T.FoldStep(ta, unit);
        var taken = B.Zero;
        var total = B.Zero;
        
        while (true)
        {
            switch (step)
            {
                case Fold<A, Unit>.Done(_):
                    return taken == B.Zero
                               ? B.Zero
                               : total / taken;

                case Fold<A, Unit>.Loop(_, var value, var next):
                    taken += B.One;
                    total += f(value);
                    step = next(default);
                    break;

                default:
                    throw new NotSupportedException();
            }
        }
    }

    /// <summary>
    /// Find the element at the specified index or `None` if out of range
    /// </summary>
    public static virtual Option<A> At<A>(K<T, A> ta, Index index)
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
    public static virtual (Seq<A> True, Seq<A> False) Partition<A>(Func<A, bool> f, K<T, A> ta)
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
