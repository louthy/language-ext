using System;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static partial class IteratorExtensions
{
    extension<A, B>(K<Iterator, A> _)
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Iterator<B> operator *(Func<A, B> f, K<Iterator, A> ma) =>
            +ma.Map(f);
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Iterator<B> operator *(K<Iterator, A> ma, Func<A, B> f) =>
            +ma.Map(f);
    }
    
    extension<A, B, C>(K<Iterator, A> _)
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Iterator<Func<B, C>> operator * (
            Func<A, B, C> f, 
            K<Iterator, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Iterator<Func<B, C>> operator * (
            K<Iterator, A> ma,
            Func<A, B, C> f) =>
            curry(f) * ma;
    }
        
    extension<A, B, C, D>(K<Iterator, A> _)
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Iterator<Func<B, Func<C, D>>> operator * (
            Func<A, B, C, D> f, 
            K<Iterator, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Iterator<Func<B, Func<C, D>>> operator * (
            K<Iterator, A> ma,
            Func<A, B, C, D> f) =>
            curry(f) * ma;
    }
            
    extension<A, B, C, D, E>(K<Iterator, A> _)
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Iterator<Func<B, Func<C, Func<D, E>>>> operator * (
            Func<A, B, C, D, E> f, 
            K<Iterator, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Iterator<Func<B, Func<C, Func<D, E>>>> operator * (
            K<Iterator, A> ma,
            Func<A, B, C, D, E> f) =>
            curry(f) * ma;
    }
                
    extension<A, B, C, D, E, F>(K<Iterator, A> _)
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Iterator<Func<B, Func<C, Func<D, Func<E, F>>>>> operator * (
            Func<A, B, C, D, E, F> f, 
            K<Iterator, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Iterator<Func<B, Func<C, Func<D, Func<E, F>>>>> operator * (
            K<Iterator, A> ma,
            Func<A, B, C, D, E, F> f) =>
            curry(f) * ma;
    }
                    
    extension<A, B, C, D, E, F, G>(K<Iterator, A> _)
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Iterator<Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> operator * (
            Func<A, B, C, D, E, F, G> f, 
            K<Iterator, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Iterator<Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> operator * (
            K<Iterator, A> ma,
            Func<A, B, C, D, E, F, G> f) =>
            curry(f) * ma;
    }    
                        
    extension<A, B, C, D, E, F, G, H>(K<Iterator, A> _)
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Iterator<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> operator * (
            Func<A, B, C, D, E, F, G, H> f, 
            K<Iterator, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Iterator<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> operator * (
            K<Iterator, A> ma,
            Func<A, B, C, D, E, F, G, H> f) =>
            curry(f) * ma;
    }
                        
    extension<A, B, C, D, E, F, G, H, I>(K<Iterator, A> _)
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Iterator<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> operator * (
            Func<A, B, C, D, E, F, G, H, I> f, 
            K<Iterator, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Iterator<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> operator * (
            K<Iterator, A> ma,
            Func<A, B, C, D, E, F, G, H, I> f) =>
            curry(f) * ma;
    }    
                        
    extension<A, B, C, D, E, F, G, H, I, J>(K<Iterator, A> _)
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Iterator<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> operator * (
            Func<A, B, C, D, E, F, G, H, I, J> f, 
            K<Iterator, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Iterator<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> operator * (
            K<Iterator, A> ma,
            Func<A, B, C, D, E, F, G, H, I, J> f) =>
            curry(f) * ma;
    }
                            
    extension<A, B, C, D, E, F, G, H, I, J, K>(K<Iterator, A> _)
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Iterator<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> operator * (
            Func<A, B, C, D, E, F, G, H, I, J, K> f, 
            K<Iterator, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Iterator<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> operator * (
            K<Iterator, A> ma,
            Func<A, B, C, D, E, F, G, H, I, J, K> f) =>
            curry(f) * ma;
    }
}
