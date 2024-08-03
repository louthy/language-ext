using LanguageExt;
using LanguageExt.Traits;
namespace CardGame;

public static class GameExtensions
{
    public static Game<A> As<A>(this K<Game, A> ma) =>
        (Game<A>)ma;

    /// <summary>
    /// Run the Game transformer down to the IO monad
    /// </summary>
    public static IO<Option<(A Value, GameState State)>> Run<A>(this K<Game, A> ma, GameState state) =>
        ma.As().runGame.Run(state).As().Run().As();
    
    public static Game<C> SelectMany<A, B, C>(
        this K<Game, A> ma,
        Func<A, K<Game, B>> bind,
        Func<A, B, C> project) =>
        ma.As().SelectMany(bind, project);

    public static Game<C> SelectMany<A, B, C>(
        this K<IO, A> ma,
        Func<A, K<Game, B>> bind,
        Func<A, B, C> project) =>
        MonadIO.liftIO<Game, A>(ma.As()).SelectMany(bind, project); 

    public static Game<C> SelectMany<A, B, C>(
        this IO<A> ma,
        Func<A, K<Game, B>> bind,
        Func<A, B, C> project) =>
        MonadIO.liftIO<Game, A>(ma).SelectMany(bind, project); 

    public static Game<C> SelectMany<A, B, C>(
        this Pure<A> ma,
        Func<A, K<Game, B>> bind,
        Func<A, B, C> project) =>
        Game.Pure(ma.Value).SelectMany(bind, project); 
}
