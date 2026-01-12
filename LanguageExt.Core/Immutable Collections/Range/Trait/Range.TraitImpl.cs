using LanguageExt.Traits;

namespace LanguageExt;

public partial class Range
    : Foldable<Range, Range.IteratorState>
{
    public static Iterator<A> ForwardIterator<A>(K<Range, A> fa)
    {
        var r = +fa;
        return new Iterator.IterRange<A>(r.From, false, r.To, r.Step, r.Eq);
    }

    public static IteratorState StepSetup<A>(K<Range, A> ta)
    {
        var r = +ta;
        return IteratorState.Setup(r.From, r.To, r.Step, r.Eq);
    }

    public static bool Step<A>(K<Range, A> ta, ref IteratorState refState, out A value) => 
        refState.Step(out value);
}
