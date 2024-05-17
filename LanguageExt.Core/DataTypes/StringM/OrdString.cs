// TODO: Work out where these types should live

using System;
using LanguageExt.Traits;
using LanguageExt.Traits.Domain;

namespace LanguageExt;

public interface HashableString<STRING> : Hashable<STRING>
    where STRING : 
        HashableString<STRING>, 
        DomainType<STRING, string>
{
    public static abstract StringComparison Comparison { get; }
    
    static int Hashable<STRING>.GetHashCode(STRING x) =>
        x.To().GetHashCode(STRING.Comparison);
}

public interface EqString<STRING> : HashableString<STRING>, Eq<STRING>
    where STRING : EqString<STRING>, 
    DomainType<STRING, string>
{
    static bool Eq<STRING>.Equals(STRING lhs, STRING rhs) =>
        string.Equals(lhs.To(), rhs.To(), STRING.Comparison);
}

public interface OrdString<STRING> : EqString<STRING>, Ord<STRING>
    where STRING : 
        OrdString<STRING>,
        DomainType<STRING, string>
{
    static int Ord<STRING>.Compare(STRING lhs, STRING rhs) =>
        string.Compare(lhs.To(), rhs.To(), STRING.Comparison);
}

public interface OrdinalIgnoreCase<STRING> : OrdString<STRING>
    where STRING : 
        OrdinalIgnoreCase<STRING>,
        DomainType<STRING, string>
{
    static StringComparison HashableString<STRING>.Comparison => 
        StringComparison.OrdinalIgnoreCase;
}

public interface Ordinal<STRING> : OrdString<STRING>
    where STRING : 
        Ordinal<STRING>,
        DomainType<STRING, string>
{
    static StringComparison HashableString<STRING>.Comparison => 
        StringComparison.Ordinal;
}

public interface CultureIgnoreCase<STRING> : OrdString<STRING>
    where STRING : 
        CultureIgnoreCase<STRING>,
        DomainType<STRING, string>
{
    static StringComparison HashableString<STRING>.Comparison => 
        StringComparison.CurrentCultureIgnoreCase;
}

public interface Culture<STRING> : OrdString<STRING>
    where STRING : 
        Culture<STRING>,
        DomainType<STRING, string>
{
    static StringComparison HashableString<STRING>.Comparison => 
        StringComparison.CurrentCulture;
}

public interface InvariantIgnoreCase<STRING> : OrdString<STRING>
    where STRING : 
        InvariantIgnoreCase<STRING>,
        DomainType<STRING, string>
{
    static StringComparison HashableString<STRING>.Comparison => 
        StringComparison.InvariantCultureIgnoreCase;
}

public interface InvariantCulture<STRING> : OrdString<STRING>
    where STRING : 
        InvariantCulture<STRING>,
        DomainType<STRING, string>
{
    static StringComparison HashableString<STRING>.Comparison => 
        StringComparison.InvariantCulture;
}
