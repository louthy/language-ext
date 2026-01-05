using System;
using System.Collections.Generic;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// Functional module for working with the Stck T type
/// </summary>
public partial class Stck
{
    [Obsolete("Use Stck instead")]
    [OverloadResolutionPriority(Change.Priority)]
    public static Stck<A> singleton<A>(A item) =>
        new Stck<A>.Top(item, Stck<A>.Empty);

    [Obsolete("Use Stck instead")]
    [OverloadResolutionPriority(Change.Priority)]
    public static Stck<A> createRange<A>(IEnumerable<A> items)
    {
        var tail = Stck<A>.Empty;
        foreach (var item in items)
        {
            tail = new Stck<A>.Top(item, tail);
        }
        return tail;
    }
    
    [Obsolete("Use Stck instead")]
    [OverloadResolutionPriority(Change.Priority)]
    public static Stck<A> createRange<A>(ReadOnlySpan<A> items) =>
        items.IsEmpty
            ? Stck<A>.Empty
            : new (items);
    
    [Obsolete("Use Stck instead")]
    [OverloadResolutionPriority(Change.Priority)]
    public static Stck<T> rev<T>(Stck<T> stack) =>
        stack.Reverse();

    [Obsolete("Use Stck instead")]
    [OverloadResolutionPriority(Change.Priority)]
    public static bool isEmpty<T>(Stck<T> stack) =>
        stack.IsEmpty;

    [Obsolete("Use Stck instead")]
    [OverloadResolutionPriority(Change.Priority)]
    public static Stck<T> clear<T>(Stck<T> stack) =>
        stack.Clear();

    [Obsolete("Use Stck instead")]
    [OverloadResolutionPriority(Change.Priority)]
    public static T peek<T>(Stck<T> stack) =>
        stack.Peek();

    [Obsolete("Use Stck instead")]
    [OverloadResolutionPriority(Change.Priority)]
    public static Stck<T> peek<T>(Stck<T> stack, Action<T> Some, Action None) =>
        stack.Peek(Some, None);

    [Obsolete("Use Stck instead")]
    [OverloadResolutionPriority(Change.Priority)]
    public static R peek<T, R>(Stck<T> stack, Func<T, R> Some, Func<R> None) =>
        stack.Peek(Some, None);

    [Obsolete("Use Stck instead")]
    [OverloadResolutionPriority(Change.Priority)]
    public static Option<T> trypeek<T>(Stck<T> stack) =>
        stack.TryPeek();

    [Obsolete("Use Stck instead")]
    [OverloadResolutionPriority(Change.Priority)]
    public static Stck<T> pop<T>(Stck<T> stack) =>
        stack.Pop();

    [Obsolete("Use Stck instead")]
    [OverloadResolutionPriority(Change.Priority)]
    public static (Stck<T>, Option<T>) trypop<T>(Stck<T> stack) =>
        stack.TryPop();

    [Obsolete("Use Stck instead")]
    [OverloadResolutionPriority(Change.Priority)]
    public static Stck<T> pop<T>(Stck<T> stack, Action<T> Some, Action None) =>
        stack.Pop(Some, None);

    [Obsolete("Use Stck instead")]
    [OverloadResolutionPriority(Change.Priority)]
    public static R pop<T, R>(Stck<T> stack, Func<Stck<T>, T, R> Some, Func<R> None) =>
        stack.Pop(Some, None);

    [Obsolete("Use Stck instead")]
    [OverloadResolutionPriority(Change.Priority)]
    public static Stck<T> push<T>(Stck<T> stack, T value) =>
        stack.Push(value);

    [Obsolete("Use Stck instead")]
    [OverloadResolutionPriority(Change.Priority)]
    public static Stck<R> map<T, R>(Stck<T> stack, Func<T, R> map) =>
        toStackRev(List.map(stack, map));

    [Obsolete("Use Stck instead")]
    [OverloadResolutionPriority(Change.Priority)]
    public static Stck<R> map<T, R>(Stck<T> stack, Func<int, T, R> map) =>
        toStackRev(List.map(stack, map));

    [Obsolete("Use Stck instead")]
    [OverloadResolutionPriority(Change.Priority)]
    public static Stck<T> filter<T>(Stck<T> stack, Func<T, bool> predicate) =>
        toStackRev(List.filter(stack, predicate));

    [Obsolete("Use Stck instead")]
    [OverloadResolutionPriority(Change.Priority)]
    public static Stck<U> choose<T, U>(Stck<T> stack, Func<T, Option<U>> selector) =>
        toStackRev(List.choose(stack, selector));

    [Obsolete("Use Stck instead")]
    [OverloadResolutionPriority(Change.Priority)]
    public static Stck<U> choose<T, U>(Stck<T> stack, Func<int, T, Option<U>> selector) =>
        toStackRev(List.choose(stack, selector));

    [Obsolete("Use Stck instead")]
    [OverloadResolutionPriority(Change.Priority)]
    public static Stck<R> collect<T, R>(Stck<T> stack, Func<T, IEnumerable<R>> map) =>
        toStackRev(List.collect(stack, map));

    [Obsolete("Use Stck instead")]
    [OverloadResolutionPriority(Change.Priority)]
    public static Stck<T> append<T>(Stck<T> lhs, IEnumerable<T> rhs) =>
        toStackRev(List.append(lhs, rhs));

    [Obsolete("Use Stck instead")]
    [OverloadResolutionPriority(Change.Priority)]
    public static S fold<S, T>(Stck<T> stack, S state, Func<S, T, S> folder) =>
        List.fold(stack, state, folder);

    [Obsolete("Use Stck instead")]
    [OverloadResolutionPriority(Change.Priority)]
    public static S foldBack<S, T>(Stck<T> stack, S state, Func<S, T, S> folder) =>
        List.foldBack(stack, state, folder);

    [Obsolete("Use Stck instead")]
    [OverloadResolutionPriority(Change.Priority)]
    public static S foldWhile<S, T>(Stck<T> stack, S state, Func<S, T, S> folder, Func<T, bool> preditem) =>
        List.foldWhile(stack, state, folder, preditem: preditem);

    [Obsolete("Use Stck instead")]
    [OverloadResolutionPriority(Change.Priority)]
    public static S foldWhile<S, T>(Stck<T> stack, S state, Func<S, T, S> folder, Func<S, bool> predstate) =>
        List.foldWhile(stack, state, folder, predstate: predstate);

    [Obsolete("Use Stck instead")]
    [OverloadResolutionPriority(Change.Priority)]
    public static S foldBackWhile<S, T>(Stck<T> stack, S state, Func<S, T, S> folder, Func<T, bool> preditem) =>
        List.foldBackWhile(stack, state, folder, preditem: preditem);

    [Obsolete("Use Stck instead")]
    [OverloadResolutionPriority(Change.Priority)]
    public static S foldBackWhile<S, T>(Stck<T> stack, S state, Func<S, T, S> folder, Func<S, bool> predstate) =>
        List.foldBackWhile(stack, state, folder, predstate: predstate);

    [Obsolete("Use Stck instead")]
    [OverloadResolutionPriority(Change.Priority)]
    public static T reduce<T>(Stck<T> stack, Func<T, T, T> reducer) =>
        List.reduce(stack, reducer);

    [Obsolete("Use Stck instead")]
    [OverloadResolutionPriority(Change.Priority)]
    public static T reduceBack<T>(Stck<T> stack, Func<T, T, T> reducer) =>
        List.reduceBack(stack, reducer);

    [Obsolete("Use Stck instead")]
    [OverloadResolutionPriority(Change.Priority)]
    public static Stck<S> scan<S, T>(Stck<T> stack, S state, Func<S, T, S> folder) =>
        toStackRev(List.scan(stack, state, folder));

    [Obsolete("Use Stck instead")]
    [OverloadResolutionPriority(Change.Priority)]
    public static Stck<S> scanBack<S, T>(Stck<T> stack, S state, Func<S, T, S> folder) =>
        toStackRev(List.scanBack(stack, state, folder));

    [Obsolete("Use Stck instead")]
    [OverloadResolutionPriority(Change.Priority)]
    public static Option<T> find<T>(Stck<T> stack, Func<T, bool> pred) =>
        List.find(stack, pred);

    [Obsolete("Use Stck instead")]
    [OverloadResolutionPriority(Change.Priority)]
    public static Stck<V> zip<T, U, V>(Stck<T> stack, IEnumerable<U> other, Func<T, U, V> zipper) =>
        toStackRev(List.zip(stack, other, zipper));

    [Obsolete("Use Stck instead")]
    [OverloadResolutionPriority(Change.Priority)]
    public static int length<T>(Stck<T> stack) =>
        List.length(stack);

    [Obsolete("Use Stck instead")]
    [OverloadResolutionPriority(Change.Priority)]
    public static Unit iter<T>(Stck<T> stack, Action<T> action) =>
        List.iter(stack, action);

    /// <summary>
    [Obsolete("Use Stck instead")]
    [OverloadResolutionPriority(Change.Priority)]
    public static Unit iter<T>(Stck<T> stack, Action<int, T> action) =>
        List.iter(stack, action);

    [Obsolete("Use Stck instead")]
    [OverloadResolutionPriority(Change.Priority)]
    public static bool forall<T>(Stck<T> stack, Func<T, bool> pred) =>
        List.forall(stack, pred);

    [Obsolete("Use Stck instead")]
    [OverloadResolutionPriority(Change.Priority)]
    public static Stck<T> distinct<T>(Stck<T> stack) =>
        toStackRev(List.distinct(stack));

    [Obsolete("Use Stck instead")]
    [OverloadResolutionPriority(Change.Priority)]
    public static Stck<T> distinct<EQ, T>(Stck<T> stack) where EQ : Eq<T> =>
        toStackRev(List.distinct<EQ,T>(stack));

    [Obsolete("Use Stck instead")]
    [OverloadResolutionPriority(Change.Priority)]
    public static Stck<T> distinct<T, K>(Stck<T> stack, Func<T, K> keySelector, Option<Func<K, K, bool>> compare = default(Option<Func<K, K, bool>>)) =>
        toStackRev(List.distinct(stack, keySelector, compare));

    [Obsolete("Use Stck instead")]
    [OverloadResolutionPriority(Change.Priority)]
    public static Stck<T> take<T>(Stck<T> stack, int count) =>
        toStackRev(List.take(stack, count));

    [Obsolete("Use Stck instead")]
    [OverloadResolutionPriority(Change.Priority)]
    public static Stck<T> takeWhile<T>(Stck<T> stack, Func<T, bool> pred) =>
        toStackRev(List.takeWhile(stack, pred));

    [Obsolete("Use Stck instead")]
    [OverloadResolutionPriority(Change.Priority)]
    public static Stck<T> takeWhile<T>(Stck<T> stack, Func<T, int, bool> pred) =>
        toStackRev(List.takeWhile(stack, pred));

    [Obsolete("Use Stck instead")]
    [OverloadResolutionPriority(Change.Priority)]
    public static bool exists<T>(Stck<T> stack, Func<T, bool> pred) =>
        List.exists(stack, pred);
}
