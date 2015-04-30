using System;
using System.Collections.Generic;
using LanguageExt;
using static LanguageExt.Prelude;

public static class __NullableExt
{
    public static Option<T> ToOption<T>(this Nullable<T> self) where T : struct =>
        self.HasValue
            ? Some(self.Value)
            : None;

    public static IEnumerable<T> AsEnumerable<T>(this Nullable<T> self) where T : struct
    {
        if (self.HasValue)
        {
            yield return self.Value;
        }
    }
}