using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;

namespace LanguageExt;

public partial class TaskT
{
    /// <summary>
    /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
    /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="ta">The subject traversable</param>
    /// <returns>Mapped monad</returns>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<Arr<A>> Sequence<A>(this Arr<Task<A>> ta) =>
        ta.Traverse(identity);
        
    /// <summary>
    /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
    /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="ta">The subject traversable</param>
    /// <returns>Mapped monad</returns>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<Either<L, A>> Sequence<L, A>(this Either<L, Task<A>> ta) =>
        ta.Traverse(identity);
        
    /// <summary>
    /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
    /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="ta">The subject traversable</param>
    /// <returns>Mapped monad</returns>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<Identity<A>> Sequence<A>(this Identity<Task<A>> ta) =>
        ta.Traverse(identity);
 
    /// <summary>
    /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
    /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="ta">The subject traversable</param>
    /// <returns>Mapped monad</returns>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<Task<A>> Sequence<A>(this Task<Task<A>> ta) =>
        ta.Traverse(identity);
        
    /// <summary>
    /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
    /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="ta">The subject traversable</param>
    /// <returns>Mapped monad</returns>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<Fin<A>> Sequence<A>(this Fin<Task<A>> ta) =>
        ta.Traverse(identity);
        
    /// <summary>
    /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
    /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="ta">The subject traversable</param>
    /// <returns>Mapped monad</returns>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<Option<A>> Sequence<A>(this Option<Task<A>> ta) =>
        ta.Traverse(identity);
        
    /// <summary>
    /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
    /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="ta">The subject traversable</param>
    /// <returns>Mapped monad</returns>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<Set<A>> Sequence<A>(this Set<Task<A>> ta) =>
        ta.Traverse(identity);
        
    /// <summary>
    /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
    /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="ta">The subject traversable</param>
    /// <returns>Mapped monad</returns>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<Validation<FAIL, A>> Sequence<FAIL, A>(this Validation<FAIL, Task<A>> ta) => 
        ta.Traverse(identity);
        
    /// <summary>
    /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
    /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="ta">The subject traversable</param>
    /// <returns>Mapped monad</returns>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<Validation<MonoidFail, FAIL, A>> Sequence<MonoidFail, FAIL, A>(this Validation<MonoidFail, FAIL, Task<A>> ta)
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
    public static Task<Eff<A>> Sequence<A>(this Eff<Task<A>> ta) =>
        ta.Traverse(identity);
}
