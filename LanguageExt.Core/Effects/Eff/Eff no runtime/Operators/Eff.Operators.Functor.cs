using System;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static partial class EffExtensions
{
    extension<A, B>(K<Eff, A> _)
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Eff<B> operator *(Func<A, B> f, K<Eff, A> ma) =>
            ma.Map(f).As();
    }
    
    extension<A, B, C>(K<Eff, A> _)
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Eff<Func<B, C>> operator * (
            Func<A, B, C> f, 
            K<Eff, A> ma) =>
            curry(f) * ma;
    }
        
    extension<A, B, C, D>(K<Eff, A> _)
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Eff<Func<B, Func<C, D>>> operator * (
            Func<A, B, C, D> f, 
            K<Eff, A> ma) =>
            curry(f) * ma;
    }
            
    extension<A, B, C, D, E>(K<Eff, A> _)
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Eff<Func<B, Func<C, Func<D, E>>>> operator * (
            Func<A, B, C, D, E> f, 
            K<Eff, A> ma) =>
            curry(f) * ma;
    }
                
    extension<A, B, C, D, E, F>(K<Eff, A> _)
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Eff<Func<B, Func<C, Func<D, Func<E, F>>>>> operator * (
            Func<A, B, C, D, E, F> f, 
            K<Eff, A> ma) =>
            curry(f) * ma;
    }
                    
    extension<A, B, C, D, E, F, G>(K<Eff, A> _)
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Eff<Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> operator * (
            Func<A, B, C, D, E, F, G> f, 
            K<Eff, A> ma) =>
            curry(f) * ma;
    }    
                        
    extension<A, B, C, D, E, F, G, H>(K<Eff, A> _)
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Eff<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> operator * (
            Func<A, B, C, D, E, F, G, H> f, 
            K<Eff, A> ma) =>
            curry(f) * ma;
    }
                        
    extension<A, B, C, D, E, F, G, H, I>(K<Eff, A> _)
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Eff<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> operator * (
            Func<A, B, C, D, E, F, G, H, I> f, 
            K<Eff, A> ma) =>
            curry(f) * ma;
    }    
                        
    extension<A, B, C, D, E, F, G, H, I, J>(K<Eff, A> _)
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Eff<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> operator * (
            Func<A, B, C, D, E, F, G, H, I, J> f, 
            K<Eff, A> ma) =>
            curry(f) * ma;
    }
                            
    extension<A, B, C, D, E, F, G, H, I, J, K>(K<Eff, A> _)
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Eff<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> operator * (
            Func<A, B, C, D, E, F, G, H, I, J, K> f, 
            K<Eff, A> ma) =>
            curry(f) * ma;
    }
}
