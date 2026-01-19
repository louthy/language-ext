using System;
using System.Numerics;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static partial class FoldableIOExtensions
{
    /// <param name="ta">FoldableIO structure</param>
    /// <typeparam name="A">Value type</typeparam>
    extension<T, A>(K<T, A> ta)
        where T : FoldableIO<T>
    {
        /// <summary>
        /// Fold until the `Option` returns `None`
        /// </summary>
        /// <param name="f">Fold function</param>
        /// <param name="initialState">Initial state for the fold</param>
        /// <typeparam name="S">State type</typeparam>
        /// <returns>Aggregated value</returns>
        public IO<S> FoldMaybeIO<S>(
            Func<S, A, Option<S>> f,
            S initialState) =>
            T.FoldMaybeIO(f, initialState, ta);

        /// <summary>
        /// Same behaviour as `Fold` but allows early exit of the operation once
        /// the predicate function becomes `false` for the state/value pair 
        /// </summary>
        public IO<S> FoldWhileIO<S>(
            Func<S, A, S> f,
            Func<(S State, A Value), bool> predicate,
            S initialState) =>
            T.FoldWhileIO(f, predicate, initialState, ta);

        /// <summary>
        /// Same behaviour as `Fold` but the fold operation returns a monadic type and allows
        /// early exit of the operation once the predicate function becomes `false` for the
        /// state/value pair 
        /// </summary>
        public K<M, S> FoldWhileM<M, S>(
            Func<S, A, K<M, S>> f,
            Func<(S State, A Value), bool> predicate,
            S initialState)
            where M : MonadIO<M> =>
            T.FoldWhileM<K<M, S>, M, A, S>(f, predicate, initialState, ta);

        /// <summary>
        /// Same behaviour as `Fold` but allows early exit of the operation once
        /// the predicate function becomes `false` for the state/value pair
        /// </summary>
        public IO<S> FoldUntilIO<S>(
            Func<S, A, S> f,
            Func<(S State, A Value), bool> predicate,
            S initialState) =>
            T.FoldUntilIO(f, predicate, initialState, ta);

        /// <summary>
        /// Same behaviour as `Fold` but the fold operation returns a monadic type and allows
        /// early exit of the operation once the predicate function becomes `false` for the
        /// state/value pair 
        /// </summary>
        public K<M, S> FoldUntilM<M, S>(
            Func<S, A, K<M, S>> f,
            Func<(S State, A Value), bool> predicate,
            S initialState)
            where M : MonadIO<M> =>
            T.FoldUntilM<K<M, S>, M, A, S>(f, predicate, initialState, ta);

        /// <summary>
        /// Right-associative fold of a structure, lazy in the accumulator.
        ///
        /// In the case of lists, 'Fold', when applied to a binary operator, a
        /// starting value (typically the right-identity of the operator), and a
        /// list, reduces the list using the binary operator, from right to left.
        /// </summary>
        public IO<S> FoldIO<S>(Func<S, A, S> f, S initialState) =>
            T.FoldIO(f, initialState, ta);

        /// <summary>
        /// Right-associative fold of a structure, lazy in the accumulator.
        ///
        /// In the case of lists, 'Fold', when applied to a binary operator, a
        /// starting value (typically the right-identity of the operator), and a
        /// list, reduces the list using the binary operator, from right to left.
        /// </summary>
        public K<M, S> FoldM<M, S>(Func<S, A, K<M, S>> f, S initialState)
            where M : MonadIO<M> =>
            T.FoldM<K<M, S>, M, A, S>(f, initialState, ta);

        /// <summary>
        /// List of elements of a structure, from left to right
        /// </summary>
        public IO<Seq<A>> ToSeqIO() =>
            T.ToSeqIO(ta);

        /// <summary>
        /// List of elements of a structure, from left to right
        /// </summary>
        public IO<Lst<A>> ToLstIO() =>
            T.ToLstIO(ta);

        /// <summary>
        /// List of elements of a structure, from left to right
        /// </summary>
        public IO<bool> IsEmptyIO =>
            T.IsEmptyIO(ta);

        /// <summary>
        /// Return the number of items in a FoldableIO structure
        /// </summary>
        public IO<long> CountIO =>
            T.CountIO(ta);

        /// <summary>
        /// Does an element that fits the predicate occur in the structure?
        /// </summary>
        public IO<bool> ExistsIO(Func<A, bool> predicate) =>
            T.ExistsIO(predicate, ta);

        /// <summary>
        /// Does the predicate hold for all elements in the structure?
        /// </summary>
        public IO<bool> ForAllIO(Func<A, bool> predicate) =>
            T.ForAllIO(predicate, ta);

        /// <summary>
        /// Does the element exist in the structure?
        /// </summary>
        public IO<bool> ContainsIO(A value) =>
            T.ContainsIO(value, ta);

        /// <summary>
        /// Find the first element that match the predicate
        /// </summary>
        public IO<Option<A>> FindIO(Func<A, bool> predicate) =>
            T.FindIO(predicate, ta);

        /// <summary>
        /// Get the head item in the FoldableIO or `None`
        /// </summary>
        public IO<Option<A>> HeadIO =>
            T.HeadIO(ta);

        /// <summary>
        /// Iterate over the structure from left to right, applying the monadic action to each element.
        /// </summary>
        public K<M, Unit> IterM<M, B>(Func<A, K<M, B>> f)
            where M : MonadIO<M> =>
            T.IterM<K<M, B>, M, A, B>(f, ta);

        /// <summary>
        /// Iterate over the structure from left to right, applying the action to each element.
        /// </summary>
        public IO<Unit> IterIO(Action<A> f) =>
            T.IterIO(f, ta);

        /// <summary>
        /// Iterate over the structure from left to right, applying the action to each element.
        /// </summary>
        public IO<Unit> IterIO(Action<long, A> f) =>
            T.IterIO(f, ta);
        
        /// <summary>
        /// Inject a value in between each item in the enumerable 
        /// </summary>
        /// <param name="sep">Item to inject</param>
        /// <returns>An iterable with the values injected</returns>
        public IteratorIO<A> IntersperseIO(A sep) =>
            T.IntersperseIO(sep, ta);
    }

    /// <param name="ta">FoldableIO structure</param>
    /// <typeparam name="A">Value type</typeparam>
    extension<T, M, A>(K<T, A> ta)
        where T : FoldableIO<T>
        where M : MonadIO<M>, Alternative<M>
    {
        /// <summary>
        /// Get the head item in the `FoldableIO` or `Alternative.Empty`
        /// </summary>
        public K<M, A> HeadM() => 
            T.HeadM<M, A>(ta);
    }

    /// <param name="ta">FoldableIO structure</param>
    /// <typeparam name="A">Value type</typeparam>
    extension<EqA, T, A>(K<T, A> ta)
        where T : FoldableIO<T>
        where EqA : Eq<A> 
    {
        /// <summary>
        /// Does the element exist in the structure?
        /// </summary>
        public IO<bool> ContainsIO(A value) =>
            T.ContainsIO<EqA, A>(value, ta);
    }
    
    /// <param name="ta">FoldableIO structure</param>
    /// <typeparam name="T">FoldableIO type</typeparam>
    /// <typeparam name="A">Bound values</typeparam>
    extension<T, A>(K<T, A> ta)
        where T : FoldableIO<T>
        where A : Monoid<A>
    {
        /// <summary>
        /// Given a structure with elements whose type is a `Monoid`, combine them
        /// via the monoid's `Append` operator.  This fold is right-associative and
        /// lazy in the accumulator.  When you need a strict left-associative fold,
        /// use `FoldMap` instead, with `identity` as the map.
        /// </summary>
        public IO<A> FoldIO() =>
            T.FoldIO((s, x) => s + x, A.Empty, ta);

        /// <summary>
        /// Given a structure with elements whose type is a `Monoid`, combine them
        /// via the monoid's `Append` operator.  This fold is right-associative and
        /// lazy in the accumulator.  When you need a strict left-associative fold,
        /// use `FoldMap` instead, with `identity` as the map.
        /// </summary>
        public IO<A> FoldWhileIO(Func<(A State, A Value), bool> predicate) =>
            T.FoldWhileIO((s, x) => s + x, predicate, A.Empty, ta);

        /// <summary>
        /// Given a structure with elements whose type is a `Monoid`, combine them
        /// via the monoid's `Append` operator.  This fold is right-associative and
        /// lazy in the accumulator.  When you need a strict left-associative fold,
        /// use `FoldMap` instead, with `identity` as the map.
        /// </summary>
        public IO<A> FoldUntilIO(Func<(A State, A Value), bool> predicate) =>
            T.FoldUntilIO((s, x) => s + x, predicate, A.Empty, ta);
    }

    /// <param name="ta">FoldableIO structure</param>
    /// <typeparam name="T">FoldableIO type</typeparam>
    /// <typeparam name="A">Bound values</typeparam>
    extension<T, A>(K<T, A> ta)
        where T : FoldableIO<T>
        where A : IAdditionOperators<A, A, A>, IAdditiveIdentity<A, A>
    {
        /// <summary>
        /// Find sum of all the values in the structure
        /// </summary>
        public IO<A> SumIO() =>
            T.FoldIO((s, x) => s + x, A.AdditiveIdentity, ta);
    }

    /// <param name="ta">FoldableIO structure</param>
    /// <typeparam name="T">FoldableIO type</typeparam>
    /// <typeparam name="A">Bound values</typeparam>
    extension<T, A>(K<T, A> ta)
        where T : FoldableIO<T>
        where A : IMultiplyOperators<A, A, A>, IMultiplicativeIdentity<A, A>
    {
        /// <summary>
        /// Find product of all the values in the structure
        /// </summary>
        public IO<A> ProductIO() =>
            T.FoldIO((s, x) => s * x, A.MultiplicativeIdentity, ta);
    }
}
