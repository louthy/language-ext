// TODO: Decide whether you want to develop this idea or not
// https://mmapped.blog/posts/25-domain-types.html

using System;

namespace LanguageExt.Traits.Domain;

public interface LocusLike<SELF, REPR, SCALAR, DISTANCE>
    where SELF : 
        DomainType<SELF, REPR>,
        LocusLike<SELF, REPR, SCALAR, DISTANCE>,
        IdentifierLike<SELF, REPR>,
        AmountLike<SELF, REPR, SCALAR>,
        VectorSpace<SELF, SCALAR>,
        IEquatable<SELF>,
        IComparable<SELF>
    where DISTANCE:
        AmountLike<SELF, REPR, SCALAR>
{
    /// The origin for the absolute coordinate system.
    public static abstract SELF Origin { get; }

    /// Moves the point away from the origin by the specified distance.
    public static abstract SELF operator+(SELF lhs, DISTANCE rhs);

    /// Returns the distance between two points.
    public static abstract SELF operator-(SELF lhs, DISTANCE rhs);
}
