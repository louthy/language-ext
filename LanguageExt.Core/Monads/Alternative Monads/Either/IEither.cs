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

    R MatchUntyped<R>(Func<object?, R> Left, Func<object?, R> Right);

    Type GetUnderlyingRightType();
    Type GetUnderlyingLeftType();
}
