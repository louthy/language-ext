namespace LanguageExt.Traits;

/// <summary>
/// Makes an atomic value within an environment available for mutation 
/// </summary>
/// <typeparam name="M">Structure trait</typeparam>
/// <typeparam name="InnerEnv">The value extracted from an environment</typeparam>
public interface Mutates<in M, InnerEnv> : Has<M, InnerEnv>
    where M : Functor<M>
{
    /// <summary>
    /// Extracts the `A` from the `Env`, passes it to the `f`
    /// mapping functions, and then wraps it back up into the generic `M〈Unit〉` type.
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <returns>Mapped value</returns>
    public static abstract K<M, Atom<InnerEnv>> Mutable { get; }
}
