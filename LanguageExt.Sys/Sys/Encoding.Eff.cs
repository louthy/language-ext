using LanguageExt.Sys.Traits;
using LanguageExt.Traits;

namespace LanguageExt.Sys;

public static class Enc<RT>
    where RT : 
        Has<Eff<RT>, EncodingIO>
{
    /// <summary>
    /// Encoding
    /// </summary>
    public static Eff<RT, System.Text.Encoding> encoding =>
        Enc<Eff<RT>, RT>.encoding.As();
}
