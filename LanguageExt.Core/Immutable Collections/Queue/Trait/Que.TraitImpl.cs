using LanguageExt.Traits;

namespace LanguageExt;

public class Que : 
    Countable<Que>,
    Foldable<Que>
{
    public static Iterator<A> ForwardIterator<A>(K<Que, A> fa) =>
        fa.As().Value.ForwardIterator();

    static long Countable<Que>.Count<A>(K<Que, A> fa) => 
        fa.As().Count;
}
