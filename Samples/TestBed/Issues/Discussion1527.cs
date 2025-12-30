using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace Issues;

public static class Discussion1527
{
    public static void Run()
    {
        Eff<IObservation> r1 = +Think<Eff>(1000);
        IO<IObservation>  r2 = +Think<IO>(1000);

        Console.WriteLine(r1.Run());
        Console.WriteLine(r2.Run());

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

    static IO<Seq<IObservation>> Think2(Duration duration, Func<IObservation, bool>? predicate = null)
    {
        var observations = Observations<IO>()
                                .FoldUntil((s, o) => s + o,
                                           x => predicate?.Invoke(x.Value) ?? false,
                                           Seq<IObservation>())
                                .TakeFor(duration) 
                         | SourceT.pure<IO, Seq<IObservation>>(Empty);

        return observations.Last().As();
    }

    public static K<M, IObservation> Think<M>(Duration duration)
        where M : MonadIO<M>, Fallible<M>, Alternative<M>
    {
        var empty        = SourceT.liftM(M.Empty<IObservation>());
        var timeout      = empty.Delay(duration);
        var observations = Observations<M>();

        return (observations | timeout)
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

    static SourceT<M, DateTime> tickTockIO<M>()
        where M : MonadIO<M>, Alternative<M>
    {
        return from token in M.LiftIO(IO.token)
               from value in SourceT.lift<M, DateTime>(go(token))
               select value;
        
        static async IAsyncEnumerable<DateTime> go(CancellationToken token)
        {
            while (true)
            {
                if(token.IsCancellationRequested) yield break;
                await Task.Delay(TimeSpan.FromMilliseconds(5000), token);
                var now = DateTime.Now;
                yield return now;
            }
        }
    }

    public static SourceT<M, IObservation> Observations<M>() 
        where M : MonadIO<M>, Alternative<M> =>
        tickTockIO<M>().Map(MakeObservation);

    /*
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
            .Collect();*/
    
    static readonly IO<Option<IObservation>> NoObservations = 
        IO.pure<Option<IObservation>>(None);
}
