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
    /// Iterate the stream, ignoring any result.
    /// </summary>
    public K<M, Unit> Iter() =>
        Run().IgnoreF();

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
    public abstract StreamT<M, A> Tail { get; }

    public static StreamT<M, A> Pure(A value) =>
        new StreamPureT<M, A>(value);

    public static StreamT<M, A> Lift(Seq<A> list) =>
        list switch
        {
            []          => Empty,
            var (x, xs) => new StreamMainT<M, A>(M.Pure<MList<A>>(new MCons<M, A>(x, Lift(xs).runListT)))
        };

    public static StreamT<M, A> Lift(IAsyncEnumerable<A> stream) =>
        new StreamAsyncEnumerableT<M, A>(stream);

    public static StreamT<M, A> Lift(IEnumerable<A> stream) =>
        StreamT.pure<M, Unit>(default) // HACK: forces re-evaluation of the enumerable
               .Bind(_ => new StreamEnumerableT<M, A>(stream));

    public static StreamT<M, A> Lift(K<M, A> ma) =>
        new StreamLiftM<M, A>(ma);

    public static StreamT<M, A> LiftIO(IO<A> ma) =>
        Lift(M.LiftIO(ma));

    public abstract StreamT<M, B> Map<B>(Func<A, B> f);

    /// <summary>
    /// Concatenate sequences
    /// </summary>
    public StreamT<M, A> Combine(StreamT<M, A> rhs) =>
        new StreamMainT<M, A>(runListT.Append(rhs.runListT));

    /// <summary>
    /// Empty sequence
    /// </summary>
    public static StreamT<M, A> Empty { get; } =
        new StreamMainT<M, A>(M.Pure<MList<A>>(new MNil<A>()));

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
        Lift(value);

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
