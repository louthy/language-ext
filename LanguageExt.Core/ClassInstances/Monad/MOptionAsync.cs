using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using static LanguageExt.Prelude;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances
{
    public struct MOptionAsync<A> :
        Alternative<OptionAsync<A>, Unit, A>,
        OptionalAsync<OptionAsync<A>, A>,
        MonadAsync<OptionAsync<A>, A>,
        FoldableAsync<OptionAsync<A>, A>,
        BiFoldableAsync<OptionAsync<A>, A, Unit>
    {
        public static readonly MOptionAsync<A> Inst = default(MOptionAsync<A>);

        [Pure]
        public OptionAsync<A> NoneAsync => OptionAsync<A>.None;

        [Pure]
        public MB BindAsync<MonadB, MB, B>(OptionAsync<A> ma, Func<A, MB> f) where MonadB : struct, MonadAsync<Unit, Unit, MB, B> =>
            default(MonadB).RunAsync(async _ =>
                (await ma.IsSome)
                    ? f(await ma.Value)
                    : default(MonadB).FailAsync(ValueIsNoneException.Default));

        [Pure]
        public MB BindAsync<MonadB, MB, B>(OptionAsync<A> ma, Func<A, Task<MB>> f) where MonadB : struct, MonadAsync<Unit, Unit, MB, B> =>
            default(MonadB).RunAsync(async _ =>
                (await ma.IsSome)
                    ? await f(await ma.Value)
                    : default(MonadB).FailAsync(ValueIsNoneException.Default));

        [Pure]
        public OptionAsync<A> FailAsync(object err = null) =>
            OptionAsync<A>.None;

        [Pure]
        public OptionAsync<A> PlusAsync(OptionAsync<A> a, OptionAsync<A> b) =>
            default(MOptionAsync<A>).RunAsync(async __ =>
                await a.IsSome
                    ? a
                    : b);

        [Pure]
        public OptionAsync<A> ReturnAsync(Func<Unit, Task<A>> f)
        {
            async Task<OptionData<A>> Do(Func<Unit, Task<A>> ff) => OptionData<A>.Optional(await ff(unit));
            return new OptionAsync<A>(Do(f));
        }

        [Pure]
        public OptionAsync<A> ZeroAsync() =>
            OptionAsync<A>.None;

        [Pure]
        public Task<bool> IsNoneAsync(OptionAsync<A> opt) =>
            opt.IsNone;

        [Pure]
        public Task<bool> IsSomeAsync(OptionAsync<A> opt) =>
            opt.IsSome;

        [Pure]
        public Task<bool> IsUnsafeAsync(OptionAsync<A> opt) =>
            Task.FromResult(false);

        [Pure]
        public async Task<B> MatchAsync<B>(OptionAsync<A> opt, Func<A, B> Some, Func<B> None)
        {
            if (Some == null) throw new ArgumentNullException(nameof(Some));
            if (None == null) throw new ArgumentNullException(nameof(None));
            return await opt.IsSome
                ? Check.NullReturn(Some(await opt.Value))
                : Check.NullReturn(None());
        }

        [Pure]
        public async Task<B> MatchAsync<B>(OptionAsync<A> opt, Func<A, Task<B>> Some, Func<B> None)
        {
            if (Some == null) throw new ArgumentNullException(nameof(Some));
            if (None == null) throw new ArgumentNullException(nameof(None));
            return await opt.IsSome
                ? Check.NullReturn(await Some(await opt.Value))
                : Check.NullReturn(None());
        }

        [Pure]
        public async Task<B> MatchAsync<B>(OptionAsync<A> opt, Func<A, B> Some, Func<Task<B>> None)
        {
            if (Some == null) throw new ArgumentNullException(nameof(Some));
            if (None == null) throw new ArgumentNullException(nameof(None));
            return await opt.IsSome
                ? Check.NullReturn(Some(await opt.Value))
                : Check.NullReturn(await None());
        }

        [Pure]
        public async Task<B> MatchAsync<B>(OptionAsync<A> opt, Func<A, Task<B>> Some, Func<Task<B>> None)
        {
            if (Some == null) throw new ArgumentNullException(nameof(Some));
            if (None == null) throw new ArgumentNullException(nameof(None));
            return await opt.IsSome
                ? Check.NullReturn(await Some(await opt.Value))
                : Check.NullReturn(await None());
        }

        public async Task<Unit> MatchAsync(OptionAsync<A> opt, Action<A> Some, Action None)
        {
            if (Some == null) throw new ArgumentNullException(nameof(Some));
            if (None == null) throw new ArgumentNullException(nameof(None));
            if (await opt.IsSome) Some(await opt.Value); else None();
            return Unit.Default;
        }

        [Pure]
        public async Task<B> MatchUnsafeAsync<B>(OptionAsync<A> opt, Func<A, B> Some, Func<B> None)
        {
            if (Some == null) throw new ArgumentNullException(nameof(Some));
            if (None == null) throw new ArgumentNullException(nameof(None));
            return await opt.IsSome
                ? Some(await opt.Value)
                : None();
        }

        [Pure]
        public async Task<B> MatchUnsafeAsync<B>(OptionAsync<A> opt, Func<A, Task<B>> Some, Func<B> None)
        {
            if (Some == null) throw new ArgumentNullException(nameof(Some));
            if (None == null) throw new ArgumentNullException(nameof(None));
            return await opt.IsSome
                ? await Some(await opt.Value)
                : None();
        }

        [Pure]
        public async Task<B> MatchUnsafeAsync<B>(OptionAsync<A> opt, Func<A, B> Some, Func<Task<B>> None)
        {
            if (Some == null) throw new ArgumentNullException(nameof(Some));
            if (None == null) throw new ArgumentNullException(nameof(None));
            return await opt.IsSome
                ? Some(await opt.Value)
                : await None();
        }

        [Pure]
        public async Task<B> MatchUnsafeAsync<B>(OptionAsync<A> opt, Func<A, Task<B>> Some, Func<Task<B>> None)
        {
            if (Some == null) throw new ArgumentNullException(nameof(Some));
            if (None == null) throw new ArgumentNullException(nameof(None));
            return await opt.IsSome
                ? await Some(await opt.Value)
                : await None();
        }

        [Pure]
        public OptionAsync<A> SomeAsync(A x) =>
            x.IsNull()
                ? throw new ArgumentNullException("Option doesn't support null values.  Use OptionUnsafe if this is desired behaviour")
                : OptionAsync<A>.Some(x);

        [Pure]
        public OptionAsync<A> OptionalAsync(A x) =>
            OptionAsync<A>.Optional(x);

        [Pure]
        public OptionAsync<A> BindReturnAsync(Unit _, OptionAsync<A> mb) =>
            mb;

        [Pure]
        public OptionAsync<A> ReturnAsync(Task<A> x) =>
            ReturnAsync(_ => x);

        [Pure]
        public OptionAsync<A> RunAsync(Func<Unit, Task<OptionAsync<A>>> ma)
        {
            async Task<OptionData<A>> Do(Func<Unit, Task<OptionAsync<A>>> mma)
            {
                var a = await mma(unit);
                return await a.IsSome
                    ? OptionData<A>.Optional(await a.Value)
                    : OptionData<A>.None;
            }
            return new OptionAsync<A>(Do(ma));
        }

        [Pure]
        public OptionAsync<A> Empty() =>
            None;

        [Pure]
        public OptionAsync<A> Append(OptionAsync<A> x, OptionAsync<A> y) =>
            PlusAsync(x, y);

        [Pure]
        public Func<Unit, Task<S>> FoldAsync<S>(OptionAsync<A> ma, S state, Func<S, A, S> f) => async _ => 
        {
            if (state.IsNull()) throw new ArgumentNullException(nameof(state));
            if (f == null) throw new ArgumentNullException(nameof(f));
            return Check.NullReturn(await ma.IsSome
                ? f(state, await ma.Value)
                : state);
        };

        [Pure]
        public Func<Unit, Task<S>> FoldAsync<S>(OptionAsync<A> ma, S state, Func<S, A, Task<S>> f) => async _ =>
        {
            if (state.IsNull()) throw new ArgumentNullException(nameof(state));
            if (f == null) throw new ArgumentNullException(nameof(f));
            return Check.NullReturn(await ma.IsSome
                ? await f(state, await ma.Value)
                : state);
        };

        [Pure]
        public Func<Unit, Task<S>> FoldBackAsync<S>(OptionAsync<A> ma, S state, Func<S, A, S> f) =>
            FoldAsync(ma, state, f);

        [Pure]
        public Func<Unit, Task<S>> FoldBackAsync<S>(OptionAsync<A> ma, S state, Func<S, A, Task<S>> f) => 
            FoldAsync(ma, state, f);

        [Pure]
        public Func<Unit, Task<int>> CountAsync(OptionAsync<A> ma) => async _ =>
            await ma.IsSome
                ? 1
                : 0;

        [Pure]
        public async Task<S> BiFoldAsync<S>(OptionAsync<A> ma, S state, Func<S, A, S> fa, Func<S, Unit, S> fb)
        {
            if (state.IsNull()) throw new ArgumentNullException(nameof(state));
            if (fa == null) throw new ArgumentNullException(nameof(fa));
            if (fb == null) throw new ArgumentNullException(nameof(fb));
            return Check.NullReturn(await ma.IsNone
                ? fa(state, await ma.Value)
                : fb(state, unit));
        }

        [Pure]
        public async Task<S> BiFoldAsync<S>(OptionAsync<A> ma, S state, Func<S, A, S> fa, Func<S, Unit, Task<S>> fb)
        {
            if (state.IsNull()) throw new ArgumentNullException(nameof(state));
            if (fa == null) throw new ArgumentNullException(nameof(fa));
            if (fb == null) throw new ArgumentNullException(nameof(fb));
            return Check.NullReturn(await ma.IsNone
                ? fa(state, await ma.Value)
                : await fb(state, unit));
        }

        [Pure]
        public async Task<S> BiFoldAsync<S>(OptionAsync<A> ma, S state, Func<S, A, Task<S>> fa, Func<S, Unit, S> fb)
        {
            if (state.IsNull()) throw new ArgumentNullException(nameof(state));
            if (fa == null) throw new ArgumentNullException(nameof(fa));
            if (fb == null) throw new ArgumentNullException(nameof(fb));
            return Check.NullReturn(await ma.IsNone
                ? await fa(state, await ma.Value)
                : fb(state, unit));
        }

        [Pure]
        public async Task<S> BiFoldAsync<S>(OptionAsync<A> ma, S state, Func<S, A, Task<S>> fa, Func<S, Unit, Task<S>> fb)
        {
            if (state.IsNull()) throw new ArgumentNullException(nameof(state));
            if (fa == null) throw new ArgumentNullException(nameof(fa));
            if (fb == null) throw new ArgumentNullException(nameof(fb));
            return Check.NullReturn(await ma.IsNone
                ? await fa(state, await ma.Value)
                : await fb(state, unit));
        }

        [Pure]
        public Task<S> BiFoldBackAsync<S>(OptionAsync<A> ma, S state, Func<S, A, S> fa, Func<S, Unit, S> fb) =>
            BiFoldAsync(ma, state, fa, fb);

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
        public OptionAsync<A> ApplyAsync(Func<A, A, A> f, OptionAsync<A> fa, OptionAsync<A> fb) =>
            default(MOptionAsync<A>).RunAsync( async _ =>
            {
                var somes = await Task.WhenAll(fa.IsSome, fb.IsSome);
                if (!somes[0] || !somes[1]) return None;
                var values = await Task.WhenAll(fa.Value, fb.Value);
                return f(values[0], values[1]);
            });
    }
}
