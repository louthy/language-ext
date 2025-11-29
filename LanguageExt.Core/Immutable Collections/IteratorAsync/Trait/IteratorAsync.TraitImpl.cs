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

    static K<IteratorAsync, B> Applicative<IteratorAsync>.Apply<A, B>(K<IteratorAsync, Func<A, B>> mf, K<IteratorAsync, A> ma) 
    {
        return from(go());

        async G.IAsyncEnumerable<B> go()
        {
            for (var fi = mf.As().Clone(); !await fi.IsEmpty; fi = await fi.Tail)
            {
                for (var ai = ma.As().Clone(); !await ai.IsEmpty; ai = await ai.Tail)
                {
                    yield return (await fi.Head)(await ai.Head);
                }
            }
        }        
    }   
    
    static K<IteratorAsync, B> Applicative<IteratorAsync>.Apply<A, B>(
        K<IteratorAsync, Func<A, B>> mf, Memo<IteratorAsync, A> ma)
    {
        return from(go());

        async G.IAsyncEnumerable<B> go()
        {
            for (var fi = mf.As().Clone(); !await fi.IsEmpty; fi = await fi.Tail)
            {
                for (var ai = ma.Value.As().Clone(); !await ai.IsEmpty; ai = await ai.Tail)
                {
                    yield return (await fi.Head)(await ai.Head);
                }
            }
        }        
    }   
}
