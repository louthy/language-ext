using System;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static partial class FunctorExtensions
{
    extension<FF, A>(Memo<FF, A> self)
        where FF : Functor<FF>
    {
        public K<FF, B> Map<B>(Func<A, B> f) =>
            FF.Map(f, self);
    }

    extension<FF, A, B>(Func<A, B> self)
        where FF : Functor<FF>
    {
        public K<FF, B> Map(Memo<FF, A> fa) =>
            FF.Map(self, fa);
    }

    extension<FF, A>(Memo<FF, A> self)
        where FF : Functor<FF>
    {
        public K<FF, Func<B, C>> Map<B, C>(Func<A, B, C> f) =>
            FF.Map(curry(f), self);
    }

    extension<FF, A, B, C>(Func<A, B, C> self)
        where FF : Functor<FF>
    {
        public K<FF, Func<B, C>> Map(Memo<FF, A> fa) =>
            FF.Map(curry(self), fa);
    }
    
    extension<FF, A>(Memo<FF, A> self)
        where FF : Functor<FF>
    {
        public K<FF, Func<B, Func<C, D>>> Map<B, C, D>(Func<A, B, C, D> f) =>
            FF.Map(curry(f), self);
    }
    
    extension<FF, A, B, C, D>(Func<A, B, C, D> self)
        where FF : Functor<FF>
    {
        public K<FF, Func<B, Func<C, D>>> Map(Memo<FF, A> fa) =>
            FF.Map(curry(self), fa);
    }
    
    extension<FF, A>(Memo<FF, A> self)
        where FF : Functor<FF>
    {
        public K<FF, Func<B, Func<C, Func<D, E>>>> Map<B, C, D, E>(Func<A, B, C, D, E> f) =>
            FF.Map(curry(f), self);
    }
    
    extension<FF, A, B, C, D, E>(Func<A, B, C, D, E> self)
        where FF : Functor<FF>
    {
        public K<FF, Func<B, Func<C, Func<D, E>>>> Map(Memo<FF, A> fa) =>
            FF.Map(curry(self), fa);
    }
    
    extension<FF, A>(Memo<FF, A> self)
        where FF : Functor<FF>
    {
        public K<FF, Func<B, Func<C, Func<D, Func<E, F>>>>> Map<B, C, D, E, F>(Func<A, B, C, D, E, F> f) =>
            FF.Map(curry(f), self);
    }
    
    extension<FF, A, B, C, D, E, F>(Func<A, B, C, D, E, F> self)
        where FF : Functor<FF>
    {
        public K<FF, Func<B, Func<C, Func<D, Func<E, F>>>>> Map(Memo<FF, A> fa) =>
            FF.Map(curry(self), fa);
    }
    
    extension<FF, A>(Memo<FF, A> self)
        where FF : Functor<FF>
    {
        public K<FF, Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> Map<B, C, D, E, F, G>(Func<A, B, C, D, E, F, G> f) =>
            FF.Map(curry(f), self);
    }
    
    extension<FF, A, B, C, D, E, F, G>(Func<A, B, C, D, E, F, G> self)
        where FF : Functor<FF>
    {
        public K<FF, Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> Map(Memo<FF, A> fa) =>
            FF.Map(curry(self), fa);
    }
    
    extension<FF, A>(Memo<FF, A> self)
        where FF : Functor<FF>
    {
        public K<FF, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> Map<B, C, D, E, F, G, H>(Func<A, B, C, D, E, F, G, H> f) =>
            FF.Map(curry(f), self);
    }
    
    extension<FF, A, B, C, D, E, F, G, H>(Func<A, B, C, D, E, F, G, H> self)
        where FF : Functor<FF>
    {
        public K<FF, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> Map(Memo<FF, A> fa) =>
            FF.Map(curry(self), fa);
    }
    
    extension<FF, A>(Memo<FF, A> self)
        where FF : Functor<FF>
    {
        public K<FF, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> Map<B, C, D, E, F, G, H, I>(Func<A, B, C, D, E, F, G, H, I> f) =>
            FF.Map(curry(f), self);
    }
    
    extension<FF, A, B, C, D, E, F, G, H, I>(Func<A, B, C, D, E, F, G, H, I> self)
        where FF : Functor<FF>
    {
        public K<FF, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> Map(Memo<FF, A> fa) =>
            FF.Map(curry(self), fa);
    }
    
    extension<FF, A>(Memo<FF, A> self)
        where FF : Functor<FF>
    {
        public K<FF, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> Map<B, C, D, E, F, G, H, I, J>(Func<A, B, C, D, E, F, G, H, I, J> f) =>
            FF.Map(curry(f), self);
    }
    
    extension<FF, A, B, C, D, E, F, G, H, I, J>(Func<A, B, C, D, E, F, G, H, I, J> self)
        where FF : Functor<FF>
    {
        public K<FF, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> Map(Memo<FF, A> fa) =>
            FF.Map(curry(self), fa);
    }    
        
    extension<FF, A>(Memo<FF, A> self)
        where FF : Functor<FF>
    {
        public K<FF, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> Map<B, C, D, E, F, G, H, I, J, K>(Func<A, B, C, D, E, F, G, H, I, J, K> f) =>
            FF.Map(curry(f), self);
    }
    
    extension<FF, A, B, C, D, E, F, G, H, I, J, K>(Func<A, B, C, D, E, F, G, H, I, J, K> self)
        where FF : Functor<FF>
    {
        public K<FF, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> Map(Memo<FF, A> fa) =>
            FF.Map(curry(self), fa);
    }
}
