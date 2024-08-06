namespace LanguageExt.Traits;

/// <summary>
/// Strongly typed way of accessing a trait interface from an environment
/// </summary>
/// <remarks>
/// When using multiple `Has` traits, the accessor (`Env.Trait`) will cause ambiguity errors.
/// This type resolves that by forcing qualification via the type parameters.  It also has
/// the benefit that we cache the trait interface in a readonly static, saving on allocations. 
/// </remarks>
/// <typeparam name="M">Structure.  Usually a monad</typeparam>
/// <typeparam name="Env">The environment type to get the trait interface from</typeparam>
/// <typeparam name="VALUE">Type of the trait interface</typeparam>
public static class Has<M, Env, VALUE>
    where Env : Has<M, VALUE>
{
    /// <summary>
    /// Cached trait interface accessor
    /// </summary>
    public static readonly K<M, VALUE> ask =
        Env.Ask;
}
