#pragma warning disable CS0693 // Type parameter has the same name as the type parameter from outer type

using System.Diagnostics.Contracts;
using System.Linq;
using LanguageExt.Traits;

namespace LanguageExt;

public static class SetExtensions
{
    public static Set<A> As<A>(this K<Set, A> ma) =>
        (Set<A>)ma;
    
    /// <summary>
    /// Convert to a queryable 
    /// </summary>
    [Pure]
    public static IQueryable<A> AsQueryable<A>(this Set<A> source) =>
        // NOTE TO FUTURE ME: Don't delete this thinking it's not needed!
        source.Value.AsQueryable();
}
