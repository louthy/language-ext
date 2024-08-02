using LanguageExt;
using static LanguageExt.Prelude;

namespace CardGame;

public static class Display
{
    public static readonly IO<Unit> introduction =
        Console.writeLine("Let's play...");

    public static readonly IO<Unit> askToEnterPlayerName =
        Console.writeLine("Enter a player name, or just press enter complete");
    
    public static readonly IO<Unit> askPlayAgain =
        Console.writeLine("Play again (Y/N)");
    
    public static readonly IO<Unit> deckFinished =
        Console.writeLine("The deck is out of cards");
    
    public static IO<Unit> cardsRemaining(int remain) =>
        Console.writeLine($"{remain} cards remaining in the deck");

    public static readonly IO<Unit> bust =
        Console.writeLine("Bust!");

    public static IO<Unit> playerExists(string name) =>
        actions(Console.writeLine($"Player '{name}' already exists"),
                Console.writeLine("Please pick a unique name")).As();

    public static IO<Unit> playerAdded(string name) =>
        Console.writeLine($"'{name}' added to the game");

    public static IO<Unit> playerStates(Seq<(Player Player, PlayerState State)> players) =>
        players.Traverse(p => playerState(p.Player, p.State)).Map(_ => unit).As();

    public static IO<Unit> playerState(Player player, PlayerState state) =>
        state.StickState
            ? Console.writeLine($"Player: {player.Name} {state.Cards}, possible scores {state.Scores} [STICK]")
            : Console.writeLine($"Player: {player.Name} {state.Cards}, possible scores {state.Scores}");

    public static IO<Unit> playerWins(Player player) =>
        Console.writeLine($"{player.Name} is the winner!");

    public static IO<Unit> everyoneIsBust() =>
        Console.writeLine("Everyone's bust!");
    
    public static IO<Unit> askStickOrTwist(Player player) =>
        Console.writeLine($"Player: {player.Name}, stick or twist? (S/T)");
    
    public static readonly IO<Unit> stickOrTwistBerate =
        Console.writeLine("'S' key for stick, 'T' for twist!");

    public static IO<Unit> showCard(Card card) =>
        Console.writeLine($"\t{card}");

    public static IO<Unit> showCardsAndScores(Seq<Card> cards, Seq<int> scores, int highScore) =>
        Console.writeLine($"\t{cards}, possible scores {scores}, high-score: {highScore}");
}
