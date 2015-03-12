using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt
{
    public interface IOptionalValue
    {
        bool IsSome
        {
            get;
        }

        bool IsNone
        {
            get;
        }

        object MatchUntyped(Func<object, object> Some, Func<object> None);

        Type GetUnderlyingType();
    }
}
