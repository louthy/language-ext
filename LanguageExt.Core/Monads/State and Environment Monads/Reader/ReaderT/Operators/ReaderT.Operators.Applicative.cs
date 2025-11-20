using System;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static partial class ReaderTExtensions
{
    extension<Env, M, A, B>(K<ReaderT<Env, M>, A> self)
        where M : Monad<M>
    {
        /// <summary>
        /// Applicative sequence operator
        /// </summary>
        public static ReaderT<Env, M, B> operator >>> (K<ReaderT<Env, M>, A> ma, K<ReaderT<Env, M>, B> mb) =>
            +ma.Action(mb);
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static ReaderT<Env, M, B> operator * (K<ReaderT<Env, M>, Func<A, B>> mf, K<ReaderT<Env, M>, A> ma) =>
            +mf.Apply(ma);
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static ReaderT<Env, M, B> operator * (K<ReaderT<Env, M>, A> ma, K<ReaderT<Env, M>, Func<A, B>> mf) =>
            +mf.Apply(ma);        
    }
    
    extension<Env, M, A, B, C>(K<ReaderT<Env, M>, A> self)
        where M : Monad<M>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static ReaderT<Env, M, Func<B, C>> operator * (
            K<ReaderT<Env, M>, Func<A, B, C>> mf, 
            K<ReaderT<Env, M>, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static ReaderT<Env, M, Func<B, C>> operator * (
            K<ReaderT<Env, M>, A> ma,
            K<ReaderT<Env, M>, Func<A, B, C>> mf) =>
            curry * mf * ma;
    }
        
    extension<Env, M, A, B, C, D>(K<ReaderT<Env, M>, A> self)
        where M : Monad<M>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static ReaderT<Env, M, Func<B, Func<C, D>>> operator * (
            K<ReaderT<Env, M>, Func<A, B, C, D>> mf, 
            K<ReaderT<Env, M>, A> ma) =>
            curry * mf * ma;

        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static ReaderT<Env, M, Func<B, Func<C, D>>> operator * (
            K<ReaderT<Env, M>, A> ma,
            K<ReaderT<Env, M>, Func<A, B, C, D>> mf) =>
            curry * mf * ma;
    }
            
    extension<Env, M, A, B, C, D, E>(K<ReaderT<Env, M>, A> self)
        where M : Monad<M>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static ReaderT<Env, M, Func<B, Func<C, Func<D, E>>>> operator * (
            K<ReaderT<Env, M>, Func<A, B, C, D, E>> mf, 
            K<ReaderT<Env, M>, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static ReaderT<Env, M, Func<B, Func<C, Func<D, E>>>> operator * (
            K<ReaderT<Env, M>, A> ma,
            K<ReaderT<Env, M>, Func<A, B, C, D, E>> mf) =>
            curry * mf * ma;
    }
                
    extension<Env, M, A, B, C, D, E, F>(K<ReaderT<Env, M>, A> self)
        where M : Monad<M>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static ReaderT<Env, M, Func<B, Func<C, Func<D, Func<E, F>>>>> operator * (
            K<ReaderT<Env, M>, Func<A, B, C, D, E, F>> mf, 
            K<ReaderT<Env, M>, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static ReaderT<Env, M, Func<B, Func<C, Func<D, Func<E, F>>>>> operator * (
            K<ReaderT<Env, M>, A> ma,
            K<ReaderT<Env, M>, Func<A, B, C, D, E, F>> mf) =>
            curry * mf * ma;
    }
                    
    extension<Env, M, A, B, C, D, E, F, G>(K<ReaderT<Env, M>, A> self)
        where M : Monad<M>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static ReaderT<Env, M, Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> operator * (
            K<ReaderT<Env, M>, Func<A, B, C, D, E, F, G>> mf, 
            K<ReaderT<Env, M>, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static ReaderT<Env, M, Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> operator * (
            K<ReaderT<Env, M>, A> ma,
            K<ReaderT<Env, M>, Func<A, B, C, D, E, F, G>> mf) =>
            curry * mf * ma;
    }
                    
    extension<Env, M, A, B, C, D, E, F, G, H>(K<ReaderT<Env, M>, A> self)
        where M : Monad<M>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static ReaderT<Env, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> operator * (
            K<ReaderT<Env, M>, Func<A, B, C, D, E, F, G, H>> mf, 
            K<ReaderT<Env, M>, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static ReaderT<Env, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> operator * (
            K<ReaderT<Env, M>, A> ma,
            K<ReaderT<Env, M>, Func<A, B, C, D, E, F, G, H>> mf) =>
            curry * mf * ma;
    }
                        
    extension<Env, M, A, B, C, D, E, F, G, H, I>(K<ReaderT<Env, M>, A> self)
        where M : Monad<M>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static ReaderT<Env, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> operator * (
            K<ReaderT<Env, M>, Func<A, B, C, D, E, F, G, H, I>> mf, 
            K<ReaderT<Env, M>, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static ReaderT<Env, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> operator * (
            K<ReaderT<Env, M>, A> ma,
            K<ReaderT<Env, M>, Func<A, B, C, D, E, F, G, H, I>> mf) =>
            curry * mf * ma;
    }
                            
    extension<Env, M, A, B, C, D, E, F, G, H, I, J>(K<ReaderT<Env, M>, A> self)
        where M : Monad<M>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static ReaderT<Env, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> operator * (
            K<ReaderT<Env, M>, Func<A, B, C, D, E, F, G, H, I, J>> mf, 
            K<ReaderT<Env, M>, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static ReaderT<Env, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> operator * (
            K<ReaderT<Env, M>, A> ma,
            K<ReaderT<Env, M>, Func<A, B, C, D, E, F, G, H, I, J>> mf) =>
            curry * mf * ma;
    }
                                
    extension<Env, M, A, B, C, D, E, F, G, H, I, J, K>(K<ReaderT<Env, M>, A> self)
        where M : Monad<M>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static ReaderT<Env, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> operator * (
            K<ReaderT<Env, M>, Func<A, B, C, D, E, F, G, H, I, J, K>> mf, 
            K<ReaderT<Env, M>, A> ma) =>
            curry * mf * ma;

        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static ReaderT<Env, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> operator *(
            K<ReaderT<Env, M>, A> ma,
            K<ReaderT<Env, M>, Func<A, B, C, D, E, F, G, H, I, J, K>> mf) =>
            curry * mf * ma;
    }
}
