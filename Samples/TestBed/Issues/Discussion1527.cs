using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Issues;

public static class Discussion1527
{
    public static void Run()
    {
        var res = Think(1000).Run();

        Console.WriteLine($"Output: {res}");

        /*var sum1 = Source.forever(1)
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

        Console.WriteLine(r4);   */
    }

    public interface IObservation;

    public record Obs(DateTime When) : IObservation;

    static IObservation MakeObservation(DateTime dt)
    {
        Console.WriteLine($"Making observation: {dt}");
        return new Obs(dt);
    }

    static SourceT<IO, DateTime> tickTockIO()
    {
        return from token in IO.token
               from value in SourceT.lift<IO, DateTime>(go(token))
               select value;
        
        static async IAsyncEnumerable<DateTime> go(CancellationToken token)
        {
            while (true)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(5000), token);
                yield return DateTime.Now;
            }
        }
    }

    public static SourceT<IO, IObservation> Observations =>
        tickTockIO().Map(MakeObservation);

    public static IO<Option<IObservation>> Think(Duration duration) =>
        +Observations
            .Take(1)
            .TakeFor(duration)
            .Last()
            .Map(Some)
        | NoObservations;


    public static IO<Seq<IObservation>> Think2(Duration duration) =>
        +Observations
            .TakeFor(duration)
            .Collect();
    
    static readonly IO<Option<IObservation>> NoObservations = 
        IO.pure<Option<IObservation>>(None);
}
