using LanguageExt.Traits;

namespace LanguageExt;

public static class FreeExtensions
{
    public static Free<F, A> As<F, A>(this K<Free<F>, A> ma)
        where F : Functor<F> =>
        (Free<F, A>)ma;
}
