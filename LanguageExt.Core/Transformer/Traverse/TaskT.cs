#pragma warning disable CS0693 // Type parameter has the same name as the type parameter from outer type
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LanguageExt.Common;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static partial class TaskT
{
    //
    // Collections
    //
 
    public static async Task<Arr<B>> Traverse<A, B>(this Arr<Task<A>> ma, Func<A, B> f)
    {
        var rb = await Task.WhenAll(ma.Map(async a => f(await a))).ConfigureAwait(false);
        return new Arr<B>(rb);
    }
        
    public static async Task<HashSet<B>> Traverse<A, B>(this HashSet<Task<A>> ma, Func<A, B> f)
    {
        var rb = await Task.WhenAll(ma.Map(async a => f(await a))).ConfigureAwait(false);
        return new HashSet<B>(rb.AsSpan());
    }
        
    [Obsolete("use TraverseSerial or TraverseParallel instead")]
    public static Task<IEnumerable<B>> Traverse<A, B>(this IEnumerable<Task<A>> ma, Func<A, B> f) =>
        TraverseParallel(ma, f);

    public static async Task<IEnumerable<B>> TraverseSerial<A, B>(this IEnumerable<Task<A>> ma, Func<A, B> f)
    {
        var rb = new List<B>();
        foreach (var a in ma)
        {
            rb.Add(f(await a.ConfigureAwait(false)));
        }
        return rb;            
    }

    public static Task<IEnumerable<B>> TraverseParallel<A, B>(this IEnumerable<Task<A>> ma, int windowSize, Func<A, B> f) =>
        ma.WindowMap(windowSize, f).Map(xs => (IEnumerable<B>)xs);

    public static Task<IEnumerable<B>> TraverseParallel<A, B>(this IEnumerable<Task<A>> ma, Func<A, B> f) =>
        ma.WindowMap(f).Map(xs => (IEnumerable<B>)xs);
                      
    [Obsolete("use TraverseSerial or TraverseParallel instead")]
    public static Task<IEnumerable<A>> Sequence<A>(this IEnumerable<Task<A>> ma) =>
        TraverseParallel(ma, identity);
 
    public static Task<IEnumerable<A>> SequenceSerial<A>(this IEnumerable<Task<A>> ma) =>
        TraverseSerial(ma, identity);
 
    public static Task<IEnumerable<A>> SequenceParallel<A>(this IEnumerable<Task<A>> ma) =>
        TraverseParallel(ma, identity);

    public static Task<IEnumerable<A>> SequenceParallel<A>(this IEnumerable<Task<A>> ma, int windowSize) =>
        TraverseParallel(ma, windowSize, identity);
 
    public static async Task<Lst<B>> Traverse<A, B>(this Lst<Task<A>> ma, Func<A, B> f)
    {
        var rb = await Task.WhenAll(ma.Map(async a => f(await a))).ConfigureAwait(false);
        return new Lst<B>(rb.AsSpan());
    }

    public static Task<Lst<A>> Sequence<A>(this Lst<Task<A>> ma) =>
        ma.Traverse(identity);
 
    public static async Task<Que<B>> Traverse<A, B>(this Que<Task<A>> ma, Func<A, B> f)
    {
        var rb = await Task.WhenAll(ma.Map(async a => f(await a))).ConfigureAwait(false);
        return new Que<B>(rb.AsSpan());
    }

    public static Task<Que<A>> Sequence<A>(this Que<Task<A>> ma) =>
        ma.Traverse(identity);

    [Obsolete("use TraverseSerial or TraverseParallel instead")]
    public static Task<Seq<B>> Traverse<A, B>(this Seq<Task<A>> ma, Func<A, B> f) =>
        TraverseParallel(ma, f);
 
    public static async Task<Seq<B>> TraverseSerial<A, B>(this Seq<Task<A>> ma, Func<A, B> f)
    {
        var rb = new List<B>();
        foreach (var a in ma)
        {
            rb.Add(f(await a.ConfigureAwait(false)));
        }
        return Seq.FromArray(rb.ToArray());
    }
        
    public static Task<Seq<B>> TraverseParallel<A, B>(this Seq<Task<A>> ma, int windowSize, Func<A, B> f) =>
        ma.WindowMap(windowSize, f).Map(toSeq);        

    public static Task<Seq<B>> TraverseParallel<A, B>(this Seq<Task<A>> ma, Func<A, B> f) =>
        ma.WindowMap(f).Map(toSeq);
        
    [Obsolete("use TraverseSerial or TraverseParallel instead")]
    public static Task<Seq<A>> Sequence<A>(this Seq<Task<A>> ma) =>
        TraverseParallel(ma, identity);
 
    public static Task<Seq<A>> SequenceSerial<A>(this Seq<Task<A>> ma) =>
        TraverseSerial(ma, identity);
 
    public static Task<Seq<A>> SequenceParallel<A>(this Seq<Task<A>> ma) =>
        TraverseParallel(ma, identity);

    public static Task<Seq<A>> SequenceParallel<A>(this Seq<Task<A>> ma, int windowSize) =>
        TraverseParallel(ma, windowSize, identity);

    public static async Task<Set<B>> Traverse<A, B>(this Set<Task<A>> ma, Func<A, B> f)
    {
        var rb = await Task.WhenAll(ma.Map(async a => f(await a))).ConfigureAwait(false);
        return new Set<B>(rb.AsSpan());
    }

    public static async Task<Stck<B>> Traverse<A, B>(this Stck<Task<A>> ma, Func<A, B> f)
    {
        var rb = await Task.WhenAll(ma.Reverse().Map(async a => f(await a))).ConfigureAwait(false);
        return new Stck<B>(rb.AsSpan());
    }
        
    public static Task<Stck<A>> Sequence<A>(this Stck<Task<A>> ma) =>
        ma.Traverse(identity);

    //
    // Async types
    //

    public static async Task<Task<B>> Traverse<A, B>(this Task<Task<A>> ma, Func<A, B> f)
    {
        var da = await ma.ConfigureAwait(false);
        var a  = await da.ConfigureAwait(false);
        return TaskSucc(f(a));
    }

    public static async Task<ValueTask<B>> Traverse<A, B>(this ValueTask<Task<A>> ma, Func<A, B> f)
    {
        var da = await ma.ConfigureAwait(false);
        var a  = await da.ConfigureAwait(false);
        return ValueTaskSucc(f(a));
    }


    //
    // Sync types
    // 
        
    public static async Task<Either<L, B>> Traverse<L, A, B>(this Either<L, Task<A>> ma, Func<A, B> f)
    {
        if (ma.IsBottom) return Either<L, B>.Bottom;
        else if (ma.IsLeft) return Either<L, B>.Left(ma.LeftValue);
        return Either<L, B>.Right(f(await ma.RightValue.ConfigureAwait(false)));
    }

    public static async Task<Identity<B>> Traverse<A, B>(this Identity<Task<A>> ma, Func<A, B> f) =>
        new (f(await ma.Value.ConfigureAwait(false)));
        
    public static async Task<Fin<B>> Traverse<A, B>(this Fin<Task<A>> ma, Func<A, B> f)
    {
        if (ma.IsFail) return ma.Cast<B>();
        return Fin<B>.Succ(f(await ma.Value.ConfigureAwait(false)));
    }
        
    public static async Task<Option<B>> Traverse<A, B>(this Option<Task<A>> ma, Func<A, B> f)
    {
        if (ma.IsNone) return Option<B>.None;
        return Option<B>.Some(f(await ma.Value!.ConfigureAwait(false)));
    }
        
    public static async Task<Validation<Fail, B>> Traverse<Fail, A, B>(this Validation<Fail, Task<A>> ma, Func<A, B> f)
    {
        if (ma.IsFail) return Validation<Fail, B>.Fail(ma.FailValue);
        return Validation<Fail, B>.Success(f(await ma.SuccessValue.ConfigureAwait(false)));
    }
        
    public static async Task<Validation<MonoidFail, Fail, B>> Traverse<MonoidFail, Fail, A, B>(this Validation<MonoidFail, Fail, Task<A>> ma, Func<A, B> f)
        where MonoidFail : Monoid<Fail>, Eq<Fail>
    {
        if (ma.IsFail) return Validation<MonoidFail, Fail, B>.Fail(ma.FailValue);
        return Validation<MonoidFail, Fail, B>.Success(f(await ma.SuccessValue.ConfigureAwait(false)));
    }
        
    public static async Task<Eff<B>> Traverse<A, B>(this Eff<Task<A>> ma, Func<A, B> f)
    {
        var mr = ma.Run();
        if (mr.IsBottom) return FailEff<B>(BottomException.Default);
        else if (mr.IsFail) return FailEff<B>(mr.Error);
        return SuccessEff(f(await mr.Value.ConfigureAwait(false)));
    }
}
