using System;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static partial class ChronicleTExtensions
{
    extension<Ch, M, A, B>(K<ChronicleT<Ch, M>, A> _)
        where M : Monad<M>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static ChronicleT<Ch, M, B> operator *(Func<A, B> f, K<ChronicleT<Ch, M>, A> ma) =>
            ma.Map(f).As();
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static ChronicleT<Ch, M, B> operator *(K<ChronicleT<Ch, M>, A> ma, Func<A, B> f) =>
            ma.Map(f).As();
    }
    
    extension<Ch, M, A, B, C>(K<ChronicleT<Ch, M>, A> _)
        where M : Monad<M>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static ChronicleT<Ch, M, Func<B, C>> operator * (
            Func<A, B, C> f, 
            K<ChronicleT<Ch, M>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static ChronicleT<Ch, M, Func<B, C>> operator * (
            K<ChronicleT<Ch, M>, A> ma,
            Func<A, B, C> f) =>
            curry(f) * ma;
        
    }
        
    extension<Ch, M, A, B, C, D>(K<ChronicleT<Ch, M>, A> _)
        where M : Monad<M>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static ChronicleT<Ch, M, Func<B, Func<C, D>>> operator * (
            Func<A, B, C, D> f, 
            K<ChronicleT<Ch, M>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static ChronicleT<Ch, M, Func<B, Func<C, D>>> operator * (
            K<ChronicleT<Ch, M>, A> ma,
            Func<A, B, C, D> f) =>
            curry(f) * ma;
    }
            
    extension<Ch, M, A, B, C, D, E>(K<ChronicleT<Ch, M>, A> _)
        where M : Monad<M>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static ChronicleT<Ch, M, Func<B, Func<C, Func<D, E>>>> operator * (
            Func<A, B, C, D, E> f, 
            K<ChronicleT<Ch, M>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static ChronicleT<Ch, M, Func<B, Func<C, Func<D, E>>>> operator * (
            K<ChronicleT<Ch, M>, A> ma,
            Func<A, B, C, D, E> f) =>
            curry(f) * ma;
    }
                
    extension<Ch, M, A, B, C, D, E, F>(K<ChronicleT<Ch, M>, A> _)
        where M : Monad<M>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static ChronicleT<Ch, M, Func<B, Func<C, Func<D, Func<E, F>>>>> operator * (
            Func<A, B, C, D, E, F> f, 
            K<ChronicleT<Ch, M>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static ChronicleT<Ch, M, Func<B, Func<C, Func<D, Func<E, F>>>>> operator * (
            K<ChronicleT<Ch, M>, A> ma,
            Func<A, B, C, D, E, F> f) =>
            curry(f) * ma;
    }
                    
    extension<Ch, M, A, B, C, D, E, F, G>(K<ChronicleT<Ch, M>, A> _)
        where M : Monad<M>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static ChronicleT<Ch, M, Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> operator * (
            Func<A, B, C, D, E, F, G> f, 
            K<ChronicleT<Ch, M>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static ChronicleT<Ch, M, Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> operator * (
            K<ChronicleT<Ch, M>, A> ma,
            Func<A, B, C, D, E, F, G> f) =>
            curry(f) * ma;
    }    
                        
    extension<Ch, M, A, B, C, D, E, F, G, H>(K<ChronicleT<Ch, M>, A> _)
        where M : Monad<M>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static ChronicleT<Ch, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> operator * (
            Func<A, B, C, D, E, F, G, H> f, 
            K<ChronicleT<Ch, M>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static ChronicleT<Ch, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> operator * (
            K<ChronicleT<Ch, M>, A> ma,
            Func<A, B, C, D, E, F, G, H> f) =>
            curry(f) * ma;
        
    }
                        
    extension<Ch, M, A, B, C, D, E, F, G, H, I>(K<ChronicleT<Ch, M>, A> _)
        where M : Monad<M>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static ChronicleT<Ch, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> operator * (
            Func<A, B, C, D, E, F, G, H, I> f, 
            K<ChronicleT<Ch, M>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static ChronicleT<Ch, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> operator * (
            K<ChronicleT<Ch, M>, A> ma,
            Func<A, B, C, D, E, F, G, H, I> f) =>
            curry(f) * ma;
    }    
                        
    extension<Ch, M, A, B, C, D, E, F, G, H, I, J>(K<ChronicleT<Ch, M>, A> _)
        where M : Monad<M>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static ChronicleT<Ch, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> operator * (
            Func<A, B, C, D, E, F, G, H, I, J> f, 
            K<ChronicleT<Ch, M>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static ChronicleT<Ch, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> operator * (
            K<ChronicleT<Ch, M>, A> ma,
            Func<A, B, C, D, E, F, G, H, I, J> f) =>
            curry(f) * ma;
    }
                            
    extension<Ch, M, A, B, C, D, E, F, G, H, I, J, K>(K<ChronicleT<Ch, M>, A> _)
        where M : Monad<M>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static ChronicleT<Ch, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> operator * (
            Func<A, B, C, D, E, F, G, H, I, J, K> f, 
            K<ChronicleT<Ch, M>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static ChronicleT<Ch, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> operator * (
            K<ChronicleT<Ch, M>, A> ma,
            Func<A, B, C, D, E, F, G, H, I, J, K> f) =>
            curry(f) * ma;
    }
}
