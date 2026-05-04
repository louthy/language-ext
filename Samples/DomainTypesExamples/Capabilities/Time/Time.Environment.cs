using LanguageExt.Traits;

namespace DomainTypesExamples.Capabilities;

public interface HasTime<RT> : Has<Eff<RT>, TimeIO>;

public sealed record TimeEnv<RT>
    where RT : HasTime<RT>
{
    private static Eff<RT, TimeIO> timeIO =>
        Has<Eff<RT>, RT, TimeIO>.ask.As();

    public static Eff<RT, DateTimeOffset> getNow =>
        timeIO.Bind(io => io.Now);
}
