// TODO: Decide whether you want to develop this idea or not
// https://mmapped.blog/posts/25-domain-types.html

namespace LanguageExt.Traits.Domain;

public interface VectorSpace<SELF, in SCALAR> where SELF : VectorSpace<SELF, SCALAR>
{
    /// Returns the additive inverse of the value.
    public static abstract SELF operator -(SELF self);

    /// Adds two vectors.
    public static abstract SELF operator +(SELF lhs, SELF rhs);
    
    /// Subtracts the other vector from self.
    public static abstract SELF operator -(SELF lhs, SELF rhs);
    
    /// Multiplies the vector by a scalar.
    public static abstract SELF operator *(SELF lhs, SCALAR rhs);
    
    /// Divides the vector by a scalar.
    public static abstract SELF operator /(SELF lhs, SCALAR rhs);
}
