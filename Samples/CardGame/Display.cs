using LanguageExt;
using static LanguageExt.Prelude;

namespace CardGame;

/// <summary>
/// UI messages
/// </summary>
public static class Display
{
    public static readonly IO<Unit> introduction =
        Console.writeLine("Let's play...");

    public static readonly IO<Unit> askPlayerNames =
        Console.writeLine("Enter a player name, or just press enter complete");
    
    public static readonly IO<Unit> askPlayAgain =
        Console.writeLine("Play again (Y/N)");
    
    public static readonly IO<Unit> deckFinished =
        Console.writeLine("The deck is out of cards");
    
    public static IO<Unit> cardsRemaining(int remain) =>
        Console.writeLine($"{remain} cards remaining in the deck");

    public static readonly IO<Unit> bust =
        Console.writeLine("\tBust!");

    public static IO<Unit> playerExists(string name) =>
        actions(Console.writeLine($"Player '{name}' already exists"),
                Console.writeLine("Please pick a unique name")).As();

    public static IO<Unit> playerAdded(string name) =>
        Console.writeLine($"'{name}' added to the game");

    public static IO<Unit> playerStates(Seq<(Player Player, PlayerState State)> players) =>
        players.Traverse(p => playerState(p.Player, p.State)).Map(_ => unit).As();

    public static IO<Unit> playerState(Player player, PlayerState state) =>
        state.StickState
            ? Console.writeLine($"{player.Name} {state.Cards}, possible scores {state.Scores} [STICK]")
            : Console.writeLine($"{player.Name} {state.Cards}, possible scores {state.Scores}");

    public static IO<Unit> winners(Seq<(Player Player, int Score)> winners) =>
        winners switch
        {
            []      => everyoneIsBust,
            [var p] => Console.writeLine($"{p.Player.Name} is the winner with {p.Score}!"),
            var ps  => Console.writeLine($"{ps.Map(p => p.Player.Name).ToFullString()} have won with {ps[0].Score}!")
        };

    public static IO<Unit> everyoneIsBust =>
        Console.writeLine("Everyone's bust!");
    
    public static IO<Unit> askStickOrTwist(Player player) =>
        Console.writeLine($"{player.Name}, stick or twist? (S/T)");
    
    public static readonly IO<Unit> stickOrTwistBerate =
        Console.writeLine("'S' key for stick, 'T' for twist!");

    public static IO<Unit> showCard(Card card) =>
        Console.writeLine($"\t{card}");

    public static IO<Unit> showCardsAndScores(Seq<Card> cards, Seq<int> scores, int highScore) =>
        Console.writeLine($"\t{cards}, possible scores {scores}, high-score: {highScore}");
}
