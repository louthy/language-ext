using System;
using System.Collections.Generic;
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
public interface FoldableBack<T, FS> : FoldableBack<T> 
    where T : FoldableBack<T, FS>
    where FS : allows ref struct
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Abstract members
    //

    public static abstract void FoldStepBackSetup<A>(K<T, A> ta, ref FS refState);
    public static abstract bool FoldStepBack<A>(K<T, A> ta, ref FS refState, out A value);
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Default implementations
    //

    /// <summary>
    /// Same behaviour as `FoldBack` but allows early exit of the operation once
    /// the predicate function becomes `false` for the state/value pair 
    /// </summary>
    static S FoldableBack<T>.FoldBackWhile<A, S>(
        Func<S, A, S> f,
        Func<(S State, A Value), bool> predicate,
        in S initialState,
        K<T, A> ta)
    {
        FS foldState = default!;
        T.FoldStepBackSetup(ta, ref foldState);
        var state = initialState;
        
        while (T.FoldStepBack(ta, ref foldState, out var value))
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
    static S FoldableBack<T>.FoldBackMaybe<A, S>(
        Func<S, A, Option<S>> f,
        in S initialState,
        K<T, A> ta)
    {
        FS foldState = default!;
        T.FoldStepBackSetup(ta, ref foldState);
        var state = initialState;
        
        while (T.FoldStepBack(ta, ref foldState, out var value))
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
    /// Same behaviour as `FoldBack` but allows early exit of the operation once
    /// the predicate function becomes `false` for the state/value pair
    /// </summary>
    static S FoldableBack<T>.FoldBackUntil<A, S>(
        Func<S, A, S> f, 
        Func<(S State, A Value), bool> predicate, 
        in S initialState, 
        K<T, A> ta)
    {
        FS foldState = default!;
        T.FoldStepBackSetup(ta, ref foldState);
        var state = initialState;
        
        while (T.FoldStepBack(ta, ref foldState, out var value))
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
    static S FoldableBack<T>.FoldBack<A, S>(Func<S, A, S> f, in S initialState, K<T, A> ta)
    {
        FS foldState = default!;
        T.FoldStepBackSetup(ta, ref foldState);
        var state = initialState;
        while (T.FoldStepBack(ta, ref foldState, out var value))
        {
            state = f(state, value);
        }
        return state;
    }

    /// <summary>
    /// List of elements of a structure, from left to right
    /// </summary>
    static Lst<A> FoldableBack<T>.ToLstBack<A>(K<T, A> ta) =>
        Lst<A>.FromFoldableBack<T, FS>(ta);

    /// <summary>
    /// List of elements of a structure, from left to right
    /// </summary>
    static Arr<A> FoldableBack<T>.ToArrBack<A>(K<T, A> ta)
    {
        var buffer = new A[32];
        var max    = buffer.Length;
        var length = 0;
        
        // TODO: Use ArrayWriter
        
        FS foldState = default!;
        T.FoldStepBackSetup(ta, ref foldState);
        while (T.FoldStepBack(ta, ref foldState, out var value))
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
    /// Does an element that fits the predicate occur in the structure?
    /// </summary>
    static bool FoldableBack<T>.ExistsBack<A>(Func<A, bool> predicate, K<T, A> ta)
    {
        FS foldState = default!;
        T.FoldStepBackSetup(ta, ref foldState);
        while (T.FoldStepBack(ta, ref foldState, out var value))
        {
            if(predicate(value)) return true;
        }
        return false;
    }

    /// <summary>
    /// Does the predicate hold for all elements in the structure?
    /// </summary>
    static bool FoldableBack<T>.ForAllBack<A>(Func<A, bool> predicate, K<T, A> ta)
    {
        FS foldState = default!;
        T.FoldStepBackSetup(ta, ref foldState);
        while (T.FoldStepBack(ta, ref foldState, out var value))
        {
            if(!predicate(value)) return false;
        }
        return true;
    }

    /// <summary>
    /// Does the element exist in the structure?
    /// </summary>
    static bool FoldableBack<T>.ContainsBack<EqA, A>(A value, K<T, A> ta) 
    {
        FS foldState = default!;
        T.FoldStepBackSetup(ta, ref foldState);
        while (T.FoldStepBack(ta, ref foldState, out var v))
        {
            if(EqA.Equals(value, v)) return true;
        }
        return false;
    }

    /// <summary>
    /// Does the element exist in the structure?
    /// </summary>
    static bool FoldableBack<T>.ContainsBack<A>(A value, K<T, A> ta) 
    {
        FS foldState = default!;
        T.FoldStepBackSetup(ta, ref foldState);
        while (T.FoldStepBack(ta, ref foldState, out var v))
        {
            if(EqualityComparer<A>.Default.Equals(value, v)) return true;
        }
        return false;
    }

    /// <summary>
    /// Find the last element that match the predicate
    /// </summary>
    static Option<A> FoldableBack<T>.FindBack<A>(Func<A, bool> predicate, K<T, A> ta)
    {
        FS foldState = default!;
        T.FoldStepBackSetup(ta, ref foldState);
        while (T.FoldStepBack(ta, ref foldState, out var value))
        {
            if(predicate(value)) return value;
        }
        return default;
    }

    /// <summary>
    /// Get the last item in the foldable or `None`
    /// </summary>
    static Option<A> FoldableBack<T>.Last<A>(K<T, A> ta)
    {
        FS foldState = default!;
        T.FoldStepBackSetup(ta, ref foldState);
        if (T.FoldStepBack(ta, ref foldState, out var value))
        {
            return value;
        }
        else
        {
            return default;
        }
    }

    /// <summary>
    /// Find the element at the specified index or `None` if out of range
    /// </summary>
    static Option<A> FoldableBack<T>.AtBack<A>(int index, K<T, A> ta)
    {
        var ix        = 0;
        FS  foldState = default!;
        T.FoldStepBackSetup(ta, ref foldState);
        while (T.FoldStepBack(ta, ref foldState, out var value))
        {
            if (ix == index) return value;
            ix++;
        }
        return default;
    }

    /// <summary>
    /// Partition a foldable into two sequences based on a predicate
    /// </summary>
    /// <param name="f">Predicate function</param>
    /// <param name="ta">Foldable structure</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Partitioned structure</returns>
    static (Arr<A> True, Arr<A> False) FoldableBack<T>.PartitionBack<A>(Func<A, bool> f, K<T, A> ta)
    {
        var @true = ArrayWriter<A>.Init();
        var @false = ArrayWriter<A>.Init();

        FS foldState = default!;
        T.FoldStepBackSetup(ta, ref foldState);
        while (T.FoldStepBack(ta, ref foldState, out var value))
        {
            if (f(value))
            {
                @true.Add(value);
            }
            else
            {
                @false.Add(value);
            }
        }
        return (@true.ToArr(), @false.ToArr());
    }
}
