using System;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static partial class WriterExtensions
{
    extension<W, A, B>(K<Writer<W>, A> self)
        where W : Monoid<W>
    {
        /// <summary>
        /// Applicative sequence operator
        /// </summary>
        public static Writer<W, B> operator >>> (K<Writer<W>, A> ma, K<Writer<W>, B> mb) =>
            +ma.Action(mb);
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Writer<W, B> operator * (K<Writer<W>, Func<A, B>> mf, K<Writer<W>, A> ma) =>
            +mf.Apply(ma);
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Writer<W, B> operator * (K<Writer<W>, A> ma, K<Writer<W>, Func<A, B>> mf) =>
            +mf.Apply(ma);        
    }
    
    extension<W, A, B, C>(K<Writer<W>, A> self)
        where W : Monoid<W>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Writer<W, Func<B, C>> operator * (
            K<Writer<W>, Func<A, B, C>> mf, 
            K<Writer<W>, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Writer<W, Func<B, C>> operator * (
            K<Writer<W>, A> ma,
            K<Writer<W>, Func<A, B, C>> mf) =>
            curry * mf * ma;
    }
        
    extension<W, A, B, C, D>(K<Writer<W>, A> self)
        where W : Monoid<W>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Writer<W, Func<B, Func<C, D>>> operator * (
            K<Writer<W>, Func<A, B, C, D>> mf, 
            K<Writer<W>, A> ma) =>
            curry * mf * ma;

        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Writer<W, Func<B, Func<C, D>>> operator * (
            K<Writer<W>, A> ma,
            K<Writer<W>, Func<A, B, C, D>> mf) =>
            curry * mf * ma;
    }
            
    extension<W, A, B, C, D, E>(K<Writer<W>, A> self)
        where W : Monoid<W>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Writer<W, Func<B, Func<C, Func<D, E>>>> operator * (
            K<Writer<W>, Func<A, B, C, D, E>> mf, 
            K<Writer<W>, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Writer<W, Func<B, Func<C, Func<D, E>>>> operator * (
            K<Writer<W>, A> ma,
            K<Writer<W>, Func<A, B, C, D, E>> mf) =>
            curry * mf * ma;
    }
                
    extension<W, A, B, C, D, E, F>(K<Writer<W>, A> self)
        where W : Monoid<W>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Writer<W, Func<B, Func<C, Func<D, Func<E, F>>>>> operator * (
            K<Writer<W>, Func<A, B, C, D, E, F>> mf, 
            K<Writer<W>, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Writer<W, Func<B, Func<C, Func<D, Func<E, F>>>>> operator * (
            K<Writer<W>, A> ma,
            K<Writer<W>, Func<A, B, C, D, E, F>> mf) =>
            curry * mf * ma;
    }
                    
    extension<W, A, B, C, D, E, F, G>(K<Writer<W>, A> self)
        where W : Monoid<W>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Writer<W, Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> operator * (
            K<Writer<W>, Func<A, B, C, D, E, F, G>> mf, 
            K<Writer<W>, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Writer<W, Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> operator * (
            K<Writer<W>, A> ma,
            K<Writer<W>, Func<A, B, C, D, E, F, G>> mf) =>
            curry * mf * ma;
    }
                    
    extension<W, A, B, C, D, E, F, G, H>(K<Writer<W>, A> self)
        where W : Monoid<W>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Writer<W, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> operator * (
            K<Writer<W>, Func<A, B, C, D, E, F, G, H>> mf, 
            K<Writer<W>, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Writer<W, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> operator * (
            K<Writer<W>, A> ma,
            K<Writer<W>, Func<A, B, C, D, E, F, G, H>> mf) =>
            curry * mf * ma;
    }
                        
    extension<W, A, B, C, D, E, F, G, H, I>(K<Writer<W>, A> self)
        where W : Monoid<W>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Writer<W, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> operator * (
            K<Writer<W>, Func<A, B, C, D, E, F, G, H, I>> mf, 
            K<Writer<W>, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Writer<W, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> operator * (
            K<Writer<W>, A> ma,
            K<Writer<W>, Func<A, B, C, D, E, F, G, H, I>> mf) =>
            curry * mf * ma;
    }
                            
    extension<W, A, B, C, D, E, F, G, H, I, J>(K<Writer<W>, A> self)
        where W : Monoid<W>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Writer<W, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> operator * (
            K<Writer<W>, Func<A, B, C, D, E, F, G, H, I, J>> mf, 
            K<Writer<W>, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Writer<W, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> operator * (
            K<Writer<W>, A> ma,
            K<Writer<W>, Func<A, B, C, D, E, F, G, H, I, J>> mf) =>
            curry * mf * ma;
    }
                                
    extension<W, A, B, C, D, E, F, G, H, I, J, K>(K<Writer<W>, A> self)
        where W : Monoid<W>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Writer<W, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> operator * (
            K<Writer<W>, Func<A, B, C, D, E, F, G, H, I, J, K>> mf, 
            K<Writer<W>, A> ma) =>
            curry * mf * ma;

        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Writer<W, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> operator *(
            K<Writer<W>, A> ma,
            K<Writer<W>, Func<A, B, C, D, E, F, G, H, I, J, K>> mf) =>
            curry * mf * ma;
    }
}
