using System;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt.Pipes.Concurrent;

/// <summary>
/// A source / stream of values
/// </summary>
/// <typeparam name="A">Bound value type</typeparam>
public abstract record Source<A> : 
    K<Source, A>, 
    Monoid<Source<A>>
{
    /// <summary>
    /// A source that never yields a value
    /// </summary>
    public static Source<A> Empty =>
        EmptySource<A>.Default;

    /// <summary>
    /// Reduce the stream to a single value
    /// </summary>
    /// <param name="state">State to reduce</param>
    /// <param name="reducer">Reducer</param>
    /// <typeparam name="S">State type</typeparam>
    /// <returns>Reduced state</returns>
    public IO<S> Reduce<S>(S state, Reducer<A, S> reducer) =>
        IO.liftVAsync(async e =>
                      {
                          var iter = GetIterator();
                          while (await iter.ReadyToRead(e.Token))
                          {
                              if(e.Token.IsCancellationRequested) throw new TaskCanceledException();
                              var value = await iter.ReadValue(e.Token);
                              switch (await reducer(state, value))
                              {
                                  case { Continue: true, Value: var nstate }:
                                      state = nstate;
                                      break;
                                  
                                  case { Value: var nstate }:
                                      return nstate;
                              }
                          }
                          return state;
                      });
    
    internal IO<S> Reduce1<S>(S state, Func<S, A, S> reducer) =>
        Reduce(state, (s, a) => new ValueTask<Reduced<S>>(Reduced.Continue(reducer(s, a))));
    
    internal IO<S> Reduce2<S>(S state, Func<S, A, Reduced<S>> reducer) =>
        Reduce(state, (s, a) => new ValueTask<Reduced<S>>(reducer(s, a)));

    /// <summary>
    /// Transform with a transducer
    /// </summary>
    /// <param name="transducer">Transducer to use to transform</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>Transformed source</returns>
    public Source<B> Transform<B>(Transducer<A, B> transducer) =>
        new TransformSource<A,B>(this, transducer);
    
    /// <summary>
    /// Functor map
    /// </summary>
    public virtual Source<B> Map<B>(Func<A, B> f) =>
        Transform(Transducer.map(f));
    
    /// <summary>
    /// Monad bind
    /// </summary>
    public virtual Source<B> Bind<B>(Func<A, Source<B>> f) =>
        new BindSource<A, B>(this, f);
    
    /// <summary>
    /// Monad bind
    /// </summary>
    public Source<B> Bind<B>(Func<A, K<Source, B>> f) =>
        Bind(x => f(x).As());
    
    /// <summary>
    /// Filter values.  Yielding downstream when `true`
    /// </summary>
    /// <param name="f">Filter function</param>
    /// <returns>Source where the only values yield are those that pass the predicate</returns>
    public Source<A> Where(Func<A, bool> f) =>
        Transform(Transducer.filter(f));
    
    /// <summary>
    /// Filter values.  Yielding downstream when `true`
    /// </summary>
    /// <param name="f">Filter function</param>
    /// <returns>Source where the only values yield are those that pass the predicate</returns>
    public Source<A> Filter(Func<A, bool> f) =>
        Transform(Transducer.filter(f));
    
    /// <summary>
    /// Applicative apply
    /// </summary>
    public virtual Source<B> ApplyBack<B>(Source<Func<A, B>> ff) =>
        new ApplySource<A, B>(ff, this);

    /// <summary>
    /// Combine two sources into a single source.  The value streams are both
    /// merged into a new stream.  Values are yielded as they become available.
    /// </summary>
    /// <param name="rhs">Right hand side</param>
    /// <returns>Merged stream of values</returns>
    public Source<A> Combine(Source<A> rhs) =>
        (this, rhs) switch
        {
            (EmptySource<A>, EmptySource<A>)         => EmptySource<A>.Default,
            (var l, EmptySource<A>)                  => l,
            (EmptySource<A>, var r)                  => r,
            (CombineSource<A> l, CombineSource<A> r) => new CombineSource<A>(l.Sources + r.Sources),
            (CombineSource<A> l, var r)              => new CombineSource<A>(l.Sources.Add(r)),
            (var l, CombineSource<A> r)              => new CombineSource<A>(l.Cons(r.Sources)),
            _                                        => new CombineSource<A>([this, rhs])
        };
    
    /// <summary>
    /// Choose a value from the first `Source` to successfully yield 
    /// </summary>
    /// <param name="rhs"></param>
    /// <returns>Value from this `Source` if there are any available, if not, from `rhs`.  If
    /// `rhs` is also empty then `Errors.SourceClosed` is raised</returns>
    public Source<A> Choose(Source<A> rhs) =>
        new ChooseSource<A>(this, rhs);

    /// <summary>
    /// Zip two sources into one
    /// </summary>
    /// <param name="second">Stream to zip with this one</param>
    /// <typeparam name="B">Bound value type of the stream to zip with this one</typeparam>
    /// <returns>Stream of values where the items from two streams are paired together</returns>
    public Source<(A First, B Second)> Zip<B>(Source<B> second) =>
        new Zip2Source<A, B>(this, second);

    /// <summary>
    /// Zip three sources into one
    /// </summary>
    /// <param name="second">Stream to zip with this one</param>
    /// <param name="third">Stream to zip with this one</param>
    /// <typeparam name="B">Bound value type of the stream to zip with this one</typeparam>
    /// <returns>Stream of values where the items from two streams are paired together</returns>
    public Source<(A First, B Second, C Third)> Zip<B, C>(Source<B> second, Source<C> third) =>
        new Zip3Source<A, B, C>(this, second, third);

    /// <summary>
    /// Zip three sources into one
    /// </summary>
    /// <param name="second">Stream to zip with this one</param>
    /// <param name="third">Stream to zip with this one</param>
    /// <param name="fourth">Stream to zip with this one</param>
    /// <typeparam name="B">Bound value type of the stream to zip with this one</typeparam>
    /// <returns>Stream of values where the items from two streams are paired together</returns>
    public Source<(A First, B Second, C Third, D Fourth)> Zip<B, C, D>(Source<B> second, Source<C> third, Source<D> fourth) =>
        new Zip4Source<A, B, C, D>(this, second, third, fourth);

    /// <summary>
    /// Skip items in the source
    /// </summary>
    /// <param name="amount">Amount to skip</param>
    /// <returns>Transformed source</returns>
    public Source<A> Skip(int amount) =>
        Transform(Transducer.skip<A>(amount)); 

    /// <summary>
    /// Limit the number of items processed 
    /// </summary>
    /// <param name="amount">Amount to take</param>
    /// <returns>Transformed source</returns>
    public Source<A> Take(int amount) =>
        Transform(Transducer.take<A>(amount)); 

    /// <summary>
    /// Fold the values flowing through.  A value is only yielded downstream upon completion of the stream.
    /// </summary>
    /// <param name="Fold">Binary operator</param>
    /// <param name="Init">Initial state</param>
    /// <typeparam name="S">State type</typeparam>
    /// <returns>Stream of aggregate state</returns>
    public Source<S> Fold<S>(Func<S, A, S> Fold, S Init) =>
        Transform(Transducer.foldWhile(Fold, (_, _) => true, Init));

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
        Transform(Transducer.foldWhile(Time, Fold, (_, _) => true, Init));

    /// <summary>
    /// Fold the values flowing through.  Values are yielded downstream when either the predicate returns
    /// `false`, or the source completes. 
    /// </summary>
    /// <param name="Fold">Binary operator</param>
    /// <param name="Pred">Predicate</param>
    /// <param name="Init">Initial state</param>
    /// <typeparam name="S">State type</typeparam>
    /// <returns>Stream of aggregate states</returns>
    public Source<S> FoldWhile<S>(Func<S, A, S> Fold, Func<S, A, bool> Pred, S Init) =>
        Transform(Transducer.foldWhile(Fold, Pred, Init));

    /// <summary>
    /// Fold the values flowing through.  Values are yielded downstream when either the predicate returns
    /// `true`, or the source completes. 
    /// </summary>
    /// <param name="Fold">Binary operator</param>
    /// <param name="Pred">Predicate</param>
    /// <param name="Init">Initial state</param>
    /// <typeparam name="S">State type</typeparam>
    /// <returns>Stream of aggregate states</returns>
    public Source<S> FoldUntil<S>(Func<S, A, S> Fold, Func<S, A, bool> Pred, S Init) =>
        Transform(Transducer.foldUntil(Fold, Pred, Init));

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
        Func<S, A, bool> Pred, 
        S Init) =>
        Transform(Transducer.foldWhile(Time, Fold, Pred, Init));

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
        Func<S, A, bool> Pred, 
        S Init) =>
        Transform(Transducer.foldUntil(Time, Fold, Pred, Init));

    /// <summary>
    /// Convert `Source` to a `ProducerT` pipe component
    /// </summary>
    /// <typeparam name="M">Monad to lift (must support `IO`)</typeparam>
    /// <returns>`ProducerT`</returns>
    public ProducerT<A, M, Unit> ToProducerT<M>()
        where M : MonadIO<M> =>
        PipeT.lift<Unit, A, M, SourceIterator<A>>(GetIterator)
             .Bind(iter => PipeT.yieldRepeatIO<M, Unit, A>(iter.Read()));

    /// <summary>
    /// Convert `Source` to a `Producer` pipe component
    /// </summary>
    /// <returns>`Producer`</returns>
    public Producer<RT, A, Unit> ToProducer<RT>() =>
        ToProducerT<Eff<RT>>();
    
    /// <summary>
    /// Combine two sources into a single source.  The value streams are both
    /// merged into a new stream.  Values are yielded as they become available.
    /// </summary>
    /// <param name="lhs">Left hand side</param>
    /// <param name="rhs">Right hand side</param>
    /// <returns>Merged stream of values</returns>
    public static Source<A> operator +(Source<A> lhs, Source<A> rhs) =>
        lhs.Combine(rhs);

    /// <summary>
    /// Choose a value from the first `Source` to successfully yield 
    /// </summary>
    /// <param name="lhs">Left hand side</param>
    /// <param name="rhs">Right hand side</param>
    /// <returns>Value from the `lhs` `Source` if there are any available, if not, from `rhs`.  If
    /// `rhs` is also empty then `Errors.SourceClosed` is raised</returns>
    public static Source<A> operator |(Source<A> lhs, Source<A> rhs) =>
        lhs.Choose(rhs);

    /// <summary>
    /// Sequentially compose two actions, discarding any value produced by the first, like sequencing operators (such
    /// as the semicolon) in C#.
    /// </summary>
    /// <param name="lhs">First action to run</param>
    /// <param name="rhs">Second action to run</param>
    /// <returns>Result of the second action</returns>
    public static Source<A> operator >> (Source<A> lhs, Source<A> rhs) =>
        lhs.Bind(_ => rhs);
    
    /// <summary>
    /// Sequentially compose two actions, discarding any value produced by the first, like sequencing operators (such
    /// as the semicolon) in C#.
    /// </summary>
    /// <param name="lhs">First action to run</param>
    /// <param name="rhs">Second action to run</param>
    /// <returns>Result of the second action</returns>
    public static Source<A> operator >> (Source<A> lhs, K<Source, A> rhs) =>
        lhs.Bind(_ => rhs);
    
    /// <summary>
    /// Sequentially compose two actions.  The second action is a unit returning action, so the result of the
    /// first action is propagated. 
    /// </summary>
    /// <param name="lhs">First action to run</param>
    /// <param name="rhs">Second action to run</param>
    /// <returns>Result of the first action</returns>
    public static Source<A> operator >> (Source<A> lhs, Source<Unit> rhs) =>
        lhs.Bind(x => rhs.Map(_ => x));
    
    /// <summary>
    /// Sequentially compose two actions.  The second action is a unit returning action, so the result of the
    /// first action is propagated. 
    /// </summary>
    /// <param name="lhs">First action to run</param>
    /// <param name="rhs">Second action to run</param>
    /// <returns>Result of the first action</returns>
    public static Source<A> operator >> (Source<A> lhs, K<Source, Unit> rhs) =>
        lhs.Bind(x => rhs.Map(_ => x));

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

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Internal
    //
    
    internal abstract ValueTask<Reduced<S>> ReduceAsync<S>(S state, Reducer<A, S> reducer, CancellationToken token);
    internal abstract SourceIterator<A> GetIterator();
}
