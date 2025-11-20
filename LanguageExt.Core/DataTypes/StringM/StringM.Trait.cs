using System;
using LanguageExt.Traits;
using LanguageExt.Traits.Domain;

namespace LanguageExt;

/// <summary>
/// Simple wrapper around String
/// </summary>
/// <param name="Value">Contained value</param>
public interface StringM<SELF> :
    DomainType<SELF, string>,
    Identifier<SELF>,
    IComparable<SELF>,
    ISpanParsable<SELF>,
    OrdString<SELF>,
    Monoid<SELF>
    where SELF : 
        StringM<SELF>,
        Identifier<SELF>,
        IComparable<SELF>,
        ISpanParsable<SELF>,
        OrdString<SELF>,
        Monoid<SELF>
{
    /// <summary>
    /// Monoid append
    /// </summary>
    SELF Semigroup<SELF>.Combine(SELF rhs) =>
        SELF.FromUnsafe(To() + rhs.To()); 

    /// <summary>
    /// Monoid empty
    /// </summary>
    static SELF Monoid<SELF>.Empty => SELF.FromUnsafe(string.Empty);

    /// <summary>
    /// Length of the string
    /// </summary>
    public int Length =>
        To().Length;

    /// <summary>
    /// ToString override
    /// </summary>
    public string ToString() =>
        To();

    public int GetHashCode() =>
        Hashable.code((SELF)this);
    
    bool IEquatable<SELF>.Equals(SELF? rhs) =>
        rhs is not null && SELF.Equals((SELF)this, rhs);
    
    int IComparable<SELF>.CompareTo(SELF? rhs) =>
        rhs is null 
            ? 1 
            : SELF.Compare((SELF)this, rhs);

    static SELF IParsable<SELF>.Parse(string s, IFormatProvider? provider) => 
        SELF.FromUnsafe(s);

    static bool IParsable<SELF>.TryParse(string? s, IFormatProvider? provider, out SELF result)
    {
        result = SELF.FromUnsafe(s ?? "");
        return s != null;
    }

    static SELF ISpanParsable<SELF>.Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
    {
        // magic number from System.String
        if (s.Length > 1073741791) throw new ArgumentException(nameof(s));
        return SELF.FromUnsafe(s.ToString());
    }

    static bool ISpanParsable<SELF>.TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out SELF result)
    {
        if (s.Length <= 1073741791) // magic number from System.String
        {
            result = SELF.FromUnsafe(s.ToString());
            return true;
        }
        result = default!;
        return false;
    }
}
