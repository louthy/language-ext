using System;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static partial class RWSTExtensions
{
    extension<R, W, S, M, A, B>(K<RWST<R, W, S, M>, A> self)
        where M : Monad<M>
        where W : Monoid<W>
    {
        /// <summary>
        /// Applicative sequence operator
        /// </summary>
        public static RWST<R, W, S, M, B> operator >>> (K<RWST<R, W, S, M>, A> ma, K<RWST<R, W, S, M>, B> mb) =>
            +ma.Action(mb);
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static RWST<R, W, S, M, B> operator * (K<RWST<R, W, S, M>, Func<A, B>> mf, K<RWST<R, W, S, M>, A> ma) =>
            +mf.Apply(ma);
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static RWST<R, W, S, M, B> operator * (K<RWST<R, W, S, M>, A> ma, K<RWST<R, W, S, M>, Func<A, B>> mf) =>
            +mf.Apply(ma);        
    }
    
    extension<R, W, S, M, A, B, C>(K<RWST<R, W, S, M>, A> self)
        where M : Monad<M>
        where W : Monoid<W>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static RWST<R, W, S, M, Func<B, C>> operator * (
            K<RWST<R, W, S, M>, Func<A, B, C>> mf, 
            K<RWST<R, W, S, M>, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static RWST<R, W, S, M, Func<B, C>> operator * (
            K<RWST<R, W, S, M>, A> ma,
            K<RWST<R, W, S, M>, Func<A, B, C>> mf) =>
            curry * mf * ma;
    }
        
    extension<R, W, S, M, A, B, C, D>(K<RWST<R, W, S, M>, A> self)
        where M : Monad<M>
        where W : Monoid<W>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static RWST<R, W, S, M, Func<B, Func<C, D>>> operator * (
            K<RWST<R, W, S, M>, Func<A, B, C, D>> mf, 
            K<RWST<R, W, S, M>, A> ma) =>
            curry * mf * ma;

        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static RWST<R, W, S, M, Func<B, Func<C, D>>> operator * (
            K<RWST<R, W, S, M>, A> ma,
            K<RWST<R, W, S, M>, Func<A, B, C, D>> mf) =>
            curry * mf * ma;
    }
            
    extension<R, W, S, M, A, B, C, D, E>(K<RWST<R, W, S, M>, A> self)
        where M : Monad<M>
        where W : Monoid<W>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static RWST<R, W, S, M, Func<B, Func<C, Func<D, E>>>> operator * (
            K<RWST<R, W, S, M>, Func<A, B, C, D, E>> mf, 
            K<RWST<R, W, S, M>, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static RWST<R, W, S, M, Func<B, Func<C, Func<D, E>>>> operator * (
            K<RWST<R, W, S, M>, A> ma,
            K<RWST<R, W, S, M>, Func<A, B, C, D, E>> mf) =>
            curry * mf * ma;
    }
                
    extension<R, W, S, M, A, B, C, D, E, F>(K<RWST<R, W, S, M>, A> self)
        where M : Monad<M>
        where W : Monoid<W>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static RWST<R, W, S, M, Func<B, Func<C, Func<D, Func<E, F>>>>> operator * (
            K<RWST<R, W, S, M>, Func<A, B, C, D, E, F>> mf, 
            K<RWST<R, W, S, M>, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static RWST<R, W, S, M, Func<B, Func<C, Func<D, Func<E, F>>>>> operator * (
            K<RWST<R, W, S, M>, A> ma,
            K<RWST<R, W, S, M>, Func<A, B, C, D, E, F>> mf) =>
            curry * mf * ma;
    }
                    
    extension<R, W, S, M, A, B, C, D, E, F, G>(K<RWST<R, W, S, M>, A> self)
        where M : Monad<M>
        where W : Monoid<W>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static RWST<R, W, S, M, Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> operator * (
            K<RWST<R, W, S, M>, Func<A, B, C, D, E, F, G>> mf, 
            K<RWST<R, W, S, M>, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static RWST<R, W, S, M, Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> operator * (
            K<RWST<R, W, S, M>, A> ma,
            K<RWST<R, W, S, M>, Func<A, B, C, D, E, F, G>> mf) =>
            curry * mf * ma;
    }
                    
    extension<R, W, S, M, A, B, C, D, E, F, G, H>(K<RWST<R, W, S, M>, A> self)
        where M : Monad<M>
        where W : Monoid<W>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static RWST<R, W, S, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> operator * (
            K<RWST<R, W, S, M>, Func<A, B, C, D, E, F, G, H>> mf, 
            K<RWST<R, W, S, M>, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static RWST<R, W, S, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> operator * (
            K<RWST<R, W, S, M>, A> ma,
            K<RWST<R, W, S, M>, Func<A, B, C, D, E, F, G, H>> mf) =>
            curry * mf * ma;
    }
                        
    extension<R, W, S, M, A, B, C, D, E, F, G, H, I>(K<RWST<R, W, S, M>, A> self)
        where M : Monad<M>
        where W : Monoid<W>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static RWST<R, W, S, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> operator * (
            K<RWST<R, W, S, M>, Func<A, B, C, D, E, F, G, H, I>> mf, 
            K<RWST<R, W, S, M>, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static RWST<R, W, S, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> operator * (
            K<RWST<R, W, S, M>, A> ma,
            K<RWST<R, W, S, M>, Func<A, B, C, D, E, F, G, H, I>> mf) =>
            curry * mf * ma;
    }
                            
    extension<R, W, S, M, A, B, C, D, E, F, G, H, I, J>(K<RWST<R, W, S, M>, A> self)
        where M : Monad<M>
        where W : Monoid<W>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static RWST<R, W, S, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> operator * (
            K<RWST<R, W, S, M>, Func<A, B, C, D, E, F, G, H, I, J>> mf, 
            K<RWST<R, W, S, M>, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static RWST<R, W, S, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> operator * (
            K<RWST<R, W, S, M>, A> ma,
            K<RWST<R, W, S, M>, Func<A, B, C, D, E, F, G, H, I, J>> mf) =>
            curry * mf * ma;
    }
                                
    extension<R, W, S, M, A, B, C, D, E, F, G, H, I, J, K>(K<RWST<R, W, S, M>, A> self)
        where M : Monad<M>
        where W : Monoid<W>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static RWST<R, W, S, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> operator * (
            K<RWST<R, W, S, M>, Func<A, B, C, D, E, F, G, H, I, J, K>> mf, 
            K<RWST<R, W, S, M>, A> ma) =>
            curry * mf * ma;

        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static RWST<R, W, S, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> operator *(
            K<RWST<R, W, S, M>, A> ma,
            K<RWST<R, W, S, M>, Func<A, B, C, D, E, F, G, H, I, J, K>> mf) =>
            curry * mf * ma;
    }
}
