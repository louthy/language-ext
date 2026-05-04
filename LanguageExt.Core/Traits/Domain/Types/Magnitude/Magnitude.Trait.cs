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
    VectorSpace<SELF, SCALAR>,
    IComparable<SELF>,
    IComparisonOperators<SELF, SELF, bool>
    where SELF : Magnitude<SELF, SCALAR>;
