#nullable enable
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.CompilerServices;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    public partial class WriterT
    {
        /// <summary>
        /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
        /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="ta">The subject traversable</param>
        /// <returns>Mapped monad</returns>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Writer<MonoidW, W, Seq<A>> Sequence<MonoidW, W, A>(this Seq<Writer<MonoidW, W, A>> ta) where MonoidW : struct, Monoid<W> =>
            SequenceFast(ta).Map(toSeq);

        /// <summary>
        /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
        /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="ta">The subject traversable</param>
        /// <returns>Mapped monad</returns>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Writer<MonoidW, W, Lst<A>> Sequence<MonoidW, W, A>(this Lst<Writer<MonoidW, W, A>> ta) where MonoidW : struct, Monoid<W> =>
            SequenceFast(ta).Map(toList);

        /// <summary>
        /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
        /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="ta">The subject traversable</param>
        /// <returns>Mapped monad</returns>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Writer<MonoidW, W, Arr<A>> Sequence<MonoidW, W, A>(this Arr<Writer<MonoidW, W, A>> ta) where MonoidW : struct, Monoid<W> =>
            SequenceFast(ta).Map(toArray);

        /// <summary>
        /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
        /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="ta">The subject traversable</param>
        /// <returns>Mapped monad</returns>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Writer<MonoidW, W, A[]> Sequence<MonoidW, W, A>(this Writer<MonoidW, W, A>[] ta) where MonoidW : struct, Monoid<W> =>
            SequenceFast(ta).Map(x => x.ToArray());

        /// <summary>
        /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
        /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="ta">The subject traversable</param>
        /// <returns>Mapped monad</returns>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Writer<MonoidW, W, Set<A>> Sequence<MonoidW, W, A>(this Set<Writer<MonoidW, W, A>> ta) where MonoidW : struct, Monoid<W> =>
            SequenceFast(ta).Map(toSet);

        /// <summary>
        /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
        /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="ta">The subject traversable</param>
        /// <returns>Mapped monad</returns>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Writer<MonoidW, W, HashSet<A>> Sequence<MonoidW, W, A>(this HashSet<Writer<MonoidW, W, A>> ta) where MonoidW : struct, Monoid<W> =>
            SequenceFast(ta).Map(toHashSet);

        /// <summary>
        /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
        /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="ta">The subject traversable</param>
        /// <returns>Mapped monad</returns>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Writer<MonoidW, W, Stck<A>> Sequence<MonoidW, W, A>(this Stck<Writer<MonoidW, W, A>> ta) where MonoidW : struct, Monoid<W> =>
            SequenceFast(ta.Reverse()).Map(toStack);

        /// <summary>
        /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
        /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="ta">The subject traversable</param>
        /// <returns>Mapped monad</returns>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Writer<MonoidW, W, IEnumerable<A>> Sequence<MonoidW, W, A>(this IEnumerable<Writer<MonoidW, W, A>> ta) where MonoidW : struct, Monoid<W> =>
            SequenceFast(ta).Map(Enumerable.AsEnumerable);
    }
}
