using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

/// <summary>
/// Usage:  Add 'using LanguageExt' to your code.
/// </summary>
public static partial class LanguageExt
{
    public static Option<T> Some<T>(T value) => new Option<T>(value);

    public static OptionNone None => OptionNone.Default;

    public static Either<R, L> Right<R, L>(R value) => new Either<R,L>(value);
    public static Either<R, L> Left<R, L>(L value) => new Either<R, L>(value);

    public static R Match<T, R>(Option<T> option, Func<T, R> Some, Func<R> None) =>
        option.IsSome
            ? Some(option.Value)
            : None();

    public static Unit Match<T>(Option<T> option, Action<T> Some, Action None)
    {
        if (option.IsSome)
        {
            Some(option.Value);
        }
        else
        {
            None();
        }
        return Unit.Default;
    }

    public static Ret Match<R, L, Ret>(Either<R, L> either, Func<R, Ret> Right, Func<L, Ret> Left) =>
        either.IsRight
            ? Right(either.RightValue)
            : Left(either.LeftValue);

    public static Unit Match<R, L>(Either<R, L> either, Action<R> Right, Action<L> Left)
    {
        if (either.IsRight)
        {
            Right(either.RightValue);
        }
        else
        {
            Left(either.LeftValue);
        }
        return Unit.Default;
    }

    public static Func<R> fun<R>(Func<R> f) => f;
    public static Func<T1, R> fun<T1, R>(Func<T1, R> f) => f;
    public static Func<T1,T2,R> fun<T1, T2, R>(Func<T1, T2, R> f) => f;
    public static Func<T1,T2,T3,R> fun<T1, T2, T3, R>(Func<T1, T2, T3, R> f) => f;
    public static Func<T1,T2,T3,T4,R> fun<T1, T2, T3, T4, R>(Func<T1, T2, T3, T4, R> f) => f;
    public static Func<T1,T2,T3,T4,T5,R> fun<T1, T2, T3, T4, T5, R>(Func<T1, T2, T3, T4, T5, R> f) => f;
    public static Func<T1,T2,T3,T4,T5,T6,R> fun<T1, T2, T3, T4, T5, T6, R>(Func<T1, T2, T3, T4, T5, T6, R> f) => f;
    public static Func<T1,T2,T3,T4,T5,T6,T7,R> fun<T1, T2, T3, T4, T5, T6, T7, R>(Func<T1, T2, T3, T4, T5, T6, T7, R> f) => f;
    public static Action fun(Action f) => f;
    public static Action<T1> fun<T1>(Action<T1> f) => f;
    public static Action<T1,T2> fun<T1, T2>(Action<T1, T2> f) => f;
    public static Action<T1,T2,T3> fun<T1, T2, T3>(Action<T1, T2, T3> f) => f;
    public static Action<T1,T2,T3,T4> fun<T1, T2, T3, T4>(Action<T1, T2, T3, T4> f) => f;
    public static Action<T1,T2,T3,T4,T5> fun<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> f) => f;
    public static Action<T1,T2,T3,T4,T5,T6> fun<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> f) => f;
    public static Action<T1,T2,T3,T4,T5,T6,T7> fun<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> f) => f;

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



    public static Unit unit => Unit.Default;

    public static Unit Do(Action action)
    {
        action();
        return unit;
    }
    public static Unit ignore<R>(Func<R> func)
    {
        func();
        return unit;
    }

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

    public static IEnumerable<T> cons<T>(this T self, IEnumerable<T> tail)
    {
        yield return self;
        foreach (var item in tail)
        {
            yield return item;
        }
    }

    public static IEnumerable<T> list<T>() => new T[0];

    public static IEnumerable<T> list<T>(params T[] items) => items;

    public static T head<T>(this IEnumerable<T> list) => list.First();

    public static Option<T> headSafe<T>(this IEnumerable<T> list) =>
        list.Take(1).Count() == 1
            ? Some(list.First())
            : None;

    public static IEnumerable<T> tail<T>(this IEnumerable<T> list) => list.Skip(1);

    public static IEnumerable<R> map<T, R>(this IEnumerable<T> list, Func<T,R> map) => 
        list.Select(map);

    public static IEnumerable<T> filter<T>(this IEnumerable<T> list, Func<T,bool> predicate) =>
        list.Where(predicate);

    public static S fold<S, T>(this IEnumerable<T> list, S state, Func<T, S, S> folder)
    {
        foreach (var item in list)
        {
            state = folder(item, state);
        }
        return state;
    }

    public static S foldr<S, T>(this IEnumerable<T> list, S state, Func<T, S, S> folder)
    {
        foreach (var item in list.Reverse())
        {
            state = folder(item, state);
        }
        return state;
    }

    public static IEnumerable<int> range(int from, int to)
    {
        for (var i = from; i <= to; i++)
        {
            yield return i;
        }
    }

    public static Func<T> memo<T>(Func<T> func)
    {
        var objectLock = new Object();
        T value = default(T);
        return () =>
            {
                if (objectLock != null)
                {
                    lock (objectLock)
                    {
                        if (objectLock != null)
                        {
                            value = func();
                        }
                    }
                    objectLock = null;
                }
                return value;
            };
    }
}