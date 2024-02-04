#pragma warning disable CS0693 // Type parameter has the same name as the type parameter from outer type
using System;
using System.Collections.Generic;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static partial class ValidationT
{
    public static Validation<Fail, Arr<B>> Traverse<Fail, A, B>(this Arr<Validation<Fail, A>> ma, Func<A, B> f)
    {
        var res  = new B[ma.Count];
        var errs = new List<Fail>();
        var ix   = 0;
        foreach (var x in ma)
        {
            if (x.IsFail)
            {
                errs.AddRange((Seq<Fail>)x);
            }
            else
            {
                res[ix] = f((A)x);                    
                ix++;
            }
        }

        return errs.Count == 0
                   ? new Arr<B>(res)
                   : Validation<Fail, Arr<B>>.Fail(Seq.FromArray(errs.ToArray()));
    }
        
    public static Validation<Fail, Either<L, B>> Traverse<L, Fail, A, B>(this Either<L, Validation<Fail, A>> ma, Func<A, B> f)
    {
        if (ma.IsLeft)
        {
            return Validation<Fail, Either<L, B>>.Success(Left(ma.LeftValue));
        }
        else
        {
            var mb = (Validation<Fail, A>)ma;
            if (mb.IsFail)
            {
                return Validation<Fail, Either<L, B>>.Fail((Seq<Fail>)mb);
            }
            else
            {
                return Validation<Fail, Either<L, B>>.Success(f((A)mb));
            }
        }
    }
        
    public static Validation<Fail, HashSet<B>> Traverse<Fail, A, B>(this HashSet<Validation<Fail, A>> ma, Func<A, B> f)
    {
        var res  = new B[ma.Count];
        var errs = new List<Fail>();
        var ix   = 0;
        foreach (var x in ma)
        {
            if (x.IsFail)
            {
                errs.AddRange((Seq<Fail>)x);
            }
            else
            {
                res[ix] = f((A)x);                    
                ix++;
            }
        }

        return errs.Count == 0
                   ? new HashSet<B>(res.AsSpan())
                   : Validation<Fail, HashSet<B>>.Fail(Seq.FromArray(errs.ToArray()));
    }

    public static Validation<Fail, Identity<B>> Traverse<Fail, A, B>(this Identity<Validation<Fail, A>> ma, Func<A, B> f)
    {
        if (ma.Value.IsFail)
        {
            return Validation<Fail, Identity<B>>.Fail((Seq<Fail>)ma.Value);
        }
        else
        {
            return Validation<Fail, Identity<B>>.Success(new Identity<B>(f((A)ma.Value)));
        }
    }

    public static Validation<Fail, Lst<B>> Traverse<Fail, A, B>(this Lst<Validation<Fail, A>> ma, Func<A, B> f)
    {
        var res  = new B[ma.Count];
        var errs = new List<Fail>();
        var ix   = 0;
        foreach (var x in ma)
        {
            if (x.IsFail)
            {
                errs.AddRange((Seq<Fail>)x);
            }
            else
            {
                res[ix] = f((A)x);                    
                ix++;
            }
        }

        return errs.Count == 0
                   ? new Lst<B>(res.AsSpan())
                   : Validation<Fail, Lst<B>>.Fail(Seq.FromArray(errs.ToArray()));
    }
        
    public static Validation<Fail, Fin<B>> Traverse<Fail, A, B>(this Fin<Validation<Fail, A>> ma, Func<A, B> f)
    {
        if (ma.IsFail)
        {
            return Validation<Fail, Fin<B>>.Success(ma.Cast<B>());
        }
        else
        {
            var mb = (Validation<Fail, A>)ma;
            if (mb.IsFail)
            {
                return Validation<Fail, Fin<B>>.Fail((Seq<Fail>)mb);
            }
            else
            {
                return Validation<Fail, Fin<B>>.Success(f((A)mb));
            }
        }
    }        
        
    public static Validation<Fail, Option<B>> Traverse<Fail, A, B>(this Option<Validation<Fail, A>> ma, Func<A, B> f)
    {
        if (ma.IsNone)
        {
            return Validation<Fail, Option<B>>.Success(Option<B>.None);
        }
        else
        {
            var mb = (Validation<Fail, A>)ma;
            if (mb.IsFail)
            {
                return Validation<Fail, Option<B>>.Fail((Seq<Fail>)mb);
            }
            else
            {
                return Validation<Fail, Option<B>>.Success(f((A)mb));
            }
        }
    }        
        
    public static Validation<Fail, Que<B>> Traverse<Fail, A, B>(this Que<Validation<Fail, A>> ma, Func<A, B> f)
    {
        var res  = new B[ma.Count];
        var errs = new List<Fail>();
        var ix   = 0;
        foreach (var x in ma)
        {
            if (x.IsFail)
            {
                errs.AddRange((Seq<Fail>)x);
            }
            else
            {
                res[ix] = f((A)x);                    
                ix++;
            }
        }

        return errs.Count == 0
                   ? new Que<B>(res.AsSpan())
                   : Validation<Fail, Que<B>>.Fail(Seq.FromArray(errs.ToArray()));
    }
        
    public static Validation<Fail, Seq<B>> Traverse<Fail, A, B>(this Seq<Validation<Fail, A>> ma, Func<A, B> f)
    {
        var res  = new B[ma.Count];
        var errs = new List<Fail>();
        var ix   = 0;
        foreach (var x in ma)
        {
            if (x.IsFail)
            {
                errs.AddRange((Seq<Fail>)x);
            }
            else
            {
                res[ix] = f((A)x);                    
                ix++;
            }
        }

        return errs.Count == 0
                   ? Seq.FromArray(res)
                   : Validation<Fail, Seq<B>>.Fail(Seq.FromArray(errs.ToArray()));
    }
        
    public static Validation<Fail, IEnumerable<B>> Traverse<Fail, A, B>(this IEnumerable<Validation<Fail, A>> ma, Func<A, B> f)
    {
        var res  = new List<B>();
        var errs = new List<Fail>();
        foreach (var x in ma)
        {
            if (x.IsFail)
            {
                errs.AddRange((Seq<Fail>)x);
            }
            else
            {
                res.Add(f((A)x));                    
            }
        }

        return errs.Count == 0
                   ? Validation<Fail, IEnumerable<B>>.Success(res)
                   : Validation<Fail, IEnumerable<B>>.Fail(Seq.FromArray(errs.ToArray()));
    }
        
    public static Validation<Fail, Set<B>> Traverse<Fail, A, B>(this Set<Validation<Fail, A>> ma, Func<A, B> f)
    {
        var res  = new B[ma.Count];
        var errs = new List<Fail>();
        var ix   = 0;
        foreach (var x in ma)
        {
            if (x.IsFail)
            {
                errs.AddRange((Seq<Fail>)x);
            }
            else
            {
                res[ix] = f((A)x);                    
                ix++;
            }
        }

        return errs.Count == 0
                   ? new Set<B>(res.AsSpan())
                   : Validation<Fail, Set<B>>.Fail(Seq.FromArray(errs.ToArray()));
    }
        
    public static Validation<Fail, Stck<B>> Traverse<Fail, A, B>(this Stck<Validation<Fail, A>> ma, Func<A, B> f)
    {
        var res  = new B[ma.Count];
        var errs = new List<Fail>();
        var ix   = ma.Count - 1;
        foreach (var x in ma)
        {
            if (x.IsFail)
            {
                errs.AddRange((Seq<Fail>)x);
            }
            else
            {
                res[ix] = f((A)x);                    
                ix--;
            }
        }

        return errs.Count == 0
                   ? new Stck<B>(res.AsSpan())
                   : Validation<Fail, Stck<B>>.Fail(Seq.FromArray(errs.ToArray()));
    }
        
    public static Validation<Fail, Validation<Fail, B>> Traverse<Fail, A, B>(this Validation<Fail, Validation<Fail, A>> ma, Func<A, B> f)
    {
        if (ma.IsFail)
        {
            return Validation<Fail, Validation<Fail, B>>.Success(ma.FailValue);
        }
        else
        {
            var mb = ma.SuccessValue;
            if (mb.IsSuccess)
            {
                return Validation<Fail, Validation<Fail, B>>.Success(f((A)mb));
            }
            else
            {
                return Validation<Fail, Validation<Fail, B>>.Fail((Seq<Fail>)mb);
            }
        }
    }
        
    public static Validation<Fail, Validation<MonoidFail, Fail, B>> Traverse<MonoidFail, Fail, A, B>(this Validation<MonoidFail, Fail, Validation<Fail, A>> ma, Func<A, B> f) 
        where MonoidFail : Monoid<Fail>, Eq<Fail>
    {
        if (ma.IsFail)
        {
            return Validation<Fail, Validation<MonoidFail, Fail, B>>.Success(ma.FailValue);
        }
        else
        {
            var mb = ma.SuccessValue;
            if (mb.IsSuccess)
            {
                return Validation<Fail, Validation<MonoidFail, Fail, B>>.Success(f((A)mb));
            }
            else
            {
                return Validation<Fail, Validation<MonoidFail, Fail, B>>.Fail((Seq<Fail>)mb);
            }
        }
    }
        
    public static Validation<Fail, Eff<B>> Traverse<Fail, A, B>(this Eff<Validation<Fail, A>> ma, Func<A, B> f)
    {
        var tres = ma.Run();
            
        if (tres.IsBottom || tres.IsFail)
        {
            return Validation<Fail, Eff<B>>.Success(FailEff<B>(tres.Error));
        }
        else if (tres.Value.IsFail)
        {
            return Validation<Fail, Eff<B>>.Fail((Seq<Fail>)tres.Value);
        }
        else
        {
            return Validation<Fail, Eff<B>>.Success(SuccessEff(f((A)tres.Value)));
        }
    }
}
