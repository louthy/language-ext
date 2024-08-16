using LanguageExt;
using static LanguageExt.Prelude;

namespace CardGame;

/// <summary>
/// UI messages
/// </summary>
public static class Display
{
    public static readonly Game<Unit> introduction =
        Console.writeLine("Let's play...");

    public static readonly Game<Unit> askPlayerNames =
        Console.writeLine("Enter a player name, or just press enter complete");
    
    public static readonly Game<Unit> askPlayAgain =
        Console.writeLine("Play again (Y/N)");
    
    public static readonly Game<Unit> deckFinished =
        Console.writeLine("The deck is out of cards");
    
    public static Game<Unit> cardsRemaining(int remain) =>
        Console.writeLine($"{remain} cards remaining in the deck");

    public static readonly Game<Unit> bust =
        Console.writeLine("\tBust!");

    public static Game<Unit> playerExists(string name) =>
        Console.writeLine($"Player '{name}' already exists") >>
        Console.writeLine("Please pick a unique name");

    public static Game<Unit> playerAdded(string name) =>
        Console.writeLine($"'{name}' added to the game");

    public static Game<Unit> playerStates(Seq<(Player Player, PlayerState State)> players) =>
        players.Traverse(p => playerState(p.Player, p.State)).Map(_ => unit).As();

    public static Game<Unit> playerState(Player player, PlayerState state) =>
        state.StickState
            ? Console.writeLine($"{player.Name} {state.Cards}, possible scores {state.Scores} [STICK]")
            : Console.writeLine($"{player.Name} {state.Cards}, possible scores {state.Scores}");

    public static Game<Unit> winners(Seq<(Player Player, int Score)> winners) =>
        winners switch
        {
            []      => everyoneIsBust,
            [var p] => Console.writeLine($"{p.Player.Name} is the winner with {p.Score}!"),
            var ps  => Console.writeLine($"{ps.Map(p => p.Player.Name).ToFullString()} have won with {ps[0].Score}!")
        };

    public static Game<Unit> everyoneIsBust =>
        Console.writeLine("Everyone's bust!");
    
    public static Game<Unit> askStickOrTwist(Player player) =>
        Console.writeLine($"{player.Name}, stick or twist? (S/T)");
    
    public static readonly Game<Unit> stickOrTwistBerate =
        Console.writeLine("'S' key for stick, 'T' for twist!");

    public static Game<Unit> showCard(Card card) =>
        Console.writeLine($"\t{card}");

    public static Game<Unit> showCardsAndScores(Seq<Card> cards, Seq<int> scores, int highScore) =>
        Console.writeLine($"\t{cards}, possible scores {scores}, high-score: {highScore}");
}
