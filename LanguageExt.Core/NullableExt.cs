using System;
using LanguageExt;
using LanguageExt.Prelude;

public static class __NullableExt
{
    public static Option<T> ToOption<T>(this Nullable<T> self) where T : struct =>
        self.HasValue
            ? Some(self.Value)
            : None;
}