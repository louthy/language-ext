using LanguageExt.ClassInstances;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System;
using System.Threading.Tasks;

namespace LanguageExt
{
    /// <summary>
    /// Default monad transformer, can nest any two monadic types and provide the
    /// correct default behaviour based on their Bind operations.
    /// 
    ///     i.e.
    ///     
    ///         TransAsync<MTask<TryAsync<int>>, Lst<Task<TryAsync>>, MTryAsync<int>, TryAsync<int>, int>
    /// 
    /// </summary>
    public struct TransAsync<OuterMonad, OuterType, InnerMonad, InnerType, A> : 
        MonadTransAsync<OuterMonad, OuterType, InnerMonad, InnerType, A>
            where OuterMonad : struct, MonadAsync<OuterType, InnerType>
            where InnerMonad : struct, MonadAsync<InnerType, A>
    {
        static readonly OuterMonad MOuter = new OuterMonad();
        static readonly InnerMonad MInner = new InnerMonad();

        public static readonly TransAsync<OuterMonad, OuterType, InnerMonad, InnerType, A> Inst;

        public NewOuterType BindAsync<NewOuterMonad, NewOuterType, NewInnerMonad, NewInnerType, B>(OuterType ma, Func<A, NewInnerType> f)
            where NewOuterMonad : struct, MonadAsync<NewOuterType, NewInnerType>
            where NewInnerMonad : struct, MonadAsync<NewInnerType, B> =>
                MOuter.BindAsync<NewOuterMonad, NewOuterType, NewInnerType>(ma, inner =>
                    default(NewOuterMonad).ReturnAsync(
                        MInner.BindAsync<NewInnerMonad, NewInnerType, B>(inner, f).AsTask()));

        public NewOuterType BindAsync<NewOuterMonad, NewOuterType, NewInnerMonad, NewInnerType, B>(OuterType ma, Func<A, Task<NewInnerType>> f)
            where NewOuterMonad : struct, MonadAsync<NewOuterType, NewInnerType>
            where NewInnerMonad : struct, MonadAsync<NewInnerType, B> =>
                MOuter.BindAsync<NewOuterMonad, NewOuterType, NewInnerType>(ma, inner =>
                    default(NewOuterMonad).ReturnAsync(
                        MInner.BindAsync<NewInnerMonad, NewInnerType, B>(inner, f).AsTask()));

        public NewOuterType BindAsync<NewOuterMonad, NewOuterType, NewInnerMonad, NewInnerType, B>(OuterType ma, Func<A, NewOuterType> f)
            where NewOuterMonad : struct, MonadAsync<NewOuterType, NewInnerType>
            where NewInnerMonad : struct, MonadAsync<NewInnerType, B> =>
                MOuter.BindAsync<NewOuterMonad, NewOuterType, NewInnerType>(ma, inner =>
                    MInner.BindAsync<NewOuterMonad, NewOuterType, NewInnerType>(inner, a =>
                        f(a).AsTask()));

        public NewOuterType BindAsync<NewOuterMonad, NewOuterType, NewInnerMonad, NewInnerType, B>(OuterType ma, Func<A, Task<NewOuterType>> f)
            where NewOuterMonad : struct, MonadAsync<NewOuterType, NewInnerType>
            where NewInnerMonad : struct, MonadAsync<NewInnerType, B> =>
                MOuter.BindAsync<NewOuterMonad, NewOuterType, NewInnerType>(ma, inner =>
                    MInner.BindAsync<NewOuterMonad, NewOuterType, NewInnerType>(inner, a =>
                        f(a)));

        public NewOuterType MapAsync<NewOuterMonad, NewOuterType, NewInnerMonad, NewInnerType, B>(OuterType ma, Func<A, B> f)
            where NewOuterMonad : struct, MonadAsync<NewOuterType, NewInnerType>
            where NewInnerMonad : struct, MonadAsync<NewInnerType, B> =>
                MOuter.BindAsync<NewOuterMonad, NewOuterType, NewInnerType>(ma,
                    inner =>
                        default(NewOuterMonad).ReturnAsync(
                            MInner.BindAsync<NewInnerMonad, NewInnerType, B>(inner,
                                a => default(NewInnerMonad).ReturnAsync(f(a).AsTask())).AsTask()));

        public NewOuterType MapAsync<NewOuterMonad, NewOuterType, NewInnerMonad, NewInnerType, B>(OuterType ma, Func<A, Task<B>> f)
            where NewOuterMonad : struct, MonadAsync<NewOuterType, NewInnerType>
            where NewInnerMonad : struct, MonadAsync<NewInnerType, B> =>
                MOuter.BindAsync<NewOuterMonad, NewOuterType, NewInnerType>(ma,
                    inner =>
                        default(NewOuterMonad).ReturnAsync(
                            MInner.BindAsync<NewInnerMonad, NewInnerType, B>(inner,
                                a => default(NewInnerMonad).ReturnAsync(f(a))).AsTask()));

        public Task<S> FoldAsync<S>(OuterType ma, S state, Func<S, A, S> f) =>
            MOuter.FoldAsync(ma, state, async (s, inner) =>
                await MInner.FoldAsync(inner, s, f)(unit))(unit);

        public Task<S> FoldBackAsync<S>(OuterType ma, S state, Func<S, A, S> f) =>
            MOuter.FoldBackAsync(ma, state, async (s, inner) =>
                await MInner.FoldBackAsync(inner, s, f)(unit))(unit);

        public Task<S> FoldAsync<S>(OuterType ma, S state, Func<S, A, Task<S>> f) =>
            MOuter.FoldAsync(ma, state, async (s, inner) =>
                await MInner.FoldAsync(inner, s, f)(unit))(unit);

        public Task<S> FoldBackAsync<S>(OuterType ma, S state, Func<S, A, Task<S>> f) =>
            MOuter.FoldBackAsync(ma, state, async (s, inner) =>
                await MInner.FoldBackAsync(inner, s, f)(unit))(unit);

        public Task<int> CountAsync(OuterType ma) =>
            MOuter.FoldAsync(ma, 0, async (s, inner) =>
                s + (await MInner.CountAsync(inner)(unit)))(unit);

        public NewOuterType SequenceAsync<NewOuterMonad, NewOuterType, NewInnerMonad, NewInnerType>(OuterType ma)
            where NewOuterMonad : struct, MonadAsync<NewOuterType, NewInnerType>
            where NewInnerMonad : struct, MonadAsync<NewInnerType, A> =>
                TraverseAsync<NewOuterMonad, NewOuterType, NewInnerMonad, NewInnerType, A>(ma, a => a);

        public NewOuterType TraverseAsync<NewOuterMonad, NewOuterType, NewInnerMonad, NewInnerType, B>(OuterType ma, Func<A, B> f)
            where NewOuterMonad : struct, MonadAsync<NewOuterType, NewInnerType>
            where NewInnerMonad : struct, MonadAsync<NewInnerType, B> =>
                default(NewOuterMonad).RunAsync( _ =>
                    MOuter.FoldAsync(ma, default(NewOuterMonad).ReturnAsync(default(NewInnerMonad).ZeroAsync().AsTask()), (outerState, innerA) =>
                        TransAsync<NewOuterMonad, NewOuterType, NewInnerMonad, NewInnerType, B>.Inst.PlusAsync(outerState,
                            MInner.BindAsync<NewOuterMonad, NewOuterType, NewInnerType>(innerA, a =>
                                default(NewOuterMonad).ReturnAsync(default(NewInnerMonad).ReturnAsync(f(a).AsTask()).AsTask()))))(unit));

        public NewOuterType TraverseAsync<NewOuterMonad, NewOuterType, NewInnerMonad, NewInnerType, B>(OuterType ma, Func<A, Task<B>> f)
            where NewOuterMonad : struct, MonadAsync<NewOuterType, NewInnerType>
            where NewInnerMonad : struct, MonadAsync<NewInnerType, B> =>
                default(NewOuterMonad).RunAsync(_ =>
                   MOuter.FoldAsync(ma, default(NewOuterMonad).ReturnAsync(default(NewInnerMonad).ZeroAsync().AsTask()), (outerState, innerA) =>
                       TransAsync<NewOuterMonad, NewOuterType, NewInnerMonad, NewInnerType, B>.Inst.PlusAsync(outerState,
                           MInner.BindAsync<NewOuterMonad, NewOuterType, NewInnerType>(innerA, a =>
                               default(NewOuterMonad).ReturnAsync(default(NewInnerMonad).ReturnAsync(f(a)).AsTask()))))(unit));

        public OuterType PlusAsync(OuterType ma, OuterType mb) =>
            MOuter.ApplyAsync(MInner.PlusAsync, ma, mb);

        public OuterType ZeroAsync() =>
            MOuter.ReturnAsync(MInner.ZeroAsync().AsTask());
    }

    public struct TransAsync<OuterMonad, OuterType, InnerMonad, InnerType, NumA, A>
        where OuterMonad : struct, MonadAsync<OuterType, InnerType>
        where InnerMonad : struct, MonadAsync<InnerType, A>
        where NumA : struct, Num<A>
    {
        public static readonly TransAsync<OuterMonad, OuterType, InnerMonad, InnerType, NumA, A> Inst;

        public Task<A> SumAsync(OuterType ma) =>
            default(OuterMonad).FoldAsync(ma, default(NumA).FromInteger(0), (s, inner) =>
                default(InnerMonad).FoldAsync(inner, s, (ss, x) => 
                    default(NumA).Plus(ss, x))(unit))(unit);
    }
}
