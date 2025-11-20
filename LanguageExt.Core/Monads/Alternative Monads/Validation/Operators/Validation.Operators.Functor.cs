using System;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static partial class ValidationExtensions
{
    extension<FF, A, B>(K<Validation<FF>, A> _)
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Validation<FF, B> operator *(Func<A, B> f, K<Validation<FF>, A> ma) =>
            +ma.Map(f);
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Validation<FF, B> operator *(K<Validation<FF>, A> ma, Func<A, B> f) =>
            +ma.Map(f);
    }
    
    extension<FF, A, B, C>(K<Validation<FF>, A> _)
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Validation<FF, Func<B, C>> operator * (
            Func<A, B, C> f, 
            K<Validation<FF>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Validation<FF, Func<B, C>> operator * (
            K<Validation<FF>, A> ma,
            Func<A, B, C> f) =>
            curry(f) * ma;
    }
        
    extension<FF, A, B, C, D>(K<Validation<FF>, A> _)
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Validation<FF, Func<B, Func<C, D>>> operator * (
            Func<A, B, C, D> f, 
            K<Validation<FF>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Validation<FF, Func<B, Func<C, D>>> operator * (
            K<Validation<FF>, A> ma,
            Func<A, B, C, D> f) =>
            curry(f) * ma;
    }
            
    extension<FF, A, B, C, D, E>(K<Validation<FF>, A> _)
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Validation<FF, Func<B, Func<C, Func<D, E>>>> operator * (
            Func<A, B, C, D, E> f, 
            K<Validation<FF>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Validation<FF, Func<B, Func<C, Func<D, E>>>> operator * (
            K<Validation<FF>, A> ma,
            Func<A, B, C, D, E> f) =>
            curry(f) * ma;
    }
                
    extension<FF, A, B, C, D, E, F>(K<Validation<FF>, A> _)
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Validation<FF, Func<B, Func<C, Func<D, Func<E, F>>>>> operator * (
            Func<A, B, C, D, E, F> f, 
            K<Validation<FF>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Validation<FF, Func<B, Func<C, Func<D, Func<E, F>>>>> operator * (
            K<Validation<FF>, A> ma,
            Func<A, B, C, D, E, F> f) =>
            curry(f) * ma;
    }
                    
    extension<FF, A, B, C, D, E, F, G>(K<Validation<FF>, A> _)
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Validation<FF, Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> operator * (
            Func<A, B, C, D, E, F, G> f, 
            K<Validation<FF>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Validation<FF, Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> operator * (
            K<Validation<FF>, A> ma,
            Func<A, B, C, D, E, F, G> f) =>
            curry(f) * ma;
    }    
                        
    extension<FF, A, B, C, D, E, F, G, H>(K<Validation<FF>, A> _)
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Validation<FF, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> operator * (
            Func<A, B, C, D, E, F, G, H> f, 
            K<Validation<FF>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Validation<FF, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> operator * (
            K<Validation<FF>, A> ma,
            Func<A, B, C, D, E, F, G, H> f) =>
            curry(f) * ma;
    }
                        
    extension<FF, A, B, C, D, E, F, G, H, I>(K<Validation<FF>, A> _)
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Validation<FF, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> operator * (
            Func<A, B, C, D, E, F, G, H, I> f, 
            K<Validation<FF>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Validation<FF, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> operator * (
            K<Validation<FF>, A> ma,
            Func<A, B, C, D, E, F, G, H, I> f) =>
            curry(f) * ma;
    }    
                        
    extension<FF, A, B, C, D, E, F, G, H, I, J>(K<Validation<FF>, A> _)
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Validation<FF, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> operator * (
            Func<A, B, C, D, E, F, G, H, I, J> f, 
            K<Validation<FF>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Validation<FF, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> operator * (
            K<Validation<FF>, A> ma,
            Func<A, B, C, D, E, F, G, H, I, J> f) =>
            curry(f) * ma;
    }
                            
    extension<FF, A, B, C, D, E, F, G, H, I, J, K>(K<Validation<FF>, A> _)
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Validation<FF, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> operator * (
            Func<A, B, C, D, E, F, G, H, I, J, K> f, 
            K<Validation<FF>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Validation<FF, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> operator * (
            K<Validation<FF>, A> ma,
            Func<A, B, C, D, E, F, G, H, I, J, K> f) =>
            curry(f) * ma;
    }
}
