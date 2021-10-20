using System;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances
{
    public struct ApplValueTask<A, B> : 
        FunctorAsync<ValueTask<A>, ValueTask<B>, A, B>,
        BiFunctorAsync<ValueTask<A>, ValueTask<B>, A, Unit, B>,
        ApplicativeAsync<ValueTask<Func<A, B>>, ValueTask<A>, ValueTask<B>, A, B>
    {
        public static readonly ApplValueTask<A, B> Inst = default(ApplValueTask<A, B>);

        [Pure]
        public ValueTask<B> BiMapAsync(ValueTask<A> ma, Func<A, B> fa, Func<Unit, B> fb) =>
            FValueTask<A, B>.Inst.BiMapAsync(ma, fa, fb);

        [Pure]
        public ValueTask<B> BiMapAsync(ValueTask<A> ma, Func<A, Task<B>> fa, Func<Unit, B> fb) =>
            FValueTask<A, B>.Inst.BiMapAsync(ma, fa, fb);

        [Pure]
        public ValueTask<B> BiMapAsync(ValueTask<A> ma, Func<A, B> fa, Func<Unit, Task<B>> fb) =>
            FValueTask<A, B>.Inst.BiMapAsync(ma, fa, fb);

        [Pure]
        public ValueTask<B> BiMapAsync(ValueTask<A> ma, Func<A, Task<B>> fa, Func<Unit, Task<B>> fb) =>
            FValueTask<A, B>.Inst.BiMapAsync(ma, fa, fb);

        [Pure]
        public ValueTask<B> Map(ValueTask<A> ma, Func<A, B> f) =>
            FValueTask<A, B>.Inst.Map(ma, f);

        [Pure]
        public ValueTask<B> MapAsync(ValueTask<A> ma, Func<A, Task<B>> f) =>
            FValueTask<A, B>.Inst.MapAsync(ma, f);

        [Pure]
        public async ValueTask<B> Apply(ValueTask<Func<A, B>> fab, ValueTask<A> fa)
        {
            await Task.WhenAll(fab.AsTask(), fa.AsTask()).ConfigureAwait(false);
            return fab.Result(fa.Result);
        }

        [Pure]
        public async ValueTask<B> Apply(ValueTask<Func<A, A, B>> fab, ValueTask<A> fa, ValueTask<A> fb)
        {
            await Task.WhenAll(fab.AsTask(), fa.AsTask(), fb.AsTask()).ConfigureAwait(false);
            return fab.Result(fa.Result, fb.Result);
        }

        [Pure]
        public async ValueTask<B> Apply(Func<A, A, B> fab, ValueTask<A> fa, ValueTask<A> fb)
        {
            await Task.WhenAll(fa.AsTask(), fb.AsTask()).ConfigureAwait(false);
            return fab(fa.Result, fb.Result);
        }

        [Pure]
        public ValueTask<A> PureAsync(Task<A> x) =>
            x.ToValue();

        [Pure]
        public async ValueTask<B> Action(ValueTask<A> fa, ValueTask<B> fb)
        {
            await Task.WhenAll(fa.AsTask(), fb.AsTask()).ConfigureAwait(false);
            return await fb.ConfigureAwait(false);
        }
    }

    public struct ApplValueTask<A, B, C> :
        ApplicativeAsync<ValueTask<Func<A, Func<B, C>>>, ValueTask<Func<B, C>>, ValueTask<A>, ValueTask<B>, ValueTask<C>, A, B, C>
    {
        public static readonly ApplValueTask<A, B, C> Inst = default(ApplValueTask<A, B, C>);

        [Pure]
        public async ValueTask<Func<B, C>> Apply(ValueTask<Func<A, Func<B, C>>> fabc, ValueTask<A> fa)
        {
            await Task.WhenAll(fabc.AsTask(), fa.AsTask()).ConfigureAwait(false);
            return (await fabc.ConfigureAwait(false))(await fa.ConfigureAwait(false));
        }

        [Pure]
        public async ValueTask<C> Apply(ValueTask<Func<A, Func<B, C>>> fabc, ValueTask<A> fa, ValueTask<B> fb)
        {
            await Task.WhenAll(fabc.AsTask(), fa.AsTask(), fb.AsTask()).ConfigureAwait(false);
            return (await fabc.ConfigureAwait(false))(await fa.ConfigureAwait(false))(await fb.ConfigureAwait(false));
        }

        [Pure]
        public ValueTask<A> PureAsync(Task<A> x) =>
            x.ToValue();
    }


    public struct ApplValueTask<A> :
        FunctorAsync<ValueTask<A>, ValueTask<A>, A, A>,
        BiFunctorAsync<ValueTask<A>, ValueTask<A>, A, Unit, A>,
        ApplicativeAsync<ValueTask<Func<A, A>>, ValueTask<A>, ValueTask<A>, A, A>,
        ApplicativeAsync<ValueTask<Func<A, Func<A, A>>>, ValueTask<Func<A, A>>, ValueTask<A>, ValueTask<A>, ValueTask<A>, A, A, A>
    {
        public static readonly ApplValueTask<A> Inst = default(ApplValueTask<A>);

        [Pure]
        public ValueTask<A> BiMapAsync(ValueTask<A> ma, Func<A, A> fa, Func<Unit, A> fb) =>
            default(FValueTask<A, A>).BiMapAsync(ma, fa, fb);

        [Pure]
        public ValueTask<A> BiMapAsync(ValueTask<A> ma, Func<A, Task<A>> fa, Func<Unit, A> fb) =>
            default(FValueTask<A, A>).BiMapAsync(ma, fa, fb);

        [Pure]
        public ValueTask<A> BiMapAsync(ValueTask<A> ma, Func<A, A> fa, Func<Unit, Task<A>> fb) =>
            default(FValueTask<A, A>).BiMapAsync(ma, fa, fb);

        [Pure]
        public ValueTask<A> BiMapAsync(ValueTask<A> ma, Func<A, Task<A>> fa, Func<Unit, Task<A>> fb) =>
            default(FValueTask<A, A>).BiMapAsync(ma, fa, fb);

        [Pure]
        public ValueTask<A> Map(ValueTask<A> ma, Func<A, A> f) =>
            default(FValueTask<A, A>).Map(ma, f);

        [Pure]
        public ValueTask<A> MapAsync(ValueTask<A> ma, Func<A, Task<A>> f) =>
            default(FValueTask<A, A>).MapAsync(ma, f);

        [Pure]
        public async ValueTask<A> Apply(ValueTask<Func<A, A>> fab, ValueTask<A> fa)
        {
            await Task.WhenAll(fab.AsTask(), fa.AsTask()).ConfigureAwait(false);
            return (await fab.ConfigureAwait(false))(await fa.ConfigureAwait(false));
        }

        [Pure]
        public ValueTask<A> PureAsync(Task<A> x) =>
            x.ToValue();

        [Pure]
        public async ValueTask<A> Action(ValueTask<A> fa, ValueTask<A> fb)
        {
            await Task.WhenAll(fa.AsTask(), fb.AsTask()).ConfigureAwait(false);
            return await fb.ConfigureAwait(false);
        }

        [Pure]
        public async ValueTask<Func<A, A>> Apply(ValueTask<Func<A, Func<A, A>>> fabc, ValueTask<A> fa)
        {
            await Task.WhenAll(fabc.AsTask(), fa.AsTask()).ConfigureAwait(false);
            return (await fabc.ConfigureAwait(false))(await fa.ConfigureAwait(false));
        }

        [Pure]
        public async ValueTask<A> Apply(ValueTask<Func<A, Func<A, A>>> fabc, ValueTask<A> fa, ValueTask<A> fb)
        {
            await Task.WhenAll(fabc.AsTask(), fa.AsTask(), fb.AsTask()).ConfigureAwait(false);
            return (await fabc.ConfigureAwait(false))(await fa.ConfigureAwait(false))(await fb.ConfigureAwait(false));
        }
    }
}
