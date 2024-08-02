using LanguageExt;

namespace CardGame;

/// <summary>
/// Simple card type. Contains an index that can be converted to a textual
/// representation of the card.
/// </summary>
public record Card(int Index)
{
    public string Name =>
        (Index % 13) switch
        {
            0      => "Ace",
            10     => "Jack",
            11     => "Queen",
            12     => "King",
            var ix => $"{ix + 1}"
        };
    
    public string Suit =>
        Index switch
        {
            < 13 => "Hearts",
            < 26 => "Clubs",
            < 39 => "Spades",
            < 52 => "Diamonds",
            _    => throw new NotSupportedException()
        };

    public override string ToString() =>
        $"{Name} of {Suit}";
    
    public Seq<int> FaceValues =>
        (Index % 13) switch
        {
            0     => [1, 11],    // Ace
            10    => [10],       // Jack
            11    => [10],       // Queen
            12    => [10],       // King   
            var x => [x + 1]
        };
}
