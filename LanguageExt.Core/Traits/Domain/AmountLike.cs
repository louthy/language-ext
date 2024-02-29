// TODO: Decide whether you want to develop this idea or not
// https://mmapped.blog/posts/25-domain-types.html

using System;

namespace LanguageExt.Traits.Domain;

public interface AmountLike<SELF, REPR, SCALAR> : 
    IdentifierLike<SELF, REPR>, 
    VectorSpace<SELF, SCALAR>
    where SELF : 
        AmountLike<SELF, REPR, SCALAR>,
        DomainType<SELF, REPR>,
        VectorSpace<SELF, SCALAR>, 
        IdentifierLike<SELF, REPR>, 
        IEquatable<SELF>,
        IComparable<SELF>;
