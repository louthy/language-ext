using LanguageExt;
using LanguageExt.Traits;
namespace CardGame;

public static class GameMExtensions
{
    public static GameM<A> As<A>(this K<GameM, A> ma) =>
        (GameM<A>)ma;

    /// <summary>
    /// Run the Game transformer down to the IO monad
    /// </summary>
    public static IO<Option<(A Value, GameState State)>> Run<A>(this K<GameM, A> ma, GameState state) =>
        ma.As().runGame.Run(state).As().Run().As();
    
    public static GameM<C> SelectMany<A, B, C>(
        this K<GameM, A> ma,
        Func<A, K<GameM, B>> bind,
        Func<A, B, C> project) =>
        ma.As().SelectMany(bind, project);

    public static GameM<C> SelectMany<A, B, C>(
        this K<IO, A> ma,
        Func<A, K<GameM, B>> bind,
        Func<A, B, C> project) =>
        MonadIO.liftIO<GameM, A>(ma.As()).SelectMany(bind, project); 

    public static GameM<C> SelectMany<A, B, C>(
        this IO<A> ma,
        Func<A, K<GameM, B>> bind,
        Func<A, B, C> project) =>
        MonadIO.liftIO<GameM, A>(ma).SelectMany(bind, project); 

    public static GameM<C> SelectMany<A, B, C>(
        this Pure<A> ma,
        Func<A, K<GameM, B>> bind,
        Func<A, B, C> project) =>
        GameM.Pure(ma.Value).SelectMany(bind, project); 
}
