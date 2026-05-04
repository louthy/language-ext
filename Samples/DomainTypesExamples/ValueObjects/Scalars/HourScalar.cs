using System;
using System.Collections.Generic;
using System.Text;
using DomainTypesExamples.ValueObjects.Spaces;

namespace DomainTypesExamples.ValueObjects.Scalars;

public abstract record HourScalar : DomainType<HourScalar, int>
{
    public abstract int To();

    public abstract int TotalMinutes { get; }

    public sealed record Hour(HourValue Value) : 
        HourScalar,
        DerivedTypeFactory<Hour, HourValue, int>
    {
        public static Hour New(HourValue @base) =>
            new(@base);

        public HourValue ToBase() =>
            Value;

        public override int To() =>
            Value.To();

        public override int TotalMinutes =>
            Value.To() * 60;

        public override string ToString() =>
            Value.ToString();
    }

    public sealed record Minute(MinuteValue Value) : 
        HourScalar,
        DerivedTypeFactory<Minute, MinuteValue, int>
    {
        public static Minute New(MinuteValue @base) =>
            new(@base);

        public MinuteValue ToBase() =>
            Value;

        public override int To() =>
            Value.To();

        public override int TotalMinutes =>
            Value.To();

        public override string ToString() =>
            Value.ToString();

    }

    public static Fin<HourScalar> FromHours(int value) =>
        New<Hour>(value).Map(HourScalar (v) => v);

    public static Fin<HourScalar> FromMinutes(int value) =>
        New<Minute>(value).Map(HourScalar (v) => v);
}
