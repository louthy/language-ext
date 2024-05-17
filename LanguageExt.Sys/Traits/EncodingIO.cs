using System.Text;
using System.Threading;
using LanguageExt.Attributes;
using LanguageExt.Common;
using LanguageExt.Effects.Traits;

namespace LanguageExt.Sys.Traits;

public interface EncodingIO
{
    IO<Encoding> Encoding { get; } 
}
