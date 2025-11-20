using System;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static partial class TryExtensions
{
    extension<A, B>(K<Try, A> self)
    {
        
        /// <summary>
        /// Applicative sequence operator
        /// </summary>
        public static Try<B> operator >>> (K<Try, A> ma, K<Try, B> mb) =>
            ma.Action(mb).As();
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Try<B> operator * (K<Try, Func<A, B>> mf, K<Try, A> ma) =>
            mf.Apply(ma);
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Try<B> operator * (K<Try, A> ma, K<Try, Func<A, B>> mf) =>
            mf.Apply(ma);        
    }
    
    extension<A, B, C>(K<Try, A> self)
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Try<Func<B, C>> operator * (
            K<Try, Func<A, B, C>> mf, 
            K<Try, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Try<Func<B, C>> operator * (
            K<Try, A> ma,
            K<Try, Func<A, B, C>> mf) =>
            curry * mf * ma;
    }
        
    extension<A, B, C, D>(K<Try, A> self)
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Try<Func<B, Func<C, D>>> operator * (
            K<Try, Func<A, B, C, D>> mf, 
            K<Try, A> ma) =>
            curry * mf * ma;

        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Try<Func<B, Func<C, D>>> operator * (
            K<Try, A> ma,
            K<Try, Func<A, B, C, D>> mf) =>
            curry * mf * ma;
    }
            
    extension<A, B, C, D, E>(K<Try, A> self)
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Try<Func<B, Func<C, Func<D, E>>>> operator * (
            K<Try, Func<A, B, C, D, E>> mf, 
            K<Try, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Try<Func<B, Func<C, Func<D, E>>>> operator * (
            K<Try, A> ma,
            K<Try, Func<A, B, C, D, E>> mf) =>
            curry * mf * ma;
    }
                
    extension<A, B, C, D, E, F>(K<Try, A> self)
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Try<Func<B, Func<C, Func<D, Func<E, F>>>>> operator * (
            K<Try, Func<A, B, C, D, E, F>> mf, 
            K<Try, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Try<Func<B, Func<C, Func<D, Func<E, F>>>>> operator * (
            K<Try, A> ma,
            K<Try, Func<A, B, C, D, E, F>> mf) =>
            curry * mf * ma;
    }
                    
    extension<A, B, C, D, E, F, G>(K<Try, A> self)
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Try<Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> operator * (
            K<Try, Func<A, B, C, D, E, F, G>> mf, 
            K<Try, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Try<Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> operator * (
            K<Try, A> ma,
            K<Try, Func<A, B, C, D, E, F, G>> mf) =>
            curry * mf * ma;
    }
                    
    extension<A, B, C, D, E, F, G, H>(K<Try, A> self)
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Try<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> operator * (
            K<Try, Func<A, B, C, D, E, F, G, H>> mf, 
            K<Try, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Try<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> operator * (
            K<Try, A> ma,
            K<Try, Func<A, B, C, D, E, F, G, H>> mf) =>
            curry * mf * ma;
    }
                        
    extension<A, B, C, D, E, F, G, H, I>(K<Try, A> self)
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Try<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> operator * (
            K<Try, Func<A, B, C, D, E, F, G, H, I>> mf, 
            K<Try, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Try<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> operator * (
            K<Try, A> ma,
            K<Try, Func<A, B, C, D, E, F, G, H, I>> mf) =>
            curry * mf * ma;
    }
                            
    extension<A, B, C, D, E, F, G, H, I, J>(K<Try, A> self)
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Try<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> operator * (
            K<Try, Func<A, B, C, D, E, F, G, H, I, J>> mf, 
            K<Try, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Try<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> operator * (
            K<Try, A> ma,
            K<Try, Func<A, B, C, D, E, F, G, H, I, J>> mf) =>
            curry * mf * ma;
    }
                                
    extension<A, B, C, D, E, F, G, H, I, J, K>(K<Try, A> self)
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Try<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> operator * (
            K<Try, Func<A, B, C, D, E, F, G, H, I, J, K>> mf, 
            K<Try, A> ma) =>
            curry * mf * ma;

        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Try<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> operator *(
            K<Try, A> ma,
            K<Try, Func<A, B, C, D, E, F, G, H, I, J, K>> mf) =>
            curry * mf * ma;
    }
}
