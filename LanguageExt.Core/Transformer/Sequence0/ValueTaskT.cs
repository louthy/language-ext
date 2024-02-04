using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;

namespace LanguageExt;

public partial class ValueTaskT
{
    /// <summary>
    /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
    /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="ta">The subject traversable</param>
    /// <returns>Mapped monad</returns>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueTask<Arr<A>> Sequence<A>(this Arr<ValueTask<A>> ta) =>
        ta.Traverse(identity);
        
    /// <summary>
    /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
    /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="ta">The subject traversable</param>
    /// <returns>Mapped monad</returns>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueTask<Either<L, A>> Sequence<L, A>(this Either<L, ValueTask<A>> ta) =>
        ta.Traverse(identity);
        
    /// <summary>
    /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
    /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="ta">The subject traversable</param>
    /// <returns>Mapped monad</returns>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueTask<Identity<A>> Sequence<A>(this Identity<ValueTask<A>> ta) =>
        ta.Traverse(identity);
 
    /// <summary>
    /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
    /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="ta">The subject traversable</param>
    /// <returns>Mapped monad</returns>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueTask<ValueTask<A>> Sequence<A>(this ValueTask<ValueTask<A>> ta) =>
        ta.Traverse(identity);
        
    /// <summary>
    /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
    /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="ta">The subject traversable</param>
    /// <returns>Mapped monad</returns>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueTask<Fin<A>> Sequence<A>(this Fin<ValueTask<A>> ta) =>
        ta.Traverse(identity);
        
    /// <summary>
    /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
    /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="ta">The subject traversable</param>
    /// <returns>Mapped monad</returns>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueTask<Option<A>> Sequence<A>(this Option<ValueTask<A>> ta) =>
        ta.Traverse(identity);
        
    /// <summary>
    /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
    /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="ta">The subject traversable</param>
    /// <returns>Mapped monad</returns>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueTask<Set<A>> Sequence<A>(this Set<ValueTask<A>> ta) =>
        ta.Traverse(identity);
        
    /// <summary>
    /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
    /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="ta">The subject traversable</param>
    /// <returns>Mapped monad</returns>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueTask<Validation<FAIL, A>> Sequence<FAIL, A>(this Validation<FAIL, ValueTask<A>> ta) => 
        ta.Traverse(identity);
        
    /// <summary>
    /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
    /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="ta">The subject traversable</param>
    /// <returns>Mapped monad</returns>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueTask<Validation<MonoidFail, FAIL, A>> Sequence<MonoidFail, FAIL, A>(this Validation<MonoidFail, FAIL, ValueTask<A>> ta)
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
    public static ValueTask<Eff<A>> Sequence<A>(this Eff<ValueTask<A>> ta) =>
        ta.Traverse(identity);
}
