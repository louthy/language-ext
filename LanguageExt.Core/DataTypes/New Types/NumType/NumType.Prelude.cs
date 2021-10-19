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
        public static NUMTYPE bind<NUMTYPE, NUM, T, PRED>(NumType<NUMTYPE, NUM, T, PRED> value, Func<T, NUMTYPE> bind)
            where NUM     : struct, Num<T>
            where PRED    : struct, Pred<T>
            where NUMTYPE : NumType<NUMTYPE, NUM, T, PRED> =>
            value.Bind(bind);

        public static Unit iter<NUMTYPE, NUM, T, PRED>(NumType<NUMTYPE, NUM, T, PRED> value, Action<T> f)
            where NUM     : struct, Num<T>
            where PRED    : struct, Pred<T>
            where NUMTYPE : NumType<NUMTYPE, NUM, T, PRED> =>
            value.Iter(f);

        [Pure]
        public static int count<NUMTYPE, NUM, T, PRED>(NumType<NUMTYPE, NUM, T, PRED> value)
            where NUM     : struct, Num<T>
            where PRED    : struct, Pred<T>
            where NUMTYPE : NumType<NUMTYPE, NUM, T, PRED> =>
            1;

        [Pure]
        public static bool exists<NUMTYPE, NUM, T, PRED>(NumType<NUMTYPE, NUM, T, PRED> value, Func<T, bool> predicate)
            where NUM     : struct, Num<T>
            where PRED    : struct, Pred<T>
            where NUMTYPE : NumType<NUMTYPE, NUM, T, PRED> =>
            predicate((T)value);

        [Pure]
        public static bool forall<NUMTYPE, NUM, T, PRED>(NumType<NUMTYPE, NUM, T, PRED> value, Func<T, bool> predicate)
            where NUM     : struct, Num<T>
            where PRED    : struct, Pred<T>
            where NUMTYPE : NumType<NUMTYPE, NUM, T, PRED> =>
            predicate((T)value);

        [Pure]
        public static NUMTYPE map<NUMTYPE, NUM, T, PRED>(NumType<NUMTYPE, NUM, T, PRED> value, Func<T, T> map)
            where NUM     : struct, Num<T>
            where PRED    : struct, Pred<T>
            where NUMTYPE : NumType<NUMTYPE, NUM, T, PRED> =>
            value.Map(map);

        [Pure]
        public static S fold<NUMTYPE, NUM, T, S, PRED>(NumType<NUMTYPE, NUM, T, PRED> value, S state, Func<S, T, S> folder)
            where NUM     : struct, Num<T>
            where PRED    : struct, Pred<T>
            where NUMTYPE : NumType<NUMTYPE, NUM, T, PRED> =>
            value.Fold(state, folder);

        /// <summary>
        /// Add the bound values of x and y, uses an Add type-class to provide the add
        /// operation for type A.  For example x.Add<Metres, TInt, int>(y)
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="x">Left hand side of the operation</param>
        /// <param name="y">Right hand side of the operation</param>
        /// <returns>An NewType with y added to x</returns>
        [Pure]
        public static NUMTYPE add<NUMTYPE, NUM, A, PRED>(NumType<NUMTYPE, NUM, A, PRED> x, NUMTYPE y)
            where NUM     : struct, Num<A>
            where PRED    : struct, Pred<A>
            where NUMTYPE : NumType<NUMTYPE, NUM, A, PRED> =>
            from a in x
            from b in y
            select default(NUM).Plus(a, b);

        /// <summary>
        /// Find the subtract between the two bound values of x and y, uses a Subtract type-class 
        /// to provide the subtract operation for type A.  For example x.Subtract<TInteger,int>(y)
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="x">Left hand side of the operation</param>
        /// <param name="y">Right hand side of the operation</param>
        /// <returns>An NewType with the subtract between x and y</returns>
        [Pure]
        public static NUMTYPE subtract<NUMTYPE, NUM, A, PRED>(NumType<NUMTYPE, NUM, A, PRED> x, NUMTYPE y)
            where NUM     : struct, Num<A>
            where PRED    : struct, Pred<A>
            where NUMTYPE : NumType<NUMTYPE, NUM, A, PRED> =>
            from a in x
            from b in y
            select default(NUM).Subtract(a, b);

        /// <summary>
        /// Find the product between the two bound values of x and y, uses a Product type-class 
        /// to provide the product operation for type A.  For example x.Product<TInteger,int>(y)
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="x">Left hand side of the operation</param>
        /// <param name="y">Right hand side of the operation</param>
        /// <returns>Product of x and y</returns>
        [Pure]
        public static NUMTYPE product<NUMTYPE, NUM, A, PRED>(NumType<NUMTYPE, NUM, A, PRED> x, NUMTYPE y)
            where NUM     : struct, Num<A>
            where PRED    : struct, Pred<A>
            where NUMTYPE : NumType<NUMTYPE, NUM, A, PRED> =>
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
        public static NUMTYPE divide<NUMTYPE, NUM, A, PRED>(NumType<NUMTYPE, NUM, A, PRED> x, NUMTYPE y)
            where NUM     : struct, Num<A>
            where PRED : struct, Pred<A>
            where NUMTYPE : NumType<NUMTYPE, NUM, A, PRED> =>
            from a in x
            from b in y
            select default(NUM).Divide(a, b);

        public static A sum<NUMTYPE, NUM, A, PRED>(NumType<NUMTYPE, NUM, A, PRED> self)
            where NUMTYPE : NumType<NUMTYPE, NUM, A, PRED>
            where PRED : struct, Pred<A>
            where NUM     : struct, Num<A> =>
                (A)self;


        [Pure]
        public static NUMTYPE bind<NUMTYPE, NUM, T>(NumType<NUMTYPE, NUM, T> value, Func<T, NUMTYPE> bind)
            where NUM : struct, Num<T>
            where NUMTYPE : NumType<NUMTYPE, NUM, T> =>
            value.Bind(bind);

        public static Unit iter<NUMTYPE, NUM, T>(NumType<NUMTYPE, NUM, T> value, Action<T> f)
            where NUM : struct, Num<T>
            where NUMTYPE : NumType<NUMTYPE, NUM, T> =>
            value.Iter(f);

        [Pure]
        public static int count<NUMTYPE, NUM, T>(NumType<NUMTYPE, NUM, T> value)
            where NUM : struct, Num<T>
            where NUMTYPE : NumType<NUMTYPE, NUM, T> =>
            1;

        [Pure]
        public static bool exists<NUMTYPE, NUM, T>(NumType<NUMTYPE, NUM, T> value, Func<T, bool> predicate)
            where NUM : struct, Num<T>
            where NUMTYPE : NumType<NUMTYPE, NUM, T> =>
            predicate((T)value);

        [Pure]
        public static bool forall<NUMTYPE, NUM, T>(NumType<NUMTYPE, NUM, T> value, Func<T, bool> predicate)
            where NUM : struct, Num<T>
            where NUMTYPE : NumType<NUMTYPE, NUM, T> =>
            predicate((T)value);

        [Pure]
        public static NUMTYPE map<NUMTYPE, NUM, T>(NumType<NUMTYPE, NUM, T> value, Func<T, T> map)
            where NUM : struct, Num<T>
            where NUMTYPE : NumType<NUMTYPE, NUM, T> =>
            value.Map(map);

        [Pure]
        public static S fold<NUMTYPE, NUM, T, S>(NumType<NUMTYPE, NUM, T> value, S state, Func<S, T, S> folder)
            where NUM : struct, Num<T>
            where NUMTYPE : NumType<NUMTYPE, NUM, T> =>
            value.Fold(state, folder);

        /// <summary>
        /// Add the bound values of x and y, uses an Add type-class to provide the add
        /// operation for type A.  For example x.Add<Metres, TInt, int>(y)
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="x">Left hand side of the operation</param>
        /// <param name="y">Right hand side of the operation</param>
        /// <returns>An NewType with y added to x</returns>
        [Pure]
        public static NUMTYPE add<NUMTYPE, NUM, A>(NumType<NUMTYPE, NUM, A> x, NUMTYPE y)
            where NUM : struct, Num<A>
            where NUMTYPE : NumType<NUMTYPE, NUM, A> =>
            from a in x
            from b in y
            select default(NUM).Plus(a, b);

        /// <summary>
        /// Find the subtract between the two bound values of x and y, uses a Subtract type-class 
        /// to provide the subtract operation for type A.  For example x.Subtract<TInteger,int>(y)
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="x">Left hand side of the operation</param>
        /// <param name="y">Right hand side of the operation</param>
        /// <returns>An NewType with the subtract between x and y</returns>
        [Pure]
        public static NUMTYPE subtract<NUMTYPE, NUM, A>(NumType<NUMTYPE, NUM, A> x, NUMTYPE y)
            where NUM : struct, Num<A>
            where NUMTYPE : NumType<NUMTYPE, NUM, A> =>
            from a in x
            from b in y
            select default(NUM).Subtract(a, b);

        /// <summary>
        /// Find the product between the two bound values of x and y, uses a Product type-class 
        /// to provide the product operation for type A.  For example x.Product<TInteger,int>(y)
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="x">Left hand side of the operation</param>
        /// <param name="y">Right hand side of the operation</param>
        /// <returns>Product of x and y</returns>
        [Pure]
        public static NUMTYPE product<NUMTYPE, NUM, A>(NumType<NUMTYPE, NUM, A> x, NUMTYPE y)
            where NUM     : struct, Num<A>
            where NUMTYPE : NumType<NUMTYPE, NUM, A> =>
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
        public static NUMTYPE divide<NUMTYPE, NUM, A>(NumType<NUMTYPE, NUM, A> x, NUMTYPE y)
            where NUM     : struct, Num<A>
            where NUMTYPE : NumType<NUMTYPE, NUM, A> =>
            from a in x
            from b in y
            select default(NUM).Divide(a, b);

        public static A sum<NUMTYPE, NUM, A>(NumType<NUMTYPE, NUM, A> self)
            where NUMTYPE : NumType<NUMTYPE, NUM, A>
            where NUM     : struct, Num<A> =>
            (A)self;

    }
}