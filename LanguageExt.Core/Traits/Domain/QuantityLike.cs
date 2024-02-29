// TODO: Decide whether you want to develop this idea or not
// https://mmapped.blog/posts/25-domain-types.html

using System;

namespace LanguageExt.Traits.Domain;

public interface QuantityLike<SELF, REPR, SCALAR, DimA> 
    where SELF : 
        QuantityLike<SELF, REPR, SCALAR, DimA>,
        DomainType<SELF, REPR>,
        VectorSpace<SELF, SCALAR>,
        IdentifierLike<SELF, REPR>,
        AmountLike<SELF, REPR, SCALAR>,
        IEquatable<SELF>,
        IComparable<SELF>
{
    public static abstract O Multiply<OTHER, O, DimB>(O rhs)
        where OTHER : QuantityLike<OTHER, REPR, SCALAR, DimB>,
                      DomainType<OTHER, REPR>,
                      VectorSpace<OTHER, SCALAR>,
                      IdentifierLike<OTHER, REPR>,
                      AmountLike<OTHER, REPR, SCALAR>,
                      IEquatable<OTHER>,
                      IComparable<OTHER>;
    
    public static abstract O Divide<OTHER, O, DimB>(O rhs)
        where OTHER : QuantityLike<OTHER, REPR, SCALAR, DimB>,
        DomainType<OTHER, REPR>,
        VectorSpace<OTHER, SCALAR>,
        IdentifierLike<OTHER, REPR>,
        AmountLike<OTHER, REPR, SCALAR>,
        IEquatable<OTHER>,
        IComparable<OTHER>;
    
}
