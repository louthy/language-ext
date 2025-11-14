using System;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static partial class IOExtensions
{
    extension<A, B>(K<IO, A> _)
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static IO<B> operator *(Func<A, B> f, K<IO, A> ma) =>
            +ma.Map(f);
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static IO<B> operator *(K<IO, A> ma, Func<A, B> f) =>
            +ma.Map(f);
    }
    
    extension<A, B, C>(K<IO, A> _)
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static IO<Func<B, C>> operator * (
            Func<A, B, C> f, 
            K<IO, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static IO<Func<B, C>> operator * (
            K<IO, A> ma,
            Func<A, B, C> f) =>
            curry(f) * ma;
    }
        
    extension<A, B, C, D>(K<IO, A> _)
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static IO<Func<B, Func<C, D>>> operator * (
            Func<A, B, C, D> f, 
            K<IO, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static IO<Func<B, Func<C, D>>> operator * (
            K<IO, A> ma,
            Func<A, B, C, D> f) =>
            curry(f) * ma;
    }
            
    extension<A, B, C, D, E>(K<IO, A> _)
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static IO<Func<B, Func<C, Func<D, E>>>> operator * (
            Func<A, B, C, D, E> f, 
            K<IO, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static IO<Func<B, Func<C, Func<D, E>>>> operator * (
            K<IO, A> ma,
            Func<A, B, C, D, E> f) =>
            curry(f) * ma;
    }
                
    extension<A, B, C, D, E, F>(K<IO, A> _)
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static IO<Func<B, Func<C, Func<D, Func<E, F>>>>> operator * (
            Func<A, B, C, D, E, F> f, 
            K<IO, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static IO<Func<B, Func<C, Func<D, Func<E, F>>>>> operator * (
            K<IO, A> ma,
            Func<A, B, C, D, E, F> f) =>
            curry(f) * ma;
    }
                    
    extension<A, B, C, D, E, F, G>(K<IO, A> _)
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static IO<Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> operator * (
            Func<A, B, C, D, E, F, G> f, 
            K<IO, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static IO<Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> operator * (
            K<IO, A> ma,
            Func<A, B, C, D, E, F, G> f) =>
            curry(f) * ma;
    }    
                        
    extension<A, B, C, D, E, F, G, H>(K<IO, A> _)
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static IO<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> operator * (
            Func<A, B, C, D, E, F, G, H> f, 
            K<IO, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static IO<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> operator * (
            K<IO, A> ma,
            Func<A, B, C, D, E, F, G, H> f) =>
            curry(f) * ma;
    }
                        
    extension<A, B, C, D, E, F, G, H, I>(K<IO, A> _)
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static IO<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> operator * (
            Func<A, B, C, D, E, F, G, H, I> f, 
            K<IO, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static IO<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> operator * (
            K<IO, A> ma,
            Func<A, B, C, D, E, F, G, H, I> f) =>
            curry(f) * ma;
    }    
                        
    extension<A, B, C, D, E, F, G, H, I, J>(K<IO, A> _)
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static IO<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> operator * (
            Func<A, B, C, D, E, F, G, H, I, J> f, 
            K<IO, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static IO<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> operator * (
            K<IO, A> ma,
            Func<A, B, C, D, E, F, G, H, I, J> f) =>
            curry(f) * ma;
    }
                            
    extension<A, B, C, D, E, F, G, H, I, J, K>(K<IO, A> _)
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static IO<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> operator * (
            Func<A, B, C, D, E, F, G, H, I, J, K> f, 
            K<IO, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static IO<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> operator * (
            K<IO, A> ma,
            Func<A, B, C, D, E, F, G, H, I, J, K> f) =>
            curry(f) * ma;
    }
}
