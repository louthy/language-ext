/*
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    public static class ValidationSeqExtensions
    {
        /// <summary>
        /// Applicative apply
        /// </summary>
        /// <param name="mf">Lifted function</param>
        /// <param name="ma">Value to apply</param>
        /// <returns>Result of function applied to the value or Fail</returns>
        [Pure]
        public static Validation<FAIL, B> Apply<FAIL, A, B>(
            this Validation<FAIL, Func<A, B>> mf, 
            Validation<FAIL, A> ma) =>
            mf.Disjunction(ma).Map(_ => mf.SuccessValue(ma.SuccessValue));
        
        /// <summary>
        /// Applicative apply
        /// </summary>
        /// <param name="mf">Lifted function</param>
        /// <param name="ma">Value to apply</param>
        /// <param name="mb">Value to apply</param>
        /// <returns>Result of function applied to the values or Fail</returns>
        [Pure]
        public static Validation<FAIL, C> Apply<FAIL, A, B, C>(
            this Validation<FAIL, Func<A, B, C>> mf, 
            Validation<FAIL, A> ma, 
            Validation<FAIL, B> mb) =>
            mf.Disjunction(ma)
              .Disjunction(mb)
              .Map(_ => mf.SuccessValue(ma.SuccessValue, mb.SuccessValue));
        
        /// <summary>
        /// Applicative apply
        /// </summary>
        /// <param name="mf">Lifted function</param>
        /// <param name="ma">Value to apply</param>
        /// <param name="mb">Value to apply</param>
        /// <returns>Result of function applied to the values or Fail</returns>
        [Pure]
        public static Validation<FAIL, Func<B, C>> Apply<FAIL, A, B, C>(
            this Validation<FAIL, Func<A, B, C>> mf, 
            Validation<FAIL, A> ma) =>
            mf.Disjunction(ma)
              .Map(_ => curry(mf.SuccessValue)(ma.SuccessValue));

        /// <summary>
        /// Applicative apply
        /// </summary>
        /// <param name="mf">Lifted function</param>
        /// <param name="ma">Value to apply</param>
        /// <returns>Result of function applied to the value or Fail</returns>
        [Pure]
        public static Validation<FAIL, B> Apply<FAIL, A, B>(
            this Func<A, B> mf, 
            Validation<FAIL, A> ma) =>
            ma.Map(_ => mf(ma.SuccessValue));
        
        /// <summary>
        /// Applicative apply
        /// </summary>
        /// <param name="mf">Lifted function</param>
        /// <param name="ma">Value to apply</param>
        /// <param name="mb">Value to apply</param>
        /// <returns>Result of function applied to the values or Fail</returns>
        [Pure]
        public static Validation<FAIL, C> Apply<FAIL, A, B, C>(
            this Func<A, B, C> mf, 
            Validation<FAIL, A> ma, 
            Validation<FAIL, B> mb) =>
            ma.Disjunction(mb)
              .Map(_ => mf(ma.SuccessValue, mb.SuccessValue));
        
        /// <summary>
        /// Applicative apply
        /// </summary>
        /// <param name="mf">Lifted function</param>
        /// <param name="ma">Value to apply</param>
        /// <param name="mb">Value to apply</param>
        /// <returns>Result of function applied to the values or Fail</returns>
        [Pure]
        public static Validation<FAIL, Func<B, C>> Apply<FAIL, A, B, C>(
            this Func<A, B, C> mf, 
            Validation<FAIL, A> ma) =>
            ma.Map(_ => curry(mf)(ma.SuccessValue));
        
        /// <summary>
        /// Applicative apply
        /// </summary>
        /// <param name="mf">Lifted function</param>
        /// <param name="ma">Value to apply</param>
        /// <returns>Result of function applied to the value or Fail</returns>
        [Pure]
        public static Validation<FAIL, B> ApplyM<FAIL, A, B>(
            this Validation<FAIL, Func<A, Validation<FAIL, B>>> mf, 
            Validation<FAIL, A> ma) =>
            mf.Disjunction(ma).Bind(_ => mf.SuccessValue(ma.SuccessValue));
        
        /// <summary>
        /// Applicative apply
        /// </summary>
        /// <param name="mf">Lifted function</param>
        /// <param name="ma">Value to apply</param>
        /// <param name="mb">Value to apply</param>
        /// <returns>Result of function applied to the values or Fail</returns>
        [Pure]
        public static Validation<FAIL, C> ApplyM<FAIL, A, B, C>(
            this Validation<FAIL, Func<A, B, Validation<FAIL, C>>> mf, 
            Validation<FAIL, A> ma, 
            Validation<FAIL, B> mb) =>
            mf.Disjunction(ma)
              .Disjunction(mb)
              .Bind(_ => mf.SuccessValue(ma.SuccessValue, mb.SuccessValue));
        
        /// <summary>
        /// Applicative apply
        /// </summary>
        /// <param name="mf">Lifted function</param>
        /// <param name="ma">Value to apply</param>
        /// <param name="mb">Value to apply</param>
        /// <returns>Result of function applied to the values or Fail</returns>
        [Pure]
        public static Validation<FAIL, Func<B, Validation<FAIL, C>>> ApplyM<FAIL, A, B, C>(
            this Validation<FAIL, Func<A, B, Validation<FAIL, C>>> mf, 
            Validation<FAIL, A> ma) =>
            mf.Disjunction(ma)
              .Map(_ => curry(mf.SuccessValue)(ma.SuccessValue));

        /// <summary>
        /// Applicative apply
        /// </summary>
        /// <param name="mf">Lifted function</param>
        /// <param name="ma">Value to apply</param>
        /// <returns>Result of function applied to the value or Fail</returns>
        [Pure]
        public static Validation<FAIL, B> ApplyM<FAIL, A, B>(
            this Func<A, Validation<FAIL, B>> mf, 
            Validation<FAIL, A> ma) =>
            ma.Bind(_ => mf(ma.SuccessValue));
        
        /// <summary>
        /// Applicative apply
        /// </summary>
        /// <param name="mf">Lifted function</param>
        /// <param name="ma">Value to apply</param>
        /// <param name="mb">Value to apply</param>
        /// <returns>Result of function applied to the values or Fail</returns>
        [Pure]
        public static Validation<FAIL, C> ApplyM<FAIL, A, B, C>(
            this Func<A, B, Validation<FAIL, C>> mf, 
            Validation<FAIL, A> ma, 
            Validation<FAIL, B> mb) =>
            ma.Disjunction(mb)
              .Bind(_ => mf(ma.SuccessValue, mb.SuccessValue));

        /// <summary>
        /// Applicative apply
        /// </summary>
        /// <param name="mf">Lifted function</param>
        /// <param name="ma">Value to apply</param>
        /// <param name="mb">Value to apply</param>
        /// <returns>Result of function applied to the values or Fail</returns>
        [Pure]
        public static Validation<FAIL, Func<B, Validation<FAIL, C>>> ApplyM<FAIL, A, B, C>(
            this Func<A, B, Validation<FAIL, C>> mf,
            Validation<FAIL, A> ma) =>
            ma.Map(_ => curry(mf)(ma.SuccessValue));
        
        /// <summary>
        /// Flatten the nested Validation type
        /// </summary>
        [Pure]
        public static Validation<FAIL, SUCCESS> Flatten<FAIL, SUCCESS>(this Validation<FAIL, Validation<FAIL, SUCCESS>> self) =>
            self.Bind(identity);

        /// <summary>
        /// Extract only the successes 
        /// </summary>
        /// <param name="vs">Enumerable of validations</param>
        /// <typeparam name="F">Fail type</typeparam>
        /// <typeparam name="S">Success type</typeparam>
        /// <returns>Enumerable of successes</returns>
        [Pure]
        public static IEnumerable<S> Successes<F, S>(this IEnumerable<Validation<F, S>> vs)
        {
            foreach(var v in vs)
            {
                if(v.IsSuccess) yield return (S)v;
            }
        }

        /// <summary>
        /// Extract only the failures 
        /// </summary>
        /// <param name="vs">Enumerable of validations</param>
        /// <typeparam name="F">Fail type</typeparam>
        /// <typeparam name="S">Success type</typeparam>
        /// <returns>Enumerable of failures</returns>
        [Pure]
        public static IEnumerable<F> Fails<F, S>(this IEnumerable<Validation<F, S>> vs)
        {
            foreach(var v in vs)
            {
                if (v.IsFail)
                {
                    var fs = (Seq<F>)v;
                    foreach (var f in fs)
                    {
                        yield return f;
                    }
                }
            }
        }

        /// <summary>
        /// Extract only the successes 
        /// </summary>
        /// <param name="vs">Seq of validations</param>
        /// <typeparam name="F">Fail type</typeparam>
        /// <typeparam name="S">Success type</typeparam>
        /// <returns>Enumerable of successes</returns>
        [Pure]
        public static Seq<S> Successes<F, S>(this Seq<Validation<F, S>> vs) =>
            toSeq(Successes(vs.AsEnumerable()));

        /// <summary>
        /// Extract only the failures 
        /// </summary>
        /// <param name="vs">Seq of validations</param>
        /// <typeparam name="F">Fail type</typeparam>
        /// <typeparam name="S">Success type</typeparam>
        /// <returns>Enumerable of failures</returns>
        [Pure]
        public static Seq<F> Fails<F, S>(this Seq<Validation<F, S>> vs) =>
            toSeq(Fails(vs.AsEnumerable()));

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
*/
