using LanguageExt.Traits;

namespace LanguageExt.Megaparsec;

public record Hints<T>(Set<ErrorItem<T>> Errors) : Monoid<Hints<T>>
{
    public Hints<T> Combine(Hints<T> rhs) => 
        new (Errors + rhs.Errors);

    public static Hints<T> Empty { get; } = new (Set<ErrorItem<T>>.Empty);
}
