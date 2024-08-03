using LanguageExt;
using static LanguageExt.Prelude;

namespace CardGame;

/// <summary>
/// The game's internal state
/// </summary>
public record GameState(HashMap<Player, PlayerState> State, Deck Deck, Option<Player> CurrentPlayer)
{
    public static readonly GameState Zero = new ([], Deck.Empty, None);
}
