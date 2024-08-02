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
    
    public Deck Take() =>
        new (Cards.Tail);

    public override string ToString() =>
        Cards.ToFullArrayString();

    static IO<Deck> generate =>
        IO.lift(() =>
                {
                    var random = new Random((int)DateTime.Now.Ticks);
                    var array  = LanguageExt.List.generate(52, ix => new Card(ix)).ToArray();
                    random.Shuffle(array);
                    return new Deck(array.ToSeqUnsafe());
                });

    public static StateT<Game, OptionT<IO>, Unit> shuffle =>
        from deck in generate
        from _    in putDeck(deck)
        select unit;

    public static StateT<Game, OptionT<IO>, Deck> deck =>
        StateT.gets<OptionT<IO>, Game, Deck>(g => g.Deck);

    public static StateT<Game, OptionT<IO>, Unit> putDeck(Deck deck) =>
        StateT.modify<OptionT<IO>, Game>(g => g with { Deck = deck });

    public static StateT<Game, OptionT<IO>, int> cardsRemaining =>
        deck.Map(d => d.Cards.Count);

    public static StateT<Game, OptionT<IO>, Card> deal =>
        from d in deck
        from c in d.Cards switch
                  {
                      [] => from _ in Display.deckFinished
                            from r in Game.noneM
                            select unit,
                      _  => putDeck(d.Take())
                  }
        from r in OptionT<IO>.lift(d.Cards.Head)
        select r;
}
