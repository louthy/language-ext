using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LanguageExt.Traits;

namespace LanguageExt.Async.Linq;

public static class AsyncEnumerableExtensions
{
    extension<A>(IAsyncEnumerable<A> ma)
    {
        public async IAsyncEnumerable<B> Select<B>(Func<A, ValueTask<B>> f)
        {
            await foreach (var a in ma)
            {
                yield return await f(a);
            }
        }

        public async IAsyncEnumerable<B> SelectMany<B>(Func<A, IAsyncEnumerable<B>> f)
        {
            await foreach (var a in ma)
            {
                await foreach (var b in f(a))
                {
                    yield return b;
                }
            }
        }

        public async IAsyncEnumerable<C> SelectMany<B, C>(Func<A, IAsyncEnumerable<B>> bind,
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

        public async IAsyncEnumerable<A> Where(Func<A, bool> f)
        {
            await foreach (var a in ma)
            {
                if(f(a)) yield return a;
            }
        }

        public async IAsyncEnumerable<A> Skip(int amount)
        {
            await foreach (var a in ma)
            {
                if (amount == 0)
                    yield return a;
                else
                    amount--;
            }
        }

        public async IAsyncEnumerable<A> Take(int amount)
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
}
