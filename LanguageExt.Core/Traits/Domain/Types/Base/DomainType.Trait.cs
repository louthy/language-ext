namespace LanguageExt.Traits.Domain;

/// <summary>
/// Represents a marker interface for a domain type.
/// </summary>
/// <typeparam name="SELF">
/// The concrete domain type, enabling static polymorphism and self-referential constraints.
/// </typeparam>
/// <remarks>
/// <para>
/// <see cref="DomainType{SELF}"/> serves as the root abstraction for all domain types,
/// providing a common type-level identity without prescribing representation or construction.
/// </para>
///
/// <para>
/// This allows higher-level traits (such as <see cref="DomainType{SELF, REPR}"/>,
/// refined types, or algebraic structures) to compose over a shared concept of "domain type".
/// </para>
/// </remarks>
public interface DomainType<SELF> 
    where SELF : DomainType<SELF>;

/// <summary>
/// Represents a domain type that can be converted to an underlying representation.
/// </summary>
/// <typeparam name="SELF">
/// The concrete domain type, enabling static polymorphism.
/// </typeparam>
/// <typeparam name="REPR">
/// The underlying representation type (e.g., <see cref="string"/>, <see cref="int"/>, <see cref="decimal"/>).
/// </typeparam>
/// <remarks>
/// <para>
/// <see cref="DomainType{SELF, REPR}"/> defines the representational aspect of a domain type,
/// allowing it to be projected into a base value.
/// </para>
///
/// <para>
/// This interface intentionally does not define how instances are created. Construction is
/// delegated to separate abstractions (e.g., <see cref="DomainFactory{SELF,IN}"/> and
/// <see cref="DomainFactory{SELF,TYPE,IN}"/>) to allow different creation strategies
/// such as pure validation, effectful generation, or runtime-dependent construction.
/// </para>
///
/// <para>
/// Conceptually:
/// </para>
/// <list type="bullet">
/// <item>This defines "what it is" (representation).</item>
/// <item>Factories define "how it is created".</item>
/// </list>
/// </remarks>
public interface DomainType<SELF, REPR> : DomainType<SELF> 
    where SELF : DomainType<SELF, REPR>
{
    /// <summary>
    /// Converts this domain value to its underlying representation.
    /// </summary>
    /// <returns>The underlying representation value.</returns>
    REPR To();
}

/// <summary>
/// Represents a domain type that can expose its underlying representation
/// and be constructed from that same representation through a pure factory.
/// </summary>
/// <typeparam name="SELF">The concrete domain type implementing this trait.</typeparam>
/// <typeparam name="REPR">The underlying representation used by the domain type.</typeparam>
public interface DomainTypeFactory<SELF, REPR> :
    DomainType<SELF, REPR>,
    DomainFactory<SELF, REPR>
    where SELF : DomainTypeFactory<SELF, REPR>;

/// <summary>
/// Represents a domain type that can expose its underlying representation
/// and be constructed from that same representation through an effectful factory.
/// </summary>
/// <typeparam name="SELF">The concrete domain type implementing this trait.</typeparam>
/// <typeparam name="M">The effect context used during construction.</typeparam>
/// <typeparam name="REPR">The underlying representation used by the domain type.</typeparam>
public interface DomainTypeFactoryM<SELF, M, REPR> :
    DomainType<SELF, REPR>,
    DomainFactoryM<SELF, M, REPR>
    where SELF : DomainTypeFactoryM<SELF, M, REPR>
    where M : Monad<M>;
