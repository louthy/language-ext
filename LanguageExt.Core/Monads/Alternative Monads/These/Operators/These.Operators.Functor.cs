using System;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static partial class TheseExtensions
{
    extension<X, A, B>(K<These<X>, A> _)
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static These<X, B> operator *(Func<A, B> f, K<These<X>, A> ma) =>
            +ma.Map(f);
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static These<X, B> operator *(K<These<X>, A> ma, Func<A, B> f) =>
            +ma.Map(f);
    }
    
    extension<X, A, B, C>(K<These<X>, A> _)
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static These<X, Func<B, C>> operator * (
            Func<A, B, C> f, 
            K<These<X>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static These<X, Func<B, C>> operator * (
            K<These<X>, A> ma,
            Func<A, B, C> f) =>
            curry(f) * ma;
    }
        
    extension<X, A, B, C, D>(K<These<X>, A> _)
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static These<X, Func<B, Func<C, D>>> operator * (
            Func<A, B, C, D> f, 
            K<These<X>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static These<X, Func<B, Func<C, D>>> operator * (
            K<These<X>, A> ma,
            Func<A, B, C, D> f) =>
            curry(f) * ma;
    }
            
    extension<X, A, B, C, D, E>(K<These<X>, A> _)
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static These<X, Func<B, Func<C, Func<D, E>>>> operator * (
            Func<A, B, C, D, E> f, 
            K<These<X>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static These<X, Func<B, Func<C, Func<D, E>>>> operator * (
            K<These<X>, A> ma,
            Func<A, B, C, D, E> f) =>
            curry(f) * ma;
    }
                
    extension<X, A, B, C, D, E, F>(K<These<X>, A> _)
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static These<X, Func<B, Func<C, Func<D, Func<E, F>>>>> operator * (
            Func<A, B, C, D, E, F> f, 
            K<These<X>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static These<X, Func<B, Func<C, Func<D, Func<E, F>>>>> operator * (
            K<These<X>, A> ma,
            Func<A, B, C, D, E, F> f) =>
            curry(f) * ma;
    }
                    
    extension<X, A, B, C, D, E, F, G>(K<These<X>, A> _)
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static These<X, Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> operator * (
            Func<A, B, C, D, E, F, G> f, 
            K<These<X>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static These<X, Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> operator * (
            K<These<X>, A> ma,
            Func<A, B, C, D, E, F, G> f) =>
            curry(f) * ma;
    }    
                        
    extension<X, A, B, C, D, E, F, G, H>(K<These<X>, A> _)
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static These<X, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> operator * (
            Func<A, B, C, D, E, F, G, H> f, 
            K<These<X>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static These<X, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> operator * (
            K<These<X>, A> ma,
            Func<A, B, C, D, E, F, G, H> f) =>
            curry(f) * ma;
    }
                        
    extension<X, A, B, C, D, E, F, G, H, I>(K<These<X>, A> _)
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static These<X, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> operator * (
            Func<A, B, C, D, E, F, G, H, I> f, 
            K<These<X>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static These<X, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> operator * (
            K<These<X>, A> ma,
            Func<A, B, C, D, E, F, G, H, I> f) =>
            curry(f) * ma;
    }    
                        
    extension<X, A, B, C, D, E, F, G, H, I, J>(K<These<X>, A> _)
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static These<X, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> operator * (
            Func<A, B, C, D, E, F, G, H, I, J> f, 
            K<These<X>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static These<X, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> operator * (
            K<These<X>, A> ma,
            Func<A, B, C, D, E, F, G, H, I, J> f) =>
            curry(f) * ma;
    }
                            
    extension<X, A, B, C, D, E, F, G, H, I, J, K>(K<These<X>, A> _)
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static These<X, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> operator * (
            Func<A, B, C, D, E, F, G, H, I, J, K> f, 
            K<These<X>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static These<X, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> operator * (
            K<These<X>, A> ma,
            Func<A, B, C, D, E, F, G, H, I, J, K> f) =>
            curry(f) * ma;
    }
}
