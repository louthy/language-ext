using System;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static partial class IteratorExtensions
{
    extension<A, B>(K<Iterator, A> self)
    {
        
        /// <summary>
        /// Applicative sequence operator
        /// </summary>
        public static Iterator<B> operator >>> (K<Iterator, A> ma, K<Iterator, B> mb) =>
            ma.Action(mb).As();
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Iterator<B> operator * (K<Iterator, Func<A, B>> mf, K<Iterator, A> ma) =>
            +mf.Apply(ma);
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Iterator<B> operator * (K<Iterator, A> ma, K<Iterator, Func<A, B>> mf) =>
            +mf.Apply(ma);        
    }
    
    extension<A, B, C>(K<Iterator, A> self)
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Iterator<Func<B, C>> operator * (
            K<Iterator, Func<A, B, C>> mf, 
            K<Iterator, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Iterator<Func<B, C>> operator * (
            K<Iterator, A> ma,
            K<Iterator, Func<A, B, C>> mf) =>
            curry * mf * ma;
    }
        
    extension<A, B, C, D>(K<Iterator, A> self)
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Iterator<Func<B, Func<C, D>>> operator * (
            K<Iterator, Func<A, B, C, D>> mf, 
            K<Iterator, A> ma) =>
            curry * mf * ma;

        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Iterator<Func<B, Func<C, D>>> operator * (
            K<Iterator, A> ma,
            K<Iterator, Func<A, B, C, D>> mf) =>
            curry * mf * ma;
    }
            
    extension<A, B, C, D, E>(K<Iterator, A> self)
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Iterator<Func<B, Func<C, Func<D, E>>>> operator * (
            K<Iterator, Func<A, B, C, D, E>> mf, 
            K<Iterator, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Iterator<Func<B, Func<C, Func<D, E>>>> operator * (
            K<Iterator, A> ma,
            K<Iterator, Func<A, B, C, D, E>> mf) =>
            curry * mf * ma;
    }
                
    extension<A, B, C, D, E, F>(K<Iterator, A> self)
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Iterator<Func<B, Func<C, Func<D, Func<E, F>>>>> operator * (
            K<Iterator, Func<A, B, C, D, E, F>> mf, 
            K<Iterator, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Iterator<Func<B, Func<C, Func<D, Func<E, F>>>>> operator * (
            K<Iterator, A> ma,
            K<Iterator, Func<A, B, C, D, E, F>> mf) =>
            curry * mf * ma;
    }
                    
    extension<A, B, C, D, E, F, G>(K<Iterator, A> self)
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Iterator<Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> operator * (
            K<Iterator, Func<A, B, C, D, E, F, G>> mf, 
            K<Iterator, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Iterator<Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> operator * (
            K<Iterator, A> ma,
            K<Iterator, Func<A, B, C, D, E, F, G>> mf) =>
            curry * mf * ma;
    }
                    
    extension<A, B, C, D, E, F, G, H>(K<Iterator, A> self)
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Iterator<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> operator * (
            K<Iterator, Func<A, B, C, D, E, F, G, H>> mf, 
            K<Iterator, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Iterator<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> operator * (
            K<Iterator, A> ma,
            K<Iterator, Func<A, B, C, D, E, F, G, H>> mf) =>
            curry * mf * ma;
    }
                        
    extension<A, B, C, D, E, F, G, H, I>(K<Iterator, A> self)
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Iterator<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> operator * (
            K<Iterator, Func<A, B, C, D, E, F, G, H, I>> mf, 
            K<Iterator, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Iterator<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> operator * (
            K<Iterator, A> ma,
            K<Iterator, Func<A, B, C, D, E, F, G, H, I>> mf) =>
            curry * mf * ma;
    }
                            
    extension<A, B, C, D, E, F, G, H, I, J>(K<Iterator, A> self)
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Iterator<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> operator * (
            K<Iterator, Func<A, B, C, D, E, F, G, H, I, J>> mf, 
            K<Iterator, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Iterator<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> operator * (
            K<Iterator, A> ma,
            K<Iterator, Func<A, B, C, D, E, F, G, H, I, J>> mf) =>
            curry * mf * ma;
    }
                                
    extension<A, B, C, D, E, F, G, H, I, J, K>(K<Iterator, A> self)
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Iterator<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> operator * (
            K<Iterator, Func<A, B, C, D, E, F, G, H, I, J, K>> mf, 
            K<Iterator, A> ma) =>
            curry * mf * ma;

        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Iterator<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> operator *(
            K<Iterator, A> ma,
            K<Iterator, Func<A, B, C, D, E, F, G, H, I, J, K>> mf) =>
            curry * mf * ma;
    }
}
