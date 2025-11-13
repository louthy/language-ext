using System;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static partial class EffExtensions
{
    extension<RT, A, B>(K<Eff<RT>, A> self)
    {
        /// <summary>
        /// Applicative sequence operator
        /// </summary>
        public static Eff<RT, B> operator >>> (K<Eff<RT>, A> ma, K<Eff<RT>, B> mb) =>
            ma.Action(mb).As();
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Eff<RT, B> operator * (K<Eff<RT>, Func<A, B>> mf, K<Eff<RT>, A> ma) =>
            mf.Apply(ma);
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Eff<RT, B> operator * (K<Eff<RT>, A> ma, K<Eff<RT>, Func<A, B>> mf) =>
            mf.Apply(ma);        
    }
    
    extension<RT, A, B, C>(K<Eff<RT>, A> self)
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Eff<RT, Func<B, C>> operator * (
            K<Eff<RT>, Func<A, B, C>> mf, 
            K<Eff<RT>, A> ma) =>
            curry * mf * ma;
    }
        
    extension<RT, A, B, C, D>(K<Eff<RT>, A> self)
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Eff<RT, Func<B, Func<C, D>>> operator * (
            K<Eff<RT>, Func<A, B, C, D>> mf, 
            K<Eff<RT>, A> ma) =>
            curry * mf * ma;

        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Eff<RT, Func<B, Func<C, D>>> operator * (
            K<Eff<RT>, A> ma,
            K<Eff<RT>, Func<A, B, C, D>> mf) =>
            curry * mf * ma;
    }
            
    extension<RT, A, B, C, D, E>(K<Eff<RT>, A> self)
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Eff<RT, Func<B, Func<C, Func<D, E>>>> operator * (
            K<Eff<RT>, Func<A, B, C, D, E>> mf, 
            K<Eff<RT>, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Eff<RT, Func<B, Func<C, Func<D, E>>>> operator * (
            K<Eff<RT>, A> ma,
            K<Eff<RT>, Func<A, B, C, D, E>> mf) =>
            curry * mf * ma;
    }
                
    extension<RT, A, B, C, D, E, F>(K<Eff<RT>, A> self)
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Eff<RT, Func<B, Func<C, Func<D, Func<E, F>>>>> operator * (
            K<Eff<RT>, Func<A, B, C, D, E, F>> mf, 
            K<Eff<RT>, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Eff<RT, Func<B, Func<C, Func<D, Func<E, F>>>>> operator * (
            K<Eff<RT>, A> ma,
            K<Eff<RT>, Func<A, B, C, D, E, F>> mf) =>
            curry * mf * ma;
    }
                    
    extension<RT, A, B, C, D, E, F, G>(K<Eff<RT>, A> self)
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Eff<RT, Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> operator * (
            K<Eff<RT>, Func<A, B, C, D, E, F, G>> mf, 
            K<Eff<RT>, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Eff<RT, Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> operator * (
            K<Eff<RT>, A> ma,
            K<Eff<RT>, Func<A, B, C, D, E, F, G>> mf) =>
            curry * mf * ma;
    }
                    
    extension<RT, A, B, C, D, E, F, G, H>(K<Eff<RT>, A> self)
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Eff<RT, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> operator * (
            K<Eff<RT>, Func<A, B, C, D, E, F, G, H>> mf, 
            K<Eff<RT>, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Eff<RT, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> operator * (
            K<Eff<RT>, A> ma,
            K<Eff<RT>, Func<A, B, C, D, E, F, G, H>> mf) =>
            curry * mf * ma;
    }
                        
    extension<RT, A, B, C, D, E, F, G, H, I>(K<Eff<RT>, A> self)
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Eff<RT, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> operator * (
            K<Eff<RT>, Func<A, B, C, D, E, F, G, H, I>> mf, 
            K<Eff<RT>, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Eff<RT, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> operator * (
            K<Eff<RT>, A> ma,
            K<Eff<RT>, Func<A, B, C, D, E, F, G, H, I>> mf) =>
            curry * mf * ma;
    }
                            
    extension<RT, A, B, C, D, E, F, G, H, I, J>(K<Eff<RT>, A> self)
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Eff<RT, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> operator * (
            K<Eff<RT>, Func<A, B, C, D, E, F, G, H, I, J>> mf, 
            K<Eff<RT>, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Eff<RT, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> operator * (
            K<Eff<RT>, A> ma,
            K<Eff<RT>, Func<A, B, C, D, E, F, G, H, I, J>> mf) =>
            curry * mf * ma;
    }
                                
    extension<RT, A, B, C, D, E, F, G, H, I, J, K>(K<Eff<RT>, A> self)
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Eff<RT, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> operator * (
            K<Eff<RT>, Func<A, B, C, D, E, F, G, H, I, J, K>> mf, 
            K<Eff<RT>, A> ma) =>
            curry * mf * ma;

        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Eff<RT, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> operator *(
            K<Eff<RT>, A> ma,
            K<Eff<RT>, Func<A, B, C, D, E, F, G, H, I, J, K>> mf) =>
            curry * mf * ma;
    }
}
