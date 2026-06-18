using System;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static partial class IteratorIOExtensions
{
    extension<A, B>(K<IteratorIO, A> _)
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static IteratorIO<B> operator *(Func<A, B> f, K<IteratorIO, A> ma) =>
            +ma.Map(f);
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static IteratorIO<B> operator *(K<IteratorIO, A> ma, Func<A, B> f) =>
            +ma.Map(f);
    }
    
    extension<A, B, C>(K<IteratorIO, A> _)
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static IteratorIO<Func<B, C>> operator * (
            Func<A, B, C> f, 
            K<IteratorIO, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static IteratorIO<Func<B, C>> operator * (
            K<IteratorIO, A> ma,
            Func<A, B, C> f) =>
            curry(f) * ma;
    }
        
    extension<A, B, C, D>(K<IteratorIO, A> _)
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static IteratorIO<Func<B, Func<C, D>>> operator * (
            Func<A, B, C, D> f, 
            K<IteratorIO, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static IteratorIO<Func<B, Func<C, D>>> operator * (
            K<IteratorIO, A> ma,
            Func<A, B, C, D> f) =>
            curry(f) * ma;
    }
            
    extension<A, B, C, D, E>(K<IteratorIO, A> _)
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static IteratorIO<Func<B, Func<C, Func<D, E>>>> operator * (
            Func<A, B, C, D, E> f, 
            K<IteratorIO, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static IteratorIO<Func<B, Func<C, Func<D, E>>>> operator * (
            K<IteratorIO, A> ma,
            Func<A, B, C, D, E> f) =>
            curry(f) * ma;
    }
                
    extension<A, B, C, D, E, F>(K<IteratorIO, A> _)
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static IteratorIO<Func<B, Func<C, Func<D, Func<E, F>>>>> operator * (
            Func<A, B, C, D, E, F> f, 
            K<IteratorIO, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static IteratorIO<Func<B, Func<C, Func<D, Func<E, F>>>>> operator * (
            K<IteratorIO, A> ma,
            Func<A, B, C, D, E, F> f) =>
            curry(f) * ma;
    }
                    
    extension<A, B, C, D, E, F, G>(K<IteratorIO, A> _)
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static IteratorIO<Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> operator * (
            Func<A, B, C, D, E, F, G> f, 
            K<IteratorIO, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static IteratorIO<Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> operator * (
            K<IteratorIO, A> ma,
            Func<A, B, C, D, E, F, G> f) =>
            curry(f) * ma;
    }    
                        
    extension<A, B, C, D, E, F, G, H>(K<IteratorIO, A> _)
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static IteratorIO<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> operator * (
            Func<A, B, C, D, E, F, G, H> f, 
            K<IteratorIO, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static IteratorIO<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> operator * (
            K<IteratorIO, A> ma,
            Func<A, B, C, D, E, F, G, H> f) =>
            curry(f) * ma;
    }
                        
    extension<A, B, C, D, E, F, G, H, I>(K<IteratorIO, A> _)
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static IteratorIO<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> operator * (
            Func<A, B, C, D, E, F, G, H, I> f, 
            K<IteratorIO, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static IteratorIO<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> operator * (
            K<IteratorIO, A> ma,
            Func<A, B, C, D, E, F, G, H, I> f) =>
            curry(f) * ma;
    }    
                        
    extension<A, B, C, D, E, F, G, H, I, J>(K<IteratorIO, A> _)
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static IteratorIO<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> operator * (
            Func<A, B, C, D, E, F, G, H, I, J> f, 
            K<IteratorIO, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static IteratorIO<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> operator * (
            K<IteratorIO, A> ma,
            Func<A, B, C, D, E, F, G, H, I, J> f) =>
            curry(f) * ma;
    }
                            
    extension<A, B, C, D, E, F, G, H, I, J, K>(K<IteratorIO, A> _)
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static IteratorIO<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> operator * (
            Func<A, B, C, D, E, F, G, H, I, J, K> f, 
            K<IteratorIO, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static IteratorIO<Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> operator * (
            K<IteratorIO, A> ma,
            Func<A, B, C, D, E, F, G, H, I, J, K> f) =>
            curry(f) * ma;
    }
}
