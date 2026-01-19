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
public interface FoldableBack<T> : IterableBackK<T>
    where T : FoldableBack<T>
{
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
        foreach(var value in T.BackwardIterator(ta))
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
        foreach(var value in T.BackwardIterator(ta))
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
        var state = initialState;
        foreach(var head in T.BackwardIterator(ta))
        {
            state = f(state, head);
            if (predicate((state, head))) return state;
        }
        return state;
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
    /// argument.
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
        var state = initialState;
        foreach (var head in ta.BackwardIterator())
        {
            state = f(state, head);
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
    /// List of elements of a structure
    /// </summary>
    /// <remarks>
    /// The sequence is lazy
    /// </remarks>
    static virtual Seq<A> ToSeqBack<A>(K<T, A> ta) =>
        new(T.BackwardIterator(ta));    

    /// <summary>
    /// List of elements of a structure
    /// </summary>
    static virtual Lst<A> ToLstBack<A>(K<T, A> ta) =>
        new (T.BackwardIterator(ta));

    /// <summary>
    /// List of elements of a structure
    /// </summary>
    static virtual Arr<A> ToArrBack<A>(K<T, A> ta)
    {
        var writer = ArrayWriter<A>.Init();
        foreach(var head in T.BackwardIterator(ta))
        {
            writer.Add(head);
        }
        return writer.ToArr();
    }

    /// <summary>
    /// List of elements of a structure
    /// </summary>
    /// <remarks>
    /// The sequence is lazy
    /// </remarks>
    static virtual Iterable<A> ToIterableBack<A>(K<T, A> ta) =>
        new IterableIterator<A>(T.BackwardIterator(ta));

    /// <summary>
    /// Does an element that fits the predicate occur in the structure?
    /// </summary>
    static virtual bool ExistsBack<A>(Func<A, bool> predicate, K<T, A> ta)
    {
        foreach(var head in T.BackwardIterator(ta))
        {
            if(predicate(head)) return true;
        }
        return false;
    }

    /// <summary>
    /// Does the predicate hold for all elements in the structure?
    /// </summary>
    static virtual bool ForAllBack<A>(Func<A, bool> predicate, K<T, A> ta)
    {
        foreach(var head in T.BackwardIterator(ta))
        {
            if(!predicate(head)) return false;
        }
        return true;
    }

    /// <summary>
    /// Does the element exist in the structure?
    /// </summary>
    static virtual bool ContainsBack<EqA, A>(A value, K<T, A> ta) 
        where EqA : Eq<A>
    {
        foreach(var head in T.BackwardIterator(ta))
        {
            if(EqA.Equals(value, head)) return true;
        }
        return false;
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
        foreach(var head in T.BackwardIterator(ta))
        {
            if(predicate(head)) return Some(head);
        }
        return default;
    }

    /// <summary>
    /// Find the elements that match the predicate
    /// </summary>
    /// <remarks>
    /// The sequence is lazy, but note, if the original foldable structure is lazy,
    /// then it will need to be consumed in its entirety before the values are yielded.
    /// </remarks>
    static virtual Iterator<A> FindAllBack<A>(Func<A, bool> predicate, K<T, A> ta) =>
        T.BackwardIterator(ta)
         .Filter(predicate);

    /// <summary>
    /// Get the last item in the foldable or `None`
    /// </summary>
    static virtual Option<A> Last<A>(K<T, A> ta)
    {
        var iter = T.BackwardIterator(ta);
        return iter switch
               {
                   (Exist<A>(var last), _) => Some(last),
                   _                       => None
               };
    }

    /// <summary>
    /// Find the element at the specified index (from the end) or `None` if out of range
    /// </summary>
    static virtual Option<A> AtBack<A>(long index, K<T, A> ta)
    {
        var ix = 0L;
        foreach(var head in T.BackwardIterator(ta))
        {
            if(ix == index) return head;
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
    static virtual (Arr<A> True, Arr<A> False) PartitionBack<A>(Func<A, bool> f, K<T, A> ta)
    {
        var @true  = ArrayWriter<A>.Init();
        var @false = ArrayWriter<A>.Init();
        foreach(var head in T.BackwardIterator(ta))
        {
            if (f(head))
            {
                @true.Add(head);
            }
            else
            {
                @false.Add(head);
            }
        }
        return (@true.ToArr(), @false.ToArr());
    }
}
