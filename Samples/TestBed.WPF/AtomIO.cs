using System;
using LanguageExt;
using LanguageExt.Effects.Traits;
using static LanguageExt.Prelude;
using static LanguageExt.Transducer;

namespace TestBed.WPF;

/// <summary>
/// Placeholder implementation waiting for the real one to be implemented in LanguageExt.Core
/// </summary>
public class AtomIO<RT, E, A>
    where RT : struct, HasIO<RT, E>
{
    public AtomIO(A value) =>
        Value = value;

    public A Value { get; private set; }

    public IO<RT, E, Unit> Swap(Func<A, A> swap) =>
        lift(() =>
        {
            Value = swap(Value);
            return unit;
        });

    public override string ToString() =>
        Value?.ToString() ?? "[null]";
}
