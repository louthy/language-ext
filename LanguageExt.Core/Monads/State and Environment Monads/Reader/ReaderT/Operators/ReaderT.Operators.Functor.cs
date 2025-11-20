using System;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static partial class ReaderTExtensions
{
    extension<Env, M, A, B>(K<ReaderT<Env, M>, A> _)
        where M : Monad<M>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static ReaderT<Env, M, B> operator *(Func<A, B> f, K<ReaderT<Env, M>, A> ma) =>
            +ma.Map(f);
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static ReaderT<Env, M, B> operator *(K<ReaderT<Env, M>, A> ma, Func<A, B> f) =>
            +ma.Map(f);
    }
    
    extension<Env, M, A, B, C>(K<ReaderT<Env, M>, A> _)
        where M : Monad<M>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static ReaderT<Env, M, Func<B, C>> operator * (
            Func<A, B, C> f, 
            K<ReaderT<Env, M>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static ReaderT<Env, M, Func<B, C>> operator * (
            K<ReaderT<Env, M>, A> ma,
            Func<A, B, C> f) =>
            curry(f) * ma;
    }
        
    extension<Env, M, A, B, C, D>(K<ReaderT<Env, M>, A> _)
        where M : Monad<M>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static ReaderT<Env, M, Func<B, Func<C, D>>> operator * (
            Func<A, B, C, D> f, 
            K<ReaderT<Env, M>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static ReaderT<Env, M, Func<B, Func<C, D>>> operator * (
            K<ReaderT<Env, M>, A> ma,
            Func<A, B, C, D> f) =>
            curry(f) * ma;
    }
            
    extension<Env, M, A, B, C, D, E>(K<ReaderT<Env, M>, A> _)
        where M : Monad<M>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static ReaderT<Env, M, Func<B, Func<C, Func<D, E>>>> operator * (
            Func<A, B, C, D, E> f, 
            K<ReaderT<Env, M>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static ReaderT<Env, M, Func<B, Func<C, Func<D, E>>>> operator * (
            K<ReaderT<Env, M>, A> ma,
            Func<A, B, C, D, E> f) =>
            curry(f) * ma;
    }
                
    extension<Env, M, A, B, C, D, E, F>(K<ReaderT<Env, M>, A> _)
        where M : Monad<M>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static ReaderT<Env, M, Func<B, Func<C, Func<D, Func<E, F>>>>> operator * (
            Func<A, B, C, D, E, F> f, 
            K<ReaderT<Env, M>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static ReaderT<Env, M, Func<B, Func<C, Func<D, Func<E, F>>>>> operator * (
            K<ReaderT<Env, M>, A> ma,
            Func<A, B, C, D, E, F> f) =>
            curry(f) * ma;
    }
                    
    extension<Env, M, A, B, C, D, E, F, G>(K<ReaderT<Env, M>, A> _)
        where M : Monad<M>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static ReaderT<Env, M, Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> operator * (
            Func<A, B, C, D, E, F, G> f, 
            K<ReaderT<Env, M>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static ReaderT<Env, M, Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> operator * (
            K<ReaderT<Env, M>, A> ma,
            Func<A, B, C, D, E, F, G> f) =>
            curry(f) * ma;
    }    
                        
    extension<Env, M, A, B, C, D, E, F, G, H>(K<ReaderT<Env, M>, A> _)
        where M : Monad<M>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static ReaderT<Env, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> operator * (
            Func<A, B, C, D, E, F, G, H> f, 
            K<ReaderT<Env, M>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static ReaderT<Env, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> operator * (
            K<ReaderT<Env, M>, A> ma,
            Func<A, B, C, D, E, F, G, H> f) =>
            curry(f) * ma;
    }
                        
    extension<Env, M, A, B, C, D, E, F, G, H, I>(K<ReaderT<Env, M>, A> _)
        where M : Monad<M>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static ReaderT<Env, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> operator * (
            Func<A, B, C, D, E, F, G, H, I> f, 
            K<ReaderT<Env, M>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static ReaderT<Env, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> operator * (
            K<ReaderT<Env, M>, A> ma,
            Func<A, B, C, D, E, F, G, H, I> f) =>
            curry(f) * ma;
    }    
                        
    extension<Env, M, A, B, C, D, E, F, G, H, I, J>(K<ReaderT<Env, M>, A> _)
        where M : Monad<M>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static ReaderT<Env, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> operator * (
            Func<A, B, C, D, E, F, G, H, I, J> f, 
            K<ReaderT<Env, M>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static ReaderT<Env, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> operator * (
            K<ReaderT<Env, M>, A> ma,
            Func<A, B, C, D, E, F, G, H, I, J> f) =>
            curry(f) * ma;
    }
                            
    extension<Env, M, A, B, C, D, E, F, G, H, I, J, K>(K<ReaderT<Env, M>, A> _)
        where M : Monad<M>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static ReaderT<Env, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> operator * (
            Func<A, B, C, D, E, F, G, H, I, J, K> f, 
            K<ReaderT<Env, M>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static ReaderT<Env, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> operator * (
            K<ReaderT<Env, M>, A> ma,
            Func<A, B, C, D, E, F, G, H, I, J, K> f) =>
            curry(f) * ma;
    }
}
