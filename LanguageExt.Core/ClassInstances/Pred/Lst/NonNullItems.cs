using LanguageExt.TypeClasses;
using System;
using System.Collections.Generic;
using System.Text;

namespace LanguageExt.ClassInstances.Pred
{
    public struct NonNullItems<A> : Pred<IReadOnlyList<A>>
    {
        public bool True(IReadOnlyList<A> value)
        {
            foreach(var item in value)
            {
                if (item.IsNull()) return false;
            }
            return true;
        }
    }
}
