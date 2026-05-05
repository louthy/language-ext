using System.Numerics;

namespace LanguageExt.Traits.Domain;

/// <summary>
/// Represents an affine space of ordered positions whose differences produce measurable distances.
/// </summary>
/// <typeparam name="SELF">
/// The concrete affine position type, enabling static polymorphism for affine operations.
/// </typeparam>
/// <typeparam name="DISTANCE">
/// The magnitude type used to represent displacements or distances between positions.
/// </typeparam>
/// <typeparam name="DISTANCE_SCALAR">
/// The scalar type used by the distance magnitude.
/// </typeparam>
/// <remarks>
/// <para>
/// An <see cref="AffineSpace{SELF, DISTANCE, DISTANCE_SCALAR}"/> models values that represent
/// positions rather than quantities. Positions can be translated by a distance, and subtracting
/// two positions produces the distance between them.
/// </para>
/// <para>
/// Conceptually, affine spaces represent "where" or "when", while magnitudes represent "how much".
/// </para>
/// <para>
/// Typical operations:
/// </para>
/// <list type="bullet">
/// <item>Position + Distance = Position</item>
/// <item>Position - Position = Distance</item>
/// </list>
/// <para>
/// Example usages include:
/// </para>
/// <list type="bullet">
/// <item>Timestamp with Duration</item>
/// <item>Date with Days</item>
/// <item>Coordinate with Distance</item>
/// <item>Position with Offset</item>
/// </list>
/// </remarks>
public interface AffineSpace<SELF, DISTANCE, DISTANCE_SCALAR> : 
    DiscreteSpace<SELF>,
    IAdditionOperators<SELF, DISTANCE, SELF>,
    ISubtractionOperators<SELF, SELF, DISTANCE>
    where SELF : AffineSpace<SELF, DISTANCE, DISTANCE_SCALAR>
    where DISTANCE : VectorSpace<DISTANCE, DISTANCE_SCALAR>;
