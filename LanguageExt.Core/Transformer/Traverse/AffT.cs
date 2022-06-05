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
    public static partial class AffT
    {
        //
        // Collections
        //

        [Pure]
        public static Aff<RT, Arr<B>> TraverseParallel<RT, A, B>(this Arr<Aff<RT, A>> ma, Func<A, B> f)
            where RT : struct, HasCancel<RT> =>
            TraverseParallel<RT, A, B>(ma, f, SysInfo.DefaultAsyncSequenceParallelism);
 
        [Pure]
        public static Aff<RT, Arr<B>> TraverseParallel<RT, A, B>(this Arr<Aff<RT, A>> ma, Func<A, B> f, int windowSize)
            where RT : struct, HasCancel<RT> =>
            AffMaybe<RT, Arr<B>>(async env =>
            {
                var rs = await ma.AsEnumerable().Map(m => m.Run(env)).WindowMap(windowSize, fa => fa.Map(f));

                var (fails, succs) = rs.Partition();
                return fails.Any()
                    ? FinFail<Arr<B>>(fails.Head())
                    : FinSucc<Arr<B>>(toArray(succs));
            });

        [Pure]
        public static Aff<RT, Arr<B>> TraverseSerial<RT, A, B>(this Arr<Aff<RT, A>> ma, Func<A, B> f)
            where RT : struct, HasCancel<RT> =>
            AffMaybe<RT, Arr<B>>(async env =>
            {
                var rs = new List<B>();
                foreach (var m in ma)
                {
                    var r = await m.Run(env);
                    if (r.IsFail) return FinFail<Arr<B>>(r.Error);
                    rs.Add(f(r.Value));
                }
                return FinSucc(new Arr<B>(rs.ToArray()));
            });

        

        [Pure]
        public static Aff<RT, HashSet<B>> TraverseParallel<RT, A, B>(this HashSet<Aff<RT, A>> ma, Func<A, B> f)
            where RT : struct, HasCancel<RT> =>
            TraverseParallel<RT, A, B>(ma, f, SysInfo.DefaultAsyncSequenceParallelism);
 
        [Pure]
        public static Aff<RT, HashSet<B>> TraverseParallel<RT, A, B>(this HashSet<Aff<RT, A>> ma, Func<A, B> f, int windowSize)
            where RT : struct, HasCancel<RT> =>
            AffMaybe<RT, HashSet<B>>(async env =>
            {
                var rs = await ma.AsEnumerable().Map(m => m.Run(env)).WindowMap(windowSize, fa => fa.Map(f));

                var (fails, succs) = rs.Partition();
                return fails.Any()
                    ? FinFail<HashSet<B>>(fails.Head())
                    : FinSucc<HashSet<B>>(toHashSet(succs));
            });

        [Pure]
        public static Aff<RT, HashSet<B>> TraverseSerial<RT, A, B>(this HashSet<Aff<RT, A>> ma, Func<A, B> f)
            where RT : struct, HasCancel<RT> =>
            AffMaybe<RT, HashSet<B>>(async env =>
            {
                var rs = new List<B>();
                foreach (var m in ma)
                {
                    var r = await m.Run(env);
                    if (r.IsFail) return FinFail<HashSet<B>>(r.Error);
                    rs.Add(f(r.Value));
                }
                return FinSucc(new HashSet<B>(rs));
            });

        
        

        [Pure]
        public static Aff<RT, IEnumerable<B>> TraverseParallel<RT, A, B>(this IEnumerable<Aff<RT, A>> ma, Func<A, B> f)
            where RT : struct, HasCancel<RT> =>
            TraverseParallel<RT, A, B>(ma, f, SysInfo.DefaultAsyncSequenceParallelism);
 
        [Pure]
        public static Aff<RT, IEnumerable<B>> TraverseParallel<RT, A, B>(this IEnumerable<Aff<RT, A>> ma, Func<A, B> f, int windowSize)
            where RT : struct, HasCancel<RT> =>
            AffMaybe<RT, IEnumerable<B>>(async env =>
            {
                var rs = await ma.AsEnumerable().Map(m => m.Run(env)).WindowMap(windowSize, fa => fa.Map(f));

                var (fails, succs) = rs.Partition();
                return fails.Any()
                    ? FinFail<IEnumerable<B>>(fails.Head())
                    : FinSucc<IEnumerable<B>>(succs);
            });

        [Pure]
        public static Aff<RT, IEnumerable<B>> TraverseSerial<RT, A, B>(this IEnumerable<Aff<RT, A>> ma, Func<A, B> f)
            where RT : struct, HasCancel<RT> =>
            AffMaybe<RT, IEnumerable<B>>(async env =>
            {
                var rs = new List<B>();
                foreach (var m in ma)
                {
                    var r = await m.Run(env);
                    if (r.IsFail) return FinFail<IEnumerable<B>>(r.Error);
                    rs.Add(f(r.Value));
                }
                return FinSucc(rs.AsEnumerable());
            });

        
        [Pure]
        public static Aff<RT, Lst<B>> TraverseParallel<RT, A, B>(this Lst<Aff<RT, A>> ma, Func<A, B> f)
            where RT : struct, HasCancel<RT> =>
            TraverseParallel<RT, A, B>(ma, f, SysInfo.DefaultAsyncSequenceParallelism);
 
        [Pure]
        public static Aff<RT, Lst<B>> TraverseParallel<RT, A, B>(this Lst<Aff<RT, A>> ma, Func<A, B> f, int windowSize)
            where RT : struct, HasCancel<RT> =>
            AffMaybe<RT, Lst<B>>(async env =>
            {
                var rs = await ma.AsEnumerable().Map(m => m.Run(env)).WindowMap(windowSize, fa => fa.Map(f));

                var (fails, succs) = rs.Partition();
                return fails.Any()
                    ? FinFail<Lst<B>>(fails.Head())
                    : FinSucc<Lst<B>>(toList(succs));
            });

        [Pure]
        public static Aff<RT, Lst<B>> TraverseSerial<RT, A, B>(this Lst<Aff<RT, A>> ma, Func<A, B> f)
            where RT : struct, HasCancel<RT> =>
            AffMaybe<RT, Lst<B>>(async env =>
            {
                var rs = new List<B>();
                foreach (var m in ma)
                {
                    var r = await m.Run(env);
                    if (r.IsFail) return FinFail<Lst<B>>(r.Error);
                    rs.Add(f(r.Value));
                }
                return FinSucc(rs.Freeze());
            });


 
        [Pure]
        public static Aff<RT, Que<B>> TraverseParallel<RT, A, B>(this Que<Aff<RT, A>> ma, Func<A, B> f)
            where RT : struct, HasCancel<RT> =>
            TraverseParallel<RT, A, B>(ma, f, SysInfo.DefaultAsyncSequenceParallelism);
 
        [Pure]
        public static Aff<RT, Que<B>> TraverseParallel<RT, A, B>(this Que<Aff<RT, A>> ma, Func<A, B> f, int windowSize)
            where RT : struct, HasCancel<RT> =>
            AffMaybe<RT, Que<B>>(async env =>
            {
                var rs = await ma.AsEnumerable().Map(m => m.Run(env)).WindowMap(windowSize, fa => fa.Map(f));

                var (fails, succs) = rs.Partition();
                return fails.Any()
                    ? FinFail<Que<B>>(fails.Head())
                    : FinSucc<Que<B>>(toQueue(succs));
            });

        [Pure]
        public static Aff<RT, Que<B>> TraverseSerial<RT, A, B>(this Que<Aff<RT, A>> ma, Func<A, B> f)
            where RT : struct, HasCancel<RT> =>
            AffMaybe<RT, Que<B>>(async env =>
            {
                var rs = new List<B>();
                foreach (var m in ma)
                {
                    var r = await m.Run(env);
                    if (r.IsFail) return FinFail<Que<B>>(r.Error);
                    rs.Add(f(r.Value));
                }
                return FinSucc(toQueue(rs));
            });

        
        
        [Pure]
        public static Aff<RT, Seq<B>> TraverseParallel<RT, A, B>(this Seq<Aff<RT, A>> ma, Func<A, B> f)
            where RT : struct, HasCancel<RT> =>
            TraverseParallel<RT, A, B>(ma, f, SysInfo.DefaultAsyncSequenceParallelism);
 
        [Pure]
        public static Aff<RT, Seq<B>> TraverseParallel<RT, A, B>(this Seq<Aff<RT, A>> ma, Func<A, B> f, int windowSize)
            where RT : struct, HasCancel<RT> =>
            AffMaybe<RT, Seq<B>>(async env =>
            {
                var rs = await ma.AsEnumerable().Map(m => m.Run(env)).WindowMap(windowSize, fa => fa.Map(f));

                var (fails, succs) = rs.Partition();
                return fails.Any()
                    ? FinFail<Seq<B>>(fails.Head())
                    : FinSucc<Seq<B>>(Seq.FromArray(succs.ToArray()));
            });

        [Pure]
        public static Aff<RT, Seq<B>> TraverseSerial<RT, A, B>(this Seq<Aff<RT, A>> ma, Func<A, B> f)
            where RT : struct, HasCancel<RT> =>
            AffMaybe<RT, Seq<B>>(async env =>
            {
                var rs = new List<B>();
                foreach (var m in ma)
                {
                    var r = await m.Run(env);
                    if (r.IsFail) return FinFail<Seq<B>>(r.Error);
                    rs.Add(f(r.Value));
                }
                return FinSucc(Seq.FromArray(rs.ToArray()));
            });

 
        [Pure]
        public static Aff<RT, Set<B>> TraverseParallel<RT, A, B>(this Set<Aff<RT, A>> ma, Func<A, B> f)
            where RT : struct, HasCancel<RT> =>
            TraverseParallel<RT, A, B>(ma, f, SysInfo.DefaultAsyncSequenceParallelism);
 
        [Pure]
        public static Aff<RT, Set<B>> TraverseParallel<RT, A, B>(this Set<Aff<RT, A>> ma, Func<A, B> f, int windowSize)
            where RT : struct, HasCancel<RT> =>
            AffMaybe<RT, Set<B>>(async env =>
            {
                var rs = await ma.AsEnumerable().Map(m => m.Run(env)).WindowMap(windowSize, fa => fa.Map(f));

                var (fails, succs) = rs.Partition();
                return fails.Any()
                    ? FinFail<Set<B>>(fails.Head())
                    : FinSucc<Set<B>>(toSet(succs));
            });

        [Pure]
        public static Aff<RT, Set<B>> TraverseSerial<RT, A, B>(this Set<Aff<RT, A>> ma, Func<A, B> f)
            where RT : struct, HasCancel<RT> =>
            AffMaybe<RT, Set<B>>(async env =>
            {
                var rs = new List<B>();
                foreach (var m in ma)
                {
                    var r = await m.Run(env);
                    if (r.IsFail) return FinFail<Set<B>>(r.Error);
                    rs.Add(f(r.Value));
                }
                return FinSucc(toSet(rs));
            });
        
        

 
        [Pure]
        public static Aff<RT, Stck<B>> TraverseParallel<RT, A, B>(this Stck<Aff<RT, A>> ma, Func<A, B> f)
            where RT : struct, HasCancel<RT> =>
            TraverseParallel<RT, A, B>(ma, f, SysInfo.DefaultAsyncSequenceParallelism);
 
        [Pure]
        public static Aff<RT, Stck<B>> TraverseParallel<RT, A, B>(this Stck<Aff<RT, A>> ma, Func<A, B> f, int windowSize)
            where RT : struct, HasCancel<RT> =>
            AffMaybe<RT, Stck<B>>(async env =>
            {
                var rs = await ma.AsEnumerable().Map(m => m.Run(env)).WindowMap(windowSize, fa => fa.Map(f));

                var (fails, succs) = rs.Partition();
                return fails.Any()
                    ? FinFail<Stck<B>>(fails.Head())
                    : FinSucc<Stck<B>>(toStack(succs));
            });

        [Pure]
        public static Aff<RT, Stck<B>> TraverseSerial<RT, A, B>(this Stck<Aff<RT, A>> ma, Func<A, B> f)
            where RT : struct, HasCancel<RT> =>
            AffMaybe<RT, Stck<B>>(async env =>
            {
                var rs = new List<B>();
                foreach (var m in ma)
                {
                    var r = await m.Run(env);
                    if (r.IsFail) return FinFail<Stck<B>>(r.Error);
                    rs.Add(f(r.Value));
                }
                return FinSucc(toStack(rs));
            });
        
        
        //
        // Async types
        //

        public static Aff<RT, EitherAsync<L, B>> Traverse<RT, L, A, B>(this EitherAsync<L, Aff<RT, A>> ma, Func<A, B> f)
            where RT : struct, HasCancel<RT>
        {
            return AffMaybe<RT, EitherAsync<L, B>>(env => Go(env, ma, f));
            async ValueTask<Fin<EitherAsync<L, B>>> Go(RT env, EitherAsync<L, Aff<RT, A>> ma, Func<A, B> f)
            {
                var da = await ma.Data.ConfigureAwait(false);
                if (da.State == EitherStatus.IsBottom) return default;
                if (da.State == EitherStatus.IsLeft) return FinSucc<EitherAsync<L,B>>(da.Left);
                var rb = await da.Right.Run(env).ConfigureAwait(false);
                if (rb.IsFail) return FinFail<EitherAsync<L, B>>(rb.Error);
                return FinSucc<EitherAsync<L, B>>(EitherAsync<L, B>.Right(f(rb.Value)));
            }
        }

        public static Aff<RT, OptionAsync<B>> Traverse<RT, A, B>(this OptionAsync<Aff<RT, A>> ma, Func<A, B> f)
            where RT : struct, HasCancel<RT>
        {
            return AffMaybe<RT, OptionAsync<B>>(env => Go(env, ma, f));
            async ValueTask<Fin<OptionAsync<B>>> Go(RT env, OptionAsync<Aff<RT, A>> ma, Func<A, B> f)
            {
                var (isSome, value) = await ma.Data.ConfigureAwait(false);
                if (!isSome) return FinSucc<OptionAsync<B>>(OptionAsync<B>.None);
                var rb = await value.Run(env).ConfigureAwait(false);
                if (rb.IsFail) return FinFail<OptionAsync<B>>(rb.Error);
                return FinSucc<OptionAsync<B>>(OptionAsync<B>.Some(f(rb.Value)));
            }
        }
        
        public static Aff<RT, TryAsync<B>> Traverse<RT, A, B>(this TryAsync<Aff<RT, A>> ma, Func<A, B> f)
            where RT : struct, HasCancel<RT>
        {
            return AffMaybe<RT, TryAsync<B>>(env => Go(env, ma, f));
            async ValueTask<Fin<TryAsync<B>>> Go(RT env, TryAsync<Aff<RT, A>> ma, Func<A, B> f)
            {
                var ra = await ma.Try().ConfigureAwait(false);
                if (ra.IsFaulted) return FinSucc<TryAsync<B>>(TryAsync<B>(ra.Exception));
                var rb = await ra.Value.Run(env).ConfigureAwait(false);
                if (rb.IsFail) return FinFail<TryAsync<B>>(rb.Error);
                return FinSucc<TryAsync<B>>(TryAsync<B>(f(rb.Value)));
            }
        }
        
        public static Aff<RT, TryOptionAsync<B>> Traverse<RT, A, B>(this TryOptionAsync<Aff<RT, A>> ma, Func<A, B> f)
            where RT : struct, HasCancel<RT>
        {
            return AffMaybe<RT, TryOptionAsync<B>>(env => Go(env, ma, f));
            async ValueTask<Fin<TryOptionAsync<B>>> Go(RT env, TryOptionAsync<Aff<RT, A>> ma, Func<A, B> f)
            {
                var ra = await ma.Try().ConfigureAwait(false);
                if (ra.IsFaulted) return FinSucc<TryOptionAsync<B>>(TryOptionAsync<B>(ra.Exception));
                if (ra.IsNone) return FinSucc<TryOptionAsync<B>>(TryOptionAsync<B>(None));
                var rb = await ra.Value.Value.Run(env).ConfigureAwait(false);
                if (rb.IsFail) return FinFail<TryOptionAsync<B>>(rb.Error);
                return FinSucc<TryOptionAsync<B>>(TryOptionAsync<B>(f(rb.Value)));
            }
        }

        public static Aff<RT, Task<B>> Traverse<RT, A, B>(this Task<Aff<RT, A>> ma, Func<A, B> f)
            where RT : struct, HasCancel<RT>
        {
            return AffMaybe<RT, Task<B>>(env => Go(env, ma, f));
            async ValueTask<Fin<Task<B>>> Go(RT env, Task<Aff<RT, A>> ma, Func<A, B> f)
            {
                var ra = await ma.ConfigureAwait(false);
                var rb = await ra.Run(env).ConfigureAwait(false);
                if (rb.IsFail) return FinFail<Task<B>>(rb.Error);
                return FinSucc<Task<B>>(f(rb.Value).AsTask());
            }
        }

        public static Aff<RT, ValueTask<B>> Traverse<RT, A, B>(this ValueTask<Aff<RT, A>> ma, Func<A, B> f)
            where RT : struct, HasCancel<RT>
        {
            return AffMaybe<RT, ValueTask<B>>(env => Go(env, ma, f));
            async ValueTask<Fin<ValueTask<B>>> Go(RT env, ValueTask<Aff<RT, A>> ma, Func<A, B> f)
            {
                var ra = await ma.ConfigureAwait(false);
                var rb = await ra.Run(env).ConfigureAwait(false);
                if (rb.IsFail) return FinFail<ValueTask<B>>(rb.Error);
                return FinSucc<ValueTask<B>>(f(rb.Value).AsValueTask());
            }
        }
        
        public static Aff<RT, Aff<B>> Traverse<RT, A, B>(this Aff<Aff<RT, A>> ma, Func<A, B> f)
            where RT : struct, HasCancel<RT>
        {
            return AffMaybe<RT, Aff<B>>(env => Go(env, ma, f));
            async ValueTask<Fin<Aff<B>>> Go(RT env, Aff<Aff<RT, A>> ma, Func<A, B> f)
            {
                var ra = await ma.Run().ConfigureAwait(false);
                if (ra.IsFail) return FinSucc<Aff<B>>(FailAff<B>(ra.Error));
                var rb = await ra.Value.Run(env).ConfigureAwait(false);
                if (rb.IsFail) return FinFail<Aff<B>>(rb.Error);
                return FinSucc<Aff<B>>(SuccessAff<B>(f(rb.Value)));
            }
        }
        
        //
        // Sync types
        // 
        
        public static Aff<RT, Either<L, B>> Traverse<RT, L, A, B>(this Either<L, Aff<RT, A>> ma, Func<A, B> f)
            where RT : struct, HasCancel<RT>
        {
            return AffMaybe<RT, Either<L, B>>(env => Go(env, ma, f));
            async ValueTask<Fin<Either<L, B>>> Go(RT env, Either<L, Aff<RT, A>> ma, Func<A, B> f)
            {
                if(ma.IsBottom) return default;
                if(ma.IsLeft) return FinSucc<Either<L, B>>(ma.LeftValue);
                var rb = await ma.RightValue.Run(env).ConfigureAwait(false);
                if(rb.IsFail) return FinFail<Either<L, B>>(rb.Error);
                return FinSucc<Either<L, B>>(f(rb.Value));
            }
        }

        public static Aff<RT, EitherUnsafe<L, B>> Traverse<RT, L, A, B>(this EitherUnsafe<L, Aff<RT, A>> ma, Func<A, B> f)
            where RT : struct, HasCancel<RT>
        {
            return AffMaybe<RT, EitherUnsafe<L, B>>(env => Go(env, ma, f));
            async ValueTask<Fin<EitherUnsafe<L, B>>> Go(RT env, EitherUnsafe<L, Aff<RT, A>> ma, Func<A, B> f)
            {
                if(ma.IsBottom) return default;
                if(ma.IsLeft) return FinSucc<EitherUnsafe<L, B>>(ma.LeftValue);
                var rb = await ma.RightValue.Run(env).ConfigureAwait(false);
                if(rb.IsFail) return FinFail<EitherUnsafe<L, B>>(rb.Error);
                return FinSucc<EitherUnsafe<L, B>>(f(rb.Value));
            }
        }

        public static Aff<RT, Identity<B>> Traverse<RT, A, B>(this Identity<Aff<RT, A>> ma, Func<A, B> f)
            where RT : struct, HasCancel<RT>
        {
            return AffMaybe<RT, Identity<B>>(env => Go(env, ma, f));
            async ValueTask<Fin<Identity<B>>> Go(RT env, Identity<Aff<RT, A>> ma, Func<A, B> f)
            {
                if(ma.IsBottom) return default;
                var rb = await ma.Value.Run(env).ConfigureAwait(false);
                if(rb.IsFail) return FinFail<Identity<B>>(rb.Error);
                return FinSucc<Identity<B>>(new Identity<B>(f(rb.Value)));
            }
        }

        public static Aff<RT, Fin<B>> Traverse<RT, A, B>(this Fin<Aff<RT, A>> ma, Func<A, B> f)
            where RT : struct, HasCancel<RT>
        {
            return AffMaybe<RT, Fin<B>>(env => Go(env, ma, f));
            async ValueTask<Fin<Fin<B>>> Go(RT env, Fin<Aff<RT, A>> ma, Func<A, B> f)
            {
                if(ma.IsFail) return FinSucc<Fin<B>>(ma.Cast<B>());
                var rb = await ma.Value.Run(env).ConfigureAwait(false);
                if(rb.IsFail) return FinFail<Fin<B>>(rb.Error);
                return FinSucc<Fin<B>>(Fin<B>.Succ(f(rb.Value)));
            }
        }

        public static Aff<RT, Option<B>> Traverse<RT, A, B>(this Option<Aff<RT, A>> ma, Func<A, B> f)
            where RT : struct, HasCancel<RT>
        {
            return AffMaybe<RT, Option<B>>(env => Go(env, ma, f));
            async ValueTask<Fin<Option<B>>> Go(RT env, Option<Aff<RT, A>> ma, Func<A, B> f)
            {
                if(ma.IsNone) return FinSucc<Option<B>>(None);
                var rb = await ma.Value.Run(env).ConfigureAwait(false);
                if(rb.IsFail) return FinFail<Option<B>>(rb.Error);
                return FinSucc<Option<B>>(Option<B>.Some(f(rb.Value)));
            }
        }
        
        public static Aff<RT, OptionUnsafe<B>> Traverse<RT, A, B>(this OptionUnsafe<Aff<RT, A>> ma, Func<A, B> f)
            where RT : struct, HasCancel<RT>
        {
            return AffMaybe<RT, OptionUnsafe<B>>(env => Go(env, ma, f));
            async ValueTask<Fin<OptionUnsafe<B>>> Go(RT env, OptionUnsafe<Aff<RT, A>> ma, Func<A, B> f)
            {
                if(ma.IsNone) return FinSucc<OptionUnsafe<B>>(None);
                var rb = await ma.Value.Run(env).ConfigureAwait(false);
                if(rb.IsFail) return FinFail<OptionUnsafe<B>>(rb.Error);
                return FinSucc<OptionUnsafe<B>>(OptionUnsafe<B>.Some(f(rb.Value)));
            }
        }
        
        public static Aff<RT, Try<B>> Traverse<RT, A, B>(this Try<Aff<RT, A>> ma, Func<A, B> f)
            where RT : struct, HasCancel<RT>
        {
            return AffMaybe<RT, Try<B>>(env => Go(env, ma, f));
            async ValueTask<Fin<Try<B>>> Go(RT env, Try<Aff<RT, A>> ma, Func<A, B> f)
            {
                var ra = ma.Try();
                if (ra.IsFaulted) return FinSucc<Try<B>>(TryFail<B>(ra.Exception));
                var rb = await ra.Value.Run(env).ConfigureAwait(false);
                if (rb.IsFail) return FinFail<Try<B>>(rb.Error);
                return FinSucc<Try<B>>(Try<B>(f(rb.Value)));
            }
        }
        
        public static Aff<RT, TryOption<B>> Traverse<RT, A, B>(this TryOption<Aff<RT, A>> ma, Func<A, B> f)
            where RT : struct, HasCancel<RT>
        {
            return AffMaybe<RT, TryOption<B>>(env => Go(env, ma, f));
            async ValueTask<Fin<TryOption<B>>> Go(RT env, TryOption<Aff<RT, A>> ma, Func<A, B> f)
            {
                var ra = ma.Try();
                if (ra.IsBottom) return default;
                if (ra.IsNone) return FinSucc<TryOption<B>>(TryOptional<B>(None));
                if (ra.IsFaulted) return FinSucc<TryOption<B>>(TryOptionFail<B>(ra.Exception));
                var rb = await ra.Value.Value.Run(env).ConfigureAwait(false);
                if (rb.IsFail) return FinFail<TryOption<B>>(rb.Error);
                return FinSucc<TryOption<B>>(TryOption<B>(f(rb.Value)));
            }
        }
        
        public static Aff<RT, Validation<Fail, B>> Traverse<RT, Fail, A, B>(this Validation<Fail, Aff<RT, A>> ma, Func<A, B> f)
            where RT : struct, HasCancel<RT>
        {
            return AffMaybe<RT, Validation<Fail, B>>(env => Go(env, ma, f));
            async ValueTask<Fin<Validation<Fail, B>>> Go(RT env, Validation<Fail, Aff<RT, A>> ma, Func<A, B> f)
            {
                if (ma.IsFail) return FinSucc<Validation<Fail, B>>(Fail<Fail, B>(ma.FailValue));
                var rb = await ma.SuccessValue.Run(env).ConfigureAwait(false);
                if(rb.IsFail) return FinFail<Validation<Fail, B>>(rb.Error);
                return FinSucc<Validation<Fail, B>>(f(rb.Value));
            }
        }
        
        public static Aff<RT, Validation<MonoidFail, Fail, B>> Traverse<RT, MonoidFail, Fail, A, B>(this Validation<MonoidFail, Fail, Aff<RT, A>> ma, Func<A, B> f)
            where RT : struct, HasCancel<RT>
            where MonoidFail : struct, Monoid<Fail>, Eq<Fail>
        {
            return AffMaybe<RT, Validation<MonoidFail, Fail, B>>(env => Go(env, ma, f));
            async ValueTask<Fin<Validation<MonoidFail, Fail, B>>> Go(RT env, Validation<MonoidFail, Fail, Aff<RT, A>> ma, Func<A, B> f)
            {
                if (ma.IsFail) return FinSucc<Validation<MonoidFail, Fail, B>>(Fail<MonoidFail, Fail, B>(ma.FailValue));
                var rb = await ma.SuccessValue.Run(env).ConfigureAwait(false);
                if(rb.IsFail) return FinFail<Validation<MonoidFail, Fail, B>>(rb.Error);
                return FinSucc<Validation<MonoidFail, Fail, B>>(f(rb.Value));
            }
        }
        
        public static Aff<RT, Eff<RT, B>> Traverse<RT, A, B>(this Eff<RT, Aff<RT, A>> ma, Func<A, B> f)
            where RT : struct, HasCancel<RT>
        {
            return AffMaybe<RT, Eff<RT, B>>(env => Go(env, ma, f));
            async ValueTask<Fin<Eff<RT, B>>> Go(RT env, Eff<RT, Aff<RT, A>> ma, Func<A, B> f)
            {
                var ra = ma.Run(env);
                if (ra.IsFail) return FinSucc<Eff<RT, B>>(FailEff<B>(ra.Error));
                var rb = await ra.Value.Run(env).ConfigureAwait(false);
                if (rb.IsFail) return FinFail<Eff<RT, B>>(rb.Error);
                return FinSucc<Eff<RT, B>>(SuccessEff<B>(f(rb.Value)));
            }
        }
        
        public static Aff<RT, Eff<B>> Traverse<RT, A, B>(this Eff<Aff<RT, A>> ma, Func<A, B> f)
            where RT : struct, HasCancel<RT>
        {
            return AffMaybe<RT, Eff<B>>(env => Go(env, ma, f));
            async ValueTask<Fin<Eff<B>>> Go(RT env, Eff<Aff<RT, A>> ma, Func<A, B> f)
            {
                var ra = ma.Run();
                if (ra.IsFail) return FinSucc<Eff<B>>(FailEff<B>(ra.Error));
                var rb = await ra.Value.Run(env).ConfigureAwait(false);
                if (rb.IsFail) return FinFail<Eff<B>>(rb.Error);
                return FinSucc<Eff<B>>(SuccessEff<B>(f(rb.Value)));
            }
        }
    }
}
