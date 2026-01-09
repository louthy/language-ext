using LanguageExt.Traits;

namespace LanguageExt;

public class Que : Foldable<Que>
{
    public static Iterator<A> ForwardIterator<A>(K<Que, A> fa) =>
        fa.As().Value.GetIterator();
}
