using System;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using static LanguageExt.Prelude;
using System.Threading.Tasks;
using LanguageExt.Common;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Class instance to give `ValueTask<A>` the following traits: 
    ///     
    ///     MonadAsync
    ///     FoldableAsync
    ///     BiFoldableAsync
    ///     OptionalAsymc
    ///     OptionalUnsafeAsync
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    public struct MValueTask<A> :
        OptionalAsync<ValueTask<A>, A>,
        OptionalUnsafeAsync<ValueTask<A>, A>,
        MonadAsync<ValueTask<A>, A>,
        FoldableAsync<ValueTask<A>, A>,
        BiFoldableAsync<ValueTask<A>, A, Unit>
    {
        public static readonly MValueTask<A> Inst = default(MValueTask<A>);

        [Pure]
        public ValueTask<A> None =>
            BottomException.Default.AsFailedValueTask<A>();

        [Pure]
        public MB Bind<MONADB, MB, B>(ValueTask<A> ma, Func<A, MB> f) where MONADB : struct, MonadAsync<Unit, Unit, MB, B> =>
            default(MONADB).RunAsync(async _ => f(await ma.ConfigureAwait(false)));

        [Pure]
        public MB BindAsync<MONADB, MB, B>(ValueTask<A> ma, Func<A, Task<MB>> f) where MONADB : struct, MonadAsync<Unit, Unit, MB, B> =>
            default(MONADB).RunAsync(async _ => await f(await ma.ConfigureAwait(false)).ConfigureAwait(false));

        [Pure]
        public ValueTask<A> Fail(object err = null) =>
            Common.Error
                  .Convert<Exception>(err)
                  .Map(e => e.AsFailedValueTask<A>())
                  .IfNone(None);            

        /// <summary>
        /// The `Plus` function will return `ma` if successful, `mb` otherwise
        /// </summary>
        [Pure]
        public async ValueTask<A> Plus(ValueTask<A> ma, ValueTask<A> mb)
        {
            try
            {
                var ra = await ma.ConfigureAwait(false);
                return ma.IsFaulted || ma.IsCanceled
                    ? await mb.ConfigureAwait(false)
                    : ra;
            }
            catch
            {
                return await mb.ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Monad return
        /// </summary>
        /// <typeparam name="A">Type of the bound monad value</typeparam>
        /// <param name="x">The bound monad value</param>
        /// <returns>Monad of A</returns>
        [Pure]
        public ValueTask<A> ReturnAsync(Task<A> x) =>
            x.ToValue();

        /// <summary>
        /// Monad return
        /// </summary>
        /// <typeparam name="A">Type of the bound monad value</typeparam>
        /// <returns>Monad of A</returns>
        [Pure]
        public async ValueTask<A> ReturnAsync(Func<Unit, Task<A>> f) =>
            await f(unit).ConfigureAwait(false);

        [Pure]
        public ValueTask<A> Zero() => 
            None;

        [Pure]
        public Task<bool> IsNone(ValueTask<A> ma) =>
            ma.IsFaulted.AsTask();

        [Pure]
        public Task<bool> IsSome(ValueTask<A> ma) =>
            from a in IsNone(ma)
            select !a;

        [Pure]
        public async Task<B> Match<B>(ValueTask<A> ma, Func<A, B> Some, Func<B> None)
        {
            if(ma.IsCanceled || ma.IsFaulted)
            {
                return Check.NullReturn(None());
            }
            try
            {
                var a = await ma.ConfigureAwait(false);
                return Check.NullReturn(Some(a));
            }
            catch (Exception)
            {
                return Check.NullReturn(None());
            }
        }

        [Pure]
        public ValueTask<A> Some(A value) =>
            new ValueTask<A>(value);

        [Pure]
        public ValueTask<A> Optional(A value) =>
            new ValueTask<A>(value);

        [Pure]
        public ValueTask<A> BindReturn(Unit _, ValueTask<A> mb) =>
            mb;

        [Pure]
        public ValueTask<A> RunAsync(Func<Unit, Task<ValueTask<A>>> ma) =>
            from ta in ma(unit).ToValue()
            from a in ta
            select a;

        [Pure]
        public Func<Unit, Task<S>> Fold<S>(ValueTask<A> fa, S state, Func<S, A, S> f) => _ =>
            (from a in fa
             select f(state, a))
            .AsTask();

        [Pure]
        public Func<Unit, Task<S>> FoldAsync<S>(ValueTask<A> fa, S state, Func<S, A, Task<S>> f) => _ =>
            (from a in fa
             from s in f(state, a).ToValue()
             select s)
            .AsTask();

        [Pure]
        public Func<Unit, Task<S>> FoldBack<S>(ValueTask<A> fa, S state, Func<S, A, S> f) => _ =>
            (from a in fa
             select f(state, a))
            .AsTask();

        [Pure]
        public Func<Unit, Task<S>> FoldBackAsync<S>(ValueTask<A> fa, S state, Func<S, A, Task<S>> f) => _ =>
            (from a in fa
             from s in f(state, a).ToValue()
             select s)
            .AsTask();

        [Pure]
        public Func<Unit, Task<int>> Count(ValueTask<A> fa) =>
            async _ =>
            {
                try
                {
                    var a = await fa.ConfigureAwait(false);
                    return fa.IsFaulted || fa.IsCanceled
                        ? 0
                        : 1;
                }
                catch
                {
                    return 0;
                }
            };

        [Pure]
        public async ValueTask<A> Apply(Func<A, A, A> f, ValueTask<A> fa, ValueTask<A> fb) 
        {
            await Task.WhenAll(fa.AsTask(), fb.AsTask()).ConfigureAwait(false);
            return f(fa.Result, fb.Result);
        }

        public async Task<B> MatchAsync<B>(ValueTask<A> ma, Func<A, Task<B>> SomeAsync, Func<B> None)
        {
            if (ma.IsCanceled || ma.IsFaulted)
            {
                return Check.NullReturn(None());
            }
            try
            {
                var a = await ma.ConfigureAwait(false);
                return Check.NullReturn(await SomeAsync(a).ConfigureAwait(false));
            }
            catch (Exception)
            {
                return Check.NullReturn(None());
            }
        }

        public async Task<B> MatchAsync<B>(ValueTask<A> ma, Func<A, B> Some, Func<Task<B>> NoneAsync)
        {
            if (ma.IsCanceled || ma.IsFaulted)
            {
                return Check.NullReturn(await NoneAsync().ConfigureAwait(false));
            }
            try
            {
                var a = await ma.ConfigureAwait(false);
                return Check.NullReturn(Some(a));
            }
            catch (Exception)
            {
                return Check.NullReturn(await NoneAsync().ConfigureAwait(false));
            }
        }

        public async Task<B> MatchAsync<B>(ValueTask<A> ma, Func<A, Task<B>> SomeAsync, Func<Task<B>> NoneAsync)
        {
            if (ma.IsCanceled || ma.IsFaulted)
            {
                return Check.NullReturn(await NoneAsync().ConfigureAwait(false));
            }
            try
            {
                var a = await ma.ConfigureAwait(false);
                return Check.NullReturn(await SomeAsync(a).ConfigureAwait(false));
            }
            catch (Exception)
            {
                return Check.NullReturn(await NoneAsync().ConfigureAwait(false));
            }
        }

        public async Task<Unit> Match(ValueTask<A> ma, Action<A> Some, Action None)
        {
            try
            {
                var a = await ma.ConfigureAwait(false);
                Some(a);
            }
            catch (Exception)
            {
                None();
            }
            return unit;
        }

        public async Task<Unit> MatchAsync(ValueTask<A> ma, Func<A, Task> SomeAsync, Action None)
        {
            try
            {
                var a = await ma.ConfigureAwait(false);
                await SomeAsync(a).ConfigureAwait(false);
            }
            catch (Exception)
            {
                None();
            }
            return unit;
        }

        public async Task<Unit> MatchAsync(ValueTask<A> ma, Action<A> Some, Func<Task> None)
        {
            try
            {
                var a = await ma.ConfigureAwait(false);
                Some(a);
            }
            catch (Exception)
            {
                await None().ConfigureAwait(false);
            }
            return unit;
        }

        public async Task<Unit> MatchAsync(ValueTask<A> ma, Func<A, Task> SomeAsync, Func<Task> NoneAsync)
        {
            try
            {
                var a = await ma.ConfigureAwait(false);
                await SomeAsync(a).ConfigureAwait(false);
            }
            catch (Exception)
            {
                await NoneAsync().ConfigureAwait(false);
            }
            return unit;
        }

        public ValueTask<A> SomeAsync(Task<A> value) =>
            value.ToValue();

        public ValueTask<A> OptionalAsync(Task<A> value) =>
            value.ToValue();

        public Task<S> BiFold<S>(ValueTask<A> ma, S state, Func<S, A, S> Some, Func<S, Unit, S> None) =>
            Match(ma,
                Some: x  => Some(state, x),
                None: () => None(state, unit));

        public Task<S> BiFoldAsync<S>(ValueTask<A> ma, S state, Func<S, A, Task<S>> SomeAsync, Func<S, Unit, S> None) =>
            MatchAsync(ma,
                SomeAsync: x => SomeAsync(state, x),
                None: () => None(state, unit));

        public Task<S> BiFoldAsync<S>(ValueTask<A> ma, S state, Func<S, A, S> Some, Func<S, Unit, Task<S>> NoneAsync) =>
            MatchAsync(ma,
                Some: x => Some(state, x),
                NoneAsync: () => NoneAsync(state, unit));

        public Task<S> BiFoldAsync<S>(ValueTask<A> ma, S state, Func<S, A, Task<S>> SomeAsync, Func<S, Unit, Task<S>> NoneAsync) =>
            MatchAsync(ma,
                SomeAsync: x => SomeAsync(state, x),
                NoneAsync: () => NoneAsync(state, unit));

        public Task<S> BiFoldBack<S>(ValueTask<A> ma, S state, Func<S, A, S> Some, Func<S, Unit, S> None) =>
            Match(ma,
                Some: x => Some(state, x),
                None: () => None(state, unit));

        public Task<S> BiFoldBackAsync<S>(ValueTask<A> ma, S state, Func<S, A, Task<S>> SomeAsync, Func<S, Unit, S> None) =>
            MatchAsync(ma,
                SomeAsync: x => SomeAsync(state, x),
                None: () => None(state, unit));

        public Task<S> BiFoldBackAsync<S>(ValueTask<A> ma, S state, Func<S, A, S> Some, Func<S, Unit, Task<S>> NoneAsync) =>
            MatchAsync(ma,
                Some: x => Some(state, x),
                NoneAsync: () => NoneAsync(state, unit));

        public Task<S> BiFoldBackAsync<S>(ValueTask<A> ma, S state, Func<S, A, Task<S>> SomeAsync, Func<S, Unit, Task<S>> NoneAsync) =>
            MatchAsync(ma,
                SomeAsync: x => SomeAsync(state, x),
                NoneAsync: () => NoneAsync(state, unit));

        public async Task<B> MatchUnsafe<B>(ValueTask<A> ma, Func<A, B> Some, Func<B> None)
        {
            if (ma.IsCanceled || ma.IsFaulted)
            {
                return None();
            }
            try
            {
                var a = await ma.ConfigureAwait(false);
                return Some(a);
            }
            catch (Exception)
            {
                return None();
            }
        }

        public async Task<B> MatchUnsafeAsync<B>(ValueTask<A> ma, Func<A, Task<B>> SomeAsync, Func<B> None)
        {
            if (ma.IsCanceled || ma.IsFaulted)
            {
                return None();
            }
            try
            {
                var a = await ma.ConfigureAwait(false);
                return await SomeAsync(a).ConfigureAwait(false);
            }
            catch (Exception)
            {
                return None();
            }
        }

        public async Task<B> MatchUnsafeAsync<B>(ValueTask<A> ma, Func<A, B> Some, Func<Task<B>> NoneAsync)
        {
            if (ma.IsCanceled || ma.IsFaulted)
            {
                return await NoneAsync().ConfigureAwait(false);
            }
            try
            {
                var a = await ma.ConfigureAwait(false);
                return Some(a);
            }
            catch (Exception)
            {
                return await NoneAsync().ConfigureAwait(false);
            }
        }

        public async Task<B> MatchUnsafeAsync<B>(ValueTask<A> ma, Func<A, Task<B>> SomeAsync, Func<Task<B>> NoneAsync)
        {
            if (ma.IsCanceled || ma.IsFaulted)
            {
                return await NoneAsync().ConfigureAwait(false);
            }
            try
            {
                var a = await ma.ConfigureAwait(false);
                return await SomeAsync(a).ConfigureAwait(false);
            }
            catch (Exception)
            {
                return await NoneAsync().ConfigureAwait(false);
            }
        }
    }
}
