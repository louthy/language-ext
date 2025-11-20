// This is a toy sample that demonstrates credit-card validation.  It shouldn't be considered complete,
// it simply demonstrates usage of the Validation applicative/monad for capturing multiple errors when
// doing complex validation.
//
// For more information, see Paul Louth's blog article on Validation:
// https://paullouth.com/higher-kinds-in-c-with-language-ext-part-5-validation/

using LanguageExt;
using LanguageExt.Common;
using LanguageExt.Traits.Domain;

namespace CreditCardValidation.Data;

/// <summary>
/// Base 12 number
/// </summary>
/// <remarks>
/// Handy for representing years and months
/// </remarks>
/// <param name="MostSig">Most significant digits</param>
/// <param name="LeastSig">Least significant digits</param>
public readonly struct Base12(int MostSig, uint LeastSig) :
    DomainType<Base12, (int MostSig, uint LeastSig)>,
    Amount<Base12, Base12>
{
    public static readonly Base12 Zero = new (0, 0);

    public static Base12 From(int value) =>
        new (value / 12, (uint)(Math.Abs(value) % 12));

    public static Fin<Base12> From((int MostSig, uint LeastSig) repr) => 
        repr.LeastSig > 11
            ? Error.New("invalid base-12 number")
            : new Base12(repr.MostSig, repr.LeastSig);

    public (int MostSig, uint LeastSig) To() => 
        (MostSig, LeastSig);
    
    public int ToBase10() =>
        MostSig * 12 + (int)LeastSig;

    public static bool operator ==(Base12 left, Base12 right) => 
        left.Equals(right);

    public static bool operator !=(Base12 left, Base12 right) => 
        !(left == right);

    public static Base12 operator -(Base12 value) =>
        value.To() switch
        {
            var (ms, ls) => new Base12(-ms, ls)
        };

    public static Base12 operator +(Base12 left, Base12 right)
    {
        var (lm, ll) = left.To();
        var (rm, rl) = right.To();
        var most  = lm + rm;
        var least = ll + rl;

        (var carry, least) = least >= 12
                                 ? (1, least - 12)
                                 : (0, least);
        
        return new (most + carry, least);
    }

    public static Base12 operator -(Base12 left, Base12 right)
    {
        var (lm, ll) = left.To();
        var (rm, rl) = right.To();
        var most   = lm - rm;
        var least = (int)ll - (int)rl;

        (var carry, least) = least < 0
                                 ? (-1, least + 12)
                                 : (0, least);
        
        return new (most + carry, (uint)least);
    }

    public static Base12 operator *(Base12 left, Base12 right)
    {
        var (lm, ll) = left.To();
        var (rm, rl) = right.To();

        var leastQuot = (ll * rl) % 12;
        var leastRem  = (ll * rl) / 12;
        var most      = lm * rm + leastRem;
        return new((int)most, leastQuot);
    }

    public static Base12 operator /(Base12 left, Base12 right) 
    {
        var (lm, ll) = left.To();
        var (rm, rl) = right.To();

        var mostQuot = lm / rm;
        var mostRem  = lm % rm;
        var least = (ll / rl + mostRem) % 12;
        return new(mostQuot, (uint)least);
    }

    public bool Equals(Base12 other) => 
        To() == other.To();

    public override bool Equals(object? obj) => 
        obj is Base12 other && Equals(other);

    public override int GetHashCode() => 
        HashCode.Combine(MostSig, LeastSig);

    public int CompareTo(Base12 other) =>
        (To(), other.To()) switch
        {
            var ((lm, ll), (rm, rl)) =>
                lm.CompareTo(rm) switch
                {
                    < 0 => -1,
                    > 0 => 1,
                    _   => ll.CompareTo(rl)
                }
        };

    public static bool operator >(Base12 left, Base12 right) => 
        left.CompareTo(right) > 0;

    public static bool operator >=(Base12 left, Base12 right) => 
        left.CompareTo(right) >= 0;

    public static bool operator <(Base12 left, Base12 right) => 
        left.CompareTo(right) < 0;

    public static bool operator <=(Base12 left, Base12 right) => 
        left.CompareTo(right) <= 0;
}
