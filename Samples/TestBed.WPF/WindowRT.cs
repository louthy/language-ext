using LanguageExt.Common;
using LanguageExt.Effects;

namespace TestBed.WPF;

public class WindowRT : WindowIO<MinRT, Error>
{
    public WindowRT(MinRT runtime) : base(runtime)
    {
    }
}
