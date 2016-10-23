using LanguageExt;
using LanguageExt.TypeClasses;

public static class NewTypeNumExtensions
{
    public static A Sum<NEWTYPE, NUM, A>(this NewType<NEWTYPE, NUM, A> self)
        where NEWTYPE : NewType<NEWTYPE, NUM, A>
        where NUM : struct, Num<A> =>
        self.Value;
}
