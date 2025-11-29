using System;

namespace LanguageExt.Traits;

public static class Act<A, B>
{
    public static readonly Func<A, Func<B, B>> fun =
        _ => y => y;
}
