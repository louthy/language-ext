using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using static LanguageExt.Prelude;
using LanguageExt.ClassInstances;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using LanguageExt.Traits;

namespace LanguageExt;

public static class AtomQueExtensions
{
    public static AtomQue<A> As<A>(this K<AtomQue, A> ma) =>
        (AtomQue<A>)ma;
}
