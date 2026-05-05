using System;
using System.Numerics;

namespace LanguageExt.Traits.Domain;

/// <summary>
/// Represents an ordered vector space whose values describe measurable magnitudes.
/// </summary>
/// <typeparam name="SELF">
/// The concrete magnitude type, enabling static polymorphism for algebraic operations.
/// </typeparam>
/// <typeparam name="SCALAR">
/// The scalar type used for scaling operations (e.g., <see cref="decimal"/>, <see cref="double"/>).
/// </typeparam>
/// <remarks>
/// <para>
/// A <see cref="Magnitude{SELF, SCALAR}"/> extends <see cref="VectorSpace{SELF, SCALAR}"/> by adding
/// total ordering capabilities. This allows values to be compared and sorted, in addition to being
/// combined and scaled algebraically.
/// </para>
/// <para>
/// Conceptually, magnitudes represent "how much" of something, such as money, distance, duration,
/// or weight.
/// </para>
/// <para>
/// Typical properties:
/// </para>
/// <list type="bullet">
/// <item>Supports addition, negation, and scalar multiplication.</item>
/// <item>Provides an additive identity (zero).</item>
/// <item>Supports comparison operations (e.g., less than, greater than).</item>
/// </list>
///
/// <para>
/// Example usages include:
/// </para>
/// <list type="bullet">
/// <item>Money</item>
/// <item>Distance</item>
/// <item>Duration</item>
/// <item>Temperature differences</item>
/// </list>
/// </remarks>
public interface Magnitude<SELF, SCALAR> :
    DomainType<SELF>,
    VectorSpace<SELF, SCALAR>,
    IComparable<SELF>,
    IComparisonOperators<SELF, SELF, bool>
    where SELF : Magnitude<SELF, SCALAR> 
    where SCALAR : notnull;

/// <summary>
/// Represents an ordered vector space backed by a representation
/// whose values describe measurable magnitudes.
/// </summary>
/// <typeparam name="SELF">
/// The concrete magnitude type, enabling static polymorphism for algebraic operations.
/// </typeparam>
/// <typeparam name="SCALAR">
/// The scalar type used for scaling operations (e.g.,
/// <see cref="decimal"/>, <see cref="double"/>).
/// </typeparam>
/// <typeparam name="REPR">
/// The underlying representation type used by the magnitude.
/// </typeparam>
/// <remarks>
/// <para>
/// A <see cref="Magnitude{SELF, SCALAR}"/> extends
/// <see cref="VectorSpace{SELF, SCALAR}"/> by adding total ordering
/// capabilities. This allows values to be compared and sorted,
/// in addition to being combined and scaled algebraically.
/// </para>
/// <para>
/// Conceptually, magnitudes represent "how much" of something,
/// such as money, distance, duration, or weight.
/// </para>
/// <para>
/// Typical properties:
/// </para>
/// <list type="bullet">
/// <item>Supports addition, negation, and scalar multiplication.</item>
/// <item>Provides an additive identity (zero).</item>
/// <item>Supports comparison operations (e.g., less than, greater than).</item>
/// </list>
///
/// <para>
/// Example usages include:
/// </para>
/// <list type="bullet">
/// <item>Money</item>
/// <item>Distance</item>
/// <item>Duration</item>
/// <item>Temperature differences</item>
/// </list>
/// </remarks>
public interface MagnitudeType<SELF, SCALAR, REPR> :
    DomainType<SELF, REPR>,
    Magnitude<SELF, SCALAR> 
    where SELF : MagnitudeType<SELF, SCALAR, REPR> 
    where SCALAR : notnull;

/// <inheritdoc/>
public interface MagnitudeType<SELF, REPR> : 
    MagnitudeType<SELF, REPR, REPR>
    where SELF : MagnitudeType<SELF, REPR>
    where REPR : notnull;

/// <summary>
/// Represents an ordered vector space whose values describe
/// measurable magnitudes and can create its own values via factory.
/// </summary>
/// <typeparam name="SELF">
/// The concrete magnitude type, enabling static polymorphism for
/// algebraic operations and self creation via factory.
/// </typeparam>
/// <typeparam name="SCALAR">
/// The scalar type used for scaling operations (e.g.,
/// <see cref="decimal"/>, <see cref="double"/>).
/// </typeparam>
/// <typeparam name="REPR">
/// The underlying representation type used by the magnitude.
/// </typeparam>
/// <remarks>
/// <para>
/// A <see cref="Magnitude{SELF, SCALAR}"/> extends
/// <see cref="VectorSpace{SELF, SCALAR}"/> by adding total ordering
/// capabilities. This allows values to be compared and sorted,
/// in addition to being combined and scaled algebraically.
/// </para>
/// <para>
/// Conceptually, magnitudes represent "how much" of something,
/// such as money, distance, duration, or weight.
/// </para>
/// <para>
/// Typical properties:
/// </para>
/// <list type="bullet">
/// <item>Supports addition, negation, and scalar multiplication.</item>
/// <item>Provides an additive identity (zero).</item>
/// <item>Supports comparison operations (e.g., less than, greater than).</item>
/// </list>
///
/// <para>
/// Example usages include:
/// </para>
/// <list type="bullet">
/// <item>Money</item>
/// <item>Distance</item>
/// <item>Duration</item>
/// <item>Temperature differences</item>
/// </list>
/// </remarks>
public interface MagnitudeTypeFactory<SELF, SCALAR, REPR> :
    MagnitudeType<SELF, SCALAR, REPR>,
    DomainTypeFactory<SELF, REPR> 
    where SELF : MagnitudeTypeFactory<SELF, SCALAR, REPR>
    where SCALAR : notnull;

/// <inheritdoc />
public interface MagnitudeTypeFactory<SELF, REPR> :
    MagnitudeTypeFactory<SELF, REPR, REPR>
    where SELF : MagnitudeTypeFactory<SELF, REPR>
    where REPR : notnull;

/// <summary>
/// Represents an ordered vector space whose values describe
/// measurable magnitudes and can create its own values via effectful
/// factory.
/// </summary>
/// <typeparam name="SELF">
/// The concrete magnitude type, enabling static polymorphism for
/// algebraic operations and self creation via effectful factory.
/// </typeparam>
/// <typeparam name="SCALAR">
/// The scalar type used for scaling operations (e.g.,
/// <see cref="decimal"/>, <see cref="double"/>).
/// </typeparam>
/// <typeparam name="M">
/// Monadic context that the factory works with-in
/// </typeparam>
/// <typeparam name="REPR">
/// The underlying representation type used by the magnitude.
/// </typeparam>
/// <remarks>
/// <para>
/// A <see cref="Magnitude{SELF, SCALAR}"/> extends
/// <see cref="VectorSpace{SELF, SCALAR}"/> by adding total ordering
/// capabilities. This allows values to be compared and sorted,
/// in addition to being combined and scaled algebraically.
/// </para>
/// <para>
/// Conceptually, magnitudes represent "how much" of something,
/// such as money, distance, duration, or weight.
/// </para>
/// <para>
/// Typical properties:
/// </para>
/// <list type="bullet">
/// <item>Supports addition, negation, and scalar multiplication.</item>
/// <item>Provides an additive identity (zero).</item>
/// <item>Supports comparison operations (e.g., less than, greater than).</item>
/// </list>
///
/// <para>
/// Example usages include:
/// </para>
/// <list type="bullet">
/// <item>Money</item>
/// <item>Distance</item>
/// <item>Duration</item>
/// <item>Temperature differences</item>
/// </list>
/// </remarks>
public interface MagnitudeTypeFactoryM<SELF, SCALAR, M, REPR> :
    MagnitudeType<SELF, SCALAR, REPR>,
    DomainTypeFactoryM<SELF, M, REPR>
    where SELF : MagnitudeTypeFactoryM<SELF, SCALAR, M, REPR>
    where SCALAR : notnull
    where M : Monad<M>;

/// <inheritdoc />
public interface MagnitudeTypeFactoryM<SELF, M, REPR> :
    MagnitudeTypeFactoryM<SELF, REPR, M, REPR>
    where SELF : MagnitudeTypeFactoryM<SELF, M, REPR>
    where M : Monad<M>
    where REPR : notnull;
