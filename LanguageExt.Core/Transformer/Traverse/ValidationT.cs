#pragma warning disable CS0693 // Type parameter has the same name as the type parameter from outer type
using System;
using System.Collections.Generic;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static partial class ValidationT
{
    public static Validation<MonoidFail, Fail, Arr<B>> Traverse<MonoidFail, Fail, A, B>(this Arr<Validation<MonoidFail, Fail, A>> ma, Func<A, B> f)
        where MonoidFail : Monoid<Fail>, Eq<Fail>
    {
        var res       = new B[ma.Count];
        var errs      = MonoidFail.Empty();
        var isSuccess = true;
        var ix        = 0;
        foreach (var x in ma)
        {
            if (x.IsFail)
            {
                errs = MonoidFail.Append(errs, x.FailValue);
                isSuccess = false;
            }
            else
            {
                res[ix] = f((A)x);                    
                ix++;
            }
        }

        return isSuccess
                   ? new Arr<B>(res)
                   : Validation<MonoidFail, Fail, Arr<B>>.Fail(errs);
    }
        
    public static Validation<MonoidFail, Fail, Either<Fail, B>> Traverse<MonoidFail, Fail, A, B>(this Either<Fail, Validation<MonoidFail, Fail, A>> ma, Func<A, B> f)
        where MonoidFail : Monoid<Fail>, Eq<Fail>
    {
        if (ma.IsLeft)
        {
            return Validation<MonoidFail, Fail, Either<Fail, B>>.Success(ma.LeftValue);
        }
        else
        {
            var mb = (Validation<MonoidFail, Fail, A>)ma;
            if (mb.IsFail)
            {
                return Validation<MonoidFail, Fail, Either<Fail, B>>.Fail(mb.FailValue);
            }
            else
            {
                return Validation<MonoidFail, Fail, Either<Fail, B>>.Success(f((A)mb));
            }
        }
    }
        
    public static Validation<MonoidFail, Fail, HashSet<B>> Traverse<MonoidFail, Fail, A, B>(this HashSet<Validation<MonoidFail, Fail, A>> ma, Func<A, B> f)
        where MonoidFail : Monoid<Fail>, Eq<Fail>
    {
        var res       = new B[ma.Count];
        var errs      = MonoidFail.Empty();
        var isSuccess = true;
        var ix        = 0;
        foreach (var x in ma)
        {
            if (x.IsFail)
            {
                errs = MonoidFail.Append(errs, x.FailValue);
                isSuccess = false;
            }
            else
            {
                res[ix] = f((A)x);                    
                ix++;
            }
        }

        return isSuccess
                   ? new HashSet<B>(res.AsSpan())
                   : Validation<MonoidFail, Fail, HashSet<B>>.Fail(errs);
    }

    public static Validation<MonoidFail, Fail, Identity<B>> Traverse<MonoidFail, Fail, A, B>(this Identity<Validation<MonoidFail, Fail, A>> ma, Func<A, B> f)
        where MonoidFail : Monoid<Fail>, Eq<Fail>
    {
        if (ma.Value.IsFail)
        {
            return Validation<MonoidFail, Fail, Identity<B>>.Fail(ma.Value.FailValue);
        }
        else
        {
            return Validation<MonoidFail, Fail, Identity<B>>.Success(Id(f((A)ma.Value)));
        }
    }

    public static Validation<MonoidFail, Fail, Lst<B>> Traverse<MonoidFail, Fail, A, B>(this Lst<Validation<MonoidFail, Fail, A>> ma, Func<A, B> f)
        where MonoidFail : Monoid<Fail>, Eq<Fail>
    {
        var res       = new B[ma.Count];
        var errs      = MonoidFail.Empty();
        var isSuccess = true;
        var ix        = 0;
        foreach (var x in ma)
        {
            if (x.IsFail)
            {
                errs = MonoidFail.Append(errs, x.FailValue);
                isSuccess = false;
            }
            else
            {
                res[ix] = f((A)x);                    
                ix++;
            }
        }

        return isSuccess
                   ? new Lst<B>(res.AsSpan())
                   : Validation<MonoidFail, Fail, Lst<B>>.Fail(errs);
    }
        
    public static Validation<MonoidFail, Fail, Fin<B>> Traverse<MonoidFail, Fail, A, B>(this Fin<Validation<MonoidFail, Fail, A>> ma, Func<A, B> f)
        where MonoidFail : Monoid<Fail>, Eq<Fail>
    {
        if (ma.IsFail)
        {
            return Validation<MonoidFail, Fail, Fin<B>>.Success(ma.Cast<B>());
        }
        else
        {
            var mb = (Validation<MonoidFail, Fail, A>)ma;
            if (mb.IsFail)
            {
                return Validation<MonoidFail, Fail, Fin<B>>.Fail(mb.FailValue);
            }
            else
            {
                return Validation<MonoidFail, Fail, Fin<B>>.Success(f((A)mb));
            }
        }
    }        
        
    public static Validation<MonoidFail, Fail, Option<B>> Traverse<MonoidFail, Fail, A, B>(this Option<Validation<MonoidFail, Fail, A>> ma, Func<A, B> f)
        where MonoidFail : Monoid<Fail>, Eq<Fail>
    {
        if (ma.IsNone)
        {
            return Validation<MonoidFail, Fail, Option<B>>.Success(Option<B>.None);
        }
        else
        {
            var mb = (Validation<MonoidFail, Fail, A>)ma;
            if (mb.IsFail)
            {
                return Validation<MonoidFail, Fail, Option<B>>.Fail(mb.FailValue);
            }
            else
            {
                return Validation<MonoidFail, Fail, Option<B>>.Success(f((A)mb));
            }
        }
    }        
        
    public static Validation<MonoidFail, Fail, Que<B>> Traverse<MonoidFail, Fail, A, B>(this Que<Validation<MonoidFail, Fail, A>> ma, Func<A, B> f)
        where MonoidFail : Monoid<Fail>, Eq<Fail>
    {
        var res       = new B[ma.Count];
        var errs      = MonoidFail.Empty();
        var isSuccess = true;
        var ix        = 0;
        foreach (var x in ma)
        {
            if (x.IsFail)
            {
                errs = MonoidFail.Append(errs, x.FailValue);
                isSuccess = false;
            }
            else
            {
                res[ix] = f((A)x);                    
                ix++;
            }
        }

        return isSuccess
                   ? new Que<B>(res.AsSpan())
                   : Validation<MonoidFail, Fail, Que<B>>.Fail(errs);
    }
        
    public static Validation<MonoidFail, Fail, Seq<B>> Traverse<MonoidFail, Fail, A, B>(this Seq<Validation<MonoidFail, Fail, A>> ma, Func<A, B> f)
        where MonoidFail : Monoid<Fail>, Eq<Fail>
    {
        var res       = new B[ma.Count];
        var errs      = MonoidFail.Empty();
        var isSuccess = true;
        var ix        = 0;
        foreach (var x in ma)
        {
            if (x.IsFail)
            {
                errs = MonoidFail.Append(errs, x.FailValue);
                isSuccess = false;
            }
            else
            {
                res[ix] = f((A)x);                    
                ix++;
            }
        }

        return isSuccess
                   ? Seq.FromArray(res)
                   : Validation<MonoidFail, Fail, Seq<B>>.Fail(errs);
    }
                
    public static Validation<MonoidFail, Fail, IEnumerable<B>> Traverse<MonoidFail, Fail, A, B>(this IEnumerable<Validation<MonoidFail, Fail, A>> ma, Func<A, B> f)
        where MonoidFail : Monoid<Fail>, Eq<Fail>
    {
        var res       = new List<B>();
        var errs      = MonoidFail.Empty();
        var isSuccess = true;
        foreach (var x in ma)
        {
            if (x.IsFail)
            {
                errs = MonoidFail.Append(errs, x.FailValue);
                isSuccess = false;
            }
            else
            {
                res.Add(f((A)x));                    
            }
        }

        return isSuccess
                   ? Validation<MonoidFail, Fail, IEnumerable<B>>.Success(res)
                   : Validation<MonoidFail, Fail, IEnumerable<B>>.Fail(errs);
    }
        
    public static Validation<MonoidFail, Fail, Set<B>> Traverse<MonoidFail, Fail, A, B>(this Set<Validation<MonoidFail, Fail, A>> ma, Func<A, B> f)
        where MonoidFail : Monoid<Fail>, Eq<Fail>
    {
        var res       = new B[ma.Count];
        var errs      = MonoidFail.Empty();
        var isSuccess = true;
        var ix        = 0;
        foreach (var x in ma)
        {
            if (x.IsFail)
            {
                errs = MonoidFail.Append(errs, x.FailValue);
                isSuccess = false;
            }
            else
            {
                res[ix] = f((A)x);                    
                ix++;
            }
        }

        return isSuccess
                   ? new Set<B>(res.AsSpan())
                   : Validation<MonoidFail, Fail, Set<B>>.Fail(errs);
    }
        
    public static Validation<MonoidFail, Fail, Stck<B>> Traverse<MonoidFail, Fail, A, B>(this Stck<Validation<MonoidFail, Fail, A>> ma, Func<A, B> f)
        where MonoidFail : Monoid<Fail>, Eq<Fail>
    {
        var res       = new B[ma.Count];
        var errs      = MonoidFail.Empty();
        var isSuccess = true;
        var ix        = ma.Count - 1;
        foreach (var x in ma)
        {
            if (x.IsFail)
            {
                errs = MonoidFail.Append(errs, x.FailValue);
                isSuccess = false;
            }
            else
            {
                res[ix] = f((A)x);                    
                ix--;
            }
        }

        return isSuccess
                   ? new Stck<B>(res.AsSpan())
                   : Validation<MonoidFail, Fail, Stck<B>>.Fail(errs);
    }
        
    public static Validation<MonoidFail, Fail, Validation<Fail, B>> Traverse<MonoidFail, Fail, A, B>(this Validation<Fail, Validation<MonoidFail, Fail, A>> ma, Func<A, B> f)
        where MonoidFail : Monoid<Fail>, Eq<Fail>
    {
        if (ma.IsFail)
        {
            return Validation<MonoidFail, Fail, Validation<Fail, B>>.Success(ma.FailValue);
        }
        else
        {
            var mb = ma.SuccessValue;
            if (mb.IsSuccess)
            {
                return Validation<MonoidFail, Fail, Validation<Fail, B>>.Success(f((A)mb));
            }
            else
            {
                return Validation<MonoidFail, Fail, Validation<Fail, B>>.Fail(mb.FailValue);
            }
        }
    }
        
    public static Validation<MonoidFail, Fail, Validation<MonoidFail, Fail, B>> Traverse<MonoidFail, Fail, A, B>(this Validation<MonoidFail, Fail, Validation<MonoidFail, Fail, A>> ma, Func<A, B> f) 
        where MonoidFail : Monoid<Fail>, Eq<Fail>
    {
        if (ma.IsFail)
        {
            return Validation<MonoidFail, Fail, Validation<MonoidFail, Fail, B>>.Success(ma.FailValue);
        }
        else
        {
            var mb = ma.SuccessValue;
            if (mb.IsSuccess)
            {
                return Validation<MonoidFail, Fail, Validation<MonoidFail, Fail, B>>.Success(f((A)mb));
            }
            else
            {
                return Validation<MonoidFail, Fail, Validation<MonoidFail, Fail, B>>.Fail(mb.FailValue);
            }
        }
    }
        
    public static Validation<MonoidFail, Fail, Eff<B>> Traverse<MonoidFail, Fail, A, B>(this Eff<Validation<MonoidFail, Fail, A>> ma, Func<A, B> f)
        where MonoidFail : Monoid<Fail>, Eq<Fail>
    {
        var tres = ma.Run();
            
        if (tres.IsBottom || tres.IsFail)
        {
            return Validation<MonoidFail, Fail, Eff<B>>.Success(FailEff<B>(tres.Error));
        }
        else if (tres.Value.IsFail)
        {
            return Validation<MonoidFail, Fail, Eff<B>>.Fail(tres.Value.FailValue);
        }
        else
        {
            return Validation<MonoidFail, Fail, Eff<B>>.Success(SuccessEff(f((A)tres.Value)));
        }
    }
}
