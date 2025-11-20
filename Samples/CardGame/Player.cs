using LanguageExt;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace CardGame;

public record Player(string Name)
{
    public override string ToString() => 
        Name;

    /// <summary>
    /// Run a computation in a `Player` local context 
    /// </summary>
    /// <param name="player"></param>
    /// <param name="ma"></param>
    /// <typeparam name="A"></typeparam>
    /// <returns></returns>
    public static Game<A> with<A>(Player player, Game<A> ma) =>
        from cp in Game.gets(s => s.CurrentPlayer)
        from _1 in setCurrent(player)
        from rs in ma >> setCurrent(cp)
        select rs;

    /// <summary>
    /// Get the current player.  If there isn't one, then `Game.cancel` is raised
    /// </summary>
    public static Game<Player> current =>
        from p in Game.gets(s => s.CurrentPlayer)
        from r in Game.lift(p)
        select r;

    /// <summary>
    /// Set the current player
    /// </summary>
    public static Game<Unit> setCurrent(Player player) =>
        Game.modify(s => s with { CurrentPlayer = player }).As();

    /// <summary>
    /// Set the current player
    /// </summary>
    public static Game<Unit> setCurrent(Option<Player> player) =>
        Game.modify(s => s with { CurrentPlayer = player }).As();
    
    /// <summary>
    /// Get a player's state from the StateT monad-transformer
    /// </summary>
    public static Game<PlayerState> state =>
        from pl in current
        from s1 in Game.state.Map(s => s.State)
        from s2 in Game.lift(s1.Find(pl))
        select s2;

    /// <summary>
    /// Show the player's cards
    /// </summary>
    public static Game<Unit> showCards =>
        from state in state
        from score in Game.currentHighScore
        from cards in Display.showCardsAndScores(state.Cards, state.Scores, score)
        select unit;

    /// <summary>
    /// Modify a player's game-state stored in the StateT monad-transformer
    /// </summary>
    static Game<Unit> modify(
        Func<PlayerState, PlayerState> f) =>
        from p in current
        from s in state
        from _ in Game.modifyPlayers(s1 => s1.SetItem(p, f(s)))
        select unit;
    
    /// <summary>
    /// Add a card to a player's state
    /// </summary>
    public static Game<Unit> addCard(Card card) =>
        modify(p => p.AddCard(card));

    /// <summary>
    /// Set a player's state to stick
    /// </summary>
    public static Game<Unit> stick =>
        modify(p => p.Stick());

    /// <summary>
    /// Gets whether the player is bust or not
    /// </summary>
    public static Game<bool> isBust =>
        state.Map(p => p.IsBust);
}
