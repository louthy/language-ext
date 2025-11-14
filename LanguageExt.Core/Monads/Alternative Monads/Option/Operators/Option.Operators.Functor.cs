using System;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static partial class OptionExtensions
{
    extension<A, B>(K<Option, A> _)
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Option<B> operator *(Func<A, B> f, K<Option, A> ma) =>
            +ma.Map(f);
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Option<B> operator *(K<Option, A> ma, Func<A, B> f) =>
            +ma.Map(f);
    }
    
    extension<A, B, C>(K<Option, A> _)
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Option<Func<B, C>> operator * (
            Func<A, B, C> f, 
            K<Option, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Option<Func<B, C>> operator * (
            K<Option, A> ma,
            Func<A, B, C> f) =>
            curry(f) * ma;
    }
        
    extension<A, B, C, D>(K<Option, A> _)
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Option<Func<B, Func<C, D>>> operator * (
            Func<A, B, C, D> f, 
            K<Option, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Option<Func<B, Func<C, D>>> operator * (
            K<Option, A> ma,
            Func<A, B, C, D> f) =>
            curry(f) * ma;
    }
            
    extension<A, B, C, D, E>(K<Option, A> _)
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Option<Func<B, Func<C, Func<D, E>>>> operator * (
            Func<A, B, C, D, E> f, 
            K<Option, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Option<Func<B, Func<C, Func<D, E>>>> operator * (
            K<Option, A> ma,
            Func<A, B, C, D, E> f) =>
            curry(f) * ma;
    }
                
    extension<A, B, C, D, E, F>(K<Option, A> _)
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Option<Func<B, Func<C, Func<D, Func<E, F>>>>> operator * (
            Func<A, B, C, D, E, F> f, 
            K<Option, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Option<Func<B, Func<C, Func<D, Func<E, F>>>>> operator * (
            K<Option, A> ma,
            Func<A, B, C, D, E, F> f) =>
            curry(f) * ma;
    }
                    
    extension<A, B, C, D, E, F, G>(K<Option, A> _)
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Option<Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> operator * (
            Func<A, B, C, D, E, F, G> f, 
            K<Option, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Option<Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> operator * (
            K<Option, A> ma,
            Func<A, B, C, D, E, F, G> f) =>
            curry(f) * ma;
    }    
                        
    extension<A, B, C, D, E, F, G, H>(K<Option, A> _)
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Option<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> operator * (
            Func<A, B, C, D, E, F, G, H> f, 
            K<Option, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Option<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> operator * (
            K<Option, A> ma,
            Func<A, B, C, D, E, F, G, H> f) =>
            curry(f) * ma;
    }
                        
    extension<A, B, C, D, E, F, G, H, I>(K<Option, A> _)
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Option<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> operator * (
            Func<A, B, C, D, E, F, G, H, I> f, 
            K<Option, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Option<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> operator * (
            K<Option, A> ma,
            Func<A, B, C, D, E, F, G, H, I> f) =>
            curry(f) * ma;
    }    
                        
    extension<A, B, C, D, E, F, G, H, I, J>(K<Option, A> _)
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Option<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> operator * (
            Func<A, B, C, D, E, F, G, H, I, J> f, 
            K<Option, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Option<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> operator * (
            K<Option, A> ma,
            Func<A, B, C, D, E, F, G, H, I, J> f) =>
            curry(f) * ma;
    }
                            
    extension<A, B, C, D, E, F, G, H, I, J, K>(K<Option, A> _)
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Option<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> operator * (
            Func<A, B, C, D, E, F, G, H, I, J, K> f, 
            K<Option, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Option<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> operator * (
            K<Option, A> ma,
            Func<A, B, C, D, E, F, G, H, I, J, K> f) =>
            curry(f) * ma;
    }
}
