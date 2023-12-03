#nullable enable
using LanguageExt.HKT;

namespace LanguageExt.Transducers;

/// <summary>
/// Transducers are composable algorithmic transformations. They are independent from the context of their input and
/// output sources and specify only the essence of the transformation in terms of an individual element. Because
/// transducers are decoupled from input or output sources, they can be used in many different processes -
/// collections, streams, channels, observables, etc. Transducers compose directly, without awareness of input or
/// creation of intermediate aggregates.
/// </summary>
/// <typeparam name="A">Input value type</typeparam>
/// <typeparam name="B">Output value type</typeparam>
public abstract record Transducer<A, B> : KArr<Any, A, B>
{
    /// <summary>
    /// Transform the transducer using the reducer provided 
    /// </summary>
    /// <param name="reduce">Reducer</param>
    /// <typeparam name="S">State type</typeparam>
    /// <returns>Reducer that captures the transformation of the `Transducer` and the provided reducer</returns>
    public abstract Reducer<A, S> Transform<S>(Reducer<B, S> reduce);

    /// <summary>
    /// Transducer from `A` to `B`
    /// </summary>
    public virtual Transducer<A, B> Morphism =>
        this;

    /// <summary>
    /// Compose this transducer with the one provided
    /// </summary>
    public virtual Transducer<A, C> Compose<C>(Transducer<B, C> next) =>
        new ComposeTransducer<A, B, C>(this, next);

    public static Transducer<Unit, B> operator |(A prev, Transducer<A, B> next) =>
        Transducer.constant<Unit, A>(prev).Compose(next);

    public static Transducer<A, B> operator |(Transducer<A, B> prev, Transducer<B, B> next) =>
        prev.Compose(next);

    public static Transducer<A, B> operator |(Transducer<A, A> prev, Transducer<A, B> next) =>
        prev.Compose(next);
}
