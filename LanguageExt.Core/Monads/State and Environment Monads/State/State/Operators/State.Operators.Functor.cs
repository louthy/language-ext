using System;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static partial class StateExtensions
{
    extension<S, A, B>(K<State<S>, A> _)
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static State<S, B> operator *(Func<A, B> f, K<State<S>, A> ma) =>
            +ma.Map(f);
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static State<S, B> operator *(K<State<S>, A> ma, Func<A, B> f) =>
            +ma.Map(f);
    }
    
    extension<S, A, B, C>(K<State<S>, A> _)
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static State<S, Func<B, C>> operator * (
            Func<A, B, C> f, 
            K<State<S>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static State<S, Func<B, C>> operator * (
            K<State<S>, A> ma,
            Func<A, B, C> f) =>
            curry(f) * ma;
    }
        
    extension<S, A, B, C, D>(K<State<S>, A> _)
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static State<S, Func<B, Func<C, D>>> operator * (
            Func<A, B, C, D> f, 
            K<State<S>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static State<S, Func<B, Func<C, D>>> operator * (
            K<State<S>, A> ma,
            Func<A, B, C, D> f) =>
            curry(f) * ma;
    }
            
    extension<S, A, B, C, D, E>(K<State<S>, A> _)
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static State<S, Func<B, Func<C, Func<D, E>>>> operator * (
            Func<A, B, C, D, E> f, 
            K<State<S>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static State<S, Func<B, Func<C, Func<D, E>>>> operator * (
            K<State<S>, A> ma,
            Func<A, B, C, D, E> f) =>
            curry(f) * ma;
    }
                
    extension<S, A, B, C, D, E, F>(K<State<S>, A> _)
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static State<S, Func<B, Func<C, Func<D, Func<E, F>>>>> operator * (
            Func<A, B, C, D, E, F> f, 
            K<State<S>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static State<S, Func<B, Func<C, Func<D, Func<E, F>>>>> operator * (
            K<State<S>, A> ma,
            Func<A, B, C, D, E, F> f) =>
            curry(f) * ma;
    }
                    
    extension<S, A, B, C, D, E, F, G>(K<State<S>, A> _)
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static State<S, Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> operator * (
            Func<A, B, C, D, E, F, G> f, 
            K<State<S>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static State<S, Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> operator * (
            K<State<S>, A> ma,
            Func<A, B, C, D, E, F, G> f) =>
            curry(f) * ma;
    }    
                        
    extension<S, A, B, C, D, E, F, G, H>(K<State<S>, A> _)
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static State<S, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> operator * (
            Func<A, B, C, D, E, F, G, H> f, 
            K<State<S>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static State<S, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> operator * (
            K<State<S>, A> ma,
            Func<A, B, C, D, E, F, G, H> f) =>
            curry(f) * ma;
    }
                        
    extension<S, A, B, C, D, E, F, G, H, I>(K<State<S>, A> _)
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static State<S, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> operator * (
            Func<A, B, C, D, E, F, G, H, I> f, 
            K<State<S>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static State<S, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> operator * (
            K<State<S>, A> ma,
            Func<A, B, C, D, E, F, G, H, I> f) =>
            curry(f) * ma;
    }    
                        
    extension<S, A, B, C, D, E, F, G, H, I, J>(K<State<S>, A> _)
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static State<S, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> operator * (
            Func<A, B, C, D, E, F, G, H, I, J> f, 
            K<State<S>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static State<S, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> operator * (
            K<State<S>, A> ma,
            Func<A, B, C, D, E, F, G, H, I, J> f) =>
            curry(f) * ma;
    }
                            
    extension<S, A, B, C, D, E, F, G, H, I, J, K>(K<State<S>, A> _)
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static State<S, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> operator * (
            Func<A, B, C, D, E, F, G, H, I, J, K> f, 
            K<State<S>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static State<S, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> operator * (
            K<State<S>, A> ma,
            Func<A, B, C, D, E, F, G, H, I, J, K> f) =>
            curry(f) * ma;
    }
}
