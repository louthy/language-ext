using System;
using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt;

public static class FailExtensions
{
    extension<A>(Fail<A> lhs)
    {
        /// <summary>
        /// Value extraction
        /// </summary>
        public static A operator >>(Fail<A> px, Lower _) =>
            px.Value;
    }

    extension<A, B>(Fail<A> lhs)
    {
        /// <summary>
        /// Function composition
        /// </summary>
        /// <param name="x">Input</param>
        /// <param name="f">Function</param>
        /// <returns>Result of invoking the function</returns>
        public static Fail<B> operator >>(Fail<A> x, Func<A, B> f) =>
            new (f(x.Value));
    }     
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Conversion
    //
    
    public static Validation<F, A> ToValidation<F, A>(this Fail<F> fail) 
        where F : Monoid<F> =>
        Validation.Fail<F, A>(fail.Value);
    
    extension(Fail<Error> fail)
    {
        public Fin<A> ToFin<A>() =>
            Fin.Fail<A>(fail.Value);
        
        public Eff<RT, A> ToEff<RT, A>() =>
            Eff<RT, A>.Fail(fail.Value);

        public Eff<A> ToEff<A>() =>
            Eff<A>.Fail(fail.Value);
    }
}
