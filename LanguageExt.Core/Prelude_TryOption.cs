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
        public static T failure<T>(TryOption<T> tryDel, Func<T> Fail) =>
            tryDel.Failure(Fail);

        public static T failure<T>(TryOption<T> tryDel, T failValue) =>
            tryDel.Failure(failValue);

        public static R match<T, R>(TryOption<T> tryDel, Func<T, R> Some, Func<R> None, Func<Exception, R> Fail) =>
            tryDel.Match(Some, None, Fail);

        public static R match<T, R>(TryOption<T> tryDel, Func<T, R> Some, R None, Func<Exception, R> Fail) =>
            tryDel.Match(Some, None, Fail);

        public static R match<T, R>(TryOption<T> tryDel, Func<T, R> Some, Func<R> None, R Fail) =>
            tryDel.Match(Some, None, Fail);

        public static Unit match<T>(TryOption<T> tryDel, Action<T> Some, Action None, Action<Exception> Fail) =>
            tryDel.Match(Some, None, Fail);

        public static S fold<S, T>(TryOption<T> tryDel, S state, Func<S, T, S> folder) =>
            tryDel.Fold(state, folder);

        public static bool forall<T>(TryOption<T> tryDel, Func<T, bool> pred) =>
            tryDel.ForAll(pred);

        public static int count<T>(TryOption<T> tryDel) =>
            tryDel.Count();

        public static bool exists<T>(TryOption<T> tryDel, Func<T, bool> pred) =>
            tryDel.Exists(pred);

        public static TryOption<R> map<T, R>(TryOption<T> tryDel, Func<T, R> mapper) =>
            tryDel.Map(mapper);

        public static TryOption<R> bind<T, R>(TryOption<T> tryDel, Func<T, TryOption<R>> binder) =>
            tryDel.Bind(binder);

        public static IImmutableList<Either<Exception, T>> toList<T>(TryOption<T> tryDel) =>
            tryDel.ToList();

        public static ImmutableArray<Either<Exception, T>> toArray<T>(TryOption<T> tryDel) =>
            tryDel.ToArray();

        public static IQueryable<Either<Exception, T>> toQuery<T>(TryOption<T> tryDel) =>
            tryDel.ToList().AsQueryable();
            
    }
}
