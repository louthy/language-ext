using System.Diagnostics.Contracts;

namespace LanguageExt.Traits;

/// <summary>
/// Equivalent of monoids for working on higher-kinded types
/// </summary>
/// <typeparam name="M">Higher kind</typeparam>
public interface MonoidK<M> : SemigroupK<M>
    where M : MonoidK<M>
{
    /// <summary>
    /// Identity
    /// <summary>
    [Pure]
    public static abstract K<M, A> Empty<A>();
}
