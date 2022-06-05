#nullable enable
using System.Linq;
using LanguageExt.TypeClasses;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    public partial class RwsT
    {
        /// <summary>
        /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
        /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="ta">The subject traversable</param>
        /// <returns>Mapped monad</returns>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RWS<MonoidW, R, W, S, Seq<A>> Sequence<MonoidW, R, W, S, A>(this Seq<RWS<MonoidW, R, W, S, A>> ta) where MonoidW : struct, Monoid<W> =>
            SequenceFast(ta).Map(toSeq);

        /// <summary>
        /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
        /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="ta">The subject traversable</param>
        /// <returns>Mapped monad</returns>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RWS<MonoidW, R, W, S, Lst<A>> Sequence<MonoidW, R, W, S, A>(this Lst<RWS<MonoidW, R, W, S, A>> ta) where MonoidW : struct, Monoid<W> =>
            SequenceFast(ta).Map(toList);

        /// <summary>
        /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
        /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="ta">The subject traversable</param>
        /// <returns>Mapped monad</returns>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RWS<MonoidW, R, W, S, Arr<A>> Sequence<MonoidW, R, W, S, A>(this Arr<RWS<MonoidW, R, W, S, A>> ta) where MonoidW : struct, Monoid<W> =>
            SequenceFast(ta).Map(toArray);

        /// <summary>
        /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
        /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="ta">The subject traversable</param>
        /// <returns>Mapped monad</returns>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RWS<MonoidW, R, W, S, A[]> Sequence<MonoidW, R, W, S, A>(this RWS<MonoidW, R, W, S, A>[] ta) where MonoidW : struct, Monoid<W> =>
            SequenceFast(ta).Map(x => x.ToArray());

        /// <summary>
        /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
        /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="ta">The subject traversable</param>
        /// <returns>Mapped monad</returns>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RWS<MonoidW, R, W, S, Set<A>> Sequence<MonoidW, R, W, S, A>(this Set<RWS<MonoidW, R, W, S, A>> ta) where MonoidW : struct, Monoid<W> =>
            SequenceFast(ta).Map(toSet);

        /// <summary>
        /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
        /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="ta">The subject traversable</param>
        /// <returns>Mapped monad</returns>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RWS<MonoidW, R, W, S, HashSet<A>> Sequence<MonoidW, R, W, S, A>(this HashSet<RWS<MonoidW, R, W, S, A>> ta) where MonoidW : struct, Monoid<W> =>
            SequenceFast(ta).Map(toHashSet);

        /// <summary>
        /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
        /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="ta">The subject traversable</param>
        /// <returns>Mapped monad</returns>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RWS<MonoidW, R, W, S, Stck<A>> Sequence<MonoidW, R, W, S, A>(this Stck<RWS<MonoidW, R, W, S, A>> ta) where MonoidW : struct, Monoid<W> =>
            SequenceFast(ta.Reverse()).Map(toStack);

        /// <summary>
        /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
        /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="ta">The subject traversable</param>
        /// <returns>Mapped monad</returns>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RWS<MonoidW, R, W, S, IEnumerable<A>> Sequence<MonoidW, R, W, S, A>(this IEnumerable<RWS<MonoidW, R, W, S, A>> ta) where MonoidW : struct, Monoid<W> =>
            SequenceFast(ta).Map(Enumerable.AsEnumerable);
                
        internal static RWS<MonoidW, R, W, S, List<A>> SequenceFast<MonoidW, R, W, S, A>(this IEnumerable<RWS<MonoidW, R, W, S, A>> ta) where MonoidW : struct, Monoid<W> => (env, state) =>
        {
            var values = new List<A>();
            var output = default(MonoidW).Empty();
            foreach (var item in ta)
            {
                var res = item(env, state);
                if (res.IsFaulted) return RWSResult<MonoidW, R, W, S, List<A>>.New(state, res.Error);
                values.Add(res.Value);
                state = res.State;
                output = default(MonoidW).Append(output, res.Output);
            }
            return RWSResult<MonoidW, R, W, S, List<A>>.New(output, state, values);
        };

    }
}
