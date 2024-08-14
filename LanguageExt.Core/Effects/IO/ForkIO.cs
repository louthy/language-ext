namespace LanguageExt;

/// <summary>
/// Result of forking an `IO` monad
/// </summary>
/// <param name="Cancel">An `IO` monad, which, if invoked, would cancel the forked IO operation</param>
/// <param name="Await">An `IO` monad, which, if invoked, would await the result of the forked
/// `IO` operation.  Obviously, this mitigates the reasons for forking somewhat, but this struct
/// could be passed to another process that does the awaiting - and so still has some value.</param>
/// <typeparam name="A">Bound value type</typeparam>
public readonly record struct ForkIO<A>(
    IO<Unit> Cancel, 
    IO<A> Await);
