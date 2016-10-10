using System;
using System.Linq.Expressions;
using System.Collections.Concurrent;
using System.Linq;
using System.Diagnostics.Contracts;

namespace LanguageExt
{
    public static partial class Prelude
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
        public static TryOption<T> append<T>(TryOption<T> lhs, TryOption<T> rhs) =>
            lhs.Append(rhs);

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
        public static TryOption<T> subtract<T>(TryOption<T> lhs, TryOption<T> rhs) =>
            lhs.Subtract(rhs);

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
        public static TryOption<T> multiply<T>(TryOption<T> lhs, TryOption<T> rhs) =>
            lhs.Multiply(rhs);

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
        public static TryOption<T> divide<T>(TryOption<T> lhs, TryOption<T> rhs) =>
            lhs.Divide(rhs);

        [Pure]
        public static bool isSome<T>(TryOption<T> value) =>
            value.Try().Value.IsSome;

        [Pure]
        public static bool isNone<T>(TryOption<T> value) =>
            value.Try().Value.IsNone;

        public static Unit ifSome<T>(TryOption<T> tryDel, Action<T> Some) =>
            tryDel.IfSome(Some);

        [Pure]
        public static T ifNone<T>(TryOption<T> tryDel, Func<T> None) =>
            tryDel.IfNone(None);

        [Pure]
        public static T ifNone<T>(TryOption<T> tryDel, T noneValue) =>
            tryDel.IfNone(noneValue);

        [Pure]
        public static T ifNoneOrFail<T>(TryOption<T> tryDel, Func<T> None, Func<Exception,T> Fail) =>
            tryDel.IfNoneOrFail(None,Fail);

        [Pure]
        public static R match<T, R>(TryOption<T> tryDel, Func<T, R> Some, Func<R> None, Func<Exception, R> Fail) =>
            tryDel.Match(Some, None, Fail);

        [Pure]
        public static R match<T, R>(TryOption<T> tryDel, Func<T, R> Some, R None, Func<Exception, R> Fail) =>
            tryDel.Match(Some, None, Fail);

        [Pure]
        public static R match<T, R>(TryOption<T> tryDel, Func<T, R> Some, Func<R> None, R Fail) =>
            tryDel.Match(Some, None, Fail);

        public static Unit match<T>(TryOption<T> tryDel, Action<T> Some, Action None, Action<Exception> Fail) =>
            tryDel.Match(Some, None, Fail);

        /// <summary>
        /// Apply a TryOption value to a TryOption function
        /// </summary>
        /// <param name="self">TryOption function</param>
        /// <param name="arg">TryOption argument</param>
        /// <returns>Returns the result of applying the TryOption argument to the TryOption function</returns>
        [Pure]
        public static TryOption<R> apply<T, R>(TryOption<Func<T, R>> self, TryOption<T> arg) =>
            self.Apply(arg);

        /// <summary>
        /// Apply a TryOption value to a TryOption function of arity 2
        /// </summary>
        /// <param name="self">TryOption function</param>
        /// <param name="arg">TryOption argument</param>
        /// <returns>Returns the result of applying the TryOption argument to the TryOption function:
        /// a TryOption function of arity 1</returns>
        [Pure]
        public static TryOption<Func<T2, R>> apply<T1, T2, R>(TryOption<Func<T1, T2, R>> self, TryOption<T1> arg) =>
            self.Apply(arg);

        /// <summary>
        /// Apply TryOption values to a TryOption function of arity 2
        /// </summary>
        /// <param name="self">TryOption function</param>
        /// <param name="arg1">TryOption argument</param>
        /// <param name="arg2">TryOption argument</param>
        /// <returns>Returns the result of applying the TryOption arguments to TryOption Try function</returns>
        [Pure]
        public static TryOption<R> apply<T1, T2, R>(TryOption<Func<T1, T2, R>> self, TryOption<T1> arg1, TryOption<T2> arg2) =>
            self.Apply(arg1, arg2);

        public static Unit iter<T>(TryOption<T> self, Action<T> action) =>
            self.Iter(action);

        public static Unit iter<T>(TryOption<T> self, Action<T> Some, Action None, Action<Exception> Fail) =>
            self.Iter(Some, None, Fail);

        /// <summary>
        /// Folds the value of TryOption into an S.
        /// https://en.wikipedia.org/wiki/Fold_(higher-order_function)
        /// </summary>
        /// <param name="tryDel">Try to fold</param>
        /// <param name="state">Initial state</param>
        /// <param name="folder">Fold function</param>
        /// <returns>Folded state</returns>
        [Pure]
        public static S fold<S, T>(TryOption<T> tryDel, S state, Func<S, T, S> folder) =>
            tryDel.Fold(state, folder);

        /// <summary>
        /// Folds the result of TryOption into an S.
        /// https://en.wikipedia.org/wiki/Fold_(higher-order_function)
        /// </summary>
        /// <param name="tryDel">Try to fold</param>
        /// <param name="state">Initial state</param>
        /// <param name="Some">Fold function for Some</param>
        /// <param name="None">Fold function for None</param>
        /// <param name="Fail">Fold function for Failure</param>
        /// <returns>Folded state</returns>
        [Pure]
        public static S fold<S, T>(TryOption<T> tryDel, S state, Func<S, T, S> Some, Func<S, S> None, Func<S, Exception, S> Fail) =>
            tryDel.Fold(state, Some, None, Fail);

        [Pure]
        public static bool forall<T>(TryOption<T> tryDel, Func<T, bool> pred) =>
            tryDel.ForAll(pred);

        [Pure]
        public static bool forall<T>(TryOption<T> tryDel, Func<T, bool> Some, Func<bool> None, Func<Exception, bool> Fail) =>
            tryDel.ForAll(Some, None, Fail);

        [Pure]
        public static int count<T>(TryOption<T> tryDel) =>
            tryDel.Count();

        [Pure]
        public static bool exists<T>(TryOption<T> tryDel, Func<T, bool> pred) =>
            tryDel.Exists(pred);

        [Pure]
        public static bool exists<T>(TryOption<T> tryDel, Func<T, bool> Some, Func<bool> None, Func<Exception, bool> Fail) =>
            tryDel.Exists(Some, None, Fail);

        [Pure]
        public static TryOption<R> map<T, R>(TryOption<T> tryDel, Func<T, R> mapper) =>
            tryDel.Map(mapper);

        [Pure]
        public static TryOption<R> map<T, R>(TryOption<T> tryDel, Func<T, R> Some, Func<R> None, Func<Exception, R> Fail) =>
            tryDel.Map(Some, None, Fail);

        /// <summary>
        /// Partial application map
        /// </summary>
        /// <remarks>TODO: Better documentation of this function</remarks>
        [Pure]
        public static TryOption<Func<T2, R>> parmap<T1, T2, R>(TryOption<T1> self, Func<T1, T2, R> func) =>
            self.ParMap(func);

        /// <summary>
        /// Partial application map
        /// </summary>
        /// <remarks>TODO: Better documentation of this function</remarks>
        [Pure]
        public static TryOption<Func<T2, Func<T3, R>>> parmap<T1, T2, T3, R>(TryOption<T1> self, Func<T1, T2, T3, R> func) =>
            self.ParMap(func);

        [Pure]
        public static TryOption<R> bind<T, R>(TryOption<T> tryDel, Func<T, TryOption<R>> binder) =>
            tryDel.Bind(binder);

        [Pure]
        public static TryOption<R> bind<T, R>(TryOption<T> tryDel, Func<T, TryOption<R>> Some, Func<TryOption<R>> None, Func<Exception, TryOption<R>> Fail) =>
            tryDel.Bind(Some, None, Fail);

        [Pure]
        public static Lst<Either<Exception, T>> toList<T>(TryOption<T> tryDel) =>
            tryDel.ToList();

        [Pure]
        public static Either<Exception, T>[] toArray<T>(TryOption<T> tryDel) =>
            tryDel.ToArray();

        [Pure]
        public static IQueryable<Either<Exception, T>> toQuery<T>(TryOption<T> tryDel) =>
            tryDel.ToList().AsQueryable();

        [Pure]
        public static TryOption<T> tryfun<T>(Func<TryOption<T>> tryDel) => () => 
            tryDel()();
    }
}
