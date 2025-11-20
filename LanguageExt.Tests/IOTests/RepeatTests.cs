namespace LanguageExt.Tests.IOTests;

using Xunit;
using LanguageExt;

public class IO_RepeatTests
{
    IO<int> CreateIncrementingIO(Atom<int> callCount)
    {
        callCount.Swap(_ => 0);
        return IO.lift(() => callCount.Swap(x => x + 1));
    }

    [Fact]
    public void RepeatWhile_ShouldRepeatUntilConditionFails()
    {
        // Arrange
        var callCount = Atom(0);
        var io        = CreateIncrementingIO(callCount);

        // Act
        var repeatedIo = io.RepeatWhile(value => value < 5);
        var result = repeatedIo.Run();

        // Assert
        Assert.Equal(5, result); // The final value when the condition `value < 5` is no longer true
        Assert.Equal(5, callCount); // Ensure the IO ran 5 times
    }

    [Fact]
    public void RepeatUntil_ShouldStopWhenConditionIsMet()
    {
        // Arrange
        var callCount = Atom(0);
        var io        = CreateIncrementingIO(callCount);

        // Act
        var repeatedIo = io.RepeatUntil(value => value >= 5);
        var result = repeatedIo.Run();

        // Assert
        Assert.Equal(5, result); // The value at which the repetition stops (condition met)
        Assert.Equal(5, callCount); // Ensure the IO ran 5 times
    }

    [Fact]
    public void RepeatWhile_WithTimeSeriesSchedule_ShouldRepeatWhileConditionIsMet()
    {
        // Arrange
        var callCount = Atom(0);
        var io        = CreateIncrementingIO(callCount);
        var schedule = Schedule.TimeSeries(new Duration(10), new Duration(15), new Duration(20));

        // Act
        var repeatedIo = io.RepeatWhile(schedule, value => value < 5);
        var result = repeatedIo.Run();

        // Assert
        Assert.Equal(4, result); // The final value when the condition `value < 5` is no longer true
        Assert.Equal(4, callCount); // Make sure the IO executed 4 times
    }

    [Fact]
    public void RepeatUntil_WithForeverSchedule_ShouldRepeatUntilConditionIsMet()
    {
        // Arrange
        var callCount = Atom(0);
        var io        = CreateIncrementingIO(callCount);
        var schedule  = Schedule.Forever;

        // Act
        var repeatedIo = io.RepeatUntil(schedule, value => value >= 5);
        var result = repeatedIo.Run();

        // Assert
        Assert.Equal(5, result); // The value at which the repetitions stop
        Assert.Equal(5, callCount); // Ensure the IO ran exactly 5 times
    }

    [Fact]
    public void RepeatWhile_FixedIntervalSchedule_ShouldFollowScheduleDelays()
    {
        // Arrange
        var callCount = Atom(0);
        var io        = CreateIncrementingIO(callCount);
        var schedule  = Schedule.fixedInterval(new Duration(1000));

        // Act
        var repeatedIo = io.RepeatWhile(schedule, value => value < 3);
        var result = repeatedIo.Run();

        // Assert
        Assert.Equal(3, result); // Stops after the value reaches 3
        Assert.Equal(3, callCount); // Ensure the IO executed 3 times
    }
}
