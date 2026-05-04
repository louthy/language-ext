using System;
using System.Collections.Generic;
using System.Text;

namespace DomainTypesExamples.Capabilities;

public interface SequencesIO
{
    IO<int> NextInt32();
}

public sealed class InMemorySequenceIO : SequencesIO
{
    private int _value = 1;

    public IO<int> NextInt32() => IO.lift(() =>
    {
        var currentValue = _value;

        _value++;

        return currentValue;
    });

}
