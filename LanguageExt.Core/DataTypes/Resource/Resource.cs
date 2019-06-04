using LanguageExt.ClassInstances;
using LanguageExt.TypeClasses;
using System;

namespace LanguageExt
{
    public delegate (A Value, HashMap<IDisposable, bool> Resources, bool IsFaulted) Resource<A>();
}
