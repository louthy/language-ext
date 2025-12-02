using LanguageExt;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace CardGame;

/// <summary>
/// Pontoon / Vingt-Un / 21
/// </summary>
public partial class Game
{
    /// <summary>
    /// Play the game!
    /// </summary>
    public static K<Game, Unit> play =>
        Display.askPlayerNames >>>
        enterPlayerNames       >>>
        Display.introduction   >>>
        Deck.shuffle           >>>
        playHands;

    /// <summary>
    /// Ask the users to enter their names until `enterPlayerName` returns `false`
    /// </summary>
    static Game<Unit> enterPlayerNames =>
        when(enterPlayerName, lazy(() => enterPlayerNames)).As();

    /// <summary>
    /// Wait for the user to enter the name of a player, then add them to the game 
    /// </summary>
    static Game<bool> enterPlayerName =>
        from name in Console.readLine
        from _    in when(notEmpty(name), addPlayer(name))
        select notEmpty(name);

    /// <summary>
    /// Play many hands until the players decide to quit
    /// </summary>
    static Game<Unit> playHands =>
        from _   in initPlayers >>>
                    playHand >>>
                    Display.askPlayAgain
        from key in Console.readKey
        from __  in when(key.Key == ConsoleKey.Y, playHands)
        select unit;

    /// <summary>
    /// Play a single hand
    /// </summary>
    static Game<Unit> playHand =>
        dealHands >>>
        playRound >>>
        gameOver  >>>
        Display.cardsRemaining >>>
        lower;

    /// <summary>
    /// Deal the initial cards to the players
    /// </summary>
    static Game<Unit> dealHands =>
        Players.with(players, dealHand);

    /// <summary>
    /// Deal the two initial cards to a player
    /// </summary>
    static Game<Unit> dealHand =>
        from cs     in dealCard  >>> dealCard
        from player in Player.current
        from state  in Player.state
        from _      in Display.playerState(player, state)
        select unit;

    /// <summary>
    /// Deal a single card
    /// </summary>
    static Game<Unit> dealCard =>
        from card in Deck.deal
        from _    in Player.addCard(card)
        select unit;
    
    /// <summary>
    /// For each active player, check if they want to stick or twist
    /// Keep looping until the game ends
    /// </summary>
    static Game<Unit> playRound =>
        when(isGameActive,
             from _ in Players.with(activePlayers, stickOrTwist) 
             from r in playRound
             select r)
            .As();

    /// <summary>
    /// Ask the player if they want to stick or twist, then follow their instruction
    /// </summary>
    static Game<Unit> stickOrTwist =>
        when(isGameActive,
             from _      in Display.askStickOrTwist >>>
                            Player.showCards
             from key    in Console.readKey
             from __     in key.Key switch
                            {
                                ConsoleKey.S => Player.stick,
                                ConsoleKey.T => twist,
                                _            => stickOrTwistBerate
                            }
             select unit)
           .As();

    /// <summary>
    /// Player wants to twist
    /// </summary>
    static Game<Unit> twist =>
        from card in Deck.deal
        from _    in Player.addCard(card) >>>
                     Display.showCard(card) >>>
                     when(Player.isBust, Display.bust)
        select unit;

    /// <summary>
    /// Berate the user for not following instructions!
    /// </summary>
    static K<Game, Unit> stickOrTwistBerate =>
        Display.stickOrTwistBerate >>>
        stickOrTwist;

    /// <summary>
    /// Show the game over summary
    /// </summary>
    static Game<Unit> gameOver =>
        from ws in winners
        from ps in playersState
        from _  in Display.winners(ws) >>>
                   Display.playerStates(ps)
        select unit;
}
