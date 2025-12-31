using System;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Pipes;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// A source / stream of values
/// </summary>
/// <typeparam name="A">Bound value type</typeparam>
public readonly record struct Source<A>(SourceT<IO, A> runSource) : 
    K<Source, A>, 
    Monoid<Source<A>>
{
    /// <summary>
    /// A source that never yields a value
    /// </summary>
    public static Source<A> Empty =>
        new (SourceT<IO, A>.Empty);

    /// <summary>
    /// Iterate the stream, flowing values downstream to the reducer, which aggregates a
    /// result value.   
    /// </summary>
    /// <param name="state">State to reduce</param>
    /// <param name="reducer">Reducer</param>
    /// <typeparam name="S">State type</typeparam>
    /// <returns>Reduced state</returns>
    public IO<S> FoldReduce<S>(S state, Func<S, A, S> reducer) =>
        +runSource.FoldReduce(state, reducer);

    /// <summary>
    /// Iterate the stream, flowing values downstream to the reducer, which aggregates a
    /// result value.   
    /// </summary>
    /// <param name="state">State to reduce</param>
    /// <param name="reducer">Reducer</param>
    /// <typeparam name="S">State type</typeparam>
    /// <returns>Reduced state</returns>
    public IO<S> Reduce<S>(S state, Reducer<A, S> reducer) =>
        +runSource.Reduce(state, reducer);

    /// <summary>
    /// Iterate the stream, flowing values downstream to the reducer, which aggregates a
    /// result value.   
    /// </summary>
    /// <param name="state">State to reduce</param>
    /// <param name="reducer">Reducer</param>
    /// <typeparam name="S">State type</typeparam>
    /// <returns>Reduced state</returns>
    public IO<S> ReduceIO<S>(S state, ReducerIO<A, S> reducer) =>
        +runSource.ReduceM(state, (s, v) => reducer(s, v));

    /// <summary>
    /// Iterate the stream, flowing values downstream to the reducer, which aggregates a
    /// result value.   
    /// </summary>
    /// <param name="state">State to reduce</param>
    /// <param name="reducer">Reducer</param>
    /// <typeparam name="S">State type</typeparam>
    /// <returns>Reduced state</returns>
    public IO<S> ReduceM<S>(S state, ReducerIO<A, S> reducer) =>
        +runSource.ReduceM(state, (s, v) => reducer(s, v));

    /// <summary>
    /// Functor map
    /// </summary>
    public Source<B> Map<B>(Func<A, B> f) =>
        new(runSource.Map(f));

    /// <summary>
    /// Monad bind
    /// </summary>
    public Source<B> Bind<B>(Func<A, Source<B>> f) =>
        new(runSource.Bind(x => f(x).runSource));

    /// <summary>
    /// Monad bind
    /// </summary>
    public Source<B> Bind<B>(Func<A, K<Source, B>> f) =>
        new(runSource.Bind(x => f(x).As().runSource));

    /// <summary>
    /// Filter values.  Yielding downstream when `true`
    /// </summary>
    /// <param name="f">Filter function</param>
    /// <returns>Source where the only values yield are those that pass the predicate</returns>
    public Source<A> Where(Func<A, bool> f) =>
        new(runSource.Where(f));

    /// <summary>
    /// Filter values.  Yielding downstream when `true`
    /// </summary>
    /// <param name="f">Filter function</param>
    /// <returns>Source where the only values yield are those that pass the predicate</returns>
    public Source<A> Filter(Func<A, bool> f) =>
        new(runSource.Filter(f));

    /// <summary>
    /// Applicative apply
    /// </summary>
    public Source<B> ApplyBack<B>(Source<Func<A, B>> ff) =>
        new(runSource.ApplyBack(ff.runSource));

    /// <summary>
    /// Concatenate two sources into a single source.
    /// </summary>
    /// <param name="lhs">Left-hand side</param>
    /// <param name="rhs">Right-hand side</param>
    /// <returns>Concatenated stream of values</returns>
    public Source<A> Combine(Source<A> rhs) =>
        new(runSource.Combine(rhs.runSource));

    /// <summary>
    /// The value streams are both merged into a new stream.  Values are yielded
    /// as they become available.
    /// </summary>
    /// <param name="this">Left-hand side</param>
    /// <param name="rhs">Right-hand side</param>
    /// <returns>Merged stream</returns>
    public Source<A> Choose(Source<A> rhs) =>
        new(runSource.Choose(rhs.runSource));

    /// <summary>
    /// The value streams are both merged into a new stream.  Values are yielded
    /// as they become available.
    /// </summary>
    /// <param name="this">Left-hand side</param>
    /// <param name="rhs">Right-hand side</param>
    /// <returns>Merged stream</returns>
    public Source<A> Choose(Memo<Source, A> rhs) =>
        new(runSource.Choose(rhs.Lower().Value.As().runSource));
    
    /// <summary>
    /// Zip two sources into one
    /// </summary>
    /// <param name="second">Stream to zip with this one</param>
    /// <typeparam name="B">Bound value-type of the stream to zip with this one</typeparam>
    /// <returns>Stream of values where the items from two streams are paired together</returns>
    public Source<(A First, B Second)> Zip<B>(Source<B> second) =>
        new (runSource.Zip(second.runSource));

    /// <summary>
    /// Zip three sources into one
    /// </summary>
    /// <param name="second">Stream to zip with this one</param>
    /// <param name="third">Stream to zip with this one</param>
    /// <typeparam name="B">Bound value-type of the stream to zip with this one</typeparam>
    /// <returns>Stream of values where the items from two streams are paired together</returns>
    public Source<(A First, B Second, C Third)> Zip<B, C>(Source<B> second, Source<C> third) =>
        new (runSource.Zip(second.runSource, third.runSource));

    /// <summary>
    /// Zip three sources into one
    /// </summary>
    /// <param name="second">Stream to zip with this one</param>
    /// <param name="third">Stream to zip with this one</param>
    /// <param name="fourth">Stream to zip with this one</param>
    /// <typeparam name="B">Bound value-type of the stream to zip with this one</typeparam>
    /// <returns>Stream of values where the items from two streams are paired together</returns>
    public Source<(A First, B Second, C Third, D Fourth)> Zip<B, C, D>(Source<B> second, Source<C> third, Source<D> fourth) =>
        new (runSource.Zip(second.runSource, third.runSource, fourth.runSource));

    /// <summary>
    /// Skip items in the source
    /// </summary>
    /// <param name="amount">Amount to skip</param>
    /// <returns>Transformed source</returns>
    public Source<A> Skip(int amount) =>
        new (runSource.Skip(amount));

    /// <summary>
    /// Limit the number of items processed
    /// </summary>
    /// <param name="amount">Amount to take</param>
    /// <returns>Transformed source</returns>
    public Source<A> Take(int amount) =>
        new (runSource.Take(amount));

    /// <summary>
    /// Fold the values flowing through.  A value is only yielded downstream upon completion of the stream.
    /// </summary>
    /// <param name="Fold">Binary operator</param>
    /// <param name="Init">Initial state</param>
    /// <typeparam name="S">State type</typeparam>
    /// <returns>Stream of aggregate state</returns>
    public Source<S> Fold<S>(Func<S, A, S> Fold, S Init) =>
        new (runSource.Fold(Fold, Init));

    /// <summary>
    /// Fold the values flowing through.  Values are yielded downstream when either the schedule expires, or the
    /// source completes.
    /// </summary>
    /// <param name="Time">Schedule to control the rate of processing</param>
    /// <param name="Fold">Binary operator</param>
    /// <param name="Init">Initial state</param>
    /// <typeparam name="S">State type</typeparam>
    /// <returns>Stream of aggregate states</returns>
    public Source<S> Fold<S>(Schedule Time, Func<S, A, S> Fold, S Init) =>
        new (runSource.Fold(Time, Fold, Init));

    /// <summary>
    /// Fold the values flowing through.  Values are yielded downstream when either the predicate returns
    /// `false`, or the source completes.
    /// </summary>
    /// <param name="Fold">Binary operator</param>
    /// <param name="Pred">Predicate</param>
    /// <param name="Init">Initial state</param>
    /// <typeparam name="S">State type</typeparam>
    /// <returns>Stream of aggregate states</returns>
    public Source<S> FoldWhile<S>(Func<S, A, S> Fold, Func<(S State, A Value), bool> Pred, S Init) =>
        new (runSource.FoldWhile(Fold, Pred, Init));

    /// <summary>
    /// Fold the values flowing through.  Values are yielded downstream when either the predicate returns
    /// `true`, or the source completes.
    /// </summary>
    /// <param name="Fold">Binary operator</param>
    /// <param name="Pred">Predicate</param>
    /// <param name="Init">Initial state</param>
    /// <typeparam name="S">State type</typeparam>
    /// <returns>Stream of aggregate states</returns>
    public Source<S> FoldUntil<S>(Func<S, A, S> Fold, Func<(S State, A Value), bool> Pred, S Init) =>
        new(runSource.FoldUntil(Fold, Pred, Init));

    /// <summary>
    /// Fold the values flowing through.  Values are yielded downstream when either the schedule expires, the
    /// predicate returns `false`, or the source completes.
    /// </summary>
    /// <param name="Time">Schedule to control the rate of processing</param>
    /// <param name="Fold">Binary operator</param>
    /// <param name="Pred">Predicate</param>
    /// <param name="Init">Initial state</param>
    /// <typeparam name="S">State type</typeparam>
    /// <returns>Stream of aggregate states</returns>
    public Source<S> FoldWhile<S>(
        Schedule Time,
        Func<S, A, S> Fold,
        Func<(S State, A Value), bool> Pred,
        S Init) =>
        new (runSource.FoldWhile(Time, Fold, Pred, Init));

    /// <summary>
    /// Fold the values flowing through.  Values are yielded downstream when either the schedule expires, the
    /// predicate returns `true`, or the source completes.
    /// </summary>
    /// <param name="Time">Schedule to control the rate of processing</param>
    /// <param name="Fold">Binary operator</param>
    /// <param name="Pred">Predicate</param>
    /// <param name="Init">Initial state</param>
    /// <typeparam name="S"></typeparam>
    /// <returns>Stream of aggregate states</returns>
    public Source<S> FoldUntil<S>(
        Schedule Time,
        Func<S, A, S> Fold,
        Func<(S State, A Value), bool> Pred,
        S Init) =>
        new (runSource.FoldUntil(Time, Fold, Pred, Init));

    /// <summary>
    /// Transform with a transducer
    /// </summary>
    /// <param name="transducer">Transducer to use to transform</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>Transformed source</returns>
    public Source<B> Transform<B>(Transducer<A, B> transducer) =>
        new(runSource.Transform(transducer));

    /// <summary>
    /// Transform with a transducer
    /// </summary>
    /// <param name="transducer">Transducer to use to transform</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>Transformed source</returns>
    public Source<B> Transform<B>(TransducerM<IO, A, B> transducer) =>
        new(runSource.Transform(transducer));
    
    /// <summary>
    /// Convert `Source` to a `ProducerT` pipe component
    /// </summary>
    /// <typeparam name="M">Monad to lift (must support `IO`)</typeparam>
    /// <returns>`ProducerT`</returns>
    public ProducerT<A, M, Unit> ToProducerT<M>()
        where M : MonadIO<M> =>
        ProducerT.yieldAll<M, A>(this);

    /// <summary>
    /// Convert `Source` to a `Producer` pipe component
    /// </summary>
    /// <returns>`Producer`</returns>
    public Producer<RT, A, Unit> ToProducer<RT>() =>
        ToProducerT<Eff<RT>>();

    /// <summary>
    /// Functor map
    /// </summary>
    public Source<B> Select<B>(Func<A, B> f) =>
        Map(f);

    /// <summary>
    /// Monad bind
    /// </summary>
    public Source<C> SelectMany<B, C>(Func<A, Source<B>> bind, Func<A, B, C> project) =>
        Bind(a => bind(a).Map(b => project(a, b)));
}
