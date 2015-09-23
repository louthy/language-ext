using System;
using System.Collections.Generic;
using LanguageExt;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    public static partial class Stack
    {
        public static Stck<T> push<T>(Stck<T> stack, T value) =>
            stack.Push(value);

        public static Tuple<Stck<T>, T> popUnsafe<T>(Stck<T> stack)
        {
            T value;
            var newStack = stack.Pop(out value);
            return Tuple(newStack, value);
        }

        public static T peekUnsafe<T>(Stck<T> stack) =>
            stack.Peek();

        public static Tuple<Stck<T>, Option<T>> pop<T>(Stck<T> stack) =>
            stack.TryPop();

        public static Option<T> peek<T>(Stck<T> stack) =>
            stack.TryPeek();

        public static Stck<T> clear<T>(Stck<T> stack) =>
            stack.Clear();

        public static IEnumerable<R> map<T, R>(Stck<T> stack, Func<int, T, R> map) =>
            List.map(stack, map);

        public static IEnumerable<T> filter<T>(Stck<T> stack, Func<T, bool> predicate) =>
            List.filter(stack, predicate);

        public static IEnumerable<T> choose<T>(Stck<T> stack, Func<T, Option<T>> selector) =>
            List.choose(stack, selector);

        public static IEnumerable<T> choose<T>(Stck<T> stack, Func<int, T, Option<T>> selector) =>
            List.choose(stack, selector);

        public static IEnumerable<R> collect<T, R>(Stck<T> stack, Func<T, IEnumerable<R>> map) =>
            List.collect(stack, map);

        public static IEnumerable<T> rev<T>(Stck<T> stack) =>
            List.rev(stack);

        public static IEnumerable<T> append<T>(IEnumerable<T> lhs, IEnumerable<T> rhs) =>
            List.append(lhs,rhs);

        public static S fold<S, T>(Stck<T> stack, S state, Func<S, T, S> folder) =>
            List.fold(stack, state,folder);

        public static S foldBack<S, T>(Stck<T> stack, S state, Func<S, T, S> folder) =>
            List.foldBack(stack, state, folder);

        public static T reduce<T>(Stck<T> stack, Func<T, T, T> reducer) =>
            List.reduce(stack, reducer);

        public static T reduceBack<T>(Stck<T> stack, Func<T, T, T> reducer) =>
            List.reduceBack(stack, reducer);

        public static IEnumerable<S> scan<S, T>(Stck<T> stack, S state, Func<S, T, S> folder) =>
            List.scan(stack,state,folder);

        public static IEnumerable<S> scanBack<S, T>(Stck<T> stack, S state, Func<S, T, S> folder) =>
            List.scanBack(stack,state,folder);

        public static Option<T> find<T>(Stck<T> stack, Func<T, bool> pred) =>
            List.find(stack, pred);

        public static IEnumerable<V> zip<T, U, V>(Stck<T> stack, IEnumerable<U> other, Func<T, U, V> zipper) =>
            List.zip(stack, other, zipper);

        public static int length<T>(Stck<T> stack) =>
            List.length(stack);

        public static Unit iter<T>(Stck<T> stack, Action<T> action) =>
            List.iter(stack, action);

        public static Unit iter<T>(Stck<T> stack, Action<int, T> action) =>
            List.iter(stack, action);

        public static bool forall<T>(Stck<T> stack, Func<T, bool> pred) =>
            List.forall(stack, pred);

        public static IEnumerable<T> distinct<T>(Stck<T> stack) =>
            List.distinct(stack);

        public static IEnumerable<T> distinct<T>(Stck<T> stack, Func<T, T, bool> compare) =>
            List.distinct(stack, compare);

        public static IEnumerable<T> take<T>(Stck<T> stack, int count) =>
            List.take(stack,count);

        public static IEnumerable<T> takeWhile<T>(Stck<T> stack, Func<T, bool> pred) =>
            List.takeWhile(stack, pred);

        public static IEnumerable<T> takeWhile<T>(Stck<T> stack, Func<T, int, bool> pred) =>
            List.takeWhile(stack, pred);

        public static bool exists<T>(Stck<T> stack, Func<T, bool> pred) =>
            List.exists(stack, pred);
    }
}

public static class __StackExt
{
    public static Tuple<Stck<T>, T> PopUnsafe<T>(this Stck<T> stack) =>
        LanguageExt.Stack.popUnsafe(stack);

    public static Tuple<Stck<T>, Option<T>> Pop<T>(this Stck<T> stack) =>
        LanguageExt.Stack.pop(stack);

    public static T PeekUnsafe<T>(this Stck<T> stack) =>
        LanguageExt.Stack.peekUnsafe(stack);

    public static Option<T> Peek<T>(this Stck<T> stack) =>
        LanguageExt.Stack.peek(stack);

    public static IEnumerable<R> Map<T, R>(this Stck<T> stack, Func<T, R> map) =>
        LanguageExt.List.map(stack, map);

    public static IEnumerable<R> Map<T, R>(this Stck<T> stack, Func<int, T, R> map) =>
        LanguageExt.List.map(stack, map);

    public static IEnumerable<T> Filter<T>(this Stck<T> stack, Func<T, bool> predicate) =>
        LanguageExt.List.filter(stack, predicate);

    public static IEnumerable<T> Choose<T>(this Stck<T> stack, Func<T, Option<T>> selector) =>
        LanguageExt.List.choose(stack, selector);

    public static IEnumerable<T> Choose<T>(this Stck<T> stack, Func<int, T, Option<T>> selector) =>
        LanguageExt.List.choose(stack, selector);

    public static IEnumerable<R> Collect<T, R>(this Stck<T> stack, Func<T, IEnumerable<R>> map) =>
        LanguageExt.List.collect(stack, map);

    public static IEnumerable<T> Rev<T>(this Stck<T> stack) =>
        LanguageExt.List.rev(stack);

    public static IEnumerable<T> Append<T>(this Stck<T> lhs, IEnumerable<T> rhs) =>
        LanguageExt.List.append(lhs, rhs);

    public static S Fold<S, T>(this Stck<T> stack, S state, Func<S, T, S> folder) =>
        LanguageExt.List.fold(stack, state, folder);

    public static S FoldBack<S, T>(this Stck<T> stack, S state, Func<S, T, S> folder) =>
        LanguageExt.List.foldBack(stack, state, folder);

    public static T ReduceBack<T>(Stck<T> stack, Func<T, T, T> reducer) =>
        LanguageExt.List.reduceBack(stack, reducer);

    public static T Reduce<T>(this Stck<T> stack, Func<T, T, T> reducer) =>
        LanguageExt.List.reduce(stack, reducer);

    public static IEnumerable<S> Scan<S, T>(this Stck<T> stack, S state, Func<S, T, S> folder) =>
        LanguageExt.List.scan(stack, state, folder);

    public static IEnumerable<S> ScanBack<S, T>(this Stck<T> stack, S state, Func<S, T, S> folder) =>
        LanguageExt.List.scanBack(stack, state, folder);

    public static Option<T> Find<T>(this Stck<T> stack, Func<T, bool> pred) =>
        LanguageExt.List.find(stack, pred);

    public static int Length<T>(this Stck<T> stack) =>
        LanguageExt.List.length(stack);

    public static Unit Iter<T>(this Stck<T> stack, Action<T> action) =>
        LanguageExt.List.iter(stack, action);

    public static Unit Iter<T>(this Stck<T> stack, Action<int, T> action) =>
        LanguageExt.List.iter(stack, action);

    public static bool ForAll<T>(this Stck<T> stack, Func<T, bool> pred) =>
        LanguageExt.List.forall(stack, pred);

    public static IEnumerable<T> Distinct<T>(Stck<T> stack, Func<T, T, bool> compare) =>
        LanguageExt.List.distinct(stack, compare);

    public static bool Exists<T>(Stck<T> stack, Func<T, bool> pred) =>
        LanguageExt.List.exists(stack, pred);
}
