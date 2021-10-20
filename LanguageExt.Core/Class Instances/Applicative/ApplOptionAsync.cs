using System;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances
{
    public struct ApplOptionAsync<A, B> :
        FunctorAsync<OptionAsync<A>, OptionAsync<B>, A, B>,
        BiFunctorAsync<OptionAsync<A>, OptionAsync<B>, A, Unit, B>,
        ApplicativeAsync<OptionAsync<Func<A, B>>, OptionAsync<A>, OptionAsync<B>, A, B>
    {
        public static readonly ApplOptionAsync<A, B> Inst = default(ApplOptionAsync<A, B>);

        [Pure]
        public OptionAsync<B> BiMapAsync(OptionAsync<A> ma, Func<A, B> fa, Func<Unit, B> fb) =>
            default(FOptionAsync<A, B>).BiMapAsync(ma, fa, fb);

        [Pure]
        public OptionAsync<B> BiMapAsync(OptionAsync<A> ma, Func<A, B> fa, Func<Unit, Task<B>> fb) =>
            default(FOptionAsync<A, B>).BiMapAsync(ma, fa, fb);

        [Pure]
        public OptionAsync<B> BiMapAsync(OptionAsync<A> ma, Func<A, Task<B>> fa, Func<Unit, Task<B>> fb) =>
            default(FOptionAsync<A, B>).BiMapAsync(ma, fa, fb);

        [Pure]
        public OptionAsync<B> BiMapAsync(OptionAsync<A> ma, Func<A, Task<B>> fa, Func<Unit, B> fb) =>
            default(FOptionAsync<A, B>).BiMapAsync(ma, fa, fb);

        [Pure]
        public OptionAsync<B> Map(OptionAsync<A> ma, Func<A, B> f) =>
            default(FOptionAsync<A, B>).Map(ma, f);

        [Pure]
        public OptionAsync<B> MapAsync(OptionAsync<A> ma, Func<A, Task<B>> f) =>
            default(FOptionAsync<A, B>).MapAsync(ma, f);

        [Pure]
        public OptionAsync<B> Apply(OptionAsync<Func<A, B>> fab, OptionAsync<A> fa)
        {
            async Task<(bool IsSome, B Value)> Do()
            {
                await Task.WhenAll(fab.Data, fa.Data).ConfigureAwait(false);
                return fab.Data.Result.IsSome && fa.Data.Result.IsSome
                    ? (true, fab.Data.Result.Value(fa.Data.Result.Value))
                    : (false, default);
            }
            return new OptionAsync<B>(Do());
        }

        [Pure]
        public OptionAsync<B> Apply(Func<A, B> fab, OptionAsync<A> fa)
        {
            async Task<(bool IsSome, B Value)> Do()
            {
                var adata = await fa.Data.ConfigureAwait(false);
                return adata.IsSome
                    ? (true, fab(adata.Value))
                    : (false, default);
            }
            return new OptionAsync<B>(Do());
        }

        [Pure]
        public OptionAsync<B> Apply(Func<A, A, B> fab, OptionAsync<A> fa, OptionAsync<A> fb)
        {
            async Task<(bool IsSome, B Value)> Do()
            {
                await Task.WhenAll(fa.Data, fb.Data).ConfigureAwait(false);
                return fa.Data.Result.IsSome && fb.Data.Result.IsSome
                    ? (true, fab(fa.Data.Result.Value, fb.Data.Result.Value))
                    : (false, default);
            }
            return new OptionAsync<B>(Do());
        }

        [Pure]
        public OptionAsync<A> PureAsync(Task<A> x) =>
            MOptionAsync<A>.Inst.ReturnAsync(x);

        [Pure]
        public OptionAsync<B> Action(OptionAsync<A> fa, OptionAsync<B> fb) =>
            from a in fa
            from b in fb
            select b;
    }

    public struct ApplOptionAsync<A, B, C> :
        ApplicativeAsync<OptionAsync<Func<A, Func<B, C>>>, OptionAsync<Func<B, C>>, OptionAsync<A>, OptionAsync<B>, OptionAsync<C>, A, B, C>
    {
        public static readonly ApplOptionAsync<A, B, C> Inst = default(ApplOptionAsync<A, B, C>);

        [Pure]
        public OptionAsync<Func<B, C>> Apply(OptionAsync<Func<A, Func<B, C>>> fab, OptionAsync<A> fa)
        {
            async Task<(bool IsSome, Func<B, C> Value)> Do()
            {
                await Task.WhenAll(fab.Data, fa.Data).ConfigureAwait(false);
                return fab.Data.Result.IsSome && fa.Data.Result.IsSome
                    ? (true, fab.Data.Result.Value(fa.Data.Result.Value))
                    : (false, default);
            }
            return new OptionAsync<Func<B, C>>(Do());
        }

        [Pure]
        public OptionAsync<C> Apply(OptionAsync<Func<A, Func<B, C>>> fab, OptionAsync<A> fa, OptionAsync<B> fb)
        {
            async Task<(bool IsSome, C Value)> Do()
            {
                await Task.WhenAll(fab.Data, fa.Data, fb.Data).ConfigureAwait(false);
                return fab.Data.Result.IsSome && fa.Data.Result.IsSome && fb.Data.Result.IsSome
                    ? (true, fab.Data.Result.Value(fa.Data.Result.Value)(fb.Data.Result.Value))
                    : (false, default);
            }
            return new OptionAsync<C>(Do());
        }


        [Pure]
        public OptionAsync<Func<B, C>> Apply(Func<A, Func<B, C>> fab, OptionAsync<A> fa)
        {
            async Task<(bool IsSome, Func<B, C> Value)> Do()
            {
                var adata = await fa.Data.ConfigureAwait(false);
                return adata.IsSome
                    ? (true, fab(adata.Value))
                    : (false, default);
            }
            return new OptionAsync<Func<B, C>>(Do());
        }

        [Pure]
        public OptionAsync<C> Apply(Func<A, Func<B, C>> fab, OptionAsync<A> fa, OptionAsync<B> fb)
        {
            async Task<(bool IsSome, C Value)> Do()
            {
                await Task.WhenAll(fa.Data, fb.Data).ConfigureAwait(false);
                return fa.Data.Result.IsSome && fb.Data.Result.IsSome
                    ? (true, fab(fa.Data.Result.Value)(fb.Data.Result.Value))
                    : (false, default);
            }
            return new OptionAsync<C>(Do());
        }

        [Pure]
        public OptionAsync<A> PureAsync(Task<A> x) =>
            MOptionAsync<A>.Inst.ReturnAsync(x);
    }
}
