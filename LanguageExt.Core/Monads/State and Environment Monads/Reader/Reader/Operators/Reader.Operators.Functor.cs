using System;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static partial class ReaderExtensions
{
    extension<Env, A, B>(K<Reader<Env>, A> _)
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Reader<Env, B> operator *(Func<A, B> f, K<Reader<Env>, A> ma) =>
            +ma.Map(f);
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Reader<Env, B> operator *(K<Reader<Env>, A> ma, Func<A, B> f) =>
            +ma.Map(f);
    }
    
    extension<Env, A, B, C>(K<Reader<Env>, A> _)
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Reader<Env, Func<B, C>> operator * (
            Func<A, B, C> f, 
            K<Reader<Env>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Reader<Env, Func<B, C>> operator * (
            K<Reader<Env>, A> ma,
            Func<A, B, C> f) =>
            curry(f) * ma;
    }
        
    extension<Env, A, B, C, D>(K<Reader<Env>, A> _)
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Reader<Env, Func<B, Func<C, D>>> operator * (
            Func<A, B, C, D> f, 
            K<Reader<Env>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Reader<Env, Func<B, Func<C, D>>> operator * (
            K<Reader<Env>, A> ma,
            Func<A, B, C, D> f) =>
            curry(f) * ma;
    }
            
    extension<Env, A, B, C, D, E>(K<Reader<Env>, A> _)
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Reader<Env, Func<B, Func<C, Func<D, E>>>> operator * (
            Func<A, B, C, D, E> f, 
            K<Reader<Env>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Reader<Env, Func<B, Func<C, Func<D, E>>>> operator * (
            K<Reader<Env>, A> ma,
            Func<A, B, C, D, E> f) =>
            curry(f) * ma;
    }
                
    extension<Env, A, B, C, D, E, F>(K<Reader<Env>, A> _)
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Reader<Env, Func<B, Func<C, Func<D, Func<E, F>>>>> operator * (
            Func<A, B, C, D, E, F> f, 
            K<Reader<Env>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Reader<Env, Func<B, Func<C, Func<D, Func<E, F>>>>> operator * (
            K<Reader<Env>, A> ma,
            Func<A, B, C, D, E, F> f) =>
            curry(f) * ma;
    }
                    
    extension<Env, A, B, C, D, E, F, G>(K<Reader<Env>, A> _)
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Reader<Env, Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> operator * (
            Func<A, B, C, D, E, F, G> f, 
            K<Reader<Env>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Reader<Env, Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> operator * (
            K<Reader<Env>, A> ma,
            Func<A, B, C, D, E, F, G> f) =>
            curry(f) * ma;
    }    
                        
    extension<Env, A, B, C, D, E, F, G, H>(K<Reader<Env>, A> _)
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Reader<Env, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> operator * (
            Func<A, B, C, D, E, F, G, H> f, 
            K<Reader<Env>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Reader<Env, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> operator * (
            K<Reader<Env>, A> ma,
            Func<A, B, C, D, E, F, G, H> f) =>
            curry(f) * ma;
    }
                        
    extension<Env, A, B, C, D, E, F, G, H, I>(K<Reader<Env>, A> _)
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Reader<Env, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> operator * (
            Func<A, B, C, D, E, F, G, H, I> f, 
            K<Reader<Env>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Reader<Env, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> operator * (
            K<Reader<Env>, A> ma,
            Func<A, B, C, D, E, F, G, H, I> f) =>
            curry(f) * ma;
    }    
                        
    extension<Env, A, B, C, D, E, F, G, H, I, J>(K<Reader<Env>, A> _)
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Reader<Env, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> operator * (
            Func<A, B, C, D, E, F, G, H, I, J> f, 
            K<Reader<Env>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Reader<Env, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> operator * (
            K<Reader<Env>, A> ma,
            Func<A, B, C, D, E, F, G, H, I, J> f) =>
            curry(f) * ma;
    }
                            
    extension<Env, A, B, C, D, E, F, G, H, I, J, K>(K<Reader<Env>, A> _)
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Reader<Env, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> operator * (
            Func<A, B, C, D, E, F, G, H, I, J, K> f, 
            K<Reader<Env>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Reader<Env, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> operator * (
            K<Reader<Env>, A> ma,
            Func<A, B, C, D, E, F, G, H, I, J, K> f) =>
            curry(f) * ma;
    }
}
