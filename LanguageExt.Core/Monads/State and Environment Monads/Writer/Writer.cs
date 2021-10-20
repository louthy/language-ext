using LanguageExt.ClassInstances;
using LanguageExt.TypeClasses;
using System;

namespace LanguageExt
{
    public delegate (A Value, W Output, bool IsBottom) Writer<MonoidW, W, A>();
}
