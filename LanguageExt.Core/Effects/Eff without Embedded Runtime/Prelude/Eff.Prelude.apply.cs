#nullable enable
using System;
using LanguageExt.Common;
using LanguageExt.Effects.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static partial class Prelude
{
    /// <summary>
    /// Applicative apply: takes the lifted function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static Eff<B> apply <A, B>(
        Eff<Func<A, B>> mf,
        Eff<A> ma) =>
        new(mf.Morphism.Apply(ma.Morphism));

    /// <summary>
    /// Applicative apply: takes the lifted function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static Eff<C> apply <A, B, C>(
        Eff<Func<A, B, C>> mf,
        Eff<A> ma,
        Eff<B> mb) =>
        mf.Map(curry).Apply(ma).Apply(mb);

    /// <summary>
    /// Applicative apply: takes the lifted function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static Eff<Func<B, C>> apply <A, B, C>(
        Eff<Func<A, B, C>> mf,
        Eff<A> ma) =>
        mf.Map(curry).Apply(ma);

    /// <summary>
    /// Applicative apply: takes the lifted function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static Eff<D> apply <A, B, C, D>(
        Eff<Func<A, B, C, D>> mf,
        Eff<A> ma,
        Eff<B> mb,
        Eff<C> mc) =>
        mf.Map(curry).Apply(ma).Apply(mb).Apply(mc);

    /// <summary>
    /// Applicative apply: takes the lifted function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static Eff<Func<C, D>> apply <A, B, C, D>(
        Eff<Func<A, B, C, D>> mf,
        Eff<A> ma,
        Eff<B> mb) =>
        mf.Map(curry).Apply(ma).Apply(mb);

    /// <summary>
    /// Applicative apply: takes the lifted function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static Eff<Func<B, Func<C, D>>> apply <A, B, C, D>(
        Eff<Func<A, B, C, D>> mf,
        Eff<A> ma) =>
        mf.Map(curry).Apply(ma);

    /// <summary>
    /// Applicative apply: takes the lifted function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static Eff<E> apply <A, B, C, D, E>(
        Eff<Func<A, B, C, D, E>> mf,
        Eff<A> ma,
        Eff<B> mb,
        Eff<C> mc,
        Eff<D> md) =>
        mf.Map(curry).Apply(ma).Apply(mb).Apply(mc).Apply(md);

    /// <summary>
    /// Applicative apply: takes the lifted function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static Eff<Func<D, E>> apply <A, B, C, D, E>(
        Eff<Func<A, B, C, D, E>> mf,
        Eff<A> ma,
        Eff<B> mb,
        Eff<C> mc) =>
        mf.Map(curry).Apply(ma).Apply(mb).Apply(mc);

    /// <summary>
    /// Applicative apply: takes the lifted function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static Eff<Func<C, Func<D, E>>> apply <A, B, C, D, E>(
        Eff<Func<A, B, C, D, E>> mf,
        Eff<A> ma,
        Eff<B> mb) =>
        mf.Map(curry).Apply(ma).Apply(mb);

    /// <summary>
    /// Applicative apply: takes the lifted function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static Eff<Func<B, Func<C, Func<D, E>>>> apply <A, B, C, D, E>(
        Eff<Func<A, B, C, D, E>> mf,
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
    public static Eff<B> apply <A, B>(
        Func<A, B> f,
        Eff<A> ma) =>
        LanguageExt.Eff<Func<A, B>>.Pure(f).Apply(ma);

    /// <summary>
    /// Applicative apply: takes the function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static Eff<C> apply <A, B, C>(
        Func<A, B, C> f,
        Eff<A> ma,
        Eff<B> mb) =>
        LanguageExt.Eff<Func<A, B, C>>.Pure(f).Apply(ma, mb);

    /// <summary>
    /// Applicative apply: takes the function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static Eff<Func<B, C>> apply <A, B, C>(
        Func<A, B, C> f,
        Eff<A> ma) =>
        LanguageExt.Eff<Func<A, B, C>>.Pure(f).Apply(ma);

    /// <summary>
    /// Applicative apply: takes the function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static Eff<D> apply <A, B, C, D>(
        Func<A, B, C, D> f,
        Eff<A> ma,
        Eff<B> mb,
        Eff<C> mc) =>
        LanguageExt.Eff<Func<A, B, C, D>>.Pure(f).Apply(ma, mb, mc);

    /// <summary>
    /// Applicative apply: takes the function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static Eff<Func<C, D>> apply <A, B, C, D>(
        Func<A, B, C, D> f,
        Eff<A> ma,
        Eff<B> mb) =>
        LanguageExt.Eff<Func<A, B, C, D>>.Pure(f).Apply(ma, mb);

    /// <summary>
    /// Applicative apply: takes the function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static Eff<Func<B, Func<C, D>>> apply <A, B, C, D>(
        Func<A, B, C, D> f,
        Eff<A> ma) =>
        LanguageExt.Eff<Func<A, B, C, D>>.Pure(f).Apply(ma);

    /// <summary>
    /// Applicative apply: takes the function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static Eff<E> apply <A, B, C, D, E>(
        Func<A, B, C, D, E> f,
        Eff<A> ma,
        Eff<B> mb,
        Eff<C> mc,
        Eff<D> md) =>
        LanguageExt.Eff<Func<A, B, C, D, E>>.Pure(f).Apply(ma, mb, mc, md);

    /// <summary>
    /// Applicative apply: takes the function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static Eff<Func<D, E>> apply <A, B, C, D, E>(
        Func<A, B, C, D, E> f,
        Eff<A> ma,
        Eff<B> mb,
        Eff<C> mc) =>
        LanguageExt.Eff<Func<A, B, C, D, E>>.Pure(f).Apply(ma, mb, mc);

    /// <summary>
    /// Applicative apply: takes the function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static Eff<Func<C, Func<D, E>>> apply <A, B, C, D, E>(
        Func<A, B, C, D, E> f,
        Eff<A> ma,
        Eff<B> mb) =>
        LanguageExt.Eff<Func<A, B, C, D, E>>.Pure(f).Apply(ma, mb);

    /// <summary>
    /// Applicative apply: takes the function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static Eff<Func<B, Func<C, Func<D, E>>>> apply <A, B, C, D, E>(
        Func<A, B, C, D, E> f,
        Eff<A> ma) =>
        LanguageExt.Eff<Func<A, B, C, D, E>>.Pure(f).Apply(ma);
}    
