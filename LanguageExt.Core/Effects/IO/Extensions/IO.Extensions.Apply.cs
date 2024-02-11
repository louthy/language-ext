using System;
using LanguageExt.Effects.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static partial class IOExtensions
{
    /// <summary>
    /// Applicative apply: takes the lifted function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static IO<Err, B> Apply<Err, A, B>(
        this IO<Err, Func<A, B>> mf,
        IO<Err, A> ma) =>
        new(mf.Morphism.Apply(ma.Morphism));

    /// <summary>
    /// Applicative apply: takes the lifted function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static IO<Err, C> Apply<Err, A, B, C>(
        this IO<Err, Func<A, B, C>> mf,
        IO<Err, A> ma,
        IO<Err, B> mb) =>
        mf.Map(curry).Apply(ma).Apply(mb);

    /// <summary>
    /// Applicative apply: takes the lifted function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static IO<Err, Func<B, C>> Apply<Err, A, B, C>(
        this IO<Err, Func<A, B, C>> mf,
        IO<Err, A> ma) =>
        mf.Map(curry).Apply(ma);

    /// <summary>
    /// Applicative apply: takes the lifted function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static IO<Err, D> Apply<Err, A, B, C, D>(
        this IO<Err, Func<A, B, C, D>> mf,
        IO<Err, A> ma,
        IO<Err, B> mb,
        IO<Err, C> mc) =>
        mf.Map(curry).Apply(ma).Apply(mb).Apply(mc);

    /// <summary>
    /// Applicative apply: takes the lifted function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static IO<Err, Func<C, D>> Apply<Err, A, B, C, D>(
        this IO<Err, Func<A, B, C, D>> mf,
        IO<Err, A> ma,
        IO<Err, B> mb) =>
        mf.Map(curry).Apply(ma).Apply(mb);

    /// <summary>
    /// Applicative apply: takes the lifted function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static IO<Err, Func<B, Func<C, D>>> Apply<Err, A, B, C, D>(
        this IO<Err, Func<A, B, C, D>> mf,
        IO<Err, A> ma) =>
        mf.Map(curry).Apply(ma);

    /// <summary>
    /// Applicative apply: takes the lifted function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static IO<Err, E> Apply<Err, A, B, C, D, E>(
        this IO<Err, Func<A, B, C, D, E>> mf,
        IO<Err, A> ma,
        IO<Err, B> mb,
        IO<Err, C> mc,
        IO<Err, D> md) =>
        mf.Map(curry).Apply(ma).Apply(mb).Apply(mc).Apply(md);

    /// <summary>
    /// Applicative apply: takes the lifted function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static IO<Err, Func<D, E>> Apply<Err, A, B, C, D, E>(
        this IO<Err, Func<A, B, C, D, E>> mf,
        IO<Err, A> ma,
        IO<Err, B> mb,
        IO<Err, C> mc) =>
        mf.Map(curry).Apply(ma).Apply(mb).Apply(mc);

    /// <summary>
    /// Applicative apply: takes the lifted function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static IO<Err, Func<C, Func<D, E>>> Apply<Err, A, B, C, D, E>(
        this IO<Err, Func<A, B, C, D, E>> mf,
        IO<Err, A> ma,
        IO<Err, B> mb) =>
        mf.Map(curry).Apply(ma).Apply(mb);

    /// <summary>
    /// Applicative apply: takes the lifted function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static IO<Err, Func<B, Func<C, Func<D, E>>>> Apply<Err, A, B, C, D, E>(
        this IO<Err, Func<A, B, C, D, E>> mf,
        IO<Err, A> ma) =>
        mf.Map(curry).Apply(ma);

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Non-lifted functions
    //

    /// <summary>
    /// Applicative apply: takes the function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static IO<Err, B> Apply<Err, A, B>(
        this Func<A, B> f,
        IO<Err, A> ma) =>
        IO<Err, Func<A, B>>.Pure(f).Apply(ma);

    /// <summary>
    /// Applicative apply: takes the function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static IO<Err, C> Apply<Err, A, B, C>(
        this Func<A, B, C> f,
        IO<Err, A> ma,
        IO<Err, B> mb) =>
        IO<Err, Func<A, B, C>>.Pure(f).Apply(ma, mb);

    /// <summary>
    /// Applicative apply: takes the function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static IO<Err, Func<B, C>> Apply<Err, A, B, C>(
        this Func<A, B, C> f,
        IO<Err, A> ma) =>
        IO<Err, Func<A, B, C>>.Pure(f).Apply(ma);

    /// <summary>
    /// Applicative apply: takes the function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static IO<Err, D> Apply<Err, A, B, C, D>(
        this Func<A, B, C, D> f,
        IO<Err, A> ma,
        IO<Err, B> mb,
        IO<Err, C> mc) =>
        IO<Err, Func<A, B, C, D>>.Pure(f).Apply(ma, mb, mc);

    /// <summary>
    /// Applicative apply: takes the function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static IO<Err, Func<C, D>> Apply<Err, A, B, C, D>(
        this Func<A, B, C, D> f,
        IO<Err, A> ma,
        IO<Err, B> mb) =>
        IO<Err, Func<A, B, C, D>>.Pure(f).Apply(ma, mb);

    /// <summary>
    /// Applicative apply: takes the function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static IO<Err, Func<B, Func<C, D>>> Apply<Err, A, B, C, D>(
        this Func<A, B, C, D> f,
        IO<Err, A> ma) =>
        IO<Err, Func<A, B, C, D>>.Pure(f).Apply(ma);

    /// <summary>
    /// Applicative apply: takes the function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static IO<Err, E> Apply<Err, A, B, C, D, E>(
        this Func<A, B, C, D, E> f,
        IO<Err, A> ma,
        IO<Err, B> mb,
        IO<Err, C> mc,
        IO<Err, D> md) =>
        IO<Err, Func<A, B, C, D, E>>.Pure(f).Apply(ma, mb, mc, md);

    /// <summary>
    /// Applicative apply: takes the function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static IO<Err, Func<D, E>> Apply<Err, A, B, C, D, E>(
        this Func<A, B, C, D, E> f,
        IO<Err, A> ma,
        IO<Err, B> mb,
        IO<Err, C> mc) =>
        IO<Err, Func<A, B, C, D, E>>.Pure(f).Apply(ma, mb, mc);

    /// <summary>
    /// Applicative apply: takes the function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static IO<Err, Func<C, Func<D, E>>> Apply<Err, A, B, C, D, E>(
        this Func<A, B, C, D, E> f,
        IO<Err, A> ma,
        IO<Err, B> mb) =>
        IO<Err, Func<A, B, C, D, E>>.Pure(f).Apply(ma, mb);

    /// <summary>
    /// Applicative apply: takes the function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static IO<Err, Func<B, Func<C, Func<D, E>>>> Apply<Err, A, B, C, D, E>(
        this Func<A, B, C, D, E> f,
        IO<Err, A> ma) =>
        IO<Err, Func<A, B, C, D, E>>.Pure(f).Apply(ma);
    
    /// <summary>
    /// Applicative apply: takes the lifted function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static IO<Err, B> Action<Err, A, B>(
        this IO<Err, A> ma,
        IO<Err, B> mb) =>
        new(ma.Morphism.Action(mb.Morphism));
}    
