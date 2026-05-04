using System;
using LanguageExt.Traits.Domain;

namespace LanguageExt;

public static partial class Prelude
{
    public static T Unsafe<T>(bool repr)
        where T : DomainFactory<T, bool>, DomainType<T, bool> =>
        T.FromUnsafe(repr);

    public static T Unsafe<T>(char repr)
        where T : DomainFactory<T, char>, DomainType<T, char> =>
        T.FromUnsafe(repr);

    public static T Unsafe<T>(string repr)
        where T : DomainFactory<T, string>, DomainType<T, string> =>
        T.FromUnsafe(repr);

    public static T Unsafe<T>(byte repr)
        where T : DomainFactory<T, byte>, DomainType<T, byte> =>
        T.FromUnsafe(repr);

    public static T Unsafe<T>(short repr)
        where T : DomainFactory<T, short>, DomainType<T, short> =>
        T.FromUnsafe(repr);

    public static T Unsafe<T>(ushort repr)
        where T : DomainFactory<T, ushort>, DomainType<T, ushort> =>
        T.FromUnsafe(repr);

    public static T Unsafe<T>(int repr)
        where T : DomainFactory<T, int>, DomainType<T, int> =>
        T.FromUnsafe(repr);

    public static T Unsafe<T>(uint repr)
        where T : DomainFactory<T, uint>, DomainType<T, uint> =>
        T.FromUnsafe(repr);

    public static T Unsafe<T>(long repr)
        where T : DomainFactory<T, long>, DomainType<T, long> =>
        T.FromUnsafe(repr);

    public static T Unsafe<T>(ulong repr)
        where T : DomainFactory<T, ulong>, DomainType<T, ulong> =>
        T.FromUnsafe(repr);

    public static T Unsafe<T>(float repr)
        where T : DomainFactory<T, float>, DomainType<T, float> =>
        T.FromUnsafe(repr);

    public static T Unsafe<T>(double repr)
        where T : DomainFactory<T, double>, DomainType<T, double> =>
        T.FromUnsafe(repr);

    public static T Unsafe<T>(decimal repr)
        where T : DomainFactory<T, decimal>, DomainType<T, decimal> =>
        T.FromUnsafe(repr);

    public static T Unsafe<T>(DateOnly repr)
        where T : DomainFactory<T, DateOnly>, DomainType<T, DateOnly> =>
        T.FromUnsafe(repr);

    public static T Unsafe<T>(TimeOnly repr)
        where T : DomainFactory<T, TimeOnly>, DomainType<T, TimeOnly> =>
        T.FromUnsafe(repr);

    public static T Unsafe<T>(DateTime repr)
        where T : DomainFactory<T, DateTime>, DomainType<T, DateTime> =>
        T.FromUnsafe(repr);

    public static T Unsafe<T>(DateTimeOffset repr)
        where T : DomainFactory<T, DateTimeOffset>, DomainType<T, DateTimeOffset> =>
        T.FromUnsafe(repr);

    public static T Unsafe<T>(TimeSpan repr)
        where T : DomainFactory<T, TimeSpan>, DomainType<T, TimeSpan> =>
        T.FromUnsafe(repr);
    
    public static T Unsafe<T>(Guid repr)
        where T : DomainFactory<T, Guid>, DomainType<T, Guid> =>
        T.FromUnsafe(repr);

}
