using LanguageExt;
using LanguageExt.Traits;
namespace CardGame;

public partial class GameM : 
    Monad<GameM>, 
    SemiAlternative<GameM>,
    Stateful<GameM, GameState>
{
    public static K<GameM, B> Bind<A, B>(K<GameM, A> ma, Func<A, K<GameM, B>> f) =>
        new GameM<B>(ma.As().runGame.Bind(a => f(a).As().runGame));

    public static K<GameM, B> Map<A, B>(Func<A, B> f, K<GameM, A> ma) => 
        new GameM<B>(ma.As().runGame.Map(f));

    public static K<GameM, A> Pure<A>(A value) => 
        new GameM<A>(Prelude.Pure(value));

    public static K<GameM, B> Apply<A, B>(K<GameM, Func<A, B>> mf, K<GameM, A> ma) => 
        new GameM<B>(mf.As().runGame.Apply(ma.As().runGame).As());

    public static K<GameM, A> Combine<A>(K<GameM, A> lhs, K<GameM, A> rhs) =>
        new GameM<A>(lhs.As().runGame.Combine(rhs.As().runGame).As());

    public static K<GameM, Unit> Put(GameState value) => 
        new GameM<Unit>(StateT.put<OptionT<IO>, GameState>(value));

    public static K<GameM, Unit> Modify(Func<GameState, GameState> modify) => 
        new GameM<Unit>(StateT.modify<OptionT<IO>, GameState>(modify));

    public static K<GameM, A> Gets<A>(Func<GameState, A> f) => 
        new GameM<A>(StateT.gets<OptionT<IO>, GameState, A>(f));

    public static K<GameM, A> LiftIO<A>(IO<A> ma) => 
        new GameM<A>(MonadIO.liftIO<StateT<GameState, OptionT<IO>>, A>(ma).As());
}
