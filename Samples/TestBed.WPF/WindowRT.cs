using LanguageExt.Common;
using LanguageExt.Effects;

namespace TestBed.WPF;

/// <summary>
/// A window base-type that bakes in a runtime 
/// </summary>
public class WindowRT : WindowIO<MinRT>
{
    public WindowRT(MinRT runtime) : base(runtime)
    {
    }
}
