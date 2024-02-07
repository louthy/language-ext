using System;
using static LanguageExt.Prelude;
using static LanguageExt.Transducer;

namespace LanguageExt.HKT;

public interface MonadReader<M, E> : Functor<M, E> 
    where M : MonadReader<M, E>
{
    public static virtual KArrow<M, E, A> Pure<A>(A value) => 
        M.Lift(constant<E, A>(value));

    public static virtual KArrow<M, E, A> Flatten<A>(KArrow<M, E, KArrow<M, E, A>> mma) =>
        M.Lift(lift<E, Transducer<E, A>>(env => mma.Invoke(env).Morphism).Flatten());

    public static virtual KArrow<M, E, B> Bind<A, B>(KArrow<M, E, A> ma, Transducer<A, KArrow<M, E, B>> f) =>
        M.Flatten(M.Map(ma, f));

    public static virtual KArrow<M, E, B> Bind<A, B>(KArrow<M, E, A> ma, Func<A, KArrow<M, E, B>> f) =>
        M.Bind(ma, lift(f));
}
