using System;
using System.Linq.Expressions;
using System.Collections.Concurrent;
using System.Linq;

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
        public static TryOption<T> subtract<T>(TryOption<T> lhs, TryOption<T> rhs) =>
            lhs.Subtract(rhs);

        /// <summary>
        /// Find the product of TryOption(x) and TryOption(y).
        /// If either of the TryOptions throw then the result is Fail
        /// If either of the TryOptions return None then the result is None
        /// For numeric values the behaviour is to multiply the TryOptions (lhs * rhs)
        /// For Lst values the behaviour is to multiply all combinations of values in both lists 
        /// to produce a new list
        /// Otherwise if the R type derives from IProductable then the behaviour
        /// is to call lhs.Product(rhs);
        /// </summary>
        /// <param name="lhs">Left-hand side of the operation</param>
        /// <param name="rhs">Right-hand side of the operation</param>
        /// <returns>lhs * rhs</returns>
        public static TryOption<T> product<T>(TryOption<T> lhs, TryOption<T> rhs) =>
            lhs.Product(rhs);

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
        public static TryOption<T> divide<T>(TryOption<T> lhs, TryOption<T> rhs) =>
            lhs.Divide(rhs);

        public static bool isSome<T>(TryOption<T> value) =>
            value.Try().Value.IsSome;

        public static bool isNone<T>(TryOption<T> value) =>
            value.Try().Value.IsNone;

        public static Unit ifSome<T>(TryOption<T> tryDel, Action<T> Some) =>
            tryDel.IfSome(Some);

        public static T ifNone<T>(TryOption<T> tryDel, Func<T> None) =>
            tryDel.IfNone(None);

        public static T ifNone<T>(TryOption<T> tryDel, T noneValue) =>
            tryDel.IfNone(noneValue);

        public static T ifNoneOrFail<T>(TryOption<T> tryDel, Func<T> None, Func<Exception,T> Fail) =>
            tryDel.IfNoneOrFail(None,Fail);

        public static R match<T, R>(TryOption<T> tryDel, Func<T, R> Some, Func<R> None, Func<Exception, R> Fail) =>
            tryDel.Match(Some, None, Fail);

        public static R match<T, R>(TryOption<T> tryDel, Func<T, R> Some, R None, Func<Exception, R> Fail) =>
            tryDel.Match(Some, None, Fail);

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
        public static TryOption<R> apply<T, R>(TryOption<Func<T, R>> self, TryOption<T> arg) =>
            self.Apply(arg);

        /// <summary>
        /// Apply a TryOption value to a TryOption function of arity 2
        /// </summary>
        /// <param name="self">TryOption function</param>
        /// <param name="arg">TryOption argument</param>
        /// <returns>Returns the result of applying the TryOption argument to the TryOption function:
        /// a TryOption function of arity 1</returns>
        public static TryOption<Func<T2, R>> apply<T1, T2, R>(TryOption<Func<T1, T2, R>> self, TryOption<T1> arg) =>
            self.Apply(arg);

        /// <summary>
        /// Apply TryOption values to a TryOption function of arity 2
        /// </summary>
        /// <param name="self">TryOption function</param>
        /// <param name="arg1">TryOption argument</param>
        /// <param name="arg2">TryOption argument</param>
        /// <returns>Returns the result of applying the TryOption arguments to TryOption Try function</returns>
        public static TryOption<R> apply<T1, T2, R>(TryOption<Func<T1, T2, R>> self, TryOption<T1> arg1, TryOption<T2> arg2) =>
            self.Apply(arg1, arg2);

        public static Unit iter<T>(TryOption<T> self, Action<T> action) =>
            self.Iter(action);

        public static Unit iter<T>(TryOption<T> self, Action<T> Some, Action None, Action<Exception> Fail) =>
            self.Iter(Some, None, Fail);

        public static S fold<S, T>(TryOption<T> tryDel, S state, Func<S, T, S> folder) =>
            tryDel.Fold(state, folder);

        public static S fold<S, T>(TryOption<T> tryDel, S state, Func<S, T, S> Some, Func<S, S> None, Func<S, Exception, S> Fail) =>
            tryDel.Fold(state, Some, None, Fail);

        public static bool forall<T>(TryOption<T> tryDel, Func<T, bool> pred) =>
            tryDel.ForAll(pred);

        public static bool forall<T>(TryOption<T> tryDel, Func<T, bool> Some, Func<bool> None, Func<Exception, bool> Fail) =>
            tryDel.ForAll(Some, None, Fail);

        public static int count<T>(TryOption<T> tryDel) =>
            tryDel.Count();

        public static bool exists<T>(TryOption<T> tryDel, Func<T, bool> pred) =>
            tryDel.Exists(pred);

        public static bool exists<T>(TryOption<T> tryDel, Func<T, bool> Some, Func<bool> None, Func<Exception, bool> Fail) =>
            tryDel.Exists(Some, None, Fail);

        public static TryOption<R> map<T, R>(TryOption<T> tryDel, Func<T, R> mapper) =>
            tryDel.Map(mapper);

        public static TryOption<R> map<T, R>(TryOption<T> tryDel, Func<T, R> Some, Func<R> None, Func<Exception, R> Fail) =>
            tryDel.Map(Some, None, Fail);

        /// <summary>
        /// Partial application map
        /// </summary>
        /// <remarks>TODO: Better documentation of this function</remarks>
        public static TryOption<Func<T2, R>> map<T1, T2, R>(TryOption<T1> self, Func<T1, T2, R> func) =>
            self.Map(func);

        /// <summary>
        /// Partial application map
        /// </summary>
        /// <remarks>TODO: Better documentation of this function</remarks>
        public static TryOption<Func<T2, Func<T3, R>>> map<T1, T2, T3, R>(TryOption<T1> self, Func<T1, T2, T3, R> func) =>
            self.Map(func);

        public static TryOption<R> bind<T, R>(TryOption<T> tryDel, Func<T, TryOption<R>> binder) =>
            tryDel.Bind(binder);

        public static TryOption<R> bind<T, R>(TryOption<T> tryDel, Func<T, TryOption<R>> Some, Func<TryOption<R>> None, Func<Exception, TryOption<R>> Fail) =>
            tryDel.Bind(Some, None, Fail);

        public static Lst<Either<Exception, T>> toList<T>(TryOption<T> tryDel) =>
            tryDel.ToList();

        public static Either<Exception, T>[] toArray<T>(TryOption<T> tryDel) =>
            tryDel.ToArray();

        public static IQueryable<Either<Exception, T>> toQuery<T>(TryOption<T> tryDel) =>
            tryDel.ToList().AsQueryable();

        public static TryOption<T> tryfun<T>(Func<TryOption<T>> tryDel) => () => 
            tryDel()();
    }
}
