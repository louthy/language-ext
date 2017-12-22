using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    public static class TaskTransformerExtensions
    {
        public static async Task<Arr<B>> Traverse<A, B>(this Arr<Task<A>> ma, Func<A, B> f) =>
            toArray((await Task.WhenAll(ma)).Map(f));

        public static async Task<Either<L, B>> Traverse<L, A, B>(this Either<L, Task<A>> ma, Func<A, B> f) =>
            await ma.MatchAsync(
                Right: r => Either<L, B>.Right(f(r)),
                Left:  l => Either<L, B>.Left(l));

        public static async Task<EitherUnsafe<L, B>> Traverse<L, A, B>(this EitherUnsafe<L, Task<A>> ma, Func<A, B> f) =>
            await ma.MatchAsync(
                Right: r => EitherUnsafe<L, B>.Right(f(r)),
                Left:  l => EitherUnsafe<L, B>.Left(l));

        public static async Task<HashMap<K, B>> Traverse<K, A, B>(this HashMap<K, Task<A>> ma, Func<A, B> f)
        {
            await Task.WhenAll(ma.Values);
            return ma.Map(a => f(a.Result));    // Hmm, is this the best way?
        }

        public static async Task<HashSet<B>> Traverse<A, B>(this HashSet<Task<A>> ma, Func<A, B> f) =>
            toHashSet((await Task.WhenAll(ma)).Map(f));

        public static async Task<Lst<B>> Traverse<A, B>(this Lst<Task<A>> ma, Func<A, B> f) =>
            toList((await Task.WhenAll(ma)).Map(f));

        public static async Task<Option<B>> Traverse<A, B>(this Option<Task<A>> ma, Func<A, B> f) =>
            await ma.MatchAsync(
                Some: async x => Option<B>.Some(f(await x)),
                None: ()      => Option<B>.None);

        public static async Task<OptionUnsafe<B>> Traverse<A, B>(this OptionUnsafe<Task<A>> ma, Func<A, B> f) =>
            await ma.MatchAsync(
                Some: async x => OptionUnsafe<B>.Some(f(await x)),
                None: ()      => OptionUnsafe<B>.None);

        public static async Task<Seq<B>> Traverse<A, B>(this Seq<Task<A>> ma, Func<A, B> f) =>
            Seq((await Task.WhenAll(ma)).Map(f));

        public static async Task<Map<K, B>> Traverse<K, A, B>(this Map<K, Task<A>> ma, Func<A, B> f)
        {
            await Task.WhenAll(ma.Values);
            return ma.Map(a => f(a.Result));    // Hmm, is this the best way?
        }

        public static async Task<IEnumerable<A>> Sequence<A>(this IEnumerable<Task<A>> ma) =>
            await Task.WhenAll(ma);

    }
}
