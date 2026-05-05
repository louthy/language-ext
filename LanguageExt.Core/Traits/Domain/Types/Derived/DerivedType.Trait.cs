namespace LanguageExt.Traits.Domain;

/// <summary>
/// Represents a domain type derived from another domain type.
/// </summary>
/// <typeparam name="SELF">The concrete derived domain type.</typeparam>
/// <typeparam name="BASE">The base domain type being wrapped or specialized.</typeparam>
public interface DerivedType<SELF, BASE> : DomainType<SELF>
    where SELF : DerivedType<SELF, BASE>
    where BASE : DomainType<BASE>
{
    /// <summary>
    /// Returns the base domain value from which this value is derived.
    /// </summary>
    /// <returns>The underlying base domain value.</returns>
    BASE ToBase();
}

/// <summary>
/// Represents a derived domain type that can be converted to a underlying representation.
/// </summary>
/// <typeparam name="SELF">The concrete derived domain type.</typeparam>
/// <typeparam name="BASE">The base domain type being wrapped or specialized.</typeparam>
/// <typeparam name="REPR">The representation shared by both the derived and base domain types.</typeparam>
public interface DerivedType<SELF, BASE, REPR> : 
    DerivedType<SELF, BASE>, 
    DomainType<SELF, REPR>
    where SELF : DerivedType<SELF, BASE, REPR>
    where BASE : DomainType<BASE, REPR>
{
    /// <inheritdoc/>
    REPR DomainType<SELF, REPR>.To() =>
        ToBase().To();
}

/// <summary>
/// Represents a derived domain type that can be constructed from the same representation
/// as its base domain type through a pure factory.
/// </summary>
/// <typeparam name="SELF">The concrete derived domain type.</typeparam>
/// <typeparam name="BASE">The base domain type being wrapped or specialized.</typeparam>
/// <typeparam name="REPR">The representation shared by both the derived and base domain types.</typeparam>
public interface DerivedTypeFactory<SELF, BASE, REPR> :
    DerivedType<SELF, BASE, REPR>,
    DomainFactory<SELF, REPR>
    where SELF : DerivedTypeFactory<SELF, BASE, REPR>
    where BASE : DomainType<BASE, REPR>, DomainTypeFactory<BASE, REPR>
{

    /// <summary>
    /// Creates a derived domain value from an already valid base domain value.
    /// </summary>
    /// <param name="base">The valid base domain value.</param>
    /// <returns>A derived domain value wrapping or specializing the base value.</returns>
    static abstract SELF New(BASE @base);

    /// <inheritdoc/>
    static Fin<SELF> DomainFactory<SELF, SELF, REPR>.From(REPR repr) => 
        BASE.From(repr).Map(SELF.New);

}

/// <summary>
/// Represents a derived domain type that can be constructed from the same representation
/// as its base domain type through an effectful factory.
/// </summary>
/// <typeparam name="SELF">The concrete derived domain type.</typeparam>
/// <typeparam name="BASE">The base domain type being wrapped or specialized.</typeparam>
/// <typeparam name="M">The effect context used during construction.</typeparam>
/// <typeparam name="REPR">The representation shared by both the derived and base domain types.</typeparam>
public interface DerivedTypeFactoryM<SELF, BASE, M, REPR> :
    DerivedType<SELF, BASE, REPR>,
    DomainFactoryM<SELF, M, REPR>
    where SELF : DerivedTypeFactoryM<SELF, BASE, M, REPR>
    where BASE : DomainType<BASE, REPR>, DomainTypeFactoryM<BASE, M, REPR>
    where M : Monad<M>
{
    /// <summary>
    /// Creates a derived domain value from an already valid base domain value.
    /// </summary>
    /// <param name="base">The valid base domain value.</param>
    /// <returns>A derived domain value wrapping or specializing the base value.</returns>
    static abstract SELF New(BASE @base);

    /// <inheritdoc/>
    static FinT<M, SELF> DomainFactoryM<SELF, M, SELF, REPR>.FromM(REPR repr) =>
        BASE.FromM(repr).Map(SELF.New);
}
