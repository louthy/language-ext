using LanguageExt;
using static LanguageExt.Prelude;

namespace CardGame;

public record Player(string Name)
{
    public override string ToString() => 
        Name;
    
    /// <summary>
    /// Get a player's state from the StateT monad-transformer
    /// </summary>
    public static GameM<PlayerState> state(Player player) =>
        from s in GameM.state.Map(s => s.State)
        from p in GameM.lift(s.Find(player))
        select p;

    /// <summary>
    /// Show the player's cards
    /// </summary>
    public static GameM<Unit> showCards(Player player) =>
        from state in state(player)
        from score in GameM.currentHighScore
        from cards in Display.showCardsAndScores(state.Cards, state.Scores, score)
        select unit;

    /// <summary>
    /// Modify a player's game-state stored in the StateT monad-transformer
    /// </summary>
    static GameM<Unit> modify(
        Player player,
        Func<PlayerState, PlayerState> f) =>
        from p in state(player)
        from _ in GameM.modifyPlayers(s => s.SetItem(player, f(p)))
        select unit;
    
    /// <summary>
    /// Add a card to a player's state
    /// </summary>
    public static GameM<Unit> addCard(Player player, Card card) =>
        modify(player, p => p.AddCard(card));

    /// <summary>
    /// Set a player's state to stick
    /// </summary>
    public static GameM<Unit> stick(Player player) =>
        modify(player, p => p.Stick());

    /// <summary>
    /// Gets whether the player is bust or not
    /// </summary>
    public static GameM<bool> isBust(Player player) =>
        state(player).Map(p => p.IsBust);
   
}
