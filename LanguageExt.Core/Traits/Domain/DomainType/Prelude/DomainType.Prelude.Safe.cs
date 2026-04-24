using System;
using LanguageExt;
using LanguageExt.Traits.Domain;

namespace LanguageExt;

public static partial class Prelude
{
    public static Fin<T> NewM<T>(bool repr)
        where T : DomainType<T, bool> =>
        T.From(repr);

    public static Fin<T> NewM<T>(char repr)
        where T : DomainType<T, char> =>
        T.From(repr);

    public static Fin<T> NewM<T>(string repr)
        where T : DomainType<T, string> =>
        T.From(repr);

    public static Fin<T> NewM<T>(byte repr)
        where T : DomainType<T, byte> =>
        T.From(repr);

    public static Fin<T> NewM<T>(short repr)
        where T : DomainType<T, short> =>
        T.From(repr);

    public static Fin<T> NewM<T>(ushort repr)
        where T : DomainType<T, ushort> =>
        T.From(repr);

    public static Fin<T> NewM<T>(int repr)
        where T : DomainType<T, int> =>
        T.From(repr);

    public static Fin<T> NewM<T>(uint repr)
        where T : DomainType<T, uint> =>
        T.From(repr);

    public static Fin<T> NewM<T>(long repr)
        where T : DomainType<T, long> =>
        T.From(repr);

    public static Fin<T> NewM<T>(ulong repr)
        where T : DomainType<T, ulong> =>
        T.From(repr);

    public static Fin<T> NewM<T>(float repr)
        where T : DomainType<T, float> =>
        T.From(repr);

    public static Fin<T> NewM<T>(double repr)
        where T : DomainType<T, double> =>
        T.From(repr);

    public static Fin<T> NewM<T>(decimal repr)
        where T : DomainType<T, decimal> =>
        T.From(repr);

    public static Fin<T> NewM<T>(DateOnly repr)
        where T : DomainType<T, DateOnly> =>
        T.From(repr);

    public static Fin<T> NewM<T>(TimeOnly repr)
        where T : DomainType<T, TimeOnly> =>
        T.From(repr);

    public static Fin<T> NewM<T>(DateTime repr)
        where T : DomainType<T, DateTime> =>
        T.From(repr);

    public static Fin<T> NewM<T>(DateTimeOffset repr)
        where T : DomainType<T, DateTimeOffset> =>
        T.From(repr);
}
