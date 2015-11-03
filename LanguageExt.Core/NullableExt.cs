using System;
using System.Collections.Generic;
using LanguageExt;
using static LanguageExt.Prelude;

public static class __NullableExt
{
    /// <summary>
    /// Convert NullableT to OptionT
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="self">Value to convert</param>
    /// <returns>OptionT with Some or None, depending on HasValue</returns>
    public static Option<T> ToOption<T>(this T? self) where T : struct =>
        self.HasValue
            ? Some(self.Value)
            : None;

    /// <summary>
    /// Convert NullableT to IEnumerableT (0..1 entries)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="self">Value to convert</param>
    /// <returns>Zero or One enumerable values, depending on HasValue</returns>
    public static IEnumerable<T> AsEnumerable<T>(this T? self) where T : struct
    {
        if (self.HasValue)
        {
            yield return self.Value;
        }
    }
}