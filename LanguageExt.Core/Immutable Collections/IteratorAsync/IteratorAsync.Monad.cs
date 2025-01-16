using System;
using LanguageExt.Traits;
using G = System.Collections.Generic;
using static LanguageExt.Prelude;

namespace LanguageExt;

public partial class IteratorAsync : 
    Monad<IteratorAsync>
{
    static K<IteratorAsync, B> Monad<IteratorAsync>.Bind<A, B>(K<IteratorAsync, A> ma, Func<A, K<IteratorAsync, B>> f) =>
        ma.As().Bind(f);

    static K<IteratorAsync, B> Functor<IteratorAsync>.Map<A, B>(Func<A, B> f, K<IteratorAsync, A> ma) => 
        ma.As().Map(f);

    static K<IteratorAsync, A> Applicative<IteratorAsync>.Pure<A>(A value) =>
        Cons(value, Nil<A>());

    static K<IteratorAsync, B> Applicative<IteratorAsync>.Apply<A, B>(K<IteratorAsync, Func<A, B>> mf, K<IteratorAsync, A> ma) =>
        mf.Bind(ma.Map);   

    static K<IteratorAsync, B> Applicative<IteratorAsync>.Action<A, B>(K<IteratorAsync, A> ma, K<IteratorAsync, B> mb)
    {
        ignore(ma.As().Count);
        return mb;
    }
}
