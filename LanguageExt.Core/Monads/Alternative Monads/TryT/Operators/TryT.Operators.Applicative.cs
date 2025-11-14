using System;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static partial class TryTExtensions
{
    extension<M, A, B>(K<TryT<M>, A> self)
        where M : Monad<M>
    {
        
        /// <summary>
        /// Applicative sequence operator
        /// </summary>
        public static TryT<M, B> operator >>> (K<TryT<M>, A> ma, K<TryT<M>, B> mb) =>
            ma.Action(mb).As();
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static TryT<M, B> operator * (K<TryT<M>, Func<A, B>> mf, K<TryT<M>, A> ma) =>
            mf.Apply(ma);
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static TryT<M, B> operator * (K<TryT<M>, A> ma, K<TryT<M>, Func<A, B>> mf) =>
            mf.Apply(ma);        
    }
    
    extension<M, A, B, C>(K<TryT<M>, A> self)
        where M : Monad<M>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static TryT<M, Func<B, C>> operator * (
            K<TryT<M>, Func<A, B, C>> mf, 
            K<TryT<M>, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static TryT<M, Func<B, C>> operator * (
            K<TryT<M>, A> ma,
            K<TryT<M>, Func<A, B, C>> mf) =>
            curry * mf * ma;
    }
        
    extension<M, A, B, C, D>(K<TryT<M>, A> self)
        where M : Monad<M>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static TryT<M, Func<B, Func<C, D>>> operator * (
            K<TryT<M>, Func<A, B, C, D>> mf, 
            K<TryT<M>, A> ma) =>
            curry * mf * ma;

        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static TryT<M, Func<B, Func<C, D>>> operator * (
            K<TryT<M>, A> ma,
            K<TryT<M>, Func<A, B, C, D>> mf) =>
            curry * mf * ma;
    }
            
    extension<M, A, B, C, D, E>(K<TryT<M>, A> self)
        where M : Monad<M>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static TryT<M, Func<B, Func<C, Func<D, E>>>> operator * (
            K<TryT<M>, Func<A, B, C, D, E>> mf, 
            K<TryT<M>, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static TryT<M, Func<B, Func<C, Func<D, E>>>> operator * (
            K<TryT<M>, A> ma,
            K<TryT<M>, Func<A, B, C, D, E>> mf) =>
            curry * mf * ma;
    }
                
    extension<M, A, B, C, D, E, F>(K<TryT<M>, A> self)
        where M : Monad<M>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static TryT<M, Func<B, Func<C, Func<D, Func<E, F>>>>> operator * (
            K<TryT<M>, Func<A, B, C, D, E, F>> mf, 
            K<TryT<M>, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static TryT<M, Func<B, Func<C, Func<D, Func<E, F>>>>> operator * (
            K<TryT<M>, A> ma,
            K<TryT<M>, Func<A, B, C, D, E, F>> mf) =>
            curry * mf * ma;
    }
                    
    extension<M, A, B, C, D, E, F, G>(K<TryT<M>, A> self)
        where M : Monad<M>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static TryT<M, Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> operator * (
            K<TryT<M>, Func<A, B, C, D, E, F, G>> mf, 
            K<TryT<M>, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static TryT<M, Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> operator * (
            K<TryT<M>, A> ma,
            K<TryT<M>, Func<A, B, C, D, E, F, G>> mf) =>
            curry * mf * ma;
    }
                    
    extension<M, A, B, C, D, E, F, G, H>(K<TryT<M>, A> self)
        where M : Monad<M>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static TryT<M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> operator * (
            K<TryT<M>, Func<A, B, C, D, E, F, G, H>> mf, 
            K<TryT<M>, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static TryT<M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> operator * (
            K<TryT<M>, A> ma,
            K<TryT<M>, Func<A, B, C, D, E, F, G, H>> mf) =>
            curry * mf * ma;
    }
                        
    extension<M, A, B, C, D, E, F, G, H, I>(K<TryT<M>, A> self)
        where M : Monad<M>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static TryT<M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> operator * (
            K<TryT<M>, Func<A, B, C, D, E, F, G, H, I>> mf, 
            K<TryT<M>, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static TryT<M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> operator * (
            K<TryT<M>, A> ma,
            K<TryT<M>, Func<A, B, C, D, E, F, G, H, I>> mf) =>
            curry * mf * ma;
    }
                            
    extension<M, A, B, C, D, E, F, G, H, I, J>(K<TryT<M>, A> self)
        where M : Monad<M>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static TryT<M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> operator * (
            K<TryT<M>, Func<A, B, C, D, E, F, G, H, I, J>> mf, 
            K<TryT<M>, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static TryT<M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> operator * (
            K<TryT<M>, A> ma,
            K<TryT<M>, Func<A, B, C, D, E, F, G, H, I, J>> mf) =>
            curry * mf * ma;
    }
                                
    extension<M, A, B, C, D, E, F, G, H, I, J, K>(K<TryT<M>, A> self)
        where M : Monad<M>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static TryT<M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> operator * (
            K<TryT<M>, Func<A, B, C, D, E, F, G, H, I, J, K>> mf, 
            K<TryT<M>, A> ma) =>
            curry * mf * ma;

        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static TryT<M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> operator *(
            K<TryT<M>, A> ma,
            K<TryT<M>, Func<A, B, C, D, E, F, G, H, I, J, K>> mf) =>
            curry * mf * ma;
    }
}
