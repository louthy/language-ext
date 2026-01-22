#pragma warning disable CS0693 // Type parameter has the same name as the type parameter from outer type

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using L = LanguageExt;

namespace LanguageExt;

public static partial class Prelude
{
    /// <summary>
    /// Construct an empty Iterable
    /// </summary>
    [Pure]
    public static IterableIO<A> IterableIO<A>() =>
        L.IterableIO<A>.Empty;
        
    /// <summary>
    /// Construct a sequence from an Enumerable
    /// </summary>
    [Pure]
    public static IterableIO<A> IterableIO<A>(params ReadOnlySpan<A> value) =>
        L.IterableIO<A>.FromSpan(value);
        
    [Pure]
    public static IterableIO<A> toIterableIO<A>(Iterator<A>? value) =>
        new (IteratorIO.lift(value ?? Iterator<A>.Empty));
        
    [Pure]
    public static IterableIO<A> toIterableIO<A>(IteratorIO<A>? value) =>
        new (value ?? IteratorIO<A>.Empty);
        
    [Pure]
    public static Iterable<A> toIterableIO<A>(IEnumerable<A>? value) =>
        value switch
        {
            null                => LanguageExt.Iterable<A>.Empty,
            Iterable<A> iter    => iter, 
            Seq<A> seq          => seq.AsIterable(),
            Arr<A> arr          => arr.AsIterable(),
            A[] array           => Iterable(array),
            _                   => new Iterable<A>(value.AsIterator())
        };
    
    [Pure]
    public static IterableIO<A> toIterableIO<A>(IAsyncEnumerable<A>? value) =>
        value switch
        {
            null => L.IterableIO<A>.Empty,
            _    => new IterableIO<A>(value.AsIteratorIO())
        };
}
