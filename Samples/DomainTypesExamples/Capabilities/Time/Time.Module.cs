namespace DomainTypesExamples.Capabilities;

public static class TimeEnv
{
    public static Eff<RT, DateTimeOffset> getNow<RT>()
        where RT : HasTime<RT> =>
        TimeEnv<RT>.getNow;
}
