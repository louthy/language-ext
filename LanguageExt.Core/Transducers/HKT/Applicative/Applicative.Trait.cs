using System;

namespace LanguageExt.HKT;



/// <summary>
/// Functor 
/// </summary>
/// <typeparam name="F">Functor trait type</typeparam>
/// <typeparam name="A">Bound value type</typeparam>
public interface Applicative<F> : Functor<F>
    where F : Applicative<F>
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Abstract members
    //
    
    public static abstract K<F, A> Pure<A>(A value);

    public static abstract K<F, B> Apply<A, B>(K<F, Func<A, B>> mf, K<F, A> ma);
    
    public static abstract K<F, B> Action<A, B>(K<F, A> ma, K<F, B> mb);
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Default implementations
    //

    public static virtual K<F, C> Apply<A, B, C>(K<F, Func<A, B, C>> mf, K<F, A> ma, K<F, B> mb) =>
        F.Apply(F.Apply(F.Map(Prelude.curry, mf), ma), mb);

    public static virtual K<F, Func<B, C>> Apply<A, B, C>(K<F, Func<A, B, C>> mf, K<F, A> ma) =>
        F.Apply(F.Map(Prelude.curry, mf), ma);

    public static virtual K<F, C> Apply<A, B, C>(K<F, Func<A, Func<B, C>>> mf, K<F, A> ma, K<F, B> mb) =>
        F.Apply(F.Apply(mf, ma), mb);
}
