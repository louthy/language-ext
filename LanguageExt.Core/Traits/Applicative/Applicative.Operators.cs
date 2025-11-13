using System;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class ApplicativeExtensions
{
    extension<M, A, B>(K<M, A> self)
        where M : Applicative<M>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static K<M, B> operator * (K<M, Func<A, B>> f, K<M, A> ma) =>
            f.Apply(ma);
    }
    
    extension<M, A, B, C>(K<M, A> self)
        where M : Applicative<M>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static K<M, Func<B, C>> operator * (K<M, Func<A, B, C>> f, K<M, A> ma) =>
            f.Apply(ma);
    }
        
    extension<M, A, B, C, D>(K<M, A> self)
        where M : Applicative<M>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static K<M, Func<B, Func<C, D>>> operator * (K<M, Func<A, B, C, D>> f, K<M, A> ma) =>
            f.Apply(ma);
    }
            
    extension<M, A, B, C, D, E>(K<M, A> self)
        where M : Applicative<M>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static K<M, Func<B, Func<C, Func<D, E>>>> operator * (K<M, Func<A, B, C, D, E>> f, K<M, A> ma) =>
            f.Apply(ma);
    }
                
    extension<M, A, B, C, D, E, F>(K<M, A> self)
        where M : Applicative<M>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static K<M, Func<B, Func<C, Func<D, Func<E, F>>>>> operator * (K<M, Func<A, B, C, D, E, F>> f, K<M, A> ma) =>
            f.Apply(ma);
    }
                    
    extension<M, A, B, C, D, E, F, G>(K<M, A> self)
        where M : Applicative<M>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static K<M, Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> operator * (K<M, Func<A, B, C, D, E, F, G>> f, K<M, A> ma) =>
            f.Apply(ma);
    }
                    
    extension<M, A, B, C, D, E, F, G, H>(K<M, A> self)
        where M : Applicative<M>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static K<M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> operator * (K<M, Func<A, B, C, D, E, F, G, H>> f, K<M, A> ma) =>
            f.Apply(ma);
    }
                        
    extension<M, A, B, C, D, E, F, G, H, I>(K<M, A> self)
        where M : Applicative<M>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static K<M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> operator * (K<M, Func<A, B, C, D, E, F, G, H, I>> f, K<M, A> ma) =>
            f.Apply(ma);
    }
                            
    extension<M, A, B, C, D, E, F, G, H, I, J>(K<M, A> self)
        where M : Applicative<M>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static K<M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> operator * (K<M, Func<A, B, C, D, E, F, G, H, I, J>> f, K<M, A> ma) =>
            f.Apply(ma);
    }
                                
    extension<M, A, B, C, D, E, F, G, H, I, J, K>(K<M, A> self)
        where M : Applicative<M>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static K<M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> operator * (K<M, Func<A, B, C, D, E, F, G, H, I, J, K>> f, K<M, A> ma) =>
            f.Apply(ma);
    }
}
