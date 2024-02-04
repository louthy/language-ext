#pragma warning disable CS0693 // Type parameter has the same name as the type parameter from outer type
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;

namespace LanguageExt;

public partial class ValidationT
{
    /// <summary>
    /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
    /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="ta">The subject traversable</param>
    /// <returns>Mapped monad</returns>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Validation<MonoidFail, Fail, Arr<A>> Sequence<MonoidFail, Fail, A>(this Arr<Validation<MonoidFail, Fail, A>> ta)
        where MonoidFail : Monoid<Fail>, Eq<Fail> =>
        ta.Traverse(identity);
        
    /// <summary>
    /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
    /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="ta">The subject traversable</param>
    /// <returns>Mapped monad</returns>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Validation<MonoidFail, Fail, Either<Fail, A>> Sequence<MonoidFail, Fail, A>(this Either<Fail, Validation<MonoidFail, Fail, A>> ta)
        where MonoidFail : Monoid<Fail>, Eq<Fail> =>
        ta.Traverse(identity);
        
    /// <summary>
    /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
    /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="ta">The subject traversable</param>
    /// <returns>Mapped monad</returns>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Validation<MonoidFail, Fail, Identity<A>> Sequence<MonoidFail, Fail, A>(this Identity<Validation<MonoidFail, Fail, A>> ta)
        where MonoidFail : Monoid<Fail>, Eq<Fail> =>
        ta.Traverse(identity);

    /// <summary>
    /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
    /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="ta">The subject traversable</param>
    /// <returns>Mapped monad</returns>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Validation<MonoidFail, Fail, IEnumerable<A>> Sequence<MonoidFail, Fail, A>(this IEnumerable<Validation<MonoidFail, Fail, A>> ta)
        where MonoidFail : Monoid<Fail>, Eq<Fail> =>
        ta.Traverse(identity);
        
    /// <summary>
    /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
    /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="ta">The subject traversable</param>
    /// <returns>Mapped monad</returns>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Validation<MonoidFail, Fail, Lst<A>> Sequence<MonoidFail, Fail, A>(this Lst<Validation<MonoidFail, Fail, A>> ta)
        where MonoidFail : Monoid<Fail>, Eq<Fail> =>
        ta.Traverse(identity);
        
    /// <summary>
    /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
    /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="ta">The subject traversable</param>
    /// <returns>Mapped monad</returns>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Validation<MonoidFail, Fail, Fin<A>> Sequence<MonoidFail, Fail, A>(this Fin<Validation<MonoidFail, Fail, A>> ta)
        where MonoidFail : Monoid<Fail>, Eq<Fail> =>
        ta.Traverse(identity);
        
    /// <summary>
    /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
    /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="ta">The subject traversable</param>
    /// <returns>Mapped monad</returns>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Validation<MonoidFail, Fail, Option<A>> Sequence<MonoidFail, Fail, A>(this Option<Validation<MonoidFail, Fail, A>> ta)
        where MonoidFail : Monoid<Fail>, Eq<Fail> =>
        ta.Traverse(identity);
        
    /// <summary>
    /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
    /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="ta">The subject traversable</param>
    /// <returns>Mapped monad</returns>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Validation<MonoidFail, Fail, Seq<A>> Sequence<MonoidFail, Fail, A>(this Seq<Validation<MonoidFail, Fail, A>> ta)
        where MonoidFail : Monoid<Fail>, Eq<Fail> =>
        ta.Traverse(identity);
        
    /// <summary>
    /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
    /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="ta">The subject traversable</param>
    /// <returns>Mapped monad</returns>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Validation<MonoidFail, Fail, Set<A>> Sequence<MonoidFail, Fail, A>(this Set<Validation<MonoidFail, Fail, A>> ta)
        where MonoidFail : Monoid<Fail>, Eq<Fail> =>
        ta.Traverse(identity);
        
    /// <summary>
    /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
    /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="ta">The subject traversable</param>
    /// <returns>Mapped monad</returns>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Validation<MonoidFail, Fail, HashSet<A>> Sequence<MonoidFail, Fail, A>(this HashSet<Validation<MonoidFail, Fail, A>> ta)
        where MonoidFail : Monoid<Fail>, Eq<Fail> =>
        ta.Traverse(identity);
        
    /// <summary>
    /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
    /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="ta">The subject traversable</param>
    /// <returns>Mapped monad</returns>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Validation<MonoidFail, Fail, Validation<Fail, A>> Sequence<MonoidFail, Fail, A>(this Validation<Fail, Validation<MonoidFail, Fail, A>> ta) 
        where MonoidFail : Monoid<Fail>, Eq<Fail> =>
        ta.Traverse(identity);
        
    /// <summary>
    /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
    /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="ta">The subject traversable</param>
    /// <returns>Mapped monad</returns>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Validation<MonoidFail, Fail, Validation<MonoidFail, Fail, A>> Sequence<MonoidFail, Fail, A>(this Validation<MonoidFail, Fail, Validation<MonoidFail, Fail, A>> ta)
        where MonoidFail : Monoid<Fail>, Eq<Fail> =>
        ta.Traverse(identity);

    /// <summary>
    /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
    /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="ta">The subject traversable</param>
    /// <returns>Mapped monad</returns>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Validation<MonoidFail, Fail, Eff<A>> Sequence<MonoidFail, Fail, A>(this Eff<Validation<MonoidFail, Fail, A>> ta) 
        where MonoidFail : Monoid<Fail>, Eq<Fail> =>
        ta.Traverse(identity);
}
