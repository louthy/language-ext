namespace LanguageExt.Traits;

/// <summary>
/// Strongly typed way of accessing a `Has` trait interface from an environment
/// </summary>
/// <remarks>
/// When using multiple `Has` traits, the accessor (`Env.Trait`) will cause ambiguity errors.
/// This type resolves that by forcing qualification via the type parameters.  It also has
/// the benefit that we cache the trait interface in a readonly static, saving on allocations. 
/// </remarks>
/// <typeparam name="M">Higher-kind</typeparam>
/// <typeparam name="Env">The environment type from which to get the trait-interface value</typeparam>
/// <typeparam name="VALUE">Trait interface type</typeparam>
public static class Has<M, Env, VALUE>
    where Env : Has<M, VALUE>
{
    /// <summary>
    /// Cached trait interface accessor
    /// </summary>
    public static readonly K<M, VALUE> ask =
        Env.Ask;
}
