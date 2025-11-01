using System.Collections.Generic;
using System.Linq;

namespace Issue1497;

using static AppPrelude;
using LanguageExt.Pipes;
using LanguageExt;
using static LanguageExt.Prelude;
using LanguageExt.Traits;
using System.Runtime.CompilerServices;
using System.Net.Sockets;
using System.Net;
using System.Text;
using LanguageExt.Common;
using LanguageExt.Sys;
using M = Application;


public static class AppPrelude
{
    public static void Test()
    {
        var effect = Module<M>.producer | Module<M>.pipe | Module<M>.consumer;
// effect.Run().As().Run();
        effect.Run().As().Run.Run(new (AtomHashMap(("", "")))).Run().Ignore();
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Application<A> As<A>(this K<Application, A> self) =>
        (Application<A>)self;
}

public record ApplicationState(AtomHashMap<string, string> Cache);

public record Application<A>(StateT<ApplicationState, IO, A> Run) : K<Application, A>;

public class Application
: Deriving.Monad<Application, StateT<ApplicationState, IO>>,
  Deriving.Stateful<Application, StateT<ApplicationState, IO>, ApplicationState>,
  MonadIO<Application>
{
    public static K<Application, A> LiftIO<A>(IO<A> ma)
    {
        return new Application<A>(StateT.liftIO<ApplicationState, IO, A>(ma));
    }

    public static K<StateT<ApplicationState, IO>, A> Transform<A>(K<Application, A> fa)
    {
        return fa.As().Run;
    }

    public static K<Application, A> CoTransform<A>(K<StateT<ApplicationState, IO>, A> fa)
    {
        return new Application<A>(fa.As());
    }

    public static K<Application, A> Actions<A>(IEnumerable<K<Application, A>> fas) =>
        new Application<A>(
            from s in StateT.get<IO, ApplicationState>()
            from r in fas.Select(fa => fa.As().Run.Run(s)).Actions()
            from _ in StateT.put<IO, ApplicationState>(r.State)
            select r.Value);
}

public static class Module<M>
where M : MonadIO<M>
{
    public static ProducerT<int, M, Unit> producer =>
        from _1 in IO.lift(() => System.Console.WriteLine("Producer"))
        from _2 in ProducerT.yieldRepeatIO<M, int>(IO.pure(1))
        select unit;

    public static PipeT<int, int, M, Unit> pipe =
        from i  in PipeT.awaiting<M, int, int>() 
        from _1 in IO.lift(() => System.Console.WriteLine("Pipe"))
        from _2 in PipeT.yield<M, int, int>(i)
        select unit;

    public static ConsumerT<int, M, Unit> consumer =
        from x in ConsumerT.awaiting<M, int>()
        from _1 in IO.lift(() => System.Console.WriteLine("Consumer"))
        select unit;
}
