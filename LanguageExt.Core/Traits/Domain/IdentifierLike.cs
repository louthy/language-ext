// TODO: Decide whether you want to develop this idea or not
// https://mmapped.blog/posts/25-domain-types.html

using System;

namespace LanguageExt.Traits.Domain;

public interface IdentifierLike<SELF, REPR> : DomainType<SELF, REPR>
    where SELF :
    DomainType<SELF, REPR>,
    IdentifierLike<SELF, REPR>,
    IEquatable<SELF>;
