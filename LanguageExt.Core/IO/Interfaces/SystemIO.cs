namespace LanguageExt.Interfaces
{
    /// <summary>
    /// Convenience trait - captures the BCL IO behaviour
    /// </summary>
    /// <typeparam name="RT">Runtime</typeparam>
    public interface HasSystem<RT> : 
        HasCancel<RT>, 
        HasConsole<RT>, 
        HasEncoding<RT>,
        HasFile<RT>, 
        HasTextRead<RT>, 
        HasTime<RT>
        where RT : 
            struct, 
            HasCancel<RT>, 
            HasConsole<RT>, 
            HasFile<RT>,
            HasTextRead<RT>,
            HasTime<RT>,
            HasEncoding<RT>
    {
        
    }
}
