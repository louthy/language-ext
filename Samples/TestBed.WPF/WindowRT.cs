using LanguageExt.Common;
using LanguageExt.Effects;

namespace TestBed.WPF;

public class WindowRT : WindowIO<MinimalRT, Error>
{
    public WindowRT(MinimalRT runtime) : base(runtime)
    {
    }
}
