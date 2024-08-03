using LanguageExt;
using static LanguageExt.Prelude;

namespace CardGame;

public record Player(string Name)
{
    public override string ToString() => 
        Name;

    /// <summary>
    /// Run a computation in a Player local context 
    /// </summary>
    /// <param name="player"></param>
    /// <param name="ma"></param>
    /// <typeparam name="A"></typeparam>
    /// <returns></returns>
    public static GameM<A> with<A>(Player player, GameM<A> ma) =>
        from p in GameM.gets(s => s.CurrentPlayer)
        from a in GameM.modify(s => s with { CurrentPlayer = player })
        from r in ma
        from b in GameM.modify(s => s with { CurrentPlayer = p })
        select r;

    /// <summary>
    /// Get the current player.  If there isn't one, then GameM.cancel is raised
    /// </summary>
    public static GameM<Player> current =>
        from p in GameM.gets(s => s.CurrentPlayer)
        from r in GameM.lift(p)
        select r;

    /// <summary>
    /// Get a player's state from the StateT monad-transformer
    /// </summary>
    public static GameM<PlayerState> state =>
        from pl in current
        from s1 in GameM.state.Map(s => s.State)
        from s2 in GameM.lift(s1.Find(pl))
        select s2;

    /// <summary>
    /// Show the player's cards
    /// </summary>
    public static GameM<Unit> showCards =>
        from state in state
        from score in GameM.currentHighScore
        from cards in Display.showCardsAndScores(state.Cards, state.Scores, score)
        select unit;

    /// <summary>
    /// Modify a player's game-state stored in the StateT monad-transformer
    /// </summary>
    static GameM<Unit> modify(
        Func<PlayerState, PlayerState> f) =>
        from p in current
        from s in state
        from _ in GameM.modifyPlayers(s1 => s1.SetItem(p, f(s)))
        select unit;
    
    /// <summary>
    /// Add a card to a player's state
    /// </summary>
    public static GameM<Unit> addCard(Card card) =>
        modify(p => p.AddCard(card));

    /// <summary>
    /// Set a player's state to stick
    /// </summary>
    public static GameM<Unit> stick =>
        modify(p => p.Stick());

    /// <summary>
    /// Gets whether the player is bust or not
    /// </summary>
    public static GameM<bool> isBust =>
        state.Map(p => p.IsBust);
}
