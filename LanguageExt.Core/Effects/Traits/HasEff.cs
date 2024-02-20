using LanguageExt.Attributes;
using LanguageExt.Common;

namespace LanguageExt.Effects.Traits;

/// <summary>
/// Minimal collection of traits for Eff operations
/// </summary>
/// <typeparam name="RT">Runtime</typeparam>
[Trait("*")]
public interface HasEff<out RT> : HasIO<RT>
    where RT : HasIO<RT>;
