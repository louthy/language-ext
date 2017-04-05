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
        Monad<OptionAsync<A>, A>,
        BiFoldable<OptionAsync<A>, A, Unit>,
        BiFoldableAsync<OptionAsync<A>, A, Unit>
    {
        public static readonly MOptionAsync<A> Inst = default(MOptionAsync<A>);

        [Pure]
        public OptionAsync<A> NoneAsync => OptionAsync<A>.None;

        [Pure]
        public MB Bind<MonadB, MB, B>(OptionAsync<A> ma, Func<A, MB> f) where MonadB : struct, Monad<Unit, Unit, MB, B> =>
            ma.IsLazy
                ? default(MonadB).Id(_ =>
                  {
                      return default(MonadB).IdAsync(async __ =>
                      {
                          if (await ma.IsSome)
                          {
                              return f(await ma.Value);
                          }
                          else
                          {
                              return default(MonadB).Fail();
                          }
                      });
                  })
                : default(MonadB).IdAsync(async _ =>
                  {
                      if(await ma.IsSome)
                      {
                          return f(await ma.Value);
                      }
                      else
                      {
                          return default(MonadB).Fail();
                      }
                  });

        [Pure]
        public OptionAsync<A> Fail(object err) =>
            OptionAsync<A>.None;

        [Pure]
        public OptionAsync<A> Fail(Exception err = null) =>
            OptionAsync<A>.None;

        [Pure]
        public OptionAsync<A> Plus(OptionAsync<A> a, OptionAsync<A> b) =>
            a.IsLazy
                ? default(MOptionAsync<A>).Id(_ =>
                    default(MOptionAsync<A>).IdAsync(async __ => 
                        await a.IsSome
                            ? a
                            : b))
                : default(MOptionAsync<A>).IdAsync(async __ =>
                        await a.IsSome
                            ? a
                            : b);

        [Pure]
        public OptionAsync<A> Return(Func<Unit, A> f) =>
            new OptionAsync<A>(OptionDataAsync.Lazy(() =>
                Task.Run(() =>
                {
                    var a = f(unit);
                    return a.IsNull()
                        ? new Result<A>()
                        : new Result<A>(a);
                })));

        [Pure]
        public OptionAsync<A> Zero() =>
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
        public Func<Unit, S> Fold<S>(OptionAsync<A> ma, S state, Func<S, A, S> f) => _ =>
            default(MOptionAsync<A>).FoldAsync(ma, state, f)(_).Result;

        [Pure]
        public Func<Unit, S> FoldBack<S>(OptionAsync<A> ma, S state, Func<S, A, S> f) => _ =>
            default(MOptionAsync<A>).FoldBackAsync(ma, state, f)(_).Result;

        [Pure]
        public S BiFold<S>(OptionAsync<A> ma, S state, Func<S, A, S> fa, Func<S, Unit, S> fb) =>
            BiFoldAsync(ma, state, fa, fb).Result;

        [Pure]
        public S BiFoldBack<S>(OptionAsync<A> ma, S state, Func<S, A, S> fa, Func<S, Unit, S> fb) =>
            BiFoldBackAsync(ma, state, fa, fb).Result;

        [Pure]
        public Func<Unit, int> Count(OptionAsync<A> ma) => _ =>
            default(MOptionAsync<A>).CountAsync(ma)(_).Result;

        [Pure]
        public OptionAsync<A> SomeAsync(A x) =>
            x.IsNull()
                ? throw new ArgumentNullException("Option doesn't support null values.  Use OptionUnsafe if this is desired behaviour")
                : new OptionAsync<A>(OptionDataAsync.Some(x));

        [Pure]
        public OptionAsync<A> OptionalAsync(A x) =>
            new OptionAsync<A>(OptionDataAsync.Optional(x));

        [Pure]
        public OptionAsync<A> Id(Func<Unit, OptionAsync<A>> ma) =>
            new OptionAsync<A>(OptionDataAsync.Lazy(async () =>
            {
                var a = ma(unit);
                return await a.IsSome
                    ? new Result<A>(await a.Value)
                    : Result<A>.None;
            }));

        [Pure]
        public OptionAsync<A> BindReturn(Unit _, OptionAsync<A> mb) =>
            mb;

        [Pure]
        public OptionAsync<A> Return(A x) =>
            OptionalAsync(x);

        [Pure]
        public OptionAsync<A> IdAsync(Func<Unit, Task<OptionAsync<A>>> ma) =>
            new OptionAsync<A>(OptionDataAsync.Lazy(async () =>
            {
                var a = await ma(unit);
                return await a.IsSome
                    ? new Result<A>(await a.Value)
                    : Result<A>.None;
            }));

        [Pure]
        public OptionAsync<A> Empty() =>
            None;

        [Pure]
        public OptionAsync<A> Append(OptionAsync<A> x, OptionAsync<A> y) =>
            Plus(x, y);

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
    }
}
