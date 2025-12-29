using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt;
using static LanguageExt.Prelude;

namespace Issues;

public static class Discussion1527
{
    public static void Run()
    {
        Console.WriteLine(Think3(1000).Run());

        /*var res = Think(1000).Run();

        Console.WriteLine($"Output: {res}");

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

        Console.WriteLine(sum2);*/
    }

    public static IO<Seq<IObservation>> Think3(Duration duration)
    {
        var empty        = SourceT.pure<IO, Seq<IObservation>>([]);
        var timeout      = empty.Delay(duration);
        var observations = Observations.Map(Seq);
        
        return +SourceT.merge(observations, timeout)
                       .Take(1)
                       .Last();
    }
    
    public interface IObservation;

    public record Obs(DateTime When) : IObservation;

    static IObservation MakeObservation(DateTime dt)
    {
        //Console.WriteLine($"Making observation: {dt.Ticks}");
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
                if(token.IsCancellationRequested) yield break;
                await Task.Delay(TimeSpan.FromMilliseconds(10000), token);
                var now = DateTime.Now;
                yield return now;
            }
        }
    }

    public static SourceT<IO, IObservation> Observations =>
        tickTockIO().Map(MakeObservation);

    public static IO<Option<IObservation>> Think(Duration duration) =>
        Observations
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
