using System;
using System.Runtime.CompilerServices;
using LanguageExt.Traits;

namespace LanguageExt;

public partial class FoldableExtensions
{
    internal const string FoldChangesMessage =
        "Foldable extensions have been updated to be consisent across the library.  There is a replacement extension " +
        "method with the same name as this one, but with the arguments in a different order.  The `S initialState` " +
        "first argument now comes after the fold function and any predicates. *Very often the last argument*. " +
        "Also, the overloads that supported curried folding functions, like `Func<S, Func<A, S>>`, have been removed," +
        "so use the non-curried versions (you can use `uncurry` if needed). And finally, the `Fold*Back` functions " +
        "that had the reversed folding function arguments: `<A, S, S>`, are now consistent: `<S, A, S>`. Nowhere near "+
        " as funny, but at least now consistent";
   
    extension<T, A>(K<T, A> ta)
        where T : Foldable<T>
    {
        [Obsolete(FoldChangesMessage)]
        [OverloadResolutionPriority(Change.Priority)]
        public S FoldMaybe<S>(
            S initialState,
            Func<S, Func<A, Option<S>>> f) =>
            T.FoldMaybe((s, a) => f(s)(a), initialState, ta);

        [Obsolete(FoldChangesMessage)]
        [OverloadResolutionPriority(Change.Priority)]
        public S FoldMaybe<S>(
            S initialState,
            Func<S, A, Option<S>> f) =>
            T.FoldMaybe(f, initialState, ta);

        [Obsolete(FoldChangesMessage)]
        [OverloadResolutionPriority(Change.Priority)]
        public S FoldWhile<S>(
            S initialState,
            Func<S, Func<A, S>> f,
            Func<(S State, A Value), bool> predicate) =>
            T.FoldWhile((s, a) => f(s)(a), predicate, initialState, ta);

        [Obsolete(FoldChangesMessage)]
        [OverloadResolutionPriority(Change.Priority)]
        public S FoldWhile<S>(
            S initialState,
            Func<S, A, S> f,
            Func<(S State, A Value), bool> predicate) =>
            T.FoldWhile(f, predicate, initialState, ta);

        [Obsolete(FoldChangesMessage)]
        [OverloadResolutionPriority(Change.Priority)]
        public K<M, S> FoldWhileM<M, S>(
            S initialState,
            Func<S, Func<A, K<M, S>>> f,
            Func<(S State, A Value), bool> predicate)
            where M : Monad<M> =>
            T.FoldWhileM<K<M, S>, M, A, S>((s, a) => f(s)(a), predicate, initialState, ta);

        [Obsolete(FoldChangesMessage)]
        [OverloadResolutionPriority(Change.Priority)]
        public K<M, S> FoldWhileM<M, S>(
            S initialState,
            Func<S, A, K<M, S>> f,
            Func<(S State, A Value), bool> predicate)
            where M : Monad<M> =>
            T.FoldWhileM<K<M, S>, M, A, S>(f, predicate, initialState, ta);

        [Obsolete(FoldChangesMessage)]
        [OverloadResolutionPriority(Change.Priority)]
        public S FoldUntil<S>(
            S initialState,
            Func<S, Func<A, S>> f,
            Func<(S State, A Value), bool> predicate) =>
            T.FoldUntil((s, a) => f(s)(a), predicate, initialState, ta);

        [Obsolete(FoldChangesMessage)]
        [OverloadResolutionPriority(Change.Priority)]
        public S FoldUntil<S>(
            S initialState,
            Func<S, A, S> f,
            Func<(S State, A Value), bool> predicate) =>
            T.FoldUntil(f, predicate, initialState, ta);

        [Obsolete(FoldChangesMessage)]
        [OverloadResolutionPriority(Change.Priority)]
        public K<M, S> FoldUntilM<M, S>(
            S initialState,
            Func<S, Func<A, K<M, S>>> f,
            Func<(S State, A Value), bool> predicate)
            where M : Monad<M> =>
            T.FoldUntilM<K<M, S>, M, A, S>((s, a) => f(s)(a), predicate, initialState, ta);

        [Obsolete(FoldChangesMessage)]
        [OverloadResolutionPriority(Change.Priority)]
        public K<M, S> FoldUntilM<M, S>(
            S initialState,
            Func<S, A, K<M, S>> f,
            Func<(S State, A Value), bool> predicate)
            where M : Monad<M> =>
            T.FoldUntilM<K<M, S>, M, A, S>(f, predicate, initialState, ta);

        [Obsolete(FoldChangesMessage)]
        [OverloadResolutionPriority(Change.Priority)]
        public S Fold<S>(
            S initialState,
            Func<S, Func<A, S>> f) =>
            T.Fold((s, a) => f(s)(a), initialState, ta);

        [Obsolete(FoldChangesMessage)]
        [OverloadResolutionPriority(Change.Priority)]
        public S Fold<S>(
            S initialState,
            Func<S, A, S> f) =>
            T.Fold(f, initialState, ta);

        [Obsolete(FoldChangesMessage)]
        [OverloadResolutionPriority(Change.Priority)]
        public K<M, S> FoldM<M, S>(
            S initialState,
            Func<S, A, K<M, S>> f)
            where M : Monad<M> =>
            T.FoldM<K<M, S>, M, A, S>(f, initialState, ta);
    }
}
