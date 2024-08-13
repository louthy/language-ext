using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// Lazy sequence monad transformer
/// </summary>
internal record IterableAsyncEnumerableT<M, A>(IAsyncEnumerable<A> items) : IterableT<M, A>
    where M : Monad<M>
{
    public override K<M, MList<A>> runListT =>
        M.LiftIO(IO.env)
         .Bind(e => IterableEnumerableT<M, A>.Lift(items.ToBlockingEnumerable(e.Token).GetEnumerator())
                                             .runListT); 

    public override IterableT<M, B> Map<B>(Func<A, B> f) =>
        new IterableAsyncEnumerableT<M, B>(MapAsync(items, f));

    async IAsyncEnumerable<B> MapAsync<B>(IAsyncEnumerable<A> ma, Func<A, B> f)
    {
        await foreach (var a in ma)
        {
            yield return f(a);
        }
    }
}
