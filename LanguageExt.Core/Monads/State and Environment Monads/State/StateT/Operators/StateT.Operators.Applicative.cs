using System;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static partial class StateTExtensions
{
    extension<S, M, A, B>(K<StateT<S, M>, A> self)
        where M : Monad<M>
    {
        /// <summary>
        /// Applicative sequence operator
        /// </summary>
        public static StateT<S, M, B> operator >>> (K<StateT<S, M>, A> ma, K<StateT<S, M>, B> mb) =>
            +ma.Action(mb);
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static StateT<S, M, B> operator * (K<StateT<S, M>, Func<A, B>> mf, K<StateT<S, M>, A> ma) =>
            +mf.Apply(ma);
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static StateT<S, M, B> operator * (K<StateT<S, M>, A> ma, K<StateT<S, M>, Func<A, B>> mf) =>
            +mf.Apply(ma);        
    }
    
    extension<S, M, A, B, C>(K<StateT<S, M>, A> self)
        where M : Monad<M>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static StateT<S, M, Func<B, C>> operator * (
            K<StateT<S, M>, Func<A, B, C>> mf, 
            K<StateT<S, M>, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static StateT<S, M, Func<B, C>> operator * (
            K<StateT<S, M>, A> ma,
            K<StateT<S, M>, Func<A, B, C>> mf) =>
            curry * mf * ma;
    }
        
    extension<S, M, A, B, C, D>(K<StateT<S, M>, A> self)
        where M : Monad<M>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static StateT<S, M, Func<B, Func<C, D>>> operator * (
            K<StateT<S, M>, Func<A, B, C, D>> mf, 
            K<StateT<S, M>, A> ma) =>
            curry * mf * ma;

        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static StateT<S, M, Func<B, Func<C, D>>> operator * (
            K<StateT<S, M>, A> ma,
            K<StateT<S, M>, Func<A, B, C, D>> mf) =>
            curry * mf * ma;
    }
            
    extension<S, M, A, B, C, D, E>(K<StateT<S, M>, A> self)
        where M : Monad<M>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static StateT<S, M, Func<B, Func<C, Func<D, E>>>> operator * (
            K<StateT<S, M>, Func<A, B, C, D, E>> mf, 
            K<StateT<S, M>, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static StateT<S, M, Func<B, Func<C, Func<D, E>>>> operator * (
            K<StateT<S, M>, A> ma,
            K<StateT<S, M>, Func<A, B, C, D, E>> mf) =>
            curry * mf * ma;
    }
                
    extension<S, M, A, B, C, D, E, F>(K<StateT<S, M>, A> self)
        where M : Monad<M>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static StateT<S, M, Func<B, Func<C, Func<D, Func<E, F>>>>> operator * (
            K<StateT<S, M>, Func<A, B, C, D, E, F>> mf, 
            K<StateT<S, M>, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static StateT<S, M, Func<B, Func<C, Func<D, Func<E, F>>>>> operator * (
            K<StateT<S, M>, A> ma,
            K<StateT<S, M>, Func<A, B, C, D, E, F>> mf) =>
            curry * mf * ma;
    }
                    
    extension<S, M, A, B, C, D, E, F, G>(K<StateT<S, M>, A> self)
        where M : Monad<M>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static StateT<S, M, Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> operator * (
            K<StateT<S, M>, Func<A, B, C, D, E, F, G>> mf, 
            K<StateT<S, M>, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static StateT<S, M, Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> operator * (
            K<StateT<S, M>, A> ma,
            K<StateT<S, M>, Func<A, B, C, D, E, F, G>> mf) =>
            curry * mf * ma;
    }
                    
    extension<S, M, A, B, C, D, E, F, G, H>(K<StateT<S, M>, A> self)
        where M : Monad<M>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static StateT<S, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> operator * (
            K<StateT<S, M>, Func<A, B, C, D, E, F, G, H>> mf, 
            K<StateT<S, M>, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static StateT<S, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> operator * (
            K<StateT<S, M>, A> ma,
            K<StateT<S, M>, Func<A, B, C, D, E, F, G, H>> mf) =>
            curry * mf * ma;
    }
                        
    extension<S, M, A, B, C, D, E, F, G, H, I>(K<StateT<S, M>, A> self)
        where M : Monad<M>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static StateT<S, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> operator * (
            K<StateT<S, M>, Func<A, B, C, D, E, F, G, H, I>> mf, 
            K<StateT<S, M>, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static StateT<S, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> operator * (
            K<StateT<S, M>, A> ma,
            K<StateT<S, M>, Func<A, B, C, D, E, F, G, H, I>> mf) =>
            curry * mf * ma;
    }
                            
    extension<S, M, A, B, C, D, E, F, G, H, I, J>(K<StateT<S, M>, A> self)
        where M : Monad<M>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static StateT<S, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> operator * (
            K<StateT<S, M>, Func<A, B, C, D, E, F, G, H, I, J>> mf, 
            K<StateT<S, M>, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static StateT<S, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> operator * (
            K<StateT<S, M>, A> ma,
            K<StateT<S, M>, Func<A, B, C, D, E, F, G, H, I, J>> mf) =>
            curry * mf * ma;
    }
                                
    extension<S, M, A, B, C, D, E, F, G, H, I, J, K>(K<StateT<S, M>, A> self)
        where M : Monad<M>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static StateT<S, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> operator * (
            K<StateT<S, M>, Func<A, B, C, D, E, F, G, H, I, J, K>> mf, 
            K<StateT<S, M>, A> ma) =>
            curry * mf * ma;

        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static StateT<S, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> operator *(
            K<StateT<S, M>, A> ma,
            K<StateT<S, M>, Func<A, B, C, D, E, F, G, H, I, J, K>> mf) =>
            curry * mf * ma;
    }
}
