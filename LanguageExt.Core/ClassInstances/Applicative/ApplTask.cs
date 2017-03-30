using System;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances
{
    public struct ApplTask<A, B> : 
        Functor<Task<A>, Task<B>, A, B>,
        BiFunctor<Task<A>, Task<B>, A, Unit, B>,
        Applicative<Task<Func<A, B>>, Task<A>, Task<B>, A, B>
    {
        public static readonly ApplTask<A, B> Inst = default(ApplTask<A, B>);

        [Pure]
        public Task<B> BiMap(Task<A> ma, Func<A, B> fa, Func<Unit, B> fb) =>
            FTask<A, B>.Inst.BiMap(ma, fa, fb);

        [Pure]
        public Task<B> Map(Task<A> ma, Func<A, B> f) =>
            FTask<A, B>.Inst.Map(ma, f);

        [Pure]
        public async Task<B> Apply(Task<Func<A, B>> fab, Task<A> fa)
        {
            await Task.WhenAll(fab, fa);

            if (fab.IsFaulted) throw fab.Exception;
            if (fa.IsFaulted) throw fa.Exception;

            return fab.Result(fa.Result);
        }

        [Pure]
        public Task<A> Pure(A x) =>
            MTask<A>.Inst.Return(x);

        [Pure]
        public async Task<B> Action(Task<A> fa, Task<B> fb)
        {
            await Task.WhenAll(fa, fb);

            if (fa.IsFaulted) throw fa.Exception;
            if (fb.IsFaulted) throw fb.Exception;

            return fb.Result;
        }
    }

    public struct ApplTask<A, B, C> :
        Applicative<Task<Func<A, Func<B, C>>>, Task<Func<B, C>>, Task<A>, Task<B>, Task<C>, A, B, C>
    {
        public static readonly ApplTask<A, B, C> Inst = default(ApplTask<A, B, C>);

        [Pure]
        public async Task<Func<B, C>> Apply(Task<Func<A, Func<B, C>>> fabc, Task<A> fa)
        {
            await Task.WhenAll(fabc, fa);

            if (fabc.IsFaulted) throw fabc.Exception;
            if (fa.IsFaulted) throw fa.Exception;

            return fabc.Result(fa.Result);
        }

        [Pure]
        public async Task<C> Apply(Task<Func<A, Func<B, C>>> fabc, Task<A> fa, Task<B> fb)
        {
            await Task.WhenAll(fabc, fa, fb);

            if (fabc.IsFaulted) throw fabc.Exception;
            if (fa.IsFaulted) throw fa.Exception;
            if (fb.IsFaulted) throw fb.Exception;

            return fabc.Result(fa.Result)(fb.Result);
        }

        [Pure]
        public Task<A> Pure(A x) =>
            MTask<A>.Inst.Return(x);
    }


    public struct ApplTask<A> :
        Functor<Task<A>, Task<A>, A, A>,
        BiFunctor<Task<A>, Task<A>, A, Unit, A>,
        Applicative<Task<Func<A, A>>, Task<A>, Task<A>, A, A>,
        Applicative<Task<Func<A, Func<A, A>>>, Task<Func<A, A>>, Task<A>, Task<A>, Task<A>, A, A, A>
    {
        public static readonly ApplTask<A> Inst = default(ApplTask<A>);

        [Pure]
        public Task<A> BiMap(Task<A> ma, Func<A, A> fa, Func<Unit, A> fb) =>
            FOptional<MTask<A>, MTask<A>, Task<A>, Task<A>, A, A>.Inst.BiMap(ma, fa, fb);

        [Pure]
        public Task<A> Map(Task<A> ma, Func<A, A> f) =>
            FOptional<MTask<A>, MTask<A>, Task<A>, Task<A>, A, A>.Inst.Map(ma, f);

        [Pure]
        public async Task<A> Apply(Task<Func<A, A>> fab, Task<A> fa)
        {
            await Task.WhenAll(fab, fa);

            if (fab.IsFaulted) throw fab.Exception;
            if (fa.IsFaulted) throw fa.Exception;

            return fab.Result(fa.Result);
        }

        [Pure]
        public Task<A> Pure(A x) =>
            MTask<A>.Inst.Return(x);

        [Pure]
        public async Task<A> Action(Task<A> fa, Task<A> fb)
        {
            await Task.WhenAll(fa, fb);

            if (fa.IsFaulted) throw fa.Exception;
            if (fb.IsFaulted) throw fb.Exception;

            return fb.Result;
        }

        [Pure]
        public async Task<Func<A, A>> Apply(Task<Func<A, Func<A, A>>> fabc, Task<A> fa)
        {
            await Task.WhenAll(fabc, fa);

            if (fabc.IsFaulted) throw fabc.Exception;
            if (fa.IsFaulted) throw fa.Exception;

            return fabc.Result(fa.Result);
        }

        [Pure]
        public async Task<A> Apply(Task<Func<A, Func<A, A>>> fabc, Task<A> fa, Task<A> fb)
        {
            await Task.WhenAll(fabc, fa, fb);

            if (fabc.IsFaulted) throw fabc.Exception;
            if (fa.IsFaulted) throw fa.Exception;
            if (fb.IsFaulted) throw fb.Exception;

            return fabc.Result(fa.Result)(fb.Result);
        }
    }
}
