#nullable enable
using System;
using LanguageExt;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using LanguageExt.DataTypes.Serialisation;
using LanguageExt.TypeClasses;
using LanguageExt.ClassInstances;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    public static partial class OptionAsyncT
    {
        //
        // Collections
        //

        public static OptionAsync<Arr<B>> Traverse<A, B>(this Arr<OptionAsync<A>> ma, Func<A, B> f)
        {
            return new OptionAsync<Arr<B>>(Go(ma, f));
            async Task<(bool, Arr<B>)> Go(Arr<OptionAsync<A>> ma, Func<A, B> f)
            {
                var rb = await Task.WhenAll(ma.Map(a => a.Map(f).Data)).ConfigureAwait(false);
                return rb.Exists(d => !d.IsSome)
                     ? (false, default)
                     : (true, new Arr<B>(rb.Map(d => d.Value)));
            }
        }

        public static OptionAsync<HashSet<B>> Traverse<A, B>(this HashSet<OptionAsync<A>> ma, Func<A, B> f)
        {
            return new OptionAsync<HashSet<B>>(Go(ma, f));
            async Task<(bool, HashSet<B>)> Go(HashSet<OptionAsync<A>> ma, Func<A, B> f)
            {
                var rb = await Task.WhenAll(ma.Map(a => a.Map(f).Data)).ConfigureAwait(false);
                return rb.Exists(d => !d.IsSome)
                    ? (false, default)
                    : (true, new HashSet<B>(rb.Map(d => d.Value)));
            }
        }
        
        [Obsolete("use TraverseSerial or TraverseParallel instead")]
        public static OptionAsync<IEnumerable<B>> Traverse<A, B>(this IEnumerable<OptionAsync<A>> ma, Func<A, B> f) =>
            TraverseParallel(ma, f);
        
        public static OptionAsync<IEnumerable<B>> TraverseSerial<A, B>(this IEnumerable<OptionAsync<A>> ma, Func<A, B> f)
        {
            return new OptionAsync<IEnumerable<B>>(Go(ma, f));
            async Task<(bool, IEnumerable<B>)> Go(IEnumerable<OptionAsync<A>> ma, Func<A, B> f)
            {
                var rb = new List<B>();
                foreach (var a in ma)
                {
                    var (isSome, b) = await a.Data;
                    if (!isSome) return (false, System.Array.Empty<B>());
                    rb.Add(f(b));
                }
                return (true, rb);
            };
        }

        public static OptionAsync<IEnumerable<B>> TraverseParallel<A, B>(this IEnumerable<OptionAsync<A>> ma, Func<A, B> f) =>
            TraverseParallel(ma, SysInfo.DefaultAsyncSequenceParallelism, f);
 
        public static OptionAsync<IEnumerable<B>> TraverseParallel<A, B>(this IEnumerable<OptionAsync<A>> ma, int windowSize, Func<A, B> f)
        {
            return new OptionAsync<IEnumerable<B>>(Go(ma, f));
            async Task<(bool, IEnumerable<B>)> Go(IEnumerable<OptionAsync<A>> ma, Func<A, B> f)
            {
                var rb = await ma.Map(a => a.Map(f).Data).WindowMap(windowSize, identity).ConfigureAwait(false);
                return rb.Exists(d => !d.IsSome)
                    ? (false, System.Array.Empty<B>())
                    : (true, rb.Map(d => d.Value));
            }
        }
        
               
        [Obsolete("use SequenceSerial or SequenceParallel instead")]
        public static OptionAsync<IEnumerable<A>> Sequence<A>(this IEnumerable<OptionAsync<A>> ma) =>
            TraverseParallel(ma, Prelude.identity);
 
        public static OptionAsync<IEnumerable<A>> SequenceSerial<A>(this IEnumerable<OptionAsync<A>> ma) =>
            TraverseSerial(ma, Prelude.identity);
 
        public static OptionAsync<IEnumerable<A>> SequenceParallel<A>(this IEnumerable<OptionAsync<A>> ma) =>
            TraverseParallel(ma, Prelude.identity);

        public static OptionAsync<IEnumerable<A>> SequenceParallel<A>(this IEnumerable<OptionAsync<A>> ma, int windowSize) =>
            TraverseParallel(ma, windowSize, Prelude.identity);        

        public static OptionAsync<Lst<B>> Traverse<A, B>(this Lst<OptionAsync<A>> ma, Func<A, B> f)
        {
            return new OptionAsync<Lst<B>>(Go(ma, f));
            async Task<(bool, Lst<B>)> Go(Lst<OptionAsync<A>> ma, Func<A, B> f)
            {
                var rb = await Task.WhenAll(ma.Map(a => a.Map(f).Data)).ConfigureAwait(false);
                return rb.Exists(d => !d.IsSome)
                    ? (false, default)
                    : (true, new Lst<B>(rb.Map(d => d.Value)));
            }
        }

        public static OptionAsync<Que<B>> Traverse<A, B>(this Que<OptionAsync<A>> ma, Func<A, B> f)
        {
            return new OptionAsync<Que<B>>(Go(ma, f));
            async Task<(bool, Que<B>)> Go(Que<OptionAsync<A>> ma, Func<A, B> f)
            {
                var rb = await Task.WhenAll(ma.Map(a => a.Map(f).Data)).ConfigureAwait(false);
                return rb.Exists(d => !d.IsSome)
                    ? (false, default)
                    : (true, new Que<B>(rb.Map(d => d.Value)));
            }
        }
        
        [Obsolete("use TraverseSerial or TraverseParallel instead")]
        public static OptionAsync<Seq<B>> Traverse<A, B>(this Seq<OptionAsync<A>> ma, Func<A, B> f) =>
            TraverseParallel(ma, f);
        
        public static OptionAsync<Seq<B>> TraverseSerial<A, B>(this Seq<OptionAsync<A>> ma, Func<A, B> f)
        {
            return new OptionAsync<Seq<B>>(Go(ma, f));
            async Task<(bool, Seq<B>)> Go(Seq<OptionAsync<A>> ma, Func<A, B> f)
            {
                var rb = new B[ma.Count];
                var ix = 0;
                foreach (var a in ma)
                {
                    var (isSome, b) = await a.Data;
                    if (!isSome) return (false, default);
                    rb[ix] = f(b);
                    ix++;
                }
                return (true, Seq.FromArray<B>(rb));
            };
        }

        public static OptionAsync<Seq<B>> TraverseParallel<A, B>(this Seq<OptionAsync<A>> ma, Func<A, B> f) =>
            TraverseParallel(ma, SysInfo.DefaultAsyncSequenceParallelism, f);
 
        public static OptionAsync<Seq<B>> TraverseParallel<A, B>(this Seq<OptionAsync<A>> ma, int windowSize, Func<A, B> f)
        {
            return new OptionAsync<Seq<B>>(Go(ma, f));
            async Task<(bool, Seq<B>)> Go(Seq<OptionAsync<A>> ma, Func<A, B> f)
            {
                var rb = await ma.Map(a => a.Map(f).Data).WindowMap(windowSize, Prelude.identity).ConfigureAwait(false);
                return rb.Exists(d => !d.IsSome)
                    ? (false, default)
                    : (true, Seq.FromArray<B>(rb.Map(d => d.Value).ToArray()));
            }
        }
               
        [Obsolete("use SequenceSerial or SequenceParallel instead")]
        public static OptionAsync<Seq<A>> Sequence<A>(this Seq<OptionAsync<A>> ma) =>
            TraverseParallel(ma, Prelude.identity);
 
        public static OptionAsync<Seq<A>> SequenceSerial<A>(this Seq<OptionAsync<A>> ma) =>
            TraverseSerial(ma, Prelude.identity);
 
        public static OptionAsync<Seq<A>> SequenceParallel<A>(this Seq<OptionAsync<A>> ma) =>
            TraverseParallel(ma, Prelude.identity);

        public static OptionAsync<Seq<A>> SequenceParallel<A>(this Seq<OptionAsync<A>> ma, int windowSize) =>
            TraverseParallel(ma, windowSize, Prelude.identity);        
        
        public static OptionAsync<Set<B>> Traverse<A, B>(this Set<OptionAsync<A>> ma, Func<A, B> f)
        {
            return new OptionAsync<Set<B>>(Go(ma, f));
            async Task<(bool, Set<B>)> Go(Set<OptionAsync<A>> ma, Func<A, B> f)
            {
                var rb = await Task.WhenAll(ma.Map(a => a.Map(f).Data)).ConfigureAwait(false);
                return rb.Exists(d => !d.IsSome)
                    ? (false, default)
                    : (true, new Set<B>(rb.Map(d => d.Value)));
            }
        }

        public static OptionAsync<Stck<B>> Traverse<A, B>(this Stck<OptionAsync<A>> ma, Func<A, B> f)
        {
            return new OptionAsync<Stck<B>>(Go(ma, f));
            async Task<(bool, Stck<B>)> Go(Stck<OptionAsync<A>> ma, Func<A, B> f)
            {
                var rb = await Task.WhenAll(ma.Reverse().Map(a => a.Map(f).Data)).ConfigureAwait(false);
                return rb.Exists(d => !d.IsSome)
                    ? (false, default)
                    : (true, new Stck<B>(rb.Map(d => d.Value)));
            }
        }
        
        //
        // Async types
        //

        public static OptionAsync<EitherAsync<L, B>> Traverse<L, A, B>(this EitherAsync<L, OptionAsync<A>> ma, Func<A, B> f)
        {
            return new OptionAsync<EitherAsync<L, B>>(Go(ma, f));
            async Task<(bool, EitherAsync<L, B>)> Go(EitherAsync<L, OptionAsync<A>> ma, Func<A, B> f)
            {
                var da = await ma.Data.ConfigureAwait(false);
                if (da.State == EitherStatus.IsBottom) return (false, default);
                if (da.State == EitherStatus.IsLeft) return (true, EitherAsync<L, B>.Left(da.Left));
                var (isSome, value) = await da.Right.Data.ConfigureAwait(false);
                if (!isSome) return (false, default);
                return (true, EitherAsync<L, B>.Right(f(value)));
            }
        }

        public static OptionAsync<OptionAsync<B>> Traverse<A, B>(this OptionAsync<OptionAsync<A>> ma, Func<A, B> f)
        {
            return new OptionAsync<OptionAsync<B>>(Go(ma, f));
            async Task<(bool, OptionAsync<B>)> Go(OptionAsync<OptionAsync<A>> ma, Func<A, B> f)
            {
                var (isSomeA, valueA) = await ma.Data.ConfigureAwait(false);
                if (!isSomeA) return (true, OptionAsync<B>.None);
                var (isSomeB, valueB) = await valueA.Data.ConfigureAwait(false);
                if (!isSomeB) return (false, default);
                return (true, OptionAsync<B>.Some(f(valueB)));
            }
        }
        
        public static OptionAsync<TryAsync<B>> Traverse<A, B>(this TryAsync<OptionAsync<A>> ma, Func<A, B> f)
        {
            return new OptionAsync<TryAsync<B>>(Go(ma, f));
            async Task<(bool, TryAsync<B>)> Go(TryAsync<OptionAsync<A>> ma, Func<A, B> f)
            {
                var resultA = await ma.Try().ConfigureAwait(false);
                if (resultA.IsBottom) return (false, TryAsyncFail<B>(BottomException.Default));
                if (resultA.IsFaulted) return (true, TryAsyncFail<B>(resultA.Exception));
                var (isSome, value) = await resultA.Value.Data.ConfigureAwait(false);
                if (!isSome) return (false, TryAsyncFail<B>(BottomException.Default));
                return (true, TryAsync<B>(f(value)));
            }
        }
        
        public static OptionAsync<TryOptionAsync<B>> Traverse<A, B>(this TryOptionAsync<OptionAsync<A>> ma, Func<A, B> f)
        {
            return new OptionAsync<TryOptionAsync<B>>(Go(ma, f));
            async Task<(bool, TryOptionAsync<B>)> Go(TryOptionAsync<OptionAsync<A>> ma, Func<A, B> f)
            {
                var resultA = await ma.Try().ConfigureAwait(false);
                if (resultA.IsBottom) return (false, TryOptionAsyncFail<B>(BottomException.Default));
                if (resultA.IsNone) return (true, TryOptionalAsync<B>(None));
                if (resultA.IsFaulted) return (true, TryOptionAsyncFail<B>(resultA.Exception));
                var (isSome, value) = await resultA.Value.Value.Data.ConfigureAwait(false);
                if (!isSome) return (false, TryOptionAsyncFail<B>(BottomException.Default));
                return (true, TryOptionAsync<B>(f(value)));
            }
        }

        public static OptionAsync<Task<B>> Traverse<A, B>(this Task<OptionAsync<A>> ma, Func<A, B> f)
        {
            return new OptionAsync<Task<B>>(Go(ma, f));
            async Task<(bool, Task<B>)> Go(Task<OptionAsync<A>> ma, Func<A, B> f)
            {
                var result = await ma.ConfigureAwait(false);
                var (isSome, value) = await result.Data.ConfigureAwait(false);
                if (!isSome) return (false, Task.FromException<B>(BottomException.Default));
                return (true, f(value).AsTask());
            }
        }

        public static OptionAsync<ValueTask<B>> Traverse<A, B>(this ValueTask<OptionAsync<A>> ma, Func<A, B> f)
        {
            return new OptionAsync<ValueTask<B>>(Go(ma, f).AsTask());
            async ValueTask<(bool, ValueTask<B>)> Go(ValueTask<OptionAsync<A>> ma, Func<A, B> f)
            {
                var result = await ma.ConfigureAwait(false);
                var (isSome, value) = await result.Data.ConfigureAwait(false);
                if (!isSome) return (false, default);
                return (true, f(value).AsValueTask());
            }
        }
                
        public static OptionAsync<Aff<B>> Traverse<A, B>(this Aff<OptionAsync<A>> ma, Func<A, B> f)
        {
            return new OptionAsync<Aff<B>>(Go(ma, f));
            async Task<(bool, Aff<B>)> Go(Aff<OptionAsync<A>> ma, Func<A, B> f)
            {
                var resultA = await ma.Run().ConfigureAwait(false);
                if (resultA.IsBottom) return (false, default);
                if (resultA.IsFail) return (true, FailAff<B>(resultA.Error));
                var (isSome, value) = await resultA.Value.Data.ConfigureAwait(false);
                if (!isSome) return (false, default);
                return (true, SuccessAff<B>(f(value)));
            }
        }

        //
        // Sync types
        // 
        
        public static OptionAsync<Either<L, B>> Traverse<L, A, B>(this Either<L, OptionAsync<A>> ma, Func<A, B> f)
        {
            return new OptionAsync<Either<L, B>>(Go(ma, f));
            async Task<(bool, Either<L, B>)> Go(Either<L, OptionAsync<A>> ma, Func<A, B> f)
            {
                if(ma.IsBottom) return (false, default);
                if(ma.IsLeft) return (true, Left<L, B>(ma.LeftValue));
                var (isSome, value) = await ma.RightValue.Data.ConfigureAwait(false);
                if(!isSome) return (false, default);
                return (true, f(value));
            }
        }

        public static OptionAsync<EitherUnsafe<L, B>> Traverse<L, A, B>(this EitherUnsafe<L, OptionAsync<A>> ma, Func<A, B> f)
        {
            return new OptionAsync<EitherUnsafe<L, B>>(Go(ma, f));
            async Task<(bool, EitherUnsafe<L, B>)> Go(EitherUnsafe<L, OptionAsync<A>> ma, Func<A, B> f)
            {
                if(ma.IsBottom) return (false, default);
                if(ma.IsLeft) return (true, LeftUnsafe<L, B>(ma.LeftValue));
                var (isSome, value) = await ma.RightValue.Data.ConfigureAwait(false);
                if(!isSome) return (false, default);
                return (true, f(value));
            }
        }

        public static OptionAsync<Identity<B>> Traverse<A, B>(this Identity<OptionAsync<A>> ma, Func<A, B> f)
        {
            return new OptionAsync<Identity<B>>(Go(ma, f));
            async Task<(bool, Identity<B>)> Go(Identity<OptionAsync<A>> ma, Func<A, B> f)
            {
                if(ma.IsBottom) return (false, default);
                var (isSome, value) = await ma.Value.Data.ConfigureAwait(false);
                if(!isSome) return (false, default);
                return (true, new Identity<B>(f(value)));
            }
        }

        public static OptionAsync<Fin<B>> Traverse<A, B>(this Fin<OptionAsync<A>> ma, Func<A, B> f)
        {
            return new OptionAsync<Fin<B>>(Go(ma, f));
            async Task<(bool, Fin<B>)> Go(Fin<OptionAsync<A>> ma, Func<A, B> f)
            {
                if(ma.IsFail) return (true, ma.Cast<B>());
                var (isSome, value) = await ma.Value.Data.ConfigureAwait(false);
                if(!isSome) return (false, default);
                return (true, Fin<B>.Succ(f(value)));
            }
        }

        public static OptionAsync<Option<B>> Traverse<A, B>(this Option<OptionAsync<A>> ma, Func<A, B> f)
        {
            return new OptionAsync<Option<B>>(Go(ma, f));
            async Task<(bool, Option<B>)> Go(Option<OptionAsync<A>> ma, Func<A, B> f)
            {
                if(ma.IsNone) return (true, Option<B>.None);
                var (isSome, value) = await ma.Value.Data.ConfigureAwait(false);
                if(!isSome) return (false, default);
                return (true, Option<B>.Some(f(value)));
            }
        }
        
        public static OptionAsync<OptionUnsafe<B>> Traverse<A, B>(this OptionUnsafe<OptionAsync<A>> ma, Func<A, B> f)
        {
            return new OptionAsync<OptionUnsafe<B>>(Go(ma, f));
            async Task<(bool, OptionUnsafe<B>)> Go(OptionUnsafe<OptionAsync<A>> ma, Func<A, B> f)
            {
                if(ma.IsNone) return (true, OptionUnsafe<B>.None);
                var (isSome, value) = await ma.Value.Data.ConfigureAwait(false);
                if(!isSome) return (false, default);
                return (true, OptionUnsafe<B>.Some(f(value)));
            }
        }
        
        public static OptionAsync<Try<B>> Traverse<A, B>(this Try<OptionAsync<A>> ma, Func<A, B> f)
        {
            try
            {
                return new OptionAsync<Try<B>>(Go(ma, f));
                async Task<(bool, Try<B>)> Go(Try<OptionAsync<A>> ma, Func<A, B> f)
                {
                    var ra = ma.Try();
                    if(ra.IsBottom) return (false, TryFail<B>(BottomException.Default));
                    if (ra.IsFaulted) return (true, TryFail<B>(ra.Exception));
                    var (isSome, value) = await ra.Value.Data.ConfigureAwait(false);
                    if(!isSome) return (false, TryFail<B>(BottomException.Default));
                    return (true, Try<B>(f(value)));
                }
            }
            catch (Exception e)
            {
                return Try<B>(e);
            }
        }
        
        public static OptionAsync<TryOption<B>> Traverse<A, B>(this TryOption<OptionAsync<A>> ma, Func<A, B> f)
        {
            try
            {
                return new OptionAsync<TryOption<B>>(Go(ma, f));
                async Task<(bool, TryOption<B>)> Go(TryOption<OptionAsync<A>> ma, Func<A, B> f)
                {
                    var ra = ma.Try();
                    if (ra.IsBottom) return (false, TryOptionFail<B>(BottomException.Default));
                    if (ra.IsNone) return (true, TryOptional<B>(None));
                    if (ra.IsFaulted) return (true, TryOptionFail<B>(ra.Exception));
                    var (isSome, value) = await ra.Value.Value.Data.ConfigureAwait(false);
                    if (!isSome) return (false, TryOptionFail<B>(BottomException.Default));
                    return (true, TryOption<B>(f(value)));
                }
            }
            catch (Exception e)
            {
                return TryOption<B>(e);
            }
        }
        
        public static OptionAsync<Validation<Fail, B>> Traverse<Fail, A, B>(this Validation<Fail, OptionAsync<A>> ma, Func<A, B> f)
        {
            return new OptionAsync<Validation<Fail, B>>(Go(ma, f));
            async Task<(bool, Validation<Fail, B>)> Go(Validation<Fail, OptionAsync<A>> ma, Func<A, B> f)
            {
                if(ma.IsFail) return (true, Fail<Fail, B>(ma.FailValue));
                var (isSome, value) = await ma.SuccessValue.Data.ConfigureAwait(false);
                if(!isSome) return (false, default);
                return (true, f(value));
            }
        }
        
        public static OptionAsync<Validation<MonoidFail, Fail, B>> Traverse<MonoidFail, Fail, A, B>(this Validation<MonoidFail, Fail, OptionAsync<A>> ma, Func<A, B> f)
            where MonoidFail : struct, Monoid<Fail>, Eq<Fail>
        {
            return new OptionAsync<Validation<MonoidFail, Fail, B>>(Go(ma, f));
            async Task<(bool, Validation<MonoidFail, Fail, B>)> Go(Validation<MonoidFail, Fail, OptionAsync<A>> ma, Func<A, B> f)
            {
                if(ma.IsFail) return (true, Fail<MonoidFail, Fail, B>(ma.FailValue));
                var (isSome, value) = await ma.SuccessValue.Data.ConfigureAwait(false);
                if(!isSome) return (false, default);
                return (true, f(value));
            }
        }
        
        public static OptionAsync<Eff<B>> Traverse<A, B>(this Eff<OptionAsync<A>> ma, Func<A, B> f)
        {
            try
            {
                return new OptionAsync<Eff<B>>(Go(ma, f));
                async Task<(bool, Eff<B>)> Go(Eff<OptionAsync<A>> ma, Func<A, B> f)
                {
                    var ra = ma.Run();
                    if(ra.IsBottom) return (false, default);
                    if (ra.IsFail) return (true, FailEff<B>(ra.Error));
                    var (isSome, value) = await ra.Value.Data.ConfigureAwait(false);
                    if(!isSome) return (false, default);
                    return (true, SuccessEff<B>(f(value)));
                }
            }
            catch (Exception e)
            {
                return FailEff<B>(e);
            }
        }        
    }
}
