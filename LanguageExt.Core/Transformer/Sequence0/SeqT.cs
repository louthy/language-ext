#nullable enable
using LanguageExt.TypeClasses;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    public partial class SeqT
    {
        /// <summary>
        /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
        /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="ta">The subject traversable</param>
        /// <returns>Mapped monad</returns>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Seq<Arr<A>> Sequence<A>(this Arr<Seq<A>> ta) =>
            ta.Traverse(identity);
        
        /// <summary>
        /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
        /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="ta">The subject traversable</param>
        /// <returns>Mapped monad</returns>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Seq<Either<L, A>> Sequence<L, A>(this Either<L, Seq<A>> ta) =>
            ta.Traverse(identity);
        
        /// <summary>
        /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
        /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="ta">The subject traversable</param>
        /// <returns>Mapped monad</returns>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Seq<EitherUnsafe<L, A>> Sequence<L, A>(this EitherUnsafe<L, Seq<A>> ta) =>
            ta.Traverse(identity);
        
        /// <summary>
        /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
        /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="ta">The subject traversable</param>
        /// <returns>Mapped monad</returns>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Seq<Identity<A>> Sequence<A>(this Identity<Seq<A>> ta) =>
            ta.Traverse(identity);

        /// <summary>
        /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
        /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="ta">The subject traversable</param>
        /// <returns>Mapped monad</returns>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Seq<IEnumerable<A>> Sequence<A>(this IEnumerable<Seq<A>> ta) =>
            ta.Traverse(identity);

        /// <summary>
        /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
        /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="ta">The subject traversable</param>
        /// <returns>Mapped monad</returns>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Seq<Lst<A>> Sequence<A>(this Lst<Seq<A>> ta) =>
            ta.Traverse(identity);

        /// <summary>
        /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
        /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="ta">The subject traversable</param>
        /// <returns>Mapped monad</returns>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Seq<HashSet<A>> Sequence<A>(this HashSet<Seq<A>> ta) =>
            ta.Traverse(identity);
        
        /// <summary>
        /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
        /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="ta">The subject traversable</param>
        /// <returns>Mapped monad</returns>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Seq<Fin<A>> Sequence<A>(this Fin<Seq<A>> ta) =>
            ta.Traverse(identity);
        
        /// <summary>
        /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
        /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="ta">The subject traversable</param>
        /// <returns>Mapped monad</returns>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Seq<Option<A>> Sequence<A>(this Option<Seq<A>> ta) =>
            ta.Traverse(identity);
        
        /// <summary>
        /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
        /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="ta">The subject traversable</param>
        /// <returns>Mapped monad</returns>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Seq<OptionUnsafe<A>> Sequence<A>(this OptionUnsafe<Seq<A>> ta) =>
            ta.Traverse(identity);
        
        /// <summary>
        /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
        /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="ta">The subject traversable</param>
        /// <returns>Mapped monad</returns>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Seq<Seq<A>> Sequence<A>(this Seq<Seq<A>> ta) =>
            ta.Traverse(identity);
        
        /// <summary>
        /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
        /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="ta">The subject traversable</param>
        /// <returns>Mapped monad</returns>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Seq<Set<A>> Sequence<A>(this Set<Seq<A>> ta) =>
            ta.Traverse(identity);
        
        /// <summary>
        /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
        /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="ta">The subject traversable</param>
        /// <returns>Mapped monad</returns>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Seq<Try<A>> Sequence<A>(this Try<Seq<A>> ta) =>
            ta.Traverse(identity);
        
        /// <summary>
        /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
        /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="ta">The subject traversable</param>
        /// <returns>Mapped monad</returns>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Seq<TryOption<A>> Sequence<A>(this TryOption<Seq<A>> ta) =>
            ta.Traverse(identity);
        
        /// <summary>
        /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
        /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="ta">The subject traversable</param>
        /// <returns>Mapped monad</returns>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Seq<Validation<FAIL, A>> Sequence<FAIL, A>(this Validation<FAIL, Seq<A>> ta) => 
            ta.Traverse(identity);
        
        /// <summary>
        /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
        /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="ta">The subject traversable</param>
        /// <returns>Mapped monad</returns>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Seq<Validation<MonoidFail, FAIL, A>> Sequence<MonoidFail, FAIL, A>(this Validation<MonoidFail, FAIL, Seq<A>> ta)
            where MonoidFail : struct, Monoid<FAIL>, Eq<FAIL> =>
            ta.Traverse(identity);

        /// <summary>
        /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
        /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="ta">The subject traversable</param>
        /// <returns>Mapped monad</returns>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Seq<Eff<A>> Sequence<A>(this Eff<Seq<A>> ta) =>
            ta.Traverse(identity);
    }
}
