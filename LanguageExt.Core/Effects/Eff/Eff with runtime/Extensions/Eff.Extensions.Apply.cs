using System;
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
        Eff<RT, A> ma) =>
        from tf in mf.Fork()
        from ta in ma.Fork()
        from rf in tf.Await
        from ra in ta.Await
        select rf(ra);
    
    /// <summary>
    /// Applicative apply: takes the lifted function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static Eff<RT, C> Apply<RT, A, B, C>(
        this Eff<RT, Func<A, B, C>> mf,
        Eff<RT, A> ma,
        Eff<RT, B> mb) =>
        mf.Map(curry).Apply(ma).Apply(mb);

    /// <summary>
    /// Applicative apply: takes the lifted function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static Eff<RT, Func<B, C>> Apply<RT, A, B, C>(
        this Eff<RT, Func<A, B, C>> mf,
        Eff<RT, A> ma)  =>
        mf.Map(curry).Apply(ma);

    /// <summary>
    /// Applicative apply: takes the lifted function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static Eff<RT, D> Apply<RT, A, B, C, D>(
        this Eff<RT, Func<A, B, C, D>> mf,
        Eff<RT, A> ma,
        Eff<RT, B> mb,
        Eff<RT, C> mc) =>
        mf.Map(curry).Apply(ma).Apply(mb).Apply(mc);

    /// <summary>
    /// Applicative apply: takes the lifted function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static Eff<RT, Func<C, D>> Apply<RT, A, B, C, D>(
        this Eff<RT, Func<A, B, C, D>> mf,
        Eff<RT, A> ma,
        Eff<RT, B> mb) =>
        mf.Map(curry).Apply(ma).Apply(mb);

    /// <summary>
    /// Applicative apply: takes the lifted function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static Eff<RT, Func<B, Func<C, D>>> Apply<RT, A, B, C, D>(
        this Eff<RT, Func<A, B, C, D>> mf,
        Eff<RT, A> ma) =>
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
        Eff<RT, D> md) =>
        mf.Map(curry).Apply(ma).Apply(mb).Apply(mc).Apply(md);

    /// <summary>
    /// Applicative apply: takes the lifted function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static Eff<RT, Func<D, E>> Apply<RT, A, B, C, D, E>(
        this Eff<RT, Func<A, B, C, D, E>> mf,
        Eff<RT, A> ma,
        Eff<RT, B> mb,
        Eff<RT, C> mc) =>
        mf.Map(curry).Apply(ma).Apply(mb).Apply(mc);

    /// <summary>
    /// Applicative apply: takes the lifted function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static Eff<RT, Func<C, Func<D, E>>> Apply<RT, A, B, C, D, E>(
        this Eff<RT, Func<A, B, C, D, E>> mf,
        Eff<RT, A> ma,
        Eff<RT, B> mb) =>
        mf.Map(curry).Apply(ma).Apply(mb);

    /// <summary>
    /// Applicative apply: takes the lifted function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static Eff<RT, Func<B, Func<C, Func<D, E>>>> Apply<RT, A, B, C, D, E>(
        this Eff<RT, Func<A, B, C, D, E>> mf,
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
    public static Eff<RT, B> Apply<RT, A, B>(
        this Func<A, B> f,
        Eff<RT, A> ma) =>
        Eff<RT, Func<A, B>>.Pure(f).Apply(ma);

    /// <summary>
    /// Applicative apply: takes the function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static Eff<RT, C> Apply<RT, A, B, C>(
        this Func<A, B, C> f,
        Eff<RT, A> ma,
        Eff<RT, B> mb) =>
        Eff<RT, Func<A, B, C>>.Pure(f).Apply(ma, mb);

    /// <summary>
    /// Applicative apply: takes the function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static Eff<RT, Func<B, C>> Apply<RT, A, B, C>(
        this Func<A, B, C> f,
        Eff<RT, A> ma) =>
        Eff<RT, Func<A, B, C>>.Pure(f).Apply(ma);

    /// <summary>
    /// Applicative apply: takes the function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static Eff<RT, D> Apply<RT, A, B, C, D>(
        this Func<A, B, C, D> f,
        Eff<RT, A> ma,
        Eff<RT, B> mb,
        Eff<RT, C> mc) =>
        Eff<RT, Func<A, B, C, D>>.Pure(f).Apply(ma, mb, mc);

    /// <summary>
    /// Applicative apply: takes the function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static Eff<RT, Func<C, D>> Apply<RT, A, B, C, D>(
        this Func<A, B, C, D> f,
        Eff<RT, A> ma,
        Eff<RT, B> mb) =>
        Eff<RT, Func<A, B, C, D>>.Pure(f).Apply(ma, mb);

    /// <summary>
    /// Applicative apply: takes the function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static Eff<RT, Func<B, Func<C, D>>> Apply<RT, A, B, C, D>(
        this Func<A, B, C, D> f,
        Eff<RT, A> ma) =>
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
        Eff<RT, D> md) =>
        Eff<RT, Func<A, B, C, D, E>>.Pure(f).Apply(ma, mb, mc, md);

    /// <summary>
    /// Applicative apply: takes the function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static Eff<RT, Func<D, E>> Apply<RT, A, B, C, D, E>(
        this Func<A, B, C, D, E> f,
        Eff<RT, A> ma,
        Eff<RT, B> mb,
        Eff<RT, C> mc) =>
        Eff<RT, Func<A, B, C, D, E>>.Pure(f).Apply(ma, mb, mc);

    /// <summary>
    /// Applicative apply: takes the function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static Eff<RT, Func<C, Func<D, E>>> Apply<RT, A, B, C, D, E>(
        this Func<A, B, C, D, E> f,
        Eff<RT, A> ma,
        Eff<RT, B> mb) =>
        Eff<RT, Func<A, B, C, D, E>>.Pure(f).Apply(ma, mb);

    /// <summary>
    /// Applicative apply: takes the function and the lifted argument, applies the function to the argument
    /// and returns the result, lifted.
    /// </summary>
    public static Eff<RT, Func<B, Func<C, Func<D, E>>>> Apply<RT, A, B, C, D, E>(
        this Func<A, B, C, D, E> f,
        Eff<RT, A> ma) =>
        Eff<RT, Func<A, B, C, D, E>>.Pure(f).Apply(ma);
        
    /// <summary>
    /// Applicative action: runs the first applicative, ignores the result, and returns the second applicative
    /// </summary>
    public static Eff<RT, B> Action<RT, A, B>(
        this Eff<RT, A> ma,
        Eff<RT, B> mb) =>
        from a in ma.Fork()
        from b in mb
        select b;
}    
