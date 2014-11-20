using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq.Expressions;
using System.Collections.Concurrent;

namespace LanguageExt
{
    /// <summary>
    /// Usage:  Add 'using LanguageExt.Prelude' to your code.
    /// </summary>
    public static partial class Prelude
    {
        /// <summary>
        /// Create a Some of T (Option<T>)
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <param name="value">Non-null value to be made optional</param>
        /// <returns>Option<T> in a Some state or throws ValueIsNullException
        /// if value == null.</returns>
        public static Option<T> Some<T>(T value) =>
            value == null
                ? raise<Option<T>>(new ValueIsNullException())
                : Option<T>.Some(value);

        /// <summary>
        /// Create a Some of T from a Nullable<T> (Option<T>)
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <param name="value">Non-null value to be made optional</param>
        /// <returns>Option<T> in a Some state or throws ValueIsNullException
        /// if value == null</returns>
        public static Option<T> Some<T>(Nullable<T> value) where T : struct =>
            value.HasValue
                ? Option<T>.Some(value.Value) 
                : raise<Option<T>>(new ValueIsNullException());

        /// <summary>
        /// Create a Some of T (Option<T>).  Use the to wrap any-type without coercian.
        /// That means you can wrap null, Nullable<T>, or Option<T> to get Option<Option<T>>
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <param name="value">Value to make optional</param>
        /// <returns>Option<T> in a Some state</returns>
        public static OptionUnsafe<T> SomeUnsafe<T>(T value) =>
            OptionUnsafe<T>.Some(value);

        /// <summary>
        /// 'No value' state of Option<T>.  
        /// </summary>
        public static OptionNone None => 
            OptionNone.Default;


        public static Either<R, L> Right<R, L>(R value) =>
            Either<R, L>.Right(value);

        public static Either<R, L> Left<R, L>(L value) =>
            Either<R, L>.Left(value);

        public static Either<R, L> Right<R, L>(Nullable<R> value) where R : struct =>
            value == null
                ? raise<Either<R, L>>(new ValueIsNullException())
                : Either<R, L>.Right(value.Value);

        public static Either<R, L> Left<R, L>(Nullable<L> value) where L : struct =>
            value == null
                ? raise<Either<R, L>>(new ValueIsNullException())
                : Either<R, L>.Left(value.Value);


        public static EitherUnsafe<R, L> RightUnsafe<R, L>(R value) =>
            EitherUnsafe<R, L>.Right(value);

        public static EitherUnsafe<R, L> LeftUnsafe<R, L>(L value) =>
            EitherUnsafe<R, L>.Left(value);


        public static T failure<T>(Option<T> option, Func<T> None) =>
            option.Failure(None);

        public static T failure<T>(Option<T> option, T noneValue) =>
            option.Failure(noneValue);

        public static R match<T, R>(Option<T> option, Func<T, R> Some, Func<R> None) =>
            option.Match(Some, None);

        public static Unit match<T>(Option<T> option, Action<T> Some, Action None) =>
            option.Match(Some, None);


        public static T failureUnsafe<T>(OptionUnsafe<T> option, Func<T> None) =>
            option.FailureUnsafe(None);

        public static T failure<T>(OptionUnsafe<T> option, T noneValue) =>
            option.FailureUnsafe(noneValue);

        public static R matchUnsafe<T, R>(OptionUnsafe<T> option, Func<T, R> Some, Func<R> None) =>
            option.MatchUnsafe(Some, None);

        public static Unit matchUnsafe<T>(OptionUnsafe<T> option, Action<T> Some, Action None) =>
            option.MatchUnsafe(Some, None);


        public static R failure<R, L>(Either<R, L> either, Func<R> None) =>
            either.Failure(None);

        public static R failure<R, L>(Either<R, L> either, R noneValue) =>
            either.Failure(noneValue);

        public static Ret match<R, L, Ret>(Either<R, L> either, Func<R, Ret> Right, Func<L, Ret> Left) =>
            either.Match(Right, Left);

        public static Unit match<R, L>(Either<R, L> either, Action<R> Right, Action<L> Left) =>
            either.Match(Right, Left);


        public static R failureUnsafe<R, L>(EitherUnsafe<R, L> either, Func<R> None) =>
            either.FailureUnsafe(None);

        public static R failureUnsafe<R, L>(EitherUnsafe<R, L> either, R noneValue) =>
            either.FailureUnsafe(noneValue);

        public static Ret matchUnsafe<R, L, Ret>(EitherUnsafe<R, L> either, Func<R, Ret> Right, Func<L, Ret> Left) =>
            either.MatchUnsafe(Right, Left);

        public static Unit matchUnsafe<R, L>(EitherUnsafe<R, L> either, Action<R> Right, Action<L> Left) =>
            either.MatchUnsafe(Right, Left);


        public static T failure<T>(Try<T> tryDel, Func<T> Fail) =>
            tryDel.Failure(Fail);

        public static T failure<T>(Try<T> tryDel, T failValue) =>
            tryDel.Failure(failValue);

        public static R match<T, R>(Try<T> tryDel, Func<T, R> Succ, Func<Exception,R> Fail) =>
            tryDel.Match(Succ, Fail);

        public static Unit match<T>(Try<T> tryDel, Action<T> Succ, Action<Exception> Fail) =>
            tryDel.Match(Succ, Fail);


        public static Func<R> fun<R>(Func<R> f) => f;
        public static Func<T1, R> fun<T1, R>(Func<T1, R> f) => f;
        public static Func<T1, T2, R> fun<T1, T2, R>(Func<T1, T2, R> f) => f;
        public static Func<T1, T2, T3, R> fun<T1, T2, T3, R>(Func<T1, T2, T3, R> f) => f;
        public static Func<T1, T2, T3, T4, R> fun<T1, T2, T3, T4, R>(Func<T1, T2, T3, T4, R> f) => f;
        public static Func<T1, T2, T3, T4, T5, R> fun<T1, T2, T3, T4, T5, R>(Func<T1, T2, T3, T4, T5, R> f) => f;
        public static Func<T1, T2, T3, T4, T5, T6, R> fun<T1, T2, T3, T4, T5, T6, R>(Func<T1, T2, T3, T4, T5, T6, R> f) => f;
        public static Func<T1, T2, T3, T4, T5, T6, T7, R> fun<T1, T2, T3, T4, T5, T6, T7, R>(Func<T1, T2, T3, T4, T5, T6, T7, R> f) => f;

        public static Func<Unit> fun(Action f) => () => { f(); return unit; };
        public static Func<T1,Unit> fun<T1>(Action<T1> f) => (a1) => { f(a1); return unit; };
        public static Func<T1, T2, Unit> fun<T1, T2>(Action<T1, T2> f) => (a1,a2) => { f(a1,a2); return unit; };
        public static Func<T1, T2, T3, Unit> fun<T1, T2, T3>(Action<T1, T2, T3> f) => (a1, a2, a3) => { f(a1, a2, a3); return unit; };
        public static Func<T1, T2, T3, T4, Unit> fun<T1, T2, T3, T4>(Action<T1, T2, T3, T4> f) => (a1, a2, a3, a4) => { f(a1, a2, a3, a4); return unit; };
        public static Func<T1, T2, T3, T4, T5, Unit> fun<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> f) => (a1, a2, a3, a4, a5) => { f(a1, a2, a3, a4, a5); return unit; };
        public static Func<T1, T2, T3, T4, T5, T6, Unit> fun<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> f) => (a1, a2, a3, a4, a5, a6) => { f(a1, a2, a3, a4, a5, a6); return unit; };
        public static Func<T1, T2, T3, T4, T5, T6, T7, Unit> fun<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> f) => (a1, a2, a3, a4, a5, a6, a7) => { f(a1, a2, a3, a4, a5, a6, a7); return unit; };

        public static Action act(Action f) => f;
        public static Action<T1> act<T1>(Action<T1> f) => f;
        public static Action<T1, T2> act<T1, T2>(Action<T1, T2> f) => f;
        public static Action<T1, T2, T3> act<T1, T2, T3>(Action<T1, T2, T3> f) => f;
        public static Action<T1, T2, T3, T4> act<T1, T2, T3, T4>(Action<T1, T2, T3, T4> f) => f;
        public static Action<T1, T2, T3, T4, T5> act<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> f) => f;
        public static Action<T1, T2, T3, T4, T5, T6> act<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> f) => f;
        public static Action<T1, T2, T3, T4, T5, T6, T7> act<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> f) => f;

        public static Action act<R>(Func<R> f) => () => f();
        public static Action<T1> act<T1, R>(Func<T1, R> f) => a1 => f(a1);
        public static Action<T1, T2> act<T1, T2, R>(Func<T1, T2, R> f) => (a1,a2) => f(a1,a2);
        public static Action<T1, T2, T3> act<T1, T2, T3, R>(Func<T1, T2, T3, R> f) => (a1, a2, a3) => f(a1, a2, a3);
        public static Action<T1, T2, T3, T4> act<T1, T2, T3, T4, R>(Func<T1, T2, T3, T4, R> f) => (a1, a2, a3, a4) => f(a1, a2, a3, a4);
        public static Action<T1, T2, T3, T4, T5> act<T1, T2, T3, T4, T5, R>(Func<T1, T2, T3, T4, T5, R> f) => (a1, a2, a3, a4, a5) => f(a1, a2, a3, a4, a5);
        public static Action<T1, T2, T3, T4, T5, T6> act<T1, T2, T3, T4, T5, T6, R>(Func<T1, T2, T3, T4, T5, T6, R> f) => (a1, a2, a3, a4, a5, a6) => f(a1, a2, a3, a4, a5, a6);
        public static Action<T1, T2, T3, T4, T5, T6, T7> act<T1, T2, T3, T4, T5, T6, T7, R>(Func<T1, T2, T3, T4, T5, T6, T7, R> f) => (a1, a2, a3, a4, a5, a6, a7) => f(a1, a2, a3, a4, a5, a6, a7);


        public static Expression<Func<R>> expr<R>(Expression<Func<R>> f) => f;
        public static Expression<Func<T1, R>> expr<T1, R>(Expression<Func<T1, R>> f) => f;
        public static Expression<Func<T1, T2, R>> expr<T1, T2, R>(Expression<Func<T1, T2, R>> f) => f;
        public static Expression<Func<T1, T2, T3, R>> expr<T1, T2, T3, R>(Expression<Func<T1, T2, T3, R>> f) => f;
        public static Expression<Func<T1, T2, T3, T4, R>> expr<T1, T2, T3, T4, R>(Expression<Func<T1, T2, T3, T4, R>> f) => f;
        public static Expression<Func<T1, T2, T3, T4, T5, R>> expr<T1, T2, T3, T4, T5, R>(Expression<Func<T1, T2, T3, T4, T5, R>> f) => f;
        public static Expression<Func<T1, T2, T3, T4, T5, T6, R>> expr<T1, T2, T3, T4, T5, T6, R>(Expression<Func<T1, T2, T3, T4, T5, T6, R>> f) => f;
        public static Expression<Func<T1, T2, T3, T4, T5, T6, T7, R>> expr<T1, T2, T3, T4, T5, T6, T7, R>(Expression<Func<T1, T2, T3, T4, T5, T6, T7, R>> f) => f;
        public static Expression<Action> expr(Expression<Action> f) => f;
        public static Expression<Action<T1>> expr<T1>(Expression<Action<T1>> f) => f;
        public static Expression<Action<T1, T2>> expr<T1, T2>(Expression<Action<T1, T2>> f) => f;
        public static Expression<Action<T1, T2, T3>> expr<T1, T2, T3>(Expression<Action<T1, T2, T3>> f) => f;
        public static Expression<Action<T1, T2, T3, T4>> expr<T1, T2, T3, T4>(Expression<Action<T1, T2, T3, T4>> f) => f;
        public static Expression<Action<T1, T2, T3, T4, T5>> expr<T1, T2, T3, T4, T5>(Expression<Action<T1, T2, T3, T4, T5>> f) => f;
        public static Expression<Action<T1, T2, T3, T4, T5, T6>> expr<T1, T2, T3, T4, T5, T6>(Expression<Action<T1, T2, T3, T4, T5, T6>> f) => f;
        public static Expression<Action<T1, T2, T3, T4, T5, T6, T7>> expr<T1, T2, T3, T4, T5, T6, T7>(Expression<Action<T1, T2, T3, T4, T5, T6, T7>> f) => f;

        public static Unit unit => 
            Unit.Default;

        public static Unit ignore<T>(T anything) => 
            unit;

        public static Tuple<T1, T2> tuple<T1, T2>(T1 item1, T2 item2) =>
            Tuple.Create(item1, item2);

        public static Tuple<T1, T2, T3> tuple<T1, T2, T3>(T1 item1, T2 item2, T3 item3) =>
            Tuple.Create(item1, item2, item3);

        public static Tuple<T1, T2, T3, T4> tuple<T1, T2, T3, T4>(T1 item1, T2 item2, T3 item3, T4 item4) =>
            Tuple.Create(item1, item2, item3, item4);

        public static Tuple<T1, T2, T3, T4, T5> tuple<T1, T2, T3, T4, T5>(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5) =>
            Tuple.Create(item1, item2, item3, item4, item5);

        public static Tuple<T1, T2, T3, T4, T5, T6> tuple<T1, T2, T3, T4, T5, T6>(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6) =>
            Tuple.Create(item1, item2, item3, item4, item5, item6);

        public static Tuple<T1, T2, T3, T4, T5, T6, T7> tuple<T1, T2, T3, T4, T5, T6, T7>(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7) =>
            Tuple.Create(item1, item2, item3, item4, item5, item6, item7);

        public static R with<T1, T2, R>(this Tuple<T1, T2> self, Func<T1, T2, R> func) =>
            func(self.Item1, self.Item2);

        public static R with<T1, T2, T3, R>(this Tuple<T1, T2, T3> self, Func<T1, T2, T3, R> func) =>
            func(self.Item1, self.Item2, self.Item3);

        public static R with<T1, T2, T3, T4, R>(this Tuple<T1, T2, T3, T4> self, Func<T1, T2, T3, T4, R> func) =>
            func(self.Item1, self.Item2, self.Item3, self.Item4);

        public static R with<T1, T2, T3, T4, T5, R>(this Tuple<T1, T2, T3, T4, T5> self, Func<T1, T2, T3, T4, T5, R> func) =>
            func(self.Item1, self.Item2, self.Item3, self.Item4, self.Item5);

        public static R with<T1, T2, T3, T4, T5, T6, R>(this Tuple<T1, T2, T3, T4, T5, T6> self, Func<T1, T2, T3, T4, T5, T6, R> func) =>
            func(self.Item1, self.Item2, self.Item3, self.Item4, self.Item5, self.Item6);

        public static R with<T1, T2, T3, T4, T5, T6, T7, R>(this Tuple<T1, T2, T3, T4, T5, T6, T7> self, Func<T1, T2, T3, T4, T5, T6, T7, R> func) =>
            func(self.Item1, self.Item2, self.Item3, self.Item4, self.Item5, self.Item6, self.Item7);

        public static Unit with<T1, T2>(this Tuple<T1, T2> self, Action<T1, T2> func)
        {
            func(self.Item1, self.Item2);
            return Unit.Default;
        }

        public static Unit with<T1, T2, T3>(this Tuple<T1, T2, T3> self, Action<T1, T2, T3> func)
        {
            func(self.Item1, self.Item2, self.Item3);
            return Unit.Default;
        }

        public static Unit with<T1, T2, T3, T4>(this Tuple<T1, T2, T3, T4> self, Action<T1, T2, T3, T4> func)
        {
            func(self.Item1, self.Item2, self.Item3, self.Item4);
            return Unit.Default;
        }

        public static Unit with<T1, T2, T3, T4, T5>(this Tuple<T1, T2, T3, T4, T5> self, Action<T1, T2, T3, T4, T5> func)
        {
            func(self.Item1, self.Item2, self.Item3, self.Item4, self.Item5);
            return Unit.Default;
        }

        public static Unit with<T1, T2, T3, T4, T5, T6>(this Tuple<T1, T2, T3, T4, T5, T6> self, Action<T1, T2, T3, T4, T5, T6> func)
        {
            func(self.Item1, self.Item2, self.Item3, self.Item4, self.Item5, self.Item6);
            return Unit.Default;
        }

        public static Unit with<T1, T2, T3, T4, T5, T6, T7>(this Tuple<T1, T2, T3, T4, T5, T6, T7> self, Action<T1, T2, T3, T4, T5, T6, T7> func)
        {
            func(self.Item1, self.Item2, self.Item3, self.Item4, self.Item5, self.Item6, self.Item7);
            return Unit.Default;
        }

        /// <summary>
        /// Construct a list from head and tail
        /// head becomes the first item in the list
        /// Is lazy
        /// </summary>
        public static IEnumerable<T> cons<T>(this T head, IEnumerable<T> tail)
        {
            yield return head;
            foreach (var item in tail)
            {
                yield return item;
            }
        }

        /// <summary>
        /// Construct a list from head and tail
        /// </summary>
        public static IImmutableList<T> cons<T>(this T head, IImmutableList<T> tail) =>
            tail.Insert(0, head);

        /// <summary>
        /// Create an empty IEnumerable<T>
        /// </summary>
        public static IEnumerable<T> empty<T>() => 
            new T[0];

        /// <summary>
        /// Create an immutable list
        /// </summary>
        public static IImmutableList<T> list<T>() => 
            ImmutableList.Create<T>();

        /// <summary>
        /// Create an immutable list
        /// </summary>
        public static IImmutableList<T> list<T>(params T[] items) => 
            ImmutableList.Create<T>(items);

        /// <summary>
        /// Lazily generate a range of integers.  
        /// from and to are inclusive.
        /// </summary>
        public static IEnumerable<int> range(int from, int to) =>
            Enumerable.Range(from, to);

        /// <summary>
        /// Create an immutable map
        /// </summary>
        public static IImmutableDictionary<K, V> map<K, V>() =>
            ImmutableDictionary.Create<K, V>();

        /// <summary>
        /// Create an immutable map
        /// </summary>
        public static IImmutableDictionary<K, V> map<K, V>(params Tuple<K, V>[] items) =>
            map(items.Select(i => new KeyValuePair<K, V>(i.Item1, i.Item2)).ToArray());

        /// <summary>
        /// Create an immutable map
        /// </summary>
        public static IImmutableDictionary<K, V> map<K, V>(params KeyValuePair<K, V>[] items)
        {
            var builder = ImmutableDictionary.CreateBuilder<K, V>();
            builder.AddRange(items);
            return builder.ToImmutableDictionary();
        }

        /// <summary>
        /// Create an immutable queue
        /// </summary>
        public static ImmutableArray<T> array<T>() =>
            ImmutableArray.Create<T>();

        /// <summary>
        /// Create an immutable queue
        /// </summary>
        public static ImmutableArray<T> array<T>(T item) =>
            ImmutableArray.Create<T>(item);

        /// <summary>
        /// Create an immutable queue
        /// </summary>
        public static ImmutableArray<T> array<T>(params T[] items) =>
            ImmutableArray.Create<T>(items);

        /// <summary>
        /// Create an immutable queue
        /// </summary>
        public static IImmutableQueue<T> queue<T>() =>
            ImmutableQueue.Create<T>();

        /// <summary>
        /// Create an immutable queue
        /// </summary>
        public static IImmutableQueue<T> queue<T>(T item) =>
            ImmutableQueue.Create<T>();

        /// <summary>
        /// Create an immutable queue
        /// </summary>
        public static IImmutableQueue<T> queue<T>(params T[] items) =>
            ImmutableQueue.Create<T>(items);

        /// <summary>
        /// Create an immutable stack
        /// </summary>
        public static IImmutableStack<T> stack<T>() =>
            ImmutableStack.Create<T>();

        /// <summary>
        /// Create an immutable stack
        /// </summary>
        public static IImmutableStack<T> stack<T>(T item) =>
            ImmutableStack.Create<T>();

        /// <summary>
        /// Create an immutable stack
        /// </summary>
        public static IImmutableStack<T> stack<T>(params T[] items) =>
            ImmutableStack.Create<T>(items);

        /// <summary>
        /// Create an immutable set
        /// </summary>
        public static IImmutableSet<T> set<T>() =>
            ImmutableHashSet.Create<T>();

        /// <summary>
        /// Create an immutable set
        /// </summary>
        public static IImmutableSet<T> set<T>(T item) =>
            ImmutableHashSet.Create<T>(item);

        /// <summary>
        /// Create an immutable set
        /// </summary>
        public static IImmutableSet<T> set<T>(params T[] items) =>
            ImmutableHashSet.Create<T>(items);

        /// <summary>
        /// Returns a Func<T> that wraps func.  The first
        /// call to the resulting Func<T> will cache the result.
        /// Subsequent calls return the cached item.
        /// </summary>
        public static Func<T> memo<T>(Func<T> func)
        {
            var value = new Lazy<T>(func, true);
            return () => value.Value;
        }

        /// <summary>
        /// Returns a Func<T,R> that wraps func.  Each time the resulting
        /// Func<T,R> is called with a new value, its result is cached.
        /// Subsequent calls use the cached value.  
        /// 
        /// Remarks: Uses a ConcurrentDictionary for the cache and is thread-safe
        /// </summary>
        public static Func<T, R> memo<T, R>(Func<T, R> func)
        {
            var cache = new ConcurrentDictionary<T, R>();
            var syncMap = new ConcurrentDictionary<T, object>();
            return inp =>
            {
                R res;
                if (!cache.TryGetValue(inp, out res))
                {
                    var sync = syncMap.GetOrAdd(inp, new object());
                    lock (sync)
                    {
                        res = cache.GetOrAdd(inp, func);
                    }
                    syncMap.TryRemove(inp, out sync);
                }
                return res;
            };
        }

        /// <summary>
        /// Generates an identity function
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Func<T,T> identity<T>() => (T id) => id;

        public static Action failaction(string message) =>
            () => { throw new Exception(message); };

        public static R failwith<R>(string message)
        {
            throw new Exception(message);
        }

        public static R raise<R>(Exception ex)
        {
            throw ex;
        }
    }
}