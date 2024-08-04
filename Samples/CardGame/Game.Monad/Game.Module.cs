using LanguageExt;
using static LanguageExt.Prelude;
namespace CardGame;

public partial class Game
{
    /// <summary>
    /// Cached unit returning Game monad
    /// </summary>
    public static readonly Game<Unit> unitM = 
        Pure(unit).As();

    /// <summary>
    /// Use this to cancel the game
    /// </summary>
    /// <remarks>Represents a None state in the embedded OptionT transformer.</remarks>
    public static readonly Game<Unit> cancel =
        lift(Option<Unit>.None);
    
    /// <summary>
    /// Get the game-state
    /// </summary>
    public static Game<GameState> state =>
        new (StateT.get<OptionT<IO>, GameState>());

    /// <summary>
    /// Return true if the game is still active
    /// </summary>
    public static Game<bool> isGameActive =>
        from non in nonActivePlayersState
        from res in non.Exists(p => p.State.Has21 || p.State.StickState)
                        ? activePlayersState.Map(ps => ps.Count > 0)
                        : activePlayersState.Map(ps => ps.Count > 1)
        select res;
    
    /// <summary>
    /// The current high-score
    /// </summary>
    public static Game<int> currentHighScore =>
        playersState.Map(ps => ps.Filter(p => !p.State.IsBust)
                                 .Choose(p => p.State.MaximumNonBustScore)
                                 .Max(0));

    /// <summary>
    /// Get the player's state from the StateT monad-transformer
    /// </summary>
    public static Game<Seq<(Player Player, PlayerState State)>> playersState =>
        state.Map(s => s.State).Map(gs => gs.AsEnumerable().ToSeq());

    /// <summary>
    /// Get the player's that are still playing
    /// </summary>
    public static Game<Seq<(Player Player, PlayerState State)>> activePlayersState =>
        playersState.Map(ps => ps.Filter(p => p.State.StillInTheGame()));

    /// <summary>
    /// Get the player's that are still playing
    /// </summary>
    public static Game<Seq<(Player Player, PlayerState State)>> nonActivePlayersState =>
        playersState.Map(ps => ps.Filter(p => !p.State.StillInTheGame()));

    /// <summary>
    /// Get the list of players from the StateT monad-transformer
    /// </summary>
    public static Game<Seq<Player>> players =>
        playersState.Map(ps => ps.Map(p => p.Player));

    /// <summary>
    /// Get the player's that are still playing
    /// </summary>
    public static Game<Seq<Player>> activePlayers =>
        activePlayersState.Map(ps => ps.Map(p => p.Player).Strict());

    /// <summary>
    /// Initialise the player's state
    /// </summary>
    public static Game<Unit> initPlayers =>
        modifyPlayers(kv => kv.Map(_ => PlayerState.Zero));

    /// <summary>
    /// Return the winners of the game (there may be multiple winners!)
    /// </summary>
    public static Game<Seq<(Player Player, int Score)>> winners =>
        from ps in playersState
        select ps.Choose(p => p.State.MaximumNonBustScore.Map(score => (p.Player, Score: score)))
                 .OrderByDescending(p => p.Score)
                 .Select(Seq)
                 .Reduce((ss, sp) => (from s in ss
                                      from p in sp
                                      select s.Score == p.Score).ForAll(x => x)
                                         ? ss + sp
                                         : ss);
    
    /// <summary>
    /// Discover if a player exists
    /// </summary>
    public static Game<bool> playerExists(string name) =>
        playerExists(new Player(name));

    /// <summary>
    /// Discover if a player exists
    /// </summary>
    public static Game<bool> playerExists(Player player) =>
        state.Map(s => s.State)
             .Map(s => s.Find(player).IsSome);

    /// <summary>
    /// Add a player to the game
    /// </summary>
    public static Game<Unit> addPlayer(string name) =>
        iff(playerExists(name),
            Then: Display.playerExists(name),
            Else: from _1 in modifyPlayers(s => s.Add(new Player(name), PlayerState.Zero))
                  from _2 in Display.playerAdded(name)
                  select unit)
           .As();

    /// <summary>
    /// Lazy wrapper
    /// </summary>
    public static Game<A> lazy<A>(Func<Game<A>> f) =>
        unitM.Bind(_ => f());
    
    /// <summary>
    /// Lift an option into the Game - None will cancel the game
    /// </summary>
    public static Game<A> lift<A>(Option<A> ma) => 
        new (StateT<GameState>.lift(OptionT<IO>.lift(ma)));
    
    /// <summary>
    /// Lift an IO operation into the Game
    /// </summary>
    public static Game<A> liftIO<A>(IO<A> ma) => 
        new (StateT.liftIO<GameState, OptionT<IO>, A>(ma));
    
    /// <summary>
    /// Represents a None state in the embedded OptionT transformer
    /// </summary>
    /// <remarks>Use this to cancel a Game computation</remarks>
    public static Game<A> lift<A>(StateT<GameState, OptionT<IO>, A> ma) => 
        new (ma);

    /// <summary>
    /// Map the game state to a new value
    /// </summary>
    public static Game<A> gets<A>(Func<GameState, A> f) =>
        new (StateT.gets<OptionT<IO>, GameState, A>(f));
    
    /// <summary>
    /// Map the game state to a new value and update it in the monad
    /// </summary>
    public static Game<Unit> modify(Func<GameState, GameState> f) =>
        new (StateT.modify<OptionT<IO>, GameState>(f));
    
    /// <summary>
    /// Modify the player map
    /// </summary>
    public static Game<Unit> modifyPlayers(Func<HashMap<Player, PlayerState>, HashMap<Player, PlayerState>> f) =>
        modify(s => s with { State = f(s.State) });
}
