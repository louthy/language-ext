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
/// <typeparam name="T">This foldable type</typeparam>
/// <typeparam name="FS">Folding state type.  Used to hold state for the duration of a fold</typeparam>
public interface Foldable<out T, FS> : Foldable<T> 
    where T : Foldable<T, FS>
    where FS : allows ref struct
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Abstract members
    //

    public static abstract bool FoldStep<TA, A>(in TA ta, ref FS refState, out A value) 
        where TA : K<T, A>;
    
    public static abstract void FoldStepInit<TA, A>(in TA ta, ref FS refState) 
        where TA : K<T, A>;

    public static abstract void FoldStepBackInit<TA, A>(in TA ta, ref FS refState) 
        where TA : K<T, A>;
    
    public static abstract bool FoldStepBack<TA, A>(in TA ta, ref FS refState, out A value) 
        where TA : K<T, A>;
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Default implementations
    //

    /// <summary>
    /// Same behaviour as `Fold` but allows early exit of the operation once
    /// the predicate function becomes `false` for the state/value pair 
    /// </summary>
    static S Foldable<T>.FoldWhile<TA, A, S>(
        Func<S, A, S> f,
        Func<(S State, A Value), bool> predicate,
        in S initialState,
        in TA ta)
    {
        FS foldState = default!;
        T.FoldStepInit<TA, A>(ta, ref foldState);
        var state = initialState;
        
        while (T.FoldStep<TA, A>(ta, ref foldState, out var value))
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
    /// Same behaviour as `FoldBack` but allows early exit of the operation once
    /// the predicate function becomes `false` for the state/value pair 
    /// </summary>
    static S Foldable<T>.FoldBackWhile<TA, A, S>(
        Func<S, A, S> f,
        Func<(S State, A Value), bool> predicate,
        in S initialState,
        in TA ta)
    {
        FS foldState = default!;
        T.FoldStepBackInit<TA, A>(ta, ref foldState);
        var state = initialState;
        
        while (T.FoldStepBack<TA, A>(ta, ref foldState, out var value))
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
    static S Foldable<T>.FoldMaybe<TA, A, S>(
        Func<S, A, Option<S>> f,
        in S initialState,
        in TA ta)
    {
        FS foldState = default!;
        T.FoldStepInit<TA, A>(ta, ref foldState);
        var state = initialState;
        
        while (T.FoldStep<TA, A>(ta, ref foldState, out var value))
        {
            var option = f(initialState, value);
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
    /// Fold until the `Option` returns `None`
    /// </summary>
    /// <param name="f">Fold function</param>
    /// <param name="initialState">Initial state for the fold</param>
    /// <param name="ta">Foldable structure</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <typeparam name="S">State type</typeparam>
    /// <returns>Aggregated value</returns>
    static S Foldable<T>.FoldBackMaybe<TA, A, S>(
        Func<S, A, Option<S>> f,
        in S initialState,
        in TA ta)
    {
        FS foldState = default!;
        T.FoldStepBackInit<TA, A>(ta, ref foldState);
        var state = initialState;
        
        while (T.FoldStepBack<TA, A>(ta, ref foldState, out var value))
        {
            var option = f(initialState, value);
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
    /// Same behaviour as `Fold` but the fold operation returns a monadic type and allows
    /// early exit of the operation once the predicate function becomes `false` for the
    /// state/value pair 
    /// </summary>
    static MS Foldable<T>.FoldWhileM<TA, MS, A, M, S>(
        Func<S, A, MS> f, 
        Func<(S State, A Value), bool> predicate, 
        in S initialState, 
        in TA ta)
    {
        var step = T.FoldStep<TA, A, S>(ta, initialState);
        return (MS)Monad.recur(step, go); 

        K<M, Next<Fold<A, S>, S>> go(Fold<A, S> step)
        {
            switch (step)
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
    static MS Foldable<T>.FoldBackWhileM<TA, MS, A, M, S>(
        Func<S, A, MS> f, 
        Func<(S State, A Value), bool> predicate, 
        in S initialState, 
        in TA ta)
    {
        var step = T.FoldStepBack<TA, A, S>(ta, initialState);
        return (MS)Monad.recur(step, go); 

        K<M, Next<Fold<A, S>, S>> go(Fold<A, S> step)
        {
            switch (step)
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
    static S Foldable<T>.FoldUntil<TA, A, S>(
        Func<S, A, S> f,
        Func<(S State, A Value), bool> predicate,
        in S initialState,
        in TA ta)
    {
        FS foldState = default!;
        T.FoldStepInit<TA, A>(ta, ref foldState);
        var state = initialState;
        
        while (T.FoldStep<TA, A>(ta, ref foldState, out var value))
        {
            if (predicate((state, value)))
            {
                return state;
            }
            else
            {
                state = f(state, value);
            }
        }
        return state;
    }
    
    /// <summary>
    /// Same behaviour as `Fold` but the fold operation returns a monadic type and allows
    /// early exit of the operation once the predicate function becomes `false` for the
    /// state/value pair 
    /// </summary>
    static MS Foldable<T>.FoldUntilM<TA, MS, A, M, S>(
        Func<S, A, MS> f, 
        Func<(S State, A Value), bool> predicate, 
        in S initialState, 
        in TA ta) 
    {
        var step = T.FoldStep<TA, A, S>(ta, initialState);
        return (MS)Monad.recur(step, go); 

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
    static S Foldable<T>.FoldBackUntil<TA, A, S>(
        Func<S, A, S> f, 
        Func<(S State, A Value), bool> predicate, 
        in S initialState, 
        in TA ta)
    {
        FS foldState = default!;
        T.FoldStepBackInit<TA, A>(ta, ref foldState);
        var state = initialState;
        
        while (T.FoldStepBack<TA, A>(ta, ref foldState, out var value))
        {
            if (predicate((state, value)))
            {
                return state;
            }
            else
            {
                state = f(state, value);
            }
        }
        return state;
    }

    /// <summary>
    /// Same behaviour as `FoldBack` but the fold operation returns a monadic type and allows
    /// early exit of the operation once the predicate function becomes `false` for the
    /// state/value pair 
    /// </summary>
    static MS Foldable<T>.FoldBackUntilM<TA, MS, A, M, S>(
        Func<S, A, MS> f, 
        Func<(S State, A Value), bool> predicate, 
        in S initialState, 
        in TA ta)
    {
        var step = T.FoldStepBack<TA, A, S>(ta, initialState);
        return (MS)Monad.recur(step, go); 

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
    /// list, reduces the list using the binary operator, from right to left.
    /// </summary>
    static S Foldable<T>.Fold<TA, A, S>(Func<S, A, S> f, in S initialState, in TA ta)
    {
        FS foldState = default!;
        T.FoldStepInit<TA, A>(ta, ref foldState);
        var state = initialState;
        while (T.FoldStep<TA, A>(ta, ref foldState, out var value))
        {
            state = f(state, value);
        }
        return state;
    }
    
    /// <summary>
    /// Right-associative fold of a structure, lazy in the accumulator.
    ///
    /// In the case of lists, 'Fold', when applied to a binary operator, a
    /// starting value (typically the right-identity of the operator), and a
    /// list, reduces the list using the binary operator, from right to left.
    /// </summary>
    static MS Foldable<T>.FoldM<TA, MS, A, M, S>(
        Func<S, A, MS> f, 
        in S initialState, 
        in TA ta) 
    {
        var step = T.FoldStep<TA, A, S>(ta, initialState);
        return (MS)Monad.recur(step, go); 

        K<M, Next<Fold<A, S>, S>> go(Fold<A, S> step)
        {
            switch (step)
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
    /// argument.
    /// 
    /// In the case of lists, 'FoldLeft', when applied to a binary operator, a
    /// starting value (typically the left-identity of the operator), and a
    /// list, reduces the list using the binary operator, from left to right
    /// </summary>
    /// <remarks>
    /// Note that to produce the outermost application of the operator, the
    /// entire input list must be traversed.  Like all left-associative folds,
    /// `FoldBack` will diverge if given an infinite list.
    /// </remarks>
    static S Foldable<T>.FoldBack<TA, A, S>(Func<S, A, S> f, in S initialState, in TA ta)
    {
        FS foldState = default!;
        T.FoldStepBackInit<TA, A>(ta, ref foldState);
        var state = initialState;
        while (T.FoldStepBack<TA, A>(ta, ref foldState, out var value))
        {
            state = f(state, value);
        }
        return state;
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
    static MS Foldable<T>.FoldBackM<TA, MS, A, M, S>(
        Func<S, A, MS> f, 
        in S initialState, 
        in TA ta)
    {
        var step = T.FoldStepBack<TA, A, S>(ta, initialState);
        return (MS)Monad.recur(step, go); 

        K<M, Next<Fold<A, S>, S>> go(Fold<A, S> step)
        {
            switch (step)
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
    /// Given a structure with elements whose type is a `Monoid`, combine them
    /// via the monoid's `Combine` operator
    /// </summary>
    static A Foldable<T>.Fold<TA, A>(in TA ta) 
    {
        FS foldState = default!;
        T.FoldStepInit<TA, A>(ta, ref foldState);
        var state = A.Empty;
        while (T.FoldStep<TA, A>(ta, ref foldState, out var value))
        {
            state += value;
        }
        return state;
    }

    /// <summary>
    /// Given a structure with elements whose type is a `Monoid`, combine them
    /// via the monoid's `Combine` operator.
    /// </summary>
    static A Foldable<T>.FoldWhile<TA, A>(Func<(A State, A Value), bool> predicate, in TA ta) 
    {
        FS foldState = default!;
        T.FoldStepInit<TA, A>(ta, ref foldState);
        var state = A.Empty;
        while (T.FoldStep<TA, A>(ta, ref foldState, out var value))
        {
            if (predicate((state, value)))
            {
                state += value;
            }
            else
            {
                return state;
            }
        }
        return state;
    }

    /// <summary>
    /// Given a structure with elements whose type is a `Monoid`, combine them
    /// via the monoid's `Combine` operator.
    /// </summary>
    static A Foldable<T>.FoldUntil<TA, A>(Func<(A State, A Value), bool> predicate, in TA ta) 
    {
        FS foldState = default!;
        T.FoldStepInit<TA, A>(ta, ref foldState);
        var state = A.Empty;
        while (T.FoldStep<TA, A>(ta, ref foldState, out var value))
        {
            if (predicate((state, value)))
            {
                return state;
            }
            else
            {
                state += value;
            }
        }
        return state;
    }

    /// <summary>
    /// Map each element of the structure into a monoid, and combine the
    /// results with `Monoid.Combine`.  
    /// </summary>
    static B Foldable<T>.FoldMap<TA, A, B>(Func<A, B> f, in TA ta)
    {
        FS foldState = default!;
        T.FoldStepInit<TA, A>(ta, ref foldState);
        var state = B.Empty;
        while (T.FoldStep<TA, A>(ta, ref foldState, out var value))
        {
            state += f(value);
        }
        return state;
    }

    /// <summary>
    /// Map each element of the structure into a monoid, and combine the
    /// results with `Combine`.  
    /// </summary>
    static B Foldable<T>.FoldMapWhile<TA, A, B>(Func<A, B> f, Func<(B State, A Value), bool> predicate, in TA ta)
    {
        FS foldState = default!;
        T.FoldStepInit<TA, A>(ta, ref foldState);
        var state = B.Empty;
        while (T.FoldStep<TA, A>(ta, ref foldState, out var value))
        {
            if (predicate((state, value)))
            {
                state += f(value);
            }
            else
            {
                return state;
            }
        }
        return state;
    }

    /// <summary>
    /// Map each element of the structure into a monoid, and combine the
    /// results with `Append`.  This fold is right-associative and lazy in the
    /// accumulator.  For strict left-associative folds consider `FoldMapBack`
    /// instead.
    /// </summary>
    static B Foldable<T>.FoldMapUntil<TA, A, B>(Func<A, B> f, Func<(B State, A Value), bool> predicate, in TA ta)
    {
        FS foldState = default!;
        T.FoldStepInit<TA, A>(ta, ref foldState);
        var state = B.Empty;
        while (T.FoldStep<TA, A>(ta, ref foldState, out var value))
        {
            if (predicate((state, value)))
            {
                return state;
            }
            else
            {
                state += f(value);
            }
        }
        return state;
    }

    /// <summary>
    /// A left-associative variant of 'FoldMap' that is strict in the
    /// accumulator.  Use this method for strict reduction when partial
    /// results are merged via `Append`.
    /// </summary>
    static B Foldable<T>.FoldMapBack<TA, A, B>(Func<A, B> f, in TA ta)
    {
        FS foldState = default!;
        T.FoldStepBackInit<TA, A>(ta, ref foldState);
        var state = B.Empty;
        while (T.FoldStepBack<TA, A>(ta, ref foldState, out var value))
        {
            state += f(value);
        }
        return state;
    }

    /// <summary>
    /// A left-associative variant of 'FoldMap' that is strict in the
    /// accumulator.  Use this method for strict reduction when partial
    /// results are merged via `Append`.
    /// </summary>
    static B Foldable<T>.FoldMapWhileBack<TA, A, B>(Func<A, B> f, Func<(B State, A Value), bool> predicate, in TA ta)
    {
        FS foldState = default!;
        T.FoldStepBackInit<TA, A>(ta, ref foldState);
        var state = B.Empty;
        while (T.FoldStepBack<TA, A>(ta, ref foldState, out var value))
        {
            if (predicate((state, value)))
            {
                state += f(value);
            }
            else
            {
                return state;
            }
        }
        return state;
    }
    
    /// <summary>
    /// A left-associative variant of 'FoldMap' that is strict in the
    /// accumulator.  Use this method for strict reduction when partial
    /// results are merged via `Append`.
    /// </summary>
    static B Foldable<T>.FoldMapUntilBack<TA, A, B>(Func<A, B> f, Func<(B State, A Value), bool> predicate, in TA ta)
    {
        FS foldState = default!;
        T.FoldStepBackInit<TA, A>(ta, ref foldState);
        var state = B.Empty;
        while (T.FoldStepBack<TA, A>(ta, ref foldState, out var value))
        {
            if (predicate((state, value)))
            {
                return state;
            }
            else
            {
                state += f(value);
            }
        }
        return state;
    }

    
    /// <summary>
    /// List of elements of a structure, from left to right
    /// </summary>
    /// <remarks>
    /// The sequence is lazy
    /// </remarks>
    static Seq<A> Foldable<T>.ToSeq<TA, A>(in TA ta)
    {
        return new Seq<A>(go(ta));

        IEnumerable<A> go(TA ta)
        {
            var step = T.FoldStep<TA, A, Unit>(ta, unit);
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
    static Lst<A> Foldable<T>.ToLst<TA, A>(in TA ta) =>
        Lst<A>.FromFoldable<TA, T, FS>(ta);

    /// <summary>
    /// List of elements of a structure, from left to right
    /// </summary>
    static Arr<A> Foldable<T>.ToArr<TA, A>(in TA ta)
    {
        var buffer = new A[32];
        var max    = buffer.Length;
        var length = 0;
        
        FS foldState = default!;
        T.FoldStepInit<TA, A>(ta, ref foldState);
        while (T.FoldStep<TA, A>(ta, ref foldState, out var value))
        {
            if (length == max)
            {
                max <<= 1;
                var newBuffer = new A[max];
                System.Array.Copy(buffer, 0, newBuffer, 0, length);
                buffer = newBuffer;
            }
            buffer[length++] = value;
        }
        return new Arr<A>(buffer, 0, length);
    }

    /// <summary>
    /// List of elements of a structure, from left to right
    /// </summary>
    /// <remarks>
    /// The sequence is lazy
    /// </remarks>
    static Iterable<A> Foldable<T>.ToIterable<TA, A>(in TA ta)
    {
        return go(ta).AsIterable();

        IEnumerable<A> go(TA ta)
        {
            var step = T.FoldStep<TA, A, Unit>(ta, unit);
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
    static bool Foldable<T>.IsEmpty<TA, A>(in TA ta)
    {
        FS foldState = default!;
        T.FoldStepInit<TA, A>(ta, ref foldState);
        return !T.FoldStep<TA, A>(ta, ref foldState, out _);
    }

    /// <summary>
    /// Returns the size/length of a finite structure as an `int`.  The
    /// default implementation just counts elements starting with the leftmost.
    /// 
    /// Instances for structures that can compute the element count faster
    /// than via element-by-element counting, should provide a specialised
    /// implementation.
    /// </summary>
    static int Foldable<T>.Count<TA, A>(in TA ta)
    {
        FS foldState = default!;
        T.FoldStepInit<TA, A>(ta, ref foldState);
        var state = 0;
        while (T.FoldStep<TA, A>(ta, ref foldState, out _))
        {
            state++;
        }
        return state;
    }

    /// <summary>
    /// Does an element that fits the predicate occur in the structure?
    /// </summary>
    static bool Foldable<T>.Exists<TA, A>(Func<A, bool> predicate, in TA ta)
    {
        FS foldState = default!;
        T.FoldStepInit<TA, A>(ta, ref foldState);
        while (T.FoldStep<TA, A>(ta, ref foldState, out var value))
        {
            if(predicate(value)) return true;
        }
        return false;
    }

    /// <summary>
    /// Does the predicate hold for all elements in the structure?
    /// </summary>
    static bool Foldable<T>.ForAll<TA, A>(Func<A, bool> predicate, in TA ta)
    {
        FS foldState = default!;
        T.FoldStepInit<TA, A>(ta, ref foldState);
        while (T.FoldStep<TA, A>(ta, ref foldState, out var value))
        {
            if(!predicate(value)) return false;
        }
        return true;
    }

    /// <summary>
    /// Does the element exist in the structure?
    /// </summary>
    static bool Foldable<T>.Contains<TA, EqA, A>(A value, in TA ta) 
    {
        FS foldState = default!;
        T.FoldStepInit<TA, A>(ta, ref foldState);
        while (T.FoldStep<TA, A>(ta, ref foldState, out var v))
        {
            if(EqA.Equals(value, v)) return true;
        }
        return false;
    }

    /// <summary>
    /// Does the element exist in the structure?
    /// </summary>
    static bool Foldable<T>.Contains<TA, A>(A value, in TA ta) 
    {
        FS foldState = default!;
        T.FoldStepInit<TA, A>(ta, ref foldState);
        while (T.FoldStep<TA, A>(ta, ref foldState, out var v))
        {
            if(EqualityComparer<A>.Default.Equals(value, v)) return true;
        }
        return false;
    }

    /// <summary>
    /// Find the first element that match the predicate
    /// </summary>
    static Option<A> Foldable<T>.Find<TA, A>(Func<A, bool> predicate, in TA ta)
    {
        FS foldState = default!;
        T.FoldStepInit<TA, A>(ta, ref foldState);
        while (T.FoldStep<TA, A>(ta, ref foldState, out var value))
        {
            if(predicate(value)) return value;
        }
        return default;
    }

    /// <summary>
    /// Find the last element that match the predicate
    /// </summary>
    static Option<A> Foldable<T>.FindBack<TA, A>(Func<A, bool> predicate, in TA ta)
    {
        FS foldState = default!;
        T.FoldStepBackInit<TA, A>(ta, ref foldState);
        while (T.FoldStepBack<TA, A>(ta, ref foldState, out var value))
        {
            if(predicate(value)) return value;
        }
        return default;
    }

    /// <summary>
    /// Find the elements that match the predicate
    /// </summary>
    /// <remarks>
    /// The sequence is lazy
    /// </remarks>
    static Iterable<A> Foldable<T>.FindAll<TA, A>(Func<A, bool> predicate, in TA ta)
    {
        return go(ta).AsIterable();
        IEnumerable<A> go(TA ta)
        {
            var step = T.FoldStep<TA, A, Unit>(ta, unit);
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
    static Iterable<A> Foldable<T>.FindAllBack<TA, A>(Func<A, bool> predicate, in TA ta)
    {
        return go(ta).AsIterable();
        IEnumerable<A> go(TA ta)
        {
            var step = T.FoldStepBack<TA, A, Unit>(ta, unit);
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
    static A Foldable<T>.Sum<TA, A>(in TA ta)
    {
        FS foldState = default!;
        T.FoldStepInit<TA, A>(ta, ref foldState);
        var state = A.AdditiveIdentity;
        while (T.FoldStep<TA, A>(ta, ref foldState, out var value))
        {
            state += value;
        }
        return state;
    }

    /// <summary>
    /// Computes the product of the numbers of a structure.
    /// </summary>
    static A Foldable<T>.Product<TA, A>(in TA ta) 
    {
        FS foldState = default!;
        T.FoldStepInit<TA, A>(ta, ref foldState);
        var state = A.MultiplicativeIdentity;
        while (T.FoldStep<TA, A>(ta, ref foldState, out var value))
        {
            state *= value;
        }
        return state;
    }

    /// <summary>
    /// Get the head item in the foldable or `None`
    /// </summary>
    static Option<A> Foldable<T>.Head<TA, A>(in TA ta)
    {
        FS foldState = default!;
        T.FoldStepInit<TA, A>(ta, ref foldState);
        if (T.FoldStep<TA, A>(ta, ref foldState, out var value))
        {
            return value;
        }
        else
        {
            return default;
        }
    }

    /// <summary>
    /// Get the last item in the foldable or `None`
    /// </summary>
    static Option<A> Foldable<T>.Last<TA, A>(in TA ta)
    {
        FS foldState = default!;
        T.FoldStepBackInit<TA, A>(ta, ref foldState);
        if (T.FoldStepBack<TA, A>(ta, ref foldState, out var value))
        {
            return value;
        }
        else
        {
            return default;
        }
    }
    
    /// <summary>
    /// Map each element of a structure to a monadic action, evaluate these
    /// actions from left to right, and ignore the results. 
    /// </summary>
    static MU Foldable<T>.IterM<TA, MB, MU, M, A, B>(Func<A, MB> f, in TA ta)
    {
        var step = T.FoldStep<TA, A, Unit>(ta, unit);
        return (MU)Monad.recur(step, go); 

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
    static Unit Foldable<T>.Iter<TA, A>(Action<A> f, in TA ta)
    {
        FS foldState = default!;
        T.FoldStepInit<TA, A>(ta, ref foldState);
        while (T.FoldStep<TA, A>(ta, ref foldState, out var value))
        {
            f(value);
        }
        return default;
    }
    
    /// <summary>
    /// Map each element of a structure to an action, evaluate these
    /// actions from left to right, and ignore the results.  For a version that
    /// doesn't ignore the results see `Traversable.traverse`.
    /// </summary>
    static Unit Foldable<T>.Iter<TA, A>(Action<int, A> f, in TA ta)
    {
        FS  foldState = default!;
        T.FoldStepInit<TA, A>(ta, ref foldState);
        var ix = 0;
        while (T.FoldStep<TA, A>(ta, ref foldState, out var value))
        {
            f(ix++, value);
        }
        return default;
    }

    /// <summary>
    /// Find the minimum value in the structure
    /// </summary>
    static Option<A> Foldable<T>.Min<TA, OrdA, A>(in TA ta)
    {
        FS foldState = default!;
        T.FoldStepInit<TA, A>(ta, ref foldState);
        A current;

        if (T.FoldStep<TA, A>(ta, ref foldState, out var head))
        {
            current = head;
        }
        else
        {
            return default;
        }
        
        while (T.FoldStep<TA, A>(ta, ref foldState, out var value))
        {
            if (OrdA.Compare(value, current) < 0)
            {
                current = value;
            }
        }
        return current;
    }

    /// <summary>
    /// Find the minimum value in the structure
    /// </summary>
    static Option<A> Foldable<T>.Min<TA, A>(in TA ta) =>
        T.Min<TA, OrdDefault<A>, A>(ta);
    
    /// <summary>
    /// Find the maximum value in the structure
    /// </summary>
    static Option<A> Foldable<T>.Max<TA, OrdA, A>(in TA ta)
    {
        FS foldState = default!;
        T.FoldStepInit<TA, A>(ta, ref foldState);
        A current;

        if (T.FoldStep<TA, A>(ta, ref foldState, out var head))
        {
            current = head;
        }
        else
        {
            return default;
        }
        
        while (T.FoldStep<TA, A>(ta, ref foldState, out var value))
        {
            if (OrdA.Compare(value, current) > 0)
            {
                current = value;
            }
        }
        return current;
    }

    /// <summary>
    /// Find the maximum value in the structure
    /// </summary>
    static Option<A> Foldable<T>.Max<TA, A>(in TA ta) =>
        T.Max<TA, OrdDefault<A>, A>(ta);
    
    /// <summary>
    /// Find the minimum value in the structure
    /// </summary>
    static A Foldable<T>.Min<TA, OrdA, A>(A initialMin, in TA ta)
    {
        FS foldState = default!;
        T.FoldStepInit<TA, A>(ta, ref foldState);
        var current = initialMin;
        
        while (T.FoldStep<TA, A>(ta, ref foldState, out var value))
        {
            if (OrdA.Compare(value, current) < 0)
            {
                current = value;
            }
        }
        return current;
    }

    /// <summary>
    /// Find the minimum value in the structure
    /// </summary>
    static A Foldable<T>.Min<TA, A>(A initialMin, in TA ta) =>
        T.Min<TA, OrdDefault<A>, A>(initialMin, ta);

    /// <summary>
    /// Find the maximum value in the structure
    /// </summary>
    static A Foldable<T>.Max<TA, OrdA, A>(A initialMax, in TA ta)
    {
        FS foldState = default!;
        T.FoldStepInit<TA, A>(ta, ref foldState);
        var current = initialMax;
        
        while (T.FoldStep<TA, A>(ta, ref foldState, out var value))
        {
            if (OrdA.Compare(value, current) > 0)
            {
                current = value;
            }
        }
        return current;
    }

    /// <summary>
    /// Find the maximum value in the structure
    /// </summary>
    static A Foldable<T>.Max<TA, A>(A initialMax, in TA ta) =>
        T.Max<TA, OrdDefault<A>, A>(initialMax, ta);

    /// <summary>
    /// Find the average of all the values in the structure
    /// </summary>
    static A Foldable<T>.Average<TA, A>(in TA ta)
    {
        FS foldState = default!;
        T.FoldStepInit<TA, A>(ta, ref foldState);
        var taken = A.Zero;
        var total = A.Zero;
        
        while (T.FoldStep<TA, A>(ta, ref foldState, out var value))
        {
            taken += A.One;
            total += value;
        }
        return taken == A.Zero
                   ? A.Zero
                   : total / taken;
    }

    /// <summary>
    /// Find the element at the specified index or `None` if out of range
    /// </summary>
    static Option<A> Foldable<T>.At<TA, A>(Index index, in TA ta)
    {
        var ix        = 0;
        FS  foldState = default!;
        if (index.IsFromEnd)
        {
            T.FoldStepBackInit<TA, A>(ta, ref foldState);
            while (T.FoldStepBack<TA, A>(ta, ref foldState, out var value))
            {
                if (ix == index.Value) return value;
                ix++;
            }
            return default;
        }
        else
        {
            T.FoldStepInit<TA, A>(ta, ref foldState);
            while (T.FoldStep<TA, A>(ta, ref foldState, out var value))
            {
                if (ix == index.Value) return value;
                ix++;
            }
            return default;
        }        
    }

    /// <summary>
    /// Partition a foldable into two sequences based on a predicate
    /// </summary>
    /// <param name="f">Predicate function</param>
    /// <param name="ta">Foldable structure</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Partitioned structure</returns>
    static (Seq<A> True, Seq<A> False) Foldable<T>.Partition<TA, A>(Func<A, bool> f, in TA ta)
    {
        var @true  = Seq<A>();
        var @false = Seq<A>();

        FS foldState = default!;
        T.FoldStepInit<TA, A>(ta, ref foldState);
        while (T.FoldStep<TA, A>(ta, ref foldState, out var value))
        {
            if (f(value))
            {
                @true = @true.Add(value);
            }
            else
            {
                @false = @false.Add(value);
            }
        }
        return (@true, @false);
    }
}
