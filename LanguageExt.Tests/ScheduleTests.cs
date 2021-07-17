using Xunit;

namespace LanguageExt.Tests
{
    public class ScheduleTests
    {
        [Fact]
        public void ScheduleUnionTest1()
        {
            var schedule = Schedule.Recurs(10) | Schedule.Spaced(100);

            Assert.True(schedule.Repeats == 10);
            Assert.True(schedule.Spacing == 100);
        }
    }
}
