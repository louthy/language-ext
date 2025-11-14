using System;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static partial class OptionExtensions
{
    extension<A, B>(K<Option, A> self)
    {
        
        /// <summary>
        /// Applicative sequence operator
        /// </summary>
        public static Option<B> operator >>> (K<Option, A> ma, K<Option, B> mb) =>
            ma.Action(mb).As();
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Option<B> operator * (K<Option, Func<A, B>> mf, K<Option, A> ma) =>
            mf.Apply(ma);
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Option<B> operator * (K<Option, A> ma, K<Option, Func<A, B>> mf) =>
            mf.Apply(ma);        
    }
    
    extension<A, B, C>(K<Option, A> self)
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Option<Func<B, C>> operator * (
            K<Option, Func<A, B, C>> mf, 
            K<Option, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Option<Func<B, C>> operator * (
            K<Option, A> ma,
            K<Option, Func<A, B, C>> mf) =>
            curry * mf * ma;
    }
        
    extension<A, B, C, D>(K<Option, A> self)
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Option<Func<B, Func<C, D>>> operator * (
            K<Option, Func<A, B, C, D>> mf, 
            K<Option, A> ma) =>
            curry * mf * ma;

        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Option<Func<B, Func<C, D>>> operator * (
            K<Option, A> ma,
            K<Option, Func<A, B, C, D>> mf) =>
            curry * mf * ma;
    }
            
    extension<A, B, C, D, E>(K<Option, A> self)
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Option<Func<B, Func<C, Func<D, E>>>> operator * (
            K<Option, Func<A, B, C, D, E>> mf, 
            K<Option, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Option<Func<B, Func<C, Func<D, E>>>> operator * (
            K<Option, A> ma,
            K<Option, Func<A, B, C, D, E>> mf) =>
            curry * mf * ma;
    }
                
    extension<A, B, C, D, E, F>(K<Option, A> self)
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Option<Func<B, Func<C, Func<D, Func<E, F>>>>> operator * (
            K<Option, Func<A, B, C, D, E, F>> mf, 
            K<Option, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Option<Func<B, Func<C, Func<D, Func<E, F>>>>> operator * (
            K<Option, A> ma,
            K<Option, Func<A, B, C, D, E, F>> mf) =>
            curry * mf * ma;
    }
                    
    extension<A, B, C, D, E, F, G>(K<Option, A> self)
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Option<Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> operator * (
            K<Option, Func<A, B, C, D, E, F, G>> mf, 
            K<Option, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Option<Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> operator * (
            K<Option, A> ma,
            K<Option, Func<A, B, C, D, E, F, G>> mf) =>
            curry * mf * ma;
    }
                    
    extension<A, B, C, D, E, F, G, H>(K<Option, A> self)
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Option<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> operator * (
            K<Option, Func<A, B, C, D, E, F, G, H>> mf, 
            K<Option, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Option<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> operator * (
            K<Option, A> ma,
            K<Option, Func<A, B, C, D, E, F, G, H>> mf) =>
            curry * mf * ma;
    }
                        
    extension<A, B, C, D, E, F, G, H, I>(K<Option, A> self)
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Option<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> operator * (
            K<Option, Func<A, B, C, D, E, F, G, H, I>> mf, 
            K<Option, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Option<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> operator * (
            K<Option, A> ma,
            K<Option, Func<A, B, C, D, E, F, G, H, I>> mf) =>
            curry * mf * ma;
    }
                            
    extension<A, B, C, D, E, F, G, H, I, J>(K<Option, A> self)
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Option<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> operator * (
            K<Option, Func<A, B, C, D, E, F, G, H, I, J>> mf, 
            K<Option, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Option<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> operator * (
            K<Option, A> ma,
            K<Option, Func<A, B, C, D, E, F, G, H, I, J>> mf) =>
            curry * mf * ma;
    }
                                
    extension<A, B, C, D, E, F, G, H, I, J, K>(K<Option, A> self)
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Option<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> operator * (
            K<Option, Func<A, B, C, D, E, F, G, H, I, J, K>> mf, 
            K<Option, A> ma) =>
            curry * mf * ma;

        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Option<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> operator *(
            K<Option, A> ma,
            K<Option, Func<A, B, C, D, E, F, G, H, I, J, K>> mf) =>
            curry * mf * ma;
    }
}
