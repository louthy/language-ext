using System.Text;

namespace LanguageExt.Sys.Live.Implementations;

public class EncodingIO : Sys.Traits.EncodingIO
{
    public static Sys.Traits.EncodingIO Default = new EncodingIO();
    
    public IO<Encoding> Encoding =>
        IO<Encoding>.Pure(System.Text.Encoding.Default);
}
