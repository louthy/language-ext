using System;
using System.Collections.Generic;
using System.Text;
using LanguageExt.Common;

namespace LanguageExt.Traits.Domain;

/// <summary>
/// Defines an effectful factory responsible for constructing domain values from an input value.
/// </summary>
/// <typeparam name="SELF">
/// The concrete factory type, enabling static polymorphism.
/// </typeparam>
/// <typeparam name="M">
/// The effect or monadic context used during construction.
/// </typeparam>
/// <typeparam name="TYPE">
/// The domain type produced by this factory.
/// </typeparam>
/// <typeparam name="IN">
/// The input type used to construct the domain value.
/// </typeparam>
/// <remarks>
/// <para>
/// <see cref="DomainFactoryM{SELF, M, TYPE, IN}"/> separates effectful construction from
/// domain representation.
/// </para>
/// <para>
/// This is useful when creating a domain value requires contextual
/// or runtime-dependent data, such as IO, configuration, time, randomness, repositories,
/// external services, or an environment.
/// </para>
/// 
/// <para>
/// Conceptually:
/// </para>
/// <list type="bullet">
/// <item>DomainType defines "what it is" (representation).</item>
/// <item>DomainFactory defines pure construction.</item>
/// <item>DomainFactoryM defines effectful construction.</item>
/// </list>
/// Factories return <see cref="FinT{M, A}"/> to model construction that can both run inside
/// an effect and fail with an explicit <see cref="Error"/>.
/// </remarks>
public interface DomainFactoryM<SELF, M, TYPE, IN>   
    where SELF : DomainFactoryM<SELF, M, TYPE, IN>
    where M : Monad<M>
    where TYPE : DomainType<TYPE>
{
    /// <summary>
    /// Attempts to create a domain value from the specified input inside an effectful context.
    /// </summary>
    /// <param name="repr">The input value used to construct the domain type.</param>
    /// <returns>
    /// A <see cref="FinT{M, A}"/> that evaluates to either a successfully constructed domain value
    /// or a failure containing an <see cref="Error"/>.
    /// </returns>
    public static abstract FinT<M, TYPE> FromM(IN repr);

    /// <summary>
    /// Creates a domain value from the specified input, throwing if construction fails.
    /// </summary>
    /// <param name="repr">The input value used to construct the domain type.</param>
    /// <returns>The constructed domain value.</returns>
    /// <exception cref="Exception">
    /// Thrown when the domain value cannot be constructed.
    /// </exception>
    public static virtual K<M, TYPE> FromUnsafeM(IN repr) =>
        SELF.FromM(repr)
            .Run()
            .Map(f => f.ThrowIfFail());

}


/// <inheritdoc/> 
public interface DomainFactoryM<SELF, M, IN> : DomainFactoryM<SELF, M, SELF, IN>
    where SELF : DomainFactoryM<SELF, M, IN>, DomainType<SELF>
    where M : Monad<M>;
