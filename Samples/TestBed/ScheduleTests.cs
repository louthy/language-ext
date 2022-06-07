using LanguageExt;
using static LanguageExt.Prelude;
using System;

namespace TestBed;

public class ScheduleTests
{
    public static void Run()
    {
        var results = Schedule.linear(1 * sec) | Schedule.recurs(3) | Schedule.repeat(3);

        Console.WriteLine(results.Run().ToSeq());
    }
}
