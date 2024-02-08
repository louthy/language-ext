using System;

namespace LanguageExt;

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
