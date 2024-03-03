using System;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static partial class EffExtensions
{
    /// <summary>
    /// Applicative apply: takes the lifted function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static Eff<RT, B> Apply<RT, A, B>(
        this K<Eff<RT>, Func<A, B>> mf,
        K<Eff<RT>, A> ma) =>
        mf.Bind(ma.Map).As();
    
    /// <summary>
    /// Applicative apply: takes the lifted function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static Eff<RT, C> Apply<RT, A, B, C>(
        this K<Eff<RT>, Func<A, B, C>> mf,
        K<Eff<RT>, A> ma,
        K<Eff<RT>, B> mb) =>
        mf.Map(curry).Apply(ma).Apply(mb);

    /// <summary>
    /// Applicative apply: takes the lifted function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static Eff<RT, Func<B, C>> Apply<RT, A, B, C>(
        this K<Eff<RT>, Func<A, B, C>> mf,
        K<Eff<RT>, A> ma)  =>
        mf.Map(curry).Apply(ma);

    /// <summary>
    /// Applicative apply: takes the lifted function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static Eff<RT, D> Apply<RT, A, B, C, D>(
        this K<Eff<RT>, Func<A, B, C, D>> mf,
        K<Eff<RT>, A> ma,
        K<Eff<RT>, B> mb,
        K<Eff<RT>, C> mc) =>
        mf.Map(curry).Apply(ma).Apply(mb).Apply(mc);

    /// <summary>
    /// Applicative apply: takes the lifted function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static Eff<RT, Func<C, D>> Apply<RT, A, B, C, D>(
        this K<Eff<RT>, Func<A, B, C, D>> mf,
        K<Eff<RT>, A> ma,
        K<Eff<RT>, B> mb) =>
        mf.Map(curry).Apply(ma).Apply(mb);

    /// <summary>
    /// Applicative apply: takes the lifted function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static Eff<RT, Func<B, Func<C, D>>> Apply<RT, A, B, C, D>(
        this K<Eff<RT>, Func<A, B, C, D>> mf,
        K<Eff<RT>, A> ma) =>
        mf.Map(curry).Apply(ma);

    /// <summary>
    /// Applicative apply: takes the lifted function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static Eff<RT, E> Apply<RT, A, B, C, D, E>(
        this K<Eff<RT>, Func<A, B, C, D, E>> mf,
        K<Eff<RT>, A> ma,
        K<Eff<RT>, B> mb,
        K<Eff<RT>, C> mc,
        K<Eff<RT>, D> md) =>
        mf.Map(curry).Apply(ma).Apply(mb).Apply(mc).Apply(md);

    /// <summary>
    /// Applicative apply: takes the lifted function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static Eff<RT, Func<D, E>> Apply<RT, A, B, C, D, E>(
        this K<Eff<RT>, Func<A, B, C, D, E>> mf,
        K<Eff<RT>, A> ma,
        K<Eff<RT>, B> mb,
        K<Eff<RT>, C> mc) =>
        mf.Map(curry).Apply(ma).Apply(mb).Apply(mc);

    /// <summary>
    /// Applicative apply: takes the lifted function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static Eff<RT, Func<C, Func<D, E>>> Apply<RT, A, B, C, D, E>(
        this K<Eff<RT>, Func<A, B, C, D, E>> mf,
        K<Eff<RT>, A> ma,
        K<Eff<RT>, B> mb) =>
        mf.Map(curry).Apply(ma).Apply(mb);

    /// <summary>
    /// Applicative apply: takes the lifted function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static Eff<RT, Func<B, Func<C, Func<D, E>>>> Apply<RT, A, B, C, D, E>(
        this K<Eff<RT>, Func<A, B, C, D, E>> mf,
        K<Eff<RT>, A> ma) =>
        mf.Map(curry).Apply(ma);
        
    /// <summary>
    /// Applicative action: runs the first applicative, ignores the result, and returns the second applicative
    /// </summary>
    public static Eff<RT, B> Action<RT, A, B>(
        this K<Eff<RT>, A> ma,
        K<Eff<RT>, B> mb) =>
        from a in ma.As().Fork()
        from b in mb
        select b;
}    
