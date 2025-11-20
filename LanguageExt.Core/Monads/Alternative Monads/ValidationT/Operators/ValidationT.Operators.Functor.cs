using System;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static partial class ValidationTExtensions
{
    extension<FF, M, A, B>(K<ValidationT<FF, M>, A> _)
        where M : Monad<M>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static ValidationT<FF, M, B> operator *(Func<A, B> f, K<ValidationT<FF, M>, A> ma) =>
            +ma.Map(f);
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static ValidationT<FF, M, B> operator *(K<ValidationT<FF, M>, A> ma, Func<A, B> f) =>
            +ma.Map(f);
    }
    
    extension<FF, M, A, B, C>(K<ValidationT<FF, M>, A> _)
        where M : Monad<M>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static ValidationT<FF, M, Func<B, C>> operator * (
            Func<A, B, C> f, 
            K<ValidationT<FF, M>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static ValidationT<FF, M, Func<B, C>> operator * (
            K<ValidationT<FF, M>, A> ma,
            Func<A, B, C> f) =>
            curry(f) * ma;
    }
        
    extension<FF, M, A, B, C, D>(K<ValidationT<FF, M>, A> _)
        where M : Monad<M>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static ValidationT<FF, M, Func<B, Func<C, D>>> operator * (
            Func<A, B, C, D> f, 
            K<ValidationT<FF, M>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static ValidationT<FF, M, Func<B, Func<C, D>>> operator * (
            K<ValidationT<FF, M>, A> ma,
            Func<A, B, C, D> f) =>
            curry(f) * ma;
    }
            
    extension<FF, M, A, B, C, D, E>(K<ValidationT<FF, M>, A> _)
        where M : Monad<M>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static ValidationT<FF, M, Func<B, Func<C, Func<D, E>>>> operator * (
            Func<A, B, C, D, E> f, 
            K<ValidationT<FF, M>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static ValidationT<FF, M, Func<B, Func<C, Func<D, E>>>> operator * (
            K<ValidationT<FF, M>, A> ma,
            Func<A, B, C, D, E> f) =>
            curry(f) * ma;
    }
                
    extension<FF, M, A, B, C, D, E, F>(K<ValidationT<FF, M>, A> _)
        where M : Monad<M>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static ValidationT<FF, M, Func<B, Func<C, Func<D, Func<E, F>>>>> operator * (
            Func<A, B, C, D, E, F> f, 
            K<ValidationT<FF, M>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static ValidationT<FF, M, Func<B, Func<C, Func<D, Func<E, F>>>>> operator * (
            K<ValidationT<FF, M>, A> ma,
            Func<A, B, C, D, E, F> f) =>
            curry(f) * ma;
    }
                    
    extension<FF, M, A, B, C, D, E, F, G>(K<ValidationT<FF, M>, A> _)
        where M : Monad<M>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static ValidationT<FF, M, Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> operator * (
            Func<A, B, C, D, E, F, G> f, 
            K<ValidationT<FF, M>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static ValidationT<FF, M, Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> operator * (
            K<ValidationT<FF, M>, A> ma,
            Func<A, B, C, D, E, F, G> f) =>
            curry(f) * ma;
    }    
                        
    extension<FF, M, A, B, C, D, E, F, G, H>(K<ValidationT<FF, M>, A> _)
        where M : Monad<M>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static ValidationT<FF, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> operator * (
            Func<A, B, C, D, E, F, G, H> f, 
            K<ValidationT<FF, M>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static ValidationT<FF, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> operator * (
            K<ValidationT<FF, M>, A> ma,
            Func<A, B, C, D, E, F, G, H> f) =>
            curry(f) * ma;
    }
                        
    extension<FF, M, A, B, C, D, E, F, G, H, I>(K<ValidationT<FF, M>, A> _)
        where M : Monad<M>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static ValidationT<FF, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> operator * (
            Func<A, B, C, D, E, F, G, H, I> f, 
            K<ValidationT<FF, M>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static ValidationT<FF, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> operator * (
            K<ValidationT<FF, M>, A> ma,
            Func<A, B, C, D, E, F, G, H, I> f) =>
            curry(f) * ma;
    }    
                        
    extension<FF, M, A, B, C, D, E, F, G, H, I, J>(K<ValidationT<FF, M>, A> _)
        where M : Monad<M>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static ValidationT<FF, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> operator * (
            Func<A, B, C, D, E, F, G, H, I, J> f, 
            K<ValidationT<FF, M>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static ValidationT<FF, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> operator * (
            K<ValidationT<FF, M>, A> ma,
            Func<A, B, C, D, E, F, G, H, I, J> f) =>
            curry(f) * ma;
    }
                            
    extension<FF, M, A, B, C, D, E, F, G, H, I, J, K>(K<ValidationT<FF, M>, A> _)
        where M : Monad<M>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static ValidationT<FF, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> operator * (
            Func<A, B, C, D, E, F, G, H, I, J, K> f, 
            K<ValidationT<FF, M>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static ValidationT<FF, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> operator * (
            K<ValidationT<FF, M>, A> ma,
            Func<A, B, C, D, E, F, G, H, I, J, K> f) =>
            curry(f) * ma;
    }
}
