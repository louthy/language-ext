using System;
using System.Diagnostics.Contracts;
using System.Linq;

namespace LanguageExt
{
    public static partial class Prelude
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
        [Pure]
        public static Try<T> append<T>(Try<T> lhs, Try<T> rhs) =>
            lhs.Append(rhs);

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
        [Pure]
        public static Try<T> subtract<T>(Try<T> lhs, Try<T> rhs) =>
            lhs.Subtract(rhs);

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
        [Pure]
        public static Try<T> multiply<T>(Try<T> lhs, Try<T> rhs) =>
            lhs.Multiply(rhs);

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
        [Pure]
        public static Try<T> divide<T>(Try<T> lhs, Try<T> rhs) =>
            lhs.Divide(rhs);

        [Pure]
        public static bool isSucc<T>(Try<T> value) =>
            !isFail(value);

        [Pure]
        public static bool isFail<T>(Try<T> value) =>
            value.Try().IsFaulted;

        public static Unit ifSucc<T>(Try<T> tryDel, Action<T> Succ) =>
            tryDel.IfSucc(Succ);

        [Pure]
        public static T ifFail<T>(Try<T> tryDel, Func<T> Fail) =>
            tryDel.IfFail(Fail);

        [Pure]
        public static T ifFail<T>(Try<T> tryDel, T failValue) =>
            tryDel.IfFail(failValue);

        [Pure]
        public static ExceptionMatch<T> ifFail<T>(Try<T> tryDel) =>
            tryDel.IfFail();

        [Pure]
        public static Try<Exception> failed<T>(Try<T> tryDel) =>
            map(tryDel, 
                Succ: _  => new NotSupportedException(),
                Fail: ex => ex
                );

        [Pure]
        public static Try<T> flatten<T>(Try<Try<T>> tryDel) =>
            tryDel.Flatten();

        [Pure]
        public static Try<T> flatten<T>(Try<Try<Try<T>>> tryDel) =>
            tryDel.Flatten();

        [Pure]
        public static Try<T> flatten<T>(Try<Try<Try<Try<T>>>> tryDel) =>
            tryDel.Flatten();

        [Pure]
        public static R match<T, R>(Try<T> tryDel, Func<T, R> Succ, Func<Exception, R> Fail) =>
            tryDel.Match(Succ, Fail);

        [Pure]
        public static R match<T, R>(Try<T> tryDel, Func<T, R> Succ, R Fail) =>
            tryDel.Match(Succ, Fail);

        public static Unit match<T>(Try<T> tryDel, Action<T> Succ, Action<Exception> Fail) =>
            tryDel.Match(Succ, Fail);

        /// <summary>
        /// Apply a Try value to a Try function
        /// </summary>
        /// <param name="self">Try function</param>
        /// <param name="arg">Try argument</param>
        /// <returns>Returns the result of applying the Try argument to the Try function</returns>
        [Pure]
        public static Try<R> apply<T, R>(Try<Func<T, R>> self, Try<T> arg) =>
            self.Apply(arg);

        /// <summary>
        /// Apply a Try value to a Try function of arity 2
        /// </summary>
        /// <param name="self">Try function</param>
        /// <param name="arg">Try argument</param>
        /// <returns>Returns the result of applying the Try argument to the Try function:
        /// a Try function of arity 1</returns>
        [Pure]
        public static Try<Func<T2, R>> apply<T1, T2, R>(Try<Func<T1, T2, R>> self, Try<T1> arg) =>
            self.Apply(arg);

        /// <summary>
        /// Apply Try values to a Try function of arity 2
        /// </summary>
        /// <param name="self">Try function</param>
        /// <param name="arg1">Try argument</param>
        /// <param name="arg2">Try argument</param>
        /// <returns>Returns the result of applying the Try arguments to the Try function</returns>
        [Pure]
        public static Try<R> apply<T1, T2, R>(Try<Func<T1, T2, R>> self, Try<T1> arg1, Try<T2> arg2) =>
            self.Apply(arg1, arg2);

        public static Unit iter<T>(Try<T> self, Action<T> action) =>
            self.Iter(action);

        public static Unit iter<T>(Try<T> self, Action<T> Succ, Action<Exception> Fail) =>
            self.Iter(Succ, Fail);

        /// <summary>
        /// Folds the value of Try into an S.
        /// https://en.wikipedia.org/wiki/Fold_(higher-order_function)
        /// </summary>
        /// <param name="tryDel">Try to fold</param>
        /// <param name="state">Initial state</param>
        /// <param name="folder">Fold function</param>
        /// <returns>Folded state</returns>
        [Pure]
        public static S fold<S, T>(Try<T> tryDel, S state, Func<S, T, S> folder) =>
            tryDel.Fold(state, folder);

        /// <summary>
        /// Folds the result of Try into an S.
        /// https://en.wikipedia.org/wiki/Fold_(higher-order_function)
        /// </summary>
        /// <param name="tryDel">Try to fold</param>
        /// <param name="state">Initial state</param>
        /// <param name="Succ">Fold function when Try succeeds</param>
        /// <param name="Fail">Fold function when Try fails</param>
        /// <returns>Folded state</returns>
        [Pure]
        public static S fold<S, T>(Try<T> self, S state, Func<S, T, S> Succ, Func<S, Exception, S> Fail) =>
            self.Fold(state, Succ, Fail);

        [Pure]
        public static bool forall<T>(Try<T> tryDel, Func<T, bool> pred) =>
            tryDel.ForAll(pred);

        [Pure]
        public static bool forall<T>(Try<T> tryDel, Func<T, bool> Succ, Func<Exception, bool> Fail) =>
            tryDel.ForAll(Succ,Fail);

        [Pure]
        public static int count<T>(Try<T> tryDel) =>
            tryDel.Count();

        [Pure]
        public static bool exists<T>(Try<T> tryDel, Func<T, bool> pred) =>
            tryDel.Exists(pred);

        [Pure]
        public static bool exists<T>(Try<T> self, Func<T, bool> Succ, Func<Exception, bool> Fail) =>
            self.Exists(Succ,Fail);

        [Pure]
        public static Try<R> map<T, R>(Try<T> tryDel, Func<T, R> mapper) =>
            tryDel.Map(mapper);

        [Pure]
        public static Try<R> map<T, R>(Try<T> tryDel, Func<T, R> Succ, Func<Exception, R> Fail) =>
            tryDel.Map(Succ, Fail);

        /// <summary>
        /// Partial application map
        /// </summary>
        /// <remarks>TODO: Better documentation of this function</remarks>
        [Pure]
        public static Try<Func<T2, R>> parmap<T1, T2, R>(Try<T1> self, Func<T1, T2, R> func) =>
            self.ParMap(func);

        /// <summary>
        /// Partial application map
        /// </summary>
        /// <remarks>TODO: Better documentation of this function</remarks>
        [Pure]
        public static Try<Func<T2, Func<T3, R>>> parmap<T1, T2, T3, R>(Try<T1> self, Func<T1, T2, T3, R> func) =>
            self.ParMap(func);

        [Pure]
        public static Try<T> filter<T>(Try<T> self, Func<T, bool> pred) =>
            self.Filter(pred);

        [Pure]
        public static Try<T> filter<T>(Try<T> self, Func<T, bool> Succ, Func<Exception, bool> Fail) =>
            self.Filter(Succ, Fail);

        [Pure]
        public static Try<R> bind<T, R>(Try<T> tryDel, Func<T, Try<R>> binder) =>
            tryDel.Bind(binder);

        [Pure]
        public static Try<R> bind<T, R>(Try<T> self, Func<T, Try<R>> Succ, Func<Exception, Try<R>> Fail) =>
            self.Bind(Succ, Fail);

        [Pure]
        public static Lst<Either<Exception, T>> toList<T>(Try<T> tryDel) =>
            tryDel.ToList();

        [Pure]
        public static Either<Exception, T>[] toArray<T>(Try<T> tryDel) =>
            tryDel.ToArray();

        [Pure]
        public static IQueryable<Either<Exception, T>> toQuery<T>(Try<T> tryDel) =>
            tryDel.ToList().AsQueryable();

        [Pure]
        public static Try<T> tryfun<T>(Func<Try<T>> tryDel) => () =>
        {
            try
            {
                return tryDel().Try();
            }
            catch (Exception e)
            {
                return new TryResult<T>(e);
            }
        };

        [Pure]
        public static Try<T> Try<T>(Func<T> tryDel) => () =>
            tryDel();

    }
}
