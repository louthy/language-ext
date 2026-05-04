namespace LanguageExt.Traits.Domain;

/// <summary>
/// Represents a domain type that refines another valid domain type with additional constraints.
/// </summary>
/// <typeparam name="SELF">The concrete refined domain type.</typeparam>
/// <typeparam name="BASE">The base domain type being refined.</typeparam>
public interface RefinedType<SELF, BASE> : DomainType<SELF>
    where SELF : RefinedType<SELF, BASE>
    where BASE : DomainType<BASE>
{
    /// <summary>
    /// Returns the base domain value from which this refined value was created.
    /// </summary>
    /// <returns>The underlying base domain value.</returns>
    BASE ToBase();
}

/// <summary>
/// Represents a refined domain type that shares the same representation as its base domain type.
/// </summary>
/// <typeparam name="SELF">The concrete refined domain type.</typeparam>
/// <typeparam name="BASE">The base domain type being refined.</typeparam>
/// <typeparam name="REPR">The representation shared by both the refined and base domain types.</typeparam>
public interface RefinedType<SELF, BASE, REPR> : 
    DomainType<SELF, REPR>, 
    RefinedType<SELF, BASE>
    where SELF : RefinedType<SELF, BASE, REPR>
    where BASE : DomainType<BASE, REPR>
{
    /// <inheritdoc/>
    REPR DomainType<SELF, REPR>.To() =>
        ToBase().To();
}
/// <summary>
/// Represents a refined domain type that can be constructed from the same representation
/// as its base domain type through a pure factory.
/// </summary>
/// <typeparam name="SELF">The concrete refined domain type.</typeparam>
/// <typeparam name="BASE">The base domain type being refined.</typeparam>
/// <typeparam name="REPR">The representation shared by both the refined and base domain types.</typeparam>
public interface RefinedTypeFactory<SELF, BASE, REPR> :
    RefinedType<SELF, BASE, REPR>,
    DomainFactory<SELF, REPR>
    where SELF : RefinedTypeFactory<SELF, BASE, REPR>
    where BASE : DomainType<BASE, REPR>, DomainTypeFactory<BASE, REPR>
{
    /// <summary>
    /// Attempts to refine an already valid base domain value.
    /// </summary>
    /// <param name="repr">The valid base domain value to refine.</param>
    /// <returns>
    /// A successful refined value when the additional constraints are satisfied;
    /// otherwise, a failed <see cref="Fin{A}"/>.
    /// </returns>
    static abstract Fin<SELF> From(BASE repr);

    /// <inheritdoc/>
    static Fin<SELF> DomainFactory<SELF, SELF, REPR>.From(REPR repr) => 
        BASE.From(repr).Bind(SELF.From);
}

/// <summary>
/// Represents a refined domain type that can be constructed from the same representation
/// as its base domain type through an effectful factory.
/// </summary>
/// <typeparam name="SELF">The concrete refined domain type.</typeparam>
/// <typeparam name="BASE">The base domain type being refined.</typeparam>
/// <typeparam name="M">The effect context used during construction.</typeparam>
/// <typeparam name="REPR">The representation shared by both the refined and base domain types.</typeparam>
public interface RefinedTypeFactoryM<SELF, BASE, M, REPR> :
    RefinedType<SELF, BASE, REPR>,
    DomainFactoryM<SELF, M, REPR>
    where SELF : RefinedTypeFactoryM<SELF, BASE, M, REPR>
    where BASE : DomainType<BASE, REPR>, DomainTypeFactoryM<BASE, M, REPR>
    where M : Monad<M>
{
    /// <summary>
    /// Attempts to refine an already valid base domain value inside the effect context
    /// <typeparamref name="M"/>.
    /// </summary>
    /// <param name="repr">The valid base domain value to refine.</param>
    /// <returns>
    /// An effectful validation producing the refined value when successful;
    /// otherwise, a failed <see cref="FinT{M, A}"/>.
    /// </returns>
    static abstract FinT<M, SELF> FromM(BASE repr);

    /// <inheritdoc/>
    static FinT<M, SELF> DomainFactoryM<SELF, M, SELF, REPR>.FromM(REPR repr) =>
        BASE.FromM(repr).Bind(SELF.FromM);
}
