using System;
using static LanguageExt.Prelude;
using static LanguageExt.TypeClass;
using LanguageExt.TypeClasses;

namespace LanguageExt
{
    public static partial class TypeClass
    {
        /// <summary>
        /// Iterate the value(s) within the structure
        /// </summary>
        /// <param name="f">Operation to perform on the value(s)</param>
        public static Unit iter<A>(Seq<A> sa, Action<A> f) =>
            sa.Iter(f);
    }
}
