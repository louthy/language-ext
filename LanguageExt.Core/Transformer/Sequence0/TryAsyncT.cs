#nullable enable
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    public partial class TryAsyncT
    {
        /// <summary>
        /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
        /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="ta">The subject traversable</param>
        /// <returns>Mapped monad</returns>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TryAsync<Arr<A>> Sequence<A>(this Arr<TryAsync<A>> ta) =>
            ta.Traverse(identity);
        
        /// <summary>
        /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
        /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="ta">The subject traversable</param>
        /// <returns>Mapped monad</returns>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TryAsync<Either<L, A>> Sequence<L, A>(this Either<L, TryAsync<A>> ta) =>
            ta.Traverse(identity);
        
        /// <summary>
        /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
        /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="ta">The subject traversable</param>
        /// <returns>Mapped monad</returns>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TryAsync<EitherUnsafe<L, A>> Sequence<L, A>(this EitherUnsafe<L, TryAsync<A>> ta) =>
            ta.Traverse(identity);
        
        /// <summary>
        /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
        /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="ta">The subject traversable</param>
        /// <returns>Mapped monad</returns>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TryAsync<Identity<A>> Sequence<A>(this Identity<TryAsync<A>> ta) =>
            ta.Traverse(identity);
        
        /// <summary>
        /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
        /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="ta">The subject traversable</param>
        /// <returns>Mapped monad</returns>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TryAsync<Lst<A>> Sequence<A>(this Lst<TryAsync<A>> ta) =>
            ta.Traverse(identity);
        
        /// <summary>
        /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
        /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="ta">The subject traversable</param>
        /// <returns>Mapped monad</returns>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TryAsync<TryAsync<A>> Sequence<A>(this TryAsync<TryAsync<A>> ta) =>
            ta.Traverse(identity);
        
        /// <summary>
        /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
        /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="ta">The subject traversable</param>
        /// <returns>Mapped monad</returns>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TryAsync<Fin<A>> Sequence<A>(this Fin<TryAsync<A>> ta) =>
            ta.Traverse(identity);
        
        /// <summary>
        /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
        /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="ta">The subject traversable</param>
        /// <returns>Mapped monad</returns>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TryAsync<Option<A>> Sequence<A>(this Option<TryAsync<A>> ta) =>
            ta.Traverse(identity);
        
        /// <summary>
        /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
        /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="ta">The subject traversable</param>
        /// <returns>Mapped monad</returns>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TryAsync<OptionUnsafe<A>> Sequence<A>(this OptionUnsafe<TryAsync<A>> ta) =>
            ta.Traverse(identity);
        
        /// <summary>
        /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
        /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="ta">The subject traversable</param>
        /// <returns>Mapped monad</returns>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TryAsync<Set<A>> Sequence<A>(this Set<TryAsync<A>> ta) =>
            ta.Traverse(identity);
        
        /// <summary>
        /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
        /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="ta">The subject traversable</param>
        /// <returns>Mapped monad</returns>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TryAsync<HashSet<A>> Sequence<A>(this HashSet<TryAsync<A>> ta) =>
            ta.Traverse(identity);
        
        /// <summary>
        /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
        /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="ta">The subject traversable</param>
        /// <returns>Mapped monad</returns>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TryAsync<Try<A>> Sequence<A>(this Try<TryAsync<A>> ta) =>
            ta.Traverse(identity);
        
        /// <summary>
        /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
        /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="ta">The subject traversable</param>
        /// <returns>Mapped monad</returns>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TryAsync<TryOption<A>> Sequence<A>(this TryOption<TryAsync<A>> ta) =>
            ta.Traverse(identity);
        
        /// <summary>
        /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
        /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="ta">The subject traversable</param>
        /// <returns>Mapped monad</returns>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TryAsync<Validation<FAIL, A>> Sequence<FAIL, A>(this Validation<FAIL, TryAsync<A>> ta) => 
            ta.Traverse(identity);
        
        /// <summary>
        /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
        /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="ta">The subject traversable</param>
        /// <returns>Mapped monad</returns>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TryAsync<Validation<MonoidFail, FAIL, A>> Sequence<MonoidFail, FAIL, A>(this Validation<MonoidFail, FAIL, TryAsync<A>> ta)
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
        public static TryAsync<Task<A>> Sequence<A>(this Task<TryAsync<A>> ta) =>
            ta.Traverse(identity);
        
        /// <summary>
        /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
        /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="ta">The subject traversable</param>
        /// <returns>Mapped monad</returns>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TryAsync<OptionAsync<A>> Sequence<A>(this OptionAsync<TryAsync<A>> ta) =>
            ta.Traverse(identity);
        
        /// <summary>
        /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
        /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="ta">The subject traversable</param>
        /// <returns>Mapped monad</returns>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TryAsync<TryOptionAsync<A>> Sequence<A>(this TryOptionAsync<TryAsync<A>> ta) =>
            ta.Traverse(identity);
        
        /// <summary>
        /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
        /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="ta">The subject traversable</param>
        /// <returns>Mapped monad</returns>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TryAsync<EitherAsync<L, A>> Sequence<L, A>(this EitherAsync<L, TryAsync<A>> ta) =>
            ta.Traverse(identity);

        /// <summary>
        /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
        /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="ta">The subject traversable</param>
        /// <returns>Mapped monad</returns>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TryAsync<Aff<A>> Sequence<A>(this Aff<TryAsync<A>> ta) =>
            ta.Traverse(identity);

        /// <summary>
        /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
        /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="ta">The subject traversable</param>
        /// <returns>Mapped monad</returns>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TryAsync<Eff<A>> Sequence<A>(this Eff<TryAsync<A>> ta) =>
            ta.Traverse(identity);
    }
}
