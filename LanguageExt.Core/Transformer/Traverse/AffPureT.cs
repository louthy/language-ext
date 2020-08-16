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
using LanguageExt.Interfaces;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    public static partial class AffPureT
    {
        //
        // Collections
        //
        [Pure]
        public static AffPure<Arr<B>> TraverseParallel<A, B>(this Arr<AffPure<A>> ma, Func<A, B> f) =>
            TraverseParallel<A, B>(ma, f, Sys.DefaultAsyncSequenceConcurrency);
 
        [Pure]
        public static AffPure<Arr<B>> TraverseParallel<A, B>(this Arr<AffPure<A>> ma, Func<A, B> f, int windowSize) =>
            AffMaybe<Arr<B>>(async () =>
            {
                var rs = await ma.AsEnumerable().Map(m => m.RunIO()).WindowMap(windowSize, fa => fa.Map(f));

                var (fails, succs) = rs.Partition();
                return fails.Any()
                    ? FinFail<Arr<B>>(fails.Head())
                    : FinSucc<Arr<B>>(toArray(succs));
            });

        [Pure]
        public static AffPure<Arr<B>> TraverseSerial<A, B>(this Arr<AffPure<A>> ma, Func<A, B> f) =>
            AffMaybe<Arr<B>>(async () =>
            {
                var rs = new List<B>();
                foreach (var m in ma)
                {
                    var r = await m.RunIO();
                    if (r.IsFail) return FinFail<Arr<B>>(r.Error);
                    rs.Add(f(r.Value));
                }
                return FinSucc(new Arr<B>(rs.ToArray()));
            });

        
        [Pure]
        public static AffPure<HashSet<B>> TraverseParallel<A, B>(this HashSet<AffPure<A>> ma, Func<A, B> f) =>
            TraverseParallel<A, B>(ma, f, Sys.DefaultAsyncSequenceConcurrency);
 
        [Pure]
        public static AffPure<HashSet<B>> TraverseParallel<A, B>(this HashSet<AffPure<A>> ma, Func<A, B> f, int windowSize) =>
            AffMaybe<HashSet<B>>(async () =>
            {
                var rs = await ma.AsEnumerable().Map(m => m.RunIO()).WindowMap(windowSize, fa => fa.Map(f));

                var (fails, succs) = rs.Partition();
                return fails.Any()
                    ? FinFail<HashSet<B>>(fails.Head())
                    : FinSucc<HashSet<B>>(toHashSet(succs));
            });

        [Pure]
        public static AffPure<HashSet<B>> TraverseSerial<A, B>(this HashSet<AffPure<A>> ma, Func<A, B> f) =>
            AffMaybe<HashSet<B>>(async () =>
            {
                var rs = new List<B>();
                foreach (var m in ma)
                {
                    var r = await m.RunIO();
                    if (r.IsFail) return FinFail<HashSet<B>>(r.Error);
                    rs.Add(f(r.Value));
                }
                return FinSucc(new HashSet<B>(rs));
            });
        
 
        [Pure]
        public static AffPure<IEnumerable<B>> TraverseParallel<A, B>(this IEnumerable<AffPure<A>> ma, Func<A, B> f, int windowSize) =>
            AffMaybe<IEnumerable<B>>(async () =>
            {
                var rs = await ma.AsEnumerable().Map(m => m.RunIO()).WindowMap(windowSize, fa => fa.Map(f));

                var (fails, succs) = rs.Partition();
                return fails.Any()
                    ? FinFail<IEnumerable<B>>(fails.Head())
                    : FinSucc<IEnumerable<B>>(succs);
            });

        [Pure]
        public static AffPure<IEnumerable<B>> TraverseSerial<A, B>(this IEnumerable<AffPure<A>> ma, Func<A, B> f) =>
            AffMaybe<IEnumerable<B>>(async () =>
            {
                var rs = new List<B>();
                foreach (var m in ma)
                {
                    var r = await m.RunIO();
                    if (r.IsFail) return FinFail<IEnumerable<B>>(r.Error);
                    rs.Add(f(r.Value));
                }
                return FinSucc(rs.AsEnumerable());
            });

 
        [Pure]
        public static AffPure<Lst<B>> TraverseParallel<A, B>(this Lst<AffPure<A>> ma, Func<A, B> f) =>
            TraverseParallel<A, B>(ma, f, Sys.DefaultAsyncSequenceConcurrency);
 
        [Pure]
        public static AffPure<Lst<B>> TraverseParallel<A, B>(this Lst<AffPure<A>> ma, Func<A, B> f, int windowSize) =>
            AffMaybe<Lst<B>>(async () =>
            {
                var rs = await ma.AsEnumerable().Map(m => m.RunIO()).WindowMap(windowSize, fa => fa.Map(f));

                var (fails, succs) = rs.Partition();
                return fails.Any()
                    ? FinFail<Lst<B>>(fails.Head())
                    : FinSucc<Lst<B>>(toList(succs));
            });

        [Pure]
        public static AffPure<Lst<B>> TraverseSerial<A, B>(this Lst<AffPure<A>> ma, Func<A, B> f) =>
            AffMaybe<Lst<B>>(async () =>
            {
                var rs = new List<B>();
                foreach (var m in ma)
                {
                    var r = await m.RunIO();
                    if (r.IsFail) return FinFail<Lst<B>>(r.Error);
                    rs.Add(f(r.Value));
                }
                return FinSucc(rs.Freeze());
            });


        
        [Pure]
        public static AffPure<Que<B>> TraverseParallel<A, B>(this Que<AffPure<A>> ma, Func<A, B> f) =>
            TraverseParallel<A, B>(ma, f, Sys.DefaultAsyncSequenceConcurrency);
 
        [Pure]
        public static AffPure<Que<B>> TraverseParallel<A, B>(this Que<AffPure<A>> ma, Func<A, B> f, int windowSize) =>
            AffMaybe<Que<B>>(async () =>
            {
                var rs = await ma.AsEnumerable().Map(m => m.RunIO()).WindowMap(windowSize, fa => fa.Map(f));

                var (fails, succs) = rs.Partition();
                return fails.Any()
                    ? FinFail<Que<B>>(fails.Head())
                    : FinSucc<Que<B>>(toQueue(succs));
            });

        [Pure]
        public static AffPure<Que<B>> TraverseSerial<A, B>(this Que<AffPure<A>> ma, Func<A, B> f) =>
            AffMaybe<Que<B>>(async () =>
            {
                var rs = new List<B>();
                foreach (var m in ma)
                {
                    var r = await m.RunIO();
                    if (r.IsFail) return FinFail<Que<B>>(r.Error);
                    rs.Add(f(r.Value));
                }
                return FinSucc(toQueue(rs));
            });

        
        
        
        [Pure]
        public static AffPure<Seq<B>> TraverseParallel<A, B>(this Seq<AffPure<A>> ma, Func<A, B> f) =>
            TraverseParallel<A, B>(ma, f, Sys.DefaultAsyncSequenceConcurrency);
 
        [Pure]
        public static AffPure<Seq<B>> TraverseParallel<A, B>(this Seq<AffPure<A>> ma, Func<A, B> f, int windowSize) =>
            AffMaybe<Seq<B>>(async () =>
            {
                var rs = await ma.AsEnumerable().Map(m => m.RunIO()).WindowMap(windowSize, fa => fa.Map(f));

                var (fails, succs) = rs.Partition();
                return fails.Any()
                    ? FinFail<Seq<B>>(fails.Head())
                    : FinSucc<Seq<B>>(Seq.FromArray(succs.ToArray()));
            });

        [Pure]
        public static AffPure<Seq<B>> TraverseSerial<A, B>(this Seq<AffPure<A>> ma, Func<A, B> f) =>
            AffMaybe<Seq<B>>(async () =>
            {
                var rs = new List<B>();
                foreach (var m in ma)
                {
                    var r = await m.RunIO();
                    if (r.IsFail) return FinFail<Seq<B>>(r.Error);
                    rs.Add(f(r.Value));
                }
                return FinSucc(Seq.FromArray(rs.ToArray()));
            });

        [Pure]
        public static AffPure<Set<B>> TraverseParallel<A, B>(this Set<AffPure<A>> ma, Func<A, B> f) =>
            TraverseParallel<A, B>(ma, f, Sys.DefaultAsyncSequenceConcurrency);
 
        [Pure]
        public static AffPure<Set<B>> TraverseParallel<A, B>(this Set<AffPure<A>> ma, Func<A, B> f, int windowSize) =>
            AffMaybe<Set<B>>(async () =>
            {
                var rs = await ma.AsEnumerable().Map(m => m.RunIO()).WindowMap(windowSize, fa => fa.Map(f));

                var (fails, succs) = rs.Partition();
                return fails.Any()
                    ? FinFail<Set<B>>(fails.Head())
                    : FinSucc<Set<B>>(toSet(succs));
            });

        [Pure]
        public static AffPure<Set<B>> TraverseSerial<A, B>(this Set<AffPure<A>> ma, Func<A, B> f) =>
            AffMaybe<Set<B>>(async () =>
            {
                var rs = new List<B>();
                foreach (var m in ma)
                {
                    var r = await m.RunIO();
                    if (r.IsFail) return FinFail<Set<B>>(r.Error);
                    rs.Add(f(r.Value));
                }
                return FinSucc(toSet(rs));
            });
        
        

        
        
        [Pure]
        public static AffPure<Stck<B>> TraverseParallel<A, B>(this Stck<AffPure<A>> ma, Func<A, B> f) =>
            TraverseParallel<A, B>(ma, f, Sys.DefaultAsyncSequenceConcurrency);
 
        [Pure]
        public static AffPure<Stck<B>> TraverseParallel<A, B>(this Stck<AffPure<A>> ma, Func<A, B> f, int windowSize) =>
            AffMaybe<Stck<B>>(async () =>
            {
                var rs = await ma.AsEnumerable().Map(m => m.RunIO()).WindowMap(windowSize, fa => fa.Map(f));

                var (fails, succs) = rs.Partition();
                return fails.Any()
                    ? FinFail<Stck<B>>(fails.Head())
                    : FinSucc<Stck<B>>(toStack(succs));
            });

        [Pure]
        public static AffPure<Stck<B>> TraverseSerial<A, B>(this Stck<AffPure<A>> ma, Func<A, B> f) =>
            AffMaybe<Stck<B>>(async () =>
            {
                var rs = new List<B>();
                foreach (var m in ma)
                {
                    var r = await m.RunIO();
                    if (r.IsFail) return FinFail<Stck<B>>(r.Error);
                    rs.Add(f(r.Value));
                }
                return FinSucc(toStack(rs));
            });
        
        
        //
        // Async types
        //

        public static AffPure<EitherAsync<L, B>> Traverse<L, A, B>(this EitherAsync<L, AffPure<A>> ma, Func<A, B> f)
        {
            return AffMaybe<EitherAsync<L, B>>(() => Go(ma, f));
            async ValueTask<Fin<EitherAsync<L, B>>> Go(EitherAsync<L, AffPure<A>> ma, Func<A, B> f)
            {
                var da = await ma.Data.ConfigureAwait(false);
                if (da.State == EitherStatus.IsBottom) return default;
                if (da.State == EitherStatus.IsLeft) return FinSucc<EitherAsync<L,B>>(da.Left);
                var rb = await da.Right.RunIO().ConfigureAwait(false);
                if (rb.IsFail) return FinFail<EitherAsync<L, B>>(rb.Error);
                return FinSucc<EitherAsync<L, B>>(EitherAsync<L, B>.Right(f(rb.Value)));
            }
        }

        public static AffPure<OptionAsync<B>> Traverse<A, B>(this OptionAsync<AffPure<A>> ma, Func<A, B> f)
        {
            return AffMaybe<OptionAsync<B>>(() => Go(ma, f));
            async ValueTask<Fin<OptionAsync<B>>> Go(OptionAsync<AffPure<A>> ma, Func<A, B> f)
            {
                var (isSome, value) = await ma.Data.ConfigureAwait(false);
                if (!isSome) return FinSucc<OptionAsync<B>>(OptionAsync<B>.None);
                var rb = await value.RunIO().ConfigureAwait(false);
                if (rb.IsFail) return FinFail<OptionAsync<B>>(rb.Error);
                return FinSucc<OptionAsync<B>>(OptionAsync<B>.Some(f(rb.Value)));
            }
        }
        
        public static AffPure<TryAsync<B>> Traverse<A, B>(this TryAsync<AffPure<A>> ma, Func<A, B> f)
        {
            return AffMaybe<TryAsync<B>>(() => Go(ma, f));
            async ValueTask<Fin<TryAsync<B>>> Go(TryAsync<AffPure<A>> ma, Func<A, B> f)
            {
                var ra = await ma.Try().ConfigureAwait(false);
                if (ra.IsFaulted) return FinSucc<TryAsync<B>>(TryAsync<B>(ra.Exception));
                var rb = await ra.Value.RunIO().ConfigureAwait(false);
                if (rb.IsFail) return FinFail<TryAsync<B>>(rb.Error);
                return FinSucc<TryAsync<B>>(TryAsync<B>(f(rb.Value)));
            }
        }
        
        public static AffPure<TryOptionAsync<B>> Traverse<A, B>(this TryOptionAsync<AffPure<A>> ma, Func<A, B> f)
        {
            return AffMaybe<TryOptionAsync<B>>(() => Go(ma, f));
            async ValueTask<Fin<TryOptionAsync<B>>> Go(TryOptionAsync<AffPure<A>> ma, Func<A, B> f)
            {
                var ra = await ma.Try().ConfigureAwait(false);
                if (ra.IsFaulted) return FinSucc<TryOptionAsync<B>>(TryOptionAsync<B>(ra.Exception));
                if (ra.IsNone) return FinSucc<TryOptionAsync<B>>(TryOptionAsync<B>(None));
                var rb = await ra.Value.Value.RunIO().ConfigureAwait(false);
                if (rb.IsFail) return FinFail<TryOptionAsync<B>>(rb.Error);
                return FinSucc<TryOptionAsync<B>>(TryOptionAsync<B>(f(rb.Value)));
            }
        }

        public static AffPure<Task<B>> Traverse<A, B>(this Task<AffPure<A>> ma, Func<A, B> f)
        {
            return AffMaybe<Task<B>>(() => Go(ma, f));
            async ValueTask<Fin<Task<B>>> Go(Task<AffPure<A>> ma, Func<A, B> f)
            {
                var ra = await ma.ConfigureAwait(false);
                var rb = await ra.RunIO().ConfigureAwait(false);
                if (rb.IsFail) return FinFail<Task<B>>(rb.Error);
                return FinSucc<Task<B>>(f(rb.Value).AsTask());
            }
        }

        public static AffPure<ValueTask<B>> Traverse<A, B>(this ValueTask<AffPure<A>> ma, Func<A, B> f)
        {
            return AffMaybe<ValueTask<B>>(() => Go(ma, f));
            async ValueTask<Fin<ValueTask<B>>> Go(ValueTask<AffPure<A>> ma, Func<A, B> f)
            {
                var ra = await ma.ConfigureAwait(false);
                var rb = await ra.RunIO().ConfigureAwait(false);
                if (rb.IsFail) return FinFail<ValueTask<B>>(rb.Error);
                return FinSucc<ValueTask<B>>(f(rb.Value).AsValueTask());
            }
        }
        
        public static AffPure<AffPure<B>> Traverse<A, B>(this AffPure<AffPure<A>> ma, Func<A, B> f)
        {
            return AffMaybe<AffPure<B>>(() => Go(ma, f));
            async ValueTask<Fin<AffPure<B>>> Go(AffPure<AffPure<A>> ma, Func<A, B> f)
            {
                var ra = await ma.RunIO().ConfigureAwait(false);
                if (ra.IsFail) return FinSucc<AffPure<B>>(FailAff<B>(ra.Error));
                var rb = await ra.Value.RunIO().ConfigureAwait(false);
                if (rb.IsFail) return FinFail<AffPure<B>>(rb.Error);
                return FinSucc<AffPure<B>>(SuccessAff<B>(f(rb.Value)));
            }
        }
        
        //
        // Sync types
        // 
        
        public static AffPure<Either<L, B>> Traverse<L, A, B>(this Either<L, AffPure<A>> ma, Func<A, B> f)
        {
            return AffMaybe<Either<L, B>>(() => Go(ma, f));
            async ValueTask<Fin<Either<L, B>>> Go(Either<L, AffPure<A>> ma, Func<A, B> f)
            {
                if(ma.IsBottom) return default;
                if(ma.IsLeft) return FinSucc<Either<L, B>>(ma.LeftValue);
                var rb = await ma.RightValue.RunIO().ConfigureAwait(false);
                if(rb.IsFail) return FinFail<Either<L, B>>(rb.Error);
                return FinSucc<Either<L, B>>(f(rb.Value));
            }
        }

        public static AffPure<EitherUnsafe<L, B>> Traverse<L, A, B>(this EitherUnsafe<L, AffPure<A>> ma, Func<A, B> f)
        {
            return AffMaybe<EitherUnsafe<L, B>>(() => Go(ma, f));
            async ValueTask<Fin<EitherUnsafe<L, B>>> Go(EitherUnsafe<L, AffPure<A>> ma, Func<A, B> f)
            {
                if(ma.IsBottom) return default;
                if(ma.IsLeft) return FinSucc<EitherUnsafe<L, B>>(ma.LeftValue);
                var rb = await ma.RightValue.RunIO().ConfigureAwait(false);
                if(rb.IsFail) return FinFail<EitherUnsafe<L, B>>(rb.Error);
                return FinSucc<EitherUnsafe<L, B>>(f(rb.Value));
            }
        }

        public static AffPure<Identity<B>> Traverse<A, B>(this Identity<AffPure<A>> ma, Func<A, B> f)
        {
            return AffMaybe<Identity<B>>(() => Go(ma, f));
            async ValueTask<Fin<Identity<B>>> Go(Identity<AffPure<A>> ma, Func<A, B> f)
            {
                if(ma.IsBottom) return default;
                var rb = await ma.Value.RunIO().ConfigureAwait(false);
                if(rb.IsFail) return FinFail<Identity<B>>(rb.Error);
                return FinSucc<Identity<B>>(new Identity<B>(f(rb.Value)));
            }
        }

        public static AffPure<Option<B>> Traverse<A, B>(this Option<AffPure<A>> ma, Func<A, B> f)
        {
            return AffMaybe<Option<B>>(() => Go(ma, f));
            async ValueTask<Fin<Option<B>>> Go(Option<AffPure<A>> ma, Func<A, B> f)
            {
                if(ma.IsNone) return FinSucc<Option<B>>(None);
                var rb = await ma.Value.RunIO().ConfigureAwait(false);
                if(rb.IsFail) return FinFail<Option<B>>(rb.Error);
                return FinSucc<Option<B>>(Option<B>.Some(f(rb.Value)));
            }
        }
        
        public static AffPure<OptionUnsafe<B>> Traverse<A, B>(this OptionUnsafe<AffPure<A>> ma, Func<A, B> f)
        {
            return AffMaybe<OptionUnsafe<B>>(() => Go(ma, f));
            async ValueTask<Fin<OptionUnsafe<B>>> Go(OptionUnsafe<AffPure<A>> ma, Func<A, B> f)
            {
                if(ma.IsNone) return FinSucc<OptionUnsafe<B>>(None);
                var rb = await ma.Value.RunIO().ConfigureAwait(false);
                if(rb.IsFail) return FinFail<OptionUnsafe<B>>(rb.Error);
                return FinSucc<OptionUnsafe<B>>(OptionUnsafe<B>.Some(f(rb.Value)));
            }
        }
        
        public static AffPure<Try<B>> Traverse<A, B>(this Try<AffPure<A>> ma, Func<A, B> f)
        {
            return AffMaybe<Try<B>>(() => Go(ma, f));
            async ValueTask<Fin<Try<B>>> Go(Try<AffPure<A>> ma, Func<A, B> f)
            {
                var ra = ma.Try();
                if (ra.IsFaulted) return FinSucc<Try<B>>(TryFail<B>(ra.Exception));
                var rb = await ra.Value.RunIO().ConfigureAwait(false);
                if (rb.IsFail) return FinFail<Try<B>>(rb.Error);
                return FinSucc<Try<B>>(Try<B>(f(rb.Value)));
            }
        }
        
        public static AffPure<TryOption<B>> Traverse<A, B>(this TryOption<AffPure<A>> ma, Func<A, B> f)
        {
            return AffMaybe<TryOption<B>>(() => Go(ma, f));
            async ValueTask<Fin<TryOption<B>>> Go(TryOption<AffPure<A>> ma, Func<A, B> f)
            {
                var ra = ma.Try();
                if (ra.IsBottom) return default;
                if (ra.IsNone) return FinSucc<TryOption<B>>(TryOptional<B>(None));
                if (ra.IsFaulted) return FinSucc<TryOption<B>>(TryOptionFail<B>(ra.Exception));
                var rb = await ra.Value.Value.RunIO().ConfigureAwait(false);
                if (rb.IsFail) return FinFail<TryOption<B>>(rb.Error);
                return FinSucc<TryOption<B>>(TryOption<B>(f(rb.Value)));
            }
        }
        
        public static AffPure<Validation<Fail, B>> Traverse<Fail, A, B>(this Validation<Fail, AffPure<A>> ma, Func<A, B> f)
        {
            return AffMaybe<Validation<Fail, B>>(() => Go(ma, f));
            async ValueTask<Fin<Validation<Fail, B>>> Go(Validation<Fail, AffPure<A>> ma, Func<A, B> f)
            {
                if (ma.IsFail) return FinSucc<Validation<Fail, B>>(Fail<Fail, B>(ma.FailValue));
                var rb = await ma.SuccessValue.RunIO().ConfigureAwait(false);
                if(rb.IsFail) return FinFail<Validation<Fail, B>>(rb.Error);
                return FinSucc<Validation<Fail, B>>(f(rb.Value));
            }
        }
        
        public static AffPure<Validation<MonoidFail, Fail, B>> Traverse<MonoidFail, Fail, A, B>(this Validation<MonoidFail, Fail, AffPure<A>> ma, Func<A, B> f)
            where MonoidFail : struct, Monoid<Fail>, Eq<Fail>
        {
            return AffMaybe<Validation<MonoidFail, Fail, B>>(() => Go(ma, f));
            async ValueTask<Fin<Validation<MonoidFail, Fail, B>>> Go(Validation<MonoidFail, Fail, AffPure<A>> ma, Func<A, B> f)
            {
                if (ma.IsFail) return FinSucc<Validation<MonoidFail, Fail, B>>(Fail<MonoidFail, Fail, B>(ma.FailValue));
                var rb = await ma.SuccessValue.RunIO().ConfigureAwait(false);
                if(rb.IsFail) return FinFail<Validation<MonoidFail, Fail, B>>(rb.Error);
                return FinSucc<Validation<MonoidFail, Fail, B>>(f(rb.Value));
            }
        }
        
        public static AffPure<EffPure<B>> Traverse<A, B>(this EffPure<AffPure<A>> ma, Func<A, B> f)
        {
            return AffMaybe<EffPure<B>>(() => Go(ma, f));
            async ValueTask<Fin<EffPure<B>>> Go(EffPure<AffPure<A>> ma, Func<A, B> f)
            {
                var ra = ma.RunIO();
                if (ra.IsFail) return FinSucc<EffPure<B>>(FailEff<B>(ra.Error));
                var rb = await ra.Value.RunIO().ConfigureAwait(false);
                if (rb.IsFail) return FinFail<EffPure<B>>(rb.Error);
                return FinSucc<EffPure<B>>(SuccessEff<B>(f(rb.Value)));
            }
        }
    }
}
