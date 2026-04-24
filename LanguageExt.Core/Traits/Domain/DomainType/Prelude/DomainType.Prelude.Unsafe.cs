using System;
using LanguageExt.Traits.Domain;

namespace LanguageExt;

public static partial class Prelude
{
    public static T New<T>(bool repr)
        where T : DomainType<T, bool> =>
        T.FromUnsafe(repr);

    public static T New<T>(char repr)
        where T : DomainType<T, char> =>
        T.FromUnsafe(repr);

    public static T New<T>(string repr)
        where T : DomainType<T, string> =>
        T.FromUnsafe(repr);

    public static T New<T>(byte repr)
        where T : DomainType<T, byte> =>
        T.FromUnsafe(repr);

    public static T New<T>(short repr)
        where T : DomainType<T, short> =>
        T.FromUnsafe(repr);

    public static T New<T>(ushort repr)
        where T : DomainType<T, ushort> =>
        T.FromUnsafe(repr);

    public static T New<T>(int repr)
        where T : DomainType<T, int> =>
        T.FromUnsafe(repr);

    public static T New<T>(uint repr)
        where T : DomainType<T, uint> =>
        T.FromUnsafe(repr);

    public static T New<T>(long repr)
        where T : DomainType<T, long> =>
        T.FromUnsafe(repr);

    public static T New<T>(ulong repr)
        where T : DomainType<T, ulong> =>
        T.FromUnsafe(repr);

    public static T New<T>(float repr)
        where T : DomainType<T, float> =>
        T.FromUnsafe(repr);

    public static T New<T>(double repr)
        where T : DomainType<T, double> =>
        T.FromUnsafe(repr);

    public static T New<T>(decimal repr)
        where T : DomainType<T, decimal> =>
        T.FromUnsafe(repr);

    public static T New<T>(DateOnly repr)
        where T : DomainType<T, DateOnly> =>
        T.FromUnsafe(repr);

    public static T New<T>(TimeOnly repr)
        where T : DomainType<T, TimeOnly> =>
        T.FromUnsafe(repr);

    public static T New<T>(DateTime repr)
        where T : DomainType<T, DateTime> =>
        T.FromUnsafe(repr);

    public static T New<T>(DateTimeOffset repr)
        where T : DomainType<T, DateTimeOffset> =>
        T.FromUnsafe(repr);

}
