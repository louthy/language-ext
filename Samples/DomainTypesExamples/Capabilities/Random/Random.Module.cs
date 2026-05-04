namespace DomainTypesExamples.Capabilities;

public static class RandomEnv
{
    public static Eff<RT, int> nextInt32<RT>(int min, int max)
        where RT : HasRandom<RT> =>
        RandomEnv<RT>.nextInt32(min, max);
}
