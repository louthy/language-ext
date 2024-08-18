using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// Lazy sequence monad transformer
/// </summary>
internal record StreamEnumerableT<M, A>(IEnumerable<A> items) : StreamT<M, A>
    where M : Monad<M>
{
    public override K<M, MList<A>> runListT =>
        Lift(items.GetEnumerator()).runListT;

    public override StreamT<M, A> Tail =>
        new StreamEnumerableT<M, A>(items.Skip(1));
    
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

    public override StreamT<M, B> Map<B>(Func<A, B> f) =>
        new StreamEnumerableT<M, B>(items.Select(f));

    /// <summary>
    /// Interleave the items of two streams
    /// </summary>
    /// <param name="rhs">Other stream to merge with</param>
    /// <returns>Stream transformer</returns>
    public override StreamT<M, A> Merge(StreamT<M, A> rhs) =>
        rhs switch
        {
            StreamAsyncEnumerableT<M, A> r => new StreamAsyncEnumerableT<M, A>(MergeAsync(items, r.items)),
            StreamEnumerableT<M, A> r      => new StreamEnumerableT<M, A>(MergeSync(items, r.items)),
            _                              => base.Merge(rhs)
        };
    
    static IEnumerable<A> MergeSync(IEnumerable<A> lhs, IEnumerable<A> rhs)
    {
        using var iter = lhs.GetEnumerator();
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
    static async IAsyncEnumerable<A> MergeAsync(IEnumerable<A> lhs, IAsyncEnumerable<A> rhs)
    {
        using var iter = lhs.GetEnumerator();
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

/*
/// <summary>
/// Lazy sequence monad transformer
/// </summary>
internal record StreamEnumerableItemT<M, A>(K<M, MList<A>> runListT) : StreamT<M, A>
    where M : Monad<M>
{
    public override K<M, MList<A>> runListT { get; } = runListT;

    public override StreamT<M, B> Map<B>(Func<A, B> f) =>
        new StreamEnumerableItemT<M, B>(runListT.Map(la => la.Map(f)));
    
    public override StreamT<M, A> Tail =>
        new StreamMainT<M, A>(
            from ml in runListT
            from rl in ml switch
                       {
                           MNil<A> => 
                               Empty.runListT,

                           MCons<M, A>(var h, var t) =>
                               new StreamMainT<M, A>(t).runListT,

                           MIter<M, A> iter  =>
                               iter.TailM(),
                           
                           _ => throw new NotSupportedException()
                       }
            select rl);    
}
*/
