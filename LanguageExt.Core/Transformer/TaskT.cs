using System;
using LanguageExt;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    public static class TaskTExtensions
    {
        //
        // Collections
        //
 
        public static async Task<Arr<B>> Traverse<A, B>(this Arr<Task<A>> ma, Func<A, B> f)
        {
            var rb = await Task.WhenAll(ma.Map(async a => f(await a)));
            return new Arr<B>(rb);
        }
        
        public static async Task<HashSet<B>> Traverse<A, B>(this HashSet<Task<A>> ma, Func<A, B> f)
        {
            var rb = await Task.WhenAll(ma.Map(async a => f(await a)));
            return new HashSet<B>(rb);
        }
        
        [Obsolete("use TraverseSerial or TraverseParallel instead")]
        public static Task<IEnumerable<B>> Traverse<L, A, B>(this IEnumerable<Task<A>> ma, Func<A, B> f) =>
            TraverseParallel(ma, f);
        
        public static async Task<IEnumerable<B>> TraverseSerial<A, B>(this IEnumerable<Task<A>> ma, Func<A, B> f)
        {
            var rb = new List<B>();
            foreach (var a in ma)
            {
                rb.Add(f(await a));
            }
            return rb;            
        }

        public static Task<IEnumerable<B>> TraverseParallel<A, B>(this IEnumerable<Task<A>> ma, int windowSize, Func<A, B> f) =>
            ma.WindowMap(windowSize, f).Map(xs => (IEnumerable<B>)xs);

        public static Task<IEnumerable<B>> TraverseParallel<A, B>(this IEnumerable<Task<A>> ma, Func<A, B> f) =>
            ma.WindowMap(f).Map(xs => (IEnumerable<B>)xs);
        
        public static async Task<Lst<B>> Traverse<A, B>(this Lst<Task<A>> ma, Func<A, B> f)
        {
            var rb = await Task.WhenAll(ma.Map(async a => f(await a)));
            return new Lst<B>(rb);
        }
        
        public static async Task<Que<B>> Traverse<A, B>(this Que<Task<A>> ma, Func<A, B> f)
        {
            var rb = await Task.WhenAll(ma.Map(async a => f(await a)));
            return new Que<B>(rb);
        }

        [Obsolete("use TraverseSerial or TraverseParallel instead")]
        public static Task<Seq<B>> Traverse<L, A, B>(this Seq<Task<A>> ma, Func<A, B> f) =>
            TraverseParallel(ma, f);
 
        public static async Task<Seq<B>> TraverseSerial<A, B>(this Seq<Task<A>> ma, Func<A, B> f)
        {
            var rb = new List<B>();
            foreach (var a in ma)
            {
                rb.Add(f(await a));
            }
            return Seq.FromArray<B>(rb.ToArray());
        }
        
        public static Task<Seq<B>> TraverseParallel<A, B>(this Seq<Task<A>> ma, int windowSize, Func<A, B> f) =>
            ma.WindowMap(windowSize, f).Map(xs => Seq(xs));        

        public static Task<Seq<B>> TraverseParallel<A, B>(this Seq<Task<A>> ma, Func<A, B> f) =>
            ma.WindowMap(f).Map(xs => Seq(xs));

        public static async Task<Set<B>> Traverse<A, B>(this Set<Task<A>> ma, Func<A, B> f)
        {
            var rb = await Task.WhenAll(ma.Map(async a => f(await a)));
            return new Set<B>(rb);
        }

        public static async Task<Stck<B>> Traverse<A, B>(this Stck<Task<A>> ma, Func<A, B> f)
        {
            var rb = await Task.WhenAll(ma.Map(async a => f(await a)));
            return new Stck<B>(rb);
        }

        //
        // Async types
        //

        public static Task<EitherAsync<L, B>> Traverse<L, A, B>(this EitherAsync<L, Task<A>> ma, Func<A, B> f) =>
            ma.MatchAsync(
                RightAsync: async ta => EitherAsync<L, B>.Right(f(await ta)), 
                Left: EitherAsync<L, B>.Left);

        public static Task<OptionAsync<B>> Traverse<A, B>(this OptionAsync<Task<A>> ma, Func<A, B> f) =>
            ma.MatchAsync(
                Some: async ta => OptionAsync<B>.Some(f(await ta)), 
                None: () => OptionAsync<B>.None);
        
        public static Task<TryAsync<B>> Traverse<A, B>(this TryAsync<Task<A>> ma, Func<A, B> f) =>
            ma.Match(
                Succ: async ta => TryAsync<B>(f(await ta)), 
                Fail: ex => TryAsync<B>(ex));
        
        public static Task<TryOptionAsync<B>> Traverse<A, B>(this TryOptionAsync<Task<A>> ma, Func<A, B> f) =>
            ma.MatchAsync(
                SomeAsync: async ta => TryOptionAsync<B>(f(await ta)),
                None: () => TryOptionAsync<B>(None),
                Fail: ex => TryOptionAsync<B>(ex));

        public static Task<Task<B>> Traverse<A, B>(this Task<Task<A>> ma, Func<A, B> f) =>
            ma.MapT(f);
        
        //
        // Sync types
        // 
        
        public static async Task<Either<L, B>> Traverse<L, A, B>(this Either<L, Task<A>> ma, Func<A, B> f)
        {
            if (ma.IsBottom) return Either<L, B>.Bottom;
            else if (ma.IsLeft) return Either<L, B>.Left(ma.LeftValue);
            return Either<L, B>.Right(f(await ma.RightValue));
        }

        public static async Task<EitherUnsafe<L, B>> Traverse<L, A, B>(this EitherUnsafe<L, Task<A>> ma, Func<A, B> f)
        {
            if (ma.IsBottom) return EitherUnsafe<L, B>.Bottom;
            else if (ma.IsLeft) return EitherUnsafe<L, B>.Left(ma.LeftValue);
            return EitherUnsafe<L, B>.Right(f(await ma.RightValue));
        }

        public static async Task<Identity<B>> Traverse<A, B>(this Identity<Task<A>> ma, Func<A, B> f) =>
            new Identity<B>(f(await ma.Value));
        
        public static async Task<Option<B>> Traverse<A, B>(this Option<Task<A>> ma, Func<A, B> f)
        {
            if (ma.IsNone) return Option<B>.None;
            return Option<B>.Some(f(await ma.Value));
        }
        
        public static async Task<OptionUnsafe<B>> Traverse<A, B>(this OptionUnsafe<Task<A>> ma, Func<A, B> f)
        {
            if (ma.IsNone) return OptionUnsafe<B>.None;
            return OptionUnsafe<B>.Some(f(await ma.Value));
        }
        
        public static async Task<Try<B>> Traverse<A, B>(this Try<Task<A>> ma, Func<A, B> f)
        {
            try
            {
                var mr = ma.Try();
                if (mr.IsBottom) return Try<B>(() => throw new BottomException());
                else if (mr.IsFaulted) return Try<B>(mr.Exception);
                return Try<B>(f(await mr.Value));
            }
            catch (Exception e)
            {
                return Try<B>(e);
            }
        }
        
        public static async Task<TryOption<B>> Traverse<A, B>(this TryOption<Task<A>> ma, Func<A, B> f)
        {
            try
            {
                var mr = ma.Try();
                if (mr.IsBottom) return TryOption<B>(() => throw new BottomException());
                else if (mr.IsNone) return TryOption<B>(None);
                else if (mr.IsFaulted) return TryOption<B>(mr.Exception);
                return TryOption<B>(f(await mr.Value.Value));
            }
            catch (Exception e)
            {
                return TryOption<B>(e);
            }
        }
        
        public static async Task<Validation<Fail, B>> Traverse<Fail, A, B>(this Validation<Fail, Task<A>> ma, Func<A, B> f)
        {
            if (ma.IsFail) return Validation<Fail, B>.Fail(ma.FailValue);
            return Validation<Fail, B>.Success(f(await ma.SuccessValue));
        }
        
        public static async Task<Validation<MonoidFail, Fail, B>> Traverse<MonoidFail, Fail, A, B>(this Validation<MonoidFail, Fail, Task<A>> ma, Func<A, B> f)
            where MonoidFail : struct, Monoid<Fail>, Eq<Fail>
        {
            if (ma.IsFail) return Validation<MonoidFail, Fail, B>.Fail(ma.FailValue);
            return Validation<MonoidFail, Fail, B>.Success(f(await ma.SuccessValue));
        }
    }
}
