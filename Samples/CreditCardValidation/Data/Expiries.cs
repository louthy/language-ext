using LanguageExt;
using LanguageExt.Traits;

namespace CreditCardValidation.Data;

public record Expiries(Expiry From, Expiry To, MonthSpan Step) : Range<Expiries, Expiry, MonthSpan>
{
    public bool InRange(Expiry value) => 
        value >= From && value <= To;

    public bool Overlaps(Range<Expiry> ra)
    {
        var (lfrom, lto) = GetExtents();
        var (rfrom, rto) = ra.GetExtents();
        return lfrom <= rto && rfrom <= lto;
    }

    public (Expiry Min, Expiry Max) GetExtents() => 
        From <= To
            ? (From, To)
            : (To, From);

    public Iterator<Expiry> ForwardIterator() =>
        new Iter(From, From, To, Step);

    public static Range<Expiry> FromMinMax(Expiry from, Expiry to) => 
        new Expiries(from, to, MonthSpan.From(1));

    public static Range<Expiry> FromMinMax(Expiry from, Expiry to, MonthSpan step) => 
        new Expiries(from, to, step);

    public static Range<Expiry> FromCount(Expiry from, long count) => 
        new Expiries(from, from + MonthSpan.From((int)(count - 1)), MonthSpan.From(1));

    public static Range<Expiry> FromCount(Expiry from, long count, MonthSpan step) => 
        new Expiries(from, from + step * (int)(count - 1), step);

    class Iter(Expiry Current, Expiry From, Expiry To, MonthSpan Step) : Iterator<Expiry>
    {
        public override (Head<Expiry> Head, Iterator<Expiry> Tail) Next()
        {
            var head = Current;
            var next = head + Step;
            return next < From || next > To
                       ? (new Exist<Expiry>(head), Iterator.empty<Expiry>())
                       : (new Exist<Expiry>(head), new Iter(next, From, To, Step));
        }
    }
}
