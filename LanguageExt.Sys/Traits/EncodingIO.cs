using System.Text;

namespace LanguageExt.Sys.Traits;

public interface EncodingIO
{
    IO<Encoding> Encoding { get; } 
}
