using System;
using System.Linq;
using System.Collections.Generic;
using LanguageExt;
using static LanguageExt.Prelude;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using System.Reactive.Linq;

namespace LanguageExt
{
    /// <summary>
    /// The TryOption monad captures exceptions and uses them to cancel the
    /// computation.  Additional the return value from the computation is an
    /// Option<T>, which can also cancel the bound computation.  This captures
    /// the complete set of real result you could get from a function:
    ///     * Success 
    ///     * Failure (exception)
    ///     * No value (null equivalent)
    /// </summary>
    /// <remarks>To invoke directly, call x.Try()</remarks>
    /// <returns>A value that represents the outcome of the computation, either
    /// Success, Failure, or None</returns>
    public delegate TryOptionResult<T> TryOption<T>();

    /// <summary>
    /// Holds the state of the TryOption post invocation.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct TryOptionResult<T>
    {
        internal readonly Option<T> Value;
        internal readonly Exception Exception;

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

        [Pure]
        public static implicit operator TryOptionResult<T>(Option<T> value) =>
            new TryOptionResult<T>(value);

        [Pure]
        public static implicit operator TryOptionResult<T>(T value) =>
            new TryOptionResult<T>(Optional(value));

        [Pure]
        public static implicit operator TryOptionResult<T>(OptionNone value) =>
            new TryOptionResult<T>(Option<T>.None);

        [Pure]
        internal bool IsFaulted => Exception != null;

        [Pure]
        public override string ToString() =>
            IsFaulted
                ? Exception.ToString()
                : Value.ToString();
    }


    public struct TryOptionSomeContext<T, R>
    {
        readonly TryOption<T> option;
        readonly Func<T, R> someHandler;

        internal TryOptionSomeContext(TryOption<T> option, Func<T, R> someHandler)
        {
            this.option = option;
            this.someHandler = someHandler;
        }

        [Pure]
        public TryOptionNoneContext<T, R> None(Func<R> noneHandler) =>
            new TryOptionNoneContext<T, R>(option, someHandler, noneHandler);

        [Pure]
        public TryOptionNoneContext<T, R> None(R noneValue) =>
            new TryOptionNoneContext<T, R>(option, someHandler, () => noneValue);
    }

    public struct TryOptionSomeUnitContext<T>
    {
        readonly TryOption<T> option;
        readonly Action<T> someHandler;

        internal TryOptionSomeUnitContext(TryOption<T> option, Action<T> someHandler)
        {
            this.option = option;
            this.someHandler = someHandler;
        }

        [Pure]
        public TryOptionNoneUnitContext<T> None(Action noneHandler) =>
            new TryOptionNoneUnitContext<T>(option, someHandler, noneHandler);
    }

    public struct TryOptionNoneContext<T, R>
    {
        readonly TryOption<T> option;
        readonly Func<T, R> someHandler;
        readonly Func<R> noneHandler;

        internal TryOptionNoneContext(TryOption<T> option, Func<T, R> someHandler, Func<R> noneHandler)
        {
            this.option = option;
            this.someHandler = someHandler;
            this.noneHandler = noneHandler;
        }

        [Pure]
        public R Fail(Func<Exception, R> failHandler) =>
            option.Match(someHandler, noneHandler, failHandler);

        [Pure]
        public R Fail(R failValue) =>
            option.Match(someHandler, noneHandler, _ => failValue);
    }

    public struct TryOptionNoneUnitContext<T>
    {
        readonly TryOption<T> option;
        readonly Action<T> someHandler;
        readonly Action noneHandler;

        internal TryOptionNoneUnitContext(TryOption<T> option, Action<T> someHandler, Action noneHandler)
        {
            this.option = option;
            this.someHandler = someHandler;
            this.noneHandler = noneHandler;
        }

        public Unit Fail(Action<Exception> failHandler) =>
            option.Match(someHandler, noneHandler, failHandler);
    }
}

/// <summary>
/// Extension methods for the TryOption monad
/// </summary>
public static class TryOptionExtensions
{
    /// <summary>
    /// Append the TryOption(x) to TryOption(y).
    /// If either of the TryOptions throw then the result is Fail
    /// If either of the TryOptions return None then the result is None
    /// For numeric values the behaviour is to sum the TryOptions (lhs + rhs)
    /// For string values the behaviour is to concatenate the strings
    /// For Lst/Stck/Que values the behaviour is to concatenate the lists
    /// For Map or Set values the behaviour is to merge the sets
    /// Otherwise if the R type derives from IAppendable then the behaviour
    /// is to call lhs.Append(rhs);
    /// </summary>
    /// <param name="lhs">Left-hand side of the operation</param>
    /// <param name="rhs">Right-hand side of the operation</param>
    /// <returns>lhs + rhs</returns>
    [Pure]
    public static TryOption<T> Append<T>(this TryOption<T> lhs, TryOption<T> rhs) => () =>
    {
        var lhsRes = lhs.Try();
        if (lhsRes.IsFaulted) return lhsRes;
        var rhsRes = rhs.Try();
        if (rhsRes.IsFaulted) return rhsRes;
        return lhsRes.Value.Append(rhsRes.Value);
    };

    /// <summary>
    /// Subtract the TryOption(x) from TryOption(y).
    /// If either of the TryOptions throw then the result is Fail
    /// If either of the TryOptions return None then the result is None
    /// For numeric values the behaviour is to find the difference between the TryOptions (lhs - rhs)
    /// For Lst values the behaviour is to remove items in the rhs from the lhs
    /// For Map or Set values the behaviour is to remove items in the rhs from the lhs
    /// Otherwise if the R type derives from ISubtractable then the behaviour
    /// is to call lhs.Subtract(rhs);
    /// </summary>
    /// <param name="lhs">Left-hand side of the operation</param>
    /// <param name="rhs">Right-hand side of the operation</param>
    /// <returns>lhs - rhs</returns>
    [Pure]
    public static TryOption<T> Subtract<T>(this TryOption<T> lhs, TryOption<T> rhs) => () =>
    {
        var lhsRes = lhs.Try();
        if (lhsRes.IsFaulted) return lhsRes;
        var rhsRes = rhs.Try();
        if (rhsRes.IsFaulted) return rhsRes;
        return lhsRes.Value.Subtract(rhsRes.Value);
    };

    /// <summary>
    /// Find the product of TryOption(x) and TryOption(y).
    /// If either of the TryOptions throw then the result is Fail
    /// If either of the TryOptions return None then the result is None
    /// For numeric values the behaviour is to multiply the TryOptions (lhs * rhs)
    /// For Lst values the behaviour is to multiply all combinations of values in both lists 
    /// to produce a new list
    /// Otherwise if the R type derives from IMultiplicable then the behaviour
    /// is to call lhs.Multiply(rhs);
    /// </summary>
    /// <param name="lhs">Left-hand side of the operation</param>
    /// <param name="rhs">Right-hand side of the operation</param>
    /// <returns>lhs * rhs</returns>
    [Pure]
    public static TryOption<T> Multiply<T>(this TryOption<T> lhs, TryOption<T> rhs) => () =>
    {
        var lhsRes = lhs.Try();
        if (lhsRes.IsFaulted) return lhsRes;
        var rhsRes = rhs.Try();
        if (rhsRes.IsFaulted) return rhsRes;
        return lhsRes.Value.Multiply(rhsRes.Value);
    };

    /// <summary>
    /// Divide TryOption(x) by TryOption(y).  
    /// If either of the TryOptions throw then the result is Fail
    /// If either of the TryOptions return None then the result is None
    /// For numeric values the behaviour is to divide the TryOptions (lhs / rhs)
    /// For Lst values the behaviour is to divide all combinations of values in both lists 
    /// to produce a new list
    /// Otherwise if the R type derives from IDivisible then the behaviour
    /// is to call lhs.Divide(rhs);
    /// </summary>
    /// <param name="lhs">Left-hand side of the operation</param>
    /// <param name="rhs">Right-hand side of the operation</param>
    /// <returns>lhs / rhs</returns>
    [Pure]
    public static TryOption<T> Divide<T>(this TryOption<T> lhs, TryOption<T> rhs) => () =>
    {
        var lhsRes = lhs.Try();
        if (lhsRes.IsFaulted) return lhsRes;
        var rhsRes = rhs.Try();
        if (rhsRes.IsFaulted) return rhsRes;
        return lhsRes.Value.Divide(rhsRes.Value);
    };

    /// <summary>
    /// Apply a TryOption value to a TryOption function
    /// </summary>
    /// <param name="self">TryOption function</param>
    /// <param name="arg">TryOption argument</param>
    /// <returns>Returns the result of applying the TryOption argument to the TryOption function</returns>
    [Pure]
    public static TryOption<R> Apply<T, R>(this TryOption<Func<T, R>> self, TryOption<T> arg) => () =>
    {
        var res = self.Try();
        if (res.IsFaulted) return new TryOptionResult<R>(res.Exception);
        if (res.Value.IsNone) return new TryOptionResult<R>(None);
        var val = arg.Try();
        if (val.IsFaulted) return new TryOptionResult<R>(val.Exception);
        if (val.Value.IsNone) return new TryOptionResult<R>(None);
        return new TryOptionResult<R>(res.Value.Value(val.Value.Value));
    };

    /// <summary>
    /// Apply a TryOption value to a TryOption function of arity 2
    /// </summary>
    /// <param name="self">TryOption function</param>
    /// <param name="arg">TryOption argument</param>
    /// <returns>Returns the result of applying the TryOption argument to the TryOption function:
    /// a TryOption function of arity 1</returns>
    [Pure]
    public static TryOption<Func<T2, R>> Apply<T1, T2, R>(this TryOption<Func<T1, T2, R>> self, TryOption<T1> arg) => () =>
    {
        var res = self.Try();
        if (res.IsFaulted) return new TryOptionResult<Func<T2, R>>(res.Exception);
        if (res.Value.IsNone) return new TryOptionResult<Func<T2, R>>(None);
        var val = arg.Try();
        if (val.IsFaulted) return new TryOptionResult<Func<T2, R>>(val.Exception);
        if (val.Value.IsNone) return new TryOptionResult<Func<T2, R>>(None);
        return new TryOptionResult<Func<T2, R>>(par(res.Value.Value, val.Value.Value));
    };

    /// <summary>
    /// Apply TryOption values to a TryOption function of arity 2
    /// </summary>
    /// <param name="self">TryOption function</param>
    /// <param name="arg1">TryOption argument</param>
    /// <param name="arg2">TryOption argument</param>
    /// <returns>Returns the result of applying the TryOption arguments to TryOption Try function</returns>
    [Pure]
    public static TryOption<R> Apply<T1, T2, R>(this TryOption<Func<T1, T2, R>> self, TryOption<T1> arg1, TryOption<T2> arg2) => () =>
    {
        var res = self.Try();
        if (res.IsFaulted) return new TryOptionResult<R>(res.Exception);
        if (res.Value.IsNone) return new TryOptionResult<R>(None);
        var val1 = arg1.Try();
        if (val1.IsFaulted) return new TryOptionResult<R>(val1.Exception);
        if (val1.Value.IsNone) return new TryOptionResult<R>(None);
        var val2 = arg2.Try();
        if (val2.IsFaulted) return new TryOptionResult<R>(val2.Exception);
        if (val2.Value.IsNone) return new TryOptionResult<R>(None);
        return new TryOptionResult<R>(res.Value.Value(val1.Value.Value, val2.Value.Value));
    };

    /// <summary>
    /// Invokes the someHandler if TryOption is in the Some state, otherwise nothing
    /// happens.
    /// </summary>
    public static Unit IfSome<T>(this TryOption<T> self, Func<T, Unit> someHandler)
    {
        var res = self.Try();
        if (res.Value.IsSome)
        {
            someHandler(res.Value.Value);
        }
        return unit;
    }

    /// <summary>
    /// Invokes the someHandler if TryOption is in the Some state, otherwise nothing
    /// happens.
    /// </summary>
    public static Unit IfSome<T>(this TryOption<T> self, Action<T> someHandler)
    {
        var res = self.Try();
        if (res.Value.IsSome)
        {
            someHandler(res.Value.Value);
        }
        return unit;
    }

    /// <summary>
    /// Returns the Some(value) of the TryOption or a default if it's None or Fail
    /// </summary>
    [Pure]
    public static T IfNone<T>(this TryOption<T> self, T defaultValue)
    {
        if (isnull(defaultValue)) throw new ArgumentNullException(nameof(defaultValue));

        var res = self.Try();
        if (res.IsFaulted || res.Value.IsNone)
            return defaultValue;
        else
            return res.Value.Value;
    }

    /// <summary>
    /// Returns the Some(value) of the TryOption or a default if it's None or Fail
    /// </summary>
    [Pure]
    public static T IfNone<T>(this TryOption<T> self, Func<T> defaultAction)
    {
        var res = self.Try();
        if (res.IsFaulted || res.Value.IsNone)
            return defaultAction();
        else
            return res.Value.Value;
    }

    [Pure]
    public static T IfNoneOrFail<T>(
        this TryOption<T> self,
        Func<T> None,
        Func<Exception, T> Fail)
    {
        var res = self.Try();
        if (res.Value.IsNone)
            return None();
        else if (res.IsFaulted)
            return Fail(res.Exception);
        else
            return res.Value.Value;
    }

    [Pure]
    public static R Match<T, R>(this TryOption<T> self, Func<T, R> Some, Func<R> None, Func<Exception, R> Fail)
    {
        var res = self.Try();
        return res.IsFaulted
            ? Fail(res.Exception)
            : match(res.Value, Some, None);
    }

    [Pure]
    public static R Match<T, R>(this TryOption<T> self, Func<T, R> Some, R None, Func<Exception, R> Fail)
    {
        var res = self.Try();
        return res.IsFaulted
            ? Fail(res.Exception)
            : match(res.Value, Some, () => None);
    }

    [Pure]
    public static R Match<T, R>(this TryOption<T> self, Func<T, R> Some, Func<R> None, R Fail)
    {
        if (isnull(Fail)) throw new ArgumentNullException(nameof(Fail));

        var res = self.Try();
        return res.IsFaulted
            ? Fail
            : match(res.Value, Some, None);
    }

    [Pure]
    public static R Match<T, R>(this TryOption<T> self, Func<T, R> Some, R None, R Fail)
    {
        if (isnull(Fail)) throw new ArgumentNullException(nameof(Fail));

        var res = self.Try();
        return res.IsFaulted
            ? Fail
            : match(res.Value, Some, () => None);
    }

    [Pure]
    public static Unit Match<T>(this TryOption<T> self, Action<T> Some, Action None, Action<Exception> Fail)
    {
        var res = self.Try();

        if (res.IsFaulted)
            Fail(res.Exception);
        else
            match(res.Value, Some, None);

        return Unit.Default;
    }

    public static async Task<R> MatchAsync<T, R>(this TryOption<T> self, Func<T, Task<R>> Succ, Func<R> None, Func<Exception, R> Fail)
    {
        var res = self.Try();
        return await (res.IsFaulted
            ? Task.FromResult(Fail(res.Exception))
            : res.Value.IsSome
                ? Succ(res.Value.Value)
                : Task.FromResult(None()));
    }

    public static async Task<R> MatchAsync<T, R>(this TryOption<T> self, Func<T, Task<R>> Succ, Func<R> None, Func<Exception, Task<R>> Fail)
    {
        var res = self.Try();
        return await (res.IsFaulted
            ? Fail(res.Exception)
            : res.Value.IsSome
                ? Succ(res.Value.Value)
                : Task.FromResult(None()));
    }

    public static async Task<R> MatchAsync<T, R>(this TryOption<T> self, Func<T, R> Succ, Func<R> None, Func<Exception, Task<R>> Fail)
    {
        var res = self.Try();
        return await (res.IsFaulted
            ? Fail(res.Exception)
            : res.Value.IsSome
                ? Task.FromResult(Succ(res.Value.Value))
                : Task.FromResult(None()));
    }

    public static async Task<R> MatchAsync<T, R>(this Task<TryOption<T>> self, Func<T, R> Succ, Func<R> None, Func<Exception, R> Fail) =>
        await self.ContinueWith(trySelf =>
        {
            TryOptionResult<T> res = trySelf.Result.Try();
            return res.IsFaulted
                ? Fail(res.Exception)
                : res.Value.IsSome
                    ? Succ(res.Value.Value)
                    : None();
        });

    public static async Task<R> MatchAsync<T, R>(this Task<TryOption<T>> self, Func<T, Task<R>> Succ, Func<R> None, Func<Exception, R> Fail) =>
        await (from tt in self.ContinueWith(trySelf =>
        {
            var res = trySelf.Result.Try();
            return res.IsFaulted
                ? Task.FromResult(Fail(res.Exception))
                : res.Value.IsSome
                    ? Succ(res.Value.Value)
                    : Task.FromResult(None());
        })
               from t in tt
               select t);

    public static async Task<R> MatchAsync<T, R>(this Task<TryOption<T>> self, Func<T, Task<R>> Succ, Func<R> None, Func<Exception, Task<R>> Fail) =>
        await (from tt in self.ContinueWith(trySelf =>
        {
            var res = trySelf.Result.Try();
            return res.IsFaulted
                ? Fail(res.Exception)
                : res.Value.IsSome
                    ? Succ(res.Value.Value)
                    : Task.FromResult(None());
        })
               from t in tt
               select t);

    public static async Task<R> MatchAsync<T, R>(this Task<TryOption<T>> self, Func<T, R> Succ, Func<R> None, Func<Exception, Task<R>> Fail) =>
        await (from tt in self.ContinueWith(trySelf =>
        {
            var res = trySelf.Result.Try();
            return res.IsFaulted
                ? Fail(res.Exception)
                : res.Value.IsSome
                    ? Task.FromResult(Succ(res.Value.Value))
                    : Task.FromResult(None());
        })
               from t in tt
               select t);

    public static async Task<R> MatchAsync<T, R>(this TryOption<T> self, Func<T, Task<R>> Succ, Func<Task<R>> None, Func<Exception, R> Fail)
    {
        var res = self.Try();
        return await (res.IsFaulted
            ? Task.FromResult(Fail(res.Exception))
            : res.Value.IsSome
                ? Succ(res.Value.Value)
                : None());
    }

    public static async Task<R> MatchAsync<T, R>(this TryOption<T> self, Func<T, Task<R>> Succ, Func<Task<R>> None, Func<Exception, Task<R>> Fail)
    {
        var res = self.Try();
        return await (res.IsFaulted
            ? Fail(res.Exception)
            : res.Value.IsSome
                ? Succ(res.Value.Value)
                : None());
    }

    public static async Task<R> MatchAsync<T, R>(this TryOption<T> self, Func<T, R> Succ, Func<Task<R>> None, Func<Exception, Task<R>> Fail)
    {
        var res = self.Try();
        return await (res.IsFaulted
            ? Fail(res.Exception)
            : res.Value.IsSome
                ? Task.FromResult(Succ(res.Value.Value))
                : None());
    }

    public static async Task<R> MatchAsync<T, R>(this Task<TryOption<T>> self, Func<T, Task<R>> Succ, Func<Task<R>> None, Func<Exception, R> Fail) =>
        await (from tt in self.ContinueWith(trySelf =>
        {
            var res = trySelf.Result.Try();
            return res.IsFaulted
                ? Task.FromResult(Fail(res.Exception))
                : res.Value.IsSome
                    ? Succ(res.Value.Value)
                    : None();
        })
               from t in tt
               select t);

    public static async Task<R> MatchAsync<T, R>(this Task<TryOption<T>> self, Func<T, Task<R>> Succ, Func<Task<R>> None, Func<Exception, Task<R>> Fail) =>
        await (from tt in self.ContinueWith(trySelf =>
        {
            var res = trySelf.Result.Try();
            return res.IsFaulted
                ? Fail(res.Exception)
                : res.Value.IsSome
                    ? Succ(res.Value.Value)
                    : None();
        })
               from t in tt
               select t);

    public static async Task<R> MatchAsync<T, R>(this Task<TryOption<T>> self, Func<T, R> Succ, Func<Task<R>> None, Func<Exception, Task<R>> Fail) =>
        await (from tt in self.ContinueWith(trySelf =>
        {
            var res = trySelf.Result.Try();
            return res.IsFaulted
                ? Fail(res.Exception)
                : res.Value.IsSome
                    ? Task.FromResult(Succ(res.Value.Value))
                    : None();
        })
               from t in tt
               select t);

    public static IObservable<R> MatchObservable<T, R>(this TryOption<T> self, Func<T, IObservable<R>> Succ, Func<R> None, Func<Exception, R> Fail)
    {
        var res = self.Try();
        return (res.IsFaulted
            ? Observable.Return(Fail(res.Exception))
            : res.Value.IsSome
                ? Succ(res.Value.Value)
                : Observable.Return(None()));
    }

    public static IObservable<R> MatchObservable<T, R>(this TryOption<T> self, Func<T, IObservable<R>> Succ, Func<R> None, Func<Exception, IObservable<R>> Fail)
    {
        var res = self.Try();
        return (res.IsFaulted
            ? Fail(res.Exception)
            : res.Value.IsSome
                ? Succ(res.Value.Value)
                : Observable.Return(None()));
    }

    public static IObservable<R> MatchObservable<T, R>(this TryOption<T> self, Func<T, R> Succ, Func<R> None, Func<Exception, IObservable<R>> Fail)
    {
        var res = self.Try();
        return (res.IsFaulted
            ? Fail(res.Exception)
            : res.Value.IsSome
                ? Observable.Return(Succ(res.Value.Value))
                : Observable.Return(None()));
    }

    public static IObservable<R> MatchObservable<T, R>(this IObservable<TryOption<T>> self, Func<T, R> Succ, Func<R> None, Func<Exception, R> Fail) =>
        self.Select(trySelf =>
        {
            TryOptionResult<T> res = trySelf.Try();
            return res.IsFaulted
                ? Fail(res.Exception)
                : res.Value.IsSome
                    ? Succ(res.Value.Value)
                    : None();
        });

    public static IObservable<R> MatchObservable<T, R>(this IObservable<TryOption<T>> self, Func<T, IObservable<R>> Succ, Func<R> None, Func<Exception, R> Fail) =>
        from tt in self.Select(trySelf =>
        {
            var res = trySelf.Try();
            return res.IsFaulted
                ? Observable.Return(Fail(res.Exception))
                : res.Value.IsSome
                    ? Succ(res.Value.Value)
                    : Observable.Return(None());
        })
        from t in tt
        select t;

    public static IObservable<R> MatchObservable<T, R>(this IObservable<TryOption<T>> self, Func<T, IObservable<R>> Succ, Func<R> None, Func<Exception, IObservable<R>> Fail) =>
        from tt in self.Select(trySelf =>
        {
            var res = trySelf.Try();
            return res.IsFaulted
                ? Fail(res.Exception)
                : res.Value.IsSome
                    ? Succ(res.Value.Value)
                    : Observable.Return(None());
        })
        from t in tt
        select t;

    public static IObservable<R> MatchObservable<T, R>(this IObservable<TryOption<T>> self, Func<T, R> Succ, Func<R> None, Func<Exception, IObservable<R>> Fail) =>
        from tt in self.Select(trySelf =>
        {
            var res = trySelf.Try();
            return res.IsFaulted
                ? Fail(res.Exception)
                : res.Value.IsSome
                    ? Observable.Return(Succ(res.Value.Value))
                    : Observable.Return(None());
        })
        from t in tt
        select t;

    public static IObservable<R> MatchObservable<T, R>(this TryOption<T> self, Func<T, IObservable<R>> Succ, Func<IObservable<R>> None, Func<Exception, R> Fail)
    {
        var res = self.Try();
        return (res.IsFaulted
            ? Observable.Return(Fail(res.Exception))
            : res.Value.IsSome
                ? Succ(res.Value.Value)
                : None());
    }

    public static IObservable<R> MatchObservable<T, R>(this TryOption<T> self, Func<T, IObservable<R>> Succ, Func<IObservable<R>> None, Func<Exception, IObservable<R>> Fail)
    {
        var res = self.Try();
        return (res.IsFaulted
            ? Fail(res.Exception)
            : res.Value.IsSome
                ? Succ(res.Value.Value)
                : None());
    }

    public static IObservable<R> MatchObservable<T, R>(this TryOption<T> self, Func<T, R> Succ, Func<IObservable<R>> None, Func<Exception, IObservable<R>> Fail)
    {
        var res = self.Try();
        return (res.IsFaulted
            ? Fail(res.Exception)
            : res.Value.IsSome
                ? Observable.Return(Succ(res.Value.Value))
                : None());
    }

    public static IObservable<R> MatchObservable<T, R>(this IObservable<TryOption<T>> self, Func<T, IObservable<R>> Succ, Func<IObservable<R>> None, Func<Exception, R> Fail) =>
        from tt in self.Select(trySelf =>
        {
            var res = trySelf.Try();
            return res.IsFaulted
                ? Observable.Return(Fail(res.Exception))
                : res.Value.IsSome
                    ? Succ(res.Value.Value)
                    : None();
        })
        from t in tt
        select t;

    public static IObservable<R> MatchObservable<T, R>(this IObservable<TryOption<T>> self, Func<T, IObservable<R>> Succ, Func<IObservable<R>> None, Func<Exception, IObservable<R>> Fail) =>
        from tt in self.Select(trySelf =>
        {
            var res = trySelf.Try();
            return res.IsFaulted
                ? Fail(res.Exception)
                : res.Value.IsSome
                    ? Succ(res.Value.Value)
                    : None();
        })
        from t in tt
        select t;

    public static IObservable<R> MatchObservable<T, R>(this IObservable<TryOption<T>> self, Func<T, R> Succ, Func<IObservable<R>> None, Func<Exception, IObservable<R>> Fail) =>
        from tt in self.Select(trySelf =>
        {
            var res = trySelf.Try();
            return res.IsFaulted
                ? Fail(res.Exception)
                : res.Value.IsSome
                    ? Observable.Return(Succ(res.Value.Value))
                    : None();
        })
        from t in tt
        select t;

    [Pure]
    public static Option<T> ToOption<T>(this TryOption<T> self) =>
        self.Try().Value;

    [Pure]
    public static TryOptionResult<T> Try<T>(this TryOption<T> self)
    {
        try
        {
            if (self == null) return None;
            return self();
        }
        catch (Exception e)
        {
            TryConfig.ErrorLogger(e);
            return new TryOptionResult<T>(e);
        }
    }

    [Pure]
    public static Option<T> IfFailThrow<T>(this TryOption<T> self)
    {
        try
        {
            if (self == null) return None;
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

    [Pure]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static TryOption<U> Select<T, U>(this TryOption<T> self, Func<T, U> select)
    {
        return new TryOption<U>(() =>
        {
            TryOptionResult<T> resT;
            resT = self.Try();
            if (resT.IsFaulted) return new TryOptionResult<U>(resT.Exception);
            if (resT.Value.IsNone) return new TryOptionResult<U>(None);

            Option<U> resU;
            try
            {
                resU = select(resT.Value.Value);
            }
            catch (Exception e)
            {
                TryConfig.ErrorLogger(e);
                return new TryOptionResult<U>(e);
            }

            return new TryOptionResult<U>(resU);
        });
    }

    public static Unit Iter<T>(this TryOption<T> self, Action<T> action) =>
        self.IfSome(action);

    public static Unit Iter<T>(this TryOption<T> self, Action<T> Some, Action None, Action<Exception> Fail)
    {
        var res = self.Try();
        if (res.IsFaulted) Fail(res.Exception);
        else if (res.Value.IsNone) None();
        else if (res.Value.IsNone) Some(res.Value.Value);
        return unit;
    }

    [Pure]
    public static int Count<T>(this TryOption<T> self)
    {
        var res = self.Try();
        return res.IsFaulted
            ? 0
            : res.Value.Count();
    }

    [Pure]
    public static bool ForAll<T>(this TryOption<T> self, Func<T, bool> pred)
    {
        var res = self.Try();
        return res.IsFaulted
            ? false
            : res.Value.ForAll(pred);
    }

    [Pure]
    public static bool ForAll<T>(this TryOption<T> self, Func<T, bool> Some, Func<bool> None, Func<Exception, bool> Fail)
    {
        var res = self.Try();
        return res.IsFaulted
            ? Fail(res.Exception)
            : res.Value.ForAll(Some, None);
    }

    /// <summary>
    /// Folds TryOption value into an S.
    /// {https://en.wikipedia.org/wiki/Fold_(higher-order_function)}
    /// </summary>
    /// <param name="self">Try to fold</param>
    /// <param name="state">Initial state</param>
    /// <param name="folder">Fold function</param>
    /// <returns>Folded state</returns>
    [Pure]
    public static S Fold<S, T>(this TryOption<T> self, S state, Func<S, T, S> folder)
    {
        var res = self.Try();
        return res.IsFaulted
            ? state
            : res.Value.Fold(state, folder);
    }

    /// <summary>
    /// Folds TryOption value into an S.
    /// {https://en.wikipedia.org/wiki/Fold_(higher-order_function)}
    /// </summary>
    /// <param name="self">Try to fold</param>
    /// <param name="state">Initial state</param>
    /// <param name="Some">Fold function for Some</param>
    /// <param name="None">Fold function for None</param>
    /// <param name="Fail">Fold function for Failure</param>
    /// <returns>Folded state</returns>
    [Pure]
    public static S Fold<S, T>(this TryOption<T> self, S state, Func<S, T, S> Some, Func<S, S> None, Func<S, Exception, S> Fail)
    {
        var res = self.Try();
        return res.IsFaulted
            ? Fail(state, res.Exception)
            : res.Value.Fold(state, Some, None);
    }

    [Pure]
    public static bool Exists<T>(this TryOption<T> self, Func<T, bool> pred)
    {
        var res = self.Try();
        return res.IsFaulted
            ? false
            : res.Value.Exists(pred);
    }

    [Pure]
    public static bool Exists<T>(this TryOption<T> self, Func<T, bool> Some, Func<bool> None, Func<Exception, bool> Fail)
    {
        var res = self.Try();
        return res.IsFaulted
            ? Fail(res.Exception)
            : res.Value.Exists(Some, None);
    }

    [Pure]
    public static TryOption<T> Filter<T>(this TryOption<T> self, Func<T, bool> pred)
    {
        var res = self.Try();
        return res.IsFaulted || res.Value.IsNone
            ? () => res
            : pred(res.Value.Value)
                ? self
                : () => None;
    }

    [Pure]
    public static TryOption<R> Map<T, R>(this TryOption<T> self, Func<T, R> mapper) =>
        self.Select(mapper);

    [Pure]
    public static TryOption<R> Map<T, R>(this TryOption<T> self, Func<T, R> Some, Func<R> None, Func<Exception, R> Fail) => () =>
    {
        var res = self.Try();
        if (res.IsFaulted) return Fail(res.Exception);
        if (res.Value.IsNone) return None();
        return Some(res.Value.Value);
    };

    /// <summary>
    /// Partial application map
    /// </summary>
    /// <remarks>TODO: Better documentation of this function</remarks>
    [Pure]
    public static TryOption<Func<T2, R>> ParMap<T1, T2, R>(this TryOption<T1> self, Func<T1, T2, R> func) =>
        self.Map(curry(func));

    /// <summary>
    /// Partial application map
    /// </summary>
    /// <remarks>TODO: Better documentation of this function</remarks>
    [Pure]
    public static TryOption<Func<T2, Func<T3, R>>> ParMap<T1, T2, T3, R>(this TryOption<T1> self, Func<T1, T2, T3, R> func) =>
        self.Map(curry(func));

    [Pure]
    public static TryOption<R> Bind<T, R>(this TryOption<T> self, Func<T, TryOption<R>> binder) => () =>
    {
        var res = self.Try();
        return res.IsFaulted
            ? new TryOptionResult<R>(res.Exception)
            : res.Value.IsNone
                ? new TryOptionResult<R>(None)
                : binder(res.Value.Value).Try();
    };

    [Pure]
    public static TryOption<R> Bind<T, R>(this TryOption<T> self, Func<T, TryOption<R>> Some, Func<TryOption<R>> None, Func<Exception, TryOption<R>> Fail) => () =>
    {
        var res = self.Try();
        return res.IsFaulted
            ? Fail(res.Exception).Try()
            : res.Value.IsNone
                ? None().Try()
                : Some(res.Value.Value).Try();
    };

    [Pure]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static TryOption<T> Where<T>(this TryOption<T> self, Func<T, bool> pred) =>
        self.Filter(pred);

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

    [Pure]
    public static Lst<Either<Exception, T>> ToList<T>(this TryOption<T> self) =>
        toList(self.AsEnumerable());

    [Pure]
    public static Either<Exception, T>[] ToArray<T>(this TryOption<T> self) =>
        toArray(self.AsEnumerable());

    [Pure]
    public static TryOptionSomeContext<T, R> Some<T, R>(this TryOption<T> self, Func<T, R> someHandler) =>
        new TryOptionSomeContext<T, R>(self, someHandler);

    [Pure]
    public static TryOptionSomeUnitContext<T> Some<T>(this TryOption<T> self, Action<T> someHandler) =>
        new TryOptionSomeUnitContext<T>(self, someHandler);

    [Pure]
    public static int Sum(this TryOption<int> self)
    {
        var res = self.Try();
        if (res.IsFaulted) return 0;
        return res.Value.Sum();
    }

    [Pure]
    public static string AsString<T>(this TryOption<T> self) =>
        match(self,
            Some: v => isnull(v)
                        ? "Some(null)"
                        : $"Some({v})",
            None: () => "None",
            Fail: ex => $"Fail({ex.Message})"
        );

    [Pure]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static TryOption<V> SelectMany<T, U, V>(
          this TryOption<T> self,
          Func<T, TryOption<U>> bind,
          Func<T, U, V> project
          )
    {
        return new TryOption<V>(
            () =>
            {
                var resT = self.Try();
                if (resT.IsFaulted) return new TryOptionResult<V>(resT.Exception);
                if (resT.Value.IsNone) return new TryOptionResult<V>(None);
                var resU = bind(resT.Value.Value).Try();
                if (resU.IsFaulted)
                {
                    return new TryOptionResult<V>(resU.Exception);
                }
                if (resU.Value.IsNone)
                {
                    return new TryOptionResult<V>(None);
                }

                try
                {
                    var res = new TryOptionResult<V>(project(resT.Value.Value, resU.Value.Value));
                    return res;
                }
                catch (Exception e)
                {
                    TryConfig.ErrorLogger(e);
                    return new TryOptionResult<V>(e);
                }
            }
        );
    }

    [Pure]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static IEnumerable<V> SelectMany<T, U, V>(this TryOption<T> self,
        Func<T, IEnumerable<U>> bind,
        Func<T, U, V> project
        )
    {
        var resT = self.Try();
        if (resT.IsFaulted || resT.Value.IsNone) return new V[0];
        return bind(resT.Value.Value).Map(resU => project(resT.Value.Value, resU));
    }

    [Pure]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static TryOption<V> SelectMany<T, U, V>(this IEnumerable<T> self,
        Func<T, TryOption<U>> bind,
        Func<T, U, V> project
        )
    {
        return new TryOption<V>(() =>
       {
           var ta = self.Take(1).ToArray();
           if (ta.Length == 0) return None;
           var u = bind(ta[0]);
           var resU = u.Try();
           if (resU.IsFaulted) return new TryOptionResult<V>(resU.Exception);
           if (resU.Value.IsNone) return new TryOptionResult<V>(None);
           return Optional(project(ta[0], resU.Value.Value));
       });
    }

    public static TryOption<V> Join<L, T, U, K, V>(
        this TryOption<T> self,
        TryOption<U> inner,
        Func<T, K> outerKeyMap,
        Func<U, K> innerKeyMap,
        Func<T, U, V> project) => () =>
        {
            var selfRes = self.Try();
            if (selfRes.IsFaulted) return new TryOptionResult<V>(selfRes.Exception);
            if (selfRes.Value.IsNone) return new TryOptionResult<V>(None);

            var innerRes = inner.Try();
            if (innerRes.IsFaulted) return new TryOptionResult<V>(innerRes.Exception);
            if (innerRes.Value.IsNone) return new TryOptionResult<V>(None);

            return EqualityComparer<K>.Default.Equals(outerKeyMap(selfRes.Value.Value), innerKeyMap(innerRes.Value.Value))
                ? new TryOptionResult<V>(project(selfRes.Value.Value, innerRes.Value.Value))
                : None;
        };
}