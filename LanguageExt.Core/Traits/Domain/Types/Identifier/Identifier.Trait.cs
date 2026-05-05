namespace LanguageExt.Traits.Domain;

/// <summary>
/// Represents a domain identifier with value-based equality semantics.
/// </summary>
/// <typeparam name="SELF">
/// The concrete identifier type.
/// </typeparam>
public interface Identifier<SELF> : 
    DomainType<SELF>,
    DiscreteSpace<SELF>
    where SELF : Identifier<SELF>;

/// <summary>
/// Represents a domain identifier backed by an underlying
/// representation type.
/// </summary>
/// <typeparam name="SELF">
/// The concrete identifier type.
/// </typeparam>
/// <typeparam name="REPR">
/// The underlying representation type used by the identifier.
/// </typeparam>
public interface IdentifierType<SELF, REPR> : 
    Identifier<SELF>, 
    DomainType<SELF, REPR>
    where SELF : IdentifierType<SELF, REPR>;

/// <summary>
/// Represents a domain identifier backed by an underlying
/// representation type and can be created via factory.
/// </summary>
/// <typeparam name="SELF">
/// The concrete identifier type.
/// </typeparam>
/// <typeparam name="REPR">
/// The underlying representation type used by the identifier.
/// </typeparam>
public interface IdentifierTypeFactory<SELF, REPR> :
    IdentifierType<SELF, REPR>,
    DomainTypeFactory<SELF, REPR>
    where SELF : IdentifierTypeFactory<SELF, REPR>;

/// <summary>
/// Represents a domain identifier backed by an underlying
/// representation type and can be created via effectful factory.
/// </summary>
/// <typeparam name="SELF">
/// The concrete identifier type.
/// </typeparam>
/// <typeparam name="M">
/// Monadic context that the factory works with-in
/// </typeparam>
/// <typeparam name="REPR">
/// The underlying representation type used by the identifier.
/// </typeparam>
public interface IdentifierTypeFactoryM<SELF, M, REPR> :
    IdentifierType<SELF, REPR>,
    DomainTypeFactoryM<SELF, M, REPR>
    where SELF : IdentifierTypeFactoryM<SELF, M, REPR>
    where M : Monad<M>;
