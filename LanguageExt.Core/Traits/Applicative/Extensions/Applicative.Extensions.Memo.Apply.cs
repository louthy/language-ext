using System;
using LanguageExt.Traits;
using System.Diagnostics.Contracts;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static partial class ApplicativeExtensions
{
    extension<AF, A, B>(K<AF, Func<A, B>> mf)
        where AF : Applicative<AF>
    {
        [Pure]
        public K<AF, B> Apply(Memo<AF, A> ma) =>
            AF.Apply(mf, ma);
    }

    extension<AF, A, B>(Memo<AF, Func<A, B>> mf)
        where AF : Applicative<AF>
    {
        [Pure]
        public K<AF, B> Apply(Memo<AF, A> ma) =>
            AF.Apply(mf, ma);
    }

    extension<AF, A, B, C>(K<AF, Func<A, B, C>> mf) 
        where AF : Applicative<AF>
    {
        [Pure]
        public K<AF, Func<B, C>> Apply(Memo<AF, A> ma) =>
            AF.Apply(AF.Map(curry, mf), ma);
    }

    extension<AF, A, B, C>(Memo<AF, Func<A, B, C>> mf) 
        where AF : Applicative<AF>
    {
        [Pure]
        public K<AF, Func<B, C>> Apply(Memo<AF, A> ma) =>
            AF.Apply(AF.Map(curry, mf), ma);
    }

    extension<AF, A, B, C, D>(K<AF, Func<A, B, C, D>> mf) 
        where AF : Applicative<AF>
    {
        [Pure]
        public K<AF, Func<B,Func<C, D>>> Apply(Memo<AF, A> ma) =>
            AF.Apply(AF.Map(curry, mf), ma);
    }

    extension<AF, A, B, C, D>(Memo<AF, Func<A, B, C, D>> mf) 
        where AF : Applicative<AF>
    {
        [Pure]
        public K<AF, Func<B,Func<C, D>>> Apply(Memo<AF, A> ma) =>
            AF.Apply(AF.Map(curry, mf), ma);
    }

    extension<AF, A, B, C, D, E>(K<AF, Func<A, B, C, D, E>> mf) 
        where AF : Applicative<AF>
    {
        [Pure]
        public K<AF, Func<B,Func<C, Func<D, E>>>> Apply(Memo<AF, A> ma) =>
            AF.Apply(AF.Map(curry, mf), ma);
    }

    extension<AF, A, B, C, D, E>(Memo<AF, Func<A, B, C, D, E>> mf) 
        where AF : Applicative<AF>
    {
        [Pure]
        public K<AF, Func<B,Func<C, Func<D, E>>>> Apply(Memo<AF, A> ma) =>
            AF.Apply(AF.Map(curry, mf), ma);
    }

    extension<AF, A, B, C, D, E, F>(K<AF, Func<A, B, C, D, E, F>> mf) 
        where AF : Applicative<AF>
    {
        [Pure]
        public K<AF, Func<B,Func<C, Func<D, Func<E, F>>>>> Apply(Memo<AF, A> ma) =>
            AF.Apply(AF.Map(curry, mf), ma);
    }

    extension<AF, A, B, C, D, E, F>(Memo<AF, Func<A, B, C, D, E, F>> mf) 
        where AF : Applicative<AF>
    {
        [Pure]
        public K<AF, Func<B,Func<C, Func<D, Func<E, F>>>>> Apply(Memo<AF, A> ma) =>
            AF.Apply(AF.Map(curry, mf), ma);
    }

    extension<AF, A, B, C, D, E, F, G>(K<AF, Func<A, B, C, D, E, F, G>> mf) 
        where AF : Applicative<AF>
    {
        [Pure]
        public K<AF, Func<B,Func<C, Func<D, Func<E, Func<F, G>>>>>> Apply(Memo<AF, A> ma) =>
            AF.Apply(AF.Map(curry, mf), ma);
    }

    extension<AF, A, B, C, D, E, F, G>(Memo<AF, Func<A, B, C, D, E, F, G>> mf) 
        where AF : Applicative<AF>
    {
        [Pure]
        public K<AF, Func<B,Func<C, Func<D, Func<E, Func<F, G>>>>>> Apply(Memo<AF, A> ma) =>
            AF.Apply(AF.Map(curry, mf), ma);
    }

    extension<AF, A, B, C, D, E, F, G, H>(K<AF, Func<A, B, C, D, E, F, G, H>> mf) 
        where AF : Applicative<AF>
    {
        [Pure]
        public K<AF, Func<B,Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> Apply(Memo<AF, A> ma) =>
            AF.Apply(AF.Map(curry, mf), ma);
    }

    extension<AF, A, B, C, D, E, F, G, H>(Memo<AF, Func<A, B, C, D, E, F, G, H>> mf) 
        where AF : Applicative<AF>
    {
        [Pure]
        public K<AF, Func<B,Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> Apply(Memo<AF, A> ma) =>
            AF.Apply(AF.Map(curry, mf), ma);
    }

    extension<AF, A, B, C, D, E, F, G, H, I>(K<AF, Func<A, B, C, D, E, F, G, H, I>> mf) 
        where AF : Applicative<AF>
    {
        [Pure]
        public K<AF, Func<B,Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> Apply(Memo<AF, A> ma) =>
            AF.Apply(AF.Map(curry, mf), ma);
    }

    extension<AF, A, B, C, D, E, F, G, H, I>(Memo<AF, Func<A, B, C, D, E, F, G, H, I>> mf) 
        where AF : Applicative<AF>
    {
        [Pure]
        public K<AF, Func<B,Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> Apply(Memo<AF, A> ma) =>
            AF.Apply(AF.Map(curry, mf), ma);
    }

    extension<AF, A, B, C, D, E, F, G, H, I, J>(K<AF, Func<A, B, C, D, E, F, G, H, I, J>> mf) 
        where AF : Applicative<AF>
    {
        [Pure]
        public K<AF, Func<B,Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> Apply(Memo<AF, A> ma) =>
            AF.Apply(AF.Map(curry, mf), ma);
    }

    extension<AF, A, B, C, D, E, F, G, H, I, J>(Memo<AF, Func<A, B, C, D, E, F, G, H, I, J>> mf) 
        where AF : Applicative<AF>
    {
        [Pure]
        public K<AF, Func<B,Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> Apply(Memo<AF, A> ma) =>
            AF.Apply(AF.Map(curry, mf), ma);
    }

    extension<AF, A, B, C, D, E, F, G, H, I, J, K>(K<AF, Func<A, B, C, D, E, F, G, H, I, J, K>> mf) 
        where AF : Applicative<AF>
    {
        [Pure]
        public K<AF, Func<B,Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> Apply(Memo<AF, A> ma) =>
            AF.Apply(AF.Map(curry, mf), ma);
    }

    extension<AF, A, B, C, D, E, F, G, H, I, J, K>(Memo<AF, Func<A, B, C, D, E, F, G, H, I, J, K>> mf) 
        where AF : Applicative<AF>
    {
        [Pure]
        public K<AF, Func<B,Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> Apply(Memo<AF, A> ma) =>
            AF.Apply(AF.Map(curry, mf), ma);
    }
}
