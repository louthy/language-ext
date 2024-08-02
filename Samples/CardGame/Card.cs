using LanguageExt;

namespace CardGame;

/// <summary>
/// Simple card type. Contains an index that can be converted to a textual
/// representation of the card.
/// </summary>
public record Card(int Index)
{
    static string CardName(int index) =>
        index switch
        {
            0      => "Ace",
            10     => "Jack",
            11     => "Queen",
            12     => "King",
            var ix => $"{ix + 1}"
        };

    public override string ToString() =>
        Index switch
        {
            < 13 => $"{CardName(Index)} of Hearts",
            < 26 => $"{CardName(Index - 13)} of Clubs",
            < 39 => $"{CardName(Index - 26)} of Spades",
            _    => $"{CardName(Index - 39)} of Diamonds"
        };
}
