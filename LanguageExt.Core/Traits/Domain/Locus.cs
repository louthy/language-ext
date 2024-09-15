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
/// hours.
///
/// While distances behave the same way as `Amount`, positions are trickier. We can compare, order, and subtract them to
/// compute the distance between two points. For example, subtracting 5 am on Friday from 3 am on Saturday gives us
/// twenty-two hours. Adding or multiplying these dates makes no sense, however. This semantic demands a new class of
/// types, loci (plural of locus).
///
/// We can view each position as a distance from a fixed origin point. Changing the origin or the distance type calls
/// for a new locus type.
/// </summary>
/// <typeparam name="SELF">Type implementing this interface</typeparam>
/// <typeparam name="DISTANCE">Additive units</typeparam>
/// <typeparam name="DISTANCE_SCALAR">Distance scalar units</typeparam>
public interface Locus<SELF, DISTANCE, DISTANCE_SCALAR> :
    Identifier<SELF>,
    IComparable<SELF>,
    IComparisonOperators<SELF, SELF, bool>,
    IUnaryNegationOperators<SELF, SELF>,
    IAdditiveIdentity<SELF, SELF>,
    IAdditionOperators<SELF, DISTANCE, SELF>,
    ISubtractionOperators<SELF, SELF, DISTANCE>
    where SELF : Locus<SELF, DISTANCE, DISTANCE_SCALAR>
    where DISTANCE : Amount<DISTANCE, DISTANCE_SCALAR>;
