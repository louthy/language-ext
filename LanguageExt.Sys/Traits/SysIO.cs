using LanguageExt.Effects.Traits;

namespace LanguageExt.Sys.Traits
{
    /// <summary>
    /// Convenience trait - captures the BCL IO behaviour
    /// </summary>
    /// <typeparam name="RT">Runtime</typeparam>
    public interface HasSys<RT> : 
        HasCancel<RT>, 
        HasConsole<RT>, 
        HasEncoding<RT>,
        HasFile<RT>, 
        HasDirectory<RT>,
        HasTextRead<RT>, 
        HasTime<RT>
        where RT : 
            struct, 
            HasCancel<RT>, 
            HasConsole<RT>, 
            HasFile<RT>,
            HasDirectory<RT>,
            HasTextRead<RT>,
            HasTime<RT>,
            HasEncoding<RT>
    {
    }
}
