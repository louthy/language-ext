using LanguageExt;
using LanguageExt.Traits;
namespace CardGame;

public partial class Game :
    Deriving<Game, StateT<GameState, OptionT<IO>>>,
    Deriving.Monad<Game, StateT<GameState, OptionT<IO>>>,
    Deriving.SemigroupK<Game, StateT<GameState, OptionT<IO>>>,
    Deriving.Stateful<Game, StateT<GameState, OptionT<IO>>, GameState>
{
    public static K<StateT<GameState, OptionT<IO>>, A> Transform<A>(K<Game, A> fa) =>
        fa.As().runGame;

    public static K<Game, A> CoTransform<A>(K<StateT<GameState, OptionT<IO>>, A> fa) => 
        new Game<A>(fa.As());
}
