namespace LanguageExt.Traits;

/// <summary>
/// Makes a value that is within the OUTER_STATE available for reading
/// </summary>
/// <typeparam name="M">State trait</typeparam>
/// <typeparam name="OUTER_STATE">The internal state of the `M` type</typeparam>
/// <typeparam name="INNER_STATE">The value extracted from the `OUTER_STATE` by the implementation of `Get`</typeparam>
public interface Reads<in M, OUTER_STATE, out INNER_STATE> 
    where M : State<M, OUTER_STATE>
{
    K<M, INNER_STATE> Get { get; }
}
