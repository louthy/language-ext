using System;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static partial class SourceTExtensions
{
    extension<M, A, B>(K<SourceT<M>, A> self)
        where M : MonadIO<M>
    {
        
        /// <summary>
        /// Applicative sequence operator
        /// </summary>
        public static SourceT<M, B> operator >>> (K<SourceT<M>, A> ma, K<SourceT<M>, B> mb) =>
            +ma.Action(mb);
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static SourceT<M, B> operator * (K<SourceT<M>, Func<A, B>> mf, K<SourceT<M>, A> ma) =>
            +mf.Apply(ma);
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static SourceT<M, B> operator * (K<SourceT<M>, A> ma, K<SourceT<M>, Func<A, B>> mf) =>
            +mf.Apply(ma);        
    }
    
    extension<M, A, B, C>(K<SourceT<M>, A> self)
        where M : MonadIO<M>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static SourceT<M, Func<B, C>> operator * (
            K<SourceT<M>, Func<A, B, C>> mf, 
            K<SourceT<M>, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static SourceT<M, Func<B, C>> operator * (
            K<SourceT<M>, A> ma,
            K<SourceT<M>, Func<A, B, C>> mf) =>
            curry * mf * ma;
    }
        
    extension<M, A, B, C, D>(K<SourceT<M>, A> self)
        where M : MonadIO<M>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static SourceT<M, Func<B, Func<C, D>>> operator * (
            K<SourceT<M>, Func<A, B, C, D>> mf, 
            K<SourceT<M>, A> ma) =>
            curry * mf * ma;

        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static SourceT<M, Func<B, Func<C, D>>> operator * (
            K<SourceT<M>, A> ma,
            K<SourceT<M>, Func<A, B, C, D>> mf) =>
            curry * mf * ma;
    }
            
    extension<M, A, B, C, D, E>(K<SourceT<M>, A> self)
        where M : MonadIO<M>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static SourceT<M, Func<B, Func<C, Func<D, E>>>> operator * (
            K<SourceT<M>, Func<A, B, C, D, E>> mf, 
            K<SourceT<M>, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static SourceT<M, Func<B, Func<C, Func<D, E>>>> operator * (
            K<SourceT<M>, A> ma,
            K<SourceT<M>, Func<A, B, C, D, E>> mf) =>
            curry * mf * ma;
    }
                
    extension<M, A, B, C, D, E, F>(K<SourceT<M>, A> self)
        where M : MonadIO<M>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static SourceT<M, Func<B, Func<C, Func<D, Func<E, F>>>>> operator * (
            K<SourceT<M>, Func<A, B, C, D, E, F>> mf, 
            K<SourceT<M>, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static SourceT<M, Func<B, Func<C, Func<D, Func<E, F>>>>> operator * (
            K<SourceT<M>, A> ma,
            K<SourceT<M>, Func<A, B, C, D, E, F>> mf) =>
            curry * mf * ma;
    }
                    
    extension<M, A, B, C, D, E, F, G>(K<SourceT<M>, A> self)
        where M : MonadIO<M>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static SourceT<M, Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> operator * (
            K<SourceT<M>, Func<A, B, C, D, E, F, G>> mf, 
            K<SourceT<M>, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static SourceT<M, Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> operator * (
            K<SourceT<M>, A> ma,
            K<SourceT<M>, Func<A, B, C, D, E, F, G>> mf) =>
            curry * mf * ma;
    }
                    
    extension<M, A, B, C, D, E, F, G, H>(K<SourceT<M>, A> self)
        where M : MonadIO<M>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static SourceT<M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> operator * (
            K<SourceT<M>, Func<A, B, C, D, E, F, G, H>> mf, 
            K<SourceT<M>, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static SourceT<M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> operator * (
            K<SourceT<M>, A> ma,
            K<SourceT<M>, Func<A, B, C, D, E, F, G, H>> mf) =>
            curry * mf * ma;
    }
                        
    extension<M, A, B, C, D, E, F, G, H, I>(K<SourceT<M>, A> self)
        where M : MonadIO<M>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static SourceT<M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> operator * (
            K<SourceT<M>, Func<A, B, C, D, E, F, G, H, I>> mf, 
            K<SourceT<M>, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static SourceT<M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> operator * (
            K<SourceT<M>, A> ma,
            K<SourceT<M>, Func<A, B, C, D, E, F, G, H, I>> mf) =>
            curry * mf * ma;
    }
                            
    extension<M, A, B, C, D, E, F, G, H, I, J>(K<SourceT<M>, A> self)
        where M : MonadIO<M>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static SourceT<M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> operator * (
            K<SourceT<M>, Func<A, B, C, D, E, F, G, H, I, J>> mf, 
            K<SourceT<M>, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static SourceT<M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> operator * (
            K<SourceT<M>, A> ma,
            K<SourceT<M>, Func<A, B, C, D, E, F, G, H, I, J>> mf) =>
            curry * mf * ma;
    }
                                
    extension<M, A, B, C, D, E, F, G, H, I, J, K>(K<SourceT<M>, A> self)
        where M : MonadIO<M>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static SourceT<M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> operator * (
            K<SourceT<M>, Func<A, B, C, D, E, F, G, H, I, J, K>> mf, 
            K<SourceT<M>, A> ma) =>
            curry * mf * ma;

        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static SourceT<M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> operator *(
            K<SourceT<M>, A> ma,
            K<SourceT<M>, Func<A, B, C, D, E, F, G, H, I, J, K>> mf) =>
            curry * mf * ma;
    }
}
