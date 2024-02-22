using System;

namespace LanguageExt;

public static partial class Prelude
{
    /// <summary>
    /// Applicative apply: takes the lifted function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static Eff<RT, B> apply<RT, A, B>(
        Eff<RT, Func<A, B>> mf,
        Eff<RT, A> ma) =>
        mf.Apply(ma);

    /// <summary>
    /// Applicative apply: takes the lifted function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static Eff<RT, C> apply<RT, A, B, C>(
        Eff<RT, Func<A, B, C>> mf,
        Eff<RT, A> ma,
        Eff<RT, B> mb) =>
        mf.Map(curry).Apply(ma).Apply(mb);

    /// <summary>
    /// Applicative apply: takes the lifted function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static Eff<RT, Func<B, C>> apply<RT, A, B, C>(
        Eff<RT, Func<A, B, C>> mf,
        Eff<RT, A> ma) =>
        mf.Map(curry).Apply(ma);

    /// <summary>
    /// Applicative apply: takes the lifted function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static Eff<RT, D> apply<RT, A, B, C, D>(
        Eff<RT, Func<A, B, C, D>> mf,
        Eff<RT, A> ma,
        Eff<RT, B> mb,
        Eff<RT, C> mc) =>
        mf.Map(curry).Apply(ma).Apply(mb).Apply(mc);

    /// <summary>
    /// Applicative apply: takes the lifted function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static Eff<RT, Func<C, D>> apply<RT, A, B, C, D>(
        Eff<RT, Func<A, B, C, D>> mf,
        Eff<RT, A> ma,
        Eff<RT, B> mb) =>
        mf.Map(curry).Apply(ma).Apply(mb);

    /// <summary>
    /// Applicative apply: takes the lifted function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static Eff<RT, Func<B, Func<C, D>>> apply<RT, A, B, C, D>(
        Eff<RT, Func<A, B, C, D>> mf,
        Eff<RT, A> ma) =>
        mf.Map(curry).Apply(ma);

    /// <summary>
    /// Applicative apply: takes the lifted function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static Eff<RT, E> apply<RT, A, B, C, D, E>(
        Eff<RT, Func<A, B, C, D, E>> mf,
        Eff<RT, A> ma,
        Eff<RT, B> mb,
        Eff<RT, C> mc,
        Eff<RT, D> md) =>
        mf.Map(curry).Apply(ma).Apply(mb).Apply(mc).Apply(md);

    /// <summary>
    /// Applicative apply: takes the lifted function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static Eff<RT, Func<D, E>> apply<RT, A, B, C, D, E>(
        Eff<RT, Func<A, B, C, D, E>> mf,
        Eff<RT, A> ma,
        Eff<RT, B> mb,
        Eff<RT, C> mc) =>
        mf.Map(curry).Apply(ma).Apply(mb).Apply(mc);

    /// <summary>
    /// Applicative apply: takes the lifted function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static Eff<RT, Func<C, Func<D, E>>> apply<RT, A, B, C, D, E>(
        Eff<RT, Func<A, B, C, D, E>> mf,
        Eff<RT, A> ma,
        Eff<RT, B> mb) =>
        mf.Map(curry).Apply(ma).Apply(mb);

    /// <summary>
    /// Applicative apply: takes the lifted function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static Eff<RT, Func<B, Func<C, Func<D, E>>>> apply<RT, A, B, C, D, E>(
        Eff<RT, Func<A, B, C, D, E>> mf,
        Eff<RT, A> ma) =>
        mf.Map(curry).Apply(ma);

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Non-lifted functions
    //

    /// <summary>
    /// Applicative apply: takes the function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static Eff<RT, B> apply<RT, A, B>(
        Func<A, B> f,
        Eff<RT, A> ma) =>
        LanguageExt.Eff<RT, Func<A, B>>.Pure(f).Apply(ma);

    /// <summary>
    /// Applicative apply: takes the function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static Eff<RT, C> apply<RT, A, B, C>(
        Func<A, B, C> f,
        Eff<RT, A> ma,
        Eff<RT, B> mb) =>
        LanguageExt.Eff<RT, Func<A, B, C>>.Pure(f).Apply(ma, mb);

    /// <summary>
    /// Applicative apply: takes the function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static Eff<RT, Func<B, C>> apply<RT, A, B, C>(
        Func<A, B, C> f,
        Eff<RT, A> ma) =>
        LanguageExt.Eff<RT, Func<A, B, C>>.Pure(f).Apply(ma);

    /// <summary>
    /// Applicative apply: takes the function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static Eff<RT, D> apply<RT, A, B, C, D>(
        Func<A, B, C, D> f,
        Eff<RT, A> ma,
        Eff<RT, B> mb,
        Eff<RT, C> mc) =>
        LanguageExt.Eff<RT, Func<A, B, C, D>>.Pure(f).Apply(ma, mb, mc);

    /// <summary>
    /// Applicative apply: takes the function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static Eff<RT, Func<C, D>> apply<RT, A, B, C, D>(
        Func<A, B, C, D> f,
        Eff<RT, A> ma,
        Eff<RT, B> mb) =>
        LanguageExt.Eff<RT, Func<A, B, C, D>>.Pure(f).Apply(ma, mb);

    /// <summary>
    /// Applicative apply: takes the function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static Eff<RT, Func<B, Func<C, D>>> apply<RT, A, B, C, D>(
        Func<A, B, C, D> f,
        Eff<RT, A> ma) =>
        LanguageExt.Eff<RT, Func<A, B, C, D>>.Pure(f).Apply(ma);

    /// <summary>
    /// Applicative apply: takes the function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static Eff<RT, E> apply<RT, A, B, C, D, E>(
        Func<A, B, C, D, E> f,
        Eff<RT, A> ma,
        Eff<RT, B> mb,
        Eff<RT, C> mc,
        Eff<RT, D> md) =>
        LanguageExt.Eff<RT, Func<A, B, C, D, E>>.Pure(f).Apply(ma, mb, mc, md);

    /// <summary>
    /// Applicative apply: takes the function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static Eff<RT, Func<D, E>> apply<RT, A, B, C, D, E>(
        Func<A, B, C, D, E> f,
        Eff<RT, A> ma,
        Eff<RT, B> mb,
        Eff<RT, C> mc) =>
        LanguageExt.Eff<RT, Func<A, B, C, D, E>>.Pure(f).Apply(ma, mb, mc);

    /// <summary>
    /// Applicative apply: takes the function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static Eff<RT, Func<C, Func<D, E>>> apply<RT, A, B, C, D, E>(
        Func<A, B, C, D, E> f,
        Eff<RT, A> ma,
        Eff<RT, B> mb) =>
        LanguageExt.Eff<RT, Func<A, B, C, D, E>>.Pure(f).Apply(ma, mb);

    /// <summary>
    /// Applicative apply: takes the function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static Eff<RT, Func<B, Func<C, Func<D, E>>>> apply<RT, A, B, C, D, E>(
        Func<A, B, C, D, E> f,
        Eff<RT, A> ma) =>
        LanguageExt.Eff<RT, Func<A, B, C, D, E>>.Pure(f).Apply(ma);
}    
