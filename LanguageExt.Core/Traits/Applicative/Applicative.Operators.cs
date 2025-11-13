using System;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static partial class ApplicativeExtensions
{
    extension<M, A, B>(K<M, A> self)
        where M : Applicative<M>
    {
        
        /// <summary>
        /// Applicative sequence operator
        /// </summary>
        public static K<M, B> operator >>> (K<M, A> ma, K<M, B> mb) =>
            M.Action(ma, mb);
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static K<M, B> operator * (K<M, Func<A, B>> mf, K<M, A> ma) =>
            M.Apply(mf, ma);
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static K<M, B> operator * (K<M, A> ma, K<M, Func<A, B>> mf) =>
            M.Apply(mf, ma);
    }
    
    extension<M, A, B, C>(K<M, A> self)
        where M : Applicative<M>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static K<M, Func<B, C>> operator * (
            K<M, Func<A, B, C>> mf, 
            K<M, A> ma) =>
            curry * mf * ma;
    }
        
    extension<M, A, B, C, D>(K<M, A> self)
        where M : Applicative<M>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static K<M, Func<B, Func<C, D>>> operator * (
            K<M, Func<A, B, C, D>> mf, 
            K<M, A> ma) =>
            curry * mf * ma;

        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static K<M, Func<B, Func<C, D>>> operator * (
            K<M, A> ma,
            K<M, Func<A, B, C, D>> mf) =>
            curry * mf * ma;        
    }
            
    extension<M, A, B, C, D, E>(K<M, A> self)
        where M : Applicative<M>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static K<M, Func<B, Func<C, Func<D, E>>>> operator * (
            K<M, Func<A, B, C, D, E>> mf, 
            K<M, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static K<M, Func<B, Func<C, Func<D, E>>>> operator * (
            K<M, A> ma,
            K<M, Func<A, B, C, D, E>> mf) =>
            curry * mf * ma;        
    }
                
    extension<M, A, B, C, D, E, F>(K<M, A> self)
        where M : Applicative<M>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static K<M, Func<B, Func<C, Func<D, Func<E, F>>>>> operator * (
            K<M, Func<A, B, C, D, E, F>> mf, 
            K<M, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static K<M, Func<B, Func<C, Func<D, Func<E, F>>>>> operator * (
            K<M, A> ma,
            K<M, Func<A, B, C, D, E, F>> mf) =>
            curry * mf * ma;
    }
                    
    extension<M, A, B, C, D, E, F, G>(K<M, A> self)
        where M : Applicative<M>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static K<M, Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> operator * (
            K<M, Func<A, B, C, D, E, F, G>> mf, 
            K<M, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static K<M, Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> operator * (
            K<M, A> ma,
            K<M, Func<A, B, C, D, E, F, G>> mf) =>
            curry * mf * ma;        
    }
                    
    extension<M, A, B, C, D, E, F, G, H>(K<M, A> self)
        where M : Applicative<M>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static K<M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> operator * (
            K<M, Func<A, B, C, D, E, F, G, H>> mf, 
            K<M, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static K<M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> operator * (
            K<M, A> ma,
            K<M, Func<A, B, C, D, E, F, G, H>> mf) =>
            curry * mf * ma;        
    }
                        
    extension<M, A, B, C, D, E, F, G, H, I>(K<M, A> self)
        where M : Applicative<M>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static K<M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> operator * (
            K<M, Func<A, B, C, D, E, F, G, H, I>> mf, 
            K<M, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static K<M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> operator * (
            K<M, A> ma,
            K<M, Func<A, B, C, D, E, F, G, H, I>> mf) =>
            curry * mf * ma;        
    }
                            
    extension<M, A, B, C, D, E, F, G, H, I, J>(K<M, A> self)
        where M : Applicative<M>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static K<M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> operator * (
            K<M, Func<A, B, C, D, E, F, G, H, I, J>> mf, 
            K<M, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static K<M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> operator * (
            K<M, A> ma,
            K<M, Func<A, B, C, D, E, F, G, H, I, J>> mf) =>
            curry * mf * ma;        
    }
                                
    extension<M, A, B, C, D, E, F, G, H, I, J, K>(K<M, A> self)
        where M : Applicative<M>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static K<M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> operator * (
            K<M, Func<A, B, C, D, E, F, G, H, I, J, K>> mf, 
            K<M, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static K<M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> operator * (
            K<M, A> ma,
            K<M, Func<A, B, C, D, E, F, G, H, I, J, K>> mf) =>
            curry * mf * ma;        
    }
}
