using System;
using System.Linq;
using System.Collections.Generic;
using LanguageExt;
using static LanguageExt.Prelude;
using System.Collections.Immutable;
using System.ComponentModel;

namespace LanguageExt
{
    /// <summary>
    /// Try delegate
    /// </summary>
    public delegate TryResult<T> Try<T>();

    /// <summary>
    /// Holds the state of the Try post invocation.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct TryResult<T>
    {
        internal readonly T Value;
        internal Exception Exception;

        public TryResult(T value)
        {
            Value = value;
            Exception = null;
        }

        public TryResult(Exception e)
        {
            Exception = e;
            Value = default(T);
        }

        public static implicit operator TryResult<T>(T value) =>
            new TryResult<T>(value);

        internal bool IsFaulted => Exception != null;

        public override string ToString() =>
            IsFaulted
                ? Exception.ToString()
                : Value.ToString();
    }


    public struct TrySuccContext<T, R>
    {
        readonly Try<T> value;
        readonly Func<T, R> succHandler;

        internal TrySuccContext(Try<T> value, Func<T, R> succHandler)
        {
            this.value = value;
            this.succHandler = succHandler;
        }

        public R Fail(Func<Exception, R> failHandler) =>
            value.Match(succHandler, failHandler);

        public R Fail(R failValue) =>
            value.Match(succHandler, _ => failValue);
    }

    public struct TrySuccUnitContext<T>
    {
        readonly Try<T> value;
        readonly Action<T> succHandler;

        internal TrySuccUnitContext(Try<T> value, Action<T> succHandler)
        {
            this.value = value;
            this.succHandler = succHandler;
        }

        public Unit Fail(Action<Exception> failHandler) =>
            value.Match(succHandler, failHandler);
    }

    public static class TryConfig
    {
        public static Action<Exception> ErrorLogger = ex => {};
    }
}

/// <summary>
/// Extension methods for the Try monad
/// </summary>
public static class __TryExt
{
    /// <summary>
    /// Invokes the succHandler if Try is in the Success state, otherwise nothing
    /// happens.
    /// </summary>
    public static Unit IfSucc<T>(this Try<T> self, Action<T> succHandler)
    {
        var res = self.Try();
        if (!res.IsFaulted)
        {
            succHandler(res.Value);
        }
        return unit;
    }

    /// <summary>
    /// Returns the Succ(value) of the Try or a default if it's Fail
    /// </summary>
    public static T IfFail<T>(this Try<T> self, T defaultValue)
    {
        if (defaultValue == null) throw new ArgumentNullException("defaultValue");

        var res = self.Try();
        if (res.IsFaulted)
            return defaultValue;
        else
            return res.Value;
    }

    /// <summary>
    /// Returns the Succ(value) of the Try or a default if it's Fail
    /// </summary>
    public static T IfFail<T>(this Try<T> self, Func<T> defaultAction)
    {
        var res = self.Try();
        if (res.IsFaulted)
            return defaultAction();
        else
            return res.Value;
    }

    public static R Match<T, R>(this Try<T> self, Func<T, R> Succ, Func<Exception, R> Fail)
    {
        var res = self.Try();
        return res.IsFaulted
            ? Fail(res.Exception)
            : Succ(res.Value);
    }

    public static R Match<T, R>(this Try<T> self, Func<T, R> Succ, R Fail)
    {
        if (Fail == null) throw new ArgumentNullException("Fail");

        var res = self.Try();
        return res.IsFaulted
            ? Fail
            : Succ(res.Value);
    }

    public static Unit Match<T>(this Try<T> self, Action<T> Succ, Action<Exception> Fail)
    {
        var res = self.Try();

        if (res.IsFaulted)
            Fail(res.Exception);
        else
            Succ(res.Value);

        return Unit.Default;
    }

    public static Option<T> ToOption<T>(this Try<T> self)
    {
        var res = self.Try();
        return res.IsFaulted
            ? None
            : Optional(res.Value);
    }

    public static TryOption<T> ToTryOption<T>(this Try<T> self) => () =>
    {
        var res = self.Try();
        return res.IsFaulted
            ? None
            : Optional(res.Value);
    };

    public static TryResult<T> Try<T>(this Try<T> self)
    {
        try
        {
            return self();
        }
        catch (Exception e)
        {
            TryConfig.ErrorLogger(e);
            return new TryResult<T>(e);
        }
    }

    public static T IfFailThrow<T>(this Try<T> self)
    {
        try
        {
            var res = self();
            if (res.IsFaulted)
            {
                throw res.Exception;
            }
            return res.Value;
        }
        catch (Exception e)
        {
            TryConfig.ErrorLogger(e);
            throw;
        }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
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
                TryConfig.ErrorLogger(e);
                return new TryResult<U>(e);
            }

            U resU;
            try
            {
                resU = select(resT.Value);
            }
            catch (Exception e)
            {
                TryConfig.ErrorLogger(e);
                return new TryResult<U>(e);
            }

            return new TryResult<U>(resU);
        });
    }

    public static Unit Iter<T>(this Try<T> self, Action<T> action) =>
        self.IfSucc(action);

    public static int Count<T>(this Try<T> self)
    {
        var res = self.Try();
        return res.IsFaulted
            ? 0
            : 1;
    }

    public static bool ForAll<T>(this Try<T> self, Func<T, bool> pred)
    {
        var res = self.Try();
        return res.IsFaulted
            ? false
            : pred(res.Value);
    }

    public static S Fold<S, T>(this Try<T> self, S state, Func<S, T, S> folder)
    {
        var res = self.Try();
        return res.IsFaulted
            ? state
            : folder(state, res.Value);
    }

    public static bool Exists<T>(this Try<T> self, Func<T, bool> pred)
    {
        var res = self.Try();
        return res.IsFaulted
            ? false
            : pred(res.Value);
    }

    public static Try<R> Map<T, R>(this Try<T> self, Func<T, R> mapper) => () =>
    {
        var res = self.Try();
        return res.IsFaulted
            ? new TryResult<R>(res.Exception)
            : mapper(res.Value);
    };

    public static Try<T> Filter<T>(this Try<T> self, Func<T, bool> pred)
    {
        var res = self.Try();
        return res.IsFaulted
            ? () => res
            : pred(res.Value)
                ? self
                : () => new TryResult<T>(new Exception("Filtered"));
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Try<T> Where<T>(this Try<T> self, Func<T, bool> pred) =>
        self.Filter(pred);

    public static Try<R> Bind<T, R>(this Try<T> self, Func<T, Try<R>> binder) => () =>
    {
        var res = self.Try();
        return res.IsFaulted
            ? new TryResult<R>(res.Exception)
            : binder(res.Value).Try();
    };

    public static IEnumerable<Either<Exception, T>> AsEnumerable<T>(this Try<T> self)
    {
        var res = self.Try();

        if (res.IsFaulted)
        {
            yield return res.Exception;
        }
        else
        {
            yield return res.Value;
        }
    }

    public static Lst<Either<Exception, T>> ToList<T>(this Try<T> self) =>
        toList(self.AsEnumerable());

    public static ImmutableArray<Either<Exception, T>> ToArray<T>(this Try<T> self) =>
        toArray(self.AsEnumerable());

    public static TrySuccContext<T, R> Succ<T, R>(this Try<T> self, Func<T, R> succHandler) =>
        new TrySuccContext<T, R>(self, succHandler);

    public static TrySuccUnitContext<T> Succ<T>(this Try<T> self, Action<T> succHandler) =>
        new TrySuccUnitContext<T>(self, succHandler);

    public static int Sum(this Try<int> self) =>
        self.Try().Value;

    public static string AsString<T>(this Try<T> self) =>
        match(self,
            Succ: v => v == null
                      ? "Succ(null)"
                      : String.Format("Succ({0})", v),
            Fail: ex => "Fail(" + ex.Message + ")"
        );

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Try<V> SelectMany<T, U, V>(
          this Try<T> self,
          Func<T, Try<U>> bind,
          Func<T, U, V> project
          )
    {
        return new Try<V>(
            () =>
            {
                try
                {
                    var resT = self.Try();
                    if (resT.IsFaulted) return new TryResult<V>(resT.Exception);
                    var resU = bind(resT.Value).Try();
                    if (resU.IsFaulted) return new TryResult<V>(resT.Exception);
                    return new TryResult<V>(project(resT.Value, resU.Value));
                }
                catch (Exception e)
                {
                    TryConfig.ErrorLogger(e);
                    return new TryResult<V>(e);
                }
            }
        );
    }
}
