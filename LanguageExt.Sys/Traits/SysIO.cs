using LanguageExt.Traits;

namespace LanguageExt.Sys.Traits;

/// <summary>
/// Convenience trait - captures the BCL IO behaviour
/// </summary>
/// <typeparam name="M">Monad and reader trait</typeparam>
public interface HasSys<in M> : 
    Has<M, ActivitySourceIO>, 
    Has<M, ConsoleIO>, 
    Has<M, FileIO>, 
    Has<M, DirectoryIO>, 
    Has<M, TextReadIO>, 
    Has<M, TimeIO>;
