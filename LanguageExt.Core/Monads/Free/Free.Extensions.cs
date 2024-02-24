using LanguageExt.Traits;

namespace LanguageExt.Free;

public static class Free
{
    public static Free<F, A> As<F, A>(this K<Free<F>, A> ma)
        where F : Functor<F>, Alternative<F> =>
        (Free<F, A>)ma;
}
