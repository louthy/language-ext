using System;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class FunctorExtensions
{
    extension<M, A, B>(K<M, A> self)
        where M : Functor<M>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static K<M, B> operator * (Func<A, B> f, K<M, A> ma) =>
            ma.Map(f);
    }
    
    extension<M, A, B, C>(K<M, A> self)
        where M : Applicative<M>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static K<M, Func<B, C>> operator * (Func<A, B, C> f, K<M, A> ma) =>
            f.Map(ma);
    }
        
    extension<M, A, B, C, D>(K<M, A> self)
        where M : Applicative<M>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static K<M, Func<B, Func<C, D>>> operator * (Func<A, B, C, D> f, K<M, A> ma) =>
            f.Map(ma);
    }
            
    extension<M, A, B, C, D, E>(K<M, A> self)
        where M : Applicative<M>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static K<M, Func<B, Func<C, Func<D, E>>>> operator * (Func<A, B, C, D, E> f, K<M, A> ma) =>
            f.Map(ma);
    }
                
    extension<M, A, B, C, D, E, F>(K<M, A> self)
        where M : Applicative<M>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static K<M, Func<B, Func<C, Func<D, Func<E, F>>>>> operator * (Func<A, B, C, D, E, F> f, K<M, A> ma) =>
            f.Map(ma);
    }
                    
    extension<M, A, B, C, D, E, F, G>(K<M, A> self)
        where M : Applicative<M>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static K<M, Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> operator * (Func<A, B, C, D, E, F, G> f, K<M, A> ma) =>
            f.Map(ma);
    }    
                        
    extension<M, A, B, C, D, E, F, G, H>(K<M, A> self)
        where M : Applicative<M>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static K<M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> operator * (Func<A, B, C, D, E, F, G, H> f, K<M, A> ma) =>
            f.Map(ma);
    }
                        
    extension<M, A, B, C, D, E, F, G, H, I>(K<M, A> self)
        where M : Applicative<M>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static K<M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> operator * (Func<A, B, C, D, E, F, G, H, I> f, K<M, A> ma) =>
            f.Map(ma);
    }    
                        
    extension<M, A, B, C, D, E, F, G, H, I, J>(K<M, A> self)
        where M : Applicative<M>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static K<M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> operator * (Func<A, B, C, D, E, F, G, H, I, J> f, K<M, A> ma) =>
            f.Map(ma);
    }
                            
    extension<M, A, B, C, D, E, F, G, H, I, J, K>(K<M, A> self)
        where M : Applicative<M>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static K<M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> operator * (Func<A, B, C, D, E, F, G, H, I, J, K> f, K<M, A> ma) =>
            f.Map(ma);
    }
}
