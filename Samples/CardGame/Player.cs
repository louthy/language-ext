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
    public static StateT<Game, OptionT<IO>, PlayerState> state(Player player) =>
        from s in Game.state
        from p in OptionT<IO>.lift(s.Find(player))
        select p;

    /// <summary>
    /// Reset the player's state
    /// </summary>
    public static StateT<Game, OptionT<IO>, Unit> reset(Player player) =>
        modify(player, _ => PlayerState.Zero); 

    /// <summary>
    /// Show the player's cards
    /// </summary>
    public static StateT<Game, OptionT<IO>, Unit> showCards(Player player) =>
        from state in state(player)
        from score in Game.currentHighScore
        from cards in Display.showCardsAndScores(state.Cards, state.Scores, score)
        select unit;

    /// <summary>
    /// Modify a player's game-state stored in the StateT monad-transformer
    /// </summary>
    static StateT<Game, OptionT<IO>, Unit> modify(
        Player player,
        Func<PlayerState, PlayerState> f) =>
        from p in state(player)
        from _ in Game.modifyState(s => s.SetItem(player, f(p)))
        select unit;
    
    /// <summary>
    /// Add a card to a player's state
    /// </summary>
    public static StateT<Game, OptionT<IO>, Unit> addCard(Player player, Card card) =>
        modify(player, p => p.AddCard(card));

    /// <summary>
    /// Set a player's state to stick
    /// </summary>
    public static StateT<Game, OptionT<IO>, Unit> stick(Player player) =>
        modify(player, p => p.Stick());

    /// <summary>
    /// Gets whether the player is bust or not
    /// </summary>
    public static StateT<Game, OptionT<IO>, bool> isBust(Player player) =>
        state(player).Map(p => p.IsBust);
   
}

public record PlayerState(Seq<Card> Cards, bool StickState)
{
    public static readonly PlayerState Zero = new ([], false);

    public PlayerState AddCard(Card card) =>
        this with { Cards = Cards.Add(card) };
    
    public PlayerState Stick() =>
        this with { StickState = true };

    public Seq<int> Scores =>
        Cards.Map(c => c.FaceValues)
             .Fold(Seq(Seq<int>()),
                   (s, vs) =>
                       from x in s
                       from v in vs
                       select x.Add(v))
             .Map(s => s.Sum<Seq, int>())
             .Distinct() 
             .OrderBy(s => s)
             .AsEnumerableM()
             .ToSeq();

    public bool StillInTheGame() =>
        !Scores.Exists(s => s == 21) && 
        !StickState && 
        !IsBust;

    public Option<int> MaximumNonBustScore =>
        Scores.Filter(s => s <= 21).Last;

    public bool Has21 =>
        Scores.Exists(s => s == 21);

    public bool IsBust =>
        Scores.ForAll(s => s > 21);
}
