﻿using System;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances
{
    public struct ApplTask<A, B> : 
        FunctorAsync<Task<A>, Task<B>, A, B>,
        BiFunctorAsync<Task<A>, Task<B>, A, Unit, B>,
        ApplicativeAsync<Task<Func<A, B>>, Task<A>, Task<B>, A, B>
    {
        public static readonly ApplTask<A, B> Inst = default(ApplTask<A, B>);

        [Pure]
        public Task<B> BiMapAsync(Task<A> ma, Func<A, B> fa, Func<Unit, B> fb) =>
            FTask<A, B>.Inst.BiMapAsync(ma, fa, fb);

        [Pure]
        public Task<B> BiMapAsync(Task<A> ma, Func<A, Task<B>> fa, Func<Unit, B> fb) =>
            FTask<A, B>.Inst.BiMapAsync(ma, fa, fb);

        [Pure]
        public Task<B> BiMapAsync(Task<A> ma, Func<A, B> fa, Func<Unit, Task<B>> fb) =>
            FTask<A, B>.Inst.BiMapAsync(ma, fa, fb);

        [Pure]
        public Task<B> BiMapAsync(Task<A> ma, Func<A, Task<B>> fa, Func<Unit, Task<B>> fb) =>
            FTask<A, B>.Inst.BiMapAsync(ma, fa, fb);

        [Pure]
        public Task<B> Map(Task<A> ma, Func<A, B> f) =>
            FTask<A, B>.Inst.Map(ma, f);

        [Pure]
        public Task<B> MapAsync(Task<A> ma, Func<A, Task<B>> f) =>
            FTask<A, B>.Inst.MapAsync(ma, f);

        [Pure]
        public async Task<B> Apply(Task<Func<A, B>> fab, Task<A> fa)
        {
            await Task.WhenAll(fab, fa);

            if (fab.IsFaulted) throw fab.Exception;
            if (fa.IsFaulted) throw fa.Exception;

            return fab.Result(fa.Result);
        }

        [Pure]
        public Task<A> PureAsync(Task<A> x) =>
            MTask<A>.Inst.ReturnAsync(x);

        [Pure]
        public async Task<B> Action(Task<A> fa, Task<B> fb)
        {
            await Task.WhenAll(fa, fb);

            if (fa.IsFaulted) throw fa.Exception;
            if (fb.IsFaulted) throw fb.Exception;

            return await fb;
        }
    }

    public struct ApplTask<A, B, C> :
        ApplicativeAsync<Task<Func<A, Func<B, C>>>, Task<Func<B, C>>, Task<A>, Task<B>, Task<C>, A, B, C>
    {
        public static readonly ApplTask<A, B, C> Inst = default(ApplTask<A, B, C>);

        [Pure]
        public async Task<Func<B, C>> Apply(Task<Func<A, Func<B, C>>> fabc, Task<A> fa)
        {
            await Task.WhenAll(fabc, fa);

            if (fabc.IsFaulted) throw fabc.Exception;
            if (fa.IsFaulted) throw fa.Exception;

            return (await fabc)(await fa);
        }

        [Pure]
        public async Task<C> Apply(Task<Func<A, Func<B, C>>> fabc, Task<A> fa, Task<B> fb)
        {
            await Task.WhenAll(fabc, fa, fb);

            if (fabc.IsFaulted) throw fabc.Exception;
            if (fa.IsFaulted) throw fa.Exception;
            if (fb.IsFaulted) throw fb.Exception;

            return (await fabc)(await fa)(await fb);
        }

        [Pure]
        public Task<A> PureAsync(Task<A> x) =>
            MTask<A>.Inst.ReturnAsync(x);
    }


    public struct ApplTask<A> :
        FunctorAsync<Task<A>, Task<A>, A, A>,
        BiFunctorAsync<Task<A>, Task<A>, A, Unit, A>,
        ApplicativeAsync<Task<Func<A, A>>, Task<A>, Task<A>, A, A>,
        ApplicativeAsync<Task<Func<A, Func<A, A>>>, Task<Func<A, A>>, Task<A>, Task<A>, Task<A>, A, A, A>
    {
        public static readonly ApplTask<A> Inst = default(ApplTask<A>);

        [Pure]
        public Task<A> BiMapAsync(Task<A> ma, Func<A, A> fa, Func<Unit, A> fb) =>
            default(FTask<A, A>).BiMapAsync(ma, fa, fb);

        [Pure]
        public Task<A> BiMapAsync(Task<A> ma, Func<A, Task<A>> fa, Func<Unit, A> fb) =>
            default(FTask<A, A>).BiMapAsync(ma, fa, fb);

        [Pure]
        public Task<A> BiMapAsync(Task<A> ma, Func<A, A> fa, Func<Unit, Task<A>> fb) =>
            default(FTask<A, A>).BiMapAsync(ma, fa, fb);

        [Pure]
        public Task<A> BiMapAsync(Task<A> ma, Func<A, Task<A>> fa, Func<Unit, Task<A>> fb) =>
            default(FTask<A, A>).BiMapAsync(ma, fa, fb);

        [Pure]
        public Task<A> Map(Task<A> ma, Func<A, A> f) =>
            default(FTask<A, A>).Map(ma, f);

        [Pure]
        public Task<A> MapAsync(Task<A> ma, Func<A, Task<A>> f) =>
            default(FTask<A, A>).MapAsync(ma, f);

        [Pure]
        public async Task<A> Apply(Task<Func<A, A>> fab, Task<A> fa)
        {
            await Task.WhenAll(fab, fa);

            if (fab.IsFaulted) throw fab.Exception;
            if (fa.IsFaulted) throw fa.Exception;

            return (await fab)(await fa);
        }

        [Pure]
        public Task<A> PureAsync(Task<A> x) =>
            MTask<A>.Inst.ReturnAsync(x);

        [Pure]
        public async Task<A> Action(Task<A> fa, Task<A> fb)
        {
            await Task.WhenAll(fa, fb);

            if (fa.IsFaulted) throw fa.Exception;
            if (fb.IsFaulted) throw fb.Exception;

            return await fb;
        }

        [Pure]
        public async Task<Func<A, A>> Apply(Task<Func<A, Func<A, A>>> fabc, Task<A> fa)
        {
            await Task.WhenAll(fabc, fa);

            if (fabc.IsFaulted) throw fabc.Exception;
            if (fa.IsFaulted) throw fa.Exception;

            return (await fabc)(await fa);
        }

        [Pure]
        public async Task<A> Apply(Task<Func<A, Func<A, A>>> fabc, Task<A> fa, Task<A> fb)
        {
            await Task.WhenAll(fabc, fa, fb);

            if (fabc.IsFaulted) throw fabc.Exception;
            if (fa.IsFaulted) throw fa.Exception;
            if (fb.IsFaulted) throw fb.Exception;

            return (await fabc)(await fa)(await fb);
        }
    }
}
