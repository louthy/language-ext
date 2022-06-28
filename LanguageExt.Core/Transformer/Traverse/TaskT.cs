#nullable enable
using System;
using LanguageExt;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using LanguageExt.Common;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    public static partial class TaskT
    {
        //
        // Collections
        //
 
        public static async Task<Arr<B>> Traverse<A, B>(this Arr<Task<A>> ma, Func<A, B> f)
        {
            var rb = await Task.WhenAll(ma.Map(async a => f(await a))).ConfigureAwait(false);
            return new Arr<B>(rb);
        }
        
        public static async Task<HashSet<B>> Traverse<A, B>(this HashSet<Task<A>> ma, Func<A, B> f)
        {
            var rb = await Task.WhenAll(ma.Map(async a => f(await a))).ConfigureAwait(false);
            return new HashSet<B>(rb);
        }
        
        [Obsolete("use TraverseSerial or TraverseParallel instead")]
        public static Task<IEnumerable<B>> Traverse<A, B>(this IEnumerable<Task<A>> ma, Func<A, B> f) =>
            TraverseParallel(ma, f);

        public static async Task<IEnumerable<B>> TraverseSerial<A, B>(this IEnumerable<Task<A>> ma, Func<A, B> f)
        {
            var rb = new List<B>();
            foreach (var a in ma)
            {
                rb.Add(f(await a.ConfigureAwait(false)));
            }
            return rb;            
        }

        public static Task<IEnumerable<B>> TraverseParallel<A, B>(this IEnumerable<Task<A>> ma, int windowSize, Func<A, B> f) =>
            ma.WindowMap(windowSize, f).Map(xs => (IEnumerable<B>)xs);

        public static Task<IEnumerable<B>> TraverseParallel<A, B>(this IEnumerable<Task<A>> ma, Func<A, B> f) =>
            ma.WindowMap(f).Map(xs => (IEnumerable<B>)xs);
                      
        [Obsolete("use TraverseSerial or TraverseParallel instead")]
        public static Task<IEnumerable<A>> Sequence<A>(this IEnumerable<Task<A>> ma) =>
            TraverseParallel(ma, Prelude.identity);
 
        public static Task<IEnumerable<A>> SequenceSerial<A>(this IEnumerable<Task<A>> ma) =>
            TraverseSerial(ma, Prelude.identity);
 
        public static Task<IEnumerable<A>> SequenceParallel<A>(this IEnumerable<Task<A>> ma) =>
            TraverseParallel(ma, Prelude.identity);

        public static Task<IEnumerable<A>> SequenceParallel<A>(this IEnumerable<Task<A>> ma, int windowSize) =>
            TraverseParallel(ma, windowSize, Prelude.identity);
 
        public static async Task<Lst<B>> Traverse<A, B>(this Lst<Task<A>> ma, Func<A, B> f)
        {
            var rb = await Task.WhenAll(ma.Map(async a => f(await a))).ConfigureAwait(false);
            return new Lst<B>(rb);
        }

        public static Task<Lst<A>> Sequence<A>(this Lst<Task<A>> ma) =>
            ma.Traverse(identity);
 
        public static async Task<Que<B>> Traverse<A, B>(this Que<Task<A>> ma, Func<A, B> f)
        {
            var rb = await Task.WhenAll(ma.Map(async a => f(await a))).ConfigureAwait(false);
            return new Que<B>(rb);
        }

        public static Task<Que<A>> Sequence<A>(this Que<Task<A>> ma) =>
            ma.Traverse(identity);

        [Obsolete("use TraverseSerial or TraverseParallel instead")]
        public static Task<Seq<B>> Traverse<A, B>(this Seq<Task<A>> ma, Func<A, B> f) =>
            TraverseParallel(ma, f);
 
        public static async Task<Seq<B>> TraverseSerial<A, B>(this Seq<Task<A>> ma, Func<A, B> f)
        {
            var rb = new List<B>();
            foreach (var a in ma)
            {
                rb.Add(f(await a.ConfigureAwait(false)));
            }
            return Seq.FromArray<B>(rb.ToArray());
        }
        
        public static Task<Seq<B>> TraverseParallel<A, B>(this Seq<Task<A>> ma, int windowSize, Func<A, B> f) =>
            ma.WindowMap(windowSize, f).Map(toSeq);        

        public static Task<Seq<B>> TraverseParallel<A, B>(this Seq<Task<A>> ma, Func<A, B> f) =>
            ma.WindowMap(f).Map(toSeq);
        
        [Obsolete("use TraverseSerial or TraverseParallel instead")]
        public static Task<Seq<A>> Sequence<A>(this Seq<Task<A>> ma) =>
            TraverseParallel(ma, Prelude.identity);
 
        public static Task<Seq<A>> SequenceSerial<A>(this Seq<Task<A>> ma) =>
            TraverseSerial(ma, Prelude.identity);
 
        public static Task<Seq<A>> SequenceParallel<A>(this Seq<Task<A>> ma) =>
            TraverseParallel(ma, Prelude.identity);

        public static Task<Seq<A>> SequenceParallel<A>(this Seq<Task<A>> ma, int windowSize) =>
            TraverseParallel(ma, windowSize, Prelude.identity);

        public static async Task<Set<B>> Traverse<A, B>(this Set<Task<A>> ma, Func<A, B> f)
        {
            var rb = await Task.WhenAll(ma.Map(async a => f(await a))).ConfigureAwait(false);
            return new Set<B>(rb);
        }

        public static async Task<Stck<B>> Traverse<A, B>(this Stck<Task<A>> ma, Func<A, B> f)
        {
            var rb = await Task.WhenAll(ma.Reverse().Map(async a => f(await a))).ConfigureAwait(false);
            return new Stck<B>(rb);
        }
        
        public static Task<Stck<A>> Sequence<A>(this Stck<Task<A>> ma) =>
            ma.Traverse(identity);

        //
        // Async types
        //

        public static async Task<EitherAsync<L, B>> Traverse<L, A, B>(this EitherAsync<L, Task<A>> ma, Func<A, B> f)
        {
            var da = await ma.Data.ConfigureAwait(false);
            if (da.State == EitherStatus.IsBottom) throw new BottomException();
            if (da.State == EitherStatus.IsLeft) return EitherAsync<L, B>.Left(da.Left);
            var a = await da.Right.ConfigureAwait(false);
            return EitherAsync<L, B>.Right(f(a));
        }

        public static async Task<OptionAsync<B>> Traverse<A, B>(this OptionAsync<Task<A>> ma, Func<A, B> f)
        {
            var (s, v) = await ma.Data.ConfigureAwait(false);
            if (!s) return OptionAsync<B>.None;
            var a = await v.ConfigureAwait(false);
            return OptionAsync<B>.Some(f(a));
        }
        
        public static async Task<TryAsync<B>> Traverse<A, B>(this TryAsync<Task<A>> ma, Func<A, B> f)
        {
            var da = await ma.Try().ConfigureAwait(false);
            if (da.IsBottom) throw new BottomException();
            if (da.IsFaulted) return TryAsyncFail<B>(da.Exception);
            var a = await da.Value.ConfigureAwait(false);
            return TryAsyncSucc(f(a));
        }
        
        public static async Task<TryOptionAsync<B>> Traverse<A, B>(this TryOptionAsync<Task<A>> ma, Func<A, B> f)
        {
            var da = await ma.Try().ConfigureAwait(false);
            if (da.IsBottom) throw new BottomException();
            if (da.IsNone) return TryOptionalAsync<B>(None);
            if (da.IsFaulted) return TryOptionAsyncFail<B>(da.Exception);
            #nullable disable
            var a = await da.Value.Value.ConfigureAwait(false);
            #nullable enable
            return TryOptionAsyncSucc(f(a));
        }

        public static async Task<Task<B>> Traverse<A, B>(this Task<Task<A>> ma, Func<A, B> f)
        {
            var da = await ma.ConfigureAwait(false);
            var a = await da.ConfigureAwait(false);
            return TaskSucc(f(a));
        }

        public static async Task<ValueTask<B>> Traverse<A, B>(this ValueTask<Task<A>> ma, Func<A, B> f)
        {
            var da = await ma.ConfigureAwait(false);
            var a = await da.ConfigureAwait(false);
            return ValueTaskSucc(f(a));
        }
        
        public static async Task<Aff<B>> Traverse<A, B>(this Aff<Task<A>> ma, Func<A, B> f)
        {
            var da = await ma.Run().ConfigureAwait(false);
            if (da.IsBottom) throw new BottomException();
            if (da.IsFail) return FailAff<B>(da.Error);
            var a = await da.Value.ConfigureAwait(false);
            return SuccessAff(f(a));
        }


        //
        // Sync types
        // 
        
        public static async Task<Either<L, B>> Traverse<L, A, B>(this Either<L, Task<A>> ma, Func<A, B> f)
        {
            if (ma.IsBottom) return Either<L, B>.Bottom;
            else if (ma.IsLeft) return Either<L, B>.Left(ma.LeftValue);
            return Either<L, B>.Right(f(await ma.RightValue.ConfigureAwait(false)));
        }

        public static async Task<EitherUnsafe<L, B>> Traverse<L, A, B>(this EitherUnsafe<L, Task<A>> ma, Func<A, B> f)
        {
            if (ma.IsBottom) return EitherUnsafe<L, B>.Bottom;
            else if (ma.IsLeft) return EitherUnsafe<L, B>.Left(ma.LeftValue);
            return EitherUnsafe<L, B>.Right(f(await ma.RightValue.ConfigureAwait(false)));
        }

        public static async Task<Identity<B>> Traverse<A, B>(this Identity<Task<A>> ma, Func<A, B> f) =>
            new Identity<B>(f(await ma.Value.ConfigureAwait(false)));
        
        public static async Task<Fin<B>> Traverse<A, B>(this Fin<Task<A>> ma, Func<A, B> f)
        {
            if (ma.IsFail) return ma.Cast<B>();
            return Fin<B>.Succ(f(await ma.Value.ConfigureAwait(false)));
        }
        
        public static async Task<Option<B>> Traverse<A, B>(this Option<Task<A>> ma, Func<A, B> f)
        {
            if (ma.IsNone) return Option<B>.None;
            #nullable disable
            return Option<B>.Some(f(await ma.Value.ConfigureAwait(false)));
            #nullable enable
        }
        
        public static async Task<OptionUnsafe<B>> Traverse<A, B>(this OptionUnsafe<Task<A>> ma, Func<A, B> f)
        {
            if (ma.IsNone) return OptionUnsafe<B>.None;
            return OptionUnsafe<B>.Some(f(await ma.Value.ConfigureAwait(false)));
        }
        
        public static async Task<Try<B>> Traverse<A, B>(this Try<Task<A>> ma, Func<A, B> f)
        {
            var mr = ma.Try();
            if (mr.IsBottom) return Try<B>(BottomException.Default);
            else if (mr.IsFaulted) return Try<B>(mr.Exception);
            return Try<B>(f(await mr.Value.ConfigureAwait(false)));
        }
        
        public static async Task<TryOption<B>> Traverse<A, B>(this TryOption<Task<A>> ma, Func<A, B> f)
        {
            var mr = ma.Try();
            if (mr.IsBottom) return TryOptionFail<B>(BottomException.Default);
            else if (mr.IsNone) return TryOption<B>(None);
            else if (mr.IsFaulted) return TryOption<B>(mr.Exception);
            #nullable disable
            return TryOption<B>(f(await mr.Value.Value.ConfigureAwait(false)));
            #nullable enable
        }
        
        public static async Task<Validation<Fail, B>> Traverse<Fail, A, B>(this Validation<Fail, Task<A>> ma, Func<A, B> f)
        {
            if (ma.IsFail) return Validation<Fail, B>.Fail(ma.FailValue);
            return Validation<Fail, B>.Success(f(await ma.SuccessValue.ConfigureAwait(false)));
        }
        
        public static async Task<Validation<MonoidFail, Fail, B>> Traverse<MonoidFail, Fail, A, B>(this Validation<MonoidFail, Fail, Task<A>> ma, Func<A, B> f)
            where MonoidFail : struct, Monoid<Fail>, Eq<Fail>
        {
            if (ma.IsFail) return Validation<MonoidFail, Fail, B>.Fail(ma.FailValue);
            return Validation<MonoidFail, Fail, B>.Success(f(await ma.SuccessValue.ConfigureAwait(false)));
        }
        
        public static async Task<Eff<B>> Traverse<A, B>(this Eff<Task<A>> ma, Func<A, B> f)
        {
            var mr = ma.Run();
            if (mr.IsBottom) return FailEff<B>(BottomException.Default);
            else if (mr.IsFail) return FailEff<B>(mr.Error);
            return SuccessEff<B>(f(await mr.Value.ConfigureAwait(false)));
        }
    }
}
