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
    public static Eff<B> Apply<A, B>(
        this Eff<Func<A, B>> mf,
        Eff<A> ma) =>
        new(mf.Morphism.Apply(ma.Morphism));

    /// <summary>
    /// Applicative apply: takes the lifted function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static Eff<C> Apply<A, B, C>(
        this Eff<Func<A, B, C>> mf,
        Eff<A> ma,
        Eff<B> mb) =>
        mf.Map(curry).Apply(ma).Apply(mb);

    /// <summary>
    /// Applicative apply: takes the lifted function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static Eff<Func<B, C>> Apply<A, B, C>(
        this Eff<Func<A, B, C>> mf,
        Eff<A> ma) =>
        mf.Map(curry).Apply(ma);

    /// <summary>
    /// Applicative apply: takes the lifted function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static Eff<D> Apply<A, B, C, D>(
        this Eff<Func<A, B, C, D>> mf,
        Eff<A> ma,
        Eff<B> mb,
        Eff<C> mc) =>
        mf.Map(curry).Apply(ma).Apply(mb).Apply(mc);

    /// <summary>
    /// Applicative apply: takes the lifted function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static Eff<Func<C, D>> Apply<A, B, C, D>(
        this Eff<Func<A, B, C, D>> mf,
        Eff<A> ma,
        Eff<B> mb) =>
        mf.Map(curry).Apply(ma).Apply(mb);

    /// <summary>
    /// Applicative apply: takes the lifted function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static Eff<Func<B, Func<C, D>>> Apply<A, B, C, D>(
        this Eff<Func<A, B, C, D>> mf,
        Eff<A> ma) =>
        mf.Map(curry).Apply(ma);

    /// <summary>
    /// Applicative apply: takes the lifted function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static Eff<E> Apply<A, B, C, D, E>(
        this Eff<Func<A, B, C, D, E>> mf,
        Eff<A> ma,
        Eff<B> mb,
        Eff<C> mc,
        Eff<D> md) =>
        mf.Map(curry).Apply(ma).Apply(mb).Apply(mc).Apply(md);

    /// <summary>
    /// Applicative apply: takes the lifted function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static Eff<Func<D, E>> Apply<A, B, C, D, E>(
        this Eff<Func<A, B, C, D, E>> mf,
        Eff<A> ma,
        Eff<B> mb,
        Eff<C> mc) =>
        mf.Map(curry).Apply(ma).Apply(mb).Apply(mc);

    /// <summary>
    /// Applicative apply: takes the lifted function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static Eff<Func<C, Func<D, E>>> Apply<A, B, C, D, E>(
        this Eff<Func<A, B, C, D, E>> mf,
        Eff<A> ma,
        Eff<B> mb) =>
        mf.Map(curry).Apply(ma).Apply(mb);

    /// <summary>
    /// Applicative apply: takes the lifted function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static Eff<Func<B, Func<C, Func<D, E>>>> Apply<A, B, C, D, E>(
        this Eff<Func<A, B, C, D, E>> mf,
        Eff<A> ma) =>
        mf.Map(curry).Apply(ma);

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Non-lifted functions
    //

    /// <summary>
    /// Applicative apply: takes the function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static Eff<B> Apply<A, B>(
        this Func<A, B> f,
        Eff<A> ma) =>
        Eff<Func<A, B>>.Pure(f).Apply(ma);

    /// <summary>
    /// Applicative apply: takes the function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static Eff<C> Apply<A, B, C>(
        this Func<A, B, C> f,
        Eff<A> ma,
        Eff<B> mb) =>
        Eff<Func<A, B, C>>.Pure(f).Apply(ma, mb);

    /// <summary>
    /// Applicative apply: takes the function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static Eff<Func<B, C>> Apply<A, B, C>(
        this Func<A, B, C> f,
        Eff<A> ma) =>
        Eff<Func<A, B, C>>.Pure(f).Apply(ma);

    /// <summary>
    /// Applicative apply: takes the function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static Eff<D> Apply<A, B, C, D>(
        this Func<A, B, C, D> f,
        Eff<A> ma,
        Eff<B> mb,
        Eff<C> mc) =>
        Eff<Func<A, B, C, D>>.Pure(f).Apply(ma, mb, mc);

    /// <summary>
    /// Applicative apply: takes the function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static Eff<Func<C, D>> Apply<A, B, C, D>(
        this Func<A, B, C, D> f,
        Eff<A> ma,
        Eff<B> mb) =>
        Eff<Func<A, B, C, D>>.Pure(f).Apply(ma, mb);

    /// <summary>
    /// Applicative apply: takes the function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static Eff<Func<B, Func<C, D>>> Apply<A, B, C, D>(
        this Func<A, B, C, D> f,
        Eff<A> ma) =>
        Eff<Func<A, B, C, D>>.Pure(f).Apply(ma);

    /// <summary>
    /// Applicative apply: takes the function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static Eff<E> Apply<A, B, C, D, E>(
        this Func<A, B, C, D, E> f,
        Eff<A> ma,
        Eff<B> mb,
        Eff<C> mc,
        Eff<D> md) =>
        Eff<Func<A, B, C, D, E>>.Pure(f).Apply(ma, mb, mc, md);

    /// <summary>
    /// Applicative apply: takes the function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static Eff<Func<D, E>> Apply<A, B, C, D, E>(
        this Func<A, B, C, D, E> f,
        Eff<A> ma,
        Eff<B> mb,
        Eff<C> mc) =>
        Eff<Func<A, B, C, D, E>>.Pure(f).Apply(ma, mb, mc);

    /// <summary>
    /// Applicative apply: takes the function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static Eff<Func<C, Func<D, E>>> Apply<A, B, C, D, E>(
        this Func<A, B, C, D, E> f,
        Eff<A> ma,
        Eff<B> mb) =>
        Eff<Func<A, B, C, D, E>>.Pure(f).Apply(ma, mb);

    /// <summary>
    /// Applicative apply: takes the function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static Eff<Func<B, Func<C, Func<D, E>>>> Apply<A, B, C, D, E>(
        this Func<A, B, C, D, E> f,
        Eff<A> ma) =>
        Eff<Func<A, B, C, D, E>>.Pure(f).Apply(ma);
}    
