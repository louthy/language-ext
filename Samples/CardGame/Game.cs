using LanguageExt;
using static LanguageExt.Prelude;

namespace CardGame;

public record Game(HashMap<Player, PlayerState> GameState, Deck Deck)
{
    public static readonly Game Zero = new ([], Deck.Empty);
    internal static readonly StateT<Game, OptionT<IO>, Unit> unitM = Pure(unit);
    internal static readonly StateT<Game, OptionT<IO>, Unit> noneM = StateT<Game>.lift(OptionT<IO>.None<Unit>());
    
    /// <summary>
    /// Play the game!
    /// </summary>
    public static StateT<Game, OptionT<IO>, Unit> play =>
        from _1 in enterPlayerNames
        from _2 in Display.introduction
        from _3 in Deck.shuffle
        from _4 in playHands
        select unit;

    /// <summary>
    /// Ask the users to enter their names
    /// </summary>
    static StateT<Game, OptionT<IO>, Unit> enterPlayerNames =>
        from _0 in Display.askToEnterPlayerName
        from _1 in iff(enterPlayerName,
                       Then: enterPlayerNames,
                       Else: unitM).As()
        select unit;

    /// <summary>
    /// Wait for the user to enter the name of a player, then add them to the game 
    /// </summary>
    static StateT<Game, OptionT<IO>, bool> enterPlayerName =>
        from name  in Console.readLine
        from added in string.IsNullOrEmpty(name)
                          ? Pure(false)
                          : addPlayer(name).Map(_ => true)
        select added;

    /// <summary>
    /// Play many rounds until the players decide to quit
    /// </summary>
    static StateT<Game, OptionT<IO>, Unit> playHands =>
        from _0 in resetPlayers
        from _1 in playHand
        from _2 in Display.askPlayAgain
        from ky in Console.readKey
        from _3 in ky.Key == ConsoleKey.Y
                       ? playHands
                       : unitM
        select unit;

    /// <summary>
    /// Play a single round
    /// </summary>
    static StateT<Game, OptionT<IO>, Unit> playHand =>
        from _1 in dealHands
        from _2 in stickOrTwists
        from _3 in gameOver
        from cr in Deck.cardsRemaining
        from _4 in Display.cardsRemaining(cr)
        select unit;

    /// <summary>
    /// Deal the initial cards to the players
    /// </summary>
    static StateT<Game, OptionT<IO>, Unit> dealHands =>
        from ps in players
        from _1 in ps.Traverse(dealHand)
        select unit;

    /// <summary>
    /// Deal the two initial cards to a player
    /// </summary>
    static StateT<Game, OptionT<IO>, Unit> dealHand(Player player) =>
        from _1 in dealCard(player)
        from _2 in dealCard(player)
        from st in Player.state(player)
        from _3 in Display.playerState(player, st)
        select unit;

    /// <summary>
    /// Deal a single card
    /// </summary>
    static StateT<Game, OptionT<IO>, Unit> dealCard(Player player) =>
        from card in Deck.deal
        from _1   in Player.addCard(player, card)
        select unit;
    
    /// <summary>
    /// For each active player check if they want to stick or twist
    /// Keep looping until the game ends
    /// </summary>
    private static StateT<Game, OptionT<IO>, Unit> stickOrTwists =>
        when(isGameActive,
             from ps in activePlayers
             from _1 in ps.Traverse(stickOrTwist)
             from _2 in stickOrTwists
             select unit)
            .As();

    /// <summary>
    /// Ask the player if they want to stick or twist, then follow their instruction
    /// </summary>
    static StateT<Game, OptionT<IO>, Unit> stickOrTwist(Player player) =>
        when(isGameActive,
             from _0 in Display.askStickOrTwist(player)
             from _1 in Player.showCards(player)
             from k  in Console.readKey
             from _2 in k.Key switch
                        {
                            ConsoleKey.S => Player.stick(player),
                            ConsoleKey.T => twist(player),
                            _            => stickOrTwistBerate(player)
                        }
             select unit)
           .As();

    /// <summary>
    /// Player wants to twist
    /// </summary>
    static StateT<Game, OptionT<IO>, Unit> twist(Player player) =>
        from card in Deck.deal
        from _1   in Player.addCard(player, card)
        from _2   in Display.showCard(card)
        from _3   in when(Player.isBust(player), Display.bust)
        select unit;

    /// <summary>
    /// Berate the user for not following instructions!
    /// </summary>
    static StateT<Game, OptionT<IO>, Unit> stickOrTwistBerate(Player player) =>
        from _1 in Display.stickOrTwistBerate
        from _2 in stickOrTwist(player)
        select unit;

    /// <summary>
    /// Show the game over summary
    /// </summary>
    static StateT<Game, OptionT<IO>, Unit> gameOver =>
        from ps  in playersState
        let top  =  ps.Choose(p => p.State.MaximumNonBustScore.Map(score => (p.Player, score)))
                      .OrderBy(p => p.score)
                      .AsEnumerableM()
                      .Map(p => p.Player) 
                      .ToSeq() 
                      .Last
        from _1 in top.Match(Some: Display.playerWins, 
                             None: Display.everyoneIsBust)
        from _2 in Display.playerStates(ps)
        select unit;
    
    /// <summary>
    /// Get the game state from the StateT monad-transformer
    /// </summary>
    public static StateT<Game, OptionT<IO>, HashMap<Player, PlayerState>> state =>
        StateT.gets<OptionT<IO>, Game, HashMap<Player, PlayerState>>(s => s.GameState);

    /// <summary>
    /// Modify the overall game-state stored in the StateT monad-transformer
    /// </summary>
    public static StateT<Game, OptionT<IO>, Unit> modifyState(
        Func<HashMap<Player, PlayerState>, HashMap<Player, PlayerState>> f) =>
        StateT.modify<OptionT<IO>, Game>(s => s with {GameState = f(s.GameState) });

    /// <summary>
    /// Return true if the game is still active
    /// </summary>
    private static StateT<Game, OptionT<IO>, bool> isGameActive =>
        from non in nonActivePlayersState
        from res in non.Exists(p => p.State.Has21 || p.State.StickState)
                        ? activePlayersState.Map(ps => ps.Count > 0)
                        : activePlayersState.Map(ps => ps.Count > 1)
        select res;
    
    /// <summary>
    /// The current high-score
    /// </summary>
    public static StateT<Game, OptionT<IO>, int> currentHighScore =>
        playersState.Map(ps => ps.Filter(p => !p.State.IsBust)
                                 .Choose(p => p.State.MaximumNonBustScore)
                                 .Max(0));

    /// <summary>
    /// Get the player's state from the StateT monad-transformer
    /// </summary>
    static StateT<Game, OptionT<IO>, Seq<(Player Player, PlayerState State)>> playersState =>
        state.Map(gs => gs.AsEnumerable().ToSeq());

    /// <summary>
    /// Get the player's that are still playing
    /// </summary>
    static StateT<Game, OptionT<IO>, Seq<(Player Player, PlayerState State)>> activePlayersState =>
        playersState.Map(ps => ps.Filter(p => p.State.StillInTheGame()));

    /// <summary>
    /// Get the player's that are still playing
    /// </summary>
    static StateT<Game, OptionT<IO>, Seq<(Player Player, PlayerState State)>> nonActivePlayersState =>
        playersState.Map(ps => ps.Filter(p => !p.State.StillInTheGame()));

    /// <summary>
    /// Get the list of players from the StateT monad-transformer
    /// </summary>
    static StateT<Game, OptionT<IO>, Seq<Player>> players =>
        playersState.Map(ps => ps.Map(p => p.Player));

    /// <summary>
    /// Get the player's that are still playing
    /// </summary>
    static StateT<Game, OptionT<IO>, Seq<Player>> activePlayers =>
        activePlayersState.Map(ps => ps.Map(p => p.Player).Strict());

    /// <summary>
    /// Reset the player's state
    /// </summary>
    public static StateT<Game, OptionT<IO>, Unit> resetPlayers =>
        modifyState(kv => kv.Map(_ => PlayerState.Zero));
    
    /// <summary>
    /// Discover if a player exists
    /// </summary>
    static StateT<Game, OptionT<IO>, bool> playerExists(string name) =>
        playerExists(new Player(name));

    /// <summary>
    /// Discover if a player exists
    /// </summary>
    static StateT<Game, OptionT<IO>, bool> playerExists(Player player) =>
        state.Map(s => s.Find(player).IsSome);

    /// <summary>
    /// Add a player to the game
    /// </summary>
    static StateT<Game, OptionT<IO>, Unit> addPlayer(string name) =>
        iff(playerExists(name),
            Then: Display.playerExists(name),
            Else: from _1 in modifyState(s => s.Add(new Player(name), PlayerState.Zero))
                  from _2 in Display.playerAdded(name)
                  select unit)
           .As();
}
