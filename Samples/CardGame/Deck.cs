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

    public static StateT<Deck, OptionT<IO>, Unit> shuffle =>
        from deck in generate
        from _    in StateT.put<OptionT<IO>, Deck>(deck)
        select unit;

    public static StateT<Deck, OptionT<IO>, Deck> deck =>
        StateT.get<OptionT<IO>, Deck>();

    public static StateT<Deck, OptionT<IO>, int> cardsRemaining =>
        deck.Map(d => d.Cards.Count);

    public static StateT<Deck, OptionT<IO>, Card> deal =>
        from d in deck
        from c in d.Cards switch
                  {
                      [] => StateT<Deck>.lift(OptionT<IO>.None<Unit>()),
                      _  => StateT.put<OptionT<IO>, Deck>(d.Take())
                  }
        from r in OptionT<IO>.lift(d.Cards.Head)
        select r;
}
