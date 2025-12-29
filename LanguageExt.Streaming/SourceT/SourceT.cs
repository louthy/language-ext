using System;
using LanguageExt.Common;
using LanguageExt.Pipes;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// A source / stream of lifted values
/// </summary>
/// <typeparam name="A">Bound value type</typeparam>
public abstract record SourceT<M, A> : 
    K<SourceT<M>, A>, 
    Monoid<SourceT<M, A>>
    where M : MonadIO<M>, Alternative<M>
{
    /// <summary>
    /// Iterate the stream, flowing values downstream to the reducer, which aggregates a
    /// result value.  This is returned lifted. 
    /// </summary>
    /// <remarks>Note, this is recursive, so `M` needs to be able to support recursion without
    /// blowing the stack.  If you have the `IO` monad in your stack, then this will automatically
    /// be the case.</remarks>
    /// <param name="state">Initial state</param>
    /// <param name="reducer">Reducer</param>
    /// <typeparam name="S">State type</typeparam>
    /// <returns>Lifted aggregate state</returns>
    public K<M, S> Reduce<S>(S state, Reducer<A, S> reducer) =>
        ReduceM(state, (s, x) => M.Pure(reducer(s, x)));
    
    /// <summary>
    /// Iterate the stream, flowing values downstream to the reducer, which aggregates a
    /// result value.  This is returned lifted. 
    /// </summary>
    /// <remarks>Note, this is recursive, so `M` needs to be able to support recursion without
    /// blowing the stack.  If you have the `IO` monad in your stack, then this will automatically
    /// be the case.</remarks>
    /// <param name="state">Initial state</param>
    /// <param name="reducer">Reducer</param>
    /// <typeparam name="S">State type</typeparam>
    /// <returns>Lifted aggregate state</returns>
    public K<M, S> FoldReduce<S>(S state, Func<S, A, S> reducer) =>
        ReduceM(state, (s, x) => M.Pure(Reduced.Continue(reducer(s, x))));

    /// <summary>
    /// Iterate the stream, flowing values downstream to the reducer, which aggregates a
    /// result value.  This is returned lifted. 
    /// </summary>
    /// <remarks>Note, this is recursive, so `M` needs to be able to support recursion without
    /// blowing the stack.  If you have the `IO` monad in your stack, then this will automatically
    /// be the case.</remarks>
    /// <param name="state">Initial state</param>
    /// <param name="reducer">Reducer</param>
    /// <typeparam name="S">State type</typeparam>
    /// <returns>Lifted aggregate state</returns>
    public K<M, S> ReduceM<S>(S state, ReducerM<M, A, S> reducer) =>
        M.Token >> (t => M.BracketIOMaybe(
                        ReduceInternalM(state,
                                        (s, mx) => t.IsCancellationRequested
                                                       ? M.Pure(Reduced.Done(s))
                                                       : mx.Bind(x => reducer(s, x)))
                           .Map(r => r.Value)));

    /// <summary>
    /// Iterate the stream, flowing values downstream to the reducer, which aggregates a
    /// result value.  This is returned lifted. 
    /// </summary>
    /// <remarks>Note, this is recursive, so `M` needs to be able to support recursion without
    /// blowing the stack.  If you have the `IO` monad in your stack, then this will automatically
    /// be the case.</remarks>
    /// <param name="state">Initial state</param>
    /// <param name="reducer">Reducer</param>
    /// <typeparam name="S">State type</typeparam>
    /// <returns>Lifted aggregate state</returns>
    public K<M, S> FoldReduceM<S>(S state, Func<S, A, K<M, S>> reducer) =>
        M.Token >> (t => M.BracketIOMaybe(
                        ReduceInternalM(
                                state,
                                (s, mx) => t.IsCancellationRequested
                                               ? M.Pure(Reduced.Done(s))
                                               : mx.Bind(x => reducer(s, x).Map(Reduced.Continue)))
                           .Map(r => r.Value)));
    
    /// <summary>
    /// A source that never yields a value
    /// </summary>
    public static SourceT<M, A> Empty =>
        EmptySourceT<M, A>.Default;

    /// <summary>
    /// Transform with a transducer
    /// </summary>
    /// <param name="transducer">Transducer to use to transform</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>Transformed source</returns>
    public SourceT<M, B> Transform<B>(TransducerM<M, A, B> transducer) =>
        new TransformSourceT<M, A, B>(this, transducer);
    
    /// <summary>
    /// Functor map
    /// </summary>
    public virtual SourceT<M, B> Map<B>(Func<A, B> f) =>
        new MapSourceT<M,A,B>(this, f);
    
    /// <summary>
    /// Monad bind
    /// </summary>
    public virtual SourceT<M, B> Bind<B>(Func<A, SourceT<M, B>> f) =>
        new BindSourceT<M, A, B>(this, f);
    
    /// <summary>
    /// Monad bind
    /// </summary>
    public SourceT<M, B> Bind<B>(Func<A, K<SourceT<M>, B>> f) =>
        Bind(x => f(x).As());
    
    /// <summary>
    /// Monad bind
    /// </summary>
    public virtual SourceT<M, B> Bind<B>(Func<A, IO<B>> f) =>
        new BindSourceT<M, A, B>(this, x => SourceT.liftIO<M, B>(f(x)));
    
    /// <summary>
    /// Filter values.  Yielding downstream when `true`
    /// </summary>
    /// <param name="f">Filter function</param>
    /// <returns>SourceT where the only values yield are those that pass the predicate</returns>
    public SourceT<M, A> Where(Func<A, bool> f) =>
        new FilterSourceT<M, A>(this, f);
    
    /// <summary>
    /// Filter values.  Yielding downstream when `true`
    /// </summary>
    /// <param name="f">Filter function</param>
    /// <returns>SourceT where the only values yield are those that pass the predicate</returns>
    public SourceT<M, A> Filter(Func<A, bool> f) =>
        new FilterSourceT<M, A>(this, f);
    
    /// <summary>
    /// Applicative apply
    /// </summary>
    public virtual SourceT<M, B> ApplyBack<B>(SourceT<M, Func<A, B>> ff) =>
        new ApplySourceT<M, A, B>(ff, this);

    /// <summary>
    /// Concatenate streams 
    /// </summary>
    /// <param name="this">Left-hand side</param>
    /// <param name="rhs">Right-hand side</param>
    /// <returns>A stream that concatenates the input streams</returns>
    public SourceT<M, A> Combine(SourceT<M, A> rhs) =>
        (this, rhs) switch
        {
            (EmptySourceT<M, A>, EmptySourceT<M, A>)         => EmptySourceT<M, A>.Default,
            (var l, EmptySourceT<M, A>)                      => l,
            (EmptySourceT<M, A>, var r)                      => r,
            (CombineSourceT<M, A> l, CombineSourceT<M, A> r) => new CombineSourceT<M, A>(l.Sources + r.Sources),
            (CombineSourceT<M, A> l, var r)                  => new CombineSourceT<M, A>(l.Sources.Add(r)),
            (var l, CombineSourceT<M, A> r)                  => new CombineSourceT<M, A>(l.Cons(r.Sources)),
            _                                                => new CombineSourceT<M, A>([this, rhs])
        };

    /// <summary>
    /// Combine two sources into a single source.  The value streams are both
    /// merged into a new stream.  Values are yielded as they become available
    /// regardless of which stream yields it.
    /// </summary>
    /// <param name="this">Left-hand side</param>
    /// <param name="rhs">Right-hand side</param>
    /// <returns>Merged stream of values</returns>
    public SourceT<M, A> Choose(SourceT<M, A> rhs) =>
        (this, rhs) switch
        {
            (EmptySourceT<M, A>, EmptySourceT<M, A>)       => EmptySourceT<M, A>.Default,
            (var l, EmptySourceT<M, A>)                    => l,
            (EmptySourceT<M, A>, var r)                    => r,
            (ChooseSourceT<M, A> l, ChooseSourceT<M, A> r) => new ChooseSourceT<M, A>(l.Sources + r.Sources),
            (ChooseSourceT<M, A> l, var r)                 => new ChooseSourceT<M, A>(l.Sources.Add(r)),
            (var l, ChooseSourceT<M, A> r)                 => new ChooseSourceT<M, A>(l.Cons(r.Sources)),
            _                                              => new ChooseSourceT<M, A>([this, rhs])
        };

    /// <summary>
    /// Combine two sources into a single source.  The value streams are both
    /// merged into a new stream.  Values are yielded as they become available
    /// regardless of which stream yields it.
    /// </summary>
    /// <param name="this">Left-hand side</param>
    /// <param name="rhs">Right-hand side</param>
    /// <returns>Merged stream of values</returns>
    public SourceT<M, A> Choose(Memo<SourceT<M>, A> rhs) =>
        Choose(rhs.Value.As());

    /// <summary>
    /// Skip items in the source
    /// </summary>
    /// <param name="amount">Amount to skip</param>
    /// <returns>Transformed source</returns>
    public SourceT<M, A> Skip(int amount) =>
        Transform(TransducerM.skip<M, A>(amount)); 

    /// <summary>
    /// Limit the number of items processed 
    /// </summary>
    /// <param name="amount">Amount to take</param>
    /// <returns>Transformed source</returns>
    public SourceT<M, A> Take(int amount) =>
        new TakeSourceT<M, A>(this, amount);

    /// <summary>
    /// Fold the values flowing through.  A value is only yielded downstream upon completion of the stream.
    /// </summary>
    /// <param name="Fold">Binary operator</param>
    /// <param name="Init">Initial state</param>
    /// <typeparam name="S">State type</typeparam>
    /// <returns>Stream of aggregate state</returns>
    public SourceT<M, S> Fold<S>(Func<S, A, S> Fold, S Init) =>
        new FoldWhileSourceT<M, A, S>(this, Schedule.Forever, Fold, _ => true, Init);

    /// <summary>
    /// Fold the values flowing through.  Values are yielded downstream when either the schedule expires, or the
    /// source completes. 
    /// </summary>
    /// <param name="Time">Schedule to control the rate of processing</param>
    /// <param name="Fold">Binary operator</param>
    /// <param name="Init">Initial state</param>
    /// <typeparam name="S">State type</typeparam>
    /// <returns>Stream of aggregate states</returns>
    public SourceT<M, S> Fold<S>(Schedule Time, Func<S, A, S> Fold, S Init) =>
        new FoldWhileSourceT<M, A, S>(this, Time, Fold, _ => true, Init);

    /// <summary>
    /// Fold the values flowing through.  Values are yielded downstream when either the predicate returns
    /// `false`, or the source completes. 
    /// </summary>
    /// <param name="Fold">Binary operator</param>
    /// <param name="Pred">Predicate</param>
    /// <param name="Init">Initial state</param>
    /// <typeparam name="S">State type</typeparam>
    /// <returns>Stream of aggregate states</returns>
    public SourceT<M, S> FoldWhile<S>(Func<S, A, S> Fold, Func<(S State, A Value), bool> Pred, S Init) =>
        new FoldWhileSourceT<M, A, S>(this, Schedule.Forever, Fold, Pred, Init);

    /// <summary>
    /// Fold the values flowing through.  Values are yielded downstream when either the predicate returns
    /// `true`, or the source completes. 
    /// </summary>
    /// <param name="Fold">Binary operator</param>
    /// <param name="Pred">Predicate</param>
    /// <param name="Init">Initial state</param>
    /// <typeparam name="S">State type</typeparam>
    /// <returns>Stream of aggregate states</returns>
    public SourceT<M, S> FoldUntil<S>(Func<S, A, S> Fold, Func<(S State, A Value), bool> Pred, S Init) =>
        new FoldUntilSourceT<M, A, S>(this, Schedule.Forever, Fold, Pred, Init);

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
    public SourceT<M, S> FoldWhile<S>(
        Schedule Time,
        Func<S, A, S> Fold,
        Func<(S State, A Value), bool> Pred,
        S Init) =>
        new FoldWhileSourceT<M, A, S>(this, Time, Fold, Pred, Init);

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
    public SourceT<M, S> FoldUntil<S>(
        Schedule Time,
        Func<S, A, S> Fold, 
        Func<(S State, A Value), bool> Pred, 
        S Init) =>
        new FoldUntilSourceT<M, A, S>(this, Time, Fold, Pred, Init);

    /// <summary>
    /// Convert `SourceT` to a `ProducerT` pipe component
    /// </summary>
    /// <typeparam name="M">Monad to lift (must support `IO`)</typeparam>
    /// <returns>`ProducerT`</returns>
    public ProducerT<A, M, Unit> ToProducerT() =>
        ProducerT.yieldAll(this);
    
    /// <summary>
    /// Concatenate streams 
    /// </summary>
    /// <param name="lhs">Left-hand side</param>
    /// <param name="rhs">Right-hand side</param>
    /// <returns>A stream that concatenates the input streams</returns>
    public static SourceT<M, A> operator +(SourceT<M, A> lhs, SourceT<M, A> rhs) =>
        lhs.Combine(rhs);

    /// <summary>
    /// Combine two sources into a single source.  The value streams are both
    /// merged into a new stream.  Values are yielded as they become available
    /// regardless of which stream yields it.
    /// </summary>
    /// <param name="lhs">Left-hand side</param>
    /// <param name="rhs">Right-hand side</param>
    /// <returns>Merged stream of values</returns>
    public static SourceT<M, A> operator |(SourceT<M, A> lhs, SourceT<M, A> rhs) =>
        lhs.Choose(rhs);

    /// <summary>
    /// Sequentially compose two actions, discarding any value produced by the first, like sequencing operators (such
    /// as the semicolon) in C#.
    /// </summary>
    /// <param name="lhs">First action to run</param>
    /// <param name="rhs">Second action to run</param>
    /// <returns>Result of the second action</returns>
    public static SourceT<M, A> operator >> (SourceT<M, A> lhs, SourceT<M, A> rhs) =>
        lhs.Bind(_ => rhs);
    
    /// <summary>
    /// Sequentially compose two actions, discarding any value produced by the first, like sequencing operators (such
    /// as the semicolon) in C#.
    /// </summary>
    /// <param name="lhs">First action to run</param>
    /// <param name="rhs">Second action to run</param>
    /// <returns>Result of the second action</returns>
    public static SourceT<M, A> operator >> (SourceT<M, A> lhs, K<SourceT<M>, A> rhs) =>
        lhs.Bind(_ => rhs);
    
    /// <summary>
    /// Sequentially compose two actions.  The second action is a unit returning action, so the result of the
    /// first action is propagated. 
    /// </summary>
    /// <param name="lhs">First action to run</param>
    /// <param name="rhs">Second action to run</param>
    /// <returns>Result of the first action</returns>
    public static SourceT<M, A> operator >> (SourceT<M, A> lhs, SourceT<M, Unit> rhs) =>
        lhs.Bind(x => rhs.Map(_ => x));
    
    /// <summary>
    /// Sequentially compose two actions.  The second action is a unit returning action, so the result of the
    /// first action is propagated. 
    /// </summary>
    /// <param name="lhs">First action to run</param>
    /// <param name="rhs">Second action to run</param>
    /// <returns>Result of the first action</returns>
    public static SourceT<M, A> operator >> (SourceT<M, A> lhs, K<SourceT<M>, Unit> rhs) =>
        lhs.Bind(x => rhs.Map(_ => x));
    
    public static implicit operator SourceT<M, A>(IO<A> ma) =>
        SourceT.liftIO<M, A>(ma);
    
    public static implicit operator SourceT<M, A>(Pure<A> ma) =>
        SourceT.liftM(M.Pure(ma.Value));

    public static implicit operator SourceT<M, A>(Fail<Error> ma) =>
        IO.fail<A>(ma.Value);

    /// <summary>
    /// Functor map
    /// </summary>
    public SourceT<M, B> Select<B>(Func<A, B> f) =>
        Map(f);
    
    /// <summary>
    /// Monad bind
    /// </summary>
    public SourceT<M, C> SelectMany<B, C>(Func<A, SourceT<M, B>> bind, Func<A, B, C> project) =>
        Bind(a => bind(a).Map(b => project(a, b)));
    
    /// <summary>
    /// Monad bind
    /// </summary>
    public SourceT<M, C> SelectMany<B, C>(Func<A, K<M, B>> bind, Func<A, B, C> project) =>
        SelectMany(x => SourceT.liftM(bind(x)), project);
    
    /// <summary>
    /// Monad bind
    /// </summary>
    public SourceT<M, C> SelectMany<B, C>(Func<A, IO<B>> bind, Func<A, B, C> project) =>
        SelectMany(a => SourceT.liftIO<M, B>(bind(a)), project);
    
    /// <summary>
    /// Monad bind
    /// </summary>
    public SourceT<M, C> SelectMany<B, C>(Func<A, Pure<B>> bind, Func<A, B, C> project) =>
        Map(a => project(a, bind(a).Value));

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Internal
    //

    /// <summary>
    /// Iterate the stream, flowing values downstream to the reducer, which aggregates a
    /// result value.  This is returned lifted. 
    /// </summary>
    /// <remarks>Note, this is recursive, so `M` needs to be able to support recursion without
    /// blowing the stack.  If you have the `IO` monad in your stack, then this will automatically
    /// be the case.</remarks>
    /// <param name="state">Initial state</param>
    /// <param name="reducer">Reducer</param>
    /// <typeparam name="S">State type</typeparam>
    /// <returns>Lifted aggregate state</returns>
    public K<M, Reduced<S>> ReduceInternal<S>(S state, ReducerM<M, A, S> reducer) =>
        ReduceInternalM(state, (s, mx) => mx.Bind(x => reducer(s, x)));
    
    /// <summary>
    /// Iterate the stream, flowing values downstream to the reducer, which aggregates a
    /// result value.  This is returned lifted. 
    /// </summary>
    /// <remarks>Note, this is recursive, so `M` needs to be able to support recursion without
    /// blowing the stack.  If you have the `IO` monad in your stack, then this will automatically
    /// be the case.</remarks>
    /// <param name="state">Initial state</param>
    /// <param name="reducer">Reducer</param>
    /// <typeparam name="S">State type</typeparam>
    /// <returns>Lifted aggregate state</returns>
    public abstract K<M, Reduced<S>> ReduceInternalM<S>(S state, ReducerM<M, K<M, A>, S> reducer);
}
