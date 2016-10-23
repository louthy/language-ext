using LanguageExt;
using System.Diagnostics.Contracts;
using static LanguageExt.TypeClass;
using static LanguageExt.Prelude;
using LanguageExt.Instances;
using LanguageExt.TypeClasses;

public static class NewTypeExtensions
{
    /// <summary>
    /// Add the bound values of x and y, uses an Add type-class to provide the add
    /// operation for type A.  For example x.Add<Metres, TInt, int>(y)
    /// </summary>
    /// <typeparam name="ADD">Add of A</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="x">Left hand side of the operation</param>
    /// <param name="y">Right hand side of the operation</param>
    /// <returns>An NewType with y added to x</returns>
    [Pure]
    public static NEWTYPE Add<NEWTYPE, NUM, A>(this NewType<NEWTYPE, A> lhs, NewType<NEWTYPE, A> rhs)
        where NUM     : struct, Num<A>
        where NEWTYPE : NewType<NEWTYPE, A> =>
        from x in lhs
        from y in rhs
        select plus<NUM, A>(x, y);

    /// <summary>
    /// Find the subtract between the two bound values of x and y, uses a Subtract type-class 
    /// to provide the subtract operation for type A.  For example x.Subtract<TInteger,int>(y)
    /// </summary>
    /// <typeparam name="DIFF">Subtract of A</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="x">Left hand side of the operation</param>
    /// <param name="y">Right hand side of the operation</param>
    /// <returns>NewType with the subtract between x and y</returns>
    [Pure]
    public static NEWTYPE Subtract<NEWTYPE, NUM, A>(this NewType<NEWTYPE, A> x, NewType<NEWTYPE, A> y) 
        where NUM     : struct, Num<A>
        where NEWTYPE : NewType<NEWTYPE, A> =>
        from a in x
        from b in y
        select subtract<NUM, A>(a, b);

    /// <summary>
    /// Find the product between the two bound values of x and y, uses a Product type-class 
    /// to provide the product operation for type A.  For example x.Product<TInteger,int>(y)
    /// </summary>
    /// <typeparam name="PROD">Product of A</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="x">Left hand side of the operation</param>
    /// <param name="y">Right hand side of the operation</param>
    /// <returns>Product of x and y</returns>
    [Pure]
    public static NEWTYPE Product<NEWTYPE, NUM, A>(this NewType<NEWTYPE, A> x, NewType<NEWTYPE, A> y) 
        where NUM     : struct, Num<A>
        where NEWTYPE : NewType<NEWTYPE, A> =>
        from a in x
        from b in y
        select product<NUM, A>(a, b);

    /// <summary>
    /// Divide the two bound values of x and y, uses a Divide type-class to provide the divide
    /// operation for type A.  For example x.Divide<TDouble,double>(y)
    /// </summary>
    /// <typeparam name="DIV">Divide of A</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="x">Left hand side of the operation</param>
    /// <param name="y">Right hand side of the operation</param>
    /// <returns>x / y</returns>
    [Pure]
    public static NEWTYPE Divide<NEWTYPE, NUM, A>(this NewType<NEWTYPE, A> x, NewType<NEWTYPE, A> y) 
        where NUM     : struct, Num<A>
        where NEWTYPE : NewType<NEWTYPE, A> =>
        from a in x
        from b in y
        select divide<NUM, A>(a, b);
}
