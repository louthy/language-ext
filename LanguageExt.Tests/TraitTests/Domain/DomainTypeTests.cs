using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LanguageExt.Common;
using LanguageExt.Traits.Domain;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests;

public sealed class OnlyTrueBoolean : 
    DomainTypeFactory<OnlyTrueBoolean, bool>
{
    private readonly bool _value;

    private OnlyTrueBoolean(bool value) =>
        _value = value;

    public static Fin<OnlyTrueBoolean> From(bool repr) =>
        repr ? new OnlyTrueBoolean(repr) : Error.New("Invalid value");

    public bool To() => _value;
}

public sealed class OnlyDigitChar : 
    DomainTypeFactory<OnlyDigitChar, char>
{
    private readonly char _value;

    private OnlyDigitChar(char value) =>
        _value = value;

    public static Fin<OnlyDigitChar> From(char repr) =>
        char.IsDigit(repr) ? new OnlyDigitChar(repr) : Error.New("Invalid value");

    public char To() => _value;
}

public sealed class OnlyDigitString : 
    DomainTypeFactory<OnlyDigitString, string>
{
    private readonly string _value;

    private OnlyDigitString(string value) =>
        _value = value;

    public static Fin<OnlyDigitString> From(string repr) =>
        repr.All(char.IsDigit) ? new OnlyDigitString(repr) : Error.New("Invalid value");

    public string To() => _value;
}

public sealed class OddOnlyByte : 
    DomainTypeFactory<OddOnlyByte, byte>
{
    private readonly byte _value;

    private OddOnlyByte(byte value) =>
        _value = value;

    public static Fin<OddOnlyByte> From(byte repr) =>
        repr % 2 == 1 ? new OddOnlyByte(repr) : Error.New("Invalid value");

    public byte To() => _value;
}

public sealed class OddOnlyInt16 : 
    DomainTypeFactory<OddOnlyInt16, short>
{
    private readonly short _value;

    private OddOnlyInt16(short value) =>
        _value = value;

    public static Fin<OddOnlyInt16> From(short repr) =>
        repr % 2 == 1 ? new OddOnlyInt16(repr) : Error.New("Invalid value");

    public short To() => _value;
}

public sealed class OddOnlyUInt16 : DomainTypeFactory<OddOnlyUInt16, ushort>
{
    private readonly ushort _value;

    private OddOnlyUInt16(ushort value) =>
        _value = value;

    public static Fin<OddOnlyUInt16> From(ushort repr) =>
        repr % 2 == 1 ? new OddOnlyUInt16(repr) : Error.New("Invalid value");

    public ushort To() => _value;
}

public sealed class OddOnlyInt32 : 
    DomainTypeFactory<OddOnlyInt32, int>
{
    private readonly int _value;

    private OddOnlyInt32(int value) =>
        _value = value;

    public static Fin<OddOnlyInt32> From(int repr) =>
        repr % 2 == 1 ? new OddOnlyInt32(repr) : Error.New("Invalid value");

    public int To() => _value;
}

public sealed class OddOnlyUInt32 : 
    DomainTypeFactory<OddOnlyUInt32, uint>
{
    private readonly uint _value;

    private OddOnlyUInt32(uint value) =>
        _value = value;

    public static Fin<OddOnlyUInt32> From(uint repr) =>
        repr % 2 == 1 ? new OddOnlyUInt32(repr) : Error.New("Invalid value");

    public uint To() => _value;
}

public sealed class OddOnlyInt64 : 
    DomainTypeFactory<OddOnlyInt64, long>
{
    private readonly long _value;

    private OddOnlyInt64(long value) =>
        _value = value;

    public static Fin<OddOnlyInt64> From(long repr) =>
        repr % 2 == 1 ? new OddOnlyInt64(repr) : Error.New("Invalid value");

    public long To() => _value;
}

public sealed class OddOnlyUInt64 : 
    DomainTypeFactory<OddOnlyUInt64, ulong>
{
    private readonly ulong _value;

    private OddOnlyUInt64(ulong value) =>
        _value = value;

    public static Fin<OddOnlyUInt64> From(ulong repr) =>
        repr % 2 == 1 ? new OddOnlyUInt64(repr) : Error.New("Invalid value");

    public ulong To() => _value;
}

public sealed class PositiveOnlySingle : 
    DomainTypeFactory<PositiveOnlySingle, float>
{
    private readonly float _value;
    private PositiveOnlySingle(float value) =>
        _value = value;
    public static Fin<PositiveOnlySingle> From(float repr) =>
        repr > 0 ? new PositiveOnlySingle(repr) : Error.New("Invalid value");
    public float To() => _value;
}

public sealed class PositiveOnlyDouble : 
    DomainTypeFactory<PositiveOnlyDouble, double>
{
    private readonly double _value;
    private PositiveOnlyDouble(double value) =>
        _value = value;
    public static Fin<PositiveOnlyDouble> From(double repr) =>
        repr > 0 ? new PositiveOnlyDouble(repr) : Error.New("Invalid value");
    public double To() => _value;
}

public sealed class PositiveOnlyDecimal : 
    DomainTypeFactory<PositiveOnlyDecimal, decimal>
{
    private readonly decimal _value;
    private PositiveOnlyDecimal(decimal value) =>
        _value = value;
    public static Fin<PositiveOnlyDecimal> From(decimal repr) =>
        repr > 0 ? new PositiveOnlyDecimal(repr) : Error.New("Invalid value");
    public decimal To() => _value;
}

public sealed class PastOnlyDate : 
    DomainTypeFactory<PastOnlyDate, DateOnly>
{
    private readonly DateOnly _value;
    private PastOnlyDate(DateOnly value) =>
        _value = value;
    public static Fin<PastOnlyDate> From(DateOnly repr) =>
        repr < DateOnly.FromDateTime(DateTime.UtcNow) ? new PastOnlyDate(repr) : Error.New("Invalid value");
    public DateOnly To() => _value;
}

public sealed class MorningOnlyTime : 
    DomainTypeFactory<MorningOnlyTime, TimeOnly>
{
    private readonly TimeOnly _value;
    private MorningOnlyTime(TimeOnly value) =>
        _value = value;
    public static Fin<MorningOnlyTime> From(TimeOnly repr) =>
        repr < TimeOnly.FromTimeSpan(TimeSpan.FromHours(12)) ? new MorningOnlyTime(repr) : Error.New("Invalid value");
    public TimeOnly To() => _value;
}

public sealed class PastOnlyDateTime : 
    DomainTypeFactory<PastOnlyDateTime, DateTime>
{
    private readonly DateTime _value;
    private PastOnlyDateTime(DateTime value) =>
        _value = value;
    public static Fin<PastOnlyDateTime> From(DateTime repr) =>
        repr < DateTime.UtcNow ? new PastOnlyDateTime(repr) : Error.New("Invalid value");
    public DateTime To() => _value;
}

public sealed class FutureOnlyDateTimeOffset : 
    DomainTypeFactory<FutureOnlyDateTimeOffset, DateTimeOffset>
{
    private readonly DateTimeOffset _value;
    private FutureOnlyDateTimeOffset(DateTimeOffset value) =>
        _value = value;
    public static Fin<FutureOnlyDateTimeOffset> From(DateTimeOffset repr) =>
        repr > DateTimeOffset.UtcNow ? new FutureOnlyDateTimeOffset(repr) : Error.New("Invalid value");
    public DateTimeOffset To() => _value;
}

public sealed class DomainTypeTests
{
    [Fact]
    public void TestPreludeNewM_FailureCase()
    {
        var f1 = New<OnlyTrueBoolean>(false);
        Assert.True(f1.IsFail, "OnlyTrueBoolean(false) should fail");

        var f2 = New<OnlyDigitChar>('a');
        Assert.True(f2.IsFail, "OnlyDigitChar('a') should fail");

        var f3 = New<OnlyDigitString>("12a");
        Assert.True(f3.IsFail, "OnlyDigitString(\"12a\") should fail");

        var f4 = New<OddOnlyByte>(2);
        Assert.True(f4.IsFail, "OddOnlyByte(2) should fail");

        var f5 = New<OddOnlyInt16>(2);
        Assert.True(f5.IsFail, "OddOnlyInt16(2) should fail");

        var f6 = New<OddOnlyUInt16>(2);
        Assert.True(f6.IsFail, "OddOnlyUInt16(2) should fail");

        var f7 = New<OddOnlyInt32>(2);
        Assert.True(f7.IsFail, "OddOnlyInt32(2) should fail");

        var f8 = New<OddOnlyUInt32>(2u);
        Assert.True(f8.IsFail, "OddOnlyUInt32(2) should fail");

        var f9 = New<OddOnlyInt64>(2L);
        Assert.True(f9.IsFail, "OddOnlyInt64(2) should fail");

        var f10 = New<OddOnlyUInt64>(2UL);
        Assert.True(f10.IsFail, "OddOnlyUInt64(2) should fail");

        var f11 = New<PositiveOnlySingle>(-5f);
        Assert.True(f11.IsFail, "PositiveOnlySingle(-5) should fail");

        var f12 = New<PositiveOnlyDouble>(-5d);
        Assert.True(f12.IsFail, "PositiveOnlyDouble(-5) should fail");

        var f13 = New<PositiveOnlyDecimal>(-5m);
        Assert.True(f13.IsFail, "PositiveOnlyDecimal(-5) should fail");

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var f14 = New<PastOnlyDate>(today.AddDays(+1));
        Assert.True(f14.IsFail, "PastOnlyDate(today + 1 day) should fail");

        var noon = TimeOnly.FromTimeSpan(TimeSpan.FromHours(18));
        var f15 = New<MorningOnlyTime>(noon);
        Assert.True(f15.IsFail, "MorningOnlyTime(18:00) should fail");

        var now = DateTime.UtcNow.AddDays(1);
        var f16 = New<PastOnlyDateTime>(now);
        Assert.True(f16.IsFail, "PastOnlyDateTime(now + 1 day) should fail");

        var nowOffset = DateTimeOffset.UtcNow.AddDays(-1);
        var f17 = New<FutureOnlyDateTimeOffset>(nowOffset);
        Assert.True(f17.IsFail, "FutureOnlyDateTimeOffset(now - 1 day) should fail");
    }

    [Fact]
    public void TestPreludeNewM_SuccessCase()
    {
        var f1 = New<OnlyTrueBoolean>(true);
        Assert.True(f1.IsSucc, "OnlyTrueBoolean(true) should succeed");

        var f2 = New<OnlyDigitChar>('5');
        Assert.True(f2.IsSucc, "OnlyDigitChar('5') should succeed");

        var f3 = New<OnlyDigitString>("123");
        Assert.True(f3.IsSucc, "OnlyDigitString(\"123\") should succeed");

        var f4 = New<OddOnlyByte>(3);
        Assert.True(f4.IsSucc, "OddOnlyByte(3) should succeed");

        var f5 = New<OddOnlyInt16>(3);
        Assert.True(f5.IsSucc, "OddOnlyInt16(3) should succeed");

        var f6 = New<OddOnlyUInt16>(3);
        Assert.True(f6.IsSucc, "OddOnlyUInt16(3) should succeed");

        var f7 = New<OddOnlyInt32>(3);
        Assert.True(f7.IsSucc, "OddOnlyInt32(3) should succeed");

        var f8 = New<OddOnlyUInt32>(3u);
        Assert.True(f8.IsSucc, "OddOnlyUInt32(3) should succeed");

        var f9 = New<OddOnlyInt64>(3L);
        Assert.True(f9.IsSucc, "OddOnlyInt64(3) should succeed");

        var f10 = New<OddOnlyUInt64>(3UL);
        Assert.True(f10.IsSucc, "OddOnlyUInt64(3) should succeed");

        var f11 = New<PositiveOnlySingle>(5f);
        Assert.True(f11.IsSucc, "PositiveOnlySingle(5) should succeed");

        var f12 = New<PositiveOnlyDouble>(5d);
        Assert.True(f12.IsSucc, "PositiveOnlyDouble(5) should succeed");

        var f13 = New<PositiveOnlyDecimal>(5m);
        Assert.True(f13.IsSucc, "PositiveOnlyDecimal(5) should succeed");

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var f14 = New<PastOnlyDate>(today.AddDays(-1));
        Assert.True(f14.IsSucc, "PastOnlyDate(today - 1 day) should succeed");

        var morning = TimeOnly.FromTimeSpan(TimeSpan.FromHours(10));
        var f15 = New<MorningOnlyTime>(morning);
        Assert.True(f15.IsSucc, "MorningOnlyTime(10:00) should succeed");

        var now = DateTime.UtcNow.AddDays(-1);
        var f16 = New<PastOnlyDateTime>(now);
        Assert.True(f16.IsSucc, "PastOnlyDateTime(now - 1 day) should succeed");

        var nowOffset = DateTimeOffset.UtcNow.AddDays(1);
        var f17 = New<FutureOnlyDateTimeOffset>(nowOffset);
        Assert.True(f17.IsSucc, "FutureOnlyDateTimeOffset(now + 1 day) should succeed");

    }

    [Fact]
    public void TestPreludeNew_FailureCase()
    {
        Func<object?> f1 = () => Unsafe<OnlyTrueBoolean>(false);
        Assert.Throws<WrappedErrorExpectedException>(f1);
        
        Func<object?> f2 = () => Unsafe<OnlyDigitChar>('a');
        Assert.Throws<WrappedErrorExpectedException>(f2);
        
        Func<object?> f3 = () => Unsafe<OnlyDigitString>("abc");
        Assert.Throws<WrappedErrorExpectedException>(f3);
        
        Func<object?> f4 = () => Unsafe<OddOnlyByte>(2);
        Assert.Throws<WrappedErrorExpectedException>(f4);
        
        Func<object?> f5 = () => Unsafe<OddOnlyInt16>(2);
        Assert.Throws<WrappedErrorExpectedException>(f5);
        
        Func<object?> f6 = () => Unsafe<OddOnlyUInt16>(2);
        Assert.Throws<WrappedErrorExpectedException>(f6);
        
        Func<object?> f7 = () => Unsafe<OddOnlyInt32>(2);
        Assert.Throws<WrappedErrorExpectedException>(f7);
        
        Func<object?> f8 = () => Unsafe<OddOnlyUInt32>(2u);
        Assert.Throws<WrappedErrorExpectedException>(f8);
        
        Func<object?> f9 = () => Unsafe<OddOnlyInt64>(2L);
        Assert.Throws<WrappedErrorExpectedException>(f9);
        
        Func<object?> f10 = () => Unsafe<OddOnlyUInt64>(2UL);
        Assert.Throws<WrappedErrorExpectedException>(f10);
        
        Func<object?> f11 = () => Unsafe<PositiveOnlySingle>(-5f);
        Assert.Throws<WrappedErrorExpectedException>(f11);
        
        Func<object?> f12 = () => Unsafe<PositiveOnlyDouble>(-5d);
        Assert.Throws<WrappedErrorExpectedException>(f12);
        
        Func<object?> f13 = () => Unsafe<PositiveOnlyDecimal>(-5m);
        Assert.Throws<WrappedErrorExpectedException>(f13);
        
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        Func<object?> f14 = () => Unsafe<PastOnlyDate>(today.AddDays(+1));
        Assert.Throws<WrappedErrorExpectedException>(f14);
        
        var morning = TimeOnly.FromTimeSpan(TimeSpan.FromHours(18));
        Func<object?> f15 = () => Unsafe<MorningOnlyTime>(morning);
        Assert.Throws<WrappedErrorExpectedException>(f15);
        
        var now = DateTime.UtcNow.AddDays(+1);
        Func<object?> f16 = () => Unsafe<PastOnlyDateTime>(now);
        Assert.Throws<WrappedErrorExpectedException>(f16);
        
        var nowOffset = DateTimeOffset.UtcNow.AddDays(-1);
        Func<object?> f17 = () => Unsafe<FutureOnlyDateTimeOffset>(nowOffset);
        Assert.Throws<WrappedErrorExpectedException>(f17);
    }

    [Fact]
    public void TestPreludeNew_SuccessCase()
    {
        _ = Unsafe<OnlyTrueBoolean>(true);
        _ = Unsafe<OnlyDigitChar>('5');
        _ = Unsafe<OnlyDigitString>("123");
        _ = Unsafe<OddOnlyByte>(3);
        _ = Unsafe<OddOnlyInt16>(3);
        _ = Unsafe<OddOnlyUInt16>(3);
        _ = Unsafe<OddOnlyInt32>(3);
        _ = Unsafe<OddOnlyUInt32>(3u);
        _ = Unsafe<OddOnlyInt64>(3L);
        _ = Unsafe<OddOnlyUInt64>(3UL);
        _ = Unsafe<PositiveOnlySingle>(5f);
        _ = Unsafe<PositiveOnlyDouble>(5d);
        _ = Unsafe<PositiveOnlyDecimal>(5m);
        
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        _ = Unsafe<PastOnlyDate>(today.AddDays(-1));
        
        var morning = TimeOnly.FromTimeSpan(TimeSpan.FromHours(10));
        _ = Unsafe<MorningOnlyTime>(morning);
        
        var now = DateTime.UtcNow.AddDays(-1);
        _ = Unsafe<PastOnlyDateTime>(now);
        
        var nowOffset = DateTimeOffset.UtcNow.AddDays(1);
        _ = Unsafe<FutureOnlyDateTimeOffset>(nowOffset);
    }
}
