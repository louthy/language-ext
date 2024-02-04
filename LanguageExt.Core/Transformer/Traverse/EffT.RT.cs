#pragma warning disable CS0693 // Type parameter has the same name as the type parameter from outer type
using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using LanguageExt.TypeClasses;
using LanguageExt.Common;
using LanguageExt.Effects.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static partial class EffT
{
    //
    // Collections
    //

    [Pure]
    public static Eff<RT, Arr<B>> Traverse<RT, A, B>(this Arr<Eff<RT, A>> ma, Func<A, B> f)
        where RT : HasIO<RT, Error> =>
        lift((RT env) =>
             {
                 var rs = new List<B>();
                 foreach (var m in ma)
                 {
                     var r = m.Run(env);
                     if (r.IsFail) return FinFail<Arr<B>>(r.Error);
                     rs.Add(f(r.Value));
                 }

                 return FinSucc(new Arr<B>(rs.ToArray()));
             });


    [Pure]
    public static Eff<RT, HashSet<B>> Traverse<RT, A, B>(this HashSet<Eff<RT, A>> ma, Func<A, B> f)
        where RT : HasIO<RT, Error> =>
        lift((RT env) =>
             {
                 var rs = new List<B>();
                 foreach (var m in ma)
                 {
                     var r = m.Run(env);
                     if (r.IsFail) return FinFail<HashSet<B>>(r.Error);
                     rs.Add(f(r.Value));
                 }

                 return FinSucc(new HashSet<B>(rs));
             });

    [Pure]
    public static Eff<RT, IEnumerable<B>> Traverse<RT, A, B>(this IEnumerable<Eff<RT, A>> ma, Func<A, B> f)
        where RT : HasIO<RT, Error> =>
        lift((RT env) =>
             {
                 var rs = new List<B>();
                 foreach (var m in ma)
                 {
                     var r = m.Run(env);
                     if (r.IsFail) return FinFail<IEnumerable<B>>(r.Error);
                     rs.Add(f(r.Value));
                 }

                 return FinSucc(rs.AsEnumerable());
             });

    [Pure]
    public static Eff<RT, Lst<B>> Traverse<RT, A, B>(this Lst<Eff<RT, A>> ma, Func<A, B> f)
        where RT : HasIO<RT, Error> =>
        lift((RT env) =>
             {
                 var rs = new List<B>();
                 foreach (var m in ma)
                 {
                     var r = m.Run(env);
                     if (r.IsFail) return FinFail<Lst<B>>(r.Error);
                     rs.Add(f(r.Value));
                 }

                 return FinSucc(rs.Freeze());
             });

    [Pure]
    public static Eff<RT, Que<B>> Traverse<RT, A, B>(this Que<Eff<RT, A>> ma, Func<A, B> f)
        where RT : HasIO<RT, Error> =>
        lift((RT env) =>
             {
                 var rs = new List<B>();
                 foreach (var m in ma)
                 {
                     var r = m.Run(env);
                     if (r.IsFail) return FinFail<Que<B>>(r.Error);
                     rs.Add(f(r.Value));
                 }

                 return FinSucc(toQueue(rs));
             });

    [Pure]
    public static Eff<RT, Seq<B>> Traverse<RT, A, B>(this Seq<Eff<RT, A>> ma, Func<A, B> f)
        where RT : HasIO<RT, Error> =>
        lift((RT env) =>
             {
                 var rs = new List<B>();
                 foreach (var m in ma)
                 {
                     var r = m.Run(env);
                     if (r.IsFail) return FinFail<Seq<B>>(r.Error);
                     rs.Add(f(r.Value));
                 }

                 return FinSucc(Seq.FromArray(rs.ToArray()));
             });

    [Pure]
    public static Eff<RT, Set<B>> Traverse<RT, A, B>(this Set<Eff<RT, A>> ma, Func<A, B> f)
        where RT : HasIO<RT, Error> =>
        lift((RT env) =>
             {
                 var rs = new List<B>();
                 foreach (var m in ma)
                 {
                     var r = m.Run(env);
                     if (r.IsFail) return FinFail<Set<B>>(r.Error);
                     rs.Add(f(r.Value));
                 }

                 return FinSucc(toSet(rs));
             });

    [Pure]
    public static Eff<RT, Stck<B>> Traverse<RT, A, B>(this Stck<Eff<RT, A>> ma, Func<A, B> f) where RT : HasIO<RT, Error> =>
        lift((RT env) =>
                              {
                 var rs = new List<B>();
                 foreach (var m in ma)
                 {
                     var r = m.Run(env);
                     if (r.IsFail) return FinFail<Stck<B>>(r.Error);
                     rs.Add(f(r.Value));
                 }
                 return FinSucc(toStack(rs));
             });
        
    //
    // Sync types
    // 
        
    public static Eff<RT, Either<L, B>> Traverse<RT, L, A, B>(this Either<L, Eff<RT, A>> ma, Func<A, B> f)
        where RT : HasIO<RT, Error> 
    {
        return lift((RT env) => Go(env, ma, f));
        Fin<Either<L, B>> Go(RT env, Either<L, Eff<RT, A>> ma, Func<A, B> f)
        {
            if(ma.IsBottom) return default;
            if(ma.IsLeft) return FinSucc<Either<L, B>>(ma.LeftValue);
            var rb = ma.RightValue.Run(env);
            if(rb.IsFail) return FinFail<Either<L, B>>(rb.Error);
            return FinSucc<Either<L, B>>(f(rb.Value));
        }
    }


    public static Eff<RT, Identity<B>> Traverse<RT, A, B>(this Identity<Eff<RT, A>> ma, Func<A, B> f)
        where RT : HasIO<RT, Error> 
    {
        return lift((RT env) => Go(env, ma, f));
        Fin<Identity<B>> Go(RT env, Identity<Eff<RT, A>> ma, Func<A, B> f)
        {
            if(ma.IsBottom) return default;
            var rb = ma.Value.Run(env);
            if(rb.IsFail) return FinFail<Identity<B>>(rb.Error);
            return FinSucc(new Identity<B>(f(rb.Value)));
        }
    }

    public static Eff<RT, Fin<B>> Traverse<RT, A, B>(this Fin<Eff<RT, A>> ma, Func<A, B> f)
        where RT : HasIO<RT, Error> 
    {
        return lift((RT env) => Go(env, ma, f));
        Fin<Fin<B>> Go(RT env, Fin<Eff<RT, A>> ma, Func<A, B> f)
        {
            if(ma.IsFail) return FinSucc(ma.Cast<B>());
            var rb = ma.Value.Run(env);
            if(rb.IsFail) return FinFail<Fin<B>>(rb.Error);
            return FinSucc(Fin<B>.Succ(f(rb.Value)));
        }
    }
        
    public static Eff<RT, Option<B>> Traverse<RT, A, B>(this Option<Eff<RT, A>> ma, Func<A, B> f)
        where RT : HasIO<RT, Error> 
    {
        return lift((RT env) => Go(env, ma, f));
        Fin<Option<B>> Go(RT env, Option<Eff<RT, A>> ma, Func<A, B> f)
        {
            if(ma.IsNone) return FinSucc<Option<B>>(None);
            var rb = ma.Value.Run(env);
            if(rb.IsFail) return FinFail<Option<B>>(rb.Error);
            return FinSucc(Option<B>.Some(f(rb.Value)));
        }
    }
        
    public static Eff<RT, Validation<Fail, B>> Traverse<RT, Fail, A, B>(this Validation<Fail, Eff<RT, A>> ma, Func<A, B> f)
        where RT : HasIO<RT, Error>
    {
        return lift((RT env) => Go(env, ma, f));
        Fin<Validation<Fail, B>> Go(RT env, Validation<Fail, Eff<RT, A>> ma, Func<A, B> f)
        {
            if (ma.IsFail) return FinSucc(Fail<Fail, B>(ma.FailValue));
            var rb = ma.SuccessValue.Run(env);
            if(rb.IsFail) return FinFail<Validation<Fail, B>>(rb.Error);
            return FinSucc<Validation<Fail, B>>(f(rb.Value));
        }
    }
        
    public static Eff<RT, Validation<MonoidFail, Fail, B>> Traverse<RT, MonoidFail, Fail, A, B>(this Validation<MonoidFail, Fail, Eff<RT, A>> ma, Func<A, B> f)
        where RT : HasIO<RT, Error> 
        where MonoidFail : Monoid<Fail>, Eq<Fail>
    {
        return lift((RT env) => Go(env, ma, f));
        Fin<Validation<MonoidFail, Fail, B>> Go(RT env, Validation<MonoidFail, Fail, Eff<RT, A>> ma, Func<A, B> f)
        {
            if (ma.IsFail) return FinSucc(Fail<MonoidFail, Fail, B>(ma.FailValue));
            var rb = ma.SuccessValue.Run(env);
            if(rb.IsFail) return FinFail<Validation<MonoidFail, Fail, B>>(rb.Error);
            return FinSucc<Validation<MonoidFail, Fail, B>>(f(rb.Value));
        }
    }
        
    public static Eff<RT, Eff<B>> Traverse<RT, A, B>(this Eff<Eff<RT, A>> ma, Func<A, B> f)
        where RT : HasIO<RT, Error>         {
        return lift((RT env) => Go(env, ma, f));
        Fin<Eff<B>> Go(RT env, Eff<Eff<RT, A>> ma, Func<A, B> f)
        {
            var ra = ma.Run();
            if (ra.IsFail) return FinSucc(FailEff<B>(ra.Error));
            var rb = ra.Value.Run(env);
            if (rb.IsFail) return FinFail<Eff<B>>(rb.Error);
            return FinSucc(SuccessEff(f(rb.Value)));
        }
    }
}
