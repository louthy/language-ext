using System;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static partial class ChronicleTExtensions
{
    extension<Ch, M, A, B>(K<ChronicleT<Ch, M>, A> self)
        where M : Monad<M>
    {
        
        /// <summary>
        /// Applicative sequence operator
        /// </summary>
        public static ChronicleT<Ch, M, B> operator >>> (K<ChronicleT<Ch, M>, A> ma, K<ChronicleT<Ch, M>, B> mb) =>
            +ma.Action(mb);
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static ChronicleT<Ch, M, B> operator * (K<ChronicleT<Ch, M>, Func<A, B>> mf, K<ChronicleT<Ch, M>, A> ma) =>
            +mf.Apply(ma);
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static ChronicleT<Ch, M, B> operator * (K<ChronicleT<Ch, M>, A> ma, K<ChronicleT<Ch, M>, Func<A, B>> mf) =>
            +mf.Apply(ma);        
    }
    
    extension<Ch, M, A, B, C>(K<ChronicleT<Ch, M>, A> self)
        where M : Monad<M>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static ChronicleT<Ch, M, Func<B, C>> operator * (
            K<ChronicleT<Ch, M>, Func<A, B, C>> mf, 
            K<ChronicleT<Ch, M>, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static ChronicleT<Ch, M, Func<B, C>> operator * (
            K<ChronicleT<Ch, M>, A> ma,
            K<ChronicleT<Ch, M>, Func<A, B, C>> mf) =>
            curry * mf * ma;
    }
        
    extension<Ch, M, A, B, C, D>(K<ChronicleT<Ch, M>, A> self)
        where M : Monad<M>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static ChronicleT<Ch, M, Func<B, Func<C, D>>> operator * (
            K<ChronicleT<Ch, M>, Func<A, B, C, D>> mf, 
            K<ChronicleT<Ch, M>, A> ma) =>
            curry * mf * ma;

        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static ChronicleT<Ch, M, Func<B, Func<C, D>>> operator * (
            K<ChronicleT<Ch, M>, A> ma,
            K<ChronicleT<Ch, M>, Func<A, B, C, D>> mf) =>
            curry * mf * ma;
    }
            
    extension<Ch, M, A, B, C, D, E>(K<ChronicleT<Ch, M>, A> self)
        where M : Monad<M>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static ChronicleT<Ch, M, Func<B, Func<C, Func<D, E>>>> operator * (
            K<ChronicleT<Ch, M>, Func<A, B, C, D, E>> mf, 
            K<ChronicleT<Ch, M>, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static ChronicleT<Ch, M, Func<B, Func<C, Func<D, E>>>> operator * (
            K<ChronicleT<Ch, M>, A> ma,
            K<ChronicleT<Ch, M>, Func<A, B, C, D, E>> mf) =>
            curry * mf * ma;
    }
                
    extension<Ch, M, A, B, C, D, E, F>(K<ChronicleT<Ch, M>, A> self)
        where M : Monad<M>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static ChronicleT<Ch, M, Func<B, Func<C, Func<D, Func<E, F>>>>> operator * (
            K<ChronicleT<Ch, M>, Func<A, B, C, D, E, F>> mf, 
            K<ChronicleT<Ch, M>, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static ChronicleT<Ch, M, Func<B, Func<C, Func<D, Func<E, F>>>>> operator * (
            K<ChronicleT<Ch, M>, A> ma,
            K<ChronicleT<Ch, M>, Func<A, B, C, D, E, F>> mf) =>
            curry * mf * ma;
    }
                    
    extension<Ch, M, A, B, C, D, E, F, G>(K<ChronicleT<Ch, M>, A> self)
        where M : Monad<M>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static ChronicleT<Ch, M, Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> operator * (
            K<ChronicleT<Ch, M>, Func<A, B, C, D, E, F, G>> mf, 
            K<ChronicleT<Ch, M>, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static ChronicleT<Ch, M, Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> operator * (
            K<ChronicleT<Ch, M>, A> ma,
            K<ChronicleT<Ch, M>, Func<A, B, C, D, E, F, G>> mf) =>
            curry * mf * ma;
    }
                    
    extension<Ch, M, A, B, C, D, E, F, G, H>(K<ChronicleT<Ch, M>, A> self)
        where M : Monad<M>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static ChronicleT<Ch, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> operator * (
            K<ChronicleT<Ch, M>, Func<A, B, C, D, E, F, G, H>> mf, 
            K<ChronicleT<Ch, M>, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static ChronicleT<Ch, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> operator * (
            K<ChronicleT<Ch, M>, A> ma,
            K<ChronicleT<Ch, M>, Func<A, B, C, D, E, F, G, H>> mf) =>
            curry * mf * ma;
    }
                        
    extension<Ch, M, A, B, C, D, E, F, G, H, I>(K<ChronicleT<Ch, M>, A> self)
        where M : Monad<M>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static ChronicleT<Ch, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> operator * (
            K<ChronicleT<Ch, M>, Func<A, B, C, D, E, F, G, H, I>> mf, 
            K<ChronicleT<Ch, M>, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static ChronicleT<Ch, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> operator * (
            K<ChronicleT<Ch, M>, A> ma,
            K<ChronicleT<Ch, M>, Func<A, B, C, D, E, F, G, H, I>> mf) =>
            curry * mf * ma;
    }
                            
    extension<Ch, M, A, B, C, D, E, F, G, H, I, J>(K<ChronicleT<Ch, M>, A> self)
        where M : Monad<M>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static ChronicleT<Ch, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> operator * (
            K<ChronicleT<Ch, M>, Func<A, B, C, D, E, F, G, H, I, J>> mf, 
            K<ChronicleT<Ch, M>, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static ChronicleT<Ch, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> operator * (
            K<ChronicleT<Ch, M>, A> ma,
            K<ChronicleT<Ch, M>, Func<A, B, C, D, E, F, G, H, I, J>> mf) =>
            curry * mf * ma;
    }
                                
    extension<Ch, M, A, B, C, D, E, F, G, H, I, J, K>(K<ChronicleT<Ch, M>, A> self)
        where M : Monad<M>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static ChronicleT<Ch, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> operator * (
            K<ChronicleT<Ch, M>, Func<A, B, C, D, E, F, G, H, I, J, K>> mf, 
            K<ChronicleT<Ch, M>, A> ma) =>
            curry * mf * ma;

        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static ChronicleT<Ch, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> operator *(
            K<ChronicleT<Ch, M>, A> ma,
            K<ChronicleT<Ch, M>, Func<A, B, C, D, E, F, G, H, I, J, K>> mf) =>
            curry * mf * ma;
    }
}
