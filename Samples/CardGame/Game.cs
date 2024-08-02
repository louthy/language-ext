using LanguageExt;
using static LanguageExt.Prelude;

namespace CardGame;

public static class Game
{
    public static StateT<Deck, OptionT<IO>, Unit> play =>
        from _0 in Console.writeLine("First let's shuffle the cards (press a key)")
        from _1 in Console.readKey
        from _2 in Deck.shuffle
        from _4 in Console.writeLine("\nShuffle done, let's play...")
        from _5 in Console.writeLine("Dealer is waiting (press a key)")
        from _6 in deal
        select unit;
        
    public static StateT<Deck, OptionT<IO>, Unit> deal =>
        from _0   in Console.readKey
        from card in Deck.deal
        from rem  in Deck.cardsRemaining
        from _1   in Console.writeLine($"\nCard dealt: {card} - {rem} cards remaining (press a key)")
        from _2   in deal
        select unit;
}
