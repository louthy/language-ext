using DomainTypesExamples.Capabilities;

namespace DomainTypesExamples;

public static partial class SamplePrelude
{
    public static Eff<RT, int> nextRandom<RT>(int min, int max)
        where RT : HasRandom<RT> =>
        RandomEnv.nextInt32<RT>(min, max);
}
