using LanguageExt.Traits;

namespace DomainTypesExamples.Capabilities;

public interface HasSequences<RT> : Has<Eff<RT>, SequencesIO>;

public sealed record SequencesEnv<RT>
    where RT : HasSequences<RT>
{
    private static Eff<RT, SequencesIO> sequencesIO =>
        Has<Eff<RT>, RT, SequencesIO>.ask.As();

    public static Eff<RT, int> nextInt32 =>
        sequencesIO.Bind(io => io.NextInt32());
}
