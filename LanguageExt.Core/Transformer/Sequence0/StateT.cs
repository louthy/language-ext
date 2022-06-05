#nullable enable
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.CompilerServices;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    public partial class StateT
    {
        /// <summary>
        /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
        /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="ta">The subject traversable</param>
        /// <returns>Mapped monad</returns>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static State<S, Seq<A>> Sequence<S, A>(this Seq<State<S, A>> ta) =>
            SequenceFast(ta).Map(toSeq);

        /// <summary>
        /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
        /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="ta">The subject traversable</param>
        /// <returns>Mapped monad</returns>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static State<S, Lst<A>> Sequence<S, A>(this Lst<State<S, A>> ta) =>
            SequenceFast(ta).Map(toList);

        /// <summary>
        /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
        /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="ta">The subject traversable</param>
        /// <returns>Mapped monad</returns>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static State<S, Arr<A>> Sequence<S, A>(this Arr<State<S, A>> ta) =>
            SequenceFast(ta).Map(toArray);

        /// <summary>
        /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
        /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="ta">The subject traversable</param>
        /// <returns>Mapped monad</returns>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static State<S, A[]> Sequence<S, A>(this State<S, A>[] ta) =>
            SequenceFast(ta).Map(x => x.ToArray());

        /// <summary>
        /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
        /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="ta">The subject traversable</param>
        /// <returns>Mapped monad</returns>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static State<S, Set<A>> Sequence<S, A>(this Set<State<S, A>> ta) =>
            SequenceFast(ta).Map(toSet);

        /// <summary>
        /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
        /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="ta">The subject traversable</param>
        /// <returns>Mapped monad</returns>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static State<S, HashSet<A>> Sequence<S, A>(this HashSet<State<S, A>> ta) =>
            SequenceFast(ta).Map(toHashSet);

        /// <summary>
        /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
        /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="ta">The subject traversable</param>
        /// <returns>Mapped monad</returns>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static State<S, Stck<A>> Sequence<S, A>(this Stck<State<S, A>> ta) =>
            SequenceFast(ta.Reverse()).Map(toStack);

        /// <summary>
        /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
        /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="ta">The subject traversable</param>
        /// <returns>Mapped monad</returns>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static State<S, IEnumerable<A>> Sequence<S, A>(this IEnumerable<State<S, A>> ta) =>
            SequenceFast(ta).Map(Enumerable.AsEnumerable);
    
        internal static State<S, List<A>> SequenceFast<S, A>(this IEnumerable<State<S, A>> ta) => state =>
        {
            var values = new List<A>();
            foreach (var item in ta)
            {
                var (a, s, bottom) = item(state);
                if (bottom) return (Value: new List<A>(), State: default(S), IsFaulted: true);
                state = s is null ? state : s;
                values.Add(a);
            }
            return (Value: values, State: state, IsFaulted: false);
        };
    }
}
