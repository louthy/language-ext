using DomainTypesExamples.Capabilities;

namespace DomainTypesExamples;

public static partial class SamplePrelude
{
    public static Eff<RT, int> nextRandomInt32<RT>()
        where RT : HasSequences<RT> =>
        SequencesEnv.nextInt32<RT>();
}
