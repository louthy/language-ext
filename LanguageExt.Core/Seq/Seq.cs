using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt.TypeClasses;
using static LanguageExt.TypeClass;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;

namespace LanguageExt
{
    /// <summary>
    /// Sequence monad
    /// </summary>
    /// <typeparam name="A">Bound value</typeparam>
    public struct Seq<A> : Monad<A>, Traversable<A>
    {
        static readonly IEnumerable<A> empty = new A[0];

        internal readonly IEnumerable<A> value;
        static readonly Seq<A> failNoMessage = new Seq<A>(new A[0]);

        IEnumerable<A> Value => value ?? empty;

        /// <summary>
        /// Takes the value-type Option<A>
        /// </summary>
        internal Seq(IEnumerable<A> value)
        {
            this.value = value;
        }

        /// <summary>
        /// Convert to enumerable
        /// </summary>
        public IEnumerable<A> AsEnumerable() => 
            Value;

        /// <summary>
        /// Functor map operation
        /// </summary>
        /// <typeparam name="B">The type that f maps to</typeparam>
        /// <param name="ma">The functor</param>
        /// <param name="f">The operation to perform on the bound value</param>
        /// <returns>A mapped functor</returns>
        [Pure]
        public Functor<B> Map<B>(Functor<A> ma, Func<A, B> f) =>
            new Seq<B>(AsList(ma).Map(f));

        /// <summary>
        /// Monad return
        /// Constructs a Monad of A
        /// </summary>
        /// <param name="a">Value to bind</param>
        /// <returns>Monad of A</returns>
        [Pure]
        public Monad<A> Return(A x, params A[] xs) =>
            new Seq<A>(List.createRange(x.Cons(xs)));

        /// <summary>
        /// Monad return
        /// Constructs a Monad of A
        /// </summary>
        /// <param name="a">Value to bind</param>
        /// <returns>Monad of A</returns>
        [Pure]
        public Monad<A> Return(IEnumerable<A> xs) =>
            new Seq<A>(xs);

        /// <summary>
        /// Monad bind
        /// </summary>
        /// <typeparam name="B">Type the bind operation returns</typeparam>
        /// <param name="ma">Monad of A</param>
        /// <param name="f">Bind operation</param>
        /// <returns>Monad of B</returns>
        [Pure]
        public Monad<B> Bind<B>(Monad<A> ma, Func<A, Monad<B>> f) =>
            new Seq<B>(BindSeq(ma, f));

        /// <summary>
        /// Monad bind
        /// </summary>
        /// <typeparam name="B">Type the bind operation returns</typeparam>
        /// <param name="ma">Monad of A</param>
        /// <param name="f">Bind operation</param>
        /// <returns>Monad of B</returns>
        [Pure]
        public MB Bind<MB, B>(Monad<A> ma, Func<A, MB> f) where MB : struct, Monad<B> =>
            Return<MB, B>(BindSeq<MB, B>(ma, f));

        /// <summary>
        /// Monad fail
        /// </summary>
        /// <param name="err">Optional error message - not supported for SeqM</param>
        /// <returns>Monad of A (for SeqM this returns an empty sequence)</returns>
        [Pure]
        public Monad<A> Fail(Exception ex) =>
            new Seq<A>(List.empty<A>());

        /// <summary>
        /// Monad fail
        /// </summary>
        /// <param name="err">Optional error message - not supported for SeqM</param>
        /// <returns>Monad of A (for SeqM this returns an empty sequence)</returns>
        [Pure]
        public Monad<A> Fail<F>(F err = default(F)) =>
            new Seq<A>(List.empty<A>());

        /// <summary>
        /// Fold the bound value
        /// </summary>
        /// <typeparam name="S">Initial state type</typeparam>
        /// <param name="ma">Monad to fold</param>
        /// <param name="state">Initial state</param>
        /// <param name="f">Fold operation</param>
        /// <returns>Aggregated state</returns>
        public S Fold<S>(Foldable<A> ma, S state, Func<S, A, S> f) =>
            AsList(ma).Fold(state, f);

        /// <summary>
        /// Fold the bound value
        /// </summary>
        /// <typeparam name="S">Initial state type</typeparam>
        /// <param name="ma">Monad to fold</param>
        /// <param name="state">Initial state</param>
        /// <param name="f">Fold operation</param>
        /// <returns>Aggregated state</returns>
        public S FoldBack<S>(Foldable<A> ma, S state, Func<S, A, S> f) =>
            AsList(ma).FoldBack(state, f);

        public Monad<Traversable<B>> Traverse<B>(Traversable<A> ta, Func<A, Monad<B>> f) =>
            new Seq<Traversable<B>>(
                from x in AsList(ta)
                select (Traversable<B>)f(x));

        public Monad<Traversable<A>> Sequence(Traversable<A> ta) =>
            Traverse(ta, a => default(Seq<A>).Return(a));

        IEnumerable<B> BindSeq<MB, B>(Monad<A> ma, Func<A, MB> f) where MB : struct, Monad<B>
        {
            var xs = AsList(ma);
            foreach (var x in xs)
            {
                var b = f(x);
                foreach (var y in TypeClass.toSeq(b as Foldable<B>))
                {
                    yield return y;
                }
            }
        }

        IEnumerable<B> BindSeq<B>(Monad<A> ma, Func<A, Monad<B>> f)
        {
            var xs = AsList(ma);
            foreach (var x in xs)
            {
                var b = f(x);
                foreach (var y in TypeClass.toSeq(b as Foldable<B>))
                {
                    yield return y;
                }
            }
        }

        /// <summary>
        /// Seq cast from Seq
        /// </summary>
        [Pure]
        private static IEnumerable<A> AsList(Traversable<A> a) =>
            ((Seq<A>)a).Value;

        /// <summary>
        /// Seq cast from Functor
        /// </summary>
        [Pure]
        private static IEnumerable<A> AsList(Functor<A> a) =>
            ((Seq<A>)a).Value;

        /// <summary>
        /// Seq cast from Foldable
        /// </summary>
        [Pure]
        private static IEnumerable<A> AsList(Foldable<A> a) =>
            ((Seq<A>)a).Value;

        /// <summary>
        /// Seq cast from Foldable
        /// </summary>
        [Pure]
        private static IEnumerable<A> AsList(Monad<A> a) =>
            ((Seq<A>)a).Value;

    }
}
