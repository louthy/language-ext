using System;
using System.Linq.Expressions;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Linq;

namespace LanguageExt
{
    /// <summary>
    /// Usage:  Add 'using LanguageExt.Prelude' to your code.
    /// </summary>
    public static partial class Prelude
    {
        public static bool isSucc<T>(Try<T> value) =>
            !isFail(value);

        public static bool isFail<T>(Try<T> value) =>
            value.Try().IsFaulted;

        public static Unit ifSucc<T>(Try<T> tryDel, Action<T> Succ) =>
            tryDel.IfSucc(Succ);

        public static T ifFail<T>(Try<T> tryDel, Func<T> Fail) =>
            tryDel.IfFail(Fail);

        public static T ifFail<T>(Try<T> tryDel, T failValue) =>
            tryDel.IfFail(failValue);

        public static R match<T, R>(Try<T> tryDel, Func<T, R> Succ, Func<Exception, R> Fail) =>
            tryDel.Match(Succ, Fail);

        public static R match<T, R>(Try<T> tryDel, Func<T, R> Succ, R Fail) =>
            tryDel.Match(Succ, Fail);

        public static Unit match<T>(Try<T> tryDel, Action<T> Succ, Action<Exception> Fail) =>
            tryDel.Match(Succ, Fail);

        public static S fold<S, T>(Try<T> tryDel, S state, Func<S, T, S> folder) =>
            tryDel.Fold(state, folder);

        public static bool forall<T>(Try<T> tryDel, Func<T, bool> pred) =>
            tryDel.ForAll(pred);

        public static int count<T>(Try<T> tryDel) =>
            tryDel.Count();

        public static bool exists<T>(Try<T> tryDel, Func<T, bool> pred) =>
            tryDel.Exists(pred);

        public static Try<R> map<T, R>(Try<T> tryDel, Func<T, R> mapper) =>
            tryDel.Map(mapper);

        public static Try<R> bind<T, R>(Try<T> tryDel, Func<T, Try<R>> binder) =>
            tryDel.Bind(binder);

        public static Lst<Either<Exception, T>> toList<T>(Try<T> tryDel) =>
            tryDel.ToList();

        public static ImmutableArray<Either<Exception, T>> toArray<T>(Try<T> tryDel) =>
            tryDel.ToArray();

        public static IQueryable<Either<Exception, T>> toQuery<T>(Try<T> tryDel) =>
            tryDel.ToList().AsQueryable();

        public static Try<T> tryfun<T>(Func<Try<T>> tryDel) => () => 
            tryDel()();
    }
}
