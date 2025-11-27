using LanguageExt.Traits;
using LanguageExt.UnsafeValueAccess;

namespace LanguageExt.Megaparsec;

/// <summary>
/// `Hints` represent a collection of `ErrorItem` values to be included into
/// q `ParseError` (when it's a `TrivialError`) as 'expected' message items
/// when a parser fails without consuming input right after a successful parser
/// that produced the hints. 
/// </summary>
/// <param name="Errors">Errors</param>
/// <typeparam name="T">Token type</typeparam>
public record Hints<T>(Set<ErrorItem<T>> Errors) : Monoid<Hints<T>>
{
    public Hints<T> Combine(Hints<T> rhs) => 
        new (Errors + rhs.Errors);

    public static Hints<T> Empty { get; } = 
        new (Set<ErrorItem<T>>.Empty);
    
    /// <summary>
    /// Replace the hints with the given `ErrorItem` (or delete it if 'Nothing' is given).
    /// This is used in the `label` primitive.
    /// </summary>
    /// <param name="errorItem">Error item</param>
    /// <returns>Refreshed hints</returns>
    public Hints<T> Refresh(Option<ErrorItem<T>> errorItem) =>
        errorItem.IsSome
            ? Errors.IsEmpty
                  ? Empty
                  : new Hints<T>(Set.singleton(errorItem.ValueUnsafe()!))
            : Empty;
}
