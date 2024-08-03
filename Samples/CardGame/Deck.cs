using LanguageExt;
using LanguageExt.UnsafeValueAccess;
using static LanguageExt.Prelude;

namespace CardGame;

/// <summary>
/// Deck of cards
/// </summary>
public record Deck(Seq<Card> Cards)
{
    public static Deck Empty = new ([]);

    /// <summary>
    /// Generate a randomly shuffled deck of cards 
    /// </summary>
    public static GameM<Unit> shuffle =>
        from deck in generate
        from _    in put(deck)
        select unit;

    /// <summary>
    /// Get the deck from the game-state
    /// </summary>
    public static GameM<Deck> deck =>
        GameM.gets(g => g.Deck);

    /// <summary>
    /// Return the number of cards remaining in the deck 
    /// </summary>
    public static GameM<int> cardsRemaining =>
        deck.Map(d => d.Cards.Count);

    /// <summary>
    /// Deal a card from the deck
    /// </summary>
    /// <remarks>When the cards are exhausted the game will cancel</remarks>
    public static GameM<Card> deal =>
        from d in deck
        from x in when(d.Cards.IsEmpty, Display.deckFinished) 
        from c in GameM.lift(d.Cards.Head)
        from _ in put(new Deck(d.Cards.Tail))
        select c;

    /// <summary>
    /// Update the deck 
    /// </summary>
    public static GameM<Unit> put(Deck deck) =>
        GameM.modify(g => g with { Deck = deck });
    
    /// <summary>
    /// Generate a randomly shuffled deck of cards
    /// </summary>
    static IO<Deck> generate =>
        IO.lift(() =>
        {
            var random = new Random((int)DateTime.Now.Ticks);
            var array  = LanguageExt.List.generate(52, ix => new Card(ix)).ToArray();
            random.Shuffle(array);
            return new Deck(array.ToSeqUnsafe());
        });

    public override string ToString() =>
        Cards.ToFullArrayString();
}
