
using System;
using LanguageExt.Traits;

namespace LanguageExt;

public static class HashSetExtensions
{
    public static HashSet<A> As<A>(this K<HashSet, A> ma) =>
        (HashSet<A>)ma;

    public static HashSet<B> Apply<A, B>(this HashSet<Func<A, B>> mf, HashSet<A> ma) =>
        mf.Bind(ma.Map);

    public static HashSet<B> Action<A, B>(this HashSet<A> ma, HashSet<B> mb) =>
        mb;
}
