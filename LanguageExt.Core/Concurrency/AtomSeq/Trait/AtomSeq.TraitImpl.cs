using LanguageExt.Traits;

namespace LanguageExt;

public class AtomSeq : Foldable<AtomSeq, Seq.FoldState>
{
    public static Iterator<A> ForwardIterator<A>(K<AtomSeq, A> fa) => 
        fa.As().Snapshot().ForwardIterator();

    public static Seq.FoldState StepSetup<A>(K<AtomSeq, A> ta) => 
        ta.As().Snapshot().StepSetup<Seq, Seq.FoldState, A>();

    public static bool Step<A>(K<AtomSeq, A> ta, ref Seq.FoldState refState, out A value) =>
        IterableK.step(ta.As().Snapshot(), ref refState, out value);
}
