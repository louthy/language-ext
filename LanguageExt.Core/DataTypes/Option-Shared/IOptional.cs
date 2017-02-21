using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt
{
    public interface IOptional
    {
        bool IsSome
        {
            get;
        }

        bool IsNone
        {
            get;
        }

        R MatchUntyped<R>(Func<object, R> Some, Func<R> None);

        Type GetUnderlyingType();
    }
}
