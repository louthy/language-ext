using System;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static partial class EitherExtensions
{
    extension<L, A, B>(K<Either<L>, A> _)
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Either<L, B> operator *(Func<A, B> f, K<Either<L>, A> ma) =>
            ma.Map(f).As();
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Either<L, B> operator *(K<Either<L>, A> ma, Func<A, B> f) =>
            ma.Map(f).As();
    }
    
    extension<L, A, B, C>(K<Either<L>, A> _)
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Either<L, Func<B, C>> operator * (
            Func<A, B, C> f, 
            K<Either<L>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Either<L, Func<B, C>> operator * (
            K<Either<L>, A> ma,
            Func<A, B, C> f) =>
            curry(f) * ma;
    }
        
    extension<L, A, B, C, D>(K<Either<L>, A> _)
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Either<L, Func<B, Func<C, D>>> operator * (
            Func<A, B, C, D> f, 
            K<Either<L>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Either<L, Func<B, Func<C, D>>> operator * (
            K<Either<L>, A> ma,
            Func<A, B, C, D> f) =>
            curry(f) * ma;
    }
            
    extension<L, A, B, C, D, E>(K<Either<L>, A> _)
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Either<L, Func<B, Func<C, Func<D, E>>>> operator * (
            Func<A, B, C, D, E> f, 
            K<Either<L>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Either<L, Func<B, Func<C, Func<D, E>>>> operator * (
            K<Either<L>, A> ma,
            Func<A, B, C, D, E> f) =>
            curry(f) * ma;
    }
                
    extension<L, A, B, C, D, E, F>(K<Either<L>, A> _)
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Either<L, Func<B, Func<C, Func<D, Func<E, F>>>>> operator * (
            Func<A, B, C, D, E, F> f, 
            K<Either<L>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Either<L, Func<B, Func<C, Func<D, Func<E, F>>>>> operator * (
            K<Either<L>, A> ma,
            Func<A, B, C, D, E, F> f) =>
            curry(f) * ma;
    }
                    
    extension<L, A, B, C, D, E, F, G>(K<Either<L>, A> _)
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Either<L, Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> operator * (
            Func<A, B, C, D, E, F, G> f, 
            K<Either<L>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Either<L, Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> operator * (
            K<Either<L>, A> ma,
            Func<A, B, C, D, E, F, G> f) =>
            curry(f) * ma;
    }    
                        
    extension<L, A, B, C, D, E, F, G, H>(K<Either<L>, A> _)
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Either<L, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> operator * (
            Func<A, B, C, D, E, F, G, H> f, 
            K<Either<L>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Either<L, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> operator * (
            K<Either<L>, A> ma,
            Func<A, B, C, D, E, F, G, H> f) =>
            curry(f) * ma;
    }
                        
    extension<L, A, B, C, D, E, F, G, H, I>(K<Either<L>, A> _)
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Either<L, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> operator * (
            Func<A, B, C, D, E, F, G, H, I> f, 
            K<Either<L>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Either<L, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> operator * (
            K<Either<L>, A> ma,
            Func<A, B, C, D, E, F, G, H, I> f) =>
            curry(f) * ma;
    }    
                        
    extension<L, A, B, C, D, E, F, G, H, I, J>(K<Either<L>, A> _)
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Either<L, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> operator * (
            Func<A, B, C, D, E, F, G, H, I, J> f, 
            K<Either<L>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Either<L, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> operator * (
            K<Either<L>, A> ma,
            Func<A, B, C, D, E, F, G, H, I, J> f) =>
            curry(f) * ma;
    }
                            
    extension<L, A, B, C, D, E, F, G, H, I, J, K>(K<Either<L>, A> _)
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Either<L, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> operator * (
            Func<A, B, C, D, E, F, G, H, I, J, K> f, 
            K<Either<L>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Either<L, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> operator * (
            K<Either<L>, A> ma,
            Func<A, B, C, D, E, F, G, H, I, J, K> f) =>
            curry(f) * ma;
    }
}
