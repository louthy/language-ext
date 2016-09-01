using System;
using System.Linq;
using static LanguageExt.Prelude;
using static LanguageExt.TypeClass;
using System.Collections.Generic;
using LanguageExt.TypeClasses;
using LanguageExt.Instances;

namespace LanguageExt.TypeClasses
{
    public static partial class TypeClassExtensions
    {
        public static Monad<Traversable<B>> Traverse<A, B>(this Seq<A> self, Func<A, Monad<B>> bind) =>
            self.Traverse(self, bind);
    }
}
