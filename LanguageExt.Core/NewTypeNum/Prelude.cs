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
        public static NEWTYPE bind<NEWTYPE, NUM, T>(NewType<NEWTYPE, NUM, T> value, Func<T, NEWTYPE> bind)
            where NUM     : struct, Num<T>
            where NEWTYPE : NewType<NEWTYPE, NUM, T> =>
            value.Bind(bind);

        public static Unit iter<NEWTYPE, NUM, T>(NewType<NEWTYPE, NUM, T> value, Action<T> f)
            where NUM     : struct, Num<T>
            where NEWTYPE : NewType<NEWTYPE, NUM, T> =>
            value.Iter(f);

        [Pure]
        public static int count<NEWTYPE, NUM, T>(NewType<NEWTYPE, NUM, T> value)
            where NUM     : struct, Num<T>
            where NEWTYPE : NewType<NEWTYPE, NUM, T> =>
            1;

        [Pure]
        public static bool exists<NEWTYPE, NUM, T>(NewType<NEWTYPE, NUM, T> value, Func<T, bool> predicate)
            where NUM     : struct, Num<T>
            where NEWTYPE : NewType<NEWTYPE, NUM, T> =>
            predicate(value.Value);

        [Pure]
        public static bool forall<NEWTYPE, NUM, T>(NewType<NEWTYPE, NUM, T> value, Func<T, bool> predicate)
            where NUM     : struct, Num<T>
            where NEWTYPE : NewType<NEWTYPE, NUM, T> =>
            predicate(value.Value);

        [Pure]
        public static NEWTYPE map<NEWTYPE, NUM, T>(NewType<NEWTYPE, NUM, T> value, Func<T, T> map)
            where NUM     : struct, Num<T>
            where NEWTYPE : NewType<NEWTYPE, NUM, T> =>
            value.Map(map);

        [Pure]
        public static S fold<NEWTYPE, NUM, T, S>(NewType<NEWTYPE, NUM, T> value, S state, Func<S, T, S> folder)
            where NUM     : struct, Num<T>
            where NEWTYPE : NewType<NEWTYPE, NUM, T> =>
            value.Fold(state, folder);

        [Pure]
        public static NEWTYPE append<NEWTYPE, NUM, T>(NEWTYPE lhs, NewType<NEWTYPE, NUM, T> rhs)
            where NUM     : struct, Num<T>
            where NEWTYPE : NewType<NEWTYPE, NUM, T> =>
            add(lhs, rhs);

        /// <summary>
        /// Add the bound values of x and y, uses an Add type-class to provide the add
        /// operation for type A.  For example x.Add<Metres, TInt, int>(y)
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="x">Left hand side of the operation</param>
        /// <param name="y">Right hand side of the operation</param>
        /// <returns>An NewType with y added to x</returns>
        [Pure]
        public static NEWTYPE add<NEWTYPE, NUM, A>(this NEWTYPE x, NewType<NEWTYPE, NUM, A> y)
            where NUM : struct, Num<A>
            where NEWTYPE : NewType<NEWTYPE, NUM, A> =>
            from a in x
            from b in y
            select default(NUM).Add(a, b);

        /// <summary>
        /// Find the difference between the two bound values of x and y, uses a Difference type-class 
        /// to provide the difference operation for type A.  For example x.Difference<TInteger,int>(y)
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="x">Left hand side of the operation</param>
        /// <param name="y">Right hand side of the operation</param>
        /// <returns>An NewType with the difference between x and y</returns>
        [Pure]
        public static NEWTYPE difference<NEWTYPE, NUM, A>(this NEWTYPE x, NewType<NEWTYPE, NUM, A> y)
            where NUM : struct, Num<A>
            where NEWTYPE : NewType<NEWTYPE, NUM, A> =>
            from a in x
            from b in y
            select default(NUM).Difference(a, b);

        /// <summary>
        /// Find the product between the two bound values of x and y, uses a Product type-class 
        /// to provide the product operation for type A.  For example x.Product<TInteger,int>(y)
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="x">Left hand side of the operation</param>
        /// <param name="y">Right hand side of the operation</param>
        /// <returns>Product of x and y</returns>
        [Pure]
        public static NEWTYPE product<NEWTYPE, NUM, A>(this NEWTYPE x, NewType<NEWTYPE, NUM, A> y)
            where NUM : struct, Num<A>
            where NEWTYPE : NewType<NEWTYPE, NUM, A> =>
            from a in x
            from b in y
            select default(NUM).Product(a, b);

        /// <summary>
        /// Find the product between the two bound values of x and y, uses a Product type-class 
        /// to provide the product operation for type A.  For example x.Product<TInteger,int>(y)
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="x">Left hand side of the operation</param>
        /// <param name="y">Right hand side of the operation</param>
        /// <returns>Product of x and y</returns>
        [Pure]
        public static NEWTYPE divide<NEWTYPE, NUM, A>(this NEWTYPE x, NewType<NEWTYPE, NUM, A> y)
            where NUM : struct, Num<A>
            where NEWTYPE : NewType<NEWTYPE, NUM, A> =>
            from a in x
            from b in y
            select default(NUM).Divide(a, b);

        public static A sum<NEWTYPE, NUM, A>(this NewType<NEWTYPE, NUM, A> self)
            where NEWTYPE : NewType<NEWTYPE, NUM, A>
            where NUM : struct, Num<A> =>
            self.Value;
    }
}