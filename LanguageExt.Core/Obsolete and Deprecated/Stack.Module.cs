using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using static LanguageExt.Prelude;
using System.Runtime.CompilerServices;
using LanguageExt.Traits;

namespace LanguageExt;

[EditorBrowsable(EditorBrowsableState.Never)]
public partial class Stack
{
    [Obsolete("Use Stck instead")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [OverloadResolutionPriority(Change.Priority)]
    public static Stck<A> singleton<A>(A item) =>
        new Stck<A>.Top(item, Stck<A>.Empty);

    [Obsolete("Use Stck instead")]
    [EditorBrowsable(EditorBrowsableState.Never)]
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
    [EditorBrowsable(EditorBrowsableState.Never)]
    [OverloadResolutionPriority(Change.Priority)]
    public static Stck<A> createRange<A>(ReadOnlySpan<A> items) =>
        [..items];
    
    [Obsolete("Use Stck instead")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [OverloadResolutionPriority(Change.Priority)]
    public static Stck<T> rev<T>(Stck<T> stack) =>
        stack.Reverse();

    [Obsolete("Use Stck instead")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [OverloadResolutionPriority(Change.Priority)]
    public static bool isEmpty<T>(Stck<T> stack) =>
        stack.IsEmpty;

    [Obsolete("Use Stck instead")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [OverloadResolutionPriority(Change.Priority)]
    public static Stck<T> clear<T>(Stck<T> stack) =>
        Stck<T>.Empty;

    [Obsolete("Use Stck instead")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [OverloadResolutionPriority(Change.Priority)]
    public static T peek<T>(Stck<T> stack) =>
        stack.PeekUnsafe();

    [Obsolete("Use Stck instead")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [OverloadResolutionPriority(Change.Priority)]
    public static R peek<T, R>(Stck<T> stack, Func<T, R> Some, Func<R> None) =>
        stack.Peek().Match(Some, None);

    [Obsolete("Use Stck instead")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [OverloadResolutionPriority(Change.Priority)]
    public static Option<T> trypeek<T>(Stck<T> stack) =>
        stack.Peek();

    [Obsolete("Use Stck instead")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [OverloadResolutionPriority(Change.Priority)]
    public static Stck<T> pop<T>(Stck<T> stack) =>
        stack.Pop();

    [Obsolete("Use Stck instead")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [OverloadResolutionPriority(Change.Priority)]
    public static Stck<T> push<T>(Stck<T> stack, T value) =>
        stack.Push(value);

    [Obsolete("Use Stck instead")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [OverloadResolutionPriority(Change.Priority)]
    public static Stck<R> map<T, R>(Stck<T> stack, Func<T, R> map) =>
        stack.Map(map);

    [Obsolete("Use Stck instead")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [OverloadResolutionPriority(Change.Priority)]
    public static Stck<R> map<T, R>(Stck<T> stack, Func<T, int, R> map) =>
        Stck.createRange(stack.AsEnumerable().Select(map));

    [Obsolete("Use Stck instead")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [OverloadResolutionPriority(Change.Priority)]
    public static Stck<T> filter<T>(Stck<T> stack, Func<T, bool> predicate) =>
        stack.Filter(predicate);

    [Obsolete("Use Stck instead")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [OverloadResolutionPriority(Change.Priority)]
    public static Stck<U> choose<T, U>(Stck<T> stack, Func<T, Option<U>> selector) =>
        stack.Choose(selector);

    [Obsolete("Use Stck instead")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [OverloadResolutionPriority(Change.Priority)]
    public static Stck<T> append<T>(Stck<T> lhs, IEnumerable<T> rhs) =>
        lhs.Combine(Stck.createRange(rhs));

    [Obsolete("Use Stck instead")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [OverloadResolutionPriority(Change.Priority)]
    public static S fold<S, T>(Stck<T> stack, S state, Func<S, T, S> folder) =>
        List.fold(stack, state, folder);

    [Obsolete("Use Stck instead")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [OverloadResolutionPriority(Change.Priority)]
    public static S foldBack<S, T>(Stck<T> stack, S state, Func<S, T, S> folder) =>
        List.foldBack(stack, state, folder);

    [Obsolete("Use Stck instead")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [OverloadResolutionPriority(Change.Priority)]
    public static S foldWhile<S, T>(Stck<T> stack, S state, Func<S, T, S> folder, Func<T, bool> preditem) =>
        List.foldWhile(stack, state, folder, preditem: preditem);

    [Obsolete("Use Stck instead")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [OverloadResolutionPriority(Change.Priority)]
    public static S foldWhile<S, T>(Stck<T> stack, S state, Func<S, T, S> folder, Func<S, bool> predstate) =>
        List.foldWhile(stack, state, folder, predstate: predstate);

    [Obsolete("Use Stck instead")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [OverloadResolutionPriority(Change.Priority)]
    public static S foldBackWhile<S, T>(Stck<T> stack, S state, Func<S, T, S> folder, Func<T, bool> preditem) =>
        List.foldBackWhile(stack, state, folder, preditem: preditem);

    [Obsolete("Use Stck instead")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [OverloadResolutionPriority(Change.Priority)]
    public static S foldBackWhile<S, T>(Stck<T> stack, S state, Func<S, T, S> folder, Func<S, bool> predstate) =>
        List.foldBackWhile(stack, state, folder, predstate: predstate);

    [Obsolete("Use Stck instead")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [OverloadResolutionPriority(Change.Priority)]
    public static T reduce<T>(Stck<T> stack, Func<T, T, T> reducer) =>
        List.reduce(stack, reducer);

    [Obsolete("Use Stck instead")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [OverloadResolutionPriority(Change.Priority)]
    public static T reduceBack<T>(Stck<T> stack, Func<T, T, T> reducer) =>
        List.reduceBack(stack, reducer);

    [Obsolete("Use Stck instead")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [OverloadResolutionPriority(Change.Priority)]
    public static Option<T> find<T>(Stck<T> stack, Func<T, bool> pred) =>
        List.find(stack, pred);

    [Obsolete("Use Stck instead")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [OverloadResolutionPriority(Change.Priority)]
    public static int length<T>(Stck<T> stack) =>
        List.length(stack);

    [Obsolete("Use Stck instead")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [OverloadResolutionPriority(Change.Priority)]
    public static Unit iter<T>(Stck<T> stack, Action<T> action) =>
        List.iter(stack, action);

    [Obsolete("Use Stck instead")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [OverloadResolutionPriority(Change.Priority)]
    public static Unit iter<T>(Stck<T> stack, Action<int, T> action) =>
        List.iter(stack, action);

    [Obsolete("Use Stck instead")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [OverloadResolutionPriority(Change.Priority)]
    public static bool forall<T>(Stck<T> stack, Func<T, bool> pred) =>
        List.forall(stack, pred);

    [Obsolete("Use Stck instead")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [OverloadResolutionPriority(Change.Priority)]
    public static bool exists<T>(Stck<T> stack, Func<T, bool> pred) =>
        List.exists(stack, pred);
}
