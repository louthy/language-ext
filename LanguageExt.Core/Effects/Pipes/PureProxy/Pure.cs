using System;
using LanguageExt.Effects.Traits;
using static LanguageExt.Prelude;
using System.Collections.Generic;
using static LanguageExt.Pipes.Proxy;
using System.Runtime.CompilerServices;

namespace LanguageExt.Pipes
{
    public class Pure<A>
    {
        public readonly A Value;
        public Pure(A value) =>
            Value = value;
    }
}
