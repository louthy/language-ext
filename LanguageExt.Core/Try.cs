using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt
{
    /// <summary>
    /// The Try monad delegate
    /// </summary>
    public delegate TryResult<T> Try<T>();

    /// <summary>
    /// Holds the state of the error monad during the bind function
    /// If IsFaulted == true then the bind function will be cancelled.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct TryResult<T>
    {
        internal readonly T Value;
        internal Exception Exception;

        /// <summary>
        /// Ctor
        /// </summary>
        public TryResult(T value)
        {
            Value = value;
            Exception = null;
        }

        /// <summary>
        /// Ctor
        /// </summary>
        public TryResult(Exception e)
        {
            Exception = e;
            Value = default(T);
        }

        public static implicit operator TryResult<T>(T value)
        {
            return new TryResult<T>(value);
        }
 

        /// <summary>
        /// True if faulted
        /// </summary>
        public bool IsFaulted
        {
            get
            {
                return Exception != null;
            }
        }

        /// <summary>
        /// ToString override
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return IsFaulted
                ? Exception.ToString()
                : Value != null
                    ? Value.ToString()
                    : "[null]";
        }
    }

    /// <summary>
    /// Extension methods for the error monad
    /// </summary>
    public static class __TryMonadExt
    {
        /// <summary>
        /// Returns the value of the Try or a default if faulted
        /// </summary>
        public static T Failure<T>(this Try<T> self, T defaultValue)
        {
            if (defaultValue == null) throw new ArgumentNullException("defaultValue");

            var res = self.Try();
            if (res.IsFaulted || res.Value == null)
                return defaultValue;
            else
                return res.Value;
        }

        /// <summary>
        /// Returns the value of the Try or a default if faulted
        /// </summary>
        public static T Failure<T>(this Try<T> self, Func<T> defaultAction)
        {
            var res = self.Try();
            if (res.IsFaulted || res.Value == null)
                return defaultAction();
            else
                return res.Value;
        }

        /// <summary>
        /// Pattern matching
        /// </summary>
        public static R Match<T, R>(this Try<T> self, Func<T, R> Succ, Func<Exception, R> Fail)
        {
            var res = self.Try();
            return res.IsFaulted || res.Value == null
                ? Fail(res.Exception)
                : Succ(res.Value);
        }

        /// <summary>
        /// Pattern matching
        /// </summary>
        public static R Match<T, R>(this Try<T> self, Func<T, R> Succ, R Fail)
        {
            if (Fail == null) throw new ArgumentNullException("Fail");

            var res = self.Try();
            return res.IsFaulted || res.Value == null
                ? Fail
                : Succ(res.Value);
        }

        /// <summary>
        /// Pattern matching
        /// </summary>
        public static Unit Match<T>(this Try<T> self, Action<T> Succ, Action<Exception> Fail)
        {
            var res = self.Try();

            if (res.IsFaulted)
                Fail(res.Exception);
            else
                Succ(res.Value);

            return Unit.Default;
        }

        private static TryResult<T> Try<T>(this Try<T> self)
        {
            try
            {
                return self();
            }
            catch (Exception e)
            {
                return new TryResult<T>(e);
            }
        }

        /// <summary>
        /// Select
        /// </summary>
        public static Try<U> Select<T, U>(this Try<T> self, Func<T, U> select)
        {
            return new Try<U>(() =>
            {
                TryResult<T> resT;
                try
                {
                    resT = self();
                    if (resT.IsFaulted)
                        return new TryResult<U>(resT.Exception);
                }
                catch (Exception e)
                {
                    return new TryResult<U>(e);
                }

                U resU;
                try
                {
                    resU = select(resT.Value);
                }
                catch (Exception e)
                {
                    return new TryResult<U>(e);
                }

                return new TryResult<U>(resU);
            });
        }

        /// <summary>
        /// SelectMany
        /// </summary>
        public static Try<V> SelectMany<T, U, V>(
            this Try<T> self,
            Func<T, Try<U>> select,
            Func<T, U, V> bind
            )
        {
            return new Try<V>(
                () =>
                {
                    TryResult<T> resT;
                    try
                    {
                        resT = self();
                        if (resT.IsFaulted)
                            return new TryResult<V>(resT.Exception);
                    }
                    catch (Exception e)
                    {
                        return new TryResult<V>(e);
                    }

                    TryResult<U> resU;
                    try
                    {
                        resU = select(resT.Value)();
                        if (resU.IsFaulted)
                            return new TryResult<V>(resU.Exception);
                    }
                    catch (Exception e)
                    {
                        return new TryResult<V>(e);
                    }

                    V resV;
                    try
                    {
                        resV = bind(resT.Value, resU.Value);
                    }
                    catch (Exception e)
                    {
                        return new TryResult<V>(e);
                    }

                    return new TryResult<V>(resV);
                }
            );
        }

        /// <summary>
        /// Converts the Try to an enumerable of T
        /// </summary>
        /// <returns>
        /// Succ: A list with one T in
        /// Error: An empty list
        /// </returns>
        public static IEnumerable<T> AsEnumerableOne<T>(this Try<T> self)
        {
            var res = self();
            if (res.IsFaulted)
                yield break;
            else
                yield return res.Value;
        }

        /// <summary>
        /// Converts the Try to an infinite enumerable of T
        /// </summary>
        /// <returns>
        /// Succ: An infinite list of T
        /// Error: An empty list
        /// </returns>
        public static IEnumerable<T> AsEnumerable<T>(this Try<T> self)
        {
            var res = self();
            if (res.IsFaulted)
                yield break;
            else
                while (true) yield return res.Value;
        }
    }
}