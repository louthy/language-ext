using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// Finally structure.  
///
/// Implicit operators use this temporarily value-type to carry a `finally` computation.
/// </summary>
/// <param name="finally"></param>
/// <typeparam name="F"></typeparam>
public readonly record struct Finally<F>(K<F, Unit> Operation);
