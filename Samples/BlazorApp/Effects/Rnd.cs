using BlazorApp.Effects.Interfaces;
using LanguageExt;
using LanguageExt.Traits;

namespace BlazorApp.Effects;

public static class Rnd<M, RT>
    where M : MonadIO<M>
    where RT : Has<M, RndIO>
{
    static readonly K<M, RndIO> trait = Has<M, RT, RndIO>.ask;

    /// <summary>
    /// Get a random integer between min and max, inclusive.
    /// </summary>
    public static K<M, int> next(int min, int max) =>
        trait.Map(t => t.Next(min, max));
    
    /// <summary>
    /// Get a random integer up to max, inclusive.
    /// </summary>
    public static K<M, int> next(int max) =>
        trait.Map(t => t.Next(max));
}


public static class Rnd<RT>
    where RT : Has<Eff<RT>, RndIO>
{
    /// <summary>
    /// Get a random integer between min and max, inclusive.
    /// </summary>
    public static Eff<RT, int> next(int min, int max) =>
        +Rnd<Eff<RT>, RT>.next(min, max);
    
    /// <summary>
    /// Get a random integer up to max, inclusive.
    /// </summary>
    public static Eff<RT, int> next(int max) =>
        +Rnd<Eff<RT>, RT>.next(max);
}

