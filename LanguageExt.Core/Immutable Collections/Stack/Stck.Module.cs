using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using LanguageExt.Common;

namespace LanguageExt;

/// <summary>
/// Module for working with the `Stck` type
/// </summary>
public partial class Stck
{
    /// <summary>
    /// Create a new stack from a single element
    /// </summary>
    /// <param name="item">Item to populate the singleton stack</param>
    /// <typeparam name="A">Type of the items</typeparam>
    /// <returns>Constructed stack collection</returns>
    [Pure]
    public static Stck<A> singleton<A>(A item) =>
        [item];

    /// <summary>
    /// Create a new stack from an existing span
    /// </summary>
    /// <param name="items">Items to populate the stack</param>
    /// <typeparam name="A">Type of the items</typeparam>
    /// <returns>Constructed stack collection</returns>
    [Pure]
    public static Stck<A> createRange<A>(IEnumerable<A> items)
    {
        var stack = new Stck<A>.Top(default!, Stck<A>.Empty);
        var top   = stack;
        foreach (var item in items)
        {
            var nstack = new Stck<A>.Top(item, Stck<A>.Empty);
            stack.Rest = nstack;
            stack = nstack;
        }
        return top.Rest;
    }
    
    /// <summary>
    /// Create a new stack from an existing span
    /// </summary>
    /// <param name="items">Items to populate the stack</param>
    /// <typeparam name="A">Type of the items</typeparam>
    /// <returns>Constructed stack collection</returns>
    [Pure]
    public static Stck<A> createRange<A>(ReadOnlySpan<A> items)
    {
        var stack = new Stck<A>.Top(default!, Stck<A>.Empty);
        var top   = stack;
        foreach (var item in items)
        {
            var nstack = new Stck<A>.Top(item, Stck<A>.Empty);
            stack.Rest = nstack;
            stack = nstack;
        }
        return top.Rest;
    }
    
    /// <summary>
    /// Reverses the order of the items in the stack
    /// </summary>
    /// <returns></returns>
    [Pure]
    public static Stck<A> rev<A>(Stck<A> stack) =>
        stack.Reverse();

    /// <summary>
    /// True if the stack is empty
    /// </summary>
    [Pure]
    public static bool isEmpty<A>(Stck<A> stack) =>
        stack.IsEmpty;

    /// <summary>
    /// Return the item on the top of the stack without affecting the stack itself
    /// </summary>
    /// <returns>Top item value or None if the stack is empty.</returns>
    [Pure]
    public static Option<A> peek<A>(Stck<A> stack) =>
        stack.Peek();

    /// <summary>
    /// Return the item on the top of the stack without affecting the stack itself
    /// </summary>
    /// <exception cref="ExpectedException">`Errors.SequenceEmpty` is thrown when the stack is empty</exception>   
    /// <returns>Top item value or None if the stack is empty.</returns>
    [Pure]
    public static A peekUnsafe<A>(Stck<A> stack) =>
        stack.PeekUnsafe();


    /// <summary>
    /// Return the item on the top of the stack without affecting the stack itself
    /// </summary>
    /// <returns>True if `value` has been updated with the top value on the stack</returns>
    [Pure]
    public static bool tryPeek<A>(Stck<A> stack, out A value) =>
        stack.TryPeek(out value);

    /// <summary>
    /// Pop an item off the top of the stack. 
    /// </summary>
    /// <remarks>
    /// If there's nothing on the stack, this does nothing.  Use pattern-matching, `IsEmpty`, or `Peek` to know
    /// whether `Pop` will have an effect.
    /// </remarks>
    /// <returns>Stack with the top item popped</returns>
    [Pure]
    public static Stck<A> pop<A>(Stck<A> stack) =>
        stack.Pop();

    /// <summary>
    /// Push an item onto the stack
    /// </summary>
    /// <param name="value">Item to push</param>
    /// <returns>New stack with the pushed item on top</returns>
    [Pure]
    public static Stck<A> push<A>(Stck<A> stack, A value) =>
        stack.Push(value);
}
