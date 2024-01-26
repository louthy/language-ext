using System;
using System.Diagnostics.Contracts;
using LanguageExt.TypeClasses;

namespace LanguageExt;

public static partial class Prelude
{
    [Pure]
    public static SELF bind<SELF, FLOAT, T, PRED>(FloatType<SELF, FLOAT, T, PRED> value, Func<T, SELF> bind)
        where FLOAT : Floating<T>
        where PRED : Pred<T>
        where SELF : FloatType<SELF, FLOAT, T, PRED> =>
        value.Bind(bind);

    public static Unit iter<SELF, FLOAT, T, PRED>(FloatType<SELF, FLOAT, T, PRED> value, Action<T> f)
        where FLOAT : Floating<T>
        where PRED : Pred<T>
        where SELF : FloatType<SELF, FLOAT, T, PRED> =>
        value.Iter(f);

    [Pure]
    public static int count<SELF, FLOAT, T, PRED>(FloatType<SELF, FLOAT, T, PRED> value)
        where FLOAT : Floating<T>
        where PRED : Pred<T>
        where SELF : FloatType<SELF, FLOAT, T, PRED> =>
        1;

    [Pure]
    public static bool exists<SELF, FLOAT, T, PRED>(FloatType<SELF, FLOAT, T, PRED> value, Func<T, bool> predicate)
        where FLOAT : Floating<T>
        where PRED : Pred<T>
        where SELF : FloatType<SELF, FLOAT, T, PRED> =>
        predicate((T)value);

    [Pure]
    public static bool forall<SELF, FLOAT, T, PRED>(FloatType<SELF, FLOAT, T, PRED> value, Func<T, bool> predicate)
        where FLOAT : Floating<T>
        where PRED : Pred<T>
        where SELF : FloatType<SELF, FLOAT, T, PRED> =>
        predicate((T)value);

    [Pure]
    public static SELF map<SELF, FLOAT, T, PRED>(FloatType<SELF, FLOAT, T, PRED> value, Func<T, T> map)
        where FLOAT : Floating<T>
        where PRED : Pred<T>
        where SELF : FloatType<SELF, FLOAT, T, PRED> =>
        value.Map(map);

    [Pure]
    public static S fold<SELF, FLOAT, T, S, PRED>(FloatType<SELF, FLOAT, T, PRED> value, S state, Func<S, T, S> folder)
        where FLOAT : Floating<T>
        where PRED : Pred<T>
        where SELF : FloatType<SELF, FLOAT, T, PRED> =>
        value.Fold(state, folder);

    /// <summary>
    /// Add the bound values of x and y, uses an Add trait to provide the add
    /// operation for type A.  For example x.Add<Metres, TInt, int>(y)
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="x">Left hand side of the operation</param>
    /// <param name="y">Right hand side of the operation</param>
    /// <returns>An NewType with y added to x</returns>
    [Pure]
    public static SELF add<SELF, FLOAT, A, PRED>(FloatType<SELF, FLOAT, A, PRED> x, SELF y)
        where FLOAT : Floating<A>
        where PRED : Pred<A>
        where SELF : FloatType<SELF, FLOAT, A, PRED> =>
        from a in x
        from b in y
        select FLOAT.Plus(a, b);

    /// <summary>
    /// Find the subtract between the two bound values of x and y, uses a Subtract trait 
    /// to provide the subtract operation for type A.  For example x.Subtract<TInteger,int>(y)
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="x">Left hand side of the operation</param>
    /// <param name="y">Right hand side of the operation</param>
    /// <returns>An NewType with the subtract between x and y</returns>
    [Pure]
    public static SELF subtract<SELF, FLOAT, A, PRED>(FloatType<SELF, FLOAT, A, PRED> x, SELF y)
        where FLOAT : Floating<A>
        where PRED : Pred<A>
        where SELF : FloatType<SELF, FLOAT, A, PRED> =>
        from a in x
        from b in y
        select FLOAT.Subtract(a, b);

    /// <summary>
    /// Find the product between the two bound values of x and y, uses a Product trait 
    /// to provide the product operation for type A.  For example x.Product<TInteger,int>(y)
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="x">Left hand side of the operation</param>
    /// <param name="y">Right hand side of the operation</param>
    /// <returns>Product of x and y</returns>
    [Pure]
    public static SELF product<SELF, FLOAT, A, PRED>(FloatType<SELF, FLOAT, A, PRED> x, SELF y)
        where FLOAT : Floating<A>
        where PRED : Pred<A>
        where SELF : FloatType<SELF, FLOAT, A, PRED> =>
        from a in x
        from b in y
        select FLOAT.Product(a, b);

    /// <summary>
    /// Find the product between the two bound values of x and y, uses a Product trait 
    /// to provide the product operation for type A.  For example x.Product<TInteger,int>(y)
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="x">Left hand side of the operation</param>
    /// <param name="y">Right hand side of the operation</param>
    /// <returns>Product of x and y</returns>
    [Pure]
    public static SELF divide<SELF, FLOAT, A, PRED>(FloatType<SELF, FLOAT, A, PRED> x, SELF y)
        where FLOAT : Floating<A>
        where PRED : Pred<A>
        where SELF : FloatType<SELF, FLOAT, A, PRED> =>
        from a in x
        from b in y
        select FLOAT.Divide(a, b);

    public static A sum<SELF, FLOAT, A, PRED>(FloatType<SELF, FLOAT, A, PRED> self)
        where SELF : FloatType<SELF, FLOAT, A, PRED>
        where PRED : Pred<A>
        where FLOAT : Floating<A> =>
        (A)self;


    [Pure]
    public static SELF bind<SELF, FLOAT, T>(FloatType<SELF, FLOAT, T> value, Func<T, SELF> bind)
        where FLOAT : Floating<T>
        where SELF : FloatType<SELF, FLOAT, T> =>
        value.Bind(bind);

    public static Unit iter<SELF, FLOAT, T>(FloatType<SELF, FLOAT, T> value, Action<T> f)
        where FLOAT : Floating<T>
        where SELF : FloatType<SELF, FLOAT, T> =>
        value.Iter(f);

    [Pure]
    public static int count<SELF, FLOAT, T>(FloatType<SELF, FLOAT, T> value)
        where FLOAT : Floating<T>
        where SELF : FloatType<SELF, FLOAT, T> =>
        1;

    [Pure]
    public static bool exists<SELF, FLOAT, T>(FloatType<SELF, FLOAT, T> value, Func<T, bool> predicate)
        where FLOAT : Floating<T>
        where SELF : FloatType<SELF, FLOAT, T> =>
        predicate((T)value);

    [Pure]
    public static bool forall<SELF, FLOAT, T>(FloatType<SELF, FLOAT, T> value, Func<T, bool> predicate)
        where FLOAT : Floating<T>
        where SELF : FloatType<SELF, FLOAT, T> =>
        predicate((T)value);

    [Pure]
    public static SELF map<SELF, FLOAT, T>(FloatType<SELF, FLOAT, T> value, Func<T, T> map)
        where FLOAT : Floating<T>
        where SELF : FloatType<SELF, FLOAT, T> =>
        value.Map(map);

    [Pure]
    public static S fold<SELF, FLOAT, T, S>(FloatType<SELF, FLOAT, T> value, S state, Func<S, T, S> folder)
        where FLOAT : Floating<T>
        where SELF : FloatType<SELF, FLOAT, T> =>
        value.Fold(state, folder);

    /// <summary>
    /// Add the bound values of x and y, uses an Add trait to provide the add
    /// operation for type A.  For example x.Add<Metres, TInt, int>(y)
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="x">Left hand side of the operation</param>
    /// <param name="y">Right hand side of the operation</param>
    /// <returns>An NewType with y added to x</returns>
    [Pure]
    public static SELF add<SELF, FLOAT, A>(FloatType<SELF, FLOAT, A> x, SELF y)
        where FLOAT : Floating<A>
        where SELF : FloatType<SELF, FLOAT, A> =>
        from a in x
        from b in y
        select FLOAT.Plus(a, b);

    /// <summary>
    /// Find the subtract between the two bound values of x and y, uses a Subtract trait 
    /// to provide the subtract operation for type A.  For example x.Subtract<TInteger,int>(y)
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="x">Left hand side of the operation</param>
    /// <param name="y">Right hand side of the operation</param>
    /// <returns>An NewType with the subtract between x and y</returns>
    [Pure]
    public static SELF subtract<SELF, FLOAT, A>(FloatType<SELF, FLOAT, A> x, SELF y)
        where FLOAT : Floating<A>
        where SELF : FloatType<SELF, FLOAT, A> =>
        from a in x
        from b in y
        select FLOAT.Subtract(a, b);

    /// <summary>
    /// Find the product between the two bound values of x and y, uses a Product trait 
    /// to provide the product operation for type A.  For example x.Product<TInteger,int>(y)
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="x">Left hand side of the operation</param>
    /// <param name="y">Right hand side of the operation</param>
    /// <returns>Product of x and y</returns>
    [Pure]
    public static SELF product<SELF, FLOAT, A>(FloatType<SELF, FLOAT, A> x, SELF y)
        where FLOAT : Floating<A>
        where SELF : FloatType<SELF, FLOAT, A> =>
        from a in x
        from b in y
        select FLOAT.Product(a, b);

    /// <summary>
    /// Find the product between the two bound values of x and y, uses a Product trait 
    /// to provide the product operation for type A.  For example x.Product<TInteger,int>(y)
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="x">Left hand side of the operation</param>
    /// <param name="y">Right hand side of the operation</param>
    /// <returns>Product of x and y</returns>
    [Pure]
    public static SELF divide<SELF, FLOAT, A>(FloatType<SELF, FLOAT, A> x, SELF y)
        where FLOAT : Floating<A>
        where SELF : FloatType<SELF, FLOAT, A> =>
        from a in x
        from b in y
        select FLOAT.Divide(a, b);

}
