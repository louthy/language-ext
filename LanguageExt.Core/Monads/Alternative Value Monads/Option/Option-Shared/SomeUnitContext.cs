using System;
using static LanguageExt.Prelude;

namespace LanguageExt;

/// <summary>
/// Provides a fluent context when calling the Some(Action) method from
/// Optional<A> trait.  Must call None(Action) or None(Value) on this 
/// context to complete the matching operation.
/// </summary>
/// <typeparam name="A">Bound optional value type</typeparam>
public class SomeUnitContext<A>
{
    readonly Option<A> option;
    readonly Action<A> someHandler;
    Action? noneHandler;

    internal SomeUnitContext(Option<A> option, Action<A> someHandler)
    {
        this.option      = option;
        this.someHandler = someHandler;
    }

    /// <summary>
    /// The None branch of the matching operation
    /// </summary>
    /// <param name="noneHandler">None branch operation</param>
    public Unit None(Action f)
    {
        noneHandler = f;
        return option.Match(HandleSome, HandleNone);
    }

    Unit HandleSome(A value)
    {
        someHandler(value);
        return unit;
    }

    Unit HandleNone()
    {
        noneHandler?.Invoke();
        return unit;
    }
}
