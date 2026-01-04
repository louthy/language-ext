#pragma warning disable CS0693 // Type parameter has the same name as the type parameter from outer type

using System;
using System.Linq;
using System.Collections.Generic;
using static LanguageExt.Prelude;
using LanguageExt.Traits;
using LanguageExt.ClassInstances;
using LanguageExt.Common;

namespace LanguageExt;

public static partial class EnumerableExtensions
{
    [Obsolete("Call AsIterable().GetIterator() use Head and Tail")]
    public static (A Head, IEnumerable<A> Tail) HeadAndTail<A>(this IEnumerable<A> ma) =>
        ma.HeadAndTailSafe()
          .IfNone(() => throw Exceptions.SequenceEmpty);
    
    [Obsolete("Call AsIterable().GetIterator() and use Head and Tail")]
    public static Option<(A Head, IEnumerable<A> Tail)> HeadAndTailSafe<A>(this IEnumerable<A> ma)
    {
        var iter = ma.GetEnumerator();
        A head;
        if (iter.MoveNext())
        {
            head = iter.Current;
        }
        else
        {
            iter.Dispose();
            return None;
        }
        return Some((head, tail(iter)));

        static IEnumerable<A> tail(IEnumerator<A> rest)
        {
            try
            {
                while (rest.MoveNext())
                {
                    yield return rest.Current;
                }
            }
            finally
            {
                rest.Dispose();
            }
        }
    }
    
    [Obsolete("Call AsIterable().GetIterator() and pattern match")]
    public static B Match<A, B>(this IEnumerable<A> list,
                                Func<B> Empty,
                                Func<Seq<A>, B> More) =>
        toSeq(list).Match(Empty, More);

    [Obsolete("Call AsIterable().GetIterator() and pattern match")]
    public static B Match<A, B>(this IEnumerable<A> list,
                                Func<B> Empty,
                                Func<A, Seq<A>, B> More) =>
        toSeq(list).Match(Empty, More);

    [Obsolete("Call AsIterable().GetIterator() and pattern match")]
    public static R Match<T, R>(this IEnumerable<T> list,
                                Func<R> Empty,
                                Func<T, R> One,
                                Func<T, Seq<T>, R> More ) =>
        toSeq(list).Match(Empty, One, More);

    [Obsolete("Call AsIterable and fold")]
    public static T Reduce<T>(this IEnumerable<T> list, Func<T, T, T> reducer) =>
        List.reduce(list, reducer);

    [Obsolete("Call AsIterable and fold")]
    public static T ReduceBack<T>(this IEnumerable<T> list, Func<T, T, T> reducer) =>
        List.reduceBack(list, reducer);

    [Obsolete("Call AsIterable and fold")]
    public static IEnumerable<S> Scan<S, T>(this IEnumerable<T> list, S state, Func<S, T, S> folder) =>
        List.scan(list, state, folder);

    [Obsolete("Call AsIterable and fold")]
    public static IEnumerable<S> ScanBack<S, T>(this IEnumerable<T> list, S state, Func<S, T, S> folder) =>
        List.scanBack(list, state, folder);

    [Obsolete("Use Iterable.distinct")]
    public static IEnumerable<T> Distinct<EQ, T>(this IEnumerable<T> list) where EQ : Eq<T> =>
        List.distinct<EQ, T>(list);

    [Obsolete("Use Iterable.tails")]
    public static IEnumerable<IEnumerable<T>> Tails<T>(this IEnumerable<T> self) =>
        List.tails(self);

    [Obsolete("Use Iterable.span")]
    public static (IEnumerable<T>, IEnumerable<T>) Span<T>(this IEnumerable<T> self, Func<T, bool> pred) =>
        List.span(self, pred);

    [Obsolete("Use ToSeq then pattern-match")]
    public static Option<A> ToOption<A>(this IEnumerable<A> self) =>
        self.Match(
            ()     => Option<A>.None,
            (x, _) => Option.Some(x));
    
}
