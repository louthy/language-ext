#pragma warning disable CS0693 // Type parameter has the same name as the type parameter from outer type
using System;
using System.Collections.Generic;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static partial class FinT
{
    public static Fin<Arr<B>> Traverse<A, B>(this Arr<Fin<A>> ma, Func<A, B> f)
    {
        var res = new B[ma.Count];
        var ix  = 0;
        foreach (var xs in ma)
        {
            if (xs.IsFail) return xs.Cast<Arr<B>>();
            res[ix] = f(xs.Value);
            ix++;
        }

        return Fin<Arr<B>>.Succ(new Arr<B>(res));
    }

    public static Fin<Either<L, B>> Traverse<L, A, B>(this Either<L, Fin<A>> ma, Func<A, B> f)
    {
        if (ma.IsLeft)
        {
            return FinSucc(Either<L, B>.Left(ma.LeftValue));
        }
        else if (ma.RightValue.IsFail)
        {
            return ma.RightValue.Cast<Either<L, B>>();
        }
        else
        {
            return Fin<Either<L, B>>.Succ(f(ma.RightValue.Value));
        }
    }

    public static Fin<HashSet<B>> Traverse<A, B>(this HashSet<Fin<A>> ma, Func<A, B> f)
    {
        var res = new B[ma.Count];
        var ix  = 0;
        foreach (var xs in ma)
        {
            if (xs.IsFail) return xs.Cast<HashSet<B>>();
            res[ix] = f(xs.Value);
            ix++;
        }
        return Fin<HashSet<B>>.Succ(new HashSet<B>(res.AsSpan()));            
    }

    public static Fin<Identity<B>> Traverse<A, B>(this Identity<Fin<A>> ma, Func<A, B> f) =>
        ma.Value.IsSucc
            ? Fin<Identity<B>>.Succ(new Identity<B>(f(ma.Value.Value)))
            : ma.Value.Cast<Identity<B>>();
        
    public static Fin<Lst<B>> Traverse<A, B>(this Lst<Fin<A>> ma, Func<A, B> f)
    {
        var res = new B[ma.Count];
        var ix  = 0;
        foreach (var xs in ma)
        {
            if (xs.IsFail) return xs.Cast<Lst<B>>();
            res[ix] = f(xs.Value);
            ix++;
        }
        return Fin<Lst<B>>.Succ(new Lst<B>(res.AsSpan()));                
    }

    public static Fin<Option<B>> Traverse<A, B>(this Option<Fin<A>> ma, Func<A, B> f)
    {
        if (ma.IsNone)
        {
            return FinSucc<Option<B>>(None);
        }
        else if (ma.Value.IsFail)
        {
            return ma.Value.Cast<Option<B>>();
        }
        else
        {
            return FinSucc(Some(f(ma.Value.Value)));
        }
    }

    public static Fin<Fin<B>> Traverse<A, B>(this Fin<Fin<A>> ma, Func<A, B> f) =>
        ma.Bind(a => a.Map(f));
        
    public static Fin<Que<B>> Traverse<A, B>(this Que<Fin<A>> ma, Func<A, B> f)
    {
        var res = new B[ma.Count];
        var ix  = 0;
        foreach (var xs in ma)
        {
            if (xs.IsFail) return xs.Cast<Que<B>>();
            res[ix] = f(xs.Value);
            ix++;
        }
        return Fin<Que<B>>.Succ(new Que<B>(res.AsSpan()));                
    }
        
    public static Fin<Seq<B>> Traverse<A, B>(this Seq<Fin<A>> ma, Func<A, B> f)
    {
        var res = new B[ma.Count];
        var ix  = 0;
        foreach (var xs in ma)
        {
            if (xs.IsFail) return xs.Cast<Seq<B>>();
            res[ix] = f(xs.Value);
            ix++;
        }
        return Fin<Seq<B>>.Succ(Seq.FromArray(res));                
    }
                
    public static Fin<IEnumerable<B>> Traverse<A, B>(this IEnumerable<Fin<A>> ma, Func<A, B> f)
    {
        var res = new List<B>();
        foreach (var xs in ma)
        {
            if (xs.IsFail) return xs.Cast<IEnumerable<B>>();
            res.Add(f(xs.Value));
        }
        return Fin<IEnumerable<B>>.Succ(res);                
    }
        
    public static Fin<Set<B>> Traverse<A, B>(this Set<Fin<A>> ma, Func<A, B> f)
    {
        var res = new B[ma.Count];
        var ix  = 0;
        foreach (var xs in ma)
        {
            if (xs.IsFail) return xs.Cast<Set<B>>();
            res[ix] = f(xs.Value);
            ix++;
        }
        return Fin<Set<B>>.Succ(new Set<B>(res.AsSpan()));                
    }
        
    public static Fin<Stck<B>> Traverse<A, B>(this Stck<Fin<A>> ma, Func<A, B> f)
    {
        var res = new B[ma.Count];
        var ix  = ma.Count - 1;
        foreach (var xs in ma)
        {
            if (xs.IsFail) return xs.Cast<Stck<B>>();
            res[ix] = f(xs.Value);
            ix--;
        }
        return Fin<Stck<B>>.Succ(new Stck<B>(res.AsSpan()));                
    }
        
    public static Fin<Validation<Fail, B>> Traverse<Fail, A, B>(this Validation<Fail, Fin<A>> ma, Func<A, B> f)
    {
        if (ma.IsFail)
        {
            return FinSucc(Fail<Fail, B>(ma.FailValue));
        }
        else if (ma.SuccessValue.IsFail)
        {
            return ma.SuccessValue.Cast<Validation<Fail, B>>();
        }
        else
        {
            return FinSucc(Validation<Fail, B>.Success(f(ma.SuccessValue.Value)));
        }
    }

    public static Fin<Validation<MonoidFail, Fail, B>> Traverse<MonoidFail, Fail, A, B>(
        this Validation<MonoidFail, Fail, Fin<A>> ma, Func<A, B> f)
        where MonoidFail : Monoid<Fail>, Eq<Fail>
    {
        if (ma.IsFail)
        {
            return FinSucc(Fail<MonoidFail, Fail, B>(ma.FailValue));
        }
        else if (ma.SuccessValue.IsFail)
        {
            return ma.SuccessValue.Cast<Validation<MonoidFail, Fail, B>>();
        }
        else
        {
            return FinSucc(Validation<MonoidFail, Fail, B>.Success(f(ma.SuccessValue.Value)));
        }
    }
                
    public static Fin<Eff<B>> Traverse<A, B>(this Eff<Fin<A>> ma, Func<A, B> f)
    {
        var tres = ma.Run();

        if (tres.IsBottom)
        {
            return tres.Cast<Eff<B>>();
        }
        else if (tres.IsFail)
        {
            return FinSucc(FailEff<B>(tres.Error));
        }
        else if (tres.Value.IsFail)
        {
            return tres.Cast<Eff<B>>();
        }
        else
        {
            return FinSucc(SuccessEff(f(tres.Value.Value)));
        }
    }
}
