using System;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static partial class EitherExtensions
{
    extension<L, A, B>(K<Either<L>, A> self)
    {
        
        /// <summary>
        /// Applicative sequence operator
        /// </summary>
        public static Either<L, B> operator >>> (K<Either<L>, A> ma, K<Either<L>, B> mb) =>
            ma.Action(mb).As();
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Either<L, B> operator * (K<Either<L>, Func<A, B>> mf, K<Either<L>, A> ma) =>
            mf.Apply(ma);
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Either<L, B> operator * (K<Either<L>, A> ma, K<Either<L>, Func<A, B>> mf) =>
            mf.Apply(ma);        
    }
    
    extension<L, A, B, C>(K<Either<L>, A> self)
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Either<L, Func<B, C>> operator * (
            K<Either<L>, Func<A, B, C>> mf, 
            K<Either<L>, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Either<L, Func<B, C>> operator * (
            K<Either<L>, A> ma,
            K<Either<L>, Func<A, B, C>> mf) =>
            curry * mf * ma;
    }
        
    extension<L, A, B, C, D>(K<Either<L>, A> self)
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Either<L, Func<B, Func<C, D>>> operator * (
            K<Either<L>, Func<A, B, C, D>> mf, 
            K<Either<L>, A> ma) =>
            curry * mf * ma;

        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Either<L, Func<B, Func<C, D>>> operator * (
            K<Either<L>, A> ma,
            K<Either<L>, Func<A, B, C, D>> mf) =>
            curry * mf * ma;
    }
            
    extension<L, A, B, C, D, E>(K<Either<L>, A> self)
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Either<L, Func<B, Func<C, Func<D, E>>>> operator * (
            K<Either<L>, Func<A, B, C, D, E>> mf, 
            K<Either<L>, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Either<L, Func<B, Func<C, Func<D, E>>>> operator * (
            K<Either<L>, A> ma,
            K<Either<L>, Func<A, B, C, D, E>> mf) =>
            curry * mf * ma;
    }
                
    extension<L, A, B, C, D, E, F>(K<Either<L>, A> self)
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Either<L, Func<B, Func<C, Func<D, Func<E, F>>>>> operator * (
            K<Either<L>, Func<A, B, C, D, E, F>> mf, 
            K<Either<L>, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Either<L, Func<B, Func<C, Func<D, Func<E, F>>>>> operator * (
            K<Either<L>, A> ma,
            K<Either<L>, Func<A, B, C, D, E, F>> mf) =>
            curry * mf * ma;
    }
                    
    extension<L, A, B, C, D, E, F, G>(K<Either<L>, A> self)
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Either<L, Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> operator * (
            K<Either<L>, Func<A, B, C, D, E, F, G>> mf, 
            K<Either<L>, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Either<L, Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> operator * (
            K<Either<L>, A> ma,
            K<Either<L>, Func<A, B, C, D, E, F, G>> mf) =>
            curry * mf * ma;
    }
                    
    extension<L, A, B, C, D, E, F, G, H>(K<Either<L>, A> self)
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Either<L, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> operator * (
            K<Either<L>, Func<A, B, C, D, E, F, G, H>> mf, 
            K<Either<L>, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Either<L, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> operator * (
            K<Either<L>, A> ma,
            K<Either<L>, Func<A, B, C, D, E, F, G, H>> mf) =>
            curry * mf * ma;
    }
                        
    extension<L, A, B, C, D, E, F, G, H, I>(K<Either<L>, A> self)
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Either<L, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> operator * (
            K<Either<L>, Func<A, B, C, D, E, F, G, H, I>> mf, 
            K<Either<L>, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Either<L, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> operator * (
            K<Either<L>, A> ma,
            K<Either<L>, Func<A, B, C, D, E, F, G, H, I>> mf) =>
            curry * mf * ma;
    }
                            
    extension<L, A, B, C, D, E, F, G, H, I, J>(K<Either<L>, A> self)
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Either<L, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> operator * (
            K<Either<L>, Func<A, B, C, D, E, F, G, H, I, J>> mf, 
            K<Either<L>, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Either<L, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> operator * (
            K<Either<L>, A> ma,
            K<Either<L>, Func<A, B, C, D, E, F, G, H, I, J>> mf) =>
            curry * mf * ma;
    }
                                
    extension<L, A, B, C, D, E, F, G, H, I, J, K>(K<Either<L>, A> self)
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Either<L, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> operator * (
            K<Either<L>, Func<A, B, C, D, E, F, G, H, I, J, K>> mf, 
            K<Either<L>, A> ma) =>
            curry * mf * ma;

        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Either<L, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> operator *(
            K<Either<L>, A> ma,
            K<Either<L>, Func<A, B, C, D, E, F, G, H, I, J, K>> mf) =>
            curry * mf * ma;
    }
}
