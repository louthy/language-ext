using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    public static class ValidationExtensions
    {
        /// <summary>
        /// Applicative apply
        /// </summary>
        /// <param name="mf">Lifted function</param>
        /// <param name="ma">Value to apply</param>
        /// <returns>Result of function applied to the value or Fail</returns>
        [Pure]
        public static Validation<MonoidFail, FAIL, B> Apply<MonoidFail, FAIL, A, B>(
            this Validation<MonoidFail, FAIL, Func<A, B>> mf, 
            Validation<MonoidFail, FAIL, A> ma) 
            where MonoidFail : Monoid<FAIL>, Eq<FAIL> =>
            mf.Disjunction(ma).Map(_ => mf.SuccessValue(ma.SuccessValue));
        
        /// <summary>
        /// Applicative apply
        /// </summary>
        /// <param name="mf">Lifted function</param>
        /// <param name="ma">Value to apply</param>
        /// <param name="mb">Value to apply</param>
        /// <returns>Result of function applied to the values or Fail</returns>
        [Pure]
        public static Validation<MonoidFail, FAIL, C> Apply<MonoidFail, FAIL, A, B, C>(
            this Validation<MonoidFail, FAIL, Func<A, B, C>> mf, 
            Validation<MonoidFail, FAIL, A> ma, 
            Validation<MonoidFail, FAIL, B> mb)
            where MonoidFail : Monoid<FAIL>, Eq<FAIL> =>
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
        public static Validation<MonoidFail, FAIL, Func<B, C>> Apply<MonoidFail, FAIL, A, B, C>(
            this Validation<MonoidFail, FAIL, Func<A, B, C>> mf, 
            Validation<MonoidFail, FAIL, A> ma) 
            where MonoidFail : Monoid<FAIL>, Eq<FAIL> =>
            mf.Disjunction(ma)
              .Map(_ => curry(mf.SuccessValue)(ma.SuccessValue));

        /// <summary>
        /// Applicative apply
        /// </summary>
        /// <param name="mf">Lifted function</param>
        /// <param name="ma">Value to apply</param>
        /// <returns>Result of function applied to the value or Fail</returns>
        [Pure]
        public static Validation<MonoidFail, FAIL, B> Apply<MonoidFail, FAIL, A, B>(
            this Func<A, B> mf, 
            Validation<MonoidFail, FAIL, A> ma) 
            where MonoidFail : Monoid<FAIL>, Eq<FAIL> =>
            ma.Map(mf);
        
        /// <summary>
        /// Applicative apply
        /// </summary>
        /// <param name="mf">Lifted function</param>
        /// <param name="ma">Value to apply</param>
        /// <param name="mb">Value to apply</param>
        /// <returns>Result of function applied to the values or Fail</returns>
        [Pure]
        public static Validation<MonoidFail, FAIL, C> Apply<MonoidFail, FAIL, A, B, C>(
            this  Func<A, B, C> mf, 
            Validation<MonoidFail, FAIL, A> ma, 
            Validation<MonoidFail, FAIL, B> mb)
            where MonoidFail : Monoid<FAIL>, Eq<FAIL> =>
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
        public static Validation<MonoidFail, FAIL, Func<B, C>> Apply<MonoidFail, FAIL, A, B, C>(
            this Func<A, B, C> mf, 
            Validation<MonoidFail, FAIL, A> ma) 
            where MonoidFail : Monoid<FAIL>, Eq<FAIL> =>
            ma.Map(a => curry(mf)(a));
        
        
        /// <summary>
        /// Applicative apply
        /// </summary>
        /// <param name="mf">Lifted function</param>
        /// <param name="ma">Value to apply</param>
        /// <returns>Result of function applied to the value or Fail</returns>
        [Pure]
        public static Validation<MonoidFail, FAIL, B> ApplyM<MonoidFail, FAIL, A, B>(
            this Validation<MonoidFail, FAIL, Func<A, Validation<MonoidFail, FAIL, B>>> mf, 
            Validation<MonoidFail, FAIL, A> ma) 
            where MonoidFail : Monoid<FAIL>, Eq<FAIL> =>
            mf.Disjunction(ma).Bind(_ => mf.SuccessValue(ma.SuccessValue));
        
        /// <summary>
        /// Applicative apply
        /// </summary>
        /// <param name="mf">Lifted function</param>
        /// <param name="ma">Value to apply</param>
        /// <param name="mb">Value to apply</param>
        /// <returns>Result of function applied to the values or Fail</returns>
        [Pure]
        public static Validation<MonoidFail, FAIL, C> ApplyM<MonoidFail, FAIL, A, B, C>(
            this Validation<MonoidFail, FAIL, Func<A, B, Validation<MonoidFail, FAIL, C>>> mf, 
            Validation<MonoidFail, FAIL, A> ma, 
            Validation<MonoidFail, FAIL, B> mb)
            where MonoidFail : Monoid<FAIL>, Eq<FAIL> =>
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
        public static Validation<MonoidFail, FAIL, Func<B, Validation<MonoidFail, FAIL, C>>> ApplyM<MonoidFail, FAIL, A, B, C>(
            this Validation<MonoidFail, FAIL, Func<A, B, Validation<MonoidFail, FAIL, C>>> mf, 
            Validation<MonoidFail, FAIL, A> ma) 
            where MonoidFail : Monoid<FAIL>, Eq<FAIL> =>
            mf.Disjunction(ma)
              .Map(_ => curry(mf.SuccessValue)(ma.SuccessValue));

        /// <summary>
        /// Applicative apply
        /// </summary>
        /// <param name="mf">Lifted function</param>
        /// <param name="ma">Value to apply</param>
        /// <returns>Result of function applied to the value or Fail</returns>
        [Pure]
        public static Validation<MonoidFail, FAIL, B> ApplyM<MonoidFail, FAIL, A, B>(
            this Func<A, Validation<MonoidFail, FAIL, B>> mf, 
            Validation<MonoidFail, FAIL, A> ma) 
            where MonoidFail : Monoid<FAIL>, Eq<FAIL> =>
            ma.Bind(mf);
        
        /// <summary>
        /// Applicative apply
        /// </summary>
        /// <param name="mf">Lifted function</param>
        /// <param name="ma">Value to apply</param>
        /// <param name="mb">Value to apply</param>
        /// <returns>Result of function applied to the values or Fail</returns>
        [Pure]
        public static Validation<MonoidFail, FAIL, C> ApplyM<MonoidFail, FAIL, A, B, C>(
            this  Func<A, B, Validation<MonoidFail, FAIL, C>> mf, 
            Validation<MonoidFail, FAIL, A> ma, 
            Validation<MonoidFail, FAIL, B> mb)
            where MonoidFail : Monoid<FAIL>, Eq<FAIL> =>
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
        public static Validation<MonoidFail, FAIL, Func<B, Validation<MonoidFail, FAIL,C>>> ApplyM<MonoidFail, FAIL, A, B, C>(
            this Func<A, B, Validation<MonoidFail, FAIL, C>> mf, 
            Validation<MonoidFail, FAIL, A> ma) 
            where MonoidFail : Monoid<FAIL>, Eq<FAIL> =>
            ma.Map(a => curry(mf)(a));
        
        /// <summary>
        /// Flatten the nested Validation type
        /// </summary>
        [Pure]
        public static Validation<MonoidFail, FAIL, SUCCESS> Flatten<MonoidFail, FAIL, SUCCESS>(
            this Validation<MonoidFail, FAIL, Validation<MonoidFail, FAIL, SUCCESS>> self)
            where MonoidFail : Monoid<FAIL>, Eq<FAIL> =>
            self.Bind(identity);

        /// <summary>
        /// Extract only the successes 
        /// </summary>
        /// <param name="vs">Enumerable of validations</param>
        /// <typeparam name="F">Fail type</typeparam>
        /// <typeparam name="S">Success type</typeparam>
        /// <returns>Enumerable of successes</returns>
        [Pure]
        public static IEnumerable<S> Successes<MonoidF, F, S>(this IEnumerable<Validation<MonoidF, F, S>> vs)
            where MonoidF : Monoid<F>, Eq<F> 
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
        public static IEnumerable<F> Fails<MonoidF, F, S>(this IEnumerable<Validation<MonoidF, F, S>> vs)
            where MonoidF : Monoid<F>, Eq<F> 
        {
            foreach(var v in vs)
            {
                if (v.IsFail) yield return (F)v;
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
        public static Seq<S> Successes<MonoidF, F, S>(this Seq<Validation<MonoidF, F, S>> vs)
            where MonoidF : Monoid<F>, Eq<F> =>
            toSeq(Successes(vs.AsEnumerable()));

        /// <summary>
        /// Extract only the failures 
        /// </summary>
        /// <param name="vs">Seq of validations</param>
        /// <typeparam name="F">Fail type</typeparam>
        /// <typeparam name="S">Success type</typeparam>
        /// <returns>Enumerable of failures</returns>
        [Pure]
        public static Seq<F> Fails<MonoidF, F, S>(this Seq<Validation<MonoidF, F, S>> vs)
            where MonoidF : Monoid<F>, Eq<F> => 
            toSeq(Fails(vs.AsEnumerable()));
        

        public static Validation<MonoidFail, FAIL, R> Apply<MonoidFail, FAIL, A, R>(this ValueTuple<Validation<MonoidFail, FAIL, A>> items, Func<A, R> f)
            where MonoidFail : Monoid<FAIL>, Eq<FAIL> =>
            items.Item1.Match(
                Succ: s => f(s),
                Fail: e => Validation<MonoidFail, FAIL, R>.Fail(e));

        public static Validation<MonoidFail, FAIL, R> Apply<MonoidFail, FAIL, A, B, R>(
            this ValueTuple<
                Validation<MonoidFail, FAIL, A>,
                Validation<MonoidFail, FAIL, B>> items,
            Func<A, B, R> f)
            where MonoidFail : Monoid<FAIL>, Eq<FAIL> =>
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
            where MonoidFail : Monoid<FAIL>, Eq<FAIL> =>
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
            where MonoidFail : Monoid<FAIL>, Eq<FAIL> =>
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
            where MonoidFail : Monoid<FAIL>, Eq<FAIL> =>
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
            where MonoidFail : Monoid<FAIL>, Eq<FAIL> =>
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
            where MonoidFail : Monoid<FAIL>, Eq<FAIL> =>
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
        
        
        public static Validation<MonoidFail, FAIL, R> ApplyM<MonoidFail, FAIL, A, R>(this ValueTuple<Validation<MonoidFail, FAIL, A>> items, Func<A, Validation<MonoidFail, FAIL, R>> f)
            where MonoidFail : Monoid<FAIL>, Eq<FAIL> =>
            items.Item1.Match(
                Succ: s => f(s),
                Fail: e => Validation<MonoidFail, FAIL, R>.Fail(e));

        public static Validation<MonoidFail, FAIL, R> ApplyM<MonoidFail, FAIL, A, B, R>(
            this ValueTuple<
                Validation<MonoidFail, FAIL, A>,
                Validation<MonoidFail, FAIL, B>> items,
            Func<A, B, Validation<MonoidFail, FAIL, R>> f)
            where MonoidFail : Monoid<FAIL>, Eq<FAIL> =>
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
            where MonoidFail : Monoid<FAIL>, Eq<FAIL> =>
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
            where MonoidFail : Monoid<FAIL>, Eq<FAIL> =>
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
            where MonoidFail : Monoid<FAIL>, Eq<FAIL> =>
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
            where MonoidFail : Monoid<FAIL>, Eq<FAIL> =>
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
            where MonoidFail : Monoid<FAIL>, Eq<FAIL> =>
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
    }
}
