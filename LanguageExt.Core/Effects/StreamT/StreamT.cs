using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using static LanguageExt.Prelude;
using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// Lazy sequence monad transformer
/// </summary>
public record StreamT<M, A>(Func<K<M, MList<A>>>  runListT) :
    K<StreamT<M>, A>,
    Monoid<StreamT<M, A>>
    where M : Monad<M>
{
    /// <summary>
    /// Retrieve the tail of the sequence
    /// </summary>
    /// <returns>Stream transformer</returns>
    public StreamT<M, A> Tail() =>
        new (() => runListT().Bind(
                     ml => ml switch
                           {
                               MNil<A>               => M.Pure(MList<A>.Nil),
                               MCons<M, A>(_, var t) => t(),
                               _                     => throw new NotSupportedException()
                           }));
    
    /// <summary>
    /// Map the stream
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="B">Resulting bound value type</typeparam>
    /// <returns>Stream transformer</returns>
    public StreamT<M, B> Map<B>(Func<A, B> f) =>
        new (() => runListT().Map(la => la.Map(f)));

    /// <summary>
    /// Empty stream
    /// </summary>
    public static StreamT<M, A> Empty { get; } =
        new (() => M.Pure<MList<A>>(new MNil<A>()));

    /// <summary>
    /// Construct a singleton stream
    /// </summary>
    /// <param name="value">Single value in the stream</param>
    /// <returns>Stream transformer</returns>
    public static StreamT<M, A> Pure(A value) =>
        new (() => M.Pure(MList<A>.Cons(value, () => M.Pure(MList<A>.Nil))));

    /// <summary>
    /// Lift any foldable into the stream
    /// </summary>
    /// <remarks>This is likely to consume the foldable structure eagerly</remarks>
    /// <param name="foldable">Foldable structure to lift</param>
    /// <returns>Stream transformer</returns>
    public static StreamT<M, A> LiftF<F>(K<F, A> foldable)
        where F : Foldable<F> =>
        foldable switch
        {
            IEnumerable<A> ma => Lift(ma),
            _ => new StreamT<M, A>(() => M.Pure(foldable.FoldBack(MList<A>.Nil, s => a => MList<A>.Cons(a, () => M.Pure(s)))))
        };

    /// <summary>
    /// Lift an async-enumerable into the stream
    /// </summary>
    /// <param name="stream">Sequence to lift</param>
    /// <returns>Stream transformer</returns>
    public static StreamT<M, A> Lift(IAsyncEnumerable<A> stream) =>
        from t in IO.token
        from s in Lift(stream.ToBlockingEnumerable(t))
        select s;

    /// <summary>
    /// Lift an async-enumerable into the stream
    /// </summary>
    /// <param name="stream">Sequence to lift</param>
    /// <returns>Stream transformer</returns>
    public static StreamT<M, A> LiftM(IAsyncEnumerable<K<M, A>> stream) =>
       from ma in StreamT<M, K<M, A>>.Lift(stream)
       from a in Lift(ma)
       select a;

    /// <summary>
    /// Lift an async-enumerator into the stream
    /// </summary>
    /// <param name="stream">Sequence to lift</param>
    /// <returns>Stream transformer</returns>
    public static StreamT<M, A> Lift(IAsyncEnumerator<A> stream) =>
        Lift(FromEnumerator(stream));

    /// <summary>
    /// Lift an async-enumerator into the stream
    /// </summary>
    /// <param name="stream">Sequence to lift</param>
    /// <returns>Stream transformer</returns>
    public static StreamT<M, A> LiftM(IAsyncEnumerator<K<M, A>> stream) =>
        from ma in StreamT<M, K<M, A>>.Lift(stream)
        from a in Lift(ma)
        select a;

    /// <summary>
    /// Lift an enumerable into the stream
    /// </summary>
    /// <param name="stream">Sequence to lift</param>
    /// <returns>Stream transformer</returns>
    public static StreamT<M, A> Lift(IEnumerable<A> stream)
    {
        return new StreamT<M, A>(() => 
        {
            var iter = stream.GetEnumerator();
            return next(iter)();
        });

        static Func<K<M, MList<A>>> next(IEnumerator<A> iter) =>
            () =>
            {
                if (iter.MoveNext())
                {
                    return M.Pure(MList<A>.Cons(iter.Current, next(iter)));
                }
                else
                {
                    iter.Dispose();
                    return M.Pure(MList<A>.Nil);
                }
            };
    }

    /// <summary>
    /// Lift an enumerable into the stream
    /// </summary>
    /// <param name="stream">Sequence to lift</param>
    /// <returns>Stream transformer</returns>
    public static StreamT<M, A> LiftM(IEnumerable<K<M, A>> stream) =>
        from ma in StreamT<M, K<M, A>>.Lift(stream)
        from a in Lift(ma)
        select a;

    /// <summary>
    /// Lift an enumerator into the stream
    /// </summary>
    /// <param name="stream">Sequence to lift</param>
    /// <returns>Stream transformer</returns>
    public static StreamT<M, A> Lift(IEnumerator<A> stream) =>
        Lift(FromEnumerator(stream));

    /// <summary>
    /// Lift an enumerator into the stream
    /// </summary>
    /// <param name="stream">Sequence to lift</param>
    /// <returns>Stream transformer</returns>
    public static StreamT<M, A> LiftM(IEnumerator<K<M, A>> stream)
    {
        return LiftM(go());
        IEnumerable<K<M, A>> go()
        {
            while (stream.MoveNext())
            {
                yield return stream.Current;
            }
        }
    }

    /// <summary>
    /// Lift an effect into the stream
    /// </summary>
    /// <param name="ma">Effect to lift</param>
    /// <returns>Stream transformer</returns>
    public static StreamT<M, A> Lift(K<M, A> ma) =>
        new (() => ma.Map(a => MList<A>.Cons(a, () => M.Pure(MList<A>.Nil))));

    /// <summary>
    /// Lift side effect into the stream
    /// </summary>
    /// <param name="ma">Side effect to lift</param>
    /// <returns>Stream transformer</returns>
    public static StreamT<M, A> LiftIO(IO<A> ma) =>
        Lift(M.LiftIO(ma));

    /// <summary>
    /// Lift side effect into the stream
    /// </summary>
    /// <param name="ma">Side effect to lift</param>
    /// <returns>Stream transformer</returns>
    public static StreamT<M, A> LiftIO(K<IO, A> ma) =>
        Lift(M.LiftIO(ma));

    /// <summary>
    /// Iterate the stream, returning final position.
    /// </summary>
    public K<M, Option<(A Head, StreamT<M, A> Tail)>> Run() =>
        runListT().Map(
            ma => ma switch
                  {
                      MNil<A> =>
                          Option<(A Head, StreamT<M, A> Tail)>.None,

                      MCons<M, A>(var h, var t) =>
                          Option<(A Head, StreamT<M, A> Tail)>.Some((h, new StreamT<M, A>(t))),

                      _ => throw new NotSupportedException()
                  });

    /// <summary>
    /// Iterate the stream, ignoring any result.
    /// </summary>
    public K<M, Unit> Iter() =>
        Run().IgnoreF();

    /// <summary>
    /// Retrieve the head of the sequence
    /// </summary>
    public K<M, Option<A>> Head() =>
        Run().Map(opt => opt.Map(o => o.Head));

    /// <summary>
    /// Retrieve the head of the sequence
    /// </summary>
    /// <exception cref="ExpectedException">Throws sequence-empty expected-exception</exception>
    public K<M, A> HeadUnsafe() =>
        Run().Map(opt => opt.Map(o => o.Head).IfNone(() => throw Exceptions.SequenceEmpty));

    /// <summary>
    /// Fold the stream itself, yielding the latest state value when the fold function returns `None`
    /// </summary>
    /// <param name="state">Initial state of the fold</param>
    /// <param name="f">Fold operation</param>
    /// <typeparam name="S">State type</typeparam>
    /// <returns>Stream transformer</returns>
    public StreamT<M, S> Fold<S>(S state, Func<S, A, Option<S>> f) =>
        from s in refIO(state)
        from a in this
        from r in snapshot<StreamT<M, S>>(
            () => {
                switch (f(s.Value, a))
                {
                    case { IsSome: true, Case: S value }:
                        s.Value = value;
                        return StreamT<M, S>.Empty;

                    default:
                        return StreamT<M, S>.Pure(s.Value);
                }
            })
        select r;

    /// <summary>
    /// Fold the stream itself, yielding values when the `until` predicate is `true`
    /// </summary>
    /// <param name="state">Initial state of the fold</param>
    /// <param name="f">Fold operation</param>
    /// <param name="until">Predicate</param>
    /// <typeparam name="S">State type</typeparam>
    /// <returns>Stream transformer</returns>
    public StreamT<M, S> FoldUntil<S>(S state, Func<S, A, S> f, Func<S, A, bool> until) =>
        from atom in atomIO(state)
        from a in this
        from r in atom.Swap(s => f(s, a)) switch
                  {
                      var ns when until(ns, a) => StreamT<M, S>.Pure(ns),
                      _                        => StreamT<M, S>.Empty
                  }
        select r;

    /// <summary>
    /// Fold the stream itself, yielding values when the `until` predicate is `true`
    /// </summary>
    /// <param name="state">Initial state of the fold</param>
    /// <param name="f">Fold operation</param>
    /// <param name="until">Predicate</param>
    /// <typeparam name="S">State type</typeparam>
    /// <returns>Stream transformer</returns>
    public StreamT<M, S> FoldWhile<S>(S state, Func<S, A, S> f, Func<S, A, bool> @while) =>
        from atom in atomIO(state)
        from a in this
        from r in atom.Swap(s => f(s, a)) switch
                  {
                      var ns when @while(ns, a) => StreamT<M, S>.Empty,
                      var ns                    => StreamT<M, S>.Pure(ns)
                  }
        select r;

    /// <summary>
    /// Left fold
    /// </summary>
    /// <param name="state">Initial state</param>
    /// <param name="f">Folding function</param>
    /// <typeparam name="S">State type</typeparam>
    /// <returns>Accumulate state wrapped in the StreamT inner monad</returns>
    public K<M, S> FoldM<S>(S state, Func<S, A, K<M, S>> f)
    {
        return go(state, runListT);
        K<M, S> go(S acc, Func<K<M, MList<A>>> run) =>
            run().Bind(
                mx => mx switch
                      {
                          MNil<A> =>
                              M.Pure(acc),

                          MCons<M, A>(var element, var next) =>
                              f(acc, element).Bind(acc1 => go(acc1, next)),

                          _ => throw new NotSupportedException()
                      });
    }

    /// <summary>
    /// Concatenate streams
    /// </summary>
    /// <returns>Stream transformer</returns>
    public StreamT<M, A> Combine(StreamT<M, A> second) =>
        new (() => runListT().Append(second.runListT()));

    /// <summary>
    /// Concatenate streams
    /// </summary>
    /// <returns>Stream transformer</returns>
    public StreamT<M, A> Combine(K<StreamT<M>, A> second) =>
        new (() => runListT().Append(second.As().runListT()));

    /// <summary>
    /// Interleave the items of two streams
    /// </summary>
    /// <param name="second">Other stream to merge with</param>
    /// <returns>Stream transformer</returns>
    public virtual StreamT<M, A> Merge(K<StreamT<M>, A> second)
    {
        return new StreamT<M, A>(() => go(runListT, second.As().runListT));

        K<M, MList<A>> go(Func<K<M, MList<A>>> lhs, Func<K<M, MList<A>>> rhs) =>
            from l in lhs()
            from r in rhs()
            from x in M.Pure((l, r) switch
                      {
                          (MNil<A>, MNil<A>) =>
                              l,

                          (MNil<A>, _) =>
                              r,

                          (_, MNil<A>) =>
                              l,

                          (MCons<M, A>(var lx, var lnext), MCons<M, A>(var rx, var rnext)) =>
                              MList<A>.Cons<M>(lx, () => M.Pure(MList<A>.Cons<M>(rx, () => go(lnext, rnext)))),

                          _ => throw new NotSupportedException()
                      })
            select x;

        IEnumerator<A> JoinIter(IEnumerator<A> lhs, IEnumerator<A> rhs)
        {
            while (lhs.MoveNext() && rhs.MoveNext())
            {
                yield return lhs.Current;
                yield return rhs.Current;
            }
            while (lhs.MoveNext()) yield return lhs.Current;
            while (rhs.MoveNext()) yield return rhs.Current;
        }
    }

    /// <summary>
    /// Interleave the items of many streams
    /// </summary>
    /// <param name="rhs">Other stream to merge with</param>
    /// <returns>Stream transformer</returns>
    public StreamT<M, A> Merge(K<StreamT<M>, A> second, params K<StreamT<M>, A>[] rest)
    {
        var r = Merge(second);
        foreach (var s in rest)
        {
            r = r.Merge(s);
        }
        return r;
    }
 
    /// <summary>
    /// Merge the items of two streams into pairs
    /// </summary>
    /// <param name="second">Other stream to merge with</param>
    /// <returns>Stream transformer</returns>
    public virtual StreamT<M, (A First, B Second)> Zip<B>(K<StreamT<M>, B> second)
    {
        return new StreamT<M, (A, B)>(() => go(runListT, second.As().runListT));

        K<M, MList<(A, B)>> go(Func<K<M, MList<A>>> lhs, Func<K<M, MList<B>>> rhs) =>
            from l in lhs()
            from r in rhs()
            from x in M.Pure((l, r) switch
                      {
                          (MNil<A>, _) =>
                              MList<(A, B)>.Nil,

                          (_, MNil<A>) =>
                              MList<(A, B)>.Nil,

                          (MCons<M, A>(var lx, var lnext), MCons<M, B>(var rx, var rnext)) =>
                              MList<(A, B)>.Cons<M>((lx, rx), () => go(lnext, rnext)),

                          _ => throw new NotSupportedException()
                      })
            select x;
    }

    /// <summary>
    /// Merge the items of two streams into 3-tuples
    /// </summary>
    /// <param name="second">Second stream to merge with</param>
    /// <param name="third">Third stream to merge with</param>
    /// <returns>Stream transformer</returns>
    public StreamT<M, (A First, B Second, C Third)> Zip<B, C>(
        K<StreamT<M>, B> second,
        K<StreamT<M>, C> third) =>
        Zip(second)
           .Zip(third)
           .Map(pp => (First: pp.First.First, Second: pp.First.Second, Third: pp.Second))
           .As();

    /// <summary>
    /// Merge the items of two streams into 4-tuples
    /// </summary>
    /// <param name="second">Second stream to merge with</param>
    /// <param name="third">Third stream to merge with</param>
    /// <param name="fourth">Fourth stream to merge with</param>
    /// <returns>Stream transformer</returns>
    public StreamT<M, (A First, B Second, C Third, D Fourth)> Zip<B, C, D>(
        K<StreamT<M>, B> second,
        K<StreamT<M>, C> third,
        K<StreamT<M>, D> fourth) =>
        Zip(second)
           .Zip(third)
           .Zip(fourth)
           .Map(ppp => (First: ppp.First.First.First, Second: ppp.First.First.Second, Third: ppp.First.Second, Fourth: ppp.Second))
           .As();
    
    public StreamT<M, A> Filter(Func<A, bool> f) =>
        this.Kind().Filter(f).As();

    public StreamT<M, A> Where(Func<A, bool> f) =>
        this.Kind().Filter(f).As();

    public StreamT<M, B> Select<B>(Func<A, B> f) =>
        Map(f);

    public StreamT<M, B> Bind<B>(Func<A, StreamT<M, B>> f) =>
        this.Kind().Bind(f).As();

    public StreamT<M, B> Bind<B>(Func<A, K<StreamT<M>, B>> f) =>
        this.Kind().Bind(f).As();

    public StreamT<M, B> Bind<B>(Func<A, Pure<B>> f) =>
        this.Kind().Bind(f).As();

    public StreamT<M, B> Bind<B>(Func<A, IO<B>> f) =>
        this.Kind().Bind(f).As();

    public StreamT<M, B> Bind<B>(Func<A, K<IO, B>> f) =>
        this.Kind().Bind(f).As();

    public StreamT<M, C> SelectMany<B, C>(Func<A, StreamT<M, B>> bind, Func<A, B, C> project) =>
        this.Kind().Bind(x => bind(x).Map(y => project(x, y))).As();

    public StreamT<M, C> SelectMany<B, C>(Func<A, K<StreamT<M>, B>> bind, Func<A, B, C> project) =>
        this.Kind().Bind(x => bind(x).Map(y => project(x, y))).As();

    public StreamT<M, C> SelectMany<B, C>(Func<A, Pure<M, B>> bind, Func<A, B, C> project) =>
        this.Kind().Map(x => project(x, bind(x).Value)).As();

    public StreamT<M, C> SelectMany<B, C>(Func<A, IO<B>> bind, Func<A, B, C> project) =>
        this.Kind().Bind(x => bind(x).Map(y => project(x, y))).As();

    public StreamT<M, C> SelectMany<B, C>(Func<A, K<IO, B>> bind, Func<A, B, C> project) =>
        this.Kind().Bind(x => bind(x).Map(y => project(x, y))).As();

    public static implicit operator StreamT<M, A>(Pure<A> value) =>
        Pure(value.Value);

    public static implicit operator StreamT<M, A>(Iterable<A> value) =>
        Lift(value.AsEnumerable());

    public static implicit operator StreamT<M, A>(IO<A> value) =>
        LiftIO(value);

    public static StreamT<M, A> operator +(StreamT<M, A> lhs, K<StreamT<M>, A> rhs) =>
        lhs.Combine(rhs);

    public static StreamT<M, A> operator &(StreamT<M, A> lhs, K<StreamT<M>, A> rhs) =>
        lhs.Merge(rhs);

    public static StreamT<M, A> operator >> (StreamT<M, A> lhs, K<StreamT<M>, A> rhs) =>
        lhs.Bind(_ => rhs);

    public static StreamT<M, A> operator >> (StreamT<M, A> lhs, K<StreamT<M>, Unit> rhs) =>
        lhs.Bind(x => rhs.Map(_ => x));

    static IEnumerable<A> FromEnumerator(IEnumerator<A> stream)
    {
        while (stream.MoveNext())
        {
            yield return stream.Current;
        }
    }

    static async IAsyncEnumerable<A> FromEnumerator(IAsyncEnumerator<A> stream)
    {
        while (await stream.MoveNextAsync().ConfigureAwait(false))
        {
            yield return stream.Current;
        }
    }
}
