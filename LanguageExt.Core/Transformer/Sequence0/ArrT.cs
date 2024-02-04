using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;

namespace LanguageExt;

public partial class ArrT
{
    /// <summary>
    /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
    /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="ta">The subject traversable</param>
    /// <returns>Mapped monad</returns>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Arr<Arr<A>> Sequence<A>(this Arr<Arr<A>> ta) =>
        ta.Traverse(identity);
        
    /// <summary>
    /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
    /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="ta">The subject traversable</param>
    /// <returns>Mapped monad</returns>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Arr<Either<L, A>> Sequence<L, A>(this Either<L, Arr<A>> ta) =>
        ta.Traverse(identity);
        
    /// <summary>
    /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
    /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="ta">The subject traversable</param>
    /// <returns>Mapped monad</returns>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Arr<Identity<A>> Sequence<A>(this Identity<Arr<A>> ta) =>
        ta.Traverse(identity);

    /// <summary>
    /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
    /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="ta">The subject traversable</param>
    /// <returns>Mapped monad</returns>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Arr<IEnumerable<A>> Sequence<A>(this IEnumerable<Arr<A>> ta) =>
        ta.Traverse(identity);
        
    /// <summary>
    /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
    /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="ta">The subject traversable</param>
    /// <returns>Mapped monad</returns>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Arr<Lst<A>> Sequence<A>(this Lst<Arr<A>> ta) =>
        ta.Traverse(identity);
        
    /// <summary>
    /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
    /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="ta">The subject traversable</param>
    /// <returns>Mapped monad</returns>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Arr<Fin<A>> Sequence<A>(this Fin<Arr<A>> ta) =>
        ta.Traverse(identity);
        
    /// <summary>
    /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
    /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="ta">The subject traversable</param>
    /// <returns>Mapped monad</returns>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Arr<Option<A>> Sequence<A>(this Option<Arr<A>> ta) =>
        ta.Traverse(identity);
        
    /// <summary>
    /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
    /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="ta">The subject traversable</param>
    /// <returns>Mapped monad</returns>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Arr<Seq<A>> Sequence<A>(this Seq<Arr<A>> ta) =>
        ta.Traverse(identity);
        
    /// <summary>
    /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
    /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="ta">The subject traversable</param>
    /// <returns>Mapped monad</returns>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Arr<Set<A>> Sequence<A>(this Set<Arr<A>> ta) =>
        ta.Traverse(identity);
        
    /// <summary>
    /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
    /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="ta">The subject traversable</param>
    /// <returns>Mapped monad</returns>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Arr<HashSet<A>> Sequence<A>(this HashSet<Arr<A>> ta) =>
        ta.Traverse(identity);
        
    /// <summary>
    /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
    /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="ta">The subject traversable</param>
    /// <returns>Mapped monad</returns>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Arr<Validation<FAIL, A>> Sequence<FAIL, A>(this Validation<FAIL, Arr<A>> ta) => 
        ta.Traverse(identity);
        
    /// <summary>
    /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
    /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="ta">The subject traversable</param>
    /// <returns>Mapped monad</returns>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Arr<Validation<MonoidFail, FAIL, A>> Sequence<MonoidFail, FAIL, A>(this Validation<MonoidFail, FAIL, Arr<A>> ta)
        where MonoidFail : Monoid<FAIL>, Eq<FAIL> =>
        ta.Traverse(identity);

    /// <summary>
    /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
    /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="ta">The subject traversable</param>
    /// <returns>Mapped monad</returns>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Arr<Eff<A>> Sequence<A>(this Eff<Arr<A>> ta) =>
        ta.Traverse(identity);
}
