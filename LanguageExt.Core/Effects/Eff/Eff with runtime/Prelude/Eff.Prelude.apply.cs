using System;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class Prelude
{
    /// <summary>
    /// Applicative apply: takes the lifted function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static Eff<RT, B> apply<RT, A, B>(
        K<Eff<RT>, Func<A, B>> mf,
        K<Eff<RT>, A> ma) =>
        mf.Apply(ma).As();

    /// <summary>
    /// Applicative apply: takes the lifted function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static Eff<RT, C> apply<RT, A, B, C>(
        K<Eff<RT>, Func<A, B, C>> mf,
        K<Eff<RT>, A> ma,
        K<Eff<RT>, B> mb) =>
        mf.Map(curry).Apply(ma).Apply(mb).As();

    /// <summary>
    /// Applicative apply: takes the lifted function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static Eff<RT, Func<B, C>> apply<RT, A, B, C>(
        K<Eff<RT>, Func<A, B, C>> mf,
        K<Eff<RT>, A> ma) =>
        mf.Map(curry).Apply(ma).As();

    /// <summary>
    /// Applicative apply: takes the lifted function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static Eff<RT, D> apply<RT, A, B, C, D>(
        K<Eff<RT>, Func<A, B, C, D>> mf,
        K<Eff<RT>, A> ma,
        K<Eff<RT>, B> mb,
        K<Eff<RT>, C> mc) =>
        mf.Map(curry).Apply(ma).Apply(mb).Apply(mc).As();

    /// <summary>
    /// Applicative apply: takes the lifted function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static Eff<RT, Func<C, D>> apply<RT, A, B, C, D>(
        K<Eff<RT>, Func<A, B, C, D>> mf,
        K<Eff<RT>, A> ma,
        K<Eff<RT>, B> mb) =>
        mf.Map(curry).Apply(ma).Apply(mb).As();

    /// <summary>
    /// Applicative apply: takes the lifted function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static Eff<RT, Func<B, Func<C, D>>> apply<RT, A, B, C, D>(
        K<Eff<RT>, Func<A, B, C, D>> mf,
        K<Eff<RT>, A> ma) =>
        mf.Map(curry).Apply(ma).As();

    /// <summary>
    /// Applicative apply: takes the lifted function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static Eff<RT, E> apply<RT, A, B, C, D, E>(
        K<Eff<RT>, Func<A, B, C, D, E>> mf,
        K<Eff<RT>, A> ma,
        K<Eff<RT>, B> mb,
        K<Eff<RT>, C> mc,
        K<Eff<RT>, D> md) =>
        mf.Map(curry).Apply(ma).Apply(mb).Apply(mc).Apply(md).As();

    /// <summary>
    /// Applicative apply: takes the lifted function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static Eff<RT, Func<D, E>> apply<RT, A, B, C, D, E>(
        K<Eff<RT>, Func<A, B, C, D, E>> mf,
        K<Eff<RT>, A> ma,
        K<Eff<RT>, B> mb,
        K<Eff<RT>, C> mc) =>
        mf.Map(curry).Apply(ma).Apply(mb).Apply(mc).As();

    /// <summary>
    /// Applicative apply: takes the lifted function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static Eff<RT, Func<C, Func<D, E>>> apply<RT, A, B, C, D, E>(
        K<Eff<RT>, Func<A, B, C, D, E>> mf,
        K<Eff<RT>, A> ma,
        K<Eff<RT>, B> mb) =>
        mf.Map(curry).Apply(ma).Apply(mb).As();

    /// <summary>
    /// Applicative apply: takes the lifted function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static Eff<RT, Func<B, Func<C, Func<D, E>>>> apply<RT, A, B, C, D, E>(
        K<Eff<RT>, Func<A, B, C, D, E>> mf,
        K<Eff<RT>, A> ma) =>
        mf.Map(curry).Apply(ma).As();
}    
