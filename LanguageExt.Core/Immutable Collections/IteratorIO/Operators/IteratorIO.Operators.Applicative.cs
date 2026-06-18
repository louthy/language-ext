using System;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static partial class IteratorIOExtensions
{
    extension<A, B>(K<IteratorIO, A> self)
    {
        
        /// <summary>
        /// Applicative sequence operator
        /// </summary>
        public static IteratorIO<B> operator >>> (K<IteratorIO, A> ma, K<IteratorIO, B> mb) =>
            ma.Action(mb).As();
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static IteratorIO<B> operator * (K<IteratorIO, Func<A, B>> mf, K<IteratorIO, A> ma) =>
            +mf.Apply(ma);
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static IteratorIO<B> operator * (K<IteratorIO, A> ma, K<IteratorIO, Func<A, B>> mf) =>
            +mf.Apply(ma);        
    }
    
    extension<A, B, C>(K<IteratorIO, A> self)
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static IteratorIO<Func<B, C>> operator * (
            K<IteratorIO, Func<A, B, C>> mf, 
            K<IteratorIO, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static IteratorIO<Func<B, C>> operator * (
            K<IteratorIO, A> ma,
            K<IteratorIO, Func<A, B, C>> mf) =>
            curry * mf * ma;
    }
        
    extension<A, B, C, D>(K<IteratorIO, A> self)
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static IteratorIO<Func<B, Func<C, D>>> operator * (
            K<IteratorIO, Func<A, B, C, D>> mf, 
            K<IteratorIO, A> ma) =>
            curry * mf * ma;

        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static IteratorIO<Func<B, Func<C, D>>> operator * (
            K<IteratorIO, A> ma,
            K<IteratorIO, Func<A, B, C, D>> mf) =>
            curry * mf * ma;
    }
            
    extension<A, B, C, D, E>(K<IteratorIO, A> self)
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static IteratorIO<Func<B, Func<C, Func<D, E>>>> operator * (
            K<IteratorIO, Func<A, B, C, D, E>> mf, 
            K<IteratorIO, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static IteratorIO<Func<B, Func<C, Func<D, E>>>> operator * (
            K<IteratorIO, A> ma,
            K<IteratorIO, Func<A, B, C, D, E>> mf) =>
            curry * mf * ma;
    }
                
    extension<A, B, C, D, E, F>(K<IteratorIO, A> self)
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static IteratorIO<Func<B, Func<C, Func<D, Func<E, F>>>>> operator * (
            K<IteratorIO, Func<A, B, C, D, E, F>> mf, 
            K<IteratorIO, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static IteratorIO<Func<B, Func<C, Func<D, Func<E, F>>>>> operator * (
            K<IteratorIO, A> ma,
            K<IteratorIO, Func<A, B, C, D, E, F>> mf) =>
            curry * mf * ma;
    }
                    
    extension<A, B, C, D, E, F, G>(K<IteratorIO, A> self)
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static IteratorIO<Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> operator * (
            K<IteratorIO, Func<A, B, C, D, E, F, G>> mf, 
            K<IteratorIO, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static IteratorIO<Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> operator * (
            K<IteratorIO, A> ma,
            K<IteratorIO, Func<A, B, C, D, E, F, G>> mf) =>
            curry * mf * ma;
    }
                    
    extension<A, B, C, D, E, F, G, H>(K<IteratorIO, A> self)
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static IteratorIO<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> operator * (
            K<IteratorIO, Func<A, B, C, D, E, F, G, H>> mf, 
            K<IteratorIO, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static IteratorIO<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> operator * (
            K<IteratorIO, A> ma,
            K<IteratorIO, Func<A, B, C, D, E, F, G, H>> mf) =>
            curry * mf * ma;
    }
                        
    extension<A, B, C, D, E, F, G, H, I>(K<IteratorIO, A> self)
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static IteratorIO<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> operator * (
            K<IteratorIO, Func<A, B, C, D, E, F, G, H, I>> mf, 
            K<IteratorIO, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static IteratorIO<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> operator * (
            K<IteratorIO, A> ma,
            K<IteratorIO, Func<A, B, C, D, E, F, G, H, I>> mf) =>
            curry * mf * ma;
    }
                            
    extension<A, B, C, D, E, F, G, H, I, J>(K<IteratorIO, A> self)
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static IteratorIO<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> operator * (
            K<IteratorIO, Func<A, B, C, D, E, F, G, H, I, J>> mf, 
            K<IteratorIO, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static IteratorIO<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> operator * (
            K<IteratorIO, A> ma,
            K<IteratorIO, Func<A, B, C, D, E, F, G, H, I, J>> mf) =>
            curry * mf * ma;
    }
                                
    extension<A, B, C, D, E, F, G, H, I, J, K>(K<IteratorIO, A> self)
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static IteratorIO<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> operator * (
            K<IteratorIO, Func<A, B, C, D, E, F, G, H, I, J, K>> mf, 
            K<IteratorIO, A> ma) =>
            curry * mf * ma;

        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static IteratorIO<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> operator *(
            K<IteratorIO, A> ma,
            K<IteratorIO, Func<A, B, C, D, E, F, G, H, I, J, K>> mf) =>
            curry * mf * ma;
    }
}
