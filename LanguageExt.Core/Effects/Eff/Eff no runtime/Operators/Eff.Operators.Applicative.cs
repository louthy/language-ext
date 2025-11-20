using System;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static partial class EffExtensions
{
    extension<A, B>(K<Eff, A> self)
    {
        /// <summary>
        /// Applicative sequence operator
        /// </summary>
        public static Eff<B> operator >>> (K<Eff, A> ma, K<Eff, B> mb) =>
            ma.Action(mb).As();
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Eff<B> operator * (K<Eff, Func<A, B>> mf, K<Eff, A> ma) =>
            mf.Apply(ma);
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Eff<B> operator * (K<Eff, A> ma, K<Eff, Func<A, B>> mf) =>
            mf.Apply(ma);        
    }

    extension<A, B, C>(K<Eff, A> self)
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Eff<Func<B, C>> operator * (
            K<Eff, Func<A, B, C>> mf, 
            K<Eff, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Eff<Func<B, C>> operator * (
            K<Eff, A> ma,
            K<Eff, Func<A, B, C>> mf) =>
            curry * mf * ma;
    }
        
    extension<A, B, C, D>(K<Eff, A> self)
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Eff<Func<B, Func<C, D>>> operator * (
            K<Eff, Func<A, B, C, D>> mf, 
            K<Eff, A> ma) =>
            curry * mf * ma;

        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Eff<Func<B, Func<C, D>>> operator * (
            K<Eff, A> ma,
            K<Eff, Func<A, B, C, D>> mf) =>
            curry * mf * ma;
    }
            
    extension<A, B, C, D, E>(K<Eff, A> self)
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Eff<Func<B, Func<C, Func<D, E>>>> operator * (
            K<Eff, Func<A, B, C, D, E>> mf, 
            K<Eff, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Eff<Func<B, Func<C, Func<D, E>>>> operator * (
            K<Eff, A> ma,
            K<Eff, Func<A, B, C, D, E>> mf) =>
            curry * mf * ma;
    }
                
    extension<A, B, C, D, E, F>(K<Eff, A> self)
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Eff<Func<B, Func<C, Func<D, Func<E, F>>>>> operator * (
            K<Eff, Func<A, B, C, D, E, F>> mf, 
            K<Eff, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Eff<Func<B, Func<C, Func<D, Func<E, F>>>>> operator * (
            K<Eff, A> ma,
            K<Eff, Func<A, B, C, D, E, F>> mf) =>
            curry * mf * ma;
    }
                    
    extension<A, B, C, D, E, F, G>(K<Eff, A> self)
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Eff<Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> operator * (
            K<Eff, Func<A, B, C, D, E, F, G>> mf, 
            K<Eff, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Eff<Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> operator * (
            K<Eff, A> ma,
            K<Eff, Func<A, B, C, D, E, F, G>> mf) =>
            curry * mf * ma;
    }
                    
    extension<A, B, C, D, E, F, G, H>(K<Eff, A> self)
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Eff<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> operator * (
            K<Eff, Func<A, B, C, D, E, F, G, H>> mf, 
            K<Eff, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Eff<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> operator * (
            K<Eff, A> ma,
            K<Eff, Func<A, B, C, D, E, F, G, H>> mf) =>
            curry * mf * ma;
    }
                        
    extension<A, B, C, D, E, F, G, H, I>(K<Eff, A> self)
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Eff<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> operator * (
            K<Eff, Func<A, B, C, D, E, F, G, H, I>> mf, 
            K<Eff, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Eff<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> operator * (
            K<Eff, A> ma,
            K<Eff, Func<A, B, C, D, E, F, G, H, I>> mf) =>
            curry * mf * ma;
    }
                            
    extension<A, B, C, D, E, F, G, H, I, J>(K<Eff, A> self)
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Eff<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> operator * (
            K<Eff, Func<A, B, C, D, E, F, G, H, I, J>> mf, 
            K<Eff, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Eff<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> operator * (
            K<Eff, A> ma,
            K<Eff, Func<A, B, C, D, E, F, G, H, I, J>> mf) =>
            curry * mf * ma;
    }
                                
    extension<A, B, C, D, E, F, G, H, I, J, K>(K<Eff, A> self)
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Eff<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> operator * (
            K<Eff, Func<A, B, C, D, E, F, G, H, I, J, K>> mf, 
            K<Eff, A> ma) =>
            curry * mf * ma;

        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Eff<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> operator *(
            K<Eff, A> ma,
            K<Eff, Func<A, B, C, D, E, F, G, H, I, J, K>> mf) =>
            curry * mf * ma;
    }
}
