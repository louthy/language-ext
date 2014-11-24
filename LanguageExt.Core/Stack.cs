using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageExt;
using LanguageExt.Prelude;
using System.Collections.Immutable;

namespace LanguageExt
{
    public static partial class Stack
    {
        public static IImmutableStack<T> push<T>(IImmutableStack<T> stack, T value) =>
            stack.Push(value);

        public static Tuple<IImmutableStack<T>, T> popUnsafe<T>(IImmutableStack<T> stack)
        {
            T value;
            var newStack = stack.Pop(out value);
            return tuple(newStack, value);
        }

        public static T peekUnsafe<T>(IImmutableStack<T> stack) =>
            stack.Peek();

        public static Tuple<IImmutableStack<T>, Option<T>> pop<T>(IImmutableStack<T> stack)
        {
            try
            {
                T value;
                var newStack = stack.Pop(out value);
                return tuple(newStack, Some(value));
            }
            catch (InvalidOperationException)
            {
                return tuple(stack, Option<T>.None);
            }
        }

        public static Option<T> peek<T>(IImmutableStack<T> stack)
        {
            try
            {
                return Some(stack.Peek());
            }
            catch(InvalidOperationException)
            {
                return None;
            }
        }

        public static IImmutableStack<T> clear<T>(IImmutableStack<T> stack) =>
            stack.Clear();

        public static IEnumerable<R> map<T, R>(IImmutableStack<T> stack, Func<int, T, R> map) =>
            List.map(stack, map);

        public static IEnumerable<T> filter<T>(IImmutableStack<T> stack, Func<T, bool> predicate) =>
            List.filter(stack, predicate);

        public static IEnumerable<T> choose<T>(IImmutableStack<T> stack, Func<T, Option<T>> selector) =>
            List.choose(stack, selector);

        public static IEnumerable<T> choose<T>(IImmutableStack<T> stack, Func<int, T, Option<T>> selector) =>
            List.choose(stack, selector);

        public static IEnumerable<R> collect<T, R>(IImmutableStack<T> stack, Func<T, IEnumerable<R>> map) =>
            List.collect(stack, map);

        public static IEnumerable<T> rev<T>(IImmutableStack<T> stack) =>
            List.rev(stack);

        public static IEnumerable<T> append<T>(IEnumerable<T> lhs, IEnumerable<T> rhs) =>
            List.append(lhs,rhs);

        public static S fold<S, T>(IImmutableStack<T> stack, S state, Func<T, S, S> folder) =>
            List.fold(stack, state,folder);

        public static S foldBack<S, T>(IImmutableStack<T> stack, S state, Func<T, S, S> folder) =>
            List.foldBack(stack, state, folder);

        public static T reduce<T>(IImmutableStack<T> stack, Func<T, T, T> reducer) =>
            List.reduce(stack, reducer);

        public static IEnumerable<S> scan<S, T>(IImmutableStack<T> stack, S state, Func<T, S, S> folder) =>
            List.scan(stack,state,folder);

        public static IEnumerable<S> scanBack<S, T>(IImmutableStack<T> stack, S state, Func<T, S, S> folder) =>
            List.scanBack(stack,state,folder);

        public static Option<T> find<T>(IImmutableStack<T> stack, Func<T, bool> pred) =>
            List.find(stack, pred);

        public static IEnumerable<V> zip<T, U, V>(IImmutableStack<T> stack, IEnumerable<U> other, Func<T, U, V> zipper) =>
            List.zip(stack, other, zipper);

        public static int length<T>(IImmutableStack<T> stack) =>
            List.length(stack);

        public static Unit iter<T>(IImmutableStack<T> stack, Action<T> action) =>
            List.iter(stack, action);

        public static Unit iter<T>(IImmutableStack<T> stack, Action<int, T> action) =>
            List.iter(stack, action);

        public static bool forall<T>(IImmutableStack<T> stack, Func<T, bool> pred) =>
            List.forall(stack, pred);

        public static IEnumerable<T> distinct<T>(IImmutableStack<T> stack) =>
            List.distinct(stack);

        public static IEnumerable<T> distinct<T>(IImmutableStack<T> stack, Func<T, T, bool> compare) =>
            List.distinct(stack, compare);

        public static IEnumerable<T> take<T>(IImmutableStack<T> stack, int count) =>
            List.take(stack,count);

        public static IEnumerable<T> takeWhile<T>(IImmutableStack<T> stack, Func<T, bool> pred) =>
            List.takeWhile(stack, pred);

        public static IEnumerable<T> takeWhile<T>(IImmutableStack<T> stack, Func<T, int, bool> pred) =>
            List.takeWhile(stack, pred);

        public static bool exists<T>(IImmutableStack<T> stack, Func<T, bool> pred) =>
            List.exists(stack, pred);
    }
}

public static class __StackExt
{
    public static Tuple<IImmutableStack<T>, T> PopUnsafe<T>(this IImmutableStack<T> stack) =>
        Stack.popUnsafe(stack);

    public static Tuple<IImmutableStack<T>, Option<T>> Pop<T>(this IImmutableStack<T> stack) =>
        Stack.pop(stack);

    public static T PeekUnsafe<T>(this IImmutableStack<T> stack) =>
        Stack.peekUnsafe(stack);

    public static Option<T> Peek<T>(this IImmutableStack<T> stack) =>
        Stack.peek(stack);

    public static IEnumerable<R> Map<T, R>(this IImmutableStack<T> list, Func<T, R> map) =>
        List.map(list, map);

    public static IEnumerable<R> Map<T, R>(this IImmutableStack<T> list, Func<int, T, R> map) =>
        List.map(list, map);

    public static IEnumerable<T> Filter<T>(this IImmutableStack<T> list, Func<T, bool> predicate) =>
        List.filter(list, predicate);

    public static IEnumerable<T> Choose<T>(this IImmutableStack<T> list, Func<T, Option<T>> selector) =>
        List.choose(list, selector);

    public static IEnumerable<T> Choose<T>(this IImmutableStack<T> list, Func<int, T, Option<T>> selector) =>
        List.choose(list, selector);

    public static IEnumerable<R> Collect<T, R>(this IImmutableStack<T> list, Func<T, IEnumerable<R>> map) =>
        List.collect(list, map);

    public static IEnumerable<T> Rev<T>(this IImmutableStack<T> list) =>
        List.rev(list);

    public static IEnumerable<T> Append<T>(this IImmutableStack<T> lhs, IEnumerable<T> rhs) =>
        List.append(lhs, rhs);

    public static S Fold<S, T>(this IImmutableStack<T> list, S state, Func<T, S, S> folder) =>
        List.fold(list, state, folder);

    public static S FoldBack<S, T>(this IImmutableStack<T> list, S state, Func<T, S, S> folder) =>
        List.foldBack(list, state, folder);

    public static T Reduce<T>(this IImmutableStack<T> list, Func<T, T, T> reducer) =>
        List.reduce(list, reducer);

    public static IEnumerable<S> Scan<S, T>(this IImmutableStack<T> list, S state, Func<T, S, S> folder) =>
        List.scan(list, state, folder);

    public static IEnumerable<S> ScanBack<S, T>(this IImmutableStack<T> list, S state, Func<T, S, S> folder) =>
        List.scanBack(list, state, folder);

    public static Option<T> Find<T>(this IImmutableStack<T> list, Func<T, bool> pred) =>
        List.find(list, pred);

    public static int Length<T>(this IImmutableStack<T> list) =>
        List.length(list);

    public static Unit Iter<T>(this IImmutableStack<T> list, Action<T> action) =>
        List.iter(list, action);

    public static Unit Iter<T>(this IImmutableStack<T> list, Action<int, T> action) =>
        List.iter(list, action);

    public static bool ForAll<T>(this IImmutableStack<T> list, Func<T, bool> pred) =>
        List.forall(list, pred);

    public static IEnumerable<T> Distinct<T>(IImmutableStack<T> list, Func<T, T, bool> compare) =>
        List.distinct(list, compare);

    public static bool Exists<T>(IImmutableStack<T> list, Func<T, bool> pred) =>
        List.exists(list, pred);
}
