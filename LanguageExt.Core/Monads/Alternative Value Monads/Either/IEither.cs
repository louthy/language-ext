using System;
using System.Threading.Tasks;

namespace LanguageExt
{
    public interface IEither
    {
        bool IsRight
        {
            get;
        }

        bool IsLeft
        {
            get;
        }

        TRes MatchUntyped<TRes>(Func<object?, TRes> Right, Func<object?, TRes> Left);

        Type GetUnderlyingRightType();
        Type GetUnderlyingLeftType();
    }

    [Obsolete(Change.UseEffMonadInstead)]
    public interface IEitherAsync
    {
        Task<bool> IsRight
        {
            get;
        }

        Task<bool> IsLeft
        {
            get;
        }

        Type GetUnderlyingRightType();
        Type GetUnderlyingLeftType();
    }
}
