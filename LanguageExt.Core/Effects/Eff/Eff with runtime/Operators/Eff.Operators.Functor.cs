using System;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static partial class EffExtensions
{
    extension<RT, A, B>(K<Eff<RT>, A> _)
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Eff<RT, B> operator *(Func<A, B> f, K<Eff<RT>, A> ma) =>
            ma.Map(f).As();
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Eff<RT, B> operator *(K<Eff<RT>, A> ma, Func<A, B> f) =>
            ma.Map(f).As();
    }
    
    extension<RT, A, B, C>(K<Eff<RT>, A> _)
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Eff<RT, Func<B, C>> operator * (
            Func<A, B, C> f, 
            K<Eff<RT>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Eff<RT, Func<B, C>> operator * (
            K<Eff<RT>, A> ma,
            Func<A, B, C> f) =>
            curry(f) * ma;
    }
        
    extension<RT, A, B, C, D>(K<Eff<RT>, A> _)
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Eff<RT, Func<B, Func<C, D>>> operator * (
            Func<A, B, C, D> f, 
            K<Eff<RT>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Eff<RT, Func<B, Func<C, D>>> operator * (
            K<Eff<RT>, A> ma,
            Func<A, B, C, D> f) =>
            curry(f) * ma;
    }
            
    extension<RT, A, B, C, D, E>(K<Eff<RT>, A> _)
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Eff<RT, Func<B, Func<C, Func<D, E>>>> operator * (
            Func<A, B, C, D, E> f, 
            K<Eff<RT>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Eff<RT, Func<B, Func<C, Func<D, E>>>> operator * (
            K<Eff<RT>, A> ma,
            Func<A, B, C, D, E> f) =>
            curry(f) * ma;
    }
                
    extension<RT, A, B, C, D, E, F>(K<Eff<RT>, A> _)
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Eff<RT, Func<B, Func<C, Func<D, Func<E, F>>>>> operator * (
            Func<A, B, C, D, E, F> f, 
            K<Eff<RT>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Eff<RT, Func<B, Func<C, Func<D, Func<E, F>>>>> operator * (
            K<Eff<RT>, A> ma,
            Func<A, B, C, D, E, F> f) =>
            curry(f) * ma;
    }
                    
    extension<RT, A, B, C, D, E, F, G>(K<Eff<RT>, A> _)
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Eff<RT, Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> operator * (
            Func<A, B, C, D, E, F, G> f, 
            K<Eff<RT>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Eff<RT, Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> operator * (
            K<Eff<RT>, A> ma,
            Func<A, B, C, D, E, F, G> f) =>
            curry(f) * ma;
    }    
                        
    extension<RT, A, B, C, D, E, F, G, H>(K<Eff<RT>, A> _)
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Eff<RT, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> operator * (
            Func<A, B, C, D, E, F, G, H> f, 
            K<Eff<RT>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Eff<RT, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> operator * (
            K<Eff<RT>, A> ma,
            Func<A, B, C, D, E, F, G, H> f) =>
            curry(f) * ma;
    }
                        
    extension<RT, A, B, C, D, E, F, G, H, I>(K<Eff<RT>, A> _)
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Eff<RT, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> operator * (
            Func<A, B, C, D, E, F, G, H, I> f, 
            K<Eff<RT>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Eff<RT, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> operator * (
            K<Eff<RT>, A> ma,
            Func<A, B, C, D, E, F, G, H, I> f) =>
            curry(f) * ma;
    }    
                        
    extension<RT, A, B, C, D, E, F, G, H, I, J>(K<Eff<RT>, A> _)
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Eff<RT, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> operator * (
            Func<A, B, C, D, E, F, G, H, I, J> f, 
            K<Eff<RT>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Eff<RT, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> operator * (
            K<Eff<RT>, A> ma,
            Func<A, B, C, D, E, F, G, H, I, J> f) =>
            curry(f) * ma;
    }
                            
    extension<RT, A, B, C, D, E, F, G, H, I, J, K>(K<Eff<RT>, A> _)
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Eff<RT, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> operator * (
            Func<A, B, C, D, E, F, G, H, I, J, K> f, 
            K<Eff<RT>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Eff<RT, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> operator * (
            K<Eff<RT>, A> ma,
            Func<A, B, C, D, E, F, G, H, I, J, K> f) =>
            curry(f) * ma;
    }
}
