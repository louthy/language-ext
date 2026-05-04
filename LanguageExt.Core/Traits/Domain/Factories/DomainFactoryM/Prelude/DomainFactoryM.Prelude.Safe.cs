using System;
using LanguageExt;
using LanguageExt.Traits;
using LanguageExt.Traits.Domain;

namespace LanguageExt;

public static partial class Prelude
{
    public static FinT<M, T> New<M, T>(bool repr)
        where M : Monad<M>
        where T : DomainFactoryM<T, M, bool>, DomainType<T, bool> =>
        T.FromM(repr);

    public static FinT<M, T> New<M, T>(char repr)
        where M : Monad<M>
        where T : DomainFactoryM<T, M, char>, DomainType<T, char> =>
        T.FromM(repr);

    public static FinT<M, T> New<M, T>(string repr)
        where M : Monad<M>
        where T : DomainFactoryM<T, M, string>, DomainType<T, string> =>
        T.FromM(repr);

    public static FinT<M, T> New<M, T>(byte repr)
        where M : Monad<M>
        where T : DomainFactoryM<T, M, byte>, DomainType<T, byte> =>
        T.FromM(repr);

    public static FinT<M, T> New<M, T>(short repr)
        where M : Monad<M>
        where T : DomainFactoryM<T, M, short>, DomainType<T, short> =>
        T.FromM(repr);

    public static FinT<M, T> New<M, T>(ushort repr)
        where M : Monad<M>
        where T : DomainFactoryM<T, M, ushort>, DomainType<T, ushort> =>
        T.FromM(repr);

    public static FinT<M, T> New<M, T>(int repr)
        where M : Monad<M>
        where T : DomainFactoryM<T, M, int>, DomainType<T, int> =>
        T.FromM(repr);

    public static FinT<M, T> New<M, T>(uint repr)
        where M : Monad<M>
        where T : DomainFactoryM<T, M, uint>, DomainType<T, uint> =>
        T.FromM(repr);

    public static FinT<M, T> New<M, T>(long repr)
        where M : Monad<M>
        where T : DomainFactoryM<T, M, long>, DomainType<T, long> =>
        T.FromM(repr);

    public static FinT<M, T> New<M, T>(ulong repr)
        where M : Monad<M>
        where T : DomainFactoryM<T, M, ulong>, DomainType<T, ulong> =>
        T.FromM(repr);

    public static FinT<M, T> New<M, T>(float repr)
        where M : Monad<M>
        where T : DomainFactoryM<T, M, float>, DomainType<T, float> =>
        T.FromM(repr);

    public static FinT<M, T> New<M, T>(double repr)
        where M : Monad<M>
        where T : DomainFactoryM<T, M, double>, DomainType<T, double> =>
        T.FromM(repr);

    public static FinT<M, T> New<M, T>(decimal repr)
        where M : Monad<M>
        where T : DomainFactoryM<T, M, decimal>, DomainType<T, decimal> =>
        T.FromM(repr);

    public static FinT<M, T> New<M, T>(DateOnly repr)
        where M : Monad<M>
        where T : DomainFactoryM<T, M, DateOnly>, DomainType<T, DateOnly> =>
        T.FromM(repr);

    public static FinT<M, T> New<M, T>(TimeOnly repr)
        where M : Monad<M>
        where T : DomainFactoryM<T, M, TimeOnly>, DomainType<T, TimeOnly> =>
        T.FromM(repr);

    public static FinT<M, T> New<M, T>(DateTime repr)
        where M : Monad<M>
        where T : DomainFactoryM<T, M, DateTime>, DomainType<T, DateTime> =>
        T.FromM(repr);

    public static FinT<M, T> New<M, T>(DateTimeOffset repr)
        where M : Monad<M>
        where T : DomainFactoryM<T, M, DateTimeOffset>, DomainType<T, DateTimeOffset> =>
        T.FromM(repr);

    public static FinT<M, T> New<M, T>(TimeSpan repr)
        where M : Monad<M>
        where T : DomainFactoryM<T, M, TimeSpan>, DomainType<T, TimeSpan> =>
        T.FromM(repr);

    public static FinT<M, T> New<M, T>(Guid repr)
        where M : Monad<M>
        where T : DomainFactoryM<T, M, Guid>, DomainType<T, Guid> =>
        T.FromM(repr);
}
