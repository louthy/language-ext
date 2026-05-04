using System;
using LanguageExt;
using LanguageExt.Traits.Domain;

namespace LanguageExt;

public static partial class Prelude
{
    public static Fin<T> New<T>(bool repr)
        where T : DomainFactory<T, bool>, DomainType<T, bool> =>
        T.From(repr);

    public static Fin<T> New<T>(char repr)
        where T : DomainFactory<T, char>, DomainType<T, char> =>
        T.From(repr);

    public static Fin<T> New<T>(string repr)
        where T : DomainFactory<T, string>, DomainType<T, string> =>
        T.From(repr);

    public static Fin<T> New<T>(byte repr)
        where T : DomainFactory<T, byte>, DomainType<T, byte> =>
        T.From(repr);

    public static Fin<T> New<T>(short repr)
        where T : DomainFactory<T, short>, DomainType<T, short> =>
        T.From(repr);

    public static Fin<T> New<T>(ushort repr)
        where T : DomainFactory<T, ushort>, DomainType<T, ushort> =>
        T.From(repr);

    public static Fin<T> New<T>(int repr)
        where T : DomainFactory<T, int>, DomainType<T, int> =>
        T.From(repr);

    public static Fin<T> New<T>(uint repr)
        where T : DomainFactory<T, uint>, DomainType<T, uint> =>
        T.From(repr);

    public static Fin<T> New<T>(long repr)
        where T : DomainFactory<T, long>, DomainType<T, long> =>
        T.From(repr);

    public static Fin<T> New<T>(ulong repr)
        where T : DomainFactory<T, ulong>, DomainType<T, ulong> =>
        T.From(repr);

    public static Fin<T> New<T>(float repr)
        where T : DomainFactory<T, float>, DomainType<T, float> =>
        T.From(repr);

    public static Fin<T> New<T>(double repr)
        where T : DomainFactory<T, double>, DomainType<T, double> =>
        T.From(repr);

    public static Fin<T> New<T>(decimal repr)
        where T : DomainFactory<T, decimal>, DomainType<T, decimal> =>
        T.From(repr);

    public static Fin<T> New<T>(DateOnly repr)
        where T : DomainFactory<T, DateOnly>, DomainType<T, DateOnly> =>
        T.From(repr);

    public static Fin<T> New<T>(TimeOnly repr)
        where T : DomainFactory<T, TimeOnly>, DomainType<T, TimeOnly> =>
        T.From(repr);

    public static Fin<T> New<T>(DateTime repr)
        where T : DomainFactory<T, DateTime>, DomainType<T, DateTime> =>
        T.From(repr);

    public static Fin<T> New<T>(DateTimeOffset repr)
        where T : DomainFactory<T, DateTimeOffset>, DomainType<T, DateTimeOffset> =>
        T.From(repr);

    public static Fin<T> New<T>(TimeSpan repr)
        where T : DomainFactory<T, TimeSpan>, DomainType<T, TimeSpan> =>
        T.From(repr);

    public static Fin<T> New<T>(Guid repr)
        where T : DomainFactory<T, Guid>, DomainType<T, Guid> =>
        T.From(repr);
}
