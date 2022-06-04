#nullable enable
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.CompilerServices;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    public partial class ReaderT
    {
        /// <summary>
        /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
        /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="ta">The subject traversable</param>
        /// <returns>Mapped monad</returns>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Reader<Env, Seq<A>> Sequence<Env, A>(this Seq<Reader<Env, A>> ta) =>
            SequenceFast(ta).Map(toSeq);

        /// <summary>
        /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
        /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="ta">The subject traversable</param>
        /// <returns>Mapped monad</returns>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Reader<Env, Lst<A>> Sequence<Env, A>(this Lst<Reader<Env, A>> ta) =>
            SequenceFast(ta).Map(toList);

        /// <summary>
        /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
        /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="ta">The subject traversable</param>
        /// <returns>Mapped monad</returns>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Reader<Env, Arr<A>> Sequence<Env, A>(this Arr<Reader<Env, A>> ta) =>
            SequenceFast(ta).Map(toArray);

        /// <summary>
        /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
        /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="ta">The subject traversable</param>
        /// <returns>Mapped monad</returns>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Reader<Env, A[]> Sequence<Env, A>(this Reader<Env, A>[] ta) =>
            SequenceFast(ta).Map(x => x.ToArray());

        /// <summary>
        /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
        /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="ta">The subject traversable</param>
        /// <returns>Mapped monad</returns>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Reader<Env, Set<A>> Sequence<Env, A>(this Set<Reader<Env, A>> ta) =>
            SequenceFast(ta).Map(toSet);

        /// <summary>
        /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
        /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="ta">The subject traversable</param>
        /// <returns>Mapped monad</returns>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Reader<Env, HashSet<A>> Sequence<Env, A>(this HashSet<Reader<Env, A>> ta) =>
            SequenceFast(ta).Map(toHashSet);

        /// <summary>
        /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
        /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="ta">The subject traversable</param>
        /// <returns>Mapped monad</returns>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Reader<Env, Stck<A>> Sequence<Env, A>(this Stck<Reader<Env, A>> ta) =>
            SequenceFast(ta.Reverse()).Map(toStack);

        /// <summary>
        /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
        /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="ta">The subject traversable</param>
        /// <returns>Mapped monad</returns>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Reader<Env, IEnumerable<A>> Sequence<Env, A>(this IEnumerable<Reader<Env, A>> ta) =>
            SequenceFast(ta).Map(Enumerable.AsEnumerable);
                
        internal static Reader<Env, List<A>> SequenceFast<Env, A>(this IEnumerable<Reader<Env, A>> ta) => env =>
        {
            var values = new List<A>();
            foreach (var item in ta)
            {
                var resA = item(env);
                if (resA.IsFaulted) return ReaderResult<List<A>>.New(resA.ErrorInt);
                values.Add(resA.Value);
            }
            return ReaderResult<List<A>>.New(values);
        };
    }
}
