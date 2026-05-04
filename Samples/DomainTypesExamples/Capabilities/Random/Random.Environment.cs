using LanguageExt.Traits;

namespace DomainTypesExamples.Capabilities;

public interface HasRandom<RT> : Has<Eff<RT>, RandomIO>;

public sealed record RandomEnv<RT>
    where RT : HasRandom<RT>
{
    private static Eff<RT, RandomIO> randomIO =>
        Has<Eff<RT>, RT, RandomIO>.ask.As();

    public static Eff<RT, int> nextInt32(int min, int max) =>
        randomIO.Bind(io => io.NextInt32(min, max));
}
