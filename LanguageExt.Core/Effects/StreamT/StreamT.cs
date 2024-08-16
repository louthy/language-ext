using System;
using System.Collections.Generic;
using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// Lazy sequence monad transformer
/// </summary>
public abstract record StreamT<M, A> :
    K<StreamT<M>, A>,
    Monoid<StreamT<M, A>>
    where M : Monad<M>
{
    /// <summary>
    /// Get the lifted monad
    /// </summary>
    public abstract K<M, MList<A>> runListT { get; }

    /// <summary>
    /// Empty stream
    /// </summary>
    public static StreamT<M, A> Empty { get; } =
        new StreamMainT<M, A>(M.Pure<MList<A>>(new MNil<A>()));

    /// <summary>
    /// Construct a singleton stream
    /// </summary>
    /// <param name="value">Single value in the stream</param>
    /// <returns>Stream transformer</returns>
    public static StreamT<M, A> Pure(A value) =>
        new StreamPureT<M, A>(value);

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
            _ => new StreamMainT<M, A>(M.Pure(foldable.FoldBack(MList<A>.Nil, s => a => MList<A>.Cons(a, M.Pure(s)))))
        };

    /// <summary>
    /// Lift an async-enumerable into the stream
    /// </summary>
    /// <param name="stream">Sequence to lift</param>
    /// <returns>Stream transformer</returns>
    public static StreamT<M, A> Lift(IAsyncEnumerable<A> stream) =>
        new StreamAsyncEnumerableT<M, A>(stream);

    /// <summary>
    /// Lift an enumerable into the stream
    /// </summary>
    /// <param name="stream">Sequence to lift</param>
    /// <returns>Stream transformer</returns>
    public static StreamT<M, A> Lift(IEnumerable<A> stream) =>
        StreamT.pure<M, Unit>(default) // HACK: forces re-evaluation of the enumerable
               .Bind(_ => new StreamEnumerableT<M, A>(stream));

    /// <summary>
    /// Lift a (possibly lazy) sequence into the stream
    /// </summary>
    /// <param name="list">Sequence to lift</param>
    /// <returns>Stream transformer</returns>
    public static StreamT<M, A> Lift(Seq<A> list) =>
        list switch
        {
            []          => Empty,
            var (x, xs) => new StreamMainT<M, A>(M.Pure<MList<A>>(new MCons<M, A>(x, Lift(xs).runListT)))
        };

    /// <summary>
    /// Lift an effect into the stream
    /// </summary>
    /// <param name="ma">Effect to lift</param>
    /// <returns>Stream transformer</returns>
    public static StreamT<M, A> Lift(K<M, A> ma) =>
        new StreamLiftM<M, A>(ma);

    /// <summary>
    /// Lift side effect into the stream
    /// </summary>
    /// <param name="ma">Side effect to lift</param>
    /// <returns>Stream transformer</returns>
    public static StreamT<M, A> LiftIO(IO<A> ma) =>
        Lift(M.LiftIO(ma));

    /// <summary>
    /// Iterate the stream, returning final position.
    /// </summary>
    public K<M, Option<(A Head, StreamT<M, A> Tail)>> Run() =>
        runListT.Map(
            ma => ma switch
                  {
                      MNil<A> =>
                          Option<(A Head, StreamT<M, A> Tail)>.None,

                      MCons<M, A>(var h, var t) =>
                          Option<(A Head, StreamT<M, A> Tail)>.Some((h, new StreamMainT<M, A>(t))),

                      MIter<M, A>(var h, _) iter =>
                          Option<(A Head, StreamT<M, A> Tail)>.Some((h, new StreamMainT<M, A>(iter.TailM()))),

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
    public K<M, Option<A>> Head =>
        Run().Map(opt => opt.Map(o => o.Head));

    /// <summary>
    /// Retrieve the head of the sequence
    /// </summary>
    /// <exception cref="ExpectedException">Throws sequence-empty expected-exception</exception>
    public K<M, A> HeadUnsafe =>
        Run().Map(opt => opt.Map(o => o.Head).IfNone(() => throw Exceptions.SequenceEmpty));

    /// <summary>
    /// Retrieve the tail of the sequence
    /// </summary>
    /// <returns>Stream transformer</returns>
    public abstract StreamT<M, A> Tail { get; }
    
    /// <summary>
    /// Map the stream
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="B">Resulting bound value type</typeparam>
    /// <returns>Stream transformer</returns>
    public abstract StreamT<M, B> Map<B>(Func<A, B> f);

    /// <summary>
    /// Left fold
    /// </summary>
    /// <param name="state">Initial state</param>
    /// <param name="f">Folding function</param>
    /// <typeparam name="S">State type</typeparam>
    /// <returns>Accumulate state wrapped in the StreamT inner monad</returns>
    public K<M, S> Fold<S>(S state, Func<S, A, K<M, S>> f)
    {
        return go(state, runListT);
        K<M, S> go(S acc, K<M, MList<A>> run) =>
            run.Bind(
                mx => mx switch
                      {
                          MNil<A> =>
                              M.Pure(acc),

                          MCons<M, A>(var element, var next) =>
                              f(acc, element).Bind(acc1 => go(acc1, next)),

                          MIter<M, A>(var element, _) iter =>
                              f(acc, element).Bind(acc1 => go(acc1, iter.TailM())),

                          _ => throw new NotSupportedException()
                      });
    }

    /// <summary>
    /// Concatenate streams
    /// </summary>
    /// <returns>Stream transformer</returns>
    public StreamT<M, A> Combine(StreamT<M, A> rhs) =>
        new StreamMainT<M, A>(runListT.Append(rhs.runListT));

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

    public static StreamT<M, A> operator +(StreamT<M, A> lhs, StreamT<M, A> rhs) =>
        lhs.Combine(rhs);

    public static StreamT<M, A> operator >> (StreamT<M, A> lhs, StreamT<M, A> rhs) =>
        lhs.Bind(_ => rhs);

    public static StreamT<M, A> operator >> (StreamT<M, A> lhs, K<StreamT<M>, A> rhs) =>
        lhs.Bind(_ => rhs);

    public static StreamT<M, A> operator >> (StreamT<M, A> lhs, StreamT<M, Unit> rhs) =>
        lhs.Bind(x => rhs.Map(_ => x));

    public static StreamT<M, A> operator >> (StreamT<M, A> lhs, K<StreamT<M>, Unit> rhs) =>
        lhs.Bind(x => rhs.Map(_ => x));
}
