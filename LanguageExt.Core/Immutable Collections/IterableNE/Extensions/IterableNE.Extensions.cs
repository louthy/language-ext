#pragma warning disable CS0693 // Type parameter has the same name as the type parameter from outer type

using System;
using LanguageExt.Traits;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static partial class IterableNEExtensions
{
    [Pure]
    public static IterableNE<A> As<A>(this K<IterableNE, A> xs) =>
        (IterableNE<A>)xs;
    
    [Pure]
    public static IterableNE<A> Flatten<A>(this IterableNE<IterableNE<A>> ma) =>
        ma.Bind(identity);
}
