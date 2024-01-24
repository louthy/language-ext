#nullable  enable
using LanguageExt.Attributes;
using LanguageExt.Common;

namespace LanguageExt.Effects.Traits;

/// <summary>
/// Minimal collection of traits for Eff operations
/// </summary>
/// <typeparam name="RT">Runtime</typeparam>
[Trait("*")]
public interface HasEff<out RT> : HasIO<RT, Error>
    where RT : HasIO<RT, Error>
{ }
