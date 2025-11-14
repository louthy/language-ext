using System;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static partial class FreeExtensions
{
    extension<Fun, A, B>(K<Free<Fun>, A> _)
        where Fun : Functor<Fun>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Free<Fun, B> operator *(Func<A, B> f, K<Free<Fun>, A> ma) =>
            +ma.Map(f);
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Free<Fun, B> operator *(K<Free<Fun>, A> ma, Func<A, B> f) =>
            +ma.Map(f);
    }
    
    extension<Fun, A, B, C>(K<Free<Fun>, A> _)
        where Fun : Functor<Fun>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Free<Fun, Func<B, C>> operator * (
            Func<A, B, C> f, 
            K<Free<Fun>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Free<Fun, Func<B, C>> operator * (
            K<Free<Fun>, A> ma,
            Func<A, B, C> f) =>
            curry(f) * ma;
    }
        
    extension<Fun, A, B, C, D>(K<Free<Fun>, A> _)
        where Fun : Functor<Fun>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Free<Fun, Func<B, Func<C, D>>> operator * (
            Func<A, B, C, D> f, 
            K<Free<Fun>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Free<Fun, Func<B, Func<C, D>>> operator * (
            K<Free<Fun>, A> ma,
            Func<A, B, C, D> f) =>
            curry(f) * ma;
    }
            
    extension<Fun, A, B, C, D, E>(K<Free<Fun>, A> _)
        where Fun : Functor<Fun>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Free<Fun, Func<B, Func<C, Func<D, E>>>> operator * (
            Func<A, B, C, D, E> f, 
            K<Free<Fun>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Free<Fun, Func<B, Func<C, Func<D, E>>>> operator * (
            K<Free<Fun>, A> ma,
            Func<A, B, C, D, E> f) =>
            curry(f) * ma;
    }
                
    extension<Fun, A, B, C, D, E, F>(K<Free<Fun>, A> _)
        where Fun : Functor<Fun>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Free<Fun, Func<B, Func<C, Func<D, Func<E, F>>>>> operator * (
            Func<A, B, C, D, E, F> f, 
            K<Free<Fun>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Free<Fun, Func<B, Func<C, Func<D, Func<E, F>>>>> operator * (
            K<Free<Fun>, A> ma,
            Func<A, B, C, D, E, F> f) =>
            curry(f) * ma;
    }
                    
    extension<Fun, A, B, C, D, E, F, G>(K<Free<Fun>, A> _)
        where Fun : Functor<Fun>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Free<Fun, Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> operator * (
            Func<A, B, C, D, E, F, G> f, 
            K<Free<Fun>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Free<Fun, Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> operator * (
            K<Free<Fun>, A> ma,
            Func<A, B, C, D, E, F, G> f) =>
            curry(f) * ma;
    }    
                        
    extension<Fun, A, B, C, D, E, F, G, H>(K<Free<Fun>, A> _)
        where Fun : Functor<Fun>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Free<Fun, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> operator * (
            Func<A, B, C, D, E, F, G, H> f, 
            K<Free<Fun>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Free<Fun, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> operator * (
            K<Free<Fun>, A> ma,
            Func<A, B, C, D, E, F, G, H> f) =>
            curry(f) * ma;
    }
                        
    extension<Fun, A, B, C, D, E, F, G, H, I>(K<Free<Fun>, A> _)
        where Fun : Functor<Fun>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Free<Fun, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> operator * (
            Func<A, B, C, D, E, F, G, H, I> f, 
            K<Free<Fun>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Free<Fun, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> operator * (
            K<Free<Fun>, A> ma,
            Func<A, B, C, D, E, F, G, H, I> f) =>
            curry(f) * ma;
    }    
                        
    extension<Fun, A, B, C, D, E, F, G, H, I, J>(K<Free<Fun>, A> _)
        where Fun : Functor<Fun>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Free<Fun, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> operator * (
            Func<A, B, C, D, E, F, G, H, I, J> f, 
            K<Free<Fun>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Free<Fun, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> operator * (
            K<Free<Fun>, A> ma,
            Func<A, B, C, D, E, F, G, H, I, J> f) =>
            curry(f) * ma;
    }
                            
    extension<Fun, A, B, C, D, E, F, G, H, I, J, K>(K<Free<Fun>, A> _)
        where Fun : Functor<Fun>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Free<Fun, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> operator * (
            Func<A, B, C, D, E, F, G, H, I, J, K> f, 
            K<Free<Fun>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Free<Fun, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> operator * (
            K<Free<Fun>, A> ma,
            Func<A, B, C, D, E, F, G, H, I, J, K> f) =>
            curry(f) * ma;
    }
}
