using System;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// Free monad makes any functor into a monad 
/// </summary>
public class Free<F> : Monad<Free<F>>, SemiAlternative<Free<F>>
    where F : Functor<F>
{
    static K<Free<F>, B> Functor<Free<F>>.Map<A, B>(Func<A, B> f, K<Free<F>, A> ma)
    {
        return go(ma.As());
        Free<F, B> go(Free<F, A> mx) =>
            mx.As() switch
            {
                Pure<F, A> (var a)  => new Pure<F, B>(f(a)),
                Bind<F, A> (var fa) => new Bind<F, B>(F.Map(go, fa)),
                _ => throw new InvalidOperationException()
            };
    }

    static K<Free<F>, B> Monad<Free<F>>.Bind<A, B>(K<Free<F>, A> ma, Func<A, K<Free<F>, B>> f) =>
        ma.As() switch
        {
            Pure<F, A> (var a) => f(a),
            Bind<F, A> (var m) => new Bind<F, B>(m.Map(mx => mx.Bind(f).As())),
            _                  => throw new InvalidOperationException()
        };

    static K<Free<F>, A> Applicative<Free<F>>.Pure<A>(A value) =>
        new Pure<F, A>(value);

    static K<Free<F>, B> Applicative<Free<F>>.Apply<A, B>(K<Free<F>, Func<A, B>> mf, K<Free<F>, A> ma) =>
        (mf.As(), ma.As()) switch
        {
            (Pure<F, Func<A, B>> (var f), Pure<F, A> (var a)) => new Pure<F, B>(f(a)),
            (Pure<F, Func<A, B>> (var f), Bind<F, A> (var a)) => new Bind<F, B>(a.Map(x => x.Map(f).As())),
            (Bind<F, Func<A, B>> (var f), Bind<F, A> a)       => new Bind<F, B>(f.Map(x => x.Apply(a).As())),
            _                                                 => throw new InvalidOperationException()
        };

    static K<Free<F>, A> SemigroupK<Free<F>>.Combine<A>(K<Free<F>, A> lhs, K<Free<F>, A> rhs) => 
        throw new NotImplementedException();
}
