using System;
using System.Diagnostics.Contracts;

namespace LanguageExt.Traits;

public static partial class Applicative
{
    [Pure]
    public static K<M, R> applyM<M, A, B, R>((Memo<M, A>, Memo<M, B>) items, Func<A, B, K<M, R>> f)
        where M : Monad<M> =>
        items.Apply(f).Flatten();

    [Pure]
    public static K<M, R> applyM<M, A, B, C, R>((Memo<M, A>, Memo<M, B>, Memo<M, C>) items, Func<A, B, C, K<M, R>> f)
        where M : Monad<M> =>
        items.Apply(f).Flatten();

    [Pure]
    public static K<M, R> applyM<M, A, B, C, D, R>((Memo<M, A>, Memo<M, B>, Memo<M, C>, Memo<M, D>) items,
                                                   Func<A, B, C, D, K<M, R>> f) where M : Monad<M> =>
        items.Apply(f).Flatten();

    [Pure]
    public static K<M, R> applyM<M, A, B, C, D, E, R>(
        (Memo<M, A>, Memo<M, B>, Memo<M, C>, Memo<M, D>, Memo<M, E>) items, Func<A, B, C, D, E, K<M, R>> f)
        where M : Monad<M> =>
        items.Apply(f).Flatten();

    [Pure]
    public static K<M, R> applyM<M, A, B, C, D, E, F, R>(
        (Memo<M, A>, Memo<M, B>, Memo<M, C>, Memo<M, D>, Memo<M, E>, Memo<M, F>) items,
        Func<A, B, C, D, E, F, K<M, R>> f)
        where M : Monad<M> =>
        items.Apply(f).Flatten();

    [Pure]
    public static K<M, R> applyM<M, A, B, C, D, E, F, G, R>(
        (Memo<M, A>, Memo<M, B>, Memo<M, C>, Memo<M, D>, Memo<M, E>, Memo<M, F>, Memo<M, G>) items,
        Func<A, B, C, D, E, F, G, K<M, R>> f) where M : Monad<M> =>
        items.Apply(f).Flatten();

    [Pure]
    public static K<M, R> applyM<M, A, B, C, D, E, F, G, H, R>(
        (Memo<M, A>, Memo<M, B>, Memo<M, C>, Memo<M, D>, Memo<M, E>, Memo<M, F>, Memo<M, G>, Memo<M, H>) items,
        Func<A, B, C, D, E, F, G, H, K<M, R>> f) where M : Monad<M> =>
        items.Apply(f).Flatten();

    [Pure]
    public static K<M, R> applyM<M, A, B, C, D, E, F, G, H, I, R>(
        (Memo<M, A>, Memo<M, B>, Memo<M, C>, Memo<M, D>, Memo<M, E>, Memo<M, F>, Memo<M, G>, Memo<M, H>, Memo<M, I>)
            items,
        Func<A, B, C, D, E, F, G, H, I, K<M, R>> f) where M : Monad<M> =>
        items.Apply(f).Flatten();

    [Pure]
    public static K<M, R> applyM<M, A, B, C, D, E, F, G, H, I, J, R>(
        (Memo<M, A>, Memo<M, B>, Memo<M, C>, Memo<M, D>, Memo<M, E>, Memo<M, F>, Memo<M, G>, Memo<M, H>, Memo<M, I>,
            Memo<M, J>) items, Func<A, B, C, D, E, F, G, H, I, J, K<M, R>> f) where M : Monad<M> =>
        items.Apply(f).Flatten();
}
