namespace LanguageExt.Traits.Domain;

public interface DomainType<SELF> 
    where SELF : DomainType<SELF>;
    
/// <summary>
/// Fundamental base-trait for implementing domain-types.  This is the basis for `Identifier`, `Locus`, `VectorSpace`,
/// and `Quantity`.  It allows the derived types to be safely instantiated from simpler values, like `int`, `float`, etc.
/// And, to be converted back from the domain-type to the simpler representation.
/// </summary>
/// <typeparam name="SELF">Type implementing this interface</typeparam>
/// <typeparam name="REPR">Underlying representation</typeparam>
public interface DomainType<SELF, REPR> : DomainType<SELF> 
    where SELF : DomainType<SELF, REPR> 
{
    /// <summary>
    /// Creates a domain value from its representation value 
    /// </summary>
    /// <returns>
    /// Either an `Error` or a validly constructed `SELF`.
    /// </returns>
    public static abstract Fin<SELF> From(REPR repr);

    /// <summary>
    /// Creates a domain value from its representation value 
    /// </summary>
    /// <returns>
    /// Either throws an exception or returns a validly constructed `SELF`.
    /// </returns>
    public static virtual SELF FromUnsafe(REPR repr) =>
        SELF.From(repr).ThrowIfFail();                    
    
    /// <summary>
    /// Extracts the representation value from its domain value  
    /// </summary>
    REPR To();
}
