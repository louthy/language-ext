using System;
using LanguageExt.Common;
using LanguageExt.Traits;
using LanguageExt.Pipes.Concurrent;
using static LanguageExt.Prelude;

namespace LanguageExt.Pipes;

public record StreamT<M, A>(Source<K<M, A>> runStreamT) : K<StreamT<M>, A>
    where M : Monad<M>, Alternative<M>
{
    /// <summary>
    /// Tuple the items from the streams provided together.  
    /// </summary>
    /// <remarks>
    /// This requires waiting for each stream to yield a value so that the zipped tuple can be constructed.  If any one
    /// of the streams ends then the zipped stream also ends.
    /// </remarks>
    /// <param name="second">Second stream</param>
    /// <typeparam name="M">`Monad` and `Alternative` traits</typeparam>
    /// <typeparam name="B">Second bound value type</typeparam>
    /// <returns>Zipped stream of values</returns>
    public StreamT<M, (A First, B Second)> Zip<B>(StreamT<M, B> second) =>
        new(runStreamT.Zip(second.runStreamT).Map(p => p.First.Zip(p.Second)));
    
    /// <summary>
    /// Tuple the items from the streams provided together.  
    /// </summary>
    /// <remarks>
    /// This requires waiting for each stream to yield a value so that the zipped tuple can be constructed.  If any one
    /// of the streams ends then the zipped stream also ends.
    /// </remarks>
    /// <param name="second">Second stream</param>
    /// <param name="third">Third stream</param>
    /// <typeparam name="M">`Monad` and `Alternative` traits</typeparam>
    /// <typeparam name="B">Second bound value type</typeparam>
    /// <typeparam name="C">Third bound value type</typeparam>
    /// <returns>Zipped stream of values</returns>
    public StreamT<M, (A First, B Second, C Third)> Zip<B, C>(StreamT<M, B> second, StreamT<M, C> third) =>
        new(runStreamT.Zip(second.runStreamT, third.runStreamT).Map(p => p.First.Zip(p.Second, p.Third)));
    
    /// <summary>
    /// Tuple the items from the streams provided together.  
    /// </summary>
    /// <remarks>
    /// This requires waiting for each stream to yield a value so that the zipped tuple can be constructed.  If any one
    /// of the streams ends then the zipped stream also ends.
    /// </remarks>
    /// <param name="second">Second stream</param>
    /// <param name="third">Third stream</param>
    /// <param name="fourth">Fourth stream</param>
    /// <typeparam name="M">`Monad` and `Alternative` traits</typeparam>
    /// <typeparam name="B">Second bound value type</typeparam>
    /// <typeparam name="C">Third bound value type</typeparam>
    /// <typeparam name="D">Fourth bound value type</typeparam>
    /// <returns>Zipped stream of values</returns>
    public StreamT<M, (A First, B Second, C Third, D Fourth)> Zip<B, C, D>(StreamT<M, B> second, StreamT<M, C> third, StreamT<M, D> fourth) =>
        new(runStreamT.Zip(second.runStreamT, third.runStreamT, fourth.runStreamT).Map(p => p.First.Zip(p.Second, p.Third, p.Fourth)));


    /// <summary>
    /// Fold the stream itself, yielding the latest state value when the fold function returns `None`
    /// </summary>
    /// <param name="state">Initial state of the fold</param>
    /// <param name="f">Fold operation</param>
    /// <typeparam name="S">State type</typeparam>
    /// <returns>Stream transformer</returns>
    public StreamT<M, S> Fold<S>(S state, Func<S, A, Option<S>> f) =>
        Bind(x => f(state, x) switch
                  {
                      { IsSome: true, Case: S newState } => Fold(newState, f),
                      _                                  => StreamT.pure<M, S>(state)
                  });

    /// <summary>
    /// Fold the stream itself, yielding values when the `until` predicate is `true`
    /// </summary>
    /// <param name="state">Initial state of the fold</param>
    /// <param name="f">Fold operation</param>
    /// <param name="until">Predicate</param>
    /// <typeparam name="S">State type</typeparam>
    /// <returns>Stream transformer</returns>
    public StreamT<M, S> FoldUntil<S>(S state, Func<S, A, S> f, Func<S, A, bool> until) =>
        Bind(x => f(state, x) switch
                  {
                      { } newState when until(newState, x) => StreamT.pure<M, S>(state),
                      { } newState                         => FoldUntil(newState, f, until),
                      _                                    => StreamT.pure<M, S>(state)
                  });

    /// <summary>
    /// Fold the stream itself, yielding values when the `until` predicate is `true`
    /// </summary>
    /// <param name="state">Initial state of the fold</param>
    /// <param name="f">Fold operation</param>
    /// <param name="until">Predicate</param>
    /// <typeparam name="S">State type</typeparam>
    /// <returns>Stream transformer</returns>
    public StreamT<M, S> FoldWhile<S>(S state, Func<S, A, S> f, Func<S, A, bool> @while) =>
        Bind(x => f(state, x) switch
                  {
                      { } newState when !@while(newState, x) => StreamT.pure<M, S>(state),
                      { } newState                           => FoldWhile(newState, f, @while),
                      _                                      => StreamT.pure<M, S>(state)
                  });
    
    public StreamT<M, B> Map<B>(Func<A, B> f) =>
        new(runStreamT.Map(ma => ma.Map(f)));
    
    public StreamT<M, B> Select<B>(Func<A, B> f) =>
        new(runStreamT.Map(ma => ma.Map(f)));
    
    public StreamT<M, A> Where(Func<A, bool> f) =>
        Bind(a => f(a) ? StreamT.pure<M, A>(a) : StreamT.empty<M, A>());
    
    public StreamT<M, A> Filter(Func<A, bool> f) =>
        Bind(a => f(a) ? StreamT.pure<M, A>(a) : StreamT.empty<M, A>());

    public StreamT<M, B> Bind<B>(Func<A, K<StreamT<M>, B>> f) =>
        new(runStreamT.Bind(ma => Source.pure(ma.Bind(x => f(x).RunAsync()))));

    public StreamT<M, B> Bind<B>(Func<A, IO<B>> f) =>
        Bind(x => StreamT.liftIO<M, B>(f(x)));

    public StreamT<M, B> Bind<B>(Func<A, K<M, B>> f) =>
        Bind(x => StreamT.liftM(f(x)));

    public StreamT<M, B> Bind<B>(Func<A, Pure<B>> f) =>
        Map(x => f(x).Value);

    public StreamT<M, C> SelectMany<B, C>(Func<A, K<StreamT<M>, B>> bind, Func<A, B, C> project) =>
        Bind(a => bind(a).Map(b => project(a, b)));

    public StreamT<M, C> SelectMany<B, C>(Func<A, IO<B>> bind, Func<A, B, C> project) =>
        Bind(a => bind(a).Map(b => project(a, b)));    

    public StreamT<M, C> SelectMany<B, C>(Func<A, K<M, B>> bind, Func<A, B, C> project) =>
        Bind(a => bind(a).Map(b => project(a, b)));    
    
    public StreamT<M, C> SelectMany<B, C>(Func<A, Pure<B>> bind, Func<A, B, C> project) =>
        Bind(a => bind(a).Map(b => project(a, b)));

    public static StreamT<M, A> operator |(StreamT<M, A> ma, StreamT<M, A> mb) =>
        ma.Choose(mb).As();

    public static StreamT<M, A> operator +(StreamT<M, A> ma, StreamT<M, A> mb) =>
        ma.Combine(mb).As();    
    
    public static StreamT<M, A> operator >> (StreamT<M, A> lhs, K<StreamT<M>, A> rhs) =>
        lhs.Bind(_ => rhs);

    public static StreamT<M, A> operator >> (StreamT<M, A> lhs, K<StreamT<M>, Unit> rhs) =>
        lhs.Bind(x => rhs.Map(_ => x));
    
    public static implicit operator StreamT<M, A>(IO<A> ma) =>
        StreamT.liftIO<M, A>(ma);
    
    public static implicit operator StreamT<M, A>(Pure<A> ma) =>
        StreamT.liftM(M.Pure(ma.Value));

    public static implicit operator StreamT<M, A>(Fail<Error> ma) =>
        IO.fail<A>(ma.Value);
}
