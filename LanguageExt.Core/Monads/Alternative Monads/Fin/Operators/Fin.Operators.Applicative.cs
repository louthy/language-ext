using System;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static partial class FinExtensions
{
    extension<A, B>(K<Fin, A> self)
    {
        
        /// <summary>
        /// Applicative sequence operator
        /// </summary>
        public static Fin<B> operator >>> (K<Fin, A> ma, K<Fin, B> mb) =>
            ma.Action(mb).As();
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Fin<B> operator * (K<Fin, Func<A, B>> mf, K<Fin, A> ma) =>
            mf.Apply(ma);
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Fin<B> operator * (K<Fin, A> ma, K<Fin, Func<A, B>> mf) =>
            mf.Apply(ma);        
    }
    
    extension<A, B, C>(K<Fin, A> self)
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Fin<Func<B, C>> operator * (
            K<Fin, Func<A, B, C>> mf, 
            K<Fin, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Fin<Func<B, C>> operator * (
            K<Fin, A> ma,
            K<Fin, Func<A, B, C>> mf) =>
            curry * mf * ma;
    }
        
    extension<A, B, C, D>(K<Fin, A> self)
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Fin<Func<B, Func<C, D>>> operator * (
            K<Fin, Func<A, B, C, D>> mf, 
            K<Fin, A> ma) =>
            curry * mf * ma;

        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Fin<Func<B, Func<C, D>>> operator * (
            K<Fin, A> ma,
            K<Fin, Func<A, B, C, D>> mf) =>
            curry * mf * ma;
    }
            
    extension<A, B, C, D, E>(K<Fin, A> self)
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Fin<Func<B, Func<C, Func<D, E>>>> operator * (
            K<Fin, Func<A, B, C, D, E>> mf, 
            K<Fin, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Fin<Func<B, Func<C, Func<D, E>>>> operator * (
            K<Fin, A> ma,
            K<Fin, Func<A, B, C, D, E>> mf) =>
            curry * mf * ma;
    }
                
    extension<A, B, C, D, E, F>(K<Fin, A> self)
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Fin<Func<B, Func<C, Func<D, Func<E, F>>>>> operator * (
            K<Fin, Func<A, B, C, D, E, F>> mf, 
            K<Fin, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Fin<Func<B, Func<C, Func<D, Func<E, F>>>>> operator * (
            K<Fin, A> ma,
            K<Fin, Func<A, B, C, D, E, F>> mf) =>
            curry * mf * ma;
    }
                    
    extension<A, B, C, D, E, F, G>(K<Fin, A> self)
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Fin<Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> operator * (
            K<Fin, Func<A, B, C, D, E, F, G>> mf, 
            K<Fin, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Fin<Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> operator * (
            K<Fin, A> ma,
            K<Fin, Func<A, B, C, D, E, F, G>> mf) =>
            curry * mf * ma;
    }
                    
    extension<A, B, C, D, E, F, G, H>(K<Fin, A> self)
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Fin<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> operator * (
            K<Fin, Func<A, B, C, D, E, F, G, H>> mf, 
            K<Fin, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Fin<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> operator * (
            K<Fin, A> ma,
            K<Fin, Func<A, B, C, D, E, F, G, H>> mf) =>
            curry * mf * ma;
    }
                        
    extension<A, B, C, D, E, F, G, H, I>(K<Fin, A> self)
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Fin<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> operator * (
            K<Fin, Func<A, B, C, D, E, F, G, H, I>> mf, 
            K<Fin, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Fin<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> operator * (
            K<Fin, A> ma,
            K<Fin, Func<A, B, C, D, E, F, G, H, I>> mf) =>
            curry * mf * ma;
    }
                            
    extension<A, B, C, D, E, F, G, H, I, J>(K<Fin, A> self)
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Fin<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> operator * (
            K<Fin, Func<A, B, C, D, E, F, G, H, I, J>> mf, 
            K<Fin, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Fin<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> operator * (
            K<Fin, A> ma,
            K<Fin, Func<A, B, C, D, E, F, G, H, I, J>> mf) =>
            curry * mf * ma;
    }
                                
    extension<A, B, C, D, E, F, G, H, I, J, K>(K<Fin, A> self)
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Fin<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> operator * (
            K<Fin, Func<A, B, C, D, E, F, G, H, I, J, K>> mf, 
            K<Fin, A> ma) =>
            curry * mf * ma;

        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Fin<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> operator *(
            K<Fin, A> ma,
            K<Fin, Func<A, B, C, D, E, F, G, H, I, J, K>> mf) =>
            curry * mf * ma;
    }
}
