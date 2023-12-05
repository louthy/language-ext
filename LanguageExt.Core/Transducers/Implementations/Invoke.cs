#nullable enable
namespace LanguageExt;

record Invoke1Reducer<A> : Reducer<A, A?>
{
    public static readonly Reducer<A, A?> Default = new Invoke1Reducer<A>();
                
    public override string ToString() =>  
        "invoke";

    public override TResult<A?> Run(TState state, A? stateValue, A value) => 
        TResult.Continue<A?>(value);
}
