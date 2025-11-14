using System;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static partial class IOExtensions
{
    extension<A, B>(K<IO, A> self)
    {
        
        /// <summary>
        /// Applicative sequence operator
        /// </summary>
        public static IO<B> operator >>> (K<IO, A> ma, K<IO, B> mb) =>
            ma.Action(mb).As();
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static IO<B> operator * (K<IO, Func<A, B>> mf, K<IO, A> ma) =>
            mf.Apply(ma);
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static IO<B> operator * (K<IO, A> ma, K<IO, Func<A, B>> mf) =>
            mf.Apply(ma);        
    }
    
    extension<A, B, C>(K<IO, A> self)
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static IO<Func<B, C>> operator * (
            K<IO, Func<A, B, C>> mf, 
            K<IO, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static IO<Func<B, C>> operator * (
            K<IO, A> ma,
            K<IO, Func<A, B, C>> mf) =>
            curry * mf * ma;
    }
        
    extension<A, B, C, D>(K<IO, A> self)
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static IO<Func<B, Func<C, D>>> operator * (
            K<IO, Func<A, B, C, D>> mf, 
            K<IO, A> ma) =>
            curry * mf * ma;

        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static IO<Func<B, Func<C, D>>> operator * (
            K<IO, A> ma,
            K<IO, Func<A, B, C, D>> mf) =>
            curry * mf * ma;
    }
            
    extension<A, B, C, D, E>(K<IO, A> self)
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static IO<Func<B, Func<C, Func<D, E>>>> operator * (
            K<IO, Func<A, B, C, D, E>> mf, 
            K<IO, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static IO<Func<B, Func<C, Func<D, E>>>> operator * (
            K<IO, A> ma,
            K<IO, Func<A, B, C, D, E>> mf) =>
            curry * mf * ma;
    }
                
    extension<A, B, C, D, E, F>(K<IO, A> self)
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static IO<Func<B, Func<C, Func<D, Func<E, F>>>>> operator * (
            K<IO, Func<A, B, C, D, E, F>> mf, 
            K<IO, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static IO<Func<B, Func<C, Func<D, Func<E, F>>>>> operator * (
            K<IO, A> ma,
            K<IO, Func<A, B, C, D, E, F>> mf) =>
            curry * mf * ma;
    }
                    
    extension<A, B, C, D, E, F, G>(K<IO, A> self)
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static IO<Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> operator * (
            K<IO, Func<A, B, C, D, E, F, G>> mf, 
            K<IO, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static IO<Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> operator * (
            K<IO, A> ma,
            K<IO, Func<A, B, C, D, E, F, G>> mf) =>
            curry * mf * ma;
    }
                    
    extension<A, B, C, D, E, F, G, H>(K<IO, A> self)
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static IO<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> operator * (
            K<IO, Func<A, B, C, D, E, F, G, H>> mf, 
            K<IO, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static IO<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> operator * (
            K<IO, A> ma,
            K<IO, Func<A, B, C, D, E, F, G, H>> mf) =>
            curry * mf * ma;
    }
                        
    extension<A, B, C, D, E, F, G, H, I>(K<IO, A> self)
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static IO<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> operator * (
            K<IO, Func<A, B, C, D, E, F, G, H, I>> mf, 
            K<IO, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static IO<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> operator * (
            K<IO, A> ma,
            K<IO, Func<A, B, C, D, E, F, G, H, I>> mf) =>
            curry * mf * ma;
    }
                            
    extension<A, B, C, D, E, F, G, H, I, J>(K<IO, A> self)
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static IO<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> operator * (
            K<IO, Func<A, B, C, D, E, F, G, H, I, J>> mf, 
            K<IO, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static IO<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> operator * (
            K<IO, A> ma,
            K<IO, Func<A, B, C, D, E, F, G, H, I, J>> mf) =>
            curry * mf * ma;
    }
                                
    extension<A, B, C, D, E, F, G, H, I, J, K>(K<IO, A> self)
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static IO<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> operator * (
            K<IO, Func<A, B, C, D, E, F, G, H, I, J, K>> mf, 
            K<IO, A> ma) =>
            curry * mf * ma;

        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static IO<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> operator *(
            K<IO, A> ma,
            K<IO, Func<A, B, C, D, E, F, G, H, I, J, K>> mf) =>
            curry * mf * ma;
    }
}
