using System;
using System.Collections.Generic;
using LanguageExt.Traits;

namespace LanguageExt.Async.Linq;

public static class AsyncEnumerableExtensions
{
    public static async IAsyncEnumerable<B> Select<A, B>(this IAsyncEnumerable<A> ma, Func<A, B> f)
    {
        await foreach (var a in ma)
        {
            yield return f(a);
        }
    }
    
    public static async IAsyncEnumerable<B> SelectMany<A, B>(this IAsyncEnumerable<A> ma, Func<A, IAsyncEnumerable<B>> f)
    {
        await foreach (var a in ma)
        {
            await foreach (var b in f(a))
            {
                yield return b;
            }
        }
    }
    
    public static async IAsyncEnumerable<C> SelectMany<A, B, C>(
        this IAsyncEnumerable<A> ma, 
        Func<A, IAsyncEnumerable<B>> bind,
        Func<A, B, C> project)
    {
        await foreach (var a in ma)
        {
            await foreach (var b in bind(a))
            {
                yield return project(a, b);
            }
        }
    }
        
    public static async IAsyncEnumerable<A> Where<A>(
        this IAsyncEnumerable<A> ma, 
        Func<A, bool> f)
    {
        await foreach (var a in ma)
        {
            if(f(a)) yield return a;
        }
    }
        
    public static async IAsyncEnumerable<A> Skip<A>(this IAsyncEnumerable<A> ma, int amount)
    {
        await foreach (var a in ma)
        {
            if (amount == 0)
                yield return a;
            else
                amount--;
        }
    }
        
    public static async IAsyncEnumerable<A> Take<A>(this IAsyncEnumerable<A> ma, int amount)
    {
        await foreach (var a in ma)
        {
            if (amount > 0) 
                yield return a;
            else
                amount--;
        }
    }
}
