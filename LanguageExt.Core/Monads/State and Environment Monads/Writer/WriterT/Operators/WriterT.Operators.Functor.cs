using System;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static partial class WriterTExtensions
{
    extension<W, M, A, B>(K<WriterT<W, M>, A> _)
        where M : Monad<M>
        where W : Monoid<W>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static WriterT<W, M, B> operator *(Func<A, B> f, K<WriterT<W, M>, A> ma) =>
            +ma.Map(f);
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static WriterT<W, M, B> operator *(K<WriterT<W, M>, A> ma, Func<A, B> f) =>
            +ma.Map(f);
    }
    
    extension<W, M, A, B, C>(K<WriterT<W, M>, A> _)
        where M : Monad<M>
        where W : Monoid<W>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static WriterT<W, M, Func<B, C>> operator * (
            Func<A, B, C> f, 
            K<WriterT<W, M>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static WriterT<W, M, Func<B, C>> operator * (
            K<WriterT<W, M>, A> ma,
            Func<A, B, C> f) =>
            curry(f) * ma;
    }
        
    extension<W, M, A, B, C, D>(K<WriterT<W, M>, A> _)
        where M : Monad<M>
        where W : Monoid<W>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static WriterT<W, M, Func<B, Func<C, D>>> operator * (
            Func<A, B, C, D> f, 
            K<WriterT<W, M>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static WriterT<W, M, Func<B, Func<C, D>>> operator * (
            K<WriterT<W, M>, A> ma,
            Func<A, B, C, D> f) =>
            curry(f) * ma;
    }
            
    extension<W, M, A, B, C, D, E>(K<WriterT<W, M>, A> _)
        where M : Monad<M>
        where W : Monoid<W>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static WriterT<W, M, Func<B, Func<C, Func<D, E>>>> operator * (
            Func<A, B, C, D, E> f, 
            K<WriterT<W, M>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static WriterT<W, M, Func<B, Func<C, Func<D, E>>>> operator * (
            K<WriterT<W, M>, A> ma,
            Func<A, B, C, D, E> f) =>
            curry(f) * ma;
    }
                
    extension<W, M, A, B, C, D, E, F>(K<WriterT<W, M>, A> _)
        where M : Monad<M>
        where W : Monoid<W>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static WriterT<W, M, Func<B, Func<C, Func<D, Func<E, F>>>>> operator * (
            Func<A, B, C, D, E, F> f, 
            K<WriterT<W, M>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static WriterT<W, M, Func<B, Func<C, Func<D, Func<E, F>>>>> operator * (
            K<WriterT<W, M>, A> ma,
            Func<A, B, C, D, E, F> f) =>
            curry(f) * ma;
    }
                    
    extension<W, M, A, B, C, D, E, F, G>(K<WriterT<W, M>, A> _)
        where M : Monad<M>
        where W : Monoid<W>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static WriterT<W, M, Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> operator * (
            Func<A, B, C, D, E, F, G> f, 
            K<WriterT<W, M>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static WriterT<W, M, Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> operator * (
            K<WriterT<W, M>, A> ma,
            Func<A, B, C, D, E, F, G> f) =>
            curry(f) * ma;
    }    
                        
    extension<W, M, A, B, C, D, E, F, G, H>(K<WriterT<W, M>, A> _)
        where M : Monad<M>
        where W : Monoid<W>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static WriterT<W, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> operator * (
            Func<A, B, C, D, E, F, G, H> f, 
            K<WriterT<W, M>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static WriterT<W, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> operator * (
            K<WriterT<W, M>, A> ma,
            Func<A, B, C, D, E, F, G, H> f) =>
            curry(f) * ma;
    }
                        
    extension<W, M, A, B, C, D, E, F, G, H, I>(K<WriterT<W, M>, A> _)
        where M : Monad<M>
        where W : Monoid<W>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static WriterT<W, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> operator * (
            Func<A, B, C, D, E, F, G, H, I> f, 
            K<WriterT<W, M>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static WriterT<W, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> operator * (
            K<WriterT<W, M>, A> ma,
            Func<A, B, C, D, E, F, G, H, I> f) =>
            curry(f) * ma;
    }    
                        
    extension<W, M, A, B, C, D, E, F, G, H, I, J>(K<WriterT<W, M>, A> _)
        where M : Monad<M>
        where W : Monoid<W>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static WriterT<W, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> operator * (
            Func<A, B, C, D, E, F, G, H, I, J> f, 
            K<WriterT<W, M>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static WriterT<W, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> operator * (
            K<WriterT<W, M>, A> ma,
            Func<A, B, C, D, E, F, G, H, I, J> f) =>
            curry(f) * ma;
    }
                            
    extension<W, M, A, B, C, D, E, F, G, H, I, J, K>(K<WriterT<W, M>, A> _)
        where M : Monad<M>
        where W : Monoid<W>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static WriterT<W, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> operator * (
            Func<A, B, C, D, E, F, G, H, I, J, K> f, 
            K<WriterT<W, M>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static WriterT<W, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> operator * (
            K<WriterT<W, M>, A> ma,
            Func<A, B, C, D, E, F, G, H, I, J, K> f) =>
            curry(f) * ma;
    }
}
