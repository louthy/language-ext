using System;
using LanguageExt.Traits;

namespace LanguageExt;

public static class TransducerM
{
    /// <summary>
    /// Identity transducer.  Has no effect on values flowing through.
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Identity transducer</returns>
    public static TransducerM<M, A, A> identity<M, A>() => 
        IdentityTransducerM<M, A>.Default;

    /// <summary>
    /// Constant transducer.  Ignores all incoming values and yields the constant value. 
    /// </summary>
    /// <typeparam name="B">Constant value type</typeparam>
    /// <returns>Constant transducer</returns>
    public static TransducerM<M, A, B> constant<M, A, B>(B value) => 
        new ConstTransducerM<M, A, B>(value);

    /// <summary>
    /// Compose transducers together.  The output of each transducer is fed into the next transducer.
    /// </summary>
    /// <param name="ta">First transducer</param>
    /// <param name="tb">Second transducer</param>
    /// <returns>Composed transducer</returns>
    public static TransducerM<M, A, C> compose<M, A, B, C>(
        TransducerM<M, A, B> ta, 
        TransducerM<M, B, C> tb) =>
        new ComposeTransducerM<M, A, B, C>(ta, tb);

    /// <summary>
    /// Compose transducers together.  The output of each transducer is fed into the next transducer.
    /// </summary>
    /// <param name="ta">First transducer</param>
    /// <param name="tb">Second transducer</param>
    /// <param name="tc">Third transducer</param>
    /// <returns>Composed transducer</returns>
    public static TransducerM<M, A, D> compose<M, A, B, C, D>(
        TransducerM<M, A, B> ta, 
        TransducerM<M, B, C> tb, 
        TransducerM<M, C, D> tc) =>
        new ComposeTransducerM<M, A, B, C, D>(ta, tb, tc);

    /// <summary>
    /// Compose transducers together.  The output of each transducer is fed into the next transducer.
    /// </summary>
    /// <param name="ta">First transducer</param>
    /// <param name="tb">Second transducer</param>
    /// <param name="tc">Third transducer</param>
    /// <param name="td">Fourth transducer</param>
    /// <returns>Composed transducer</returns>
    public static TransducerM<M, A, E> compose<M, A, B, C, D, E>(
        TransducerM<M, A, B> ta, 
        TransducerM<M, B, C> tb, 
        TransducerM<M, C, D> tc, 
        TransducerM<M, D, E> td) =>
        new ComposeTransducerM<M, A, B, C, D, E>(ta, tb, tc, td);

    /// <summary>
    /// Compose transducers together.  The output of each transducer is fed into the next transducer.
    /// </summary>
    /// <param name="ta">First transducer</param>
    /// <param name="tb">Second transducer</param>
    /// <param name="tc">Third transducer</param>
    /// <param name="td">Fourth transducer</param>
    /// <param name="te">Fifth transducer</param>
    /// <returns>Composed transducer</returns>
    public static TransducerM<M, A, F> compose<M, A, B, C, D, E, F>(
        TransducerM<M, A, B> ta, 
        TransducerM<M, B, C> tb, 
        TransducerM<M, C, D> tc, 
        TransducerM<M, D, E> td, 
        TransducerM<M, E, F> te) =>
        new ComposeTransducerM<M, A, B, C, D, E, F>(ta, tb, tc, td, te);

    /// <summary>
    /// Compose transducers together.  The output of each transducer is fed into the next transducer.
    /// </summary>
    /// <param name="ta">First transducer</param>
    /// <param name="tb">Second transducer</param>
    /// <param name="tc">Third transducer</param>
    /// <param name="td">Fourth transducer</param>
    /// <param name="te">Fifth transducer</param>
    /// <param name="tf">Sixth transducer</param>
    /// <returns>Composed transducer</returns>
    public static TransducerM<M, A, G> compose<M, A, B, C, D, E, F, G>(
        TransducerM<M, A, B> ta, 
        TransducerM<M, B, C> tb, 
        TransducerM<M, C, D> tc, 
        TransducerM<M, D, E> td, 
        TransducerM<M, E, F> te, 
        TransducerM<M, F, G> tf) =>
        new ComposeTransducerM<M, A, B, C, D, E, F, G>(ta, tb, tc, td, te, tf);
    
    /// <summary>
    /// Skip `amount` items in the sequence before yielding
    /// </summary>
    /// <param name="amount">Number of items to skip</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns>Transducer that skips values</returns>
    public static TransducerM<M, A, A> skip<M, A>(int amount) 
        where M : Applicative<M> =>
        new SkipTransducerM<M, A>(amount);

    /// <summary>
    /// Take `amount` items in the sequence before terminating
    /// </summary>
    /// <param name="amount">Number of items to take</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns>Transducer that takes `amount` values only</returns>
    public static TransducerM<M, A, A> take<M, A>(int amount) 
        where M : Applicative<M> =>
        new TakeTransducerM<M, A>(amount);

    /// <summary>
    /// Functor map transducer
    /// </summary>
    /// <param name="f">Function to map values of type `A` to values of type `B`</param>
    /// <typeparam name="A">Input value type</typeparam>
    /// <typeparam name="B">Output value type</typeparam>
    /// <returns>Mapping transducer</returns>
    public static TransducerM<M, A, B> map<M, A, B>(Func<A, B> f) =>
        new MapTransducerM<M, A, B>(f);
    
    /// <summary>
    /// Applicative filter transducer 
    /// </summary>
    /// <param name="predicate">Filters each value flowing through the transducer.  If `true` the value will flow
    /// downstream;  if `false`, the value is dropped</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Filtering transducer</returns>
    public static TransducerM<M, A, A> filter<M, A>(Func<A, bool> predicate)
        where M : Applicative<M> =>
        new FilterTransducerM<M, A>(predicate);

    /// <summary>
    /// Monad bind transducer
    /// </summary>
    /// <remarks>
    /// Chains two transducers together
    /// </remarks>
    /// <param name="ta">Initial transducer to run</param>
    /// <param name="f">Chaining function to run with the result of `ta` that will produce a new `Transducer`</param>
    /// <typeparam name="Env">Input value type</typeparam>
    /// <typeparam name="A">Result value type of the first transducer</typeparam>
    /// <typeparam name="B">Result value type of returned transducer</typeparam>
    /// <returns>A monadic bind transducer operation</returns>
    public static TransducerM<M, Env, B> bind<M, Env, A, B>(TransducerM<M, Env, A> ta, Func<A, K<TransduceFromM<M, Env>, B>> f) =>
        new BindTransducerM1<M, Env, A, B>(ta, f);    

    /// <summary>
    /// Monad bind transducer
    /// </summary>
    /// <remarks>
    /// Chains two transducers together
    /// </remarks>
    /// <param name="ta">Initial transducer to run</param>
    /// <param name="f">Chaining function to run with the result of `ta` that will produce a new `Transducer`</param>
    /// <typeparam name="Env">Input value type</typeparam>
    /// <typeparam name="A">Result value type of the first transducer</typeparam>
    /// <typeparam name="B">Result value type of returned transducer</typeparam>
    /// <returns>A monadic bind transducer operation</returns>
    public static TransducerM<M, Env, B> bind<M, Env, A, B>(TransducerM<M, Env, A> ta, Func<A, TransducerM<M, Env, B>> f) =>
        new BindTransducerM2<M, Env, A, B>(ta, f);

    /// <summary>
    /// Fold items in the stream while the predicate returns true; once the predicate returns false, the
    /// aggregated value is yielded downstream. 
    /// </summary>
    /// <param name="Folder">Aggregating binary fold function</param>
    /// <param name="Pred">Predicate</param>
    /// <param name="State">Initial state</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <typeparam name="S">Yielded aggregate value type</typeparam>
    /// <returns>Aggregating binary folding transducer</returns>
    public static TransducerM<M, A, S> foldWhile<M, A, S>(
        Func<S, A, S> Folder,
        Func<S, A, bool> Pred,
        S State) 
        where M : Applicative<M> =>
        new FoldWhileTransducerM<M, A, S>(Folder, Pred, State);
    
    /// <summary>
    /// Fold items in the stream while the predicate returns true; once the predicate returns false, the
    /// aggregated value is yielded downstream. 
    /// </summary>
    /// <param name="Schedule">Schedule for each yielded item</param>
    /// <param name="Folder">Aggregating binary fold function</param>
    /// <param name="Pred">Predicate</param>
    /// <param name="State">Initial state</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <typeparam name="S">Yielded aggregate value type</typeparam>
    /// <returns>Aggregating binary folding transducer</returns>
    public static TransducerM<M, A, S> foldWhile<M, A, S>(
        Schedule Schedule,
        Func<S, A, S> Folder,
        Func<S, A, bool> Pred,
        S State) 
        where M : Applicative<M> =>
        new FoldWhileTransducerM2<M, A, S>(Schedule, Folder, Pred, State);
    

    /// <summary>
    /// Fold items in the stream until the predicate returns true; once the predicate returns true, the
    /// aggregated value is yielded downstream. 
    /// </summary>
    /// <param name="Folder">Aggregating binary fold function</param>
    /// <param name="Pred">Predicate</param>
    /// <param name="State">Initial state</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <typeparam name="S">Yielded aggregate value type</typeparam>
    /// <returns>Aggregating binary folding transducer</returns>
    public static TransducerM<M, A, S> foldUntil<M, A, S>(
        Func<S, A, S> Folder,
        Func<S, A, bool> Pred,
        S State) 
        where M : Applicative<M> =>
        new FoldUntilTransducerM<M, A, S>(Folder, Pred, State);
    
    /// <summary>
    /// Fold items in the stream until the predicate returns true; once the predicate returns true, the
    /// aggregated value is yielded downstream. 
    /// </summary>
    /// <param name="Schedule">Schedule for each yielded item</param>
    /// <param name="Folder">Aggregating binary fold function</param>
    /// <param name="Pred">Predicate</param>
    /// <param name="State">Initial state</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <typeparam name="S">Yielded aggregate value type</typeparam>
    /// <returns>Aggregating binary folding transducer</returns>
    public static TransducerM<M, A, S> foldUntil<M, A, S>(
        Schedule Schedule,
        Func<S, A, S> Folder,
        Func<S, A, bool> Pred,
        S State) 
        where M : Applicative<M> =>
        new FoldUntilTransducerM2<M, A, S>(Schedule, Folder, Pred, State);    
}
