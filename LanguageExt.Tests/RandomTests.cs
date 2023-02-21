#nullable enable

using System;
using System.Linq;
using CodeGeneration.Roslyn;
using FluentAssertions;
using LanguageExt.Sys;
using LanguageExt.Sys.Traits;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests;

using LR = Random<Sys.Live.Runtime>;
using TR = Random<Sys.Test.Runtime>;
using LRX = Random<SysX.Live.Runtime>;
using TRX = Random<SysX.Test.Runtime>;

public static class RandomTests
{
    static Sys.Live.Runtime live = Sys.Live.Runtime.New();    
    static SysX.Live.Runtime liveX = SysX.Live.Runtime.New();
    
    static Sys.Test.Runtime test() => 
        Sys.Test.Runtime.New(seed: 1234567);
    
    static SysX.Test.Runtime testX() => 
        SysX.Test.Runtime.New(seed: 1234567);

    [Fact(DisplayName = "Generate int provides a random int")]
    public static void Case1() => 
        LR.nextInt().Run(live).ThrowIfFail().Should().BePositive();

    [Fact(DisplayName = "Generate int provides a random int between 2 values")]
    public static void Case2() =>
        LR.nextInt(2, 6).Run(live).ThrowIfFail().Should().BePositive().And.BeInRange(2, 6);

    [Fact(DisplayName = "Generate int provides a random int up to a max value")]
    public static void Case3() =>
        LR.nextInt(0, 6).Run(live).ThrowIfFail().Should().BeGreaterOrEqualTo(0).And.BeLessThan(6);

    [Fact(DisplayName = "Generate int provides a random int up to a max value (SysX.Live)")]
    public static void Case3a() =>
        LRX.nextInt(0, 6).Run(liveX).ThrowIfFail().Should().BeGreaterOrEqualTo(0).And.BeLessThan(6);

    [Fact(DisplayName = "Generate int provides a random int up to a max value (SysX.Test)")]
    public static void Case3b() =>
        TRX.nextInt(0, 6).Run(testX()).ThrowIfFail().Should().BeGreaterOrEqualTo(0).And.BeLessThan(6);
    
    sealed class MockRandom : RandomIO
    {
        public int NextInt(int? min = default, int? max = default) => 666;

        public byte[] NextByteArray(long length) => throw new NotImplementedException();

        public double NextDouble() => throw new NotImplementedException();

        public long NextLong() => throw new NotImplementedException();

        public float NextFloat() => throw new NotImplementedException();

        public Guid NextGuid() => throw new NotImplementedException();
    }

    struct CustomRuntime : HasRandom<CustomRuntime>
    {
        public Eff<CustomRuntime, RandomIO> RandomEff =>
            SuccessEff<CustomRuntime, RandomIO>(new MockRandom());
    }

    [Fact(DisplayName = "Random can be mocked")]
    public static void Case4() =>
        Random<CustomRuntime>.nextInt()
            .Run(new CustomRuntime())
            .ThrowIfFail()
            .Should()
            .Be(666);

    [Fact(DisplayName = "Random bytes can be generated to length")]
    public static void Case5() => 
        LR.nextByteArray(7).Run(live).ThrowIfFail().Should().HaveCount(7);
    
    [Fact(DisplayName = "Random bytes can be generated to length (SysX.Live)")]
    public static void Case5a() => 
        LRX.nextByteArray(7).Run(liveX).ThrowIfFail().Should().HaveCount(7);
    
    [Fact(DisplayName = "Random bytes can be generated to length (SysX.Test)")]
    public static void Case5b() => 
        TRX.nextByteArray(7).Run(testX()).ThrowIfFail().Should().HaveCount(7);

    [Fact(DisplayName = "Random doubles can be generated")]
    public static void Case6() => 
        LR.nextDouble().Run(live).ThrowIfFail().Should().BePositive();

    [Fact(DisplayName = "Random longs can be generated")]
    public static void Case7() =>
        LR.nextLong().Run(live).ThrowIfFail().Should().BeInRange(long.MinValue, long.MaxValue);

    [Fact(DisplayName = "Random floats can be generated")]
    public static void Case8() =>
        LR.nextFloat().Run(live).ThrowIfFail().Should().BeInRange(float.MinValue, float.MaxValue);

    [Fact(DisplayName = "Random guids can be generated")]
    public static void Case9() => 
        LR.nextGuid().Run(live).ThrowIfFail().Should().NotBeEmpty();

    [Fact(DisplayName = "Random guids can be generated")]
    public static void Case10() => 
        LR.nextChar().Run(live).ThrowIfFail().Should().NotBeNull();

    [Fact(DisplayName = "Random durations between a range can be generated")]
    public static void Case11() =>
        LR.nextDuration(10 * seconds, 1 * minute)
            .Run(live)
            .ThrowIfFail()
            .Should()
            .BeInRange(10 * seconds, 1 * minute);

    [Fact(DisplayName = "Random time spans between a range can be generated")]
    public static void Case12() =>
        LR.nextTimespan(10 * seconds, 1 * minute)
            .Run(live)
            .ThrowIfFail()
            .Should()
            .BeGreaterThan(TimeSpan.FromSeconds(10))
            .And.BeLessThan(TimeSpan.FromMinutes(1));

    [Fact(DisplayName = "Random date time offsets between a range can be generated")]
    public static void Case13() =>
        LR.nextDateTimeOffset(DateTimeOffset.Now, DateTimeOffset.Now + TimeSpan.FromDays(10))
            .Run(live)
            .ThrowIfFail()
            .Should()
            .BeWithin(TimeSpan.FromDays(10));

    [Fact(DisplayName = "Random date time between a range can be generated")]
    public static void Case14() =>
        LR.nextDateTime(DateTime.Now, DateTime.Now + TimeSpan.FromDays(10))
            .Run(live)
            .ThrowIfFail()
            .Should()
            .BeWithin(TimeSpan.FromDays(10));

    [Fact(DisplayName = "Random ranges can be generated")]
    public static void Case17() =>
        LR.nextRange(LR.nextChar(), 4, 25)
            .Run(live)
            .ThrowIfFail()
            .Should()
            .HaveCountGreaterOrEqualTo(4)
            .And.HaveCountLessThan(25);

    [Fact(DisplayName = "Random ranges can be generated with other random effects")]
    public static void Case18() =>
        LR.nextRange(LR.nextGuid(), 4, 25)
            .Run(live)
            .ThrowIfFail()
            .Should()
            .HaveCountGreaterOrEqualTo(4)
            .And.HaveCountLessThanOrEqualTo(25);

    [Fact(DisplayName = "Generate int provides a random int deterministically")]
    public static void Case19() => 
        TR.nextInt().Run(test()).ThrowIfFail().Should().Be(1673116976);

    [Fact(DisplayName = "Generate int provides a random int between 2 values deterministically")]
    public static void Case20() => 
        TR.nextInt(2, 6).Run(test()).ThrowIfFail().Should().Be(5);

    [Fact(DisplayName = "Generate int provides a random int up to a max value deterministically")]
    public static void Case21() => 
        TR.nextInt(0, 6).Run(test()).ThrowIfFail().Should().Be(4);

    [Fact(DisplayName = "Random bytes can be generated to length deterministically")]
    public static void Case22() =>
        TR.nextByteArray(7)
            .Run(test())
            .ThrowIfFail()
            .Should()
            .BeEquivalentTo(new[] { 0x30, 0xE6, 0x63, 0xCA, 0x5F, 0x41, 0x21 });

    [Fact(DisplayName = "Random doubles can be generated deterministically")]
    public static void Case23() =>
        TR.nextDouble().Run(test()).ThrowIfFail().Should().Be(0.7791058052233913);

    [Fact(DisplayName = "Random doubles within range can be generated deterministically")]
    public static void Case23b() =>
        TR.nextDouble(0.8, 0.9).Run(test()).ThrowIfFail().Should().Be(0.8779105805223392);

    [Fact(DisplayName = "Random longs can be generated deterministically")]
    public static void Case24() =>
        TR.nextLong().Run(test()).ThrowIfFail().Should().Be(5197507324635506224L);

    [Fact(DisplayName = "Random longs within a range can be generated deterministically")]
    public static void Case24b() =>
        TR.nextLong(10L, 900L).Run(test()).ThrowIfFail().Should().Be(264L);

    [Fact(DisplayName = "Random floats can be generated deterministically")]
    public static void Case25() => 
        TR.nextFloat().Run(test()).ThrowIfFail().Should().Be(3.4028233E+38F);

    [Fact(DisplayName = "Random floats within a range can be generated deterministically")]
    public static void Case25b() => 
        TR.nextFloat(0.8F, 0.9F).Run(test()).ThrowIfFail().Should().Be(0.8999999F);

    [Fact(DisplayName = "Random guids can be generated deterministically")]
    public static void Case26() =>
        TR.nextGuid()
            .Run(test())
            .ThrowIfFail()
            .Should()
            .Be(Guid.Parse("ca63e630-415f-4821-1f8f-fbe9db201f70"));

    [Fact(DisplayName = "Random char can be generated deterministically")]
    public static void Case27() => 
        TR.nextChar().Run(test()).ThrowIfFail().Should().Be('i');

    [Fact(DisplayName = "Random durations between a range can be generated deterministically")]
    public static void Case28() =>
        TR.nextDuration(10 * seconds, 1 * minute)
            .Run(test())
            .ThrowIfFail()
            .Should()
            .Be(48955.290261169568);

    [Fact(DisplayName = "Random time spans between a range can be generated deterministically")]
    public static void Case29() =>
        TR.nextTimespan(10 * seconds, 1 * minute)
            .Run(test())
            .ThrowIfFail()
            .Should()
            .Be(TimeSpan.FromMilliseconds(48955.290261169568));

    [Fact(DisplayName = "Random ranges can be generated deterministically")]
    public static void Case30() =>
        TR.nextRange(TR.nextChar(), 4, 25).Run(test()).ThrowIfFail().Should().HaveCount(20);

    [Fact(DisplayName = "Random ranges can be generated with other random effects deterministically")]
    public static void Case31() =>
        TR.nextRange(TR.nextGuid(), 4, 25)
            .Run(test())
            .ThrowIfFail()
            .Should()
            .HaveCount(20)
            .And.Contain(x => x == Guid.Parse("448f4128-f539-deef-e7ba-f20ed04afc32"));

    [Fact(DisplayName = "Random strings can be generated")]
    public static void Case32() =>
        LR.nextString(4, 25).Run(live).ThrowIfFail().Should().NotBeEmpty();

    [Fact(DisplayName = "Random strings can be generated deterministically")]
    public static void Case33() =>
        TR.nextString(4, 25).Run(test()).ThrowIfFail().Should().Be("g9.XyvJR3Ng\"J-v<br`E");

    [Fact(DisplayName = "Random chars in a range can be generated")]
    public static void Case34() =>
        LR.nextChar('a', 'z')
            .Run(live)
            .ThrowIfFail()
            .Should()
            .BeGreaterOrEqualTo('a')
            .And.BeLessThanOrEqualTo('z');

    [Fact(DisplayName = "Random chars in a range can be generated deterministically")]
    public static void Case35() => 
        TR.nextChar('a', 'z').Run(test()).ThrowIfFail().Should().Be('t');

    [Fact(DisplayName = "Random pick will get a random element from the collection")]
    public static void Case36() =>
        LR.uniform(1, 2, 3, 4, 5)
            .Run(live)
            .ThrowIfFail()
            .Should()
            .BeGreaterThan(0)
            .And.BeLessThan(6);

    [Fact(DisplayName = "Random pick will get a random element from the collection deterministically")]
    public static void Case37() =>
        TR.uniform<int>(List(1, 2, 3, 4, 5)).Run(test()).ThrowIfFail().Should().Be(4);

    [Fact(DisplayName = "Random enum will get a random enum from the enumeration")]
    public static void Case38() =>
        LR.nextEnum<LogLevel>().Run(live).ThrowIfFail().Should().BeOneOf(Enum.GetValues<LogLevel>());

    [Fact(DisplayName = "Random enum will get a random enum from the enumeration deterministically")]
    public static void Case39() =>
        TR.nextEnum<LogLevel>().Run(test()).ThrowIfFail().Should().Be(LogLevel.Warning);

    [Fact(DisplayName = "Random boolean can be generated deterministically")]
    public static void Case40() => 
        TR.nextBool().Run(test()).ThrowIfFail().Should().BeTrue();

    [Fact(DisplayName = "Random weighted pick can be returned")]
    public static void Case41()
    {
        var result = TR.weighted(
                (5, 1), (15, 2), (30, 3), (30, 4), (15, 5), (5, 6)
            )
            .Fold(Schedule.recurs(1000), Seq.empty<int>(), (seq, i) => seq.Add(i))
            .Run(test())
            .ThrowIfFail()
            .GroupBy(x => x)
            .ToDictionary(x => x.Key, x => Math.Round((double)x.Count() / 1000 * 100));
        result[1].Should().Be(5);
        result[2].Should().Be(14);
        result[3].Should().Be(31);
        result[4].Should().Be(29);
        result[5].Should().Be(15);
        result[6].Should().Be(5);
    }

    [Fact(DisplayName = "Random weighted pick can be returned with a sum of weights less than 100")]
    public static void Case42()
    {
        var result = TR.weighted<int>(List((1, 1), (1, 2), (30, 3), (30, 4), (1, 5), (1, 6)))
            .Fold(Schedule.recurs(1000), Seq.empty<int>(), (seq, i) => seq.Add(i))
            .Run(test())
            .ThrowIfFail()
            .GroupBy(x => x)
            .ToDictionary(x => x.Key, x => Math.Round((double)x.Count() / 1000 * 100));
        result[1].Should().Be(1);
        result[2].Should().Be(1);
        result[3].Should().Be(48);
        result[4].Should().Be(46);
        result[5].Should().Be(2);
        result[6].Should().Be(2);
    }

    [Fact(DisplayName = "Random weighted pick can be returned with a sum of weights greater than 100")]
    public static void Case43()
    {
        var result = TR.weighted<int>(
                List((30, 1), (30, 2), (30, 3), (30, 4), (1, 5), (1, 6))
            )
            .Fold(Schedule.recurs(1000), Seq.empty<int>(), (seq, i) => seq.Add(i))
            .Run(test())
            .ThrowIfFail()
            .GroupBy(x => x)
            .ToDictionary(x => x.Key, x => Math.Round((double)x.Count() / 1000 * 100));
        result[1].Should().Be(24);
        result[2].Should().Be(25);
        result[3].Should().Be(24);
        result[4].Should().Be(25);
        result[5].Should().Be(1);
        result[6].Should().Be(1);
    }

    [Fact(DisplayName = "Random tuples can be generated")]
    public static void Case44() => (
            from a in TR.nextInt()
            from b in TR.nextInt()
            select (a, b)).Run(test())
        .ThrowIfFail()
        .Should()
        .Be((1673116976, 1631631590));

    [Fact(DisplayName = "Lists can be shuffled")]
    public static void Case45() =>
        TR.shuffle(1, 2, 3, 4, 5)
            .Run(test())
            .ThrowIfFail()
            .AsEnumerable()
            .Should()
            .ContainInOrder(2, 3, 1, 5, 4);
}
