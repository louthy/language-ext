using LanguageExt.ClassInstances;
using LanguageExt.TypeClasses;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace LanguageExt;

public static partial class TypeClass
{
    /// <summary>
    /// An associative binary operation
    /// </summary>
    /// <param name="x">The left hand side of the operation</param>
    /// <param name="y">The right hand side of the operation</param>
    /// <returns>The result of the operation</returns>
    [Pure]
    public static A append<SEMI, A>(A x, A y) where SEMI : Semigroup<A> =>
        SEMI.Append(x, y);

    /// <summary>
    /// An associative binary operation
    /// </summary>
    /// <param name="lhs">Left-hand side of the operation</param>
    /// <param name="rhs">Right-hand side of the operation</param>
    /// <returns>lhs + rhs</returns>
    [Pure]
    public static Either<L, R> append<SEMI, L, R>(Either<L, R> lhs, Either<L, R> rhs) where SEMI : Semigroup<R> =>
        from x in lhs
        from y in rhs
        select SEMI.Append(x, y);

    /// <summary>
    /// An associative binary operation
    /// </summary>
    /// <param name="x">The left hand side of the operation</param>
    /// <param name="y">The right hand side of the operation</param>
    /// <returns>The result of the operation</returns>
    [Pure]
    public static HashMap<K, V> append<K, V>(HashMap<K, V> x, HashMap<K, V> y) =>
        MHashMap<K, V>.Append(x, y);

    /// <summary>
    /// An associative binary operation
    /// </summary>
    /// <param name="x">The left hand side of the operation</param>
    /// <param name="y">The right hand side of the operation</param>
    /// <returns>The result of the operation</returns>
    [Pure]
    public static HashSet<A> append<A>(HashSet<A> x, HashSet<A> y) =>
        MHashSet<A>.Append(x, y);

    /// <summary>
    /// An associative binary operation
    /// </summary>
    /// <param name="x">The left hand side of the operation</param>
    /// <param name="y">The right hand side of the operation</param>
    /// <returns>The result of the operation</returns>
    [Pure]
    public static Lst<A> append<A>(Lst<A> x, Lst<A> y) =>
        MLst<A>.Append(x, y);

    /// <summary>
    /// An associative binary operation
    /// </summary>
    /// <param name="x">The left hand side of the operation</param>
    /// <param name="y">The right hand side of the operation</param>
    /// <returns>The result of the operation</returns>
    [Pure]
    public static Map<K, V> append<K, V>(Map<K, V> x, Map<K, V> y) =>
        MMap<K, V>.Append(x, y);

    /// <summary>
    /// An associative binary operation
    /// </summary>
    /// <param name="x">The left hand side of the operation</param>
    /// <param name="y">The right hand side of the operation</param>
    /// <returns>The result of the operation</returns>
    [Pure]
    public static NEWTYPE append<NEWTYPE, SEMI, A>(NewType<NEWTYPE, A> x, NewType<NEWTYPE, A> y)
        where NEWTYPE : NewType<NEWTYPE, A>
        where SEMI : Semigroup<A> =>
        from a in x
        from b in y
        select SEMI.Append(a, b);

    /// <summary>
    /// An associative binary operation
    /// </summary>
    /// <param name="x">The left hand side of the operation</param>
    /// <param name="y">The right hand side of the operation</param>
    /// <returns>The result of the operation</returns>
    [Pure]
    public static NUMTYPE append<NUMTYPE, NUM, A>(NumType<NUMTYPE, NUM, A> x, NumType<NUMTYPE, NUM, A> y)
        where NUMTYPE : NumType<NUMTYPE, NUM, A>
        where NUM : Num<A> =>
        from a in x
        from b in y
        select NUM.Append(a, b);

    /// <summary>
    /// An associative binary operation
    /// </summary>
    /// <param name="x">The left hand side of the operation</param>
    /// <param name="y">The right hand side of the operation</param>
    /// <returns>The result of the operation</returns>
    [Pure]
    public static NEWTYPE append<NEWTYPE, SEMI, A, PRED>(NewType<NEWTYPE, A, PRED> x, NewType<NEWTYPE, A, PRED> y)
        where NEWTYPE : NewType<NEWTYPE, A, PRED>
        where PRED    : Pred<A>
        where SEMI    : Semigroup<A> =>
        from a in x
        from b in y
        select SEMI.Append(a, b);

    /// <summary>
    /// An associative binary operation
    /// </summary>
    /// <param name="x">The left hand side of the operation</param>
    /// <param name="y">The right hand side of the operation</param>
    /// <returns>The result of the operation</returns>
    [Pure]
    public static NUMTYPE append<NUMTYPE, NUM, A, PRED>(NumType<NUMTYPE, NUM, A, PRED> x, NumType<NUMTYPE, NUM, A, PRED> y)
        where NUMTYPE : NumType<NUMTYPE, NUM, A, PRED>
        where PRED    : Pred<A>
        where NUM     : Num<A> =>
        from a in x
        from b in y
        select NUM.Append(a, b);

    /// <summary>
    /// An associative binary operation
    /// </summary>
    /// <param name="x">The left hand side of the operation</param>
    /// <param name="y">The right hand side of the operation</param>
    /// <returns>The result of the operation</returns>
    [Pure]
    public static Option<A> append<SEMI, A>(Option<A> x, Option<A> y) where SEMI : Semigroup<A> =>
        from a in x
        from b in y
        select SEMI.Append(a, b);

    /// <summary>
    /// An associative binary operation
    /// </summary>
    /// <param name="x">The left hand side of the operation</param>
    /// <param name="y">The right hand side of the operation</param>
    /// <returns>The result of the operation</returns>
    [Pure]
    public static IEnumerable<A> append<SEMI, A>(IEnumerable<A> x, IEnumerable<A> y) where SEMI : Semigroup<A>
    {
        foreach (var a in x)
        foreach (var b in y)
            yield return SEMI.Append(a, b);
    }

    /// <summary>
    /// An associative binary operation
    /// </summary>
    /// <param name="x">The left hand side of the operation</param>
    /// <param name="y">The right hand side of the operation</param>
    /// <returns>The result of the operation</returns>
    [Pure]
    public static Try<A> append<SEMI, A>(Try<A> x, Try<A> y) where SEMI : Semigroup<A> =>
        from a in x
        from b in y
        select SEMI.Append(a, b);

    /// <summary>
    /// An associative binary operation
    /// </summary>
    /// <param name="x">The left hand side of the operation</param>
    /// <param name="y">The right hand side of the operation</param>
    /// <returns>The result of the operation</returns>
    [Pure]
    public static TryOption<A> append<SEMI, A>(TryOption<A> x, TryOption<A> y) where SEMI : Semigroup<A> =>
        from a in x
        from b in y
        select SEMI.Append(a, b);
}
