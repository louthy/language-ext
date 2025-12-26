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
                          .FoldWhile(
                               (s, x) => s + x,
                               x => x.State <= 10,
                               0
                           )
                          .As()
                          .Take(1)
                          .Last()
                          .Run();

        Console.WriteLine(sum2);

        var src3 = from x in SourceT.lift<IO, int>(Range(1, 100))
                   where x % 10 == 0
                   select x;

        var r3 = src3.Skip(2).Take(5).Collect().Run();

        Console.WriteLine(r3);

        var src4 = +SourceT.lift<IO, int>(Range(1, 100))
                           .FoldWhile((s, x) => s + x, sv => sv.State.Count < 5, Seq<int>());

        var r4 = src4.Skip(2).Take(5).Collect().Run();

        Console.WriteLine(r4);        
    }
}
