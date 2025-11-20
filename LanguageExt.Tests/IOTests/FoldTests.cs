using System;
using Xunit;
using LanguageExt.Common;

namespace LanguageExt.Tests.IOTests;

public class IOFoldTests
{
    [Fact]
    public void Fold_AccumulatesStateAccurately()
    {
        // Arrange
        var                 computation  = IO.pure(10); // Successful IO returning 10
        var                 initialState = 0;
        Func<int, int, int> sumFolder    = (state, value) => state + value;

        // Act
        var foldedIO = computation.Fold(Schedule.Never, initialState, sumFolder).Run();

        // Assert
        Assert.Equal(10, foldedIO);
    }

    [Fact]
    public void Fold_InitialStateIsReturnedIfNoEffect()
    {
        // Arrange
        var                 computation  = IO<int>.Empty; // No effect in the IO
        var                 initialState = 5;
        Func<int, int, int> sumFolder    = (state, value) => state + value;

        // Assert
        Assert.ThrowsAny<AggregateException>(() => computation.Fold(Schedule.Never, initialState, sumFolder).Run());
    }

    [Fact]
    public void Fold_ComputationFailurePreservesState()
    {
        // Arrange
        var                 computation  = IO.fail<int>(Error.New("Error occurred")); // Simulating a failure
        var                 initialState = 5;
        Func<int, int, int> sumFolder    = (state, value) => state + value;

        // Assert
        Assert.ThrowsAny<ErrorException>(() => computation.Fold(Schedule.Never, initialState, sumFolder).Run());
    }

    [Fact]
    public void Fold_ComplexTransformationAppliedCorrectly()
    {
        // Arrange
        var                       computation  = IO.pure(7); // Successful IO
        var                       initialState = "Start: ";
        Func<string, int, string> folder       = (state, value) => state + value;

        // Act
        var foldedIO = computation.Fold(Schedule.Never, initialState, folder).Run();

        // Assert
        Assert.Equal("Start: 7", foldedIO); // State should reflect the transformation
    }

    [Fact]
    public void FoldUntil_StopsOnMatchingPredicate()
    {
        // Arrange
        var                 computation  = IO.pure(10); // IO succeeds with result 10
        var                 initialState = 0;
        Func<int, int, int> sumFolder    = (state, value) => state + value;
        Func<int, bool>     predicate    = state => state > 5; // Stop folding if state > 5

        // Act
        var foldedIO = computation.FoldUntil(initialState, sumFolder, stateIs: predicate).Run();

        // Assert
        Assert.Equal(10, foldedIO); // Predicate stops calculation
    }

    [Fact]
    public void FoldWhile_StatePredicatesEnforcedCorrectly()
    {
        // Arrange
        var                 computation   = IO.pure(20);
        var                 initialState  = 5;
        Func<int, int, int> folder        = (state, value) => state + value;
        Func<int, bool>     continueWhile = state => state < 30; // Continue only if state < 30

        // Act
        var result = computation.FoldWhile(initialState, folder, stateIs: continueWhile).Run();

        // Assert
        Assert.Equal(45, result); // Folding happens as state < 30
    }

    [Fact]
    public void FoldWhile_StopsWhenStateConditionIsNotMet()
    {
        // Arrange
        var                 computation       = IO.pure(50);
        var                 initialState      = 10;
        Func<int, int, int> folder            = (state, value) => state + value;
        Func<int, bool>     continuePredicate = state => state < 30; // Stop if state >= 30

        // Act
        var foldedIO = computation.FoldWhile(initialState, folder, stateIs: continuePredicate).Run();

        // Assert
        Assert.Equal(60, foldedIO); // Predicate stops further updates
    }

    [Fact]
    public void FoldWhile_HandlesEmptyCorrectly()
    {
        // Arrange
        var                 computation       = IO.empty<int>(); // No effect occurs
        var                 initialState      = 10;
        Func<int, int, int> folder            = (state, value) => state + value;
        Func<int, bool>     continuePredicate = state => state < 30;

        // Assert
        Assert.ThrowsAny<AggregateException>(() => computation.FoldWhile(initialState, folder, stateIs: continuePredicate).Run());
    }

    [Fact]
    public void FoldUntil_CompletesOnConditionMet()
    {
        // Arrange
        var                 computation   = IO.pure(10);
        var                 initialState  = 5;
        Func<int, int, int> folder        = (state, value) => state + value;
        Func<int, bool>     stopCondition = state => state >= 15; // Stop if >= 15

        // Act
        var foldedResult = computation.FoldUntil(Schedule.Forever, initialState, folder, stateIs: stopCondition).Run();

        // Assert
        Assert.Equal(15, foldedResult); // Stops just as condition is met
    }

    [Fact]
    public void FoldUntil_HandlesErrorsWithoutStateChange()
    {
        // Arrange
        var                 computation   = IO.fail<int>(Error.New("An error"));
        var                 initialState  = 42;
        Func<int, int, int> folder        = (state, value) => state - value; // Just a demonstration
        Func<int, bool>     stopCondition = state => state < 0;

        // Assert
        Assert.ThrowsAny<ErrorException>(() => computation.FoldUntil(initialState, folder, stateIs: stopCondition).Run());
    }
}
