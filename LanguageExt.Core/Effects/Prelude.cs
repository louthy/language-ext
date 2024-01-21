#nullable enable
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace LanguageExt;

public static partial class Prelude
{
    /// <summary>
    /// Runtime
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static Transducer<RT, RT> runtime<RT>() =>
        lift<RT, RT>(x => x);
}
