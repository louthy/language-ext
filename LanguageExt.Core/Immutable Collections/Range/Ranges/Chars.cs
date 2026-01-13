using LanguageExt.Traits;

namespace LanguageExt.Ranges;

/// <summary>
/// Number range
/// </summary>
/// <param name="From">Start of the range (inclusive)</param>
/// <param name="To">End of the range (inclusive)</param>
/// <param name="Step">Size of the step between each element in the range</param>
public record Chars(char From, char To, int Step) : Range<Chars, char, int>
{
    public static Range<char> FromMinMax(char from, char to) =>
        from <= to
            ? new Chars(from, to, 1)
            : new Chars(from, to, -1);

    public static Range<char> FromMinMax(char from, char to, int step) =>
        (from <= to) switch
        {
            true  => new Chars(from, to, step),
            false => new Chars(from, to, -step),
        };
        
    public static Range<char> FromCount(char from, long count) =>
        FromCount(from, count, (char)1);

    public static Range<char> FromCount(char from, long count, int step) =>
        count == (char)0
            ? VoidRange<char>.Default
            : FromMinMax(from, (char)(from + (count - 1) * step), step);

    public bool InRange(char value)
    {
        var inRange = From <= To
                          ? value >= From && value <= To
                          : value >= To   && value <= From;

        if(!inRange || Step == (char)1) return inRange;
        
        var diff = value - From;
        return (char)(diff % Step) == (char)0;
    }
    
    public bool Overlaps(Range<char> ra)
    {
        var (lfrom, lto) = GetExtents();
        var (rfrom, rto) = ra.GetExtents();
        return lfrom <= rto && rfrom <= lto;
    }

    public (char Min, char Max) GetExtents() =>
        From <= To
            ? (From, To)
            : (To, From);

    public Iterator<char> ForwardIterator() =>
        new Iter(From, From, To, Step);

    public override string ToString() =>
        $"['{From}'..'{To}']";

    class Iter(char Current, char From, char To, int Step) : Iterator<char>
    {
        public override (Head<char> Head, Iterator<char> Tail) Next()
        {
            var head = Current;
            var next = (char)(head + Step);
            return next < From || next > To
                       ? (new Exist<char>(head), Iterator.empty<char>())
                       : (new Exist<char>(head), new Iter(next, From, To, Step));
        }

        public override Iterator<char> Using() =>
            this;
    }
}
