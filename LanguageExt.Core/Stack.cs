using System;
using System.Collections.Generic;
using LanguageExt;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;

namespace LanguageExt
{
    /// <summary>
    /// Functional module for working with the Stck T type
    /// </summary>
    public static class Stack
    {
        /// <summary>
        /// Reverses the order of the items in the stack
        /// </summary>
        /// <returns></returns>
        [Pure]
        public static Stck<T> rev<T>(Stck<T> stack) =>
            stack.Reverse();

        /// <summary>
        /// True if the stack is empty
        /// </summary>
        [Pure]
        public static bool isEmpty<T>(Stck<T> stack) =>
            stack.IsEmpty;

        /// <summary>
        /// Clear the stack (returns Empty)
        /// </summary>
        /// <returns>Stck.Empty of T</returns>
        [Pure]
        public static Stck<T> clear<T>(Stck<T> stack) =>
            stack.Clear();

        /// <summary>
        /// Return the item on the top of the stack without affecting the stack itself
        /// NOTE: Will throw an InvalidOperationException if the stack is empty
        /// </summary>
        /// <exception cref="InvalidOperationException">Stack is empty</exception>
        /// <returns>Top item value</returns>
        [Pure]
        public static T peek<T>(Stck<T> stack) =>
            stack.Peek();

        /// <summary>
        /// Peek and match
        /// </summary>
        /// <param name="Some">Handler if there is a value on the top of the stack</param>
        /// <param name="None">Handler if the stack is empty</param>
        /// <returns>Untouched stack</returns>
        [Pure]
        public static Stck<T> peek<T>(Stck<T> stack, Action<T> Some, Action None) =>
            stack.Peek(Some, None);

        /// <summary>
        /// Peek and match
        /// </summary>
        /// <typeparam name="R">Return type</typeparam>
        /// <param name="Some">Handler if there is a value on the top of the stack</param>
        /// <param name="None">Handler if the stack is empty</param>
        /// <returns>Return value from Some or None</returns>
        [Pure]
        public static R peek<T, R>(Stck<T> stack, Func<T, R> Some, Func<R> None) =>
            stack.Peek(Some, None);

        /// <summary>
        /// Safely return the item on the top of the stack without affecting the stack itself
        /// </summary>
        /// <returns>Returns the top item value, or None</returns>
        [Pure]
        public static Option<T> trypeek<T>(Stck<T> stack) =>
            stack.TryPeek();

        /// <summary>
        /// Pop an item off the top of the stack
        /// NOTE: Will throw an InvalidOperationException if the stack is empty
        /// </summary>
        /// <exception cref="InvalidOperationException">Stack is empty</exception>
        /// <returns>Stack with the top item popped</returns>
        [Pure]
        public static Stck<T> pop<T>(Stck<T> stack) =>
            stack.Pop();

        /// <summary>
        /// Safe pop
        /// </summary>
        /// <returns>Tuple of popped stack and optional top-of-stack value</returns>
        [Pure]
        public static Tuple<Stck<T>, Option<T>> trypop<T>(Stck<T> stack) =>
            stack.TryPop();

        /// <summary>
        /// Pop and match
        /// </summary>
        /// <param name="Some">Handler if there is a value on the top of the stack</param>
        /// <param name="None">Handler if the stack is empty</param>
        /// <returns>Popped stack</returns>
        [Pure]
        public static Stck<T> pop<T>(Stck<T> stack, Action<T> Some, Action None) =>
            stack.Pop(Some, None);

        /// <summary>
        /// Pop and match
        /// </summary>
        /// <typeparam name="R">Return type</typeparam>
        /// <param name="Some">Handler if there is a value on the top of the stack</param>
        /// <param name="None">Handler if the stack is empty</param>
        /// <returns>Return value from Some or None</returns>
        [Pure]
        public static R pop<T, R>(Stck<T> stack, Func<Stck<T>, T, R> Some, Func<R> None) =>
            stack.Pop(Some, None);

        /// <summary>
        /// Push an item onto the stack
        /// </summary>
        /// <param name="value">Item to push</param>
        /// <returns>New stack with the pushed item on top</returns>
        [Pure]
        public static Stck<T> push<T>(Stck<T> stack, T value) =>
            stack.Push(value);

        /// <summary>
        /// Projects the values in the stack using a map function into a new enumerable (Select in LINQ).
        /// </summary>
        /// <typeparam name="T">Stack item type</typeparam>
        /// <typeparam name="R">Return enumerable item type</typeparam>
        /// <param name="stack">Stack to map</param>
        /// <param name="map">Map function</param>
        /// <returns>Mapped enumerable</returns>
        [Pure]
        public static IEnumerable<R> map<T, R>(Stck<T> stack, Func<T, R> map) =>
            List.map(stack, map);

        /// <summary>
        /// Projects the values in the stack using a map function into a new enumerable (Select in LINQ).
        /// </summary>
        /// <typeparam name="T">Stack item type</typeparam>
        /// <typeparam name="R">Return enumerable item type</typeparam>
        /// <param name="stack">Stack to map</param>
        /// <param name="map">Map function</param>
        /// <returns>Mapped enumerable</returns>
        [Pure]
        public static IEnumerable<R> map<T, R>(Stck<T> stack, Func<int, T, R> map) =>
            List.map(stack, map);

        /// <summary>
        /// Removes items from the stack that do not match the given predicate (Where in LINQ)
        /// </summary>
        /// <typeparam name="T">Stack item type</typeparam>
        /// <param name="stack">Stack to filter</param>
        /// <param name="predicate">Predicate function</param>
        /// <returns>Filtered stack</returns>
        [Pure]
        public static IEnumerable<T> filter<T>(Stck<T> stack, Func<T, bool> predicate) =>
            List.filter(stack, predicate);

        /// <summary>
        /// Applies the given function 'selector' to each element of the stack. Returns an enumerable comprised of 
        /// the results for each element where the function returns Some(f(x)).
        /// </summary>
        /// <typeparam name="T">Stack item type</typeparam>
        /// <param name="stack">Stack</param>
        /// <param name="selector">Selector function</param>
        /// <returns>Mapped and filtered enumerable</returns>
        [Pure]
        public static IEnumerable<T> choose<T>(Stck<T> stack, Func<T, Option<T>> selector) =>
            List.choose(stack, selector);

        /// <summary>
        /// Applies the given function 'selector' to each element of the stack. Returns an enumerable comprised of 
        /// the results for each element where the function returns Some(f(x)).
        /// An index value is passed through to the selector function also.
        /// </summary>
        /// <typeparam name="T">Stack item type</typeparam>
        /// <param name="stack">Stack</param>
        /// <param name="selector">Selector function</param>
        /// <returns>Mapped and filtered enumerable</returns>
        [Pure]
        public static IEnumerable<T> choose<T>(Stck<T> stack, Func<int, T, Option<T>> selector) =>
            List.choose(stack, selector);

        /// <summary>
        /// For each element of the stack, applies the given function. Concatenates all the results and 
        /// returns the combined list.
        /// </summary>
        /// <typeparam name="T">Stack item type</typeparam>
        /// <typeparam name="R">Return enumerable item type</typeparam>
        /// <param name="stack">Stack to map</param>
        /// <param name="map">Map function</param>
        /// <returns>Mapped enumerable</returns>
        [Pure]
        public static IEnumerable<R> collect<T, R>(Stck<T> stack, Func<T, IEnumerable<R>> map) =>
            List.collect(stack, map);

        /// <summary>
        /// Append another stack to the top of this stack
        /// The rhs will be reversed and pushed onto 'this' stack.  That will
        /// maintain the order of the items in the resulting stack.  So the top
        /// of 'rhs' will be the top of the newly created stack.  'this' stack
        /// will be under the 'rhs' stack.
        /// </summary>
        /// <param name="rhs">Stack to append</param>
        /// <returns>Appended stacks</returns>
        [Pure]
        public static IEnumerable<T> append<T>(IEnumerable<T> lhs, IEnumerable<T> rhs) =>
            List.append(lhs,rhs);

        /// <summary>
        /// Applies a function 'folder' to each element of the collection, threading an accumulator 
        /// argument through the computation. The fold function takes the state argument, and 
        /// applies the function 'folder' to it and the first element of the stack. Then, it feeds this 
        /// result into the function 'folder' along with the second element, and so on. It returns the 
        /// final result. (Aggregate in LINQ)
        /// </summary>
        /// <typeparam name="S">State type</typeparam>
        /// <typeparam name="T">Stack item type</typeparam>
        /// <param name="stack">Stack to fold</param>
        /// <param name="state">Initial state</param>
        /// <param name="folder">Fold function</param>
        /// <returns>Aggregate value</returns>
        [Pure]
        public static S fold<S, T>(Stck<T> stack, S state, Func<S, T, S> folder) =>
            List.fold(stack, state,folder);

        /// <summary>
        /// Applies a function 'folder' to each element of the collection (from last element to first), 
        /// threading an aggregate state through the computation. The fold function takes the state 
        /// argument, and applies the function 'folder' to it and the first element of the stack. Then, 
        /// it feeds this result into the function 'folder' along with the second element, and so on. It 
        /// returns the final result.
        /// </summary>
        /// <typeparam name="S">State type</typeparam>
        /// <typeparam name="T">Stack item type</typeparam>
        /// <param name="stack">Stack to fold</param>
        /// <param name="state">Initial state</param>
        /// <param name="folder">Fold function</param>
        /// <returns>Aggregate value</returns>
        [Pure]
        public static S foldBack<S, T>(Stck<T> stack, S state, Func<S, T, S> folder) =>
            List.foldBack(stack, state, folder);

        /// <summary>
        /// Applies a function 'folder' to each element of the collection whilst the predicate function 
        /// returns true for the item being processed, threading an aggregate state through the 
        /// computation. The fold function takes the state argument, and applies the function 'folder' 
        /// to it and the first element of the stack. Then, it feeds this result into the function 'folder' 
        /// along with the second element, and so on. It returns the final result.
        /// </summary>
        /// <typeparam name="S">State type</typeparam>
        /// <typeparam name="T">Stack item type</typeparam>
        /// <param name="stack">Stack to fold</param>
        /// <param name="state">Initial state</param>
        /// <param name="folder">Fold function</param>
        /// <param name="pred">Predicate function</param>
        /// <returns>Aggregate value</returns>
        [Pure]
        public static S foldWhile<S, T>(Stck<T> stack, S state, Func<S, T, S> folder, Func<T, bool> pred) =>
            List.foldWhile(stack, state, folder, pred);

        /// <summary>
        /// Applies a function 'folder' to each element of the collection, threading an accumulator 
        /// argument through the computation (and whilst the predicate function returns true when passed 
        /// the aggregate state). The fold function takes the state argument, and applies the function 
        /// 'folder' to it and the first element of the stack. Then, it feeds this result into the 
        /// function 'folder' along with the second element, and so on. It returns the final result. 
        /// </summary>
        /// <typeparam name="S">State type</typeparam>
        /// <typeparam name="T">Stack item type</typeparam>
        /// <param name="stack">Stack to fold</param>
        /// <param name="state">Initial state</param>
        /// <param name="folder">Fold function</param>
        /// <param name="pred">Predicate function</param>
        /// <returns>Aggregate value</returns>
        [Pure]
        public static S foldWhile<S, T>(Stck<T> stack, S state, Func<S, T, S> folder, Func<S, bool> pred) =>
            List.foldWhile(stack, state, folder, pred);

        /// <summary>
        /// Applies a function 'folder' to each element of the collection (from last element to first)
        /// whilst the predicate function returns true for the item being processed, threading an 
        /// aggregate state through the computation. The fold function takes the state argument, and 
        /// applies the function 'folder' to it and the first element of the stack. Then, it feeds this 
        /// result into the function 'folder' along with the second element, and so on. It returns the 
        /// final result.
        /// </summary>
        /// <typeparam name="S">State type</typeparam>
        /// <typeparam name="T">Stack item type</typeparam>
        /// <param name="stack">Stack to fold</param>
        /// <param name="state">Initial state</param>
        /// <param name="folder">Fold function</param>
        /// <param name="pred">Predicate function</param>
        /// <returns>Aggregate value</returns>
        [Pure]
        public static S foldBackWhile<S, T>(Stck<T> stack, S state, Func<S, T, S> folder, Func<T, bool> pred) =>
            List.foldBackWhile(stack, state, folder, pred);

        /// <summary>
        /// Applies a function 'folder' to each element of the collection (from last element to first), 
        /// threading an accumulator argument through the computation (and whilst the predicate function 
        /// returns true when passed the aggregate state). The fold function takes the state argument, 
        /// and applies the function 'folder' to it and the first element of the stack. Then, it feeds 
        /// this result into the function 'folder' along with the second element, and so on. It returns 
        /// the final result.
        /// </summary>
        /// <typeparam name="S">State type</typeparam>
        /// <typeparam name="T">Stack item type</typeparam>
        /// <param name="stack">Stack to fold</param>
        /// <param name="state">Initial state</param>
        /// <param name="folder">Fold function</param>
        /// <param name="pred">Predicate function</param>
        /// <returns>Aggregate value</returns>
        [Pure]
        public static S foldBackWhile<S, T>(Stck<T> stack, S state, Func<S, T, S> folder, Func<S, bool> pred) =>
            List.foldBackWhile(stack, state, folder, pred);

        /// <summary>
        /// Applies a function to each element of the collection (from last element to first), threading 
        /// an accumulator argument through the computation. This function first applies the function 
        /// to the first two elements of the stack. Then, it passes this result into the function along 
        /// with the third element and so on. Finally, it returns the final result.
        /// </summary>
        /// <typeparam name="T">Stack item type</typeparam>
        /// <param name="stack">Stack to fold</param>
        /// <param name="reducer">Reduce function</param>
        /// <returns>Aggregate value</returns>
        [Pure]
        public static T reduce<T>(Stck<T> stack, Func<T, T, T> reducer) =>
            List.reduce(stack, reducer);

        /// <summary>
        /// Applies a function to each element of the collection, threading an accumulator argument 
        /// through the computation. This function first applies the function to the first two 
        /// elements of the stack. Then, it passes this result into the function along with the third 
        /// element and so on. Finally, it returns the final result.
        /// </summary>
        /// <typeparam name="T">Stack item type</typeparam>
        /// <param name="stack">Stack</param>
        /// <param name="reducer">Reduce function</param>
        /// <returns>Aggregate value</returns>
        [Pure]
        public static T reduceBack<T>(Stck<T> stack, Func<T, T, T> reducer) =>
            List.reduceBack(stack, reducer);

        /// <summary>
        /// Applies a function to each element of the collection, threading an accumulator argument 
        /// through the computation. This function takes the state argument, and applies the function 
        /// to it and the first element of the stack. Then, it passes this result into the function 
        /// along with the second element, and so on. Finally, it returns the list of intermediate 
        /// results and the final result.
        /// </summary>
        /// <typeparam name="S">State type</typeparam>
        /// <typeparam name="T">Stack item type</typeparam>
        /// <param name="stack">Stack</param>
        /// <param name="state">Initial state</param>
        /// <param name="folder">Folding function</param>
        /// <returns>Aggregate state</returns>
        [Pure]
        public static IEnumerable<S> scan<S, T>(Stck<T> stack, S state, Func<S, T, S> folder) =>
            List.scan(stack,state,folder);

        /// <summary>
        /// Applies a function to each element of the collection (from last element to first), 
        /// threading an accumulator argument through the computation. This function takes the state 
        /// argument, and applies the function to it and the first element of the stack. Then, it 
        /// passes this result into the function along with the second element, and so on. Finally, 
        /// it returns the list of intermediate results and the final result.
        /// </summary>
        /// <typeparam name="S">State type</typeparam>
        /// <typeparam name="T">Stack item type</typeparam>
        /// <param name="stack">Stack</param>
        /// <param name="state">Initial state</param>
        /// <param name="folder">Folding function</param>
        /// <returns>Aggregate state</returns>
        [Pure]
        public static IEnumerable<S> scanBack<S, T>(Stck<T> stack, S state, Func<S, T, S> folder) =>
            List.scanBack(stack,state,folder);

        /// <summary>
        /// Returns Some(x) for the first item in the stack that matches the predicate 
        /// provided, None otherwise.
        /// </summary>
        /// <typeparam name="T">Stack item type</typeparam>
        /// <param name="stack">Stack</param>
        /// <param name="pred">Predicate</param>
        /// <returns>Some(x) for the first item in the stack that matches the predicate 
        /// provided, None otherwise.</returns>
        [Pure]
        public static Option<T> find<T>(Stck<T> stack, Func<T, bool> pred) =>
            List.find(stack, pred);

        /// <summary>
        /// Joins a stack and and enumerable together either into a single enumerable
        /// using the join function provided
        /// </summary>
        /// <param name="stack">First stack to join</param>
        /// <param name="other">Second list to join</param>
        /// <param name="zipper">Join function</param>
        /// <returns>Joined enumerable</returns>
        [Pure]
        public static IEnumerable<V> zip<T, U, V>(Stck<T> stack, IEnumerable<U> other, Func<T, U, V> zipper) =>
            List.zip(stack, other, zipper);

        /// <summary>
        /// Returns the number of items in the stack
        /// </summary>
        /// <typeparam name="T">Stack item type</typeparam>
        /// <param name="stack">Stack</param>
        /// <returns>The number of items in the enumerable</returns>
        [Pure]
        public static int length<T>(Stck<T> stack) =>
            List.length(stack);

        /// <summary>
        /// Invokes an action for each item in the stack in order
        /// </summary>
        /// <typeparam name="T">Stack item type</typeparam>
        /// <param name="stack">Stack to iterate</param>
        /// <param name="action">Action to invoke with each item</param>
        /// <returns>Unit</returns>
        public static Unit iter<T>(Stck<T> stack, Action<T> action) =>
            List.iter(stack, action);

        /// <summary>
        /// Invokes an action for each item in the stack in order and supplies
        /// a running index value.
        /// </summary>
        /// <typeparam name="T">Stack item type</typeparam>
        /// <param name="stack">Stack to iterate</param>
        /// <param name="action">Action to invoke with each item</param>
        /// <returns>Unit</returns>
        public static Unit iter<T>(Stck<T> stack, Action<int, T> action) =>
            List.iter(stack, action);

        /// <summary>
        /// Returns true if all items in the stack match a predicate (Any in LINQ)
        /// </summary>
        /// <typeparam name="T">Stack item type</typeparam>
        /// <param name="stack">Stack to test</param>
        /// <param name="pred">Predicate</param>
        /// <returns>True if all items in the stack match the predicate</returns>
        [Pure]
        public static bool forall<T>(Stck<T> stack, Func<T, bool> pred) =>
            List.forall(stack, pred);

        /// <summary>
        /// Return an enumerable with all duplicate values removed
        /// </summary>
        /// <typeparam name="T">Stack item type</typeparam>
        /// <param name="stack">Stack</param>
        /// <returns>An enumerable with all duplicate values removed</returns>
        [Pure]
        public static IEnumerable<T> distinct<T>(Stck<T> stack) =>
            List.distinct(stack);

        /// <summary>
        /// Return an enumerable with all duplicate values removed
        /// </summary>
        /// <typeparam name="T">Stack item type</typeparam>
        /// <param name="stack">Stack</param>
        /// <returns>An enumerable with all duplicate values removed</returns>
        [Pure]
        public static IEnumerable<T> distinct<T>(Stck<T> stack, Func<T, T, bool> compare) =>
            List.distinct(stack, compare);

        /// <summary>
        /// Returns a new enumerable with the first 'count' items from the stack
        /// </summary>
        /// <typeparam name="T">Stack item type</typeparam>
        /// <param name="stack">Stack</param>
        /// <param name="count">Number of items to take</param>
        /// <returns>A new enumerable with the first 'count' items from the enumerable provided</returns>
        [Pure]
        public static IEnumerable<T> take<T>(Stck<T> stack, int count) =>
            List.take(stack,count);

        /// <summary>
        /// Iterate the stack, yielding items if they match the predicate provided, and stopping 
        /// as soon as one doesn't
        /// </summary>
        /// <typeparam name="T">Stack item type</typeparam>
        /// <param name="stack">Stack</param>
        /// <param name="count">Number of items to take</param>
        /// <returns>A new enumerable with the first items that match the predicate</returns>
        [Pure]
        public static IEnumerable<T> takeWhile<T>(Stck<T> stack, Func<T, bool> pred) =>
            List.takeWhile(stack, pred);

        /// <summary>
        /// Iterate the stack, yielding items if they match the predicate provided, and stopping 
        /// as soon as one doesn't  An index value is also provided to the predicate function.
        /// </summary>
        /// <typeparam name="T">Stack item type</typeparam>
        /// <param name="stack">Stack</param>
        /// <param name="count">Number of items to take</param>
        /// <returns>A new enumerable with the first items that match the predicate</returns>
        [Pure]
        public static IEnumerable<T> takeWhile<T>(Stck<T> stack, Func<T, int, bool> pred) =>
            List.takeWhile(stack, pred);

        /// <summary>
        /// Returns true if any item in the stack matches the predicate provided
        /// </summary>
        /// <typeparam name="T">Stack item type</typeparam>
        /// <param name="stack">Stack</param>
        /// <param name="pred">Predicate</param>
        /// <returns>True if any item in the stack matches the predicate provided</returns>
        [Pure]
        public static bool exists<T>(Stck<T> stack, Func<T, bool> pred) =>
            List.exists(stack, pred);
    }
}

public static class __StackExt
{
    /// <summary>
    /// Projects the values in the stack using a map function into a new enumerable (Select in LINQ).
    /// </summary>
    /// <typeparam name="T">Stack item type</typeparam>
    /// <typeparam name="R">Return enumerable item type</typeparam>
    /// <param name="stack">Stack to map</param>
    /// <param name="map">Map function</param>
    /// <returns>Mapped enumerable</returns>
    [Pure]
    public static IEnumerable<R> Map<T, R>(this Stck<T> stack, Func<T, R> map) =>
        LanguageExt.List.map(stack, map);

    /// <summary>
    /// Projects the values in the stack using a map function into a new enumerable (Select in LINQ).
    /// </summary>
    /// <typeparam name="T">Stack item type</typeparam>
    /// <typeparam name="R">Return enumerable item type</typeparam>
    /// <param name="stack">Stack to map</param>
    /// <param name="map">Map function</param>
    /// <returns>Mapped enumerable</returns>
    [Pure]
    public static IEnumerable<R> Map<T, R>(this Stck<T> stack, Func<int, T, R> map) =>
        LanguageExt.List.map(stack, map);

    /// <summary>
    /// Removes items from the stack that do not match the given predicate (Where in LINQ)
    /// </summary>
    /// <typeparam name="T">Stack item type</typeparam>
    /// <param name="stack">Stack to filter</param>
    /// <param name="predicate">Predicate function</param>
    /// <returns>Filtered stack</returns>
    [Pure]
    public static IEnumerable<T> Filter<T>(this Stck<T> stack, Func<T, bool> predicate) =>
        LanguageExt.List.filter(stack, predicate);

    /// <summary>
    /// Applies the given function 'selector' to each element of the stack. Returns an enumerable comprised of 
    /// the results for each element where the function returns Some(f(x)).
    /// </summary>
    /// <typeparam name="T">Stack item type</typeparam>
    /// <param name="stack">Stack</param>
    /// <param name="selector">Selector function</param>
    /// <returns>Mapped and filtered enumerable</returns>
    [Pure]
    public static IEnumerable<T> Choose<T>(this Stck<T> stack, Func<T, Option<T>> selector) =>
        LanguageExt.List.choose(stack, selector);

    /// <summary>
    /// Applies the given function 'selector' to each element of the stack. Returns an enumerable comprised of 
    /// the results for each element where the function returns Some(f(x)).
    /// An index value is passed through to the selector function also.
    /// </summary>
    /// <typeparam name="T">Stack item type</typeparam>
    /// <param name="stack">Stack</param>
    /// <param name="selector">Selector function</param>
    /// <returns>Mapped and filtered enumerable</returns>
    [Pure]
    public static IEnumerable<T> Choose<T>(this Stck<T> stack, Func<int, T, Option<T>> selector) =>
        LanguageExt.List.choose(stack, selector);

    /// <summary>
    /// For each element of the stack, applies the given function. Concatenates all the results and 
    /// returns the combined list.
    /// </summary>
    /// <typeparam name="T">Stack item type</typeparam>
    /// <typeparam name="R">Return enumerable item type</typeparam>
    /// <param name="stack">Stack to map</param>
    /// <param name="map">Map function</param>
    /// <returns>Mapped enumerable</returns>
    [Pure]
    public static IEnumerable<R> Collect<T, R>(this Stck<T> stack, Func<T, IEnumerable<R>> map) =>
        LanguageExt.List.collect(stack, map);

    /// <summary>
    /// Reverses the order of the items in the stack
    /// </summary>
    /// <returns></returns>
    [Pure]
    public static IEnumerable<T> Rev<T>(this Stck<T> stack) =>
        LanguageExt.List.rev(stack);

    /// <summary>
    /// Applies a function 'folder' to each element of the collection, threading an accumulator 
    /// argument through the computation. The fold function takes the state argument, and 
    /// applies the function 'folder' to it and the first element of the stack. Then, it feeds this 
    /// result into the function 'folder' along with the second element, and so on. It returns the 
    /// final result. (Aggregate in LINQ)
    /// </summary>
    /// <typeparam name="S">State type</typeparam>
    /// <typeparam name="T">Stack item type</typeparam>
    /// <param name="stack">Stack to fold</param>
    /// <param name="state">Initial state</param>
    /// <param name="folder">Fold function</param>
    /// <returns>Aggregate value</returns>
    [Pure]
    public static S Fold<S, T>(this Stck<T> stack, S state, Func<S, T, S> folder) =>
        LanguageExt.List.fold(stack, state, folder);

    /// <summary>
    /// Applies a function 'folder' to each element of the collection (from last element to first), 
    /// threading an aggregate state through the computation. The fold function takes the state 
    /// argument, and applies the function 'folder' to it and the first element of the stack. Then, 
    /// it feeds this result into the function 'folder' along with the second element, and so on. It 
    /// returns the final result.
    /// </summary>
    /// <typeparam name="S">State type</typeparam>
    /// <typeparam name="T">Stack item type</typeparam>
    /// <param name="stack">Stack to fold</param>
    /// <param name="state">Initial state</param>
    /// <param name="folder">Fold function</param>
    /// <returns>Aggregate value</returns>
    [Pure]
    public static S FoldBack<S, T>(this Stck<T> stack, S state, Func<S, T, S> folder) =>
        LanguageExt.List.foldBack(stack, state, folder);

    /// <summary>
    /// Applies a function 'folder' to each element of the collection whilst the predicate function 
    /// returns true for the item being processed, threading an aggregate state through the 
    /// computation. The fold function takes the state argument, and applies the function 'folder' 
    /// to it and the first element of the stack. Then, it feeds this result into the function 'folder' 
    /// along with the second element, and so on. It returns the final result.
    /// </summary>
    /// <typeparam name="S">State type</typeparam>
    /// <typeparam name="T">Stack item type</typeparam>
    /// <param name="stack">Stack to fold</param>
    /// <param name="state">Initial state</param>
    /// <param name="folder">Fold function</param>
    /// <param name="pred">Predicate function</param>
    /// <returns>Aggregate value</returns>
    [Pure]
    public static S FoldWhile<S, T>(this Stck<T> stack, S state, Func<S, T, S> folder, Func<T, bool> pred) =>
        LanguageExt.List.foldWhile(stack, state, folder, pred);

    /// <summary>
    /// Applies a function 'folder' to each element of the collection, threading an accumulator 
    /// argument through the computation (and whilst the predicate function returns true when passed 
    /// the aggregate state). The fold function takes the state argument, and applies the function 
    /// 'folder' to it and the first element of the stack. Then, it feeds this result into the 
    /// function 'folder' along with the second element, and so on. It returns the final result. 
    /// </summary>
    /// <typeparam name="S">State type</typeparam>
    /// <typeparam name="T">Stack item type</typeparam>
    /// <param name="stack">Stack to fold</param>
    /// <param name="state">Initial state</param>
    /// <param name="folder">Fold function</param>
    /// <param name="pred">Predicate function</param>
    /// <returns>Aggregate value</returns>
    [Pure]
    public static S FoldWhile<S, T>(this Stck<T> stack, S state, Func<S, T, S> folder, Func<S, bool> pred) =>
        LanguageExt.List.foldWhile(stack, state, folder, pred);

    /// <summary>
    /// Applies a function 'folder' to each element of the collection (from last element to first)
    /// whilst the predicate function returns true for the item being processed, threading an 
    /// aggregate state through the computation. The fold function takes the state argument, and 
    /// applies the function 'folder' to it and the first element of the stack. Then, it feeds this 
    /// result into the function 'folder' along with the second element, and so on. It returns the 
    /// final result.
    /// </summary>
    /// <typeparam name="S">State type</typeparam>
    /// <typeparam name="T">Stack item type</typeparam>
    /// <param name="stack">Stack to fold</param>
    /// <param name="state">Initial state</param>
    /// <param name="folder">Fold function</param>
    /// <param name="pred">Predicate function</param>
    /// <returns>Aggregate value</returns>
    [Pure]
    public static S FoldBackWhile<S, T>(this Stck<T> stack, S state, Func<S, T, S> folder, Func<T, bool> pred) =>
        LanguageExt.List.foldBackWhile(stack, state, folder, pred);

    /// <summary>
    /// Applies a function 'folder' to each element of the collection (from last element to first), 
    /// threading an accumulator argument through the computation (and whilst the predicate function 
    /// returns true when passed the aggregate state). The fold function takes the state argument, 
    /// and applies the function 'folder' to it and the first element of the stack. Then, it feeds 
    /// this result into the function 'folder' along with the second element, and so on. It returns 
    /// the final result.
    /// </summary>
    /// <typeparam name="S">State type</typeparam>
    /// <typeparam name="T">Stack item type</typeparam>
    /// <param name="stack">Stack to fold</param>
    /// <param name="state">Initial state</param>
    /// <param name="folder">Fold function</param>
    /// <param name="pred">Predicate function</param>
    /// <returns>Aggregate value</returns>
    [Pure]
    public static S FoldBackWhile<S, T>(this Stck<T> stack, S state, Func<S, T, S> folder, Func<S, bool> pred) =>
        LanguageExt.List.foldBackWhile(stack, state, folder, pred);

    /// <summary>
    /// Applies a function to each element of the collection, threading an accumulator argument 
    /// through the computation. This function first applies the function to the first two 
    /// elements of the stack. Then, it passes this result into the function along with the third 
    /// element and so on. Finally, it returns the final result.
    /// </summary>
    /// <typeparam name="T">Stack item type</typeparam>
    /// <param name="stack">Stack</param>
    /// <param name="reducer">Reduce function</param>
    /// <returns>Aggregate value</returns>
    [Pure]
    public static T ReduceBack<T>(Stck<T> stack, Func<T, T, T> reducer) =>
        LanguageExt.List.reduceBack(stack, reducer);

    /// <summary>
    /// Applies a function to each element of the collection (from last element to first), threading 
    /// an accumulator argument through the computation. This function first applies the function 
    /// to the first two elements of the stack. Then, it passes this result into the function along 
    /// with the third element and so on. Finally, it returns the final result.
    /// </summary>
    /// <typeparam name="T">Stack item type</typeparam>
    /// <param name="stack">Stack to fold</param>
    /// <param name="reducer">Reduce function</param>
    /// <returns>Aggregate value</returns>
    [Pure]
    public static T Reduce<T>(this Stck<T> stack, Func<T, T, T> reducer) =>
        LanguageExt.List.reduce(stack, reducer);

    /// <summary>
    /// Applies a function to each element of the collection, threading an accumulator argument 
    /// through the computation. This function takes the state argument, and applies the function 
    /// to it and the first element of the stack. Then, it passes this result into the function 
    /// along with the second element, and so on. Finally, it returns the list of intermediate 
    /// results and the final result.
    /// </summary>
    /// <typeparam name="S">State type</typeparam>
    /// <typeparam name="T">Stack item type</typeparam>
    /// <param name="stack">Stack</param>
    /// <param name="state">Initial state</param>
    /// <param name="folder">Folding function</param>
    /// <returns>Aggregate state</returns>
    [Pure]
    public static IEnumerable<S> Scan<S, T>(this Stck<T> stack, S state, Func<S, T, S> folder) =>
        LanguageExt.List.scan(stack, state, folder);

    /// <summary>
    /// Applies a function to each element of the collection (from last element to first), 
    /// threading an accumulator argument through the computation. This function takes the state 
    /// argument, and applies the function to it and the first element of the stack. Then, it 
    /// passes this result into the function along with the second element, and so on. Finally, 
    /// it returns the list of intermediate results and the final result.
    /// </summary>
    /// <typeparam name="S">State type</typeparam>
    /// <typeparam name="T">Stack item type</typeparam>
    /// <param name="stack">Stack</param>
    /// <param name="state">Initial state</param>
    /// <param name="folder">Folding function</param>
    /// <returns>Aggregate state</returns>
    [Pure]
    public static IEnumerable<S> ScanBack<S, T>(this Stck<T> stack, S state, Func<S, T, S> folder) =>
        LanguageExt.List.scanBack(stack, state, folder);

    /// <summary>
    /// Returns Some(x) for the first item in the stack that matches the predicate 
    /// provided, None otherwise.
    /// </summary>
    /// <typeparam name="T">Stack item type</typeparam>
    /// <param name="stack">Stack</param>
    /// <param name="pred">Predicate</param>
    /// <returns>Some(x) for the first item in the stack that matches the predicate 
    /// provided, None otherwise.</returns>
    [Pure]
    public static Option<T> Find<T>(this Stck<T> stack, Func<T, bool> pred) =>
        LanguageExt.List.find(stack, pred);

    /// <summary>
    /// Returns the number of items in the stack
    /// </summary>
    /// <typeparam name="T">Stack item type</typeparam>
    /// <param name="stack">Stack</param>
    /// <returns>The number of items in the enumerable</returns>
    [Pure]
    public static int Length<T>(this Stck<T> stack) =>
        LanguageExt.List.length(stack);

    /// <summary>
    /// Invokes an action for each item in the stack in order
    /// </summary>
    /// <typeparam name="T">Stack item type</typeparam>
    /// <param name="stack">Stack to iterate</param>
    /// <param name="action">Action to invoke with each item</param>
    /// <returns>Unit</returns>
    public static Unit Iter<T>(this Stck<T> stack, Action<T> action) =>
        LanguageExt.List.iter(stack, action);

    /// <summary>
    /// Invokes an action for each item in the stack in order and supplies
    /// a running index value.
    /// </summary>
    /// <typeparam name="T">Stack item type</typeparam>
    /// <param name="stack">Stack to iterate</param>
    /// <param name="action">Action to invoke with each item</param>
    /// <returns>Unit</returns>
    public static Unit Iter<T>(this Stck<T> stack, Action<int, T> action) =>
        LanguageExt.List.iter(stack, action);

    /// <summary>
    /// Return an enumerable with all duplicate values removed
    /// </summary>
    /// <typeparam name="T">Stack item type</typeparam>
    /// <param name="stack">Stack</param>
    /// <returns>An enumerable with all duplicate values removed</returns>
    [Pure]
    public static bool ForAll<T>(this Stck<T> stack, Func<T, bool> pred) =>
        LanguageExt.List.forall(stack, pred);

    /// <summary>
    /// Return an enumerable with all duplicate values removed
    /// </summary>
    /// <typeparam name="T">Stack item type</typeparam>
    /// <param name="stack">Stack</param>
    /// <returns>An enumerable with all duplicate values removed</returns>
    [Pure]
    public static IEnumerable<T> Distinct<T>(this Stck<T> stack) =>
        LanguageExt.List.distinct(stack);

    /// <summary>
    /// Return an enumerable with all duplicate values removed
    /// </summary>
    /// <typeparam name="T">Stack item type</typeparam>
    /// <param name="stack">Stack</param>
    /// <returns>An enumerable with all duplicate values removed</returns>
    [Pure]
    public static IEnumerable<T> Distinct<T>(this Stck<T> stack, Func<T, T, bool> compare) =>
        LanguageExt.List.distinct(stack, compare);

    /// <summary>
    /// Returns a new enumerable with the first 'count' items from the stack
    /// </summary>
    /// <typeparam name="T">Stack item type</typeparam>
    /// <param name="stack">Stack</param>
    /// <param name="count">Number of items to take</param>
    /// <returns>A new enumerable with the first 'count' items from the enumerable provided</returns>
    [Pure]
    public static IEnumerable<T> Take<T>(this Stck<T> stack, int count) =>
        LanguageExt.List.take(stack, count);

    /// <summary>
    /// Iterate the stack, yielding items if they match the predicate provided, and stopping 
    /// as soon as one doesn't
    /// </summary>
    /// <typeparam name="T">Stack item type</typeparam>
    /// <param name="stack">Stack</param>
    /// <param name="count">Number of items to take</param>
    /// <returns>A new enumerable with the first items that match the predicate</returns>
    [Pure]
    public static IEnumerable<T> TakeWhile<T>(this Stck<T> stack, Func<T, bool> pred) =>
        LanguageExt.List.takeWhile(stack, pred);

    /// <summary>
    /// Iterate the stack, yielding items if they match the predicate provided, and stopping 
    /// as soon as one doesn't  An index value is also provided to the predicate function.
    /// </summary>
    /// <typeparam name="T">Stack item type</typeparam>
    /// <param name="stack">Stack</param>
    /// <param name="count">Number of items to take</param>
    /// <returns>A new enumerable with the first items that match the predicate</returns>
    [Pure]
    public static IEnumerable<T> TakeWhile<T>(this Stck<T> stack, Func<T, int, bool> pred) =>
        LanguageExt.List.takeWhile(stack, pred);

    /// <summary>
    /// Returns true if any item in the stack matches the predicate provided
    /// </summary>
    /// <typeparam name="T">Stack item type</typeparam>
    /// <param name="stack">Stack</param>
    /// <param name="pred">Predicate</param>
    /// <returns>True if any item in the stack matches the predicate provided</returns>
    [Pure]
    public static bool Exists<T>(Stck<T> stack, Func<T, bool> pred) =>
        LanguageExt.List.exists(stack, pred);
}
