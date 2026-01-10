using System;
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
public interface Foldable<T> : IterableK<T> 
    where T : Foldable<T>
{
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
        var state = initialState;
        foreach(var head in T.ForwardIterator(ta).Using())
        {
            if (!predicate((state, head))) return state;
            state = f(state, head);
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
    static virtual S FoldMaybe<A, S>(
        Func<S, A, Option<S>> f,
        in S initialState,
        K<T, A> ta)
    {
        var state = initialState;
        foreach(var head in T.ForwardIterator(ta).Using())
        {
            var option = f(state, head);
            if(option.IsNone) return state;
            state = (S)option;
        }
        return state;
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
        return (MS)Monad.recur((initialState, T.ForwardIterator(ta)), go);
        
        K<M, Next<(S State, Iterator<A> Iter), S>> go((S State, Iterator<A> Iter) input) =>
            input.Iter is (Exist<A> (var head), var tail) && predicate((input.State, head)) 
                ? f(input.State, head).Map(ns => Next.Loop<(S State, Iterator<A> Iter), S>((ns, tail)))
                : M.Pure(Next.Done<(S State, Iterator<A> Iter), S>(input.State));
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
        var state = initialState;
        foreach(var head in T.ForwardIterator(ta).Using())
        {
            state = f(state, head);
            if (predicate((state, head))) return state;
        }
        return state;
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
        return (MS)Monad.recur((initialState, T.ForwardIterator(ta)), go);

        K<M, Next<(S State, Iterator<A> Iter), S>> go((S State, Iterator<A> Iter) input) =>
            input.Iter is (Exist<A> (var head), var tail)
                ? f(input.State, head).Map(ns => predicate((ns, head))
                                                     ? Next.Done<(S State, Iterator<A> Iter), S>(ns)
                                                     : Next.Loop<(S State, Iterator<A> Iter), S>((ns, tail)))
                : M.Pure(Next.Done<(S State, Iterator<A> Iter), S>(input.State));
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
        var state = initialState;
        foreach(var head in T.ForwardIterator(ta).Using())
        {
            state = f(state, head);
        }
        return state;
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
        return (MS)Monad.recur((initialState, T.ForwardIterator(ta)), go);

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
    static virtual Seq<A> ToSeq<A>(K<T, A> ta) =>
        new(T.ForwardIterator(ta));

    /// <summary>
    /// List of elements of a structure
    /// </summary>
    static virtual Lst<A> ToLst<A>(K<T, A> ta) =>
        new (T.ForwardIterator(ta));

    /// <summary>
    /// List of elements of a structure
    /// </summary>
    static virtual Arr<A> ToArr<A>(K<T, A> ta)
    {
        var writer = ArrayWriter<A>.Init();
        foreach(var head in T.ForwardIterator(ta).Using())
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
    static virtual Iterable<A> ToIterable<A>(K<T, A> ta) =>
        new IterableIterator<A>(T.ForwardIterator(ta));

    /// <summary>
    /// List of elements of a structure, from left to right
    /// </summary>
    static virtual bool IsEmpty<A>(K<T, A> ta) =>
        T.ForwardIterator(ta) is (Nil<A>, _);

    /// <summary>
    /// Returns the size/length of a finite structure as an `int`.  The
    /// default implementation just counts elements starting with the leftmost.
    /// 
    /// Instances for structures that can compute the element count faster
    /// than via element-by-element counting, should provide a specialised
    /// implementation.
    /// </summary>
    static virtual int Count<A>(K<T, A> ta)
    {
        var count = 0;
        foreach(var _ in T.ForwardIterator(ta).Using())
        {
            count++;
        }
        return count;
    }

    /// <summary>
    /// Does an element that fits the predicate occur in the structure?
    /// </summary>
    static virtual bool Exists<A>(Func<A, bool> predicate, K<T, A> ta)
    {
        foreach(var head in T.ForwardIterator(ta).Using())
        {
            if(predicate(head)) return true;
        }
        return false;
    }

    /// <summary>
    /// Does the predicate hold for all elements in the structure?
    /// </summary>
    static virtual bool ForAll<A>(Func<A, bool> predicate, K<T, A> ta)
    {
        foreach(var head in T.ForwardIterator(ta).Using())
        {
            if(!predicate(head)) return false;
        }
        return true;
    }

    /// <summary>
    /// Does the element exist in the structure?
    /// </summary>
    static virtual bool Contains<EqA, A>(A value, K<T, A> ta) 
        where EqA : Eq<A>
    {
        foreach(var head in T.ForwardIterator(ta).Using())
        {
            if(EqA.Equals(value, head)) return true;
        }
        return false;
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
        foreach(var head in T.ForwardIterator(ta).Using())
        {
            if(predicate(head)) return Some(head);
        }
        return default;
    }

    /// <summary>
    /// Find the elements that match the predicate
    /// </summary>
    /// <remarks>
    /// The sequence is lazy
    /// </remarks>
    static virtual Iterable<A> FindAll<A>(Func<A, bool> predicate, K<T, A> ta) =>
        T.ForwardIterator(ta)
         .Filter(predicate)
         .AsIterable();

    /// <summary>
    /// Get the head item in the foldable or `None`
    /// </summary>
    static virtual Option<A> Head<A>(K<T, A> ta)
    {
        using var iter = T.ForwardIterator(ta).Using();
        return iter switch
        {
            (Exist<A>(var head), _) => Some(head),
            _                       => None
        };
    }

    /// <summary>
    /// Map each element of a structure to a monadic action, evaluate these
    /// actions from left to right, and ignore the results. 
    /// </summary>
    static virtual K<M, Unit> IterM<MB, M, A, B>(Func<A, MB> f, K<T, A> ta)
        where M : Monad<M>
        where MB : K<M, B>
    {
        return Monad.recur(T.ForwardIterator(ta), go);

        K<M, Next<Iterator<A>, Unit>> go(Iterator<A> input) =>
            input is (Exist<A> (var head), var tail)
                ? f(head).Map(_ => Next.Loop<Iterator<A>, Unit>(tail))
                : M.Pure(Next.Done<Iterator<A>, Unit>(default));
    }
    
    /// <summary>
    /// Map each element of a structure to an action, evaluate these
    /// actions from left to right, and ignore the results.  For a version that
    /// doesn't ignore the results see `Traversable.traverse`.
    /// </summary>
    static virtual Unit Iter<A>(Action<A> f, K<T, A> ta)
    {
        foreach(var head in T.ForwardIterator(ta).Using())
        {
            f(head);
        }
        return default;
    }
    
    /// <summary>
    /// Map each element of a structure to an action, evaluate these
    /// actions from left to right, and ignore the results.  For a version that
    /// doesn't ignore the results see `Traversable.traverse`.
    /// </summary>
    static virtual Unit Iter<A>(Action<int, A> f, K<T, A> ta)
    {
        var ix = 0;
        foreach(var head in T.ForwardIterator(ta).Using())
        {
            f(ix++, head);
        }
        return default;
    }

    /// <summary>
    /// Find the minimum value in the structure
    /// </summary>
    static virtual Option<A> Min<OrdA, A>(K<T, A> ta)
        where OrdA : Ord<A>
    {
        A current;
        using var iter = T.ForwardIterator(ta).Using();
        if (iter is (Exist<A> (var h), var t))
        {
            current = h;
        }
        else
        {
            return None;
        }

        foreach(var head in t)
        {
            if(OrdA.Compare(head, current) < 0) current = head;
        }
        return current;
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
        A current;
        using var iter = T.ForwardIterator(ta).Using();
        if (iter is (Exist<A> (var h), var t))
        {
            current = h;
        }
        else
        {
            return None;
        }

        foreach(var head in t)
        {
            if(OrdA.Compare(head, current) > 0) current = head;
        }
        return current;
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
    static virtual Option<A> At<A>(int index, K<T, A> ta)
    {
        var ix = 0;
        foreach(var head in T.ForwardIterator(ta).Using())
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
    static virtual (Arr<A> True, Arr<A> False) Partition<A>(Func<A, bool> f, K<T, A> ta)
    {
        var @true  = ArrayWriter<A>.Init();
        var @false = ArrayWriter<A>.Init();
        foreach(var head in T.ForwardIterator(ta).Using())
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
