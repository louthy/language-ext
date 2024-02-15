namespace LanguageExt;

/// <summary>
/// Result of forking an IO monad
/// </summary>
/// <param name="Cancel">An IO monad, which if invoked, would cancel the forked IO operation</param>
/// <param name="Await">An IO monad, which if invoked, would attempt to get the result of the forked IO operation</param>
/// <typeparam name="A">Bound value type</typeparam>
public readonly record struct ForkEff<A>(
    Eff<Unit> Cancel,
    Eff<A> Await);