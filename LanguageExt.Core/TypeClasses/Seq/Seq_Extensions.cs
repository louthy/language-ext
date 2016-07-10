using System;
using static LanguageExt.Prelude;
using static LanguageExt.TypeClass;
using LanguageExt.TypeClasses;

namespace LanguageExt
{
    public static partial class TypeClassExtensions
    {
        /// <summary>
        /// Iterate the value(s) within the structure
        /// </summary>
        /// <param name="f">Operation to perform on the value(s)</param>
        public static Unit Iter<A>(this Seq<A> sa, Action<A> f)
        {
            foreach(var item in sa.ToSeq(sa))
            {
                f(item);
            }
            return unit;
        }
    }
}
