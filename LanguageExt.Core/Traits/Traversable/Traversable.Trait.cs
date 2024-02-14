using System;

namespace LanguageExt.Traits;

public interface Traversable<T> : Functor<T>, Foldable<T> 
    where T : Traversable<T>, Functor<T>, Foldable<T>
{
    public static abstract K<F, K<T, B>> Traverse<F, A, B>(
        Func<A, K<F, B>> f,
        K<T, A> ta)
        where F : Applicative<F>;

    public static virtual K<F, K<T, A>> SequenceA<F, A>(
        K<T, K<F, A>> ta)
        where F : Applicative<F> =>
        Traversable.traverse(x => x, ta);

    public static virtual K<M, K<T, B>> MapM<M, A, B>(
        Func<A, K<M, B>> f,
        K<T, A> ta)
        where M : Monad<M> =>
        Traversable.traverse(f, ta);

    public static virtual K<F, K<T, A>> Sequence<F, A>(
        K<T, K<F, A>> ta)
        where F : Monad<F> =>
        Traversable.traverse(x => x, ta);
    
    public static virtual K<F, K<T, B>> TraverseDefault<F, A, B>(
        Func<A, K<F, B>> f,
        K<T, A> ta)
        where F : Applicative<F> =>
        Traversable.sequenceA(T.Map(f, ta));
}
