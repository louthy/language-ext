using System;
using System.Diagnostics.Contracts;
using LanguageExt.Instances;
using LanguageExt.TypeClasses;
using static LanguageExt.TypeClass;

namespace LanguageExt
{
    public static partial class Prelude
    {
        [Pure]
        public static int compare<NEWTYPE, SEMI, ORD, T>(NEWTYPE x, NEWTYPE y) 
            where ORD : struct, Ord<T>
            where SEMI : struct, Semigroup<T>
            where NEWTYPE : NewType<NEWTYPE, SEMI, ORD, T> =>
            default(ORD).Compare(x.Value, y.Value);

        [Pure]
        public static NEWTYPE bind<NEWTYPE, SEMI, ORD, T>(NewType<NEWTYPE, SEMI, ORD, T> value, Func<T, NEWTYPE> bind)
            where ORD : struct, Ord<T>
            where SEMI : struct, Semigroup<T>
            where NEWTYPE : NewType<NEWTYPE, SEMI, ORD, T> =>
            value.Bind(bind);

        public static Unit iter<NEWTYPE, SEMI, ORD, T>(NewType<NEWTYPE, SEMI, ORD, T> value, Action<T> f)
            where ORD : struct, Ord<T>
            where SEMI : struct, Semigroup<T>
            where NEWTYPE : NewType<NEWTYPE, SEMI, ORD, T> =>
            value.Iter(f);

        [Pure]
        public static int count<NEWTYPE, SEMI, ORD, T>(NewType<NEWTYPE, SEMI, ORD, T> value)
            where ORD : struct, Ord<T>
            where SEMI : struct, Semigroup<T>
            where NEWTYPE : NewType<NEWTYPE, SEMI, ORD, T> =>
            1;

        [Pure]
        public static bool exists<NEWTYPE, SEMI, ORD, T>(NewType<NEWTYPE, SEMI, ORD, T> value, Func<T, bool> predicate)
            where ORD     : struct, Ord<T>
            where SEMI    : struct, Semigroup<T>
            where NEWTYPE : NewType<NEWTYPE, SEMI, ORD, T> =>
            predicate(value.Value);

        [Pure]
        public static bool forall<NEWTYPE, SEMI, ORD, T>(NewType<NEWTYPE, SEMI, ORD, T> value, Func<T, bool> predicate)
            where ORD     : struct, Ord<T>
            where SEMI    : struct, Semigroup<T>
            where NEWTYPE : NewType<NEWTYPE, SEMI, ORD, T> =>
            predicate(value.Value);

        [Pure]
        public static NEWTYPE map<NEWTYPE, SEMI, ORD, T>(NewType<NEWTYPE, SEMI, ORD, T> value, Func<T, T> map)
            where ORD     : struct, Ord<T>
            where SEMI    : struct, Semigroup<T>
            where NEWTYPE : NewType<NEWTYPE, SEMI, ORD, T> =>
            value.Map(map);

        [Pure]
        public static S fold<NEWTYPE, SEMI, ORD, T, S>(NewType<NEWTYPE, SEMI, ORD, T> value, S state, Func<S, T, S> folder)
            where ORD     : struct, Ord<T>
            where SEMI    : struct, Semigroup<T>
            where NEWTYPE : NewType<NEWTYPE, SEMI, ORD, T> =>
            value.Fold(state, folder);

        [Pure]
        public static NEWTYPE add<NEWTYPE, SEMI, ORD, NUM, T>(NEWTYPE lhs, NEWTYPE rhs)
            where ORD  : struct, Ord<T>
            where SEMI : struct, Semigroup<T>
            where NUM  : struct, Num<T>
            where NEWTYPE : NewType<NEWTYPE, SEMI, ORD, T> =>
            from x in lhs
            from y in rhs
            select default(NUM).Add(x, y);

        [Pure]
        public static NEWTYPE subtract<NEWTYPE, SEMI, ORD, NUM, T>(NEWTYPE lhs, NEWTYPE rhs)
            where ORD     : struct, Ord<T>
            where SEMI    : struct, Semigroup<T>
            where NUM     : struct, Num<T>
            where NEWTYPE : NewType<NEWTYPE, SEMI, ORD, T> =>
            from x in lhs
            from y in rhs
            select default(NUM).Subtract(x, y);

        [Pure]
        public static NEWTYPE divide<NEWTYPE, SEMI, ORD, NUM, T>(NEWTYPE lhs, NEWTYPE rhs)
            where ORD     : struct, Ord<T>
            where SEMI    : struct, Semigroup<T>
            where NUM     : struct, Num<T>
            where NEWTYPE : NewType<NEWTYPE, SEMI, ORD, T> =>
            from x in lhs
            from y in rhs
            select default(NUM).Divide(x, y);

        [Pure]
        public static NEWTYPE product<NEWTYPE, SEMI, ORD, NUM, T>(NEWTYPE lhs, NEWTYPE rhs)
            where ORD     : struct, Ord<T>
            where SEMI    : struct, Semigroup<T>
            where NUM     : struct, Num<T>
            where NEWTYPE : NewType<NEWTYPE, SEMI, ORD, T> =>
            from x in lhs
            from y in rhs
            select default(NUM).Product(x, y);
    }
}
