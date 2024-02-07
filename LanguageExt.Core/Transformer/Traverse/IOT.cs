#pragma warning disable CS0693 // Type parameter has the same name as the type parameter from outer type
using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using LanguageExt.Effects;
using LanguageExt.TypeClasses;
using LanguageExt.Effects.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static partial class IOT
{
    //
    // Collections
    //

    [Pure]
    public static IO<E, Arr<B>> TraverseParallel<E, A, B>(this Arr<IO<E, A>> ma, Func<A, B> f) =>
        TraverseParallel(ma, f, SysInfo.DefaultAsyncSequenceParallelism);
 
    [Pure]
    public static IO<E, Arr<B>> TraverseParallel<E, A, B>(this Arr<IO<E, A>> ma, Func<A, B> f, int windowSize) =>
        IO<E, Arr<B>>.LiftIO(async env =>
                                 {
                                     var rs = await ma.AsEnumerable()
                                                      .Map(m => m.RunAsync(env))
                                                      .WindowMap(windowSize, fa => fa.Map(f))
                                                      .ConfigureAwait(false);;

                                     var (fails, succs) = rs.Partition();
                                     return fails.Any()
                                                ? Either<E, Arr<B>>.Left(fails.Head())
                                                : Either<E, Arr<B>>.Right(toArray(succs));
                                 });

    [Pure]
    public static IO<E, Arr<B>> TraverseSerial<E, A, B>(this Arr<IO<E, A>> ma, Func<A, B> f) =>
        IO<E, Arr<B>>.Lift(env =>
                               {
                                   var rs = new List<B>();
                                   foreach (var m in ma)
                                   {
                                       var r = m.Run(env);
                                       if (r.IsLeft) return r.LeftValue;
                                       rs.Add(f(r.RightValue));
                                   }
                                   return new Arr<B>(rs.ToArray());
                               });

    [Pure]
    public static IO<E, HashSet<B>> TraverseParallel<E, A, B>(this HashSet<IO<E, A>> ma, Func<A, B> f) =>
        TraverseParallel(ma, f, SysInfo.DefaultAsyncSequenceParallelism);
 
    [Pure]
    public static IO<E, HashSet<B>> TraverseParallel<E, A, B>(this HashSet<IO<E, A>> ma, Func<A, B> f, int windowSize) =>
        IO<E, HashSet<B>>.LiftIO(async env =>
                                     {
                                         var rs = await ma.AsEnumerable()
                                                          .Map(m => m.RunAsync(env)).WindowMap(windowSize, fa => fa.Map(f))
                                                          .ConfigureAwait(false);

                                         var (fails, succs) = rs.Partition();
                                         return fails.Any()
                                                    ? Either<E, HashSet<B>>.Left(fails.Head())
                                                    : Either<E, HashSet<B>>.Right(toHashSet(succs));
                                     });

    [Pure]
    public static IO<E, HashSet<B>> TraverseSerial<E, A, B>(this HashSet<IO<E, A>> ma, Func<A, B> f) =>
        IO<E, HashSet<B>>.Lift(env =>
                                   {
                                       var rs = new List<B>();
                                       foreach (var m in ma)
                                       {
                                           var r = m.Run(env);
                                           if (r.IsLeft) return r.LeftValue;
                                           rs.Add(f(r.RightValue));
                                       }
                                       return new HashSet<B>(rs);
                                   });
        

    [Pure]
    public static IO<E, IEnumerable<B>> TraverseParallel<E, A, B>(this IEnumerable<IO<E, A>> ma, Func<A, B> f) =>
        TraverseParallel(ma, f, SysInfo.DefaultAsyncSequenceParallelism);
 
    [Pure]
    public static IO<E, IEnumerable<B>> TraverseParallel<E, A, B>(this IEnumerable<IO<E, A>> ma, Func<A, B> f, int windowSize) =>
        IO<E, IEnumerable<B>>.LiftIO(async env =>
                                         {
                                             var rs = await ma.AsEnumerable()
                                                              .Map(m => m.RunAsync(env))
                                                              .WindowMap(windowSize, fa => fa.Map(f))
                                                              .ConfigureAwait(false);

                                             var (fails, succs) = rs.Partition();
                                             return fails.Any()
                                                        ? Either<E, IEnumerable<B>>.Left(fails.Head())
                                                        : Either<E, IEnumerable<B>>.Right(succs);
                                         });

    [Pure]
    public static IO<E, IEnumerable<B>> TraverseSerial<E, A, B>(this IEnumerable<IO<E, A>> ma, Func<A, B> f) =>
        IO<E, IEnumerable<B>>.Lift(env =>
                                       {
                                           var rs = new List<B>();
                                           foreach (var m in ma)
                                           {
                                               var r = m.Run(env);
                                               if (r.IsLeft) return r.LeftValue;
                                               rs.Add(f(r.RightValue));
                                           }
                                           return Either<E, IEnumerable<B>>.Right(rs);
                                       });

        
    [Pure]
    public static IO<E, Lst<B>> TraverseParallel<E, A, B>(this Lst<IO<E, A>> ma, Func<A, B> f) =>
        TraverseParallel(ma, f, SysInfo.DefaultAsyncSequenceParallelism);
 
    [Pure]
    public static IO<E, Lst<B>> TraverseParallel<E, A, B>(this Lst<IO<E, A>> ma, Func<A, B> f, int windowSize) =>
        IO<E, Lst<B>>.LiftIO(async env =>
                                 {
                                     var rs = await ma.AsEnumerable()
                                                      .Map(m => m.RunAsync(env))
                                                      .WindowMap(windowSize, fa => fa.Map(f))
                                                      .ConfigureAwait(false);

                                     var (fails, succs) = rs.Partition();
                                     return fails.Any()
                                                ? Either<E, Lst<B>>.Left(fails.Head())
                                                : Either<E, Lst<B>>.Right(toList(succs));
                                 });

    [Pure]
    public static IO<E, Lst<B>> TraverseSerial<E, A, B>(this Lst<IO<E, A>> ma, Func<A, B> f) =>
        IO<E, Lst<B>>.Lift(env =>
                               {
                                   var rs = new List<B>();
                                   foreach (var m in ma)
                                   {
                                       var r = m.Run(env);
                                       if (r.IsLeft) return r.LeftValue;
                                       rs.Add(f(r.RightValue));
                                   }
                                   return rs.Freeze();
                               });
 
    [Pure]
    public static IO<E, Que<B>> TraverseParallel<E, A, B>(this Que<IO<E, A>> ma, Func<A, B> f) =>
        TraverseParallel(ma, f, SysInfo.DefaultAsyncSequenceParallelism);
 
    [Pure]
    public static IO<E, Que<B>> TraverseParallel<E, A, B>(this Que<IO<E, A>> ma, Func<A, B> f, int windowSize) =>
        IO<E, Que<B>>.LiftIO(async env =>
                                 {
                                     var rs = await ma.AsEnumerable()
                                                      .Map(m => m.RunAsync(env))
                                                      .WindowMap(windowSize, fa => fa.Map(f))
                                                      .ConfigureAwait(false);

                                     var (fails, succs) = rs.Partition();
                                     return fails.Any()
                                                ? Either<E, Que<B>>.Left(fails.Head())
                                                : Either<E, Que<B>>.Right(toQueue(succs));
                                 });

    [Pure]
    public static IO<E, Que<B>> TraverseSerial<E, A, B>(this Que<IO<E, A>> ma, Func<A, B> f) =>
        IO<E, Que<B>>.Lift(env =>
                               {
                                   var rs = new List<B>();
                                   foreach (var m in ma)
                                   {
                                       var r = m.Run(env);
                                       if (r.IsLeft) return r.LeftValue;
                                       rs.Add(f(r.RightValue));
                                   }
                                   return toQueue(rs);
                               });
        
    [Pure]
    public static IO<E, Seq<B>> TraverseParallel<E, A, B>(this Seq<IO<E, A>> ma, Func<A, B> f) =>
        TraverseParallel(ma, f, SysInfo.DefaultAsyncSequenceParallelism);
 
    [Pure]
    public static IO<E, Seq<B>> TraverseParallel<E, A, B>(this Seq<IO<E, A>> ma, Func<A, B> f, int windowSize) =>
        IO<E, Seq<B>>.LiftIO(async env =>
                                 {
                                     var rs = await ma.AsEnumerable()
                                                      .Map(m => m.RunAsync(env))
                                                      .WindowMap(windowSize, fa => fa.Map(f))
                                                      .ConfigureAwait(false);

                                     var (fails, succs) = rs.Partition();
                                     return fails.Any()
                                                ? Either<E, Seq<B>>.Left(fails.Head())
                                                : Either<E, Seq<B>>.Right(Seq.FromArray(succs.ToArray()));
                                 });

    [Pure]
    public static IO<E, Seq<B>> TraverseSerial<E, A, B>(this Seq<IO<E, A>> ma, Func<A, B> f) =>
        IO<E, Seq<B>>.Lift(env =>
                               {
                                   var rs = new List<B>();
                                   foreach (var m in ma)
                                   {
                                       var r = m.Run(env);
                                       if (r.IsLeft) return r.LeftValue;
                                       rs.Add(f(r.RightValue));
                                   }
                                   return Seq.FromArray(rs.ToArray());
                               });
 
    [Pure]
    public static IO<E, Set<B>> TraverseParallel<E, A, B>(this Set<IO<E, A>> ma, Func<A, B> f) =>
        TraverseParallel(ma, f, SysInfo.DefaultAsyncSequenceParallelism);
 
    [Pure]
    public static IO<E, Set<B>> TraverseParallel<E, A, B>(this Set<IO<E, A>> ma, Func<A, B> f, int windowSize) =>
        IO<E, Set<B>>.LiftIO(async env =>
                                 {
                                     var rs = await ma.AsEnumerable()
                                                      .Map(m => m.RunAsync(env))
                                                      .WindowMap(windowSize, fa => fa.Map(f))
                                                      .ConfigureAwait(false);

                                     var (fails, succs) = rs.Partition();
                                     return fails.Any()
                                                ? Either<E, Set<B>>.Left(fails.Head())
                                                : Either<E, Set<B>>.Right(toSet(succs));
                                 });

    [Pure]
    public static IO<E, Set<B>> TraverseSerial<E, A, B>(this Set<IO<E, A>> ma, Func<A, B> f) =>
        IO<E, Set<B>>.Lift(env =>
                               {
                                   var rs = new List<B>();
                                   foreach (var m in ma)
                                   {
                                       var r = m.Run(env);
                                       if (r.IsLeft) return r.LeftValue;
                                       rs.Add(f(r.RightValue));
                                   }
                                   return toSet(rs);
                               });
 
    [Pure]
    public static IO<E, Stck<B>> TraverseParallel<E, A, B>(this Stck<IO<E, A>> ma, Func<A, B> f) =>
        TraverseParallel(ma, f, SysInfo.DefaultAsyncSequenceParallelism);
 
    [Pure]
    public static IO<E, Stck<B>> TraverseParallel<E, A, B>(this Stck<IO<E, A>> ma, Func<A, B> f, int windowSize) =>
        IO<E, Stck<B>>.LiftIO(async env =>
                                  {
                                      var rs = await ma.AsEnumerable()
                                                       .Map(m => m.RunAsync(env))
                                                       .WindowMap(windowSize, fa => fa.Map(f))
                                                       .ConfigureAwait(false);

                                      var (fails, succs) = rs.Partition();
                                      return fails.Any()
                                                 ? Either<E, Stck<B>>.Left(fails.Head())
                                                 : Either<E, Stck<B>>.Right(toStack(succs));
                                  });

    [Pure]
    public static IO<E, Stck<B>> TraverseSerial<E, A, B>(this Stck<IO<E, A>> ma, Func<A, B> f) =>
        IO<E, Stck<B>>.Lift(env =>
                                {
                                    var rs = new List<B>();
                                    foreach (var m in ma)
                                    {
                                        var r = m.Run(env);
                                        if (r.IsLeft) return r.LeftValue;
                                        rs.Add(f(r.RightValue));
                                    }
                                    return toStack(rs);
                                });
        
    //
    // Async types
    //

    public static IO<E, Task<B>> Traverse<E, A, B>(this Task<IO<E, A>> ma, Func<A, B> f)
    {
        return IO<E, Task<B>>.LiftIO(env => Go(env, ma, f));
        async Task<Either<E, Task<B>>> Go(MinRT<E> env, Task<IO<E, A>> ma, Func<A, B> f)
        {
            var ra = await ma.ConfigureAwait(false);
            var rb = await ra.RunAsync(env).ConfigureAwait(false);
            if (rb.IsLeft) return rb.LeftValue;
            return f(rb.RightValue).AsTask();
        }
    }

    public static IO<E, ValueTask<B>> Traverse<E, A, B>(this ValueTask<IO<E, A>> ma, Func<A, B> f)
    {
        return IO<E, ValueTask<B>>.LiftIO(env => Go(env, ma, f));
        async Task<Either<E, ValueTask<B>>> Go(MinRT<E> env, ValueTask<IO<E, A>> ma, Func<A, B> f)
        {
            var ra = await ma.ConfigureAwait(false);
            var rb = await ra.RunAsync(env).ConfigureAwait(false);
            if (rb.IsLeft) return rb.LeftValue;
            return f(rb.RightValue).AsValueTask();
        }
    }
        
    //
    // Sync types
    // 
        
    public static IO<L, Either<L, B>> Traverse<L, A, B>(this Either<L, IO<L, A>> ma, Func<A, B> f)
    {
        return IO<L, Either<L, B>>.Lift(env => Go(env, ma, f));
        Either <L, Either<L, B>> Go(MinRT<L> env, Either<L, IO<L, A>> ma, Func<A, B> f)
        {
            if(ma.IsBottom) return default;
            if(ma.IsLeft) return ma.LeftValue;
            var rb = ma.RightValue.Run(env);
            if(rb.IsLeft) return rb.LeftValue;
            return Either<L, B>.Right(f(rb.RightValue));
        }
    }

    public static IO<E, Identity<B>> Traverse<E, A, B>(this Identity<IO<E, A>> ma, Func<A, B> f)
    {
        return IO<E, Identity<B>>.Lift(env => Go(env, ma, f));
        Either<E, Identity<B>> Go(MinRT<E> env, Identity<IO<E, A>> ma, Func<A, B> f)
        {
            var rb = ma.Value.Run(env);
            if(rb.IsLeft) return rb.LeftValue;
            return new Identity<B>(f(rb.RightValue));
        }
    }

    public static IO<E, Fin<B>> Traverse<E, A, B>(this Fin<IO<E, A>> ma, Func<A, B> f)
    {
        return IO<E, Fin<B>>.Lift(env => Go(env, ma, f));
        Either<E, Fin<B>> Go(MinRT<E> env, Fin<IO<E, A>> ma, Func<A, B> f)
        {
            if(ma.IsFail) return Either<E, Fin<B>>.Right(ma.Error);
            var rb = ma.Value.Run(env);
            if(rb.IsLeft) return rb.LeftValue;
            return Fin<B>.Succ(f(rb.RightValue));
        }
    }

    public static IO<E, Option<B>> Traverse<E, A, B>(this Option<IO<E, A>> ma, Func<A, B> f)
    {
        return IO<E, Option<B>>.Lift(env => Go(env, ma, f));
        Either<E, Option<B>> Go(MinRT<E> env, Option<IO<E, A>> ma, Func<A, B> f)
        {
            if(ma.IsNone) return Either<E, Option<B>>.Right(None);
            var rb = ma.Value.Run(env);
            if(rb.IsLeft) return rb.LeftValue;
            return Option<B>.Some(f(rb.RightValue));
        }
    }
        
    public static IO<Fail, Validation<Fail, B>> Traverse<Fail, A, B>(
        this Validation<Fail, IO<Fail, A>> ma, Func<A, B> f)
    {
        return IO<Fail, Validation<Fail, B>>.Lift(env => Go(env, ma, f));
        Either<Fail, Validation<Fail, B>> Go(MinRT<Fail> env, Validation<Fail, IO<Fail, A>> ma, Func<A, B> f)
        {
            if (ma.IsFail) return Either<Fail, Validation<Fail, B>>.Right(Fail<Fail, B>(ma.FailValue));
            var rb = ma.SuccessValue.Run(env);
            if(rb.IsLeft) return rb.LeftValue;
            return Either<Fail, Validation<Fail, B>>.Right(f(rb.RightValue));
        }
    }
        
    public static IO<Fail, Validation<MonoidFail, Fail, B>> Traverse<MonoidFail, Fail, A, B>(
        this Validation<MonoidFail, Fail, IO<Fail, A>> ma, Func<A, B> f)
        where MonoidFail : Monoid<Fail>, Eq<Fail>
    {
        return IO<Fail, Validation<MonoidFail, Fail, B>>.Lift(env => Go(env, ma, f));
        Either<Fail, Validation<MonoidFail, Fail, B>> Go(MinRT<Fail> env, Validation<MonoidFail, Fail, IO<Fail, A>> ma, Func<A, B> f)
        {
            if (ma.IsFail) return Either<Fail, Validation<MonoidFail, Fail, B>>.Right(Fail<MonoidFail, Fail, B>(ma.FailValue));
            var rb = ma.SuccessValue.Run(env);
            if(rb.IsLeft) return rb.LeftValue;
            return Either<Fail, Validation<MonoidFail, Fail, B>>.Right(f(rb.RightValue));
        }
    }
}
