using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// Lazy sequence monad transformer
/// </summary>
internal record IterableEnumerableT<M, A>(IEnumerable<A> items) : IterableT<M, A>
    where M : Monad<M>
{
    public override K<M, MList<A>> runListT => 
        Lift(items.GetEnumerator()).runListT;
    
    public static IterableT<M, A> Lift(IEnumerator<A> iter)
    {
        if (iter.MoveNext())
        {
            return new IterableEnumerableItemT<M, A>(
                M.Pure(MList<A>.Iter<M>(iter.Current, iter)));
        }
        else
        {
            iter.Dispose();
            return Empty;
        }
    }

    public override IterableT<M, B> Map<B>(Func<A, B> f) =>
        new IterableEnumerableT<M, B>(items.Select(f));
}

/// <summary>
/// Lazy sequence monad transformer
/// </summary>
internal record IterableEnumerableItemT<M, A>(K<M, MList<A>> runListT) : IterableT<M, A>
    where M : Monad<M>
{
    public override K<M, MList<A>> runListT { get; } = runListT;

    public override IterableT<M, B> Map<B>(Func<A, B> f) =>
        new IterableEnumerableItemT<M, B>(runListT.Map(la => la.Map(f)));
}
