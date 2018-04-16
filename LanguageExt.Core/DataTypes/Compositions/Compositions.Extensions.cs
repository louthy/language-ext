using LanguageExt;
using LanguageExt.TypeClasses;

public static class CompositionsExt
{
    public static Compositions<A> Cons<MonoidA, A>(this A a, Compositions<A> ma) where MonoidA : struct, Monoid<A> =>
        Compositions.cons<MonoidA, A>(a, ma);
}
