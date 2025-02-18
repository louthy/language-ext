#pragma warning disable LX_StreamT

using System;
using System.Collections.Generic;
using LanguageExt.Traits;

namespace LanguageExt;

public static class AsyncEnumerableExtensions
{
    /*
    public static StreamT<M, A> AsStream<M, A>(this IAsyncEnumerable<A> ma) 
        where M : Monad<M> =>
        StreamT.lift<M, A>(ma);
    
    */
    public static async IAsyncEnumerable<B> MapAsync<A, B>(this IAsyncEnumerable<A> ma, Func<A, B> f)
    {
        await foreach (var a in ma)
        {
            yield return f(a);
        }
    }
    
    public static async IAsyncEnumerable<B> ApplyAsync<A, B>(this IAsyncEnumerable<Func<A, B>> ff, IAsyncEnumerable<A> fa)
    {
        await foreach (var f in ff)
        {
            // ReSharper disable once PossibleMultipleEnumeration
            await foreach (var a in fa)
            {
                yield return f(a);
            }
        }
    }
    
    public static async IAsyncEnumerable<B> BindAsync<A, B>(this IAsyncEnumerable<A> ma, Func<A, IAsyncEnumerable<B>> f)
    {
        await foreach (var a in ma)
        {
            await foreach (var b in f(a))
            {
                yield return b;
            }
        }
    }
    
    public static async IAsyncEnumerable<A> FilterAsync<A>(
        this IAsyncEnumerable<A> ma, 
        Func<A, bool> f)
    {
        await foreach (var a in ma)
        {
            if(f(a)) yield return a;
        }
    }
}
