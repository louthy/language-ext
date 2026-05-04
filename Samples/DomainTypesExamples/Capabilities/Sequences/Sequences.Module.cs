namespace DomainTypesExamples.Capabilities;

public static class SequencesEnv
{
    public static Eff<RT, int> nextInt32<RT>()
        where RT : HasSequences<RT> =>
        SequencesEnv<RT>.nextInt32;
}
