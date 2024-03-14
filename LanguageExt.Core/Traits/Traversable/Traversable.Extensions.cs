using System;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// Functors representing data structures that can be transformed to structures of the same
/// shape by performing an `Applicative` (or, therefore, `Monad`) action on each element from
/// left to right.
///
/// A more detailed description of what same shape means, the various methods, how traversals
/// are constructed, and example advanced use-cases can be found in the Overview section of Data.Traversable.
/// </summary>
/// <typeparam name="T"></typeparam>
public static partial class TraversableExtensions
{
    public static K<F, K<T, B>> Traverse<T, F, A, B>(
        this K<T, A> ta,
        Func<A, K<F, B>> f)
        where T : Traversable<T>
        where F : Applicative<F> =>
        Traversable.traverse(f, ta);

    public static K<F, K<T, A>> SequenceA<T, F, A>(
        this K<T, K<F, A>> ta)
        where T : Traversable<T>
        where F : Applicative<F> =>
        Traversable.traverse(x => x, ta);

    public static K<M, K<T, B>> MapM<M, T, A, B>(
        this Func<A, K<M, B>> f,
        K<T, A> ta)
        where T : Traversable<T>
        where M : Monad<M> =>
        Traversable.traverse(f, ta);

    public static K<F, K<T, A>> Sequence<T, F, A>(
        this K<T, K<F, A>> ta)
        where T : Traversable<T>
        where F : Monad<F> =>
        Traversable.traverse(x => x, ta);
}
