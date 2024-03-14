using System;
using LanguageExt.Traits;
using LanguageExt.Traits.Domain;

namespace LanguageExt;

/// <summary>
/// Simple wrapper around String
/// </summary>
/// <param name="Value">Contained value</param>
public interface StringM<SELF> : 
    IdentifierLike<SELF, string>,
    IEquatable<SELF>,
    IComparable<SELF>,
    ISpanParsable<SELF>,
    OrdString<SELF>,
    Monoid<SELF>
    where SELF : 
        StringM<SELF>,
        DomainType<SELF, string>,
        IdentifierLike<SELF, string>,
        IEquatable<SELF>,
        IComparable<SELF>,
        ISpanParsable<SELF>,
        OrdString<SELF>,
        Monoid<SELF>
{
    /// <summary>
    /// Monoid append
    /// </summary>
    SELF Semigroup<SELF>.Combine(SELF rhs) =>
        SELF.From(To() + rhs.To()); 

    /// <summary>
    /// Monoid empty
    /// </summary>
    static SELF Monoid<SELF>.Empty => SELF.From(string.Empty);

    /// <summary>
    /// Length of the string
    /// </summary>
    public int Length =>
        To()?.Length ?? 0;

    /// <summary>
    /// ToString override
    /// </summary>
    public string ToString() =>
        To();

    public int GetHashCode() =>
        Hashable.code((SELF)this);

    public static virtual bool operator ==(SELF lhs, SELF rhs) =>
        SELF.Equals(lhs, rhs);

    public static virtual bool operator !=(SELF lhs, SELF rhs) =>
        !(lhs == rhs);
    
    bool IEquatable<SELF>.Equals(SELF? rhs) =>
        rhs is not null && SELF.Equals((SELF)this, rhs);
    
    int IComparable<SELF>.CompareTo(SELF? rhs) =>
        rhs is null 
            ? 1 
            : SELF.Compare((SELF)this, rhs);

    static SELF IParsable<SELF>.Parse(string s, IFormatProvider? provider) => 
        SELF.From(s);

    static bool IParsable<SELF>.TryParse(string? s, IFormatProvider? provider, out SELF result)
    {
        result = SELF.From(s ?? "");
        return s != null;
    }

    static SELF ISpanParsable<SELF>.Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
    {
        // magic number from System.String
        if (s.Length > 1073741791) throw new ArgumentException(nameof(s));
        return SELF.From(s.ToString());
    }

    static bool ISpanParsable<SELF>.TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out SELF result)
    {
        if (s.Length <= 1073741791) // magic number from System.String
        {
            result = SELF.From(s.ToString());
            return true;
        }
        result = default!;
        return false;
    }
}
