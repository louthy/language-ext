#nullable enable
namespace LanguageExt.Transducers;

record Invoke1Reducer<A> : Reducer<A, A?>
{
    public static readonly Reducer<A, A?> Default = new Invoke1Reducer<A>();
    
    public override TResult<A?> Run(TState state, A? stateValue, A value) => 
        TResult.Continue<A?>(value);
}
