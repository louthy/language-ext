using System;

namespace Issues;

public class Discussion1527
{
    public static void Run()
    {
        var sum1 = Source.forever(1)
                         .FoldWhile(
                              (s, x) => s + x,
                              (s, _) => s <= 10,
                              0
                          )
                         .Take(1)
                         .Last()
                         .Run();

        Console.WriteLine(sum1);

        var sum2 = SourceT.forever<IO, int>(1)
                          .FoldWhileIO(
                               0,
                               (s, x) => s + x,
                               x => x.State <= 10
                           )
                          .As()
                          .Take(1)
                          .Last()
                          .Run();

        Console.WriteLine(sum2);
    }
}
