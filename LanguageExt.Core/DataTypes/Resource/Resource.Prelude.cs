
using LanguageExt.ClassInstances;
using LanguageExt.TypeClasses;
using static LanguageExt.TypeClass;
using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Collections.Generic;

namespace LanguageExt
{
    public static partial class Prelude
    {
        public static Resource<R> Use<R>(Func<R> acquire) where R : IDisposable =>
            default(MResource<R>).Use(acquire);


        public static Resource<Unit> release<R>(R resource) where R : IDisposable =>
            default(MResource<R>).Release(
            default(MResource<R>).Use(() => resource));
    }
}