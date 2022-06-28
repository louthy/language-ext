using System;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using LanguageExt.Common;
using LanguageExt.DataTypes.Serialisation;

namespace LanguageExt.ClassInstances
{
    public struct MEitherAsync<L, R> :
        ChoiceAsync<EitherAsync<L, R>, L, R>,
        Alternative<EitherAsync<L, R>, L, R>,
        OptionalAsync<EitherAsync<L, R>, R>,
        OptionalUnsafeAsync<EitherAsync<L, R>, R>,
        MonadAsync<EitherAsync<L, R>, R>,
        FoldableAsync<EitherAsync<L, R>, R>,
        BiFoldableAsync<EitherAsync<L, R>, L, R>
    {
        public EitherAsync<L, R> None => throw new NotSupportedException();

        [Pure]
        public EitherAsync<L, R> Append(EitherAsync<L, R> ma, EitherAsync<L, R> mb) =>
            Plus(ma, mb);

        [Pure]
        public EitherAsync<L, R> Apply(Func<R, R, R> f, EitherAsync<L, R> ma, EitherAsync<L, R> mb)
        {
            async Task<EitherData<L, R>> Do(Func<R, R, R> ff, EitherAsync<L, R> mma, EitherAsync<L, R> mmb)
            {
                var resA = ma.ToEither();
                var resB = mb.ToEither();
                await Task.WhenAll(resA, resB).ConfigureAwait(false);
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
        public Task<S> BiFold<S>(EitherAsync<L, R> ma, S state, Func<S, L, S> fa, Func<S, R, S> fb) =>
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
        public Task<S> BiFoldBack<S>(EitherAsync<L, R> ma, S state, Func<S, L, S> fa, Func<S, R, S> fb) =>
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
        public MB Bind<MONADB, MB, B>(EitherAsync<L, R> ma, Func<R, MB> f) where MONADB : struct, MonadAsync<Unit, Unit, MB, B> =>
            default(MONADB).RunAsync(_ =>
                ma.Match(
                    Left: l => default(MONADB).Fail(l),
                    Right: r => f(r),
                    Bottom: () => default(MONADB).Fail(BottomException.Default)));

        [Pure]
        public MB BindAsync<MONADB, MB, B>(EitherAsync<L, R> ma, Func<R, Task<MB>> f) where MONADB : struct, MonadAsync<Unit, Unit, MB, B>
        {
            Task<MB> Do(Unit _) =>
                ma.MatchAsync(
                    Left:  l       => default(MONADB).Fail(l),
                    RightAsync: async r => await f(r).ConfigureAwait(false),
                    Bottom: ()     => default(MONADB).Fail(BottomException.Default));

            return default(MONADB).RunAsync(Do);
        }

        [Pure]
        public EitherAsync<L, R> BindReturn(Unit outputma, EitherAsync<L, R> mb) =>
            mb;

        [Pure]
        public Func<Unit, Task<int>> Count(EitherAsync<L, R> fa) => _ =>
            fa.Match(r => 1, l => 0, () => 0);

        [Pure]
        public EitherAsync<L, R> Empty() =>
            EitherAsync<L, R>.Bottom;

        [Pure]
        public EitherAsync<L, R> Fail(object err = null) =>
            Common.Error
                  .Convert<L>(err)
                  .Map(EitherAsync<L, R>.Left)
                  .IfNone(EitherAsync<L, R>.Bottom);                


        [Pure]
        public Func<Unit, Task<S>> Fold<S>(EitherAsync<L, R> fa, S state, Func<S, R, S> f) => _ =>
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
        public Func<Unit, Task<S>> FoldBack<S>(EitherAsync<L, R> fa, S state, Func<S, R, S> f) => _ =>
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
        public Task<bool> IsNone(EitherAsync<L, R> ma) =>
            ma.IsLeft;

        [Pure]
        public Task<bool> IsSome(EitherAsync<L, R> ma) =>
            ma.IsRight;

        [Pure]
        public Task<B> Match<B>(EitherAsync<L, R> ma, Func<R, B> Some, Func<B> None) =>
            ma.Match(Some, l => None());

        [Pure]
        public Task<B> MatchAsync<B>(EitherAsync<L, R> ma, Func<R, Task<B>> SomeAsync, Func<B> None) =>
            ma.MatchAsync(SomeAsync, l => None());

        [Pure]
        public Task<B> MatchAsync<B>(EitherAsync<L, R> ma, Func<R, B> Some, Func<Task<B>> NoneAsync) =>
            ma.MatchAsync(Some, async l => await NoneAsync().ConfigureAwait(false));

        [Pure]
        public Task<B> MatchAsync<B>(EitherAsync<L, R> ma, Func<R, Task<B>> SomeAsync, Func<Task<B>> NoneAsync) =>
            ma.MatchAsync(SomeAsync, async l => await NoneAsync().ConfigureAwait(false));

        [Pure]
        public Task<Unit> Match(EitherAsync<L, R> ma, Action<R> Some, Action None) =>
            ma.Match(Some, l => None());
        [Pure]
        public Task<Unit> MatchAsync(EitherAsync<L, R> ma, Func<R, Task> SomeAsync, Action None) =>
            ma.MatchAsync(RightAsync: SomeAsync, Left: l => None());

        [Pure]
        public Task<Unit> MatchAsync(EitherAsync<L, R> ma, Action<R> Some, Func<Task> NoneAsync) =>
            ma.MatchAsync(Some, async l => await NoneAsync().ConfigureAwait(false));

        [Pure]
        public Task<Unit> MatchAsync(EitherAsync<L, R> ma, Func<R, Task> SomeAsync, Func<Task> NoneAsync) =>
            ma.MatchAsync(SomeAsync, async l => await NoneAsync().ConfigureAwait(false));

        [Pure]
        public Task<B> MatchUnsafe<B>(EitherAsync<L, R> ma, Func<R, B> Some, Func<B> None) =>
            ma.MatchUnsafe(Some, l => None());

        [Pure]
        public Task<B> MatchUnsafeAsync<B>(EitherAsync<L, R> ma, Func<R, Task<B>> SomeAsync, Func<B> None) =>
            ma.MatchUnsafeAsync(SomeAsync, l => None());

        [Pure]
        public Task<B> MatchUnsafeAsync<B>(EitherAsync<L, R> ma, Func<R, B> Some, Func<Task<B>> NoneAsync) =>
            ma.MatchUnsafeAsync(Some, async l => await NoneAsync().ConfigureAwait(false));

        [Pure]
        public Task<B> MatchUnsafeAsync<B>(EitherAsync<L, R> ma, Func<R, Task<B>> SomeAsync, Func<Task<B>> NoneAsync) =>
            ma.MatchUnsafeAsync(SomeAsync, async l => await NoneAsync().ConfigureAwait(false));

        [Pure]
        public EitherAsync<L, R> Optional(R value) =>
            EitherAsync<L, R>.Right(value);

        [Pure]
        public EitherAsync<L, R> OptionalAsync(Task<R> value) =>
            EitherAsync<L, R>.RightAsync(value);

        [Pure]
        public EitherAsync<L, R> Plus(EitherAsync<L, R> ma, EitherAsync<L, R> mb)
        {
            async Task<EitherData<L, R>> Do(EitherAsync<L, R> mma, EitherAsync<L, R> mmb)
            {
                var resA = ma.ToEither();
                var resB = mb.ToEither();

                var tasks = Set<OrdTask<Either<L, R>>, Task<Either<L, R>>>(resA, resB);

                while (tasks.Count > 0)
                {
                    // Return first one that completes
                    var completed = await Task.WhenAny(tasks).ConfigureAwait(false);
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
            await(await ma(unit).ConfigureAwait(false)).Data.ConfigureAwait(false);

        [Pure]
        public EitherAsync<L, R> Some(R value) =>
            EitherAsync<L, R>.Right(value);

        [Pure]
        public EitherAsync<L, R> SomeAsync(Task<R> value) =>
            EitherAsync<L, R>.RightAsync(value);

        [Pure]
        public EitherAsync<L, R> Zero() =>
            EitherAsync<L, R>.Bottom;

        [Pure]
        public Task<bool> IsLeft(EitherAsync<L, R> choice) =>
            choice.IsLeft;

        [Pure]
        public Task<bool> IsRight(EitherAsync<L, R> choice) =>
            choice.IsRight;

        [Pure]
        public Task<bool> IsBottom(EitherAsync<L, R> choice) =>
            choice.IsBottom;

        [Pure]
        public Task<C> Match<C>(EitherAsync<L, R> choice, Func<L, C> Left, Func<R, C> Right, Func<C> Bottom = null) =>
            choice.Match(Right, Left, Bottom);

        [Pure]
        public Task<C> MatchAsync<C>(EitherAsync<L, R> choice, Func<L, Task<C>> LeftAsync, Func<R, C> Right, Func<C> Bottom = null) =>
            choice.MatchAsync(Right, LeftAsync, Bottom);

        [Pure]
        public Task<C> MatchAsync<C>(EitherAsync<L, R> choice, Func<L, C> Left, Func<R, Task<C>> RightAsync, Func<C> Bottom = null) =>
            choice.MatchAsync(RightAsync, Left, Bottom);

        [Pure]
        public Task<C> MatchAsync<C>(EitherAsync<L, R> choice, Func<L, Task<C>> LeftAsync, Func<R, Task<C>> RightAsync, Func<C> Bottom = null) =>
            choice.MatchAsync(RightAsync, LeftAsync, Bottom);

        public Task<Unit> Match(EitherAsync<L, R> choice, Action<L> Left, Action<R> Right, Action Bottom = null) =>
            choice.Match(Right, Left, Bottom);

        public Task<Unit> MatchAsync(EitherAsync<L, R> choice, Func<L, Task> LeftAsync, Action<R> Right, Action Bottom = null) =>
            choice.MatchAsync(Right, LeftAsync, Bottom);

        public Task<Unit> MatchAsync(EitherAsync<L, R> choice, Action<L> Left, Func<R, Task> RightAsync, Action Bottom = null) =>
            choice.MatchAsync(RightAsync, Left, Bottom);

        public Task<Unit> MatchAsync(EitherAsync<L, R> choice, Func<L, Task> LeftAsync, Func<R, Task> RightAsync, Action Bottom = null) =>
            choice.MatchAsync(RightAsync, LeftAsync, Bottom);

        [Pure]
        public Task<C> MatchUnsafe<C>(EitherAsync<L, R> choice, Func<L, C> Left, Func<R, C> Right, Func<C> Bottom = null) =>
            choice.MatchUnsafe(Right, Left, Bottom);

        [Pure]
        public Task<C> MatchUnsafeAsync<C>(EitherAsync<L, R> choice, Func<L, Task<C>> LeftAsync, Func<R, C> Right, Func<C> Bottom = null) =>
            choice.MatchUnsafeAsync(Right, LeftAsync, Bottom);

        [Pure]
        public Task<C> MatchUnsafeAsync<C>(EitherAsync<L, R> choice, Func<L, C> Left, Func<R, Task<C>> RightAsync, Func<C> Bottom = null) =>
            choice.MatchUnsafeAsync(RightAsync, Left, Bottom);

        [Pure]
        public Task<C> MatchUnsafeAsync<C>(EitherAsync<L, R> choice, Func<L, Task<C>> LeftAsync, Func<R, Task<C>> RightAsync, Func<C> Bottom = null) =>
            choice.MatchUnsafeAsync(RightAsync, LeftAsync, Bottom);
    }
}
