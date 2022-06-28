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
    public static partial class ValueTaskT
    {
        //
        // Collections
        //
 
        public static async ValueTask<Arr<B>> Traverse<A, B>(this Arr<ValueTask<A>> ma, Func<A, B> f)
        {
            var rb = await Task.WhenAll(ma.Map(async a => f(await a.ConfigureAwait(false)))).ConfigureAwait(false);
            return new Arr<B>(rb);
        }
        
        public static async ValueTask<HashSet<B>> Traverse<A, B>(this HashSet<ValueTask<A>> ma, Func<A, B> f)
        {
            var rb = await Task.WhenAll(ma.Map(async a => f(await a.ConfigureAwait(false)))).ConfigureAwait(false);
            return new HashSet<B>(rb);
        }
        
        [Obsolete("use TraverseSerial or TraverseParallel instead")]
        public static ValueTask<IEnumerable<B>> Traverse<A, B>(this IEnumerable<ValueTask<A>> ma, Func<A, B> f) =>
            TraverseParallel(ma, f);

        public static async ValueTask<IEnumerable<B>> TraverseSerial<A, B>(this IEnumerable<ValueTask<A>> ma, Func<A, B> f)
        {
            var rb = new List<B>();
            foreach (var a in ma)
            {
                rb.Add(f(await a.ConfigureAwait(false)));
            }
            return rb;            
        }

        public static ValueTask<IEnumerable<B>> TraverseParallel<A, B>(this IEnumerable<ValueTask<A>> ma, int windowSize, Func<A, B> f) =>
            ma.WindowMap(windowSize, f).Map(xs => (IEnumerable<B>)xs);

        public static ValueTask<IEnumerable<B>> TraverseParallel<A, B>(this IEnumerable<ValueTask<A>> ma, Func<A, B> f) =>
            ma.WindowMap(f).Map(xs => (IEnumerable<B>)xs);
                      
        [Obsolete("use TraverseSerial or TraverseParallel instead")]
        public static ValueTask<IEnumerable<A>> Sequence<A>(this IEnumerable<ValueTask<A>> ma) =>
            TraverseParallel(ma, Prelude.identity);
 
        public static ValueTask<IEnumerable<A>> SequenceSerial<A>(this IEnumerable<ValueTask<A>> ma) =>
            TraverseSerial(ma, Prelude.identity);
 
        public static ValueTask<IEnumerable<A>> SequenceParallel<A>(this IEnumerable<ValueTask<A>> ma) =>
            TraverseParallel(ma, Prelude.identity);

        public static ValueTask<IEnumerable<A>> SequenceParallel<A>(this IEnumerable<ValueTask<A>> ma, int windowSize) =>
            TraverseParallel(ma, windowSize, Prelude.identity);
 
        public static async ValueTask<Lst<B>> Traverse<A, B>(this Lst<ValueTask<A>> ma, Func<A, B> f)
        {
            var rb = await Task.WhenAll(ma.Map(async a => f(await a.ConfigureAwait(false)))).ConfigureAwait(false);
            return new Lst<B>(rb);
        }
        
        public static async ValueTask<Que<B>> Traverse<A, B>(this Que<ValueTask<A>> ma, Func<A, B> f)
        {
            var rb = await Task.WhenAll(ma.Map(async a => f(await a.ConfigureAwait(false)))).ConfigureAwait(false);
            return new Que<B>(rb);
        }

        [Obsolete("use TraverseSerial or TraverseParallel instead")]
        public static ValueTask<Seq<B>> Traverse<A, B>(this Seq<ValueTask<A>> ma, Func<A, B> f) =>
            TraverseParallel(ma, f);
 
        public static async ValueTask<Seq<B>> TraverseSerial<A, B>(this Seq<ValueTask<A>> ma, Func<A, B> f)
        {
            var rb = new List<B>();
            foreach (var a in ma)
            {
                rb.Add(f(await a.ConfigureAwait(false)));
            }
            return Seq.FromArray<B>(rb.ToArray());
        }
        
        public static ValueTask<Seq<B>> TraverseParallel<A, B>(this Seq<ValueTask<A>> ma, int windowSize, Func<A, B> f) =>
            ma.WindowMap(windowSize, f).Map(toSeq);        

        public static ValueTask<Seq<B>> TraverseParallel<A, B>(this Seq<ValueTask<A>> ma, Func<A, B> f) =>
            ma.WindowMap(f).Map(toSeq);
        
        [Obsolete("use TraverseSerial or TraverseParallel instead")]
        public static ValueTask<Seq<A>> Sequence<A>(this Seq<ValueTask<A>> ma) =>
            TraverseParallel(ma, Prelude.identity);
 
        public static ValueTask<Seq<A>> SequenceSerial<A>(this Seq<ValueTask<A>> ma) =>
            TraverseSerial(ma, Prelude.identity);
 
        public static ValueTask<Seq<A>> SequenceParallel<A>(this Seq<ValueTask<A>> ma) =>
            TraverseParallel(ma, Prelude.identity);

        public static ValueTask<Seq<A>> SequenceParallel<A>(this Seq<ValueTask<A>> ma, int windowSize) =>
            TraverseParallel(ma, windowSize, Prelude.identity);

        public static async ValueTask<Set<B>> Traverse<A, B>(this Set<ValueTask<A>> ma, Func<A, B> f)
        {
            var rb = await Task.WhenAll(ma.Map(async a => f(await a.ConfigureAwait(false)))).ConfigureAwait(false);
            return new Set<B>(rb);
        }

        public static async ValueTask<Stck<B>> Traverse<A, B>(this Stck<ValueTask<A>> ma, Func<A, B> f)
        {
            var rb = await Task.WhenAll(ma.Reverse().Map(async a => f(await a.ConfigureAwait(false)))).ConfigureAwait(false);
            return new Stck<B>(rb);
        }

        //
        // Async types
        //

        public static async ValueTask<EitherAsync<L, B>> Traverse<L, A, B>(this EitherAsync<L, ValueTask<A>> ma, Func<A, B> f)
        {
            var da = await ma.Data.ConfigureAwait(false);
            if (da.State == EitherStatus.IsBottom) throw new BottomException();
            if (da.State == EitherStatus.IsLeft) return EitherAsync<L, B>.Left(da.Left);
            var a = await da.Right.ConfigureAwait(false);
            return EitherAsync<L, B>.Right(f(a));
        }

        public static async ValueTask<OptionAsync<B>> Traverse<A, B>(this OptionAsync<ValueTask<A>> ma, Func<A, B> f)
        {
            var (s, v) = await ma.Data.ConfigureAwait(false);
            if (!s) return OptionAsync<B>.None;
            var a = await v.ConfigureAwait(false);
            return OptionAsync<B>.Some(f(a));
        }
        
        public static async ValueTask<TryAsync<B>> Traverse<A, B>(this TryAsync<ValueTask<A>> ma, Func<A, B> f)
        {
            var da = await ma.Try().ConfigureAwait(false);
            if (da.IsBottom) throw new BottomException();
            if (da.IsFaulted) return TryAsyncFail<B>(da.Exception);
            var a = await da.Value.ConfigureAwait(false);
            return TryAsyncSucc(f(a));
        }
        
        public static async ValueTask<TryOptionAsync<B>> Traverse<A, B>(this TryOptionAsync<ValueTask<A>> ma, Func<A, B> f)
        {
            var da = await ma.Try().ConfigureAwait(false);
            if (da.IsBottom) throw new BottomException();
            if (da.IsNone) return TryOptionalAsync<B>(None);
            if (da.IsFaulted) return TryOptionAsyncFail<B>(da.Exception);
            var a = await da.Value.Value.ConfigureAwait(false);
            return TryOptionAsyncSucc(f(a));
        }

        public static async ValueTask<Task<B>> Traverse<A, B>(this Task<ValueTask<A>> ma, Func<A, B> f)
        {
            var da = await ma.ConfigureAwait(false);
            var a = await da.ConfigureAwait(false);
            return f(a).AsTask();
        }

        public static async ValueTask<ValueTask<B>> Traverse<A, B>(this ValueTask<ValueTask<A>> ma, Func<A, B> f)
        {
            var da = await ma.ConfigureAwait(false);
            var a = await da.ConfigureAwait(false);
            return ValueTaskSucc(f(a));
        }
                
        public static async ValueTask<Aff<B>> Traverse<A, B>(this Aff<ValueTask<A>> ma, Func<A, B> f)
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
        
        public static async ValueTask<Either<L, B>> Traverse<L, A, B>(this Either<L, ValueTask<A>> ma, Func<A, B> f)
        {
            if (ma.IsBottom) return Either<L, B>.Bottom;
            else if (ma.IsLeft) return Either<L, B>.Left(ma.LeftValue);
            return Either<L, B>.Right(f(await ma.RightValue.ConfigureAwait(false)));
        }

        public static async ValueTask<EitherUnsafe<L, B>> Traverse<L, A, B>(this EitherUnsafe<L, ValueTask<A>> ma, Func<A, B> f)
        {
            if (ma.IsBottom) return EitherUnsafe<L, B>.Bottom;
            else if (ma.IsLeft) return EitherUnsafe<L, B>.Left(ma.LeftValue);
            return EitherUnsafe<L, B>.Right(f(await ma.RightValue.ConfigureAwait(false)));
        }

        public static async ValueTask<Identity<B>> Traverse<A, B>(this Identity<ValueTask<A>> ma, Func<A, B> f) =>
            new Identity<B>(f(await ma.Value.ConfigureAwait(false)));
        
        public static async ValueTask<Fin<B>> Traverse<A, B>(this Fin<ValueTask<A>> ma, Func<A, B> f)
        {
            if (ma.IsFail) return ma.Cast<B>();
            return Fin<B>.Succ(f(await ma.Value.ConfigureAwait(false)));
        }
        
        public static async ValueTask<Option<B>> Traverse<A, B>(this Option<ValueTask<A>> ma, Func<A, B> f)
        {
            if (ma.IsNone) return Option<B>.None;
            return Option<B>.Some(f(await ma.Value.ConfigureAwait(false)));
        }
        
        public static async ValueTask<OptionUnsafe<B>> Traverse<A, B>(this OptionUnsafe<ValueTask<A>> ma, Func<A, B> f)
        {
            if (ma.IsNone) return OptionUnsafe<B>.None;
            return OptionUnsafe<B>.Some(f(await ma.Value.ConfigureAwait(false)));
        }
        
        public static async ValueTask<Try<B>> Traverse<A, B>(this Try<ValueTask<A>> ma, Func<A, B> f)
        {
            var mr = ma.Try();
            if (mr.IsBottom) return Try<B>(BottomException.Default);
            else if (mr.IsFaulted) return Try<B>(mr.Exception);
            return Try<B>(f(await mr.Value.ConfigureAwait(false)));
        }
        
        public static async ValueTask<TryOption<B>> Traverse<A, B>(this TryOption<ValueTask<A>> ma, Func<A, B> f)
        {
            var mr = ma.Try();
            if (mr.IsBottom) return TryOptionFail<B>(BottomException.Default);
            else if (mr.IsNone) return TryOption<B>(None);
            else if (mr.IsFaulted) return TryOption<B>(mr.Exception);
            return TryOption<B>(f(await mr.Value.Value.ConfigureAwait(false)));
        }
        
        public static async ValueTask<Validation<Fail, B>> Traverse<Fail, A, B>(this Validation<Fail, ValueTask<A>> ma, Func<A, B> f)
        {
            if (ma.IsFail) return Validation<Fail, B>.Fail(ma.FailValue);
            return Validation<Fail, B>.Success(f(await ma.SuccessValue.ConfigureAwait(false)));
        }
        
        public static async ValueTask<Validation<MonoidFail, Fail, B>> Traverse<MonoidFail, Fail, A, B>(this Validation<MonoidFail, Fail, ValueTask<A>> ma, Func<A, B> f)
            where MonoidFail : struct, Monoid<Fail>, Eq<Fail>
        {
            if (ma.IsFail) return Validation<MonoidFail, Fail, B>.Fail(ma.FailValue);
            return Validation<MonoidFail, Fail, B>.Success(f(await ma.SuccessValue.ConfigureAwait(false)));
        }
        
        public static async ValueTask<Eff<B>> Traverse<A, B>(this Eff<ValueTask<A>> ma, Func<A, B> f)
        {
            var mr = ma.Run();
            if (mr.IsBottom) return FailEff<B>(BottomException.Default);
            else if (mr.IsFail) return FailEff<B>(mr.Error);
            return SuccessEff<B>(f(await mr.Value.ConfigureAwait(false)));
        }
    }
}
