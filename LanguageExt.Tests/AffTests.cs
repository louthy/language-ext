using Xunit;
using System;
using System.Threading;
using LanguageExt;
using LanguageExt.Common;
using System.Threading.Tasks;
using FluentAssertions;
using static LanguageExt.Prelude;

namespace AffTests
{
    public class AffMapFailTests
    {
        [Fact] // Fails
        public async Task Aff_WithFailingTaskInAff_EndsWithMapFailValue()
        {
            // Arrange
            var affWithException = Aff(async () => await FailingTask());
            var affWithMappedFailState = affWithException.MapFail(error => Error.New(error.Message + "MapFail Aff"));

            // Act
            var fin = await affWithMappedFailState.Run();
            var result = fin.Match(_ => "Success!", error => error.Message);

            // Assert
            Assert.True(result.IndexOf("MapFail Aff", StringComparison.Ordinal) > 0);
        }

        [Fact] // Fails
        public async Task Aff_WithExceptionInAff_EndsWithMapFailValue()
        {
            // Arrange
            var affWithException = Aff<Unit>(() => throw new Exception());
            var affWithMappedFailState = affWithException.MapFail(error => Error.New(error.Message + "MapFail Aff"));

            // Act
            var fin = await affWithMappedFailState.Run();
            var result = fin.Match(_ => "Success!", error => error.Message);

            // Assert
            Assert.True(result.IndexOf("MapFail Aff", StringComparison.Ordinal) > 0);
        }

        [Fact] // Fails
        public async Task Aff_FailingTaskToAff_EndsWithMapFailValue()
        {
            // Arrange
            var affWithException = FailingTask().ToAff();
            var affWithMappedFailState = affWithException.MapFail(error => Error.New(error.Message + "MapFail Aff"));

            // Act
            var fin = await affWithMappedFailState.Run();
            var result = fin.Match(_ => "Success!", error => error.Message);

            // Assert
            Assert.True(result.IndexOf("MapFail Aff", StringComparison.Ordinal) > 0);
        }

        [Fact] // Succeeds
        public async Task EffToAff_WithExceptionInEff_EndsWithMapFailValue()
        {
            // Arrange
            var effWithException = Eff<Unit>(() => throw new Exception("Error!"));
            var affWithException = effWithException.ToAff();

            var affWithMappedFailState = affWithException.MapFail(error => Error.New(error.Message + "MapFail Aff"));

            // Act
            var fin = await affWithMappedFailState.Run();
            var result = fin.Match(_ => "Success!", error => error.Message);

            // Assert
            Assert.True(result.IndexOf("MapFail Aff", StringComparison.Ordinal) > 0);
        }

        private static async Task<Unit> FailingTask()
        {
            await Task.Delay(1);
            throw new Exception("Error!");
        }

        [Fact(DisplayName = "Fork against Aff<T> can be cancelled")]
        public static void CancelForkTest()
        {
            var counter = Atom(0);

            Aff<int> EffectToFork() =>
                AffMaybe<int>(
                        async () =>
                        {
                            swap(counter, i => i + 1);
                            return await counter.Value.AsValueTask();
                        })
                    .Repeat(Schedule.Forever);

            var daemonEffect = EffectToFork().Fork();

            // start the effect
            var cancelEffect = daemonEffect.Run().ThrowIfFail();

            // cancel the running effect
            cancelEffect.RunUnit();

            counter.Value.Should().BeGreaterThan(0);
        }

        [Fact(DisplayName = "Fork against Aff<T> can be cancelled by the parent that spawned it")]
        public static async Task CancelForkFromParentTest()
        {
            var counter = Atom(0);
            var counter2 = Atom(0);
            var counter3 = Atom(0);
            var counter4 = Atom(0);

            Aff<int> EffectToFork(Atom<int> atom) =>
                AffMaybe<int>(
                        async () =>
                        {
                            swap(atom, i => i + 1);
                            return await atom.Value.AsValueTask();
                        })
                    .Repeat(Schedule.Forever);

            var parentEffect =
                from ct in cancelToken()
                from _1 in EffectToFork(counter).Fork(ct)
                from _2 in fork(EffectToFork(counter2), ct)
                from _3 in EffectToFork(counter3).ForkWithLinkedCancel()
                from _4 in forkWithLinkedCancel(EffectToFork(counter4))
                select _1;

            // start the effect
            var cts = new CancellationTokenSource();
            var result = await parentEffect.Run(cts.Token);

            // cancel the running effect
            cts.Cancel();

            result.IsSucc.Should().BeTrue();
            counter.Value.Should().BeGreaterThan(0);
            counter2.Value.Should().BeGreaterThan(0);
            counter3.Value.Should().BeGreaterThan(0);
            counter4.Value.Should().BeGreaterThan(0);
        }

        [Fact(DisplayName = "An Aff can be folded and always returns an S")]
        public static async Task FoldTest()
        {
            var effect = Aff(async () => await 2.AsValueTask());
            var result = await effect.Fold(2, (i, j) => i + j).Run();
            result.ThrowIfFail().Should().Be(4);

            effect = FailAff<int>("Some error");
            result = await effect.Fold(2, (i, j) => i + j).Run();
            result.ThrowIfFail().Should().Be(2);
        }

        [Fact(DisplayName = "Exists against an Aff always returns a boolean")]
        public static async Task ExistsTest()
        {
            var effect = Aff(async () => await 2.AsValueTask());
            var result = await effect.Exists(i => i == 2).Run();
            result.ThrowIfFail().Should().BeTrue();

            result = await effect.Exists(i => i != 2).Run();
            result.ThrowIfFail().Should().BeFalse();

            effect = FailAff<int>("Some error");
            result = await effect.Exists(i => i == 2).Run();
            result.ThrowIfFail().Should().BeFalse();
        }

        [Fact(DisplayName = "ForAll against an Aff always returns a boolean")]
        public static async Task ForAllTest()
        {
            var effect = Aff(async () => await 2.AsValueTask());
            var result = await effect.ForAll(i => i == 2).Run();
            result.ThrowIfFail().Should().BeTrue();

            result = await effect.ForAll(i => i != 2).Run();
            result.ThrowIfFail().Should().BeFalse();

            effect = FailAff<int>("Some error");
            result = await effect.ForAll(i => i == 2).Run();
            result.ThrowIfFail().Should().BeTrue();
        }
    }
}
