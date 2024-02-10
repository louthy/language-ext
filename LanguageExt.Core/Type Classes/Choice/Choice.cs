using System;
using System.Diagnostics.Contracts;
using LanguageExt.Attributes;

namespace LanguageExt.TypeClasses;

[Trait("Choice*")]
public interface Choice<in CH, out L, out R> : Trait
{
    /// <summary>
    /// Is the choice in the first state
    /// </summary>
    [Pure]
    public static abstract bool IsLeft(CH choice);

    /// <summary>
    /// Is the choice in the second state
    /// </summary>
    [Pure]
    public static abstract bool IsRight(CH choice);

    /// <summary>
    /// Is the choice in the bottom
    /// </summary>
    [Pure]
    public static abstract bool IsBottom(CH choice);

    /// <summary>
    /// Match the two states of the Choice and return a non-null C.
    /// </summary>
    /// <typeparam name="C">Return type</typeparam>
    [Pure]
    public static abstract C Match<C>(CH choice, Func<L, C> Left, Func<R, C> Right, Func<C>? Bottom = null);

    /// <summary>
    /// Match the two states of the Choice and return a non-null C.
    /// </summary>
    /// <typeparam name="C">Return type</typeparam>
    public static abstract Unit Match(CH choice, Action<L> Left, Action<R> Right, Action? Bottom = null);
}
