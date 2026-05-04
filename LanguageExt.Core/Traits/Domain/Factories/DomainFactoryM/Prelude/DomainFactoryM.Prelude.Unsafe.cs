using System;
using LanguageExt.Traits;
using LanguageExt.Traits.Domain;

namespace LanguageExt;

public static partial class Prelude
{
    public static K<M, T> Unsafe<M, T>(bool repr)
        where M : Monad<M>
        where T : DomainFactoryM<T, M, bool>, DomainType<T, bool> =>
        T.FromUnsafeM(repr);

    public static K<M, T> Unsafe<M, T>(char repr)
        where M : Monad<M>
        where T : DomainFactoryM<T, M, char>, DomainType<T, char> =>
        T.FromUnsafeM(repr);

    public static K<M, T> Unsafe<M, T>(string repr)
        where M : Monad<M>
        where T : DomainFactoryM<T, M, string>, DomainType<T, string> =>
        T.FromUnsafeM(repr);

    public static K<M, T> Unsafe<M, T>(byte repr)
        where M : Monad<M>
        where T : DomainFactoryM<T, M, byte>, DomainType<T, byte> =>
        T.FromUnsafeM(repr);

    public static K<M, T> Unsafe<M, T>(short repr)
        where M : Monad<M>
        where T : DomainFactoryM<T, M, short>, DomainType<T, short> =>
        T.FromUnsafeM(repr);

    public static K<M, T> Unsafe<M, T>(ushort repr)
        where M : Monad<M>
        where T : DomainFactoryM<T, M, ushort>, DomainType<T, ushort> =>
        T.FromUnsafeM(repr);

    public static K<M, T> Unsafe<M, T>(int repr)
        where M : Monad<M>
        where T : DomainFactoryM<T, M, int>, DomainType<T, int> =>
        T.FromUnsafeM(repr);

    public static K<M, T> Unsafe<M, T>(uint repr)
        where M : Monad<M>
        where T : DomainFactoryM<T, M, uint>, DomainType<T, uint> =>
        T.FromUnsafeM(repr);

    public static K<M, T> Unsafe<M, T>(long repr)
        where M : Monad<M>
        where T : DomainFactoryM<T, M, long>, DomainType<T, long> =>
        T.FromUnsafeM(repr);

    public static K<M, T> Unsafe<M, T>(ulong repr)
        where M : Monad<M>
        where T : DomainFactoryM<T, M, ulong>, DomainType<T, ulong> =>
        T.FromUnsafeM(repr);

    public static K<M, T> Unsafe<M, T>(float repr)
        where M : Monad<M>
        where T : DomainFactoryM<T, M, float>, DomainType<T, float> =>
        T.FromUnsafeM(repr);

    public static K<M, T> Unsafe<M, T>(double repr)
        where M : Monad<M>
        where T : DomainFactoryM<T, M, double>, DomainType<T, double> =>
        T.FromUnsafeM(repr);

    public static K<M, T> Unsafe<M, T>(decimal repr)
        where M : Monad<M>
        where T : DomainFactoryM<T, M, decimal>, DomainType<T, decimal> =>
        T.FromUnsafeM(repr);

    public static K<M, T> Unsafe<M, T>(DateOnly repr)
        where M : Monad<M>
        where T : DomainFactoryM<T, M, DateOnly>, DomainType<T, DateOnly> =>
        T.FromUnsafeM(repr);

    public static K<M, T> Unsafe<M, T>(TimeOnly repr)
        where M : Monad<M>
        where T : DomainFactoryM<T, M, TimeOnly>, DomainType<T, TimeOnly> =>
        T.FromUnsafeM(repr);

    public static K<M, T> Unsafe<M, T>(DateTime repr)
        where M : Monad<M>
        where T : DomainFactoryM<T, M, DateTime>, DomainType<T, DateTime> =>
        T.FromUnsafeM(repr);

    public static K<M, T> Unsafe<M, T>(DateTimeOffset repr)
        where M : Monad<M>
        where T : DomainFactoryM<T, M, DateTimeOffset>, DomainType<T, DateTimeOffset> =>
        T.FromUnsafeM(repr);

    public static K<M, T> Unsafe<M, T>(TimeSpan repr)
        where M : Monad<M>
        where T : DomainFactoryM<T, M, TimeSpan>, DomainType<T, TimeSpan> =>
        T.FromUnsafeM(repr);

    public static K<M, T> Unsafe<M, T>(Guid repr)
        where M : Monad<M>
        where T : DomainFactoryM<T, M, Guid>, DomainType<T, Guid> =>
        T.FromUnsafeM(repr);

}
