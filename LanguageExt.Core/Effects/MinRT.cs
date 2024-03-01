using LanguageExt.Effects.Traits;

namespace LanguageExt.Effects;

public static class MinRTExtensions
{
    public static MinRT ToMin<RT>(this RT rt) 
        where RT : HasIO<RT> =>
        new (rt.EnvIO);
}

/// <summary>
/// Minimal runtime for running the non-runtime based IO monads
/// </summary>
public readonly struct MinRT : HasIO<MinRT>
{
    public MinRT(EnvIO env) =>
        EnvIO = env;

    public MinRT() : this(EnvIO.New())
    { }

    public MinRT WithIO(EnvIO envIO) => 
        new(envIO);
    
    public EnvIO EnvIO { get; }

    public override string ToString() => 
        "MinRT";
}
