using System.Collections.Generic;
using System.Diagnostics.Contracts;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    public static class ValidationSeqExtensions
    {
        /// <summary>
        /// Flatten the nested Validation type
        /// </summary>
        [Pure]
        public static Validation<FAIL, SUCCESS> Flatten<FAIL, SUCCESS>(this Validation<FAIL, Validation<FAIL, SUCCESS>> self) =>
            self.Bind(identity);
        
        /// <summary>
        /// Flatten the nested Validation type
        /// </summary>
        [Pure]
        public static Validation<MonoidFail, FAIL, SUCCESS> Flatten<MonoidFail, FAIL, SUCCESS>(this Validation<MonoidFail, FAIL, Validation<MonoidFail, FAIL, SUCCESS>> self)
            where MonoidFail : struct, Monoid<FAIL>, Eq<FAIL> =>
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
            Seq(Successes(vs.AsEnumerable()));

        /// <summary>
        /// Extract only the failures 
        /// </summary>
        /// <param name="vs">Seq of validations</param>
        /// <typeparam name="F">Fail type</typeparam>
        /// <typeparam name="S">Success type</typeparam>
        /// <returns>Enumerable of failures</returns>
        [Pure]
        public static Seq<F> Fails<F, S>(this Seq<Validation<F, S>> vs) =>
            Seq(Fails(vs.AsEnumerable()));

        /// <summary>
        /// Extract only the successes 
        /// </summary>
        /// <param name="vs">Enumerable of validations</param>
        /// <typeparam name="F">Fail type</typeparam>
        /// <typeparam name="S">Success type</typeparam>
        /// <returns>Enumerable of successes</returns>
        [Pure]
        public static IEnumerable<S> Successes<MonoidF, F, S>(this IEnumerable<Validation<MonoidF, F, S>> vs)
            where MonoidF : struct, Monoid<F>, Eq<F> 
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
            where MonoidF : struct, Monoid<F>, Eq<F> 
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
            where MonoidF : struct, Monoid<F>, Eq<F> =>
            Seq(Successes(vs.AsEnumerable()));

        /// <summary>
        /// Extract only the failures 
        /// </summary>
        /// <param name="vs">Seq of validations</param>
        /// <typeparam name="F">Fail type</typeparam>
        /// <typeparam name="S">Success type</typeparam>
        /// <returns>Enumerable of failures</returns>
        [Pure]
        public static Seq<F> Fails<MonoidF, F, S>(this Seq<Validation<MonoidF, F, S>> vs)
            where MonoidF : struct, Monoid<F>, Eq<F> => 
            Seq(Fails(vs.AsEnumerable()));
    }
}
