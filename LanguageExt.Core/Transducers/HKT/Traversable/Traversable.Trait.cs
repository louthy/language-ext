using System;

namespace LanguageExt.HKT;

public interface Traversable<T> : Functor<T>, Foldable<T> 
    where T : Traversable<T>, Functor<T>, Foldable<T>
{
    public static virtual Applicative<F, Traversable<T, B>> Traverse<F, A, B>(
        Func<A, Applicative<F, B>> f,
        Traversable<T, A> ta)
        where F : Applicative<F> =>
        Traversable.sequenceA(T.Map(f, ta));

    public static virtual Applicative<F, Traversable<T, A>> SequenceA<F, A>(
        Traversable<T, Applicative<F, A>> ta)
        where F : Applicative<F> =>
        Traversable.traverse(x => x, ta);

    public static virtual Monad<M, Traversable<T, B>> MapM<M, A, B>(
        Func<A, Monad<M, B>> f,
        Traversable<T, A> ta)
        where M : Monad<M> =>
        Traversable.traverse(f, ta).AsMonad();

    public static virtual Monad<F, Traversable<T, A>> Sequence<F, A>(
        Traversable<T, Monad<F, A>> ta)
        where F : Monad<F> =>
        Traversable.traverse(x => x, ta).AsMonad();
    
    // Functor

    public static virtual Traversable<T, B> Map<A, B>(Transducer<A, B> f, Traversable<T, A> ma) =>
        Functor.map(f, ma).AsTraversable();

    public static virtual Traversable<T, B> Map<A, B>(Func<A, B> f, Traversable<T, A> ma) =>
        Functor.map(f, ma).AsTraversable();
}

public interface Traversable<T, A> : Functor<T, A>, Foldable<T, A>
    where T : Functor<T>, Foldable<T>;
