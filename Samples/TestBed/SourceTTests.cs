using System;
using LanguageExt;
using LanguageExt.Traits;
using System.Threading.Tasks;
using static LanguageExt.Prelude;

namespace TestBed;

public static class SourceExt
{

    public static SourceT<M, A> Log<M, A>(this SourceT<M, A> src, Func<A, string> log)
        where M: MonadIO<M>, Alternative<M> =>
        src.Map(x => { Console.WriteLine(log(x)); return x; });

    public static Source<A> Log<A>(this Source<A> src, Func<A, string> log) =>
        src.Map(x => { Console.WriteLine(log(x)); return x; });
}

public delegate void TickEvent(DateTime dt);
				
public class SourceTTests
{
    public static event Action<DateTime> Tick;
    public static readonly Event<DateTime> tickEvent = Event.from(ref Tick);

    public static void Run()
    {
        var t = Task.Run(Clock);

        var ticks = SourceT.iter(from v in tickEvent.Subscribe<IO>()
                                 from _ in writeLine(v)
                                 select unit);
        
        var app = from f in ticks.ForkIO()
                  from k in readKey
                  from _ in f.Cancel
                  from i in writeLine("The messages should have stopped")
                  select unit;
                            ;
        ignore(app.Run());
        Console.WriteLine("Press any key to exit");
        Console.ReadKey();
    }

    static async Task Clock()
    {
        while (true)
        {
            await Task.Delay(1000);
            Tick?.Invoke(DateTime.Now);
        }
    }
    
    static IO<Unit> writeLine(object? s) => 
        IO.lift(() => Console.WriteLine(s));
    
    static IO<ConsoleKeyInfo> readKey => 
        IO.lift(Console.ReadKey);

    /*
    static SourceT<M, A> fromEvent<M, D, A>(ref D del, Action<D, A> f)
        where M : MonadIO<M>, Alternative<M>
        where D : MulticastDelegate
    {
        
    }

    static SourceT<M, A> fromEvent<M, A>(ref Action<A> del)
        where M : MonadIO<M>, Alternative<M>
    {
        var sub = new Sub<A>(ref del);
        return from _ in use(() => sub.Plus())
               from v in SourceT.lift<M, A>(sub.Channel)
               select v;
    }

    public class SourceRefT<M, A>(SourceT<M, A> Source, Sub<A> Sub) : IDisposable
        where M : MonadIO<M>, Alternative<M> 
    {
        public void Dispose() => 
            Sub.Dispose();
    }
    
    public class Sub<A> : IDisposable
    {
        public readonly Channel<A> Channel;
        Action<A> @delegate;
        int count;

        public Sub(Event<A> @delegate)
        {
            @delegate += Post;
            this.@delegate = @delegate;
            Channel = System.Threading.Channels.Channel.CreateUnbounded<A>();
        }

        public IDisposable Plus()
        {
            Interlocked.Increment(ref count);
            return this;
        }

        public void Post(A value) =>
            Channel.Writer.TryWrite(value);
        
        public void Dispose()
        {
            if (Interlocked.Decrement(ref count) <= 0)
            {
                #pragma warning disable CS8601 // Possible null reference assignment.
                Channel.Writer.Complete();
                @delegate -= Post;
            }
        }
    }
        */

}
