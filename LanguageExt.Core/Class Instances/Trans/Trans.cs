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
    public struct Trans<OuterMonad, OuterType, InnerMonad, InnerType, A> : 
        MonadTrans<OuterType, A>
            where OuterMonad : Monad<OuterType, InnerType>
            where InnerMonad : Monad<InnerType, A>
    {
        public static NewOuterType Bind<NewOuterMonad, NewOuterType, NewInnerMonad, NewInnerType, B>(OuterType ma, Func<A, NewInnerType> f)
            where NewOuterMonad : Monad<NewOuterType, NewInnerType>
            where NewInnerMonad : Monad<NewInnerType, B> =>
                OuterMonad.Bind<NewOuterMonad, NewOuterType, NewInnerType>(ma, inner =>
                    NewOuterMonad.Return(
                        InnerMonad.Bind<NewInnerMonad, NewInnerType, B>(inner, f)));

        public static NewOuterType Bind<NewOuterMonad, NewOuterType, NewInnerType, B>(OuterType ma, Func<A, NewOuterType> f)
            where NewOuterMonad : Monad<NewOuterType, NewInnerType> =>
                OuterMonad.Bind<NewOuterMonad, NewOuterType, NewInnerType>(ma, inner =>
                    InnerMonad.Bind<NewOuterMonad, NewOuterType, NewInnerType>(inner, f));
        
        public static NewOuterType Map<NewOuterMonad, NewOuterType, NewInnerMonad, NewInnerType, B>(OuterType ma, Func<A, B> f)
            where NewOuterMonad : Monad<NewOuterType, NewInnerType>
            where NewInnerMonad : Monad<NewInnerType, B> =>
                OuterMonad.Bind<NewOuterMonad, NewOuterType, NewInnerType>(ma,
                    inner =>
                        NewOuterMonad.Return(
                            InnerMonad.Bind<NewInnerMonad, NewInnerType, B>(inner,
                                a => NewInnerMonad.Return(f(a)))));

        public static S Fold<S>(OuterType ma, S state, Func<S, A, S> f) =>
            OuterMonad.Fold(ma, state, (s, inner) =>
                InnerMonad.Fold(inner, s, f)(unit))(unit);

        public static S FoldBack<S>(OuterType ma, S state, Func<S, A, S> f) =>
            OuterMonad.FoldBack(ma, state, (s, inner) =>
                InnerMonad.FoldBack(inner, s, f)(unit))(unit);

        public static int Count(OuterType ma) =>
            OuterMonad.Fold(ma, 0, (s, inner) =>
                s + InnerMonad.Count(inner)(unit))(unit);

        public static NewOuterType Sequence<NewOuterMonad, NewOuterType, NewInnerMonad, NewInnerType>(OuterType ma)
            where NewOuterMonad : Monad<NewOuterType, NewInnerType>
            where NewInnerMonad : Monad<NewInnerType, A> =>
                Traverse<NewOuterMonad, NewOuterType, NewInnerMonad, NewInnerType, A>(ma, identity);

        public static NewOuterType Traverse<NewOuterMonad, NewOuterType, NewInnerMonad, NewInnerType, B>(OuterType ma, Func<A, B> f)
            where NewOuterMonad : Monad<NewOuterType, NewInnerType>
            where NewInnerMonad : Monad<NewInnerType, B> =>
                OuterMonad.Fold(ma, NewOuterMonad.Return(NewInnerMonad.Zero()), (outerState, innerA) =>
                    Trans<NewOuterMonad, NewOuterType, NewInnerMonad, NewInnerType, B>.Plus(outerState,
                        InnerMonad.Bind<NewOuterMonad, NewOuterType, NewInnerType>(innerA, a =>
                            NewOuterMonad.Return(NewInnerMonad.Return(f(a))))))(unit);

        public static OuterType Plus(OuterType ma, OuterType mb) =>
            OuterMonad.Apply(InnerMonad.Plus, ma, mb);

        public static OuterType Zero() =>
            OuterMonad.Return(InnerMonad.Zero());
    }

    public struct Trans<OuterMonad, OuterType, InnerMonad, InnerType, NumA, A>
        where OuterMonad : Monad<OuterType, InnerType>
        where InnerMonad : Monad<InnerType, A>
        where NumA : Num<A>
    {
        public static A Sum(OuterType ma) =>
            OuterMonad.Fold(ma, NumA.FromInteger(0), (s, inner) =>
                InnerMonad.Fold(inner, s, (ss, x) => NumA.Plus(ss, x))(unit))(unit);
    }
}
