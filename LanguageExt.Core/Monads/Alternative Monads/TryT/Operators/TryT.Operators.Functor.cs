using System;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static partial class TryTExtensions
{
    extension<M, A, B>(K<TryT<M>, A> _)
        where M : Monad<M>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static TryT<M, B> operator *(Func<A, B> f, K<TryT<M>, A> ma) =>
            +ma.Map(f);
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static TryT<M, B> operator *(K<TryT<M>, A> ma, Func<A, B> f) =>
            +ma.Map(f);
    }
    
    extension<M, A, B, C>(K<TryT<M>, A> _)
        where M : Monad<M>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static TryT<M, Func<B, C>> operator * (
            Func<A, B, C> f, 
            K<TryT<M>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static TryT<M, Func<B, C>> operator * (
            K<TryT<M>, A> ma,
            Func<A, B, C> f) =>
            curry(f) * ma;
    }
        
    extension<M, A, B, C, D>(K<TryT<M>, A> _)
        where M : Monad<M>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static TryT<M, Func<B, Func<C, D>>> operator * (
            Func<A, B, C, D> f, 
            K<TryT<M>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static TryT<M, Func<B, Func<C, D>>> operator * (
            K<TryT<M>, A> ma,
            Func<A, B, C, D> f) =>
            curry(f) * ma;
    }
            
    extension<M, A, B, C, D, E>(K<TryT<M>, A> _)
        where M : Monad<M>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static TryT<M, Func<B, Func<C, Func<D, E>>>> operator * (
            Func<A, B, C, D, E> f, 
            K<TryT<M>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static TryT<M, Func<B, Func<C, Func<D, E>>>> operator * (
            K<TryT<M>, A> ma,
            Func<A, B, C, D, E> f) =>
            curry(f) * ma;
    }
                
    extension<M, A, B, C, D, E, F>(K<TryT<M>, A> _)
        where M : Monad<M>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static TryT<M, Func<B, Func<C, Func<D, Func<E, F>>>>> operator * (
            Func<A, B, C, D, E, F> f, 
            K<TryT<M>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static TryT<M, Func<B, Func<C, Func<D, Func<E, F>>>>> operator * (
            K<TryT<M>, A> ma,
            Func<A, B, C, D, E, F> f) =>
            curry(f) * ma;
    }
                    
    extension<M, A, B, C, D, E, F, G>(K<TryT<M>, A> _)
        where M : Monad<M>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static TryT<M, Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> operator * (
            Func<A, B, C, D, E, F, G> f, 
            K<TryT<M>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static TryT<M, Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> operator * (
            K<TryT<M>, A> ma,
            Func<A, B, C, D, E, F, G> f) =>
            curry(f) * ma;
    }    
                        
    extension<M, A, B, C, D, E, F, G, H>(K<TryT<M>, A> _)
        where M : Monad<M>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static TryT<M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> operator * (
            Func<A, B, C, D, E, F, G, H> f, 
            K<TryT<M>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static TryT<M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> operator * (
            K<TryT<M>, A> ma,
            Func<A, B, C, D, E, F, G, H> f) =>
            curry(f) * ma;
    }
                        
    extension<M, A, B, C, D, E, F, G, H, I>(K<TryT<M>, A> _)
        where M : Monad<M>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static TryT<M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> operator * (
            Func<A, B, C, D, E, F, G, H, I> f, 
            K<TryT<M>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static TryT<M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> operator * (
            K<TryT<M>, A> ma,
            Func<A, B, C, D, E, F, G, H, I> f) =>
            curry(f) * ma;
    }    
                        
    extension<M, A, B, C, D, E, F, G, H, I, J>(K<TryT<M>, A> _)
        where M : Monad<M>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static TryT<M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> operator * (
            Func<A, B, C, D, E, F, G, H, I, J> f, 
            K<TryT<M>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static TryT<M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> operator * (
            K<TryT<M>, A> ma,
            Func<A, B, C, D, E, F, G, H, I, J> f) =>
            curry(f) * ma;
    }
                            
    extension<M, A, B, C, D, E, F, G, H, I, J, K>(K<TryT<M>, A> _)
        where M : Monad<M>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static TryT<M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> operator * (
            Func<A, B, C, D, E, F, G, H, I, J, K> f, 
            K<TryT<M>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static TryT<M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> operator * (
            K<TryT<M>, A> ma,
            Func<A, B, C, D, E, F, G, H, I, J, K> f) =>
            curry(f) * ma;
    }
}
