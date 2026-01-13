using System.Numerics;
using LanguageExt.Traits;

namespace LanguageExt.Ranges;

/// <summary>
/// Number range
/// </summary>
/// <param name="From">Start of the range (inclusive)</param>
/// <param name="To">End of the range (inclusive)</param>
/// <param name="Step">Size of the step between each element in the range</param>
public record Numbers<N>(N From, N To, N Step) : Range<Numbers<N>, N, N>
    where N : INumber<N>
{
    public static Range<N> FromMinMax(N from, N to) =>
        from <= to
            ? new Numbers<N>(from, to, N.One)
            : new Numbers<N>(from, to, -N.One);

    public static Range<N> FromMinMax(N from, N to, N step) =>
        (from <= to, step >= N.Zero) switch
        {
            (true, true)   => new Numbers<N>(from, to, step),
            (true, false)  => new Numbers<N>(from, to, -step),
            (false, true)  => new Numbers<N>(from, to, -step),
            (false, false) => new Numbers<N>(from, to, step)
        };
        
    public static Range<N> FromCount(N from, long count) =>
        FromCount(from, count, N.One);

    public static Range<N> FromCount(N from, long count, N step) =>
        count <= 0
            ? VoidRange<N, N>.Default
            : FromMinMax(from, from + N.CreateChecked(count - 1) * step, step);

    public bool InRange(N value)
    {
        var inRange = From <= To
                          ? value >= From && value <= To
                          : value >= To   && value <= From;
        
        if(!inRange || Step == N.One) return inRange;
        var diff = value - From;
        return diff % Step == N.Zero;
    }
    
    public bool Overlaps(Range<N> ra)
    {
        var (lfrom, lto) = GetExtents();
        var (rfrom, rto) = ra.GetExtents();
        return lfrom <= rto && rfrom <= lto;
    }

    public (N Min, N Max) GetExtents() =>
        From <= To
            ? (From, To)
            : (To, From);

    public Iterator<N> ForwardIterator() =>
        From <= To
            ? new Iter(From, From, To, Step)
            : new Iter(From, To, From, Step);


    public override string ToString() =>
        $"[{From}..{To}]";

    class Iter(N Current, N Min, N Max, N Step) : Iterator<N>
    {
        public override (Head<N> Head, Iterator<N> Tail) Next()
        {
            var head = Current;
            var next = head + Step;
            return next < Min || next > Max
                       ? (new Exist<N>(head), Iterator.empty<N>())
                       : (new Exist<N>(head), new Iter(next, Min, Max, Step));
        }

        public override Iterator<N> Using() =>
            this;
    }
}
