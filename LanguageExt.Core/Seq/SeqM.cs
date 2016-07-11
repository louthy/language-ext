using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;

namespace LanguageExt
{
    /// <summary>
    /// Sequence monad
    /// </summary>
    /// <typeparam name="A">Bound value</typeparam>
    public struct SeqM<A> : Monad<A>, Traversable<A>
    {
        internal readonly IEnumerable<A> Value;
        static readonly SeqM<A> failNoMessage = new SeqM<A>(new A[0]);

        /// <summary>
        /// Takes the value-type Option<A>
        /// </summary>
        internal SeqM(IEnumerable<A> value)
        {
            Value = value;
        }

        /// <summary>
        /// Functor map operation
        /// </summary>
        /// <typeparam name="B">The type that f maps to</typeparam>
        /// <param name="ma">The functor</param>
        /// <param name="f">The operation to perform on the bound value</param>
        /// <returns>A mapped functor</returns>
        [Pure]
        public Functor<B> Map<B>(Functor<A> ma, Func<A, B> f) =>
            new SeqM<B>(AsList(ma).Map(f));

        /// <summary>
        /// Seq cast from Seq
        /// </summary>
        [Pure]
        private static IEnumerable<A> AsList(Traversable<A> a) =>
            ((SeqM<A>)a).Value ?? new A[0].AsEnumerable();

        /// <summary>
        /// Seq cast from Functor
        /// </summary>
        [Pure]
        private static IEnumerable<A> AsList(Functor<A> a) =>
            ((SeqM<A>)a).Value ?? new A[0].AsEnumerable();

        /// <summary>
        /// Seq cast from Foldable
        /// </summary>
        [Pure]
        private static IEnumerable<A> AsList(Foldable<A> a) =>
            ((SeqM<A>)a).Value ?? new A[0].AsEnumerable();

        /// <summary>
        /// Seq cast from Foldable
        /// </summary>
        [Pure]
        private static IEnumerable<A> AsList(Monad<A> a) =>
            ((SeqM<A>)a).Value ?? new A[0].AsEnumerable();

        /// <summary>
        /// Monad return
        /// 
        /// Constructs a Monad of A
        /// </summary>
        /// <param name="a">Value to bind</param>
        /// <returns>Monad of A</returns>
        [Pure]
        public Monad<A> Return(A x, params A[] xs) =>
            new SeqM<A>(List.createRange(x.Cons(xs)));

        /// <summary>
        /// Monad bind
        /// </summary>
        /// <typeparam name="B">Type the bind operation returns</typeparam>
        /// <param name="ma">Monad of A</param>
        /// <param name="f">Bind operation</param>
        /// <returns>Monad of B</returns>
        [Pure]
        public Monad<B> Bind<B>(Monad<A> ma, Func<A, Monad<B>> f) =>
            new SeqM<B>(BindSeq(ma, f));

        /// <summary>
        /// Monad fail
        /// </summary>
        /// <param name="err">Optional error message - not supported for Option</param>
        /// <returns>Monad of A (for Option this returns a None state)</returns>
        [Pure]
        public Monad<A> Fail(string err = "") =>
            failNoMessage;

        /// <summary>
        /// Applicative pure
        /// 
        /// Constructs an Applicative of A
        /// </summary>
        /// <param name="a">Value to bind</param>
        /// <returns>Applicative of A</returns>
        [Pure]
        public Applicative<A> Pure(A x, params A[] xs) =>
            new SeqM<A>(List.createRange(x.Cons(xs)));

        /// <summary>
        /// Applicative bind
        /// </summary>
        /// <typeparam name="B">The type of the bind result</typeparam>
        /// <param name="ma">Applicative of A</param>
        /// <param name="f">Bind operation to perform</param>
        /// <returns>Applicative of B</returns>
        [Pure]
        public Applicative<B> Bind<B>(Applicative<A> ma, Func<A, Applicative<B>> f) =>
            new SeqM<B>(BindSeq(ma, f));

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

        public Applicative<Traversable<B>> Traverse<B>(Traversable<A> ta, Func<A, Applicative<B>> f) =>
            new SeqM<Traversable<B>>(
                from x in AsList(ta)
                select (Traversable<B>)f(x));

        public Applicative<Traversable<A>> SequenceA(Traversable<A> ta) =>
            Traverse(ta, a => default(SeqM<A>).Pure(a));

        public Monad<Traversable<B>> Traverse<B>(Traversable<A> ta, Func<A, Monad<B>> f) =>
            new SeqM<Traversable<B>>(
                from x in AsList(ta)
                select (Traversable<B>)f(x));

        public Monad<Traversable<A>> Sequence(Traversable<A> ta) =>
            Traverse(ta, a => default(SeqM<A>).Return(a));

        IEnumerable<B> BindSeq<B>(Monad<A> ma, Func<A, Monad<B>> f)
        {
            var xs = AsList(ma);
            foreach (var x in xs)
            {
                var b = f(x);
                if (b is Foldable<B>) // TODO: Decide what to do when something isn't foldable
                {
                    foreach (var y in TypeClass.toSeq(b as Foldable<B>))
                    {
                        yield return y;
                    }
                }
            }
        }

        IEnumerable<B> BindSeq<B>(Applicative<A> ma, Func<A, Applicative<B>> f)
        {
            var xs = AsList(ma);
            foreach (var x in xs)
            {
                var b = f(x);
                if (b is Foldable<B>) // TODO: Decide what to do when something isn't foldable
                {
                    foreach (var y in TypeClass.toSeq(b as Foldable<B>))
                    {
                        yield return y;
                    }
                }
            }
        }
    }
}
