using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// Create a `finally` operation that can be used as the right-hand side of a `|` operator to
/// cause a final operation to be run regardless of whether the primary operation succeeds or not.
/// </summary>
/// <param name="finally"></param>
/// <typeparam name="F"></typeparam>
public readonly record struct Finally<F, X>(K<F, X> Operation);
