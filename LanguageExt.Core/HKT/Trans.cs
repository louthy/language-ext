using LanguageExt.ClassInstances;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System;

namespace LanguageExt
{
    /// <summary>
    /// Default monad transformer, can nest any two monadic types and provide the
    /// correct default behaviour based on their Bind operations.
    /// 
    ///     i.e.
    ///     
    ///         Trans<MLst<Option<int>>, Lst<Option<int>>, MOption<int>, Option<int>, int>
    /// 
    /// </summary>
    public struct Trans<OuterMonad, OuterType, InnerMonad, InnerType, A> : MonadTrans<OuterMonad, OuterType, InnerMonad, InnerType, A>
        where OuterMonad : struct, Monad<OuterType, InnerType>
        where InnerMonad : struct, Monad<InnerType, A>
    {
        public static readonly Trans<OuterMonad, OuterType, InnerMonad, InnerType, A> Inst;

        public NewOuterType Bind<NewOuterMonad, NewOuterType, NewInnerMonad, NewInnerType, B>(OuterType ma, Func<A, NewInnerType> f)
            where NewOuterMonad : struct, Monad<NewOuterType, NewInnerType>
            where NewInnerMonad : struct, Monad<NewInnerType, B> =>
            default(OuterMonad).Bind<NewOuterMonad, NewOuterType, NewInnerType>(ma,
                inner =>
                    default(NewOuterMonad).Return(
                        default(InnerMonad).Bind<NewInnerMonad, NewInnerType, B>(inner, f)));

        public NewOuterType Map<NewOuterMonad, NewOuterType, NewInnerMonad, NewInnerType, B>(OuterType ma, Func<A, B> f)
            where NewOuterMonad : struct, Monad<NewOuterType, NewInnerType>
            where NewInnerMonad : struct, Monad<NewInnerType, B> =>
            default(OuterMonad).Bind<NewOuterMonad, NewOuterType, NewInnerType>(ma,
                inner =>
                    default(NewOuterMonad).Return(
                        default(InnerMonad).Bind<NewInnerMonad, NewInnerType, B>(inner,
                            a => default(NewInnerMonad).Return(f(a)))));

        public S Fold<S>(OuterType ma, S state, Func<S, A, S> f) =>
            default(OuterMonad).Fold(ma, state, (s, inner) =>
                default(InnerMonad).Fold(inner, s, f));

        public S FoldBack<S>(OuterType ma, S state, Func<S, A, S> f) =>
            default(OuterMonad).FoldBack(ma, state, (s, inner) =>
                default(InnerMonad).FoldBack(inner, s, f));

        public int Count(OuterType ma) =>
            default(OuterMonad).Fold(ma, 0, (s, inner) =>
                s + default(InnerMonad).Count(inner));
    }

    public struct Trans<OuterMonad, OuterType, InnerMonad, InnerType, NumA, A>
        where OuterMonad : struct, Monad<OuterType, InnerType>
        where InnerMonad : struct, Monad<InnerType, A>
        where NumA : struct, Num<A>
    {
        public static readonly Trans<OuterMonad, OuterType, InnerMonad, InnerType, NumA, A> Inst;

        public A Sum(OuterType ma) =>
            default(OuterMonad).Fold(ma, default(NumA).FromInteger(0), (s, inner) =>
                default(InnerMonad).Fold(inner, s, (ss, x) => default(NumA).Plus(ss, x)));
    }
}
