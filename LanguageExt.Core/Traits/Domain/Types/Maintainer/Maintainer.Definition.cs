namespace LanguageExt.Traits.Domain;

/// <summary>
/// Represents a maintained domain set with a finite, known collection of values.
/// </summary>
/// <typeparam name="SELF">Self type.</typeparam>
public interface Maintainer<SELF> :
    DomainType<SELF>,
    DiscreteSpace<SELF>
    where SELF : Maintainer<SELF>
{
    /// <summary>
    /// All maintained values for this domain set.
    /// </summary>
    static abstract Seq<SELF> All { get; }
}

/// <summary>
/// Represents a maintained domain set whose values can be projected to a canonical representation.
/// </summary>
/// <typeparam name="SELF">Self type.</typeparam>
/// <typeparam name="REPR">Canonical representation type.</typeparam>
public interface Maintainer<SELF, REPR> :
    Maintainer<SELF>
    where SELF : Maintainer<SELF, REPR>
{
    /// <summary>
    /// Returns the canonical representation of this maintained value.
    /// </summary>
    REPR To();
}
