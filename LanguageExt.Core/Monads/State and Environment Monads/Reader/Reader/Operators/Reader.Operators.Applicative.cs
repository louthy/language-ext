using System;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static partial class ReaderExtensions
{
    extension<Env, A, B>(K<Reader<Env>, A> self)
    {
        /// <summary>
        /// Applicative sequence operator
        /// </summary>
        public static Reader<Env, B> operator >>> (K<Reader<Env>, A> ma, K<Reader<Env>, B> mb) =>
            +ma.Action(mb);
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Reader<Env, B> operator * (K<Reader<Env>, Func<A, B>> mf, K<Reader<Env>, A> ma) =>
            +mf.Apply(ma);
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Reader<Env, B> operator * (K<Reader<Env>, A> ma, K<Reader<Env>, Func<A, B>> mf) =>
            +mf.Apply(ma);        
    }
    
    extension<Env, A, B, C>(K<Reader<Env>, A> self)
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Reader<Env, Func<B, C>> operator * (
            K<Reader<Env>, Func<A, B, C>> mf, 
            K<Reader<Env>, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Reader<Env, Func<B, C>> operator * (
            K<Reader<Env>, A> ma,
            K<Reader<Env>, Func<A, B, C>> mf) =>
            curry * mf * ma;
    }
        
    extension<Env, A, B, C, D>(K<Reader<Env>, A> self)
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Reader<Env, Func<B, Func<C, D>>> operator * (
            K<Reader<Env>, Func<A, B, C, D>> mf, 
            K<Reader<Env>, A> ma) =>
            curry * mf * ma;

        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Reader<Env, Func<B, Func<C, D>>> operator * (
            K<Reader<Env>, A> ma,
            K<Reader<Env>, Func<A, B, C, D>> mf) =>
            curry * mf * ma;
    }
            
    extension<Env, A, B, C, D, E>(K<Reader<Env>, A> self)
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Reader<Env, Func<B, Func<C, Func<D, E>>>> operator * (
            K<Reader<Env>, Func<A, B, C, D, E>> mf, 
            K<Reader<Env>, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Reader<Env, Func<B, Func<C, Func<D, E>>>> operator * (
            K<Reader<Env>, A> ma,
            K<Reader<Env>, Func<A, B, C, D, E>> mf) =>
            curry * mf * ma;
    }
                
    extension<Env, A, B, C, D, E, F>(K<Reader<Env>, A> self)
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Reader<Env, Func<B, Func<C, Func<D, Func<E, F>>>>> operator * (
            K<Reader<Env>, Func<A, B, C, D, E, F>> mf, 
            K<Reader<Env>, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Reader<Env, Func<B, Func<C, Func<D, Func<E, F>>>>> operator * (
            K<Reader<Env>, A> ma,
            K<Reader<Env>, Func<A, B, C, D, E, F>> mf) =>
            curry * mf * ma;
    }
                    
    extension<Env, A, B, C, D, E, F, G>(K<Reader<Env>, A> self)
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Reader<Env, Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> operator * (
            K<Reader<Env>, Func<A, B, C, D, E, F, G>> mf, 
            K<Reader<Env>, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Reader<Env, Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> operator * (
            K<Reader<Env>, A> ma,
            K<Reader<Env>, Func<A, B, C, D, E, F, G>> mf) =>
            curry * mf * ma;
    }
                    
    extension<Env, A, B, C, D, E, F, G, H>(K<Reader<Env>, A> self)
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Reader<Env, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> operator * (
            K<Reader<Env>, Func<A, B, C, D, E, F, G, H>> mf, 
            K<Reader<Env>, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Reader<Env, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> operator * (
            K<Reader<Env>, A> ma,
            K<Reader<Env>, Func<A, B, C, D, E, F, G, H>> mf) =>
            curry * mf * ma;
    }
                        
    extension<Env, A, B, C, D, E, F, G, H, I>(K<Reader<Env>, A> self)
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Reader<Env, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> operator * (
            K<Reader<Env>, Func<A, B, C, D, E, F, G, H, I>> mf, 
            K<Reader<Env>, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Reader<Env, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> operator * (
            K<Reader<Env>, A> ma,
            K<Reader<Env>, Func<A, B, C, D, E, F, G, H, I>> mf) =>
            curry * mf * ma;
    }
                            
    extension<Env, A, B, C, D, E, F, G, H, I, J>(K<Reader<Env>, A> self)
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Reader<Env, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> operator * (
            K<Reader<Env>, Func<A, B, C, D, E, F, G, H, I, J>> mf, 
            K<Reader<Env>, A> ma) =>
            curry * mf * ma;
        
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Reader<Env, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> operator * (
            K<Reader<Env>, A> ma,
            K<Reader<Env>, Func<A, B, C, D, E, F, G, H, I, J>> mf) =>
            curry * mf * ma;
    }
                                
    extension<Env, A, B, C, D, E, F, G, H, I, J, K>(K<Reader<Env>, A> self)
    {
        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Reader<Env, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> operator * (
            K<Reader<Env>, Func<A, B, C, D, E, F, G, H, I, J, K>> mf, 
            K<Reader<Env>, A> ma) =>
            curry * mf * ma;

        /// <summary>
        /// Applicative apply operator
        /// </summary>
        public static Reader<Env, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> operator *(
            K<Reader<Env>, A> ma,
            K<Reader<Env>, Func<A, B, C, D, E, F, G, H, I, J, K>> mf) =>
            curry * mf * ma;
    }
}
