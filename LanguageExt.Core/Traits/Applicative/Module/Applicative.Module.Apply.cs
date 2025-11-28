using System;
using System.Diagnostics.Contracts;
using static LanguageExt.Prelude;

namespace LanguageExt.Traits;

public static partial class Applicative
{
    [Pure]
    public static K<AF, B> apply<AF, A, B>(K<AF, Func<A, B>> mf, K<AF, A> ma) where AF : Applicative<AF> =>
        AF.Apply(mf, ma);

    [Pure]
    public static K<AF, Func<B, C>> apply<AF, A, B, C>(K<AF, Func<A, B, C>> mf, K<AF, A> ma) where AF : Applicative<AF> =>
        AF.Apply(AF.Map(curry, mf), ma);

    [Pure]
    public static K<AF, Func<B,Func<C, D>>> apply<AF, A, B, C, D>(K<AF, Func<A, B, C, D>> mf, K<AF, A> ma) where AF : Applicative<AF> =>
        AF.Apply(AF.Map(curry, mf), ma);

    [Pure]
    public static K<AF, Func<B,Func<C, Func<D, E>>>> apply<AF, A, B, C, D, E>(K<AF, Func<A, B, C, D, E>> mf, K<AF, A> ma) where AF : Applicative<AF> =>
        AF.Apply(AF.Map(curry, mf), ma);

    [Pure]
    public static K<AF, Func<B,Func<C, Func<D, Func<E, F>>>>> apply<AF, A, B, C, D, E, F>(K<AF, Func<A, B, C, D, E, F>> mf, K<AF, A> ma) where AF : Applicative<AF> =>
        AF.Apply(AF.Map(curry, mf), ma);

    [Pure]
    public static K<AF, Func<B,Func<C, Func<D, Func<E, Func<F, G>>>>>> apply<AF, A, B, C, D, E, F, G>(K<AF, Func<A, B, C, D, E, F, G>> mf, K<AF, A> ma) where AF : Applicative<AF> =>
        AF.Apply(AF.Map(curry, mf), ma);

    [Pure]
    public static K<AF, Func<B,Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> apply<AF, A, B, C, D, E, F, G, H>(K<AF, Func<A, B, C, D, E, F, G, H>> mf, K<AF, A> ma) where AF : Applicative<AF> =>
        AF.Apply(AF.Map(curry, mf), ma);

    [Pure]
    public static K<AF, Func<B,Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> apply<AF, A, B, C, D, E, F, G, H, I>(K<AF, Func<A, B, C, D, E, F, G, H, I>> mf, K<AF, A> ma) where AF : Applicative<AF> =>
        AF.Apply(AF.Map(curry, mf), ma);

    [Pure]
    public static K<AF, Func<B,Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> apply<AF, A, B, C, D, E, F, G, H, I, J>(K<AF, Func<A, B, C, D, E, F, G, H, I, J>> mf, K<AF, A> ma) where AF : Applicative<AF> =>
        AF.Apply(AF.Map(curry, mf), ma);

    [Pure]
    public static K<AF, Func<B,Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> apply<AF, A, B, C, D, E, F, G, H, I, J, K>(K<AF, Func<A, B, C, D, E, F, G, H, I, J, K>> mf, K<AF, A> ma) where AF : Applicative<AF> =>
        AF.Apply(AF.Map(curry, mf), ma);

    [Pure]
    public static K<AF, R> apply<AF, A, B, R>(
        (K<AF, A>, K<AF, B>) items,
        Func<A, B, R> f)
        where AF : Applicative<AF> =>
        f.Map(items.Item1)
         .Apply(items.Item2);

    [Pure]
    public static K<AF, R> apply<AF, A, B, C, R>(
        (K<AF, A>, K<AF, B>, K<AF, C>) items, Func<A, B, C, R> f)
        where AF : Applicative<AF> =>
        f.Map(items.Item1)
         .Apply(items.Item2)
         .Apply(items.Item3);

    [Pure]
    public static K<AF, R> apply<AF, A, B, C, D, R>(
        (K<AF, A>, K<AF, B>, K<AF, C>, K<AF, D>) items,
        Func<A, B, C, D, R> f)
        where AF : Applicative<AF> =>
        f.Map(items.Item1)
         .Apply(items.Item2)
         .Apply(items.Item3)
         .Apply(items.Item4);

    [Pure]
    public static K<AF, R> apply<AF, A, B, C, D, E, R>(
        (K<AF, A>, K<AF, B>, K<AF, C>, K<AF, D>, K<AF, E>) items,
        Func<A, B, C, D, E, R> f)
        where AF : Applicative<AF> =>
        f.Map(items.Item1)
         .Apply(items.Item2)
         .Apply(items.Item3)
         .Apply(items.Item4)
         .Apply(items.Item5);

    [Pure]
    public static K<AF, R> apply<AF, A, B, C, D, E, F, R>(
        (K<AF, A>, K<AF, B>, K<AF, C>, K<AF, D>, K<AF, E>, K<AF, F>) items,
        Func<A, B, C, D, E, F, R> f)
        where AF : Applicative<AF> =>
        f.Map(items.Item1)
         .Apply(items.Item2)
         .Apply(items.Item3)
         .Apply(items.Item4)
         .Apply(items.Item5)
         .Apply(items.Item6);

    [Pure]
    public static K<AF, R> apply<AF, A, B, C, D, E, F, G, R>(
        (K<AF, A>, K<AF, B>, K<AF, C>, K<AF, D>, K<AF, E>, K<AF, F>, K<AF, G>) items,
        Func<A, B, C, D, E, F, G, R> f)
        where AF : Applicative<AF> =>
        f.Map(items.Item1)
         .Apply(items.Item2)
         .Apply(items.Item3)
         .Apply(items.Item4)
         .Apply(items.Item5)
         .Apply(items.Item6)
         .Apply(items.Item7);

    [Pure]
    public static K<AF, R> apply<AF, A, B, C, D, E, F, G, H, R>(
        (K<AF, A>, K<AF, B>, K<AF, C>, K<AF, D>, K<AF, E>, K<AF, F>, K<AF, G>, K<AF, H>) items,
        Func<A, B, C, D, E, F, G, H, R> f)
        where AF : Applicative<AF> =>
        f.Map(items.Item1)
         .Apply(items.Item2)
         .Apply(items.Item3)
         .Apply(items.Item4)
         .Apply(items.Item5)
         .Apply(items.Item6)
         .Apply(items.Item7)
         .Apply(items.Item8);

    [Pure]
    public static K<AF, R> apply<AF, A, B, C, D, E, F, G, H, I, R>(
        (K<AF, A>,
              K<AF, B>,
              K<AF, C>,
              K<AF, D>,
              K<AF, E>,
              K<AF, F>,
              K<AF, G>,
              K<AF, H>,
              K<AF, I>) items,
        Func<A, B, C, D, E, F, G, H, I, R> f)
        where AF : Applicative<AF> =>
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
    public static K<AF, R> apply<AF, A, B, C, D, E, F, G, H, I, J, R>(
        (K<AF, A>,
            K<AF, B>,
            K<AF, C>,
            K<AF, D>,
            K<AF, E>,
            K<AF, F>,
            K<AF, G>,
            K<AF, H>,
            K<AF, I>,
            K<AF, J>) items,
        Func<A, B, C, D, E, F, G, H, I, J, R> f)
        where AF : Applicative<AF> =>
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
