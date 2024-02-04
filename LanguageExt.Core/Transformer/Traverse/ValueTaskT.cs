#pragma warning disable CS0693 // Type parameter has the same name as the type parameter from outer type
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LanguageExt.Common;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static partial class ValueTaskT
{
    //
    // Collections
    //
 
    public static async ValueTask<Arr<B>> Traverse<A, B>(this Arr<ValueTask<A>> ma, Func<A, B> f)
    {
        var rb = await Task.WhenAll(ma.Map(async a => f(await a.ConfigureAwait(false)))).ConfigureAwait(false);
        return new Arr<B>(rb);
    }
        
    public static async ValueTask<HashSet<B>> Traverse<A, B>(this HashSet<ValueTask<A>> ma, Func<A, B> f)
    {
        var rb = await Task.WhenAll(ma.Map(async a => f(await a.ConfigureAwait(false)))).ConfigureAwait(false);
        return new HashSet<B>(rb.AsSpan());
    }
        
    [Obsolete("use TraverseSerial or TraverseParallel instead")]
    public static ValueTask<IEnumerable<B>> Traverse<A, B>(this IEnumerable<ValueTask<A>> ma, Func<A, B> f) =>
        TraverseParallel(ma, f);

    public static async ValueTask<IEnumerable<B>> TraverseSerial<A, B>(this IEnumerable<ValueTask<A>> ma, Func<A, B> f)
    {
        var rb = new List<B>();
        foreach (var a in ma)
        {
            rb.Add(f(await a.ConfigureAwait(false)));
        }
        return rb;            
    }

    public static ValueTask<IEnumerable<B>> TraverseParallel<A, B>(this IEnumerable<ValueTask<A>> ma, int windowSize, Func<A, B> f) =>
        ma.WindowMap(windowSize, f).Map(xs => (IEnumerable<B>)xs);

    public static ValueTask<IEnumerable<B>> TraverseParallel<A, B>(this IEnumerable<ValueTask<A>> ma, Func<A, B> f) =>
        ma.WindowMap(f).Map(xs => (IEnumerable<B>)xs);
                      
    [Obsolete("use TraverseSerial or TraverseParallel instead")]
    public static ValueTask<IEnumerable<A>> Sequence<A>(this IEnumerable<ValueTask<A>> ma) =>
        TraverseParallel(ma, identity);
 
    public static ValueTask<IEnumerable<A>> SequenceSerial<A>(this IEnumerable<ValueTask<A>> ma) =>
        TraverseSerial(ma, identity);
 
    public static ValueTask<IEnumerable<A>> SequenceParallel<A>(this IEnumerable<ValueTask<A>> ma) =>
        TraverseParallel(ma, identity);

    public static ValueTask<IEnumerable<A>> SequenceParallel<A>(this IEnumerable<ValueTask<A>> ma, int windowSize) =>
        TraverseParallel(ma, windowSize, identity);
 
    public static async ValueTask<Lst<B>> Traverse<A, B>(this Lst<ValueTask<A>> ma, Func<A, B> f)
    {
        var rb = await Task.WhenAll(ma.Map(async a => f(await a.ConfigureAwait(false)))).ConfigureAwait(false);
        return new Lst<B>(rb.AsSpan());
    }
        
    public static async ValueTask<Que<B>> Traverse<A, B>(this Que<ValueTask<A>> ma, Func<A, B> f)
    {
        var rb = await Task.WhenAll(ma.Map(async a => f(await a.ConfigureAwait(false)))).ConfigureAwait(false);
        return new Que<B>(rb.AsSpan());
    }

    [Obsolete("use TraverseSerial or TraverseParallel instead")]
    public static ValueTask<Seq<B>> Traverse<A, B>(this Seq<ValueTask<A>> ma, Func<A, B> f) =>
        TraverseParallel(ma, f);
 
    public static async ValueTask<Seq<B>> TraverseSerial<A, B>(this Seq<ValueTask<A>> ma, Func<A, B> f)
    {
        var rb = new List<B>();
        foreach (var a in ma)
        {
            rb.Add(f(await a.ConfigureAwait(false)));
        }
        return Seq.FromArray(rb.ToArray());
    }
        
    public static ValueTask<Seq<B>> TraverseParallel<A, B>(this Seq<ValueTask<A>> ma, int windowSize, Func<A, B> f) =>
        ma.WindowMap(windowSize, f).Map(toSeq);        

    public static ValueTask<Seq<B>> TraverseParallel<A, B>(this Seq<ValueTask<A>> ma, Func<A, B> f) =>
        ma.WindowMap(f).Map(toSeq);
        
    [Obsolete("use TraverseSerial or TraverseParallel instead")]
    public static ValueTask<Seq<A>> Sequence<A>(this Seq<ValueTask<A>> ma) =>
        TraverseParallel(ma, identity);
 
    public static ValueTask<Seq<A>> SequenceSerial<A>(this Seq<ValueTask<A>> ma) =>
        TraverseSerial(ma, identity);
 
    public static ValueTask<Seq<A>> SequenceParallel<A>(this Seq<ValueTask<A>> ma) =>
        TraverseParallel(ma, identity);

    public static ValueTask<Seq<A>> SequenceParallel<A>(this Seq<ValueTask<A>> ma, int windowSize) =>
        TraverseParallel(ma, windowSize, identity);

    public static async ValueTask<Set<B>> Traverse<A, B>(this Set<ValueTask<A>> ma, Func<A, B> f)
    {
        var rb = await Task.WhenAll(ma.Map(async a => f(await a.ConfigureAwait(false)))).ConfigureAwait(false);
        return new Set<B>(rb.AsSpan());
    }

    public static async ValueTask<Stck<B>> Traverse<A, B>(this Stck<ValueTask<A>> ma, Func<A, B> f)
    {
        var rb = await Task.WhenAll(ma.Reverse().Map(async a => f(await a.ConfigureAwait(false)))).ConfigureAwait(false);
        return new Stck<B>(rb.AsSpan());
    }

    //
    // Async types
    //

    public static async ValueTask<Task<B>> Traverse<A, B>(this Task<ValueTask<A>> ma, Func<A, B> f)
    {
        var da = await ma.ConfigureAwait(false);
        var a  = await da.ConfigureAwait(false);
        return f(a).AsTask();
    }

    public static async ValueTask<ValueTask<B>> Traverse<A, B>(this ValueTask<ValueTask<A>> ma, Func<A, B> f)
    {
        var da = await ma.ConfigureAwait(false);
        var a  = await da.ConfigureAwait(false);
        return ValueTaskSucc(f(a));
    }

    //
    // Sync types
    // 
        
    public static async ValueTask<Either<L, B>> Traverse<L, A, B>(this Either<L, ValueTask<A>> ma, Func<A, B> f)
    {
        if (ma.IsBottom) return Either<L, B>.Bottom;
        else if (ma.IsLeft) return Either<L, B>.Left(ma.LeftValue);
        return Either<L, B>.Right(f(await ma.RightValue.ConfigureAwait(false)));
    }

    public static async ValueTask<Identity<B>> Traverse<A, B>(this Identity<ValueTask<A>> ma, Func<A, B> f) =>
        new (f(await ma.Value.ConfigureAwait(false)));
        
    public static async ValueTask<Fin<B>> Traverse<A, B>(this Fin<ValueTask<A>> ma, Func<A, B> f)
    {
        if (ma.IsFail) return ma.Cast<B>();
        return Fin<B>.Succ(f(await ma.Value.ConfigureAwait(false)));
    }
        
    public static async ValueTask<Option<B>> Traverse<A, B>(this Option<ValueTask<A>> ma, Func<A, B> f)
    {
        if (ma.IsNone) return Option<B>.None;
        return Option<B>.Some(f(await ma.Value.ConfigureAwait(false)));
    }
        
    public static async ValueTask<Validation<Fail, B>> Traverse<Fail, A, B>(this Validation<Fail, ValueTask<A>> ma, Func<A, B> f)
    {
        if (ma.IsFail) return Validation<Fail, B>.Fail(ma.FailValue);
        return Validation<Fail, B>.Success(f(await ma.SuccessValue.ConfigureAwait(false)));
    }
        
    public static async ValueTask<Validation<MonoidFail, Fail, B>> Traverse<MonoidFail, Fail, A, B>(this Validation<MonoidFail, Fail, ValueTask<A>> ma, Func<A, B> f)
        where MonoidFail : Monoid<Fail>, Eq<Fail>
    {
        if (ma.IsFail) return Validation<MonoidFail, Fail, B>.Fail(ma.FailValue);
        return Validation<MonoidFail, Fail, B>.Success(f(await ma.SuccessValue.ConfigureAwait(false)));
    }
        
    public static async ValueTask<Eff<B>> Traverse<A, B>(this Eff<ValueTask<A>> ma, Func<A, B> f)
    {
        var mr = ma.Run();
        if (mr.IsBottom) return FailEff<B>(BottomException.Default);
        else if (mr.IsFail) return FailEff<B>(mr.Error);
        return SuccessEff(f(await mr.Value.ConfigureAwait(false)));
    }
}
