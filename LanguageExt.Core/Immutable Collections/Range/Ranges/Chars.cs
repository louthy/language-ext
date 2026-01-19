using System.Runtime.CompilerServices;
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
            ? VoidRange<char, int>.Default
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
        From <= To
            ? new Iter(From, From, To, Step)
            : new Iter(From, To, From, Step);

    public override string ToString() =>
        $"['{From}'..'{To}']";
    
    public bool SupportsFastIteration => 
        true;
    
    public int FastIterationStateSizeInBytes { get; } = 
        Unsafe.SizeOf<IteratorState>();

    public void FastIterationSetup(ref Range.IteratorState state)
    {
        ref var s = ref Range.IteratorState.To<IteratorState>(ref state);
        if (From <= To)
        {
            IteratorState.Setup(ref s, From, From, To, Step);
        }
        else
        {
            IteratorState.Setup(ref s, From, To, From, Step);
        }
        state = Range.IteratorState.From(ref s);
    }

    public bool FastIterationStep(ref Range.IteratorState state, out char value)
    {
        ref var s = ref Range.IteratorState.To<IteratorState>(ref state);
        var     c = IteratorState.Next(ref s, out value);
        state = Range.IteratorState.From(ref s);
        return c;
    }    

    class Iter(char Current, char Min, char Max, int Step) : Iterator<char>
    {
        public override (Head<char> Head, Iterator<char> Tail) Next()
        {
            var head = Current;
            var next = (char)(head + Step);
            return next < Min || next > Max
                       ? (new Exist<char>(head), Iterator.empty<char>())
                       : (new Exist<char>(head), new Iter(next, Min, Max, Step));
        }
    }
    
    ref struct IteratorState
    {
        char Current;
        char Min;
        char Max;
        int Step;

        public IteratorState(char current, char min, char max, int step)
        { 
            Current = current;
            Min = min;
            Max = max;
            Step = step;
        }

        public static void Setup(ref IteratorState state, char current, char min, char max, int step)
        {
            ref var scurrent = ref state.Current;
            ref var smin     = ref state.Min;
            ref var smax     = ref state.Max;
            ref var sstep    = ref state.Step;

            scurrent = current;
            smin     = min;
            smax     = max;
            sstep    = step;
        }
        
        public static bool Next(ref IteratorState state, out char head)
        {
            ref var          current = ref state.Current;
            ref readonly var min     = ref state.Min; 
            ref readonly var max     = ref state.Max;
            ref readonly var step    = ref state.Step;

            if (current < min || current > max)
            {
                head = default!;
                return false;
            }
            else
            {
                head = current;
                current = (char)(current + step);
                return true;
            }
        }
    }    
}
