using System;

namespace LanguageExt.TypeClasses
{
    [Typeclass]
    public interface Traversable<A> : Foldable<A>, Functor<A>
    {
        /// <summary>
        /// Traverse the structure, mapping the bound values to applicatives
        /// </summary>
        /// <typeparam name="B">Return value type</typeparam>
        /// <param name="ta">Traversable to traverse</param>
        /// <param name="f">Function to apply</param>
        /// <returns>Mapped traversable</returns>
        Monad<Traversable<B>> Traverse<B>(Traversable<A> ta, Func<A, Monad<B>> f);

        /// <summary>
        /// Sequence
        /// </summary>
        Monad<Traversable<A>> Sequence(Traversable<A> ta);
    }
}
