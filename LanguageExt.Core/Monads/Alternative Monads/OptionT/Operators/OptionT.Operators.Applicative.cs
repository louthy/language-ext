using System;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static partial class OptionTExtensions
{
    extension<M, A, B>(K<OptionT<M>, A> self)
        where M : Monad<M>
    {
        
        /// <summary>
        /// Applicative sequence operator
        /// </summary>
        public static OptionT<M, B> operator >>> (K<OptionT<M>, A> ma, K<OptionT<M>, B> mb) =>
            ma.Action(mb).As();
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static OptionT<M, B> operator * (K<OptionT<M>, Func<A, B>> mf, K<OptionT<M>, A> ma) =>
            mf.Apply(ma);
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static OptionT<M, B> operator * (K<OptionT<M>, A> ma, K<OptionT<M>, Func<A, B>> mf) =>
            mf.Apply(ma);        
    }
    
    extension<M, A, B, C>(K<OptionT<M>, A> self)
        where M : Monad<M>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static OptionT<M, Func<B, C>> operator * (
            K<OptionT<M>, Func<A, B, C>> mf, 
            K<OptionT<M>, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static OptionT<M, Func<B, C>> operator * (
            K<OptionT<M>, A> ma,
            K<OptionT<M>, Func<A, B, C>> mf) =>
            curry * mf * ma;
    }
        
    extension<M, A, B, C, D>(K<OptionT<M>, A> self)
        where M : Monad<M>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static OptionT<M, Func<B, Func<C, D>>> operator * (
            K<OptionT<M>, Func<A, B, C, D>> mf, 
            K<OptionT<M>, A> ma) =>
            curry * mf * ma;

        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static OptionT<M, Func<B, Func<C, D>>> operator * (
            K<OptionT<M>, A> ma,
            K<OptionT<M>, Func<A, B, C, D>> mf) =>
            curry * mf * ma;
    }
            
    extension<M, A, B, C, D, E>(K<OptionT<M>, A> self)
        where M : Monad<M>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static OptionT<M, Func<B, Func<C, Func<D, E>>>> operator * (
            K<OptionT<M>, Func<A, B, C, D, E>> mf, 
            K<OptionT<M>, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static OptionT<M, Func<B, Func<C, Func<D, E>>>> operator * (
            K<OptionT<M>, A> ma,
            K<OptionT<M>, Func<A, B, C, D, E>> mf) =>
            curry * mf * ma;
    }
                
    extension<M, A, B, C, D, E, F>(K<OptionT<M>, A> self)
        where M : Monad<M>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static OptionT<M, Func<B, Func<C, Func<D, Func<E, F>>>>> operator * (
            K<OptionT<M>, Func<A, B, C, D, E, F>> mf, 
            K<OptionT<M>, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static OptionT<M, Func<B, Func<C, Func<D, Func<E, F>>>>> operator * (
            K<OptionT<M>, A> ma,
            K<OptionT<M>, Func<A, B, C, D, E, F>> mf) =>
            curry * mf * ma;
    }
                    
    extension<M, A, B, C, D, E, F, G>(K<OptionT<M>, A> self)
        where M : Monad<M>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static OptionT<M, Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> operator * (
            K<OptionT<M>, Func<A, B, C, D, E, F, G>> mf, 
            K<OptionT<M>, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static OptionT<M, Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> operator * (
            K<OptionT<M>, A> ma,
            K<OptionT<M>, Func<A, B, C, D, E, F, G>> mf) =>
            curry * mf * ma;
    }
                    
    extension<M, A, B, C, D, E, F, G, H>(K<OptionT<M>, A> self)
        where M : Monad<M>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static OptionT<M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> operator * (
            K<OptionT<M>, Func<A, B, C, D, E, F, G, H>> mf, 
            K<OptionT<M>, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static OptionT<M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> operator * (
            K<OptionT<M>, A> ma,
            K<OptionT<M>, Func<A, B, C, D, E, F, G, H>> mf) =>
            curry * mf * ma;
    }
                        
    extension<M, A, B, C, D, E, F, G, H, I>(K<OptionT<M>, A> self)
        where M : Monad<M>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static OptionT<M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> operator * (
            K<OptionT<M>, Func<A, B, C, D, E, F, G, H, I>> mf, 
            K<OptionT<M>, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static OptionT<M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> operator * (
            K<OptionT<M>, A> ma,
            K<OptionT<M>, Func<A, B, C, D, E, F, G, H, I>> mf) =>
            curry * mf * ma;
    }
                            
    extension<M, A, B, C, D, E, F, G, H, I, J>(K<OptionT<M>, A> self)
        where M : Monad<M>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static OptionT<M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> operator * (
            K<OptionT<M>, Func<A, B, C, D, E, F, G, H, I, J>> mf, 
            K<OptionT<M>, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static OptionT<M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> operator * (
            K<OptionT<M>, A> ma,
            K<OptionT<M>, Func<A, B, C, D, E, F, G, H, I, J>> mf) =>
            curry * mf * ma;
    }
                                
    extension<M, A, B, C, D, E, F, G, H, I, J, K>(K<OptionT<M>, A> self)
        where M : Monad<M>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static OptionT<M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> operator * (
            K<OptionT<M>, Func<A, B, C, D, E, F, G, H, I, J, K>> mf, 
            K<OptionT<M>, A> ma) =>
            curry * mf * ma;

        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static OptionT<M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> operator *(
            K<OptionT<M>, A> ma,
            K<OptionT<M>, Func<A, B, C, D, E, F, G, H, I, J, K>> mf) =>
            curry * mf * ma;
    }
}
