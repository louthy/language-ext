using System;
using LanguageExt.TypeClasses;

namespace LanguageExt
{
    public static partial class Prelude
    {
        /// <summary>
        /// Represents a successful operation
        /// </summary>
        /// <typeparam name="ERROR">Error type</typeparam>
        /// <typeparam name="A">Value type</typeparam>
        /// <param name="value">Value</param>
        /// <returns>Validation applicative</returns>
        public static Validation<ERROR, A> Success<ERROR, A>(A value) =>
            Validation<ERROR, A>.Success(value);

        /// <summary>
        /// Represents a successful operation
        /// </summary>
        /// <typeparam name="MonoidError">Monoid for collecting the errors</typeparam>
        /// <typeparam name="ERROR">Error type</typeparam>
        /// <typeparam name="A">Value type</typeparam>
        /// <param name="value">Value</param>
        /// <returns>Validation applicative</returns>
        public static Validation<MonoidError, ERROR, A> Success<MonoidError, ERROR, A>(A value)
            where MonoidError : struct, Monoid<ERROR>, Eq<ERROR> =>
            Validation<MonoidError, ERROR, A>.Success(value);

        /// <summary>
        /// Represents a failed operation
        /// </summary>
        /// <typeparam name="ERROR">Error type</typeparam>
        /// <typeparam name="A">Value type</typeparam>
        /// <param name="value">Error value</param>
        /// <returns>Validation applicative</returns>
        public static Validation<ERROR, A> Fail<ERROR, A>(ERROR value) =>
            Validation<ERROR, A>.Fail(Seq1(value));

        /// <summary>
        /// Represents a failed operation
        /// </summary>
        /// <typeparam name="ERROR">Error type</typeparam>
        /// <typeparam name="A">Value type</typeparam>
        /// <param name="value">Error value</param>
        /// <returns>Validation applicative</returns>
        public static Validation<ERROR, A> Fail<ERROR, A>(Seq<ERROR> values) =>
            Validation<ERROR, A>.Fail(values);

        /// <summary>
        /// Represents a failed operation
        /// </summary>
        /// <typeparam name="MonoidError">Monoid for collecting the errors</typeparam>
        /// <typeparam name="ERROR">Error type</typeparam>
        /// <typeparam name="A">Value type</typeparam>
        /// <param name="value">Error value</param>
        /// <returns>Validation applicative</returns>
        public static Validation<MonoidError, ERROR, A> Fail<MonoidError, ERROR, A>(ERROR value)
            where MonoidError : struct, Monoid<ERROR>, Eq<ERROR> =>
            Validation<MonoidError, ERROR, A>.Fail(value);

        public static Validation<MonoidFail, FAIL, R> Apply<MonoidFail, FAIL, A, R>(this ValueTuple<Validation<MonoidFail, FAIL, A>> items, Func<A, R> f)
            where MonoidFail : struct, Monoid<FAIL>, Eq<FAIL> =>
            items.Item1.Match(
                Succ: s => f(s),
                Fail: e => Validation<MonoidFail, FAIL, R>.Fail(e));

        public static Validation<MonoidFail, FAIL, R> Apply<MonoidFail, FAIL, A, B, R>(
            this ValueTuple<
                Validation<MonoidFail, FAIL, A>,
                Validation<MonoidFail, FAIL, B>> items,
            Func<A, B, R> f)
            where MonoidFail : struct, Monoid<FAIL>, Eq<FAIL> =>
            items.Item1
                 .Disjunction(items.Item2)
                 .Map(_ => f(items.Item1.SuccessValue, items.Item2.SuccessValue));

        public static Validation<MonoidFail, FAIL, R> Apply<MonoidFail, FAIL, A, B, C, R>(
            this ValueTuple<
                Validation<MonoidFail, FAIL, A>,
                Validation<MonoidFail, FAIL, B>,
                Validation<MonoidFail, FAIL, C>
                > items,
            Func<A, B, C, R> f)
            where MonoidFail : struct, Monoid<FAIL>, Eq<FAIL> =>
            items.Item1
                 .Disjunction(items.Item2)
                 .Disjunction(items.Item3)
                 .Map(_ => f(
                     items.Item1.SuccessValue,
                     items.Item2.SuccessValue,
                     items.Item3.SuccessValue
                     ));

        public static Validation<MonoidFail, FAIL, R> Apply<MonoidFail, FAIL, A, B, C, D, R>(
            this ValueTuple<
                Validation<MonoidFail, FAIL, A>,
                Validation<MonoidFail, FAIL, B>,
                Validation<MonoidFail, FAIL, C>,
                Validation<MonoidFail, FAIL, D>
                > items,
            Func<A, B, C, D, R> f)
            where MonoidFail : struct, Monoid<FAIL>, Eq<FAIL> =>
            items.Item1
                 .Disjunction(items.Item2)
                 .Disjunction(items.Item3)
                 .Disjunction(items.Item4)
                 .Map(_ => f(
                     items.Item1.SuccessValue,
                     items.Item2.SuccessValue,
                     items.Item3.SuccessValue,
                     items.Item4.SuccessValue
                     ));

        public static Validation<MonoidFail, FAIL, R> Apply<MonoidFail, FAIL, A, B, C, D, E, R>(
            this ValueTuple<
                Validation<MonoidFail, FAIL, A>,
                Validation<MonoidFail, FAIL, B>,
                Validation<MonoidFail, FAIL, C>,
                Validation<MonoidFail, FAIL, D>,
                Validation<MonoidFail, FAIL, E>
                > items,
            Func<A, B, C, D, E, R> f)
            where MonoidFail : struct, Monoid<FAIL>, Eq<FAIL> =>
            items.Item1
                 .Disjunction(items.Item2)
                 .Disjunction(items.Item3)
                 .Disjunction(items.Item4)
                 .Disjunction(items.Item5)
                 .Map(_ => f(
                     items.Item1.SuccessValue,
                     items.Item2.SuccessValue,
                     items.Item3.SuccessValue,
                     items.Item4.SuccessValue,
                     items.Item5.SuccessValue
                     ));

        public static Validation<MonoidFail, FAIL, R> Apply<MonoidFail, FAIL, A, B, C, D, E, F, R>(
            this ValueTuple<
                Validation<MonoidFail, FAIL, A>,
                Validation<MonoidFail, FAIL, B>,
                Validation<MonoidFail, FAIL, C>,
                Validation<MonoidFail, FAIL, D>,
                Validation<MonoidFail, FAIL, E>,
                Validation<MonoidFail, FAIL, F>
                > items,
            Func<A, B, C, D, E, F, R> f)
            where MonoidFail : struct, Monoid<FAIL>, Eq<FAIL> =>
            items.Item1
                 .Disjunction(items.Item2)
                 .Disjunction(items.Item3)
                 .Disjunction(items.Item4)
                 .Disjunction(items.Item5)
                 .Disjunction(items.Item6)
                 .Map(_ => f(
                     items.Item1.SuccessValue,
                     items.Item2.SuccessValue,
                     items.Item3.SuccessValue,
                     items.Item4.SuccessValue,
                     items.Item5.SuccessValue,
                     items.Item6.SuccessValue
                     ));

        public static Validation<MonoidFail, FAIL, R> Apply<MonoidFail, FAIL, A, B, C, D, E, F, G, R>(
            this ValueTuple<
                Validation<MonoidFail, FAIL, A>,
                Validation<MonoidFail, FAIL, B>,
                Validation<MonoidFail, FAIL, C>,
                Validation<MonoidFail, FAIL, D>,
                Validation<MonoidFail, FAIL, E>,
                Validation<MonoidFail, FAIL, F>,
                Validation<MonoidFail, FAIL, G>
                > items,
            Func<A, B, C, D, E, F, G, R> f)
            where MonoidFail : struct, Monoid<FAIL>, Eq<FAIL> =>
            items.Item1
                 .Disjunction(items.Item2)
                 .Disjunction(items.Item3)
                 .Disjunction(items.Item4)
                 .Disjunction(items.Item5)
                 .Disjunction(items.Item6)
                 .Disjunction(items.Item7)
                 .Map(_ => f(
                     items.Item1.SuccessValue,
                     items.Item2.SuccessValue,
                     items.Item3.SuccessValue,
                     items.Item4.SuccessValue,
                     items.Item5.SuccessValue,
                     items.Item6.SuccessValue,
                     items.Item7.SuccessValue
                     ));



        public static Validation<FAIL, R> Apply<FAIL, A, B, R>(
            this ValueTuple<
                Validation<FAIL, A>,
                Validation<FAIL, B>> items,
            Func<A, B, R> f) =>
            items.Item1
                 .Disjunction(items.Item2)
                 .Map(_ => f(items.Item1.SuccessValue, items.Item2.SuccessValue));

        public static Validation<FAIL, R> Apply<FAIL, A, B, C, R>(
            this ValueTuple<
                Validation<FAIL, A>,
                Validation<FAIL, B>,
                Validation<FAIL, C>
                > items,
            Func<A, B, C, R> f) =>
            items.Item1
                 .Disjunction(items.Item2)
                 .Disjunction(items.Item3)
                 .Map(_ => f(
                     items.Item1.SuccessValue,
                     items.Item2.SuccessValue,
                     items.Item3.SuccessValue
                     ));

        public static Validation<FAIL, R> Apply<FAIL, A, B, C, D, R>(
            this ValueTuple<
                Validation<FAIL, A>,
                Validation<FAIL, B>,
                Validation<FAIL, C>,
                Validation<FAIL, D>
                > items,
            Func<A, B, C, D, R> f) =>
            items.Item1
                 .Disjunction(items.Item2)
                 .Disjunction(items.Item3)
                 .Disjunction(items.Item4)
                 .Map(_ => f(
                     items.Item1.SuccessValue,
                     items.Item2.SuccessValue,
                     items.Item3.SuccessValue,
                     items.Item4.SuccessValue
                     ));

        public static Validation<FAIL, R> Apply<FAIL, A, B, C, D, E, R>(
            this ValueTuple<
                Validation<FAIL, A>,
                Validation<FAIL, B>,
                Validation<FAIL, C>,
                Validation<FAIL, D>,
                Validation<FAIL, E>
                > items,
            Func<A, B, C, D, E, R> f) =>
            items.Item1
                 .Disjunction(items.Item2)
                 .Disjunction(items.Item3)
                 .Disjunction(items.Item4)
                 .Disjunction(items.Item5)
                 .Map(_ => f(
                     items.Item1.SuccessValue,
                     items.Item2.SuccessValue,
                     items.Item3.SuccessValue,
                     items.Item4.SuccessValue,
                     items.Item5.SuccessValue
                     ));

        public static Validation<FAIL, R> Apply<FAIL, A, B, C, D, E, F, R>(
            this ValueTuple<
                Validation<FAIL, A>,
                Validation<FAIL, B>,
                Validation<FAIL, C>,
                Validation<FAIL, D>,
                Validation<FAIL, E>,
                Validation<FAIL, F>
                > items,
            Func<A, B, C, D, E, F, R> f) =>
            items.Item1
                 .Disjunction(items.Item2)
                 .Disjunction(items.Item3)
                 .Disjunction(items.Item4)
                 .Disjunction(items.Item5)
                 .Disjunction(items.Item6)
                 .Map(_ => f(
                     items.Item1.SuccessValue,
                     items.Item2.SuccessValue,
                     items.Item3.SuccessValue,
                     items.Item4.SuccessValue,
                     items.Item5.SuccessValue,
                     items.Item6.SuccessValue
                     ));

        public static Validation<FAIL, R> Apply<FAIL, A, B, C, D, E, F, G, R>(
            this ValueTuple<
                Validation<FAIL, A>,
                Validation<FAIL, B>,
                Validation<FAIL, C>,
                Validation<FAIL, D>,
                Validation<FAIL, E>,
                Validation<FAIL, F>,
                Validation<FAIL, G>
                > items,
            Func<A, B, C, D, E, F, G, R> f) =>
            items.Item1
                 .Disjunction(items.Item2)
                 .Disjunction(items.Item3)
                 .Disjunction(items.Item4)
                 .Disjunction(items.Item5)
                 .Disjunction(items.Item6)
                 .Disjunction(items.Item7)
                 .Map(_ => f(
                     items.Item1.SuccessValue,
                     items.Item2.SuccessValue,
                     items.Item3.SuccessValue,
                     items.Item4.SuccessValue,
                     items.Item5.SuccessValue,
                     items.Item6.SuccessValue,
                     items.Item7.SuccessValue
                     ));

        public static Validation<FAIL, R> Apply<FAIL, A, B, C, D, E, F, G, H, R>(
            this (
                Validation<FAIL, A>,
                Validation<FAIL, B>,
                Validation<FAIL, C>,
                Validation<FAIL, D>,
                Validation<FAIL, E>,
                Validation<FAIL, F>,
                Validation<FAIL, G>,
                Validation<FAIL, H>
                ) items,
            Func<A, B, C, D, E, F, G, H, R> f) =>
            items.Item1
                 .Disjunction(items.Item2)
                 .Disjunction(items.Item3)
                 .Disjunction(items.Item4)
                 .Disjunction(items.Item5)
                 .Disjunction(items.Item6)
                 .Disjunction(items.Item7)
                 .Disjunction(items.Item8)
                 .Map(_ => f(
                     items.Item1.SuccessValue,
                     items.Item2.SuccessValue,
                     items.Item3.SuccessValue,
                     items.Item4.SuccessValue,
                     items.Item5.SuccessValue,
                     items.Item6.SuccessValue,
                     items.Item7.SuccessValue,
                     items.Item8.SuccessValue
                     ));

        public static Validation<FAIL, R> Apply<FAIL, A, B, C, D, E, F, G, H, I, R>(
            this (
                Validation<FAIL, A>,
                Validation<FAIL, B>,
                Validation<FAIL, C>,
                Validation<FAIL, D>,
                Validation<FAIL, E>,
                Validation<FAIL, F>,
                Validation<FAIL, G>,
                Validation<FAIL, H>,
                Validation<FAIL, I>
                ) items,
            Func<A, B, C, D, E, F, G, H, I, R> f) =>
            items.Item1
                 .Disjunction(items.Item2)
                 .Disjunction(items.Item3)
                 .Disjunction(items.Item4)
                 .Disjunction(items.Item5)
                 .Disjunction(items.Item6)
                 .Disjunction(items.Item7)
                 .Disjunction(items.Item8)
                 .Disjunction(items.Item9)
                 .Map(_ => f(
                     items.Item1.SuccessValue,
                     items.Item2.SuccessValue,
                     items.Item3.SuccessValue,
                     items.Item4.SuccessValue,
                     items.Item5.SuccessValue,
                     items.Item6.SuccessValue,
                     items.Item7.SuccessValue,
                     items.Item8.SuccessValue,
                     items.Item9.SuccessValue
                     ));

        public static Validation<FAIL, R> Apply<FAIL, A, B, C, D, E, F, G, H, I, J, R>(
            this (
                Validation<FAIL, A>,
                Validation<FAIL, B>,
                Validation<FAIL, C>,
                Validation<FAIL, D>,
                Validation<FAIL, E>,
                Validation<FAIL, F>,
                Validation<FAIL, G>,
                Validation<FAIL, H>,
                Validation<FAIL, I>,
                Validation<FAIL, J>
                ) items,
            Func<A, B, C, D, E, F, G, H, I, J, R> f) =>
            items.Item1
                 .Disjunction(items.Item2)
                 .Disjunction(items.Item3)
                 .Disjunction(items.Item4)
                 .Disjunction(items.Item5)
                 .Disjunction(items.Item6)
                 .Disjunction(items.Item7)
                 .Disjunction(items.Item8)
                 .Disjunction(items.Item9)
                 .Disjunction(items.Item10)
                 .Map(_ => f(
                     items.Item1.SuccessValue,
                     items.Item2.SuccessValue,
                     items.Item3.SuccessValue,
                     items.Item4.SuccessValue,
                     items.Item5.SuccessValue,
                     items.Item6.SuccessValue,
                     items.Item7.SuccessValue,
                     items.Item8.SuccessValue,
                     items.Item9.SuccessValue,
                     items.Item10.SuccessValue
                     ));

        public static Validation<FAIL, R> Apply<FAIL, A, B, C, D, E, F, G, H, I, J, K, R>(
            this (
                Validation<FAIL, A>,
                Validation<FAIL, B>,
                Validation<FAIL, C>,
                Validation<FAIL, D>,
                Validation<FAIL, E>,
                Validation<FAIL, F>,
                Validation<FAIL, G>,
                Validation<FAIL, H>,
                Validation<FAIL, I>,
                Validation<FAIL, J>,
                Validation<FAIL, K>
                ) items,
            Func<A, B, C, D, E, F, G, H, I, J, K, R> f) =>
            items.Item1
                 .Disjunction(items.Item2)
                 .Disjunction(items.Item3)
                 .Disjunction(items.Item4)
                 .Disjunction(items.Item5)
                 .Disjunction(items.Item6)
                 .Disjunction(items.Item7)
                 .Disjunction(items.Item8)
                 .Disjunction(items.Item9)
                 .Disjunction(items.Item10)
                 .Disjunction(items.Item11)
                 .Map(_ => f(
                     items.Item1.SuccessValue,
                     items.Item2.SuccessValue,
                     items.Item3.SuccessValue,
                     items.Item4.SuccessValue,
                     items.Item5.SuccessValue,
                     items.Item6.SuccessValue,
                     items.Item7.SuccessValue,
                     items.Item8.SuccessValue,
                     items.Item9.SuccessValue,
                     items.Item10.SuccessValue,
                     items.Item11.SuccessValue
                     ));

        public static Validation<FAIL, R> Apply<FAIL, A, B, C, D, E, F, G, H, I, J, K, L, R>(
            this (
                Validation<FAIL, A>,
                Validation<FAIL, B>,
                Validation<FAIL, C>,
                Validation<FAIL, D>,
                Validation<FAIL, E>,
                Validation<FAIL, F>,
                Validation<FAIL, G>,
                Validation<FAIL, H>,
                Validation<FAIL, I>,
                Validation<FAIL, J>,
                Validation<FAIL, K>,
                Validation<FAIL, L>
                ) items,
            Func<A, B, C, D, E, F, G, H, I, J, K, L, R> f) =>
            items.Item1
                 .Disjunction(items.Item2)
                 .Disjunction(items.Item3)
                 .Disjunction(items.Item4)
                 .Disjunction(items.Item5)
                 .Disjunction(items.Item6)
                 .Disjunction(items.Item7)
                 .Disjunction(items.Item8)
                 .Disjunction(items.Item9)
                 .Disjunction(items.Item10)
                 .Disjunction(items.Item11)
                 .Disjunction(items.Item12)
                 .Map(_ => f(
                     items.Item1.SuccessValue,
                     items.Item2.SuccessValue,
                     items.Item3.SuccessValue,
                     items.Item4.SuccessValue,
                     items.Item5.SuccessValue,
                     items.Item6.SuccessValue,
                     items.Item7.SuccessValue,
                     items.Item8.SuccessValue,
                     items.Item9.SuccessValue,
                     items.Item10.SuccessValue,
                     items.Item11.SuccessValue,
                     items.Item12.SuccessValue
                     ));

        public static Validation<FAIL, R> Apply<FAIL, A, B, C, D, E, F, G, H, I, J, K, L, M, R>(
            this (
                Validation<FAIL, A>,
                Validation<FAIL, B>,
                Validation<FAIL, C>,
                Validation<FAIL, D>,
                Validation<FAIL, E>,
                Validation<FAIL, F>,
                Validation<FAIL, G>,
                Validation<FAIL, H>,
                Validation<FAIL, I>,
                Validation<FAIL, J>,
                Validation<FAIL, K>,
                Validation<FAIL, L>,
                Validation<FAIL, M>
                ) items,
            Func<A, B, C, D, E, F, G, H, I, J, K, L, M, R> f) =>
            items.Item1
                 .Disjunction(items.Item2)
                 .Disjunction(items.Item3)
                 .Disjunction(items.Item4)
                 .Disjunction(items.Item5)
                 .Disjunction(items.Item6)
                 .Disjunction(items.Item7)
                 .Disjunction(items.Item8)
                 .Disjunction(items.Item9)
                 .Disjunction(items.Item10)
                 .Disjunction(items.Item11)
                 .Disjunction(items.Item12)
                 .Disjunction(items.Item13)
                 .Map(_ => f(
                     items.Item1.SuccessValue,
                     items.Item2.SuccessValue,
                     items.Item3.SuccessValue,
                     items.Item4.SuccessValue,
                     items.Item5.SuccessValue,
                     items.Item6.SuccessValue,
                     items.Item7.SuccessValue,
                     items.Item8.SuccessValue,
                     items.Item9.SuccessValue,
                     items.Item10.SuccessValue,
                     items.Item11.SuccessValue,
                     items.Item12.SuccessValue,
                     items.Item13.SuccessValue
                     ));

        public static Validation<FAIL, R> Apply<FAIL, A, B, C, D, E, F, G, H, I, J, K, L, M, N, R>(
            this (
                Validation<FAIL, A>,
                Validation<FAIL, B>,
                Validation<FAIL, C>,
                Validation<FAIL, D>,
                Validation<FAIL, E>,
                Validation<FAIL, F>,
                Validation<FAIL, G>,
                Validation<FAIL, H>,
                Validation<FAIL, I>,
                Validation<FAIL, J>,
                Validation<FAIL, K>,
                Validation<FAIL, L>,
                Validation<FAIL, M>,
                Validation<FAIL, N>
                ) items,
            Func<A, B, C, D, E, F, G, H, I, J, K, L, M, N, R> f) =>
            items.Item1
                 .Disjunction(items.Item2)
                 .Disjunction(items.Item3)
                 .Disjunction(items.Item4)
                 .Disjunction(items.Item5)
                 .Disjunction(items.Item6)
                 .Disjunction(items.Item7)
                 .Disjunction(items.Item8)
                 .Disjunction(items.Item9)
                 .Disjunction(items.Item10)
                 .Disjunction(items.Item11)
                 .Disjunction(items.Item12)
                 .Disjunction(items.Item13)
                 .Disjunction(items.Item14)
                 .Map(_ => f(
                     items.Item1.SuccessValue,
                     items.Item2.SuccessValue,
                     items.Item3.SuccessValue,
                     items.Item4.SuccessValue,
                     items.Item5.SuccessValue,
                     items.Item6.SuccessValue,
                     items.Item7.SuccessValue,
                     items.Item8.SuccessValue,
                     items.Item9.SuccessValue,
                     items.Item10.SuccessValue,
                     items.Item11.SuccessValue,
                     items.Item12.SuccessValue,
                     items.Item13.SuccessValue,
                     items.Item14.SuccessValue
                     ));

        public static Validation<FAIL, R> Apply<FAIL, A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, R>(
            this (
                Validation<FAIL, A>,
                Validation<FAIL, B>,
                Validation<FAIL, C>,
                Validation<FAIL, D>,
                Validation<FAIL, E>,
                Validation<FAIL, F>,
                Validation<FAIL, G>,
                Validation<FAIL, H>,
                Validation<FAIL, I>,
                Validation<FAIL, J>,
                Validation<FAIL, K>,
                Validation<FAIL, L>,
                Validation<FAIL, M>,
                Validation<FAIL, N>,
                Validation<FAIL, O>
                ) items,
            Func<A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, R> f) =>
            items.Item1
                 .Disjunction(items.Item2)
                 .Disjunction(items.Item3)
                 .Disjunction(items.Item4)
                 .Disjunction(items.Item5)
                 .Disjunction(items.Item6)
                 .Disjunction(items.Item7)
                 .Disjunction(items.Item8)
                 .Disjunction(items.Item9)
                 .Disjunction(items.Item10)
                 .Disjunction(items.Item11)
                 .Disjunction(items.Item12)
                 .Disjunction(items.Item13)
                 .Disjunction(items.Item14)
                 .Disjunction(items.Item15)
                 .Map(_ => f(
                     items.Item1.SuccessValue,
                     items.Item2.SuccessValue,
                     items.Item3.SuccessValue,
                     items.Item4.SuccessValue,
                     items.Item5.SuccessValue,
                     items.Item6.SuccessValue,
                     items.Item7.SuccessValue,
                     items.Item8.SuccessValue,
                     items.Item9.SuccessValue,
                     items.Item10.SuccessValue,
                     items.Item11.SuccessValue,
                     items.Item12.SuccessValue,
                     items.Item13.SuccessValue,
                     items.Item14.SuccessValue,
                     items.Item15.SuccessValue
                     ));

        public static Validation<FAIL, R> Apply<FAIL, A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, R>(
            this (
                Validation<FAIL, A>,
                Validation<FAIL, B>,
                Validation<FAIL, C>,
                Validation<FAIL, D>,
                Validation<FAIL, E>,
                Validation<FAIL, F>,
                Validation<FAIL, G>,
                Validation<FAIL, H>,
                Validation<FAIL, I>,
                Validation<FAIL, J>,
                Validation<FAIL, K>,
                Validation<FAIL, L>,
                Validation<FAIL, M>,
                Validation<FAIL, N>,
                Validation<FAIL, O>,
                Validation<FAIL, P>
                ) items,
            Func<A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, R> f) =>
            items.Item1
                 .Disjunction(items.Item2)
                 .Disjunction(items.Item3)
                 .Disjunction(items.Item4)
                 .Disjunction(items.Item5)
                 .Disjunction(items.Item6)
                 .Disjunction(items.Item7)
                 .Disjunction(items.Item8)
                 .Disjunction(items.Item9)
                 .Disjunction(items.Item10)
                 .Disjunction(items.Item11)
                 .Disjunction(items.Item12)
                 .Disjunction(items.Item13)
                 .Disjunction(items.Item14)
                 .Disjunction(items.Item15)
                 .Disjunction(items.Item16)
                 .Map(_ => f(
                     items.Item1.SuccessValue,
                     items.Item2.SuccessValue,
                     items.Item3.SuccessValue,
                     items.Item4.SuccessValue,
                     items.Item5.SuccessValue,
                     items.Item6.SuccessValue,
                     items.Item7.SuccessValue,
                     items.Item8.SuccessValue,
                     items.Item9.SuccessValue,
                     items.Item10.SuccessValue,
                     items.Item11.SuccessValue,
                     items.Item12.SuccessValue,
                     items.Item13.SuccessValue,
                     items.Item14.SuccessValue,
                     items.Item15.SuccessValue,
                     items.Item16.SuccessValue
                     ));

        public static Validation<MonoidFail, FAIL, R> ApplyM<MonoidFail, FAIL, A, R>(this ValueTuple<Validation<MonoidFail, FAIL, A>> items, Func<A, Validation<MonoidFail, FAIL, R>> f)
            where MonoidFail : struct, Monoid<FAIL>, Eq<FAIL> =>
            items.Item1.Match(
                Succ: s => f(s),
                Fail: e => Validation<MonoidFail, FAIL, R>.Fail(e));

        public static Validation<MonoidFail, FAIL, R> ApplyM<MonoidFail, FAIL, A, B, R>(
            this ValueTuple<
                Validation<MonoidFail, FAIL, A>,
                Validation<MonoidFail, FAIL, B>> items,
            Func<A, B, Validation<MonoidFail, FAIL, R>> f)
            where MonoidFail : struct, Monoid<FAIL>, Eq<FAIL> =>
            items.Item1
                 .Disjunction(items.Item2)
                 .Bind(_ => f(items.Item1.SuccessValue, items.Item2.SuccessValue));

        public static Validation<MonoidFail, FAIL, R> ApplyM<MonoidFail, FAIL, A, B, C, R>(
            this ValueTuple<
                Validation<MonoidFail, FAIL, A>,
                Validation<MonoidFail, FAIL, B>,
                Validation<MonoidFail, FAIL, C>
                > items,
            Func<A, B, C, Validation<MonoidFail, FAIL, R>> f)
            where MonoidFail : struct, Monoid<FAIL>, Eq<FAIL> =>
            items.Item1
                 .Disjunction(items.Item2)
                 .Disjunction(items.Item3)
                 .Bind(_ => f(
                     items.Item1.SuccessValue,
                     items.Item2.SuccessValue,
                     items.Item3.SuccessValue
                     ));

        public static Validation<MonoidFail, FAIL, R> ApplyM<MonoidFail, FAIL, A, B, C, D, R>(
            this ValueTuple<
                Validation<MonoidFail, FAIL, A>,
                Validation<MonoidFail, FAIL, B>,
                Validation<MonoidFail, FAIL, C>,
                Validation<MonoidFail, FAIL, D>
                > items,
            Func<A, B, C, D, Validation<MonoidFail, FAIL, R>> f)
            where MonoidFail : struct, Monoid<FAIL>, Eq<FAIL> =>
            items.Item1
                 .Disjunction(items.Item2)
                 .Disjunction(items.Item3)
                 .Disjunction(items.Item4)
                 .Bind(_ => f(
                     items.Item1.SuccessValue,
                     items.Item2.SuccessValue,
                     items.Item3.SuccessValue,
                     items.Item4.SuccessValue
                     ));

        public static Validation<MonoidFail, FAIL, R> ApplyM<MonoidFail, FAIL, A, B, C, D, E, R>(
            this ValueTuple<
                Validation<MonoidFail, FAIL, A>,
                Validation<MonoidFail, FAIL, B>,
                Validation<MonoidFail, FAIL, C>,
                Validation<MonoidFail, FAIL, D>,
                Validation<MonoidFail, FAIL, E>
                > items,
            Func<A, B, C, D, E, Validation<MonoidFail, FAIL, R>> f)
            where MonoidFail : struct, Monoid<FAIL>, Eq<FAIL> =>
            items.Item1
                 .Disjunction(items.Item2)
                 .Disjunction(items.Item3)
                 .Disjunction(items.Item4)
                 .Disjunction(items.Item5)
                 .Bind(_ => f(
                     items.Item1.SuccessValue,
                     items.Item2.SuccessValue,
                     items.Item3.SuccessValue,
                     items.Item4.SuccessValue,
                     items.Item5.SuccessValue
                     ));

        public static Validation<MonoidFail, FAIL, R> ApplyM<MonoidFail, FAIL, A, B, C, D, E, F, R>(
            this ValueTuple<
                Validation<MonoidFail, FAIL, A>,
                Validation<MonoidFail, FAIL, B>,
                Validation<MonoidFail, FAIL, C>,
                Validation<MonoidFail, FAIL, D>,
                Validation<MonoidFail, FAIL, E>,
                Validation<MonoidFail, FAIL, F>
                > items,
            Func<A, B, C, D, E, F, Validation<MonoidFail, FAIL, R>> f)
            where MonoidFail : struct, Monoid<FAIL>, Eq<FAIL> =>
            items.Item1
                 .Disjunction(items.Item2)
                 .Disjunction(items.Item3)
                 .Disjunction(items.Item4)
                 .Disjunction(items.Item5)
                 .Disjunction(items.Item6)
                 .Bind(_ => f(
                     items.Item1.SuccessValue,
                     items.Item2.SuccessValue,
                     items.Item3.SuccessValue,
                     items.Item4.SuccessValue,
                     items.Item5.SuccessValue,
                     items.Item6.SuccessValue
                     ));

        public static Validation<MonoidFail, FAIL, R> ApplyM<MonoidFail, FAIL, A, B, C, D, E, F, G, R>(
            this ValueTuple<
                Validation<MonoidFail, FAIL, A>,
                Validation<MonoidFail, FAIL, B>,
                Validation<MonoidFail, FAIL, C>,
                Validation<MonoidFail, FAIL, D>,
                Validation<MonoidFail, FAIL, E>,
                Validation<MonoidFail, FAIL, F>,
                Validation<MonoidFail, FAIL, G>
                > items,
            Func<A, B, C, D, E, F, G, Validation<MonoidFail, FAIL, R>> f)
            where MonoidFail : struct, Monoid<FAIL>, Eq<FAIL> =>
            items.Item1
                 .Disjunction(items.Item2)
                 .Disjunction(items.Item3)
                 .Disjunction(items.Item4)
                 .Disjunction(items.Item5)
                 .Disjunction(items.Item6)
                 .Disjunction(items.Item7)
                 .Bind(_ => f(
                     items.Item1.SuccessValue,
                     items.Item2.SuccessValue,
                     items.Item3.SuccessValue,
                     items.Item4.SuccessValue,
                     items.Item5.SuccessValue,
                     items.Item6.SuccessValue,
                     items.Item7.SuccessValue
                     ));

        public static Validation<FAIL, R> ApplyM<FAIL, A, B, R>(
            this ValueTuple<
                Validation<FAIL, A>,
                Validation<FAIL, B>> items,
            Func<A, B, Validation<FAIL, R>> f) =>
            items.Item1
                 .Disjunction(items.Item2)
                 .Bind(_ => f(items.Item1.SuccessValue, items.Item2.SuccessValue));

        public static Validation<FAIL, R> ApplyM<FAIL, A, B, C, R>(
            this ValueTuple<
                Validation<FAIL, A>,
                Validation<FAIL, B>,
                Validation<FAIL, C>
                > items,
            Func<A, B, C, Validation<FAIL, R>> f) =>
            items.Item1
                 .Disjunction(items.Item2)
                 .Disjunction(items.Item3)
                 .Bind(_ => f(
                     items.Item1.SuccessValue,
                     items.Item2.SuccessValue,
                     items.Item3.SuccessValue
                     ));

        public static Validation<FAIL, R> ApplyM<FAIL, A, B, C, D, R>(
            this ValueTuple<
                Validation<FAIL, A>,
                Validation<FAIL, B>,
                Validation<FAIL, C>,
                Validation<FAIL, D>
                > items,
            Func<A, B, C, D, Validation<FAIL, R>> f) =>
            items.Item1
                 .Disjunction(items.Item2)
                 .Disjunction(items.Item3)
                 .Disjunction(items.Item4)
                 .Bind(_ => f(
                     items.Item1.SuccessValue,
                     items.Item2.SuccessValue,
                     items.Item3.SuccessValue,
                     items.Item4.SuccessValue
                     ));

        public static Validation<FAIL, R> ApplyM<FAIL, A, B, C, D, E, R>(
            this ValueTuple<
                Validation<FAIL, A>,
                Validation<FAIL, B>,
                Validation<FAIL, C>,
                Validation<FAIL, D>,
                Validation<FAIL, E>
                > items,
            Func<A, B, C, D, E, Validation<FAIL, R>> f) =>
            items.Item1
                 .Disjunction(items.Item2)
                 .Disjunction(items.Item3)
                 .Disjunction(items.Item4)
                 .Disjunction(items.Item5)
                 .Bind(_ => f(
                     items.Item1.SuccessValue,
                     items.Item2.SuccessValue,
                     items.Item3.SuccessValue,
                     items.Item4.SuccessValue,
                     items.Item5.SuccessValue
                     ));

        public static Validation<FAIL, R> ApplyM<FAIL, A, B, C, D, E, F, R>(
            this ValueTuple<
                Validation<FAIL, A>,
                Validation<FAIL, B>,
                Validation<FAIL, C>,
                Validation<FAIL, D>,
                Validation<FAIL, E>,
                Validation<FAIL, F>
                > items,
            Func<A, B, C, D, E, F, Validation<FAIL, R>> f) =>
            items.Item1
                 .Disjunction(items.Item2)
                 .Disjunction(items.Item3)
                 .Disjunction(items.Item4)
                 .Disjunction(items.Item5)
                 .Disjunction(items.Item6)
                 .Bind(_ => f(
                     items.Item1.SuccessValue,
                     items.Item2.SuccessValue,
                     items.Item3.SuccessValue,
                     items.Item4.SuccessValue,
                     items.Item5.SuccessValue,
                     items.Item6.SuccessValue
                     ));

        public static Validation<FAIL, R> ApplyM<FAIL, A, B, C, D, E, F, G, R>(
            this ValueTuple<
                Validation<FAIL, A>,
                Validation<FAIL, B>,
                Validation<FAIL, C>,
                Validation<FAIL, D>,
                Validation<FAIL, E>,
                Validation<FAIL, F>,
                Validation<FAIL, G>
                > items,
            Func<A, B, C, D, E, F, G, Validation<FAIL, R>> f) =>
            items.Item1
                 .Disjunction(items.Item2)
                 .Disjunction(items.Item3)
                 .Disjunction(items.Item4)
                 .Disjunction(items.Item5)
                 .Disjunction(items.Item6)
                 .Disjunction(items.Item7)
                 .Bind(_ => f(
                     items.Item1.SuccessValue,
                     items.Item2.SuccessValue,
                     items.Item3.SuccessValue,
                     items.Item4.SuccessValue,
                     items.Item5.SuccessValue,
                     items.Item6.SuccessValue,
                     items.Item7.SuccessValue
                     ));

        public static Validation<FAIL, R> ApplyM<FAIL, A, B, C, D, E, F, G, H, R>(
            this (
                Validation<FAIL, A>,
                Validation<FAIL, B>,
                Validation<FAIL, C>,
                Validation<FAIL, D>,
                Validation<FAIL, E>,
                Validation<FAIL, F>,
                Validation<FAIL, G>,
                Validation<FAIL, H>
                ) items,
            Func<A, B, C, D, E, F, G, H, Validation<FAIL, R>> f) =>
            items.Item1
                 .Disjunction(items.Item2)
                 .Disjunction(items.Item3)
                 .Disjunction(items.Item4)
                 .Disjunction(items.Item5)
                 .Disjunction(items.Item6)
                 .Disjunction(items.Item7)
                 .Disjunction(items.Item8)
                 .Bind(_ => f(
                     items.Item1.SuccessValue,
                     items.Item2.SuccessValue,
                     items.Item3.SuccessValue,
                     items.Item4.SuccessValue,
                     items.Item5.SuccessValue,
                     items.Item6.SuccessValue,
                     items.Item7.SuccessValue,
                     items.Item8.SuccessValue
                     ));

        public static Validation<FAIL, R> ApplyM<FAIL, A, B, C, D, E, F, G, H, I, R>(
            this (
                Validation<FAIL, A>,
                Validation<FAIL, B>,
                Validation<FAIL, C>,
                Validation<FAIL, D>,
                Validation<FAIL, E>,
                Validation<FAIL, F>,
                Validation<FAIL, G>,
                Validation<FAIL, H>,
                Validation<FAIL, I>
                ) items,
            Func<A, B, C, D, E, F, G, H, I, Validation<FAIL, R>> f) =>
            items.Item1
                 .Disjunction(items.Item2)
                 .Disjunction(items.Item3)
                 .Disjunction(items.Item4)
                 .Disjunction(items.Item5)
                 .Disjunction(items.Item6)
                 .Disjunction(items.Item7)
                 .Disjunction(items.Item8)
                 .Disjunction(items.Item9)
                 .Bind(_ => f(
                     items.Item1.SuccessValue,
                     items.Item2.SuccessValue,
                     items.Item3.SuccessValue,
                     items.Item4.SuccessValue,
                     items.Item5.SuccessValue,
                     items.Item6.SuccessValue,
                     items.Item7.SuccessValue,
                     items.Item8.SuccessValue,
                     items.Item9.SuccessValue
                     ));

        public static Validation<FAIL, R> ApplyM<FAIL, A, B, C, D, E, F, G, H, I, J, R>(
            this (
                Validation<FAIL, A>,
                Validation<FAIL, B>,
                Validation<FAIL, C>,
                Validation<FAIL, D>,
                Validation<FAIL, E>,
                Validation<FAIL, F>,
                Validation<FAIL, G>,
                Validation<FAIL, H>,
                Validation<FAIL, I>,
                Validation<FAIL, J>
                ) items,
            Func<A, B, C, D, E, F, G, H, I, J, Validation<FAIL, R>> f) =>
            items.Item1
                 .Disjunction(items.Item2)
                 .Disjunction(items.Item3)
                 .Disjunction(items.Item4)
                 .Disjunction(items.Item5)
                 .Disjunction(items.Item6)
                 .Disjunction(items.Item7)
                 .Disjunction(items.Item8)
                 .Disjunction(items.Item9)
                 .Disjunction(items.Item10)
                 .Bind(_ => f(
                     items.Item1.SuccessValue,
                     items.Item2.SuccessValue,
                     items.Item3.SuccessValue,
                     items.Item4.SuccessValue,
                     items.Item5.SuccessValue,
                     items.Item6.SuccessValue,
                     items.Item7.SuccessValue,
                     items.Item8.SuccessValue,
                     items.Item9.SuccessValue,
                     items.Item10.SuccessValue
                     ));

        public static Validation<FAIL, R> ApplyM<FAIL, A, B, C, D, E, F, G, H, I, J, K, R>(
            this (
                Validation<FAIL, A>,
                Validation<FAIL, B>,
                Validation<FAIL, C>,
                Validation<FAIL, D>,
                Validation<FAIL, E>,
                Validation<FAIL, F>,
                Validation<FAIL, G>,
                Validation<FAIL, H>,
                Validation<FAIL, I>,
                Validation<FAIL, J>,
                Validation<FAIL, K>
                ) items,
            Func<A, B, C, D, E, F, G, H, I, J, K, Validation<FAIL, R>> f) =>
            items.Item1
                 .Disjunction(items.Item2)
                 .Disjunction(items.Item3)
                 .Disjunction(items.Item4)
                 .Disjunction(items.Item5)
                 .Disjunction(items.Item6)
                 .Disjunction(items.Item7)
                 .Disjunction(items.Item8)
                 .Disjunction(items.Item9)
                 .Disjunction(items.Item10)
                 .Disjunction(items.Item11)
                 .Bind(_ => f(
                     items.Item1.SuccessValue,
                     items.Item2.SuccessValue,
                     items.Item3.SuccessValue,
                     items.Item4.SuccessValue,
                     items.Item5.SuccessValue,
                     items.Item6.SuccessValue,
                     items.Item7.SuccessValue,
                     items.Item8.SuccessValue,
                     items.Item9.SuccessValue,
                     items.Item10.SuccessValue,
                     items.Item11.SuccessValue
                     ));

        public static Validation<FAIL, R> ApplyM<FAIL, A, B, C, D, E, F, G, H, I, J, K, L, R>(
            this (
                Validation<FAIL, A>,
                Validation<FAIL, B>,
                Validation<FAIL, C>,
                Validation<FAIL, D>,
                Validation<FAIL, E>,
                Validation<FAIL, F>,
                Validation<FAIL, G>,
                Validation<FAIL, H>,
                Validation<FAIL, I>,
                Validation<FAIL, J>,
                Validation<FAIL, K>,
                Validation<FAIL, L>
                ) items,
            Func<A, B, C, D, E, F, G, H, I, J, K, L, Validation<FAIL, R>> f) =>
            items.Item1
                 .Disjunction(items.Item2)
                 .Disjunction(items.Item3)
                 .Disjunction(items.Item4)
                 .Disjunction(items.Item5)
                 .Disjunction(items.Item6)
                 .Disjunction(items.Item7)
                 .Disjunction(items.Item8)
                 .Disjunction(items.Item9)
                 .Disjunction(items.Item10)
                 .Disjunction(items.Item11)
                 .Disjunction(items.Item12)
                 .Bind(_ => f(
                     items.Item1.SuccessValue,
                     items.Item2.SuccessValue,
                     items.Item3.SuccessValue,
                     items.Item4.SuccessValue,
                     items.Item5.SuccessValue,
                     items.Item6.SuccessValue,
                     items.Item7.SuccessValue,
                     items.Item8.SuccessValue,
                     items.Item9.SuccessValue,
                     items.Item10.SuccessValue,
                     items.Item11.SuccessValue,
                     items.Item12.SuccessValue
                     ));

        public static Validation<FAIL, R> ApplyM<FAIL, A, B, C, D, E, F, G, H, I, J, K, L, M, R>(
            this (
                Validation<FAIL, A>,
                Validation<FAIL, B>,
                Validation<FAIL, C>,
                Validation<FAIL, D>,
                Validation<FAIL, E>,
                Validation<FAIL, F>,
                Validation<FAIL, G>,
                Validation<FAIL, H>,
                Validation<FAIL, I>,
                Validation<FAIL, J>,
                Validation<FAIL, K>,
                Validation<FAIL, L>,
                Validation<FAIL, M>
                ) items,
            Func<A, B, C, D, E, F, G, H, I, J, K, L, M, Validation<FAIL, R>> f) =>
            items.Item1
                 .Disjunction(items.Item2)
                 .Disjunction(items.Item3)
                 .Disjunction(items.Item4)
                 .Disjunction(items.Item5)
                 .Disjunction(items.Item6)
                 .Disjunction(items.Item7)
                 .Disjunction(items.Item8)
                 .Disjunction(items.Item9)
                 .Disjunction(items.Item10)
                 .Disjunction(items.Item11)
                 .Disjunction(items.Item12)
                 .Disjunction(items.Item13)
                 .Bind(_ => f(
                     items.Item1.SuccessValue,
                     items.Item2.SuccessValue,
                     items.Item3.SuccessValue,
                     items.Item4.SuccessValue,
                     items.Item5.SuccessValue,
                     items.Item6.SuccessValue,
                     items.Item7.SuccessValue,
                     items.Item8.SuccessValue,
                     items.Item9.SuccessValue,
                     items.Item10.SuccessValue,
                     items.Item11.SuccessValue,
                     items.Item12.SuccessValue,
                     items.Item13.SuccessValue
                     ));

        public static Validation<FAIL, R> ApplyM<FAIL, A, B, C, D, E, F, G, H, I, J, K, L, M, N, R>(
            this (
                Validation<FAIL, A>,
                Validation<FAIL, B>,
                Validation<FAIL, C>,
                Validation<FAIL, D>,
                Validation<FAIL, E>,
                Validation<FAIL, F>,
                Validation<FAIL, G>,
                Validation<FAIL, H>,
                Validation<FAIL, I>,
                Validation<FAIL, J>,
                Validation<FAIL, K>,
                Validation<FAIL, L>,
                Validation<FAIL, M>,
                Validation<FAIL, N>
                ) items,
            Func<A, B, C, D, E, F, G, H, I, J, K, L, M, N, Validation<FAIL, R>> f) =>
            items.Item1
                 .Disjunction(items.Item2)
                 .Disjunction(items.Item3)
                 .Disjunction(items.Item4)
                 .Disjunction(items.Item5)
                 .Disjunction(items.Item6)
                 .Disjunction(items.Item7)
                 .Disjunction(items.Item8)
                 .Disjunction(items.Item9)
                 .Disjunction(items.Item10)
                 .Disjunction(items.Item11)
                 .Disjunction(items.Item12)
                 .Disjunction(items.Item13)
                 .Disjunction(items.Item14)
                 .Bind(_ => f(
                     items.Item1.SuccessValue,
                     items.Item2.SuccessValue,
                     items.Item3.SuccessValue,
                     items.Item4.SuccessValue,
                     items.Item5.SuccessValue,
                     items.Item6.SuccessValue,
                     items.Item7.SuccessValue,
                     items.Item8.SuccessValue,
                     items.Item9.SuccessValue,
                     items.Item10.SuccessValue,
                     items.Item11.SuccessValue,
                     items.Item12.SuccessValue,
                     items.Item13.SuccessValue,
                     items.Item14.SuccessValue
                     ));

        public static Validation<FAIL, R> ApplyM<FAIL, A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, R>(
            this (
                Validation<FAIL, A>,
                Validation<FAIL, B>,
                Validation<FAIL, C>,
                Validation<FAIL, D>,
                Validation<FAIL, E>,
                Validation<FAIL, F>,
                Validation<FAIL, G>,
                Validation<FAIL, H>,
                Validation<FAIL, I>,
                Validation<FAIL, J>,
                Validation<FAIL, K>,
                Validation<FAIL, L>,
                Validation<FAIL, M>,
                Validation<FAIL, N>,
                Validation<FAIL, O>
                ) items,
            Func<A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, Validation<FAIL, R>> f) =>
            items.Item1
                 .Disjunction(items.Item2)
                 .Disjunction(items.Item3)
                 .Disjunction(items.Item4)
                 .Disjunction(items.Item5)
                 .Disjunction(items.Item6)
                 .Disjunction(items.Item7)
                 .Disjunction(items.Item8)
                 .Disjunction(items.Item9)
                 .Disjunction(items.Item10)
                 .Disjunction(items.Item11)
                 .Disjunction(items.Item12)
                 .Disjunction(items.Item13)
                 .Disjunction(items.Item14)
                 .Disjunction(items.Item15)
                 .Bind(_ => f(
                     items.Item1.SuccessValue,
                     items.Item2.SuccessValue,
                     items.Item3.SuccessValue,
                     items.Item4.SuccessValue,
                     items.Item5.SuccessValue,
                     items.Item6.SuccessValue,
                     items.Item7.SuccessValue,
                     items.Item8.SuccessValue,
                     items.Item9.SuccessValue,
                     items.Item10.SuccessValue,
                     items.Item11.SuccessValue,
                     items.Item12.SuccessValue,
                     items.Item13.SuccessValue,
                     items.Item14.SuccessValue,
                     items.Item15.SuccessValue
                     ));

        public static Validation<FAIL, R> ApplyM<FAIL, A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, R>(
            this (
                Validation<FAIL, A>,
                Validation<FAIL, B>,
                Validation<FAIL, C>,
                Validation<FAIL, D>,
                Validation<FAIL, E>,
                Validation<FAIL, F>,
                Validation<FAIL, G>,
                Validation<FAIL, H>,
                Validation<FAIL, I>,
                Validation<FAIL, J>,
                Validation<FAIL, K>,
                Validation<FAIL, L>,
                Validation<FAIL, M>,
                Validation<FAIL, N>,
                Validation<FAIL, O>,
                Validation<FAIL, P>
                ) items,
            Func<A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Validation<FAIL, R>> f) =>
            items.Item1
                 .Disjunction(items.Item2)
                 .Disjunction(items.Item3)
                 .Disjunction(items.Item4)
                 .Disjunction(items.Item5)
                 .Disjunction(items.Item6)
                 .Disjunction(items.Item7)
                 .Disjunction(items.Item8)
                 .Disjunction(items.Item9)
                 .Disjunction(items.Item10)
                 .Disjunction(items.Item11)
                 .Disjunction(items.Item12)
                 .Disjunction(items.Item13)
                 .Disjunction(items.Item14)
                 .Disjunction(items.Item15)
                 .Disjunction(items.Item16)
                 .Bind(_ => f(
                     items.Item1.SuccessValue,
                     items.Item2.SuccessValue,
                     items.Item3.SuccessValue,
                     items.Item4.SuccessValue,
                     items.Item5.SuccessValue,
                     items.Item6.SuccessValue,
                     items.Item7.SuccessValue,
                     items.Item8.SuccessValue,
                     items.Item9.SuccessValue,
                     items.Item10.SuccessValue,
                     items.Item11.SuccessValue,
                     items.Item12.SuccessValue,
                     items.Item13.SuccessValue,
                     items.Item14.SuccessValue,
                     items.Item15.SuccessValue,
                     items.Item16.SuccessValue
                     ));    
    }
}
