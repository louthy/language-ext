using LanguageExt;
using LanguageExt.Traits;
namespace CardGame;

public partial class Game : 
    Monad<Game>, 
    SemigroupK<Game>,
    Stateful<Game, GameState>
{
    public static K<Game, B> Bind<A, B>(K<Game, A> ma, Func<A, K<Game, B>> f) =>
        new Game<B>(ma.As().runGame.Bind(a => f(a).As().runGame));

    public static K<Game, B> Map<A, B>(Func<A, B> f, K<Game, A> ma) => 
        new Game<B>(ma.As().runGame.Map(f));

    public static K<Game, A> Pure<A>(A value) => 
        new Game<A>(Prelude.Pure(value));

    public static K<Game, B> Apply<A, B>(K<Game, Func<A, B>> mf, K<Game, A> ma) => 
        new Game<B>(mf.As().runGame.Apply(ma.As().runGame).As());

    public static K<Game, A> Combine<A>(K<Game, A> lhs, K<Game, A> rhs) =>
        new Game<A>(lhs.As().runGame.Combine(rhs.As().runGame).As());

    public static K<Game, Unit> Put(GameState value) => 
        new Game<Unit>(StateT.put<OptionT<IO>, GameState>(value));

    public static K<Game, Unit> Modify(Func<GameState, GameState> modify) => 
        new Game<Unit>(StateT.modify<OptionT<IO>, GameState>(modify));

    public static K<Game, A> Gets<A>(Func<GameState, A> f) => 
        new Game<A>(StateT.gets<OptionT<IO>, GameState, A>(f));

    public static K<Game, A> LiftIO<A>(IO<A> ma) => 
        new Game<A>(MonadIO.liftIO<StateT<GameState, OptionT<IO>>, A>(ma).As());
}
