using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using LanguageExt;
using static LanguageExt.Prelude;

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

        public static implicit operator TryOptionResult<T>(OptionNone value) =>
            new TryOptionResult<T>(Option<T>.None);

        internal bool IsFaulted => Exception != null;

        public override string ToString() =>
            IsFaulted
                ? Exception.ToString()
                : Value.ToString();
    }


    public struct TrySomeContext<T, R>
    {
        readonly TryOption<T> option;
        readonly Func<T, R> someHandler;

        internal TrySomeContext(TryOption<T> option, Func<T, R> someHandler)
        {
            this.option = option;
            this.someHandler = someHandler;
        }

        public TryNoneContext<T, R> None(Func<R> noneHandler) =>
            new TryNoneContext<T, R>(option, someHandler, noneHandler);

        public TryNoneContext<T, R> None(R noneValue) =>
            new TryNoneContext<T, R>(option, someHandler, () => noneValue);
    }

    public struct TryNoneContext<T, R>
    {
        readonly TryOption<T> option;
        readonly Func<T, R> someHandler;
        readonly Func<R> noneHandler;

        internal TryNoneContext(TryOption<T> option, Func<T, R> someHandler, Func<R> noneHandler)
        {
            this.option = option;
            this.someHandler = someHandler;
            this.noneHandler = noneHandler;
        }

        public R Fail(Func<Exception, R> failHandler) =>
            option.Match(someHandler, noneHandler, failHandler);

        public R Fail(R failValue) =>
            option.Match(someHandler, noneHandler, _ => failValue);
    }
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

    public static int Count<T>(this TryOption<T> self)
    {
        var res = self.Try();
        return res.IsFaulted
            ? 0
            : res.Value.Count;
    }

    public static bool ForAll<T>(this TryOption<T> self, Func<T, bool> pred)
    {
        var res = self.Try();
        return res.IsFaulted
            ? false
            : res.Value.ForAll(pred);
    }

    public static S Fold<S, T>(this TryOption<T> self, S state, Func<S, T, S> folder)
    {
        var res = self.Try();
        return res.IsFaulted
            ? state
            : res.Value.Fold(state, folder);
    }

    public static bool Exists<T>(this TryOption<T> self, Func<T, bool> pred)
    {
        var res = self.Try();
        return res.IsFaulted
            ? false
            : res.Value.Exists(pred);
    }

    public static TryOption<T> Where<T>(this TryOption<T> self, Func<T, bool> pred)
    {
        var res = self.Try();
        return res.IsFaulted || res.Value.IsNone
            ? () => res
            : pred(res.Value.Value)
                ? self
                : () => None;
    }

    public static TryOption<R> Map<T, R>(this TryOption<T> self, Func<T, R> mapper) => () =>
    {
        var res = self.Try();
        return res.IsFaulted
            ? new TryOptionResult<R>(res.Exception)
            : res.Value.Map(mapper);
    };

    public static TryOption<R> Bind<T, R>(this TryOption<T> self, Func<T, TryOption<R>> binder) => () =>
    {
        var res = self.Try();
        return !res.IsFaulted && res.Value.IsSome
            ? binder(res.Value.Value)()
            : new TryOptionResult<R>(res.Exception);
    };

    public static IEnumerable<Either<Exception, T>> AsEnumerable<T>(this TryOption<T> self)
    {
        var res = self.Try();

        if (res.IsFaulted)
        {
            yield return res.Exception;
        }
        else if (res.Value.IsSome)
        {
            yield return res.Value.Value;
        }
    }

    public static IImmutableList<Either<Exception, T>> ToList<T>(this TryOption<T> self) =>
        toList(self.AsEnumerable());

    public static ImmutableArray<Either<Exception, T>> ToArray<T>(this TryOption<T> self) =>
        toArray(self.AsEnumerable());

    public static TrySomeContext<T, R> Some<T, R>(this TryOption<T> self, Func<T, R> someHandler) =>
        new TrySomeContext<T, R>(self, someHandler);

    public static string AsString<T>(this TryOption<T> self) =>
        match(self,
            Some: v => v == null
                        ? "Some(null)"
                        : String.Format("Some({0})", v),
            None: () => "None",
            Fail: ex => "Fail(" + ex.Message + ")"
        );
}
