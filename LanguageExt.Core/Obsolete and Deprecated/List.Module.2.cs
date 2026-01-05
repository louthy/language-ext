#pragma warning disable CS0693 // Type parameter has the same name as the type parameter from outer type

using System;
using System.Linq;
using LanguageExt.Traits;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using static LanguageExt.Prelude;
using LanguageExt.ClassInstances;

namespace LanguageExt;

public static partial class List
{
    [Obsolete("Use Iterable.flatten or SelectMany(x => x)")]
    [OverloadResolutionPriority(Change.Priority)]
    public static Iterable<A> flatten<A>(IEnumerable<IEnumerable<A>> ma) =>
        ma.Bind(identity).AsIterable();

    [Obsolete("Use Iterable.generate")]
    [OverloadResolutionPriority(Change.Priority)]
    public static Iterable<T> generate<T>(int count, Func<int, T> generator) =>
        IterableExtensions.AsIterable(Range(0, count)).Map(generator);

    [Obsolete("Use Iterable.generate")]
    [OverloadResolutionPriority(Change.Priority)]
    public static Iterable<T> generate<T>(Func<int, T> generator) =>
        IterableExtensions.AsIterable(Range(0, int.MaxValue)).Map(generator);

    [Obsolete("Use Iterable.repeat")]
    [OverloadResolutionPriority(Change.Priority)]
    public static Iterable<T> repeat<T>(T item, int count) =>
        IterableExtensions.AsIterable(Range(0, count)).Map(_ => item);

    [Obsolete("Use Iterable.head")]
    [OverloadResolutionPriority(Change.Priority)]
    public static T head<T>(IEnumerable<T> list) => 
        list.First();
    
    [Obsolete("Use Iterable.headOrNone")]
    [OverloadResolutionPriority(Change.Priority)]
    public static Option<A> headOrNone<A>(IEnumerable<A> list) =>
        list.Select(Option.Some)
            .DefaultIfEmpty(Option<A>.None)
            .FirstOrDefault();

    [Obsolete("Use Iterable.headOrLeft")]
    [OverloadResolutionPriority(Change.Priority)]
    public static Either<L, R> headOrLeft<L, R>(IEnumerable<R> list, L left) =>
        list.Select(Either.Right<L, R>)
            .DefaultIfEmpty(Either.Left<L, R>(left))
            .FirstOrDefault() ?? left;

    [Obsolete("Use Iterable.headOrInvalid")]
    [OverloadResolutionPriority(Change.Priority)]
    public static Validation<Fail, Success> headOrInvalid<Fail, Success>(IEnumerable<Success> list, Fail fail) 
        where Fail : Monoid<Fail> =>
        list.Select(Validation.Success<Fail, Success>)
            .DefaultIfEmpty(Validation.Fail<Fail, Success>(fail))
            .FirstOrDefault() ?? Fail.Empty;

    [Obsolete("Use Iterable.headOrInvalid")]
    [OverloadResolutionPriority(Change.Priority)]
    public static Validation<Fail, Success> headOrInvalid<Fail, Success>(IEnumerable<Success> list)
        where Fail : Monoid<Fail> =>
        list.Select(Validation.Success<Fail, Success>)
            .DefaultIfEmpty(Validation.Fail<Fail, Success>(Fail.Empty))
            .FirstOrDefault() ?? Fail.Empty;

    [Obsolete("Use Iterable.last")]
    [OverloadResolutionPriority(Change.Priority)]
    public static A last<A>(IEnumerable<A> list) =>
        list.Last();

    [Obsolete("Use Iterable.lastOrNone")]
    [OverloadResolutionPriority(Change.Priority)]
    public static Option<A> lastOrNone<A>(IEnumerable<A> list) =>
        list.Select(Option.Some)
            .DefaultIfEmpty(Option<A>.None)
            .LastOrDefault();

    [Obsolete("Use Iterable.lastOrLeft")]
    [OverloadResolutionPriority(Change.Priority)]
    public static Either<L, R> lastOrLeft<L, R>(IEnumerable<R> list, L left) =>
        list.Select(Either.Right<L, R>)
            .DefaultIfEmpty(Either.Left<L, R>(left))
            .LastOrDefault() ?? left;

    [Obsolete("Use Iterable.lastOrInvalid")]
    [OverloadResolutionPriority(Change.Priority)]
    public static Validation<Fail, Success> lastOrInvalid<Fail, Success>(IEnumerable<Success> list, Fail fail)
        where Fail : Monoid<Fail> =>
        list.Select(Validation.Success<Fail, Success>)
            .DefaultIfEmpty(Validation.Fail<Fail, Success>(fail))
            .LastOrDefault() ?? fail;

    [Obsolete("Use Iterable.lastOrInvalid")]
    [OverloadResolutionPriority(Change.Priority)]
    public static Validation<Fail, Success> lastOrInvalid<Fail, Success>(IEnumerable<Success> list)
        where Fail : Monoid<Fail> =>
        list.Select(Validation.Success<Fail, Success>)
            .DefaultIfEmpty(Validation.Fail<Fail, Success>(Fail.Empty))
            .LastOrDefault() ?? Fail.Empty;

    [Obsolete("Use Iterable.init")]
    [OverloadResolutionPriority(Change.Priority)]
    public static Seq<A> init<A>(IEnumerable<A> list)
    {
        var items = list.ToArray();
        return new Seq<A>(new SeqStrict<A>(items, 0, Math.Max(0, items.Length - 1), 0, 0));
    }

    [Obsolete("Use Iterable.tail")]
    [OverloadResolutionPriority(Change.Priority)]
    public static Iterable<T> tail<T>(IEnumerable<T> list) =>
        list.Skip(1).AsIterable();

    [Obsolete("Use Iterable.map")]
    [OverloadResolutionPriority(Change.Priority)]
    public static Iterable<R> map<T, R>(IEnumerable<T> list, Func<T, R> map) =>
        list.Select(map).AsIterable();

    [Obsolete("Use Iterable.map")]
    [OverloadResolutionPriority(Change.Priority)]
    public static Iterable<R> map<T, R>(IEnumerable<T> list, Func<int, T, R> map) =>
        zip(list, Range(0, int.MaxValue), (t, i) => map(i, t)).AsIterable();

    [Obsolete("Use Iterable.filter")]
    [OverloadResolutionPriority(Change.Priority)]
    public static Iterable<T> filter<T>(IEnumerable<T> list, Func<T, bool> predicate) =>
        list.Where(predicate).AsIterable();

    [Obsolete("Use Iterable.choose")]
    [OverloadResolutionPriority(Change.Priority)]
    public static Iterable<R> choose<T, R>(IEnumerable<T> list, Func<T, Option<R>> selector) =>
        map(filter(map(list, selector), t => t.IsSome), t => t.Value!);

    [Obsolete("Use Iterable.choose")]
    [OverloadResolutionPriority(Change.Priority)]
    public static Iterable<R> choose<T, R>(IEnumerable<T> list, Func<int, T, Option<R>> selector) =>
        map(filter(map(list, selector), t => t.IsSome), t => t.Value!);

    [Obsolete("This is just Monad.Bind, so convert to an Iterable with AsIterable and use Iterable.Bind")]
    [OverloadResolutionPriority(Change.Priority)]
    public static Iterable<R> collect<T, R>(IEnumerable<T> list, Func<T, IEnumerable<R>> map) =>
        (from t in list
         from r in map(t)
         select r).AsIterable();

    [Obsolete("This is just Foldable.Sum, so convert to an Iterable with AsIterable and use Foldable.sum")]
    [OverloadResolutionPriority(Change.Priority)]
    public static int sum(IEnumerable<int> list) =>
        fold(list, 0, (s, x) => s + x);

    [Obsolete("This is just Foldable.Sum, so convert to an Iterable with AsIterable and use Foldable.sum")]
    [OverloadResolutionPriority(Change.Priority)]
    public static float sum(IEnumerable<float> list) =>
        fold(list, 0.0f, (s, x) => s + x);

    [Obsolete("This is just Foldable.Sum, so convert to an Iterable with AsIterable and use Foldable.sum")]
    [OverloadResolutionPriority(Change.Priority)]
    public static double sum(IEnumerable<double> list) =>
        fold(list, 0.0, (s, x) => s + x);

    [Obsolete("This is just Foldable.Sum, so convert to an Iterable with AsIterable and use Foldable.sum")]
    [OverloadResolutionPriority(Change.Priority)]
    public static decimal sum(IEnumerable<decimal> list) =>
        fold(list, (decimal)0, (s, x) => s + x);

    [Obsolete("Use Iterable.reverse")]
    [OverloadResolutionPriority(Change.Priority)]
    public static Iterable<T> rev<T>(IEnumerable<T> list) =>
        list.Reverse().AsIterable();

    [Obsolete("Use Iterable `+` operator")]
    [OverloadResolutionPriority(Change.Priority)]
    public static Iterable<T> append<T>(IEnumerable<T> lhs, IEnumerable<T> rhs) =>
        lhs.ConcatFast(rhs).AsIterable();

    [Obsolete("Use Iterable `+` operator")]
    [OverloadResolutionPriority(Change.Priority)]
    public static Iterable<T> append<T>(IEnumerable<T> x, IEnumerable<IEnumerable<T>> xs) =>
        xs.HeadAndTailSafe()
          .Match(
               None: x.AsIterable,
               Some: tuple => append(x, append(tuple.Head, tuple.Tail)));

    [Obsolete("Use Iterable `+` operator")]
    [OverloadResolutionPriority(Change.Priority)]
    public static Iterable<T> append<T>(params IEnumerable<T>[] lists) =>
        lists.Length == 0
            ? Iterable.empty<T>()
            : lists.Length == 1
                ? lists[0].AsIterable()
                : append(lists[0], lists.Skip(1));

    [Obsolete("Use AsIterable() then Foldable.fold")]
    [OverloadResolutionPriority(Change.Priority)]
    public static S fold<S, T>(IEnumerable<T> list, S state, Func<S, T, S> folder)
    {
        foreach (var item in list)
        {
            state = folder(state, item);
        }
        return state;
    }

    [Obsolete("Use AsIterable() then Foldable.foldBack")]
    [OverloadResolutionPriority(Change.Priority)]
    public static S foldBack<S, T>(IEnumerable<T> list, S state, Func<S, T, S> folder) =>
        fold(rev(list), state, folder);

    [Obsolete("Use AsIterable() then Foldable.foldWhile")]
    [OverloadResolutionPriority(Change.Priority)]
    public static S foldWhile<S, T>(IEnumerable<T> list, S state, Func<S, T, S> folder, Func<T, bool> preditem)
    {
        foreach (var item in list)
        {
            if (!preditem(item))
            {
                return state;
            }
            state = folder(state, item);
        }
        return state;
    }

    [Obsolete("Use AsIterable() then Foldable.foldWhile")]
    [OverloadResolutionPriority(Change.Priority)]
    public static S foldWhile<S, T>(IEnumerable<T> list, S state, Func<S, T, S> folder, Func<S, bool> predstate)
    {
        foreach (var item in list)
        {
            if (!predstate(state))
            {
                return state;
            }
            state = folder(state, item);
        }
        return state;
    }

    [Obsolete("Use AsIterable()  then Foldable.foldBackWhile")]
    [OverloadResolutionPriority(Change.Priority)]
    public static S foldBackWhile<S, T>(IEnumerable<T> list, S state, Func<S, T, S> folder, Func<T, bool> preditem) =>
        foldWhile(rev(list), state, folder, preditem: preditem);

    [Obsolete("Use AsIterable() then Foldable.foldBackWhile")]
    [OverloadResolutionPriority(Change.Priority)]
    public static S foldBackWhile<S, T>(IEnumerable<T> list, S state, Func<S, T, S> folder, Func<S, bool> predstate) =>
        foldWhile(rev(list), state, folder, predstate: predstate);

    [Obsolete("Use AsIterable() then Foldable.foldUntil")]
    [OverloadResolutionPriority(Change.Priority)]
    public static S foldUntil<S, T>(IEnumerable<T> list, S state, Func<S, T, S> folder, Func<T, bool> preditem)
    {
        foreach (var item in list)
        {
            if (preditem(item))
            {
                return state;
            }
            state = folder(state, item);
        }
        return state;
    }

    [Obsolete("Use AsIterable() then Foldable.foldUntil")]
    [OverloadResolutionPriority(Change.Priority)]
    public static S foldUntil<S, T>(IEnumerable<T> list, S state, Func<S, T, S> folder, Func<S, bool> predstate)
    {
        foreach (var item in list)
        {
            if (predstate(state))
            {
                return state;
            }
            state = folder(state, item);
        }
        return state;
    }

    [Obsolete("Use AsIterable() then Foldable.foldBackUntil")]
    [OverloadResolutionPriority(Change.Priority)]
    public static S foldBackUntil<S, T>(IEnumerable<T> list, S state, Func<S, T, S> folder, Func<T, bool> preditem) =>
        foldUntil(rev(list), state, folder, preditem: preditem);

    [Obsolete("Use AsIterable() t then Foldable.foldBackUntil")]
    [OverloadResolutionPriority(Change.Priority)]
    public static S foldBackUntil<S, T>(IEnumerable<T> list, S state, Func<S, T, S> folder, Func<S, bool> predstate) =>
        foldUntil(rev(list), state, folder, predstate: predstate);

    [Obsolete("Use AsIterable() then Foldable.fold")]
    [OverloadResolutionPriority(Change.Priority)]
    public static A reduce<A>(IEnumerable<A> list, Func<A, A, A> reducer) =>
        list.Match(
            ()      => failwith<A>("Input list was empty"),
            (x, xs) => fold(xs, x, reducer));

    [Obsolete("Use AsIterable() then Foldable.fold")]
    [OverloadResolutionPriority(Change.Priority)]
    public static Option<A> reduceOrNone<A>(IEnumerable<A> list, Func<A, A, A> reducer) =>
        list.Match(
            ()      => None,
            (x, xs) => Some(fold(xs, x, reducer)));

    [Obsolete("Use AsIterable() then Foldable.foldBack")]
    [OverloadResolutionPriority(Change.Priority)]
    public static A reduceBack<A>(IEnumerable<A> list, Func<A, A, A> reducer) =>
        list.Match(
            ()      => failwith<A>("Input list was empty"),
            (x, xs) => foldBack(xs, x, reducer));

    [Obsolete("Use AsIterable() then Foldable.foldBack")]
    [OverloadResolutionPriority(Change.Priority)]
    public static Option<A> reduceBackOrNone<A>(IEnumerable<A> list, Func<A, A, A> reducer) =>
        list.Match(
            ()      => None,
            (x, xs) => Some(foldBack(xs, x, reducer)));

    [Obsolete("Use AsIterable() then Foldable.fold")]
    [OverloadResolutionPriority(Change.Priority)]
    public static IEnumerable<S> scan<S, T>(IEnumerable<T> list, S state, Func<S, T, S> folder)
    {
        yield return state;
        foreach (var item in list)
        {
            state = folder(state, item);
            yield return state;
        }
    }

    [Obsolete("Use AsIterable() then Foldable.foldBack")]
    [OverloadResolutionPriority(Change.Priority)]
    public static IEnumerable<S> scanBack<S, T>(IEnumerable<T> list, S state, Func<S, T, S> folder) =>
        scan(rev(list), state, folder);

    [Obsolete("Use AsIterable() then Foldable.find")]
    [OverloadResolutionPriority(Change.Priority)]
    public static Option<T> find<T>(IEnumerable<T> list, Func<T, bool> pred)
    {
        foreach (var item in list)
        {
            if (pred(item)) return Some(item);
        }
        return None;
    }

    [Obsolete("Use AsIterable() then Foldable.findAll")]
    [OverloadResolutionPriority(Change.Priority)]
    public static IEnumerable<T> findSeq<T>(IEnumerable<T> list, Func<T, bool> pred)
    {
        foreach (var item in list)
        {
            if (pred(item))
            {
                yield return item;
                break;
            }
        }
    }

    [Obsolete("Use AsIterable().ToArr, AsIterable().ToSeq, AsIterable().ToLst, or other natural transformations")]
    [OverloadResolutionPriority(Change.Priority)]
    public static Lst<T> freeze<T>(IEnumerable<T> list) =>
        toList(list);

    [Obsolete("Use AsIterable().Zip")]
    [OverloadResolutionPriority(Change.Priority)]
    public static IEnumerable<V> zip<T, U, V>(IEnumerable<T> list, IEnumerable<U> other, Func<T, U, V> zipper) =>
        list.Zip(other, zipper);

    [Obsolete("Use AsIterable().Zip")]
    [OverloadResolutionPriority(Change.Priority)]
    public static IEnumerable<(T First, U Second)> zip<T, U>(IEnumerable<T> list, IEnumerable<U> other) =>
        list.Zip(other, (t, u) => (t, u));

    [Obsolete("Use AsIterable() then Foldable.count")]
    [OverloadResolutionPriority(Change.Priority)]
    public static int length<T>(IEnumerable<T> list) =>
        list.Count();

    [Obsolete("Use AsIterable() then Foldable.iter")]
    [OverloadResolutionPriority(Change.Priority)]
    public static Unit iter<T>(IEnumerable<T> list, Action<T> action)
    {
        foreach (var item in list)
        {
            action(item);
        }
        return unit;
    }

    [Obsolete("Use AsIterable() then Foldable.iter")]
    [OverloadResolutionPriority(Change.Priority)]
    public static Unit iter<T>(IEnumerable<T> list, Action<int, T> action)
    {
        var i = 0;
        foreach (var item in list)
        {
            action(i++, item);
        }
        return unit;
    }

    [Obsolete("Use AsIterable() then Foldable.iter")]
    [OverloadResolutionPriority(Change.Priority)]
    public static Unit consume<T>(IEnumerable<T> list)
    {
        foreach (var _ in list)
        {
        }
        return unit;
    }

    [Obsolete("Use AsIterable() then Foldable.forAll")]
    [OverloadResolutionPriority(Change.Priority)]
    public static bool forall<T>(IEnumerable<T> list, Func<T, bool> pred) =>
        list.All(pred);

    [Obsolete("Use Iterable.distinct")]
    [OverloadResolutionPriority(Change.Priority)]
    public static IEnumerable<T> distinct<T>(IEnumerable<T> list) =>
        list.Distinct();

    [Obsolete("Use Iterable.distinct")]
    [OverloadResolutionPriority(Change.Priority)]
    public static IEnumerable<T> distinct<EQ, T>(IEnumerable<T> list) where EQ : Eq<T> =>
        list.Distinct(new EqCompare<T>(static (x, y) => EQ.Equals(x, y), static x => EQ.GetHashCode(x)));

    [Obsolete("Use Iterable.distinct")]
    [OverloadResolutionPriority(Change.Priority)]
    public static IEnumerable<T> distinct<T, K>(IEnumerable<T> list, Func<T, K> keySelector, Option<Func<K, K, bool>> compare = default) =>
        list.Distinct(new EqCompare<T>(
                          (a, b) => compare.IfNone(EqDefault<K>.Equals)(keySelector(a), keySelector(b)), 
                          a => keySelector(a)?.GetHashCode() ?? 0));

    [Obsolete("Use Iterable.take")]
    [OverloadResolutionPriority(Change.Priority)]
    public static IEnumerable<T> take<T>(IEnumerable<T> list, int count) =>
        list.Take(count);

    [Obsolete("Use Iterable.takeWhile")]
    [OverloadResolutionPriority(Change.Priority)]
    public static IEnumerable<T> takeWhile<T>(IEnumerable<T> list, Func<T, bool> pred) =>
        list.TakeWhile(pred);

    [Obsolete("Use Iterable.takeWhile")]
    [OverloadResolutionPriority(Change.Priority)]
    public static IEnumerable<T> takeWhile<T>(IEnumerable<T> list, Func<T, int, bool> pred) =>
        list.TakeWhile(pred);

    [Obsolete("Use Iterable.unfold")]
    [OverloadResolutionPriority(Change.Priority)]
    public static IEnumerable<S> unfold<S>(S state, Func<S, Option<S>> unfolder)
    {
        while (true)
        {
            yield return state;
            var res = unfolder(state);
            if (res.IsNone)
            {
                yield break;
            }
            else
            {
                state = res.Value!;
            }
        }
    }

    [Obsolete("Use Iterable.unfold")]
    [OverloadResolutionPriority(Change.Priority)]
    public static IEnumerable<A> unfold<S, A>(S state, Func<S, Option<(A, S)>> unfolder)
    {
        while (true)
        {
            var res = unfolder(state);
            if (res.IsNone)
            {
                yield break;
            }
            else
            {
                state = res.Value.Item2;
                yield return res.Value.Item1;
            }
        }
    }

    [Obsolete("Use Iterable.unfold")]
    [OverloadResolutionPriority(Change.Priority)]
    public static IEnumerable<A> unfold<S1, S2, A>((S1, S2) state, Func<S1, S2, Option<(A, S1, S2)>> unfolder)
    {
        while (true)
        {
            var res = unfolder(state.Item1, state.Item2);
            if (res.IsNone)
            {
                yield break;
            }
            else
            {
                state = (res.Value.Item2, res.Value.Item3);
                yield return res.Value.Item1;
            }
        }
    }

    [Obsolete("Use Iterable.unfold")]
    [OverloadResolutionPriority(Change.Priority)]
    public static IEnumerable<A> unfold<S1, S2, S3, A>((S1, S2, S3) state, Func<S1, S2, S3, Option<(A, S1, S2, S3)>> unfolder)
    {
        while (true)
        {
            var res = unfolder(state.Item1, state.Item2, state.Item3);
            if (res.IsNone)
            {
                yield break;
            }
            else
            {
                state = (res.Value.Item2, res.Value.Item3, res.Value.Item4);
                yield return res.Value.Item1;
            }
        }
    }

    [Obsolete("Use Iterable.unfold")]
    [OverloadResolutionPriority(Change.Priority)]
    public static IEnumerable<A> unfold<S1, S2, S3, S4, A>((S1, S2, S3, S4) state, Func<S1, S2, S3, S4, Option<(A, S1, S2, S3, S4)>> unfolder)
    {
        while (true)
        {
            var res = unfolder(state.Item1, state.Item2, state.Item3, state.Item4);
            if (res.IsNone)
            {
                yield break;
            }
            else
            {
                state = (res.Value.Item2, res.Value.Item3, res.Value.Item4, res.Value.Item5);
                yield return res.Value.Item1;
            }
        }
    }

    [Obsolete("Use AsIterable then Foldable.exists")]
    [OverloadResolutionPriority(Change.Priority)]
    public static bool exists<T>(IEnumerable<T> list, Func<T, bool> pred)
    {
        foreach (var item in list)
        {
            if (pred(item))
                return true;
        }
        return false;
    }

    [Obsolete("headSafe has been deprecated, please use headOrNone")]
    [OverloadResolutionPriority(Change.Priority)]
    public static Option<T> headSafe<T>(IEnumerable<T> list) =>
        (from x in list
         select Some(x))
       .DefaultIfEmpty(None)
       .FirstOrDefault();

    [Obsolete("Use Iterable.tails")]
    [OverloadResolutionPriority(Change.Priority)]
    public static IEnumerable<IEnumerable<T>> tails<T>(IEnumerable<T> self)
    {
        var lst = new List<T>(self);
        for (var skip = 0; skip < lst.Count; skip++)
        {
            yield return lst.Skip(skip);
        }
        yield return Enumerable.Empty<T>();
    }

    [Obsolete("Use Iterable.span")]
    [OverloadResolutionPriority(Change.Priority)]
    public static (IEnumerable<T> Initial, IEnumerable<T> Remainder) span<T>(IEnumerable<T> self, Func<T, bool> pred)
    {
        var iter    = self.GetEnumerator();
        var diposed = false;

        IEnumerable<T> first(IEnumerator<T> items)
        {
            while (items.MoveNext())
            {
                if (pred(items.Current))
                {
                    yield return items.Current;
                }
                else
                {
                    yield break;
                }
            }
            items.Dispose();
            diposed = true;
        }

        IEnumerable<T> second(IEnumerator<T> items)
        {
            if (diposed) yield break;
            while (items.MoveNext())
            {
                yield return items.Current;
            }
            items.Dispose();
        }

        return (first(iter), second(iter));
    }
}
