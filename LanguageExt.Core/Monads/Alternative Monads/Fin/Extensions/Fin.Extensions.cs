using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using LanguageExt.Traits;
using LanguageExt.Common;
using NSE = System.NotSupportedException;

namespace LanguageExt;

/// <summary>
/// Extension methods for Fin
/// </summary>
public static partial class FinExtensions
{
    public static Fin<A> As<A>(this K<Fin, A> ma) =>
        (Fin<A>)ma;
    
    /// <summary>
    /// Natural transformation from `Either` to `Fin`
    /// </summary>
    public static Fin<A> ToFin<A>(this Either<Error, A> ma) =>
        ma.Match(Right: Fin.Succ, Left: Fin.Fail<A>);
    
    /// <summary>
    /// Monadic join
    /// </summary>
    [Pure]
    public static Fin<R> Flatten<R>(this Fin<Fin<R>> ma) =>
        ma.Bind(identity);

    /// <summary>
    /// Add the bound values of x and y, uses an Add trait to provide the add
    /// operation for type A.  For example x.Add〈TInteger, int〉(y)
    /// </summary>
    /// <typeparam name="NUM">Num of A</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="x">Left hand side of the operation</param>
    /// <param name="y">Right hand side of the operation</param>
    /// <returns>Fin with y added to x</returns>
    [Pure]
    public static Fin<R> Plus<NUM, R>(this Fin<R> x, Fin<R> y) where NUM : Arithmetic<R> =>
        from a in x
        from b in y
        select NUM.Add(a, b);

    /// <summary>
    /// Find the difference between the two bound values of x and y, uses a Subtract trait 
    /// to provide the subtract operation for type A.  For example x.Subtract〈TInteger, int〉(y)
    /// </summary>
    /// <typeparam name="NUM">Num of A</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="x">Left hand side of the operation</param>
    /// <param name="y">Right hand side of the operation</param>
    /// <returns>Fin with the difference between x and y</returns>
    [Pure]
    public static Fin<R> Subtract<NUM, R>(this Fin<R> x, Fin<R> y) where NUM : Arithmetic<R> =>
        from a in x
        from b in y
        select NUM.Subtract(a, b);

    /// <summary>
    /// Find the product between the two bound values of x and y, uses a Product trait 
    /// to provide the product operation for type A.  For example x.Product〈TInteger, int〉(y)
    /// </summary>
    /// <typeparam name="NUM">Num of A</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="x">Left hand side of the operation</param>
    /// <param name="y">Right hand side of the operation</param>
    /// <returns>Fin with the product of x and y</returns>
    [Pure]
    public static Fin<R> Product<NUM, R>(this Fin<R> x, Fin<R> y) where NUM : Arithmetic<R> =>
        from a in x
        from b in y
        select NUM.Multiply(a, b);

    /// <summary>
    /// Divide the two bound values of x and y, uses a Divide trait to provide the divide
    /// operation for type A.  For example x.Divide〈TDouble, double〉(y)
    /// </summary>
    /// <typeparam name="NUM">Num of A</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="x">Left hand side of the operation</param>
    /// <param name="y">Right hand side of the operation</param>
    /// <returns>Fin x / y</returns>
    [Pure]
    public static Fin<R> Divide<NUM, R>(this Fin<R> x, Fin<R> y) where NUM : Num<R> =>
        from a in x
        from b in y
        select NUM.Divide(a, b);

    extension<F, A>(K<F, Fin<A>> self) where F : Foldable<F>
    {
        /// <summary>
        /// Partitions a foldable of `Fin` into two sequences.
        /// 
        /// All the `Fail` elements are extracted, in order, to the first component of the output.
        /// Similarly, the `Succ` elements are extracted to the second component of the output.
        /// </summary>
        /// <returns>A pair containing the sequences of partitioned values</returns>
        [Pure]
        public (Seq<Error> Fails, Seq<A> Succs) Partition() =>
            self.Fold((Fail: Seq<Error>.Empty, Succ: Seq<A>.Empty),
                      (s, ma) =>
                          ma switch
                          {
                              Fin<A>.Succ (var r) => (s.Fail, s.Succ.Add(r)),
                              Fin<A>.Fail (var l) => (s.Fail.Add(l), s.Succ),
                              _                   => throw new NSE()
                          });

        /// <summary>
        /// Partitions a foldable of `Fin` into two lists and returns the `Fail` items only.
        /// </summary>
        /// <returns>A sequence of partitioned items</returns>
        [Pure]
        public Seq<Error> Fails() =>
            self.Fold(Seq<Error>.Empty,
                      (s, ma) =>
                          ma switch
                          {
                              Fin<A>.Fail (var l) => s.Add(l),
                              _                   => throw new NSE()
                          });

        /// <summary>
        /// Partitions a foldable of `Fin` into two lists and returns the `Succ` items only.
        /// </summary>
        /// <returns>A sequence of partitioned items</returns>
        [Pure]
        public Seq<A> Succs() =>
            self.Fold(Seq<A>.Empty,
                      (s, ma) =>
                          ma switch
                          {
                              Fin<A>.Succ (var r) => s.Add(r),
                              _                   => throw new NSE()
                          });
    }
}
