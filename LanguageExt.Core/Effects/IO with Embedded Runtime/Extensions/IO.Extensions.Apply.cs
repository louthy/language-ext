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
    public static IO<RT, Err, B> Apply<RT, Err, A, B>(
        this IO<RT, Err, Func<A, B>> mf,
        IO<RT, Err, A> ma)
        where RT : HasIO<RT, Err> =>
        new(mf.Morphism.Apply(ma.Morphism));

    /// <summary>
    /// Applicative apply: takes the lifted function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static IO<RT, Err, C> Apply<RT, Err, A, B, C>(
        this IO<RT, Err, Func<A, B, C>> mf,
        IO<RT, Err, A> ma,
        IO<RT, Err, B> mb)
        where RT : HasIO<RT, Err> =>
        mf.Map(curry).Apply(ma).Apply(mb);

    /// <summary>
    /// Applicative apply: takes the lifted function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static IO<RT, Err, Func<B, C>> Apply<RT, Err, A, B, C>(
        this IO<RT, Err, Func<A, B, C>> mf,
        IO<RT, Err, A> ma)
        where RT : HasIO<RT, Err> =>
        mf.Map(curry).Apply(ma);

    /// <summary>
    /// Applicative apply: takes the lifted function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static IO<RT, Err, D> Apply<RT, Err, A, B, C, D>(
        this IO<RT, Err, Func<A, B, C, D>> mf,
        IO<RT, Err, A> ma,
        IO<RT, Err, B> mb,
        IO<RT, Err, C> mc)
        where RT : HasIO<RT, Err> =>
        mf.Map(curry).Apply(ma).Apply(mb).Apply(mc);

    /// <summary>
    /// Applicative apply: takes the lifted function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static IO<RT, Err, Func<C, D>> Apply<RT, Err, A, B, C, D>(
        this IO<RT, Err, Func<A, B, C, D>> mf,
        IO<RT, Err, A> ma,
        IO<RT, Err, B> mb)
        where RT : HasIO<RT, Err> =>
        mf.Map(curry).Apply(ma).Apply(mb);

    /// <summary>
    /// Applicative apply: takes the lifted function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static IO<RT, Err, Func<B, Func<C, D>>> Apply<RT, Err, A, B, C, D>(
        this IO<RT, Err, Func<A, B, C, D>> mf,
        IO<RT, Err, A> ma)
        where RT : HasIO<RT, Err> =>
        mf.Map(curry).Apply(ma);

    /// <summary>
    /// Applicative apply: takes the lifted function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static IO<RT, Err, E> Apply<RT, Err, A, B, C, D, E>(
        this IO<RT, Err, Func<A, B, C, D, E>> mf,
        IO<RT, Err, A> ma,
        IO<RT, Err, B> mb,
        IO<RT, Err, C> mc,
        IO<RT, Err, D> md)
        where RT : HasIO<RT, Err> =>
        mf.Map(curry).Apply(ma).Apply(mb).Apply(mc).Apply(md);

    /// <summary>
    /// Applicative apply: takes the lifted function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static IO<RT, Err, Func<D, E>> Apply<RT, Err, A, B, C, D, E>(
        this IO<RT, Err, Func<A, B, C, D, E>> mf,
        IO<RT, Err, A> ma,
        IO<RT, Err, B> mb,
        IO<RT, Err, C> mc)
        where RT : HasIO<RT, Err> =>
        mf.Map(curry).Apply(ma).Apply(mb).Apply(mc);

    /// <summary>
    /// Applicative apply: takes the lifted function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static IO<RT, Err, Func<C, Func<D, E>>> Apply<RT, Err, A, B, C, D, E>(
        this IO<RT, Err, Func<A, B, C, D, E>> mf,
        IO<RT, Err, A> ma,
        IO<RT, Err, B> mb)
        where RT : HasIO<RT, Err> =>
        mf.Map(curry).Apply(ma).Apply(mb);

    /// <summary>
    /// Applicative apply: takes the lifted function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static IO<RT, Err, Func<B, Func<C, Func<D, E>>>> Apply<RT, Err, A, B, C, D, E>(
        this IO<RT, Err, Func<A, B, C, D, E>> mf,
        IO<RT, Err, A> ma)
        where RT : HasIO<RT, Err> =>
        mf.Map(curry).Apply(ma);

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Non-lifted functions
    //

    /// <summary>
    /// Applicative apply: takes the function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static IO<RT, Err, B> Apply<RT, Err, A, B>(
        this Func<A, B> f,
        IO<RT, Err, A> ma)
        where RT : HasIO<RT, Err> =>
        IO<RT, Err, Func<A, B>>.Pure(f).Apply(ma);

    /// <summary>
    /// Applicative apply: takes the function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static IO<RT, Err, C> Apply<RT, Err, A, B, C>(
        this Func<A, B, C> f,
        IO<RT, Err, A> ma,
        IO<RT, Err, B> mb)
        where RT : HasIO<RT, Err> =>
        IO<RT, Err, Func<A, B, C>>.Pure(f).Apply(ma, mb);

    /// <summary>
    /// Applicative apply: takes the function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static IO<RT, Err, Func<B, C>> Apply<RT, Err, A, B, C>(
        this Func<A, B, C> f,
        IO<RT, Err, A> ma)
        where RT : HasIO<RT, Err> =>
        IO<RT, Err, Func<A, B, C>>.Pure(f).Apply(ma);

    /// <summary>
    /// Applicative apply: takes the function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static IO<RT, Err, D> Apply<RT, Err, A, B, C, D>(
        this Func<A, B, C, D> f,
        IO<RT, Err, A> ma,
        IO<RT, Err, B> mb,
        IO<RT, Err, C> mc)
        where RT : HasIO<RT, Err> =>
        IO<RT, Err, Func<A, B, C, D>>.Pure(f).Apply(ma, mb, mc);

    /// <summary>
    /// Applicative apply: takes the function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static IO<RT, Err, Func<C, D>> Apply<RT, Err, A, B, C, D>(
        this Func<A, B, C, D> f,
        IO<RT, Err, A> ma,
        IO<RT, Err, B> mb)
        where RT : HasIO<RT, Err> =>
        IO<RT, Err, Func<A, B, C, D>>.Pure(f).Apply(ma, mb);

    /// <summary>
    /// Applicative apply: takes the function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static IO<RT, Err, Func<B, Func<C, D>>> Apply<RT, Err, A, B, C, D>(
        this Func<A, B, C, D> f,
        IO<RT, Err, A> ma)
        where RT : HasIO<RT, Err> =>
        IO<RT, Err, Func<A, B, C, D>>.Pure(f).Apply(ma);

    /// <summary>
    /// Applicative apply: takes the function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static IO<RT, Err, E> Apply<RT, Err, A, B, C, D, E>(
        this Func<A, B, C, D, E> f,
        IO<RT, Err, A> ma,
        IO<RT, Err, B> mb,
        IO<RT, Err, C> mc,
        IO<RT, Err, D> md)
        where RT : HasIO<RT, Err> =>
        IO<RT, Err, Func<A, B, C, D, E>>.Pure(f).Apply(ma, mb, mc, md);

    /// <summary>
    /// Applicative apply: takes the function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static IO<RT, Err, Func<D, E>> Apply<RT, Err, A, B, C, D, E>(
        this Func<A, B, C, D, E> f,
        IO<RT, Err, A> ma,
        IO<RT, Err, B> mb,
        IO<RT, Err, C> mc)
        where RT : HasIO<RT, Err> =>
        IO<RT, Err, Func<A, B, C, D, E>>.Pure(f).Apply(ma, mb, mc);

    /// <summary>
    /// Applicative apply: takes the function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static IO<RT, Err, Func<C, Func<D, E>>> Apply<RT, Err, A, B, C, D, E>(
        this Func<A, B, C, D, E> f,
        IO<RT, Err, A> ma,
        IO<RT, Err, B> mb)
        where RT : HasIO<RT, Err> =>
        IO<RT, Err, Func<A, B, C, D, E>>.Pure(f).Apply(ma, mb);

    /// <summary>
    /// Applicative apply: takes the function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static IO<RT, Err, Func<B, Func<C, Func<D, E>>>> Apply<RT, Err, A, B, C, D, E>(
        this Func<A, B, C, D, E> f,
        IO<RT, Err, A> ma)
        where RT : HasIO<RT, Err> =>
        IO<RT, Err, Func<A, B, C, D, E>>.Pure(f).Apply(ma);
}    
