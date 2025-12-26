using System;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static partial class SourceExtensions
{
    extension<A, B>(K<Source, A> self)
    {
        /// <summary>
        /// Applicative sequence operator
        /// </summary>
        public static Source<B> operator >>> (K<Source, A> ma, K<Source, B> mb) =>
            +ma.Action(mb);
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Source<B> operator * (K<Source, Func<A, B>> mf, K<Source, A> ma) =>
            +mf.Apply(ma);
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Source<B> operator * (K<Source, A> ma, K<Source, Func<A, B>> mf) =>
            +mf.Apply(ma);        
    }
    
    extension<A, B, C>(K<Source, A> self)
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Source<Func<B, C>> operator * (
            K<Source, Func<A, B, C>> mf, 
            K<Source, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Source<Func<B, C>> operator * (
            K<Source, A> ma,
            K<Source, Func<A, B, C>> mf) =>
            curry * mf * ma;
    }
        
    extension<A, B, C, D>(K<Source, A> self)
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Source<Func<B, Func<C, D>>> operator * (
            K<Source, Func<A, B, C, D>> mf, 
            K<Source, A> ma) =>
            curry * mf * ma;

        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Source<Func<B, Func<C, D>>> operator * (
            K<Source, A> ma,
            K<Source, Func<A, B, C, D>> mf) =>
            curry * mf * ma;
    }
            
    extension<A, B, C, D, E>(K<Source, A> self)
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Source<Func<B, Func<C, Func<D, E>>>> operator * (
            K<Source, Func<A, B, C, D, E>> mf, 
            K<Source, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Source<Func<B, Func<C, Func<D, E>>>> operator * (
            K<Source, A> ma,
            K<Source, Func<A, B, C, D, E>> mf) =>
            curry * mf * ma;
    }
                
    extension<A, B, C, D, E, F>(K<Source, A> self)
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Source<Func<B, Func<C, Func<D, Func<E, F>>>>> operator * (
            K<Source, Func<A, B, C, D, E, F>> mf, 
            K<Source, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Source<Func<B, Func<C, Func<D, Func<E, F>>>>> operator * (
            K<Source, A> ma,
            K<Source, Func<A, B, C, D, E, F>> mf) =>
            curry * mf * ma;
    }
                    
    extension<A, B, C, D, E, F, G>(K<Source, A> self)
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Source<Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> operator * (
            K<Source, Func<A, B, C, D, E, F, G>> mf, 
            K<Source, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Source<Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> operator * (
            K<Source, A> ma,
            K<Source, Func<A, B, C, D, E, F, G>> mf) =>
            curry * mf * ma;
    }
                    
    extension<A, B, C, D, E, F, G, H>(K<Source, A> self)
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Source<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> operator * (
            K<Source, Func<A, B, C, D, E, F, G, H>> mf, 
            K<Source, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Source<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> operator * (
            K<Source, A> ma,
            K<Source, Func<A, B, C, D, E, F, G, H>> mf) =>
            curry * mf * ma;
    }
                        
    extension<A, B, C, D, E, F, G, H, I>(K<Source, A> self)
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Source<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> operator * (
            K<Source, Func<A, B, C, D, E, F, G, H, I>> mf, 
            K<Source, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Source<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> operator * (
            K<Source, A> ma,
            K<Source, Func<A, B, C, D, E, F, G, H, I>> mf) =>
            curry * mf * ma;
    }
                            
    extension<A, B, C, D, E, F, G, H, I, J>(K<Source, A> self)
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Source<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> operator * (
            K<Source, Func<A, B, C, D, E, F, G, H, I, J>> mf, 
            K<Source, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Source<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> operator * (
            K<Source, A> ma,
            K<Source, Func<A, B, C, D, E, F, G, H, I, J>> mf) =>
            curry * mf * ma;
    }
                                
    extension<A, B, C, D, E, F, G, H, I, J, K>(K<Source, A> self)
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Source<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> operator * (
            K<Source, Func<A, B, C, D, E, F, G, H, I, J, K>> mf, 
            K<Source, A> ma) =>
            curry * mf * ma;

        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Source<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> operator *(
            K<Source, A> ma,
            K<Source, Func<A, B, C, D, E, F, G, H, I, J, K>> mf) =>
            curry * mf * ma;
    }
}
