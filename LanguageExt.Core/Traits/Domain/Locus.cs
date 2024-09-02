// TODO: Decide whether you want to develop this idea or not
// https://mmapped.blog/posts/25-domain-types.html

using System;
using System.Numerics;

namespace LanguageExt.Traits.Domain;

/// <summary>
/// Working with space-like structures, such as time and space, poses an interesting challenge. Spaces have two types of
/// values: absolute positions and relative distances.
///
/// Positions refer to points in space, such as timestamps or geographical coordinates. Distances represent a difference
/// between two such points.
///
/// Some natural languages acknowledge the distinction and offer different words for these concepts, such as o’clock vs.
/// hours in English or Uhr vs. Stunden in German.
///
/// While distances behave the same way as `Amount`, positions are trickier. We can compare, order, and subtract them to
/// compute the distance between two points. For example, subtracting 5 am on Friday from 3 am on Saturday gives us
/// twenty-two hours. Adding or multiplying these dates makes no sense, however. This semantic demands a new class of
/// types, loci (plural of locus).
///
/// We can view each position as a distance from a fixed origin point. Changing the origin or the distance type calls for
/// a new locus type.
/// </summary>
/// <typeparam name="SELF">Type implementing this interface</typeparam>
/// <typeparam name="SCALAR">Scalar units</typeparam>
/// <typeparam name="DISTANCE">Additive units</typeparam>
public interface Locus<SELF, SCALAR, DISTANCE> :
    Identifier<SELF>,
    IComparable<SELF>,
    IUnaryNegationOperators<SELF, SELF>,
    IComparisonOperators<SELF, SELF, bool>,
    IAdditiveIdentity<SELF, SELF>,
    IAdditionOperators<SELF, DISTANCE, SELF>,
    ISubtractionOperators<SELF, DISTANCE, SELF>
    where SELF : Locus<SELF, SCALAR, DISTANCE>
    where DISTANCE: Amount<DISTANCE, SCALAR>
{
    /// The origin for the absolute coordinate system.
    public static abstract SELF Origin { get; }

    /// <summary>
    /// Default AdditiveIdentity
    /// </summary>
    public static virtual SELF AdditiveIdentity => 
        SELF.Origin;
}
