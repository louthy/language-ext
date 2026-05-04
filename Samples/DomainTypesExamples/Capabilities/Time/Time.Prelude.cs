using System;
using System.Collections.Generic;
using System.Text;
using DomainTypesExamples.Capabilities;

namespace DomainTypesExamples;

public static partial class SamplePrelude
{
    public static Eff<RT, DateTimeOffset> getNow<RT>()
        where RT : HasTime<RT> =>
        TimeEnv.getNow<RT>();
}
