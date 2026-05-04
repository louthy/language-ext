using System;
using System.Collections.Generic;
using System.Text;

namespace DomainTypesExamples.Capabilities;

public interface RandomIO
{
    IO<int> NextInt32(int min, int max);
}

public sealed class InMemoryRandomIO : RandomIO
{
    private int _value = 1;

    public IO<int> NextInt32(int min, int max) => IO.lift(() => Random.Shared.Next(min, max));

}

