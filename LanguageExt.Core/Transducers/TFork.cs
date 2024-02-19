using LanguageExt.Common;
using LanguageExt.Effects;
using LanguageExt.Effects.Traits;

namespace LanguageExt;

/// <summary>
/// Result of forking a transducer
/// </summary>
/// <param name="Cancel">A transducer, which if invoked, would cancel the forked transducer</param>
/// <param name="Await">A transducer, which if invoked, would attempt to get the result of the forked transducer</param>
/// <typeparam name="A"></typeparam>
public readonly record struct TFork<A>(
    Transducer<Unit, Unit> Cancel,
    Transducer<Unit, A> Await);
