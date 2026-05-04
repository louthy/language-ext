using System;
using System.Collections.Generic;
using System.Text;

namespace System;

public static class DateTimeOffsetExtensions
{
    public static DateOnly ToDateOnly(this DateTimeOffset dt) =>
        DateOnly.FromDateTime(dt.DateTime);
}
