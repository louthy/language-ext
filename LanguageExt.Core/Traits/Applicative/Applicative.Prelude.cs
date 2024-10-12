using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class Prelude
{
    [Pure]
    public static K<M, B> applyM<M, A, B>(K<M, Func<A, K<M, B>>> mf, K<M, A> ma)
        where M : Monad<M> =>
        M.Apply(mf, ma).Flatten();
    
    [Pure]
    public static K<AF, B> apply<AF, A, B>(K<AF, Func<A, B>> mf, K<AF, A> ma)
        where AF : Applicative<AF> =>
        AF.Apply(mf, ma);

    [Pure]
    public static K<AF, Func<B, C>> apply<AF, A, B, C>(K<AF, Func<A, B, C>> mf, K<AF, A> ma)
        where AF : Applicative<AF> =>
        AF.Apply(AF.Map(curry, mf), ma);

    [Pure]
    public static K<AF, Func<B,Func<C, D>>> apply<AF, A, B, C, D>(K<AF, Func<A, B, C, D>> mf, K<AF, A> ma)
        where AF : Applicative<AF> =>
        AF.Apply(AF.Map(curry, mf), ma);

    [Pure]
    public static K<AF, Func<B,Func<C, Func<D, E>>>> apply<AF, A, B, C, D, E>(K<AF, Func<A, B, C, D, E>> mf, K<AF, A> ma)
        where AF : Applicative<AF> =>
        AF.Apply(AF.Map(curry, mf), ma);

    [Pure]
    public static K<AF, Func<B,Func<C, Func<D, Func<E, F>>>>> apply<AF, A, B, C, D, E, F>(K<AF, Func<A, B, C, D, E, F>> mf, K<AF, A> ma)
        where AF : Applicative<AF> =>
        AF.Apply(AF.Map(curry, mf), ma);

    [Pure]
    public static K<AF, Func<B,Func<C, Func<D, Func<E, Func<F, G>>>>>> apply<AF, A, B, C, D, E, F, G>(K<AF, Func<A, B, C, D, E, F, G>> mf, K<AF, A> ma)
        where AF : Applicative<AF> =>
        AF.Apply(AF.Map(curry, mf), ma);

    [Pure]
    public static K<AF, Func<B,Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> apply<AF, A, B, C, D, E, F, G, H>(K<AF, Func<A, B, C, D, E, F, G, H>> mf, K<AF, A> ma)
        where AF : Applicative<AF> =>
        AF.Apply(AF.Map(curry, mf), ma);

    [Pure]
    public static K<AF, Func<B,Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> apply<AF, A, B, C, D, E, F, G, H, I>(K<AF, Func<A, B, C, D, E, F, G, H, I>> mf, K<AF, A> ma)
        where AF : Applicative<AF> =>
        AF.Apply(AF.Map(curry, mf), ma);

    [Pure]
    public static K<AF, Func<B,Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> apply<AF, A, B, C, D, E, F, G, H, I, J>(K<AF, Func<A, B, C, D, E, F, G, H, I, J>> mf, K<AF, A> ma)
        where AF : Applicative<AF> =>
        AF.Apply(AF.Map(curry, mf), ma);

    [Pure]
    public static K<AF, Func<B,Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> apply<AF, A, B, C, D, E, F, G, H, I, J, K>(K<AF, Func<A, B, C, D, E, F, G, H, I, J, K>> mf, K<AF, A> ma)
        where AF : Applicative<AF> =>
        AF.Apply(AF.Map(curry, mf), ma);
    
    [Pure]
    public static K<F, B> action<F, A, B>(K<F, A> ma, K<F, B> mb)
        where F : Applicative<F> =>
        F.Action(ma, mb);
    
    [Pure]
    public static K<F, A> actions<F, A>(IEnumerable<K<F, A>> ma)
        where F : Applicative<F> =>
        F.Actions(ma);
    
    [Pure]
    public static K<F, A> actions<F, A>(IAsyncEnumerable<K<F, A>> ma)
        where F : Applicative<F> =>
        F.Actions(ma);
}
