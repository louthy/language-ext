using System;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using LanguageExt.DataTypes.Serialisation;

namespace LanguageExt.ClassInstances
{
    public struct MEitherAsync<L, R> :
        Alternative<EitherAsync<L, R>, L, R>,
        OptionalAsync<EitherAsync<L, R>, R>,
        MonadAsync<EitherAsync<L, R>, R>,
        FoldableAsync<EitherAsync<L, R>, R>,
        BiFoldableAsync<EitherAsync<L, R>, L, R>
    {
        public EitherAsync<L, R> NoneAsync => throw new NotImplementedException();

        [Pure]
        public EitherAsync<L, R> Append(EitherAsync<L, R> ma, EitherAsync<L, R> mb) =>
            PlusAsync(ma, mb);

        [Pure]
        public EitherAsync<L, R> ApplyAsync(Func<R, R, R> f, EitherAsync<L, R> ma, EitherAsync<L, R> mb)
        {
            async Task<EitherData<L, R>> Do(Func<R, R, R> ff, EitherAsync<L, R> mma, EitherAsync<L, R> mmb)
            {
                var resA = ma.ToEither();
                var resB = mb.ToEither();
                await Task.WhenAll(resA, resB);
                if (resA.IsCompleted && !resA.IsFaulted && !resA.IsCanceled && resA.Result.IsLeft) return resA.Result.Head();
                if (resB.IsCompleted && !resB.IsFaulted && !resB.IsCanceled && resB.Result.IsLeft) return resB.Result.Head();
                if(resA.Result.IsRight && resB.Result.IsRight)
                {
                    return new EitherData<L, R>(EitherStatus.IsRight, ff(resA.Result.RightValue, resB.Result.RightValue), default(L));
                }
                return EitherData<L, R>.Bottom;
            }
            return new EitherAsync<L, R>(Do(f, ma, mb));
        }

        [Pure]
        public Task<S> BiFoldAsync<S>(EitherAsync<L, R> ma, S state, Func<S, L, S> fa, Func<S, R, S> fb) =>
            ma.BiFold(state, fb, fa);

        [Pure]
        public Task<S> BiFoldAsync<S>(EitherAsync<L, R> ma, S state, Func<S, L, Task<S>> fa, Func<S, R, S> fb) =>
            ma.BiFoldAsync(state, fb, fa);

        [Pure]
        public Task<S> BiFoldAsync<S>(EitherAsync<L, R> ma, S state, Func<S, L, S> fa, Func<S, R, Task<S>> fb) =>
            ma.BiFoldAsync(state, fb, fa);

        [Pure]
        public Task<S> BiFoldAsync<S>(EitherAsync<L, R> ma, S state, Func<S, L, Task<S>> fa, Func<S, R, Task<S>> fb) =>
            ma.BiFoldAsync(state, fb, fa);

        [Pure]
        public Task<S> BiFoldBackAsync<S>(EitherAsync<L, R> ma, S state, Func<S, L, S> fa, Func<S, R, S> fb) =>
            ma.BiFold(state, fb, fa);

        [Pure]
        public Task<S> BiFoldBackAsync<S>(EitherAsync<L, R> ma, S state, Func<S, L, Task<S>> fa, Func<S, R, S> fb) =>
            ma.BiFoldAsync(state, fb, fa);

        [Pure]
        public Task<S> BiFoldBackAsync<S>(EitherAsync<L, R> ma, S state, Func<S, L, S> fa, Func<S, R, Task<S>> fb) =>
            ma.BiFoldAsync(state, fb, fa);

        [Pure]
        public Task<S> BiFoldBackAsync<S>(EitherAsync<L, R> ma, S state, Func<S, L, Task<S>> fa, Func<S, R, Task<S>> fb) =>
            ma.BiFoldAsync(state, fb, fa);

        [Pure]
        public MB BindAsync<MONADB, MB, B>(EitherAsync<L, R> ma, Func<R, MB> f) where MONADB : struct, MonadAsync<Unit, Unit, MB, B> =>
            default(MONADB).RunAsync(_ =>
                ma.Match(
                    Left: l => default(MONADB).FailAsync(l),
                    Right: r => f(r),
                    Bottom: () => default(MONADB).FailAsync(BottomException.Default)));

        [Pure]
        public MB BindAsync<MONADB, MB, B>(EitherAsync<L, R> ma, Func<R, Task<MB>> f) where MONADB : struct, MonadAsync<Unit, Unit, MB, B>
        {
            Task<MB> Do(Unit _) =>
                ma.MatchAsync(
                    Left:  l       => default(MONADB).FailAsync(l),
                    Right: async r => await f(r),
                    Bottom: ()     => default(MONADB).FailAsync(BottomException.Default));

            return default(MONADB).RunAsync(Do);
        }

        [Pure]
        public EitherAsync<L, R> BindReturnAsync(Unit outputma, EitherAsync<L, R> mb) =>
            mb;

        [Pure]
        public Func<Unit, Task<int>> CountAsync(EitherAsync<L, R> fa) => _ =>
            fa.Match(r => 1, l => 0, () => 0);

        [Pure]
        public EitherAsync<L, R> Empty() =>
            EitherAsync<L, R>.Bottom;

        [Pure]
        public EitherAsync<L, R> FailAsync(object err = null) =>
            err != null && err is L
                ? EitherAsync<L, R>.Left((L)err)
                : EitherAsync<L, R>.Bottom;

        [Pure]
        public Func<Unit, Task<S>> FoldAsync<S>(EitherAsync<L, R> fa, S state, Func<S, R, S> f) => _ =>
            fa.Fold(state, f);

        [Pure]
        public Func<Unit, Task<S>> FoldAsync<S>(EitherAsync<L, R> fa, S state, Func<S, R, Task<S>> f) => _ =>
            fa.FoldAsync(state, f);

        [Pure]
        public Func<Unit, Task<S>> FoldAsync<S>(EitherAsync<L, R> fa, S state, Func<S, L, S> f) => _ =>
            fa.BiFold(state, (s, r) => s, f);

        [Pure]
        public Func<Unit, Task<S>> FoldAsync<S>(EitherAsync<L, R> fa, S state, Func<S, L, Task<S>> f) => _ =>
            fa.BiFoldAsync(state, (s, r) => s, f);

        [Pure]
        public Func<Unit, Task<S>> FoldBackAsync<S>(EitherAsync<L, R> fa, S state, Func<S, R, S> f) => _ =>
            fa.Fold(state, f);

        [Pure]
        public Func<Unit, Task<S>> FoldBackAsync<S>(EitherAsync<L, R> fa, S state, Func<S, R, Task<S>> f) => _ =>
            fa.FoldAsync(state, f);

        [Pure]
        public Func<Unit, Task<S>> FoldBackAsync<S>(EitherAsync<L, R> fa, S state, Func<S, L, S> f) => _ =>
            fa.BiFold(state, (s, r) => s, f);

        [Pure]
        public Func<Unit, Task<S>> FoldBackAsync<S>(EitherAsync<L, R> fa, S state, Func<S, L, Task<S>> f) => _ =>
            fa.BiFoldAsync(state, (s, r) => s, f);

        [Pure]
        public Task<bool> IsNoneAsync(EitherAsync<L, R> ma) =>
            ma.IsLeft;

        [Pure]
        public Task<bool> IsSomeAsync(EitherAsync<L, R> ma) =>
            ma.IsRight;

        [Pure]
        public Task<bool> IsUnsafeAsync(EitherAsync<L, R> ma) =>
            false.AsTask();

        [Pure]
        public Task<B> MatchAsync<B>(EitherAsync<L, R> ma, Func<R, B> Some, Func<B> None) =>
            ma.Match(Some, l => None());

        [Pure]
        public Task<B> MatchAsync<B>(EitherAsync<L, R> ma, Func<R, Task<B>> Some, Func<B> None) =>
            ma.MatchAsync(Some, l => None());

        [Pure]
        public Task<B> MatchAsync<B>(EitherAsync<L, R> ma, Func<R, B> Some, Func<Task<B>> None) =>
            ma.MatchAsync(Some, l => None());

        [Pure]
        public Task<B> MatchAsync<B>(EitherAsync<L, R> ma, Func<R, Task<B>> Some, Func<Task<B>> None) =>
            ma.MatchAsync(Some, l => None());

        [Pure]
        public Task<Unit> MatchAsync(EitherAsync<L, R> ma, Action<R> Some, Action None) =>
            ma.Match(Some, l => None());

        [Pure]
        public Task<B> MatchUnsafeAsync<B>(EitherAsync<L, R> ma, Func<R, B> Some, Func<B> None) =>
            ma.MatchUnsafe(Some, l => None());

        [Pure]
        public Task<B> MatchUnsafeAsync<B>(EitherAsync<L, R> ma, Func<R, Task<B>> Some, Func<B> None) =>
            ma.MatchUnsafeAsync(Some, l => None());

        [Pure]
        public Task<B> MatchUnsafeAsync<B>(EitherAsync<L, R> ma, Func<R, B> Some, Func<Task<B>> None) =>
            ma.MatchUnsafeAsync(Some, l => None());

        [Pure]
        public Task<B> MatchUnsafeAsync<B>(EitherAsync<L, R> ma, Func<R, Task<B>> Some, Func<Task<B>> None) =>
            ma.MatchUnsafeAsync(Some, l => None());

        [Pure]
        public EitherAsync<L, R> OptionalAsync(R value) =>
            EitherAsync<L, R>.Right(value);

        [Pure]
        public EitherAsync<L, R> PlusAsync(EitherAsync<L, R> ma, EitherAsync<L, R> mb)
        {
            async Task<EitherData<L, R>> Do(EitherAsync<L, R> mma, EitherAsync<L, R> mmb)
            {
                var resA = ma.ToEither();
                var resB = mb.ToEither();

                var tasks = Set<OrdTask<Either<L, R>>, Task<Either<L, R>>>(resA, resB);

                while (tasks.Count > 0)
                {
                    // Return first one that completes
                    var completed = await Task.WhenAny(tasks);
                    if (!completed.IsFaulted && !completed.Result.IsRight) return completed.Result.Head();
                    tasks = tasks.Remove(completed);
                }

                if (resA.IsCompleted && !resA.IsFaulted && !resA.IsCanceled && resA.Result.IsRight) return resA.Result.Head();
                if (resB.IsCompleted && !resB.IsFaulted && !resB.IsCanceled && resB.Result.IsRight) return resB.Result.Head();
                return EitherData<L, R>.Bottom;
            }
            return new EitherAsync<L, R>(Do(ma, mb));
        }

        [Pure]
        public EitherAsync<L, R> ReturnAsync(Task<R> x) =>
            EitherAsync<L, R>.RightAsync(x);

        [Pure]
        public EitherAsync<L, R> ReturnAsync(Func<Unit, Task<R>> f) =>
            EitherAsync<L, R>.RightAsync(f(unit));

        [Pure]
        public EitherAsync<L, R> RunAsync(Func<Unit, Task<EitherAsync<L, R>>> ma) =>
            new EitherAsync<L, R>(RunAsyncImpl(ma));

        [Pure]
        async Task<EitherData<L, R>> RunAsyncImpl(Func<Unit, Task<EitherAsync<L, R>>> ma) =>
            await(await ma(unit)).data;

        [Pure]
        public EitherAsync<L, R> SomeAsync(R value) =>
            EitherAsync<L, R>.Right(value);

        [Pure]
        public EitherAsync<L, R> ZeroAsync() =>
            EitherAsync<L, R>.Bottom;
    }
}
