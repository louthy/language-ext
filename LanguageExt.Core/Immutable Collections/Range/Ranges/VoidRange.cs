using LanguageExt.Traits;

namespace LanguageExt.Ranges;

internal class VoidRange<A, S> : Range<VoidRange<A, S>, A, S>
{
    public static readonly Range<VoidRange<A, S>, A, S> Default = new VoidRange<A, S>();
    
    public bool InRange(A value) => 
        false;

    public bool Overlaps(Range<A> ra) => 
        false;

    public (A Min, A Max) GetExtents() =>
        (default!, default!);

    public Iterator<A> ForwardIterator() => 
        Iterator<A>.Empty;

    public static Range<A> FromMinMax(A from, A to) => 
        Default;

    public static Range<A> FromMinMax(A from, A to, S step) => 
        Default;

    public static Range<A> FromCount(A from, long count) => 
        Default;

    public static Range<A> FromCount(A from, long  count, S step) => 
        Default;
}
