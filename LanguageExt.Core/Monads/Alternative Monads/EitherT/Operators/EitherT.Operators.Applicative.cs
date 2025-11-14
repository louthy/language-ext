using System;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static partial class EitherTExtensions
{
    extension<L, M, A, B>(K<EitherT<L, M>, A> self)
        where M : Monad<M>
    {
        
        /// <summary>
        /// Applicative sequence operator
        /// </summary>
        public static EitherT<L, M, B> operator >>> (K<EitherT<L, M>, A> ma, K<EitherT<L, M>, B> mb) =>
            ma.Action(mb);
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static EitherT<L, M, B> operator * (K<EitherT<L, M>, Func<A, B>> mf, K<EitherT<L, M>, A> ma) =>
            mf.Apply(ma);
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static EitherT<L, M, B> operator * (K<EitherT<L, M>, A> ma, K<EitherT<L, M>, Func<A, B>> mf) =>
            mf.Apply(ma);        
    }
    
    extension<L, M, A, B, C>(K<EitherT<L, M>, A> self)
        where M : Monad<M>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static EitherT<L, M, Func<B, C>> operator * (
            K<EitherT<L, M>, Func<A, B, C>> mf, 
            K<EitherT<L, M>, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static EitherT<L, M, Func<B, C>> operator * (
            K<EitherT<L, M>, A> ma,
            K<EitherT<L, M>, Func<A, B, C>> mf) =>
            curry * mf * ma;
    }
        
    extension<L, M, A, B, C, D>(K<EitherT<L, M>, A> self)
        where M : Monad<M>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static EitherT<L, M, Func<B, Func<C, D>>> operator * (
            K<EitherT<L, M>, Func<A, B, C, D>> mf, 
            K<EitherT<L, M>, A> ma) =>
            curry * mf * ma;

        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static EitherT<L, M, Func<B, Func<C, D>>> operator * (
            K<EitherT<L, M>, A> ma,
            K<EitherT<L, M>, Func<A, B, C, D>> mf) =>
            curry * mf * ma;
    }
            
    extension<L, M, A, B, C, D, E>(K<EitherT<L, M>, A> self)
        where M : Monad<M>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static EitherT<L, M, Func<B, Func<C, Func<D, E>>>> operator * (
            K<EitherT<L, M>, Func<A, B, C, D, E>> mf, 
            K<EitherT<L, M>, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static EitherT<L, M, Func<B, Func<C, Func<D, E>>>> operator * (
            K<EitherT<L, M>, A> ma,
            K<EitherT<L, M>, Func<A, B, C, D, E>> mf) =>
            curry * mf * ma;
    }
                
    extension<L, M, A, B, C, D, E, F>(K<EitherT<L, M>, A> self)
        where M : Monad<M>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static EitherT<L, M, Func<B, Func<C, Func<D, Func<E, F>>>>> operator * (
            K<EitherT<L, M>, Func<A, B, C, D, E, F>> mf, 
            K<EitherT<L, M>, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static EitherT<L, M, Func<B, Func<C, Func<D, Func<E, F>>>>> operator * (
            K<EitherT<L, M>, A> ma,
            K<EitherT<L, M>, Func<A, B, C, D, E, F>> mf) =>
            curry * mf * ma;
    }
                    
    extension<L, M, A, B, C, D, E, F, G>(K<EitherT<L, M>, A> self)
        where M : Monad<M>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static EitherT<L, M, Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> operator * (
            K<EitherT<L, M>, Func<A, B, C, D, E, F, G>> mf, 
            K<EitherT<L, M>, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static EitherT<L, M, Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> operator * (
            K<EitherT<L, M>, A> ma,
            K<EitherT<L, M>, Func<A, B, C, D, E, F, G>> mf) =>
            curry * mf * ma;
    }
                    
    extension<L, M, A, B, C, D, E, F, G, H>(K<EitherT<L, M>, A> self)
        where M : Monad<M>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static EitherT<L, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> operator * (
            K<EitherT<L, M>, Func<A, B, C, D, E, F, G, H>> mf, 
            K<EitherT<L, M>, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static EitherT<L, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> operator * (
            K<EitherT<L, M>, A> ma,
            K<EitherT<L, M>, Func<A, B, C, D, E, F, G, H>> mf) =>
            curry * mf * ma;
    }
                        
    extension<L, M, A, B, C, D, E, F, G, H, I>(K<EitherT<L, M>, A> self)
        where M : Monad<M>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static EitherT<L, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> operator * (
            K<EitherT<L, M>, Func<A, B, C, D, E, F, G, H, I>> mf, 
            K<EitherT<L, M>, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static EitherT<L, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> operator * (
            K<EitherT<L, M>, A> ma,
            K<EitherT<L, M>, Func<A, B, C, D, E, F, G, H, I>> mf) =>
            curry * mf * ma;
    }
                            
    extension<L, M, A, B, C, D, E, F, G, H, I, J>(K<EitherT<L, M>, A> self)
        where M : Monad<M>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static EitherT<L, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> operator * (
            K<EitherT<L, M>, Func<A, B, C, D, E, F, G, H, I, J>> mf, 
            K<EitherT<L, M>, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static EitherT<L, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> operator * (
            K<EitherT<L, M>, A> ma,
            K<EitherT<L, M>, Func<A, B, C, D, E, F, G, H, I, J>> mf) =>
            curry * mf * ma;
    }
                                
    extension<L, M, A, B, C, D, E, F, G, H, I, J, K>(K<EitherT<L, M>, A> self)
        where M : Monad<M>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static EitherT<L, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> operator * (
            K<EitherT<L, M>, Func<A, B, C, D, E, F, G, H, I, J, K>> mf, 
            K<EitherT<L, M>, A> ma) =>
            curry * mf * ma;

        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static EitherT<L, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> operator *(
            K<EitherT<L, M>, A> ma,
            K<EitherT<L, M>, Func<A, B, C, D, E, F, G, H, I, J, K>> mf) =>
            curry * mf * ma;
    }
}
