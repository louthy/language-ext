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
        public static int compare<NEWTYPE, ORD, T>(NEWTYPE x, NEWTYPE y) 
            where ORD : struct, Ord<T>
            where NEWTYPE : NewType<NEWTYPE, T> =>
            default(OrdNewType<NEWTYPE, ORD, T>).Compare(x, y);

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
        public static A sum<NEWTYPE, NUM, A>(NewType<NEWTYPE, A> value)
            where NUM     : struct, Num<A>
            where NEWTYPE : NewType<NEWTYPE, A> =>
            value.Sum<NUM, A>();

        [Pure]
        public static int sum<NEWTYPE>(NewType<NEWTYPE, int> value)
            where NEWTYPE : NewType<NEWTYPE, int> =>
            value.Sum<TInt, int>();

        [Pure]
        public static float sum<NEWTYPE>(NewType<NEWTYPE, float> value)
            where NEWTYPE : NewType<NEWTYPE, float> =>
            value.Sum<TFloat, float>();

        [Pure]
        public static double sum<NEWTYPE>(NewType<NEWTYPE, double> value)
            where NEWTYPE : NewType<NEWTYPE, double> =>
            value.Sum<TDouble, double>();

        [Pure]
        public static decimal sum<NEWTYPE>(NewType<NEWTYPE, decimal> value)
            where NEWTYPE : NewType<NEWTYPE, decimal> =>
            value.Sum<TDecimal, decimal>();

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
        public static NEWTYPE add<NEWTYPE, ADD, T>(NEWTYPE lhs, NEWTYPE rhs)
            where ADD     : struct, Addition<T>
            where NEWTYPE : NewType<NEWTYPE, T> =>
            from x in lhs
            from y in rhs
            select default(ADD).Add(x, y);

        [Pure]
        public static NEWTYPE append<NEWTYPE, SEMI, T>(NEWTYPE lhs, NEWTYPE rhs)
            where SEMI    : struct, Semigroup<T>
            where NEWTYPE : NewType<NEWTYPE, T> =>
            from x in lhs
            from y in rhs
            select default(SEMI).Append(x, y);

        [Pure]
        public static NEWTYPE difference<NEWTYPE, DIFF, T>(NEWTYPE lhs, NEWTYPE rhs)
            where DIFF    : struct, Difference<T>
            where NEWTYPE : NewType<NEWTYPE, T> =>
            from x in lhs
            from y in rhs
            select default(DIFF).Difference(x, y);

        [Pure]
        public static NEWTYPE divide<NEWTYPE, DIV, T>(NEWTYPE lhs, NEWTYPE rhs)
            where DIV     : struct, Divisible<T>
            where NEWTYPE : NewType<NEWTYPE, T> =>
            from x in lhs
            from y in rhs
            select default(DIV).Divide(x, y);

        [Pure]
        public static NEWTYPE product<NEWTYPE, PROD, T>(NEWTYPE lhs, NEWTYPE rhs)
            where PROD    : struct, Product<T>
            where NEWTYPE : NewType<NEWTYPE, T> =>
            from x in lhs
            from y in rhs
            select default(PROD).Product(x, y);
    }
}
