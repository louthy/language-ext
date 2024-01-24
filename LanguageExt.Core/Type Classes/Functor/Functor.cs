#nullable enable
using System;
using System.Diagnostics.Contracts;
using LanguageExt.Attributes;

namespace LanguageExt.TypeClasses;

/// <summary>
/// Functor trait
/// </summary>
/// <typeparam name="FA">Source functor type</typeparam>
/// <typeparam name="FB">Target functor type</typeparam>
/// <typeparam name="A">Source functor bound value type</typeparam>
/// <typeparam name="B">Target functor bound value type</typeparam>
[Trait("F*")]
public interface Functor<in FA, out FB, out A, in B> : Trait
{
    /// <summary>
    /// Projection from one value to another
    /// </summary>
    /// <param name="ma">Functor value to map from </param>
    /// <param name="f">Projection function</param>
    /// <returns>Mapped functor</returns>
    [Pure]
    public static abstract FB Map(FA ma, Func<A, B> f);
}
