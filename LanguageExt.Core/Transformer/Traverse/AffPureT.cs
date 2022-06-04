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
    public static partial class AffPureT
    {
        //
        // Collections
        //
        [Pure]
        public static Aff<Arr<B>> TraverseParallel<A, B>(this Arr<Aff<A>> ma, Func<A, B> f) =>
            TraverseParallel<A, B>(ma, f, SysInfo.DefaultAsyncSequenceParallelism);
 
        [Pure]
        public static Aff<Arr<B>> TraverseParallel<A, B>(this Arr<Aff<A>> ma, Func<A, B> f, int windowSize) =>
            AffMaybe<Arr<B>>(async () =>
            {
                var rs = await ma.AsEnumerable().Map(m => m.Run()).WindowMap(windowSize, fa => fa.Map(f)).ConfigureAwait(false);

                var (fails, succs) = rs.Partition();
                var fails1 = fails.Take(1).ToArray();
                
                return fails1.Length == 1
                    ? FinFail<Arr<B>>(fails1[0])
                    : FinSucc<Arr<B>>(toArray(succs));
            });

        [Pure]
        public static Aff<Arr<B>> TraverseSerial<A, B>(this Arr<Aff<A>> ma, Func<A, B> f) =>
            AffMaybe<Arr<B>>(async () =>
            {
                var rs = new List<B>();
                foreach (var m in ma)
                {
                    var r = await m.Run().ConfigureAwait(false);
                    if (r.IsFail) return FinFail<Arr<B>>(r.Error);
                    rs.Add(f(r.Value));
                }
                return FinSucc(new Arr<B>(rs.ToArray()));
            });

        
        [Pure]
        public static Aff<HashSet<B>> TraverseParallel<A, B>(this HashSet<Aff<A>> ma, Func<A, B> f) =>
            TraverseParallel<A, B>(ma, f, SysInfo.DefaultAsyncSequenceParallelism);
 
        [Pure]
        public static Aff<HashSet<B>> TraverseParallel<A, B>(this HashSet<Aff<A>> ma, Func<A, B> f, int windowSize) =>
            AffMaybe<HashSet<B>>(async () =>
            {
                var rs = await ma.AsEnumerable().Map(m => m.Run()).WindowMap(windowSize, fa => fa.Map(f)).ConfigureAwait(false);

                var (fails, succs) = rs.Partition();
                var fails1 = fails.Take(1).ToArray();
                
                return fails1.Length == 1
                           ? FinFail<HashSet<B>>(fails1[0])
                           : FinSucc<HashSet<B>>(toHashSet(succs));
            });

        [Pure]
        public static Aff<HashSet<B>> TraverseSerial<A, B>(this HashSet<Aff<A>> ma, Func<A, B> f) =>
            AffMaybe<HashSet<B>>(async () =>
            {
                var rs = new List<B>();
                foreach (var m in ma)
                {
                    var r = await m.Run().ConfigureAwait(false);;
                    if (r.IsFail) return FinFail<HashSet<B>>(r.Error);
                    rs.Add(f(r.Value));
                }
                return FinSucc(new HashSet<B>(rs));
            });
        
 
        [Pure]
        public static Aff<IEnumerable<B>> TraverseParallel<A, B>(this IEnumerable<Aff<A>> ma, Func<A, B> f, int windowSize) =>
            AffMaybe<IEnumerable<B>>(async () =>
            {
                var rs = await ma.AsEnumerable().Map(m => m.Run()).WindowMap(windowSize, fa => fa.Map(f)).ConfigureAwait(false);

                var (fails, succs) = rs.Partition();
                var fails1 = fails.Take(1).ToArray();
                
                return fails1.Length == 1
                           ? FinFail<IEnumerable<B>>(fails1[0])
                           : FinSucc<IEnumerable<B>>(succs);
            });

        [Pure]
        public static Aff<IEnumerable<B>> TraverseSerial<A, B>(this IEnumerable<Aff<A>> ma, Func<A, B> f) =>
            AffMaybe<IEnumerable<B>>(async () =>
            {
                var rs = new List<B>();
                foreach (var m in ma)
                {
                    var r = await m.Run().ConfigureAwait(false);
                    if (r.IsFail) return FinFail<IEnumerable<B>>(r.Error);
                    rs.Add(f(r.Value));
                }
                return FinSucc(rs.AsEnumerable());
            });

 
        [Pure]
        public static Aff<Lst<B>> TraverseParallel<A, B>(this Lst<Aff<A>> ma, Func<A, B> f) =>
            TraverseParallel<A, B>(ma, f, SysInfo.DefaultAsyncSequenceParallelism);
 
        [Pure]
        public static Aff<Lst<B>> TraverseParallel<A, B>(this Lst<Aff<A>> ma, Func<A, B> f, int windowSize) =>
            AffMaybe<Lst<B>>(async () =>
            {
                var rs = await ma.AsEnumerable().Map(m => m.Run()).WindowMap(windowSize, fa => fa.Map(f)).ConfigureAwait(false);

                var (fails, succs) = rs.Partition();
                var fails1 = fails.Take(1).ToArray();

                return fails1.Length == 1
                           ? FinFail<Lst<B>>(fails1[0])
                           : FinSucc<Lst<B>>(toList(succs));
            });

        [Pure]
        public static Aff<Lst<B>> TraverseSerial<A, B>(this Lst<Aff<A>> ma, Func<A, B> f) =>
            AffMaybe<Lst<B>>(async () =>
            {
                var rs = new List<B>();
                foreach (var m in ma)
                {
                    var r = await m.Run().ConfigureAwait(false);
                    if (r.IsFail) return FinFail<Lst<B>>(r.Error);
                    rs.Add(f(r.Value));
                }
                return FinSucc(rs.Freeze());
            });


        
        [Pure]
        public static Aff<Que<B>> TraverseParallel<A, B>(this Que<Aff<A>> ma, Func<A, B> f) =>
            TraverseParallel<A, B>(ma, f, SysInfo.DefaultAsyncSequenceParallelism);
 
        [Pure]
        public static Aff<Que<B>> TraverseParallel<A, B>(this Que<Aff<A>> ma, Func<A, B> f, int windowSize) =>
            AffMaybe<Que<B>>(async () =>
            {
                var rs = await ma.AsEnumerable().Map(m => m.Run()).WindowMap(windowSize, fa => fa.Map(f)).ConfigureAwait(false);

                var (fails, succs) = rs.Partition();
                var fails1 = fails.Take(1).ToArray();

                return fails1.Length == 1
                           ? FinFail<Que<B>>(fails1[0])
                           : FinSucc<Que<B>>(toQueue(succs));
            });

        [Pure]
        public static Aff<Que<B>> TraverseSerial<A, B>(this Que<Aff<A>> ma, Func<A, B> f) =>
            AffMaybe<Que<B>>(async () =>
            {
                var rs = new List<B>();
                foreach (var m in ma)
                {
                    var r = await m.Run().ConfigureAwait(false);
                    if (r.IsFail) return FinFail<Que<B>>(r.Error);
                    rs.Add(f(r.Value));
                }
                return FinSucc(toQueue(rs));
            });

        
        
        
        [Pure]
        public static Aff<Seq<B>> TraverseParallel<A, B>(this Seq<Aff<A>> ma, Func<A, B> f) =>
            TraverseParallel<A, B>(ma, f, SysInfo.DefaultAsyncSequenceParallelism);
 
        [Pure]
        public static Aff<Seq<B>> TraverseParallel<A, B>(this Seq<Aff<A>> ma, Func<A, B> f, int windowSize) =>
            AffMaybe<Seq<B>>(async () =>
            {
                var rs = await ma.AsEnumerable().Map(m => m.Run()).WindowMap(windowSize, fa => fa.Map(f)).ConfigureAwait(false);

                var (fails, succs) = rs.Partition();
                var fails1 = fails.Take(1).ToArray();

                return fails1.Length == 1
                           ? FinFail<Seq<B>>(fails1[0])
                           : FinSucc<Seq<B>>(Seq.FromArray(succs.ToArray()));
            });

        [Pure]
        public static Aff<Seq<B>> TraverseSerial<A, B>(this Seq<Aff<A>> ma, Func<A, B> f) =>
            AffMaybe<Seq<B>>(async () =>
            {
                var rs = new List<B>();
                foreach (var m in ma)
                {
                    var r = await m.Run().ConfigureAwait(false);
                    if (r.IsFail) return FinFail<Seq<B>>(r.Error);
                    rs.Add(f(r.Value));
                }
                return FinSucc(Seq.FromArray(rs.ToArray()));
            });

        [Pure]
        public static Aff<Set<B>> TraverseParallel<A, B>(this Set<Aff<A>> ma, Func<A, B> f) =>
            TraverseParallel<A, B>(ma, f, SysInfo.DefaultAsyncSequenceParallelism);
 
        [Pure]
        public static Aff<Set<B>> TraverseParallel<A, B>(this Set<Aff<A>> ma, Func<A, B> f, int windowSize) =>
            AffMaybe<Set<B>>(async () =>
            {
                var rs = await ma.AsEnumerable().Map(m => m.Run()).WindowMap(windowSize, fa => fa.Map(f)).ConfigureAwait(false);

                var (fails, succs) = rs.Partition();
                var fails1 = fails.Take(1).ToArray();

                return fails1.Length == 1
                           ? FinFail<Set<B>>(fails1[0])
                           : FinSucc<Set<B>>(toSet(succs));
            });

        [Pure]
        public static Aff<Set<B>> TraverseSerial<A, B>(this Set<Aff<A>> ma, Func<A, B> f) =>
            AffMaybe<Set<B>>(async () =>
            {
                var rs = new List<B>();
                foreach (var m in ma)
                {
                    var r = await m.Run().ConfigureAwait(false);
                    if (r.IsFail) return FinFail<Set<B>>(r.Error);
                    rs.Add(f(r.Value));
                }
                return FinSucc(toSet(rs));
            });
        
        

        
        
        [Pure]
        public static Aff<Stck<B>> TraverseParallel<A, B>(this Stck<Aff<A>> ma, Func<A, B> f) =>
            TraverseParallel<A, B>(ma, f, SysInfo.DefaultAsyncSequenceParallelism);
 
        [Pure]
        public static Aff<Stck<B>> TraverseParallel<A, B>(this Stck<Aff<A>> ma, Func<A, B> f, int windowSize) =>
            AffMaybe<Stck<B>>(async () =>
            {
                var rs = await ma.AsEnumerable().Map(m => m.Run()).WindowMap(windowSize, fa => fa.Map(f)).ConfigureAwait(false);

                var (fails, succs) = rs.Partition();
                var fails1 = fails.Take(1).ToArray();

                return fails1.Length == 1
                           ? FinFail<Stck<B>>(fails1[0])
                           : FinSucc<Stck<B>>(toStack(succs));
            });

        [Pure]
        public static Aff<Stck<B>> TraverseSerial<A, B>(this Stck<Aff<A>> ma, Func<A, B> f) =>
            AffMaybe<Stck<B>>(async () =>
            {
                var rs = new List<B>();
                foreach (var m in ma)
                {
                    var r = await m.Run().ConfigureAwait(false);;
                    if (r.IsFail) return FinFail<Stck<B>>(r.Error);
                    rs.Add(f(r.Value));
                }
                return FinSucc(toStack(rs));
            });
        
        
        //
        // Async types
        //

        public static Aff<EitherAsync<L, B>> Traverse<L, A, B>(this EitherAsync<L, Aff<A>> ma, Func<A, B> f)
        {
            return AffMaybe<EitherAsync<L, B>>(() => Go(ma, f));
            async ValueTask<Fin<EitherAsync<L, B>>> Go(EitherAsync<L, Aff<A>> ma, Func<A, B> f)
            {
                var da = await ma.Data.ConfigureAwait(false);
                if (da.State == EitherStatus.IsBottom) return default;
                if (da.State == EitherStatus.IsLeft) return FinSucc<EitherAsync<L,B>>(da.Left);
                var rb = await da.Right.Run().ConfigureAwait(false);
                if (rb.IsFail) return FinFail<EitherAsync<L, B>>(rb.Error);
                return FinSucc<EitherAsync<L, B>>(EitherAsync<L, B>.Right(f(rb.Value)));
            }
        }

        public static Aff<OptionAsync<B>> Traverse<A, B>(this OptionAsync<Aff<A>> ma, Func<A, B> f)
        {
            return AffMaybe<OptionAsync<B>>(() => Go(ma, f));
            async ValueTask<Fin<OptionAsync<B>>> Go(OptionAsync<Aff<A>> ma, Func<A, B> f)
            {
                var (isSome, value) = await ma.Data.ConfigureAwait(false);
                if (!isSome) return FinSucc<OptionAsync<B>>(OptionAsync<B>.None);
                var rb = await value.Run().ConfigureAwait(false);
                if (rb.IsFail) return FinFail<OptionAsync<B>>(rb.Error);
                return FinSucc<OptionAsync<B>>(OptionAsync<B>.Some(f(rb.Value)));
            }
        }
        
        public static Aff<TryAsync<B>> Traverse<A, B>(this TryAsync<Aff<A>> ma, Func<A, B> f)
        {
            return AffMaybe<TryAsync<B>>(() => Go(ma, f));
            async ValueTask<Fin<TryAsync<B>>> Go(TryAsync<Aff<A>> ma, Func<A, B> f)
            {
                var ra = await ma.Try().ConfigureAwait(false);
                if (ra.IsFaulted) return FinSucc<TryAsync<B>>(TryAsync<B>(ra.Exception));
                var rb = await ra.Value.Run().ConfigureAwait(false);
                if (rb.IsFail) return FinFail<TryAsync<B>>(rb.Error);
                return FinSucc<TryAsync<B>>(TryAsync<B>(f(rb.Value)));
            }
        }
        
        public static Aff<TryOptionAsync<B>> Traverse<A, B>(this TryOptionAsync<Aff<A>> ma, Func<A, B> f)
        {
            return AffMaybe<TryOptionAsync<B>>(() => Go(ma, f));
            async ValueTask<Fin<TryOptionAsync<B>>> Go(TryOptionAsync<Aff<A>> ma, Func<A, B> f)
            {
                var ra = await ma.Try().ConfigureAwait(false);
                if (ra.IsFaulted) return FinSucc<TryOptionAsync<B>>(TryOptionAsync<B>(ra.Exception));
                if (ra.IsNone) return FinSucc<TryOptionAsync<B>>(TryOptionAsync<B>(None));
                var rb = await ra.Value.Value.Run().ConfigureAwait(false);
                if (rb.IsFail) return FinFail<TryOptionAsync<B>>(rb.Error);
                return FinSucc<TryOptionAsync<B>>(TryOptionAsync<B>(f(rb.Value)));
            }
        }

        public static Aff<Task<B>> Traverse<A, B>(this Task<Aff<A>> ma, Func<A, B> f)
        {
            return AffMaybe<Task<B>>(() => Go(ma, f));
            async ValueTask<Fin<Task<B>>> Go(Task<Aff<A>> ma, Func<A, B> f)
            {
                var ra = await ma.ConfigureAwait(false);
                var rb = await ra.Run().ConfigureAwait(false);
                if (rb.IsFail) return FinFail<Task<B>>(rb.Error);
                return FinSucc<Task<B>>(f(rb.Value).AsTask());
            }
        }

        public static Aff<ValueTask<B>> Traverse<A, B>(this ValueTask<Aff<A>> ma, Func<A, B> f)
        {
            return AffMaybe<ValueTask<B>>(() => Go(ma, f));
            async ValueTask<Fin<ValueTask<B>>> Go(ValueTask<Aff<A>> ma, Func<A, B> f)
            {
                var ra = await ma.ConfigureAwait(false);
                var rb = await ra.Run().ConfigureAwait(false);
                if (rb.IsFail) return FinFail<ValueTask<B>>(rb.Error);
                return FinSucc<ValueTask<B>>(f(rb.Value).AsValueTask());
            }
        }
        
        public static Aff<Aff<B>> Traverse<A, B>(this Aff<Aff<A>> ma, Func<A, B> f)
        {
            return AffMaybe<Aff<B>>(() => Go(ma, f));
            async ValueTask<Fin<Aff<B>>> Go(Aff<Aff<A>> ma, Func<A, B> f)
            {
                var ra = await ma.Run().ConfigureAwait(false);
                if (ra.IsFail) return FinSucc<Aff<B>>(FailAff<B>(ra.Error));
                var rb = await ra.Value.Run().ConfigureAwait(false);
                if (rb.IsFail) return FinFail<Aff<B>>(rb.Error);
                return FinSucc<Aff<B>>(SuccessAff<B>(f(rb.Value)));
            }
        }
        
        //
        // Sync types
        // 
        
        public static Aff<Either<L, B>> Traverse<L, A, B>(this Either<L, Aff<A>> ma, Func<A, B> f)
        {
            return AffMaybe<Either<L, B>>(() => Go(ma, f));
            async ValueTask<Fin<Either<L, B>>> Go(Either<L, Aff<A>> ma, Func<A, B> f)
            {
                if(ma.IsBottom) return default;
                if(ma.IsLeft) return FinSucc<Either<L, B>>(ma.LeftValue);
                var rb = await ma.RightValue.Run().ConfigureAwait(false);
                if(rb.IsFail) return FinFail<Either<L, B>>(rb.Error);
                return FinSucc<Either<L, B>>(f(rb.Value));
            }
        }

        public static Aff<EitherUnsafe<L, B>> Traverse<L, A, B>(this EitherUnsafe<L, Aff<A>> ma, Func<A, B> f)
        {
            return AffMaybe<EitherUnsafe<L, B>>(() => Go(ma, f));
            async ValueTask<Fin<EitherUnsafe<L, B>>> Go(EitherUnsafe<L, Aff<A>> ma, Func<A, B> f)
            {
                if(ma.IsBottom) return default;
                if(ma.IsLeft) return FinSucc<EitherUnsafe<L, B>>(ma.LeftValue);
                var rb = await ma.RightValue.Run().ConfigureAwait(false);
                if(rb.IsFail) return FinFail<EitherUnsafe<L, B>>(rb.Error);
                return FinSucc<EitherUnsafe<L, B>>(f(rb.Value));
            }
        }

        public static Aff<Identity<B>> Traverse<A, B>(this Identity<Aff<A>> ma, Func<A, B> f)
        {
            return AffMaybe<Identity<B>>(() => Go(ma, f));
            async ValueTask<Fin<Identity<B>>> Go(Identity<Aff<A>> ma, Func<A, B> f)
            {
                if(ma.IsBottom) return default;
                var rb = await ma.Value.Run().ConfigureAwait(false);
                if(rb.IsFail) return FinFail<Identity<B>>(rb.Error);
                return FinSucc<Identity<B>>(new Identity<B>(f(rb.Value)));
            }
        }

        public static Aff<Fin<B>> Traverse<A, B>(this Fin<Aff<A>> ma, Func<A, B> f)
        {
            return AffMaybe<Fin<B>>(() => Go(ma, f));
            async ValueTask<Fin<Fin<B>>> Go(Fin<Aff<A>> ma, Func<A, B> f)
            {
                if(ma.IsFail) return FinSucc<Fin<B>>(ma.Cast<B>());
                var rb = await ma.Value.Run().ConfigureAwait(false);
                if(rb.IsFail) return FinFail<Fin<B>>(rb.Error);
                return FinSucc<Fin<B>>(Fin<B>.Succ(f(rb.Value)));
            }
        }

        public static Aff<Option<B>> Traverse<A, B>(this Option<Aff<A>> ma, Func<A, B> f)
        {
            return AffMaybe<Option<B>>(() => Go(ma, f));
            async ValueTask<Fin<Option<B>>> Go(Option<Aff<A>> ma, Func<A, B> f)
            {
                if(ma.IsNone) return FinSucc<Option<B>>(None);
                var rb = await ma.Value.Run().ConfigureAwait(false);
                if(rb.IsFail) return FinFail<Option<B>>(rb.Error);
                return FinSucc<Option<B>>(Option<B>.Some(f(rb.Value)));
            }
        }
        
        public static Aff<OptionUnsafe<B>> Traverse<A, B>(this OptionUnsafe<Aff<A>> ma, Func<A, B> f)
        {
            return AffMaybe<OptionUnsafe<B>>(() => Go(ma, f));
            async ValueTask<Fin<OptionUnsafe<B>>> Go(OptionUnsafe<Aff<A>> ma, Func<A, B> f)
            {
                if(ma.IsNone) return FinSucc<OptionUnsafe<B>>(None);
                var rb = await ma.Value.Run().ConfigureAwait(false);
                if(rb.IsFail) return FinFail<OptionUnsafe<B>>(rb.Error);
                return FinSucc<OptionUnsafe<B>>(OptionUnsafe<B>.Some(f(rb.Value)));
            }
        }
        
        public static Aff<Try<B>> Traverse<A, B>(this Try<Aff<A>> ma, Func<A, B> f)
        {
            return AffMaybe<Try<B>>(() => Go(ma, f));
            async ValueTask<Fin<Try<B>>> Go(Try<Aff<A>> ma, Func<A, B> f)
            {
                var ra = ma.Try();
                if (ra.IsFaulted) return FinSucc<Try<B>>(TryFail<B>(ra.Exception));
                var rb = await ra.Value.Run().ConfigureAwait(false);
                if (rb.IsFail) return FinFail<Try<B>>(rb.Error);
                return FinSucc<Try<B>>(Try<B>(f(rb.Value)));
            }
        }
        
        public static Aff<TryOption<B>> Traverse<A, B>(this TryOption<Aff<A>> ma, Func<A, B> f)
        {
            return AffMaybe<TryOption<B>>(() => Go(ma, f));
            async ValueTask<Fin<TryOption<B>>> Go(TryOption<Aff<A>> ma, Func<A, B> f)
            {
                var ra = ma.Try();
                if (ra.IsBottom) return default;
                if (ra.IsNone) return FinSucc<TryOption<B>>(TryOptional<B>(None));
                if (ra.IsFaulted) return FinSucc<TryOption<B>>(TryOptionFail<B>(ra.Exception));
                var rb = await ra.Value.Value.Run().ConfigureAwait(false);
                if (rb.IsFail) return FinFail<TryOption<B>>(rb.Error);
                return FinSucc<TryOption<B>>(TryOption<B>(f(rb.Value)));
            }
        }
        
        public static Aff<Validation<Fail, B>> Traverse<Fail, A, B>(this Validation<Fail, Aff<A>> ma, Func<A, B> f)
        {
            return AffMaybe<Validation<Fail, B>>(() => Go(ma, f));
            async ValueTask<Fin<Validation<Fail, B>>> Go(Validation<Fail, Aff<A>> ma, Func<A, B> f)
            {
                if (ma.IsFail) return FinSucc<Validation<Fail, B>>(Fail<Fail, B>(ma.FailValue));
                var rb = await ma.SuccessValue.Run().ConfigureAwait(false);
                if(rb.IsFail) return FinFail<Validation<Fail, B>>(rb.Error);
                return FinSucc<Validation<Fail, B>>(f(rb.Value));
            }
        }
        
        public static Aff<Validation<MonoidFail, Fail, B>> Traverse<MonoidFail, Fail, A, B>(this Validation<MonoidFail, Fail, Aff<A>> ma, Func<A, B> f)
            where MonoidFail : struct, Monoid<Fail>, Eq<Fail>
        {
            return AffMaybe<Validation<MonoidFail, Fail, B>>(() => Go(ma, f));
            async ValueTask<Fin<Validation<MonoidFail, Fail, B>>> Go(Validation<MonoidFail, Fail, Aff<A>> ma, Func<A, B> f)
            {
                if (ma.IsFail) return FinSucc<Validation<MonoidFail, Fail, B>>(Fail<MonoidFail, Fail, B>(ma.FailValue));
                var rb = await ma.SuccessValue.Run().ConfigureAwait(false);
                if(rb.IsFail) return FinFail<Validation<MonoidFail, Fail, B>>(rb.Error);
                return FinSucc<Validation<MonoidFail, Fail, B>>(f(rb.Value));
            }
        }
        
        public static Aff<Eff<B>> Traverse<A, B>(this Eff<Aff<A>> ma, Func<A, B> f)
        {
            return AffMaybe<Eff<B>>(() => Go(ma, f));
            async ValueTask<Fin<Eff<B>>> Go(Eff<Aff<A>> ma, Func<A, B> f)
            {
                var ra = ma.Run();
                if (ra.IsFail) return FinSucc<Eff<B>>(FailEff<B>(ra.Error));
                var rb = await ra.Value.Run().ConfigureAwait(false);
                if (rb.IsFail) return FinFail<Eff<B>>(rb.Error);
                return FinSucc<Eff<B>>(SuccessEff<B>(f(rb.Value)));
            }
        }
    }
}
