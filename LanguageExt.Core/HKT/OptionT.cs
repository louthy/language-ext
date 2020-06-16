using System;
using LanguageExt.TypeClasses;
using LanguageExt.ClassInstances;
using System.Collections.Generic;

namespace LanguageExt
{
    /// <summary>
    /// Testing - Ignore
    /// </summary>
    /// <typeparam name="OuterMonad"></typeparam>
    /// <typeparam name="OuterType"></typeparam>
    /// <typeparam name="A"></typeparam>

    struct OptionT<OuterMonad, OuterType, A> : MonadTrans<OuterMonad, OuterType, MOption<A>, Option<A>, A>
        where OuterMonad : struct, Monad<OuterType, Option<A>>
    {
        public static readonly OptionT<OuterMonad, OuterType, A> Inst = default(OptionT<OuterMonad, OuterType, A>);

        public OuterType Bind(OuterType ma, Func<A, Option<A>> f) =>
            Trans<OuterMonad, OuterType, MOption<A>, Option<A>, A>
                .Inst.Bind<OuterMonad, OuterType, MOption<A>, Option<A>, A>(ma, f);

        public NewOuterType Bind<NewOuterMonad, NewOuterType, B>(OuterType ma, Func<A, Option<B>> f)
            where NewOuterMonad : struct, Monad<NewOuterType, Option<B>> =>
            Trans<OuterMonad, OuterType, MOption<A>, Option<A>, A>
                .Inst.Bind<NewOuterMonad, NewOuterType, MOption<B>, Option<B>, B>(ma, f);

        public NewOuterType Bind<NewOuterMonad, NewOuterType, NewInnerMonad, NewInnerType, B>(OuterType ma, Func<A, NewInnerType> f)
            where NewOuterMonad : struct, Monad<NewOuterType, NewInnerType>
            where NewInnerMonad : struct, Monad<NewInnerType, B> =>
            Trans<OuterMonad, OuterType, MOption<A>, Option<A>, A>
                .Inst.Bind<NewOuterMonad, NewOuterType, NewInnerMonad, NewInnerType, B>(ma, f);

        public NewOuterType Bind<NewOuterMonad, NewOuterType, NewInnerMonad, NewInnerType, B>(OuterType ma, Func<A, NewOuterType> f)
            where NewOuterMonad : struct, Monad<NewOuterType, NewInnerType>
            where NewInnerMonad : struct, Monad<NewInnerType, B> =>
            Trans<OuterMonad, OuterType, MOption<A>, Option<A>, A>
                .Inst.Bind<NewOuterMonad, NewOuterType, NewInnerMonad, NewInnerType, B>(ma, f);

        public NewOuterType BindAsync<NewOuterMonad, NewOuterType, NewInnerMonad, NewInnerType, B>(OuterType ma, Func<A, NewOuterType> f)
            where NewOuterMonad : struct, MonadAsync<NewOuterType, NewInnerType>
            where NewInnerMonad : struct, Monad<NewInnerType, B> =>
            Trans<OuterMonad, OuterType, MOption<A>, Option<A>, A>
                .Inst.BindAsync<NewOuterMonad, NewOuterType, NewInnerMonad, NewInnerType, B>(ma, f);

        public S Fold<S>(OuterType ma, S state, Func<S, A, S> f) =>
            Trans<OuterMonad, OuterType, MOption<A>, Option<A>, A>.Inst.Fold(ma, state, f);

        public S FoldBack<S>(OuterType ma, S state, Func<S, A, S> f) =>
            Trans<OuterMonad, OuterType, MOption<A>, Option<A>, A>.Inst.FoldBack(ma, state, f);

        public NewOuterType Map<NewOuterMonad, NewOuterType, NewInnerMonad, NewInnerType, B>(OuterType ma, Func<A, B> f)
            where NewOuterMonad : struct, Monad<NewOuterType, NewInnerType>
            where NewInnerMonad : struct, Monad<NewInnerType, B> =>
            Trans<OuterMonad, OuterType, MOption<A>, Option<A>, A>
                .Inst.Map<NewOuterMonad, NewOuterType, NewInnerMonad, NewInnerType, B>(ma, f);

        public OuterType Plus(OuterType a, OuterType b)
        {
            throw new NotSupportedException();
        }

        public NewOuterType Sequence<NewOuterMonad, NewOuterType, NewInnerMonad, NewInnerType>(OuterType ma)
            where NewOuterMonad : struct, Monad<NewOuterType, NewInnerType>
            where NewInnerMonad : struct, Monad<NewInnerType, A>
        {
            throw new NotSupportedException();
        }

        public NewOuterType Traverse<NewOuterMonad, NewOuterType, NewInnerMonad, NewInnerType, B>(OuterType ma, Func<A, B> f)
            where NewOuterMonad : struct, Monad<NewOuterType, NewInnerType>
            where NewInnerMonad : struct, Monad<NewInnerType, B>
        {
            throw new NotSupportedException();
        }

        public OuterType Zero()
        {
            throw new NotSupportedException();
        }
    }
}

