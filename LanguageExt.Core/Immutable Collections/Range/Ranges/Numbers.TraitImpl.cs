using LanguageExt.Traits;

namespace LanguageExt.Ranges;

public class Numbers : Foldable<Numbers>
{
    public static Iterator<A> ForwardIterator<A>(K<Numbers, A> fa) =>
        ((Range<A>)fa).ForwardIterator();
}
