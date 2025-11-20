using System;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static partial class FreeExtensions
{
    extension<Fun, A, B>(K<Free<Fun>, A> self)
        where Fun : Functor<Fun>
    {
        
        /// <summary>
        /// Applicative sequence operator
        /// </summary>
        public static Free<Fun, B> operator >>> (K<Free<Fun>, A> ma, K<Free<Fun>, B> mb) =>
            +ma.Action(mb);
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Free<Fun, B> operator * (K<Free<Fun>, Func<A, B>> mf, K<Free<Fun>, A> ma) =>
            mf.Apply(ma);
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Free<Fun, B> operator * (K<Free<Fun>, A> ma, K<Free<Fun>, Func<A, B>> mf) =>
            mf.Apply(ma);        
    }
    
    extension<Fun, A, B, C>(K<Free<Fun>, A> self)
        where Fun : Functor<Fun>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Free<Fun, Func<B, C>> operator * (
            K<Free<Fun>, Func<A, B, C>> mf, 
            K<Free<Fun>, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Free<Fun, Func<B, C>> operator * (
            K<Free<Fun>, A> ma,
            K<Free<Fun>, Func<A, B, C>> mf) =>
            curry * mf * ma;
    }
        
    extension<Fun, A, B, C, D>(K<Free<Fun>, A> self)
        where Fun : Functor<Fun>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Free<Fun, Func<B, Func<C, D>>> operator * (
            K<Free<Fun>, Func<A, B, C, D>> mf, 
            K<Free<Fun>, A> ma) =>
            curry * mf * ma;

        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Free<Fun, Func<B, Func<C, D>>> operator * (
            K<Free<Fun>, A> ma,
            K<Free<Fun>, Func<A, B, C, D>> mf) =>
            curry * mf * ma;
    }
            
    extension<Fun, A, B, C, D, E>(K<Free<Fun>, A> self)
        where Fun : Functor<Fun>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Free<Fun, Func<B, Func<C, Func<D, E>>>> operator * (
            K<Free<Fun>, Func<A, B, C, D, E>> mf, 
            K<Free<Fun>, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Free<Fun, Func<B, Func<C, Func<D, E>>>> operator * (
            K<Free<Fun>, A> ma,
            K<Free<Fun>, Func<A, B, C, D, E>> mf) =>
            curry * mf * ma;
    }
                
    extension<Fun, A, B, C, D, E, F>(K<Free<Fun>, A> self)
        where Fun : Functor<Fun>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Free<Fun, Func<B, Func<C, Func<D, Func<E, F>>>>> operator * (
            K<Free<Fun>, Func<A, B, C, D, E, F>> mf, 
            K<Free<Fun>, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Free<Fun, Func<B, Func<C, Func<D, Func<E, F>>>>> operator * (
            K<Free<Fun>, A> ma,
            K<Free<Fun>, Func<A, B, C, D, E, F>> mf) =>
            curry * mf * ma;
    }
                    
    extension<Fun, A, B, C, D, E, F, G>(K<Free<Fun>, A> self)
        where Fun : Functor<Fun>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Free<Fun, Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> operator * (
            K<Free<Fun>, Func<A, B, C, D, E, F, G>> mf, 
            K<Free<Fun>, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Free<Fun, Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> operator * (
            K<Free<Fun>, A> ma,
            K<Free<Fun>, Func<A, B, C, D, E, F, G>> mf) =>
            curry * mf * ma;
    }
                    
    extension<Fun, A, B, C, D, E, F, G, H>(K<Free<Fun>, A> self)
        where Fun : Functor<Fun>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Free<Fun, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> operator * (
            K<Free<Fun>, Func<A, B, C, D, E, F, G, H>> mf, 
            K<Free<Fun>, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Free<Fun, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> operator * (
            K<Free<Fun>, A> ma,
            K<Free<Fun>, Func<A, B, C, D, E, F, G, H>> mf) =>
            curry * mf * ma;
    }
                        
    extension<Fun, A, B, C, D, E, F, G, H, I>(K<Free<Fun>, A> self)
        where Fun : Functor<Fun>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Free<Fun, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> operator * (
            K<Free<Fun>, Func<A, B, C, D, E, F, G, H, I>> mf, 
            K<Free<Fun>, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Free<Fun, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> operator * (
            K<Free<Fun>, A> ma,
            K<Free<Fun>, Func<A, B, C, D, E, F, G, H, I>> mf) =>
            curry * mf * ma;
    }
                            
    extension<Fun, A, B, C, D, E, F, G, H, I, J>(K<Free<Fun>, A> self)
        where Fun : Functor<Fun>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Free<Fun, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> operator * (
            K<Free<Fun>, Func<A, B, C, D, E, F, G, H, I, J>> mf, 
            K<Free<Fun>, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Free<Fun, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> operator * (
            K<Free<Fun>, A> ma,
            K<Free<Fun>, Func<A, B, C, D, E, F, G, H, I, J>> mf) =>
            curry * mf * ma;
    }
                                
    extension<Fun, A, B, C, D, E, F, G, H, I, J, K>(K<Free<Fun>, A> self)
        where Fun : Functor<Fun>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Free<Fun, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> operator * (
            K<Free<Fun>, Func<A, B, C, D, E, F, G, H, I, J, K>> mf, 
            K<Free<Fun>, A> ma) =>
            curry * mf * ma;

        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Free<Fun, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> operator *(
            K<Free<Fun>, A> ma,
            K<Free<Fun>, Func<A, B, C, D, E, F, G, H, I, J, K>> mf) =>
            curry * mf * ma;
    }
}
