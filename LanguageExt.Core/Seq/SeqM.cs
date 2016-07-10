using System;
using System.Collections;
using System.Linq;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using System.Collections.Generic;

namespace LanguageExt
{
    /// <summary>
    /// Sequence monad
    /// </summary>
    /// <typeparam name="A">Bound value</typeparam>
    public struct SeqM<A> : Monad<A>, Foldable<A>
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
        /// To sequence operation
        /// </summary>
        public IEnumerable<A> ToSeq(Seq<A> seq) =>
            ((SeqM<A>)seq).Value;

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
        /// Option cast from Seq
        /// </summary>
        [Pure]
        private static IEnumerable<A> AsList(Seq<A> a) =>
            ((SeqM<A>)a).Value ?? new A[0].AsEnumerable();

        /// <summary>
        /// Option cast from Functor
        /// </summary>
        [Pure]
        private static IEnumerable<A> AsList(Functor<A> a) =>
            ((SeqM<A>)a).Value ?? new A[0].AsEnumerable();

        /// <summary>
        /// Option cast from Foldable
        /// </summary>
        [Pure]
        private static IEnumerable<A> AsList(Foldable<A> a) =>
            ((SeqM<A>)a).Value ?? new A[0].AsEnumerable();

        /// <summary>
        /// Monad return
        /// 
        /// Constructs a Monad of A
        /// </summary>
        /// <param name="a">Value to bind</param>
        /// <returns>Monad of A</returns>
        [Pure]
        public Monad<A> Return(A a) =>
            new SeqM<A>(List(a));

        /// <summary>
        /// Monad return
        /// 
        /// Constructs a Monad of A
        /// </summary>
        /// <param name="a">Value to bind</param>
        /// <returns>Monad of A</returns>
        [Pure]
        public Monad<A> Return(IEnumerable<A> a) =>
            new SeqM<A>(a);

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
        public Applicative<A> Pure(IEnumerable<A> a) =>
            new SeqM<A>(a);

        /// <summary>
        /// Applicative pure
        /// 
        /// Constructs an Applicative of A
        /// </summary>
        /// <param name="a">Value to bind</param>
        /// <returns>Applicative of A</returns>
        [Pure]
        public Applicative<A> Pure(A a) =>
            new SeqM<A>(List(a));

        /// <summary>
        /// Apply y to x
        /// </summary>
        [Pure]
        public Applicative<B> Apply<B>(Applicative<Func<A, B>> x, Applicative<A> y) =>
            from a in x
            from b in y
            select a(b);

        /// <summary>
        /// Apply y and z to x
        /// </summary>
        [Pure]
        public Applicative<C> Apply<B, C>(Applicative<Func<A, B, C>> x, Applicative<A> y, Applicative<B> z) =>
            from a in x
            from b in y
            from c in z
            select a(b, c);

        /// <summary>
        /// Apply y to x
        /// </summary>
        [Pure]
        public Applicative<Func<B, C>> Apply<B, C>(Applicative<Func<A, Func<B, C>>> x, Applicative<A> y) =>
            from a in x
            from b in y
            select a(b);

        /// <summary>
        /// Apply x, then y, ignoring the result of x
        /// </summary>
        [Pure]
        public Applicative<B> Action<B>(Applicative<A> x, Applicative<B> y) =>
            from a in x
            from b in y
            select b;

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

        IEnumerable<B> BindSeq<B>(Monad<A> ma, Func<A, Monad<B>> f)
        {
            var xs = AsList(ma);
            foreach (var x in xs)
            {
                var b = f(x);
                foreach (var y in b.ToSeq(b))
                {
                    yield return y;
                }
            }
        }

        IEnumerable<B> BindSeq<B>(Applicative<A> ma, Func<A, Applicative<B>> f)
        {
            var xs = AsList(ma);
            foreach (var x in xs)
            {
                var b = f(x);
                foreach (var y in b.ToSeq(b))
                {
                    yield return y;
                }
            }
        }
    }
}
