/*
#nullable enable
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
    public static IO<Err, B> apply<Err, A, B>(
        IO<Err, Func<A, B>> mf,
        IO<Err, A> ma) =>
        new(mf.Morphism.Apply(ma.Morphism));

    /// <summary>
    /// Applicative apply: takes the lifted function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static IO<Err, C> apply<Err, A, B, C>(
        IO<Err, Func<A, B, C>> mf,
        IO<Err, A> ma,
        IO<Err, B> mb) =>
        mf.Map(curry).Apply(ma).Apply(mb);

    /// <summary>
    /// Applicative apply: takes the lifted function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static IO<Err, Func<B, C>> apply<Err, A, B, C>(
        IO<Err, Func<A, B, C>> mf,
        IO<Err, A> ma) =>
        mf.Map(curry).Apply(ma);

    /// <summary>
    /// Applicative apply: takes the lifted function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static IO<Err, D> apply<Err, A, B, C, D>(
        IO<Err, Func<A, B, C, D>> mf,
        IO<Err, A> ma,
        IO<Err, B> mb,
        IO<Err, C> mc) =>
        mf.Map(curry).Apply(ma).Apply(mb).Apply(mc);

    /// <summary>
    /// Applicative apply: takes the lifted function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static IO<Err, Func<C, D>> apply<Err, A, B, C, D>(
        IO<Err, Func<A, B, C, D>> mf,
        IO<Err, A> ma,
        IO<Err, B> mb) =>
        mf.Map(curry).Apply(ma).Apply(mb);

    /// <summary>
    /// Applicative apply: takes the lifted function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static IO<Err, Func<B, Func<C, D>>> apply<Err, A, B, C, D>(
        IO<Err, Func<A, B, C, D>> mf,
        IO<Err, A> ma) =>
        mf.Map(curry).Apply(ma);

    /// <summary>
    /// Applicative apply: takes the lifted function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static IO<Err, E> apply<Err, A, B, C, D, E>(
        IO<Err, Func<A, B, C, D, E>> mf,
        IO<Err, A> ma,
        IO<Err, B> mb,
        IO<Err, C> mc,
        IO<Err, D> md) =>
        mf.Map(curry).Apply(ma).Apply(mb).Apply(mc).Apply(md);

    /// <summary>
    /// Applicative apply: takes the lifted function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static IO<Err, Func<D, E>> apply<Err, A, B, C, D, E>(
        IO<Err, Func<A, B, C, D, E>> mf,
        IO<Err, A> ma,
        IO<Err, B> mb,
        IO<Err, C> mc) =>
        mf.Map(curry).Apply(ma).Apply(mb).Apply(mc);

    /// <summary>
    /// Applicative apply: takes the lifted function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static IO<Err, Func<C, Func<D, E>>> apply<Err, A, B, C, D, E>(
        IO<Err, Func<A, B, C, D, E>> mf,
        IO<Err, A> ma,
        IO<Err, B> mb) =>
        mf.Map(curry).Apply(ma).Apply(mb);

    /// <summary>
    /// Applicative apply: takes the lifted function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static IO<Err, Func<B, Func<C, Func<D, E>>>> apply<Err, A, B, C, D, E>(
        IO<Err, Func<A, B, C, D, E>> mf,
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
    public static IO<Err, B> apply<Err, A, B>(
        Func<A, B> f,
        IO<Err, A> ma) =>
        IO<Err, Func<A, B>>.Pure(f).Apply(ma);

    /// <summary>
    /// Applicative apply: takes the function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static IO<Err, C> apply<Err, A, B, C>(
        Func<A, B, C> f,
        IO<Err, A> ma,
        IO<Err, B> mb) =>
        IO<Err, Func<A, B, C>>.Pure(f).Apply(ma, mb);

    /// <summary>
    /// Applicative apply: takes the function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static IO<Err, Func<B, C>> apply<Err, A, B, C>(
        Func<A, B, C> f,
        IO<Err, A> ma) =>
        IO<Err, Func<A, B, C>>.Pure(f).Apply(ma);

    /// <summary>
    /// Applicative apply: takes the function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static IO<Err, D> apply<Err, A, B, C, D>(
        Func<A, B, C, D> f,
        IO<Err, A> ma,
        IO<Err, B> mb,
        IO<Err, C> mc) =>
        IO<Err, Func<A, B, C, D>>.Pure(f).Apply(ma, mb, mc);

    /// <summary>
    /// Applicative apply: takes the function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static IO<Err, Func<C, D>> apply<Err, A, B, C, D>(
        Func<A, B, C, D> f,
        IO<Err, A> ma,
        IO<Err, B> mb) =>
        IO<Err, Func<A, B, C, D>>.Pure(f).Apply(ma, mb);

    /// <summary>
    /// Applicative apply: takes the function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static IO<Err, Func<B, Func<C, D>>> apply<Err, A, B, C, D>(
        Func<A, B, C, D> f,
        IO<Err, A> ma) =>
        IO<Err, Func<A, B, C, D>>.Pure(f).Apply(ma);

    /// <summary>
    /// Applicative apply: takes the function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static IO<Err, E> apply<Err, A, B, C, D, E>(
        Func<A, B, C, D, E> f,
        IO<Err, A> ma,
        IO<Err, B> mb,
        IO<Err, C> mc,
        IO<Err, D> md) =>
        IO<Err, Func<A, B, C, D, E>>.Pure(f).Apply(ma, mb, mc, md);

    /// <summary>
    /// Applicative apply: takes the function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static IO<Err, Func<D, E>> apply<Err, A, B, C, D, E>(
        Func<A, B, C, D, E> f,
        IO<Err, A> ma,
        IO<Err, B> mb,
        IO<Err, C> mc) =>
        IO<Err, Func<A, B, C, D, E>>.Pure(f).Apply(ma, mb, mc);

    /// <summary>
    /// Applicative apply: takes the function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static IO<Err, Func<C, Func<D, E>>> apply<Err, A, B, C, D, E>(
        Func<A, B, C, D, E> f,
        IO<Err, A> ma,
        IO<Err, B> mb) =>
        IO<Err, Func<A, B, C, D, E>>.Pure(f).Apply(ma, mb);

    /// <summary>
    /// Applicative apply: takes the function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static IO<Err, Func<B, Func<C, Func<D, E>>>> apply<Err, A, B, C, D, E>(
        Func<A, B, C, D, E> f,
        IO<Err, A> ma) =>
        IO<Err, Func<A, B, C, D, E>>.Pure(f).Apply(ma);
}    
*/
