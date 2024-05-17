using LanguageExt;
using LanguageExt.Traits;

public static class CompositionsExt
{
    public static Compositions<A> Cons<A>(this A a, Compositions<A> ma) where A : Monoid<A> =>
        Compositions.cons(a, ma);
}
