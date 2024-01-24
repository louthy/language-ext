using LanguageExt.Common;

namespace LanguageExt.ClassInstances;

public struct HashableResult<A> : Hashable<Result<A>>
{
    /// <summary>
    /// Get hash code of the value
    /// </summary>
    /// <param name="x">Value to get the hash code of</param>
    /// <returns>The hash code of x</returns>
    public static int GetHashCode(Result<A> x) =>
        x.IsBottom    ? -2
      : x.IsFaulted ? -1
      : x.Value?.GetHashCode() ?? 0;
}

public struct HashableOptionalResult<A> : Hashable<OptionalResult<A>>
{
    /// <summary>
    /// Get hash code of the value
    /// </summary>
    /// <param name="x">Value to get the hash code of</param>
    /// <returns>The hash code of x</returns>
    public static int GetHashCode(OptionalResult<A> x) =>
        x.IsBottom  ? -2
      : x.IsFaulted ? -1 
      : HashableOption<A>.GetHashCode(x.Value);
}
