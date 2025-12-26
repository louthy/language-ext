using System;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static partial class SourceTExtensions
{
    extension<M, A, B>(K<SourceT<M>, A> _)
        where M : MonadIO<M>, Alternative<M>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static SourceT<M, B> operator *(Func<A, B> f, K<SourceT<M>, A> ma) =>
            +ma.Map(f);
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static SourceT<M, B> operator *(K<SourceT<M>, A> ma, Func<A, B> f) =>
            +ma.Map(f);
    }
    
    extension<M, A, B, C>(K<SourceT<M>, A> _)
        where M : MonadIO<M>, Alternative<M>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static SourceT<M, Func<B, C>> operator * (
            Func<A, B, C> f, 
            K<SourceT<M>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static SourceT<M, Func<B, C>> operator * (
            K<SourceT<M>, A> ma,
            Func<A, B, C> f) =>
            curry(f) * ma;
    }
        
    extension<M, A, B, C, D>(K<SourceT<M>, A> _)
        where M : MonadIO<M>, Alternative<M>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static SourceT<M, Func<B, Func<C, D>>> operator * (
            Func<A, B, C, D> f, 
            K<SourceT<M>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static SourceT<M, Func<B, Func<C, D>>> operator * (
            K<SourceT<M>, A> ma,
            Func<A, B, C, D> f) =>
            curry(f) * ma;
    }
            
    extension<M, A, B, C, D, E>(K<SourceT<M>, A> _)
        where M : MonadIO<M>, Alternative<M>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static SourceT<M, Func<B, Func<C, Func<D, E>>>> operator * (
            Func<A, B, C, D, E> f, 
            K<SourceT<M>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static SourceT<M, Func<B, Func<C, Func<D, E>>>> operator * (
            K<SourceT<M>, A> ma,
            Func<A, B, C, D, E> f) =>
            curry(f) * ma;
    }
                
    extension<M, A, B, C, D, E, F>(K<SourceT<M>, A> _)
        where M : MonadIO<M>, Alternative<M>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static SourceT<M, Func<B, Func<C, Func<D, Func<E, F>>>>> operator * (
            Func<A, B, C, D, E, F> f, 
            K<SourceT<M>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static SourceT<M, Func<B, Func<C, Func<D, Func<E, F>>>>> operator * (
            K<SourceT<M>, A> ma,
            Func<A, B, C, D, E, F> f) =>
            curry(f) * ma;
    }
                    
    extension<M, A, B, C, D, E, F, G>(K<SourceT<M>, A> _)
        where M : MonadIO<M>, Alternative<M>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static SourceT<M, Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> operator * (
            Func<A, B, C, D, E, F, G> f, 
            K<SourceT<M>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static SourceT<M, Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> operator * (
            K<SourceT<M>, A> ma,
            Func<A, B, C, D, E, F, G> f) =>
            curry(f) * ma;
    }    
                        
    extension<M, A, B, C, D, E, F, G, H>(K<SourceT<M>, A> _)
        where M : MonadIO<M>, Alternative<M>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static SourceT<M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> operator * (
            Func<A, B, C, D, E, F, G, H> f, 
            K<SourceT<M>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static SourceT<M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> operator * (
            K<SourceT<M>, A> ma,
            Func<A, B, C, D, E, F, G, H> f) =>
            curry(f) * ma;
    }
                        
    extension<M, A, B, C, D, E, F, G, H, I>(K<SourceT<M>, A> _)
        where M : MonadIO<M>, Alternative<M>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static SourceT<M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> operator * (
            Func<A, B, C, D, E, F, G, H, I> f, 
            K<SourceT<M>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static SourceT<M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> operator * (
            K<SourceT<M>, A> ma,
            Func<A, B, C, D, E, F, G, H, I> f) =>
            curry(f) * ma;
    }    
                        
    extension<M, A, B, C, D, E, F, G, H, I, J>(K<SourceT<M>, A> _)
        where M : MonadIO<M>, Alternative<M>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static SourceT<M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> operator * (
            Func<A, B, C, D, E, F, G, H, I, J> f, 
            K<SourceT<M>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static SourceT<M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> operator * (
            K<SourceT<M>, A> ma,
            Func<A, B, C, D, E, F, G, H, I, J> f) =>
            curry(f) * ma;
    }
                            
    extension<M, A, B, C, D, E, F, G, H, I, J, K>(K<SourceT<M>, A> _)
        where M : MonadIO<M>, Alternative<M>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static SourceT<M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> operator * (
            Func<A, B, C, D, E, F, G, H, I, J, K> f, 
            K<SourceT<M>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static SourceT<M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> operator * (
            K<SourceT<M>, A> ma,
            Func<A, B, C, D, E, F, G, H, I, J, K> f) =>
            curry(f) * ma;
    }
}
