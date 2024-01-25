using LanguageExt.TypeClasses;
using System;

namespace LanguageExt;

/// <summary>
/// Provides a fluent context when calling the Some(Func) method from
/// a member of the Optional<A> trait.  Must call None(Func) or 
/// None(Value) on this context to complete the matching operation.
/// </summary>
/// <typeparam name="A">Bound optional value type</typeparam>
/// <typeparam name="B">The operation return value type</typeparam>
public class SomeContext<OPT, OA, A, B> where OPT : Optional<OA, A>
{
    readonly OA option;
    readonly Func<A, B> someHandler;

    internal SomeContext(OA option, Func<A, B> someHandler)
    {
        this.option      = option;
        this.someHandler = someHandler;
    }

    /// <summary>
    /// The None branch of the matching operation
    /// </summary>
    /// <param name="noneHandler">None branch operation</param>
    public B None(Func<B> noneHandler) =>
        OPT.Match(option, someHandler, noneHandler);

    /// <summary>
    /// The None branch of the matching operation
    /// </summary>
    /// <param name="noneHandler">None branch operation</param>
    public B None(B noneValue) =>
        OPT.Match(option, someHandler, noneValue);
}

/// <summary>
/// Provides a fluent context when calling the Some(Func) method from
/// a member of the Optional<A> trait.  Must call None(Func) or 
/// None(Value) on this context to complete the matching operation.
/// </summary>
/// <typeparam name="A">Bound optional value type</typeparam>
/// <typeparam name="B">The operation return value type</typeparam>
public class SomeUnsafeContext<OPT, OA, A, B> where OPT : OptionalUnsafe<OA, A>
{
    readonly OA option;
    readonly Func<A, B> someHandler;

    internal SomeUnsafeContext(OA option, Func<A, B> someHandler)
    {
        this.option      = option;
        this.someHandler = someHandler;
    }

    /// <summary>
    /// The None branch of the matching operation
    /// </summary>
    /// <param name="noneHandler">None branch operation</param>
    public B None(Func<B> noneHandler) =>
        OPT.MatchUnsafe(option, someHandler, noneHandler);

    /// <summary>
    /// The None branch of the matching operation
    /// </summary>
    /// <param name="noneHandler">None branch operation</param>
    public B None(B noneValue) =>
        OPT.MatchUnsafe(option, someHandler, noneValue);
}
