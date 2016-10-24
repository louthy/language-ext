using System;
using System.Diagnostics.Contracts;
using LanguageExt.ClassInstances;
using LanguageExt.TypeClasses;
using static LanguageExt.TypeClass;

namespace LanguageExt
{
    public static partial class Prelude
    {
        [Pure]
        public static NEWTYPE bind<NEWTYPE, T>(NewType<NEWTYPE, T> value, Func<T, NEWTYPE> bind)
            where NEWTYPE : NewType<NEWTYPE, T> =>
            value.Bind(bind);

        public static Unit iter<NEWTYPE, T>(NewType<NEWTYPE, T> value, Action<T> f)
            where NEWTYPE : NewType<NEWTYPE, T> =>
            value.Iter(f);

        [Pure]
        public static int count<NEWTYPE, T>(NewType<NEWTYPE, T> value)
            where NEWTYPE : NewType<NEWTYPE, T> =>
            1;

        [Pure]
        public static bool exists<NEWTYPE, T>(NewType<NEWTYPE, T> value, Func<T, bool> predicate)
            where NEWTYPE : NewType<NEWTYPE, T> =>
            predicate(value.Value);

        [Pure]
        public static bool forall<NEWTYPE, T>(NewType<NEWTYPE, T> value, Func<T, bool> predicate)
            where NEWTYPE : NewType<NEWTYPE, T> =>
            predicate(value.Value);

        [Pure]
        public static NEWTYPE map<NEWTYPE, T>(NewType<NEWTYPE, T> value, Func<T, T> map)
            where NEWTYPE : NewType<NEWTYPE, T> =>
            value.Map(map);

        [Pure]
        public static S fold<NEWTYPE, T, S>(NewType<NEWTYPE, T> value, S state, Func<S, T, S> folder)
            where NEWTYPE : NewType<NEWTYPE, T> =>
            value.Fold(state, folder);

        [Pure]
        public static NEWTYPE plus<NEWTYPE, NUM, T>(NEWTYPE lhs, NEWTYPE rhs)
            where NUM     : struct, Num<T>
            where NEWTYPE : NewType<NEWTYPE, T> =>
            from x in lhs
            from y in rhs
            select default(NUM).Plus(x, y);

        [Pure]
        public static NEWTYPE subtract<NEWTYPE, NUM, T>(NEWTYPE lhs, NEWTYPE rhs)
            where NUM    : struct, Num<T>
            where NEWTYPE : NewType<NEWTYPE, T> =>
            from x in lhs
            from y in rhs
            select default(NUM).Subtract(x, y);

        [Pure]
        public static NEWTYPE divide<NEWTYPE, NUM, T>(NEWTYPE lhs, NEWTYPE rhs)
            where NUM     : struct, Num<T>
            where NEWTYPE : NewType<NEWTYPE, T> =>
            from x in lhs
            from y in rhs
            select default(NUM).Divide(x, y);

        [Pure]
        public static NEWTYPE product<NEWTYPE, NUM, T>(NEWTYPE lhs, NEWTYPE rhs)
            where NUM    : struct, Num<T>
            where NEWTYPE : NewType<NEWTYPE, T> =>
            from x in lhs
            from y in rhs
            select default(NUM).Product(x, y);
    }
}
