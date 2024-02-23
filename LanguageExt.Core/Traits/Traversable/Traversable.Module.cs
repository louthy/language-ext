using System;

namespace LanguageExt.Traits;

public static partial class Traversable 
{
    /// <summary>
    /// Map each element of a structure to an action, evaluate these actions from
    /// left to right, and collect the results.
    /// </summary>
    /// <remarks>
    /// This version of `traverse` works with the lifted `K` types which are
    /// sometimes difficult to work with when nested.  If you need to get concrete
    /// types out of your traversal operation then use `traverse2` - it needs more
    /// generic parameters but it retains the concrete types.
    /// </remarks>
    /// <param name="f"></param>
    /// <param name="ta">Traversable structure</param>
    /// <typeparam name="T">Traversable trait</typeparam>
    /// <typeparam name="F">Applicative functor trait</typeparam>
    /// <typeparam name="A">Bound value (input)</typeparam>
    /// <typeparam name="B">Bound value (output)</typeparam>
    /// <returns></returns>
    public static K<F, K<T, B>> traverse<T, F, A, B>(
        Func<A, K<F, B>> f,
        K<T, A> ta)
        where T : Traversable<T>
        where F : Applicative<F> =>
        T.Traverse(f, ta);

    /// <summary>
    /// Evaluate each action in the structure from left to right, and collect the results. 
    /// </summary>
    /// <param name="ta">Traversable structure</param>
    /// <typeparam name="T">Traversable trait</typeparam>
    /// <typeparam name="F">Applicative functor trait</typeparam>
    /// <typeparam name="A">Bound value (input)</typeparam>
    /// <returns></returns>
    public static K<F, K<T, A>> sequenceA<T, F, A>(
        K<T, K<F, A>> ta)
        where T : Traversable<T>
        where F : Applicative<F> =>
        T.SequenceA(ta);

    /// <summary>
    /// Map each element of a structure to a monadic action, evaluate these actions from
    /// left to right, and collect the results.
    /// </summary>
    /// <param name="f"></param>
    /// <param name="ta">Traversable structure</param>
    /// <typeparam name="T">Traversable trait</typeparam>
    /// <typeparam name="M">Monad trait</typeparam>
    /// <typeparam name="A">Bound value (input)</typeparam>
    /// <typeparam name="B">Bound value (output)</typeparam>
    /// <returns></returns>
    public static K<M, K<T, B>> mapM<T, M, A, B>(
        Func<A, K<M, B>> f,
        K<T, A> ta)
        where T : Traversable<T>
        where M : Monad<M> =>
        T.MapM(f, ta);

    /// <summary>
    /// Evaluate each monadic action in the structure from left to right, and collect the results. 
    /// </summary>
    /// <param name="ta">Traversable structure</param>
    /// <typeparam name="T">Traversable trait</typeparam>
    /// <typeparam name="M">Monad trait</typeparam>
    /// <typeparam name="A">Bound value (input)</typeparam>
    /// <returns></returns>
    public static K<M, K<T, A>> sequence<T, M, A>(
        K<T, K<M, A>> ta)
        where T : Traversable<T>
        where M : Monad<M> =>
        T.Sequence(ta);
}
