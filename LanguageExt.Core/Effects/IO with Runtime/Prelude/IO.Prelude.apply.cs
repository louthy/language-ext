using System;
using LanguageExt.Effects.Traits;

namespace LanguageExt;

public static partial class Prelude
{
    /// <summary>
    /// Applicative apply: takes the lifted function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static IO<RT, Err, B> apply<RT, Err, A, B>(
        IO<RT, Err, Func<A, B>> mf,
        IO<RT, Err, A> ma)
        where RT : HasIO<RT, Err> =>
        new(mf.Morphism.Apply(ma.Morphism));

    /// <summary>
    /// Applicative apply: takes the lifted function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static IO<RT, Err, C> apply<RT, Err, A, B, C>(
        IO<RT, Err, Func<A, B, C>> mf,
        IO<RT, Err, A> ma,
        IO<RT, Err, B> mb)
        where RT : HasIO<RT, Err> =>
        mf.Map(curry).Apply(ma).Apply(mb);

    /// <summary>
    /// Applicative apply: takes the lifted function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static IO<RT, Err, Func<B, C>> apply<RT, Err, A, B, C>(
        IO<RT, Err, Func<A, B, C>> mf,
        IO<RT, Err, A> ma)
        where RT : HasIO<RT, Err> =>
        mf.Map(curry).Apply(ma);

    /// <summary>
    /// Applicative apply: takes the lifted function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static IO<RT, Err, D> apply<RT, Err, A, B, C, D>(
        IO<RT, Err, Func<A, B, C, D>> mf,
        IO<RT, Err, A> ma,
        IO<RT, Err, B> mb,
        IO<RT, Err, C> mc)
        where RT : HasIO<RT, Err> =>
        mf.Map(curry).Apply(ma).Apply(mb).Apply(mc);

    /// <summary>
    /// Applicative apply: takes the lifted function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static IO<RT, Err, Func<C, D>> apply<RT, Err, A, B, C, D>(
        IO<RT, Err, Func<A, B, C, D>> mf,
        IO<RT, Err, A> ma,
        IO<RT, Err, B> mb)
        where RT : HasIO<RT, Err> =>
        mf.Map(curry).Apply(ma).Apply(mb);

    /// <summary>
    /// Applicative apply: takes the lifted function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static IO<RT, Err, Func<B, Func<C, D>>> apply<RT, Err, A, B, C, D>(
        IO<RT, Err, Func<A, B, C, D>> mf,
        IO<RT, Err, A> ma)
        where RT : HasIO<RT, Err> =>
        mf.Map(curry).Apply(ma);

    /// <summary>
    /// Applicative apply: takes the lifted function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static IO<RT, Err, E> apply<RT, Err, A, B, C, D, E>(
        IO<RT, Err, Func<A, B, C, D, E>> mf,
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
    public static IO<RT, Err, Func<D, E>> apply<RT, Err, A, B, C, D, E>(
        IO<RT, Err, Func<A, B, C, D, E>> mf,
        IO<RT, Err, A> ma,
        IO<RT, Err, B> mb,
        IO<RT, Err, C> mc)
        where RT : HasIO<RT, Err> =>
        mf.Map(curry).Apply(ma).Apply(mb).Apply(mc);

    /// <summary>
    /// Applicative apply: takes the lifted function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static IO<RT, Err, Func<C, Func<D, E>>> apply<RT, Err, A, B, C, D, E>(
        IO<RT, Err, Func<A, B, C, D, E>> mf,
        IO<RT, Err, A> ma,
        IO<RT, Err, B> mb)
        where RT : HasIO<RT, Err> =>
        mf.Map(curry).Apply(ma).Apply(mb);

    /// <summary>
    /// Applicative apply: takes the lifted function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static IO<RT, Err, Func<B, Func<C, Func<D, E>>>> apply<RT, Err, A, B, C, D, E>(
        IO<RT, Err, Func<A, B, C, D, E>> mf,
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
    public static IO<RT, Err, B> apply<RT, Err, A, B>(
        Func<A, B> f,
        IO<RT, Err, A> ma)
        where RT : HasIO<RT, Err> =>
        IO<RT, Err, Func<A, B>>.Pure(f).Apply(ma);

    /// <summary>
    /// Applicative apply: takes the function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static IO<RT, Err, C> apply<RT, Err, A, B, C>(
        Func<A, B, C> f,
        IO<RT, Err, A> ma,
        IO<RT, Err, B> mb)
        where RT : HasIO<RT, Err> =>
        IO<RT, Err, Func<A, B, C>>.Pure(f).Apply(ma, mb);

    /// <summary>
    /// Applicative apply: takes the function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static IO<RT, Err, Func<B, C>> apply<RT, Err, A, B, C>(
        Func<A, B, C> f,
        IO<RT, Err, A> ma)
        where RT : HasIO<RT, Err> =>
        IO<RT, Err, Func<A, B, C>>.Pure(f).Apply(ma);

    /// <summary>
    /// Applicative apply: takes the function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static IO<RT, Err, D> apply<RT, Err, A, B, C, D>(
        Func<A, B, C, D> f,
        IO<RT, Err, A> ma,
        IO<RT, Err, B> mb,
        IO<RT, Err, C> mc)
        where RT : HasIO<RT, Err> =>
        IO<RT, Err, Func<A, B, C, D>>.Pure(f).Apply(ma, mb, mc);

    /// <summary>
    /// Applicative apply: takes the function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static IO<RT, Err, Func<C, D>> apply<RT, Err, A, B, C, D>(
        Func<A, B, C, D> f,
        IO<RT, Err, A> ma,
        IO<RT, Err, B> mb)
        where RT : HasIO<RT, Err> =>
        IO<RT, Err, Func<A, B, C, D>>.Pure(f).Apply(ma, mb);

    /// <summary>
    /// Applicative apply: takes the function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static IO<RT, Err, Func<B, Func<C, D>>> apply<RT, Err, A, B, C, D>(
        Func<A, B, C, D> f,
        IO<RT, Err, A> ma)
        where RT : HasIO<RT, Err> =>
        IO<RT, Err, Func<A, B, C, D>>.Pure(f).Apply(ma);

    /// <summary>
    /// Applicative apply: takes the function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static IO<RT, Err, E> apply<RT, Err, A, B, C, D, E>(
        Func<A, B, C, D, E> f,
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
    public static IO<RT, Err, Func<D, E>> apply<RT, Err, A, B, C, D, E>(
        Func<A, B, C, D, E> f,
        IO<RT, Err, A> ma,
        IO<RT, Err, B> mb,
        IO<RT, Err, C> mc)
        where RT : HasIO<RT, Err> =>
        IO<RT, Err, Func<A, B, C, D, E>>.Pure(f).Apply(ma, mb, mc);

    /// <summary>
    /// Applicative apply: takes the function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static IO<RT, Err, Func<C, Func<D, E>>> apply<RT, Err, A, B, C, D, E>(
        Func<A, B, C, D, E> f,
        IO<RT, Err, A> ma,
        IO<RT, Err, B> mb)
        where RT : HasIO<RT, Err> =>
        IO<RT, Err, Func<A, B, C, D, E>>.Pure(f).Apply(ma, mb);

    /// <summary>
    /// Applicative apply: takes the function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static IO<RT, Err, Func<B, Func<C, Func<D, E>>>> apply<RT, Err, A, B, C, D, E>(
        Func<A, B, C, D, E> f,
        IO<RT, Err, A> ma)
        where RT : HasIO<RT, Err> =>
        IO<RT, Err, Func<A, B, C, D, E>>.Pure(f).Apply(ma);
}    
