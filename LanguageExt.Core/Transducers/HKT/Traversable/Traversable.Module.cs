using System;

namespace LanguageExt.HKT;

public static class Traversable 
{
    public static Applicative<F, Traversable<T, B>> traverse<T, F, A, B>(
        Func<A, Applicative<F, B>> f,
        Traversable<T, A> ta)
        where T : Traversable<T>
        where F : Applicative<F> =>
        T.Traverse(f, ta);

    public static Applicative<F, Traversable<T, A>> sequenceA<T, F, A>(
        Traversable<T, Applicative<F, A>> ta)
        where T : Traversable<T>
        where F : Applicative<F> =>
        T.SequenceA(ta);

    public static Monad<M, Traversable<T, B>> mapM<T, M, A, B>(
        Func<A, Monad<M, B>> f,
        Traversable<T, A> ta)
        where T : Traversable<T>
        where M : Monad<M> =>
        T.MapM(f, ta);

    public static Monad<M, Traversable<T, A>> sequence<T, M, A>(
        Traversable<T, Monad<M, A>> ta)
        where T : Traversable<T>
        where M : Monad<M> =>
        T.Sequence(ta);
}
