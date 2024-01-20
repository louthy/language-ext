#nullable enable
using System;
using LanguageExt.Common;
using LanguageExt.Effects.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static partial class EffExtensions
{
    /// <summary>
    /// Applicative apply: takes the lifted function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static Eff<RT, B> Apply<RT, A, B>(
        this Eff<RT, Func<A, B>> mf,
        Eff<RT, A> ma)
        where RT : struct, HasIO<RT, Error> =>
        new(mf.Morphism.Apply(ma.Morphism));

    /// <summary>
    /// Applicative apply: takes the lifted function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static Eff<RT, C> Apply<RT, A, B, C>(
        this Eff<RT, Func<A, B, C>> mf,
        Eff<RT, A> ma,
        Eff<RT, B> mb)
        where RT : struct, HasIO<RT, Error> =>
        mf.Map(curry).Apply(ma).Apply(mb);

    /// <summary>
    /// Applicative apply: takes the lifted function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static Eff<RT, Func<B, C>> Apply<RT, A, B, C>(
        this Eff<RT, Func<A, B, C>> mf,
        Eff<RT, A> ma)
        where RT : struct, HasIO<RT, Error> =>
        mf.Map(curry).Apply(ma);

    /// <summary>
    /// Applicative apply: takes the lifted function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static Eff<RT, D> Apply<RT, A, B, C, D>(
        this Eff<RT, Func<A, B, C, D>> mf,
        Eff<RT, A> ma,
        Eff<RT, B> mb,
        Eff<RT, C> mc)
        where RT : struct, HasIO<RT, Error> =>
        mf.Map(curry).Apply(ma).Apply(mb).Apply(mc);

    /// <summary>
    /// Applicative apply: takes the lifted function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static Eff<RT, Func<C, D>> Apply<RT, A, B, C, D>(
        this Eff<RT, Func<A, B, C, D>> mf,
        Eff<RT, A> ma,
        Eff<RT, B> mb)
        where RT : struct, HasIO<RT, Error> =>
        mf.Map(curry).Apply(ma).Apply(mb);

    /// <summary>
    /// Applicative apply: takes the lifted function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static Eff<RT, Func<B, Func<C, D>>> Apply<RT, A, B, C, D>(
        this Eff<RT, Func<A, B, C, D>> mf,
        Eff<RT, A> ma)
        where RT : struct, HasIO<RT, Error> =>
        mf.Map(curry).Apply(ma);

    /// <summary>
    /// Applicative apply: takes the lifted function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static Eff<RT, E> Apply<RT, A, B, C, D, E>(
        this Eff<RT, Func<A, B, C, D, E>> mf,
        Eff<RT, A> ma,
        Eff<RT, B> mb,
        Eff<RT, C> mc,
        Eff<RT, D> md)
        where RT : struct, HasIO<RT, Error> =>
        mf.Map(curry).Apply(ma).Apply(mb).Apply(mc).Apply(md);

    /// <summary>
    /// Applicative apply: takes the lifted function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static Eff<RT, Func<D, E>> Apply<RT, A, B, C, D, E>(
        this Eff<RT, Func<A, B, C, D, E>> mf,
        Eff<RT, A> ma,
        Eff<RT, B> mb,
        Eff<RT, C> mc)
        where RT : struct, HasIO<RT, Error> =>
        mf.Map(curry).Apply(ma).Apply(mb).Apply(mc);

    /// <summary>
    /// Applicative apply: takes the lifted function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static Eff<RT, Func<C, Func<D, E>>> Apply<RT, A, B, C, D, E>(
        this Eff<RT, Func<A, B, C, D, E>> mf,
        Eff<RT, A> ma,
        Eff<RT, B> mb)
        where RT : struct, HasIO<RT, Error> =>
        mf.Map(curry).Apply(ma).Apply(mb);

    /// <summary>
    /// Applicative apply: takes the lifted function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static Eff<RT, Func<B, Func<C, Func<D, E>>>> Apply<RT, A, B, C, D, E>(
        this Eff<RT, Func<A, B, C, D, E>> mf,
        Eff<RT, A> ma)
        where RT : struct, HasIO<RT, Error> =>
        mf.Map(curry).Apply(ma);

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Non-lifted functions
    //

    /// <summary>
    /// Applicative apply: takes the function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static Eff<RT, B> Apply<RT, A, B>(
        this Func<A, B> f,
        Eff<RT, A> ma)
        where RT : struct, HasIO<RT, Error> =>
        Eff<RT, Func<A, B>>.Pure(f).Apply(ma);

    /// <summary>
    /// Applicative apply: takes the function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static Eff<RT, C> Apply<RT, A, B, C>(
        this Func<A, B, C> f,
        Eff<RT, A> ma,
        Eff<RT, B> mb)
        where RT : struct, HasIO<RT, Error> =>
        Eff<RT, Func<A, B, C>>.Pure(f).Apply(ma, mb);

    /// <summary>
    /// Applicative apply: takes the function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static Eff<RT, Func<B, C>> Apply<RT, A, B, C>(
        this Func<A, B, C> f,
        Eff<RT, A> ma)
        where RT : struct, HasIO<RT, Error> =>
        Eff<RT, Func<A, B, C>>.Pure(f).Apply(ma);

    /// <summary>
    /// Applicative apply: takes the function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static Eff<RT, D> Apply<RT, A, B, C, D>(
        this Func<A, B, C, D> f,
        Eff<RT, A> ma,
        Eff<RT, B> mb,
        Eff<RT, C> mc)
        where RT : struct, HasIO<RT, Error> =>
        Eff<RT, Func<A, B, C, D>>.Pure(f).Apply(ma, mb, mc);

    /// <summary>
    /// Applicative apply: takes the function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static Eff<RT, Func<C, D>> Apply<RT, A, B, C, D>(
        this Func<A, B, C, D> f,
        Eff<RT, A> ma,
        Eff<RT, B> mb)
        where RT : struct, HasIO<RT, Error> =>
        Eff<RT, Func<A, B, C, D>>.Pure(f).Apply(ma, mb);

    /// <summary>
    /// Applicative apply: takes the function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static Eff<RT, Func<B, Func<C, D>>> Apply<RT, A, B, C, D>(
        this Func<A, B, C, D> f,
        Eff<RT, A> ma)
        where RT : struct, HasIO<RT, Error> =>
        Eff<RT, Func<A, B, C, D>>.Pure(f).Apply(ma);

    /// <summary>
    /// Applicative apply: takes the function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static Eff<RT, E> Apply<RT, A, B, C, D, E>(
        this Func<A, B, C, D, E> f,
        Eff<RT, A> ma,
        Eff<RT, B> mb,
        Eff<RT, C> mc,
        Eff<RT, D> md)
        where RT : struct, HasIO<RT, Error> =>
        Eff<RT, Func<A, B, C, D, E>>.Pure(f).Apply(ma, mb, mc, md);

    /// <summary>
    /// Applicative apply: takes the function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static Eff<RT, Func<D, E>> Apply<RT, A, B, C, D, E>(
        this Func<A, B, C, D, E> f,
        Eff<RT, A> ma,
        Eff<RT, B> mb,
        Eff<RT, C> mc)
        where RT : struct, HasIO<RT, Error> =>
        Eff<RT, Func<A, B, C, D, E>>.Pure(f).Apply(ma, mb, mc);

    /// <summary>
    /// Applicative apply: takes the function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static Eff<RT, Func<C, Func<D, E>>> Apply<RT, A, B, C, D, E>(
        this Func<A, B, C, D, E> f,
        Eff<RT, A> ma,
        Eff<RT, B> mb)
        where RT : struct, HasIO<RT, Error> =>
        Eff<RT, Func<A, B, C, D, E>>.Pure(f).Apply(ma, mb);

    /// <summary>
    /// Applicative apply: takes the function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static Eff<RT, Func<B, Func<C, Func<D, E>>>> Apply<RT, A, B, C, D, E>(
        this Func<A, B, C, D, E> f,
        Eff<RT, A> ma)
        where RT : struct, HasIO<RT, Error> =>
        Eff<RT, Func<A, B, C, D, E>>.Pure(f).Apply(ma);
}    
