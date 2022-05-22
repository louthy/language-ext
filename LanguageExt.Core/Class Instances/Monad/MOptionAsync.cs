using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using static LanguageExt.Prelude;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances
{
    public readonly struct MOptionAsync<A> :
        Alternative<OptionAsync<A>, Unit, A>,
        OptionalAsync<OptionAsync<A>, A>,
        OptionalUnsafeAsync<OptionAsync<A>, A>,
        MonadAsync<OptionAsync<A>, A>,
        FoldableAsync<OptionAsync<A>, A>,
        BiFoldableAsync<OptionAsync<A>, A, Unit>
    {
        public static readonly MOptionAsync<A> Inst = default(MOptionAsync<A>);

        [Pure]
        public OptionAsync<A> None => OptionAsync<A>.None;

        [Pure]
        public MB Bind<MonadB, MB, B>(OptionAsync<A> ma, Func<A, MB> f) where MonadB : struct, MonadAsync<Unit, Unit, MB, B> =>
            default(MonadB).RunAsync(async _ =>
                (await ma.IsSome.ConfigureAwait(false))
                    ? f(await ma.Value.ConfigureAwait(false))
                    : default(MonadB).Fail(ValueIsNoneException.Default));

        [Pure]
        public MB BindAsync<MonadB, MB, B>(OptionAsync<A> ma, Func<A, Task<MB>> f) where MonadB : struct, MonadAsync<Unit, Unit, MB, B> =>
            default(MonadB).RunAsync(async _ =>
                (await ma.IsSome.ConfigureAwait(false))
                    ? await f(await ma.Value.ConfigureAwait(false)).ConfigureAwait(false)
                    : default(MonadB).Fail(ValueIsNoneException.Default));

        [Pure]
        public OptionAsync<A> Fail(object err = null) =>
            OptionAsync<A>.None;

        [Pure]
        public OptionAsync<A> Plus(OptionAsync<A> a, OptionAsync<A> b) =>
            default(MOptionAsync<A>).RunAsync(async __ =>
                await a.IsSome.ConfigureAwait(false)
                    ? a
                    : b);

        [Pure]
        public OptionAsync<A> ReturnAsync(Func<Unit, Task<A>> f) =>
            OptionAsync<A>.SomeAsync(f(unit));

        [Pure]
        public OptionAsync<A> Zero() =>
            OptionAsync<A>.None;

        [Pure]
        public Task<bool> IsNone(OptionAsync<A> opt) =>
            opt.IsNone;

        [Pure]
        public Task<bool> IsSome(OptionAsync<A> opt) =>
            opt.IsSome;

        [Pure]
        public async Task<B> Match<B>(OptionAsync<A> opt, Func<A, B> Some, Func<B> None)
        {
            if (Some == null) throw new ArgumentNullException(nameof(Some));
            if (None == null) throw new ArgumentNullException(nameof(None));
            return await opt.IsSome.ConfigureAwait(false)
                ? Check.NullReturn(Some(await opt.Value.ConfigureAwait(false)))
                : Check.NullReturn(None());
        }

        [Pure]
        public async Task<B> MatchAsync<B>(OptionAsync<A> opt, Func<A, Task<B>> SomeAsync, Func<B> None)
        {
            if (SomeAsync == null) throw new ArgumentNullException(nameof(SomeAsync));
            if (None == null) throw new ArgumentNullException(nameof(None));
            return await opt.IsSome.ConfigureAwait(false)
                ? Check.NullReturn(await SomeAsync(await opt.Value.ConfigureAwait(false)).ConfigureAwait(false))
                : Check.NullReturn(None());
        }

        [Pure]
        public async Task<B> MatchAsync<B>(OptionAsync<A> opt, Func<A, B> Some, Func<Task<B>> NoneAsync)
        {
            if (Some == null) throw new ArgumentNullException(nameof(Some));
            if (NoneAsync == null) throw new ArgumentNullException(nameof(NoneAsync));
            return await opt.IsSome.ConfigureAwait(false)
                ? Check.NullReturn(Some(await opt.Value.ConfigureAwait(false)))
                : Check.NullReturn(await NoneAsync().ConfigureAwait(false));
        }

        [Pure]
        public async Task<B> MatchAsync<B>(OptionAsync<A> opt, Func<A, Task<B>> SomeAsync, Func<Task<B>> NoneAsync)
        {
            if (SomeAsync == null) throw new ArgumentNullException(nameof(SomeAsync));
            if (NoneAsync == null) throw new ArgumentNullException(nameof(NoneAsync));
            return await opt.IsSome.ConfigureAwait(false)
                ? Check.NullReturn(await SomeAsync(await opt.Value.ConfigureAwait(false)).ConfigureAwait(false))
                : Check.NullReturn(await NoneAsync().ConfigureAwait(false));
        }

        public async Task<Unit> Match(OptionAsync<A> opt, Action<A> Some, Action None)
        {
            if (Some == null) throw new ArgumentNullException(nameof(Some));
            if (None == null) throw new ArgumentNullException(nameof(None));
            if (await opt.IsSome.ConfigureAwait(false)) Some(await opt.Value.ConfigureAwait(false)); else None();
            return Unit.Default;
        }

        public async Task<Unit> MatchAsync(OptionAsync<A> opt, Func<A, Task> SomeAsync, Action None)
        {
            if (SomeAsync == null) throw new ArgumentNullException(nameof(SomeAsync));
            if (None == null) throw new ArgumentNullException(nameof(None));
            if (await opt.IsSome.ConfigureAwait(false)) await SomeAsync(await opt.Value.ConfigureAwait(false)); else None();
            return Unit.Default;
        }

        public async Task<Unit> MatchAsync(OptionAsync<A> opt, Action<A> Some, Func<Task> NoneAsync)
        {
            if (Some == null) throw new ArgumentNullException(nameof(Some));
            if (NoneAsync == null) throw new ArgumentNullException(nameof(NoneAsync));
            if (await opt.IsSome.ConfigureAwait(false)) Some(await opt.Value.ConfigureAwait(false)); else await NoneAsync().ConfigureAwait(false);
            return Unit.Default;
        }

        public async Task<Unit> MatchAsync(OptionAsync<A> opt, Func<A, Task> SomeAsync, Func<Task> NoneAsync)
        {
            if (SomeAsync == null) throw new ArgumentNullException(nameof(SomeAsync));
            if (NoneAsync == null) throw new ArgumentNullException(nameof(NoneAsync));
            if (await opt.IsSome.ConfigureAwait(false)) await SomeAsync(await opt.Value.ConfigureAwait(false)); else await NoneAsync().ConfigureAwait(false);
            return Unit.Default;
        }


        [Pure]
        public async Task<B> MatchUnsafe<B>(OptionAsync<A> opt, Func<A, B> Some, Func<B> None)
        {
            if (Some == null) throw new ArgumentNullException(nameof(Some));
            if (None == null) throw new ArgumentNullException(nameof(None));
            return await opt.IsSome.ConfigureAwait(false)
                ? Some(await opt.Value.ConfigureAwait(false))
                : None();
        }

        [Pure]
        public async Task<B> MatchUnsafeAsync<B>(OptionAsync<A> opt, Func<A, Task<B>> SomeAsync, Func<B> None)
        {
            if (SomeAsync == null) throw new ArgumentNullException(nameof(SomeAsync));
            if (None == null) throw new ArgumentNullException(nameof(None));
            return await opt.IsSome.ConfigureAwait(false)
                ? await SomeAsync(await opt.Value.ConfigureAwait(false))
                : None();
        }

        [Pure]
        public async Task<B> MatchUnsafeAsync<B>(OptionAsync<A> opt, Func<A, B> Some, Func<Task<B>> NoneAsync)
        {
            if (Some == null) throw new ArgumentNullException(nameof(Some));
            if (NoneAsync == null) throw new ArgumentNullException(nameof(NoneAsync));
            return await opt.IsSome.ConfigureAwait(false)
                ? Some(await opt.Value.ConfigureAwait(false))
                : await NoneAsync().ConfigureAwait(false);
        }

        [Pure]
        public async Task<B> MatchUnsafeAsync<B>(OptionAsync<A> opt, Func<A, Task<B>> SomeAsync, Func<Task<B>> NoneAsync)
        {
            if (SomeAsync == null) throw new ArgumentNullException(nameof(SomeAsync));
            if (NoneAsync == null) throw new ArgumentNullException(nameof(NoneAsync));
            return await opt.IsSome.ConfigureAwait(false)
                ? await SomeAsync(await opt.Value.ConfigureAwait(false)).ConfigureAwait(false)
                : await NoneAsync().ConfigureAwait(false);
        }

        [Pure]
        public OptionAsync<A> Some(A value) =>
            isnull(value)
                ? throw new ArgumentNullException(nameof(value))
                : OptionAsync<A>.Some(value);

        [Pure]
        public OptionAsync<A> SomeAsync(Task<A> taskA) =>
            isnull(taskA)
                ? throw new ArgumentNullException(nameof(taskA))
                : OptionAsync<A>.SomeAsync(taskA);

        [Pure]
        public OptionAsync<A> Optional(A value) =>
            OptionAsync<A>.Optional(value);

        [Pure]
        public OptionAsync<A> OptionalAsync(Task<A> taskA) =>
            OptionAsync<A>.OptionalAsync(taskA);

        [Pure]
        public OptionAsync<A> BindReturn(Unit _, OptionAsync<A> mb) =>
            mb;

        [Pure]
        public OptionAsync<A> ReturnAsync(Task<A> x) =>
            ReturnAsync(_ => x);

        [Pure]
        public OptionAsync<A> RunAsync(Func<Unit, Task<OptionAsync<A>>> ma)
        {
            async Task<(bool IsSome, A Value)> Do(Func<Unit, Task<OptionAsync<A>>> mma)
            {
                var a = await mma(unit).ConfigureAwait(false);
                return await a.Data.ConfigureAwait(false);
            }
            return new OptionAsync<A>(Do(ma));
        }

        [Pure]
        public OptionAsync<A> Empty() =>
            None;

        [Pure]
        public OptionAsync<A> Append(OptionAsync<A> x, OptionAsync<A> y) =>
            Plus(x, y);

        [Pure]
        public Func<Unit, Task<S>> Fold<S>(OptionAsync<A> ma, S state, Func<S, A, S> f) => async _ => 
        {
            if (state.IsNull()) throw new ArgumentNullException(nameof(state));
            f = f ?? throw new ArgumentNullException(nameof(f));
            return Check.NullReturn(await ma.IsSome.ConfigureAwait(false)
                ? f(state, await ma.Value.ConfigureAwait(false))
                : state);
        };

        [Pure]
        public Func<Unit, Task<S>> FoldAsync<S>(OptionAsync<A> ma, S state, Func<S, A, Task<S>> f) => async _ =>
        {
            if (state.IsNull()) throw new ArgumentNullException(nameof(state));
            f = f ?? throw new ArgumentNullException(nameof(f));
            return Check.NullReturn(await ma.IsSome.ConfigureAwait(false)
                ? await f(state, await ma.Value.ConfigureAwait(false))
                : state);
        };

        [Pure]
        public Func<Unit, Task<S>> FoldBack<S>(OptionAsync<A> ma, S state, Func<S, A, S> f) =>
            Fold(ma, state, f);

        [Pure]
        public Func<Unit, Task<S>> FoldBackAsync<S>(OptionAsync<A> ma, S state, Func<S, A, Task<S>> f) => 
            FoldAsync(ma, state, f);

        [Pure]
        public Func<Unit, Task<int>> Count(OptionAsync<A> ma) => async _ =>
            await ma.IsSome.ConfigureAwait(false)
                ? 1
                : 0;

        [Pure]
        public async Task<S> BiFold<S>(OptionAsync<A> ma, S state, Func<S, A, S> fa, Func<S, Unit, S> fb)
        {
            if (state.IsNull()) throw new ArgumentNullException(nameof(state));
            if (fa == null) throw new ArgumentNullException(nameof(fa));
            if (fb == null) throw new ArgumentNullException(nameof(fb));
            return Check.NullReturn(await ma.IsSome.ConfigureAwait(false)
                ? fa(state, await ma.Value.ConfigureAwait(false))
                : fb(state, unit));
        }

        [Pure]
        public async Task<S> BiFoldAsync<S>(OptionAsync<A> ma, S state, Func<S, A, S> fa, Func<S, Unit, Task<S>> fb)
        {
            if (state.IsNull()) throw new ArgumentNullException(nameof(state));
            if (fa == null) throw new ArgumentNullException(nameof(fa));
            if (fb == null) throw new ArgumentNullException(nameof(fb));
            return Check.NullReturn(await ma.IsSome.ConfigureAwait(false)
                ? fa(state, await ma.Value.ConfigureAwait(false))
                : await fb(state, unit).ConfigureAwait(false));
        }

        [Pure]
        public async Task<S> BiFoldAsync<S>(OptionAsync<A> ma, S state, Func<S, A, Task<S>> fa, Func<S, Unit, S> fb)
        {
            if (state.IsNull()) throw new ArgumentNullException(nameof(state));
            if (fa == null) throw new ArgumentNullException(nameof(fa));
            if (fb == null) throw new ArgumentNullException(nameof(fb));
            return Check.NullReturn(await ma.IsSome.ConfigureAwait(false)
                ? await fa(state, await ma.Value.ConfigureAwait(false))
                : fb(state, unit));
        }

        [Pure]
        public async Task<S> BiFoldAsync<S>(OptionAsync<A> ma, S state, Func<S, A, Task<S>> fa, Func<S, Unit, Task<S>> fb)
        {
            if (state.IsNull()) throw new ArgumentNullException(nameof(state));
            if (fa == null) throw new ArgumentNullException(nameof(fa));
            if (fb == null) throw new ArgumentNullException(nameof(fb));
            return Check.NullReturn(await ma.IsSome.ConfigureAwait(false)
                ? await fa(state, await ma.Value.ConfigureAwait(false)).ConfigureAwait(false)
                : await fb(state, unit).ConfigureAwait(false));
        }

        [Pure]
        public Task<S> BiFoldBack<S>(OptionAsync<A> ma, S state, Func<S, A, S> fa, Func<S, Unit, S> fb) =>
            BiFold(ma, state, fa, fb);

        [Pure]
        public Task<S> BiFoldBackAsync<S>(OptionAsync<A> ma, S state, Func<S, A, S> fa, Func<S, Unit, Task<S>> fb) =>
            BiFoldAsync(ma, state, fa, fb);

        [Pure]
        public Task<S> BiFoldBackAsync<S>(OptionAsync<A> ma, S state, Func<S, A, Task<S>> fa, Func<S, Unit, S> fb) =>
            BiFoldAsync(ma, state, fa, fb);

        [Pure]
        public Task<S> BiFoldBackAsync<S>(OptionAsync<A> ma, S state, Func<S, A, Task<S>> fa, Func<S, Unit, Task<S>> fb) =>
            BiFoldAsync(ma, state, fa, fb);

        [Pure]
        public OptionAsync<A> Apply(Func<A, A, A> f, OptionAsync<A> fa, OptionAsync<A> fb) =>
            default(MOptionAsync<A>).RunAsync( async _ =>
            {
                var somes = await Task.WhenAll(fa.IsSome, fb.IsSome).ConfigureAwait(false);
                if (!somes[0] || !somes[1]) return OptionAsync<A>.None;
                var values = await Task.WhenAll(fa.Value, fb.Value).ConfigureAwait(false);
                return f(values[0], values[1]);
            });
    }
}
