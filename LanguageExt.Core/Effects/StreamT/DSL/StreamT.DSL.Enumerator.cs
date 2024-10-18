using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// Lazy sequence monad transformer
/// </summary>
internal record StreamEnumeratorT<M, A>(IEnumerator<A> items) : StreamT<M, A>
    where M : Monad<M>
{
    public override K<M, MList<A>> runListT =>
        Lift(items).runListT;

    public override StreamT<M, A> Tail() =>
        items.MoveNext()
            ? new StreamEnumeratorT<M, A>(items)
            : Empty;

    public static StreamT<M, A> Lift(IEnumerator<A> iter)
    {
        if (iter.MoveNext())
        {
            return new StreamMainT<M, A>(M.Pure(MList<A>.Iter<M>(iter.Current, iter)));
        }
        else
        {
            iter.Dispose();
            return Empty;
        }
    }

    public override StreamT<M, B> Map<B>(Func<A, B> f)
    {
        return new StreamEnumerableT<M, B>(go());
        IEnumerable<B> go()
        {
            while (items.MoveNext())
            {
                yield return f(items.Current);
            }
        }
    }

    /// <summary>
    /// Interleave the items of two streams
    /// </summary>
    /// <param name="rhs">Other stream to merge with</param>
    /// <returns>Stream transformer</returns>
    public override StreamT<M, A> Merge(K<StreamT<M>, A> rhs) =>
        rhs switch
        {
            StreamAsyncEnumerableT<M, A> r => new StreamAsyncEnumerableT<M, A>(MergeAsync(items, r.items)),
            StreamEnumerableT<M, A> r      => new StreamEnumerableT<M, A>(MergeSync(items, r.items)),
            StreamEnumeratorT<M, A> r      => new StreamEnumerableT<M, A>(MergeSync(items, r.items)),
            _                              => base.Merge(rhs)
        };
    
    static IEnumerable<A> MergeSync(IEnumerator<A> lhs, IEnumerator<A> rhs)
    {
        using var iter = lhs;
        while(rhs.MoveNext())
        {
            var a = rhs.Current;
            if (iter.MoveNext())
            {
                yield return iter.Current;
                yield return a;
            }
            else
            {
                yield return a;
            }
        }
        while (iter.MoveNext())
        {
            yield return iter.Current;
        }
    } 
    
    static IEnumerable<A> MergeSync(IEnumerator<A> lhs, IEnumerable<A> rhs)
    {
        using var iter = lhs;
        foreach(var a in rhs)
        {
            if (iter.MoveNext())
            {
                yield return iter.Current;
                yield return a;
            }
            else
            {
                yield return a;
            }
        }
        while (iter.MoveNext())
        {
            yield return iter.Current;
        }
    }
    
    static async IAsyncEnumerable<A> MergeAsync(IEnumerator<A> lhs, IAsyncEnumerable<A> rhs)
    {
        using var iter = lhs;
        await foreach(var a in rhs)
        {
            if (iter.MoveNext())
            {
                yield return iter.Current;
                yield return a;
            }
            else
            {
                yield return a;
            }
        }
        while (iter.MoveNext())
        {
            yield return iter.Current;
        }
    }    
}
