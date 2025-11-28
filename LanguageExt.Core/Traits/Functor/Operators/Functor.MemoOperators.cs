using System;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static partial class FunctorExtensions
{
    extension<M, A, B>(Memo<M, A> self)
        where M : Functor<M>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static K<M, B> operator * (Func<A, B> f, Memo<M, A> ma) =>
            M.Map(f, ma);
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static K<M, B> operator * (Memo<M, A> ma, Func<A, B> f) =>
            M.Map(f, ma);
    }
    
    extension<M, A, B, C>(Memo<M, A> self)
        where M : Functor<M>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static K<M, Func<B, C>> operator * (
            Func<A, B, C> f, 
            Memo<M, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static K<M, Func<B, C>> operator * (
            Memo<M, A> ma,
            Func<A, B, C> f) =>
            curry(f) * ma;
    }
        
    extension<M, A, B, C, D>(Memo<M, A> self)
        where M : Functor<M>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static K<M, Func<B, Func<C, D>>> operator * (
            Func<A, B, C, D> f, 
            Memo<M, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static K<M, Func<B, Func<C, D>>> operator * (
            Memo<M, A> ma,
            Func<A, B, C, D> f) =>
            curry(f) * ma;
    }
            
    extension<M, A, B, C, D, E>(Memo<M, A> self)
        where M : Functor<M>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static K<M, Func<B, Func<C, Func<D, E>>>> operator * (
            Func<A, B, C, D, E> f, 
            Memo<M, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static K<M, Func<B, Func<C, Func<D, E>>>> operator * (
            Memo<M, A> ma,
            Func<A, B, C, D, E> f) =>
            curry(f) * ma;
    }
                
    extension<M, A, B, C, D, E, F>(Memo<M, A> self)
        where M : Functor<M>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static K<M, Func<B, Func<C, Func<D, Func<E, F>>>>> operator * (
            Func<A, B, C, D, E, F> f, 
            Memo<M, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static K<M, Func<B, Func<C, Func<D, Func<E, F>>>>> operator * (
            Memo<M, A> ma,
            Func<A, B, C, D, E, F> f) =>
            curry(f) * ma;
    }
                    
    extension<M, A, B, C, D, E, F, G>(Memo<M, A> self)
        where M : Functor<M>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static K<M, Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> operator * (
            Func<A, B, C, D, E, F, G> f, 
            Memo<M, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static K<M, Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> operator * (
            Memo<M, A> ma,
            Func<A, B, C, D, E, F, G> f) =>
            curry(f) * ma;
    }    
                        
    extension<M, A, B, C, D, E, F, G, H>(Memo<M, A> self)
        where M : Functor<M>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static K<M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> operator * (
            Func<A, B, C, D, E, F, G, H> f, 
            Memo<M, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static K<M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> operator * (
            Memo<M, A> ma,
            Func<A, B, C, D, E, F, G, H> f) =>
            curry(f) * ma;
    }
                        
    extension<M, A, B, C, D, E, F, G, H, I>(Memo<M, A> self)
        where M : Functor<M>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static K<M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> operator * (
            Func<A, B, C, D, E, F, G, H, I> f, 
            Memo<M, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static K<M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> operator * (
            Memo<M, A> ma,
            Func<A, B, C, D, E, F, G, H, I> f) =>
            curry(f) * ma;
    }    
                        
    extension<M, A, B, C, D, E, F, G, H, I, J>(Memo<M, A> self)
        where M : Functor<M>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static K<M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> operator * (
            Func<A, B, C, D, E, F, G, H, I, J> f, 
            Memo<M, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static K<M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> operator * (
            Memo<M, A> ma,
            Func<A, B, C, D, E, F, G, H, I, J> f) =>
            curry(f) * ma;
    }
                            
    extension<M, A, B, C, D, E, F, G, H, I, J, K>(Memo<M, A> self)
        where M : Functor<M>
    {
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static K<M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> operator * (
            Func<A, B, C, D, E, F, G, H, I, J, K> f, 
            Memo<M, A> ma) =>
            curry(f) * ma;
        
        /// <summary>
        /// Functor map operator
        /// </summary>
        public static K<M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> operator * (
            Memo<M, A> ma,
            Func<A, B, C, D, E, F, G, H, I, J, K> f) =>
            curry(f) * ma;
    }
}
