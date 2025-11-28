using System.Diagnostics.Contracts;

namespace LanguageExt.Traits;

public static partial class Applicative
{
    [Pure]
    public static K<F, A> pure<F, A>(A value) 
        where F : Applicative<F> =>
        F.Pure(value);

    [Pure]
    public static K<F, Unit> when<F>(bool flag, K<F, Unit> fx)
        where F : Applicative<F> =>
        flag ? fx : F.Pure<Unit>(default);

    [Pure]
    public static K<F, Unit> unless<F>(bool flag, K<F, Unit> fx)
        where F : Applicative<F> =>
        when(!flag, fx);
}
