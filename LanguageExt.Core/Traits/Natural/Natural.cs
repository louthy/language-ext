namespace LanguageExt.Traits;

/// <summary>
/// Natural transformation
/// </summary>
/// <typeparam name="F">From functor</typeparam>
/// <typeparam name="G">To functor</typeparam>
public interface Natural<out F, in G>
{
    public static abstract K<G, A> Transform<A>(K<F, A> fa);
}

public static class NaturalTest
{
    public static void Foo()
    {
        var fa = Free.pure<Option, int>(100);
        var ga = Free.hoist<Option, Seq, int>(fa);
    }
}
