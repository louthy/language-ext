using LanguageExt;
using static LanguageExt.Prelude;

namespace CardGame;

/// <summary>
/// Pontoon / Vingt-Un / 21
/// </summary>
public static class Game
{
    /// <summary>
    /// Play the game!
    /// </summary>
    public static GameM<Unit> play =>
        from _1 in enterPlayerNames
        from _2 in Display.introduction
        from _3 in Deck.shuffle
        from _4 in playHands
        select unit;

    /// <summary>
    /// Ask the users to enter their names
    /// </summary>
    static GameM<Unit> enterPlayerNames =>
        from _0 in Display.askToEnterPlayerName
        from _1 in when(enterPlayerName, enterPlayerNames)
        select unit;

    /// <summary>
    /// Wait for the user to enter the name of a player, then add them to the game 
    /// </summary>
    static GameM<bool> enterPlayerName =>
        from name  in Console.readLine
        from added in string.IsNullOrEmpty(name)
                          ? Pure(false)
                          : GameM.addPlayer(name).Map(_ => true)
        select added;

    /// <summary>
    /// Play many rounds until the players decide to quit
    /// </summary>
    static GameM<Unit> playHands =>
        from _0 in GameM.resetPlayers
        from _1 in playHand
        from _2 in Display.askPlayAgain
        from ky in Console.readKey
        from _3 in when(ky.Key == ConsoleKey.Y, playHands)
        select unit;

    /// <summary>
    /// Play a single round
    /// </summary>
    static GameM<Unit> playHand =>
        from _1 in dealHands
        from _2 in stickOrTwists
        from _3 in gameOver
        from cr in Deck.cardsRemaining
        from _4 in Display.cardsRemaining(cr)
        select unit;

    /// <summary>
    /// Deal the initial cards to the players
    /// </summary>
    static GameM<Unit> dealHands =>
        from ps in GameM.players
        from _1 in ps.Traverse(dealHand)
        select unit;

    /// <summary>
    /// Deal the two initial cards to a player
    /// </summary>
    static GameM<Unit> dealHand(Player player) =>
        from _1 in dealCard(player)
        from _2 in dealCard(player)
        from st in Player.state(player)
        from _3 in Display.playerState(player, st)
        select unit;

    /// <summary>
    /// Deal a single card
    /// </summary>
    static GameM<Unit> dealCard(Player player) =>
        from card in Deck.deal
        from _1   in Player.addCard(player, card)
        select unit;
    
    /// <summary>
    /// For each active player check if they want to stick or twist
    /// Keep looping until the game ends
    /// </summary>
    private static GameM<Unit> stickOrTwists =>
        when(GameM.isGameActive,
             from ps in GameM.activePlayers
             from _1 in ps.Traverse(stickOrTwist)
             from _2 in stickOrTwists
             select unit)
            .As();

    /// <summary>
    /// Ask the player if they want to stick or twist, then follow their instruction
    /// </summary>
    static GameM<Unit> stickOrTwist(Player player) =>
        when(GameM.isGameActive,
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
    static GameM<Unit> twist(Player player) =>
        from card in Deck.deal
        from _1   in Player.addCard(player, card)
        from _2   in Display.showCard(card)
        from _3   in when(Player.isBust(player), Display.bust)
        select unit;

    /// <summary>
    /// Berate the user for not following instructions!
    /// </summary>
    static GameM<Unit> stickOrTwistBerate(Player player) =>
        from _1 in Display.stickOrTwistBerate
        from _2 in stickOrTwist(player)
        select unit;

    /// <summary>
    /// Show the game over summary
    /// </summary>
    static GameM<Unit> gameOver =>
        from ps  in GameM.playersState
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
}
