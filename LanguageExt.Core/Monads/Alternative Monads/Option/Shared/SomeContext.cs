using System;

namespace LanguageExt;

/// <summary>
/// Provides a fluent context when calling the Some(Func) method from
/// a member of the Optional〈A〉 trait.  Must call None(Func) or 
/// None(Value) on this context to complete the matching operation.
/// </summary>
/// <typeparam name="A">Bound optional value type</typeparam>
/// <typeparam name="B">The operation return value type</typeparam>
public class SomeContext<A, B>
{
    readonly Option<A> option;
    readonly Func<A, B> someHandler;

    internal SomeContext(Option<A> option, Func<A, B> someHandler)
    {
        this.option      = option;
        this.someHandler = someHandler;
    }

    /// <summary>
    /// The None branch of the matching operation
    /// </summary>
    /// <param name="noneHandler">None branch operation</param>
    public B None(Func<B> noneHandler) =>
        option.Match(someHandler, noneHandler);

    /// <summary>
    /// The None branch of the matching operation
    /// </summary>
    /// <param name="noneHandler">None branch operation</param>
    public B None(B noneValue) =>
        option.Match(someHandler, noneValue);
}
