using System;
using LanguageExt;
using System.Threading.Tasks;
using static LanguageExt.Prelude;

namespace TestBed;

public class AwaitAnyTest
{
    public static void Run()
    {
        Console.WriteLine("Started");

        var eff = awaitAny(uninterruptible(delayed("A", 9)),
                           uninterruptible(delayed("B", 7)),
                           uninterruptible(delayed("C", 5)));

        using var env = EnvIO.New();
        ignore(eff.Run(env));
        
        Console.ReadKey();
        env.Source.Cancel();
        Console.WriteLine("Cancellation trigger, press any key to exit");
        Console.ReadKey();
    }

    static Eff<Unit> delayed(string info, int time) =>
        Eff.lift(async e =>
                 {
                     await Task.Delay(time * 1000, e.Token);
                     Console.WriteLine(info);
                     return unit;
                 });
}
