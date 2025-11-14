using System;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static partial class EitherTExtensions
{
    extension<L, M, A, B>(K<EitherT<L, M>, A> _)
        where M : Monad<M>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static EitherT<L, M, B> operator *(Func<A, B> f, K<EitherT<L, M>, A> ma) =>
            +ma.Map(f);
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static EitherT<L, M, B> operator *(K<EitherT<L, M>, A> ma, Func<A, B> f) =>
            +ma.Map(f);
    }
    
    extension<L, M, A, B, C>(K<EitherT<L, M>, A> _)
        where M : Monad<M>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static EitherT<L, M, Func<B, C>> operator * (
            Func<A, B, C> f, 
            K<EitherT<L, M>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static EitherT<L, M, Func<B, C>> operator * (
            K<EitherT<L, M>, A> ma,
            Func<A, B, C> f) =>
            curry(f) * ma;
    }
        
    extension<L, M, A, B, C, D>(K<EitherT<L, M>, A> _)
        where M : Monad<M>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static EitherT<L, M, Func<B, Func<C, D>>> operator * (
            Func<A, B, C, D> f, 
            K<EitherT<L, M>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static EitherT<L, M, Func<B, Func<C, D>>> operator * (
            K<EitherT<L, M>, A> ma,
            Func<A, B, C, D> f) =>
            curry(f) * ma;
    }
            
    extension<L, M, A, B, C, D, E>(K<EitherT<L, M>, A> _)
        where M : Monad<M>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static EitherT<L, M, Func<B, Func<C, Func<D, E>>>> operator * (
            Func<A, B, C, D, E> f, 
            K<EitherT<L, M>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static EitherT<L, M, Func<B, Func<C, Func<D, E>>>> operator * (
            K<EitherT<L, M>, A> ma,
            Func<A, B, C, D, E> f) =>
            curry(f) * ma;
    }
                
    extension<L, M, A, B, C, D, E, F>(K<EitherT<L, M>, A> _)
        where M : Monad<M>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static EitherT<L, M, Func<B, Func<C, Func<D, Func<E, F>>>>> operator * (
            Func<A, B, C, D, E, F> f, 
            K<EitherT<L, M>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static EitherT<L, M, Func<B, Func<C, Func<D, Func<E, F>>>>> operator * (
            K<EitherT<L, M>, A> ma,
            Func<A, B, C, D, E, F> f) =>
            curry(f) * ma;
    }
                    
    extension<L, M, A, B, C, D, E, F, G>(K<EitherT<L, M>, A> _)
        where M : Monad<M>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static EitherT<L, M, Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> operator * (
            Func<A, B, C, D, E, F, G> f, 
            K<EitherT<L, M>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static EitherT<L, M, Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> operator * (
            K<EitherT<L, M>, A> ma,
            Func<A, B, C, D, E, F, G> f) =>
            curry(f) * ma;
    }    
                        
    extension<L, M, A, B, C, D, E, F, G, H>(K<EitherT<L, M>, A> _)
        where M : Monad<M>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static EitherT<L, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> operator * (
            Func<A, B, C, D, E, F, G, H> f, 
            K<EitherT<L, M>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static EitherT<L, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> operator * (
            K<EitherT<L, M>, A> ma,
            Func<A, B, C, D, E, F, G, H> f) =>
            curry(f) * ma;
    }
                        
    extension<L, M, A, B, C, D, E, F, G, H, I>(K<EitherT<L, M>, A> _)
        where M : Monad<M>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static EitherT<L, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> operator * (
            Func<A, B, C, D, E, F, G, H, I> f, 
            K<EitherT<L, M>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static EitherT<L, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> operator * (
            K<EitherT<L, M>, A> ma,
            Func<A, B, C, D, E, F, G, H, I> f) =>
            curry(f) * ma;
    }    
                        
    extension<L, M, A, B, C, D, E, F, G, H, I, J>(K<EitherT<L, M>, A> _)
        where M : Monad<M>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static EitherT<L, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> operator * (
            Func<A, B, C, D, E, F, G, H, I, J> f, 
            K<EitherT<L, M>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static EitherT<L, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> operator * (
            K<EitherT<L, M>, A> ma,
            Func<A, B, C, D, E, F, G, H, I, J> f) =>
            curry(f) * ma;
    }
                            
    extension<L, M, A, B, C, D, E, F, G, H, I, J, K>(K<EitherT<L, M>, A> _)
        where M : Monad<M>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static EitherT<L, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> operator * (
            Func<A, B, C, D, E, F, G, H, I, J, K> f, 
            K<EitherT<L, M>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static EitherT<L, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> operator * (
            K<EitherT<L, M>, A> ma,
            Func<A, B, C, D, E, F, G, H, I, J, K> f) =>
            curry(f) * ma;
    }
}
