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
        public K<AF, B> Apply(K<AF, A> ma) =>
            AF.Apply(mf, ma);
    }

    extension<AF, A, B, C>(K<AF, Func<A, B, C>> mf) 
        where AF : Applicative<AF>
    {
        [Pure]
        public K<AF, Func<B, C>> Apply(K<AF, A> ma) =>
            AF.Apply(AF.Map(curry, mf), ma);
    }

    extension<AF, A, B, C, D>(K<AF, Func<A, B, C, D>> mf) 
        where AF : Applicative<AF>
    {
        [Pure]
        public K<AF, Func<B,Func<C, D>>> Apply(K<AF, A> ma) =>
            AF.Apply(AF.Map(curry, mf), ma);
    }

    extension<AF, A, B, C, D, E>(K<AF, Func<A, B, C, D, E>> mf) 
        where AF : Applicative<AF>
    {
        [Pure]
        public K<AF, Func<B,Func<C, Func<D, E>>>> Apply(K<AF, A> ma) =>
            AF.Apply(AF.Map(curry, mf), ma);
    }

    extension<AF, A, B, C, D, E, F>(K<AF, Func<A, B, C, D, E, F>> mf) 
        where AF : Applicative<AF>
    {
        [Pure]
        public K<AF, Func<B,Func<C, Func<D, Func<E, F>>>>> Apply(K<AF, A> ma) =>
            AF.Apply(AF.Map(curry, mf), ma);
    }

    extension<AF, A, B, C, D, E, F, G>(K<AF, Func<A, B, C, D, E, F, G>> mf) 
        where AF : Applicative<AF>
    {
        [Pure]
        public K<AF, Func<B,Func<C, Func<D, Func<E, Func<F, G>>>>>> Apply(K<AF, A> ma) =>
            AF.Apply(AF.Map(curry, mf), ma);
    }

    extension<AF, A, B, C, D, E, F, G, H>(K<AF, Func<A, B, C, D, E, F, G, H>> mf) 
        where AF : Applicative<AF>
    {
        [Pure]
        public K<AF, Func<B,Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> Apply(K<AF, A> ma) =>
            AF.Apply(AF.Map(curry, mf), ma);
    }

    extension<AF, A, B, C, D, E, F, G, H, I>(K<AF, Func<A, B, C, D, E, F, G, H, I>> mf) 
        where AF : Applicative<AF>
    {
        [Pure]
        public K<AF, Func<B,Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> Apply(K<AF, A> ma) =>
            AF.Apply(AF.Map(curry, mf), ma);
    }

    extension<AF, A, B, C, D, E, F, G, H, I, J>(K<AF, Func<A, B, C, D, E, F, G, H, I, J>> mf) 
        where AF : Applicative<AF>
    {
        [Pure]
        public K<AF, Func<B,Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> Apply(K<AF, A> ma) =>
            AF.Apply(AF.Map(curry, mf), ma);
    }

    extension<AF, A, B, C, D, E, F, G, H, I, J, K>(K<AF, Func<A, B, C, D, E, F, G, H, I, J, K>> mf) 
        where AF : Applicative<AF>
    {
        [Pure]
        public K<AF, Func<B,Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> Apply(K<AF, A> ma) =>
            AF.Apply(AF.Map(curry, mf), ma);
    }

    [Pure]
    public static K<Fnctr, R> Apply<Fnctr, A, B, R>(
        this (K<Fnctr, A>, K<Fnctr, B>) items,
        Func<A, B, R> f)
        where Fnctr : Applicative<Fnctr> =>
        f.Map(items.Item1)
         .Apply(items.Item2);

    [Pure]
    public static K<Fnctr, R> Apply<Fnctr, A, B, C, R>(
        this (K<Fnctr, A>, K<Fnctr, B>, K<Fnctr, C>) items, Func<A, B, C, R> f)
        where Fnctr : Applicative<Fnctr> =>
        f.Map(items.Item1)
         .Apply(items.Item2)
         .Apply(items.Item3);

    [Pure]
    public static K<Fnctr, R> Apply<Fnctr, A, B, C, D, R>(
        this (K<Fnctr, A>, K<Fnctr, B>, K<Fnctr, C>, K<Fnctr, D>) items,
        Func<A, B, C, D, R> f)
        where Fnctr : Applicative<Fnctr> =>
        f.Map(items.Item1)
         .Apply(items.Item2)
         .Apply(items.Item3)
         .Apply(items.Item4);

    [Pure]
    public static K<Fnctr, R> Apply<Fnctr, A, B, C, D, E, R>(
        this (K<Fnctr, A>, K<Fnctr, B>, K<Fnctr, C>, K<Fnctr, D>, K<Fnctr, E>) items,
        Func<A, B, C, D, E, R> f)
        where Fnctr : Applicative<Fnctr> =>
        f.Map(items.Item1)
         .Apply(items.Item2)
         .Apply(items.Item3)
         .Apply(items.Item4)
         .Apply(items.Item5);

    [Pure]
    public static K<Fnctr, R> Apply<Fnctr, A, B, C, D, E, F, R>(
        this (K<Fnctr, A>, K<Fnctr, B>, K<Fnctr, C>, K<Fnctr, D>, K<Fnctr, E>, K<Fnctr, F>) items,
        Func<A, B, C, D, E, F, R> f)
        where Fnctr : Applicative<Fnctr> =>
        f.Map(items.Item1)
         .Apply(items.Item2)
         .Apply(items.Item3)
         .Apply(items.Item4)
         .Apply(items.Item5)
         .Apply(items.Item6);

    [Pure]
    public static K<Fnctr, R> Apply<Fnctr, A, B, C, D, E, F, G, R>(
        this (K<Fnctr, A>, K<Fnctr, B>, K<Fnctr, C>, K<Fnctr, D>, K<Fnctr, E>, K<Fnctr, F>, K<Fnctr, G>) items,
        Func<A, B, C, D, E, F, G, R> f)
        where Fnctr : Applicative<Fnctr> =>
        f.Map(items.Item1)
         .Apply(items.Item2)
         .Apply(items.Item3)
         .Apply(items.Item4)
         .Apply(items.Item5)
         .Apply(items.Item6)
         .Apply(items.Item7);

    [Pure]
    public static K<Fnctr, R> Apply<Fnctr, A, B, C, D, E, F, G, H, R>(
        this (K<Fnctr, A>, K<Fnctr, B>, K<Fnctr, C>, K<Fnctr, D>, K<Fnctr, E>, K<Fnctr, F>, K<Fnctr, G>, K<Fnctr, H>) items,
        Func<A, B, C, D, E, F, G, H, R> f)
        where Fnctr : Applicative<Fnctr> =>
        f.Map(items.Item1)
         .Apply(items.Item2)
         .Apply(items.Item3)
         .Apply(items.Item4)
         .Apply(items.Item5)
         .Apply(items.Item6)
         .Apply(items.Item7)
         .Apply(items.Item8);

    [Pure]
    public static K<Fnctr, R> Apply<Fnctr, A, B, C, D, E, F, G, H, I, R>(
        this (K<Fnctr, A>,
              K<Fnctr, B>,
              K<Fnctr, C>,
              K<Fnctr, D>,
              K<Fnctr, E>,
              K<Fnctr, F>,
              K<Fnctr, G>,
              K<Fnctr, H>,
              K<Fnctr, I>) items,
        Func<A, B, C, D, E, F, G, H, I, R> f)
        where Fnctr : Applicative<Fnctr> =>
        f.Map(items.Item1)
         .Apply(items.Item2)
         .Apply(items.Item3)
         .Apply(items.Item4)
         .Apply(items.Item5)
         .Apply(items.Item6)
         .Apply(items.Item7)
         .Apply(items.Item8)
         .Apply(items.Item9);

    [Pure]
    public static K<Fnctr, R> Apply<Fnctr, A, B, C, D, E, F, G, H, I, J, R>(
        this (K<Fnctr, A>,
            K<Fnctr, B>,
            K<Fnctr, C>,
            K<Fnctr, D>,
            K<Fnctr, E>,
            K<Fnctr, F>,
            K<Fnctr, G>,
            K<Fnctr, H>,
            K<Fnctr, I>,
            K<Fnctr, J>) items,
        Func<A, B, C, D, E, F, G, H, I, J, R> f)
        where Fnctr : Applicative<Fnctr> =>
        f.Map(items.Item1)
         .Apply(items.Item2)
         .Apply(items.Item3)
         .Apply(items.Item4)
         .Apply(items.Item5)
         .Apply(items.Item6)
         .Apply(items.Item7)
         .Apply(items.Item8)
         .Apply(items.Item9)
         .Apply(items.Item10);
}
