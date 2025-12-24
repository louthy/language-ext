using System;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static partial class RWSTExtensions
{
    extension<R, W, S, M, A, B>(K<RWST<R, W, S, M>, A> _)
        where M : Monad<M>
        where W : Monoid<W>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static RWST<R, W, S, M, B> operator *(Func<A, B> f, K<RWST<R, W, S, M>, A> ma) =>
            +ma.Map(f);
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static RWST<R, W, S, M, B> operator *(K<RWST<R, W, S, M>, A> ma, Func<A, B> f) =>
            +ma.Map(f);
    }
    
    extension<R, W, S, M, A, B, C>(K<RWST<R, W, S, M>, A> _)
        where M : Monad<M>
        where W : Monoid<W>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static RWST<R, W, S, M, Func<B, C>> operator * (
            Func<A, B, C> f, 
            K<RWST<R, W, S, M>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static RWST<R, W, S, M, Func<B, C>> operator * (
            K<RWST<R, W, S, M>, A> ma,
            Func<A, B, C> f) =>
            curry(f) * ma;
    }
        
    extension<R, W, S, M, A, B, C, D>(K<RWST<R, W, S, M>, A> _)
        where M : Monad<M>
        where W : Monoid<W>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static RWST<R, W, S, M, Func<B, Func<C, D>>> operator * (
            Func<A, B, C, D> f, 
            K<RWST<R, W, S, M>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static RWST<R, W, S, M, Func<B, Func<C, D>>> operator * (
            K<RWST<R, W, S, M>, A> ma,
            Func<A, B, C, D> f) =>
            curry(f) * ma;
    }
            
    extension<R, W, S, M, A, B, C, D, E>(K<RWST<R, W, S, M>, A> _)
        where M : Monad<M>
        where W : Monoid<W>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static RWST<R, W, S, M, Func<B, Func<C, Func<D, E>>>> operator * (
            Func<A, B, C, D, E> f, 
            K<RWST<R, W, S, M>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static RWST<R, W, S, M, Func<B, Func<C, Func<D, E>>>> operator * (
            K<RWST<R, W, S, M>, A> ma,
            Func<A, B, C, D, E> f) =>
            curry(f) * ma;
    }
                
    extension<R, W, S, M, A, B, C, D, E, F>(K<RWST<R, W, S, M>, A> _)
        where M : Monad<M>
        where W : Monoid<W>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static RWST<R, W, S, M, Func<B, Func<C, Func<D, Func<E, F>>>>> operator * (
            Func<A, B, C, D, E, F> f, 
            K<RWST<R, W, S, M>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static RWST<R, W, S, M, Func<B, Func<C, Func<D, Func<E, F>>>>> operator * (
            K<RWST<R, W, S, M>, A> ma,
            Func<A, B, C, D, E, F> f) =>
            curry(f) * ma;
    }
                    
    extension<R, W, S, M, A, B, C, D, E, F, G>(K<RWST<R, W, S, M>, A> _)
        where M : Monad<M>
        where W : Monoid<W>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static RWST<R, W, S, M, Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> operator * (
            Func<A, B, C, D, E, F, G> f, 
            K<RWST<R, W, S, M>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static RWST<R, W, S, M, Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> operator * (
            K<RWST<R, W, S, M>, A> ma,
            Func<A, B, C, D, E, F, G> f) =>
            curry(f) * ma;
    }    
                        
    extension<R, W, S, M, A, B, C, D, E, F, G, H>(K<RWST<R, W, S, M>, A> _)
        where W : Monoid<W>
        where M : Monad<M>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static RWST<R, W, S, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> operator * (
            Func<A, B, C, D, E, F, G, H> f, 
            K<RWST<R, W, S, M>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static RWST<R, W, S, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> operator * (
            K<RWST<R, W, S, M>, A> ma,
            Func<A, B, C, D, E, F, G, H> f) =>
            curry(f) * ma;
    }
                        
    extension<R, W, S, M, A, B, C, D, E, F, G, H, I>(K<RWST<R, W, S, M>, A> _)
        where M : Monad<M>
        where W : Monoid<W>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static RWST<R, W, S, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> operator * (
            Func<A, B, C, D, E, F, G, H, I> f, 
            K<RWST<R, W, S, M>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static RWST<R, W, S, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> operator * (
            K<RWST<R, W, S, M>, A> ma,
            Func<A, B, C, D, E, F, G, H, I> f) =>
            curry(f) * ma;
    }    
                        
    extension<R, W, S, M, A, B, C, D, E, F, G, H, I, J>(K<RWST<R, W, S, M>, A> _)
        where M : Monad<M>
        where W : Monoid<W>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static RWST<R, W, S, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> operator * (
            Func<A, B, C, D, E, F, G, H, I, J> f, 
            K<RWST<R, W, S, M>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static RWST<R, W, S, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> operator * (
            K<RWST<R, W, S, M>, A> ma,
            Func<A, B, C, D, E, F, G, H, I, J> f) =>
            curry(f) * ma;
    }
                            
    extension<R, W, S, M, A, B, C, D, E, F, G, H, I, J, K>(K<RWST<R, W, S, M>, A> _)
        where M : Monad<M>
        where W : Monoid<W>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static RWST<R, W, S, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> operator * (
            Func<A, B, C, D, E, F, G, H, I, J, K> f, 
            K<RWST<R, W, S, M>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static RWST<R, W, S, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> operator * (
            K<RWST<R, W, S, M>, A> ma,
            Func<A, B, C, D, E, F, G, H, I, J, K> f) =>
            curry(f) * ma;
    }
}
