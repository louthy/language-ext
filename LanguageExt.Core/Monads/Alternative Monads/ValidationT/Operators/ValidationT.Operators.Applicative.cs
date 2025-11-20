using System;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static partial class ValidationTExtensions
{
    extension<FF, M, A, B>(K<ValidationT<FF, M>, A> self)
        where M : Monad<M>
    {
        /// <summary>
        /// Applicative sequence operator
        /// </summary>
        public static ValidationT<FF, M, B> operator >>> (K<ValidationT<FF, M>, A> ma, K<ValidationT<FF, M>, B> mb) =>
            +ma.Action(mb);
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static ValidationT<FF, M, B> operator * (K<ValidationT<FF, M>, Func<A, B>> mf, K<ValidationT<FF, M>, A> ma) =>
            +mf.Apply(ma);
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static ValidationT<FF, M, B> operator * (K<ValidationT<FF, M>, A> ma, K<ValidationT<FF, M>, Func<A, B>> mf) =>
            +mf.Apply(ma);        
    }
    
    extension<FF, M, A, B, C>(K<ValidationT<FF, M>, A> self)
        where M : Monad<M>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static ValidationT<FF, M, Func<B, C>> operator * (
            K<ValidationT<FF, M>, Func<A, B, C>> mf, 
            K<ValidationT<FF, M>, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static ValidationT<FF, M, Func<B, C>> operator * (
            K<ValidationT<FF, M>, A> ma,
            K<ValidationT<FF, M>, Func<A, B, C>> mf) =>
            curry * mf * ma;
    }
        
    extension<FF, M, A, B, C, D>(K<ValidationT<FF, M>, A> self)
        where M : Monad<M>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static ValidationT<FF, M, Func<B, Func<C, D>>> operator * (
            K<ValidationT<FF, M>, Func<A, B, C, D>> mf, 
            K<ValidationT<FF, M>, A> ma) =>
            curry * mf * ma;

        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static ValidationT<FF, M, Func<B, Func<C, D>>> operator * (
            K<ValidationT<FF, M>, A> ma,
            K<ValidationT<FF, M>, Func<A, B, C, D>> mf) =>
            curry * mf * ma;
    }
            
    extension<FF, M, A, B, C, D, E>(K<ValidationT<FF, M>, A> self)
        where M : Monad<M>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static ValidationT<FF, M, Func<B, Func<C, Func<D, E>>>> operator * (
            K<ValidationT<FF, M>, Func<A, B, C, D, E>> mf, 
            K<ValidationT<FF, M>, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static ValidationT<FF, M, Func<B, Func<C, Func<D, E>>>> operator * (
            K<ValidationT<FF, M>, A> ma,
            K<ValidationT<FF, M>, Func<A, B, C, D, E>> mf) =>
            curry * mf * ma;
    }
                
    extension<FF, M, A, B, C, D, E, F>(K<ValidationT<FF, M>, A> self)
        where M : Monad<M>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static ValidationT<FF, M, Func<B, Func<C, Func<D, Func<E, F>>>>> operator * (
            K<ValidationT<FF, M>, Func<A, B, C, D, E, F>> mf, 
            K<ValidationT<FF, M>, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static ValidationT<FF, M, Func<B, Func<C, Func<D, Func<E, F>>>>> operator * (
            K<ValidationT<FF, M>, A> ma,
            K<ValidationT<FF, M>, Func<A, B, C, D, E, F>> mf) =>
            curry * mf * ma;
    }
                    
    extension<FF, M, A, B, C, D, E, F, G>(K<ValidationT<FF, M>, A> self)
        where M : Monad<M>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static ValidationT<FF, M, Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> operator * (
            K<ValidationT<FF, M>, Func<A, B, C, D, E, F, G>> mf, 
            K<ValidationT<FF, M>, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static ValidationT<FF, M, Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> operator * (
            K<ValidationT<FF, M>, A> ma,
            K<ValidationT<FF, M>, Func<A, B, C, D, E, F, G>> mf) =>
            curry * mf * ma;
    }
                    
    extension<FF, M, A, B, C, D, E, F, G, H>(K<ValidationT<FF, M>, A> self)
        where M : Monad<M>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static ValidationT<FF, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> operator * (
            K<ValidationT<FF, M>, Func<A, B, C, D, E, F, G, H>> mf, 
            K<ValidationT<FF, M>, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static ValidationT<FF, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> operator * (
            K<ValidationT<FF, M>, A> ma,
            K<ValidationT<FF, M>, Func<A, B, C, D, E, F, G, H>> mf) =>
            curry * mf * ma;
    }
                        
    extension<FF, M, A, B, C, D, E, F, G, H, I>(K<ValidationT<FF, M>, A> self)
        where M : Monad<M>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static ValidationT<FF, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> operator * (
            K<ValidationT<FF, M>, Func<A, B, C, D, E, F, G, H, I>> mf, 
            K<ValidationT<FF, M>, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static ValidationT<FF, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> operator * (
            K<ValidationT<FF, M>, A> ma,
            K<ValidationT<FF, M>, Func<A, B, C, D, E, F, G, H, I>> mf) =>
            curry * mf * ma;
    }
                            
    extension<FF, M, A, B, C, D, E, F, G, H, I, J>(K<ValidationT<FF, M>, A> self)
        where M : Monad<M>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static ValidationT<FF, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> operator * (
            K<ValidationT<FF, M>, Func<A, B, C, D, E, F, G, H, I, J>> mf, 
            K<ValidationT<FF, M>, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static ValidationT<FF, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> operator * (
            K<ValidationT<FF, M>, A> ma,
            K<ValidationT<FF, M>, Func<A, B, C, D, E, F, G, H, I, J>> mf) =>
            curry * mf * ma;
    }
                                
    extension<FF, M, A, B, C, D, E, F, G, H, I, J, K>(K<ValidationT<FF, M>, A> self)
        where M : Monad<M>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static ValidationT<FF, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> operator * (
            K<ValidationT<FF, M>, Func<A, B, C, D, E, F, G, H, I, J, K>> mf, 
            K<ValidationT<FF, M>, A> ma) =>
            curry * mf * ma;

        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static ValidationT<FF, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> operator *(
            K<ValidationT<FF, M>, A> ma,
            K<ValidationT<FF, M>, Func<A, B, C, D, E, F, G, H, I, J, K>> mf) =>
            curry * mf * ma;
    }
}
