using LanguageExt.Attributes;

namespace LanguageExt.Effects.Traits;

/// <summary>
/// Minimal collection of traits for IO operations
/// </summary>
/// <typeparam name="RT">Runtime</typeparam>
[Trait("*")]
public interface HasIO<out RT> 
    where RT : HasIO<RT>
{
    /// <summary>
    /// Injects a new IO state into the runtime 
    /// </summary>
    RT WithIO(EnvIO envIO);
    
    /// <summary>
    /// Get the IO environment from the runtime state 
    /// </summary>
    EnvIO EnvIO { get; }
}
