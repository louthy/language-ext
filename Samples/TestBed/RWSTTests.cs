using System;
using LanguageExt;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace TestBed;


public record AppConfig(int X, int Y);

public record AppState(int Value)
{
    public AppState SetValue(int value) =>
        this with { Value = value };
}

public static class App
{
    public static App<M, A> As<M, A>(this K<App<M>, A> ma)
        where M : Monad<M>, SemigroupK<M> =>
        (App<M, A>)ma;
    
    public static K<M, (A Value, AppState State)> Run<M, A>(this K<App<M>, A> ma, AppConfig config, AppState state)
        where M : Monad<M>, SemigroupK<M> =>
        ma.As().runApp.Run(config, state).Map(
            ma => ma switch
                  {
                      var (value, _, newState) => (value, newState)
                  });

    public static App<M, AppConfig> config<M>() 
        where M : Monad<M>, SemigroupK<M> =>
        Readable.ask<App<M>, AppConfig>().As();

    public static App<M, Unit> modify<M>(Func<AppState, AppState> f) 
        where M : Monad<M>, SemigroupK<M> =>
        Stateful.modify<App<M>, AppState>(f).As();
}

public class App<M> : 
    MonadT<App<M>, M>,
    Readable<App<M>, AppConfig>,
    Stateful<App<M>, AppState>
    where M : Monad<M>, SemigroupK<M>
{
    public static K<App<M>, B> Bind<A, B>(K<App<M>, A> ma, Func<A, K<App<M>, B>> f) =>
        new App<M, B>(ma.As().runApp.Bind(x => f(x).As().runApp));

    public static K<App<M>, B> Map<A, B>(Func<A, B> f, K<App<M>, A> ma) => 
        new App<M, B>(ma.As().runApp.Map(f));

    public static K<App<M>, A> Pure<A>(A value) => 
        new App<M, A>(RWST<AppConfig, Unit, AppState, M, A>.Pure(value));

    public static K<App<M>, B> Apply<A, B>(K<App<M>, Func<A, B>> mf, K<App<M>, A> ma) => 
        new App<M, B>(mf.As().runApp.Apply(ma.As().runApp));

    public static K<App<M>, A> Lift<A>(K<M, A> ma) => 
        new App<M, A>(RWST<AppConfig, Unit, AppState, M, A>.Lift(ma));

    public static K<App<M>, A> Asks<A>(Func<AppConfig, A> f) => 
        new App<M, A>(RWST<AppConfig, Unit, AppState, M, A>.Asks(f));

    public static K<App<M>, A> Local<A>(Func<AppConfig, AppConfig> f, K<App<M>, A> ma) => 
        new App<M, A>(ma.As().runApp.Local(f));

    public static K<App<M>, Unit> Put(AppState value) => 
        new App<M, Unit>(RWST<AppConfig, Unit, AppState, M, Unit>.Put(value));

    public static K<App<M>, Unit> Modify(Func<AppState, AppState> modify) => 
        new App<M, Unit>(RWST<AppConfig, Unit, AppState, M, Unit>.Modify(modify));

    public static K<App<M>, A> Gets<A>(Func<AppState, A> f) => 
        new App<M, A>(RWST<AppConfig, Unit, AppState, M, A>.Gets(f));
}

public readonly record struct App<M, A>(RWST<AppConfig, Unit, AppState, M, A> runApp) : K<App<M>, A>
    where M : Monad<M>, SemigroupK<M>
{
    // Your application monad implementation
}

public static class RwstTest
{
    static IO<Unit> writeLine(object value) =>
        IO.lift(() => Console.WriteLine(value));

    static void Test()
    {
var app = from config in App.config<IO>()
          from value  in App<IO>.Pure(config.X * config.Y)
          from _1     in App.modify<IO>(s => s.SetValue(value)) 
          from _2     in writeLine(value) 
          select unit;
    }
}
