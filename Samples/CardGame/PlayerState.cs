using LanguageExt;
using static LanguageExt.Prelude;

namespace CardGame;

/// <summary>
/// Holds a running sequence of dealt cards and the stick-status
/// </summary>
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
