using System;
using LanguageExt.Traits;

namespace LanguageExt;

public static class Transducer
{
    /// <summary>
    /// Identity transducer.  Has no effect on values flowing through.
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Identity transducer</returns>
    public static Transducer<A, A> identity<A>() => 
        IdentityTransducer<A>.Default;

    /// <summary>
    /// Constant transducer.  Ignores all incoming values and yields the constant value. 
    /// </summary>
    /// <typeparam name="B">Constant value type</typeparam>
    /// <returns>Constant transducer</returns>
    public static Transducer<A, B> constant<A, B>(B value) => 
        new ConstTransducer<A, B>(value);

    /// <summary>
    /// Compose transducers together.  The output of each transducer is fed into the next transducer.
    /// </summary>
    /// <param name="ta">First transducer</param>
    /// <param name="tb">Second transducer</param>
    /// <returns>Composed transducer</returns>
    public static Transducer<A, C> compose<A, B, C>(
        Transducer<A, B> ta, 
        Transducer<B, C> tb) =>
        new ComposeTransducer<A, B, C>(ta, tb);

    /// <summary>
    /// Compose transducers together.  The output of each transducer is fed into the next transducer.
    /// </summary>
    /// <param name="ta">First transducer</param>
    /// <param name="tb">Second transducer</param>
    /// <param name="tc">Third transducer</param>
    /// <returns>Composed transducer</returns>
    public static Transducer<A, D> compose<A, B, C, D>(
        Transducer<A, B> ta, 
        Transducer<B, C> tb, 
        Transducer<C, D> tc) =>
        new ComposeTransducer<A, B, C, D>(ta, tb, tc);

    /// <summary>
    /// Compose transducers together.  The output of each transducer is fed into the next transducer.
    /// </summary>
    /// <param name="ta">First transducer</param>
    /// <param name="tb">Second transducer</param>
    /// <param name="tc">Third transducer</param>
    /// <param name="td">Fourth transducer</param>
    /// <returns>Composed transducer</returns>
    public static Transducer<A, E> compose<A, B, C, D, E>(
        Transducer<A, B> ta, 
        Transducer<B, C> tb, 
        Transducer<C, D> tc, 
        Transducer<D, E> td) =>
        new ComposeTransducer<A, B, C, D, E>(ta, tb, tc, td);

    /// <summary>
    /// Compose transducers together.  The output of each transducer is fed into the next transducer.
    /// </summary>
    /// <param name="ta">First transducer</param>
    /// <param name="tb">Second transducer</param>
    /// <param name="tc">Third transducer</param>
    /// <param name="td">Fourth transducer</param>
    /// <param name="te">Fifth transducer</param>
    /// <returns>Composed transducer</returns>
    public static Transducer<A, F> compose<A, B, C, D, E, F>(
        Transducer<A, B> ta, 
        Transducer<B, C> tb, 
        Transducer<C, D> tc, 
        Transducer<D, E> td, 
        Transducer<E, F> te) =>
        new ComposeTransducer<A, B, C, D, E, F>(ta, tb, tc, td, te);

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
    public static Transducer<A, G> compose<A, B, C, D, E, F, G>(
        Transducer<A, B> ta, 
        Transducer<B, C> tb, 
        Transducer<C, D> tc, 
        Transducer<D, E> td, 
        Transducer<E, F> te, 
        Transducer<F, G> tf) =>
        new ComposeTransducer<A, B, C, D, E, F, G>(ta, tb, tc, td, te, tf);
    
    /// <summary>
    /// Skip `amount` items in the sequence before yielding
    /// </summary>
    /// <param name="amount">Number of items to skip</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns>Transducer that skips values</returns>
    public static Transducer<A, A> skip<A>(int amount) =>
        new SkipTransducer<A>(amount);

    /// <summary>
    /// Take `amount` items in the sequence before terminating
    /// </summary>
    /// <param name="amount">Number of items to take</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns>Transducer that takes `amount` values only</returns>
    public static Transducer<A, A> take<A>(int amount) =>
        new TakeTransducer<A>(amount);

    /// <summary>
    /// Functor map transducer
    /// </summary>
    /// <param name="f">Function to map values of type `A` to values of type `B`</param>
    /// <typeparam name="A">Input value type</typeparam>
    /// <typeparam name="B">Output value type</typeparam>
    /// <returns>Mapping transducer</returns>
    public static Transducer<A, B> map<A, B>(Func<A, B> f) =>
        new MapTransducer<A, B>(f);
    
    /// <summary>
    /// Applicative filter transducer 
    /// </summary>
    /// <param name="predicate">Filters each value flowing through the transducer.  If `true` the value flows downstream;
    /// if `false`, the value is dropped</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Filtering transducer</returns>
    public static Transducer<A, A> filter<A>(Func<A, bool> predicate) =>
        new FilterTransducer<A>(predicate);

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
    public static Transducer<Env, B> bind<Env, A, B>(Transducer<Env, A> ta, Func<A, K<TransduceFrom<Env>, B>> f) =>
        new BindTransducer1<Env, A, B>(ta, f);    

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
    public static Transducer<Env, B> bind<Env, A, B>(Transducer<Env, A> ta, Func<A, Transducer<Env, B>> f) =>
        new BindTransducer2<Env, A, B>(ta, f);

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
    public static Transducer<A, S> foldWhile<A, S>(
        Func<S, A, S> Folder,
        Func<S, A, bool> Pred,
        S State) =>
        new FoldWhileTransducer<A, S>(Folder, Pred, State);
    
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
    public static Transducer<A, S> foldWhile<A, S>(
        Schedule Schedule,
        Func<S, A, S> Folder,
        Func<S, A, bool> Pred,
        S State) =>
        new FoldWhileTransducer2<A, S>(Schedule, Folder, Pred, State);
    

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
    public static Transducer<A, S> foldUntil<A, S>(
        Func<S, A, S> Folder,
        Func<S, A, bool> Pred,
        S State) =>
        new FoldUntilTransducer<A, S>(Folder, Pred, State);
    
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
    public static Transducer<A, S> foldUntil<A, S>(
        Schedule Schedule,
        Func<S, A, S> Folder,
        Func<S, A, bool> Pred,
        S State) =>
        new FoldUntilTransducer2<A, S>(Schedule, Folder, Pred, State);    
}
