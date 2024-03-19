
using System;
using System.Diagnostics.Contracts;
using System.Linq;
using LanguageExt.Traits;

namespace LanguageExt;

public static class HashSetExtensions
{
    [Pure]
    public static HashSet<A> As<A>(this K<HashSet, A> ma) =>
        (HashSet<A>)ma;
    
    /// <summary>
    /// Convert to a queryable 
    /// </summary>
    [Pure]
    public static IQueryable<A> AsQueryable<A>(this HashSet<A> source) =>
        // NOTE TO FUTURE ME: Don't delete this thinking it's not needed!
        Queryable.AsQueryable(source.Value.AsQueryable());
}
