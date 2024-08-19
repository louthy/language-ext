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

    R MatchUntyped<R>(Func<object?, R> Right, Func<object?, R> Left);

    Type GetUnderlyingRightType();
    Type GetUnderlyingLeftType();
}
