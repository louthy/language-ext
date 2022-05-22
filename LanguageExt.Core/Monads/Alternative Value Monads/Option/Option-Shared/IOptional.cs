#nullable enable
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
        R MatchUntypedUnsafe<R>(Func<object, R?> Some, Func<R?> None);

        Type GetUnderlyingType();
    }

    public interface IOptionalAsync
    {
        Task<bool> IsSome
        {
            get;
        }

        Task<bool> IsNone
        {
            get;
        }

        Task<R> MatchUntyped<R>(Func<object, R> Some, Func<R> None);

        Type GetUnderlyingType();
    }

}
