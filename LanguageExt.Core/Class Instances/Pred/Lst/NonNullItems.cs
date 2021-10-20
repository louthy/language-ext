using LanguageExt.TypeClasses;
using System;
using System.Collections.Generic;
using System.Text;

namespace LanguageExt.ClassInstances.Pred
{
    public struct NonNullItems<A> : Pred<A>
    {
        public bool True(A value) => 
            !value.IsNull();
    }
}
