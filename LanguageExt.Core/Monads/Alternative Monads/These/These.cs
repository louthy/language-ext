using System;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

public abstract partial record These<A, B> : K<These<A>, B>
{
    /// <summary>
    /// Stop other types deriving from These
    /// </summary>
    private These() {}
    
    /// <summary>
    /// Case analysis for the `These` type
    /// </summary>
    /// <param name="This">Match for `This` state</param>
    /// <param name="That">Match for `That` state</param>
    /// <param name="Both">Match for `Both` state</param>
    /// <typeparam name="C">Result type</typeparam>
    /// <returns>Result of running either `This`, `That`, or `Both`</returns>
    public abstract C Match<C>(Func<A, C> This, Func<B, C> That, Func<A, B, C> Both);

    /// <summary>
    /// Takes two default values and produces a tuple
    /// </summary>
    /// <param name="x">Default value A</param>
    /// <param name="y">Default value B</param>
    /// <returns>Tuple</returns>
    public abstract (A, B) ToTuple(A x, B y);

    /// <summary>
    /// Bi-functor map operation
    /// </summary>
    /// <param name="This">Mapping of `This`</param>
    /// <param name="That">Mapping of `That`</param>
    /// <typeparam name="C">Resulting `This` bound value type</typeparam>
    /// <typeparam name="D">Resulting `That` bound value type</typeparam>
    /// <returns></returns>
    public abstract These<C, D> BiMap<C, D>(Func<A, C> This, Func<B, D> That)
        where C : Semigroup<C>;

    /// <summary>
    /// Functor map operation
    /// </summary>
    /// <param name="f">Mapping function</param>
    public abstract These<A, C> Map<C>(Func<B, C> f);

    /// <summary>
    /// Traverse
    /// </summary>
    public K<F, These<A, C>> Traverse<F, C>(Func<B, K<F, C>> f) 
        where F : Applicative<F> =>
        this.Kind().Traverse(f).Map(ac => ac.As());
    
    /// <summary>
    /// Bi-map and coalesce results with the provided operation.
    /// </summary>
    /// <param name="This">This mapping</param>
    /// <param name="That">That mapping</param>
    /// <param name="Both">Both mapping</param>
    /// <typeparam name="C"></typeparam>
    /// <returns></returns>
    public C Merge<C>(Func<A, C> This, Func<B, C> That, Func<C, C, C> Both) 
        where C : Semigroup<C> =>
        BiMap(This, That).Merge(Both);
}
