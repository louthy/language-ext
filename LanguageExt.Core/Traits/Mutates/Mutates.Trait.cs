using System;

namespace LanguageExt.Traits;

/// <summary>
/// Makes a value, within a bound `State`, mutable via a mapping function
/// </summary>
/// <typeparam name="M">State trait</typeparam>
/// <typeparam name="OUTER_STATE">The internal state of the `M` type</typeparam>
/// <typeparam name="INNER_STATE">The value extracted from the `OUTER_STATE` by the implementation
/// of `Modify`, so that it can be mapped via `Modify` and then re-wrapped up into the `OUTER_STATE`</typeparam>
public interface Mutates<in M, OUTER_STATE, INNER_STATE> : Reads<M, OUTER_STATE, INNER_STATE>  
    where M : State<M, OUTER_STATE>
{
    /// <summary>
    /// Extracts the `INNER_STATE` from the `OUTER_STATE`, passes it to the `f`
    /// mapping functions, and then wraps it back up into the generic `M<Unit>` type.
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <returns>Mapped value</returns>
    K<M, Unit> Modify(Func<INNER_STATE, INNER_STATE> f);
}
