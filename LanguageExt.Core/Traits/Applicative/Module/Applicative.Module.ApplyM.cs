using System;
using System.Diagnostics.Contracts;

namespace LanguageExt.Traits;

public static partial class Applicative
{
    [Pure]
    public static K<M, R> applyM<M, A, B, R>((K<M, A>, K<M, B>) items, Func<A, B, K<M, R>> f) where M : Monad<M> =>
        items.Apply(f).Flatten();

    [Pure]
    public static K<M, R> applyM<M, A, B, C, R>((K<M, A>, K<M, B>, K<M, C>) items, Func<A, B, C, K<M, R>> f) where M : Monad<M> =>
        items.Apply(f).Flatten();

    [Pure]
    public static K<M, R> applyM<M, A, B, C, D, R>((K<M, A>, K<M, B>, K<M, C>, K<M, D>) items, Func<A, B, C, D, K<M, R>> f) where M : Monad<M> =>
        items.Apply(f).Flatten();

    [Pure]
    public static K<M, R> applyM<M, A, B, C, D, E, R>((K<M, A>, K<M, B>, K<M, C>, K<M, D>, K<M, E>) items, Func<A, B, C, D, E, K<M, R>> f) where M : Monad<M> =>
        items.Apply(f).Flatten();

    [Pure]
    public static K<M, R> applyM<M, A, B, C, D, E, F, R>((K<M, A>, K<M, B>, K<M, C>, K<M, D>, K<M, E>, K<M, F>) items, Func<A, B, C, D, E, F, K<M, R>> f) where M : Monad<M> =>
        items.Apply(f).Flatten();

    [Pure]
    public static K<M, R> applyM<M, A, B, C, D, E, F, G, R>((K<M, A>, K<M, B>, K<M, C>, K<M, D>, K<M, E>, K<M, F>, K<M, G>) items, Func<A, B, C, D, E, F, G, K<M, R>> f) where M : Monad<M> =>
        items.Apply(f).Flatten();

    [Pure]
    public static K<M, R> applyM<M, A, B, C, D, E, F, G, H, R>((K<M, A>, K<M, B>, K<M, C>, K<M, D>, K<M, E>, K<M, F>, K<M, G>, K<M, H>) items, Func<A, B, C, D, E, F, G, H, K<M, R>> f) where M : Monad<M> =>
        items.Apply(f).Flatten();

    [Pure]
    public static K<M, R> applyM<M, A, B, C, D, E, F, G, H, I, R>((K<M, A>, K<M, B>, K<M, C>, K<M, D>, K<M, E>, K<M, F>, K<M, G>, K<M, H>, K<M, I>) items, Func<A, B, C, D, E, F, G, H, I, K<M, R>> f) where M : Monad<M> =>
        items.Apply(f).Flatten();

    [Pure]
    public static K<M, R> applyM<M, A, B, C, D, E, F, G, H, I, J, R>((K<M, A>, K<M, B>, K<M, C>, K<M, D>, K<M, E>, K<M, F>, K<M, G>, K<M, H>, K<M, I>, K<M, J>) items, Func<A, B, C, D, E, F, G, H, I, J, K<M, R>> f) where M : Monad<M> =>
        items.Apply(f).Flatten();
}
