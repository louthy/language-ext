global using LanguageExt;
global using static LanguageExt.Prelude;
global using LanguageExt.Pipes;
global using LanguageExt.Traits;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace TestBed;

public static class SourceExt
{

    public static SourceT<M, A> Log<M, A>(this SourceT<M, A> src, Func<A, string> log)
        where M: MonadIO<M>, Alternative<M> =>
        src.Map(x => { Console.WriteLine(log(x)); return x; });

    public static Source<A> Log<A>(this Source<A> src, Func<A, string> log) =>
        src.Map(x => { Console.WriteLine(log(x)); return x; });
}
				
public class SourceTTests
{
    public record MySkipTransducerM<M, A>(int Amount) : TransducerM<M, A, A>
        where M : Applicative<M>
    {
        public int amount = Amount;
        public override ReducerM<M, A, S> Reduce<S>(ReducerM<M, A, S> reducer)  
        {
            return (s, x) =>
                   {
                       if (amount > 0)
                       {
                           amount--;
                           return M.Pure(s);
                       }
                       else
                       {
                           return reducer(s, x);
                       }
                   };
        }
    }

    public static SourceT<IO, int> getSourceT() =>
        SourceT.lift<IO, int>(Enumerable.Range(1, 5)).Log(x => $"Producing {x}");

    public static Source<int> getSource() =>
        Source.lift(Enumerable.Range(1, 5)).Log(x => $"Producing {x}");

    public static void Run()
    {
        Console.WriteLine("\nCurrent Source Skip\n");
        getSource().Skip(1).Log(x => $"CurrentSkip: Got {x}").Iter().Run();
        
        Console.WriteLine("\nCurrent SourceT Skip\n");
        getSourceT().Skip(1).Log(x => $"CurrentSkipT: Got {x}").Iter().Run();
        
        //Console.WriteLine("\nPatched SourceT Skip\n");
        //getSourceT().Transform(new MySkipTransducerM<IO, int>(1)).Log(x => $"PatchedSkip: Got {x}").Iter().Run();
    }
}
