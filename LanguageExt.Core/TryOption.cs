using System;
using System.Collections.Generic;
using LanguageExt.Prelude;

namespace LanguageExt
{
    /// <summary>
    /// TryOption delegate
    /// </summary>
    public delegate TryOptionResult<T> TryOption<T>();

    /// <summary>
    /// Holds the state of the TryOption post invocation.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct TryOptionResult<T>
    {
        internal readonly Option<T> Value;
        internal Exception Exception;

        public TryOptionResult(Option<T> value)
        {
            Value = value;
            Exception = null;
        }

        public TryOptionResult(Exception e)
        {
            Exception = e;
            Value = default(T);
        }

        public static implicit operator TryOptionResult<T>(Option<T> value) =>
            new TryOptionResult<T>(value);

        public static implicit operator TryOptionResult<T>(T value) =>
            new TryOptionResult<T>(Option.Cast(value));

        internal bool IsFaulted => Exception != null;

        public override string ToString() =>
            IsFaulted
                ? Exception.ToString()
                : Value.ToString();
    }

    /// <summary>
    /// Extension methods for the TryOption monad
    /// </summary>
    public static class __TryOptionExt
    {
        /// <summary>
        /// Returns the Some(value) of the TryOption or a default if it's None or Fail
        /// </summary>
        public static T Failure<T>(this TryOption<T> self, T defaultValue)
        {
            if (defaultValue == null) throw new ArgumentNullException("defaultValue");

            var res = self.Try();
            if (res.IsFaulted || res.Value.IsNone)
                return defaultValue;
            else
                return res.Value.Value;
        }

        /// <summary>
        /// Returns the Some(value) of the TryOption or a default if it's None or Fail
        /// </summary>
        public static T Failure<T>(this TryOption<T> self, Func<T> defaultAction)
        {
            var res = self.Try();
            if (res.IsFaulted || res.Value.IsNone)
                return defaultAction();
            else
                return res.Value.Value;
        }

        public static R Match<T, R>(this TryOption<T> self, Func<T, R> Some, Func<R> None, Func<Exception, R> Fail)
        {
            var res = self.Try();
            return res.IsFaulted
                ? Fail(res.Exception)
                : match(res.Value, Some, None);
        }

        public static R Match<T, R>(this TryOption<T> self, Func<T, R> Some, R None, Func<Exception, R> Fail)
        {
            var res = self.Try();
            return res.IsFaulted
                ? Fail(res.Exception)
                : match(res.Value, Some, () => None);
        }

        public static R Match<T, R>(this TryOption<T> self, Func<T, R> Some, Func<R> None, R Fail)
        {
            if (Fail == null) throw new ArgumentNullException("Fail");

            var res = self.Try();
            return res.IsFaulted
                ? Fail
                : match(res.Value, Some, None);
        }

        public static R Match<T, R>(this TryOption<T> self, Func<T, R> Some, R None, R Fail)
        {
            if (Fail == null) throw new ArgumentNullException("Fail");

            var res = self.Try();
            return res.IsFaulted
                ? Fail
                : match(res.Value, Some, () => None);
        }

        public static Unit Match<T>(this TryOption<T> self, Action<T> Some, Action None, Action<Exception> Fail)
        {
            var res = self.Try();

            if (res.IsFaulted)
                Fail(res.Exception);
            else
                match(res.Value, Some, None);

            return Unit.Default;
        }

        private static TryOptionResult<T> Try<T>(this TryOption<T> self)
        {
            try
            {
                return self();
            }
            catch (Exception e)
            {
                return new TryOptionResult<T>(e);
            }
        }

        public static TryOption<U> Select<T, U>(this TryOption<T> self, Func<Option<T>, Option<U>> select)
        {
            return new TryOption<U>(() =>
            {
                TryOptionResult<T> resT;
                try
                {
                    resT = self();
                    if (resT.IsFaulted)
                        return new TryOptionResult<U>(resT.Exception);
                }
                catch (Exception e)
                {
                    return new TryOptionResult<U>(e);
                }

                Option<U> resU;
                try
                {
                    resU = select(resT.Value);
                }
                catch (Exception e)
                {
                    return new TryOptionResult<U>(e);
                }

                return new TryOptionResult<U>(resU);
            });
        }

        public static TryOption<V> SelectMany<T, U, V>(
            this TryOption<T> self,
            Func<Option<T>, TryOption<U>> select,
            Func<Option<T>, Option<U>, Option<V>> bind
            )
        {
            return new TryOption<V>(
                () =>
                {
                    TryOptionResult<T> resT;
                    try
                    {
                        resT = self();
                        if (resT.IsFaulted)
                            return new TryOptionResult<V>(resT.Exception);
                    }
                    catch (Exception e)
                    {
                        return new TryOptionResult<V>(e);
                    }

                    TryOptionResult<U> resU;
                    try
                    {
                        resU = select(resT.Value)();
                        if (resU.IsFaulted)
                            return new TryOptionResult<V>(resU.Exception);
                    }
                    catch (Exception e)
                    {
                        return new TryOptionResult<V>(e);
                    }

                    Option<V> resV;
                    try
                    {
                        resV = bind(resT.Value, resU.Value);
                    }
                    catch (Exception e)
                    {
                        return new TryOptionResult<V>(e);
                    }

                    return new TryOptionResult<V>(resV);
                }
            );
        }

        /// <summary>
        /// Converts the TryOption to an infinite enumerable of T
        /// </summary>
        /// <returns>
        /// Some: An infinite list of T
        /// None|Fail: An empty list
        /// </returns>
        public static IEnumerable<T> AsEnumerable<T>(this TryOption<T> self)
        {
            var res = self();
            if (res.IsFaulted || res.Value.IsNone)
                yield break;
            else
                yield return res.Value.Value;
        }
    }
}