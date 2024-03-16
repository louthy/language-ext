
using System;
using LanguageExt.Traits;

namespace LanguageExt;

public static class HashSetExtensions
{
    public static HashSet<A> As<A>(this K<HashSet, A> ma) =>
        (HashSet<A>)ma;
}
