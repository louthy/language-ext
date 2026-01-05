using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// Immutable stack
/// </summary>
/// <typeparam name="A">Stack element type</typeparam>
[Serializable]
[CollectionBuilder(typeof(Stck), nameof(Stck.createRange))]
public abstract partial record Stck<A> : 
    IEnumerable<A>, 
    Monoid<Stck<A>>,
    K<Stck, A>
{
    /// <summary>
    /// Empty stack
    /// </summary>
    public static Stck<A> Empty { get; } = new Nil();

    /// <summary>
    /// Reference version for use in pattern-matching
    /// </summary>
    [Pure]
    public abstract object? Case { get; }

    /// <summary>
    /// Reverses the order of the items in the stack
    /// </summary>
    /// <returns>Reversed stack</returns>
    [Pure]
    public Stck<A> Reverse()
    {
        var stack = Empty;
        foreach (var item in this)
        {
            stack = item.Top(stack);
        }
        return stack;
    }

    /// <summary>
    /// Is the stack empty?
    /// </summary>
    [Pure]
    public abstract bool IsEmpty { get; }

    /// <summary>
    /// Get enumerator
    /// </summary>
    /// <remarks>From top to bottom</remarks>
    /// <returns>Stack enumerator</returns>
    [Pure]
    public IEnumerator<A> GetEnumerator()
    {
        var stack = this;
        while (stack is Top top)
        {
            yield return top.Value;
            stack = top.Rest;
        }
    }

    /// <summary>
    /// Convert this type to a lazy sequence
    /// </summary>
    /// <remarks>From top to bottom</remarks>
    /// <returns>Enumerable</returns>
    [Pure]
    public IEnumerable<A> AsEnumerable()
    {
        var stack = this;
        while (stack is Top top)
        {
            yield return top.Value;
            stack = top.Rest;
        }
    }

    /// <summary>
    /// Convert this type to a lazy sequence
    /// </summary>
    /// <returns>Iterable</returns>
    [Pure]
    public IEnumerable<A> AsIterable() =>
        AsEnumerable().AsIterable();

    /// <summary>
    /// Convert this type to a lazy sequence
    /// </summary>
    /// <remarks>From top to bottom</remarks>
    /// <returns>Iterable</returns>
    [Pure]
    public Seq<A> ToSeq() =>
        AsEnumerable().AsSeq();

    /// <summary>
    /// Format the collection as `[a, b, c, ...]`
    /// The elipsis is used for collections over 50 items
    /// To get a formatted string with all the items, use `ToFullString`
    /// or `ToFullArrayString`.
    /// </summary>
    [Pure]
    public override string ToString() =>
        CollectionFormat.ToShortArrayString(this);

    /// <summary>
    /// Format the collection as `a, b, c, ...`
    /// </summary>
    [Pure]
    public string ToFullString(string separator = ", ") =>
        CollectionFormat.ToFullString(this, separator);

    /// <summary>
    /// Format the collection as `[a, b, c, ...]`
    /// </summary>
    [Pure]
    public string ToFullArrayString(string separator = ", ") =>
        CollectionFormat.ToFullArrayString(this, separator);

    /// <summary>
    /// Impure iteration of the bound value in the structure
    /// </summary>
    /// <returns>
    /// Returns the original unmodified structure
    /// </returns>
    public Stck<A> Do(Action<A> f)
    {
        this.Iter(f);
        return this;
    }

    /// <summary>
    /// Return the item on the top of the stack without affecting the stack itself
    /// </summary>
    /// <returns>Top item value or None if the stack is empty.</returns>
    [Pure]
    public abstract Option<A> Peek();

    /// <summary>
    /// Return the item on the top of the stack without affecting the stack itself
    /// </summary>
    /// <exception cref="ExpectedException">`Errors.SequenceEmpty` is thrown when the stack is empty</exception>   
    /// <returns>Top item value or None if the stack is empty.</returns>
    [Pure]
    public A PeekUnsafe() =>
        Peek().IfNone(() => throw Errors.SequenceEmpty);

    /// <summary>
    /// Return the item on the top of the stack without affecting the stack itself
    /// </summary>
    /// <returns>True if `value` has been updated with the top value on the stack</returns>
    [Pure]
    public bool TryPeek(out A value)
    {
        var top = Peek();
        if (top.IsSome)
        {
            value = top.Value!;
            return true;
        }
        else
        {
            value = default!;
            return false;       
        }
    }

    /// <summary>
    /// Pop an item off the top of the stack. 
    /// </summary>
    /// <remarks>
    /// If there's nothing on the stack, this does nothing.  Use pattern-matching, `IsEmpty`, or `Peek` to know
    /// whether `Pop` will have an effect.
    /// </remarks>
    /// <returns>Stack with the top item popped</returns>
    [Pure]
    public abstract Stck<A> Pop();

    /// <summary>
    /// Push an item onto the stack
    /// </summary>
    /// <param name="value">Item to push</param>
    /// <returns>New stack with the pushed item on top</returns>
    [Pure]
    public Stck<A> Push(A value) =>
        new Top(value, this);

    /// <summary>
    /// Get enumerator
    /// </summary>
    /// <returns>IEnumerator of T</returns>
    [Pure]
    IEnumerator IEnumerable.GetEnumerator()
    {
        var stack = this;
        while (stack is Top top)
        {
            yield return top.Value;
            stack = top.Rest;
        }
    }
    
    /// <summary>
    /// Implicit conversion from an untyped empty list
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Stck<A>(SeqEmpty _) =>
        Empty;

    /// <summary>
    /// Append another stack to the top of this stack
    /// The rhs will be reversed and pushed onto 'this' stack.  That will
    /// maintain the order of the items in the resulting stack.  So the top
    /// of 'rhs' will be the top of the newly created stack.  'this' stack
    /// will be under the 'rhs' stack.
    /// </summary>
    [Pure]
    public static Stck<A> operator +(Stck<A> lhs, K<Stck, A> rhs) =>
        lhs.Combine(rhs.As());

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
    public Stck<A> Combine(Stck<A> rhs)
    {
        var self = this;
        foreach (var item in rhs.As().Reverse())
        {
            self = self.Push(item);
        }
        return self;
    }
}
