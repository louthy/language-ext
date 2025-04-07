using System;
using System.Runtime.Serialization;
using LanguageExt;

public static class Issue1453
{
    public static void Test()
    {
        Deserialize("invalid")
            .BindFail(_ => Fallback())
            .Match(Succ: time =>
                         {
                             Console.WriteLine(time);
                             return Unit.Default;
                         },
                   Fail: _ =>
                         {
                             Console.WriteLine("Fail");
                             return Unit.Default;
                         });

        Try<DateTime> Deserialize(string dateTime) =>
            Try.lift<DateTime>(() => throw new SerializationException());
    
        Try<DateTime> Fallback() =>
            Try.Succ(DateTime.MinValue);
    }
}
