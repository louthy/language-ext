using System;
using LanguageExt.UnitsOfMeasure;

using static LanguageExt.Prelude;

namespace LanguageExt
{
    static class TimeAttr
    {
        public static Option<Time> TryParse(double value, string unit)
        {
            switch (unit)
            {
                case "m":
                case "min":
                case "mins":
                case "minute":
                case "minutes":
                    return value.Minutes();

                case "s":
                case "sec":
                case "secs":
                case "second":
                case "seconds":
                    return value.Seconds();

                case "hr":
                case "hour":
                case "hours":
                    return value.Hours();

                default:
                    return None;
            }
        }
    }
}
