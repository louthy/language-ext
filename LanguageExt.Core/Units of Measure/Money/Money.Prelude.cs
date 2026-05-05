namespace LanguageExt;

public static partial class Prelude
{
    public static Fin<Money<C>> money<C>(decimal amount)
        where C : Currency, new() =>
        Money<C>.From(amount);

    public static Money<C> zeroMoney<C>()
        where C : Currency, new() =>
        Money<C>.Zero;

    public static Fin<ExchangeRate<FROM, TO>> exchangeRate<FROM, TO>(decimal rate)
        where FROM : Currency, new()
        where TO : Currency, new() =>
        ExchangeRate<FROM, TO>.From(rate);
}
