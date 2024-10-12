using LanguageExt.Traits;

namespace LanguageExt;

public static partial class FreeExtensions
{
    public static Free<Fnctr, A> As<Fnctr, A>(this K<Free<Fnctr>, A> ma)
        where Fnctr : Functor<Fnctr> =>
        (Free<Fnctr, A>)ma;
}
