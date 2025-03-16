using LanguageExt.Sys.Traits;
using LanguageExt.Traits;

namespace LanguageExt.Sys;

public static class Enc<M, RT>
    where M : 
        MonadIO<M>
    where RT : 
        Has<M, EncodingIO>
{
    static readonly K<M, EncodingIO> encIO =
        Has<M, RT, EncodingIO>.ask;

    /// <summary>
    /// Encoding
    /// </summary>
    public static K<M, System.Text.Encoding> encoding =>
        encIO.Bind(e => e.Encoding);
}
