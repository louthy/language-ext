using System;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static partial class StateTExtensions
{
    extension<W, M, A, B>(K<WriterT<W, M>, A> self)
        where M : Monad<M>
        where W : Monoid<W>
    {
        /// <summary>
        /// Applicative sequence operator
        /// </summary>
        public static WriterT<W, M, B> operator >>> (K<WriterT<W, M>, A> ma, K<WriterT<W, M>, B> mb) =>
            +ma.Action(mb);
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static WriterT<W, M, B> operator * (K<WriterT<W, M>, Func<A, B>> mf, K<WriterT<W, M>, A> ma) =>
            +mf.Apply(ma);
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static WriterT<W, M, B> operator * (K<WriterT<W, M>, A> ma, K<WriterT<W, M>, Func<A, B>> mf) =>
            +mf.Apply(ma);        
    }
    
    extension<W, M, A, B, C>(K<WriterT<W, M>, A> self)
        where M : Monad<M>
        where W : Monoid<W>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static WriterT<W, M, Func<B, C>> operator * (
            K<WriterT<W, M>, Func<A, B, C>> mf, 
            K<WriterT<W, M>, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static WriterT<W, M, Func<B, C>> operator * (
            K<WriterT<W, M>, A> ma,
            K<WriterT<W, M>, Func<A, B, C>> mf) =>
            curry * mf * ma;
    }
        
    extension<W, M, A, B, C, D>(K<WriterT<W, M>, A> self)
        where M : Monad<M>
        where W : Monoid<W>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static WriterT<W, M, Func<B, Func<C, D>>> operator * (
            K<WriterT<W, M>, Func<A, B, C, D>> mf, 
            K<WriterT<W, M>, A> ma) =>
            curry * mf * ma;

        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static WriterT<W, M, Func<B, Func<C, D>>> operator * (
            K<WriterT<W, M>, A> ma,
            K<WriterT<W, M>, Func<A, B, C, D>> mf) =>
            curry * mf * ma;
    }
            
    extension<W, M, A, B, C, D, E>(K<WriterT<W, M>, A> self)
        where M : Monad<M>
        where W : Monoid<W>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static WriterT<W, M, Func<B, Func<C, Func<D, E>>>> operator * (
            K<WriterT<W, M>, Func<A, B, C, D, E>> mf, 
            K<WriterT<W, M>, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static WriterT<W, M, Func<B, Func<C, Func<D, E>>>> operator * (
            K<WriterT<W, M>, A> ma,
            K<WriterT<W, M>, Func<A, B, C, D, E>> mf) =>
            curry * mf * ma;
    }
                
    extension<W, M, A, B, C, D, E, F>(K<WriterT<W, M>, A> self)
        where M : Monad<M>
        where W : Monoid<W>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static WriterT<W, M, Func<B, Func<C, Func<D, Func<E, F>>>>> operator * (
            K<WriterT<W, M>, Func<A, B, C, D, E, F>> mf, 
            K<WriterT<W, M>, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static WriterT<W, M, Func<B, Func<C, Func<D, Func<E, F>>>>> operator * (
            K<WriterT<W, M>, A> ma,
            K<WriterT<W, M>, Func<A, B, C, D, E, F>> mf) =>
            curry * mf * ma;
    }
                    
    extension<W, M, A, B, C, D, E, F, G>(K<WriterT<W, M>, A> self)
        where M : Monad<M>
        where W : Monoid<W>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static WriterT<W, M, Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> operator * (
            K<WriterT<W, M>, Func<A, B, C, D, E, F, G>> mf, 
            K<WriterT<W, M>, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static WriterT<W, M, Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> operator * (
            K<WriterT<W, M>, A> ma,
            K<WriterT<W, M>, Func<A, B, C, D, E, F, G>> mf) =>
            curry * mf * ma;
    }
                    
    extension<W, M, A, B, C, D, E, F, G, H>(K<WriterT<W, M>, A> self)
        where M : Monad<M>
        where W : Monoid<W>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static WriterT<W, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> operator * (
            K<WriterT<W, M>, Func<A, B, C, D, E, F, G, H>> mf, 
            K<WriterT<W, M>, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static WriterT<W, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> operator * (
            K<WriterT<W, M>, A> ma,
            K<WriterT<W, M>, Func<A, B, C, D, E, F, G, H>> mf) =>
            curry * mf * ma;
    }
                        
    extension<W, M, A, B, C, D, E, F, G, H, I>(K<WriterT<W, M>, A> self)
        where M : Monad<M>
        where W : Monoid<W>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static WriterT<W, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> operator * (
            K<WriterT<W, M>, Func<A, B, C, D, E, F, G, H, I>> mf, 
            K<WriterT<W, M>, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static WriterT<W, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> operator * (
            K<WriterT<W, M>, A> ma,
            K<WriterT<W, M>, Func<A, B, C, D, E, F, G, H, I>> mf) =>
            curry * mf * ma;
    }
                            
    extension<W, M, A, B, C, D, E, F, G, H, I, J>(K<WriterT<W, M>, A> self)
        where M : Monad<M>
        where W : Monoid<W>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static WriterT<W, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> operator * (
            K<WriterT<W, M>, Func<A, B, C, D, E, F, G, H, I, J>> mf, 
            K<WriterT<W, M>, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static WriterT<W, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> operator * (
            K<WriterT<W, M>, A> ma,
            K<WriterT<W, M>, Func<A, B, C, D, E, F, G, H, I, J>> mf) =>
            curry * mf * ma;
    }
                                
    extension<W, M, A, B, C, D, E, F, G, H, I, J, K>(K<WriterT<W, M>, A> self)
        where M : Monad<M>
        where W : Monoid<W>
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static WriterT<W, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> operator * (
            K<WriterT<W, M>, Func<A, B, C, D, E, F, G, H, I, J, K>> mf, 
            K<WriterT<W, M>, A> ma) =>
            curry * mf * ma;

        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static WriterT<W, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> operator *(
            K<WriterT<W, M>, A> ma,
            K<WriterT<W, M>, Func<A, B, C, D, E, F, G, H, I, J, K>> mf) =>
            curry * mf * ma;
    }
}
