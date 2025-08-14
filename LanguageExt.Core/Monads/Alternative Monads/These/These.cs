using System;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

public abstract record These<A, B> : K<These<A>, B>
    where A : Semigroup<A>
{
    /// <summary>
    /// Case analysis for the `These` type
    /// </summary>
    /// <param name="This">Match for `This` state</param>
    /// <param name="That">Match for `That` state</param>
    /// <param name="Pair">Match for `Pair` state</param>
    /// <typeparam name="C">Result type</typeparam>
    /// <returns>Result of running either `This`, `That`, or `Pair`</returns>
    public abstract C Match<C>(Func<A, C> This, Func<B, C> That, Func<A, B, C> Pair);

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
    /// Monad bind operation
    /// </summary>
    /// <param name="f">Chaining function</param>
    public abstract These<A, C> Bind<C>(Func<B, K<These<A>, C>> f);

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
    /// <param name="Pair">Pair mapping</param>
    /// <typeparam name="C"></typeparam>
    /// <returns></returns>
    public C Merge<C>(Func<A, C> This, Func<B, C> That, Func<C, C, C> Pair) 
        where C : Semigroup<C> =>
        BiMap(This, That).Merge(Pair);
}

public sealed record This<A, B>(A Value) : These<A, B>
    where A : Semigroup<A>
{
    public override C Match<C>(Func<A, C> This, Func<B, C> That, Func<A, B, C> Pair) =>
        This(Value);
    
    public override (A, B) ToTuple(A x, B y) =>
        (Value, y);

    public override These<A, C> Map<C>(Func<B, C> f) =>
        new This<A, C>(Value);

    public override These<C, D> BiMap<C, D>(Func<A, C> This, Func<B, D> That) =>
        new This<C, D>(This(Value));   
    
    public override These<A, C> Bind<C>(Func<B, K<These<A>, C>> f) =>
        new This<A, C>(Value);
}

public sealed record That<A, B>(B Value) : These<A, B>
    where A : Semigroup<A>
{
    public override C Match<C>(Func<A, C> This, Func<B, C> That, Func<A, B, C> Pair) =>
        That(Value);
    
    public override (A, B) ToTuple(A x, B y) =>
        (x, Value);

    public override These<A, C> Map<C>(Func<B, C> f) =>
        new That<A, C>(f(Value));
    
    public override These<C, D> BiMap<C, D>(Func<A, C> This, Func<B, D> That) =>
        new That<C, D>(That(Value));  
    
    public override These<A, C> Bind<C>(Func<B, K<These<A>, C>> f) =>
        f(Value).As();
}

public sealed record Pair<A, B>(A First, B Second) : These<A, B>
    where A : Semigroup<A>
{
    public override C Match<C>(Func<A, C> This, Func<B, C> That, Func<A, B, C> Pair) =>
        Pair(First, Second);   
    
    public override (A, B) ToTuple(A x, B y) =>
        (First, Second);    

    public override These<A, C> Map<C>(Func<B, C> f) =>
        new Pair<A, C>(First, f(Second));
    
    public override These<C, D> BiMap<C, D>(Func<A, C> This, Func<B, D> That) =>
        new Pair<C, D>(This(First), That(Second));

    public override These<A, C> Bind<C>(Func<B, K<These<A>, C>> f) =>
        f(Second) switch
        {
            This<A, C> (var a)        => These.This<A, C>(First + a),
            That<A, C> (var b)        => These.Pair(First, b),
            Pair<A, C> (var a, var b) => These.Pair(First + a, b),
            _                         => throw new NotSupportedException()
        };
}
