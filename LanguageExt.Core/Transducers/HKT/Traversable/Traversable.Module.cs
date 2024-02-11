using System;

namespace LanguageExt.HKT;

public static class Traversable 
{
    public static K<F, K<T, B>> traverse<T, F, A, B>(
        Func<A, K<F, B>> f,
        K<T, A> ta)
        where T : Traversable<T>
        where F : Applicative<F> =>
        T.Traverse(f, ta);

    public static K<F, K<T, A>> sequenceA<T, F, A>(
        K<T, K<F, A>> ta)
        where T : Traversable<T>
        where F : Applicative<F> =>
        T.SequenceA(ta);

    public static K<M, K<T, B>> mapM<T, M, A, B>(
        Func<A, K<M, B>> f,
        K<T, A> ta)
        where T : Traversable<T>
        where M : Monad<M> =>
        T.MapM(f, ta);

    public static K<M, K<T, A>> sequence<T, M, A>(
        K<T, K<M, A>> ta)
        where T : Traversable<T>
        where M : Monad<M> =>
        T.Sequence(ta);
}
