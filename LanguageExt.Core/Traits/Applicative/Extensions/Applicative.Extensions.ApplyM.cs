using System;
using System.Diagnostics.Contracts;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class ApplicativeExtensions
{
    extension<M, A, B>((K<M, A>, K<M, B>) items) 
        where M : Monad<M>
    {
        [Pure]
        public K<M, R> ApplyM<R>(Func<A, B, K<M, R>> f) =>
            items.Apply(f).Flatten();
    }

    extension<M, A, B, C>((K<M, A>, K<M, B>, K<M, C>) items) 
        where M : Monad<M>
    {
        [Pure]
        public K<M, R> ApplyM<R>(Func<A, B, C, K<M, R>> f) =>
            items.Apply(f).Flatten();
    }

    extension<M, A, B, C, D>((K<M, A>, K<M, B>, K<M, C>, K<M, D>) items) 
        where M : Monad<M>
    {
        [Pure]
        public K<M, R> ApplyM<R>(Func<A, B, C, D, K<M, R>> f) =>
            items.Apply(f).Flatten();
    }

    extension<M, A, B, C, D, E>((K<M, A>, K<M, B>, K<M, C>, K<M, D>, K<M, E>) items) 
        where M : Monad<M>
    {
        [Pure]
        public K<M, R> ApplyM<R>(Func<A, B, C, D, E, K<M, R>> f) =>
            items.Apply(f).Flatten();
    }

    extension<M, A, B, C, D, E, F>((K<M, A>, K<M, B>, K<M, C>, K<M, D>, K<M, E>, K<M, F>) items) 
        where M : Monad<M>
    {
        [Pure]
        public K<M, R> ApplyM<R>(Func<A, B, C, D, E, F, K<M, R>> f) =>
            items.Apply(f).Flatten();
    }

    extension<M, A, B, C, D, E, F, G>((K<M, A>, K<M, B>, K<M, C>, K<M, D>, K<M, E>, K<M, F>, K<M, G>) items) 
        where M : Monad<M>
    {
        [Pure]
        public K<M, R> ApplyM<R>(Func<A, B, C, D, E, F, G, K<M, R>> f) =>
            items.Apply(f).Flatten();
    }

    extension<M, A, B, C, D, E, F, G, H>((K<M, A>, K<M, B>, K<M, C>, K<M, D>, K<M, E>, K<M, F>, K<M, G>, K<M, H>) items) 
        where M : Monad<M>
    {
        [Pure]
        public K<M, R> ApplyM<R>(Func<A, B, C, D, E, F, G, H, K<M, R>> f) =>
            items.Apply(f).Flatten();
    }

    extension<M, A, B, C, D, E, F, G, H, I>((K<M, A>, K<M, B>, K<M, C>, K<M, D>, K<M, E>, K<M, F>, K<M, G>, K<M, H>, K<M, I>) items) 
        where M : Monad<M>
    {
        [Pure]
        public K<M, R> ApplyM<R>(Func<A, B, C, D, E, F, G, H, I, K<M, R>> f) =>
            items.Apply(f).Flatten();
    }

    extension<M, A, B, C, D, E, F, G, H, I, J>((K<M, A>, K<M, B>, K<M, C>, K<M, D>, K<M, E>, K<M, F>, K<M, G>, K<M, H>, K<M, I>, K<M, J>) items) 
        where M : Monad<M>
    {
        [Pure]
        public K<M, R> ApplyM<R>(Func<A, B, C, D, E, F, G, H, I, J, K<M, R>> f) =>
            items.Apply(f).Flatten();
    }
}
