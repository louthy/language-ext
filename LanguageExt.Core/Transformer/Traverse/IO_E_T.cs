#nullable enable
using System;
using LanguageExt;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using LanguageExt.DataTypes.Serialisation;
using LanguageExt.TypeClasses;
using LanguageExt.ClassInstances;
using LanguageExt.Common;
using LanguageExt.Effects.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    public static partial class IO_E_T
    {
        //
        // Collections
        //

        [Pure]
        public static IO<RT, E, Arr<B>> TraverseParallel<RT, E, A, B>(this Arr<IO<RT, E, A>> ma, Func<A, B> f)
            where RT : HasIO<RT, E> =>
            TraverseParallel(ma, f, SysInfo.DefaultAsyncSequenceParallelism);
 
        [Pure]
        public static IO<RT, E, Arr<B>> TraverseParallel<RT, E, A, B>(this Arr<IO<RT, E, A>> ma, Func<A, B> f, int windowSize)
            where RT : HasIO<RT, E> =>
            IO<RT, E, Arr<B>>.LiftIO(async env =>
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
        public static IO<RT, E, Arr<B>> TraverseSerial<RT, E, A, B>(this Arr<IO<RT, E, A>> ma, Func<A, B> f)
            where RT : HasIO<RT, E> =>
            IO<RT, E, Arr<B>>.Lift(env =>
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
        public static IO<RT, E, HashSet<B>> TraverseParallel<RT, E, A, B>(this HashSet<IO<RT, E, A>> ma, Func<A, B> f)
            where RT : HasIO<RT, E> =>
            TraverseParallel(ma, f, SysInfo.DefaultAsyncSequenceParallelism);
 
        [Pure]
        public static IO<RT, E, HashSet<B>> TraverseParallel<RT, E, A, B>(this HashSet<IO<RT, E, A>> ma, Func<A, B> f, int windowSize)
            where RT : HasIO<RT, E> =>
            IO<RT, E, HashSet<B>>.LiftIO(async env =>
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
        public static IO<RT, E, HashSet<B>> TraverseSerial<RT, E, A, B>(this HashSet<IO<RT, E, A>> ma, Func<A, B> f)
            where RT : HasIO<RT, E> =>
            IO<RT, E, HashSet<B>>.Lift(env =>
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
        public static IO<RT, E, IEnumerable<B>> TraverseParallel<RT, E, A, B>(this IEnumerable<IO<RT, E, A>> ma, Func<A, B> f)
            where RT : HasIO<RT, E> =>
            TraverseParallel(ma, f, SysInfo.DefaultAsyncSequenceParallelism);
 
        [Pure]
        public static IO<RT, E, IEnumerable<B>> TraverseParallel<RT, E, A, B>(this IEnumerable<IO<RT, E, A>> ma, Func<A, B> f, int windowSize)
            where RT : HasIO<RT, E> =>
            IO<RT, E, IEnumerable<B>>.LiftIO(async env =>
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
        public static IO<RT, E, IEnumerable<B>> TraverseSerial<RT, E, A, B>(this IEnumerable<IO<RT, E, A>> ma, Func<A, B> f)
            where RT : HasIO<RT, E> =>
            IO<RT, E, IEnumerable<B>>.Lift(env =>
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
        public static IO<RT, E, Lst<B>> TraverseParallel<RT, E, A, B>(this Lst<IO<RT, E, A>> ma, Func<A, B> f)
            where RT : HasIO<RT, E> =>
            TraverseParallel(ma, f, SysInfo.DefaultAsyncSequenceParallelism);
 
        [Pure]
        public static IO<RT, E, Lst<B>> TraverseParallel<RT, E, A, B>(this Lst<IO<RT, E, A>> ma, Func<A, B> f, int windowSize)
            where RT : HasIO<RT, E> =>
            IO<RT, E, Lst<B>>.LiftIO(async env =>
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
        public static IO<RT, E, Lst<B>> TraverseSerial<RT, E, A, B>(this Lst<IO<RT, E, A>> ma, Func<A, B> f)
            where RT : HasIO<RT, E> =>
            IO<RT, E, Lst<B>>.Lift(env =>
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
        public static IO<RT, E, Que<B>> TraverseParallel<RT, E, A, B>(this Que<IO<RT, E, A>> ma, Func<A, B> f)
            where RT : HasIO<RT, E> =>
            TraverseParallel(ma, f, SysInfo.DefaultAsyncSequenceParallelism);
 
        [Pure]
        public static IO<RT, E, Que<B>> TraverseParallel<RT, E, A, B>(this Que<IO<RT, E, A>> ma, Func<A, B> f, int windowSize)
            where RT : HasIO<RT, E> =>
            IO<RT, E, Que<B>>.LiftIO(async env =>
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
        public static IO<RT, E, Que<B>> TraverseSerial<RT, E, A, B>(this Que<IO<RT, E, A>> ma, Func<A, B> f)
            where RT : HasIO<RT, E> =>
            IO<RT, E, Que<B>>.Lift(env =>
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
        public static IO<RT, E, Seq<B>> TraverseParallel<RT, E, A, B>(this Seq<IO<RT, E, A>> ma, Func<A, B> f)
            where RT : HasIO<RT, E> =>
            TraverseParallel(ma, f, SysInfo.DefaultAsyncSequenceParallelism);
 
        [Pure]
        public static IO<RT, E, Seq<B>> TraverseParallel<RT, E, A, B>(this Seq<IO<RT, E, A>> ma, Func<A, B> f, int windowSize)
            where RT : HasIO<RT, E> =>
            IO<RT, E, Seq<B>>.LiftIO(async env =>
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
        public static IO<RT, E, Seq<B>> TraverseSerial<RT, E, A, B>(this Seq<IO<RT, E, A>> ma, Func<A, B> f)
            where RT : HasIO<RT, E> =>
            IO<RT, E, Seq<B>>.Lift(env =>
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
        public static IO<RT, E, Set<B>> TraverseParallel<RT, E, A, B>(this Set<IO<RT, E, A>> ma, Func<A, B> f)
            where RT : HasIO<RT, E> =>
            TraverseParallel(ma, f, SysInfo.DefaultAsyncSequenceParallelism);
 
        [Pure]
        public static IO<RT, E, Set<B>> TraverseParallel<RT, E, A, B>(this Set<IO<RT, E, A>> ma, Func<A, B> f, int windowSize)
            where RT : HasIO<RT, E> =>
            IO<RT, E, Set<B>>.LiftIO(async env =>
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
        public static IO<RT, E, Set<B>> TraverseSerial<RT, E, A, B>(this Set<IO<RT, E, A>> ma, Func<A, B> f)
            where RT : HasIO<RT, E> =>
            IO<RT, E, Set<B>>.Lift(env =>
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
        public static IO<RT, E, Stck<B>> TraverseParallel<RT, E, A, B>(this Stck<IO<RT, E, A>> ma, Func<A, B> f)
            where RT : HasIO<RT, E> =>
            TraverseParallel(ma, f, SysInfo.DefaultAsyncSequenceParallelism);
 
        [Pure]
        public static IO<RT, E, Stck<B>> TraverseParallel<RT, E, A, B>(this Stck<IO<RT, E, A>> ma, Func<A, B> f, int windowSize)
            where RT : HasIO<RT, E> =>
            IO<RT, E, Stck<B>>.LiftIO(async env =>
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
        public static IO<RT, E, Stck<B>> TraverseSerial<RT, E, A, B>(this Stck<IO<RT, E, A>> ma, Func<A, B> f)
            where RT : HasIO<RT, E> =>
            IO<RT, E, Stck<B>>.Lift(env =>
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

        public static IO<RT, E, Task<B>> Traverse<RT, E, A, B>(this Task<IO<RT, E, A>> ma, Func<A, B> f)
            where RT : HasIO<RT, E>
        {
            return IO<RT, E, Task<B>>.LiftIO(env => Go(env, ma, f));
            async Task<Either<E, Task<B>>> Go(RT env, Task<IO<RT, E, A>> ma, Func<A, B> f)
            {
                var ra = await ma.ConfigureAwait(false);
                var rb = await ra.RunAsync(env).ConfigureAwait(false);
                if (rb.IsLeft) return rb.LeftValue;
                return f(rb.RightValue).AsTask();
            }
        }

        public static IO<RT, E, ValueTask<B>> Traverse<RT, E, A, B>(this ValueTask<IO<RT, E, A>> ma, Func<A, B> f)
            where RT : HasIO<RT, E>
        {
            return IO<RT, E, ValueTask<B>>.LiftIO(env => Go(env, ma, f));
            async Task<Either<E, ValueTask<B>>> Go(RT env, ValueTask<IO<RT, E, A>> ma, Func<A, B> f)
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
        
        public static IO<RT, L, Either<L, B>> Traverse<RT, L, A, B>(this Either<L, IO<RT, L, A>> ma, Func<A, B> f)
            where RT : HasIO<RT, L>
        {
            return IO<RT, L, Either<L, B>>.Lift(env => Go(env, ma, f));
            Either <L, Either<L, B>> Go(RT env, Either<L, IO<RT, L, A>> ma, Func<A, B> f)
            {
                if(ma.IsBottom) return default;
                if(ma.IsLeft) return ma.LeftValue;
                var rb = ma.RightValue.Run(env);
                if(rb.IsLeft) return rb.LeftValue;
                return Either<L, B>.Right(f(rb.RightValue));
            }
        }

        public static IO<RT, L, EitherUnsafe<L, B>> Traverse<RT, L, A, B>(this EitherUnsafe<L, IO<RT, L, A>> ma, Func<A, B> f)
            where RT : HasIO<RT, L>
        {
            return IO<RT, L, EitherUnsafe<L, B>>.Lift(env => Go(env, ma, f));

            Either<L, EitherUnsafe<L, B>> Go(RT env, EitherUnsafe<L, IO<RT, L, A>> ma, Func<A, B> f)
            {
                if (ma.IsBottom) return default;
                if (ma.IsLeft) return ma.LeftValue;
                var rb = ma.RightValue.Run(env);
                if (rb.IsLeft) return rb.LeftValue;
                return EitherUnsafe<L, B>.Right(f(rb.RightValue));
            }
        }

        public static IO<RT, E, Identity<B>> Traverse<RT, E, A, B>(this Identity<IO<RT, E, A>> ma, Func<A, B> f)
            where RT : HasIO<RT, E>
        {
            return IO<RT, E, Identity<B>>.Lift(env => Go(env, ma, f));
            Either<E, Identity<B>> Go(RT env, Identity<IO<RT, E, A>> ma, Func<A, B> f)
            {
                if(ma.IsBottom) return default;
                var rb = ma.Value.Run(env);
                if(rb.IsLeft) return rb.LeftValue;
                return new Identity<B>(f(rb.RightValue));
            }
        }

        public static IO<RT, E, Fin<B>> Traverse<RT, E, A, B>(this Fin<IO<RT, E, A>> ma, Func<A, B> f)
            where RT : HasIO<RT, E>
        {
            return IO<RT, E, Fin<B>>.Lift(env => Go(env, ma, f));
            Either<E, Fin<B>> Go(RT env, Fin<IO<RT, E, A>> ma, Func<A, B> f)
            {
                if(ma.IsFail) return Either<E, Fin<B>>.Right(ma.Error);
                var rb = ma.Value.Run(env);
                if(rb.IsLeft) return rb.LeftValue;
                return Fin<B>.Succ(f(rb.RightValue));
            }
        }

        public static IO<RT, E, Option<B>> Traverse<RT, E, A, B>(this Option<IO<RT, E, A>> ma, Func<A, B> f)
            where RT : HasIO<RT, E>
        {
            return IO<RT, E, Option<B>>.Lift(env => Go(env, ma, f));
            Either<E, Option<B>> Go(RT env, Option<IO<RT, E, A>> ma, Func<A, B> f)
            {
                if(ma.IsNone) return Either<E, Option<B>>.Right(None);
                var rb = ma.Value.Run(env);
                if(rb.IsLeft) return rb.LeftValue;
                return Option<B>.Some(f(rb.RightValue));
            }
        }
        
        public static IO<RT, E, OptionUnsafe<B>> Traverse<RT, E, A, B>(this OptionUnsafe<IO<RT, E, A>> ma, Func<A, B> f)
            where RT : HasIO<RT, E>
        {
            return IO<RT, E, OptionUnsafe<B>>.Lift(env => Go(env, ma, f));
            Either<E, OptionUnsafe<B>> Go(RT env, OptionUnsafe<IO<RT, E, A>> ma, Func<A, B> f)
            {
                if(ma.IsNone) return Either<E, OptionUnsafe<B>>.Right(None);
                var rb = ma.Value.Run(env);
                if(rb.IsLeft) return rb.LeftValue;
                return OptionUnsafe<B>.Some(f(rb.RightValue));
            }
        }
        
        
        public static IO<RT, Fail, Validation<Fail, B>> Traverse<RT, Fail, A, B>(
            this Validation<Fail, IO<RT, Fail, A>> ma, Func<A, B> f)
            where RT : HasIO<RT, Fail>
        {
            return IO<RT, Fail, Validation<Fail, B>>.Lift(env => Go(env, ma, f));
            Either<Fail, Validation<Fail, B>> Go(RT env, Validation<Fail, IO<RT, Fail, A>> ma, Func<A, B> f)
            {
                if (ma.IsFail) return Either<Fail, Validation<Fail, B>>.Right(Fail<Fail, B>(ma.FailValue));
                var rb = ma.SuccessValue.Run(env);
                if(rb.IsLeft) return rb.LeftValue;
                return Either<Fail, Validation<Fail, B>>.Right(f(rb.RightValue));
            }
        }
        
        public static IO<RT, Fail, Validation<MonoidFail, Fail, B>> Traverse<RT, MonoidFail, Fail, A, B>(
            this Validation<MonoidFail, Fail, IO<RT, Fail, A>> ma, Func<A, B> f)
            where RT : HasIO<RT, Fail>
            where MonoidFail : Monoid<Fail>, Eq<Fail>
        {
            return IO<RT, Fail, Validation<MonoidFail, Fail, B>>.Lift(env => Go(env, ma, f));
            Either<Fail, Validation<MonoidFail, Fail, B>> Go(RT env, Validation<MonoidFail, Fail, IO<RT, Fail, A>> ma, Func<A, B> f)
            {
                if (ma.IsFail) return Either<Fail, Validation<MonoidFail, Fail, B>>.Right(Fail<MonoidFail, Fail, B>(ma.FailValue));
                var rb = ma.SuccessValue.Run(env);
                if(rb.IsLeft) return rb.LeftValue;
                return Either<Fail, Validation<MonoidFail, Fail, B>>.Right(f(rb.RightValue));
            }
        }
    }
}
