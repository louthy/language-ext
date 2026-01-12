using System.Linq;
namespace LanguageExt.Tests.TraitTests;

public record FList<A>(A[] Values) : K<FList, A>;

public static class FListExtensions
{
    public static FList<A> As<A>(this K<FList, A> self) =>
        (FList<A>)self;
}

public class FList : Foldable<FList>, FoldableBack<FList>
{
    public static FList<A> New<A>(params A[] values) =>
        new (values);

    public static Iterator<A> ForwardIterator<A>(K<FList, A> fa) =>
        Iterator.lazy(() => fa.As() switch
                            {
                                { Values.Length: 0 } =>
                                    Iterator.empty<A>(),

                                { Values: var items } =>
                                    Iterator.cons(items[0], new FList<A>(items.Skip(1).ToArray()).ForwardIterator())
                            });

    public static Iterator<A> BackwardIterator<A>(K<FList, A> fa) => 
        Iterator.lazy(() => fa.As() switch
                            {
                                { Values.Length: 0 } =>
                                    Iterator.empty<A>(),

                                { Values: var items } =>
                                    Iterator.cons(items[^1], new FList<A>(items.Take(items.Length - 1).ToArray()).BackwardIterator())
                            });
}
