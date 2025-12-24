using LanguageExt;
using LanguageExt.Traits;
namespace CardGame;

public static class GameExtensions
{
    extension<A>(K<Game, A> ma)
    {
        public Game<A> As() =>
            (Game<A>)ma;

        /// <summary>
        /// Run the Game transformer down to the IO monad
        /// </summary>
        public IO<Option<(A Value, GameState State)>> Run(GameState state) =>
            ma.As().runGame.Run(state).As().Run().As();

        public Game<C> SelectMany<B, C>(Func<A, K<Game, B>> bind, Func<A, B, C> project) =>
            ma.As().SelectMany(bind, project);
        
        public static Game<A> operator + (K<Game, A> lhs) =>
            (Game<A>)lhs;
        
        public static Game<A> operator >> (K<Game, A> lhs, Lower _) =>
            (Game<A>)lhs;
    }

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
