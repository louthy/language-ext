#pragma warning disable CS0693 // Type parameter has the same name as the type parameter from outer type

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using LanguageExt.Traits;
using L = LanguageExt;

namespace LanguageExt;

public static partial class Prelude
{
    /// <summary>
    /// Construct an empty Iterable
    /// </summary>
    [Pure]
    public static Iterable<A> Iterable<A>() =>
        L.Iterable<A>.Empty;
        
    /// <summary>
    /// Construct a sequence from an Enumerable
    /// </summary>
    [Pure]
    public static Iterable<A> Iterable<A>(params ReadOnlySpan<A> value) =>
        L.Iterable<A>.FromSpan(value);
        
    [Pure]
    public static Iterable<A> toIterable<A>(Iterator<A>? value) =>
        new (value ?? Iterator<A>.Empty);

    [Pure]
    public static Iterable<A> toIterable<T, A>(K<T, A>? value)
        where T : IterableK<T, A> =>
        value switch
        {
            null => L.Iterable<A>.Empty,
            _    => new Iterable<A>(value.ForwardIterator())
        };
    
    [Pure]
    public static Iterable<A> toIterable<A>(IEnumerable<A>? value) =>
        value switch
        {
            null                => L.Iterable<A>.Empty,
            _                   => new Iterable<A>(value.AsIterator())
        };
}
