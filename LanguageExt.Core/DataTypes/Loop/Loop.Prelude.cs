using System;
using System.Diagnostics.Contracts;

namespace LanguageExt;

public static partial class Prelude
{
    [Pure]
    public static Loop<A> Loop<A>(A value) => 
        new (value);
}
