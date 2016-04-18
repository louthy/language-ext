using System;
using System.Collections.Generic;
using LanguageExt;
using LanguageExt.Trans;
using static LanguageExt.Prelude;
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

    public static class TryResult
    {
        public static TryResult<T> Cast<T>(T value) =>
            new TryResult<T>(value);
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
    /// Append the Try(x) to Try(y).  If either of the Trys throw then the result is Fail
    /// For numeric values the behaviour is to sum the Trys (lhs + rhs)
    /// For string values the behaviour is to concatenate the strings
    /// For Lst/Stck/Que values the behaviour is to concatenate the lists
    /// For Map or Set values the behaviour is to merge the sets
    /// Otherwise if the R type derives from IAppendable then the behaviour
    /// is to call lhs.Append(rhs);
    /// </summary>
    /// <param name="lhs">Left-hand side of the operation</param>
    /// <param name="rhs">Right-hand side of the operation</param>
    /// <returns>lhs + rhs</returns>
    public static Try<T> Append<T>(this Try<T> lhs, Try<T> rhs) => () =>
    {
        var lhsRes = lhs.Try();
        if (lhsRes.IsFaulted) return lhsRes;
        var rhsRes = rhs.Try();
        if (rhsRes.IsFaulted) return rhsRes;
        return TypeDesc.Append(lhsRes.Value, rhsRes.Value, TypeDesc<T>.Default);
    };

    /// <summary>
    /// Subtract the Try(x) from Try(y).  If either of the Trys throw then the result is Fail
    /// For numeric values the behaviour is to find the difference between the Trys (lhs - rhs)
    /// For Lst values the behaviour is to remove items in the rhs from the lhs
    /// For Map or Set values the behaviour is to remove items in the rhs from the lhs
    /// Otherwise if the R type derives from ISubtractable then the behaviour
    /// is to call lhs.Subtract(rhs);
    /// </summary>
    /// <param name="lhs">Left-hand side of the operation</param>
    /// <param name="rhs">Right-hand side of the operation</param>
    /// <returns>lhs - rhs</returns>
    public static Try<T> Subtract<T>(this Try<T> lhs, Try<T> rhs) => () =>
    {
        var lhsRes = lhs.Try();
        if (lhsRes.IsFaulted) return lhsRes;
        var rhsRes = rhs.Try();
        if (rhsRes.IsFaulted) return rhsRes;
        return TypeDesc.Subtract(lhsRes.Value, rhsRes.Value, TypeDesc<T>.Default);
    };

    /// <summary>
    /// Find the product of Try(x) and Try(y).  If either of the Trys throw then the result is Fail
    /// For numeric values the behaviour is to multiply the Trys (lhs * rhs)
    /// For Lst values the behaviour is to multiply all combinations of values in both lists 
    /// to produce a new list
    /// Otherwise if the R type derives from IMultiplicable then the behaviour
    /// is to call lhs.Multiply(rhs);
    /// </summary>
    /// <param name="lhs">Left-hand side of the operation</param>
    /// <param name="rhs">Right-hand side of the operation</param>
    /// <returns>lhs * rhs</returns>
    public static Try<T> Multiply<T>(this Try<T> lhs, Try<T> rhs) => () =>
    {
        var lhsRes = lhs.Try();
        if (lhsRes.IsFaulted) return lhsRes;
        var rhsRes = rhs.Try();
        if (rhsRes.IsFaulted) return rhsRes;
        return TypeDesc.Multiply(lhsRes.Value, rhsRes.Value, TypeDesc<T>.Default);
    };

    /// <summary>
    /// Divide Try(x) by Try(y).  If either of the Trys throw then the result is Fail
    /// For numeric values the behaviour is to divide the Trys (lhs / rhs)
    /// For Lst values the behaviour is to divide all combinations of values in both lists 
    /// to produce a new list
    /// Otherwise if the R type derives from IDivisible then the behaviour
    /// is to call lhs.Divide(rhs);
    /// </summary>
    /// <param name="lhs">Left-hand side of the operation</param>
    /// <param name="rhs">Right-hand side of the operation</param>
    /// <returns>lhs / rhs</returns>
    public static Try<T> Divide<T>(this Try<T> lhs, Try<T> rhs) => () =>
    {
        var lhsRes = lhs.Try();
        if (lhsRes.IsFaulted) return lhsRes;
        var rhsRes = rhs.Try();
        if (rhsRes.IsFaulted) return rhsRes;
        return TypeDesc.Divide(lhsRes.Value, rhsRes.Value, TypeDesc<T>.Default);
    };

    /// <summary>
    /// Apply a Try value to a Try function
    /// </summary>
    /// <param name="self">Try function</param>
    /// <param name="arg">Try argument</param>
    /// <returns>Returns the result of applying the Try argument to the Try function</returns>
    public static Try<R> Apply<T, R>(this Try<Func<T, R>> self, Try<T> arg) => () =>
    {
        var res = self.Try();
        if (res.IsFaulted) return new TryResult<R>(res.Exception);
        var val = arg.Try();
        if (val.IsFaulted) return new TryResult<R>(val.Exception);
        return new TryResult<R>(res.Value(val.Value));
    };

    /// <summary>
    /// Apply a Try value to a Try function of arity 2
    /// </summary>
    /// <param name="self">Try function</param>
    /// <param name="arg">Try argument</param>
    /// <returns>Returns the result of applying the Try argument to the Try function:
    /// a Try function of arity 1</returns>
    public static Try<Func<T2, R>> Apply<T1, T2, R>(this Try<Func<T1, T2, R>> self, Try<T1> arg) => () =>
    {
        var res = self.Try();
        if (res.IsFaulted) return new TryResult<Func<T2, R>>(res.Exception);
        var val = arg.Try();
        if (val.IsFaulted) return new TryResult<Func<T2, R>>(val.Exception);
        return new TryResult<Func<T2, R>>(par(res.Value,val.Value));
    };

    /// <summary>
    /// Apply Try values to a Try function of arity 2
    /// </summary>
    /// <param name="self">Try function</param>
    /// <param name="arg1">Try argument</param>
    /// <param name="arg2">Try argument</param>
    /// <returns>Returns the result of applying the Try arguments to the Try function</returns>
    public static Try<R> Apply<T1, T2, R>(this Try<Func<T1, T2, R>> self, Try<T1> arg1, Try<T2> arg2) => () =>
    {
        var res = self.Try();
        if (res.IsFaulted) return new TryResult<R>(res.Exception);
        var val1 = arg1.Try();
        if (val1.IsFaulted) return new TryResult<R>(val1.Exception);
        var val2 = arg2.Try();
        if (val2.IsFaulted) return new TryResult<R>(val2.Exception);
        return new TryResult<R>(res.Value(val1.Value, val2.Value));
    };

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
        if (isnull(defaultValue)) throw new ArgumentNullException(nameof(defaultValue));

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

    /// <summary>
    /// Returns the Succ(value) of the Try or a default if it's Fail
    /// </summary>
    public static T IfFail<T>(this Try<T> self, Func<Exception, T> defaultAction)
    {
        var res = self.Try();
        if (res.IsFaulted)
            return defaultAction(res.Exception);
        else
            return res.Value;
    }

    /// <summary>
    /// Returns an exception matching context.  Call a chain of With<ExceptionType>() to handle specific
    /// exceptions, followed by Otherwise or OtherwiseThrow()
    /// </summary>
    public static ExceptionMatch<T> IfFail<T>(this Try<T> self)
    {
        var res = self.Try();
        if (res.IsFaulted)
            return res.Exception.Match<T>();
        else
            return new ExceptionMatch<T>(res.Value);
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
        if (isnull(Fail)) throw new ArgumentNullException(nameof(Fail));

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

    public static Try<U> Use<T,U>(this Try<T> self, Func<T, U> select)
        where T : IDisposable
    {
        return () =>
        {
            var t = self.Try();
            if (t.IsFaulted)
            {
                return new TryResult<U>(t.Exception);
            }
            try
            {
                return select(t.Value);
            }
            finally
            {
                t.Value.Dispose();
            }
        };
    }


    public static Try<T> Flatten<T>(this Try<Try<T>> self) => () =>
    {
        var res1 = self.Try();
        if (res1.IsFaulted)
        {
            return new TryResult<T>(res1.Exception);
        }
        else
        {
            var res2 = res1.Value.Try();
            if (res2.IsFaulted)
            {
                return new TryResult<T>(res2.Exception);
            }
            else
            {
                return new TryResult<T>(res2.Value);
            }
        }
    };

    public static Try<T> Flatten<T>(this Try<Try<Try<T>>> self)
    {
        var res = self.Try();
        if (res.IsFaulted)
        {
            return () => new TryResult<T>(res.Exception);
        }
        else
        {
            return res.Value.Flatten();
        }
    }

    public static Try<T> Flatten<T>(this Try<Try<Try<Try<T>>>> self)
    {
        var res = self.Try();
        if (res.IsFaulted)
        {
            return () => new TryResult<T>(res.Exception);
        }
        else
        {
            return res.Value.Flatten();
        }
    }

    public static Try<U> Use<T, U>(this Try<T> self, Func<T, Try<U>> select)
        where T : IDisposable
    {
        return () =>
        {
            var t = self.Try();
            if (t.IsFaulted)
            {
                return new TryResult<U>(t.Exception);
            }
            try
            {
                return select(t.Value).Try();
            }
            finally
            {
                t.Value.Dispose();
            }
        };
    }

    public static Unit Iter<T>(this Try<T> self, Action<T> action) =>
        self.IfSucc(action);

    public static Unit Iter<T>(this Try<T> self, Action<T> Succ, Action<Exception> Fail)
    {
        var res = self.Try();
        if (res.IsFaulted)
        {
            Fail(res.Exception);
        }
        else
        {
            Succ(res.Value);
        }
        return unit;
    }

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

    public static bool ForAll<T>(this Try<T> self, Func<T, bool> Succ, Func<Exception, bool> Fail)
    {
        var res = self.Try();
        return res.IsFaulted
            ? Fail(res.Exception)
            : Succ(res.Value);
    }

    /// <summary>
    /// Folds Try value into an S.
    /// https://en.wikipedia.org/wiki/Fold_(higher-order_function)
    /// </summary>
    /// <param name="self">Try to fold</param>
    /// <param name="state">Initial state</param>
    /// <param name="folder">Fold function</param>
    /// <returns>Folded state</returns>
    public static S Fold<S, T>(this Try<T> self, S state, Func<S, T, S> folder)
    {
        var res = self.Try();
        return res.IsFaulted
            ? state
            : folder(state, res.Value);
    }

    /// <summary>
    /// Folds Try value into an S.
    /// https://en.wikipedia.org/wiki/Fold_(higher-order_function)
    /// </summary>
    /// <param name="self">Try to fold</param>
    /// <param name="state">Initial state</param>
    /// <param name="Succ">Fold function for Success</param>
    /// <param name="Fail">Fold function for Failure</param>
    /// <returns>Folded state</returns>
    public static S Fold<S, T>(this Try<T> self, S state, Func<S, T, S> Succ, Func<S, Exception, S> Fail)
    {
        var res = self.Try();
        return res.IsFaulted
            ? Fail(state, res.Exception)
            : Succ(state, res.Value);
    }

    public static bool Exists<T>(this Try<T> self, Func<T, bool> pred)
    {
        var res = self.Try();
        return res.IsFaulted
            ? false
            : pred(res.Value);
    }

    public static bool Exists<T>(this Try<T> self, Func<T, bool> Succ, Func<Exception, bool> Fail)
    {
        var res = self.Try();
        return res.IsFaulted
            ? Fail(res.Exception)
            : Succ(res.Value);
    }

    public static Try<R> Map<T, R>(this Try<T> self, Func<T, R> mapper) => () =>
    {
        var res = self.Try();
        return res.IsFaulted
            ? new TryResult<R>(res.Exception)
            : mapper(res.Value);
    };

    public static Try<R> Map<T, R>(this Try<T> self, Func<T, R> Succ, Func<Exception, R> Fail) => () =>
    {
        var res = self.Try();
        return res.IsFaulted
            ? Fail(res.Exception)
            : Succ(res.Value);
    };

    /// <summary>
    /// Partial application map
    /// </summary>
    /// <remarks>TODO: Better documentation of this function</remarks>
    public static Try<Func<T2, R>> Map<T1, T2, R>(this Try<T1> self, Func<T1, T2, R> func) =>
        self.Map(curry(func));

    /// <summary>
    /// Partial application map
    /// </summary>
    /// <remarks>TODO: Better documentation of this function</remarks>
    public static Try<Func<T2, Func<T3, R>>> Map<T1, T2, T3, R>(this Try<T1> self, Func<T1, T2, T3, R> func) =>
        self.Map(curry(func));

    public static Try<T> Filter<T>(this Try<T> self, Func<T, bool> pred)
    {
        var res = self.Try();
        return res.IsFaulted
            ? () => res
            : pred(res.Value)
                ? self
                : () => new TryResult<T>(new BottomException());
    }

    public static Try<T> Filter<T>(this Try<T> self, Func<T, bool> Succ, Func<Exception, bool> Fail)
    {
        var res = self.Try();
        return res.IsFaulted
            ? Fail(res.Exception)
                ? self
                : () => new TryResult<T>(new BottomException())
            : Succ(res.Value)
                ? self
                : () => new TryResult<T>(new BottomException());
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

    public static Try<R> Bind<T, R>(this Try<T> self, Func<T, Try<R>> Succ, Func<Exception, Try<R>> Fail) => () =>
    {
        var res = self.Try();
        return res.IsFaulted
            ? Fail(res.Exception).Try()
            : Succ(res.Value).Try();
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

    public static Either<Exception, T>[] ToArray<T>(this Try<T> self) =>
        toArray(self.AsEnumerable());

    public static TrySuccContext<T, R> Succ<T, R>(this Try<T> self, Func<T, R> succHandler) =>
        new TrySuccContext<T, R>(self, succHandler);

    public static TrySuccUnitContext<T> Succ<T>(this Try<T> self, Action<T> succHandler) =>
        new TrySuccUnitContext<T>(self, succHandler);

    public static int Sum(this Try<int> self) =>
        self.Try().Value;

    public static string AsString<T>(this Try<T> self) =>
        match(self,
            Succ: v => isnull(v)
                      ? "Succ(null)"
                      : $"Succ({v})",
            Fail: ex => $"Fail({ex.Message})"
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
                var resT = self.Try();
                if (resT.IsFaulted) return new TryResult<V>(resT.Exception);

                var resU = bind(resT.Value).Try();
                if (resU.IsFaulted)
                {
                    return new TryResult<V>(resT.Exception);
                }
                try
                {
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
