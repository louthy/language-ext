using System;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static partial class FinTExtensions
{
    extension<M, A, B>(K<FinT<M>, A> self)
        where M : Monad<M>
    {
        
        /// <summary>
        /// Applicative sequence operator
        /// </summary>
        public static FinT<M, B> operator >>> (K<FinT<M>, A> ma, K<FinT<M>, B> mb) =>
            +ma.Action(mb);
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static FinT<M, B> operator * (K<FinT<M>, Func<A, B>> mf, K<FinT<M>, A> ma) =>
            mf.Apply(ma);
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static FinT<M, B> operator * (K<FinT<M>, A> ma, K<FinT<M>, Func<A, B>> mf) =>
            mf.Apply(ma);        
    }
    
    extension<M, A, B, C>(K<FinT<M>, A> self)
        where M : Monad<M>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static FinT<M, Func<B, C>> operator * (
            K<FinT<M>, Func<A, B, C>> mf, 
            K<FinT<M>, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static FinT<M, Func<B, C>> operator * (
            K<FinT<M>, A> ma,
            K<FinT<M>, Func<A, B, C>> mf) =>
            curry * mf * ma;
    }
        
    extension<M, A, B, C, D>(K<FinT<M>, A> self)
        where M : Monad<M>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static FinT<M, Func<B, Func<C, D>>> operator * (
            K<FinT<M>, Func<A, B, C, D>> mf, 
            K<FinT<M>, A> ma) =>
            curry * mf * ma;

        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static FinT<M, Func<B, Func<C, D>>> operator * (
            K<FinT<M>, A> ma,
            K<FinT<M>, Func<A, B, C, D>> mf) =>
            curry * mf * ma;
    }
            
    extension<M, A, B, C, D, E>(K<FinT<M>, A> self)
        where M : Monad<M>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static FinT<M, Func<B, Func<C, Func<D, E>>>> operator * (
            K<FinT<M>, Func<A, B, C, D, E>> mf, 
            K<FinT<M>, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static FinT<M, Func<B, Func<C, Func<D, E>>>> operator * (
            K<FinT<M>, A> ma,
            K<FinT<M>, Func<A, B, C, D, E>> mf) =>
            curry * mf * ma;
    }
                
    extension<M, A, B, C, D, E, F>(K<FinT<M>, A> self)
        where M : Monad<M>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static FinT<M, Func<B, Func<C, Func<D, Func<E, F>>>>> operator * (
            K<FinT<M>, Func<A, B, C, D, E, F>> mf, 
            K<FinT<M>, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static FinT<M, Func<B, Func<C, Func<D, Func<E, F>>>>> operator * (
            K<FinT<M>, A> ma,
            K<FinT<M>, Func<A, B, C, D, E, F>> mf) =>
            curry * mf * ma;
    }
                    
    extension<M, A, B, C, D, E, F, G>(K<FinT<M>, A> self)
        where M : Monad<M>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static FinT<M, Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> operator * (
            K<FinT<M>, Func<A, B, C, D, E, F, G>> mf, 
            K<FinT<M>, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static FinT<M, Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> operator * (
            K<FinT<M>, A> ma,
            K<FinT<M>, Func<A, B, C, D, E, F, G>> mf) =>
            curry * mf * ma;
    }
                    
    extension<M, A, B, C, D, E, F, G, H>(K<FinT<M>, A> self)
        where M : Monad<M>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static FinT<M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> operator * (
            K<FinT<M>, Func<A, B, C, D, E, F, G, H>> mf, 
            K<FinT<M>, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static FinT<M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> operator * (
            K<FinT<M>, A> ma,
            K<FinT<M>, Func<A, B, C, D, E, F, G, H>> mf) =>
            curry * mf * ma;
    }
                        
    extension<M, A, B, C, D, E, F, G, H, I>(K<FinT<M>, A> self)
        where M : Monad<M>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static FinT<M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> operator * (
            K<FinT<M>, Func<A, B, C, D, E, F, G, H, I>> mf, 
            K<FinT<M>, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static FinT<M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> operator * (
            K<FinT<M>, A> ma,
            K<FinT<M>, Func<A, B, C, D, E, F, G, H, I>> mf) =>
            curry * mf * ma;
    }
                            
    extension<M, A, B, C, D, E, F, G, H, I, J>(K<FinT<M>, A> self)
        where M : Monad<M>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static FinT<M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> operator * (
            K<FinT<M>, Func<A, B, C, D, E, F, G, H, I, J>> mf, 
            K<FinT<M>, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static FinT<M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> operator * (
            K<FinT<M>, A> ma,
            K<FinT<M>, Func<A, B, C, D, E, F, G, H, I, J>> mf) =>
            curry * mf * ma;
    }
                                
    extension<M, A, B, C, D, E, F, G, H, I, J, K>(K<FinT<M>, A> self)
        where M : Monad<M>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static FinT<M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> operator * (
            K<FinT<M>, Func<A, B, C, D, E, F, G, H, I, J, K>> mf, 
            K<FinT<M>, A> ma) =>
            curry * mf * ma;

        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static FinT<M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> operator *(
            K<FinT<M>, A> ma,
            K<FinT<M>, Func<A, B, C, D, E, F, G, H, I, J, K>> mf) =>
            curry * mf * ma;
    }
}
