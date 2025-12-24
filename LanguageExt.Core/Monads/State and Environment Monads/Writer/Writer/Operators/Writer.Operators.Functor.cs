using System;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static partial class WriterExtensions
{
    extension<W, A, B>(K<Writer<W>, A> _)
        where W : Monoid<W>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Writer<W, B> operator *(Func<A, B> f, K<Writer<W>, A> ma) =>
            +ma.Map(f);
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Writer<W, B> operator *(K<Writer<W>, A> ma, Func<A, B> f) =>
            +ma.Map(f);
    }
    
    extension<W, A, B, C>(K<Writer<W>, A> _)
        where W : Monoid<W>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Writer<W, Func<B, C>> operator * (
            Func<A, B, C> f, 
            K<Writer<W>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Writer<W, Func<B, C>> operator * (
            K<Writer<W>, A> ma,
            Func<A, B, C> f) =>
            curry(f) * ma;
    }
        
    extension<W, A, B, C, D>(K<Writer<W>, A> _)
        where W : Monoid<W>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Writer<W, Func<B, Func<C, D>>> operator * (
            Func<A, B, C, D> f, 
            K<Writer<W>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Writer<W, Func<B, Func<C, D>>> operator * (
            K<Writer<W>, A> ma,
            Func<A, B, C, D> f) =>
            curry(f) * ma;
    }
            
    extension<W, A, B, C, D, E>(K<Writer<W>, A> _)
        where W : Monoid<W>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Writer<W, Func<B, Func<C, Func<D, E>>>> operator * (
            Func<A, B, C, D, E> f, 
            K<Writer<W>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Writer<W, Func<B, Func<C, Func<D, E>>>> operator * (
            K<Writer<W>, A> ma,
            Func<A, B, C, D, E> f) =>
            curry(f) * ma;
    }
                
    extension<W, A, B, C, D, E, F>(K<Writer<W>, A> _)
        where W : Monoid<W>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Writer<W, Func<B, Func<C, Func<D, Func<E, F>>>>> operator * (
            Func<A, B, C, D, E, F> f, 
            K<Writer<W>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Writer<W, Func<B, Func<C, Func<D, Func<E, F>>>>> operator * (
            K<Writer<W>, A> ma,
            Func<A, B, C, D, E, F> f) =>
            curry(f) * ma;
    }
                    
    extension<W, A, B, C, D, E, F, G>(K<Writer<W>, A> _)
        where W : Monoid<W>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Writer<W, Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> operator * (
            Func<A, B, C, D, E, F, G> f, 
            K<Writer<W>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Writer<W, Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> operator * (
            K<Writer<W>, A> ma,
            Func<A, B, C, D, E, F, G> f) =>
            curry(f) * ma;
    }    
                        
    extension<W, A, B, C, D, E, F, G, H>(K<Writer<W>, A> _)
        where W : Monoid<W>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Writer<W, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> operator * (
            Func<A, B, C, D, E, F, G, H> f, 
            K<Writer<W>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Writer<W, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> operator * (
            K<Writer<W>, A> ma,
            Func<A, B, C, D, E, F, G, H> f) =>
            curry(f) * ma;
    }
                        
    extension<W, A, B, C, D, E, F, G, H, I>(K<Writer<W>, A> _)
        where W : Monoid<W>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Writer<W, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> operator * (
            Func<A, B, C, D, E, F, G, H, I> f, 
            K<Writer<W>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Writer<W, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> operator * (
            K<Writer<W>, A> ma,
            Func<A, B, C, D, E, F, G, H, I> f) =>
            curry(f) * ma;
    }    
                        
    extension<W, A, B, C, D, E, F, G, H, I, J>(K<Writer<W>, A> _)
        where W : Monoid<W>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Writer<W, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> operator * (
            Func<A, B, C, D, E, F, G, H, I, J> f, 
            K<Writer<W>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Writer<W, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> operator * (
            K<Writer<W>, A> ma,
            Func<A, B, C, D, E, F, G, H, I, J> f) =>
            curry(f) * ma;
    }
                            
    extension<W, A, B, C, D, E, F, G, H, I, J, K>(K<Writer<W>, A> _)
        where W : Monoid<W>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Writer<W, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> operator * (
            Func<A, B, C, D, E, F, G, H, I, J, K> f, 
            K<Writer<W>, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static Writer<W, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> operator * (
            K<Writer<W>, A> ma,
            Func<A, B, C, D, E, F, G, H, I, J, K> f) =>
            curry(f) * ma;
    }
}
